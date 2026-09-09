using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using Newtonsoft.Json;
using PoliceMP.Client.Services;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;
using PoliceMP.Shared.Options;

namespace PoliceMP.Client.Scripts.Spawn
{
    public class SpawnScript : Script, ISpawnScript
    {
        private const string HasSpawnedState = "HasSpawned";
        private const int TimeoutMs = 15000;
        private readonly ILegacyClientCommunicationsManager _legacyComms;
        private readonly IClientCommunicationsManager _comms;
        private readonly ILogger<SpawnScript> _logger;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly ICustomCharacterService _customCharacterService;
        private readonly ICommandManager _command;
        private readonly INotificationService _notification;
        private readonly IFeatureService _featureService;
        private bool _weatherFeatureEnabled = false;
        private SpawnOptions _options;
        private bool _didLogIdentifiers = false;

        private static List<SpawnLocation> _spawnLocations;
        public static Vector3 LastSpawnPosition { get; private set; }

        private UserAces _userAces;
        
        private string _currentBranch = null;
        private string _currentDivision = null;
        private DateTime _divisionStartTime = DateTime.MinValue;
        private string _discordId = null;

        private string _lastBranch = null;
        private string _lastDivision = null;

        public SpawnScript(ILegacyClientCommunicationsManager legacyComms,
            IClientCommunicationsManager comms,
            ILogger<SpawnScript> logger,
            ITickManager ticks,
            IPermissionService permissionService,
            ICustomCharacterService customCharacterService,
            ICommandManager command,
            IFeatureService featureService,
            INotificationService notification)
        {
            _legacyComms = legacyComms;
            _comms = comms;
            _logger = logger;
            _ticks = ticks;
            _featureService = featureService;
            _permissionService = permissionService;
            _customCharacterService = customCharacterService;
            _command = command;
            _notification = notification;
        }

        protected override async Task OnStartAsync()
        {
            _logger.Debug("SpawnScript Started!");
            _userAces = await _permissionService.GetUserAces();

            #region Spawn Menu

            _spawnLocations = new List<SpawnLocation>
            {
                new("Centralised Control Room",
                    new Core.Shared.Models.PmpVector3(1205.2452392578f, -1463.3435058594f, 34.922050476074f),
                    UserBranch.Control),
                new("Bishopgate", new Core.Shared.Models.PmpVector3(441.787109375f, -977.85845947266f, 35.931079864502f),
                    UserBranch.Control),
                new("Davis", new Core.Shared.Models.PmpVector3(384.04272460938f, -1578.4118652344f, 22.1438331604f),
                    UserBranch.Control),
                new("Vespucci", new Core.Shared.Models.PmpVector3(-1110, -847, 19), UserBranch.Police, UserDivision.Cid),
                new("Vinewood", new Core.Shared.Models.PmpVector3(647, -8, 84), UserBranch.Police, UserDivision.Rpu),
                new("Davis", new Core.Shared.Models.PmpVector3(377.29183959961f, -1583.8188476563f, 29.291717529297f),
                    UserBranch.Police, UserDivision.Dsu),
                new("Rockford", new Core.Shared.Models.PmpVector3(-562.336f, -142.497f, 38.365f), UserBranch.Police),
                new("Traffic Base", new Core.Shared.Models.PmpVector3(817, -1290, 26), UserBranch.Police),
                new("Sandy Shores", new Core.Shared.Models.PmpVector3(1858, 3678, 33), UserBranch.Police),
                new("Paleto Bay", new Core.Shared.Models.PmpVector3(-440, 6019, 31), UserBranch.Police),
                new("Bishopgate", new Core.Shared.Models.PmpVector3(427, -984, 32), UserBranch.Police),
                new("MET Traffic Base", new Core.Shared.Models.PmpVector3(2515.66f, -360.47f, 94.12f), UserBranch.Police),
                new("Alperton Police Base", new Core.Shared.Models.PmpVector3(-1011.261f, -2002.471f, 13.17298f), UserBranch.Police),
                new("Leman Street Firearms Base", new Core.Shared.Models.PmpVector3(-309.7708f, -2698.7690f, 6.0021f), UserBranch.Police),
                new("St Thomas' Hospital",
                    new Core.Shared.Models.PmpVector3(375.75045776367f, -595.30920410156f, 28.798269271851f),
                    UserBranch.Nhs),
                new("Mount Zonah Hospital",
                    new Core.Shared.Models.PmpVector3(-449.91345214844f, -340.67440795898f, 34.501735687256f),
                    UserBranch.Nhs),
                new("Whitechapel Fire Station",
                    new Core.Shared.Models.PmpVector3(1205.2452392578f, -1463.3435058594f, 34.922050476074f),
                    UserBranch.Fire, UserDivision.LFB),
                new("North City Civ Garage", new Core.Shared.Models.PmpVector3(-452.9132f, 1600.729f, 359.2247f),
                    UserBranch.Civ),
                new("West Coast Civ Garage", new Core.Shared.Models.PmpVector3(-2949.847f, 56.87762f, 11.6085f),
                    UserBranch.Civ),
                //new("Highways Operations Centre", new Core.Shared.Models.Vector3( -669.003f, -406.460f, 34.787f), UserBranch.Highways),
                new("M25 National Highways Depot", new Core.Shared.Models.PmpVector3(1527.562f, 816.1177f, 77.43f),
                    UserBranch.Highways),
                new("DVSA Recovery Depot", new Core.Shared.Models.PmpVector3(-193.456f, -1157.848f, 23.05f),
                    UserBranch.Highways),
                // new("DVSA City Depot", new Core.Shared.Models.Vector3(219.377f, -1391.041f, 30.587f), UserBranch.Highways),
                new("Sandy Shores Hospital", new Core.Shared.Models.PmpVector3(1839.821f, 3671.9978f, 34.27668f),
                    UserBranch.Nhs),
                new("Croydon Fire Station", new Core.Shared.Models.PmpVector3(-383.9563f, 6122.702f, 31.47955f),
                    UserBranch.Fire, UserDivision.LFB),
                  new("Rewley Road", new Core.Shared.Models.PmpVector3( 1133.877f, -909.9958f, 51.15871f),
                    UserBranch.Fire, UserDivision.LFB),
                  new("Dockhead Station", new Core.Shared.Models.PmpVector3(-1179.064f, -1256.125f, 6.871882f),
                      UserBranch.Fire, UserDivision.LFB),
                new("Victoria Medical Centre", new Core.Shared.Models.PmpVector3(-233.2601f, 6317.174f, 31.48956f),
                    UserBranch.Nhs),
                new("Sandy Shores Civ Garage", new Core.Shared.Models.PmpVector3(604.688f, 2786.548f, 42.19191f),
                    UserBranch.Civ),
                //new("Thames Valley Station",
                    //new Core.Shared.Models.PmpVector3(-894.52496337891f, -2403.7014160156f, 14.02429485321f),
                   // UserBranch.Police),
                new("Gatwick: Hangar 3", new Core.Shared.Models.PmpVector3(-3097.0657f, 7209.1963f, 43.9886f),
                       UserBranch.Police),   
                new("Heathrow: Hangar A17", new Core.Shared.Models.PmpVector3(-1616.1442f, -3150.717285f, 13.995713f),
                    UserBranch.Police),
                new("Heathrow Civ Garage", new Core.Shared.Models.PmpVector3(-770.3228f, -2611.4282f, 13.9447f),
                    UserBranch.Civ),
                new("Kingston Civ Garage", new Core.Shared.Models.PmpVector3(-2657.392f, 6320.811f, 16.67093f),
                    UserBranch.Civ),
                new("Grapeseed Civ Garage", new Core.Shared.Models.PmpVector3(734.2658f, 4171.511f, 40.69537f),
                    UserBranch.Civ),
                new("South East Civ Garage", new Core.Shared.Models.PmpVector3(1501.2f, -2133.961f, 76.28783f),
                    UserBranch.Civ),
                new("Paleto Outskirts Civ Garage", new Core.Shared.Models.PmpVector3(1522.652f, 6332.058f, 24.15998f),
                    UserBranch.Civ),
                new("South Central Ambulance Station",
                    new Core.Shared.Models.PmpVector3(-1427.465f, -276.3439f, 46.53823f), UserBranch.Nhs),
                new("Cody Road HART Ready Centre",
                    new Core.Shared.Models.PmpVector3(-570.06774902344f, -2198.3117675781f, 6.47974395752f),
                    UserBranch.Nhs),          
                //new("Romford Fire Station",
                   // new Core.Shared.Models.PmpVector3(1922.9144287109f, 4575.517578125f, 38.830474853516f),
                  //  UserBranch.Fire, UserDivision.LFB),
                new("East Ham Fire Station",
                   new Core.Shared.Models.PmpVector3(355.29089355469f, 3414.9938964844f, 36.580894470215f),
                    UserBranch.Fire, UserDivision.LFB),
                new("Lambeth Forensics Centre", new Core.Shared.Models.PmpVector3(-1604.412f, -856.3712f, 10.12169f),
                    UserBranch.Police, UserDivision.Cid),
                new("MCA Coastguard", new Core.Shared.Models.PmpVector3(-1231.382f, -1785.366f, 4.125291f),
                    UserBranch.Fire, UserDivision.CoastGuard),
                new("London Royal Hospital", new Core.Shared.Models.PmpVector3(1148.809f, -1523.7f, 34.84342f),
                    UserBranch.Nhs),
                new("JRU East Vinewood", new Core.Shared.Models.PmpVector3(903.74f, -167.44f, 74.09f),
                    UserBranch.Nhs),
            };


            //_logger.Debug($"Received {_spawnLocations.Count} spawn locations");

            #endregion Spawn Menu

            LastSpawnPosition = Vector3.Zero;

            _options = await _legacyComms.Request<SpawnOptions>(ServerEvents.SpawnGetOptions);

            //_logger.Debug(_options.ToString());

            if (_featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment) || _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess)) { 
                _command.Register("respawn").WithHandler(Respawn); 
            }

            _command.Register("reviveself").WithHandler(async () =>
            {
                var currentRole = _permissionService.CurrentUserRole.Branch;
                var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
                var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);
                var isProductionServer = _featureService.IsFeatureEnabled(FeatureToggle.IsProduction);

                if (_userAces.IsDeveloper ||
                    ((_userAces.IsTierTwo || _userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsDigitalTeam || _userAces.IsQaTeam) && !isProductionServer) ||
                     _userAces.IsDevFunNight)
                {
                    BaseScript.TriggerServerEvent("visn_are:prepareReviveSelf", "", 0);
                    await Delay(100);
                    API.ExecuteCommand("rsac");
                    Revive();

                    API.SetEntityHealth(Game.PlayerPed.Handle, 200);
                    API.ClearPedTasksImmediately(Game.PlayerPed.Handle);
                    MovementHandler._currentState = MovementStates.Normal;

                    if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo || _userAces.IsDigitalTeam || _userAces.IsQaTeam) return;

                    API.ExecuteCommand(
                        "[ADMIN WARNING] ------------------------------------------------- [ADMIN WARNING]");
                    API.ExecuteCommand(
                        "[ADMIN WARNING] THIS PLAYER HAS JUST USED THE /reviveself COMMAND [ADMIN WARNING]");
                    API.ExecuteCommand(
                        "[ADMIN WARNING] ------------------------------------------------- [ADMIN WARNING]");

                    return;
                }

                if (isDevServer && _userAces.IsDigitalTeam || isEaServer && _userAces.IsDigitalTeam)
                {
                    Revive();

                    API.SetEntityHealth(Game.PlayerPed.Handle, 200);
                    API.ClearPedTasksImmediately(Game.PlayerPed.Handle);
                    MovementHandler._currentState = MovementStates.Normal;

                    return;
                }

                _notification.Error("Developer", "Dev things, begone!");
            });

            _command.Register("revive").WithHandler(() =>
            {
                API.ExecuteCommand("[ADMIN WARNING] --------------------------------------------- [ADMIN WARNING]");
                API.ExecuteCommand("[ADMIN WARNING] THIS PLAYER HAS JUST USED THE /revive COMMAND [ADMIN WARNING]");
                API.ExecuteCommand("[ADMIN WARNING] --------------------------------------------- [ADMIN WARNING]");

                if (_userAces.IsDeveloper || _userAces.IsAdmin ||
                    _permissionService.CurrentUserRole.Branch == UserBranch.Nhs)
                {
                    var playerName = API.GetPlayerName(API.PlayerId());
                    foreach (var ped in World.GetAllPeds())
                    {
                        var playerPos = Game.PlayerPed.Position;
                        if (API.GetDistanceBetweenCoords(ped.Position.X, ped.Position.Y, ped.Position.Z, playerPos.X,
                                playerPos.Y, playerPos.Z, true) > 1f)
                        {
                            continue;
                        }

                        if (ped.IsPlayer)
                        {
                            _legacyComms.ToServer(ServerEvents.SpawnNotifyPlayerToRevive, ped.NetworkId);
                            var revivedPlayerName = API.GetPlayerName(API.NetworkGetPlayerIndexFromPed(ped.Handle));
                            if (!string.IsNullOrEmpty(playerName) && !string.IsNullOrEmpty(revivedPlayerName))
                            {
                                //_logger.Debug($"{playerName} has revived {revivedPlayerName}.");
                            }

                            MovementHandler._currentState = MovementStates.Normal;
                        }
                    }
                }
                else
                {
                    _notification.Error("Revive", "You are not NHS.");
                }
            });

            _legacyComms.On(ClientEvents.SpawnToldToRevive, Revive);

            API.RegisterNuiCallback("GetIsRunningAsCiv", new Action<ExpandoObject, CallbackDelegate>(async (_, cb) =>
            {
                cb(_permissionService.CurrentUserRole.Branch == UserBranch.Civ);
            }));
            
            await CreateSpawnSelectionMenu(true);

            _ticks.On(SpawnScriptTick);
            _ticks.On(ModelSuppressionTick);
        }

        private List<string> _vehicleSuppressionList = new List<string>
        {
            "blimp",
            "blimp2",
            "blimp3",
            "duster",
            "stunt",
            "mammatus",
            "dodo",
            "frogger",
            "frogger2",
            "cargoplane",
            "tug",
            "jet",
            "cargobob",
            "lazer",
            "polmav",
            "swift",
            "supervolito",
            "supervolito2",
        };

        private async Task WeatherFeatureCheck()
        {
            _weatherFeatureEnabled = _featureService.IsFeatureEnabled(FeatureToggle.AllowMediaWeatherChange);
            await Delay(30000);
        }

        private Task ModelSuppressionTick()
        {
            foreach (var vehicleSuppression in _vehicleSuppressionList)
            {
                var model = new Model(vehicleSuppression);
                if (!model.IsValid) return Task.FromResult(0);

                var hash = (uint)model.Hash;
                API.SetVehicleModelIsSuppressed(hash, true);
            }

            return Task.FromResult(0);
        }

        private bool _spawnAttempting = false;
        private int _notifyCooldown = 0;

        private async Task SpawnScriptTick()
        {
            if (Game.Player.IsDead)
            {
                if (_notifyCooldown > 0)
                {
                    _notifyCooldown--;
                    await Delay(1);
                    return;
                }

                _notifyCooldown = 2000;
                var builder = new StringBuilder();
                builder.Append(
                    "You have <b>DIED</b> NHS can revive you. Or /respawn.");

                _notification.Info("Dead", builder.ToString());
            }
        }

        private async Task Respawn()
        {
            if (_spawnAttempting) return;
            
            _spawnAttempting = true;
            _logger.Debug("Player died, about to respawn...");
            Game.PlayerPed.Resurrect();
            await SpawnPlayerAsync(LastSpawnPosition, false);
            _spawnAttempting = false;
            _notifyCooldown = 0;
            _logger.Debug("Player should be spawned");
            
            // Revive medical script
            BaseScript.TriggerServerEvent("visn_are:prepareReviveSelf", "", 0);
            await Delay(100);
            API.ExecuteCommand("rsac");
            API.SetEntityHealth(Game.PlayerPed.Handle, 200);
            API.ClearPedTasksImmediately(Game.PlayerPed.Handle);
            MovementHandler._currentState = MovementStates.Normal;
        }

        private async Task Revive()
        {
            if (_spawnAttempting) return;

            _spawnAttempting = true;
            Game.PlayerPed.Resurrect();

            API.SetEntityHealth(Game.PlayerPed.Handle, 200);
            API.ClearPedTasksImmediately(Game.PlayerPed.Handle);
            MovementHandler._currentState = MovementStates.Normal;

            _spawnAttempting = false;
            _notifyCooldown = 0;
            _logger.Debug("Player should be respawned");
        }

        public async Task CreateSpawnSelectionMenu(bool firstLoadIn)
        {
            //_logger.Debug("Creating Spawn Selection Menu");

            if (firstLoadIn)
            {
                var player = Game.PlayerPed;
                var playerID = API.PlayerId();
                var playerSrc = playerID;
                var playerName = API.GetPlayerName(playerID);
                if (!_didLogIdentifiers)
                    _legacyComms.ToServer(ServerEvents.LogDiscordIDToServer, playerName, playerSrc);
                _didLogIdentifiers = true;
            }

            MenuController.CloseAllMenus();

            var spawnMenu = new Menu("Spawn", "Select your spawn");

            MenuController.DisableBackButton = true;
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.AddMenu(spawnMenu);

            var branchNames = new Dictionary<string, UserBranch>
            {
                { "Police", UserBranch.Police },
                { "NHS", UserBranch.Nhs }
            };

            /*if(_userAces.IsNhsClinical)
                branchNames.Add("NHS", UserBranch.Nhs);*/

            if (_userAces.IsFireTrained || _userAces.IsCoastTrained || _userAces.IsMrescueTrained ||
                _userAces.IsFruTrained)
                branchNames.Add("Fire", UserBranch.Fire);
            if (_userAces.IsCivTrained)
                branchNames.Add("Civilian", UserBranch.Civ);
            if (_userAces.IsHighwaysTrained)
                branchNames.Add("Highways", UserBranch.Highways);
            if (_userAces.IsControl)
                branchNames.Add("Control", UserBranch.Control);
            if (_userAces.IsFimTrained)
                branchNames.Add("FIM", UserBranch.Control);

            var branchSelection = new MenuListItem("Role", branchNames.Keys.ToList(), 0);

            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            spawnMenu.AddMenuItem(branchSelection);

            var userRole = PermissionService.DefaultRole;

            #region Police

            var policeDivisionList = new Dictionary<string, UserDivision>
            {
                { "Emergency Response Team", UserDivision.Ert }
            };

            if (_userAces.IsAfoTrained)
            {
                policeDivisionList.Add("Authorized Firearms Officer", UserDivision.Afo);
                //_logger.Debug("Is AFO");
            }

            if (_userAces.IsCidTrained)
            {
                policeDivisionList.Add("CID", UserDivision.Cid);
                //_logger.Debug("Is CID");
            }

            if (_userAces.IsDsuTrained)
            {
                policeDivisionList.Add("Dog Support Unit", UserDivision.Dsu);
                //_logger.Debug("Is DSU");
            }

            if (_userAces.IsNpasTrained)
            {
                policeDivisionList.Add("NPAS", UserDivision.Npas);
                //_logger.Debug("Is NPAS");
            }

            if (_userAces.IsRpuTrained)
            {
                policeDivisionList.Add("Road Transport Policing Command", UserDivision.Rpu);
                //_logger.Debug("Is RPU");
            }
            
            if (_userAces.IsTsgTrained)
            {
                policeDivisionList.Add("Territorial Support Group", UserDivision.Tsg);
                //_logger.Debug("Is TSG");
            }

            var policeDivisionMenuItem = new MenuListItem("Division", policeDivisionList.Keys.ToList(), 0);

            var policeSpawnList = _spawnLocations.Where(o => o.Branch == UserBranch.Police)
                .Select(spawnLocation => spawnLocation.Name).ToList();

            var policeSpawnItem = new MenuListItem("Location", policeSpawnList, 0);

            #endregion Police

            #region NHS

            var nhsDivisionList = new Dictionary<string, UserDivision>
            {
                { "St John", UserDivision.StJohn }
            };

            if(_userAces.IsNhsClinicalTl || _userAces.IsNhsHemsTl || _userAces.IsNhsSectionLeader) 
            {
                nhsDivisionList.Add("NHS Management", UserDivision.NhsMananagement);
            }

            if (_userAces.HasNhsBloodDlc)
            {
                nhsDivisionList.Add("NHS Blood Team", UserDivision.blood);
            }

            if (_userAces.IsNhsParamedic)
            {
                nhsDivisionList.Add("NHS Paramedic", UserDivision.Clinical);
            }

            if (_userAces.IsNhsClinicalAdv)
            {
                nhsDivisionList.Add("NHS Advanced Paramedic", UserDivision.ClinicalAdv);
            }

            if (_userAces.IsStudentPara)
            {
                nhsDivisionList.Add("NHS Student", UserDivision.ClinicalStudent);
            }

            if (_userAces.IsNhsDoctor)
            {
                nhsDivisionList.Add("NHS HEMS Doctor", UserDivision.HemsDoctor);
            }

            if (_userAces.IsNhsHems)
            {
                nhsDivisionList.Add("NHS HEMS Ground Team", UserDivision.Hems);
            }

            if (_userAces.IsHartTrained)
            {
                nhsDivisionList.Add("NHS HART", UserDivision.hart);
            }
            
            if (_userAces.IsBeepDoctor)
            {
                nhsDivisionList.Add("NHS BEEP TEAM", UserDivision.BeepDoctor);
            }

            var nhsDivisionMenuItem = new MenuListItem("Division", nhsDivisionList.Keys.ToList(), 0);

            var nhsSpawnList = _spawnLocations.Where(o => o.Branch == UserBranch.Nhs)
                .Select(spawnLocation => spawnLocation.Name).ToList();

            var nhsSpawnItem = new MenuListItem("Location", nhsSpawnList, 0);

            #endregion NHS

            var lfbDivisionList = new Dictionary<string, UserDivision> { };
            if (_userAces.IsFireTrained)
            {
                lfbDivisionList.Add("Fire & Rescue", UserDivision.LFB);
                //_logger.Debug("Is LFRS");
            }

            if (_userAces.IsCoastTrained)
            {
                lfbDivisionList.Add("Coastguard Agency", UserDivision.CoastGuard);
                //_logger.Debug("Is MCA");
            }

            if (_userAces.IsMrescueTrained)
            {
                lfbDivisionList.Add("Mountain Rescue", UserDivision.mountainRescue);
                //_logger.Debug("Is MR");
            }

            if (_userAces.IsFruTrained)
            {
                lfbDivisionList.Add("FRU", UserDivision.FRU);
                //_logger.Debug("Is FRU");
            }

            var lfbDivisionMenuItem = new MenuListItem("Division", lfbDivisionList.Keys.ToList(), 0);

            var lfbSpawnList = _spawnLocations.Where(o => o.Branch == UserBranch.Fire)
                .Select(spawnLocation => spawnLocation.Name).ToList();

            var lfbSpawnItem = new MenuListItem("Location", lfbSpawnList, 0);

            var civSpawnList = _spawnLocations.Where(o => o.Branch == UserBranch.Civ)
                .Select(sL => sL.Name).ToList();
            var civSpawnItem = new MenuListItem("Location", civSpawnList, 0);

            var highwaysSpawnList = _spawnLocations.Where(o => o.Branch == UserBranch.Highways)
                .Select(sL => sL.Name).ToList();
            var highwaysSpawnItem = new MenuListItem("Location", highwaysSpawnList, 0);

            var controlSpawnList = _spawnLocations.Where(o => o.Branch == UserBranch.Control)
                .Select(sL => sL.Name).ToList();
            var controlSpawnItem = new MenuListItem("Location", controlSpawnList, 0);

            var spawnMenuItem = new MenuItem("Spawn")
            {
                LeftIcon = MenuItem.Icon.STAR
            };

            spawnMenu.AddMenuItem(policeDivisionMenuItem);
            spawnMenu.AddMenuItem(policeSpawnItem);
            spawnMenu.AddMenuItem(spawnMenuItem);
            var spawnInPlaceMenuItem = new MenuItem("<Dev> Spawn In Place")
            {
                LeftIcon = MenuItem.Icon.SHIELD
            };
            if ((_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo))
                spawnMenu.AddMenuItem(spawnInPlaceMenuItem);
            spawnMenu.OpenMenu();

            var itemSelected = false;

            spawnMenu.OnListIndexChange += (menu, item, index, selectionIndex, itemIndex) =>
            {
                if (item == branchSelection)
                {
                    switch (branchNames[item.GetCurrentSelection()])
                    {
                        case UserBranch.Police:
                            menu.ClearMenuItems();
                            menu.AddMenuItem(branchSelection);
                            menu.AddMenuItem(policeDivisionMenuItem);
                            menu.AddMenuItem(policeSpawnItem);
                            menu.AddMenuItem(spawnMenuItem);
                            if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo)
                            {
                                menu.AddMenuItem(spawnInPlaceMenuItem);
                            }

                            break;

                        case UserBranch.Nhs:
                            menu.ClearMenuItems();
                            menu.AddMenuItem(branchSelection);
                            menu.AddMenuItem(nhsDivisionMenuItem);
                            menu.AddMenuItem(nhsSpawnItem);
                            menu.AddMenuItem(spawnMenuItem);
                            if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo)
                            {
                                menu.AddMenuItem(spawnInPlaceMenuItem);
                            }

                            break;

                        case UserBranch.Fire:
                            menu.ClearMenuItems();
                            menu.AddMenuItem(branchSelection);
                            menu.AddMenuItem(lfbDivisionMenuItem);
                            menu.AddMenuItem(lfbSpawnItem);
                            menu.AddMenuItem(spawnMenuItem);
                            if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo)
                            {
                                menu.AddMenuItem(spawnInPlaceMenuItem);
                            }

                            break;

                        case UserBranch.Civ:
                            menu.ClearMenuItems();
                            menu.AddMenuItem(branchSelection);
                            menu.AddMenuItem(civSpawnItem);
                            menu.AddMenuItem(spawnMenuItem);
                            if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo)
                            {
                                menu.AddMenuItem(spawnInPlaceMenuItem);
                            }

                            break;

                        case UserBranch.Highways:
                            menu.ClearMenuItems();
                            menu.AddMenuItem(branchSelection);
                            menu.AddMenuItem(highwaysSpawnItem);
                            menu.AddMenuItem(spawnMenuItem);
                            if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo)
                            {
                                menu.AddMenuItem(spawnInPlaceMenuItem);
                            }

                            break;

                        case UserBranch.Control:
                            menu.ClearMenuItems();
                            menu.AddMenuItem(branchSelection);
                            menu.AddMenuItem(controlSpawnItem);
                            menu.AddMenuItem(spawnMenuItem);
                            if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo)
                            {
                                menu.AddMenuItem(spawnInPlaceMenuItem);
                            }

                            break;

                        default:
                            menu.ClearMenuItems();
                            menu.AddMenuItem(branchSelection);
                            menu.AddMenuItem(spawnMenuItem);
                            if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo)
                            {
                                menu.AddMenuItem(spawnInPlaceMenuItem);
                            }

                            break;
                    }
                }
                else if (item == policeDivisionMenuItem)
                {
                    Debug.WriteLine($"Division Changed: {item.GetCurrentSelection()}");

                    policeSpawnItem.ListIndex = policeDivisionList[item.GetCurrentSelection()] switch
                    {
                        UserDivision.Ert => policeSpawnList.IndexOf("Vespucci"),
                        UserDivision.Afo => policeSpawnList.IndexOf("Leman Street Firearms Base"),
                        UserDivision.Cid => policeSpawnList.IndexOf("Vespucci"),
                        UserDivision.Dsu => policeSpawnList.IndexOf("Davis"),
                        UserDivision.Npas => policeSpawnList.IndexOf("Heathrow: Hangar A17"),
                        UserDivision.Rpu => policeSpawnList.IndexOf("Alperton Police Base"),
                        UserDivision.Tsg => policeSpawnList.IndexOf("Rockford"),
                        _ => policeSpawnItem.ListIndex
                    };
                }
                else if (item == nhsDivisionMenuItem)
                {
                    Debug.WriteLine($"Division Changed: {item.GetCurrentSelection()}");

                    nhsSpawnItem.ListIndex = nhsDivisionList[item.GetCurrentSelection()] switch
                    {
                        UserDivision.StJohn => nhsSpawnList.IndexOf("St Thomas' Hospital"),
                        UserDivision.blood => nhsSpawnList.IndexOf("St Thomas' Hospital"),
                        UserDivision.hart => nhsSpawnList.IndexOf("Cody Road HART Ready Centre"),
                        UserDivision.Clinical => nhsSpawnList.IndexOf("St Thomas' Hospital"),
                        UserDivision.ClinicalStudent => nhsSpawnList.IndexOf("St Thomas' Hospital"),
                        UserDivision.ClinicalAdv => nhsSpawnList.IndexOf("St Thomas' Hospital"),
                        UserDivision.NhsMananagement => nhsSpawnList.IndexOf("St Thomas' Hospital"),
                        UserDivision.Hems => nhsSpawnList.IndexOf("South Central Ambulance Station"),
                        UserDivision.HemsDoctor => nhsSpawnList.IndexOf("South Central Ambulance Station"),
                        UserDivision.BeepDoctor => nhsSpawnList.IndexOf("South Central Ambulance Station"),
                        _ => nhsSpawnItem.ListIndex
                    };
                }
                else if (item == lfbDivisionMenuItem)
                {
                    Debug.WriteLine($"Division Changed: {item.GetCurrentSelection()}");

                    lfbSpawnItem.ListIndex = lfbDivisionList[item.GetCurrentSelection()] switch
                    {
                        UserDivision.LFB => lfbSpawnList.IndexOf("Whitechapel Fire Station"),
                        UserDivision.CoastGuard => lfbSpawnList.IndexOf("MCA Coastguard"),
                        UserDivision.mountainRescue => lfbSpawnList.IndexOf("Croydon Fire Station"),
                        UserDivision.FRU => lfbSpawnList.IndexOf("Whitechapel Fire Station"),
                        _ => lfbSpawnItem.ListIndex
                    };
                }
            };

            spawnMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item != spawnMenuItem && item != spawnInPlaceMenuItem) return;

                if (!item.Selected) return;

                itemSelected = true;

                userRole.Branch = branchNames[branchSelection.GetCurrentSelection()];
                userRole.Division = UserDivision.None;

                if (userRole.Branch == UserBranch.Police)
                {
                    userRole.Division = policeDivisionList[policeDivisionMenuItem.GetCurrentSelection()];
                }

                if (userRole.Branch == UserBranch.Nhs)
                {
                    userRole.Division = nhsDivisionList[nhsDivisionMenuItem.GetCurrentSelection()];
                }

                if (userRole.Branch == UserBranch.Fire)
                {
                    userRole.Division = lfbDivisionList[lfbDivisionMenuItem.GetCurrentSelection()];
                }

                Debug.WriteLine($"Spawn selected: ({userRole.Branch.ToString()}, {userRole.Division.ToString()})");
                Game.Player.State.Set(PlayerStates.HideBlipState, false, true);

                _permissionService.SetUserRole(userRole);

                _legacyComms.ToServer(ServerEvents.SendUserDivision, userRole.Division.ToString());
                
                // Save previous division session (if any)
                if (_currentBranch != null && _currentDivision != null)
                {
                    _lastBranch = _currentBranch;
                    _lastDivision = _currentDivision;

                    SaveDivisionSession(_lastBranch, _lastDivision);
                }

                // Start new division session
                _currentBranch = userRole.Branch.ToString();
                _currentDivision = userRole.Division.ToString();
                _divisionStartTime = DateTime.UtcNow;

                BaseScript.TriggerServerEvent("UserDivisionPlaytime:StartSession", _currentBranch, _currentDivision);


                if (menu.GetMenuItems().Contains(policeSpawnItem) && userRole.Branch == UserBranch.Police)
                {
                    var policeSpawnLocation =
                        _spawnLocations.Where(o => o.Branch == UserBranch.Police).ToList()[policeSpawnItem.ListIndex];
                    _logger.Debug($"Selected: {policeSpawnLocation.Name}");
                    await SpawnPlayerAsync(policeSpawnLocation.Position.ToCitizenVector3(),
                        spawnInPlace: item == spawnInPlaceMenuItem);
                    API.SetPedAsCop(API.PlayerId(), true);
                }

                if (menu.GetMenuItems().Contains(nhsSpawnItem) && userRole.Branch == UserBranch.Nhs)
                {
                    var nhsSpawnLocation =
                        _spawnLocations.Where(o => o.Branch == UserBranch.Nhs).ToList()[nhsSpawnItem.ListIndex];
                    _logger.Debug($"Selected: {nhsSpawnLocation.Name}");
                    await SpawnPlayerAsync(nhsSpawnLocation.Position.ToCitizenVector3(),
                        spawnInPlace: item == spawnInPlaceMenuItem);
                    API.SetPedAsCop(API.PlayerId(), true);
                }

                if (menu.GetMenuItems().Contains(lfbSpawnItem) && userRole.Branch == UserBranch.Fire)
                {
                    var fireSpawnLocation =
                        _spawnLocations.Where(o => o.Branch == UserBranch.Fire).ToList()[lfbSpawnItem.ListIndex];
                    _logger.Debug($"Selected: {fireSpawnLocation.Name}");
                    await SpawnPlayerAsync(fireSpawnLocation.Position.ToCitizenVector3(),
                        spawnInPlace: item == spawnInPlaceMenuItem);
                    API.SetPedAsCop(API.PlayerId(), true);
                }

                if (menu.GetMenuItems().Contains(civSpawnItem) && userRole.Branch == UserBranch.Civ)
                {
                    var civSpawnLocation =
                        _spawnLocations.Where(o => o.Branch == UserBranch.Civ).ToList()[civSpawnItem.ListIndex];
                    Game.Player.State.Set(PlayerStates.CallSign, null, true);
                    Game.Player.State.Set(PlayerStates.HideBlipState, true, true);
                    _logger.Debug($"Selected: {civSpawnLocation.Name}");
                    await SpawnPlayerAsync(civSpawnLocation.Position.ToCitizenVector3(),
                        spawnInPlace: item == spawnInPlaceMenuItem);
                    API.SetPedAsCop(API.PlayerId(), false);
                }

                if (menu.GetMenuItems().Contains(highwaysSpawnItem) && userRole.Branch == UserBranch.Highways)
                {
                    var highwaysSpawnLocation =
                        _spawnLocations.Where(o => o.Branch == UserBranch.Highways).ToList()[
                            highwaysSpawnItem.ListIndex];
                    _logger.Debug($"Selected: {highwaysSpawnLocation.Name}");
                    await SpawnPlayerAsync(highwaysSpawnLocation.Position.ToCitizenVector3(),
                        spawnInPlace: item == spawnInPlaceMenuItem);
                    API.SetPedAsCop(API.PlayerId(), true);
                }

                if (menu.GetMenuItems().Contains(controlSpawnItem) && userRole.Branch == UserBranch.Control)
                {
                    var controlSpawnLocation =
                        _spawnLocations.Where(o => o.Branch == UserBranch.Control).ToList()[controlSpawnItem.ListIndex];
                    _logger.Debug($"Selected: {controlSpawnLocation.Name}");
                    await SpawnPlayerAsync(controlSpawnLocation.Position.ToCitizenVector3(),
                        spawnInPlace: item == spawnInPlaceMenuItem);
                    API.SetPedAsCop(API.PlayerId(), true);
                }

                menu.CloseMenu();
                API.NetworkSetFriendlyFireOption(true);
                API.SetCanAttackFriendly(API.PlayerPedId(), true, false);
            };

            spawnMenu.OnMenuOpen += menu =>
            {
                if (userRole.Branch == UserBranch.Police && !menu.GetMenuItems().Contains(policeDivisionMenuItem))
                {
                    menu.AddMenuItem(policeDivisionMenuItem);
                }
            };

            spawnMenu.OnMenuClose += async menu =>
            {
                if (menu == spawnMenu)
                {
                    MenuController.DisableBackButton = false;
                    if (!itemSelected && firstLoadIn)
                    {
                        await CreateSpawnSelectionMenu(true);
                    }
                }
            };
        }

        private async Task SpawnPlayerAsync(Vector3 position, bool isFirstSpawn = true, bool spawnInPlace = false)
        {
            Debug.WriteLine("Spawning...");

            var customCharacterString = API.GetResourceKvpString(ResourceKvp.LastUsedCharacter);

            Game.PlayerPed.IsInvincible = true;
            Screen.Hud.IsVisible = false;
            Game.Player.Freeze();
            API.SetEnableHandcuffs(Game.PlayerPed.Handle, false);
            if (!API.IsPlayerSwitchInProgress())
            {
                _logger.Trace("Player switch is in progress. Switching out player now...");
                API.SwitchOutPlayer(API.PlayerPedId(), 0, 1);
            }

            var timeOut = Game.GameTime + TimeoutMs;
            while (API.GetPlayerSwitchState() != 5)
            {
                _logger.Trace($"Waiting for SwitchState 5. Current: {API.GetPlayerSwitchState()}");
                await Delay(10);

                if (Game.GameTime > timeOut)
                    break;
            }

            //Game.Player.State.Set(PlayerStates.CallSign, "");


            if (isFirstSpawn)
            {
                API.ShutdownLoadingScreen();
                API.ShutdownLoadingScreenNui();
                if (!spawnInPlace)
                    Game.PlayerPed.Position = position;
                while (!await Game.Player.ChangeModel(new Model(API.GetHashKey(_options.Model))))
                {
                    _logger.Debug("Waiting for Player model change...");
                    await Delay(10);
                }

                API.SetPedDefaultComponentVariation(Game.PlayerPed.Handle);
                _legacyComms.ToClient(ClientEvents.SetSpawnClothes);
                BaseScript.TriggerEvent("PoliceMP:SetPlayerSpawnLocation", position.X, position.Y, position.Z);
            }
            else
            {
                Game.Player.Freeze();
                if (!spawnInPlace)
                    Game.PlayerPed.Position = position;

                API.SetPedDefaultComponentVariation(Game.PlayerPed.Handle);
                _legacyComms.ToClient(ClientEvents.SetSpawnClothes);

                await Delay(500);

                Game.PlayerPed.Resurrect();
            }

            timeOut = Game.GameTime + TimeoutMs;
            while (API.IsEntityWaitingForWorldCollision(Game.PlayerPed.Handle))
            {
                _logger.Trace($"Waiting for SwitchState 8. Current: {API.GetPlayerSwitchState()}");
                await Delay(10);

                if (Game.GameTime > timeOut)
                    break;
            }

            // Publish spawned event before switch in
            _comms.PublishToAll(new PlayerSpawnedEvent
            {
                PedNetworkId = Game.PlayerPed.Handle,
                ServerHandle = Game.Player.Handle
            });

            API.SwitchInPlayer(API.PlayerPedId());

            while (API.GetPlayerSwitchState() < 8)
                await Delay(1);

            Game.Player.Freeze(false);
            API.PlaceObjectOnGroundProperly(Game.PlayerPed.Handle);
            Screen.Hud.IsVisible = true;
            Game.Player.Character.IsVisible = true;
            Game.PlayerPed.IsInvincible = false;

            //_logger.Trace("Spawning player... Done!");

            _legacyComms.ToServer(ServerEvents.PlayerSpawned, isFirstSpawn);
            _legacyComms.ToClient(ClientEvents.PlayerSpawned, isFirstSpawn);

            if (!(_userAces.IsDeveloper && _userAces.IsAdmin))
            {
                Game.PlayerPed.Weapons.RemoveAll();
            }

            Game.PlayerPed.GiveDefaultEquipment(_permissionService.CurrentUserRole, _userAces);

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsProDonator)
            {
                if (!string.IsNullOrEmpty(customCharacterString))
                {
                    var customCharacter = JsonConvert.DeserializeObject<CustomCharacter>(customCharacterString);

                    if (_permissionService.CurrentUserRole.Branch != UserBranch.Civ)
                    {
                        customCharacter.PedOutfit = JsonConvert.SerializeObject(Game.PlayerPed.FetchCurrentPedOutfit());
                    }

                    _customCharacterService.SetCharacterAppearance(customCharacter);
                }
            }

            LastSpawnPosition = position;

            Game.Player.State.Set(PlayerStates.HasSpawned, true, true);
        }
        
        private async void SaveDivisionSession(string branch, string division)
        {
            var duration = (DateTime.UtcNow - _divisionStartTime).TotalSeconds;
            if (duration < 10)
                return;

            if (string.IsNullOrEmpty(_discordId))
            {
                // Get Discord ID
                _discordId = await GetDiscordId();
            }

            var data = new
            {
                Month = DateTime.UtcNow.ToString("yyyy-MM"),
                PlayerName = Game.Player.Name,
                DiscordId = _discordId,
                Branch = branch,
                Division = division,
                PlaytimeSeconds = (int)duration
            };

            BaseScript.TriggerServerEvent("RolePlaytime:LogSession", JsonConvert.SerializeObject(data));

            Debug.WriteLine($"[DivisionPlaytime] Saved {branch} - {division} playtime ({duration} seconds)");
        }

        private Task<string> GetDiscordId()
        {
            var tcs = new TaskCompletionSource<string>();

            BaseScript.TriggerServerEvent("RequestDiscordID");

            _legacyComms.On("ReceiveDiscordID", new Action<string>((id) =>
            {
                tcs.TrySetResult(id);
            }));

            return tcs.Task;
        }

        protected override void OnStop()
        {
            if (_currentBranch != null && _currentDivision != null)
            {
                SaveDivisionSession(_currentBranch, _currentDivision);
            }
        }

    }
}