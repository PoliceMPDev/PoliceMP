using System;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Behaviors.Dog;
using PoliceMP.Shared.NetworkMessages.Dog.Commands;
using PoliceMP.Shared.NetworkMessages.Dog.Notifications;
using PoliceMP.Shared.NetworkMessages.Dog.Queries;
using PoliceMP.Shared.Options;
using PoliceMP.Shared.Enums;
using CitizenFX.Core.UI;

namespace PoliceMP.Client.Scripts.Dsu
{
    public interface IDogScript
    {
        bool IsActive();
        Ped GetDog();
        void Sniff(Entity target);
        void FollowOwner();
        void Wait();
        void TakeDownTarget(Ped ped);
        void Pickup();
        DogBehaviorState? GetCurrentState();
        DogOptionsEntry GetActiveDogOptions();
    }
    
    public class DogScript : Script, IDogScript
    {
        private readonly ILogger<DogScript> _log;
        private readonly IClientCommunicationsManager _comms;
        private readonly INotificationService _notifications;
        private readonly IBehaviorService _behaviors;
        private readonly IAnimationService _animationService;
        private readonly ICommandManager _commands;
        private readonly ITickManager _ticks;
        private IPermissionService _permissionService;
        private readonly IGameInputManager _gameInputManager;
        private Ped Dog => _blackboard.Ped;
        private Blackboard<DogBehavior> _blackboard;

        private Menu _dogKennelMenu = new Menu("Dog Kennel", "Select dog from kennel");
        private const string _dogModel = "a_c_shepherd"; //a_c_shepherd
        private const string _dogModel2 = "a_c_retriever"; //a_c_shepherd these are used for dog spawning 
        private DogOptions _dogOptions;
        private DogOptionsEntry _activeDogOptions;
        public DogOptionsEntry GetActiveDogOptions() => _activeDogOptions;

        public DogScript(
            ILogger<DogScript> log,
            IClientCommunicationsManager comms,
            INotificationService notifications,
            IBehaviorService behaviors,
            IAnimationService animationService,
            ICommandManager commands,
            ITickManager ticks,
            IPermissionService permissionService,
            IGameInputManager gameInputManager)

        {
            _log = log;
            _comms = comms;
            _notifications = notifications;
            _behaviors = behaviors;
            _animationService = animationService;
            _commands = commands;
            _ticks = ticks;
            _permissionService = permissionService;
            _gameInputManager = gameInputManager;
        }

        protected override async Task OnStartAsync()
        {
            _comms.AddRequestHandler<GetPlayerDogClientQuery, GetPlayerDogQueryResponse>(GetPlayerDogQueryHandler);
            _comms.AddNotificationHandler<PlayerDogSpawnedEvent>(PlayerSpawnedDogHandler);
            _comms.AddNotificationHandler<PlayerDogRemovedEvent>(PlayerDogRemovedHandler);
            _comms.AddNotificationHandler<PlayerDogSmelledDrugsOnPedEvent>(OnPlayerDogSmelledDrugsOnPedEvent);
            _comms.AddNotificationHandler<PlayerDogSmelledNothingOnPedEvent>(OnPlayerDogSmelledNothingOnPedEvent);
            _comms.AddNotificationHandler<DogTookDownPlayerEvent>(OnDogTookDownPlayerEvent);

            if (_permissionService.CurrentUserRole?.Division == UserDivision.Dsu)
            {
                _commands.Register("dogmenu").WithHandler(() => _dogKennelMenu.OpenMenu());
            };
            
            await LoadDogKennelOptionsAsync();
            MenuController.AddMenu(_dogKennelMenu);
            
            _ticks.On(DrawKennelMarkers);
        }

        private async Task DrawKennelMarkers()
        {
            if (_permissionService.CurrentUserRole == null) return;
            if (_permissionService.CurrentUserRole.Division != UserDivision.Dsu) return;
            
            foreach (var kennel in _dogOptions.Kennels)
            {
                if (MenuController.IsAnyMenuOpen())
                {
                    return;
                }

                var distance = World.GetDistance(Game.PlayerPed.Position, kennel.Position.ToCitizenVector3());

                if (distance > kennel.DisplayRange)
                {
                    continue;
                }

                World.DrawMarker(
                    MarkerType.VerticalCylinder,
                    kennel.Position.ToCitizenVector3() + Vector3.Down,
                    Vector3.Zero,
                    Vector3.Zero,
                    new Vector3(1f, 1f, 2f),
                    Color.FromArgb(50, 232, 232, 0));

                float scale = 0.1F * API.GetGameplayCamFov();
                API.SetTextScale(0.1F * scale, 0.1F * scale);
                API.SetTextFont(4);
                API.SetTextProportional(true);
                API.SetTextColour(250, 250, 250, 255);
                API.SetTextDropshadow(1, 1, 1, 1, 255);
                API.SetTextEdge(2, 0, 0, 0, 255);
                API.SetTextDropShadow();
                API.SetTextOutline();
                API.SetTextEntry("STRING");
                API.SetTextCentre(true);
                API.AddTextComponentString($"Police Kennel");
                API.SetDrawOrigin(kennel.Position.X, kennel.Position.Y, kennel.Position.Z + 1F, 0);
                API.DrawText(0, 0);
                API.ClearDrawOrigin();

                if (distance < kennel.ActivationRange)
                {
                    _dogKennelMenu.OpenMenu();
                }

                while (MenuController.IsAnyMenuOpen() &&
                       World.GetDistance(Game.PlayerPed.Position, kennel.Position.ToCitizenVector3()) < kennel.ActivationRange)
                {
                    await BaseScript.Delay(0);
                }

                _dogKennelMenu.CloseMenu();

                // Wait for player to leave if they've spawned a dog
                while (World.GetDistance(Game.PlayerPed.Position, kennel.Position.ToCitizenVector3()) < kennel.ActivationRange)
                {
                    await BaseScript.Delay(0);
                }
            }
        }

        private async Task OnDogTookDownPlayerEvent(DogTookDownPlayerEvent @event)
        {
            _log.Debug($"OnTookDownPlayerEvent: {@event.PlayerServerHandle} == {Game.Player.ServerId}");
            if (@event.PlayerServerHandle == Game.Player.ServerId)
            {
                // await _animationService.Play(Game.PlayerPed, notification.AnimDict, notification.AnimName, flag: 0);
                // await BaseScript.Delay((int) API.GetAnimDuration(notification.AnimDict, notification.AnimName) * 1000);
                Game.PlayerPed.Task.ClearAllImmediately();
                Game.PlayerPed.Ragdoll(10000);
            }
        }

        private Task OnPlayerDogSmelledDrugsOnPedEvent(PlayerDogSmelledDrugsOnPedEvent @event)
        {
            _log.Debug($"OnPlayerDogSmelledDrugsOnPedEvent");
            _notifications.Warning("Dog Found Something!", "Your dog has smelled something!");
            return Task.FromResult(0);
        }
        
        private Task OnPlayerDogSmelledNothingOnPedEvent(PlayerDogSmelledNothingOnPedEvent @event)
        {
            _log.Debug($"OnPlayerDogSmelledNothingOnPedEvent");
            _notifications.Success("Dog Found Nothing.", "Your dog didn't smell anything suspicious.");
            return Task.FromResult(0);
        }

        private GetPlayerDogQueryResponse GetPlayerDogQueryHandler(GetPlayerDogClientQuery _)
        {
            return new GetPlayerDogQueryResponse()
            {
                NetworkId = Dog.NetworkId
            };
        }

        private Task PlayerSpawnedDogHandler(PlayerDogSpawnedEvent notification)
        {
            _log.Debug($"PlayerSpawnedDogHandler: {API.GetPlayerServerId(API.PlayerId())} {notification.PlayerServerHandle}");
            
            var player = new Player(API.NetworkGetPlayerIndex(notification.PlayerServerHandle));
            if (API.GetPlayerServerId(API.PlayerId()) != notification.PlayerServerHandle)
            {
                return Task.FromResult(-1);
            }

            var dogEntity = Entity.FromNetworkId(notification.DogNetworkId);
            if (dogEntity is not Ped dog)
            {
                _log.Error("Could not set player dog as it's not a Ped!");
                return Task.FromResult(-1);
            }
            
            _blackboard = Blackboard<DogBehavior>.Create(dog);
            _notifications.Info("Dog Deployed", "Your dog has been deployed!");
            return Task.FromResult(0);
        }

        private Task PlayerDogRemovedHandler(PlayerDogRemovedEvent notification)
        {
            _log.Debug($"PlayerDogRemovedHandler: {API.GetPlayerServerId(API.PlayerId())} {notification.PlayerServerHandle}");
            
            if (API.GetPlayerServerId(API.PlayerId()) != notification.PlayerServerHandle)
            {
                return Task.FromResult(0);
            }
            
            _notifications.Info("Dog Returned", "Your dog has been returned.");
            return Task.FromResult(0);
        }

        public bool IsActive()
        {
            try
            {
                return Dog != null && Dog.Exists() && _activeDogOptions != null;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public Ped GetDog()
        {
            return Dog;
        }

        public void Sniff(Entity target)
        {
            _log.Debug($"Dog Sniff Target: {target.NetworkId}");
            _blackboard.Set(bb => bb.TargetNetworkId, target.NetworkId);
            _blackboard.Set(bb => bb.State, DogBehaviorState.SniffTarget);
        }

        public void FollowOwner()
        {
            _log.Debug($"Dog Follow Owner");
            _blackboard.Set(bb => bb.State, DogBehaviorState.FollowOwner);
        }

        public void Wait()
        {
            _log.Debug($"Dog Wait");
            _blackboard.Set(bb => bb.State, DogBehaviorState.Wait);
        }

        public void TakeDownTarget(Ped ped)
        {
            _log.Debug($"Dog TakeDownTarget: {ped.NetworkId}");
            _blackboard.Set(bb => bb.TargetNetworkId, ped.NetworkId);
            _blackboard.Set(bb => bb.State, DogBehaviorState.TakeDownTarget);
        }

        public void Pickup()
        {
            _log.Debug($"Pick up dog");
            Dog.AttachTo(Game.PlayerPed, new Vector3(0.2f, 0f, 0f));
            _ticks.On(DogHoldTick);
        }

        private async Task DogHoldTick()
        {
            if (!Dog.Exists())
            {
                _ticks.Off(DogHoldTick);
            }

            Screen.ShowSubtitle("Press E to drop dog");

            if (_gameInputManager.IsJustPressed(Control.VehicleHorn))
            {
                Dog.Detach();
                _ticks.Off(DogHoldTick);
            }
        }

        public DogBehaviorState? GetCurrentState()
        {
            return _blackboard?.Get(bb => bb.State);
        }

        public Blackboard<DogBehavior> GetBlackboard()
        {
            return _blackboard;
        }

        private async Task DeployDog(int dogOptionsIndex)
        {
            _log.Debug($"Dog DeployDog");
            var result = await _comms.SendToServer(new ServerSpawnDogCommand
            {
                DogOptionsIndex = dogOptionsIndex
            });

            if (result.IsSuccess)
            {
                _activeDogOptions = result.DogOptions;
            }
        }
        
        private async Task LoadDogKennelOptionsAsync()
        {
            _dogOptions = await _comms.SendToServer(new GetDogOptionsQuery());

            for (int i = 0; i < _dogOptions.Dogs.Count; i++)
            {
                var option = _dogOptions.Dogs[i];
                _dogKennelMenu.AddMenuItem(new MenuItem(option.Name)
                {
                    ItemData = i
                });
            }

            _dogKennelMenu.OnItemSelect += (menu, item, index) =>
            {
                DeployDog(item.ItemData);
                _dogKennelMenu.CloseMenu();
            };
        }
    }
}
