using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using CitizenFX.Core.UI;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.HemsHeliMissions
{
    public class HemsHeliMissions : Script
    {
        private ILogger<HemsHeliMissions> _logger;
        private ICommandManager _commandManager;
        private IPermissionService _permissionService;
        private ITickManager _tickManager;
        private readonly IPlayerListAccessor _playerListAccessor;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IFeatureService _featureService;

        private Menu _HEMSMenu = new("HEMS Team");

        private readonly List<Vector3> _missionBases = new()
        {
            new Vector3(-446.1926f, -296.2921f, 82.03433f),
            new Vector3(298.5199f, -1448.1329f, 48.82415f),
            new Vector3(343.7734916f, -592.18261f, 77.481674f)
        };

        private readonly List<Vector3> _incidentLocations = new()
        {
            new Vector3(-1472.983f, -1241.787f, 2.601692f),
            new Vector3(-2112.428f, -496.6394f, 3.530993f),
            new Vector3(-1282.275f, 5359.651f, 2.99125f),
            new Vector3(146.4574f, 6425.471f, 31.28379f),
            new Vector3(2803.866f, 4774.049f, 46.81132f),
            new Vector3(612.5733f, 2807.442f, 41.93818f),
            new Vector3(1450.350f, 1106.152f, 114.33395f),
            new Vector3(1145.866f, 148.4881f, 80.87580f),
            new Vector3(1365.644f, -745.171f, 67.10335f),
            new Vector3(1482.447f, -2424.311f, 66.05972f),
            new Vector3(1189.735f, -3213.085f, 5.799363f),
            new Vector3(-241.8820f, -2067.535f, 27.62043f),
            new Vector3(-398.0584f, -2476.370f, 5.996006f),
            new Vector3(-1636.070f, -215.5898f, 55.00962f),
            new Vector3(-2031.534f, -261.0308f, 23.38624f),
            new Vector3(-1876.573f, 82.93414f, 82.24980f),
            new Vector3(-1747.708f, 162.0291f, 64.37453f),
            new Vector3(-1200.945f, 97.18770f, 57.91138f),
            new Vector3(205.4376f, 1231.770f, 225.4453f),
            new Vector3(334.0286f, 3563.990f, 33.75543f),
            new Vector3(2639.391f, 3519.382f, 53.40572f)
        };

        private readonly List<string> _patientPeds = new()
        {
            "a_f_m_bevhills_01",
            "a_f_m_bevhills_02",
            "a_f_m_ktown_02",
            "a_f_m_soucent_02",
            "a_f_y_bevhills_02",
            "a_f_y_eastsa_03",
            "a_m_m_beach_01",
            "a_m_m_genfat_02",
            "a_m_m_og_boss_01",
            "a_m_m_soucent_01",
            "a_m_y_bevhills_01",
            "a_m_y_dhill_01",
            "a_m_y_ktown_01",
            "a_m_y_stlat_01",
            "a_m_y_smartcaspat_01",
            "cs_jewelass",
            "cs_lamardavis",
            "cs_nigel",
            "g_f_y_vagos_01",
            "g_m_m_mexboss_02",
            "s_m_m_lsmetro_01",
            "s_m_y_construct_02",
            "ig_kerrymcintosh_02",
            "ig_magenta"
        };

        private readonly string _paramedicPedModel = "s_m_m_paramedic_01";

        private readonly List<string> _ambulanceVehicles = new()
        {
            "ambulance7",
            "ambulance",
            "ambulance8",
            "ambulance6"
        };

        private readonly List<string> _injuries = new()
        {
            "Severe head trauma",
            "Spinal injury",
            "Internal bleeding",
            "Multiple fractures",
            "Penetrating chest injury",
            "Severe burns"
        };

        private bool _missionActive = false;
        private bool _incidentSpawned = false;
        private bool _patientLoaded = false;
        private Vector3 _currentIncidentLocation;
        private Vector3 _currentReturnLocation;
        private string _currentInjury;

        private bool _despawnIncidentAfterLoading = false;
        private float _despawnRadius = 300f;

        private Vehicle _spawnedAmbulance;
        private Ped _spawnedPatient;
        private Ped _spawnedParamedic;

        private int hemsScore = 0;

        public HemsHeliMissions(
            ILogger<HemsHeliMissions> logger, 
            ICommandManager commandManager, 
            IPermissionService permissionService,
            ITickManager tickManager, 
            IPlayerListAccessor playerListAccessor,
            INewNotificationOverlay newNotificationOverlay, 
            IFeatureService featureService)
        {
            _logger = logger;
            _commandManager = commandManager;
            _permissionService = permissionService;
            _tickManager = tickManager;
            _playerListAccessor = playerListAccessor;
            _newNotificationOverlay = newNotificationOverlay;
            _featureService = featureService;
            _tickManager.On(CreateBlips);
            _tickManager.On(MissionLogic);
            _tickManager.On(DisplayHEMSCallText);
            MenuController.AddMenu(_HEMSMenu);
        }

        private async Task DisplayHEMSCallText()
        {
            if (!_missionActive)
            {
                await Task.FromResult(0);
                return;
            }

            var posX = 0.17f;
            var posY = 0.915f;
            var scale = 0.8f;
            var font = 4;

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName($"~g~Current Call: {_currentInjury}");
            API.EndTextCommandDisplayText(posX, posY);

            await Task.FromResult(0);
        }

        private async Task CreateBlips()
        {
            var _userAces = await _permissionService.GetUserAces();
            if (_permissionService.CurrentUserRole == null) return;
            if (_permissionService.CurrentUserRole.Division != UserDivision.Npas) return;
            if (!_userAces.IsAdmin && !_userAces.IsDeveloper && !_userAces.IsNpasTrained && !_userAces.IsDigitalTeam) return;
            Vector3 playerPos = Game.PlayerPed.Position;
            foreach (var baseLoc in _missionBases)
            {
                var distance = Vector3.Distance(playerPos, baseLoc);
                var scale = 0.1f * API.GetGameplayCamFov();
                if (distance < 20.0f)
                {
                    API.DrawMarker(1, baseLoc.X, baseLoc.Y, baseLoc.Z - 1, 0, 0, 0, 0, 0, 0, 1f, 1f, 2f, 0, 150, 255, 50, false, true, 2, false, null, null, false);
                    API.SetTextScale(0.1f * scale, 0.1f * scale);
                    API.SetTextFont(4);
                    API.SetTextProportional(true);
                    API.SetTextColour(250, 250, 250, 255);
                    API.SetTextDropshadow(1, 1, 1, 1, 255);
                    API.SetTextEdge(2, 0, 0, 0, 255);
                    API.SetTextDropShadow();
                    API.SetTextOutline();
                    API.SetTextEntry("STRING");
                    API.SetTextCentre(true);
                    API.AddTextComponentString("HEMS Missions");
                    API.SetDrawOrigin(baseLoc.X, baseLoc.Y, baseLoc.Z + 1f, 0);
                    API.DrawText(0, 0);
                    API.ClearDrawOrigin();
                    if (distance < 1.0f)
                    {
                        OpenHEMSMenu(baseLoc);
                    }
                }
            }
            await Task.FromResult(0);
        }

        private void OpenHEMSMenu(Vector3 currentBasePos)
        {
            if (MenuController.IsAnyMenuOpen()) return;
            _HEMSMenu.ClearMenuItems();

            var scoreItem = new MenuItem("Show HEMS Score", "See your current HEMS score");
            _HEMSMenu.AddMenuItem(scoreItem);

            var lastBonusDate = GetLastDailyBonusDateHems();
            var currentDate = DateTime.Today.ToString();
            if (lastBonusDate != currentDate)
            {
                var dailyBonusItem = new MenuItem("Collect Daily Bonus", "Receive extra points daily");
                _HEMSMenu.AddMenuItem(dailyBonusItem);
            }

            if (!_missionActive)
            {
                var startItem = new MenuItem("Start HEMS Mission", "Begin a new HEMS mission");
                _HEMSMenu.AddMenuItem(startItem);
            }
            else
            {
                float distToReturnBase = Vector3.Distance(currentBasePos, _currentReturnLocation);
                if (_patientLoaded && distToReturnBase < 50f)
                {
                    var endItem = new MenuItem("End Mission", "End the current HEMS mission");
                    _HEMSMenu.AddMenuItem(endItem);
                }
                else
                {
                    var cancelItem = new MenuItem("Cancel Mission", "Cancel the current mission");
                    _HEMSMenu.AddMenuItem(cancelItem);
                }
            }
            _HEMSMenu.OnItemSelect += HEMSMenuItemSelected;
            _HEMSMenu.OpenMenu();
        }

        private async void HEMSMenuItemSelected(Menu menu, MenuItem item, int index)
        {
            _HEMSMenu.OnItemSelect -= HEMSMenuItemSelected;
            if (item.Text == "Show HEMS Score")
            {
                int score = GetHemsScore();
                await Delay(250);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Score", "info", $"Your HEMS score is: {score}", new NewNotificationMessageContent[0]));
            }
            else if (item.Text == "Collect Daily Bonus")
            {
                var lastBonus = GetLastDailyBonusDateHems();
                var currentDate = DateTime.Today.ToString();
                if (lastBonus == currentDate)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Score", "error", "You have already claimed your daily bonus today! Come back tomorrow!", new NewNotificationMessageContent[0]));
                    _HEMSMenu.CloseMenu();
                    return;
                }
                await AddHemsScore(30);
                int newScore = GetHemsScore();
                API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Score", "success", $"Daily bonus collected. Your HEMS score is now: {newScore}", new NewNotificationMessageContent[0]));
                SetLastDailyBonusDateHems(currentDate);
            }
            else if (item.Text == "Start HEMS Mission")
            {
                StartMission();
            }
            else if (item.Text == "End Mission")
            {
                CompleteMission();
            }
            else if (item.Text == "Cancel Mission")
            {
                CancelMission();
            }
            _HEMSMenu.CloseMenu();
            await Task.FromResult(0);
        }

        private void StartMission()
        {
            if (_missionActive) return;
            _missionActive = true;
            _incidentSpawned = false;
            _patientLoaded = false;
            _despawnIncidentAfterLoading = false;
            Random rnd = new();
            var locIndex = rnd.Next(_incidentLocations.Count);
            _currentIncidentLocation = _incidentLocations[locIndex];
            var injuryIndex = rnd.Next(_injuries.Count);
            _currentInjury = _injuries[injuryIndex];
            var baseIndex = rnd.Next(_missionBases.Count);
            _currentReturnLocation = _missionBases[baseIndex];
            API.SetNewWaypoint(_currentIncidentLocation.X, _currentIncidentLocation.Y);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Dispatch", "info", $"New call: Patient with {_currentInjury}. Proceed to the incident location.", new NewNotificationMessageContent[0]));
        }

        private async Task MissionLogic()
        {
            if (!_missionActive)
            {
                await Task.FromResult(0);
                return;
            }
            if (!_incidentSpawned)
            {
                var dist = Vector3.Distance(Game.PlayerPed.Position, _currentIncidentLocation);
                if (dist < 300f)
                {
                    await SpawnIncident();
                }
            }
            else if (!_patientLoaded)
            {
                if (_spawnedPatient != null && _spawnedPatient.Exists())
                {
                    var distToPatient = Vector3.Distance(Game.PlayerPed.Position, _spawnedPatient.Position);
                    if (API.IsControlJustPressed(0, 38))
                    {
                        if (distToPatient < 10f)
                        {
                            LoadPatient();
                        }
                        else
                        {
                            if (API.IsControlJustReleased(0, 38))
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Dispatch", "error", "Get closer to the patient to load them.", new NewNotificationMessageContent[0]));
                                await Delay(500);
                            }
                        }
                    }
                    if (distToPatient < 10f)
                    {
                        Screen.ShowSubtitle($"~y~Paramedic: He has suffered {_currentInjury} and needs rushing to hospital!", 7500);
                    }
                }
            }
            else
            {
                if (_despawnIncidentAfterLoading)
                {
                    if (_spawnedParamedic != null && _spawnedParamedic.Exists())
                    {
                        var distToParamedic = Vector3.Distance(Game.PlayerPed.Position, _spawnedParamedic.Position);
                        if (distToParamedic > _despawnRadius)
                        {
                            _spawnedParamedic.Delete();
                            _spawnedParamedic = null;
                        }
                    }
                    if (_spawnedAmbulance != null && _spawnedAmbulance.Exists())
                    {
                        var distToAmbulance = Vector3.Distance(Game.PlayerPed.Position, _spawnedAmbulance.Position);
                        if (distToAmbulance > _despawnRadius)
                        {
                            _spawnedAmbulance.Delete();
                            _spawnedAmbulance = null;
                        }
                    }
                }
            }
            await Task.FromResult(0);
        }

        private async Task SpawnIncident()
        {
            if (_incidentSpawned) return;
            _incidentSpawned = true;
            Random rnd = new();
            var offsetX = (float)(rnd.NextDouble() * 10 - 5);
            var offsetY = (float)(rnd.NextDouble() * 10 - 5);
            Vector3 spawnPos = _currentIncidentLocation + new Vector3(offsetX, offsetY, 0f);
            var ambIndex = rnd.Next(_ambulanceVehicles.Count);
            var ambulanceModel = _ambulanceVehicles[ambIndex];
            var ambHash = (uint)API.GetHashKey(ambulanceModel);
            API.RequestModel(ambHash);
            while (!API.HasModelLoaded(ambHash))
            {
                await BaseScript.Delay(0);
            }
            var vehHandle = API.CreateVehicle(ambHash, spawnPos.X, spawnPos.Y, spawnPos.Z, 0f, true, false);
            _spawnedAmbulance = new Vehicle(vehHandle);
            _spawnedAmbulance.Heading = 0f;
            var patientIndex = rnd.Next(_patientPeds.Count);
            var patientModel = _patientPeds[patientIndex];
            var patientHash = (uint)API.GetHashKey(patientModel);
            API.RequestModel(patientHash);
            while (!API.HasModelLoaded(patientHash))
            {
                await BaseScript.Delay(0);
            }
            Vector3 patientPos = spawnPos + new Vector3(2f, 0f, 0f);
            var patientHandle = API.CreatePed(26, patientHash, patientPos.X, patientPos.Y, patientPos.Z, 0f, true, false);
            _spawnedPatient = new Ped(patientHandle);
            API.RequestAnimDict("misslamar1dead_body");
            while (!API.HasAnimDictLoaded("misslamar1dead_body"))
            {
                await BaseScript.Delay(0);
            }
            API.TaskPlayAnim(_spawnedPatient.Handle, "misslamar1dead_body", "dead_idle", 8.0f, -8.0f, -1, 1, 0, false, false, false);
            var paramedicHash = (uint)API.GetHashKey(_paramedicPedModel);
            API.RequestModel(paramedicHash);
            while (!API.HasModelLoaded(paramedicHash))
            {
                await BaseScript.Delay(0);
            }
            Vector3 paramedicPos = patientPos + new Vector3(1f, 0f, 0f);
            var paramedicHandle = API.CreatePed(26, paramedicHash, paramedicPos.X, paramedicPos.Y, paramedicPos.Z, 0f, true, false);
            _spawnedParamedic = new Ped(paramedicHandle);
            API.TaskStartScenarioInPlace(_spawnedParamedic.Handle, "WORLD_HUMAN_STAND_MOBILE", 0, true);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Dispatch", "info", "Ambulance on scene. Land and press E near the patient to load them.", new NewNotificationMessageContent[0]));
        }

        private void LoadPatient()
        {
            if (_patientLoaded) return;
            _patientLoaded = true;
            if (_spawnedPatient != null && _spawnedPatient.Exists())
            {
                _spawnedPatient.Delete();
                _spawnedPatient = null;
            }
            if (_spawnedParamedic != null && _spawnedParamedic.Exists())
            {
                API.ClearPedTasksImmediately(_spawnedParamedic.Handle);
            }
            _despawnIncidentAfterLoading = true;
            API.SetNewWaypoint(_currentReturnLocation.X, _currentReturnLocation.Y);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Dispatch", "success", "Patient loaded. Proceed to the designated base to finish the mission.", new NewNotificationMessageContent[0]));
        }

        private async void CompleteMission()
        {
            if (!_missionActive) return;
            _missionActive = false;
            _incidentSpawned = false;
            _patientLoaded = false;
            _despawnIncidentAfterLoading = false;
            API.ClearGpsMultiRoute();
            API.ClearGpsPlayerWaypoint();
            if (_spawnedAmbulance != null && _spawnedAmbulance.Exists())
            {
                _spawnedAmbulance.Delete();
                _spawnedAmbulance = null;
            }
            if (_spawnedParamedic != null && _spawnedParamedic.Exists())
            {
                _spawnedParamedic.Delete();
                _spawnedParamedic = null;
            }
            if (_spawnedPatient != null && _spawnedPatient.Exists())
            {
                _spawnedPatient.Delete();
                _spawnedPatient = null;
            }
            await AddHemsScore(15);
            int score = GetHemsScore();
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Dispatch", "success", "Mission complete. Patient delivered safely.", new NewNotificationMessageContent[0]));
            await Delay(250);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Dispatch", "success", $"You now have a reputation score of: {score}!", new NewNotificationMessageContent[0]));
        }

        private void CancelMission()
        {
            if (!_missionActive) return;
            _missionActive = false;
            _incidentSpawned = false;
            _patientLoaded = false;
            _despawnIncidentAfterLoading = false;
            API.ClearGpsMultiRoute();
            API.ClearGpsPlayerWaypoint();
            if (_spawnedAmbulance != null && _spawnedAmbulance.Exists())
            {
                _spawnedAmbulance.Delete();
                _spawnedAmbulance = null;
            }
            if (_spawnedParamedic != null && _spawnedParamedic.Exists())
            {
                _spawnedParamedic.Delete();
                _spawnedParamedic = null;
            }
            if (_spawnedPatient != null && _spawnedPatient.Exists())
            {
                _spawnedPatient.Delete();
                _spawnedPatient = null;
            }
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("HEMS Dispatch", "error", "Mission canceled.", new NewNotificationMessageContent[0]));
        }

        private int GetHemsScore()
        {
            hemsScore = API.GetResourceKvpInt("player_score_hems");
            return hemsScore;
        }

        private async Task AddHemsScore(int scoreToAdd)
        {
            int currentScore = API.GetResourceKvpInt("player_score_hems");
            int newScore = currentScore + scoreToAdd;
            API.SetResourceKvpInt("player_score_hems", newScore);
        }

        private async Task RemoveHemsScore(int scoreToRemove)
        {
            int currentScore = API.GetResourceKvpInt("player_score_hems");
            int newScore = currentScore - scoreToRemove;
            API.SetResourceKvpInt("player_score_hems", newScore);
        }

        private string GetLastDailyBonusDateHems()
        {
            return API.GetResourceKvpString("HemsLastDailyBonusDate") ?? "";
        }

        private void SetLastDailyBonusDateHems(string newDate)
        {
            API.SetResourceKvp("HemsLastDailyBonusDate", newDate);
        }
    }
}