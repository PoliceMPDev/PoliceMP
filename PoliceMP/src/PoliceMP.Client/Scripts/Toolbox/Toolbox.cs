using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.Armoury;
using PoliceMP.Client.Scripts.Civ;
using PoliceMP.Client.Scripts.Spawn;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Shared.Behaviors.AiCallouts;
using PoliceMP.Shared.NetworkMessages.Callouts.Notifications;

namespace PoliceMP.Client.Scripts.Toolbox
{
    public class Toolbox : Script
    {
        #region Services

        private readonly ITickManager _tickManager;
        private readonly IPermissionService _permissionService;
        private readonly IArmoury _armoury;
        private readonly ISpawnScript _spawnScript;
        private readonly ICustomCharacterService _customCharacterService;
        private readonly ICommandManager _commandManager;
        private readonly ICivArmoury _civArmoury;
        private readonly IVehicleInfoService _vehicleInfo;
        private readonly ILogger<Toolbox> _logger;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IBehaviorService _behaviors;
        private readonly IFeatureService _featureService;
        private readonly IClientCommunicationsManager _comms;
        private readonly ILegacyClientCommunicationsManager _legacyComms;

        #endregion Services

        #region Lists

        private readonly List<string> _colorNames = new()
        {
            "Midnight Blue", "Blue", "Black Blue", "Dark Blue",
            "Saxon Blue", "Royal Blue", "Black", "Carbon Black",
            "Graphite", "Black Steel", "Silver", "Stone Silver",
            "Midnight Silver", "Util Shadow Silver", "Ice White", "Frost White",
            "Grace Red", "Carbernet Red", "Bluish Silver", "Cast Iron Silver",
            "Red", "Gold", "Orange", "Racing Green",
            "Olive Green", "Bright Green", "Gasoline Green", "Mariner Blue",
            "Harbor Blue", "Diamond Blue", "Ultra Blue", "Purple Blue",
            "Racing Blue", "Yellow", "Race Yellow", "Lime Green",
            "Dark Ivory", "Maple Brown", "Moss Brown", "Woodbeach Brown",
            "Choco Orangesienna", "Sandy Brown", "Cream", "Pea Green",
            "Hot Pink", "Salmon Pink", "Green", "Bright Purple",
            "Brushed Steel", "Brushed Black Steel", "Brushed Aluminium", "Brushed Gold",
            "Pure Gold", "MP100", "Chrome", "Pure White",
            "Fluorescent Blue", "Epsilon Blue", "Matte Black", "Matte Light Grey",
            "Matte Red", "Matte Dark Red", "Matte Orange", "Matte Yellow",
            "Matte Lime Green", "Matte Dark Blue", "Matte Blue", "Matte Green",
            "Matte Ice White", "Matte Schafter Purple", "Matte Olive Drab", "Matte Olive Earth", "Matte Olive Tan" };

        private readonly List<int> _colorValues = new()
        { 61, 64, 141, 62, 63, 64, 0, 147, 1, 2, 4, 8, 9, 20, 111, 112, 31, 34, 5, 10, 27, 37, 38, 50, 52, 53, 54, 65, 66, 67, 70,
            71, 73, 88, 89, 92, 95, 97, 100, 102, 104, 105, 107, 125, 135, 136, 139, 145, 117, 118, 119, 159,
            158, 160, 120, 134, 140, 157, 12, 14, 39, 40, 41, 42, 55, 82, 83, 128, 131, 148, 152, 153, 154 };

        private int[] _allowedEmergencyColours =
        {
            61,      //Metallic - Midnight Blue
            64,      //Metallic - Blue
            141,     //Metallic - Metallic Black Blue
            62,      //Metallic - Dark Blue
            63,      //Metallic - Saxon Blue
            64,      //Metallic - Royal Blue
            0,       //Metallic - Black
            147,     //Metallic - Carbon Black
            1,       //Metallic - Graphite
            2,       //Metallic - Black Steel
            4,       //Metallic - Silver
            8,       //Metallic - Stone Silver
            9,       //Metallic - Midnight Silver
            20,      //Metallic - Util Shadow Silver
            111,     //Metallic - Ice White
            112,     //Metallic - Frost White
            31,      //Metallic - Grace Red (for CID)
            34,      //Metallic - Carbernet Red (for CID)
        };

        private int[] _allowedCivColours =
        {
            0,      //Metallic - Black
            2,      //Metallic - Black Steel
            3,      //Metallic - Dark Steel
            4,      //Metallic - Silver
            5,      //Metallic - Bluish Silver
            9,      //Metallic - Midnight Silver
            10,     //Metallic - Cast Iron Silver
            27,     //Metallic - Red
            31,     //Metallic - Grace Red
            34,     //Metallic - Carbernet Red
            37,     //Metallic - Gold (classic)
            38,     //Metallic - Orange
            50,     //Metallic - Racing Green
            52,     //Metallic - Olive Green
            53,     //Metallic - Bright Green
            54,     //Metallic - Gasoline Green
            62,     //Metallic - Dark Blue
            64,     //Metallic - Royal Blue
            65,     //Metallic - Mariner Blue
            66,     //Metallic - Harbor Blue
            67,     //Metallic - Diamond Blue
            70,     //Metallic - Ultra Blue
            71,     //Metallic - Purple Blue
            73,     //Metallic - Racing Blue
            88,     //Metallic - Yellow
            89,     //Metallic - Race Yellow
            92,     //Metallic - Lime Green
            95,     //Metallic - Dark Ivory
            97,     //Metallic - Maple Brown
            100,    //Metallic - Moss Brown
            102,    //Metallic - WoodBeach Brown
            104,    //Metallic - Choco OrangeSienna
            105,    //Metallic - Sandy Brown
            107,    //Metallic - Cream
            111,    //Metallic - Ice White
            112,    //Metallic - Frost White
            125,    //Metallic - Pea Green
            135,    //Metallic - Hot Pink
            136,    //Metallic - Salmon Pink
            139,    //Metallic - Green
            145,    //Metallic - Bright Purple

            117,    //Metal - Brushed Steel
            118,    //Metal - Brushed Black Steel
            119,    //Metal - Brushed Aluminium
            159,    //Metal - Brushed Gold
            158,    //Metal - Pure Gold
            160,    //Metal - MP100 (Secret gold)

            120,    //Chrome - Chrome (🤮)

            134,    //Unknown - Pure white
            140,    //Unknown - Fluorescent Blue
            157,    //Unknown - Epsilon Blue

            12,     //Matte - Black
            14,     //Matte - Light Grey
            39,     //Matte - Red
            40,     //Matte - Dark Red
            41,     //Matte - Orange
            42,     //Matte - Yellow
            55,     //Matte - Lime Green
            82,     //Matte - Dark Blue
            83,     //Matte - Blue
            128,    //Matte - Green
            131,    //Matte - Ice White
            148,    //Matte - Schafter Purple
            152,    //Matte - Olive Drab
            153,    //Matte - Olive Earth
            154,    //Matte - Olive Tan
        };

        private readonly List<uint> _fuelPumps = new List<uint>
        {
            1339433404,
            1694452750,
            1933174915,
            2287735495,
            3825272565,
            4130089803,
            3832150195,
            1767019582,
        };

        #endregion Lists

        #region Toolbox Menus

        private class ToolboxCategory
        {
            private readonly string _name;
            private readonly string _description;

            private Dictionary<MenuItem, Action> MenuItems { get; } = new();
            public Menu Menu { get; }

            public ToolboxCategory(string name, string description)
            {
                _name = name;
                _description = description;
                Menu = new Menu(name, description);
            }

            public void AddItem(string n, string d, Action onSelect)
            {
                var menuItem = new MenuItem(n, d);
                MenuItems.Add(menuItem, onSelect);
                Menu.AddMenuItem(menuItem);
            }

            public void AddCheckbox(string n, string d, bool isChecked, Action onSelect)
            {
                var menuItem = new MenuCheckboxItem(n, d, isChecked);
                MenuItems.Add(menuItem, onSelect);
                Menu.AddMenuItem(menuItem);
            }

            public void AddDirectItem(MenuItem menuItem, Action onSelect)
            {
                MenuItems.Add(menuItem, onSelect);
                Menu.AddMenuItem(menuItem);
            }

            public void Finish(Menu toolboxMenu)
            {
                Menu.OnItemSelect += async (menu, item, index) =>
                {
                    if (menu != Menu) return;
                    if (MenuItems.TryGetValue(item, out var onSelect))
                    {
                        MenuController.CloseAllMenus();
                        onSelect.DynamicInvoke();
                    }
                };

                Menu.OnCheckboxChange += async (menu, item, index, isChecked) =>
                {
                    if (menu != Menu) return;
                    if (MenuItems.TryGetValue(item, out var onSelect))
                        onSelect.DynamicInvoke(isChecked);
                };

                var menuItem = new MenuItem(_name, _description);
                MenuController.BindMenuItem(toolboxMenu, Menu, menuItem);
                toolboxMenu.AddMenuItem(menuItem);
            }
        }

        #endregion

        private UserAces UserAces { get; set; }
        private Vehicle FixingVehicle { get; set; }

        private Menu _toolboxMenu;

        private bool _enableAutoHelmets = false;

        private bool _isClocked = false;
        
        private bool _isBodyCamOn = false;

        public Toolbox(ITickManager tickManager, IPermissionService permissionService, IArmoury armoury, ISpawnScript spawnScript, ICustomCharacterService customCharacterService,
            ICommandManager commandManager, ICivArmoury civArmoury, IVehicleInfoService vehicleInfo, ILogger<Toolbox> logger, INewNotificationOverlay newNotificationOverlay, IBehaviorService behaviors, IFeatureService featureService, IClientCommunicationsManager comms, ILegacyClientCommunicationsManager legacyComms)
        {
            _tickManager = tickManager;
            _permissionService = permissionService;
            _armoury = armoury;
            _spawnScript = spawnScript;
            _customCharacterService = customCharacterService;
            _commandManager = commandManager;
            _civArmoury = civArmoury;
            _vehicleInfo = vehicleInfo;
            _logger = logger;
            _newNotificationOverlay = newNotificationOverlay;
            _behaviors = behaviors;
            _featureService = featureService;
            _comms = comms;
            _legacyComms = legacyComms;

            commandManager.Register("toolbox").WithHandler(async () =>
            {
                if (_toolboxMenu == null)
                {
                    await CreateToolboxMenu();
                    return;
                }
                if (_toolboxMenu.Visible)
                {
                    _toolboxMenu.CloseMenu();
                    return;
                }

                if (MenuController.IsAnyMenuOpen()) return;
                await CreateToolboxMenu();
            });

            commandManager.Register("recordstart").WithHandler(() =>
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Recording", "info", "Recording Started", new NewNotificationMessageContent[0]));
                API.StartRecording(1);
            });

            commandManager.Register("recordstop").WithHandler(() =>
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Recording", "info", "Recording Stopped", new NewNotificationMessageContent[0]));
                API.StopRecording();
            });

            commandManager.Register("recordeditor").WithHandler(() =>
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Recording", "info", "Rockstar Editor Opening", new NewNotificationMessageContent[0]));
                API.ActivateRockstarEditor();
            });

            commandManager.Register("discord").WithHandler(DiscordLink);

            API.RegisterKeyMapping("toolbox", "Toggles the Toolbox", "keyboard", "F1");

            comms.AddNotificationHandler<UserClockedOnNotificationEvent>(ClockOn);
            comms.AddNotificationHandler<UserClockedOffNotificationEvent>(ClockOff);

            API.RegisterNuiCallback("CanOpenPauseMenu", new Action<ExpandoObject, CallbackDelegate>((_, cb) =>
            {
                cb(!MenuController.IsAnyMenuOpen());
            }));
        }

        protected override async Task OnStartAsync()
        {
            _logger.Debug("Toolbox Started");
            _tickManager.On(ToolboxTick);

            _commandManager.Register("colour").HasGreedyArgs().WithHandler(async (colour) =>
            {
                if (colour != null) await ColorVehicle(colour);
                _logger.Debug("colour car with " + colour);
            });

            _commandManager.Register("openMap").WithHandler(() =>
            {
                API.ActivateFrontendMenu((uint)API.GetHashKey("FE_MENU_VERSION_MP_PAUSE"), false, -1);
            });

            // Register controller "START" button to open the map
            API.RegisterKeyMapping("openMap", "Open the map", "keyboard", "p");
            API.RegisterKeyMapping("~!openMap", "Open the map via controller", "pad_digitalbutton", "START_INDEX");

            #region Waypoint Colours

            _commandManager.Register("waypointcolour").HasGreedyArgs().WithHandler(args =>
            {
                try
                {
                    var splits = args.Split();
                    //var tryParse = Int32.TryParse(args[0].ToString(), out int r);
                    if (!Int32.TryParse(splits[0].ToString(), out int r) ||
                        !Int32.TryParse(splits[1].ToString(), out int g) ||
                        !Int32.TryParse(splits[2].ToString(), out int b))
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Waypoint Colour", "error", "Please you a proper RGB value", new NewNotificationMessageContent[0]));
                        return;
                    }
                    API.ReplaceHudColourWithRgba(142, r, g, b, 255);
                    API.SetResourceKvpInt("PMP_WAYPOINT_COLOUR_R", r);
                    API.SetResourceKvpInt("PMP_WAYPOINT_COLOUR_G", g);
                    API.SetResourceKvpInt("PMP_WAYPOINT_COLOUR_B", b);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Waypoint Colour", "error", $"{r} {g} {b}", new NewNotificationMessageContent[0]));
                }
                catch (Exception ex)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Waypoint Colour", "error", "Please you a proper RGB value", new NewNotificationMessageContent[0]));
                }
            });
            int r = API.GetResourceKvpInt("PMP_WAYPOINT_COLOUR_R");
            int g = API.GetResourceKvpInt("PMP_WAYPOINT_COLOUR_G");
            int b = API.GetResourceKvpInt("PMP_WAYPOINT_COLOUR_B");
            if (r != 0 && b != 0 && g != 0)
            {
                API.ReplaceHudColourWithRgba(142, r, g, b, 255);
            }

            #endregion Waypoint Colours

            UserAces = await _permissionService.GetUserAces();

            var otherFuelPumps = new List<string>
            {
                "petrol_pump"
            };

            foreach (var otherFuelPump in otherFuelPumps)
            {
                _fuelPumps.Add((uint)API.GetHashKey(otherFuelPump));
            }

            var playerId = API.PlayerId();
            var playerName = API.GetPlayerName(playerId);

            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Welcome!", "info", $"Welcome to PoliceMP {playerName}, We hope you enjoy your time, if you need anything please come on the discord: discord.gg/policemp", new NewNotificationMessageContent[0]));
        }

        private async Task ToolboxTick()
        {
            #region PCSO Grab Player Check

            var enteringVehicleHandle = API.GetVehiclePedIsTryingToEnter(Game.PlayerPed.Handle);
            var enteringVehicle = (Vehicle)Entity.FromHandle(enteringVehicleHandle);
            if (enteringVehicle != null)
            {
                var vehicleDriver = enteringVehicle.Driver;
                if (vehicleDriver != null && vehicleDriver.IsPlayer && !UserAces.IsWhiteListed)
                {
                    // Vehicle has driver who is a player and the client is a PCSO
                    Game.PlayerPed.Task.ClearAllImmediately();
                }
            }

            #endregion PCSO Grab Player Check

            #region RPU No Auto Helmets

            var currentUserRole = _permissionService.CurrentUserRole;

            if (currentUserRole == null) return;

            if (currentUserRole.Branch != UserBranch.Civ)
            {
                API.RemovePlayerHelmet(Game.Player.Handle, true);
                API.SetPedHelmetFlag(Game.PlayerPed.Handle, 0);
                API.SetPedConfigFlag(Game.PlayerPed.Handle, 34, false);
                API.SetPedConfigFlag(Game.PlayerPed.Handle, 35, false);
            }
            else
            {
                if (!_enableAutoHelmets)
                {
                    API.RemovePlayerHelmet(Game.Player.Handle, true);
                    API.SetPedHelmetFlag(Game.PlayerPed.Handle, 0);
                    API.SetPedConfigFlag(Game.PlayerPed.Handle, 34, false);
                    API.SetPedConfigFlag(Game.PlayerPed.Handle, 35, false);
                }
                else
                {
                    API.SetPedConfigFlag(Game.PlayerPed.Handle, 35, true);
                }
            }

            #endregion RPU No Auto Helmets

            #region Disable Vehicle Rewards

            API.DisablePlayerVehicleRewards(Game.Player.Handle);

            #endregion Disable Vehicle Rewards
        }

        private Task ClockOn(UserClockedOnNotificationEvent _) {
            _isClocked = true;
            return Task.FromResult(0);
        }

        private Task ClockOff(UserClockedOffNotificationEvent _) {
            _isClocked = false;
            return Task.FromResult(0);
        }

        private async Task CreateToolboxMenu()
        {
            var currentUserRole = _permissionService.CurrentUserRole;

            _toolboxMenu?.ClearMenuItems();
            _toolboxMenu = new Menu("PoliceMP Toolbox", "Everything you may need");

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.AddMenu(_toolboxMenu);

            #region player options

            var playerOptionsCategory = new ToolboxCategory("Player Options", "Options for the player");

            playerOptionsCategory.AddItem("Change Role", "Change your role", GotoPd);

            if (UserAces.IsProDonator || UserAces.IsAdmin || UserAces.IsDeveloper)
            {
                playerOptionsCategory.AddItem("Custom Characters", "View your custom characters",
                    ShowCustomCharacterSelection);
                playerOptionsCategory.AddItem("Outfits Menu", "Edit, amend and add outfits", UseDisplayOutfitsCommand);
            }

            if (UserAces.IsProDonator && UserAces.IsAfoTrained && currentUserRole.Division == UserDivision.Afo ||
                UserAces.IsAdmin || UserAces.IsDeveloper)
                playerOptionsCategory.AddItem("Remote Armoury", "Take out or return weapons", AfoGearUp);

            if (!string.IsNullOrEmpty(API.GetResourceKvpString(ResourceKvp.LastUsedCharacter)))
                playerOptionsCategory.AddItem("Clear Saved Character", "Clears spawn character",
                    () => API.DeleteResourceKvp(ResourceKvp.LastUsedCharacter));

            playerOptionsCategory.Finish(_toolboxMenu);

            #endregion player options

            #region vehicle options

            var vehicleOptionsCategory = new ToolboxCategory("Vehicle Options", "Options for the vehicle");

            if (UserAces.IsAdmin || UserAces.IsDeveloper || UserAces.IsTierTwo ||
                (UserAces.IsProDonator && currentUserRole.Branch != UserBranch.Civ) ||
                (currentUserRole.Branch == UserBranch.Civ && GiveVehicleFixOption(currentUserRole)))
            {
                vehicleOptionsCategory.AddItem("Fix Vehicle", "Fix your vehicle", FixVehicle);
            }

            if (currentUserRole.Branch == UserBranch.Civ ||
                currentUserRole.Division is UserDivision.Afo or UserDivision.Rpu or UserDivision.Cid or UserDivision.Dsu
                    or UserDivision.Ert or UserDivision.hart or UserDivision.ClinicalAdv ||
                UserAces.IsNhsSectionLeader && currentUserRole.Branch == UserBranch.Nhs ||
                UserAces.IsFireStationCommander && currentUserRole.Branch == UserBranch.Fire || UserAces.IsAdmin ||
                UserAces.IsDeveloper)
                vehicleOptionsCategory.AddItem("Colour Car", "Change the colour of your vehicle", ShowColorVehicleMenu);

            vehicleOptionsCategory.AddItem("Manual Transmission", "Open the manual transmission menu", ShowManualMenu);
            
            if (
                UserAces.IsAdmin || UserAces.IsDeveloper || UserAces.IsTierTwo ||
                (currentUserRole.Division == UserDivision.Afo) ||
                (currentUserRole.Division == UserDivision.Rpu) ||
                (currentUserRole.Branch == UserBranch.Highways)
            )
                vehicleOptionsCategory.AddItem("Matrix Board", "Open Matrix Board Settings", () => ShowMatrixBoardMenu(currentUserRole));

            if (UserAces.IsProDonator || UserAces.IsTierOne || UserAces.IsTierTwo || UserAces.IsAdmin ||
                UserAces.IsDeveloper)
                vehicleOptionsCategory.AddItem("Autopilot", "Open the autopilot menu", ShowAutopilotMenu);

            vehicleOptionsCategory.Finish(_toolboxMenu);

            #endregion vehicle options

            #region systems menu

            var systemsCategory = new ToolboxCategory("Systems", "Systems to use while on duty");

            systemsCategory.AddItem("ANPR System", "Place or trace ANPR hits", () =>
            {
                API.ExecuteCommand("alprtablet");
            });
            systemsCategory.AddItem("Notepad", "Write your notes", () =>
            {
                API.ExecuteCommand("notes");
            });
            systemsCategory.AddItem("Traffic Management", "Manage AI Traffic", () =>
            {
                API.ExecuteCommand("traffic");
            });
            
            if (UserAces.IsAdmin || UserAces.IsDeveloper || UserAces.IsTierTwo ||
                (currentUserRole.Division == UserDivision.Cid))
            {
                systemsCategory.AddItem("Cell Site Ping", "Ping Cell Tower of suspects mobile phone", () =>
                {
                    API.ExecuteCommand("+zcelltrace");
                });
            }
            
            if (UserAces.IsAdmin || UserAces.IsDeveloper || UserAces.IsTierTwo ||
                (currentUserRole.Division == UserDivision.Afo))
            {
                systemsCategory.AddItem("Body Cam Menu", "Body Cam Tools", ShowBodyCamMenu);
            }
            
            systemsCategory.AddItem("Seize Vehicle", "Add seize vehicle prop", () =>
            {
                API.ExecuteCommand("+seize");
            });
            systemsCategory.AddItem("Binoculars", "To magnify view ", () =>
            {
                API.ExecuteCommand("+xbino");
            });

            if (UserAces.IsWhiteListed || UserAces.IsAdmin || UserAces.IsDeveloper)
                systemsCategory.Finish(_toolboxMenu);

            #endregion systems menu

            #region civ menu

            var civCategory = new ToolboxCategory("Civ Menu", "Options for Civilians");

            civCategory.AddItem("Character Menu", "Character Options", () =>
            {
                API.ExecuteCommand("+charmenu");
            });

            civCategory.AddItem("Injuries Menu", "Addinjury to character", () =>
            {
                API.ExecuteCommand("+cxinjury");
            });

            civCategory.AddItem("Weapons", "Weapons Menu", () =>
            {
                _civArmoury.ShowCivArmouryMenu();
            });

            civCategory.AddItem("Change Vehicle Reg", "Change the registration of your vehicle", () =>
            {
                API.ExecuteCommand("cvlptc");
            });

            civCategory.AddItem("Leave Clues", "Leave clues for the police", () =>
            {
                API.ExecuteCommand("+xclue");
            });

            civCategory.AddItem("Create Scent", "Allow nearby dogs to indicate", () =>
            {
                API.ExecuteCommand("createscent");
            });

            civCategory.AddItem("Set HGV Weight", "Set the weight of your HGV", () =>
            {
                API.ExecuteCommand("setvehicleweight");
            });

            civCategory.AddItem("Set Person Contents", "Set the contents of your person", () =>
            {
                API.ExecuteCommand("cpbsc");
            });

            civCategory.AddItem("Set Vehicle Contents", "Set the contents of your vehicle", () =>
            {
                API.ExecuteCommand("cvbsc");
            });

            civCategory.AddItem("Walking Styles", "Change your walking style", () =>
            {
                API.ExecuteCommand("walkingstyles");
            });

            civCategory.AddItem("Toggle Auto Helmets", "Toggle auto helmets on/off", () =>
            {
                _enableAutoHelmets = !_enableAutoHelmets;
                string status = _enableAutoHelmets ? "Toggled on!" : "Toggled off!";

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Auto Helmet", "info", status));
            });

            civCategory.AddItem("Toggle L Plates", "Toggle Learners Plates on/off", () =>
            {
                API.ExecuteCommand("+xlplates");
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Learner's Plates", "info", "Toggled Learner's Plates! Drive Safe!"));
            });

            civCategory.AddItem("Toggle P Plates", "Toggle Probationary Plates on/off", () =>
            {
                API.ExecuteCommand("+xpplates");
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Probationary Plates", "info", "Toggled Probationary Plates! Drive Safe!"));
            });

            civCategory.AddItem("Clear Task", "Clear your current task", () => { Game.PlayerPed.Task.ClearAll(); });

            civCategory.AddItem("Task Wander", "Task yourself to wander", () =>
            {
                if (Game.PlayerPed.IsInVehicle()) Game.PlayerPed.TaskDriveWander();
                else Game.PlayerPed.Task.WanderAround();
            });

            civCategory.AddItem("Task Go To Waypoint", "Task yourself to go to the waypoint", () =>
            {
                var waypoint = World.GetWaypointBlip();

                if (waypoint == null)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("No Waypoint", "error",
                        "You must have a waypoint set to use this.", new NewNotificationMessageContent[0]));
                    return;
                }

                if (Game.PlayerPed.IsInVehicle())
                    Game.PlayerPed.Task.DriveTo(Game.PlayerPed.CurrentVehicle, waypoint.Position, 10f, 15f,
                        (int)DrivingStyle.Normal);
                else Game.PlayerPed.Task.GoTo(waypoint.Position);
            });

            if (currentUserRole.Branch == UserBranch.Civ || UserAces.IsAdmin || UserAces.IsDeveloper)
                civCategory.Finish(_toolboxMenu);

            #endregion civ menu

            #region civ vehicle menu

            if (Game.PlayerPed.CurrentVehicle != null) {
                var civVehicleCategory = new ToolboxCategory("Civ Vehicle Menu", "Options for Civilians in Vehicles");

                var vehicleInfo = await _vehicleInfo.GetByNetworkId(Game.PlayerPed.CurrentVehicle.NetworkId,
                    Game.PlayerPed.CurrentVehicle.GetPlateText());

                var taxExpiredItem = new MenuCheckboxItem("Tax Expired", vehicleInfo.IsTaxExpired)
                {
                    ItemData = "Tax Expired",
                    Checked = vehicleInfo.IsTaxExpired
                };

                var motExpiredItem = new MenuCheckboxItem("MOT Expired", vehicleInfo.IsMotExpired)
                {
                    ItemData = "MOT Expired",
                    Checked = vehicleInfo.IsMotExpired
                };

                var insuranceExpiredItem = new MenuCheckboxItem("Insurance Expired", vehicleInfo.IsInsuranceExpired)
                {
                    ItemData = "Insurance Expired",
                    Checked = vehicleInfo.IsInsuranceExpired
                };

                var drugsMarkerItem = new MenuCheckboxItem("Drugs Marker", vehicleInfo.HasMarker("Drugs Intel"))
                {
                    ItemData = "Drugs Intel"
                };

                var weaponsMarkerItem = new MenuCheckboxItem("Weapons Marker", vehicleInfo.HasMarker("Weapons Intel"))
                {
                    ItemData = "Weapons Intel"
                };

                var ftsMarkerItem = new MenuCheckboxItem("FTS Marker", vehicleInfo.HasMarker("Fail to Stop"))
                {
                    ItemData = "Fail to Stop"
                };

                var outstandingItem = new MenuCheckboxItem("Outstanding Crime", vehicleInfo.HasMarker("Outstanding Crime"))
                {
                    ItemData = "Outstanding Crime"
                };

                var stolenItem = new MenuCheckboxItem("Stolen Marker", vehicleInfo.HasMarker("Stolen"))
                {
                    ItemData = "Stolen"
                };

                var writtenOffItem = new MenuCheckboxItem("Written Off Marker", vehicleInfo.HasMarker("Written Off"))
                {
                    ItemData = "Written off"
                };

                var scrappedItem = new MenuCheckboxItem("Scrapped Marker", vehicleInfo.HasMarker("Scrapped"))
                {
                    ItemData = "Scrapped"
                };

                var exportedItem = new MenuCheckboxItem("Exported Marker", vehicleInfo.HasMarker("Exported"))
                {
                    ItemData = "Exported"
                };

                civVehicleCategory.Menu.OnCheckboxChange += async (menu, item, index, state) =>
                {
                    if (item == taxExpiredItem || item == motExpiredItem || item == insuranceExpiredItem)
                    {
                        _logger.Debug(
                            $"Marker State: {taxExpiredItem.Checked.ToString()}, {motExpiredItem.Checked.ToString()}, {insuranceExpiredItem.Checked.ToString()}");
                        var trySave = await _vehicleInfo.UpdateExpiredMarkers(vehicleInfo.NetworkId,
                            vehicleInfo.Plate, taxExpiredItem.Checked, motExpiredItem.Checked,
                            insuranceExpiredItem.Checked);
                        _logger.Debug($"Saved Expired Markers? {trySave.ToString()}");
                        return;
                    }

                    _logger.Debug($"Set Vehicle Marker {item.ItemData} to {state.ToString()}");
                    if (state) vehicleInfo.Markers.Add(item.ItemData);
                    else vehicleInfo.Markers.Remove(item.ItemData);
                };

                civVehicleCategory.AddDirectItem(taxExpiredItem, () => { });
                civVehicleCategory.AddDirectItem(motExpiredItem, () => { });
                civVehicleCategory.AddDirectItem(insuranceExpiredItem, () => { });
                civVehicleCategory.AddDirectItem(drugsMarkerItem, () => { });
                civVehicleCategory.AddDirectItem(weaponsMarkerItem, () => { });
                civVehicleCategory.AddDirectItem(ftsMarkerItem, () => { });
                civVehicleCategory.AddDirectItem(outstandingItem, () => { });
                civVehicleCategory.AddDirectItem(stolenItem, () => { });
                civVehicleCategory.AddDirectItem(writtenOffItem, () => { });
                civVehicleCategory.AddDirectItem(scrappedItem, () => { });
                civVehicleCategory.AddDirectItem(exportedItem, () => { });

                civVehicleCategory.AddDirectItem(new MenuItem("Save Markers") { LeftIcon = MenuItem.Icon.TICK }, async () =>
                {
                    var vehicleMarkers = vehicleInfo.Markers;
                    var trySave =
                        await _vehicleInfo.UpdateMarkers(vehicleInfo.NetworkId, vehicleInfo.Plate, vehicleMarkers);
                    _logger.Debug($"Saved Vehicle Markers? {trySave.ToString()}");
                });

                if (currentUserRole.Branch == UserBranch.Civ || UserAces.IsAdmin || UserAces.IsDeveloper)
                    civVehicleCategory.Finish(_toolboxMenu);
            }

            #endregion civ vehicle menu

            #region cordon menu
            var cordonCategory = new ToolboxCategory("Cordon Menu", "Mark a Cordon For Officers");

            cordonCategory.AddItem("50m No Entry Cordon", "Mark a 50m No Entry Cordon", () => API.ExecuteCommand("cordonarea50"));
            cordonCategory.AddItem("100m No Entry Cordon", "Mark a 100m No Entry Cordon", () => API.ExecuteCommand("cordonarea100"));
            cordonCategory.AddItem("150m No Entry Cordon", "Mark a 150m No Entry Cordon", () => API.ExecuteCommand("cordonarea150"));

            if (currentUserRole.Division == UserDivision.hart || currentUserRole.Division == UserDivision.FRU || currentUserRole.Division == UserDivision.Afo || UserAces.IsBandTwo || UserAces.IsBandThree || UserAces.IsBandFour || UserAces.IsModerator || UserAces.IsDeveloper)
                cordonCategory.Finish(_toolboxMenu);
            #endregion cordon menu

            #region tubespeaker menu
            var tubeCategory = new ToolboxCategory("Tube PA Menu", "PA for ongoing incidents");

            tubeCategory.AddItem("Emergency Announcement", "Passengers to leave station", () => API.ExecuteCommand("+tubefire"));
            tubeCategory.AddItem("Tube Station Closed", "Ongoing Police Incident", () => API.ExecuteCommand("+armedevac"));

            if (UserAces.IsBandTwo || UserAces.IsBandThree || UserAces.IsBandFour || UserAces.IsModerator || UserAces.IsDeveloper)
                tubeCategory.Finish(_toolboxMenu);
            #endregion tubespeaker menu

            #region explosive menu

            var explosivesCategory = new ToolboxCategory("Explosives Menu", "Explosives Control Menu");

            if (currentUserRole.Branch == UserBranch.Civ && UserAces.IsSeniorCiv || UserAces.IsAdmin || UserAces.IsDeveloper)
                explosivesCategory.AddItem("Plant Explosive", "Plant an Explosive", () => API.ExecuteCommand("+xbplant"));

            if (currentUserRole.Branch == UserBranch.Civ && UserAces.IsSeniorCiv || currentUserRole.Division == UserDivision.Afo || UserAces.IsAdmin || UserAces.IsDeveloper)
                explosivesCategory.AddItem("Defuse Explosive", "Defuse an Explosive", () => API.ExecuteCommand("+xbdefuse"));

            if (currentUserRole.Branch == UserBranch.Civ && UserAces.IsSeniorCiv || UserAces.IsAdmin || UserAces.IsDeveloper)
                explosivesCategory.AddItem("Freeze Explosive", "Freeze explosive timer", () => API.ExecuteCommand("+xbfreeze"));

            if (currentUserRole.Branch == UserBranch.Civ && UserAces.IsSeniorCiv || UserAces.IsAdmin || UserAces.IsDeveloper)
                explosivesCategory.AddItem("Remote Detonate", "Detonate an explosive remotely", () => API.ExecuteCommand("+xbremote"));

            if (currentUserRole.Branch == UserBranch.Civ && UserAces.IsSeniorCiv || UserAces.IsAdmin || UserAces.IsDeveloper)
                explosivesCategory.AddItem("Add Timer (5mins)", "Add 5 minutes to explosive", () => API.ExecuteCommand("+xbtimer 300"));

            if (currentUserRole.Branch == UserBranch.Civ && UserAces.IsSeniorCiv || UserAces.IsAdmin || UserAces.IsDeveloper)
                explosivesCategory.AddItem("Add Timer (15mins)", "Add 15 minutes to explosive", () => API.ExecuteCommand("+xbtimer 900"));

            if (currentUserRole.Branch == UserBranch.Civ && UserAces.IsSeniorCiv || currentUserRole.Division == UserDivision.Afo || UserAces.IsAdmin || UserAces.IsDeveloper)
                explosivesCategory.Finish(_toolboxMenu);

            #endregion explosive menu
            
            #region IR Strobe
            
            var strobeCategory = new ToolboxCategory("IR Strobe Menu", "Infrared Strobe lights Menu");
            
            if (currentUserRole.Division == UserDivision.Afo || UserAces.IsAdmin || UserAces.IsDeveloper)
                strobeCategory.AddItem("Deactivate Strobe Lights", "Switch off strobe lights", () => API.ExecuteCommand("+xstrobe"));
            
            if (currentUserRole.Division == UserDivision.Afo || UserAces.IsAdmin || UserAces.IsDeveloper)
                strobeCategory.AddItem("Activate Strobe (Red)", "Switch on RED strobe lights", () => API.ExecuteCommand("+xstrobe red"));
            
            if (currentUserRole.Division == UserDivision.Afo || UserAces.IsAdmin || UserAces.IsDeveloper)
                strobeCategory.AddItem("Activate Strobe (Green)", "Switch on GREEN strobe lights", () => API.ExecuteCommand("+xstrobe green"));
            
            if (currentUserRole.Division == UserDivision.Afo || UserAces.IsAdmin || UserAces.IsDeveloper)
                strobeCategory.AddItem("Activate Strobe (Blue)", "Switch on BLUE strobe lights", () => API.ExecuteCommand("+xstrobe blue"));
            
            if (currentUserRole.Division == UserDivision.Afo || UserAces.IsAdmin || UserAces.IsDeveloper)
                strobeCategory.AddItem("Activate Strobe (Violet)", "Switch on VIOLET strobe lights", () => API.ExecuteCommand("+xstrobe purple"));
            
            if (currentUserRole.Division == UserDivision.Afo || UserAces.IsAdmin || UserAces.IsDeveloper)
                strobeCategory.Finish(_toolboxMenu);
            
            #endregion

            #region other options
            var otherOptionsCategory = new ToolboxCategory("Other", "Other options");

            otherOptionsCategory.AddItem("Toggle Who's Talking", "Toggle indicator at top of screen", () => API.ExecuteCommand("radiochatdisplay"));
            otherOptionsCategory.AddItem("View Discord Link", "View the discord invite link", DiscordLink);

            if (currentUserRole.Branch == UserBranch.Control || UserAces.IsAdmin || UserAces.IsDeveloper)
                otherOptionsCategory.AddItem("Control Status Notifications", "Notify units of current control status", () => API.ExecuteCommand("controlnotify"));

            if (currentUserRole.Branch == UserBranch.Fire || UserAces.IsAdmin || UserAces.IsDeveloper)
                otherOptionsCategory.AddItem("LFRS PASS Alarm", "Open the LFRS PASS Alarm menu", () => API.ExecuteCommand("passmenu"));

            if (UserAces.IsDeveloper || UserAces.IsTierTwo || UserAces.IsAdmin)
                otherOptionsCategory.AddItem("Developer Restricted", "Some developer restricted features :)", ShowDevRestrictedMenu);

            if (_featureService.IsFeatureEnabled(FeatureToggle.AICallouts))
                otherOptionsCategory.AddItem("Callouts Menu", "Open the callouts menu", ShowCalloutsMenu);

            if (currentUserRole.Division == UserDivision.Npas || UserAces.IsAdmin || UserAces.IsDeveloper)
                otherOptionsCategory.AddItem("HeliHUD Settings", "Adjust your HeliHUD Preferences", () => API.ExecuteCommand("helihud"));
            
            if (currentUserRole.Branch == UserBranch.Nhs || UserAces.IsAdmin || UserAces.IsDeveloper)
                otherOptionsCategory.AddItem("Box Ambulance Request", "Request an AI box ambulance", () => API.ExecuteCommand("+xambulance"));
            
            if (currentUserRole.Branch == UserBranch.Nhs || UserAces.IsAdmin || UserAces.IsDeveloper)
                otherOptionsCategory.AddItem("Hospital Patient Check-In", "Check-In Patient to Hospital beds", () => API.ExecuteCommand("+xhcheckin"));

            otherOptionsCategory.Finish(_toolboxMenu);
            #endregion other options

            _toolboxMenu.OpenMenu();
        }

        private void ShowCalloutsMenu()
        {
            _toolboxMenu.CloseMenu();
            var calloutsMenu = new Menu("Callouts Menu", "");
            MenuController.AddMenu(calloutsMenu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            var calloutsClockOn = new MenuItem("Clock On", "Receive callouts");
            var calloutsClockOff = new MenuItem("Clock Off", "Clock off callouts");
            var acceptRecentCallout = new MenuItem("Accept Recent Callout", "Accept the last callout");
            var detachCallout = new MenuItem("Detach Callout", "Detach from your current callout");
            var calloutInfo = new MenuItem("Callout Details", "See details on your current callout");

            calloutsMenu.AddMenuItem(_isClocked ? calloutsClockOff : calloutsClockOn);
            calloutsMenu.AddMenuItem(acceptRecentCallout);
            calloutsMenu.AddMenuItem(detachCallout);
            calloutsMenu.AddMenuItem(calloutInfo);

            calloutsMenu.OnItemSelect += async (_, item, _) =>
            {
                if (item == calloutsClockOn)
                    API.ExecuteCommand("calloutclockon");
                if (item == calloutsClockOff)
                    API.ExecuteCommand("calloutclockoff");
                if (item == acceptRecentCallout)
                    API.ExecuteCommand("acceptrecentcallout");
                if (item == detachCallout)
                    API.ExecuteCommand("detachcallout");
                if (item == calloutInfo)
                    API.ExecuteCommand("calloutinfo");
                await BaseScript.Delay(100);
                calloutsMenu.CloseMenu();
                ShowCalloutsMenu();
            };

            calloutsMenu.OpenMenu();
        }

        private void ShowDevRestrictedMenu()
        {
            _toolboxMenu.CloseMenu();
            var devRestrictedMenu = new Menu("Dev Restricted", "Developer restricted features");
            MenuController.AddMenu(devRestrictedMenu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            var behaviorDebug = new MenuItem("Behaviors", "Append behaviors on yourself");
            var civWeaponsMenu = new MenuItem("Civ Weapons Menu", "Open the civ weapons menu");

            devRestrictedMenu.AddMenuItem(behaviorDebug);
            devRestrictedMenu.AddMenuItem(civWeaponsMenu);

            devRestrictedMenu.OnItemSelect += (_, item, _) =>
            {
                devRestrictedMenu.CloseMenu();
                if (item == behaviorDebug)
                    ShowBehaviorsDebugMenu();
                if (item == civWeaponsMenu)
                    _civArmoury.ShowCivArmouryMenu();
            };

            devRestrictedMenu.OpenMenu();
        }

        private void ShowManualMenu()
        {
            _toolboxMenu.CloseMenu();
            var manualTransmissionMenu = new Menu("Manual Gear", "Toggle manual transmission mode");
            MenuController.AddMenu(manualTransmissionMenu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            var manualOn = new MenuItem("Turn On", "Turn on manual transmission mode. Scroll wheel controls the clutch, left click to downshift, right click to upshift.");
            var manualOff = new MenuItem("Turn Off", "Turn off manual transmission mode");

            manualTransmissionMenu.AddMenuItem(manualOn);
            manualTransmissionMenu.AddMenuItem(manualOff);

            manualTransmissionMenu.OnItemSelect += (_, item, _) =>
            {
                manualTransmissionMenu.CloseMenu();
                if (item == manualOn)
                    _legacyComms.ToClient(ClientEvents.SetManualCarMode, true);
                if (item == manualOff)
                    _legacyComms.ToClient(ClientEvents.SetManualCarMode, false);
            };

            manualTransmissionMenu.OpenMenu();
        }
        
        private void ShowMatrixBoardMenu(UserRole currentUserRole)

        {
            _toolboxMenu.CloseMenu();
            var matrixMenu = new Menu("Matrix Board", "Select a matrix board display");
            MenuController.AddMenu(matrixMenu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            // Clear option
            var clearItem = new MenuItem("CLEAR MATRIX", "Turn off the matrix board display.");
            matrixMenu.AddMenuItem(clearItem);

            // Matrix options
            string[] allMatrixOptions = new string[]
            {
                "police_stop", "police_keep_back", "lane_closed", "thank_you",
                "use_hard_shoulder", "police", "pull_over", "follow_me",
                "do_not_pass", "road_closed", "accident_slow", "police_accident",
                "right_arrow", "left_arrow"
            };

            string[] hetoAllowedOptions = new string[]
            {
                "lane_closed", "thank_you",
                "use_hard_shoulder", "follow_me",
                "do_not_pass", "road_closed",
                "right_arrow", "left_arrow"
            };

            string[] matrixOptions = currentUserRole.Branch == UserBranch.Highways
                ? hetoAllowedOptions
                : allMatrixOptions;

            // Add matrix command items
            foreach (var option in matrixOptions)
            {
                string label = option.Replace('_', ' ').ToUpperInvariant();
                matrixMenu.AddMenuItem(new MenuItem(label, $"Display the '{option}' matrix message."));
            }

            // Handle selection
            matrixMenu.OnItemSelect += (_, item, index) =>
            {
                matrixMenu.CloseMenu();

                if (index == 0)
                {
                    // Clear Matrix
                    API.ExecuteCommand("matrixClear");
                }
                else
                {
                    // Adjust for the clear item at index 0
                    string selectedCommand = $"matrixSet {matrixOptions[index - 1]}";
                    API.ExecuteCommand(selectedCommand);
                }
            };

            matrixMenu.OpenMenu();
        }
        private void ShowBodyCamMenu()
        {
            _toolboxMenu.CloseMenu();
            var bodyCamMenu = new Menu("Body Cam Menu", "Body Cam Options");
            MenuController.AddMenu(bodyCamMenu);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            var currentUserRole = _permissionService.CurrentUserRole;

            var recordFootage = new MenuItem("Bodycam On/Off", "Manually Toggle On/Off Bodycam");
            bodyCamMenu.AddMenuItem(recordFootage);

            var autoBodycam = new MenuItem("Automatic Recording", "Enable or disable automatic recording when firing weapon");
            bodyCamMenu.AddMenuItem(autoBodycam);

            if ((currentUserRole.Division == UserDivision.Afo && UserAces.IsChiefInspector) || UserAces.IsAdmin || UserAces.IsDeveloper)
            {
                var viewRecordings = new MenuItem("Bodycam Recordings", "Access saved bodycam footage");
                bodyCamMenu.AddMenuItem(viewRecordings);
            }

            bodyCamMenu.OnItemSelect += async (_, item, _) =>
            {
                if (item.Text == "Bodycam Recordings")
                {
                    API.ExecuteCommand("+zrecords");
                }
                else if (item.Text == "Bodycam On/Off")
                {
                    API.ExecuteCommand("+zbodycam");
                }
                else if (item.Text == "Automatic Recording")
                {
                    API.ExecuteCommand("autobodycam");
                }

                await BaseScript.Delay(100);
                bodyCamMenu.CloseMenu();
                ShowBodyCamMenu();
            };

            bodyCamMenu.OpenMenu();
        }

        private bool GiveVehicleFixOption(UserRole currentUserRole)
        {
            if (currentUserRole.Branch != UserBranch.Civ)
            {
                return false;
            }

            var currentVehicle = Game.PlayerPed.CurrentVehicle;
            if (currentVehicle == null) return false;

            var isDriver = currentVehicle.Driver == Game.PlayerPed;
            if (!isDriver) return false;

            var vehiclePosition = currentVehicle.Position;
            var nearestObject = 0;

            foreach (var fuelPump in _fuelPumps)
            {
                if (nearestObject != 0) break;
                nearestObject = API.GetClosestObjectOfType(vehiclePosition.X, vehiclePosition.Y, vehiclePosition.Z, 5f,
                    fuelPump, false, true, true);
            }

            var isNearPump = nearestObject > 0;
            if (!isNearPump) return false;

            return true;
        }

        private void ShowCustomCharacterSelection()
        {
            _toolboxMenu.CloseMenu();
            var characterMenu = new Menu("Characters", "Select an option");
            MenuController.AddMenu(characterMenu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            var customCharacters = _customCharacterService.FetchCustomCharacters();

            var addCharacterMale = new MenuItem("Create Male Character");
            var addCharacterFemale = new MenuItem("Create Female Character");

            foreach (var customCharacter in customCharacters)
            {
                var characterItem = new MenuItem(customCharacter.Name)
                {
                    ItemData = customCharacter.Appearance
                };

                characterMenu.AddMenuItem(characterItem);
            }

            if (UserAces.IsDeveloper || UserAces.IsAdmin)
            {
                characterMenu.AddMenuItem(addCharacterMale);
                characterMenu.AddMenuItem(addCharacterFemale);
            }
            else if (customCharacters.Count < 3 && UserAces.IsProDonator)
            {
                characterMenu.AddMenuItem(addCharacterMale);
                characterMenu.AddMenuItem(addCharacterFemale);
            }

            characterMenu.OnItemSelect += (menu, item, index) =>
            {
                if (menu != characterMenu) return;

                characterMenu.CloseMenu();

                if (item == addCharacterMale)
                {
                    _customCharacterService.ShowCharacterCreator(0);
                    return;
                }
                if (item == addCharacterFemale)
                {
                    _customCharacterService.ShowCharacterCreator(1);
                    return;
                }

                var characterSubMenu = new Menu(item.Text);
                var selectItem = new MenuItem("Select");
                var deleteItem = new MenuItem("Delete")
                {
                    LeftIcon = MenuItem.Icon.WARNING
                };
                var saveItem = new MenuItem("Save Current Outfit");
                characterSubMenu.AddMenuItem(selectItem);
                characterSubMenu.AddMenuItem(saveItem);
                characterSubMenu.AddMenuItem(deleteItem);

                characterSubMenu.OnItemSelect += (subMenu, subMenuItem, subIndex) =>
                {
                    characterSubMenu.CloseMenu();
                    if (subMenuItem == deleteItem) _customCharacterService.DeleteCustomCharacter(item.Text, item.ItemData);
                    if (subMenuItem == selectItem) _customCharacterService.SetCharacterAppearance(item.Text, item.ItemData);
                    if (subMenuItem == saveItem) _customCharacterService.SaveOutfitToCharacter(item.Text, item.ItemData);
                };

                MenuController.AddMenu(characterSubMenu);
                characterSubMenu.OpenMenu();
            };

            characterMenu.OpenMenu();
        }

        private void ShowColorVehicleMenu()
        {
            _toolboxMenu.CloseMenu();
            var colorMenu = new Menu("Vehicle Colour", "Select a colour");
            MenuController.AddMenu(colorMenu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            var randomColor = new MenuItem("Random Colour");
            colorMenu.AddMenuItem(randomColor);

            foreach (var color in _colorNames)
            {
                var addColor = false;
                var index = _colorNames.FindIndex(s => s.EqualsIgnoreCase(color));
                var colorId = _colorValues[index];
                switch (_permissionService.CurrentUserRole.Branch)
                {
                    case UserBranch.Police or UserBranch.Fire or UserBranch.Nhs:
                    {
                        if (_allowedEmergencyColours.Contains(colorId))
                            addColor = true;
                        break;
                    }
                    case UserBranch.Civ:
                    {
                        if (_allowedCivColours.Contains(colorId))
                            addColor = true;
                        break;
                    }
                }
                if (addColor) colorMenu.AddMenuItem(new MenuItem(color));
            }

            colorMenu.OnItemSelect += (subMenu, subMenuItem, subIndex) =>
            {
                if (subMenuItem == randomColor)
                    ColorVehicle();
                else
                    ColorVehicle(subMenuItem.Text);
            };
            colorMenu.OpenMenu();
        }

        private void ShowBehaviorsDebugMenu()
        {
            _toolboxMenu.CloseMenu();
            var behaviorsMenu = new Menu("Behaviors", "Select a behavior to activate on yourself");
            MenuController.AddMenu(behaviorsMenu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            var clearBehaviors = new MenuItem("Clear Behaviors & Tasks");

            var drunkBehavior = new MenuItem("Drunk");
            var misperBehavior = new MenuItem("Misper");
            var fighterBehavior = new MenuItem("Fighter");
            var drugDealerBehavior = new MenuItem("Drug Dealer");
            var aggressiveDrunkBehavior = new MenuItem("Aggressive Drunk");
            var burningInFireBehavior = new MenuItem("Burning in Fire");
            var strandedBehavior = new MenuItem("Stranded");
            var filmingShockedBehavior = new MenuItem("Filming Shocked");
            var paparazziBehavior = new MenuItem("Paparazzi");

            behaviorsMenu.AddMenuItem(clearBehaviors);

            behaviorsMenu.AddMenuItem(drunkBehavior);

            behaviorsMenu.AddMenuItem(aggressiveDrunkBehavior);

            behaviorsMenu.AddMenuItem(misperBehavior);

            behaviorsMenu.AddMenuItem(fighterBehavior);

            behaviorsMenu.AddMenuItem(drugDealerBehavior);

            behaviorsMenu.AddMenuItem(burningInFireBehavior);

            behaviorsMenu.AddMenuItem(strandedBehavior);

            behaviorsMenu.AddMenuItem(filmingShockedBehavior);

            behaviorsMenu.AddMenuItem(paparazziBehavior);

            behaviorsMenu.OnItemSelect += async (_, subMenuItem, _) =>
            {
                if (subMenuItem == clearBehaviors)
                {
                    _behaviors.RemovePedBehaviors(Game.PlayerPed);
                    Game.PlayerPed.Task.ClearAllImmediately();
                } else if (subMenuItem == drunkBehavior)
                {
                    _behaviors.SetPedBehavior<DrunkBehavior>(Game.PlayerPed);
                }
                else if (subMenuItem == misperBehavior)
                {
                    _behaviors.SetPedBehavior<MisperBehavior>(Game.PlayerPed);
                }
                else if (subMenuItem == fighterBehavior)
                {
                    _behaviors.SetPedBehavior<FighterBehavior>(Game.PlayerPed);
                }
                else if (subMenuItem == drugDealerBehavior)
                {
                    _behaviors.SetPedBehavior<DrugDealerBehavior>(Game.PlayerPed);
                }
                else if (subMenuItem == aggressiveDrunkBehavior)
                {
                    var drunkBehaviorBlackboard = _behaviors.SetPedBehavior<DrunkBehavior>(Game.PlayerPed);
                    drunkBehaviorBlackboard.Set(bb=>bb.WillBecomeAggressive, true);
                }
                else if (subMenuItem == burningInFireBehavior)
                {
                    _behaviors.SetPedBehavior<BurningInFireBehavior>(Game.PlayerPed);
                }
                else if (subMenuItem == strandedBehavior)
                {
                    _behaviors.SetPedBehavior<StrandedBehavior>(Game.PlayerPed);
                }
                else if (subMenuItem == filmingShockedBehavior)
                {
                    _behaviors.SetPedBehavior<FilmingShockedBehavior>(Game.PlayerPed);
                }
                else if (subMenuItem == paparazziBehavior)
                {
                    var paparazziBlackboard = _behaviors.SetPedBehavior<PlayScenarioBehavior>(Game.PlayerPed);
                    paparazziBlackboard.Set(bb => bb.ScenarioName, "WORLD_HUMAN_PAPARAZZI");
                    paparazziBlackboard.Set(bb => bb.Speeches, new List<string>() { "I'm a paparazzi!" });
                }
            };

            behaviorsMenu.OpenMenu();
        }

       private void ShowAutopilotMenu()
       {
           _toolboxMenu.CloseMenu();
           var autopilotMenu = new Menu("Autopilot", "Select an option");
           MenuController.AddMenu(autopilotMenu);
           MenuController.EnableMenuToggleKeyOnController = false;
           MenuController.MenuToggleKey = (Control)(-1);
           MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

           var autopilotOn = new MenuItem("Engage Full Self Driving") { Description = "Human supervision is REQUIRED! You are responsible if you CRASH with it! Press [H] to toggle debug mode. Scroll to change speed." };
           var autocorrectOn = new MenuItem("Engage Collision Warning & Prevention") { Description = "It will apply corrective brakes if you are about to crash into another car. Press [H] to toggle debug mode." };
           var off = new MenuItem("Disengage") { Description = "Intervention of gas/brakes will disengage autopilot automatically." };

           autopilotMenu.AddMenuItem(autopilotOn);
           autopilotMenu.AddMenuItem(autocorrectOn);
           autopilotMenu.AddMenuItem(off);

           autopilotMenu.OnItemSelect += async (_, subMenuItem, _) =>
           {
               if (subMenuItem == autopilotOn)
                   _legacyComms.ToClient(ClientEvents.AutopilotState, true, true);
               else if (subMenuItem == autocorrectOn)
                   _legacyComms.ToClient(ClientEvents.AutopilotState, true, false);
               else if (subMenuItem == off)
                   _legacyComms.ToClient(ClientEvents.AutopilotState, false, false);
           };
           autopilotMenu.OpenMenu();
       }

        private async void UseDisplayOutfitsCommand()
        {
            _toolboxMenu.CloseMenu();
            await Delay(0);
            API.ExecuteCommand("outfitsmenu");
            _logger.Debug("Outfits Menu has been opened via the Toolbox!");
        }

        [Command("fix")]
        private async void FixVehicle()
        {
            var currentVehicle = Game.PlayerPed.CurrentVehicle;
            if (currentVehicle == null) return;
            if (currentVehicle.Driver != Game.PlayerPed) return;

            API.DecorSetBool(currentVehicle.Handle, "WindowSmashed", false); // For EMG Hammer script

            var fuelBeforeFix = currentVehicle.FuelLevel;

            if (FixingVehicle != null)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Repair", "error", "You are already fixing your vehicle!", new NewNotificationMessageContent[0]));
                return;
            }

            if (UserAces.IsAdmin || UserAces.IsDeveloper || UserAces.IsTierTwo || UserAces.IsProDonator || UserAces.IsDevFunNight || UserAces.IsCivTrained)
            {
                FixingVehicle = currentVehicle;

                _legacyComms.ToClient(ClientEvents.IsFixingVehicle, true);

                currentVehicle.IsHandbrakeForcedOn = true;
                currentVehicle.IsDriveable = false;

                if (currentVehicle.Doors.HasDoor(VehicleDoorIndex.Trunk))
                    currentVehicle.Doors[VehicleDoorIndex.Hood].Open();
            }

            var fixTime = 60000;

            if (UserAces.IsAdmin || UserAces.IsDeveloper || UserAces.IsTierTwo) fixTime = 0;

            if (fixTime != 0)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Repair", "info", $"Fixing your vehicle. This will take {fixTime / 1000} seconds!", new NewNotificationMessageContent[0], fixTime / 1000));
                await Script.Delay(fixTime);
            }

            _legacyComms.ToClient(ClientEvents.IsFixingVehicle, false);
            FixingVehicle = null;

            if (!currentVehicle.Exists()) return;

            var vehFuel = UserAces.IsTierTwo ? 100f : currentVehicle.FuelLevel;

            currentVehicle.IsHandbrakeForcedOn = false;
            currentVehicle.IsDriveable = true;

            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Repair", "info", "Your vehicle has been fixed!", new NewNotificationMessageContent[0]));

            currentVehicle.Doors[VehicleDoorIndex.Hood].Close();

            currentVehicle.Repair();
            currentVehicle.Wash();

            currentVehicle.FuelLevel = vehFuel;
        }

        [Command("topd")]
        private void GotoPd()
        {
            API.ExecuteCommand("fecctvc"); // Forces CCTV Cams off and resets changes from cctv cams
            API.ExecuteCommand("bcmocr"); // Cancels Blood DLC Missions

            var currentVehicle = Game.PlayerPed.CurrentVehicle;

            if (currentVehicle != null && currentVehicle.Driver == Game.PlayerPed)
            {
                currentVehicle.Delete();
            }

            /*
            API.DoScreenFadeOut(100);
            API.NetworkFadeOutEntity(Game.PlayerPed.Handle, true, false);

            var positionZero = SpawnScript.LastSpawnPosition == Vector3.Zero;
            var spawnPosition = positionZero ? new Vector3(427, -980, 30) : SpawnScript.LastSpawnPosition;
            API.SetEntityCoords(Game.PlayerPed.Handle, spawnPosition.X, spawnPosition.Y, spawnPosition.Z, false, false, false, false);

            API.NetworkFadeInEntity(Game.PlayerPed.Handle, true);
            API.DoScreenFadeIn(100);*/
            _spawnScript.CreateSpawnSelectionMenu(false);
            return;
        }

        private void DiscordLink()
        {
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("PoliceMP Discord", "info", "Head over to our discord @ https://discord.gg/policemp", new NewNotificationMessageContent[0]));
        }

        private void AfoGearUp()
        {
            _toolboxMenu.CloseMenu();
            _armoury.ReloadArmouryMenu();
        }

        private async Task ColorVehicle(string colorString = "random")
        {
            var currentVehicle = Game.PlayerPed.CurrentVehicle;
            if (currentVehicle == null) return;

            if (currentVehicle.Driver != Game.PlayerPed) return;

            var currentUserRole = _permissionService.CurrentUserRole;

            var color = 0;
            var isVerified = false;
            if (colorString == "random")
            {
                var rand = new Random();
                switch (currentUserRole.Branch)
                {
                    case UserBranch.Civ:
                        var rc = rand.Next(0, _allowedCivColours.Length);
                        color = _allowedCivColours[rc];
                        break;
                    case UserBranch.Police:
                        var r = rand.Next(0, _allowedEmergencyColours.Length);
                        color = _allowedEmergencyColours[r];
                        break;
                    case UserBranch.Nhs:
                        var random1 = rand.Next(0, _allowedEmergencyColours.Length);
                        color = _allowedEmergencyColours[random1];
                        break;
                    case UserBranch.Fire:
                        var random2 = rand.Next(0, _allowedEmergencyColours.Length);
                        color = _allowedEmergencyColours[random2];
                        break;
                    default:
                        return;
                }

                isVerified = true;
            }
            else
            {
                var index = _colorNames.FindIndex(s => s.EqualsIgnoreCase(colorString));
                if (index == -1)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Colour", "error", "This colour wasn't found!", new NewNotificationMessageContent[0]));
                }
                color = _colorValues[index];
                switch (currentUserRole.Branch)
                {
                    case UserBranch.Civ:
                        if (_allowedCivColours.Contains(color)) isVerified = true;
                        break;
                    case UserBranch.Police or UserBranch.Nhs or UserBranch.Fire:
                        if (_allowedEmergencyColours.Contains(color)) isVerified = true;
                        break;
                    default:
                        return;
                }
            }

            await Delay(0);
            if (isVerified)
                API.SetVehicleColours(Game.PlayerPed.CurrentVehicle.Handle, color, color);
            else _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Colour", "error", "You cannot use this colour in your current branch!", new NewNotificationMessageContent[0]));
        }
    }
}
