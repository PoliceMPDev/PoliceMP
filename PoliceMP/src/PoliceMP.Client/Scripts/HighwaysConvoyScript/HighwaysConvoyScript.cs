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

namespace PoliceMP.Client.Scripts.HighwaysConvoyScript
{
    public class HighwaysConvoyScript : Script
    {
        private ILogger<HighwaysConvoyScript> _logger;
        private ICommandManager _commandManager;
        private IPermissionService _permissionService;
        private readonly ISoundService _soundService;
        private UserAces _userAces;
        private ITickManager _tickManager;
        private readonly IFeatureService _featureService;
        private readonly IPlayerListAccessor _playerListAccessor;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private Menu _convoyMenu = new("PMP Haulage");

        // Delivery points for trailers
        private Vector3 _claymoreWarehouse = new(1219.4792f, -3202.822f, 5.528051f);
        private Vector3 _airportWarehouse = new(-874.4112f, -2734.0527f, 13.887f);
        private Vector3 _murrietaWarehouse = new(1212.283f, -1262.375f, 35.226f);
        private Vector3 _elBurroWarehouse = new(1689.3363f, -1547.99f, 112.6851f);
        private Vector3 _sandyScrapCentre = new(2336.7956f, 3137.4421f, 48.195674f);
        private Vector3 _bellfarmWarehouse = new(145.3579f, 6365.8891f, 31.5292f);
        private Vector3 _grapeseedMarina = new(1320.5714f, 4314.7001f, 38.14345f);
        private Vector3 _laFuentaBlanca = new(1407.72692f, 1115.30139f, 114.83765f);
        private Vector3 _humaneLabs = new(3626.479003f, 3752.24560f, 28.51573f);
        private Vector3 _youTool = new(2681.21435f, 3517.55029f, 52.712047f);

        private Dictionary<Vector3, int> _blips;

        private bool _parcel = false;
        private Ped _player;
        private string trailerSpawncode = "trans-laso_trailer";
        private string randomParcel;
        private bool crashDetected = false;
        private bool noClipped = false;
        private string deliveringTo;
        private int lawsonsScore;
        private string lastCachedDate;
        private float voiceVolume = 0.85f;
        private Vector3 spawnCoords;
        private float spawnHeading;
        private int trailer;
        private int decorBlista;
        private int joinedMissionVehicleHandle = 0;

        public HighwaysConvoyScript(ILogger<HighwaysConvoyScript> logger, ICommandManager commandManager,
            IPermissionService permissionService, ISoundService soundService, ITickManager tickManager,
            IPlayerListAccessor playerListAccessor, INewNotificationOverlay newNotificationOverlay,
            IFeatureService featureService)
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
            _tickManager.Off(CrashDetector);
            _tickManager.Off(LawsonsMissionTimer);
            _tickManager.Off(JoinedMissionStateCheceker);
            _tickManager.Off(LawsonsJoinedMissionTimer);

            API.DecorRegister("lawson_mission_state", 3);

            _commandManager.Register("bcmocr").WithHandler(CancelOnChangeRole);
            _commandManager.Register("debuglawsons").WithHandler(GetPlayerHeading);
            _commandManager.Register("debugdevalhp").WithHandler(DevAddPoints);

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            MenuController.AddMenu(_convoyMenu);
            CreateMapBlips();
        }

        private readonly List<Vector3> _startLocations = new()
        {
            // Claymore Warehouse
            new Vector3(1219.4792f, -3202.822f, 5.528051f),
            // Airport Fedex Warehouse
            new Vector3(-874.4112f, -2734.0527f, 13.887f),
            // Murrieta Warehouse
            new Vector3(1212.283f, -1262.375f, 35.226f),
            // El Burro Warehouse
            new Vector3(1689.3363f, -1547.99f, 112.6851f),
            // Sandy Scrap Centre
            new Vector3(2336.7956f, 3137.4421f, 48.195674f),
            // Bell Farms Warehouse
            new Vector3(145.3579f, 6365.8891f, 31.5292f),
            // Grapeseed Marina
            new Vector3(1320.5714f, 4314.7001f, 38.14345f),
            // La Fuenta Blanca
            new(1407.72692f, 1115.30139f, 114.83765f),
            // Humane Labs
            new(3626.479003f, 3752.24560f, 28.51573f),
            // You Tool
            new(2681.21435f, 3517.55029f, 52.712047f),
        };

        private async void CreateMapBlips()
        {
            var _userAces = await _permissionService.GetUserAces();

            while (_permissionService.CurrentUserRole == null) await Delay(250); // prevent console spam

            if (!_userAces.IsHighwaysTrained) return;
            foreach (Vector3 start in _startLocations)
            {
                var blip = API.AddBlipForCoord(start.X, start.Y, start.Z);
                API.SetBlipSprite(blip, 477); // truck symbol
                API.SetBlipScale(blip, 0.825f);
                API.SetBlipColour(blip, 64); // Orange

                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentSubstringPlayerName("Haulage Warehouse");
                API.EndTextCommandSetBlipName(blip);
            }
        }

        private void GetPlayerHeading()
        {
            var player = Game.PlayerPed.Handle;
            var playerHeading = API.GetEntityHeading(player);
            Debug.WriteLine($"HEADING: {playerHeading}");

            if (API.IsVehicleExtraTurnedOn(trailer, 0)) Debug.WriteLine("EXTRA 0");
            if (API.IsVehicleExtraTurnedOn(trailer, 1)) Debug.WriteLine("EXTRA 1");
            if (API.IsVehicleExtraTurnedOn(trailer, 2)) Debug.WriteLine("EXTRA 2");
            if (API.IsVehicleExtraTurnedOn(trailer, 3)) Debug.WriteLine("EXTRA 3");
            if (API.IsVehicleExtraTurnedOn(trailer, 4)) Debug.WriteLine("EXTRA 4");
            if (API.IsVehicleExtraTurnedOn(trailer, 5)) Debug.WriteLine("EXTRA 5");
            if (API.IsVehicleExtraTurnedOn(trailer, 6)) Debug.WriteLine("EXTRA 6");
            if (API.IsVehicleExtraTurnedOn(trailer, 7)) Debug.WriteLine("EXTRA 7");
        }


        public async Task CreateBlip()
        {
            _userAces = await _permissionService.GetUserAces();
            if (_permissionService.CurrentUserRole == null) return;
            if (_permissionService.CurrentUserRole.Branch != UserBranch.Highways) return;

            if (_userAces.IsAdmin || _userAces.IsDeveloper || _userAces.IsHighwaysTrained || _userAces.IsDigitalTeam)
            {
                Vector3 playerPos = Game.PlayerPed.Position;

                foreach (Vector3 starts in _startLocations)
                {
                    var distance = API.GetDistanceBetweenCoords(playerPos.X, playerPos.Y, playerPos.Z, starts.X,
                        starts.Y, starts.Z, true);
                    var scale = 0.1F * API.GetGameplayCamFov();

                    if (distance < 5.0f)
                    {
                        API.DrawMarker(1, starts.X, starts.Y, starts.Z - 1, 0, 0, 0, 0, 0, 0, 1F, 1F, 2F, 232, 232, 0,
                            50,
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
                        API.AddTextComponentString($"PMP Haulage");
                        API.SetDrawOrigin(starts.X, starts.Y, starts.Z + 1F, 0);
                        API.DrawText(0, 0);
                        API.ClearDrawOrigin();

                        if (!(distance < 1.0f)) continue;

                        ConvoyMenu();
                    }
                }
            }
        }

        private async Task LawsonsMissionTimer()
        {
            var posX = 0.17f;
            var posY = 0.915f;
            var scale = 0.8f;
            var font = 4;

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");

            API.AddTextComponentSubstringPlayerName($"~g~Transporting: {randomParcel}");
            API.EndTextCommandDisplayText(posX, posY);
        }

        private async Task LawsonsJoinedMissionTimer()
        {
            var posX = 0.17f;
            var posY = 0.915f;
            var scale = 0.8f;
            var font = 4;

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");

            API.AddTextComponentSubstringPlayerName($"~g~Escort Vehicle");
            API.EndTextCommandDisplayText(posX, posY);
        }


        public async void ConvoyMenu()
        {
            if (MenuController.IsAnyMenuOpen()) return;
            _convoyMenu.ClearMenuItems();
            _convoyMenu.OpenMenu();
            var GenerateCallout = new MenuItem("Start Mission");
            var Deliver = new MenuItem("Confirm Delivery");
            var Reputation = new MenuItem("Show Driver Reputation", "See your reputation score");
            var DailyBonus = new MenuItem("Collect Daily Bonus", "Recieve extra points daily");
            var VoicelineVolume = new MenuItem("Voiceline Volume", "Adjust the volume of the voicelines");
            var JoinMission = new MenuItem("Join Multiplayer Mission", "Earn more points in multiplayer");

            _convoyMenu.AddMenuItem(GenerateCallout);
            _convoyMenu.AddMenuItem(Deliver);
            _convoyMenu.AddMenuItem(Reputation);
            _convoyMenu.AddMenuItem(VoicelineVolume);

            // LAWSON MISSION STATE DECOR 
            // STATE 0: MISSION NOT STARTED
            // STATE 1: MISSION STARTED
            // STATE 2: MISSION STARTED WITH PLAYERS JOINED
            // STATE 3: MISSION CRASHED
            // STATE 4: MISSION VOIDED
            // STATE 5: MISSION COMPLETED
            // STATE 5: MISSION WRONG LOC

            var playerPos = Game.PlayerPed.Position;
            var player = Game.PlayerPed.Handle;
            int vehicleHandle = API.GetClosestVehicle(playerPos.X, playerPos.Y, playerPos.Z, 25f,
                (uint)API.GetHashKey("blista"), 70);
            if (vehicleHandle != 0)
            {
                Vehicle nearestVehicle = new Vehicle(vehicleHandle);
                var missionState = API.DecorGetInt(vehicleHandle, "lawson_mission_state");
                //if (Game.PlayerPed.LastVehicle.Handle == vehicleHandle) break;

                var trailerNetworkOwner = API.NetworkGetEntityOwner(vehicleHandle);
                var pedNetworkOwner = API.NetworkGetEntityOwner(player);
                var skipJoinOption = false;
                if (trailerNetworkOwner == pedNetworkOwner) skipJoinOption = true;
                if (missionState == 1 && !skipJoinOption)
                {
                    _convoyMenu.AddMenuItem(JoinMission);
                }
            }

            GetLastDailyBonusDate();
            var currentDate = DateTime.Today;
            var strCurrentDate = currentDate.ToString();
            if (lastCachedDate != strCurrentDate)
            {
                _convoyMenu.AddMenuItem(DailyBonus);
            }

            _convoyMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (item == JoinMission)
                {
                    JoinedMission(vehicleHandle);
                }

                if (item == GenerateCallout)
                {
                    if (vehicleHandle != 0 && vehicleHandle != Game.PlayerPed.LastVehicle.Handle)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage", "error",
                            $"It appears another driver is loading here, either join their mission or wait until a slot is free!",
                            new NewNotificationMessageContent[0]));
                        return;
                    }

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
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage",
                                "error",
                                $"Error updating your selected volume, ensure you have chosen a number between 0 and 100.",
                                new NewNotificationMessageContent[0]));
                            return;
                        }

                        voiceVolume = volumeInt / 100f;
                        _logger.Debug($"VOICE VOLUME HAS BEEN SET TO {voiceVolume}");
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage",
                            "success", $"You have set the voiceline volume to: {volumeInt}%",
                            new NewNotificationMessageContent[0]));
                    }
                }

                if (item == Reputation)
                {
                    GetLawsonsScore();
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Reputation",
                        "info",
                        $"Based on your previous deliveries and success rate, you have a reputation score of: {lawsonsScore}!",
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
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                            "PMP Haulage Reputation", "error",
                            $"Hmmm... It seems you have already claimed your daily bonus today! Come back tomorrow!",
                            new NewNotificationMessageContent[0]));
                        return;
                    }

                    GetLawsonsScore();
                    await Delay(150);
                    API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                    AddLawsonsScore(30);
                    GetLawsonsScore();
                    await Delay(250);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Reputation",
                        "success",
                        $"You have recieved a PMP Haulage Team Daily Bonus! You now have a reputation score of: {lawsonsScore}",
                        new NewNotificationMessageContent[0]));
                    SetLastDailyBonusDate(strCurrentDate);
                    _convoyMenu.CloseMenu();
                }

                _convoyMenu.CloseMenu();

                if (item == Deliver)
                {
                    _logger.Debug("Deliver option selected.");
                    if (_parcel == false)
                    {
                        API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                            "PMP Haulage Dispatcher", "error",
                            "You currently do not have a delivery here, please accept a delivery",
                            new NewNotificationMessageContent[0]));
                        await Delay(3000);
                        return;
                    }

                    if (_parcel == true)
                    {
                        _logger.Debug("Parcel == true");

                        var playerPos = Game.PlayerPed.Position;

                        Dictionary<Vector3, float> distances = new Dictionary<Vector3, float>
                        {
                            { _claymoreWarehouse, Vector3.Distance(playerPos, _claymoreWarehouse) },
                            { _airportWarehouse, Vector3.Distance(playerPos, _airportWarehouse) },
                            { _murrietaWarehouse, Vector3.Distance(playerPos, _murrietaWarehouse) },
                            { _elBurroWarehouse, Vector3.Distance(playerPos, _elBurroWarehouse) },
                            { _humaneLabs, Vector3.Distance(playerPos, _humaneLabs) },
                            { _sandyScrapCentre, Vector3.Distance(playerPos, _sandyScrapCentre) },
                            { _bellfarmWarehouse, Vector3.Distance(playerPos, _bellfarmWarehouse) },
                            { _grapeseedMarina, Vector3.Distance(playerPos, _grapeseedMarina) },
                            { _laFuentaBlanca, Vector3.Distance(playerPos, _laFuentaBlanca) },
                            { _youTool, Vector3.Distance(playerPos, _youTool) },
                        };
                        var closestLocation = distances.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

                        _logger.Debug("Closest location decided.");

                        Dictionary<Vector3, string> warehouseNames = new Dictionary<Vector3, string>
                        {
                            { _claymoreWarehouse, "Claymore Warehouse" },
                            { _airportWarehouse, "Fedex Airport Warehouse" },
                            { _murrietaWarehouse, "Murrieta Warehouse" },
                            { _elBurroWarehouse, "El Burro Warehouse" },
                            { _humaneLabs, "Humane Labs" },
                            { _sandyScrapCentre, "Sandy Scrap Centre" },
                            { _bellfarmWarehouse, "Bell Farm Warehouse" },
                            { _grapeseedMarina, "Grapeseed Marina" },
                            { _laFuentaBlanca, "La Fuenta Blanca" },
                            { _youTool, "You Tool" },
                        };

                        string closestWarehouseName = warehouseNames[closestLocation];

                        _logger.Debug("Closest location name grabbed.");

                        if (deliveringTo != closestWarehouseName)
                        {
                            _logger.Debug("Delivered to wrong location.");

                            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                                "PMP Haulage Dispatcher", "error",
                                $"You have delivered this to the wrong location. This warehouse will take it this time, however you have let the {deliveringTo} site down. They were counting on you!",
                                new NewNotificationMessageContent[0]));
                            API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                            _parcel = false;
                            _tickManager.Off(LawsonsMissionTimer);
                            _tickManager.Off(CrashDetector);
                            _soundService.Play("LawsonsWrongLocation.wav", voiceVolume);
                            API.ClearGpsMultiRoute();
                            API.ClearGpsPlayerWaypoint();
                            RemoveLawsonsScore(20);
                            await Delay(3000);
                            UpdateDecorState(6);
                            await Delay(5000);
                            API.DecorSetInt(Game.PlayerPed.LastVehicle.Handle, "lawson_mission_state", 0);
                            GetLawsonsScore();
                            API.DeleteEntity(ref trailer);
                            API.DeleteEntity(ref decorBlista);
                            API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                                "PMP Haulage Reputation", "error",
                                $"You have lost reputation points. You now have a score of: {lawsonsScore}!",
                                new NewNotificationMessageContent[0]));
                            return;
                        }

                        var trailerPos = API.GetEntityCoords(trailer, false);
                        if (API.GetDistanceBetweenCoords(trailerPos.X, trailerPos.Y, trailerPos.Z, playerPos.X,
                                playerPos.Y, playerPos.Z, false) > 30f)
                        {
                            _logger.Debug("Delivered to correct location without trailer.");

                            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                                "PMP Haulage Dispatcher", "error",
                                "You have made it to the warehouse, but where is the trailer? Looks like you have left it behind...",
                                new NewNotificationMessageContent[0]));
                            API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                            _parcel = false;
                            _tickManager.Off(LawsonsMissionTimer);
                            _tickManager.Off(CrashDetector);
                            API.ClearGpsMultiRoute();
                            API.ClearGpsPlayerWaypoint();
                            UpdateDecorState(4);
                            await Delay(5000);
                            API.DeleteEntity(ref trailer);
                            API.DeleteEntity(ref decorBlista);
                            return;
                        }

                        _logger.Debug("Delivered to correct location.");

                        _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                            "PMP Haulage Dispatcher", "success",
                            "You have made a successfully delivery. This site will now take it off your hands for you.",
                            new NewNotificationMessageContent[0]));
                        API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                        _parcel = false;
                        _tickManager.Off(LawsonsMissionTimer);
                        _tickManager.Off(CrashDetector);
                        _soundService.Play("LawsonsDelivered.wav", voiceVolume);
                        await Delay(3000);
                        API.ClearGpsMultiRoute();
                        API.ClearGpsPlayerWaypoint();
                        await Delay(250);

                        var blistaState = API.DecorGetInt(decorBlista, "lawson_mission_state");

                        // POINTS PER PLAYERS JOINED
                        // 30 - Solo
                        // 45 - 2 plyrs
                        // 60 - 3 plyrs
                        // 75 - 4 plyrs
                        // 90 - 5 plyrs

                        switch (blistaState)
                        {
                            case 1:
                                AddLawsonsScore(30);
                                break;
                            case 2:
                                AddLawsonsScore(45);
                                UpdateDecorState(5);
                                break;
                            case 10:
                                AddLawsonsScore(60);
                                UpdateDecorState(15);
                                break;
                            case 11:
                                AddLawsonsScore(75);
                                UpdateDecorState(16);
                                break;
                            case 12:
                                AddLawsonsScore(90);
                                UpdateDecorState(17);
                                break;
                        }

                        await Delay(4500);
                        API.DecorSetInt(Game.PlayerPed.LastVehicle.Handle, "lawson_mission_state", 0);
                        GetLawsonsScore();
                        await Delay(500);
                        API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                        API.DeleteEntity(ref trailer);
                        API.DeleteEntity(ref decorBlista);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                            "PMP Haulage Reputation", "success",
                            $"You have gained reputation points. You now have a score of: {lawsonsScore}!",
                            new NewNotificationMessageContent[0]));
                    }
                }
            };
            return;
        }

        private async void UpdateDecorState(int state)
        {
            await Delay(10);
            API.DecorSetInt(decorBlista, "lawson_mission_state", state);
            Debug.WriteLine($"BLISTA STATE: {state}");
        }


        private async void JoinedMission(int vehicleHandle)
        {
            var currentState = API.DecorGetInt(decorBlista, "lawson_mission_state");
            switch (currentState)
            {
                case 1:
                    UpdateDecorState(2);
                    break;
                case 2:
                    UpdateDecorState(10);
                    break;
                case 10:
                    UpdateDecorState(11);
                    break;
                case 11:
                    UpdateDecorState(12);
                    break;
            }

            _soundService.Play("LawsonsMultiplayerJoin.wav", voiceVolume);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher", "success",
                "You have joined a delivery as an escort vehicle! Help keep the package safe to gain extra points for completing with a partner!",
                new NewNotificationMessageContent[0]));
            joinedMissionVehicleHandle = vehicleHandle;
            var multiplayerTrailerHandle = API.DecorGetInt(joinedMissionVehicleHandle, "lawson_mission_state");
            if (multiplayerTrailerHandle == null || multiplayerTrailerHandle == 0)
            {
                API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                    "info", "This mission is not ready yet, await the Trailer to arrive and retry!",
                    new NewNotificationMessageContent[0]));
                return;
            }

            await Delay(2000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher", "info",
                "Recieve any delivery information from the primary driver. You will need to guide the route, or backup from the rear.",
                new NewNotificationMessageContent[0]));

            _tickManager.On(JoinedMissionStateCheceker);
            _tickManager.On(LawsonsJoinedMissionTimer);
        }

        private async Task JoinedMissionStateCheceker()
        {
            await Delay(100);

            // LAWSON MISSION STATE DECOR: "lawson_mission_state"
            // STATE 0: N/A
            // STATE 1: MISSION STARTED
            // STATE 2: MISSION STARTED WITH PLAYERS JOINED
            // STATE 3: MISSION CRASHED
            // STATE 4: MISSION VOIDED
            // STATE 5: MISSION COMPLETED
            // STATE 6: DELIVERED TO WRONG PLACE

            // Decor State 1 - 1x Mission Ongoing
            // Decor State 2 - 2x Mission Ongoing
            // Decor State 10 - 3x Mission Ongoing
            // Decor State 11 - 4x Mission Ongoing
            // Decor State 12 - 5x Mission Ongoing

            // Decor State 5 - 2x Mission complete
            // Decor State 15 - 3x Mission complete
            // Decor State 16 - 4x Mission complete
            // Decor State 17 - 5x Mission complete

            var currentMissionState = API.DecorGetInt(joinedMissionVehicleHandle, "lawson_mission_state");
            //Debug.WriteLine($"JOINED MULTIPLAYER, BLISTA STATE: {currentMissionState}");

            switch (currentMissionState)
            {
                case 10: // 3 Player Mission Ongoing
                case 11: // 4 Player Mission Ongoing
                case 12: // 5 Player Mission Ongoing
                case 2: // 2 Player Mission Ongoing
                    //_tickManager.On(LawsonsJoinedMissionTimer);
                    break;
                case 3:
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                        "error", "The primary vehicle has crashed. As a team you have failed the delivery!",
                        new NewNotificationMessageContent[0]));
                    joinedMissionVehicleHandle = 0;
                    _soundService.Play("LawsonsMissionCrash.wav", voiceVolume);
                    API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                    RemoveLawsonsScore(20);
                    _tickManager.Off(LawsonsJoinedMissionTimer);
                    await Delay(750);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Reputation",
                        "error", $"You have lost reputation points. You now have a score of: {lawsonsScore}!",
                        new NewNotificationMessageContent[0]));
                    _tickManager.Off(JoinedMissionStateCheceker);
                    break;
                case 4:
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                        "error", "This mission has been cancelled by the primary driver!",
                        new NewNotificationMessageContent[0]));
                    joinedMissionVehicleHandle = 0;
                    API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                    _tickManager.Off(LawsonsJoinedMissionTimer);
                    _tickManager.Off(JoinedMissionStateCheceker);
                    break;
                case 5:
                    API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                        "success",
                        "You have made a successfully delivery. This site will now take it off your hands for you.",
                        new NewNotificationMessageContent[0]));
                    joinedMissionVehicleHandle = 0;
                    _soundService.Play("LawsonsDelivered.wav", voiceVolume);
                    AddLawsonsScore(45);
                    await Delay(750);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Reputation",
                        "success", $"You have gained reputation points. You now have a score of: {lawsonsScore}!",
                        new NewNotificationMessageContent[0]));
                    _tickManager.Off(LawsonsJoinedMissionTimer);
                    _tickManager.Off(JoinedMissionStateCheceker);
                    break;
                case 6:
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                        "error",
                        $"You have delivered this to the wrong location. This warehouse will take it this time, however you have let the {deliveringTo} site down. They were counting on you!",
                        new NewNotificationMessageContent[0]));
                    joinedMissionVehicleHandle = 0;
                    _soundService.Play("LawsonsWrongLocation.wav", voiceVolume);
                    API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                    RemoveLawsonsScore(20);
                    _tickManager.Off(LawsonsJoinedMissionTimer);
                    await Delay(750);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Reputation",
                        "error", $"You have lost reputation points. You now have a score of: {lawsonsScore}!",
                        new NewNotificationMessageContent[0]));
                    _tickManager.Off(JoinedMissionStateCheceker);
                    break;


                case 15:
                    API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                        "success",
                        "You have made a successfully delivery. This site will now take it off your hands for you.",
                        new NewNotificationMessageContent[0]));
                    joinedMissionVehicleHandle = 0;
                    _soundService.Play("LawsonsDelivered.wav", voiceVolume);
                    AddLawsonsScore(60);
                    await Delay(750);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Reputation",
                        "success", $"You have gained reputation points. You now have a score of: {lawsonsScore}!",
                        new NewNotificationMessageContent[0]));
                    _tickManager.Off(LawsonsJoinedMissionTimer);
                    _tickManager.Off(JoinedMissionStateCheceker);
                    break;

                case 16:
                    API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                        "success",
                        "You have made a successfully delivery. This site will now take it off your hands for you.",
                        new NewNotificationMessageContent[0]));
                    joinedMissionVehicleHandle = 0;
                    _soundService.Play("LawsonsDelivered.wav", voiceVolume);
                    AddLawsonsScore(75);
                    await Delay(750);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Reputation",
                        "success", $"You have gained reputation points. You now have a score of: {lawsonsScore}!",
                        new NewNotificationMessageContent[0]));
                    _tickManager.Off(LawsonsJoinedMissionTimer);
                    _tickManager.Off(JoinedMissionStateCheceker);
                    break;

                case 17:
                    API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                        "success",
                        "You have made a successfully delivery. This site will now take it off your hands for you.",
                        new NewNotificationMessageContent[0]));
                    joinedMissionVehicleHandle = 0;
                    _soundService.Play("LawsonsDelivered.wav", voiceVolume);
                    AddLawsonsScore(90);
                    await Delay(750);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Reputation",
                        "success", $"You have gained reputation points. You now have a score of: {lawsonsScore}!",
                        new NewNotificationMessageContent[0]));
                    _tickManager.Off(LawsonsJoinedMissionTimer);
                    _tickManager.Off(JoinedMissionStateCheceker);
                    break;
            }
        }

        private async void CalloutTimer()
        {
            _tickManager.On(CrashDetector);
            crashDetected = false;
            noClipped = false;
            var player = Game.Player;
            var vehicle = player.Character.LastVehicle;
            vehicle.Repair();

            while (!crashDetected && !noClipped)
            {
                await Delay(1000);
            }

            if (crashDetected)
            {
                API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                    "error", "You have crashed! The delivery is now damaged and cannot be used. Go careful next time!",
                    new NewNotificationMessageContent[0]));
                if (!crashDetected) return;
                _parcel = false;
                crashDetected = false;
                _soundService.Play("LawsonsMissionCrash.wav", voiceVolume);
                RemoveLawsonsScore(15);
                await Delay(3000);
                UpdateDecorState(3);
                await Delay(5000);
                API.DecorSetInt(Game.PlayerPed.LastVehicle.Handle, "lawson_mission_state", 0);
                GetLawsonsScore();
                API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Reputation",
                    "error", $"You have lost reputation points. You now have a score of: {lawsonsScore}!",
                    new NewNotificationMessageContent[0]));
                await Delay(10000);
                API.DeleteEntity(ref trailer);
                API.DeleteEntity(ref decorBlista);
                _tickManager.Off(LawsonsMissionTimer);
                _tickManager.Off(CrashDetector);
                await Delay(1500);
                return;
            }

            if (noClipped)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                    "error", "This mission has been cancelled!", new NewNotificationMessageContent[0]));
                _parcel = false;
                UpdateDecorState(4);
                await Delay(5000);
                API.DecorSetInt(Game.PlayerPed.LastVehicle.Handle, "lawson_mission_state", 0);
                _tickManager.Off(LawsonsMissionTimer);
                _tickManager.Off(CrashDetector);
                API.DeleteEntity(ref trailer);
                API.DeleteEntity(ref decorBlista);
                return;
            }
        }

        private async void CancelOnChangeRole()
        {
            noClipped = true;
            _tickManager.Off(LawsonsMissionTimer);
        }

        private async Task CrashDetector()
        {
            await Delay(1000);

            var player = Game.PlayerPed;
            var vehicleHealth = API.GetVehicleBodyHealth(trailer);
            if (vehicleHealth < 85) crashDetected = true;
            if (AdminMenu.NoClipActive) noClipped = true;
        }

        private async void GetLastDailyBonusDate()
        {
            var cachedDate = API.GetResourceKvpString("LawsonsLastDailyBonusDate");
            lastCachedDate = cachedDate;
        }

        private async void SetLastDailyBonusDate(string newDate)
        {
            GetLastDailyBonusDate();
            await Delay(100);
            API.SetResourceKvp("LawsonsLastDailyBonusDate", newDate);
        }

        private void GetLawsonsScore()
        {
            var cahcedScore = API.GetResourceKvpInt("Lawsons_player_score");
            lawsonsScore = cahcedScore;
            return;
        }

        private async void AddLawsonsScore(int scoreToAdd)
        {
            var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
            var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);

            //if (isEaServer || isDevServer) return;

            GetLawsonsScore();
            await Delay(100);
            var score = lawsonsScore + scoreToAdd;
            API.SetResourceKvpInt("Lawsons_player_score", score);
        }

        private async void RemoveLawsonsScore(int scoreToRemove)
        {
            var isEaServer = _featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess);
            var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);

            if (isEaServer || isDevServer) return;
            GetLawsonsScore();
            await Delay(100);
            var score = lawsonsScore - scoreToRemove;
            API.SetResourceKvpInt("Lawsons_player_score", score);
        }

        private async void DevAddPoints()
        {
            var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);
            if (!_userAces.IsDeveloper) return;
            if (!isDevServer) return;

            GetLawsonsScore();
            await Delay(100);
            var score = lawsonsScore + 500;
            API.SetResourceKvpInt("Lawsons_player_score", score);
        }


        public async void CalloutHandler()
        {
            var currentCar = Game.PlayerPed.CurrentVehicle;
            var lastCar = Game.PlayerPed.LastVehicle;
            var cabSpawnCode = "trans_mbenzarocs";
            var modelHashCab = (uint)API.GetHashKey(cabSpawnCode);

            if (lastCar == null || (uint)Game.PlayerPed.LastVehicle.Model.Hash != modelHashCab)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                    "error", "You must have the PMP Haulage Cab with you to accept a job!",
                    new NewNotificationMessageContent[0]));
                return;
            }

            if (_parcel == true)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher",
                    "error", "Please complete your current delivery before taking a new delivery",
                    new NewNotificationMessageContent[0]));
                API.PlaySoundFrontend(-1, "ScreenFlash", "MissionFailedSounds", false);
                return;
            }

            var playerPos = Game.PlayerPed.Position;
            Dictionary<Vector3, float> distances = new Dictionary<Vector3, float>
            {
                { _claymoreWarehouse, Vector3.Distance(playerPos, _claymoreWarehouse) },
                { _airportWarehouse, Vector3.Distance(playerPos, _airportWarehouse) },
                { _murrietaWarehouse, Vector3.Distance(playerPos, _murrietaWarehouse) },
                { _elBurroWarehouse, Vector3.Distance(playerPos, _elBurroWarehouse) },
                { _humaneLabs, Vector3.Distance(playerPos, _humaneLabs) },
                { _sandyScrapCentre, Vector3.Distance(playerPos, _sandyScrapCentre) },
                { _bellfarmWarehouse, Vector3.Distance(playerPos, _bellfarmWarehouse) },
                { _grapeseedMarina, Vector3.Distance(playerPos, _grapeseedMarina) },
                { _laFuentaBlanca, Vector3.Distance(playerPos, _laFuentaBlanca) },
                { _youTool, Vector3.Distance(playerPos, _youTool) },
            };

            var closestLocation = distances.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

            if (closestLocation == _airportWarehouse)
            {
                spawnCoords = new Vector3(-895.55175f, -2730.7287f, 13.82848f);
                spawnHeading = 326.8f;
            }

            if (closestLocation == _claymoreWarehouse)
            {
                spawnCoords = new Vector3(1196.34631f, -3205.08471f, 6.0280f);
                spawnHeading = 268.5f;
            }

            if (closestLocation == _murrietaWarehouse)
            {
                spawnCoords = new Vector3(1191.2512f, -1284.983f, 35.07528f);
                spawnHeading = 265;
            }

            if (closestLocation == _elBurroWarehouse)
            {
                spawnCoords = new Vector3(1686.666f, -1554.510f, 112.64150f);
                spawnHeading = 73.5f;
            }

            if (closestLocation == _sandyScrapCentre)
            {
                spawnCoords = new Vector3(2347.7165f, 3157.7003f, 48.198f);
                spawnHeading = 110;
            }

            if (closestLocation == _bellfarmWarehouse)
            {
                spawnCoords = new Vector3(140.592239f, 6360.6577f, 31.426548f);
                spawnHeading = 27;
            }

            if (closestLocation == _grapeseedMarina)
            {
                spawnCoords = new Vector3(1315.10302f, 4317.16845f, 38.18745f);
                spawnHeading = 348;
            }

            if (closestLocation == _laFuentaBlanca)
            {
                spawnCoords = new Vector3(1404.3765f, 1118.1704f, 114.837f);
                spawnHeading = 89.4f;
            }

            if (closestLocation == _humaneLabs)
            {
                spawnCoords = new Vector3(3639.80224f, 3766.12573f, 28.51573f);
                spawnHeading = 18.6f;
            }

            if (closestLocation == _youTool)
            {
                spawnCoords = new Vector3(2673.54809f, 3519.301757f, 52.712001f);
                spawnHeading = 336.7f;
            }

            List<string> possibleWarehouses = new List<string>
            {
                "You Tool", "Claymore Warehouse", "Fedex Airport Warehouse", "Murrieta Warehouse", "El Burro Warehouse",
                "Humane Labs", "Sandy Scrap Centre", "Bell Farm Warehouse", "Grapeseed Marina", "La Fuenta Blanca"
            };

            Dictionary<Vector3, string> warehouseNames = new Dictionary<Vector3, string>
            {
                { _claymoreWarehouse, "Claymore Warehouse" },
                { _airportWarehouse, "Fedex Airport Warehouse" },
                { _murrietaWarehouse, "Murrieta Warehouse" },
                { _elBurroWarehouse, "El Burro Warehouse" },
                { _humaneLabs, "Humane Labs" },
                { _sandyScrapCentre, "Sandy Scrap Centre" },
                { _bellfarmWarehouse, "Bell Farm Warehouse" },
                { _grapeseedMarina, "Grapeseed Marina" },
                { _laFuentaBlanca, "La Fuenta Blanca" },
                { _youTool, "You Tool" },
            };

            string closestWarehouseName = warehouseNames[closestLocation];
            possibleWarehouses.Remove(closestWarehouseName);
            Random randomWar = new Random();
            int randomWarInedx = randomWar.Next(possibleWarehouses.Count);
            string randomWarehouse = possibleWarehouses[randomWarInedx];

            deliveringTo = randomWarehouse;

            API.ClearGpsMultiRoute();
            API.ClearGpsPlayerWaypoint();

            API.PlaySoundFrontend(-1, "Mission_Pass_Notify", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);

            _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher", "info",
                "Park your cab out of the way, and a member of the team will bring out the trailer shortly!",
                new NewNotificationMessageContent[0]));

            _soundService.Play("LawsonsNewMission.wav", voiceVolume);
            await Delay(7500);

            _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Multiplayer", "info",
                "If other players are joining your job, remain outside your vehicle until they have joined. They will have 45s to join.",
                new NewNotificationMessageContent[0]));
            JoinCountdown();

            var modelHash = (uint)API.GetHashKey(trailerSpawncode);
            var modelHash2 = (uint)API.GetHashKey("blista");

            API.RequestModel(modelHash);
            API.RequestModel(modelHash2);

            while (!API.HasModelLoaded(modelHash))
            {
                await Delay(100);
            }

            while (!API.HasModelLoaded(modelHash2))
            {
                await Delay(100);
            }

            var warehouseNum = 0;
            switch (deliveringTo)
            {
                case "Claymore Warehouse":
                    warehouseNum = 1;
                    API.SetNewWaypoint(_claymoreWarehouse.X, _claymoreWarehouse.Y);
                    break;
                case "Fedex Airport Warehouse":
                    warehouseNum = 2;
                    API.SetNewWaypoint(_airportWarehouse.X, _airportWarehouse.Y);
                    break;
                case "Murrieta Warehouse":
                    warehouseNum = 3;
                    API.SetNewWaypoint(_murrietaWarehouse.X, _murrietaWarehouse.Y);
                    break;
                case "El Burro Warehouse":
                    warehouseNum = 4;
                    API.SetNewWaypoint(_elBurroWarehouse.X, _elBurroWarehouse.Y);
                    break;
                case "Humane Labs":
                    warehouseNum = 5;
                    API.SetNewWaypoint(_humaneLabs.X, _humaneLabs.Y);
                    break;
                case "Sandy Scrap Centre":
                    warehouseNum = 6;
                    API.SetNewWaypoint(_sandyScrapCentre.X, _sandyScrapCentre.Y);
                    break;
                case "Bell Farm Warehouse":
                    warehouseNum = 7;
                    API.SetNewWaypoint(_bellfarmWarehouse.X, _bellfarmWarehouse.Y);
                    break;
                case "Grapeseed Marina":
                    warehouseNum = 8;
                    API.SetNewWaypoint(_grapeseedMarina.X, _grapeseedMarina.Y);
                    break;
                case "La Fuenta Blanca":
                    warehouseNum = 8;
                    API.SetNewWaypoint(_laFuentaBlanca.X, _laFuentaBlanca.Y);
                    break;
            }

            trailer = API.CreateVehicle(modelHash, spawnCoords.X, spawnCoords.Y, spawnCoords.Z, spawnHeading, true,
                false);
            var bone = API.GetEntityBoneIndexByName(trailer, "bonnet");
            decorBlista = API.CreateVehicle(modelHash2, spawnCoords.X, spawnCoords.Y, spawnCoords.Z, spawnHeading, true,
                false);
            API.NetworkSetEntityInvisibleToNetwork(decorBlista, true);
            API.AttachEntityToEntity(decorBlista, trailer, bone, 0, 0, -7.5f, 0, 0, 0, false, false, false, false, 2,
                true);
            API.SetEntityCompletelyDisableCollision(decorBlista, true, false);
            API.SetEntityCollision(decorBlista, false, false);
            API.SetEntityVisible(decorBlista, false, false);

            Debug.WriteLine("BLISTA SPAWNED");
            Debug.WriteLine("BLISTA ATTACHED");
            Debug.WriteLine("BLISTA COLLISIONS DISABLED");

            await Delay(100);

            UpdateDecorState(1);
            Debug.WriteLine("BLISTA DECOR UPDATED TO: 1");

            if (API.IsVehicleExtraTurnedOn(trailer, 1)) randomParcel = "Construction Vehicle";
            if (API.IsVehicleExtraTurnedOn(trailer, 2)) randomParcel = "Yacht Boat";
            if (API.IsVehicleExtraTurnedOn(trailer, 3)) randomParcel = "Farming Vehicle";
            if (API.IsVehicleExtraTurnedOn(trailer, 4)) randomParcel = "Shipping Crate";
            if (API.IsVehicleExtraTurnedOn(trailer, 5)) randomParcel = "Structural Component";

            _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Haulage Dispatcher", "info",
                $"The trailer is ready to go! Pick it up and drive carefully to the destination, avoiding any damages! You are heading off to {deliveringTo} (Warehouse: {warehouseNum})!",
                new NewNotificationMessageContent[0]));
            _parcel = true;
            _tickManager.On(LawsonsMissionTimer);
            CalloutTimer();
            return;
        }

        private async void JoinCountdown()
        {
            await Delay(45000);
            API.DecorSetInt(Game.PlayerPed.LastVehicle.Handle, "lawson_mission_state", 0);
        }
    }
}