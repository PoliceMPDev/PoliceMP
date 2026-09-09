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

namespace PoliceMP.Client.Scripts.CoronerCalls
{
    public class CoronerCalls : Script
    {
        private ILogger<CoronerCalls> _logger;
        private ICommandManager _commandManager;
        private IPermissionService _permissionService;
        private readonly ISoundService _soundService;
        private UserAces _userAces;
        private ITickManager _tickManager;
        private readonly IFeatureService _featureService;
        private readonly IPlayerListAccessor _playerListAccessor;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private Menu _CoronerMenu = new("Coroner Team");

        // Pickup Locations for Bodies
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
        private int coronerScore;
        private string lastCachedDate;
        private float voiceVolume = 0.85f;


        public CoronerCalls(ILogger<CoronerCalls> logger, ICommandManager commandManager, IPermissionService permissionService,
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
            _tickManager.Off(CoronerDLCTimer);
            _tickManager.Off(CrashDetector);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            MenuController.AddMenu(_CoronerMenu);
        }

        private readonly List<Vector3> _hospitalPoints = new() // Blips for pickup points here
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
            if (_permissionService.CurrentUserRole.Division != UserDivision.blood) return; //CHANGE TO CORONER PERMS

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.HasNhsBloodDlc || _userAces.IsDigitalTeam)
            {
                Vector3 playerPos = Game.PlayerPed.Position;

                foreach (Vector3 hospitalPoint in _hospitalPoints)
                {
                    var distance = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, hospitalPoint.X,
                        hospitalPoint.Y, hospitalPoint.Z, true);
                    var scale = 0.1F * API.GetGameplayCamFov();

                    if (distance < 5.0f)
                    {
                        API.DrawMarker(1, hospitalPoint.X, hospitalPoint.Y, hospitalPoint.Z - 1, 0, 0, 0, 0, 0, 0, 1F, 1F, 2F, 232,
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
                        API.AddTextComponentString($"Coroner Pickup");
                        API.SetDrawOrigin(hospitalPoint.X, hospitalPoint.Y, hospitalPoint.Z + 1F, 0);
                        API.DrawText(0, 0);
                        API.ClearDrawOrigin();

                        if (!(distance < 1.0f)) continue;

                        CoronerMenu();
                    }
                }

                return;
            }
        }

        public void CoronerMenu()
        {
            if (MenuController.IsAnyMenuOpen()) return;
            _CoronerMenu.ClearMenuItems();
            _CoronerMenu.OpenMenu();
            var GenerateCallout = new MenuItem("Start Mission");
            var Deliver = new MenuItem("Deliver Body");
            var Reputation = new MenuItem("Show Driver Reputation", "See your reputation score");
            var DailyBonus = new MenuItem("Collect Daily Bonus", "Recieve extra points daily");
            var VoicelineVolume = new MenuItem("Voiceline Volume", "Adjust the volume of the voicelines");

            _CoronerMenu.AddMenuItem(GenerateCallout);
            _CoronerMenu.AddMenuItem(Deliver);
            _CoronerMenu.AddMenuItem(Reputation);
            _CoronerMenu.AddMenuItem(VoicelineVolume);

            GetLastDailyBonusDate();
            var currentDate = DateTime.Today;
            var strCurrentDate = currentDate.ToString();
            if (lastCachedDate != strCurrentDate)
            {
                _CoronerMenu.AddMenuItem(DailyBonus);
            }

            _CoronerMenu.OnItemSelect += async (menu, item, index) =>
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
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team",
                                "error",
                                $"Error updating your selected volume, ensure you have chosen a number between 0 and 100.",
                                new NewNotificationMessageContent[0]));
                            return;
                        }

                        voiceVolume = volumeInt / 100f;
                        _logger.Debug($"VOICE VOLUME HAS BEEN SET TO {voiceVolume}");
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team", "success",
                            $"You have set the voiceline volume to: {volumeInt}%",
                            new NewNotificationMessageContent[0]));
                    }
                }

                if (item == Reputation)
                {
                    GetCoronerScore();
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Reputation",
                        "info",
                        $"Based on your previous transports and success rate, you have a reputation score of: {coronerScore}!",
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
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Reputation",
                            "error",
                            $"Hmmm... It seems you have already claimed your daily bonus today! Come back tomorrow!",
                            new NewNotificationMessageContent[0]));
                        return;
                    }

                    GetCoronerScore();
                    await Delay(150);
                    API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                    AddCoronerScore(30);
                    GetCoronerScore();
                    await Delay(250);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Reputation",
                        "sucess",
                        $"You have recieved a Coroner Team Daily Bonus! You now have a reputation score of: {coronerScore}",
                        new NewNotificationMessageContent[0]));
                    SetLastDailyBonusDate(strCurrentDate);
                    _CoronerMenu.CloseMenu();
                }

                _CoronerMenu.CloseMenu();

                if (item == Deliver)
                {
                    if (_parcel == false)
                    {
                        API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Dispatcher",
                            "error", "You currently do not have a body to deliver here, please accept a delivery",
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
                                "Coroner Team Dispatcher", "error",
                                $"You have delivered this specimen to the wrong Morgue. This Morgue will take it this time, however you have let the {deliveringTo} Morgue down. They were counting on you!",
                                new NewNotificationMessageContent[0]));
                            API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                            _tickManager.Off(CoronerDLCTimer);
                            _parcel = false;
                            timerSecsRem = 0;
                            API.ClearGpsMultiRoute();
                            API.ClearGpsPlayerWaypoint();
                            RemoveCoronerScore(5);
                            await Delay(3000);
                            GetCoronerScore();
                            API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                                "Coroner Team Reputation", "error",
                                $"You have lost reputation points. You now have a score of: {coronerScore}!",
                                new NewNotificationMessageContent[0]));
                            return;
                        }

                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Dispatcher",
                            "success",
                            "You have successfully delivered the body. Well done!",
                            new NewNotificationMessageContent[0]));
                        _tickManager.Off(CoronerDLCTimer);
                        API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                        _parcel = false;
                        await Delay(3000);
                        timerSecsRem = 0;
                        API.ClearGpsMultiRoute();
                        API.ClearGpsPlayerWaypoint();
                        AddCoronerScore(35);
                        await Delay(300);
                        if (currentTemperature < 7) AddCoronerScore(25);
                        await Delay(3000);
                        GetCoronerScore();
                        API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Corner Team Reputation",
                            "success", $"You have gained reputation points. You now have a score of: {coronerScore}!",
                            new NewNotificationMessageContent[0]));
                    }
                }
            };
            return;
        }

        private async void CalloutTimer()
        {
            _tickManager.On(CoronerDLCTimer);
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

            _tickManager.Off(CoronerDLCTimer);
            _tickManager.Off(CrashDetector);

            if (crashDetected)
            {
                await RandomCrashVoiceline();
                API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Dispatcher",
                    "error", "You have crashed! The body is now tampered with and cannot be preserved properly. Go careful next time!",
                    new NewNotificationMessageContent[0]));
                _parcel = false;
                crashDetected = false;
                RemoveCoronerScore(20);
                await Delay(3000);
                GetCoronerScore();
                API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Reputation",
                    "error", $"You have lost reputation points. You now have a score of: {coronerScore}!",
                    new NewNotificationMessageContent[0]));
                return;
            }

            if (noClipped)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Dispatcher",
                    "error", "This mission has been cancelled!", new NewNotificationMessageContent[0]));
                _parcel = false;
                return;
            }

            if (!_parcel) return;
            await Delay(1000);
            if (!_parcel) return;
            await RandomTimelimitVoiceline();
            API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Dispatcher", "error",
                "You have ran out of time. The morgue needed this body sooner before it got too warm! It's too late, you can stand down.",
                new NewNotificationMessageContent[0]));
            _parcel = false;
            RemoveCoronerScore(10);
            await Delay(3000);
            GetCoronerScore();
            API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Coroner Team Reputation", "error",
                $"You have lost reputation points. You now have a score of: {coronerScore}!",
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

        private async Task CoronerDLCTimer()
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
                API.AddTextComponentSubstringPlayerName($"~g~Body Temperature: ~g~{roundedCurrentTemp}°c");
            if (currentTemperature >= 5 && currentTemperature < 7)
                API.AddTextComponentSubstringPlayerName($"~g~Body Temperature: ~y~{roundedCurrentTemp}°c");
            if (currentTemperature >= 7)
                API.AddTextComponentSubstringPlayerName($"~g~Body Temperature: ~r~{roundedCurrentTemp}°c");

            API.EndTextCommandDisplayText(posX, posY);
        }


        private async void GetLastDailyBonusDate()
        {
            var cachedDate = API.GetResourceKvpString("CoronerLastDailyBonusDate");
            lastCachedDate = cachedDate;
        }

        private async void SetLastDailyBonusDate(string newDate)
        {
            GetLastDailyBonusDate();
            await Delay(100);
            API.SetResourceKvp("CoronerLastDailyBonusDate", newDate);
        }

        private void GetCoronerScore()
        {
            var cahcedScore = API.GetResourceKvpInt("player_score_Coroner");
            coronerScore = cahcedScore;
            return;
        }

        private async void AddCoronerScore(int scoreToAdd)
        {
            var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
            var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);

            if (isEaServer || isDevServer) return;

            GetCoronerScore();
            await Delay(100);
            var score = coronerScore + scoreToAdd;
            API.SetResourceKvpInt("player_score_Coroner", score);
        }

        private async void RemoveCoronerScore(int scoreToRemove)
        {
            var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
            var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);

            if (isEaServer || isDevServer) return;
            GetCoronerScore();
            await Delay(100);
            var score = coronerScore - scoreToRemove;
            API.SetResourceKvpInt("player_score_Coroner", score);
        }
    }
}