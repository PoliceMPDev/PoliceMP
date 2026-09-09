using System;
using CitizenFX.Core;
using PoliceMP.Shared.Enums;
using NotImplementedException = System.NotImplementedException;

namespace PoliceMP.Server.Services.Interfaces
{
    public interface IBucketService
    {
        void MovePlayer(string playerSrc, RoutingBucket routingBucket);
        void MovePlayer(Player player, RoutingBucket routingBucket);
        void MovePlayers(string[] playerSrc, RoutingBucket routingBucket);
        void MovePlayers(Player[] players, RoutingBucket routingBucket);
        void MoveEntity(int entityHandle, RoutingBucket routingBucket);
        void MoveEntity(Entity entity, RoutingBucket routingBucket);
        RoutingBucket GetPlayerBucket(string playerSrc);
        RoutingBucket GetPlayerBucket(Player player);
        RoutingBucket GetEntityBucket(Entity entity);
    }
}
