using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using static CitizenFX.Core.Native.API;
using MenuAPI;
using PoliceMP.Client.Extensions;

namespace PoliceMP.Client.Scripts.LFBPanic
{
    public class LFBPanicScript : Script
    {
        private readonly ILogger<LFBPanicScript> _logger;
        private readonly IClientCommunicationsManager _comms;
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _legacyComms;
        private readonly ICommandManager _commands;
        private readonly IPermissionService _permission;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IFeatureService _featureService;
        private readonly IExportsAccessor _exportsAccessor;
        private readonly ISoundService _soundService;

        private UserAces _userAccess;

        public static List<Vector3> Positions { get; set; } = new List<Vector3>();
        private int passAlarmStatus { get; set; } = 0;
        private bool passPanicTriggered { get; set; } = false;
        private bool soundPlaying { get; set; } = false;
        private readonly int _netid = Game.Player.Character.NetworkId;
        private readonly int BaIDUntie = 214;
        private readonly int BaIDLink = 213;
        private float alarmVolume = 0.85f;
        private int savedAlarVolume = 85;
        private int AlarmSource = PlayerId();
        private float AlarmDistance = 30;

        public LFBPanicScript(ILogger<LFBPanicScript> logger, ITickManager ticks,
            ILegacyClientCommunicationsManager legacyComms,
            ICommandManager commands, IPermissionService permission, INewNotificationOverlay newNotificationOverlay,
            IFeatureService featureService, IExportsAccessor exportsAccessor, ISoundService soundService,
            IClientCommunicationsManager comms)
        {
            _logger = logger;
            _ticks = ticks;
            _legacyComms = legacyComms;
            _commands = commands;
            _permission = permission;
            _newNotificationOverlay = newNotificationOverlay;
            _featureService = featureService;
            _exportsAccessor = exportsAccessor;
            _soundService = soundService;
            _comms = comms;
        }

        protected override async Task OnStartAsync()
        {
            _legacyComms.On(ServerEvents.FirefighterPass, (int passAlarm) =>
            {
                // Console.WriteLine("FirefighterPass event received. Client side.");
                AlarmHandler(passAlarm);
            });


            _commands.Register("pass").WithHandler(async () =>
            {
                var aces = await _permission.GetUserAces();

                if (!(aces.IsDeveloper || aces.IsFireTrained || aces.IsFruTrained || aces.IsTierTwo))
                {
                    return;
                }

                // _logger.Debug("Pass Alarm Command Executing.");
                await Delay(100);
                EnablePass(); // Enables Pass Alarm
            });

            _commands.Register("passreset").WithHandler(async () =>
            {
                var aces = await _permission.GetUserAces();

                if (!(aces.IsDeveloper || aces.IsFireTrained || aces.IsFruTrained || aces.IsTierTwo))
                {
                    return;
                }

                // _logger.Debug("Pass Alarm Reset Command Executing.");
                await Delay(100);
                passReset();
            });

            _commands.Register("passalarm").WithHandler(async () =>
            {
                var aces = await _permission.GetUserAces();

                if (!(aces.IsDeveloper || aces.IsFireTrained || aces.IsFruTrained || aces.IsTierTwo))
                {
                    return;
                }

                await Delay(100);
                passAlarm();

            });
            
            _commands.Register("passmenu").WithHandler(async () =>
            {
                MenuController.CloseAllMenus();

                var aces = await _permission.GetUserAces();
                if (!(aces.IsDeveloper || aces.IsFireTrained || aces.IsFruTrained || aces.IsTierTwo))
                {
                    return;
                }

                var passAlarmMenu = new Menu("Pass Alarm Menu", "Enable or Disable your Pass Alarm");
                MenuController.EnableMenuToggleKeyOnController = false;
                MenuController.MenuToggleKey = (Control)(-1);
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                MenuController.AddMenu(passAlarmMenu);

                await Delay(0);
                passAlarmMenu.OpenMenu();

                var passAlarmOn = new MenuItem("Enable Pass Alarm", "Turns on Pass Alarm.");
                passAlarmMenu.AddMenuItem(passAlarmOn);

                var passAlarmOff = new MenuItem("Disable Pass Alarm", "Turns off Pass Alarm.");
                passAlarmMenu.AddMenuItem(passAlarmOff);

                var passManualAlarm = new MenuItem("Trigger Pass Alarm", "Triggers the manual Pass Alarm.");
                passAlarmMenu.AddMenuItem(passManualAlarm);

                var passAlarmReset = new MenuItem("Reset Pass Alarm", "Resets the Pass Alarm.");
                passAlarmMenu.AddMenuItem(passAlarmReset);
                
                var passAlarmVolume = new MenuItem("Pass Alarm Volume", "Adjust the volume of the Pass Alarm.");
                passAlarmMenu.AddMenuItem(passAlarmVolume);
                

                passAlarmMenu.OpenMenu();

                passAlarmMenu.OnItemSelect += async (menu, item, index) =>
                {
                    if (menu != passAlarmMenu) return;

                    if (item == passAlarmOn)
                    {
                        // _logger.Debug("Pass Alarm Command Executing.");
                        await Delay(100);
                        EnablePass(); // Enables Pass Alarm
                    }
                    if (item == passAlarmOff)
                    {
                        //_logger.Debug("Pass Alarm Reset Command Executing.");
                        await Delay(100);
                        DisablePass();
                    }
                    if (item == passManualAlarm)
                    {
                        await Delay(100);
                        passAlarm();
                    }
                    if (item == passAlarmReset)
                    {
                        await Delay(100);
                        passReset();
                    }
                    if (item == passAlarmVolume)
                    {
                        GetSavedVolume();
                        await Delay(100);
                        AddTextEntry("FMMC_KEY_TIP1", "Enter Alarm Volume (0-100)");
                        DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "" + alarmVolume, "", "", "", 8);

                        UpdateOnscreenKeyboard();

                        while (UpdateOnscreenKeyboard() == 0)
                        {
                            await Delay(10);
                            UpdateOnscreenKeyboard();
                        }

                        if (UpdateOnscreenKeyboard() == 1)
                        {
                            var alarmStr = GetOnscreenKeyboardResult();
                            if (!int.TryParse(alarmStr, out int volumeInt))
                            {
                                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm",
                                    "error",
                                    $"Error updating your selected volume, ensure you have chosen a number between 0 and 100.",
                                    new NewNotificationMessageContent[0]));
                                return;
                            }

                            alarmVolume = volumeInt / 100f;
                            // _logger.Debug($"ALARM VOLUME HAS BEEN SET TO {alarmVolume}");
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm", "success",
                                $"You have set the alarm volume to: {volumeInt}%",
                                new NewNotificationMessageContent[0]));
                            savedAlarVolume = volumeInt;
                            SaveVolume();
                            // _logger.Debug($"{savedAlarVolume} has been saved to the cache.");
                        }
                    }
                    
                    MenuController.CloseAllMenus();
                };
            });
        }

        private void GetSavedVolume()
        {
            var cachedVolume = GetResourceKvpInt("passAlarmVolume");
            savedAlarVolume = cachedVolume;
            alarmVolume = (float)savedAlarVolume;
        }

        private void SaveVolume()
        {
            SetResourceKvpInt("passAlarmVolume", savedAlarVolume);
        }
        

        private async Task PlayAnimationRadio()
        {
            while (!HasAnimDictLoaded("random@arrests"))
            {
                RequestAnimDict("random@arrests");
                await Delay(100);
            }
            
            Ped player = Game.Player.Character;
            TaskPlayAnim(player.Handle, "random@arrests", "generic_radio_chatter", 8.0F, 2.0F, 600, 50, 2.0F, false,
                false, false);
            if (IsEntityPlayingAnim(player.Handle, "random@arrests", "generic_radio_chatter", 3))
            {
                ClearPedSecondaryTask(player.Handle);
                SetCurrentPedWeapon(player.Handle, (uint)GetHashKey("GENERIC_RADIO_CHATTER"), true);
            }
        }

        private void EnablePass()
        {
            Ped player = Game.Player.Character;
            if (_permission.CurrentUserRole == null || (_permission.CurrentUserRole.Division != UserDivision.FRU &&
                                                        _permission.CurrentUserRole.Division != UserDivision.LFB))
            {
                // _logger.Debug("User does not have the correct permissions to enable the pass alarm.");
                return;
            }

            if (!(GetPedDrawableVariation(player.Handle, 8) == BaIDUntie || GetPedDrawableVariation(player.Handle, 8) == BaIDLink)) // Checks to see if they have component 228
            {
                // _logger.Debug("User does not have the correct component to enable the pass alarm.");
                return;
            }

            if (passAlarmStatus == 0)
            {
                // _logger.Debug("Pass Alarm Enabled");
                passAlarmStatus = 1;
                _soundService.Play("passon.wav", 0.3f);
                PlayAnimationRadio();
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm", "info",
                    "Pass Alarm Enabled", new NewNotificationMessageContent[0]));
                _ticks.On(CheckPassAlarm);
            }
            else
            {
                // _logger.Debug("Pass Alarm Disabled");
                DisablePass();
            }
        }

        private void AlarmHandler(int passAlarm)
        {
            if (_permission.CurrentUserRole == null || (_permission.CurrentUserRole.Division != UserDivision.FRU &&
                                                        _permission.CurrentUserRole.Division != UserDivision.LFB))
            {
                // Console.WriteLine("User does not have the correct permissions to handle the pass alarm.");
                return;
            }

            // _logger.Debug("Alarm Handler Triggered.");
            switch (passAlarm)
            {
                case 1:
                    soundPlaying = false;
                    _ticks.Off(stage1sound);
                    _ticks.Off(stage2sound);
                    break;
                case 2:
                    soundPlaying = true;
                    _ticks.On(stage1sound);
                    break;
                case 3:
                    soundPlaying = true;
                    _ticks.On(stage2sound);
                    break;
                default:
                    break;
            }

        }

        private async Task stage1sound()
        {
            // _logger.Debug("Playing stage1pass.wav");
            _soundService.Play("stage1pass.wav", alarmVolume);
            await Delay(13000);
        }

        private async Task stage2sound()
        {
            // _logger.Debug("Playing stage2pass.wav");
            _soundService.Play("stage2pass.wav", alarmVolume);
            await Delay(5000);
        }

        private async Task CheckPassAlarm()
        {
            double tolerance = 0.0001;
            if (passPanicTriggered) return;
            if (passAlarmStatus == 0 && Positions.Count >= 1)
            {
                Positions.Clear();
            }
            else if (passAlarmStatus == 1)
            {
                var playerPed = Game.Player.Character.Handle;
                var playerCoords = GetEntityCoords(playerPed, false);

                Positions.Add(playerCoords);
                // _logger.Debug("Position Added to List");
                await Delay(1000);

                if (Positions.Count >= 18)
                {
                    var total = new Vector3(0, 0, 0);
                    foreach (var pos in Positions)
                    {
                        total += pos;
                    }

                    var average = total / Positions.Count;

                    if (Math.Abs(average.X - playerCoords.X) <= tolerance ||
                        Math.Abs(average.Y - playerCoords.Y) <= tolerance ||
                        Math.Abs(average.Z - playerCoords.Z) <= tolerance)
                    {
                        passAlarmStatus = 2;
                        // _logger.Debug("Pass Alarm Triggered");
                        _legacyComms.ToServer(ServerEvents.FirefighterPass, passAlarmStatus);
                    }

                    Positions.RemoveAt(0);
                }
            }
            else if (passAlarmStatus == 2)
            {
                var firstCoord = Game.Player.Character.Position;
                await Delay(1000);
                var secondCoord = Game.Player.Character.Position;

                if (Math.Abs(firstCoord.X - secondCoord.X) > tolerance ||
                    Math.Abs(firstCoord.Y - secondCoord.Y) > tolerance ||
                    Math.Abs(firstCoord.Z - secondCoord.Z) > tolerance)
                {
                    passAlarmStatus = 1;
                    // Trigger server event to stop playing alarm
                    // _logger.Debug("Pass Alarm Stopped by movement");
                    _legacyComms.ToServer(ServerEvents.FirefighterPass, passAlarmStatus);
                }
            }
        }

        private void DisablePass()
        {
            _soundService.Play("passoff.wav", 0.3f);
            passAlarmStatus = 0;
            _legacyComms.ToServer(ServerEvents.FirefighterPass, passAlarmStatus);

            _ticks.Off(CheckPassAlarm);
            
            PlayAnimationRadio();
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm", "info",
                "Pass Alarm Disabled", new NewNotificationMessageContent[0]));
            if (soundPlaying)
            {
                soundPlaying = false;
                _ticks.Off(stage1sound);
                _ticks.Off(stage2sound);
            }
        }

        private void passReset()
        {
            if (_permission.CurrentUserRole == null || (_permission.CurrentUserRole.Division != UserDivision.FRU &&
                                                        _permission.CurrentUserRole.Division != UserDivision.LFB))
            {
                return;
            }

            switch (passAlarmStatus)
            {
                case 0:
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm", "error",
                        "Pass Alarm is not enabled", new NewNotificationMessageContent[0]));
                    break;
                case 1:
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm", "info",
                        "Your Pass Alarm is enabled, but not in Alarm; there is nothing to reset",
                        new NewNotificationMessageContent[0]));
                    break;
                case 2:
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm", "info",
                        "Your Pass Alarm has been reset, remember moving will reset the alarm.",
                        new NewNotificationMessageContent[0]));
                    PlayAnimationRadio();
                    passAlarmStatus = 1;
                    passPanicTriggered = false;
                    _legacyComms.ToServer(ServerEvents.FirefighterPass, passAlarmStatus);
                    break;
                case 3:
                    PlayAnimationRadio();
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm", "info",
                        "Your Pass Alarm is enabled and in Alarm; it has been reset",
                        new NewNotificationMessageContent[0]));
                    passAlarmStatus = 1;
                    passPanicTriggered = false;
                    _legacyComms.ToServer(ServerEvents.FirefighterPass, passAlarmStatus);
                    break;
            }
        }

        private async void passAlarm()
        {

            if (_permission.CurrentUserRole == null || (_permission.CurrentUserRole.Division != UserDivision.FRU &&
                                                        _permission.CurrentUserRole.Division != UserDivision.LFB))
            {
                return;
            }

            switch (passAlarmStatus)
            {
                case 0:
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm", "error",
                        "Pass Alarm is not enabled", new NewNotificationMessageContent[0]));
                    break;
                case 1:
                case 2:
                    PlayAnimationRadio();
                    await Delay(300);

                    passAlarmStatus = 3;
                    passPanicTriggered = true;

                    // Trigger server event to play stage2pass.wav
                    // _logger.Debug("Pass Alarm status 3 Triggered");
                    _legacyComms.ToServer(ServerEvents.FirefighterPass, passAlarmStatus);

                    break;
                case 3:
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Pass Alarm", "error",
                        "Pass Alarm is already in Alarm", new NewNotificationMessageContent[0]));
                    break;
                default:
                    // Add a default case to handle unexpected values of passAlarmStatus
                    _logger.Error($"Unexpected passAlarmStatus value: {passAlarmStatus}");
                    break;
            }
        }
    }
}