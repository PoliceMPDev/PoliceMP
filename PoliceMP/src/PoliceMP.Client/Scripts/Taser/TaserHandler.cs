using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.Taser
{
    public class TaserHandler : Script
    {
        public static bool DriveStunEnabled = false;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly INotificationService _notification;
        private readonly ILogger<TaserHandler> _logger;
        private readonly ITickManager _ticks;
        private readonly ICommandManager _command;
        private readonly ISoundService _sounds;

        private int _loadedCartridges = 2;
        private static int _transportCartridges = 2;
        private static int _driveStunKey = 38;
        private readonly int _taserHash = GetHashKey("WEAPON_STUNGUN");
        private readonly Ped _player = Game.Player.Character;
        private readonly int _netid = Game.Player.Character.NetworkId;
        private DateTime _driveStunTime;
        private bool _taserSafetyStatus;
        private bool _awaitingReactivation1;
        private bool _awaitingReactivation2;
        private int _suspect1;
        private int _suspect2;
        private bool _detectedAiming;
        private bool _displayedInitial;


        private static readonly string SerialNumber = "X" + new Random().Next(21202574, 49202574);
        private static string _batteryStatus = Convert.ToString(new Random().Next(95, 99));
        private static bool _logEnabled = true;
        
        private static string _notificationTitle = $"STUNGUN X2 ({SerialNumber})";


        public TaserHandler(ILegacyClientCommunicationsManager comms, INotificationService notification,
            ILogger<TaserHandler> logger, ITickManager ticks, ICommandManager command)
        {
            _comms = comms;
            _notification = notification;
            _logger = logger;
            _ticks = ticks;
            _command = command;

            _ticks.On(OnTick);

            var getDriveStunKey = int.TryParse(GetResourceMetadata(GetCurrentResourceName(), "driveStunKey", 0), out _driveStunKey);
            
            SetPedMinGroundTimeForStungun(PlayerPedId(), 5000);
            
            _comms.On(ClientEvents.TaserReload, () =>
            {
                _logger.Debug("TaserReload");

                ResetTaser();
            });
            
            _comms.On<int, Vector3>(ClientEvents.TaserDetectSuspect, (carrierNetworkId, carrierLocation) =>
            {
                _logger.Debug($"TaserDetectSuspect - carrierNetworkId: {carrierNetworkId}, carrierLocation: {carrierLocation}");
                if (!IsPedRagdoll(PlayerPedId()) && !IsPedBeingStunned(PlayerPedId(), 0))
                {
                    return;
                }
                
                var suspectLocation = Game.Player.Character.Position;
                var compare = Vdist(suspectLocation.X, suspectLocation.Y, suspectLocation.Z, carrierLocation.X, carrierLocation.Y, carrierLocation.Z);
                if (compare < 30.0f)
                {
                    _comms.ToServer(ServerEvents.TaserSuspectDetected, carrierNetworkId, _netid);
                }
            });
            
            _comms.On<int, int>(ClientEvents.TaserSuspectDetected, (carrierNetworkId, suspectPlayerId) =>
            {
                _logger.Debug($"TaserSuspectDetected - carrierNetworkId: {carrierNetworkId}, suspectPlayerId: {suspectPlayerId}");

                if (_netid != carrierNetworkId)
                {
                    return;
                }
                
                if (_suspect1 == 0 & _loadedCartridges == 1)
                {
                    _suspect1 = suspectPlayerId;
                    _awaitingReactivation1 = true;
                }
                else if (_suspect2 == 0 & _loadedCartridges == 0)
                {
                    _suspect2 = suspectPlayerId;
                    _awaitingReactivation2 = true;
                }
            });
            
            _comms.On<int>(ClientEvents.TaserReactivateSuspect, (suspectPlayerId) =>
            {
                _logger.Debug($"TaserReactivateSuspect - suspectPlayerId: {suspectPlayerId}");

                if (_netid != suspectPlayerId)
                {
                    return;
                }
                
                _notification.Info(_notificationTitle, $"You are being reactivated.");
                SetPedToRagdoll(PlayerPedId(), 5000, 5000, 0, true, true, false);
            });
            
            _comms.On<int, int, Vector3, int>(ClientEvents.TaserCheckLocation, (suspectPlayerId, carrierNetId, carrierLocation, suspectNumber) =>
            {
                _logger.Debug($"TaserCheckLocation - suspectPlayerId: {suspectPlayerId}, carrierNetId: {carrierNetId}, carrierLocation: {carrierLocation}, suspectNumber: {suspectNumber}");

                
                if (_netid != suspectPlayerId)
                {
                    return;
                }
                
                var suspectLocation = Game.Player.Character.Position;
                var compareDistance = Vdist(suspectLocation.X, suspectLocation.Y, suspectLocation.Z, carrierLocation.X, carrierLocation.Y, carrierLocation.Z);
                
                if (compareDistance < 10.0f)
                {
                    return;
                }
                
                _notification.Info(_notificationTitle, $"Your barbs have been ~y~disconnected~w~.");
                _comms.ToServer(ServerEvents.TaserBarbsInvalidated, carrierNetId, suspectNumber);
            });
            
            _comms.On<int, int>(ClientEvents.TaserBarbsInvalidated, (carrierNetId, suspectNumber) =>
            {
                _logger.Debug($"TaserBarbsInvalidated - carrierNetId: {carrierNetId}, suspectNumber: {suspectNumber}");

                if (_netid != carrierNetId)
                {
                    return;
                }

                switch (suspectNumber)
                {
                    case 1:
                        _awaitingReactivation1 = false;
                        _suspect1 = 0;
                        _notification.Info(_notificationTitle, $"Barbs Disconnected - ~b~Cartridge 1");
                        break;
                    case 2:
                        _awaitingReactivation2 = false;
                        _suspect2 = 0;
                        _notification.Info(_notificationTitle, $"~Barbs Disconnected - ~b~Cartridge 2");
                        break;
                }
            });
            
            _comms.On<int>(ClientEvents.TaserNotifyBarbsRemoved, (suspectPlayerId) =>
            {
                if (_netid == suspectPlayerId)
                {
                    _notification.Info(_notificationTitle, $"Your barbs have been ~y~removed~w~.");
                }
            });

            _comms.On<string>(ClientEvents.TaserDisplayNotification, DisplayTopNotification);
            _comms.On<bool>(ClientEvents.TaserUpdateLog, LogStatus);
            
            _command.Register("rt").WithHandler(TaserReloadHandler);
            _command.Register("resettaser").WithHandler(ResetTaser);
            _command.Register("ts").WithHandler(TaserSafety);
            _command.Register("rb1").WithHandler(RemoveBarbs1);
            _command.Register("rb2").WithHandler(RemoveBarbs2);
            _command.Register("taserlogging").WithHandler(TaserLogging);
            _command.Register("taserlogging").WithHandler(TaserLogging);
            _command.Register("stun").WithHandler(() => DriveStun(_loadedCartridges, _transportCartridges));
        }

        private async Task OnTick()
        {
            if (HudWeaponWheelGetSelectedHash() == _taserHash)
            {
                if (_displayedInitial == false)
                {
                    var taserId = GetTaserInformation();
                    Debug.WriteLine($"Taser issued - Serial: {taserId}");
                    _notification.Info(_notificationTitle, $"Your taser has been issued.");
                    _displayedInitial = true;
                }
                
                if (_player.IsAiming && ((GetSelectedPedWeapon(PlayerPedId()) == _taserHash) && (_detectedAiming == false)))
                {
                    UpdateLog("Armed - Red Dot", _loadedCartridges, DateTime.Now, 16722944);
                    _detectedAiming = true;
                }


                if (DriveStunEnabled)
                {
                    DisablePlayerFiring(_player.Handle, true);

                    if (Game.IsDisabledControlPressed(32, Control.Attack) && _detectedAiming)
                    {
                        CheckPedDriveStun();
                    }

                    if ((IsControlJustReleased(1, _driveStunKey) && ((GetSelectedPedWeapon(PlayerPedId()) == _taserHash)) && !IsPedInAnyVehicle(PlayerPedId(), false) & (_taserSafetyStatus == false)))
                    {
                        _logger.Debug("Drive stun");
                        if ((DateTime.Now - _driveStunTime).TotalMilliseconds > (double)5000)
                        {
                            DriveStun(_loadedCartridges, _transportCartridges);
                            _driveStunTime = DateTime.Now;
                        }
                        else
                        {
                            _notification.Info(_notificationTitle, $"Drive Stun Cooldown - ~w~5 Seconds");
                        }
                    }

                }

                if (_awaitingReactivation1 || _awaitingReactivation2)
                {
                    if (IsControlJustReleased(1, 10) && _awaitingReactivation1)
                    {
                        DisplayTopNotification("Reactivated cartridge: ~INPUT_SELECT_WEAPON_UNARMED~");
                        _notification.Info(_notificationTitle, $"Use <code>/rb1</code> (Remove Barbs)");
                        ReactivateSuspect(_loadedCartridges, _suspect1, 1);
                    }
                    else if (IsControlJustReleased(1, 11) & _awaitingReactivation2)
                    {
                        DisplayTopNotification("Reactivated cartridge: 	~INPUT_SELECT_WEAPON_MELEE~");
                        _notification.Info(_notificationTitle, $"Use <code>/rb2</code> (Remove Barbs)");
                        ReactivateSuspect(_loadedCartridges, _suspect2, 2);
                    }
                    if (_awaitingReactivation1)
                    {
                        if (IsPedDeadOrDying(PlayerPedId(), true))
                        {
                            _awaitingReactivation1 = false;
                        } 
                        var carrierLocation = Game.Player.Character.Position;
                        _comms.ToServer(ServerEvents.TaserCheckLocation, _suspect1, _netid, carrierLocation, 1);
                    }
                    if (_awaitingReactivation2)
                    {
                        if (IsPedDeadOrDying(PlayerPedId(), true))
                        {
                            _awaitingReactivation2 = false;
                        }
                        var carrierLocation = Game.Player.Character.Position;
                        _comms.ToServer(ServerEvents.TaserCheckLocation, _suspect2, _netid, carrierLocation, 2);
                    }
                }
                if (HudWeaponWheelGetSelectedHash() == _taserHash && (_loadedCartridges == 0 || _taserSafetyStatus))
                {
                    TaserLock(false);
                }

                if (IsPedShooting(PlayerPedId()) && (GetSelectedPedWeapon(PlayerPedId()) == _taserHash))
                {
                    var carrierLocation = Game.Player.Character.Position;
                    _comms.ToServer(ServerEvents.TaserDetectSuspect, _netid, carrierLocation);

                    var cartridge = 0;
                    _loadedCartridges--;
                    
                    _comms.ToServer(ServerEvents.SoundToRadius, _netid, 30.0f, "taser.ogg", 0.75f);
                    DisplayStatus(_loadedCartridges, "MANUAL", _transportCartridges);
                    
                    cartridge = _loadedCartridges switch
                    {
                        1 => 1,
                        0 => 2,
                        _ => cartridge
                    };
                    
                    UpdateLog("Trigger - Cartridge " + cartridge, _loadedCartridges, DateTime.Now, 16753920);
                }
            }
        }

        private static void DisplayTopNotification(string text)
        {
            BeginTextCommandDisplayHelp ("STRING");
            AddTextComponentSubstringPlayerName(text);
            SetNotificationTextEntry("STRING");
            EndTextCommandDisplayHelp(0, false, true, -1);
        }
        
        private void TaserReloadHandler()
        {
            switch (_loadedCartridges)
            {
                case 0:
                    switch (_transportCartridges)
                    {
                        case 0:
                            _notification.Error(_notificationTitle, $"You have no cartridges left.<br/><br/>You can fetch some more from your boot.");
                            break;
                        case 1:
                            _transportCartridges = 0;
                            _loadedCartridges = 1;
                            TaserLock(false);
                            _notification.Success(_notificationTitle, $"RELOADED<br/><br/>Spare Cartridges: <span class='text-warning'>0</span>");
                            UpdateLog("Reload - Cartridge 1", _loadedCartridges, DateTime.Now, 589883);
                            _detectedAiming = false;
                            break;
                        case 2:
                            _transportCartridges = 0;
                            _loadedCartridges = 2;
                            TaserLock(false);
                            _notification.Success(_notificationTitle, $"RELOADED<br/><br/>Spare Cartridges: <span class='text-warning'>0</span>");
                            UpdateLog("Reload - 2 Cartridges", _loadedCartridges, DateTime.Now, 589883);
                            _detectedAiming = false;
                            break;
                    }    
                    break;
                case 1:
                    switch (_transportCartridges)
                    {
                        case 1:
                            _transportCartridges = 0;
                            _loadedCartridges = 2;
                            TaserLock(false);
                            _notification.Success(_notificationTitle, $"RELOADED<br/><br/>Spare Cartridges: <span class='text-warning'>0</span>");
                            UpdateLog("Reload - Cartridge 1", _loadedCartridges, DateTime.Now, 589883);
                            _detectedAiming = false;
                            break;
                        case 2:
                            _transportCartridges = 1;
                            _loadedCartridges = 2;
                            TaserLock(false);
                            _notification.Success(_notificationTitle, $"RELOADED<br/><br/>Spare Cartridges: <span class='text-warning'>1</span>");
                            UpdateLog("Reload - Cartridge 2", _loadedCartridges, DateTime.Now, 589883);
                            _detectedAiming = false;
                            break;
                    }
                    break;
                case 2:
                    switch (_transportCartridges)
                    {
                        case 0:
                            _notification.Success(_notificationTitle, $"SLOTS FULL<br/><br/>Spare Cartridges: <span class='text-warning'>0</span>");
                            break;
                        case 1:
                            _notification.Success(_notificationTitle, $"SLOTS FULL<br/><br/>Spare Cartridges: <span class='text-warning'>1</span>");
                            break;
                        case 2:
                            _notification.Success(_notificationTitle, $"SLOTS FULL<br/><br/>Spare Cartridges: <span class='text-warning'>2</span>");
                            break;
                    }

                    break;
                default:
                    _loadedCartridges = 2;
                    _notification.Success(_notificationTitle, $"RELOADED");
                    _detectedAiming = false;
                    break;
            }
        }
        
        private void ResetTaser()
        {
            _loadedCartridges = 2;
            _transportCartridges = 2;
            _detectedAiming = false;
            _awaitingReactivation1 = false;
            _awaitingReactivation2 = false;
            _suspect1 = 0;
            _suspect2 = 0;
            TaserLock(false);
            _notification.Success(_notificationTitle, $"Your taser has been reset.");

        }
        
        private void TaserSafety()
        {
            _taserSafetyStatus = TaserSafety(_loadedCartridges, _taserSafetyStatus);
        }
        
        private void RemoveBarbs1()
        {
            _notification.Info(_notificationTitle, $"Barbs Removed - Cartridge: 1");
            _awaitingReactivation1 = false;
            _suspect2 = 0;
        }

        
        private void RemoveBarbs2()
        {
            _notification.Info(_notificationTitle, $"Barbs Removed - Cartridge: 2");
            _awaitingReactivation2 = false;
            _suspect2 = 0;
        }
        
        private void TaserLogging()
        {
            if (_logEnabled)
            {
                _comms.ToServer(ServerEvents.TaserUpdateLog, false);
                _comms.ToServer(ServerEvents.TaserDisplayNotification, "Taser - Tracking Disabled" );
                _notification.Success(_notificationTitle, $"Forcewide Logging: <b class='text-danger'>DISABLED</b>");
                OverrideLog("Forcewide Tracking - Disabled", _loadedCartridges, DateTime.Now, 10093824);
            }
            else
            {
                _comms.ToServer(ServerEvents.TaserUpdateLog, true);
                _comms.ToServer(ServerEvents.TaserDisplayNotification, "Taser - Tracking Enabled");
                _notification.Success(_notificationTitle, $"Forcewide Logging: <b class='text-success'>ENABLED</b>");
                OverrideLog("Forcewide Tracking - Enabled", _loadedCartridges, DateTime.Now, 10093824);
            }
        }
        
        private void DisplayStatus(int cartridges, string mode, int transportCartridges)
        {
            switch (cartridges)
            {
                case 0:
                    _notification.Info(_notificationTitle, $"Mode: <span class='text-warning'>{mode}</span><br><b class='text-danger'>RELOAD REQUIRED</b>");
                    break;
                case 1:
                case 2:
                    _notification.Info(_notificationTitle, $"Mode: <span class='text-warning'>{mode}</span><br>Spare Cartridges: <span class='text-warning'>{transportCartridges}</span>");
                    break;
                default:
                    _notification.Info(_notificationTitle, $"Mode: <span class='text-warning'>{mode}</span><br>Spare Cartridges: <span class='text-warning'>{cartridges}</span>");
                    break;
            }
        }
        
        private void DriveStun(int cartridges, int transportCartridges)
        {
            _logger.Debug("DriveStun");
            var netId = Game.Player.Character.NetworkId;
            var handle = PlayerPedId();
            var hash = (uint)GetHashKey("Ballistic");
            SetPlayerSimulateAiming(handle, true);
            const uint motionHash = 0x3f67c6af;
            ForcePedMotionState(handle, motionHash, false, false, false);
            SetWeaponAnimationOverride(handle, hash);
            DisplayStatus(cartridges, "ARC", transportCartridges);

            _comms.ToServer(ServerEvents.SoundToRadius, netId, 30.0f, "arcsound.ogg", 0.95f);
            
            UpdateLog("Arc", cartridges, DateTime.Now, 1672140);
        }

        private static void TaserLock(bool locked)
        {
            DisablePlayerFiring(PlayerPedId(), locked);
        }

        private static void LogStatus(bool logStatus)
        {
            _logEnabled = logStatus;
        }

        private void UpdateLog(string action, int cartridges, DateTime time, int colour)
        {
            if (_logEnabled != true)
            {
                return;
            }
            var position = Game.Player.Character.Position;
            uint streetName = 0;
            uint crossingRoad = 0;
            GetStreetNameAtCoord(position.X, position.Y, position.Z, ref streetName, ref crossingRoad);
            var eventId = Convert.ToString(new Random().Next(1020263, 8029263));
            
            _logger.Debug($"action: {action}, cartridges: {cartridges}, time: {time}, colour: {colour}, position: {position}, streetName: {streetName}, crossingRoad: {crossingRoad}");
        }

        private static void OverrideLog(string action, int cartridges, DateTime time, int colour)
        {
            var position = Game.Player.Character.Position;
            uint streetName = 0;
            uint crossingRoad = 0;
            GetStreetNameAtCoord(position.X, position.Y, position.Z, ref streetName, ref crossingRoad);
        }

        private bool TaserSafety(int cartridges, bool taserSafetyStatus)
        {
            if (taserSafetyStatus)
            {
                taserSafetyStatus = false;
                UpdateLog("Safety Deactivated", cartridges, DateTime.Now, 15158332);
                _notification.Success(_notificationTitle, $"Safety: <b class='text-danger'>DEACTIVATED</b>");

            }
            else
            {
                taserSafetyStatus = true;
                UpdateLog("Safety Activated", cartridges, DateTime.Now, 3257928);
                _notification.Success(_notificationTitle, $"Safety: <b class='text-success'>ACTIVATED</b>");

            }
            
            return taserSafetyStatus;

        }

        private static string GetTaserInformation()
        {
            return SerialNumber;
        }

        private void ReactivateSuspect(int loadedCartridges, int suspectPlayerId, int cartridgenumber)
        {
            _comms.ToServer(ServerEvents.TaserReactivateSuspect, suspectPlayerId);
            UpdateLog("Reactivation - Cartridge " + cartridgenumber, loadedCartridges, DateTime.Now, 16734208);
            _comms.ToServer(ServerEvents.SoundToRadius, Game.Player.Character.NetworkId, 30.0f, "reactivate.ogg", 0.95f);
        }

        private void CheckPedDriveStun()
        {
            if (!_detectedAiming || ((DateTime.Now - _driveStunTime).TotalMilliseconds < (double) 5000))
            {
                return;
            }
            
            DriveStun(_loadedCartridges, _transportCartridges);
            _driveStunTime = DateTime.Now;
            
            var entity = 0;

            if (!GetEntityPlayerIsFreeAimingAt(PlayerId(), ref entity)
                || !CanDriveStunEntity(entity))
            {
                return;
            }
            
            var carrierLocation = Game.Player.Character.Position;
            var entityLocation = GetEntityCoords(entity, true);

            if (!(GetDistanceBetweenCoords(carrierLocation.X, carrierLocation.Y, carrierLocation.Z,
                entityLocation.X, entityLocation.Y, entityLocation.Z, true) < 2))
            {
                return;
            }
            
            SetPedToRagdoll(entity, 5000, 5000, 0, true, true, false);
        }

        private bool CanDriveStunEntity(int entity)
        {
            return DoesEntityExist(entity) && IsEntityAPed(entity) && !IsEntityDead(entity);
        }
    }
}
