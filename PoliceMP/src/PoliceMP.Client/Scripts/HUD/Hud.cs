using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Actions.AskDriverToStepOut;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Properties;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
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
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Timers;

namespace PoliceMP.Client.Scripts.HUD
{
    public class Hud : Script
    {
        private readonly ITickManager _ticks;
        private readonly ILogger<Hud> _logger;
        private readonly IPermissionService _useraces;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ILegacyClientCommunicationsManager _comms;
        private bool _seatbeltState = false;
        private bool _isControlActive;
        private bool _isControlHudVisible;
        private bool _isCROActive;
        private bool _isCROHudVisible;
        private DateTime _lastControlStatus;
        private DateTime _lastCROStatus;
        private float _vehicleSpeed = 0f;
        private Vector3 _vehicleVelocity = Vector3.Zero;
        private string _callSign = "";
        private int _lastVehicle = 0;
        private string direction;

        #region HeliHUD Variables
        private bool forceOff = true;
        private float speed = 0f;
        private string eng1Text = "~r~ENG 1";
        private string eng2Text = "~r~ENG 2";
        private string apuText = "~r~APU";
        private string speedText = "~r~SPEED";
        private string tempsText = "~r~TEMPS";
        private string brakeText = "~g~BRAKE";
        private int hiddenJet;
        private bool heliHudDisabled = false;
        private bool leftHeli = false;
        private bool eng1Failed = false;
        private bool blipNotCreatedYet = false;
        private bool warningCleared = false;
        private bool justFueled = false;
        private bool engineRunning = false;
        #endregion

        private float _speedLimiter = -1;

        #region Zones

        private readonly List<string> zoneNameShort = new()
        {
            "AIRP", "ALAMO", "ALTA", "ARMYB", "BANHAMC", "BANNING", "BEACH",
            "BHAMCA", "BRADP", "BRADT", "BURTON", "CALAFB", "CANNY", "CCREAK", "CHAMH", "CHIL", "CHU", "CMSW", "CYPRE",
            "DAVIS", "DELBE", "DELPE", "DELSOL", "DESRT", "DOWNT", "DTVINE", "EAST_V", "EBURO", "ELGORL", "ELYSIAN",
            "GALFISH", "GOLF", "GRAPES", "GREATC", "HARMO", "HAWICK", "HORS", "HUMLAB", "JAIL", "KOREAT", "LACT",
            "LAGO", "LDAM", "LEGSQU", "LMESA", "LOSPUER", "MIRR", "MORN", "MOVIE", "MTCHIL", "MTGORDO", "MTJOSE", "MURRI",
            "NCHU", "NOOSE", "OCEANA", "PALCOV", "PALETO", "PALFOR", "PALHIGH", "PALMPOW", "PBLUFF", "PBOX", "PROCOB",
            "RANCHO", "RGLEN", "RICHM", "ROCKF", "RTRAK", "SANAND", "SANCHIA", "SANDY", "SKID", "SLAB", "STAD", "STRAW",
            "TATAMO", "TERMINA", "TEXTI", "TONGVAH", "TONGVAV", "VCANA", "VESP", "VINE", "WINDF", "WVINE", "ZANCUDO",
            "ZP_ORT", "ZQ_UAR", "ROXSHELL", "ROXBYADR", "ROXVOYA", "ROXBYABR", "ROXWOOD", "ROXMARB", "ROXDOCK",
            "ROXSQUID", "ROXCLAM", "ROXORTEG", "ROXPARTO", "ROXRIVAV", "ROXDOLPH", "ROXSUNLN", "ROXPEARL", "ROXCONNY",
            "ROXSHELL", "ROXBYADR", "ROXVOYA", "ROXBYABR", "ROXWOOD", "ROXMARB", "ROXDOCK"
        };

        private readonly List<string> zoneListComp = new()
        {
            "Los Santos International Airport", "Alamo Sea", "Alta", "Fort Zancudo", "Banham Canyon Dr", "Banning",
            "Vespucci Beach", "Banham Canyon", "Braddock Pass", "Braddock Tunnel", "Burton", "Calafia Bridge",
            "Raton Canyon", "Cassidy Creek", "Chamberlain Hills", "Vinewood Hills", "Chumash",
            "Chiliad Mountain State Wilderness", "Cypress Flats", "Davis", "Del Perro Beach", "Del Perro", "La Puerta",
            "Grand Senora Desert", "Downtown", "Downtown Vinewood", "East Vinewood", "El Burro Heights",
            "El Gordo Lighthouse", "Elysian Island", "Galilee", "GWC and Golfing Society", "Grapeseed",
            "Great Chaparral", "Harmony", "Hawick", "Vinewood Racetrack", "Humane Labs and Research",
            "Bolingbroke Penitentiary", "Little Seoul", "Land Act Reservoir", "Lago Zancudo", "Land Act Dam",
            "Legion Square", "La Mesa", "La Puerta", "Mirror Park", "Morningwood", "Richards Majestic", "Mount Chiliad",
            "Mount Gordo", "Mount Josiah", "Murrieta Heights", "North Chumash", "N.O.O.S.E", "Pacific Ocean",
            "Paleto Cove", "Paleto Bay", "Paleto Forest", "Palomino Highlands", "Palmer - Taylor Power Station",
            "Pacific Bluffs", "Pillbox Hill", "Procopio Beach", "Rancho", "Richman Glen", "Richman", "Rockford Hills",
            "Redwood Lights Track", "San Andreas", "San Chianski Mountain Range", "Sandy Shores", "Mission Row",
            "Stab City", "Maze Bank Arena", "Strawberry", "Tataviam Mountains", "Terminal", "Textile City",
            "Tongva Hills", "Tongva Valley", "Vespucci Canals", "Vespucci", "Vinewood", "Ron Alternates Wind Farm",
            "West Vinewood", "Zancudo River", "Port of South Los Santos", "Davis Quartz", "Roxwood"
        };


        #endregion

        public Hud(ITickManager tick, ICommandManager command, IPermissionService useraces, ILogger<Hud> logger, INewNotificationOverlay newNotificationOverlay, ILegacyClientCommunicationsManager comms)
        {
            _logger = logger;
            _ticks = tick;
            _newNotificationOverlay = newNotificationOverlay;
            _comms = comms;
            _useraces = useraces;

            _comms.On(ClientEvents.ControlStatus, async (bool isActive) =>
            {
                _isControlActive = isActive;
                _isControlHudVisible = true;
                _lastControlStatus = DateTime.Now;

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Control Status", "info", "The Control Room has updated their status! See text chat for the latest channel status update.", new NewNotificationMessageContent[0]));

                const int disappearAfterSeconds = 60 * 2; // 2 minutes
                await BaseScript.Delay(disappearAfterSeconds * 1000);
                if (_isControlHudVisible && _lastControlStatus.AddSeconds(disappearAfterSeconds) <= DateTime.Now && !_isControlActive)
                {
                    _isControlHudVisible = false;
                }
            });

            _comms.On(ClientEvents.CROStatus, async (bool isActive) =>
            {
                _isCROActive = isActive;
                _isCROHudVisible = true;
                _lastCROStatus = DateTime.Now;

                const int disappearAfterSeconds = 60 * 2; // 2 minutes
                await BaseScript.Delay(disappearAfterSeconds * 1000);
                if (_isCROHudVisible && _lastCROStatus.AddSeconds(disappearAfterSeconds) <= DateTime.Now && !_isCROActive)
                {
                    _isCROHudVisible = false;
                }
            });

            command.Register("sb").WithHandler(ToggleSeatBelt);
            command.Register("speedlimiter").WithHandler(ToggleSpeedLimiter);

            command.Register("setspeed").HasGreedyArgs().WithHandler(async stringSpeed =>
            {
                var currentVehicle = Game.PlayerPed.CurrentVehicle;

                if (currentVehicle == null)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Speed Limiter", "error", "You must be in a car to use this feature!", new NewNotificationMessageContent[0]));
                    return;
                }

                if (currentVehicle.Driver == null)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Speed Limiter", "error", "You must be the driver to use this feature!", new NewNotificationMessageContent[0]));
                    return;
                }

                if (currentVehicle.Driver != Game.PlayerPed) return;
                var vehClass = currentVehicle.ClassType;

                if (vehClass is VehicleClass.Boats or VehicleClass.Planes or VehicleClass.Helicopters) return;

                if (stringSpeed == "0")
                {
                    _speedLimiter = -1;

                    API.SetVehicleMaxSpeed(currentVehicle.Handle, 0.0f);

                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Speed Limiter", "success", "Speed Limiter Disabled", new NewNotificationMessageContent[0]));
                    return;
                }

                if (!Int32.TryParse(stringSpeed, out int intSpeed))
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Speed Limiter", "error", "You must input a speed as a parameter. e.g. /setspeed 20", new NewNotificationMessageContent[0]));
                    return;
                }

                _speedLimiter = intSpeed / 2.236936f;

                if (_speedLimiter < currentVehicle.Speed)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Speed Limiter", "error", "You cannot set a lower speed limit whilst driving.", new NewNotificationMessageContent[0]));
                    _speedLimiter = -1;
                    API.SetVehicleMaxSpeed(currentVehicle.Handle, 0.0f);
                    return;
                }

                int speedRounded = (int)Math.Round(_speedLimiter, 0);
                API.SetVehicleMaxSpeed(currentVehicle.Handle, speedRounded);

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Speed Limiter", "success", $"Speed Limiter set to {intSpeed} MPH", new NewNotificationMessageContent[0]));

            });
        }

        protected override async Task OnStartAsync()
        {
            _ticks.On(LocationTick);
            _ticks.On(VehicleTick);
            _ticks.On(SpeedLimiterTick);
            _ticks.On(FetchCallsign);
            _ticks.On(ShowCallsign);
            _ticks.On(ShowControlStatus);
            _ticks.On(DirectionDecider);
            _ticks.On(DirectionIndicatorText);
            _ticks.On(NoClipSpeedIndicator);

            API.RegisterKeyMapping("sb", "Toggle Vehicle Seatbelt", "keyboard", "y");
            API.RegisterKeyMapping("speedlimiter", "Toggle Vehicle Speed Limiter", "keyboard", "F5");

            _isControlActive = await _comms.Request<bool>(ServerEvents.GetControlStatus);
            _isControlHudVisible = _isControlActive;

            _isCROActive = await _comms.Request<bool>(ServerEvents.GetCROStatus);
            _isCROHudVisible = _isCROActive;
        }

        private async Task FetchCallsign()
        {
            var callsign = (string)Game.Player.State.Get(PlayerStates.CallSign);

            _callSign = callsign;
            await Script.Delay(5000);
            return;
        }

        private async Task ShowCallsign()
        {
            if (!Screen.Hud.IsRadarVisible) return;
            if (string.IsNullOrEmpty(_callSign)) return;

            var posX = 0.17f;
            var posY = 0.96f;
            var scale = 0.5f;
            var font = 4;

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName($"Running As: ~b~{_callSign}");
            API.EndTextCommandDisplayText(posX, posY);
        }

        private async Task ShowControlStatus()
        {
            if (!Screen.Hud.IsRadarVisible) return;
            if (!_isControlHudVisible && !_isCROHudVisible) return;

            var isCiv = _useraces.CurrentUserRole.Branch == UserBranch.Civ;

            if (isCiv && !_isCROHudVisible) return;
            if (!isCiv && !_isControlHudVisible) return;

            var posX = 0.015f;
            var posY = 0.725f;
            var scale = 0.42f;
            var font = 4;

            const string onlineText = "~g~Online";
            const string offlineText = "~r~Offline";

            var preText = isCiv && _isCROActive ? "CRO" : "Control";
            var controlStatus = _isControlActive ? onlineText : offlineText;

            // if civ: show CRO status, if control: show control status
            var text = isCiv ? (_isCROActive ? onlineText : controlStatus) : controlStatus;

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName($"{preText} Status: {text}");
            API.EndTextCommandDisplayText(posX, posY);
        }

        private async Task DirectionDecider()
        {
            if (!Screen.Hud.IsRadarVisible) return;

            var player = Game.PlayerPed.Handle;
            var playerHeading = API.GetEntityHeading(player);

            #region DirectionChooser

            if (playerHeading > 270 && playerHeading < 330)
            {
                direction = "NE";
            }
            if (playerHeading > 30 && playerHeading < 90)
            {
                direction = "NW";
            }
            if (playerHeading > 210 && playerHeading < 270)
            {
                direction = "SE";
            }
            if (playerHeading > 90 && playerHeading < 150)
            {
                direction = "SW";
            }

            if (playerHeading < 30)
            {
                direction = " N";
            }
            if (playerHeading > 330)
            {
                direction = " N";
            }

            if (playerHeading > 90 && playerHeading < 120)
            {
                direction = " W";
            }
            if (playerHeading > 150 && playerHeading < 210)
            {
                direction = " S";
            }
            if (playerHeading > 240 && playerHeading < 300)
            {
                direction = " E";
            }

            await Delay(1250);

            #endregion DirectionChooser

        }

        private async Task NoClipSpeedIndicator()
        {
            if (!Screen.Hud.IsRadarVisible) return;
            if (!AdminMenu.NoClipActive) return;

            var posX = 0.015f;
            var posY = 0.675f + OffsetY();
            var scale = 0.42f;
            var font = 4;

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName($"NoClip Speed: {AdminMenu.NoClipPublicSpeed}");
            API.EndTextCommandDisplayText(posX, posY);
        }

        private async Task DirectionIndicatorText()
        {
            if (!Screen.Hud.IsRadarVisible) return;

            var posX = 0.1372f;

            float ratio = API.GetScreenAspectRatio(true);
            if (ratio >= 2.3)
            {
                // Ultrawide display
                posX = 0.11f;
            }

            var posY = 0.765f;
            var scale = 0.675f;
            var font = 4;

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName(direction);
            API.EndTextCommandDisplayText(posX, posY);
        }

        private async Task LocationTick()
        {
            if (!Screen.Hud.IsRadarVisible) return;

            var playerPos = Game.PlayerPed.Position;

            uint streetNameHash = 0;
            uint intersectionHash = 0;

            string streetName = "*";
            string zoneName = "Somewhere outside the M25";

            API.GetStreetNameAtCoord(playerPos.X, playerPos.Y, playerPos.Z, ref streetNameHash, ref intersectionHash);
            if(streetNameHash != 0)
            {
                streetName = API.GetStreetNameFromHashKey(streetNameHash);
            }

            var zoneShort = API.GetNameOfZone(playerPos.X, playerPos.Y, playerPos.Z);

            if(zoneShort != "ROXMARB")
            {
                var zoneIndex = zoneNameShort.IndexOf(zoneShort);
                if (zoneIndex != -1)
                {
                    zoneName = zoneListComp[zoneIndex];
                }
            }          

            var streetZoneText = $"{streetName} - {zoneName}";

            var posX = 0.015f;
            var posY = 0.750f;
            var scale = 0.42f;
            var font = 4;

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName(streetZoneText);
            API.EndTextCommandDisplayText(posX, posY);
            Screen.Hud.HideComponentThisFrame(HudComponent.StreetName);
            Screen.Hud.HideComponentThisFrame(HudComponent.AreaName);
            Screen.Hud.HideComponentThisFrame(HudComponent.VehicleName);
        }

        private float OffsetY()
        {
            return _isControlHudVisible || _isCROHudVisible ? 0.0f : 0.025f;
        }

        private async Task VehicleTick()
        {
            if (!Screen.Hud.IsRadarVisible) return;

            var currentVehicle = Game.PlayerPed.CurrentVehicle;

            if (currentVehicle == null) return;

            var isDriver = currentVehicle.Driver != null && currentVehicle.Driver == Game.PlayerPed;

            #region Speed

            var speedPosX = 0.015f;
            var speedPosY = 0.700f + OffsetY();
            var scale = 0.42f;
            var font = 4;

            var vehicleSpeed = currentVehicle.Speed * 2.236936;
            var speedRounded = Math.Round(vehicleSpeed, 0);
            var speedString = $"{speedRounded} ~f~MPH";

            var showSpeedLimiter = true;

            if (currentVehicle.ClassType is VehicleClass.Planes or VehicleClass.Helicopters)
            {
                var knotSpeed = currentVehicle.Speed * 1.94384;
                var knotRounded = Math.Round(knotSpeed, 0);
                var currentHeight = currentVehicle.HeightAboveGround;
                var currentFeet = currentHeight * 3.28084;
                var feetRounded = Math.Round(currentFeet, 0) - 3;
                speedString = $"{knotRounded} ~f~KTS~w~ - {feetRounded} ~g~FT";
                showSpeedLimiter = false;
            }

            API.SetTextScale(scale, scale);
            API.SetTextFont(font);
            API.SetTextOutline();
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName(speedString);
            API.EndTextCommandDisplayText(speedPosX, speedPosY);

            #endregion

            #region Speed Limit

            var speedLimitPosX = 0.015f;
            var speedLimitPosY = 0.625f + OffsetY();

            var speedLimitText = "Speed Limiter: ~r~OFF";
            if (_speedLimiter > -1)
            {
                var vehicleSpeedLimiter = _speedLimiter * 2.236936;
                var speedLimiterRounded = Math.Round(vehicleSpeedLimiter, 0);
                speedLimitText = $"Speed Limiter: ~o~{speedLimiterRounded} MPH";
            }

            #endregion

            if (isDriver)
            {
                #region Speed Limiter
                if (showSpeedLimiter)
                {

                    API.SetTextScale(scale, scale);
                    API.SetTextFont(font);
                    API.SetTextOutline();
                    API.BeginTextCommandDisplayText("STRING");
                    API.AddTextComponentSubstringPlayerName(speedLimitText);
                    API.EndTextCommandDisplayText(speedLimitPosX, speedLimitPosY);

                }
                #endregion

                #region Fuel

                var player = Game.PlayerPed.Handle;
                if (API.IsPedInAnyHeli(player)) return;

                var fuel = Math.Round(currentVehicle.FuelLevel, 0);

                var fuelPosX = 0.015f;
                var fuelPosY = 0.660f + OffsetY();
                var fuelScale = 0.2f;

                var fuelString = $"🟩 🟩 🟩 🟩";

                if (fuel < 75)
                {
                    fuelString = $"🟩 🟩 🟩";
                }

                if (fuel < 50)
                {
                    fuelString = $"🟧 🟧";
                }

                if (fuel < 25)
                {
                    fuelString = $"🟧";
                }

                if (fuel < 15)
                {
                    fuelString = $"🟥";
                }

                if (fuel < 5)
                {
                    fuelString = $"🆘";
                }

                API.SetTextScale(fuelScale, fuelScale);
                API.SetTextFont(font);
                API.SetTextOutline();
                API.BeginTextCommandDisplayText("STRING");
                API.AddTextComponentSubstringPlayerName(fuelString);
                API.EndTextCommandDisplayText(fuelPosX, fuelPosY);

                #endregion
            }
        }

        private double[] Fwv(Ped player)
        {
            var hr = API.GetEntityHeading(player.Handle) + 90f;
            if (hr < 0)
            {
                var oldHr = hr;
                hr = 360f + oldHr;
            }

            hr = (float)(hr * 0.0174533);

            var x = Math.Cos(hr) * 2;
            var y = Math.Sin(hr) * 2;

            return new double[] { x, y };
        }

        private void ToggleSeatBelt()
        {
            _seatbeltState = !_seatbeltState;

        }

        private void ToggleSpeedLimiter()
        {
            var currentVehicle = Game.PlayerPed.CurrentVehicle;

            if (currentVehicle == null) return;
            if (currentVehicle.Driver == null) return;
            if (currentVehicle.Driver != Game.PlayerPed) return;
            var vehClass = currentVehicle.ClassType;

            if (vehClass is VehicleClass.Boats or VehicleClass.Planes or VehicleClass.Helicopters) return;

            if (_speedLimiter > -1)
            {
                _speedLimiter = -1;

                API.SetVehicleMaxSpeed(currentVehicle.Handle, 0.0f);

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Speed Limiter", "success", "Speed Limiter Disabled", new NewNotificationMessageContent[0]));
                return;
            }

            _speedLimiter = currentVehicle.Speed;

            var vehicleSpeed = currentVehicle.Speed * 2.236936;
            var speedRounded = Math.Round(vehicleSpeed, 0);
            API.SetVehicleMaxSpeed(currentVehicle.Handle, _speedLimiter);
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Speed Limiter", "success", $"Speed Limiter set to {speedRounded} MPH", new NewNotificationMessageContent[0]));

        }

        private async Task SpeedLimiterTick()
        {
            if (_speedLimiter == -1f) return;

            var currentVehicle = Game.PlayerPed.CurrentVehicle;

            if (currentVehicle == null)
            {
                if (_speedLimiter != -1f)
                {
                    _speedLimiter = -1f;
                }
                if (_lastVehicle != 0)
                {
                    if (!API.DoesEntityExist(_lastVehicle))
                    {
                        _lastVehicle = 0;
                    }
                    else
                    {
                        API.SetVehicleMaxSpeed(_lastVehicle, 0.0f);
                    }
                }
                return;
            }

            if (_lastVehicle != currentVehicle.Handle)
            {
                _lastVehicle = currentVehicle.Handle;
            }

            /*
            if (currentVehicle.Driver == null) return;
            if (currentVehicle.Driver != Game.PlayerPed) return;

            if (currentVehicle.Speed > _speedLimiter)
            {
                currentVehicle.Speed = _speedLimiter;
            }*/
        }
    }
}