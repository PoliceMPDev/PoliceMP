using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.NHS
{
    public class NHSblood : Script
    {
        private ILogger<NHSblood> _logger;
        private ICommandManager _commandManager;
        private IPermissionService _permissionService;
        private readonly ISoundService _soundService;
        private UserAces _userAces;
        private ITickManager _tickManager;
        private readonly IFeatureService _featureService;
        private readonly IPlayerListAccessor _playerListAccessor;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private Menu _BloodMenu = new("LHS Blood Team");

        // Delivery points for blood teams
        private Vector3 _StThomas = new(351.6978f, -588.3935f, 28.79687f);
        private Vector3 _MtZonah = new(-435.7615f, -326.4781f, 34.91076f);
        private Vector3 _victoraMC = new(-251.0961f, 6328.521f, 32.45869f);
        private Vector3 _RoyalLondon = new(1125.304f, -1528.955f, 35.03268f);
        private Vector3 _sandyHosp = new(1832.006f, 3674.494f, 34.27486f);
        private Vector3 _kortzCentre = new(-2290.5778f, 365.01037f, 174.6017f);

        private bool _parcel = false;
        private Ped _player;

        private string randomSpecimen;
        private int timerSecsRem = 0;
        private int startingTime = 0;
        private bool crashDetected = false;
        private bool noClipped = false;
        private string deliveringTo;
        private decimal currentTemperature;
        private int bloodScore;
        private string lastCachedDate;
        private float voiceVolume = 0.85f;

        public NHSblood(ILogger<NHSblood> logger, ICommandManager commandManager, IPermissionService permissionService,
            ISoundService soundService, ITickManager tickManager, IPlayerListAccessor playerListAccessor,
            INewNotificationOverlay newNotificationOverlay, IFeatureService featureService)
        {
            _logger = logger;
            _commandManager = commandManager;
            _permissionService = permissionService;
            _tickManager = tickManager;
            _playerListAccessor = playerListAccessor;
            _soundService = soundService;
            _featureService = featureService;
            _newNotificationOverlay = newNotificationOverlay;
            _tickManager.On(CreateBlip);
            _tickManager.On(PropOnCar);
            _tickManager.Off(BloodDlcTimer);
            _tickManager.Off(CrashDetector);

            _commandManager.Register("helmetup").WithHandler(BikeHelmetUp);
            _commandManager.Register("helmetdown").WithHandler(BikeHelmetDown);
            _commandManager.Register("controlopen").WithHandler(control);
            _commandManager.Register("controlclose").WithHandler(controlClosed);

            _commandManager.Register("bcmocr").WithHandler(CancelOnChangeRole);

            _commandManager.Register("openambo").WithHandler(OpenAmbo);
            _commandManager.Register("closeambo").WithHandler(CloseAmbo);

            _commandManager.Register("openipv").WithHandler(Openipv);
            _commandManager.Register("closeipv").WithHandler(Closeipv);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            MenuController.AddMenu(_BloodMenu);
        }

        private readonly List<Vector3> _bloodbanks = new() // Blips for Blood banks go here
        {
            // St Thomas
            new Vector3(351.6978f, -588.3935f, 28.79687f),
            // Mt Zonah 
            new Vector3(-435.7615f, -326.4781f, 34.91076f),
            // Victoria Medical Centre
            new Vector3(-251.0961f, 6328.521f, 32.45869f),
            // Sandy Hospital
            new Vector3(1832.006f, 3674.494f, 34.27486f),
            // Royal london
            new Vector3(1125.304f, -1528.955f, 35.03268f),
            // Kortz Centre
            new Vector3(-2290.5778f, 365.01037f, 174.6017f),
        };

        public async Task CreateBlip()
        {
            _userAces = await _permissionService.GetUserAces();
            if (_permissionService.CurrentUserRole == null) return; // prevent console spam
            if (_permissionService.CurrentUserRole.Division != UserDivision.blood) return;

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.HasNhsBloodDlc || _userAces.IsDigitalTeam)
            {
                Vector3 playerPos = Game.PlayerPed.Position;

                foreach (Vector3 bloodBank in _bloodbanks)
                {
                    var distance = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, bloodBank.X,
                        bloodBank.Y, bloodBank.Z, true);
                    var scale = 0.1F * API.GetGameplayCamFov();

                    if (distance < 5.0f)
                    {
                        API.DrawMarker(1, bloodBank.X, bloodBank.Y, bloodBank.Z - 1, 0, 0, 0, 0, 0, 0, 1F, 1F, 2F, 232,
                            232, 0, 50,
                            false, true, 2, false, null, null, false);
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
                        API.AddTextComponentString($"LHS Blood Bank");
                        API.SetDrawOrigin(bloodBank.X, bloodBank.Y, bloodBank.Z + 1F, 0);
                        API.DrawText(0, 0);
                        API.ClearDrawOrigin();

                        if (!(distance < 1.0f)) continue;

                        BloodMenu();
                    }
                }

                return;
            }
        }


        public void BloodMenu()
        {
            if (MenuController.IsAnyMenuOpen()) return;
            _BloodMenu.ClearMenuItems();
            _BloodMenu.OpenMenu();
            var GenerateCallout = new MenuItem("Start Mission");
            var Deliver = new MenuItem("Deliver Blood Box");
            var Reputation = new MenuItem("Show Driver Reputation", "See your reputation score");
            var DailyBonus = new MenuItem("Collect Daily Bonus", "Recieve extra points daily");
            var VoicelineVolume = new MenuItem("Voiceline Volume", "Adjust the volume of the voicelines");

            _BloodMenu.AddMenuItem(GenerateCallout);
            _BloodMenu.AddMenuItem(Deliver);
            _BloodMenu.AddMenuItem(Reputation);
            _BloodMenu.AddMenuItem(VoicelineVolume);

            GetLastDailyBonusDate();
            var currentDate = DateTime.Today;
            var strCurrentDate = currentDate.ToString();
            if (lastCachedDate != strCurrentDate)
            {
                _BloodMenu.AddMenuItem(DailyBonus);
            }

            _BloodMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item == GenerateCallout)
                {
                    CalloutHandler();
                }

                if (item == VoicelineVolume)
                {
                    API.AddTextEntry("FMMC_KEY_TIP1", "Enter Voiceline Volume (0-100)");
                    API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "" + voiceVolume * 100, "", "", "", 8);

                    API.UpdateOnscreenKeyboard();

                    while (API.UpdateOnscreenKeyboard() == 0)
                    {
                        await Delay(10);
                        API.UpdateOnscreenKeyboard();
                    }

                    if (API.UpdateOnscreenKeyboard() == 1)
                    {
                        var volumeStr = API.GetOnscreenKeyboardResult();
                        if (!int.TryParse(volumeStr, out int volumeInt))
                        {
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team",
                                "error",
                                $"Error updating your selected volume, ensure you have chosen a number between 0 and 100.",
                                new NewNotificationMessageContent[0]));
                            return;
                        }

                        voiceVolume = volumeInt / 100f;
                        _logger.Debug($"VOICE VOLUME HAS BEEN SET TO {voiceVolume}");
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team", "success",
                            $"You have set the voiceline volume to: {volumeInt}%",
                            new NewNotificationMessageContent[0]));
                    }
                }

                if (item == Reputation)
                {
                    GetBloodScore();
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Reputation",
                        "info",
                        $"Based on your previous transports and success rate, you have a reputation score of: {bloodScore}!",
                        new NewNotificationMessageContent[0]));
                }

                if (item == DailyBonus)
                {
                    GetLastDailyBonusDate();
                    await Delay(200);
                    var currentDate1 = DateTime.Today;
                    var strCurrentDate1 = currentDate1.ToString();
                    if (lastCachedDate == strCurrentDate1)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Reputation",
                            "error",
                            $"Hmmm... It seems you have already claimed your daily bonus today! Come back tomorrow!",
                            new NewNotificationMessageContent[0]));
                        return;
                    }

                    GetBloodScore();
                    await Delay(150);
                    API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                    AddBloodScore(30);
                    GetBloodScore();
                    await Delay(250);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Reputation",
                        "sucess",
                        $"You have recieved a Blood Team Daily Bonus! You now have a reputation score of: {bloodScore}",
                        new NewNotificationMessageContent[0]));
                    SetLastDailyBonusDate(strCurrentDate);
                    _BloodMenu.CloseMenu();
                }

                _BloodMenu.CloseMenu();

                if (item == Deliver)
                {
                    if (_parcel == false)
                    {
                        API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher",
                            "error", "You currently do not have a specimen to deliver here, please accept a delivery",
                            new NewNotificationMessageContent[0]));
                        await Delay(3000);
                        return;
                    }

                    if (_parcel == true)
                    {
                        var playerPos = Game.PlayerPed.Position;

                        Dictionary<Vector3, float> distances = new Dictionary<Vector3, float>
                        {
                            { _StThomas, Vector3.Distance(playerPos, _StThomas) },
                            { _MtZonah, Vector3.Distance(playerPos, _MtZonah) },
                            { _victoraMC, Vector3.Distance(playerPos, _victoraMC) },
                            { _RoyalLondon, Vector3.Distance(playerPos, _RoyalLondon) },
                            { _sandyHosp, Vector3.Distance(playerPos, _sandyHosp) },
                            { _kortzCentre, Vector3.Distance(playerPos, _kortzCentre) }
                        };
                        var closestLocation = distances.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

                        Dictionary<Vector3, string> hospitalNames = new Dictionary<Vector3, string>
                        {
                            { _StThomas, "St Thomas" },
                            { _MtZonah, "Mt Zonah" },
                            { _victoraMC, "Victoria ARI" },
                            { _RoyalLondon, "Royal London" },
                            { _sandyHosp, "Sandy Shores" },
                            { _kortzCentre, "Kortz Medical Science Centre" }
                        };

                        string closestHospitalName = hospitalNames[closestLocation];
                        if (deliveringTo != closestHospitalName)
                        {
                            await RandomWrongHospitalVoiceline();
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                                "LHS Blood Team Dispatcher", "error",
                                $"You have delivered this specimen to the wrong hospital. This hospital will take it this time, however you have let the {deliveringTo} hospital down. They were counting on you!",
                                new NewNotificationMessageContent[0]));
                            API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                            _tickManager.Off(BloodDlcTimer);
                            _parcel = false;
                            timerSecsRem = 0;
                            API.ClearGpsMultiRoute();
                            API.ClearGpsPlayerWaypoint();
                            RemoveBloodScore(5);
                            await Delay(3000);
                            GetBloodScore();
                            API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                                "LHS Blood Team Reputation", "error",
                                $"You have lost reputation points. You now have a score of: {bloodScore}!",
                                new NewNotificationMessageContent[0]));
                            return;
                        }

                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher",
                            "success",
                            "You have successfully delivered the specimen. The delivery box has been removed from the vehicle",
                            new NewNotificationMessageContent[0]));
                        _tickManager.Off(BloodDlcTimer);
                        API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                        _parcel = false;
                        await Delay(3000);
                        timerSecsRem = 0;
                        API.ClearGpsMultiRoute();
                        API.ClearGpsPlayerWaypoint();
                        AddBloodScore(35);
                        await Delay(300);
                        if (currentTemperature < 7) AddBloodScore(25);
                        await Delay(3000);
                        GetBloodScore();
                        API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Reputation",
                            "success", $"You have gained reputation points. You now have a score of: {bloodScore}!",
                            new NewNotificationMessageContent[0]));
                    }
                }
            };
            return;
        }

        private async void CalloutTimer()
        {
            _tickManager.On(BloodDlcTimer);
            _tickManager.On(CrashDetector);
            crashDetected = false;
            noClipped = false;
            var player = Game.Player;
            var vehicle = player.Character.LastVehicle;
            vehicle.Repair();

            decimal tempIncreasePerSecond = 10M / startingTime;

            while (timerSecsRem > 0 && !crashDetected && !noClipped)
            {
                await Delay(1000);
                timerSecsRem = timerSecsRem - 1;
                currentTemperature += tempIncreasePerSecond;
                currentTemperature = Math.Min(currentTemperature, 10);
            }

            _tickManager.Off(BloodDlcTimer);
            _tickManager.Off(CrashDetector);

            if (crashDetected)
            {
                await RandomCrashVoiceline();
                API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher",
                    "error", "You have crashed! The specimen is now damaged and cannot be used. Go careful next time!",
                    new NewNotificationMessageContent[0]));
                _parcel = false;
                crashDetected = false;
                RemoveBloodScore(20);
                await Delay(3000);
                GetBloodScore();
                API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Reputation",
                    "error", $"You have lost reputation points. You now have a score of: {bloodScore}!",
                    new NewNotificationMessageContent[0]));
                return;
            }

            if (noClipped)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher",
                    "error", "This mission has been cancelled!", new NewNotificationMessageContent[0]));
                _parcel = false;
                return;
            }

            if (!_parcel) return;
            await Delay(1000);
            if (!_parcel) return;
            await RandomTimelimitVoiceline();
            API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher", "error",
                "You have ran out of time. The hospital needed this specimen sooner! It's too late, you can stand down.",
                new NewNotificationMessageContent[0]));
            _parcel = false;
            RemoveBloodScore(10);
            await Delay(3000);
            GetBloodScore();
            API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Reputation", "error",
                $"You have lost reputation points. You now have a score of: {bloodScore}!",
                new NewNotificationMessageContent[0]));
            return;
        }

        private async void CancelOnChangeRole()
        {
            noClipped = true;
        }

        private async Task CrashDetector()
        {
            await Delay(1000);

            var player = Game.Player;
            var vehicle = player.Character.CurrentVehicle;

            if (vehicle != null && vehicle.IsDamaged)
            {
                crashDetected = true;
            }

            if (AdminMenu.NoClipActive) noClipped = true;
        }

        private async Task BloodDlcTimer()
        {
            var posX = 0.17f;
            var posY = 0.915f;
            var scale = 0.8f;
            var font = 4;

            var roundedCurrentTemp = Math.Round(currentTemperature, 1);

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");

            if (currentTemperature < 5)
                API.AddTextComponentSubstringPlayerName($"~g~{randomSpecimen} Temperature: ~g~{roundedCurrentTemp}°c");
            if (currentTemperature >= 5 && currentTemperature < 7)
                API.AddTextComponentSubstringPlayerName($"~g~{randomSpecimen} Temperature: ~y~{roundedCurrentTemp}°c");
            if (currentTemperature >= 7)
                API.AddTextComponentSubstringPlayerName($"~g~{randomSpecimen} Temperature: ~r~{roundedCurrentTemp}°c");

            API.EndTextCommandDisplayText(posX, posY);
        }

        private async void GetLastDailyBonusDate()
        {
            var cachedDate = API.GetResourceKvpString("BloodLastDailyBonusDate");
            lastCachedDate = cachedDate;
        }

        private async void SetLastDailyBonusDate(string newDate)
        {
            GetLastDailyBonusDate();
            await Delay(100);
            API.SetResourceKvp("BloodLastDailyBonusDate", newDate);
        }

        private void GetBloodScore()
        {
            var cahcedScore = API.GetResourceKvpInt("player_score");
            bloodScore = cahcedScore;
            return;
        }

        private async void AddBloodScore(int scoreToAdd)
        {
            var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
            var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);

            if (isEaServer || isDevServer) return;

            GetBloodScore();
            await Delay(100);
            var score = bloodScore + scoreToAdd;
            API.SetResourceKvpInt("player_score", score);
        }

        private async void RemoveBloodScore(int scoreToRemove)
        {
            var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
            var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);

            if (isEaServer || isDevServer) return;
            GetBloodScore();
            await Delay(100);
            var score = bloodScore - scoreToRemove;
            API.SetResourceKvpInt("player_score", score);
        }


        public async void CalloutHandler()
        {
            var currentCar = Game.PlayerPed.CurrentVehicle;
            var LastCar = Game.PlayerPed.LastVehicle;

            if (LastCar == null)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher",
                    "error", "Please take out a blood car/bike before starting a job",
                    new NewNotificationMessageContent[0]));
                return;
            }

            if (_parcel == true)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher",
                    "error", "Please deliver your specimen first before taking a new delivery",
                    new NewNotificationMessageContent[0]));
                API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                return;
            }

            string[] possibleSpecimens = { "Heart", "Kidney", "Blood", "Liver", "Lungs", "Cornea" };
            Random random = new Random();
            int randomIndex = random.Next(possibleSpecimens.Length);
            randomSpecimen = possibleSpecimens[randomIndex];

            var playerPos = Game.PlayerPed.Position;
            Dictionary<Vector3, float> distances = new Dictionary<Vector3, float>
            {
                { _StThomas, Vector3.Distance(playerPos, _StThomas) },
                { _MtZonah, Vector3.Distance(playerPos, _MtZonah) },
                { _victoraMC, Vector3.Distance(playerPos, _victoraMC) },
                { _RoyalLondon, Vector3.Distance(playerPos, _RoyalLondon) },
                { _sandyHosp, Vector3.Distance(playerPos, _sandyHosp) }
            };

            var closestLocation = distances.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
            List<string> possibleHospitals = new List<string>
            {
                "St Thomas", "Mt Zonah", "Victoria ARI", "Sandy Shores", "Royal London", "Kortz Medical Science Centre"
            };

            Dictionary<Vector3, string> hospitalNames = new Dictionary<Vector3, string>
            {
                { _StThomas, "St Thomas" },
                { _MtZonah, "Mt Zonah" },
                { _victoraMC, "Victoria ARI" },
                { _RoyalLondon, "Royal London" },
                { _sandyHosp, "Sandy Shores" },
                { _kortzCentre, "Kortz Medical Science Centre" }
            };

            string closestHospitalName = hospitalNames[closestLocation];
            possibleHospitals.Remove(closestHospitalName);
            Random randomHos = new Random();
            int randomHosIndex = randomHos.Next(possibleHospitals.Count);
            string randomHospital = possibleHospitals[randomHosIndex];

            deliveringTo = randomHospital;

            API.ClearGpsMultiRoute();
            API.ClearGpsPlayerWaypoint();
            timerSecsRem = 0;
            startingTime = 0;
            currentTemperature = 0;
            var randomOffset = 0;

            switch (randomIndex)
            {
                case 0: // Heart
                    timerSecsRem = 250;
                    randomOffset = random.Next(-45, 45);
                    timerSecsRem = timerSecsRem + randomOffset;
                    break;
                case 1: // Kidney
                    timerSecsRem = 300;
                    randomOffset = random.Next(-45, 45);
                    timerSecsRem = timerSecsRem + randomOffset;
                    break;
                case 2: // Blood
                    timerSecsRem = 350;
                    randomOffset = random.Next(-45, 45);
                    timerSecsRem = timerSecsRem + randomOffset;
                    break;
                case 3: // Liver
                    timerSecsRem = 300;
                    randomOffset = random.Next(-45, 45);
                    timerSecsRem = timerSecsRem + randomOffset;
                    break;
                case 4: // Lungs
                    timerSecsRem = 265;
                    randomOffset = random.Next(-45, 45);
                    timerSecsRem = timerSecsRem + randomOffset;
                    break;
                case 5: // Cornea
                    timerSecsRem = 370;
                    randomOffset = random.Next(-45, 45);
                    timerSecsRem = timerSecsRem + randomOffset;
                    break;
            }

            startingTime = timerSecsRem;

            switch (deliveringTo)
            {
                case "St Thomas":
                    API.SetNewWaypoint(_StThomas.X, _StThomas.Y);
                    _soundService.Play("BloodStThomasNew.wav", voiceVolume);
                    break;
                case "Mt Zonah":
                    API.SetNewWaypoint(_MtZonah.X, _MtZonah.Y);
                    _soundService.Play("BloodMtZonahNew.wav", voiceVolume);
                    break;
                case "Victoria ARI":
                    API.SetNewWaypoint(_victoraMC.X, _victoraMC.Y);
                    _soundService.Play("BloodVictoriaNew.wav", voiceVolume);
                    break;
                case "Sandy Shores":
                    API.SetNewWaypoint(_sandyHosp.X, _sandyHosp.Y);
                    _soundService.Play("BloodSandyNew.wav", voiceVolume);
                    break;
                case "Royal London":
                    API.SetNewWaypoint(_RoyalLondon.X, _RoyalLondon.Y);
                    _soundService.Play("BloodRoyalLondonNew.wav", voiceVolume);
                    break;
                case "Kortz Medical Science Centre":
                    API.SetNewWaypoint(_kortzCentre.X, _kortzCentre.Y);
                    _soundService.Play("BloodKortzNew.wav", voiceVolume);
                    timerSecsRem = timerSecsRem + 150;
                    break;
            }

            API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);

            if (deliveringTo == "Kortz Medical Science Centre")
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher",
                    "success",
                    $"You have collected a {randomSpecimen} transplant package that needs delivered to {randomHospital} for sample testing!",
                    new NewNotificationMessageContent[0]));
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher", "info",
                    "The package has been placed in your vehicle and location uploaded to your in vehilce GPS, please deliver with care! You have much more time than usual to deliver this.",
                    new NewNotificationMessageContent[0]));
            }
            else
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher",
                    "success",
                    $"You have collected a {randomSpecimen} transplant package that needs delivered to {randomHospital} urgently!",
                    new NewNotificationMessageContent[0]));
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("LHS Blood Team Dispatcher", "info",
                    "The package has been placed in your vehicle and location uploaded to your in vehilce GPS, please deliver on grade 1!",
                    new NewNotificationMessageContent[0]));
            }

            _parcel = true;

            await PropOnCar();

            CalloutTimer();
            return;
        }

        private async Task RandomCrashVoiceline()
        {
            Random rnd = new Random();
            int audioChoice = rnd.Next(3); // Generates a number between 0 and 2

            switch (audioChoice)
            {
                case 0:
                    _soundService.Play("BloodCrashEng.wav", voiceVolume);
                    break;
                case 1:
                    _soundService.Play("BloodCrashWel.wav", voiceVolume);
                    break;
                case 2:
                    _soundService.Play("BloodCrashSco.wav", voiceVolume);
                    break;
            }
        }

        private async Task RandomTimelimitVoiceline()
        {
            Random rnd = new Random();
            int audioChoice = rnd.Next(3);

            switch (audioChoice)
            {
                case 0:
                    _soundService.Play("BloodOOTEng.wav", voiceVolume);
                    break;
                case 1:
                    _soundService.Play("BloodOOTWel.wav", voiceVolume);
                    break;
                case 2:
                    _soundService.Play("BloodOOTSco.wav", voiceVolume);
                    break;
            }
        }

        private async Task RandomWrongHospitalVoiceline()
        {
            Random rnd = new Random();
            int audioChoice = rnd.Next(3);

            switch (audioChoice)
            {
                case 0:
                    _soundService.Play("BloodWrongHosEng.wav", voiceVolume);
                    break;
                case 1:
                    _soundService.Play("BloodWrongHosWel.wav", voiceVolume);
                    break;
                case 2:
                    _soundService.Play("BloodWrongHosSco.wav", voiceVolume);
                    break;
            }
        }


        public async Task PropOnCar()
        {
            var currentCar = Game.PlayerPed.LastVehicle;
            if (currentCar == null) return;

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.HasNhsBloodDlc || _userAces.IsDigitalTeam)
            {
                if (_parcel == true)
                {
                    API.SetVehicleExtra(currentCar.Handle, 11, false);
                    await Delay(0);
                }
                else
                {
                    API.SetVehicleExtra(currentCar.Handle, 11, true);
                    await Delay(0);
                }

                ;
            }
        }

        public async Task BikeHelmetUp()
        {
            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.HasNhsBloodDlc || _userAces.IsDigitalTeam ||
                _userAces.IsRpuTrained)
            {
                var ped = Game.PlayerPed.Handle;
                var rpuHelmetDown = 221;
                var rpuHelmetUp = 222;
                var currentUserRole = _permissionService.CurrentUserRole;

                if (currentUserRole.Division == UserDivision.Rpu)
                {
                    if (API.GetPedPropIndex(ped, 0) == rpuHelmetDown)
                    {
                        API.SetPedPropIndex(ped, 0, rpuHelmetUp, 0, true);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helmet Visor", "success",
                            "You've lifted your helmet!", new NewNotificationMessageContent[0]));
                        _logger.Debug("Helmet Lifted");
                        await Delay(0);
                        return;
                    }
                }
                else if (API.GetPedPropIndex(ped, 0) == 221)
                {
                    if (!_userAces.HasNhsBloodDlc && !_userAces.IsDeveloper && !_userAces.IsAdmin &&
                        !_userAces.IsDigitalTeam)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helmet Visor", "error",
                            "This feature requires the LHS Blood DLC!", new NewNotificationMessageContent[0]));
                    }

                    API.SetPedPropIndex(ped, 0, 222, 1, true);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helmet Visor", "success",
                        "You've lifted your helmet!", new NewNotificationMessageContent[0]));
                    _logger.Debug("Helmet Lifted");
                    await Delay(0);
                    return;
                }

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helmet Visor", "error",
                    "You must be wearing a helmet with the visor down to use this!",
                    new NewNotificationMessageContent[0]));
            }
        }

        public async Task BikeHelmetDown()
        {
            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.HasNhsBloodDlc || _userAces.IsDigitalTeam ||
                _userAces.IsRpuTrained)
            {
                var ped = Game.PlayerPed.Handle;
                var rpuHelmetDown = 221;
                var rpuHelmetUp = 222;
                var currentUserRole = _permissionService.CurrentUserRole;

                if (currentUserRole.Division == UserDivision.Rpu)
                {
                    if (API.GetPedPropIndex(ped, 0) == rpuHelmetUp)
                    {
                        API.SetPedPropIndex(ped, 0, rpuHelmetDown, 0, true);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helmet Visor", "success",
                            "You've put down your helmet!", new NewNotificationMessageContent[0]));
                        _logger.Debug("Helmet Closed");
                        await Delay(0);
                        return;
                    }
                }
                else if (API.GetPedPropIndex(ped, 0) == 222)
                {
                    if (!_userAces.HasNhsBloodDlc && !_userAces.IsDeveloper && !_userAces.IsAdmin &&
                        !_userAces.IsDigitalTeam)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helmet Visor", "error",
                            "This feature requires the LHS Blood DLC!", new NewNotificationMessageContent[0]));
                    }

                    API.SetPedPropIndex(ped, 0, 221, 1, true);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helmet Visor", "success",
                        "You've put down your helmet!", new NewNotificationMessageContent[0]));
                    _logger.Debug("Helmet Closed");
                    await Delay(0);
                    return;
                }

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helmet Visor", "error",
                    "You must be wearing a helmet with the visor up use this!", new NewNotificationMessageContent[0]));
            }
        }

        public async void OpenAmbo()
        {
            var currentCar = Game.PlayerPed.LastVehicle;

            if (currentCar == null) return;
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Nhs || _userAces.IsDeveloper)
            {
                //API.SetVehicleFixed(currentCar.Handle);
                API.SetVehicleDoorOpen(currentCar.Handle, 2, false, false);
                API.SetVehicleDoorOpen(currentCar.Handle, 3, false, false);
                API.SetVehicleDoorOpen(currentCar.Handle, 4, false, false);
                API.SetVehicleDoorOpen(currentCar.Handle, 5, false, false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Ambulance", "success",
                    "Ambulance rear doors open!", new NewNotificationMessageContent[0]));
                await Delay(5000);
            }
            else return;
        }

        public async void CloseAmbo()
        {
            var currentCar = Game.PlayerPed.LastVehicle;

            if (currentCar == null) return;
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Nhs || _userAces.IsDeveloper)
            {
                API.SetVehicleDoorShut(currentCar.Handle, 2, false);
                API.SetVehicleDoorShut(currentCar.Handle, 3, false);
                API.SetVehicleDoorShut(currentCar.Handle, 4, false);
                API.SetVehicleDoorShut(currentCar.Handle, 5, false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Ambulance", "success",
                    "Ambulance rear doors closed!", new NewNotificationMessageContent[0]));
                await Delay(5000);
            }
            else return;
        }


        public async Task control()
        {
            var currentCar = Game.PlayerPed.LastVehicle;

            if (currentCar == null) return;
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Control || _userAces.IsDeveloper)
            {
                API.SetVehicleExtra(currentCar.Handle, 12, false);
                API.SetVehicleFixed(currentCar.Handle);
                API.SetVehicleDoorOpen(currentCar.Handle, 2, false, false);
                API.SetVehicleDoorOpen(currentCar.Handle, 3, false, false);
                API.SetVehicleDoorOpen(currentCar.Handle, 4, false, false);
                API.SetVehicleDoorOpen(currentCar.Handle, 5, false, false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Control Panel", "success",
                    "You can now enter the control centre!", new NewNotificationMessageContent[0]));
                await Delay(5000);
            }
            else return;
        }

        public async Task controlClosed()
        {
            var currentCar = Game.PlayerPed.LastVehicle;

            if (currentCar == null) return;
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Control || _userAces.IsDeveloper)
            {
                API.SetVehicleDoorShut(currentCar.Handle, 2, false);
                API.SetVehicleDoorShut(currentCar.Handle, 3, false);
                API.SetVehicleDoorShut(currentCar.Handle, 4, false);
                API.SetVehicleDoorShut(currentCar.Handle, 5, false);
                API.SetVehicleExtra(currentCar.Handle, 12, true);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Control Panel", "success",
                    "You can drive the control room", new NewNotificationMessageContent[0]));
                await Delay(5000);
            }
            else return;
        }

        public async void Openipv()
        {
            var currentCar = Game.PlayerPed.LastVehicle;

            if (currentCar == null) return;
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Highways || _userAces.IsDeveloper)
            {
                API.SetVehicleFixed(currentCar.Handle);
                API.SetVehicleDoorOpen(currentCar.Handle, 2, false, false);
                API.SetVehicleDoorOpen(currentCar.Handle, 3, false, false);
                API.SetVehicleDoorOpen(currentCar.Handle, 4, false, false);
                API.SetVehicleDoorOpen(currentCar.Handle, 5, false, false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("IPV", "success", "IPV Setup!",
                    new NewNotificationMessageContent[0]));
                await Delay(5000);
            }
            else return;
        }

        public async void Closeipv()
        {
            var currentCar = Game.PlayerPed.LastVehicle;

            if (currentCar == null) return;
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Highways || _userAces.IsDeveloper)
            {
                API.SetVehicleDoorShut(currentCar.Handle, 2, false);
                API.SetVehicleDoorShut(currentCar.Handle, 3, false);
                API.SetVehicleDoorShut(currentCar.Handle, 4, false);
                API.SetVehicleDoorShut(currentCar.Handle, 5, false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("IPV", "success", "IPV Setup!",
                    new NewNotificationMessageContent[0]));
                await Delay(5000);
            }
            else return;
        }
    }
}