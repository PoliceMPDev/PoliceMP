using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Actions;
using PoliceMP.Client.Actions.AnalyseBody;
using PoliceMP.Client.Actions.AnalyseEngineTemp;
using PoliceMP.Client.Actions.AskAllOccupantsToStepOut;
using PoliceMP.Client.Actions.AskDriverToStepOut;
using PoliceMP.Client.Actions.AskForId;
using PoliceMP.Client.Actions.BagBody;
using PoliceMP.Client.Actions.BikePickup;
using PoliceMP.Client.Actions.BootEnterHandler;
using PoliceMP.Client.Actions.Breathalyse;
using PoliceMP.Client.Actions.CollectDNA;
using PoliceMP.Client.Actions.CPR;
using PoliceMP.Client.Actions.Cuff;
using PoliceMP.Client.Actions.CuffPlayer;
using PoliceMP.Client.Actions.Defib;
using PoliceMP.Client.Actions.Drugalyse;
using PoliceMP.Client.Actions.Follow;
using PoliceMP.Client.Actions.Grab;
using PoliceMP.Client.Actions.HandsUp;
using PoliceMP.Client.Actions.JailPed;
using PoliceMP.Client.Actions.Kneel;
using PoliceMP.Client.Actions.LieDown;
using PoliceMP.Client.Actions.Medic;
using PoliceMP.Client.Actions.ObservePed;
using PoliceMP.Client.Actions.PutPedInCar;
using PoliceMP.Client.Actions.QuestionPed;
using PoliceMP.Client.Actions.ReleasePed;
using PoliceMP.Client.Actions.RunName;
using PoliceMP.Client.Actions.RunPlate;
using PoliceMP.Client.Actions.SearchPed;
using PoliceMP.Client.Actions.SearchVehicle;
using PoliceMP.Client.Actions.SmashWindows;
using PoliceMP.Client.Actions.StandUp;
using PoliceMP.Client.Actions.TacklePed;
using PoliceMP.Client.Actions.TicketPed;
using PoliceMP.Client.Actions.ToggleSirens;
using PoliceMP.Client.Actions.UseVehicleDoor;
using PoliceMP.Client.Actions.WarnPed;
using PoliceMP.Client.Overlays.Hooks;
using PoliceMP.Client.Overlays.Interaction;
using PoliceMP.Client.Overlays.Legacy.AnprOverlay;
using PoliceMP.Client.Overlays.Legacy.IdCardOverlay;
using PoliceMP.Client.Overlays.Legacy.NotificationOverlay;
using PoliceMP.Client.Overlays.Legacy.SoundPlayerOverlay;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Scripts.Afk;
using PoliceMP.Client.Scripts.AiCallouts;
using PoliceMP.Client.Scripts.ALP;
using PoliceMP.Client.Scripts.Anpr;
using PoliceMP.Client.Scripts.AnprPings;
using PoliceMP.Client.Scripts.Anticheat;
using PoliceMP.Client.Scripts.Armoury;
using PoliceMP.Client.Scripts.Autopilot;
using PoliceMP.Client.Scripts.Backup;
using PoliceMP.Client.Scripts.Boot;
using PoliceMP.Client.Scripts.Callsign;
using PoliceMP.Client.Scripts.CalmVehicles;
using PoliceMP.Client.Scripts.CameraScripts;
using PoliceMP.Client.Scripts.CCTV;
using PoliceMP.Client.Scripts.CharCustom;
using PoliceMP.Client.Scripts.Civ;
using PoliceMP.Client.Scripts.CivVehicleContents;
using PoliceMP.Client.Scripts.CivWalkingStylesMenu;
using PoliceMP.Client.Scripts.ClothingLocker;
using PoliceMP.Client.Scripts.Commands;
using PoliceMP.Client.Scripts.ControlNotify;
using PoliceMP.Client.Scripts.DamageReducer;
using PoliceMP.Client.Scripts.DebugScripts;
using PoliceMP.Client.Scripts.DiscordRichPresence;
using PoliceMP.Client.Scripts.Dsu;
using PoliceMP.Client.Scripts.ELS;
using PoliceMP.Client.Scripts.EmergencyCallScript;
using PoliceMP.Client.Scripts.EntityFreeze;
using PoliceMP.Client.Scripts.Fuel;
using PoliceMP.Client.Scripts.Garage;
using PoliceMP.Client.Scripts.HideBlips;
using PoliceMP.Client.Scripts.HighwaysConvoyScript;
using PoliceMP.Client.Scripts.HUD;
using PoliceMP.Client.Scripts.HUD.HeliHUD;
using PoliceMP.Client.Scripts.Indicators;
using PoliceMP.Client.Scripts.LFBPanic;
using PoliceMP.Client.Scripts.LorryWeighScript;
using PoliceMP.Client.Scripts.Manual;
using PoliceMP.Client.Scripts.MedicalActions;
using PoliceMP.Client.Scripts.MedicalEquipment;
using PoliceMP.Client.Scripts.NHS;
using PoliceMP.Client.Scripts.NoWantedLevel;
using PoliceMP.Client.Scripts.Npas;
using PoliceMP.Client.Scripts.PedInteractionMenu;
using PoliceMP.Client.Scripts.PlateGenerator;
using PoliceMP.Client.Scripts.PlayerControllerScript;
using PoliceMP.Client.Scripts.PlayerControllerScript.Interfaces;
using PoliceMP.Client.Scripts.PlayerTags;
using PoliceMP.Client.Scripts.PlayerToPlayerActions;
using PoliceMP.Client.Scripts.Pointing;
using PoliceMP.Client.Scripts.PrisonBlips;
using PoliceMP.Client.Scripts.Pullover;
using PoliceMP.Client.Scripts.RadioMenu;
using PoliceMP.Client.Scripts.RemoteBlips;
using PoliceMP.Client.Scripts.RoadManagement;
using PoliceMP.Client.Scripts.RoleplayCommands;
//using PoliceMP.Client.Scripts.ServicePoints;
using PoliceMP.Client.Scripts.Sound;
using PoliceMP.Client.Scripts.Spawn;
using PoliceMP.Client.Scripts.SpeedBumps;
using PoliceMP.Client.Scripts.SyncDeadPlayers;
using PoliceMP.Client.Scripts.Tackle;
using PoliceMP.Client.Scripts.Taser;
using PoliceMP.Client.Scripts.Toolbox;
using PoliceMP.Client.Scripts.Trains;
using PoliceMP.Client.Scripts.TwitterChatScript;
using PoliceMP.Client.Scripts.VehicleFaultGenerator;
using PoliceMP.Client.Scripts.VehicleMenu;
using PoliceMP.Client.Scripts.Voice;
using PoliceMP.Client.Scripts.Weapons;
using PoliceMP.Client.Scripts.Weather;
using PoliceMP.Client.Scripts.WhatThreeWord;
using PoliceMP.Client.Scripts.HemsHeliMissions;
using PoliceMP.Client.Scripts.XPSystem;
using PoliceMP.Client.Services;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Services.Overlays;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Commands;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.IoC;
using PoliceMP.Core.Client.Options;
using PoliceMP.Core.Client.Options.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Shared.Services;
using PoliceMP.Shared.Services.Interfaces;

namespace PoliceMP.Client
{
    public class Client : BaseScript,
        ITickManager,
        IGlobalStateAccessor,
        IExportsAccessor,
        IPlayerListAccessor
    {
        private readonly Dictionary<Type, Type> _actionHandlers = new Dictionary<Type, Type>();
        private readonly Dictionary<string, Func<Task>> _tickFuncs = new();

        private IClientRpcManager _clientRpc;
        private IServiceContainer _container;
        private ILogger<Client> _logger;
        private IScriptManager _scriptManager;

        public Client()
        {
            EventHandlers["onClientResourceStart"] += new Func<string, Task>(OnClientResourceStart);
            EventHandlers["onClientResourceStop"] += new Func<string, Task>(OnClientResourceStop);
            EventHandlers[RpcConstants.RpcMessage] += new Func<string, Task>(OnRpcMessage);
        }

        public new ExportDictionary Exports => base.Exports;
        public new StateBag GlobalState => base.GlobalState;
        public new PlayerList Players => base.Players;

        public async Task OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            var services = new ServiceContainerBuilder();

            #region Core Services

            // TODO: Add singleton support to container
            var fiveMEvents = new FiveEventManager(EventHandlers);
            var events = new ClientEventManager(new Logger<ClientEventManager>());
            var rpc = new ClientRpcManager(new Logger<ClientRpcManager>(), fiveMEvents, events);
            var comms = new LegacyClientCommunicationsManager(events, rpc);

            services.Add<IFiveEventManager>(fiveMEvents);
            services.Add<IClientEventManager>(events);
            services.Add<IClientRpcManager>(rpc);
            services.Add<ILegacyClientCommunicationsManager>(comms);
            services.Add<IClientCommunicationsManager, ClientCommunicationsManager>();
            services.Add(typeof(ILogger<>), typeof(Logger<>));
            services.Add<IFeatureService, FeatureService>();
            services.Add<INuiManager>(new NuiManager(EventHandlers, new Logger<NuiManager>()));
            services.Add<ILegacyNuiManager>(new LegacyNuiManager(EventHandlers, new Logger<NuiManager>()));
            services.Add<ICommandManager, CommandManager>();
            services.Add<ITickManager>(this);
            services.Add<IGlobalStateAccessor>(this);
            services.Add<IExportsAccessor>(this);
            services.Add<IPlayerListAccessor>(this);
            services.Add<IOptionsManager, OptionsManager>();
            services.Add<IContextualActionsProvider, ContextualActionsProvider>();
            services.Add<IInstructionalButtonsService, InstructionalButtonsService>();
            services.Add<IActionManager, ActionManager>();
            services.Add<IAnimationService, AnimationService>();
            services.Add<ISoundService, SoundService>();
            services.Add<IPermissionService, PermissionService>();
            services.Add<IPersonalityService, PersonalityService>();
            services.Add<IGameInputManager, GameInputManager>();
            // services.Add<IAiEventService, AiEventService>(); // Suspected Deadloop Crash
            services.AddBehaviorTypes(Assembly.GetExecutingAssembly());

            #endregion Core Services

            #region Other Services

            services.Add<INotificationService, NotificationService>();
            services.Add<IPedInfoService, PedInfoService>();
            services.Add<IVehicleInfoService, VehicleInfoService>();
            services.Add<IVehicleInfoCacheService, VehicleInfoCacheService>();
            services.Add<IPedInfoCacheService, PedInfoCacheService>();
            services.Add<IQuestionService, QuestionService>();
            services.Add<ISpeechService, SpeechService>();
            services.Add<IDateTimeService, DateTimeService>();
            services.Add<ICurrentTicketService, CurrentTicketService>();
            services.Add<IPlayerService, PlayerService>();
            services.Add<ICustomCharacterService, CustomCharacterService>();
            services.Add<IInputService, InputService>();
            services.Add<ICommonFunctionsService, CommonFunctionsService>();

            #endregion Other Services

            #region Inject Scripts

            _ = services.AddScriptManager()
                .Add<ISpawnScript, SpawnScript>()
                .Add<IClothingLocker, ClothingLocker>()
                .Add<IGarageInterface, Garage>()
                .Add<DiscordRichPresence>()
                .Add<VehicleFaultGenerator>()
                .Add<PlateGenerator>()
                .Add<DamageReducer>()
                .Add<DsuNaming>()
                .Add<RoleplayCommands>()
                .Add<TwitterChatScript>()
                .Add<VoiceKeyScript>()
                .Add<AfkScript>()
                .Add<AdminTool>()
                .Add<FlashBang>()
                .Add<IArmoury, Armoury>()
                .Add<IBootSystem, BootSystem>()
                .Add<IDogScript, DogScript>()
                .Add<SpeedBumps>()
                .Add<IPlayerController, PlayerController>()
                .Add<IPedInteractionMenuScript, PedInteractionMenuScript>()
                .Add<IPulloverScript, PulloverScript>()
                .Add<AnprScript>()
                .Add<ICalmVehicles, CalmVehiclesScript>()
                .Add<RadioMenuScript>()
                .Add<VehicleMenu>()
                .Add<ForbiddenWeapon>()
                .Add<Toolbox>()
                .Add<IDebugPedScript, DebugPedScript>()
                .Add<FuelScript>()
                .Add<NoWantedLevelScript>()
                //.Add<ServicePoints>()
                .Add<IHideBlipsScript, HideBlipsScript>()
                .Add<OutfitCommands>()
                .Add<IELS, Els>()
                .Add<ICharCustom, CharCustomScript>()
                .Add<BlacklistWeapons>()
                .Add<SkinCheck>()
                .Add<HealthCheck>()
                .Add<ISoundHandler, SoundHandler>()
                .Add<EntityFreezeHandler>()
                .Add<BackupMenu>()
                .Add<IAdmin, AdminMenu>()
                .Add<AdminYeetGun>()
                .Add<WhatThreeWordScript>()
                .Add<WeatherScript>()
                .Add<AdminChecks>()
                .Add<MedicalActionsScript>()
                .Add<NHSblood>()
                .Add<VehicleCustomMenu>()
                .Add<AnprPings>()
                .Add<RemoteBlips>()
                .Add<PlayerTagsScript>()
                .Add<DistantBlipsScript>()
                .Add<Hud>()
                .Add<NhsProps>()
                .Add<CallSignMenuScript>()
                .Add<ICivArmoury, CivArmoury>()
                .Add<FireModeSelect>()
                .Add<Pointing>()
                .Add<PitCameraScript>()
                .Add<HelicopterHoldCameraScript>()
                .Add<RoadManagement>()
                //.Add<RoadManagementView>()
                .Add<BurstTyre>()
                .Add<CivVehicleContents>()
                .Add<TackleScript>()
                .Add<ActionArrestHandler>()
                .Add<ActionCPRHandler>()
                .Add<ActionDefibHandler>()
                .Add<MovementHandler>()
                .Add<VehicleTowing>()
                .Add<OtherCommands>()
                .Add<SpawnVehicleCommand>()
                .Add<EmergencyCallScript>()
                .Add<HeliHUD>()
                //.Add<HeliTasksScript>()
                .Add<ControlNotify>()
                //.Add<TestScript>()
                //.Add<TrainsScript>()
                .Add<TaserPedWritheScript>()
                .Add<DebugScript>()
                .Add<SyncDeadPlayersScript>()
                .Add<NoExitHelicopterScript>()
                .Add<HighwaysConvoyScript>()
                .Add<MenuCheck>()
                .Add<CCTV>()
                .Add<MedicalEquipment>()
                .Add<LorryWeighScript>()
                .Add<CivWalkingStylesMenu>()
                .Add<CivSearchDictionaries>()
                .Add<RadioDisplayScript>()
                //.Add<CuffedPlayerScript>()
                .Add<AiCallouts>()
                .Add<PrisonBlips>()
                // .Add<SmackBoyle>()
                //.Add<TrainsScript>()
                // .Add<ExperienceSystem>()
                .Add<NpasHoverScript>()
                .Add<ActionCuffHandler>()
                .Add<Autopilot>()
                .Add<Indicators>()
                .Add<ALPScript>()
                .Add<LFBPanicScript>()
                .Add<XPSystem>()
                .Add<HemsHeliMissions>()
                .Add<Manual>();


            // Optional
            //.Add<LoadInteriors>()
            //.Add<IPlayerNames, PlayerNamesScript>()

            //.Add<TaserHandler>()

            #endregion Inject Scripts

            #region Overlays

            // Legacy
            services.Add<NotificationOverlay>();
            services.Add<SoundPlayerOverlay>();
            services.Add<IdCardOverlay>();
            services.Add<AnprOverlay>();

            // Game hooks
            services.Add<UserInterfaceHooksResource>();

            // React Overlays
            services.AddOverlayManager()
                .AddOverlay<IInteractionHud, InteractionHud>("InteractionHud")
                .AddOverlay<INewNotificationOverlay, NewNotificationOverlay>("NewNotificationOverlay")
                .Build();

            #endregion Overlays

            #region Inject Action Handlers

            _actionHandlers.Add(typeof(ActionHandler<Breathalyse>), typeof(BreathalyseHandler));
            _actionHandlers.Add(typeof(ActionHandler<Drugalyse>), typeof(DrugalyseHandler));
            _actionHandlers.Add(typeof(ActionHandler<AskForId>), typeof(AskForIdHandler));
            _actionHandlers.Add(typeof(ActionHandler<Cuff>), typeof(CuffHandler));
            _actionHandlers.Add(typeof(ActionHandler<CuffPlayer>), typeof(CuffPlayerHandler));
            _actionHandlers.Add(typeof(ActionHandler<ObservePed>), typeof(ObservePedHandler));
            _actionHandlers.Add(typeof(ActionHandler<SearchPed>), typeof(SearchPedHandler));
            _actionHandlers.Add(typeof(ActionHandler<WarnPed>), typeof(WarnPedHandler));
            _actionHandlers.Add(typeof(ActionHandler<ReleasePed>), typeof(ReleasePedHandler));
            _actionHandlers.Add(typeof(ActionHandler<Follow>), typeof(FollowHandler));
            _actionHandlers.Add(typeof(ActionHandler<HandsUp>), typeof(HandsUpHandler));
            _actionHandlers.Add(typeof(ActionHandler<Kneel>), typeof(KneelHandler));
            _actionHandlers.Add(typeof(ActionHandler<StandUp>), typeof(StandUpHandler));
            _actionHandlers.Add(typeof(ActionHandler<LieDown>), typeof(LieDownHandler));
            _actionHandlers.Add(typeof(ActionHandler<Grab>), typeof(GrabHandler));
            _actionHandlers.Add(typeof(ActionHandler<AskPedToStepOut>), typeof(AskPedToStepOutHandler));
            _actionHandlers.Add(typeof(ActionHandler<AskAllOccupantsToStepOut>),
                typeof(AskAllOccupantsToStepOutHandler));
            _actionHandlers.Add(typeof(ActionHandler<SearchVehicle>), typeof(SearchVehicleHandler));
            _actionHandlers.Add(typeof(ActionHandler<RunPlate>), typeof(RunPlateHandler));
            _actionHandlers.Add(typeof(ActionHandler<RunName>), typeof(RunNameHandler));
            _actionHandlers.Add(typeof(ActionHandler<JailPed>), typeof(JailPedHandler));
            _actionHandlers.Add(typeof(ActionHandler<QuestionPed>), typeof(QuestionPedHandler));
            _actionHandlers.Add(typeof(ActionHandler<PutPedInCar>), typeof(PutPedInCarHandler));
            _actionHandlers.Add(typeof(ActionHandler<ToggleSirens>), typeof(ToggleSirensHandler));
            _actionHandlers.Add(typeof(ActionHandler<TicketPed>), typeof(TicketPedHandler));
            _actionHandlers.Add(typeof(ActionHandler<UseVehicleDoor>), typeof(UseVehicleDoorHandler));
            _actionHandlers.Add(typeof(ActionHandler<CPR>), typeof(CPRHandler));
            _actionHandlers.Add(typeof(ActionHandler<AnalyseBody>), typeof(AnalyseBodyHandler));
            _actionHandlers.Add(typeof(ActionHandler<SmashWindows>), typeof(SmashWindowsHandler));
            _actionHandlers.Add(typeof(ActionHandler<BagBody>), typeof(BagBodyHandler));
            _actionHandlers.Add(typeof(ActionHandler<CollectDNA>), typeof(CollectDNAHandler));
            _actionHandlers.Add(typeof(ActionHandler<AnalyseEngineTemp>), typeof(AnalyseEngineTempHandler));
            _actionHandlers.Add(typeof(ActionHandler<Defib>), typeof(DefibHandler));
            _actionHandlers.Add(typeof(ActionHandler<Medic>), typeof(MedicHandler));
            _actionHandlers.Add(typeof(ActionHandler<TacklePed>), typeof(TacklePedHandler));
            _actionHandlers.Add(typeof(ActionHandler<BootEnter>), typeof(BootEnterHandler));
            _actionHandlers.Add(typeof(ActionHandler<BikePickup>), typeof(BikePickupHandler));

            foreach (var keyValuePair in _actionHandlers)
            {
                services.Add(keyValuePair.Key, keyValuePair.Value);
            }

            #endregion Inject Action Handlers

            // Start the resources
            _container = services.Build();
            _logger = _container.GetRequired<ILogger<Client>>();
            _clientRpc = _container.GetRequired<IClientRpcManager>();

            var options = _container.GetRequired<IOptionsManager>();
            _logger.Trace("Initializing options...");
            await options.Initialise();
            _logger.Trace("Initializing options... done!");

            _scriptManager = _container.GetRequired<IScriptManager>();

            //await StartScripts(_container);
            await InitialiseActionHandlers(_container);
        }

        private async Task InitialiseActionHandlers(IServiceContainer container)
        {
            // TODO: Be able to get all these using container.GetAllRequired<IActionHandler>()
            var actionHandlers = _actionHandlers
                .Select(keyValuePair => (IActionHandler)container.GetRequired(keyValuePair.Key))
                .ToList();
            var actionManager = container.GetRequired<IActionManager>();

            _logger.Trace($"Initialising {actionHandlers.Count()} action handlers");
            foreach (var actionHandler in actionHandlers)
            {
                _logger.Trace($"Adding {actionHandler.GetType()} to the action manager");
                actionManager.AddHandler(actionHandler);
            }

            foreach (var actionHandler in actionHandlers)
            {
                await actionHandler.Initialise();
            }
        }

        public async Task OnClientResourceStop(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            var scripts = _container.GetAllRequired<Script>();

            _logger.Trace($"Stopping {scripts.Count()} scripts");

            foreach (var script in scripts)
            {
                _logger.Error($"Stopping script {script.GetType().Name}");
                await script.StopAsync();
            }
        }

        public async Task OnRpcMessage(string json)
        {
            try
            {
                //_logger.Debug($"OnRpcMessage: {json}");
                await _clientRpc.HandleMessage(json);
            }
            catch (Exception ex)
            {
                _logger.Error("An RPC Exception occurred", ex);
                _logger.Error($"JSON DATA: {json}");
            }
        }

        #region ITickManager Implementation

        public void On(Func<Task> handler)
        {
            Tick += handler;
        }

        public void Off(Func<Task> handler)
        {
            Tick -= handler;
        }

        #endregion ITickManager Implementation
    }
}