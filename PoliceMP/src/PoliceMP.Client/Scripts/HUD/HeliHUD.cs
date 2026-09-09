using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;
using System.Runtime.CompilerServices;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using static PoliceMP.Client.Scripts.MovementHandler;
using MenuAPI;

namespace PoliceMP.Client.Scripts.HUD.HeliHUD
{
    public class HeliHUD : Script
    {
        private readonly ITickManager _ticks;
        private readonly ILogger<HeliHUD> _logger;
        private INotificationService _notification;
        private ICommandManager _commandManager;
        private readonly IPermissionService _permissionService;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ISoundService _soundService;

        #region Variables
        private bool forceOff = false;
        private string eng1Text = "~c~ENG 1";
        private string eng2Text = "~c~ENG 2";
        private string apuText = "~c~APU";
        private string speedText = "~c~SPEED";
        private string tempsText = "~c~TEMPS";
        private string brakeText = "~g~BRAKE";
        private string prevBrakeState = "";
        private bool inAir = false;
        private bool showHeliHud = false;
        private bool rotorBrakeEnabled = false;

        public bool maintenanceEnabled = false;

        private const float MaxFuel = 2350f; // adjust for longer time (2350f = 39ins)
        private Dictionary<int, float> vehicleFuel = new Dictionary<int, float>();

        private List<Vector3> fuelPoints = new List<Vector3>
        {
            new Vector3(-1178.68f, -2846.35f, 13.832f), // HEATHROW
            new Vector3(-733.11f, -1453.87f, 5.00f), // DOCK BASE
            new Vector3(-1869.43f, 2801.00f, 32.80f), // MIL BASE
            new Vector3(1775.348f, 3332.48f, 42.03f), // SANDY
            new Vector3(-3241.0393f, 7453.0673f, 44.2649f), // GATWICK
            new Vector3(-1799.436f, -3155.019f, 13.944f) // HEATHROW TANKER SPAWNER
        };

        private bool isRefueling = false;
        public float fuelStatus = 2350f;
        private bool heliTaxiEnabled = false;

        private float scaleX = 1f;
        private float scaleY = 1f;

        private bool soundPlayed100ft = false;
        private bool soundPlayed50ft = false;
        private bool soundPlayed40ft = false;
        private bool soundPlayed30ft = false;
        private bool soundPlayed20ft = false;
        private bool soundPlayed10ft = false;
        private float previousAltitude = float.MaxValue;
        private float altVolume = 0.2f;
        private float apuVolume = 0.2f;
        private int heliHudDesign = 1;
        #endregion Variables

        public HeliHUD(ITickManager tick, ICommandManager command, ISoundService soundService, IPermissionService permission, ICommandManager commandManager, INewNotificationOverlay newNotificationOverlay, INotificationService notification, ILogger<HeliHUD> logger)
        {
            _notification = notification;
            _logger = logger;
            _ticks = tick;
            _newNotificationOverlay = newNotificationOverlay;
            _commandManager = commandManager;
            _permissionService = permission;
            _soundService = soundService;
        }

        #region Ticks and Commands
        protected override Task OnStartAsync()
        {
            _ticks.Off(ForceHeliEngineOff);
            _ticks.On(LeaveHeliCheck);
            _ticks.On(SpritesAndUI);
            _ticks.On(HeliStartAndStop);
            _ticks.Off(APULoop);
            _ticks.Off(AltitudeCallouts);

            _ticks.On(TankerSpawner);
            _ticks.On(FuelSystem);
            _ticks.Off(HelicopterTaxi);

            API.DecorRegister("helihud_aog_state", 2);
            API.DecorRegister("helihud_aog_playerIndex", 3);

            _ticks.On(PropWash);

            _commandManager.Register("helihud").WithHandler(ToggleHeliHud);
            _commandManager.Register("togglehelitaxi").WithHandler(ToggleHeliTaxi);
            _commandManager.Register("devheliaog").WithHandler(DevSetHeliAOG);

            API.RegisterKeyMapping("togglehelitaxi", "Toggle HeliHud Taxi Mode", "keyboard", "UP");

            CreateMapBlips();

            scaleX = Screen.Width / 1280f;
            scaleY = Screen.Height / 720f;

            return Task.FromResult(0);
        }
        #endregion Ticks and Commands

        private async void CreateMapBlips()
        {
            var _userAces = await _permissionService.GetUserAces();

            while (_permissionService.CurrentUserRole == null) await Delay(250);

            if (!_userAces.IsNpasTrained) return;
            foreach (Vector3 fuelFarm in fuelPoints)
            {
                var blip = API.AddBlipForCoord(fuelFarm.X, fuelFarm.Y, fuelFarm.Z);
                API.SetBlipSprite(blip, 360); // heli hangar symbol
                API.SetBlipScale(blip, 0.825f);
                API.SetBlipColour(blip, 58); // dark bluey purple

                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentSubstringPlayerName("Fuel Farms");
                API.EndTextCommandSetBlipName(blip);
            }
        }

        #region APU Sound (Looped)
        private async Task APULoop()
        {
            _soundService.Play("APULooped.wav", apuVolume);
            await Delay(5005);
        }
        #endregion APU Sound (Looped)

        #region Toggle HeliHUD
        private async void ToggleHeliHud()
        {
            MenuController.CloseAllMenus();
            var aces = await _permissionService.GetUserAces();
            if (!aces.IsDeveloper && !aces.IsNpasTrained) return;

            var helihudMenu = new Menu("HeliHUD Settings");
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.AddMenu(helihudMenu);

            await Delay(0);
            helihudMenu.OpenMenu();

            var toggleHeliHud = new MenuItem("Toggle System", "Toggle the entire HeliHUD system");
            helihudMenu.AddMenuItem(toggleHeliHud);
            var apuVol = new MenuItem("APU Volume", "Set volume for APU effects");
            helihudMenu.AddMenuItem(apuVol);
            var altVol = new MenuItem("GPWS Callout Volume", "Set volume for altitude callouts");
            helihudMenu.AddMenuItem(altVol);
            var layouts = new List<int> { 1, 2, 3 };
            var currentLayoutIndex = heliHudDesign;
            var cycleLayout = new MenuItem("HeliHUD Layout", "Cycle between HeliHUD layouts");
            helihudMenu.AddMenuItem(cycleLayout);
            var pmpAerospace = new MenuItem("PMP Aerospace", "Line Maintenance Tasks");
            helihudMenu.AddMenuItem(pmpAerospace);

            helihudMenu.OpenMenu();

            helihudMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (menu != helihudMenu) return;

                if (item == cycleLayout)
                {
                    int currentIndex = layouts.IndexOf(heliHudDesign);
                    int nextIndex = (currentIndex + 1) % layouts.Count;
                    heliHudDesign = layouts[nextIndex];
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("HeliHUD Settings", "success", $"You have set the HUD layout to: Layout {heliHudDesign}/2!", new NewNotificationMessageContent[0]));
                }

                if (item == altVol)
                {
                    API.AddTextEntry("FMMC_KEY_TIP1", "Enter Voiceline Volume (0-100)");
                    API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", $"{altVolume * 100}", "", "", "", 8);

                    API.UpdateOnscreenKeyboard();

                    while (API.UpdateOnscreenKeyboard() == 0)
                    {
                        await Delay(10);
                        API.UpdateOnscreenKeyboard();
                    }
                    if (API.UpdateOnscreenKeyboard() == 1)
                    {
                        var volumeStr = API.GetOnscreenKeyboardResult();
                        if (!float.TryParse(volumeStr, out float altVolumeInt))
                        {
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("HeliHUD Settings", "error", $"Error updating your selected volume, ensure you have chosen a number between 0 and 100.", new NewNotificationMessageContent[0]));
                            return;
                        }

                        altVolume = altVolumeInt / 100;
                        _logger.Debug($"VOICE VOLUME HAS BEEN SET TO {altVolume}");
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("HeliHUD Settings", "success", $"You have set the GPWS Altitude Callout volume to: {altVolume * 100}%", new NewNotificationMessageContent[0]));
                    }
                }

                if (item == apuVol)
                {
                    API.AddTextEntry("FMMC_KEY_TIP1", "Enter APU Volume (0-100)");
                    API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", $"{apuVolume * 100}", "", "", "", 8);

                    API.UpdateOnscreenKeyboard();

                    while (API.UpdateOnscreenKeyboard() == 0)
                    {
                        await Delay(10);
                        API.UpdateOnscreenKeyboard();
                    }
                    if (API.UpdateOnscreenKeyboard() == 1)
                    {
                        var volumeStr = API.GetOnscreenKeyboardResult();
                        if (!float.TryParse(volumeStr, out float apuVolumeInt))
                        {
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("HeliHUD Settings", "error", $"Error updating your selected volume, ensure you have chosen a number between 0 and 100.", new NewNotificationMessageContent[0]));
                            return;
                        }

                        apuVolume = apuVolumeInt / 100;
                        _logger.Debug($"VOICE VOLUME HAS BEEN SET TO {apuVolume}");
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("HeliHUD Settings", "success", $"You have set the APU volume to: {apuVolume * 100}%", new NewNotificationMessageContent[0]));
                    }
                }
                
                if (item == toggleHeliHud)
                {
                    if (showHeliHud)
                    {
                        showHeliHud = false;
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter HUD", "info", $"HeliHUD has been enabled.", new NewNotificationMessageContent[0]));
                        return;
                    }

                    var aces2 = await _permissionService.GetUserAces();
                    if (aces2.IsDeveloper || aces2.IsBandThree || aces2.IsBandFour)
                    {
                        showHeliHud = true;
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter HUD", "success", $"The option to disable the system is currently unavailable! However, Dev/B3/B4 Detected!", new NewNotificationMessageContent[0]));
                        return;

                    }
                    //showHeliHud = true;
                    showHeliHud = false;
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter HUD", "error", $"The option to disable the system is currently unavailable!", new NewNotificationMessageContent[0]));
                }

                if (item == pmpAerospace)
                {
                    maintenanceEnabled = true;
                    MenuController.CloseAllMenus();
                    var maintenanceMenu = new Menu("Line Maintenance", "Select a maintenance task");
                    MenuController.AddMenu(maintenanceMenu);
                    var cleanPitot = new MenuItem("Clean Pitot-static Tubes", "Clean air speed equipment");
                    var hydraulicFunctions = new MenuItem("Hydraulic Functions", "Check performance of Hyd systems");
                    var lowPwrEngineRun = new MenuItem("Low Power Engine Testing", "Test engine vibrations and Power");
                    maintenanceMenu.AddMenuItem(cleanPitot);
                    maintenanceMenu.AddMenuItem(hydraulicFunctions);
                    maintenanceMenu.AddMenuItem(lowPwrEngineRun);
                    maintenanceMenu.OpenMenu();

                    maintenanceMenu.OnItemSelect += async (maintMenu, maintItem, maintIndex) =>
                    {   
                        if (maintItem  == cleanPitot) CleanPitotTask();
                        if (maintItem  == hydraulicFunctions) HydFunctionsTask();
                        if (maintItem  == lowPwrEngineRun) LowPowerEngineRun();

                        MenuController.CloseAllMenus();
                    };

                    return;
                }
            };         
        }

        private async void CleanPitotTask()
        {
            var player = Game.PlayerPed.Handle;
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "info", "Please enter the cockpit of the helicopter to pull relevant circuit breakers for the task!", new NewNotificationMessageContent[0]));
            while (!API.IsPedInAnyHeli(Game.PlayerPed.Handle))
            {
                await Delay(500);
            }
            await Delay(3000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "You have pulled all the circuit breakers required, the aircraft is now safe for maintenance!", new NewNotificationMessageContent[0]));
            await Delay(2000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "Now leave the cockpit and head to the front of the aircraft!", new NewNotificationMessageContent[0]));
            await Delay(3000);
            API.ExecuteCommand("e clean2");
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "You are now cleaning the Pitot-Static tubes, these measure the speed in the air!", new NewNotificationMessageContent[0]));
            API.FreezeEntityPosition(player, true);
            await Delay(8000);
            API.ExecuteCommand("e c");
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "The Pitot tubes are now clean again, re-enter the cockpit!", new NewNotificationMessageContent[0]));
            API.FreezeEntityPosition(player, false);
            while (!API.IsPedInAnyHeli(Game.PlayerPed.Handle))
            {
                await Delay(500);
            }
            await Delay(3000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "You have reset the circuit breakers you pulled, Task completed!", new NewNotificationMessageContent[0]));
            maintenanceEnabled = false;
            return;        
        }

        private async void HydFunctionsTask()
        {
            var player = Game.PlayerPed.Handle;
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "info", "Please enter the cockpit of the helicopter to pull relevant circuit breakers for the task!", new NewNotificationMessageContent[0]));
            while (!API.IsPedInAnyHeli(Game.PlayerPed.Handle))
            {
                await Delay(500);
            }
            await Delay(3000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "You have pulled all the circuit breakers required, the aircraft is now safe for maintenance!", new NewNotificationMessageContent[0]));
            await Delay(3000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "Move around the flight controls to test the hydraulic systems, check all surfaces move as intended!", new NewNotificationMessageContent[0]));
            await Delay(8000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "All controls have been tested and are working!", new NewNotificationMessageContent[0]));
            await Delay(3000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "You have reset the circuit breakers you pulled, Task completed!", new NewNotificationMessageContent[0]));
            maintenanceEnabled = false;
            return;
        }

        private async void LowPowerEngineRun()
        {
            var player = Game.PlayerPed.Handle;
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "info", "Please enter the cockpit of the helicopter to prepare for an engine run!", new NewNotificationMessageContent[0]));
            while (!API.IsPedInAnyHeli(Game.PlayerPed.Handle))
            {
                await Delay(500);
            }
            await Delay(3000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "Make sure the area is clear around you, as its a low power run it can be performed in the hangar!", new NewNotificationMessageContent[0]));
            await Delay(5000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "Watch the props and reduction gearbox for any issues as the engine is tested!", new NewNotificationMessageContent[0]));
            await Delay(3000);
            var propSpeed = 0f;
            var heli = API.GetVehiclePedIsIn(player, false);
            while (propSpeed < 0.9f)
            {
                API.SetHeliBladesSpeed(heli, propSpeed);
                await Delay(10);
                propSpeed = propSpeed + 0.00033f;
            }
            API.SetHeliBladesSpeed(heli, 0f);
            _ticks.On(ForceHeliEngineOff);
            await Delay(2000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "The engine run has finished!", new NewNotificationMessageContent[0]));
            await Delay(3000);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Maintenance", "success", "You have reset the circuit breakers you pulled, Task completed!", new NewNotificationMessageContent[0]));
            maintenanceEnabled = false;
            return;
        }
        #endregion Toggle HeliHUD

        #region Force Heli Engine Off
        private async Task ForceHeliEngineOff()
        {
            if (_permissionService.CurrentUserRole == null) return;
            if (_permissionService.CurrentUserRole.Division != UserDivision.Npas) return;
            await Delay(1);
            var player = Game.PlayerPed.Handle;
            var vehicle = API.GetVehiclePedIsIn(player, false);
            if (API.GetVehicleClass(vehicle) != 15) return;            
            API.SetVehicleEngineOn(API.GetVehiclePedIsIn(player, false), false, true, true);
        }
        #endregion Force Heli Engine Off

        #region Left Heli Checker
        private async Task LeaveHeliCheck()
        {
            if (_permissionService.CurrentUserRole == null) return;
            if (_permissionService.CurrentUserRole.Division != UserDivision.Npas) return;
            var player = Game.PlayerPed;

            if (API.IsPedInAnyHeli(player.Handle))
            {
                var height = API.GetEntityHeightAboveGround(player.Handle);
                if (height > 15f && !showHeliHud) _ticks.On(FuelSystem);
            }

            if (!API.IsPedInAnyHeli(player.Handle) && !rotorBrakeEnabled)
            {

                if (eng1Text == "~g~ENG 1" && eng2Text == "~g~ENG 2" && brakeText == "~g~BRAKE") return;
                _ticks.Off(ForceHeliEngineOff);
                _ticks.Off(APULoop);
                _ticks.Off(AltitudeCallouts);
                _ticks.Off(FuelSystem);
                _ticks.Off(StartRefueling);
                await Delay(100);
                forceOff = false;
                if (player.LastVehicle == null) return;
                inAir = false;
                var lastVehicle = player.LastVehicle;
                eng1Text = "~c~ENG 1";
                eng2Text = "~c~ENG 2";
                apuText = "~c~APU";
                speedText = "~c~SPEED";
                tempsText = "~c~TEMPS";
                brakeText = "~g~BRAKE";
                Screen.ShowSubtitle(" ", 10);
            }

            if (showHeliHud)
            {
                _ticks.Off(ForceHeliEngineOff);
                _ticks.Off(APULoop);
                _ticks.Off(AltitudeCallouts);
                _ticks.Off(FuelSystem);
                _ticks.Off(StartRefueling);
                await Delay(100);
                forceOff = false;
                if (player.LastVehicle == null) return;
                inAir = false;
                var lastVehicle = player.LastVehicle;
                eng1Text = "~c~ENG 1";
                eng2Text = "~c~ENG 2";
                apuText = "~c~APU";
                speedText = "~c~SPEED";
                tempsText = "~c~TEMPS";
                brakeText = "~g~BRAKE";
                Screen.ShowSubtitle(" ", 10);
            }

            if (API.IsPedInAnyHeli(player.Handle))
            {
                if (showHeliHud) return;
                if (!inAir) return;
                API.SetVehicleEngineOn(API.GetVehiclePedIsIn(player.Handle, false), true, true, false);
                await Delay(1000);
            }
        }
        #endregion Left Heli Checker

        #region HUD / UI
        private async Task SpritesAndUI()
        {
            if (_permissionService.CurrentUserRole == null) return;
            if (_permissionService.CurrentUserRole.Division != UserDivision.Npas) return;

            var player = Game.PlayerPed.Handle;
            var vehicle = API.GetVehiclePedIsIn(player, false);
            var pedInSeat = API.GetPedInVehicleSeat(vehicle, -1);
            var height = API.GetEntityHeightAboveGround(player);

            if (!API.IsPedInAnyHeli(player)) return;
            if (pedInSeat != player) return;
            if (showHeliHud) return;
            if (height > 20f) return;

            float mainBorderX = 0f, mainBorderY = 0f, mainBorderWX = 0f, mainBorderWY = 0f;
            float mainBoxX = 0f, mainBoxY = 0f, mainBoxWX = 0f, mainBoxWY = 0f;
            float eng1BoxX = 0f, eng1BoxY = 0f, eng1BoxWX = 0f, eng1BoxWY = 0f;
            float eng2BoxX = 0f, eng2BoxY = 0f, eng2BoxWX = 0f, eng2BoxWY = 0f;
            float apuBoxX = 0f, apuBoxY = 0f, apuBoxWX = 0f, apuBoxWY = 0f;
            float speedBoxX = 0f, speedBoxY = 0f, speedBoxWX = 0f, speedBoxWY = 0f;
            float tempsBoxX = 0f, tempsBoxY = 0f, tempsBoxWX = 0f, tempsBoxWY = 0f;
            float brakeBoxX = 0f, brakeBoxY = 0f, brakeBoxWX = 0f, brakeBoxWY = 0f;
            float ENG1PosX = 0f, ENG1PosY = 0f, ENG2PosX = 0f, ENG2PosY = 0f, APUTextPosX = 0f, APUTextPosY = 0f;
            float SPEEDTextPosX = 0f, SPEEDTextPosY = 0f, TEMPSTextPosX = 0f, TEMPSTextPosY = 0f, BRAKETextPosX = 0f, BRAKETextPosY = 0f;

            switch (heliHudDesign)
            {
                case 1:
                    mainBorderX = 0.5f; mainBorderY = 0.852f; mainBorderWX = 0.150f; mainBorderWY = 0.19752f;
                    mainBoxX = 0.5f; mainBoxY = 0.852f; mainBoxWX = 0.140f; mainBoxWY = 0.1802f;
                    eng1BoxX = 0.467f; eng1BoxY = 0.7925f; eng1BoxWX = 0.0575f; eng1BoxWY = 0.0480f;
                    eng2BoxX = 0.532f; eng2BoxY = 0.7925f; eng2BoxWX = 0.0575f; eng2BoxWY = 0.0480f;
                    apuBoxX = 0.467f; apuBoxY = 0.85f; apuBoxWX = 0.0575f; apuBoxWY = 0.0480f;
                    speedBoxX = 0.467f; speedBoxY = 0.91f; speedBoxWX = 0.0575f; speedBoxWY = 0.0480f;
                    tempsBoxX = 0.532f; tempsBoxY = 0.85f; tempsBoxWX = 0.0575f; tempsBoxWY = 0.0480f;
                    brakeBoxX = 0.532f; brakeBoxY = 0.91f; brakeBoxWX = 0.0575f; brakeBoxWY = 0.0480f;
                    ENG1PosX = 566f * scaleX; ENG1PosY = 555f * scaleY;
                    ENG2PosX = 647f * scaleX; ENG2PosY = 555f * scaleY;
                    APUTextPosX = 572f * scaleX; APUTextPosY = 596f * scaleY;
                    SPEEDTextPosX = 565f * scaleX; SPEEDTextPosY = 642f * scaleY;
                    TEMPSTextPosX = 647f * scaleX; TEMPSTextPosY = 598f * scaleY;
                    BRAKETextPosX = 647f * scaleX; BRAKETextPosY = 642f * scaleY;
                    break;

                case 2:
                    mainBorderX = 0.484f; mainBorderY = 0.852f; mainBorderWX = 0.225f; mainBorderWY = 0.1252f;
                    mainBoxX = 0.484f; mainBoxY = 0.852f; mainBoxWX = 0.215f; mainBoxWY = 0.1152f;
                    float paddingX = mainBoxWX * 0.04f;
                    float paddingY = mainBoxWY * 0.06f;
                    float smallBoxWX = (mainBoxWX - 4 * paddingX) / 3;
                    float smallBoxWY = (mainBoxWY - 3 * paddingY) / 2;
                    float startX = mainBoxX - mainBoxWX / 2 + smallBoxWX / 2 + paddingX;
                    float startY = mainBoxY - mainBoxWY / 2 + smallBoxWY / 2 + paddingY;
                    eng1BoxX = startX; eng1BoxY = startY; eng1BoxWX = smallBoxWX; eng1BoxWY = smallBoxWY;
                    eng2BoxX = startX + smallBoxWX + paddingX; eng2BoxY = startY; eng2BoxWX = smallBoxWX; eng2BoxWY = smallBoxWY;
                    tempsBoxX = startX + 2 * (smallBoxWX + paddingX); tempsBoxY = startY; tempsBoxWX = smallBoxWX; tempsBoxWY = smallBoxWY;
                    apuBoxX = startX; apuBoxY = startY + smallBoxWY + paddingY; apuBoxWX = smallBoxWX; apuBoxWY = smallBoxWY;
                    speedBoxX = startX + smallBoxWX + paddingX; speedBoxY = startY + smallBoxWY + paddingY; speedBoxWX = smallBoxWX; speedBoxWY = smallBoxWY;
                    brakeBoxX = startX + 2 * (smallBoxWX + paddingX); brakeBoxY = startY + smallBoxWY + paddingY; brakeBoxWX = smallBoxWX; brakeBoxWY = smallBoxWY;
                    ENG1PosX = eng1BoxX * Screen.Width - 33 * scaleX;
                    ENG1PosY = eng1BoxY * Screen.Height - 13.7f * scaleY;
                    ENG2PosX = eng2BoxX * Screen.Width - 35 * scaleX;
                    ENG2PosY = eng2BoxY * Screen.Height - 13.7f * scaleY;
                    TEMPSTextPosX = tempsBoxX * Screen.Width - 34 * scaleX;
                    TEMPSTextPosY = tempsBoxY * Screen.Height - 13.5f * scaleY;
                    APUTextPosX = apuBoxX * Screen.Width - 26 * scaleX;
                    APUTextPosY = apuBoxY * Screen.Height - 15f * scaleY;
                    SPEEDTextPosX = speedBoxX * Screen.Width - 34 * scaleX;
                    SPEEDTextPosY = speedBoxY * Screen.Height - 13.5f * scaleY;
                    BRAKETextPosX = brakeBoxX * Screen.Width - 34.25f * scaleX;
                    BRAKETextPosY = brakeBoxY * Screen.Height - 13.5f * scaleY;
                    break;
            }
            
            API.DrawSprite("", "", mainBorderX, mainBorderY, mainBorderWX, mainBorderWY, 0, 26, 25, 25, 255); // Main Border
            API.DrawSprite("", "", mainBoxX, mainBoxY, mainBoxWX, mainBoxWY, 0, 64, 64, 64, 255); // Main Box
            API.DrawSprite("", "", eng1BoxX, eng1BoxY, eng1BoxWX, eng1BoxWY, 0, 26, 25, 25, 255); // ENG1 Box
            API.DrawSprite("", "", eng2BoxX, eng2BoxY, eng2BoxWX, eng2BoxWY, 0, 26, 25, 25, 255); // ENG2 Box
            API.DrawSprite("", "", apuBoxX, apuBoxY, apuBoxWX, apuBoxWY, 0, 26, 25, 25, 255); // APU Box
            API.DrawSprite("", "", speedBoxX, speedBoxY, speedBoxWX, speedBoxWY, 0, 26, 25, 25, 255); // SPEED Box
            API.DrawSprite("", "", tempsBoxX, tempsBoxY, tempsBoxWX, tempsBoxWY, 0, 26, 25, 25, 255); // TEMPS Box
            API.DrawSprite("", "", brakeBoxX, brakeBoxY, brakeBoxWX, brakeBoxWY, 0, 26, 25, 25, 255); // BRAKE Box

            Text ENG1 = new Text(eng1Text, new System.Drawing.PointF(ENG1PosX, ENG1PosY), 0.55f);
            Text ENG2 = new Text(eng2Text, new System.Drawing.PointF(ENG2PosX, ENG2PosY), 0.55f);
            Text APU = new Text(apuText, new System.Drawing.PointF(APUTextPosX, APUTextPosY), 0.6f);
            Text SPEED = new Text(speedText, new System.Drawing.PointF(SPEEDTextPosX, SPEEDTextPosY), 0.5f);
            Text TEMPS = new Text(tempsText, new System.Drawing.PointF(TEMPSTextPosX, TEMPSTextPosY), 0.5f);
            Text BRAKE = new Text(brakeText, new System.Drawing.PointF(BRAKETextPosX, BRAKETextPosY), 0.5f);

            ENG1.Draw();
            ENG2.Draw();
            APU.Draw();
            SPEED.Draw();
            TEMPS.Draw();
            BRAKE.Draw();
        }
        #endregion HUD / UI

        #region Altitude Callouts
        private async Task AltitudeCallouts()
        {
            if (_permissionService.CurrentUserRole == null || _permissionService.CurrentUserRole.Division != UserDivision.Npas) return;
            var player = Game.PlayerPed;
            if (!API.IsPedInAnyHeli(player.Handle)) return;

            float currentAltitude = API.GetEntityHeightAboveGround(player.Handle);

            if (currentAltitude < previousAltitude)
            {
                PlayAltitudeCallout(ref soundPlayed100ft, currentAltitude, 31f, "GPWS100ftwav.wav");
                PlayAltitudeCallout(ref soundPlayed50ft, currentAltitude, 16f, "GPWS50ft.wav");
                PlayAltitudeCallout(ref soundPlayed40ft, currentAltitude, 14f, "GPWS40ft.wav");
                PlayAltitudeCallout(ref soundPlayed30ft, currentAltitude, 10f, "GPWS30ft.wav");
                PlayAltitudeCallout(ref soundPlayed20ft, currentAltitude, 7f, "GPWS20ft.wav");
                PlayAltitudeCallout(ref soundPlayed10ft, currentAltitude, 4f, "GPWS10ft.wav");
            }

            previousAltitude = currentAltitude;
            await Delay(100);
        }
        #endregion Altitude Callouts

        #region Heli Start/Stop
        private async Task HeliStartAndStop()
        {
            if (_permissionService.CurrentUserRole == null) return;
            if (_permissionService.CurrentUserRole.Division != UserDivision.Npas) return;
            if (showHeliHud) return;
            if (maintenanceEnabled) return;

            var player = Game.PlayerPed.Handle;
            var height = API.GetEntityHeightAboveGround(player);
            var vehicleIsIn = API.GetVehiclePedIsIn(player, true);
            var vehicle = API.GetVehiclePedIsIn(player, false);
            var pedInSeat = API.GetPedInVehicleSeat(vehicle, -1);

            if (!API.IsPedInAnyHeli(player)) return;
            if (pedInSeat != player) return;

            var engineOn = API.IsVehicleEngineOn(vehicle);

            if (API.IsPedInAnyHeli(player) && !inAir)
            {
                var vehicleHandle = API.GetVehiclePedIsIn(player, false);
                if (vehicleFuel.TryGetValue(vehicleHandle, out float fuel) && fuel <= 0)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Fuel System", "error", "Cannot start - no fuel!", new NewNotificationMessageContent[0]));
                    return;
                }
                API.SetVehicleEngineOn(vehicle, false, true, false);
                _ticks.On(ForceHeliEngineOff);

                var plateText1 = API.GetVehicleNumberPlateText(vehicleIsIn);
                if (plateText1 == "AOGFIXED") 
                {
                    API.DecorSetBool(vehicleIsIn, "helihud_aog_state", false);
                }

                while (!API.IsControlJustPressed(0, 22))
                {
                    if (isRefueling) return;
                    LeaveHeliCheck();
                    if (!API.IsPedInAnyHeli(player)) return;
                    if (maintenanceEnabled) return;
                    Screen.ShowSubtitle("~bold~~r~Press SPACE to start APU.", 0);
                    await Delay(10);
                }
                if (!API.IsPedInAnyHeli(player)) return;
                apuText = "~y~APU";

                if (fuelStatus < 15)
                {
                    await Delay(2000);
                    apuText = "~r~APU";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    Screen.ShowSubtitle("~bold~~r~LOW FUEL - APU START FAILED", 5000);
                    await Delay(5750);
                    return;
                }

                var plateText = API.GetVehicleNumberPlateText(vehicle);
                if (plateText == "AOGFIXED") API.DecorSetBool(vehicle, "helihud_aog_state", false);
                var aogState = API.DecorGetBool(vehicle, "helihud_aog_state");
                if (aogState == false)
                {
                    var random = new Random();
                    var setHeliAOG = random.Next(100) < 2;
                    if (setHeliAOG) API.DecorSetBool(vehicle, "helihud_aog_state", true);
                    if (!setHeliAOG) API.DecorSetBool(vehicle, "helihud_aog_state", false);

                    if (API.DecorGetBool(vehicle, "helihud_aog_state") == true)
                    {
                        var pilotIndex = API.NetworkPlayerIdToInt();
                        API.DecorSetInt(vehicle, "helihud_aog_playerIndex", pilotIndex);
                        API.SetVehicleNumberPlateText(vehicle, "ISAOG");
                    }
                }

                Debug.WriteLine($"MAINTENANCE COMPLETED, DECOR BOOL UPDATED TO {API.DecorGetBool(vehicle, "helihud_aog_state")}");

                var isHeliAOG = API.DecorGetBool(vehicle, "helihud_aog_state");
                if (isHeliAOG)
                {
                    var nearestHeli = API.GetVehiclePedIsIn(Game.PlayerPed.Handle, false);
                    var playerIndex = API.NetworkPlayerIdToInt();
                    var savedPlayerIndex = API.DecorGetInt(nearestHeli, "helihud_aog_playerIndex");
                    var jbibDrawable = API.GetPedDrawableVariation(Game.PlayerPed.Handle, 11);
                    var jbibTexture = API.GetPedTextureVariation(Game.PlayerPed.Handle, 11);
                    var inEngUniform = false;
                    if (jbibDrawable == 525 && jbibTexture == 3) inEngUniform = true; // PMP Aerospace uniform JBIB and texture
                    if (playerIndex != savedPlayerIndex && inEngUniform)
                    {
                        maintenanceEnabled = true;
                        while (!API.IsControlJustReleased(0, 38))
                        {
                            Screen.ShowSubtitle("~bold~~r~PRESS 'E' TO START COMPRESSOR MAINTENANCE", 1000);
                            await Delay(2);
                        }

                        Random rnd = new Random();
                        int randomDelay = rnd.Next(60000, 200000);

                        Screen.ShowSubtitle("~bold~~r~You are now fixing the problem...", randomDelay - 2500);
                        await Delay(randomDelay);

                        var currentVehicle = API.GetVehiclePedIsIn(player, true);
                        var modelHash = (uint)API.GetEntityModel(currentVehicle);
                        var position = API.GetEntityCoords(currentVehicle, false);        
                        var heading = API.GetEntityHeading(currentVehicle);
                        var model = (uint)modelHash;
                        API.RequestModel(model);

                        await Delay(1000);
                        API.SetVehicleNumberPlateText(currentVehicle, "AOGFIXED");
                        Screen.ShowSubtitle("~bold~~r~MAINTENANCE COMPLETE, COMPRESSOR REPAIRED", 4000);

                        await Delay(3000);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Aerospace", "success", "You have fixed the aircraft! Perform a function test of the APU startup!", new NewNotificationMessageContent[0]));
                        maintenanceEnabled = false;
                        inEngUniform = false;
                        await Delay(2000);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("PMP Aerospace", "success", "If the APU begins to start correctly, you can leave the helicopter and hand back over to the flight crew!", new NewNotificationMessageContent[0]));
                        return;
                    }

                    apuText = "~bold~~r~APU";
                    Screen.ShowSubtitle("~bold~~r~COMPRESSOR STALL - REQUIRES MAINTENANCE", 25000);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter Start Fail", "error", "The Compressor is stalling and preventing APU Start. The aircraft is now tech! Call a PMP Aerospace Engineer out to the aircraft to resolve the issue!", new NewNotificationMessageContent[0]));
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(2000);
                    apuText = "~r~APU";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(2000);
                    apuText = "~bold~~r~APU";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(2000);
                    apuText = "~r~APU";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(2000);
                    apuText = "~bold~~r~APU";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(2000);
                    apuText = "~r~APU";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(2000);
                    apuText = "~bold~~r~APU";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(2000);
                    apuText = "~r~APU";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(2000);
                    apuText = "~bold~~r~APU";
                    return;
                }

                _ticks.On(FuelSystem);
                await Delay(2000);
                _soundService.Play("APUStart.wav", apuVolume);
                await Delay(28000);
                _ticks.On(APULoop);
                apuText = "~g~APU";
                _ticks.On(ForceHeliEngineOff);
                if (!API.IsPedInAnyHeli(player)) return;
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                await Delay(1750);

                while (!API.IsControlJustPressed(0, 22))
                {
                    if (isRefueling) return;
                    LeaveHeliCheck();
                    if (!API.IsPedInAnyHeli(player)) return;
                    Screen.ShowSubtitle("~bold~~r~Press SPACE to start Engine One.", 0);
                    await Delay(10);
                }
                _ticks.Off(StartRefueling);
                if (!API.IsPedInAnyHeli(player)) return;
                eng1Text = "~y~ENG 1";
                await Delay(1000);
                _ticks.On(ForceHeliEngineOff);
                await Delay(7000);
                eng1Text = "~g~ENG 1";
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                await Delay(1750);
                while (!API.IsControlJustPressed(0, 22))
                {
                    if (isRefueling) return;
                    LeaveHeliCheck();
                    if (!API.IsPedInAnyHeli(player)) return;
                    Screen.ShowSubtitle("~bold~~r~Press SPACE to start Engine Two.", 0);
                    await Delay(10);
                }
                if (!API.IsPedInAnyHeli(player)) return;
                eng2Text = "~y~ENG 2";
                await Delay(7000);
                eng2Text = "~g~ENG 2";
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                await Delay(1750);
                if (!API.IsPedInAnyHeli(player)) return;
                while (!API.IsControlJustPressed(0, 22))
                {
                    if (isRefueling) return;
                    LeaveHeliCheck();
                    if (!API.IsPedInAnyHeli(player)) return;
                    Screen.ShowSubtitle("~bold~~r~Press SPACE to stop APU.", 0);
                    await Delay(10);
                }
                if (!API.IsPedInAnyHeli(player)) return;
                await Delay(750);
                apuText = "~y~APU";
                await Delay(5000);
                _ticks.Off(APULoop);
                await Delay(1250);
                _soundService.Play("APUStop.wav", apuVolume);
                apuText = "~c~APU";
                await Delay(1000);
                if (!API.IsPedInAnyHeli(player)) return;
                while (!API.IsControlJustPressed(0, 22))
                {
                    if (isRefueling) return;
                    LeaveHeliCheck();
                    if (!API.IsPedInAnyHeli(player)) return;
                    Screen.ShowSubtitle("~bold~~r~Press SPACE to disable Rotor Brake.", 0);
                    await Delay(10);
                }
                if (!API.IsPedInAnyHeli(player)) return;
                await Delay(1750);
                brakeText = "~c~BRAKE";
                rotorBrakeEnabled = false;
                _ticks.Off(ForceHeliEngineOff);
                API.SetVehicleEngineOn(vehicle, true, false, false);
                await Delay(7000);
                if (!API.IsPedInAnyHeli(player)) return;
                speedText = "~g~SPEED";
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                await Delay(1750);
                tempsText = "~y~TEMPS";
                await Delay(3000);
                tempsText = "~g~TEMPS";
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                await Delay(4500);
                inAir = true;
                await Delay(5000);
                _ticks.On(AltitudeCallouts);
            }

            vehicle = API.GetVehiclePedIsIn(player, false);
            engineOn = API.IsVehicleEngineOn(vehicle);

            if (API.IsPedInAnyHeli(player) && inAir)
            {
                if (_permissionService.CurrentUserRole == null) return;
                if (_permissionService.CurrentUserRole.Division != UserDivision.Npas) return;
                if (showHeliHud) return;
                if (API.GetEntityHeightAboveGround(player) > 10f) return;

                bool inputReceived = false;
                bool choseRefuel = false;
                while (!inputReceived)
                {
                    if (isRefueling) return;
                    LeaveHeliCheck();
                    if (!API.IsPedInAnyHeli(player)) return;
                    if (API.GetEntityHeightAboveGround(player) > 10f) return;

                    var playerPos = Game.Player.Character.Position;
                    bool nearFuel = false;

                    foreach (Vector3 fuelPoint in fuelPoints)
                    {
                        if (playerPos.DistanceToSquared(fuelPoint) < 800f)
                        {
                            nearFuel = true;
                            break;
                        }
                    }

                    if (!nearFuel)
                    {
                        foreach (var veh1 in World.GetAllVehicles())
                        {
                            if (veh1.Model.Hash == API.GetHashKey("tanker"))
                            {
                                if (playerPos.DistanceToSquared(veh1.Position) < 600f)
                                {
                                    nearFuel = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (nearFuel)
                        Screen.ShowSubtitle("~bold~~r~Press E to start fuelling.", 0);
                    else
                        Screen.ShowSubtitle("~bold~~r~Press SPACE to enable Rotor Brake.", 0);

                    if (API.IsControlJustPressed(0, 51)) // E key pressed
                    {
                        if (!nearFuel)
                        {
                            _newNotificationOverlay.SendNotification(
                                new NewNotificationMessage("Fuel Farm", "info", "You must be near a fuel farm to refuel!", new NewNotificationMessageContent[0])
                            );
                        }
                        else
                        {
                            while (API.GetEntityHeightAboveGround(player) > 2f)
                            {
                                await Delay(650);
                                Screen.ShowSubtitle("~bold~~y~Touch down before refuelling!", 650);
                            }
                            Screen.ShowSubtitle(" ", 100);
                            _ticks.On(StartRefueling);
                            choseRefuel = true;
                            inputReceived = true;
                        }
                    }
                    else if (API.IsControlJustPressed(0, 22)) // SPACE key pressed for rotor brake
                    {
                        inputReceived = true;
                    }
                    await Delay(10);
                }

                if (!choseRefuel)
                {
                    while (!API.IsControlJustPressed(0, 22))
                    {
                        if (isRefueling) return;
                        LeaveHeliCheck();
                        if (!API.IsPedInAnyHeli(player)) return;
                        Screen.ShowSubtitle("~bold~~r~Press SPACE to enable Rotor Brake.", 0);
                        await Delay(10);
                    }

                    rotorBrakeEnabled = true;
                    if (API.GetEntityHeightAboveGround(player) > 3f) return;
                    API.SetVehicleEngineOn(vehicle, false, false, true);

                    speedText = "~c~SPEED";
                    brakeText = "~y~BRAKE";
                    await Delay(2500);

                    if (API.GetEntityHeightAboveGround(player) > 3f) return;

                    brakeText = "~g~BRAKE";
                    inAir = false;
                    _ticks.Off(AltitudeCallouts);
                    _ticks.On(ForceHeliEngineOff);
                    if (!API.IsPedInAnyHeli(player)) return;

                    while (!API.IsControlJustPressed(0, 22) && !API.IsControlJustPressed(0, 38) && API.IsPedInAnyHeli(player))
                    {
                        if (isRefueling) return;
                        LeaveHeliCheck();
                        Screen.ShowSubtitle("~bold~~r~Press SPACE to start APU. OR Press E to disable Rotor Brake.", 0);
                        await Delay(10);
                    }
                    
                    while (!API.IsPedInAnyHeli(player) && eng1Text == "~g~ENG 1" && eng2Text == "~g~ENG 2" && brakeText == "~g~BRAKE")
                    {
                        await Delay(10);
                        Screen.ShowSubtitle("~bold~~r~Engines are still on, Re-enter the Aircraft to continue.", 0);
                    }
                    
                    while (!API.IsControlJustPressed(0, 22) && !API.IsControlJustPressed(0, 38) && API.IsPedInAnyHeli(player))
                    {
                        if (isRefueling) return;
                        LeaveHeliCheck();
                        Screen.ShowSubtitle("~bold~~r~Press SPACE to start APU. OR Press E to disable Rotor Brake.", 0);
                        await Delay(10);
                    }

                    if (API.IsControlJustPressed(0, 38))
                    {
                        await Delay(1000);
                        brakeText = "~c~BRAKE";
                        rotorBrakeEnabled = false;
                        _ticks.Off(ForceHeliEngineOff);
                        API.SetVehicleEngineOn(vehicle, true, false, false);
                        await Delay(7000);
                        if (!API.IsPedInAnyHeli(player)) return;
                        speedText = "~g~SPEED";
                        API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                        await Delay(1750);
                        tempsText = "~y~TEMPS";
                        await Delay(3000);
                        tempsText = "~g~TEMPS";
                        API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                        await Delay(4500);
                        inAir = true;
                        await Delay(5000);
                        _ticks.On(AltitudeCallouts);
                        return;
                    }

                    if (API.GetEntityHeightAboveGround(player) > 3f) return;

                    apuText = "~y~APU";
                    rotorBrakeEnabled = true;
                    tempsText = "~c~TEMPS";
                    await Delay(2000);

                    if (API.GetEntityHeightAboveGround(player) > 3f) return;

                    var playerHeading = API.GetEntityHeading(player);
                    if (!API.IsPedInAnyHeli(player)) return;
                    if (API.GetEntityHeightAboveGround(player) > 3f) return;
                    _soundService.Play("APUStart.wav", apuVolume);
                    await Delay(28000);
                    _ticks.On(APULoop);
                    apuText = "~g~APU";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(1750);
                    if (API.GetEntityHeightAboveGround(player) > 3f) return;
                    while (!API.IsControlJustPressed(0, 22))
                    {
                        if (isRefueling) return;
                        LeaveHeliCheck();
                        Screen.ShowSubtitle("~bold~~r~Press SPACE to stop Engine One.", 0);
                        await Delay(10);
                    }
                    if (!API.IsPedInAnyHeli(player)) return;

                    if (API.GetEntityHeightAboveGround(player) > 3f) return;

                    eng1Text = "~y~ENG 1";
                    rotorBrakeEnabled = true;
                    await Delay(1000);
                    _ticks.On(ForceHeliEngineOff);
                    await Delay(7000);
                    eng1Text = "~c~ENG 1";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(1750);
                    if (!API.IsPedInAnyHeli(player)) return;
                    while (!API.IsControlJustPressed(0, 22))
                    {
                        if (isRefueling) return;
                        LeaveHeliCheck();
                        Screen.ShowSubtitle("~bold~~r~Press SPACE to stop Engine Two.", 0);
                        await Delay(10);
                    }
                    eng2Text = "~y~ENG 2";
                    await Delay(7000);
                    eng2Text = "~c~ENG 2";
                    API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                    await Delay(1750);
                    if (!API.IsPedInAnyHeli(player)) return;
                    while (!API.IsControlJustPressed(0, 22))
                    {
                        if (isRefueling) return;
                        Screen.ShowSubtitle("~bold~~r~Press SPACE to stop APU.", 0);
                        await Delay(10);
                    }

                    _ticks.Off(FuelSystem);

                    await Delay(750);
                    apuText = "~y~APU";
                    await Delay(5000);
                    _ticks.Off(APULoop);
                    await Delay(2000);
                    _soundService.Play("APUStop.wav");
                    apuText = "~c~APU";
                    await Delay(1000);
                    API.SetVehicleEngineOn(vehicleIsIn, false, true, true);
                }
            }
        }
        #endregion Heli Start/Stop

        #region Play Callout Sounds
        private void PlayAltitudeCallout(ref bool soundPlayed, float currentAltitude, float threshold, string altitudeName)
        {
            if (!soundPlayed && currentAltitude <= threshold)
            {
                _soundService.Play(altitudeName, altVolume);
                soundPlayed = true;
            }
            else if (currentAltitude > threshold)
            {
                soundPlayed = false;
            }
        }
        #endregion Play Callout Sounds

        #region Fuel System
        private async Task FuelSystem()
        {
            var player = Game.PlayerPed.Handle;
            var height = API.GetEntityHeightAboveGround(player);
            if (!API.IsPedInAnyHeli(player)) return;

            var vehicleIsIn = API.GetVehiclePedIsIn(player, true);
            var pedInSeat = API.GetPedInVehicleSeat(vehicleIsIn, -1);
            if (pedInSeat != player) return;

            if (height < 10f) fuelStatus = fuelStatus - 0.5f;
            else fuelStatus = fuelStatus - 1;

            //Debug.WriteLine($"HeliHUD: FUEL REMAINING = {fuelStatus} SECS!");

            if (height > 19f)
            {
                float percentage = (fuelStatus / 2350f) * 100f;
                percentage = (int)Math.Round(percentage);
                switch (percentage)
                {
                    case <30:
                        Screen.ShowSubtitle($"~r~Fuel: {percentage}%", 1000);
                        break;
                    
                    case <60:
                        Screen.ShowSubtitle($"~y~Fuel: {percentage}%", 1000);
                        break;

                    case >59:
                        Screen.ShowSubtitle($"~g~Fuel: {percentage}%", 1000);
                        break;

                    default:
                        Screen.ShowSubtitle($"~g~Fuel: {percentage}%", 1000);
                        break;
                }
            }

            if (fuelStatus < 5)
            {
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                await Delay(2000);
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                Screen.ShowSubtitle($"~r~NO FUEL", 45000);
                await Delay(3000);
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                _ticks.On(ForceHeliEngineOff);
                await Delay(4000);
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                _ticks.Off(ForceHeliEngineOff);
                await Delay(3000);
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                _ticks.On(ForceHeliEngineOff);
                await Delay(2500);
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                _ticks.Off(ForceHeliEngineOff);
                await Delay(3000);
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                _ticks.On(ForceHeliEngineOff);
                await Delay(2000);
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                _ticks.Off(ForceHeliEngineOff);
                await Delay(1000);
                API.PlaySoundFromEntity(48, "Landing_Tone", vehicleIsIn, "DLC_PILOT_ENGINE_FAILURE_SOUNDS", true, 0);
                _ticks.On(ForceHeliEngineOff);
                await Delay(25000);
                return;
            }

            await Delay(1000);
        }

        private async Task StartRefueling()
        {
            await Delay(2);
            _ticks.Off(FuelSystem);
            var player = Game.PlayerPed;
            var vehicle = API.GetVehiclePedIsIn(player.Handle, false);

            isRefueling = true;
            Screen.ShowSubtitle(" ", 0);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter Fuel", "info", "Refueling started!", new NewNotificationMessageContent[0]));

            while (fuelStatus < 2350)
            {
                API.FreezeEntityPosition(vehicle, true);
                fuelStatus = fuelStatus + 15;
                float percentage = (fuelStatus / 2350) * 100f;
                percentage = (int)Math.Round(percentage);
                Screen.ShowSubtitle($"~y~Refueling: {percentage}%!", 1000);
                await Delay(1000);
            }

            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter Fuel", "success", "Tank refueled!", new NewNotificationMessageContent[0]));
            isRefueling = false;
            API.FreezeEntityPosition(vehicle, false);
            _ticks.On(FuelSystem);
            _ticks.Off(StartRefueling);
        }
        #endregion Fuel System

        #region Heli Taxi
        private async void ToggleHeliTaxi()
        {
            var aces = await _permissionService.GetUserAces();
            if (!aces.IsDeveloper && !aces.IsNpasTrained) return;
            var player = Game.PlayerPed.Handle;
            if (!API.IsPedInAnyHeli(player)) return;
            var vehicle = API.GetVehiclePedIsIn(player, false);
            var height = API.GetEntityHeightAboveGround(player);
            if (height > 7f) return;


            var pedInSeat = API.GetPedInVehicleSeat(vehicle, -1);
            if (pedInSeat != player) return;

            var aw109Hash = (uint)API.GetHashKey("aw109vip");
            var dauphinHash = (uint)API.GetHashKey("addpolmh65");
            var modelHash = (uint)API.GetEntityModel(vehicle);
            if (modelHash != dauphinHash && modelHash != aw109Hash) return;  

            if (eng1Text != "~g~ENG 1" && eng2Text != "~g~ENG 2")
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter Taxi Mode", "error", "The helicopter requires power from the engines to taxi!", new NewNotificationMessageContent[0]));
                _ticks.Off(HelicopterTaxi);
                heliTaxiEnabled = false;
                return;
            }

            if (heliTaxiEnabled == true)
            {
                _ticks.Off(HelicopterTaxi);
                heliTaxiEnabled = false;
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter Taxi Mode", "success", "Taxi mode has been disabled!", new NewNotificationMessageContent[0]));
                return;
            }
            
            _ticks.On(HelicopterTaxi);
            heliTaxiEnabled = true;
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter Taxi Mode", "success", "Taxi mode has been enabled!", new NewNotificationMessageContent[0]));
        }

        private async Task HelicopterTaxi()
        {
            await Delay(10);

            var aces = await _permissionService.GetUserAces();
            if (!aces.IsDeveloper && !aces.IsNpasTrained) return;
            var player = Game.PlayerPed.Handle;
            if (!API.IsPedInAnyHeli(player)) return;
            var vehicle = API.GetVehiclePedIsIn(player, false);
            var height = API.GetEntityHeightAboveGround(player);
            if (height > 7f) return;

            if (eng1Text != "~g~ENG 1" && eng2Text != "~g~ENG 2")
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Helicopter Taxi Mode", "error", "The helicopter requires power from the engines to taxi!", new NewNotificationMessageContent[0]));
                _ticks.Off(HelicopterTaxi);
                heliTaxiEnabled = false;
                return;
            }

            API.SetHeliBladesSpeed(vehicle, 0.57785f);
            Debug.WriteLine("Helicopter Taxi Mode Enabled...");
        }
        #endregion Heli Taxi

        private void DevSetHeliAOG()
        {
            var player = Game.PlayerPed.Handle;
            var vehicle = Game.PlayerPed.CurrentVehicle.Handle;
            var playerIndex = API.NetworkPlayerIdToInt();

            API.DecorSetBool(vehicle, "helihud_aog_state", true);
            API.DecorSetInt(vehicle, "helihud_aog_playerIndex", playerIndex);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("HeliHUD Dev Tool", "success", "SET THE AIRCRAFT TO AGO STATE AND UPDATED PILOT INDEX DECORS!", new NewNotificationMessageContent[0]));
        }


        private async Task PropWash()
        {
            await Delay(100);

            var playerHandle = Game.PlayerPed.Handle;
            if (API.IsPedInAnyVehicle(playerHandle, false)) return;

            var playerPos = Game.PlayerPed.Position;
            Vehicle nearestHeli = null;
            var nearestDistance = float.MaxValue;

            foreach (var veh in World.GetAllVehicles())
            {
                if (API.GetVehicleClass(veh.Handle) == 15)
                {
                    float dist = (playerPos - veh.Position).Length();
                    if (dist < nearestDistance)
                    {
                        nearestDistance = dist;
                        nearestHeli = veh;
                    }
                }
            }

            float propWashDistance = 16f;
            if (nearestHeli == null || nearestDistance > propWashDistance) return;
            if (Vector3.Distance(playerPos, nearestHeli.Position) <= 3.45f) return;

            bool engineOn = API.IsVehicleEngineOn(nearestHeli.Handle);
            if (!engineOn) return;
            if (MovementHandler._currentState == MovementStates.Crouched) return;

            Vector3 forceDirection = playerPos - nearestHeli.Position;
            forceDirection.Normalize();

            var forceMultiplier = 8.95f; 
            var upwardForce = 1.95f;
            var forceX = forceDirection.X * forceMultiplier;
            var forceY = forceDirection.Y * forceMultiplier;
            var forceZ = upwardForce;

            API.ApplyForceToEntity(
                playerHandle,
                1,             
                forceX,
                forceY,
                forceZ,
                0f, 0f, 0f,     
                0,              
                false,          
                true,           
                true,           
                false,
                true
            );

            await Delay(350);
            API.SetPedToRagdoll(playerHandle, 3000, 3000, 0, false, false, false);
            await Delay(100);
        }

        private async Task TankerSpawner()
        {
            await Delay(100);
            var aces = await _permissionService.GetUserAces();
            if (!aces.IsDeveloper && !aces.IsNpasTrained) return;

            var player = Game.PlayerPed.Handle;
            var playerPos = Game.PlayerPed.Position;

            var spawnerPos = new Vector3(-1799.436f, -3155.019f, 13.944f);
            var spawnPos = new Vector3(-1812.59f, -3178.055f, 13.944f);
            while (playerPos.DistanceToSquared(spawnerPos) <= 5.0f)
            {
                await Delay(3);
                Screen.ShowSubtitle("~r~Press E to spawn Fuel Tanker!", 5000);
                if (API.IsControlJustPressed(0, 51))
                {
                    var modelHash = (uint)API.GetHashKey("tanker");
                    API.RequestModel(modelHash);
                    while (!API.HasModelLoaded(modelHash))
                    {
                        await Delay(100);
                    }

                    var tankerTrailer = API.CreateVehicle(modelHash, spawnPos.X, spawnPos.Y, spawnPos.Z, 328.7f, true, false);
                    Screen.ShowSubtitle("~r~Tanker has spawned!", 3000);
                    await Delay(15000);
                    return;
                }
            }
        }
    }
}