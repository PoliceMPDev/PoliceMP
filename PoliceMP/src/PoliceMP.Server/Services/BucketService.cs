using System;
using System.Linq;
using System.Numerics;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Controllers;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.NetworkMessages.Buckets.Notifications;

namespace PoliceMP.Server.Services
{
    public class BucketService : IBucketService
    {
        private readonly ILogger<BucketService> _log;
        private readonly IPlayerListAccessor _playerListAccessor;
        private readonly ILegacyServerCommunicationsManager _legacyComms;
        private readonly IServerCommunicationsManager _comms;
        private readonly PlayerInfoController _pic;

        public BucketService(
            ILogger<BucketService> log, 
            IPlayerListAccessor playerListAccessor, 
            ILegacyServerCommunicationsManager legacyComms,
            IServerCommunicationsManager comms)
        {
            _log = log;
            _playerListAccessor = playerListAccessor;
            _legacyComms = legacyComms;
            _comms = comms;

            SetupBuckets();
        }

        private void SetupBuckets()
        {
            var buckets = Enum.GetValues(typeof(RoutingBucket)).Cast<RoutingBucket>();

            foreach (var bucket in buckets)
            {
                var attribute = bucket.GetCustomAttribute<RoutingBucketAttribute>();
                if (attribute != null)
                {
                    if (attribute.PopulationEnabled)
                    {
                        API.SetRoutingBucketPopulationEnabled((int)bucket, true);
                    }
                    else
                    {
                        API.SetRoutingBucketPopulationEnabled((int)bucket, false);
                    }
                }
            }
        }

        public void MovePlayer(string playerSrc, RoutingBucket routingBucket)
        {
            var name = API.GetPlayerName(playerSrc);
            _log.Debug($"Moving {name} to bucket {routingBucket}...");

            var hPed = API.GetPlayerPed(playerSrc);
            var vehicle = API.GetVehiclePedIsIn(hPed, false);

            if (vehicle == 0)
            {
                vehicle = API.GetVehiclePedIsIn(hPed, true);
            }

            if (vehicle > 0)
            {
                MoveEntity(vehicle, routingBucket);
            }

            var oldBucket = GetPlayerBucket(playerSrc);
            API.SetPlayerRoutingBucket(playerSrc, (int)routingBucket);

            _comms.PublishAll(new EntityMovedBucketEvent()
            {
                EntityServerHandle = hPed,
                OldBucket = oldBucket,
                NewBucket = routingBucket
            });

            _comms.PublishAll(new PlayerMovedBucketEvent
            {
                PlayerServerHandle = int.Parse(playerSrc),
                PlayerName = name,
                OldBucket = oldBucket,
                NewBucket = routingBucket
            });
        }

        public void MovePlayer(Player player, RoutingBucket routingBucket)
        {
            MovePlayer(player.Handle, routingBucket);
            _legacyComms.ToClient(player, ClientEvents.ShowNotification, "Moved Instance",
                $"You have moved or been moved to instance {routingBucket}!", "info");
        }

        public void MovePlayers(string[] playerSrc, RoutingBucket routingBucket)
        {
            foreach (var p in playerSrc)
            {
                MovePlayer(p, routingBucket);
            }
        }

        public void MovePlayers(Player[] players, RoutingBucket routingBucket)
            => MovePlayers(players.Select(p => p.Handle).ToArray(), routingBucket);

        public void MoveEntity(int entityHandle, RoutingBucket routingBucket)
        {
            var oldBucket = GetEntityBucket(entityHandle);
            API.SetEntityRoutingBucket(entityHandle, (int)routingBucket);

            _comms.PublishAll(new EntityMovedBucketEvent()
            {
                EntityServerHandle = entityHandle,
                OldBucket = oldBucket,
                NewBucket = routingBucket
            });
        }

        public void MoveEntity(Entity entity, RoutingBucket routingBucket)
            => MoveEntity(entity.Handle, routingBucket);

        public RoutingBucket GetPlayerBucket(string playerSrc)
{
            return (RoutingBucket)API.GetPlayerRoutingBucket(playerSrc);
        }

        public RoutingBucket GetPlayerBucket(Player player)
            => GetPlayerBucket(player.Handle);

        public RoutingBucket GetEntityBucket(int entityHandle)
        {
            return (RoutingBucket)API.GetEntityRoutingBucket(entityHandle);
        }

        public RoutingBucket GetEntityBucket(Entity entity)
            => GetEntityBucket(entity.Handle);
    }
}