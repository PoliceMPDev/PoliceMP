using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.Admin
{
    public class AdminChecks : Script
    {
        private readonly ITickManager _tick;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly IClientEventManager _events;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<AdminChecks> _logger;
        private readonly IFiveEventManager _fiveEvent;
        private readonly INotificationService _notifications;
        private readonly ICommandManager _command;
        private readonly IFeatureService _featureService;
        private readonly INewNotificationOverlay _newNotificationOverlay;

        private UserAces _userAces;
        private readonly bool _hasSpawned = false;

        private string[] _bannedCommands =
        {
            "menuv",
            "vmenu",
            "heal"
        };

        private readonly List<string> _bikeSpawnNames = new List<string>
        {
            "offbike",
            "offbike1",
            "polbumk",
            "2020exc"
        };

        private bool _wasOnBike = false;
        private bool _wasInVehicle = false;

        #region Blacklisted

        private readonly List<string> _blackListedWeapons = new List<string>
        {
            "WEAPON_RAILGUN",
            "WEAPON_GARBAGEBAG",
            "WEAPON_GRENADELAUNCHER",
            "WEAPON_RPG",
            "WEAPON_GRENADE",
            "WEAPON_HOMINGLAUNCHER",
            "WEAPON_COMPACTLAUNCHER",
            "WEAPON_STICKYBOMB",
            "WEAPON_PIPEBOMB",
            "WEAPON_MINIGUN",
            "WEAPON_RAYPISTOL",
            "WEAPON_RAYCARBINE",
            "WEAPON_RAYMINIGUN",
        };

        #endregion Blacklisted

        #region Last Time Checks

        private DateTime _lastSkinCheck;
        private DateTime _lastHealthCheck;

        #endregion Last Time Checks

        public AdminChecks(ITickManager tick, ILegacyClientCommunicationsManager comms, IFeatureService featureService,
            IClientEventManager events, IPermissionService permissionService, ILogger<AdminChecks> logger,
            IFiveEventManager fiveEvent, INotificationService notifications, ICommandManager command, INewNotificationOverlay newNotificationOverlay)
        {
            _tick = tick;
            _comms = comms;
            _events = events;
            _permissionService = permissionService;
            _logger = logger;
            _fiveEvent = fiveEvent;
            _notifications = notifications;
            _featureService = featureService;
            _command = command;
            _newNotificationOverlay = newNotificationOverlay;
        }

        protected override async Task OnStartAsync()
        {
            _userAces = await _permissionService.GetUserAces();

            _comms.On<string>(ClientEvents.ReceiveModMessage, OnReceiveMessageForMod);
            //_comms.On<bool>(ClientEvents.PlayerSpawned);

            _fiveEvent.On("playerSpawned", NativePlayerSpawned);

            SetPlayerToPoliceGroup();


            foreach (var command in _bannedCommands)
            {
                _command.Register(command).WithHandler(BannedCommandEntered);
            }

            _command.Register("autobodycam").WithHandler(() =>
            {
                var role = _permissionService.CurrentUserRole;
                if (role.Division != UserDivision.Afo && !_userAces.IsAdmin && !_userAces.IsDeveloper)
                {
                    return;
                }
                ToggleAutoBodycam();
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Bodycam", "info", $"Auto bodycam is now {(_autoBodycamEnabled ? "enabled" : "disabled")}"));
            });
            
            _tick.On(CheckWeaponHacks);
            _tick.On(CheckPlayerSkin);
            _tick.On(CheckForProfiler);
            _tick.On(CheckPlayerHealth);
            _tick.On(PcsoDisallowAircraft);
            _tick.On(PcsoDisallowGround);
            //_tick.On(SpeedLimit);
            _tick.On(StaminaRegen);
            _tick.On(NoDriveByWithBikeChecks);
            _tick.On(CheckWeaponsDischarge);
        }

        private void BannedCommandEntered()
        {
            _notifications.Warning("BANNED COMMAND",
                "You have entered a banned command, mods will be notified of continued misuse.");
        }

        private void SendMessageToMods(string message)
        {
            _comms.ToServer(ServerEvents.SendMessageToMods, message);
        }

        private void OnReceiveMessageForMod(string message)
        {
            if (!_userAces.IsModerator) return;
            _events.EmitRaw("chat:addMessage", $"^8 ^* [ADMIN SYSTEM] {message}");
        }

        private async Task CheckWeaponHacks()
        {
            if (_userAces.IsAdmin) return;

            if (DateTime.Compare(DateTime.Now, _lastSkinCheck.AddSeconds(30)) <= 0) return;

            foreach (var blackListedWeapon in _blackListedWeapons)
            {
                int weaponHash = API.GetHashKey(blackListedWeapon);

                if (API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)weaponHash, false))
                {
                    SendMessageToMods(
                        $"Banned Weapon Detected for {Game.Player.Name} ({blackListedWeapon}). Weapon has been removed.");
                    API.RemoveWeaponFromPed(Game.PlayerPed.Handle, (uint)weaponHash);
                    break;
                }
            }

            await Delay(0);
        }

        private async Task CheckPlayerSkin()
        {
            if (!_hasSpawned) return;

            if (_userAces.IsModerator || _userAces.IsCivTrained) return;

            if (Game.PlayerPed.IsDead) return;

            if (DateTime.Compare(DateTime.Now, _lastSkinCheck.AddMinutes(1)) <= 0) return;

            Player player = Game.Player;

            if (player == null || player.Character == null) return;

            if (player.Character.Model.Hash == API.GetHashKey("mp_m_freemode_01") ||
                player.Character.Model.Hash == API.GetHashKey("mp_f_freemode_01")) return;

            SendMessageToMods($"Non-MP Skin Detected for {player.Name}.");

            _lastSkinCheck = DateTime.Now;
            await Delay(0);
        }

        private async Task CheckForProfiler()
        {
            if (!API.ProfilerIsRecording()) return;

            SendMessageToMods($"Profiler has been activated by {Game.Player.Name}.");
            await Delay(0);
        }

        private async Task NativePlayerSpawned()
        {
            SendMessageToMods($"{Game.Player.Name} native spawned (usually got a mod menu)");
            await Delay(0);
        }

        private async Task CheckPlayerHealth()
        {
            if (!_hasSpawned) return;
            if (!_userAces.IsModerator) return;

            if (DateTime.Compare(DateTime.Now, _lastHealthCheck.AddMinutes(1)) <= 0) return;

            Random rnd = new Random();

            Player player = Game.Player;

            if (Game.PlayerPed.IsDead) return;

            int currentHealth = player.Character.Health;
            if (currentHealth == 0) return;

            int waitTime = rnd.Next(10, 150);

            player.Character.Health -= 10;

            await Delay(waitTime);

            if (player.Character.Health > 100)
            {
                SendMessageToMods($"Possible Health Hack for {player.Name}.");
            }
            else if (player.Character.Health == currentHealth)
            {
                SendMessageToMods($"Possible Health Hack for {player.Name}.");
            }

            player.Character.Health = currentHealth;
            _lastHealthCheck = DateTime.Now;
            await Delay(0);
        }


        #region Ported Legacy Features - PCSO disallow vehicele, Set AI to friendly

        private DateTime _lastNotified = DateTime.Now;


        private readonly string[] _allowedPlaneModels =
            { "pasplane" };

        public async Task PcsoDisallowAircraft()
        {
            var player = Game.PlayerPed;
            if (!API.IsPedInAnyVehicle(player.Handle, true)) return; //If no vehicle, no tick
            if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo || _userAces.IsDevFunNight) return;

            var isDevServer = _featureService.IsFeatureEnabled(FeatureToggle.IsDevelopment);
            if (isDevServer && _userAces.IsQaTeam) return;

            var currentVehicle = Game.PlayerPed.CurrentVehicle;
            if (currentVehicle == null) return; //If no vehicle, no tick

            switch (currentVehicle.ClassType)
            {
                case VehicleClass.Planes:
                    foreach (var plane in _allowedPlaneModels)
                    {
                        if (Game.PlayerPed.CurrentVehicle.Model.Hash == API.GetHashKey(plane))
                        {
                            if (_userAces.IsNpasTrained || _userAces.IsCommsTrained) return;
                        }
                        else
                        {
                            player.Task.LeaveVehicle();

                            TimeSpan timeSinceNotified = DateTime.Now - _lastNotified;
                            if (timeSinceNotified.TotalSeconds >= 6)
                            {
                                _lastNotified = DateTime.Now;
                                _notifications.Error("Error", "You cannot fly this. Go play Flight Simulator");
                                SendMessageToMods($"INFO {Game.Player.Name} has tried to enter a Plane.");
                            }

                            await Delay(2000);
                        }
                    }

                    return;
                case VehicleClass.Helicopters:
                    if (_userAces.IsNpasTrained || _userAces.IsCommsTrained) return; //Npas and comms can enter anything

                    if (currentVehicle.Driver == player)
                    {
                        player.Task.LeaveVehicle();

                        TimeSpan timeSinceNotified = DateTime.Now - _lastNotified;
                        if (timeSinceNotified.TotalSeconds >= 6)
                        {
                            _lastNotified = DateTime.Now;
                            _notifications.Error("Error", "You need to be NPAS to fly this vehicle.");
                            SendMessageToMods($"INFO {Game.Player.Name} has tried to enter a Helicopter.");
                        }

                        await Delay(2000);
                        return;
                    }

                    //If there is a driver in the vehicle anyone can enter passanger
                    if (currentVehicle.Driver != null) return;

                    player.Task.LeaveVehicle();
                    _notifications.Error("Error", "Please allow a pilot to enter first.");
                    await Delay(2000);
                    return;
            }

            await Delay(0);
        }

        private readonly string[] _disallowedVehicles =
            { "police", "police2", "police3", "police4", "policeb", "riot", "rhino", "hydra", "lazer", "apc" };

        #region NoDriveBy
        private async Task NoDriveByWithBikeChecks()
        {
            var playerPed = Game.PlayerPed;
            var playerId = Game.Player.Handle;
            var vehicle = playerPed.CurrentVehicle;
            var isInVehicle = API.IsPedInAnyVehicle(playerPed.Handle, false);

            if (isInVehicle)
            {
                bool isDriver = vehicle.Driver == playerPed;
                bool isBike = IsVehicleBike(vehicle);
                float speed = API.GetEntitySpeed(vehicle.Handle) * 2.23694f;

                if (!_wasInVehicle)
                {
                    API.SetCurrentPedWeapon(playerPed.Handle, (uint)API.GetHashKey("WEAPON_UNARMED"), true);
                    _wasInVehicle = true;
                }

                if (_userAces.IsDeveloper || _userAces.IsAdmin)
                {
                    API.SetPlayerCanDoDriveBy(playerId, true);
                }
                // RPU officers on a bike with speed restriction
                else if (isDriver && isBike && _permissionService.CurrentUserRole.Division == UserDivision.Rpu)
                {
                    API.SetPlayerCanDoDriveBy(playerId, speed < 30.0f);

                    if (!_wasOnBike && _permissionService.CurrentUserRole.Division == UserDivision.Rpu)
                    {
                        API.RemoveAllPedWeapons(playerPed.Handle, true);
                        _wasOnBike = true;
                    }
                }
                else
                {
                    API.SetPlayerCanDoDriveBy(playerId, false);
                }
            }
            else
            {
                API.SetPlayerCanDoDriveBy(playerId, false);

                // Restore fixed weapons when RPU officer gets off the bike
                if (_wasOnBike && _permissionService.CurrentUserRole.Division == UserDivision.Rpu)
                {
                    API.GiveWeaponToPed(playerPed.Handle, (uint)API.GetHashKey("weapon_stungun"), 1, false, false);
                    API.GiveWeaponToPed(playerPed.Handle, (uint)API.GetHashKey("weapon_nightstick"), 1, false, false);
                    API.GiveWeaponToPed(playerPed.Handle, (uint)API.GetHashKey("weapon_speedcuffs"), 1, false, false);
                    API.GiveWeaponToPed(playerPed.Handle, (uint)API.GetHashKey("weapon_pepperspray"), 1, false, false);
                    _wasOnBike = false;
                }

                _wasInVehicle = false;
            }

            await Delay(500);
        }
        #endregion 
        
        #region Bodycam Trigger
        
        private bool _autoBodycamEnabled = false;
        private bool _weaponFiredRecently = false;
        private DateTime _lastWeaponCheck;

        public void ToggleAutoBodycam()
        {
            _autoBodycamEnabled = !_autoBodycamEnabled;
            Debug.WriteLine($"[Bodycam] Auto recording is now {(_autoBodycamEnabled ? "ENABLED" : "DISABLED")}");
        }

        private async Task CheckWeaponsDischarge()
        {
            while (true)
            {
                await Delay(30);

                if (!_autoBodycamEnabled) continue;
                if (_userAces == null) continue;

                var role = _permissionService.CurrentUserRole;
                if (role == null || role.Division != UserDivision.Afo) continue;

                if (Game.PlayerPed.IsShooting && !_weaponFiredRecently)
                {
                    _weaponFiredRecently = true;
                    _lastWeaponCheck = DateTime.Now;
                    API.ExecuteCommand("+zbodycam");
                    _ = Delay(61000).ContinueWith(_ => _weaponFiredRecently = false);
                }
            }
        }
        
        #endregion
        
        public async Task PcsoDisallowGround()
        {
            var Player = Game.PlayerPed;
            var currentVehicle = Game.PlayerPed.CurrentVehicle;
            if (currentVehicle == null) return;

            if (API.IsPedInAnyVehicle(Player.Handle, true))
            {
                if (_userAces.IsWhiteListed || _userAces.IsCivTrained || _userAces.IsFireTrained ||
                    _userAces.IsNhsParamedic || _userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsStudentPara || _userAces.IsDevFunNight) return;
                else
                {
                    if (currentVehicle.ClassType != VehicleClass.Emergency &&
                        currentVehicle.ClassType != VehicleClass.Cycles)
                    {
                        Player.Task.LeaveVehicle();

                        TimeSpan timeSinceNotified = DateTime.Now - _lastNotified;
                        if (timeSinceNotified.TotalSeconds >= 6)
                        {
                            _lastNotified = DateTime.Now;
                            _notifications.Error("Error",
                                "You cannot drive vehicles other than Emergency Vehicles. God likes a trier though");
                            SendMessageToMods($"INFO {Game.Player.Name} has tried to enter a non emergency vehicle.");
                        }

                        await Delay(0);
                        return;
                    }
                }

                if (_userAces.IsDeveloper || _userAces.IsAdmin || _userAces.IsTierTwo || _userAces.IsDevFunNight) return;
                else
                    foreach (var vehicle in _disallowedVehicles)
                    {
                        if (Game.PlayerPed.CurrentVehicle.Model.Hash == API.GetHashKey(vehicle))
                        {
                            Game.PlayerPed.Task.LeaveVehicle();
                            _notifications.Error("Error",
                                "You cannot drive this sorry! go to the garage icon on the map. These can be found at stations");

                            return;
                        }
                    }
            }
        }

        private static void SetRelationships(uint playerGroupHash, string groupName, Relationship pedToPlayer,
            Relationship playerToPed)
        {
            API.SetRelationshipBetweenGroups((int)pedToPlayer, (uint)API.GetHashKey(groupName), playerGroupHash);
            API.SetRelationshipBetweenGroups((int)playerToPed, playerGroupHash, (uint)API.GetHashKey(groupName));
        }

        private static void SetPlayerToPoliceGroup()
        {
            var hash = uint.MinValue;
            API.AddRelationshipGroup("bigpopo", ref hash);
            API.SetPedRelationshipGroupHash(Game.PlayerPed.Handle, hash);

            // Respect Groups

            SetRelationships(hash, "COP", Relationship.Respect, Relationship.Respect);
            SetRelationships(hash, "SECURITY_GUARD", Relationship.Respect, Relationship.Respect);
            SetRelationships(hash, "PRIVATE_SECURITY", Relationship.Respect, Relationship.Respect);
            SetRelationships(hash, "FIREMAN", Relationship.Respect, Relationship.Respect);
            SetRelationships(hash, "ARMY", Relationship.Respect, Relationship.Respect);
            SetRelationships(hash, "MEDIC", Relationship.Respect, Relationship.Respect);

            // Neutral Groups

            SetRelationships(hash, "CAT", Relationship.Neutral, Relationship.Neutral);
            SetRelationships(hash, "GUARD_DOG", Relationship.Neutral, Relationship.Neutral);


            // Dislike Groups

            SetRelationships(hash, "GANG_1", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "GANG_2", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "GANG_9", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "GANG_10", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "AMBIENT_GANG_LOST", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "AMBIENT_GANG_MEXICAN", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "AMBIENT_GANG_FAMILY", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "AMBIENT_GANG_BALLAS", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "AMBIENT_GANG_MARABUNTE", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "AMBIENT_GANG_CULT", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "AMBIENT_GANG_SALVA", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "AMBIENT_GANG_WEICHENG", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "AMBIENT_GANG_HILLBILLY", Relationship.Dislike, Relationship.Dislike);
            SetRelationships(hash, "DEALER", Relationship.Dislike, Relationship.Dislike);

            API.SetPlayerCanBeHassledByGangs(Game.PlayerPed.Handle, true);
            API.SetEntityCanBeDamagedByRelationshipGroup(Game.PlayerPed.Handle, false, 1);
        }

        private string GetVehicleDisplayName(Vehicle vehicle)
        {
            return API.GetDisplayNameFromVehicleModel((uint)vehicle.Model.Hash).ToLower();
        }

        private bool IsVehicleBike(Vehicle vehicle)
        {
            string displayName = GetVehicleDisplayName(vehicle);
            return _bikeSpawnNames.Contains(displayName);
        }

        private async Task SpeedLimit()
        {
            var Player = Game.PlayerPed;
            var currentVehicle = Game.PlayerPed.CurrentVehicle;
            if (currentVehicle == null) return;

            if (_userAces.IsWhiteListed || _userAces.IsCivTrained || _userAces.IsFireTrained ||
                _userAces.IsNhsParamedic || _userAces.IsDeveloper || _userAces.IsAdmin) return;
            else
            {
                API.SetVehicleMaxSpeed(currentVehicle.Handle, 40.23f);
                await Delay(0);
            }

            return;
        }

        private async Task StaminaRegen()
        {
            API.RestorePlayerStamina(Game.Player.Handle, 1f);
            await Delay(0);
        }

        #endregion Ported Legacy Features - PCSO disallow vehicele, Set AI to friendly
    }
}