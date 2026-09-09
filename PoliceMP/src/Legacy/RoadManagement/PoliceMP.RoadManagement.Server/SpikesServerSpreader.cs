using System;
using System.Linq;
using CitizenFX.Core;
using PoliceMP.RoadManagement.Shared;

namespace PoliceMP.RoadManagement.Server
{
    class SpikesServerSpreader : BaseScript
    {
        public SpikesServerSpreader()
        {
            EventHandlers[ServerEvents.BURST_TYRES_ON_ALL_CLIENTS] += new Action<int>(BurstTyresOnAllClients);
        }

        private void BurstTyresOnAllClients(int networkId)
        {
            foreach (var player in Players.ToList())
            {
                player.TriggerEvent(ClientEvents.BURST_TYRES_ON_CLIENTS, networkId);
            }
        }
    }
}
