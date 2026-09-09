using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Actions.Breathalyse;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.Commands;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Enums;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.Decors;

namespace PoliceMP.Client.Scripts.ELS
{
    public enum ElsSirenType
    {
        Disabled,
        Police,
        Nhs,
        Fire,
        Bike,
        PoliceGamma,
        Lfb2,
        Police2,
        NHSdev,
        MatrixPolice,
        MatrixNHS,
        MatrixLFB,
        Bike2,
        Gamma2,
        PoliceB2,
    }

    public class Els : Script, IELS
    {
        private readonly ILogger<Els> _logger;
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ICommandManager _commands;
        private readonly IPermissionService _useraces;
        private readonly ISoundService _soundService;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IFeatureService _featureService;
        private readonly IGameInputManager _input;

        private const int FlashTime = 50;
        private const int SwitchTime = 125;

        private readonly Dictionary<int, Task> _mainElsTasks = new();
        private readonly Dictionary<int, Task> _onSceneElsTasks = new();
        private readonly Dictionary<int, Task> _nhsSteadyBurnerTask = new();
        private readonly Dictionary<int, Task> _redElsTasks = new();
        private readonly Dictionary<int, Task> _warnElsTasks = new();
        private readonly Dictionary<int, Task> _takeDownTasks = new();

        private bool flashingHeadlights = false;
        private bool flashingEnabled = false;

        //NOTE: Siren type codes from decors will be:
        // 0 - Siren disabled
        // 1 - Siren popo
        // 2 - Siren NHS
        // 3 - Siren Fire
        // 4 -
        // 5 -
        // 6 -
        // 7 - Old Police (JAM PACK)
        // 8 - NHS Dev
        // 9 - MATRIX Police Extra flashy
        // 10 - MATRIX LAS Extra flashy
        // 11 - MATRIX LFB Extra flashy

        //NOTE: each type will have 0 as off, then 1-X as relevant sounds

        //ANOTHER NOTE: Structure is "{AudioString}", "{sirenSoundSet}"

        private readonly Dictionary<ElsSirenType, List<string[]>> _sirenMapping =
            new Dictionary<ElsSirenType, List<string[]>>
            {
                //SIREN TYPE CODE 0
                {
                    ElsSirenType.Disabled, new List<string[]>()
                    {
                        new string[] { "", "" }
                    }
                },
                //SIREN TYPE CODE 1 (Met Pollice Cars General) (2024 RSG MCS 32 - Moddex)
                {
                    ElsSirenType.Police, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_lspd_new_siren_adam", "oiss_ssa_vehaud_lspd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_new_siren_boy", "oiss_ssa_vehaud_lspd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_new_siren_charles", "oiss_ssa_vehaud_lspd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_new_siren_david", "oiss_ssa_vehaud_lspd_new_soundset" }
                    }
                },
                //SIREN TYPE CODE 2 (Ambulances General) (Wheelen Gama 1 - 3 tone - Moddex) 
                {
                    ElsSirenType.Nhs, new List<string[]>()
                    { 
                        new string[] { "oiss_ssa_vehaud_lspd_old_siren_adam", "oiss_ssa_vehaud_lspd_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_old_siren_boy", "oiss_ssa_vehaud_lspd_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_old_siren_charles", "oiss_ssa_vehaud_lspd_old_soundset" }
                    }
                },
                //SIREN TYPE CODE 3 (Fire General) (Whelen Alpha 12R  - 295HFS)
                {
                    ElsSirenType.Fire, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_lssd_new_siren_adam", "oiss_ssa_vehaud_lssd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lssd_new_siren_boy", "oiss_ssa_vehaud_lssd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lssd_new_siren_charles", "oiss_ssa_vehaud_lssd_new_soundset" }
                    }
                },
                //SIREN TYPE CODE 4 (Police Bikes Siren Code) (H�nsch Typ 620 Bike Siren + SEG whistle) (Moddex) (tones wrong order)
                {
                    ElsSirenType.Bike, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_lssd_old_siren_adam", "oiss_ssa_vehaud_lssd_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_lssd_old_siren_boy", "oiss_ssa_vehaud_lssd_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_lssd_old_siren_adam", "oiss_ssa_vehaud_lssd_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_lssd_old_siren_boy", "oiss_ssa_vehaud_lssd_old_soundset" }
                    }
                },
                //SIREN TYPE CODE 5 ( Bikes Siren Code) (H�nsch Typ 620 Bike Siren + Phaser) (Moddex) (To Fix)
                {
                    ElsSirenType.PoliceGamma, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_bcso_old_siren_adam", "oiss_ssa_vehaud_bcso_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_bcso_old_siren_boy", "oiss_ssa_vehaud_bcso_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_bcso_old_siren_charles", "oiss_ssa_vehaud_bcso_old_soundset" }
                    }
                },
                //SIREN TYPE CODE 6 (Prime Mover Fire Siren Code) (Air Horn Style) 
                {
                    ElsSirenType.Lfb2, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_bcso_new_siren_adam", "oiss_ssa_vehaud_bcso_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_bcso_new_siren_boy", "oiss_ssa_vehaud_bcso_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_bcso_new_siren_charles", "oiss_ssa_vehaud_bcso_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_bcso_new_siren_david", "oiss_ssa_vehaud_bcso_new_soundset" }
                    }
                },
                //SIREN TYPE CODE 7 (MET OLD) (Stirling SS2H4 - Moddex) 
                {
                    ElsSirenType.Police2, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_sahp_new_siren_adam", "oiss_ssa_vehaud_sahp_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_sahp_new_siren_boy", "oiss_ssa_vehaud_sahp_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_sahp_new_siren_charles", "oiss_ssa_vehaud_sahp_new_soundset" }
                    }
                },
                //SIREN TYPE CODE 8 (Dev NHS)
                {
                    ElsSirenType.NHSdev, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_noose_new_siren_adam", "oiss_ssa_vehaud_noose_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_noose_new_siren_charles", "oiss_ssa_vehaud_noose_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_noose_new_siren_boy", "oiss_ssa_vehaud_noose_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_noose_new_siren_edward", "oiss_ssa_vehaud_noose_new_soundset" }
                    }
                },
                //SIREN TYPE CODE 9 (Met Pollice Cars General)
                {
                    ElsSirenType.MatrixPolice, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_lspd_new_siren_adam", "oiss_ssa_vehaud_lspd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_new_siren_boy", "oiss_ssa_vehaud_lspd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_new_siren_charles", "oiss_ssa_vehaud_lspd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_new_siren_david", "oiss_ssa_vehaud_lspd_new_soundset" }
                    }
                },
                //SIREN TYPE CODE 10 (Ambulances General)
                {
                    ElsSirenType.MatrixNHS, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_lspd_old_siren_adam", "oiss_ssa_vehaud_lspd_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_old_siren_boy", "oiss_ssa_vehaud_lspd_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_lspd_old_siren_charles", "oiss_ssa_vehaud_lspd_old_soundset" }
                    }
                },
                //SIREN TYPE CODE 11 (Fire General) 
                {
                    ElsSirenType.MatrixLFB, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_lssd_new_siren_adam", "oiss_ssa_vehaud_lssd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lssd_new_siren_boy", "oiss_ssa_vehaud_lssd_new_soundset" },
                        new string[] { "oiss_ssa_vehaud_lssd_new_siren_charles", "oiss_ssa_vehaud_lssd_new_soundset" }
                    }
                },
                //SIREN TYPE 12 (Ambualance / Police  - Premier Hazzard 7004) 
                {
                    ElsSirenType.Bike2, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_sahp_old_siren_adam", "oiss_ssa_vehaud_sahp_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_sahp_old_siren_boy", "oiss_ssa_vehaud_sahp_old_soundset" },
                        new string[] { "oiss_ssa_vehaud_sahp_old_siren_charles", "oiss_ssa_vehaud_sahp_old_soundset" }
                    }
                },
                //SIREN TYPE 13 (Ambualance RRV  - Gamma 2 ) 
                {
                    ElsSirenType.Gamma2, new List<string[]>()
                    {
                        new string[] { "oiss_ssa_vehaud_sahp_bike_siren_adam", "oiss_ssa_vehaud_sahp_bike_soundset" },
                        new string[] { "oiss_ssa_vehaud_sahp_bike_siren_boy", "oiss_ssa_vehaud_sahp_bike_soundset" },
                        new string[] { "oiss_ssa_vehaud_sahp_bike_siren_charles", "oiss_ssa_vehaud_sahp_bike_soundset" }
                        // new string[] { "oiss_ssa_vehaud_sahp_bike_siren_david", "oiss_ssa_vehaud_sahp_bike_soundset" }
                    }
                },

            };


        private Dictionary<int, int> _sirenSoundIds = new();

        public Els(ILogger<Els> logger, ITickManager ticks, ILegacyClientCommunicationsManager comms,
            ICommandManager commands, IPermissionService useraces, ISoundService soundService,
            INewNotificationOverlay newNotificationOverlay, IFeatureService featureService, IGameInputManager input)
        {
            _logger = logger;
            _ticks = ticks;
            _comms = comms;
            _commands = commands;
            _useraces = useraces;
            _soundService = soundService;
            _newNotificationOverlay = newNotificationOverlay;
            _featureService = featureService;
            _input = input;

            comms.OnRequest<int, int>(ServerEvents.RequestVehicleSirenType, (netId) =>
            {
                //_logger.Debug($"NETWORK_GET_NETWORK_ID_FROM_ENTITY {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                var vehicle = (Vehicle)Entity.FromNetworkId(netId);
                if (vehicle == null) return Task.FromResult(-1);

                if (!API.DecorExistOn(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE))
                {
                    return Task.FromResult<int>(-1);
                }

                var sirenType = API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE);

                return Task.FromResult(sirenType);
            });
        }

        protected override async Task OnStartAsync()
        {
            RegisterDecor(ELSDecors.LIGHTS_MAIN, DecorType.Bool);
            RegisterDecor(ELSDecors.LIGHTS_RED, DecorType.Bool);
            RegisterDecor(ELSDecors.LIGHTS_WARN, DecorType.Bool);
            RegisterDecor(ELSDecors.ELS_ENABLED, DecorType.Bool);
            RegisterDecor(ELSDecors.SIREN_SOUND_NUMBER, DecorType.Int);
            RegisterDecor(ELSDecors.SIREN_TYPE_CODE, DecorType.Int);
            RegisterDecor(ELSDecors.DECOR_SIREN_ACTIVE, DecorType.Bool);
            RegisterDecor(ELSDecors.DECOR_HAZARD_ACTIVE, DecorType.Bool);
            RegisterDecor(ELSDecors.DECOR_BULLHORN_TYPE, DecorType.Int);
            //RegisterDecor(ELSDecors.DECOR_INDICATOR_LEFT, DecorType.Bool);
            //RegisterDecor(ELSDecors.DECOR_INDICATOR_RIGHT, DecorType.Bool);
            //RegisterDecor(ELSDecors.DECOR_INDICATOR_HAZARD, DecorType.Bool);
            
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSPD_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSPD_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSSD_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSSD_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_BCSO_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_BCSO_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_SAHP_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_SAHP_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_SAHP_BIKE", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_NOOSE_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_NOOSE_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_FIB_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_FIB_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_RHPD_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_RHPD_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_DPPD_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_DPPD_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSIA_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSIA_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSPP_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSPP_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSFD_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSFD_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSCOFD_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_LSCOFD_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_BCFD_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_BCFD_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_SANFIRE_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_SANFIRE_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_SAMS_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_SAMS_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_USFS_NEW", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_USFS_OLD", false);
        API.RequestScriptAudioBank("DLC_SERVERSIDEAUDIO\\OISS_SSA_VEHAUD_ETC", false);

            // https://github.com/fk-1997/Server-Sided-Sounds-and-Sirens/blob/main/IDTABLE.md#table-with-soundbank-soundset-and-audiostring

            _ticks.On(OnTickELSKeyHandlerKeyboard);
            _ticks.On(OnTickELSKeyHandlerController);
            _ticks.On(OnTickKeyELSChecker);
            _ticks.On(OnTickSirenChecker);
            //_ticks.On(OnTickAutoSteeringIndicators);
            //_ticks.On(OnTickBullhornChecker);
            //_ = BlinkerLoop(); // fire-and-forget background task

            _commands.Register("elsme").WithHandler(async () =>
            {
                var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
                var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);
                var userAces = await _useraces.GetUserAces();

                if (!isEaServer && !isDevServer)
                {
                    if (!userAces.IsDeveloper && !userAces.IsAdmin && !userAces.IsTierTwo &&
                        !SpawnVehicleCommand.spawnWithELS) return;

                    await InitElsVehicle(Game.PlayerPed.CurrentVehicle.Handle, 1);
                    return;
                }

                await InitElsVehicle(Game.PlayerPed.CurrentVehicle.Handle, 1);
            });

            _commands.Register("flights").WithHandler(async () =>
            {
                var userAces = await _useraces.GetUserAces();
                if (!userAces.IsDeveloper && !userAces.IsAdmin && !userAces.IsTierTwo)
                {
                    return;
                }

                flashingEnabled = !flashingEnabled;
                
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("ELS Headlights", "info",
                    "Flashing headlights toggled", new NewNotificationMessageContent[0]));
            });
            
            _commands.Register("flashlights").WithHandler(async () =>
            {
                var userAces = await _useraces.GetUserAces();
                if (!userAces.IsDeveloper && !userAces.IsAdmin && !userAces.IsTierTwo)
                {
                    return;
                }
            
                int ped = API.PlayerPedId();
                int vehicle = API.GetVehiclePedIsIn(ped, false);
            
                if (vehicle != 0)
                {
                    if (flashingEnabled)
                    {
                        flashingHeadlights = !flashingHeadlights;
            
                        if (flashingHeadlights)
                        {
                            while (flashingHeadlights)
                            {
                                API.SetVehicleLights(vehicle, 3);
                                API.SetVehicleFullbeam(vehicle, true);
            
                                await BaseScript.Delay(250);
            
                                if (!flashingHeadlights) break;
            
                                API.SetVehicleLights(vehicle, 3);
                                API.SetVehicleFullbeam(vehicle, false);
            
                                await BaseScript.Delay(250);
            
                                if (!flashingHeadlights) break;
                            }
                        }
                        else
                        {
                            API.SetVehicleLights(vehicle, 3);
                            API.SetVehicleFullbeam(vehicle, false);
                        }
                    }
                }
            });
            
            _commands.Register("sirentone").WithHandler(async (args) =>
            {
                var userAces = await _useraces.GetUserAces();
                if (!userAces.IsDeveloper && !userAces.IsAdmin && !userAces.IsTierTwo)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Action Blocked!", "error",
                        "I have detected you're not a Dev... Be gone!", new NewNotificationMessageContent[0]));
                    return;
                }

                if (args.Length == 0 || !int.TryParse(string.Join("", args), out int requestedSirenType) || requestedSirenType < 1 || requestedSirenType > 14)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Invalid Siren Number", "error",
                        "Please enter a siren number between 1 and 11.", new NewNotificationMessageContent[0]));
                    return;
                }

                var player = Game.PlayerPed.Handle;
                var vehicleIsIn = API.GetVehiclePedIsIn(player, false);

                API.DecorSetInt(vehicleIsIn, ELSDecors.SIREN_TYPE_CODE, requestedSirenType);
                
                _logger.Debug($"Set Siren Type: {requestedSirenType}, Sound Number: {API.DecorGetInt(vehicleIsIn, ELSDecors.SIREN_SOUND_NUMBER)}");


                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Siren Type", "info",
                    $"Siren has been changed to: {requestedSirenType}", new NewNotificationMessageContent[0]));
            });

            _commands.Register("toggleprimary").WithHandler(() =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle == null) return;
                if (!API.DecorGetBool(vehicle.Handle, ELSDecors.ELS_ENABLED)) return;

                PlayInterfaceSound();
                ToggleMain(vehicle);
                API.ExecuteCommand("flashlights");
            });
            API.RegisterKeyMapping("toggleprimary", "Toggle Primary Lights", "keyboard", "q");

            _commands.Register("togglesecondary").WithHandler(() =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle == null) return;
                if (!API.DecorGetBool(vehicle.Handle, ELSDecors.ELS_ENABLED)) return;

                PlayInterfaceSound();
                API.DecorSetBool(vehicle.Handle, ELSDecors.LIGHTS_RED,
                    (!API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_RED)));
            });
            API.RegisterKeyMapping("togglesecondary", "Toggle Secondary Lights", "keyboard", "k");

            _commands.Register("togglematrix").WithHandler(() =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle == null) return;
                if (!API.DecorGetBool(vehicle.Handle, ELSDecors.ELS_ENABLED)) return;

                PlayInterfaceSound();

                API.DecorSetBool(vehicle.Handle, ELSDecors.LIGHTS_WARN,
                    !API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_WARN));
            });
            API.RegisterKeyMapping("togglematrix", "Toggle Matrix Boards", "keyboard", "l");
            
            /*API.RegisterCommand("toggleLeftIndicator", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle == null || vehicle.Driver.Handle != Game.PlayerPed.Handle) return;
                
                bool current = API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT, !current);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT, false);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_HAZARD, false);
            }), false);
            API.RegisterKeyMapping("toggleLeftIndicator", "Toggle Left Indicator", "keyboard", "LEFT");

            API.RegisterCommand("toggleRightIndicator", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var ped = Game.PlayerPed;
                var vehicle = ped.CurrentVehicle;

                if (vehicle == null || vehicle.Driver == null || vehicle.Driver.Handle != ped.Handle) return;
                bool current = API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT, !current);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT, false);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_HAZARD, false);
            }), false);
            API.RegisterKeyMapping("toggleRightIndicator", "Toggle Right Indicator", "keyboard", "RIGHT");

            API.RegisterCommand("toggleHazards", new Action<int, List<object>, string>((src, args, raw) =>
            {
                var ped = Game.PlayerPed;
                var vehicle = ped.CurrentVehicle;

                if (vehicle == null || vehicle.Driver == null || vehicle.Driver.Handle != ped.Handle) return;
                bool current = API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_HAZARD);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_HAZARD, !current);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT, false);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT, false);
            }), false);
            API.RegisterKeyMapping("toggleHazards", "Toggle Hazard Lights", "keyboard", "UP");*/
        }
        
        //private bool _blinkerOn = false;

        /*private async Task BlinkerLoop()
        {
            while (true)
            {
                _blinkerOn = !_blinkerOn;
                await BaseScript.Delay(500);

                foreach (var vehicle in World.GetAllVehicles())
                {
                    if (!vehicle.Exists()) continue;

                    bool hazard = API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_HAZARD);
                    bool left = API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT);
                    bool right = API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT);

                    bool blinkingLeft = (hazard || left) && _blinkerOn;
                    bool blinkingRight = (hazard || right) && _blinkerOn;

                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, vehicle.Handle, 0, blinkingRight);
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, vehicle.Handle, 1, blinkingLeft);

                    if (hazard)
                    {
                        API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
                    }
                }
            }
        }
        
        private bool _autoLeftActive = false;
        private bool _autoRightActive = false;
        
        private async Task OnTickAutoSteeringIndicators()
        {
            await BaseScript.Delay(300);

            var ped = Game.PlayerPed;
            if (!ped.IsInVehicle()) return;

            var vehicle = ped.CurrentVehicle;
            if (vehicle == null || !vehicle.Exists() || !vehicle.IsDriveable) return;
            if (vehicle.Driver == null || vehicle.Driver.Handle != ped.Handle) return;

            float steer = API.GetControlNormal(0, (int)Control.VehicleMoveLeftRight); // -1 to 1
            bool turningLeft = steer < -0.4f;
            bool turningRight = steer > 0.4f;
            bool steeringStraight = steer > -0.2f && steer < 0.2f;

            bool leftOn = API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT);
            bool rightOn = API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT);
            bool hazard = API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_HAZARD);

            if (hazard) return;

            // Turn off if steering straight again
            if (steeringStraight)
            {
                if (_autoLeftActive && leftOn)
                {
                    await ToggleIndicator(vehicle, "left", false);
                    _autoLeftActive = false;
                }
                if (_autoRightActive && rightOn)
                {
                    await ToggleIndicator(vehicle, "right", false);
                    _autoRightActive = false;
                }
                return;
            }

            //* Turn on indicator if not already blinking
            if (turningLeft && !leftOn)
            {
                await ToggleIndicator(vehicle, "right", false); // Turn off right if active
                await ToggleIndicator(vehicle, "left", true, withDelay: true); // ← add delay
                _autoLeftActive = true;
                _autoRightActive = false;
            }
            else if (turningRight && !rightOn)
            {
                await ToggleIndicator(vehicle, "left", false); // Turn off left if active
                await ToggleIndicator(vehicle, "right", true, withDelay: true); // ← add delay
                _autoRightActive = true;
                _autoLeftActive = false;
            }
        }*/

        public async Task InitElsVehicle(int vehicle, int sirenType)
        {
            API.DisableControlAction(0, (int)Control.VehicleRadioWheel, true); //Q
            API.DisableControlAction(0, (int)Control.VehicleHorn, true); //E
            API.DisableControlAction(0, (int)Control.CinematicSlowMo, true); //R

            if (!await EnsureVehicleExists(vehicle))
            {
                return;
            }
            
            API.SetVehicleRadioEnabled(vehicle, false);
            API.SetDisableVehicleSirenSound(vehicle, true);
            API.SetVehicleAutoRepairDisabled(vehicle, true);
            API.DecorSetBool(vehicle, ELSDecors.ELS_ENABLED, true);
            API.DecorSetInt(vehicle, ELSDecors.SIREN_TYPE_CODE, sirenType);
            API.DecorSetInt(vehicle, ELSDecors.SIREN_SOUND_NUMBER, 0);

            var v = (Vehicle)Entity.FromHandle(vehicle);
            for (var i = 0; i < 15; i++)
            {
                v.ToggleExtra(i, false);
            }
        }

        private async Task OnTickELSKeyHandlerKeyboard()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle == null) return;
            
            if (!await EnsureVehicleExists(vehicle.Handle))
            {
                return;
            }
            
            API.DisableControlAction(0, (int)Control.PhoneLeft, true);
            API.DisableControlAction(0, (int)Control.PhoneRight, true);
            
            // - (INDICATOR LEFT | HAZARDS)
            //if (_input.IsJustReleased(Control.VehiclePrevRadioTrack))
            //{
            //   if (_input.IsPressed(Control.CharacterWheel))
            //   {
            //        Alt and - are pressed, toggle hazards
            //        ToggleHazards(vehicle);
            //        return;
            //    }
            //
            //    ToggleIndicator(vehicle, 0);
            //}

            // = (INDICATOR RIGHT)
            //if (_input.IsJustReleased(Control.VehicleNextRadioTrack))
            //{
            //    ToggleIndicator(vehicle, 1);
            //}
            
            if (!API.DecorGetBool(vehicle.Handle, ELSDecors.ELS_ENABLED)) return;

            var sirenType = API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE);
            
            API.SetVehicleRadioEnabled(vehicle.Handle, false);
            API.SetDisableVehicleSirenSound(vehicle.Handle, true);
            API.SetVehicleAutoRepairDisabled(vehicle.Handle, true);

            API.DisableControlAction(0, (int)Control.VehicleRadioWheel, true); //Q

            //Enable the horn if lights are off
            if (API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN))
            {
                API.DisableControlAction(0, (int)Control.VehicleHorn, true); //E
            }
            else
            {
                API.EnableControlAction(0, (int)Control.VehicleHorn, true);
            }

            API.DisableControlAction(0, (int)Control.ReplayShowhotkey, true); //K
            API.DisableControlAction(0, (int)Control.PhoneCameraFocusLock, true); //L
            API.DisableControlAction(0, (int)Control.VehicleCinCam, true); //R
            API.DisableControlAction(2, (int)Control.VehicleCinCam, true); // B on controller
            API.SetCinematicButtonActive(false);
            
            //Handle Keyboard
            if (!API.GetLastInputMethod(0)) return;

            //E
            if (_input.IsJustPressed(Control.VehicleHorn))
            {
                if (API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE) == 0)
                {
                    await SoundHorn(vehicle);
                    return;
                }

                SirenSwitch(vehicle);
            }

            //R
            if (_input.IsJustBeenTapped(Control.VehicleCinCam, holdMs: 150))
            {
                SirenOff(vehicle);
            }

            // Bull Horn (B)
            /*
            if (_input.IsBeingHeld(Control.ReplayStartpoint, holdMs: 75) && API.DecorGetBool(vehicle.Handle, ELSDecors.ELS_ENABLED))
            {
                switch (sirenType)
                {
                    case 0:
                        API.DecorSetInt(vehicle.Handle, ELSDecors.DECOR_BULLHORN_TYPE, 0);
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 5:
                    case 6:
                    case 7:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                        API.DecorSetInt(vehicle.Handle, ELSDecors.DECOR_BULLHORN_TYPE, 1);
                        break;
                    case 4:
                        API.DecorSetInt(vehicle.Handle, ELSDecors.DECOR_BULLHORN_TYPE, 3);
                        break;
                    case 8:
                        API.DecorSetInt(vehicle.Handle, ELSDecors.DECOR_BULLHORN_TYPE, 2);
                        break;
                    default:
                        API.DecorSetInt(vehicle.Handle, ELSDecors.DECOR_BULLHORN_TYPE, 1);
                        break;
                }
            }
            else
            {
                API.DecorSetInt(vehicle.Handle, ELSDecors.DECOR_BULLHORN_TYPE, 0);
            }
            */
        }

        private async Task OnTickELSKeyHandlerController()
        {
            var vehicle = Game.PlayerPed.CurrentVehicle;
            if (vehicle == null) return;
            
            if (!await EnsureVehicleExists(vehicle.Handle))
            {
                return;
            }
            
            if (!API.DecorGetBool(vehicle.Handle, ELSDecors.ELS_ENABLED)) return;

            //Handle Controler
            if (API.GetLastInputMethod(0)) return;

            var sirenType = API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE);

            //Dpad Left
            if (_input.IsJustPressed(Control.Detonate))
            {
                PlayInterfaceSound();
                ToggleMain(vehicle);
            }

            //L3
            if (_input.IsJustPressed(Control.VehicleHorn))
            {
                if (API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE) == 0)
                {
                    SoundHorn(vehicle);
                    return;
                }

                SirenSwitch(vehicle);
            }

            //B or Circle
            if (_input.IsJustBeenTapped(Control.Reload, holdMs: 150))
            {
                SirenOff(vehicle);
            }
        }

        private void ToggleMain(Vehicle vehicle)
        {
            API.DecorSetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN,
                (!API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN)));
            API.DecorSetBool(vehicle.Handle, ELSDecors.LIGHTS_RED, false);
            API.DecorSetBool(vehicle.Handle, ELSDecors.LIGHTS_WARN, false);
            API.SetVehicleSiren(vehicle.Handle, API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN));

            vehicle.ToggleExtra(10, true); //Steady burner

            if (!API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN))
            {
                //Main lights have gone off.
                vehicle.ToggleExtra(10, false); //Steady burner
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE, false);
                API.DecorSetInt(vehicle.Handle, ELSDecors.SIREN_SOUND_NUMBER, 0);
            }
            else
            {
                if (API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE) == 2)
                {
                    _soundService.Play("999mode.ogg", 0.50f);
                }

                if (API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE) == 10)
                {
                    _soundService.Play("999mode.ogg", 0.50f);
                }
            }
        }
        
        public List<int> GetSirenIndexesWithThreeSirens()
        {
            List<int> indexes = new List<int>();

            int index = 0;
            foreach (var entry in _sirenMapping)
            {
                if (entry.Value.Count == 3)
                {
                    indexes.Add(index);
                }
                index++;
            }

            return indexes;
        }


        private async Task SirenSwitch(Vehicle vehicle)
        {
            if (!API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN)) return;
            API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE, false);
            int currentsound = API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_SOUND_NUMBER);
            currentsound++;

            if (GetSirenIndexesWithThreeSirens().Contains(API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE)))
            {
                if (currentsound >= 4) currentsound = 1;  
            }
            else
            {
                if (currentsound >= 5) currentsound = 1;  
            }
            
            API.DecorSetInt(vehicle.Handle, ELSDecors.SIREN_SOUND_NUMBER, currentsound);
            await SoundHorn(vehicle);
            API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE, true);
        }

        private async Task SirenOff(Vehicle vehicle)
        {
            API.DecorSetInt(vehicle.Handle, ELSDecors.DECOR_BULLHORN_TYPE, 0);
            if (!API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE)) return;

            API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE, false);
            API.DecorSetInt(vehicle.Handle, ELSDecors.SIREN_SOUND_NUMBER, 0);
            await SoundHorn(vehicle);
            await SoundHorn(vehicle);
        }
        
        /*private async Task ToggleIndicator(Vehicle vehicle, string direction, bool state, bool withDelay = false)
        {
            if (withDelay && state)
            {
                // Prevent immediate blinking
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT, false);
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT, false);
                await BaseScript.Delay(300); // ⏳ 300ms delay
            }

            if (direction == "left")
            {
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT, state);
                if (state) API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT, false);
            }
            else if (direction == "right")
            {
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT, state);
                if (state) API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT, false);
            }

            if (state)
            {
                API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_HAZARD, false);
            }
        }
        
        private void ToggleHazards(Vehicle vehicle, bool state)
        {
            API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_LEFT, state);
            API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_RIGHT, state);
            API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_INDICATOR_HAZARD, state);
        }*/


        /// <summary>
        /// Toggles Vehicle Indicator
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="type">0 = Left, 1 = Right</param>
        //private void ToggleIndicator(Vehicle vehicle, int type)
        //{
        //    switch (type)
        //     {
        //        case 0:
        //            vehicle.IsLeftIndicatorLightOn = !vehicle.IsLeftIndicatorLightOn;
        //            API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
        //             break;
        //
        //         case 1:
        //             vehicle.IsRightIndicatorLightOn = !vehicle.IsRightIndicatorLightOn;
        //             API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
        //             break;
        //     }
        // }

        //private void ToggleHazards(Vehicle vehicle)
        //{
        //    if (!API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_HAZARD_ACTIVE))
        //    {
        //        // Hazards not active
        //        vehicle.IsLeftIndicatorLightOn = true;
        //        vehicle.IsRightIndicatorLightOn = true;
        //        API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_HAZARD_ACTIVE, true);
        //        API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
        //        return;
        //    }

        //    vehicle.IsLeftIndicatorLightOn = false;
        //    vehicle.IsRightIndicatorLightOn = false;
        //    API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_HAZARD_ACTIVE, false);
        //    API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
        //}

        private async Task OnTickKeyELSChecker()
        {
            var vehicles = World.GetAllVehicles();
            for (int i = 0; i < vehicles.Length; i++)
            {
                lock (_mainElsTasks)
                {
                    foreach (var task in _mainElsTasks)
                        if (task.Value.IsCompleted || task.Value.IsCanceled || task.Value.IsFaulted)
                        {
                            _mainElsTasks.Remove(task.Key);
                            break; // break out the loop -- remove one a frame and avoid enumeration changed exception
                        }
                }

                lock (_onSceneElsTasks)
                {
                    foreach (var task in _onSceneElsTasks)
                    {
                        if (task.Value.IsCompleted || task.Value.IsCanceled || task.Value.IsFaulted)
                        {
                            _onSceneElsTasks.Remove(task.Key);
                            break; // break out the loop -- remove one a frame and avoid enumeration changed exception
                        }
                    }
                }

                lock (_redElsTasks)
                {
                    foreach (var task in _redElsTasks)
                        if (task.Value.IsCompleted || task.Value.IsCanceled || task.Value.IsFaulted)
                        {
                            _redElsTasks.Remove(task.Key);
                            break;
                        }
                }

                lock (_warnElsTasks)
                {
                    foreach (var task in _warnElsTasks)
                        if (task.Value.IsCompleted || task.Value.IsCanceled || task.Value.IsFaulted)
                        {
                            _warnElsTasks.Remove(task.Key);
                            break;
                        }
                }

                lock (_takeDownTasks)
                {
                    foreach (var task in _takeDownTasks)
                    {
                        if (task.Value.IsCompleted || task.Value.IsCanceled || task.Value.IsFaulted)
                        {
                            _takeDownTasks.Remove(task.Key);
                            break;
                        }
                    }
                }

                lock (_nhsSteadyBurnerTask)
                {
                    foreach (var task in _nhsSteadyBurnerTask)
                    {
                        if (task.Value.IsCompleted || task.Value.IsCanceled || task.Value.IsFaulted)
                        {
                            _nhsSteadyBurnerTask.Remove(task.Key);
                            break;
                        }
                    }
                }

                var vehicle = vehicles[i];
                if (!API.DoesEntityExist(vehicle.Handle)) continue;
                API.SetVehicleAutoRepairDisabled(vehicle.Handle, true); //This may stop the flickering doors....

                //Some stuff for siren redundancy. Stops the awful blipping noises.
                //This is a bit hacky but basically sirens can be activated on keypress still somehow
                //This works as a final redundancy so the sirens won't get stuck in a "blip loop"
                API.SetDisableVehicleSirenSound(vehicle.Handle, true);
                if (API.IsVehicleSirenOn(vehicle.Handle) && !API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN))
                {
                    _logger.Debug("Siren on but meant to be off....");
                    API.SetVehicleSiren(vehicle.Handle, false);
                    API.SetSirenKeepOn(vehicle.Handle, false);
                }

                //If either of these return false then the vehicle is not ELS enabled.
                if (!API.DecorGetBool(vehicle.Handle, ELSDecors.ELS_ENABLED)) continue;

                //Check to see if vehicle ded
                if (!API.DoesEntityExist(vehicle.Handle)) continue;
                if (!vehicle.IsDriveable || vehicle.IsDead)
                {
                    API.DecorSetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN, false);
                    API.DecorSetBool(vehicle.Handle, ELSDecors.LIGHTS_RED, false);
                    API.DecorSetBool(vehicle.Handle, ELSDecors.LIGHTS_WARN, false);
                    API.DecorSetBool(vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE, false);
                    continue;
                }

                var sirenType = API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE);

                lock (_mainElsTasks)
                {
                    if (sirenType == 2)
                    {
                        if (!_mainElsTasks.ContainsKey(vehicle.Handle)
                            && API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN))
                        {
                            _mainElsTasks.Add(vehicle.Handle, PrimaryFlasher(vehicle));
                        }
                    }
                    else
                    {
                        if (!_mainElsTasks.ContainsKey(vehicle.Handle)
                            && API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN))
                        {
                            _mainElsTasks.Add(vehicle.Handle, PrimaryFlasher(vehicle));
                        }
                    }
                }

                lock (_nhsSteadyBurnerTask)
                {
                    if (sirenType == 2)
                    {
                        if (!_nhsSteadyBurnerTask.ContainsKey(vehicle.Handle)
                            && API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN))
                        {
                            _nhsSteadyBurnerTask.Add(vehicle.Handle, NhsSteadyBurn(vehicle));
                        }
                    }
                }

                lock (_redElsTasks)
                {
                    if (!_redElsTasks.ContainsKey(vehicle.Handle)
                        && API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_RED))
                    {
                        _redElsTasks.Add(vehicle.Handle, RedFlasher(vehicle));
                    }
                }

                lock (_warnElsTasks)
                {
                    if (!_warnElsTasks.ContainsKey(vehicle.Handle)
                        && API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_WARN))
                    {
                        _warnElsTasks.Add(vehicle.Handle, WarnFlasher(vehicle));
                    }
                }

                //If any of the lights are on it will keep the engine running so lights don't dim.
                if (API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN)
                    || API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_RED)
                    || API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_WARN))
                {
                    API.SetVehicleEngineOn(vehicle.Handle, true, true, false);
                }

                await Delay(0);
            }
        }

        private async Task OnTickSirenChecker()
        {
            var vehicles = World.GetAllVehicles();
            for (int i = 0; i < vehicles.Length; i++)
            {
                lock (_sirenSoundIds)
                {
                    foreach (var sound in _sirenSoundIds)
                    {
                        if (!API.DecorGetBool(sound.Key, ELSDecors.DECOR_SIREN_ACTIVE))
                        {
                            StopSiren(sound.Key, sound.Value);
                            break;
                        }
                    }
                }

                var vehicle = vehicles[i];

                if (!API.DoesEntityExist(vehicle.Handle)) continue;
                if (!vehicle.IsDriveable || vehicle.IsDead) continue;

                lock (_sirenSoundIds)
                {
                    if (!_sirenSoundIds.ContainsKey(vehicle.Handle)
                        && API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE))
                    {
                        StartSiren(vehicle);
                    }

                    if (!_sirenSoundIds.ContainsKey(vehicle.Handle)
                        && API.DecorGetBool(vehicle.Handle, ELSDecors.DECOR_SIREN_ACTIVE))
                    {
                        StartSiren(vehicle);
                    }
                }
            }
        }
        /*
        private async Task OnTickBullhornChecker()
        {
            var vehicles = World.GetAllVehicles();
            for (int i = 0; i < vehicles.Length; i++)
            {
                var vehicle = vehicles[i];

                if (!API.DoesEntityExist(vehicle.Handle)) continue;
                if (!vehicle.IsDriveable || vehicle.IsDead) continue;

                var bullhornNumber = API.DecorGetInt(vehicle.Handle, ELSDecors.DECOR_BULLHORN_TYPE);
                switch (bullhornNumber)
                {
                    case 1:
                        _soundService.PlayFromLocation("Bullhorn.wav", 0.1f, Game.PlayerPed.Position, vehicle.GetOffsetPosition(new Vector3(2f, 0f, 0f)), Game.PlayerPed.Rotation);
                        // API.PlaySoundFromEntity(-1, "oiss_ssa_vehaud_noose_new_horn", vehicle.Handle, "DLC_SERVERSIDEAUDIO\\oiss_ssa_vehaud_noose_new", true, 0);
                        break;
                    case 2:
                        _soundService.PlayFromLocation("rumbler.wav", 0.1f, Game.PlayerPed.Position, vehicle.Position,Game.PlayerPed.Rotation);
                        break;
                    case 3:
                        _soundService.PlayFromLocation("whistle.wav", 0.2f, Game.PlayerPed.Position, vehicle.Position,Game.PlayerPed.Rotation);
                        break;
                }
            }
            await Delay(1);
        }
        */

        private async Task NhsSteadyBurn(Vehicle vehicle)
        {
            while (API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN))
            {
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
                vehicle.ToggleExtra(12, true);
                await Delay(1500);
                vehicle.ToggleExtra(12, false);
                await Delay(1);
            }
        }

        private async Task PrimaryFlasher(Vehicle vehicle)
        {
            while (API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_MAIN))
            {
                //API.SetVehicleSiren(vehicle.Handle, true);
                string colour = "blue";
                if (API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE) == 0) colour = "amber";

                bool isMatrix = false;
                if (API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE) == 9 || API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE) == 10 || API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE) == 11)
                {
                    isMatrix = true;
                }

                if (isMatrix)
                {
                    await TripleFlicker(vehicle, 1, 3, 8);
                    AmbientLight(colour, vehicle.Position);
                    await TripleFlicker(vehicle, 1, 3, 8);
                    AmbientLight(colour, vehicle.Position);
                    await TripleFlicker(vehicle, 1, 3, 8);
                    AmbientLight(colour, vehicle.Position);
                    await TripleFlicker(vehicle, 1, 3, 8);
                    AmbientLight(colour, vehicle.Position);
                    vehicle.ToggleExtra(8, false);
                }
                else
                {
                    await DoubleFlicker(vehicle, 1, 3);
                    AmbientLight(colour, vehicle.Position);
                    await DoubleFlicker(vehicle, 1, 3);
                    AmbientLight(colour, vehicle.Position);
                    await DoubleFlicker(vehicle, 1, 3);
                    AmbientLight(colour, vehicle.Position);
                    await DoubleFlicker(vehicle, 1, 3);
                    AmbientLight(colour, vehicle.Position);
                }

                vehicle.ToggleExtra(1, false);
                vehicle.ToggleExtra(3, false);

                if (isMatrix)
                {
                    await TripleFlicker(vehicle, 2, 4, 12);
                    AmbientLight(colour, vehicle.Position);
                    await TripleFlicker(vehicle, 2, 4, 12);
                    AmbientLight(colour, vehicle.Position);
                    await TripleFlicker(vehicle, 2, 4, 12);
                    AmbientLight(colour, vehicle.Position);
                    await TripleFlicker(vehicle, 2, 4, 12);
                    AmbientLight(colour, vehicle.Position);
                    vehicle.ToggleExtra(12, false);
                }
                else
                {
                    await DoubleFlicker(vehicle, 2, 4);
                    AmbientLight(colour, vehicle.Position);
                    await DoubleFlicker(vehicle, 2, 4);
                    AmbientLight(colour, vehicle.Position);
                    await DoubleFlicker(vehicle, 2, 4);
                    AmbientLight(colour, vehicle.Position);
                    await DoubleFlicker(vehicle, 2, 4);
                    AmbientLight(colour, vehicle.Position);
                }

                vehicle.ToggleExtra(2, false);
                vehicle.ToggleExtra(4, false);
            }
        }

        private async Task RedFlasher(Vehicle vehicle)
        {
            while (API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_RED))
            {
                vehicle.ToggleExtra(7, true);
                AmbientLight("red", vehicle.Position);
                await Delay(125);
                vehicle.ToggleExtra(7, false);
                await Delay(125);
                vehicle.ToggleExtra(7, true);
                AmbientLight("red", vehicle.Position);
                await Delay(125);
                vehicle.ToggleExtra(7, false);
                await Delay(125);
                vehicle.ToggleExtra(9, true);
                AmbientLight("red", vehicle.Position);
                await Delay(125);
                vehicle.ToggleExtra(9, false);
                await Delay(125);
                vehicle.ToggleExtra(9, true);
                AmbientLight("red", vehicle.Position);
                await Delay(125);
                vehicle.ToggleExtra(9, false);
            }
        }

        private async Task WarnFlasher(Vehicle vehicle)
        {
            while (API.DecorGetBool(vehicle.Handle, ELSDecors.LIGHTS_WARN))
            {
                vehicle.ToggleExtra(5, true);
                await Delay(800);
                vehicle.ToggleExtra(5, false);
                //await Delay(200);
                vehicle.ToggleExtra(6, true);
                await Delay(800);
                vehicle.ToggleExtra(6, false);
                //await Delay(200);
            }
        }

        private async Task DoubleFlicker(Vehicle vehicle, int extra1, int extra2)
        {
            vehicle.ToggleExtra(extra1, true);
            vehicle.ToggleExtra(extra2, true);
            await Delay(50);
            vehicle.ToggleExtra(extra1, false);
            vehicle.ToggleExtra(extra2, false);
            await Delay(50);
        }

        private async Task TripleFlicker(Vehicle vehicle, int extra1, int extra2, int extra3)
        {
            vehicle.ToggleExtra(extra1, true);
            vehicle.ToggleExtra(extra2, true);
            vehicle.ToggleExtra(extra3, true);
            await Delay(50);
            vehicle.ToggleExtra(extra1, false);
            vehicle.ToggleExtra(extra2, false);
            vehicle.ToggleExtra(extra3, false);
            await Delay(50);
        }

        private void AmbientLight(string colour, Vector3 position)
        {
            switch (colour)
            {
                case "blue":
                    API.DrawLightWithRangeAndShadow(position.X, position.Y, position.Z, 0, 0, 255, 50, 0.5f, 5.0f);
                    break;

                case "red":
                    API.DrawLightWithRangeAndShadow(position.X, position.Y, position.Z, 255, 0, 0, 50, 0.5f, 5.0f);
                    break;

                case "amber":
                    API.DrawLightWithRangeAndShadow(position.X, position.Y, position.Z, 255, 194, 0, 50, 0.5f, 5.0f);
                    break;

                case "green":
                    API.DrawLightWithRangeAndShadow(position.X, position.Y, position.Z, 0, 255, 0, 50, 0.5f, 5.0f);
                    break;

                case "white":
                    API.DrawLightWithRangeAndShadow(position.X, position.Y, position.Z, 255, 255, 255, 50, 0.5f, 5.0f);
                    break;
            }
        }

        private void StartSiren(Vehicle vehicle)
        {
            if (!API.DoesEntityExist(vehicle.Handle)) return;

            lock (_sirenSoundIds)
            {
                if (_sirenSoundIds.TryGetValue(vehicle.Handle, out int currentSoundId))
                {
                    API.ReleaseSoundId(currentSoundId);
                    _sirenSoundIds.Remove(vehicle.Handle);
                    _logger.Debug($"Releaseing sound id {currentSoundId} ({vehicle.Handle})");
                }
            }

            if (!API.DoesEntityExist(vehicle.Handle)) return;

            if (!API.DecorExistOn(vehicle.Handle, ELSDecors.SIREN_SOUND_NUMBER))
            {
                _logger.Debug(
                    "Unable to find Siren Sound Number or missing <vehicleClass>VC_EMERGENCY</vehicleClass>!");
                return;
            }

            var elsSirenType = ElsSirenTypeSelector(vehicle);
            var sirenSoundNumber = API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_SOUND_NUMBER) - 1;

            if (sirenSoundNumber < 0)
            {
                _logger.Trace("Siren Sound Number is Negative!");
                return;
            }

            var sirenMaps = _sirenMapping[elsSirenType];
            if (!sirenMaps.Any())
            {
                _logger.Error($"No siren maps for {elsSirenType}");
                return;
            }

            var sound = sirenMaps[sirenSoundNumber];

            if (string.IsNullOrEmpty(sound[0]) || string.IsNullOrEmpty(sound[1]))
            {
                _logger.Trace("Unable to find Siren Sound");
                return;
            }

            _logger.Debug(
                $"Found Siren Map: {sound} - {_sirenMapping[ElsSirenTypeSelector(vehicle)].ToArray()[sirenSoundNumber]}");

            var soundId = API.GetSoundId();

            if (soundId == null || soundId < 1)
            {
                _logger.Trace("BIG JOBBIE SOUND ID MIGHT OF RAN OUT");

                for (int i = 1; i < 100; i++)
                {
                    API.ReleaseSoundId(i);
                }

                _logger.Trace("Told sounds to release");
            }

            API.PlaySoundFromEntity(soundId, sound[0], vehicle.Handle, sound[1], false, 0);
            API.SetVehicleHasMutedSirens(vehicle.Handle, true);

            lock (_sirenSoundIds)
            {
                _sirenSoundIds.Add(vehicle.Handle, soundId);
            }
        }

        private void StopSiren(int vehicleHandle, int soundId)
        {
            API.StopSound(soundId);
            API.ReleaseSoundId(soundId);
            lock (_sirenSoundIds)
                _sirenSoundIds.Remove(vehicleHandle);
        }

        private async Task SoundHorn(Vehicle vehicle)
        {
            API.StartVehicleHorn(vehicle.Handle, 500, (uint)API.GetHashKey("HELDDOWN"), false);
            await Delay(250);
        }

        private ElsSirenType ElsSirenTypeSelector(Vehicle vehicle)
        {
            if (vehicle.ClassType != VehicleClass.Emergency) return ElsSirenType.Disabled;
            var sirenCode = API.DecorGetInt(vehicle.Handle, ELSDecors.SIREN_TYPE_CODE);
            return sirenCode switch
            {
                0 => ElsSirenType.Disabled,
                1 => ElsSirenType.Police,
                2 => ElsSirenType.Nhs,
                3 => ElsSirenType.Fire,
                4 => ElsSirenType.Bike,
                5 => ElsSirenType.PoliceGamma,
                6 => ElsSirenType.Lfb2,
                7 => ElsSirenType.Police2,
                8 => ElsSirenType.NHSdev,
                9 => ElsSirenType.MatrixPolice,
                10 => ElsSirenType.MatrixNHS,
                11 => ElsSirenType.MatrixLFB,
                12 => ElsSirenType.Bike2,
                13 => ElsSirenType.Gamma2,
                14 => ElsSirenType.PoliceB2,
                _ => ElsSirenType.Disabled
            };
        }

        private void PlayInterfaceSound()
        {
            var soundid = API.GetSoundId();
            API.PlaySoundFrontend(soundid, "PIN_BUTTON", "ATM_SOUNDS", false);
        }

        private async Task<bool> EnsureVehicleExists(int handle)
        {
            try
            {
                int attempts = 0;
                while (!API.DoesEntityExist(handle) && attempts < 10)
                {
                    attempts++;
                    await BaseScript.Delay(250);
                }

                if (!API.DoesEntityExist(handle))
                {
                    _logger.Warn("EnsureVehicleExists: Could not set important vehicle flags since the vehicle does not exist!");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}