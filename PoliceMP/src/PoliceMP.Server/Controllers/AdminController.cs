using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Server.Services;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ILogger<AdminController> _logger;
        private readonly PlayerList _players;
        private readonly bool localTest = false;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _perms;
        private readonly IBucketService _buckets;
        private readonly ISonoranService _sonoranService;

        public AdminController(ILegacyServerCommunicationsManager comms, ILogger<AdminController> logger, PlayerList players, INotificationService notificationService, IPermissionService perms, IBucketService buckets, ISonoranService sonoranService)
        {
            _comms = comms;
            _logger = logger;
            _players = players;
            _notificationService = notificationService;
            _perms = perms;
            _buckets = buckets;
            _sonoranService = sonoranService;

            _comms.OnRequest(ServerEvents.FetchHasModPerm, FetchPlayerModeratorPerms);
            _comms.On<string>(ServerEvents.SendMessageToMods, SendMessageToOnlineMods);
            _comms.On<int, int>(ServerEvents.SendBackupPointToPlayer, SendBackupPointToPlayer);

            _comms.On<int>(ServerEvents.AdminSummonPlayerToPlayer, OnAdminSummonPlayer);
            _comms.On<int>(ServerEvents.AdminTeleportPlayerToPlayer, OnAdminTeleportPlayer);
            _comms.On<int>(ServerEvents.AdminKillPlayer, OnAdminKillPlayer);
            _comms.On(ServerEvents.ClearAreaOfPlayer, (player) =>
            {
                Vector3 position = player.Character.Position;

                foreach (var replicateToPlayer in _players)
                {
                    if (null == replicateToPlayer?.Character?.Position)
                        continue;

                    var distance = Vector3.Distance(replicateToPlayer.Character.Position, player.Character.Position);
                    if (distance > 100f)
                    {
                        continue;
                    }

                    _comms.ToClient(replicateToPlayer, ClientEvents.AdminClearAreaAroundPosition, position);
                }                
            });
            _comms.On<int>(ServerEvents.AdminSendFreezeEventToServer, OnAdminFreezePlayer);
            _comms.On<int>(ServerEvents.AdminAttachPedToServer, OnAdminAttachPlayerToPlayer);

            _comms.On<RoutingBucket>(ServerEvents.AdminJoinBucket, AdminJoinBucket);
            _comms.OnRequest<string[]>(ServerEvents.AdminListBucket, AdminListBucket);
            _comms.On<string, RoutingBucket>(ServerEvents.AdminMovePlayerToBucket, AdminMovePlayerToBucket);
            _comms.On<string[], RoutingBucket>(ServerEvents.AdminMoveAllPlayersToBucket, AdminMovePlayersToBucket);
            _comms.OnRequest<string, RoutingBucket>(ServerEvents.AdminGetPlayerBucket, AdminGetPlayerBucket);
        }

        private async Task<bool> CheckCanRun(Player player, string commandUsed)
        {
            var aces = await _perms.GetUserAces(player);
            var allowed = aces.IsModerator || aces.IsAdmin || aces.IsDeveloper;

            if (!allowed)
            {
                SendMessageToOnlineMods($"\"{player.Name}\" tried to run an admin command without permission! Using command: {commandUsed}");
            }

            return allowed;
        }

        private async Task<string[]> AdminListBucket(Player player)
        {
            if (!await CheckCanRun(player, "AdminListBucket")) return Array.Empty<string>();

            return Enum.GetNames(typeof(RoutingBucket));
        }

        private async Task AdminJoinBucket(Player player, RoutingBucket bucket)
        {
            if (!await CheckCanRun(player, "AdminJoinBucket")) return;

            _buckets.MovePlayer(player, bucket);
        }

        private async Task OnAdminAttachPlayerToPlayer(Player player, int targetNetworkId)
        {
            if (!await CheckCanRun(player, "OnAdminAttachPlayerToPlayer")) return;
            var targetPed = (Ped)Entity.FromNetworkId(targetNetworkId);
            if (targetPed == null)
            {
                _notificationService.Error(player, "Attach", "Unable to attach the ped at this time.");
                return;
            }

            _comms.ToClient(targetPed.Owner, ClientEvents.AdminSendAttachEventToPlayer, player.Character.NetworkId);
        }

        private async Task OnAdminFreezePlayer(Player player, int targetNetworkId)
        {
            if (!await CheckCanRun(player, "OnAdminFreezePlayer")) return;
            var targetPed = (Ped)Entity.FromNetworkId(targetNetworkId);
            if (targetPed == null)
            {
                _notificationService.Error(player, "Freeze", "Unable to Freeze them.");
                return;
            }

            _comms.ToClient(targetPed.Owner, ClientEvents.AdminSendFreezeEventToPlayer);
        }

        public async Task OnAdminKillPlayer(Player player, int targetNetworkId)
        {
            if (!await CheckCanRun(player, "OnAdminKillPlayer")) return;
            var targetPed = (Ped)Entity.FromNetworkId(targetNetworkId);
            if (targetPed == null)
            {
                _notificationService.Error(player, "Kill", "Unable to kill them.");
                return;
            }

            _comms.ToClient(targetPed.Owner, ClientEvents.AdminSendKillEventToPlayer);
        }

        private async Task OnAdminTeleportPlayer(Player player, int targetNetworkId)
        {
            if (!await CheckCanRun(player, "OnAdminTeleportPlayer")) return;
            var targetPed = (Ped)Entity.FromNetworkId(targetNetworkId);

            if (targetPed == null || targetPed.Handle == 0)
            {
                _notificationService.Error(player, "Admin Teleport PLayer", "Unable to teleport player. Please try again later.");
                return;
            }

            var bucket = _buckets.GetEntityBucket(targetPed);
            _buckets.MovePlayer(player, bucket);

            var targetVehicleId = API.GetVehiclePedIsIn(targetPed.Handle, false);
            if (targetVehicleId == 0)
            {
                player.Character.Position = targetPed.Position + new Vector3(2f, 2, 0f);
                _notificationService.Success(player, "Teleport", $"You've teleported to the player");
                return;
            }

            var targetVehicle = (Vehicle)Entity.FromHandle(targetVehicleId);
            if (targetVehicle == null)
            {
                player.Character.Position = targetPed.Position + new Vector3(2f, 2, 0f);
                _notificationService.Success(player, "Teleport", $"You've teleported to the player");
                return;
            }

            player.Character.Position = targetVehicle.Position + new Vector3(5f, 5f, 0f);
            await Task.Delay(100);

            _comms.ToClient(player, ClientEvents.AdminTeleportIntoVehicle, targetVehicle.NetworkId);
        }

        private async Task OnAdminSummonPlayer(Player player, int targetNetworkId)
        {
            if (!await CheckCanRun(player, "OnAdminSummonPlayer")) return;
            var playerBucket = _buckets.GetPlayerBucket(player);
            var targetPed = (Ped)Entity.FromNetworkId(targetNetworkId);

            _buckets.MoveEntity(targetPed, playerBucket);
            var targetVehicleId = API.GetVehiclePedIsIn(targetPed.Handle, false);
            if (targetVehicleId == 0)
            {
                targetPed.Position = player.Character.Position + new Vector3(2f, 2, 0f);
                _notificationService.Success(player, "Summon", $"You've summoned the player");
                return;
            }

            var targetVehicle = (Vehicle)Entity.FromHandle(targetVehicleId);
            if (targetVehicle == null)
            {
                targetPed.Position = player.Character.Position + new Vector3(2f, 2, 0f);
                _notificationService.Success(player, "Summon", $"You've summoned the player");
                return;
            }

            targetVehicle.Position = player.Character.Position + new Vector3(5f, 5f, 0f);
        }

        private void SendMessageToOnlineMods(string message)
        {
            BaseScript.TriggerEvent("txaLogger:CommandExecuted", $"[ADMIN WARNING] {message} [ADMIN WARNING]");
            _logger.Error($"[ADMIN WARNING] {message} [ADMIN WARNING]");

            foreach (var player in _players.ToArray())
            {
                if(API.IsPlayerAceAllowed(player.Handle, "Police.adminAuth") || API.IsPlayerAceAllowed(player.Handle, "Police.developer") || API.IsPlayerAceAllowed(player.Handle, "Police.modAuth"))
                {
                    _comms.ToClient(player, ClientEvents.ReceiveModMessage, message);
                }
            }
        }

        private Task<bool> FetchPlayerModeratorPerms(Player player)
        {
            if (localTest)
            {
                return Task.FromResult(true);
            }

            bool hasPerm = API.IsPlayerAceAllowed(player.Handle, "Police.adminAuth");

            if (hasPerm) return Task.FromResult(true);

            hasPerm = API.IsPlayerAceAllowed(player.Handle, "Police.developer");

            if (hasPerm) return Task.FromResult(true);

            hasPerm = API.IsPlayerAceAllowed(player.Handle, "Police.modAuth");

            return Task.FromResult(hasPerm);
        }

        private void SendBackupPointToPlayer(int toNetworkId, int backupPlayerId)
        {
            var onlinePlayer = _players.FirstOrDefault(x => x.Character.NetworkId == toNetworkId);

            if (onlinePlayer == null) return;

            _comms.ToClient(onlinePlayer, ClientEvents.SendBackupLocationToClient, backupPlayerId);
        }

        private async Task AdminMovePlayerToBucket(Player player, string playerMoveHandle, RoutingBucket routingBucket)
        {
            if (!await CheckCanRun(player, "AdminMovePlayerToBucket")) return;
            _buckets.MovePlayer(playerMoveHandle, routingBucket);
        }

        private async Task AdminMovePlayersToBucket(Player player, string[] playerMoveHandle, RoutingBucket routingBucket)
        {
            if (!await CheckCanRun(player, "AdminMovePlayersToBucket")) return;
            _buckets.MovePlayers(playerMoveHandle, routingBucket);
        }

        private async Task<RoutingBucket> AdminGetPlayerBucket(Player player, string playerSrc)
        {
            if (!await CheckCanRun(player, "AdminGetPlayerBucket")) return (RoutingBucket)(-1);
            return _buckets.GetPlayerBucket(playerSrc);
        }
    }
}