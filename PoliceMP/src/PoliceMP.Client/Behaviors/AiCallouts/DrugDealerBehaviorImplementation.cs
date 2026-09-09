using System;
using System.Reflection;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Behaviors.AiCallouts;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class DrugDealerBehaviorImplementation : PedBehavior<DrugDealerBehavior>
    {
        private readonly ILogger<DrugDealerBehaviorImplementation> _logger;
        private readonly IBehaviorService _behaviors;
        private readonly IPlayerListAccessor _playerListAccessor;

        private int randomThink;
        private const float HostMigrateThreshold = 10f;

        // ReSharper disable once ConvertToPrimaryConstructor
        public DrugDealerBehaviorImplementation(ITickManager ticks, ILogger<DrugDealerBehaviorImplementation> logger,
            IBehaviorService behaviors, IPlayerListAccessor playerListAccessor) : base(ticks)
        {
            _logger = logger;
            _behaviors = behaviors;
            _playerListAccessor = playerListAccessor;
        }

        public override void Initialize()
        {
            Blackboard.Set(bb => bb.DrugDealerState, DrugDealerState.Idle);
        }

        protected override void Start()
        {
            ThePed.Task.WanderAround();
        }

        protected override async Task Think()
        {
            if (Blackboard.Get(bb=>bb.LastStateChangeTime) + 10000 < API.GetGameTimer())
            {
                Blackboard.Set(bb => bb.DrugDealerState, DrugDealerState.Idle);
            }
            
            var numEntitiesFound = 0;
            Ped entityFound = null;
            foreach (var player in _playerListAccessor.Players)
            {
                if (World.GetDistance(player.Character.Position, ThePed.Position) > 10f) continue;
                if (player.Character == null) continue;

                numEntitiesFound = numEntitiesFound + 1;
                entityFound = player.Character;
                _logger.Debug("Entity found in area: " + player.Name);
            }

            if (entityFound != null && Blackboard.Get(bb => bb.DrugDealerState) == DrugDealerState.Idle && numEntitiesFound > 2)
            {
                Blackboard.Set(bb => bb.DrugDealerState, DrugDealerState.Fleeing);
                StartFleeState(entityFound);
            }

            if (entityFound != null && Blackboard.Get(bb => bb.DrugDealerState) == DrugDealerState.Idle && numEntitiesFound < 2)
            {
                Blackboard.Set(bb => bb.DrugDealerState, DrugDealerState.Phone);
                StartPhoneState();
            }
        }

        private async Task IdleThink()
        {
            var position = ThePed.Position;
            ThePed.Task.StartScenario("WORLD_HUMAN_DRUG_DEALER_HARD", position);
            API.PlayAmbientSpeechWithVoice(ThePed.Handle, "GENERIC_HI", "SHOP_SPECIAL_DISCOUNT", "SPEECH_PARAMS_ALLOW_REPEAT", true);
        }
        
        private void StartFleeState(Ped entityFound)
        {
            ThePed.Task.FleeFrom(entityFound);
        }
        
        private void StartPhoneState() 
        {
            var position = ThePed.Position;
            ThePed.Task.StartScenario("WORLD_HUMAN_STAND_MOBILE", position);
        }

        protected override async Task ThinkRemote()
        {
            var currentHost = API.NetworkGetEntityOwner(ThePed.Handle);
            var hostServerId = Blackboard.Get(bb => bb.HostServerId);
            var nextAbleToMigrate = Blackboard.Get(bb => bb.NextAbleToMigrate);
            var serverTime = ServerInfo.GetServerTime();

            if (nextAbleToMigrate > serverTime.TotalSeconds) return;

            if (hostServerId != Game.Player.ServerId && nextAbleToMigrate < serverTime.TotalSeconds)
            {
                var hostPed = Entity.FromHandle(API.GetPlayerPed(currentHost));
                var hostDistance = World.GetDistance(ThePed.Position, hostPed?.Position ?? Vector3.Zero);

                if (hostDistance > HostMigrateThreshold)
                {
                    var distance = World.GetDistance(ThePed.Position, Game.PlayerPed.Position);
                    if (distance < hostDistance)
                    {
                        if (ThePed.CurrentVehicle != null)
                        {
                            if (await ThePed.TryRequestNetworkEntityControl(false, 100) &&
                                await ThePed.CurrentVehicle.TryRequestNetworkEntityControl(false, 100))
                            {
                                API.SetEntityAsMissionEntity(ThePed.Handle, true, true);
                                API.SetEntityAsMissionEntity(ThePed.CurrentVehicle.Handle, true, true);
                            }
                        }
                        else if (await ThePed.TryRequestNetworkEntityControl(false, 100))
                            API.SetEntityAsMissionEntity(ThePed.Handle, true, true);

                        SetMigrateInformation();

                        return;
                    }
                }
            }

            var handle = ThePed.Handle;
            API.SetEntityAsNoLongerNeeded(ref handle);
            if (ThePed.CurrentVehicle != null)
            {
                handle = ThePed.CurrentVehicle.Handle;
                API.SetEntityAsNoLongerNeeded(ref handle);
            }
        }
    }
}