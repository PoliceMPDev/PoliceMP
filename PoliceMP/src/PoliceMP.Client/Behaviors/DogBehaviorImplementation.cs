using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Drawing.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Client.Scripts.Dsu;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Behaviors.Dog;
using PoliceMP.Shared.Behaviors.FailToStop;
using PoliceMP.Shared.Behaviors.Test;
using PoliceMP.Shared.Behaviors.Writhe;
using PoliceMP.Shared.NetworkMessages.Dog.Commands;
using PoliceMP.Shared.NetworkMessages.Dog.Notifications;
using PoliceMP.Shared.Options;
using PoliceMP.Client.Scripts.CivVehicleContents;

namespace PoliceMP.Client.Behaviors
{
    public class DogBehaviorImplementation : PedBehavior<DogBehavior>
    {
        private readonly ILogger<DogBehaviorImplementation> _log;
        private readonly IPedInfoService _peds;
        private readonly IVehicleInfoService _vehicles;
        private readonly IAnimationService _animationService;
        private readonly IBehaviorService _behaviors;
        private readonly IPedInfoService _pedInfoService;
        private readonly IClientCommunicationsManager _comms;
        private readonly IPersonalityService _personalityService;
        private readonly IPlayerService _playerService;
        private readonly ISpeechService _speech;
        private readonly IFiveEventManager _fiveEvents;
        private double _lastHandledDogVocal = 0;
        private const float NavDistanceToTeleport = 50f;
        private const int DelayBetweenTeleportsSecs = 10;
        private const int SitAfterOwnerStillForMs = 10000;
        private const int BarkSpeechMinTime = 10000;
        private const float TakeDownSpeed = 1.5f;
        private int _lastOwnerMove = 0;
        private const int NavMeshSafeZoneFlags = 2;
        private bool hasCheckedDriverDoor = false;
        private Blip _blip;

        public DogBehaviorImplementation(
            ITickManager ticks, 
            ILogger<DogBehaviorImplementation> log, 
            IPedInfoService peds,
            IVehicleInfoService vehicles,
            IAnimationService animationService,
            IBehaviorService behaviors,
            IPedInfoService pedInfoService,
            IClientCommunicationsManager comms,
            IPersonalityService personalityService,
            IPlayerService playerService,
            ISpeechService speech) : base(ticks)
        {
            _log = log;
            _peds = peds;
            _vehicles = vehicles;
            _animationService = animationService;
            _behaviors = behaviors;
            _pedInfoService = pedInfoService;
            _comms = comms;
            _personalityService = personalityService;
            _playerService = playerService;
            _speech = speech;
        }

        public override void Initialize()
        {
            ResetToDefaultState();
            Blackboard.ResetData(); // Ensure the blackboard is clear
            Blackboard.Set(bb => bb.OwnerNetworkId, Game.PlayerPed.NetworkId);
            Blackboard.Set(bb => bb.TargetNetworkId, Game.PlayerPed.NetworkId);
            Blackboard.Set(bb => bb.DistanceToSniff, 1.5f);
            Blackboard.Set(bb => bb.DistanceToTakeDown, 3f);
            Blackboard.Set(bb => bb.TimeToRagdollAfterTakedownMs, 10000);
            Blackboard.Set(bb => bb.DogName, "Dog");
            ThePed.SetName("Dog");
        }

        protected override void Start()
        {
            Blackboard.AddChangeHandler(bb => bb.State, OnStateChange);
            Blackboard.AddChangeHandler(bb => bb.DogName, OnDogNameChange); // Not working for some reason
            
            // Workaround for above not working
            _comms.AddNotificationHandler<DogNameChangedEvent>(@event =>
            {
                if (@event.DogNetworkId == ThePed.NetworkId)
                {
                    OnDogNameChange(@event.OldName, @event.NewName, true);
                }

                return Task.FromResult(0);
            });
            
            _comms.AddNotificationHandler<DogShouldTeleportEvent>(DogShouldTeleportEventHandler);

            ThePed.RelationshipGroup = new RelationshipGroup(API.GetHashKey("policedog"));
            ThePed.SetIsNameKnown(true);
        }

        private Task OnDogNameChange(string oldvalue, string newvalue, bool replicated)
        {
            _log.Debug($"Setting dog name to {newvalue}.");
            ThePed.SetName(newvalue);
            
            if (_blip != null)
            {
                _blip.Name = newvalue;
            }

            return Task.FromResult(0);
        }

        protected override void Stop()
        {
            if (_blip != null)
            {
                _blip.Delete();
            }
        }

        private async Task DogShouldTeleportEventHandler(DogShouldTeleportEvent @event)
        {
            if (@event.DogNetworkId == ThePed.NetworkId)
            {
                await TeleportTo(@event.Position.ToCitizenVector3());
            }
        }

        private async Task<bool> TeleportTo(Vector3 position)
        {
            await ThePed.NetworkFadeOut();
            ThePed.IsInvincible = true;
            //var pos = World.GetSafeCoordForPed(position, false, NavMeshSafeZoneFlags);
            var pos = position;

            if (pos == Vector3.Zero)
            {
                return false;
            }
            
            ThePed.Position = pos + Vector3.Down * 0.5f;
            await ThePed.NetworkFadeIn();

            while (ThePed.IsInAir)
            {
                await BaseScript.Delay(0);
            }
                
            ThePed.IsInvincible = false;
            return true;
        }

        private void HandleLostOwner()
        {
            _log.Debug("Lost owner. Sending notification!");
            _comms.PublishToAll(new DogLostOwnerEvent
            {
                DogNetworkId = ThePed.NetworkId,
                OwnerNetworkId = Blackboard.Get(bb => bb.OwnerNetworkId)
            });
        }

        private Task OnStateChange(DogBehaviorState oldvalue, DogBehaviorState newvalue, bool replicated)
        {
            ThePed.Task.ClearAll();
            ThePed.AlwaysKeepTask = false;
            ThePed.BlockPermanentEvents = false;
            _lastOwnerMove = Game.GameTime;
            
            return Task.FromResult(0);
        }

        protected override async Task Think()
        {
            var state = Blackboard.Get(bb => bb.State);
            var ownerNetId = Blackboard.Get(bb => bb.OwnerNetworkId);
            var ownerPed = Entity.FromNetworkId(ownerNetId) as Ped;
            API.SetPedCanBeTargetted(ThePed.Handle, false);
            API.SetCanAttackFriendly(ThePed.Handle, false, false);
            API.SetPedFleeAttributes(ThePed.Handle, 0, false);
            API.SetPedAsCop(ThePed.Handle, true);

            if (_blip == null && ownerPed == Game.PlayerPed)
            {
                _blip = ThePed.AttachBlip();
                _blip.Sprite = (BlipSprite) 442; // radar_beast
                _blip.Color = BlipColor.MichaelBlue;
                _blip.Name = Blackboard.Get(bb => bb.DogName);
            }

            if (ownerPed == null)
            {
                HandleLostOwner();
                await BaseScript.Delay(5000); // Avoid server spam
                return;
            }
            
            // Ensure we're companion to our owner
            if (ThePed.RelationshipGroup.GetRelationshipBetweenGroups(ownerPed.RelationshipGroup) !=
                Relationship.Companion)
            {
                ThePed.RelationshipGroup.SetRelationshipBetweenGroups(ownerPed.RelationshipGroup, Relationship.Companion);
            }

            API.SetEntityAsMissionEntity(ThePed.Handle, false, false);

            if (ThePed.IsDead)
            {
                _behaviors.RemovePedBehaviors(ThePed);
                return;
            }
            
            // If either the ped or the owner are moving then we reset the timer
            // this lets the dog move on when the player moves
            if (ownerPed.Velocity.Length() + ThePed.Velocity.Length() > 0.5f)
            {
                _lastOwnerMove = Game.GameTime;
            }

            if (!API.IsEntityAMissionEntity(ThePed.Handle))
            {
                API.SetEntityAsMissionEntity(ThePed.Handle, true, true);
            }

            /*
            if (API.IsAnimalVocalizationPlaying(ThePed.Handle) && 
                ServerInfo.GetServerTime().TotalMilliseconds > Blackboard.Get(bb => bb.LastBarkTime) + BarkSpeechMinTime)
            {
                _speech.Say(ThePed, "*WOOF*", BarkSpeechMinTime);
                Blackboard.Set(bb => bb.LastBarkTime, ServerInfo.GetServerTime().TotalMilliseconds);
            }
            */
            
            switch (state)
            {
                case DogBehaviorState.FollowOwner:
                    DogIdleSniffing();
                    await ThinkFollow(ownerPed);
                    break;
                case DogBehaviorState.Wait:
                    DogIdleSniffing();
                    await ThinkWait();
                    break;
                case DogBehaviorState.IntimidateTarget:
                    await ThinkIntimidate();
                    break;
                case DogBehaviorState.TakeDownTarget:
                    await ThinkTakeDown();
                    break;
                case DogBehaviorState.SniffTarget:
                    DogIdleSniffing();
                    await ThinkSniffTarget();
                    break;
                case DogBehaviorState.InPlayerVehicle:
                    await ThinkInPlayerVehicle(ownerPed);
                    break;
            }
        }

        private async Task ThinkFollow(Ped ownerPed)
        {
            if (ownerPed.IsInVehicle())
            {
                Blackboard.Set(bb => bb.State, DogBehaviorState.InPlayerVehicle);
                hasCheckedDriverDoor = false;
                return;
            }

            var ownerVehicle = ownerPed.LastVehicle;

            Blackboard.Set(bb => bb.State, DogBehaviorState.FollowOwner);
            
            await CheckTeleportToOwner(ownerPed);
            
            var shouldSit = Game.GameTime > _lastOwnerMove + SitAfterOwnerStillForMs;
            if (shouldSit)
            {
                var options = Blackboard.Get(bb => bb.Options);
                await EnsureScenario(options.SitScenario);
            }
            else
            {
                if (!ThePed.IsTaskActive(TaskType.CTaskComplexControlMovement))
                {
                    ThePed.Task.ClearAll();
                    ThePed.Task.FollowToOffsetFromEntity(ownerPed, Vector3.Right, 100f, -1, 1f, true);
                    ThePed.AlwaysKeepTask = true;
                    ThePed.BlockPermanentEvents = true;
                }
            }
        }

        private async Task ThinkWait()
        {
            var options = Blackboard.Get(bb => bb.Options);
            await EnsureScenario(options.SitScenario);
        }

        private async Task ThinkIntimidate()
        {
            var options = Blackboard.Get(bb => bb.Options);
            var targetEntity = Ped.FromNetworkId(Blackboard.Get(bb => bb.TargetNetworkId));

            ThePed.Task.ClearAll();
            foreach (var anim in options.Animations.Intimidate)
            {
                await _animationService.Play(ThePed, anim.Dict, anim.Anim, flag: 0);
                await BaseScript.Delay(
                    (int) API.GetAnimDuration(anim.Dict, anim.Anim) * 1000);
            }
        }

        private async Task ThinkTakeDown()
        {
            var options = Blackboard.Get(bb => bb.Options);
            var distanceToTakeDown = Blackboard.Get(bb => bb.DistanceToTakeDown);
            var targetNetId = Blackboard.Get(bb => bb.TargetNetworkId);

            if(targetNetId == -1)
            {
                _log.Error("Target lost, -1 player, no self attack.");
                ResetToDefaultState();
                return;
            }

            var targetEntity = Entity.FromNetworkId(targetNetId);

            if (options == null)
            {
                _log.Error("Options is null. Cannot determine if dog can take down a target!");
                return;
            }
            
            if (!options.CanTakeDownTarget)
            {
                _log.Error("This dog cannot take down their target due to configuration!");
                ResetToDefaultState();
                return;
            }
            
            if (targetEntity is not Ped targetPed)
            {
                _log.Debug($"Cannot find entity with networkId {targetNetId}. Cannot intimidate!");
                ResetToDefaultState();
                return;
            }

            var distance = World.GetDistance(ThePed.Position, targetPed.Position);
            if (distance > distanceToTakeDown)
            {
                API.SetPedMoveRateOverride(ThePed.Handle, TakeDownSpeed);
                if (!ThePed.IsTaskActive(TaskType.CTaskComplexControlMovement))
                {
                    ThePed.Task.FollowToOffsetFromEntity(targetEntity, Vector3.ForwardRH * targetPed.Velocity.Length() * 0.5f, 300f, -1, 5f, true);
                }
            }
            else
            {
                _log.Debug($"Taking down target... CanRagdoll {targetPed.CanRagdoll}");
                
                var deltaPos = targetPed.Position - ThePed.Position;
                var pos = ThePed.Position + deltaPos / 2 + Vector3.Down * 0.4f;
                var direction = deltaPos;
                direction.Normalize();
                var heading = GameMath.DirectionToRotation(direction, 0f).Z;

                var raycast = World.Raycast(pos, Vector3.Down, 10.0f, (IntersectOptions)(ShapeTestFlags.IncludeMover));
                if (raycast.DitHit)
                {
                    pos.Z = raycast.HitPosition.Z;
                }

                var effectiveRot = ThePed.Rotation;
                effectiveRot.Z = heading;
                ThePed.Rotation = effectiveRot;
                ThePed.SetNoCollision(targetPed, true);
                await _animationService.Play(ThePed, options.Animations.TakedownFromBackDog.Dict,
                    options.Animations.TakedownFromBackDog.Anim, flag: 0);

                // Damage ped for medical
                if (targetPed.IsPlayer)
                {
                    var player = API.GetPlayerServerId(API.NetworkGetPlayerIndexFromPed(targetPed.Handle));

                    // Check if player is inside a vehicle
                    if (API.IsPedInAnyVehicle(targetPed.Handle, false))
                    {
                        _log.Debug("Takedown aborted: Target entered a vehicle.");
                        ResetToDefaultState();
                        return;
                    }

                    // Proceed with takedown
                    targetPed.Ragdoll();
                    _comms.PublishToServer(new DogTookDownPlayerEvent
                    {
                        PlayerServerHandle = player,
                        OwnerNetworkId = Blackboard.Get(bb => bb.OwnerNetworkId),
                        DogNetworkId = ThePed.NetworkId,
                        AnimDict = options.Animations.TakedownFromBackVictim.Dict,
                        AnimName = options.Animations.TakedownFromBackVictim.Anim,
                    });
                }

                else
                {
                    targetPed.Task.ClearAllImmediately();
                    targetPed.BlockPermanentEvents = true;
                    await _animationService.Play(targetPed, options.Animations.TakedownFromBackVictim.Dict,
                        options.Animations.TakedownFromBackVictim.Anim, flag: 0);

                    targetPed.Position = pos - direction * 0.4f;
                    targetPed.Rotation = ThePed.Rotation + new Vector3(0f, 20f, 0f);
                }

                await BaseScript.Delay((int)API.GetAnimDuration(options.Animations.TakedownFromBackDog.Dict,
                    options.Animations.TakedownFromBackDog.Anim) * 1000 - 1000);
                
                Blackboard.Set(bb => bb.State, DogBehaviorState.Wait);
                ThePed.SetNoCollision(targetPed, false);
                var timeToRagdoll = Blackboard.Get(bb => bb.TimeToRagdollAfterTakedownMs);
                
                ThePed.Task.TurnTo(targetPed);

                if (!targetPed.IsPlayer)
                {
                    targetPed.Ragdoll(timeToRagdoll);
                    targetPed.Task.HandsUp(-1);
                }
            }
        }
        
        private async Task ThinkSniffTarget()
        {
            var options = Blackboard.Get(bb => bb.Options);
            if (!options.CanSniffTarget)
            {
                _log.Error("Dog cannot sniff target due to configuration!");
                ResetToDefaultState();
                return;
            }
            
            var targetNetId = Blackboard.Get(bb => bb.TargetNetworkId);
            var targetEntity = Entity.FromNetworkId(targetNetId);
            var sniffDistance = Blackboard.Get(bb => bb.DistanceToSniff);

            if (targetEntity == null)
            {
                _log.Error("targetEntity is null!");
                ResetToDefaultState();
                return;
            }

            if (!targetEntity.Exists())
            {
                _log.Debug("Target does not exist!");
                ResetToDefaultState();
                return;
            }

            if (targetEntity is Ped pedEntity)
            {
                if (!pedEntity.IsTaskActive(TaskType.CTaskTurnToFaceEntityOrCoord))
                {
                    pedEntity.Task.ClearAll();
                    pedEntity.Task.TurnTo(ThePed);
                }

                if (World.GetDistance(ThePed.Position, pedEntity.Position) > sniffDistance)
                {
                    if (!ThePed.IsTaskActive(TaskType.CTaskComplexControlMovement))
                    {
                        ThePed.Task.ClearAll();
                        API.TaskGoToEntity(ThePed.Handle, pedEntity.Handle, -1, sniffDistance, 300f, 0f, 0);
                    }
                }
                else
                {
                    PlayVocalization(DogBehaviorVocalization.Sniff);
                    ThePed.Task.ClearAll();

                    var info = await _pedInfoService.GetByNetworkId(pedEntity.NetworkId);
                    await BaseScript.Delay(1000);

                    if (info.IsOnAnyDrugs)
                    {
                        _comms.PublishToAll(new PlayerDogSmelledDrugsOnPedEvent
                        {
                            PlayerServerHandle = Game.Player.ServerId,
                            DogNetworkId = ThePed.NetworkId,
                            PedNetworkId = pedEntity.NetworkId
                        });

                        Blackboard.Set(bb => bb.State, DogBehaviorState.Wait);
                    }
                    else
                    {
                        _comms.PublishToAll(new PlayerDogSmelledNothingOnPedEvent
                        {
                            PlayerServerHandle = Game.Player.ServerId,
                            DogNetworkId = ThePed.NetworkId,
                            PedNetworkId = pedEntity.NetworkId
                        });
                        
                        pedEntity.Task.ClearAll();
                        Blackboard.Set(bb => bb.State, DogBehaviorState.FollowOwner);
                    }
                }
            }
            else if (targetEntity is Vehicle vehicleEntity)
            {
                var closestDoorPos = vehicleEntity.Doors.GetAll()
                    .Where(d => API.GetIsDoorValid(vehicleEntity.Handle, (int)d.Index))
                    .Select(d => API.GetEntryPositionOfDoor(vehicleEntity.Handle, (int) d.Index))
                    .OrderBy(ep => World.GetDistance(ThePed.Position, ep))
                    .FirstOrDefault();

                if (closestDoorPos == default)
                {
                    _log.Warn("Could not find a position to sniff");
                    closestDoorPos = vehicleEntity.Position;
                }

                if (World.GetDistance(ThePed.Position, closestDoorPos) > sniffDistance)
                {
                    if (!ThePed.IsTaskActive(TaskType.CTaskComplexControlMovement))
                    {
                        ThePed.Task.ClearAll();
                        API.TaskGoStraightToCoord(ThePed.Handle, closestDoorPos.X, closestDoorPos.Y, closestDoorPos.Z, 300f, -1, 0, 0f);
                    }
                }
                else
                {
                    PlayVocalization(DogBehaviorVocalization.Sniff);
                    ThePed.Task.ClearAll();
                    
                    //TODO: Update this when vehicles get inventory
                    foreach (var door in vehicleEntity.Doors)
                    {
                        door.Open();
                    }

                    var info = await _vehicles.GetByNetworkId(vehicleEntity.NetworkId, vehicleEntity.GetPlateText(),
                        vehicleEntity.GetDriverPedNetworkId());
                    
                    await BaseScript.Delay(1000);

                    if (info.HasIllegalItems)
                    {
                        // TODO: Publish vehicle events
                        _comms.PublishToAll(new PlayerDogSmelledDrugsOnPedEvent
                        {
                            PlayerServerHandle = Game.Player.ServerId,
                            DogNetworkId = ThePed.NetworkId,
                            PedNetworkId = -1
                        });

                        Blackboard.Set(bb => bb.State, DogBehaviorState.Wait);
                    }
                    else
                    {
                        // TODO: Publish vehicle events
                        _comms.PublishToAll(new PlayerDogSmelledNothingOnPedEvent
                        {
                            PlayerServerHandle = Game.Player.ServerId,
                            DogNetworkId = ThePed.NetworkId,
                            PedNetworkId = -1
                        });
                        
                        ResetToDefaultState();
                    }
                    
                    foreach (var door in vehicleEntity.Doors)
                    {
                        door.Close();
                    }
                }
            }
            else
            {
                _log.Error($"Dog does not know how to sniff target of type {targetEntity.GetType()}");
                ResetToDefaultState();
            }
        }

        private async Task ThinkInPlayerVehicle(Ped ownerPed)
        {
            if (!ownerPed.IsInVehicle())
            {
                var ownerVehicle = ownerPed.LastVehicle;
                if (ownerVehicle != null && !API.GetIsVehicleEngineRunning(ownerVehicle.Handle))
                {
                    _log.Debug("Player left the vehicle, but engine is off; dog stays in the car.");
                    ThePed.AttachTo(ownerVehicle);
                    ThePed.IsVisible = true;
                    return;
                }

                //var safePos = World.GetSafeCoordForPed(ownerPed.Position, false, 2) - Vector3.Down * 0.4f;
                //_log.Debug($"Safe pos for dog {safePos}");
                
                ThePed.Detach();
                ThePed.IsPositionFrozen = false;
                ThePed.Position = ownerPed.Position;
                await ThePed.NetworkFadeIn();
                ThePed.IsVisible = true;
                Blackboard.Set(bb => bb.State, DogBehaviorState.FollowOwner);
            }
            else if(ThePed.IsVisible)
            {
                ThePed.AttachTo(ownerPed.CurrentVehicle);
                await ThePed.NetworkFadeOut();
            }
        }

        private async Task DogIdleSniffing()
        {
            await BaseScript.Delay(4250);

            Ped nearestPed = World.GetAllPeds()
                .Where(ped => ped.Handle != ThePed.Handle)
                .OrderBy(ped => Vector3.Distance(ped.Position, ThePed.Position))
                .FirstOrDefault();

            if (nearestPed == null) return;

            int nearestPedHandle = nearestPed.Handle;

            if (API.DecorExistOn(nearestPedHandle, "PoliceMP_Ped_Inventory1") ||
                API.DecorExistOn(nearestPedHandle, "PoliceMP_Ped_Inventory2") ||
                API.DecorExistOn(nearestPedHandle, "PoliceMP_Ped_Inventory3") ||
                API.DecorExistOn(nearestPedHandle, "PoliceMP_Ped_Inventory4"))
            {
                var encodedValueSet1 = API.DecorGetInt(nearestPedHandle, "PoliceMP_Ped_Inventory1");
                var encodedValueSet2 = API.DecorGetInt(nearestPedHandle, "PoliceMP_Ped_Inventory2");
                var encodedValueSet3 = API.DecorGetInt(nearestPedHandle, "PoliceMP_Ped_Inventory3");
                var encodedValueSet4 = API.DecorGetInt(nearestPedHandle, "PoliceMP_Ped_Inventory4");

                if (encodedValueSet1 == 0 && encodedValueSet2 == 0 && encodedValueSet3 == 0 && encodedValueSet4 == 0)
                    return;

                List<string> decodedItems = new List<string>();

                foreach (var item in CivSearchDictionaries.vehicleSearchPossibleItems)
                {
                    if ((encodedValueSet1 & item.Value) != 0)
                        decodedItems.Add(item.Key);
                }

                foreach (var item in CivSearchDictionaries.vehicleSearchPossibleItems2)
                {
                    if ((encodedValueSet2 & item.Value) != 0)
                        decodedItems.Add(item.Key);
                }

                foreach (var item in CivSearchDictionaries.vehicleSearchPossibleItemsSenior)
                {
                    if ((encodedValueSet3 & item.Value) != 0)
                        decodedItems.Add(item.Key);
                }

                foreach (var item in CivSearchDictionaries.vehicleSearchPossibleItems3)
                {
                    if ((encodedValueSet4 & item.Value) != 0)
                        decodedItems.Add(item.Key);
                }

                var drugsFound = decodedItems.Where(x =>
                    x.Contains("Cocaine Parcel") ||
                    x.Contains("Cannabis Grinder") ||
                    x.Contains("Spliff") ||
                    x.Contains("Briefcase of Drugs") ||
                    x.Contains("Small Bags of Drugs")
                ).ToList();

                if (drugsFound.Any())
                {
                    var foundString = string.Join(", ", drugsFound);
                    var direction = nearestPed.Position - ThePed.Position;
                    direction.Normalize();

                    _speech.Say(ThePed, $"~r~*Indicates the smell of Drugs*", 6750);
                    

                    Vector3 dogPos;
                    for (int tickCount = 0; tickCount < 1500; tickCount++)
                    {
                        dogPos = ThePed.Position;
                        direction = nearestPed.Position - dogPos;
                        direction.Normalize();
                        await BaseScript.Delay(2);
                        API.DrawMarker(20, 
                            dogPos.X, dogPos.Y, dogPos.Z + 1.0f,
                            direction.X, direction.Y, direction.Z,
                            90f, 0f, 0f, 0.55f, 0.55f, 1f,
                            255, 0, 0, 110, false, false, 2, false, null, null, false);
                    }
                    return;
                }

                _log.Debug("Dog has sniffed nearest player, nothing sus picked up...");
            }
}

        private void PlayVocalization(DogBehaviorVocalization vocalization)
        {
            var soundName = GetBehaviorSoundName(vocalization);

            if (soundName == null)
            {
                _log.Error($"Could not vocalize as could not find sound for {vocalization}!");
                return;
            }
            
            API.PlayAnimalVocalization(ThePed.Handle, 3, soundName);
        }

        private async Task EnsureScenario(string scenarioName)
        {
            if (!API.IsPedUsingScenario(ThePed.Handle, scenarioName))
            {
                _log.Debug($"Making dog use scenario {scenarioName}!");
                ThePed.Task.ClearAll();
                API.TaskStartScenarioInPlace(ThePed.Handle, scenarioName, -1, true);
                ThePed.AlwaysKeepTask = true;
                ThePed.BlockPermanentEvents = true;
            }
        }

        private async Task CheckTeleportToOwner(Ped ownerPed)
        {
            float distanceRemaining = 0f;
            bool isLastRouteSection = false;
            var navResult = API.GetNavmeshRouteDistanceRemaining(ThePed.Handle, ref distanceRemaining, ref isLastRouteSection);

            if (!ownerPed.IsInAir 
                && !ownerPed.IsClimbing 
                && !ownerPed.IsRagdoll
                && ownerPed.IsAlive
                && !ThePed.IsInAir
                && !ThePed.IsRagdoll
                && !ThePed.IsClimbing
                && ThePed.IsAlive
                && ((NavMeshRouteResult)navResult == NavMeshRouteResult.RouteNotFound 
                || distanceRemaining > NavDistanceToTeleport))
            {
                // var nextTeleport = Blackboard.Get(bb => bb.NextCanTeleport);
                // if (ServerInfo.GetServerTime().TotalSeconds < nextTeleport)
                // {
                //     return;
                // }
                if (distanceRemaining < NavDistanceToTeleport)
                {
                    var timeout = Game.GameTime + 5000;
                    while ((NavMeshRouteResult) API.GetNavmeshRouteResult(ThePed.Handle) ==
                           NavMeshRouteResult.RouteNotYetTried && Game.GameTime < timeout)
                    {
                        await BaseScript.Delay(0);
                    }

                    if ((NavMeshRouteResult) API.GetNavmeshRouteResult(ThePed.Handle) != NavMeshRouteResult.RouteNotFound)
                    {
                        return;
                    }
                }

                await TeleportTo(ownerPed.Position);
            }
        }

        private static string GetBehaviorSoundName(DogBehaviorVocalization vocal)
        {
            return vocal switch
            {
                DogBehaviorVocalization.Bark => "BARK",
                DogBehaviorVocalization.Sniff => "SNIFF",
                _ => null
            };
        }

        private void ResetToDefaultState()
        {
            Blackboard.Set(bb => bb.State, DogBehaviorState.FollowOwner);
            Blackboard.Set(bb => bb.TargetNetworkId, -1);
        }

        protected override async Task ThinkRemote()
        {
            var ownerNetworkId = Blackboard.Get(bb => bb.OwnerNetworkId);

            if (Game.PlayerPed.NetworkId == ownerNetworkId)
            {
                await ThePed.TryRequestNetworkEntityControl(false);
            }
        }

        protected override void OnDrawDebug()
        {
            base.OnDrawDebug();
            float distanceRemaining = 0f;
            bool isLastRouteSection = false;
            var navResult = API.GetNavmeshRouteDistanceRemaining(ThePed.Handle, ref distanceRemaining, ref isLastRouteSection);
            
            DrawDebugText($"LastHandledDogVocal: {_lastHandledDogVocal}");
            DrawDebugText($"LastOwnerMove: {_lastOwnerMove}");
            DrawDebugText($"NavMeshDistance: {distanceRemaining} ({isLastRouteSection}");
        }
    }
}
