using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using RoadManagement.Shared;

namespace RoadManagement.Server
{
    public class SpikesEvents : BaseScript
    {
        public SpikesEvents()
        {
            EventHandlers[ServerEvents.BURST_TYRES_ON_ALL_CLIENTS] += new Action<int>(BurstTyresOnEveryClient);
        }

        public void BurstTyresOnEveryClient(int networkId)
        {
           foreach (var player in new PlayerList())
            {
                player.TriggerEvent(ClientEvents.BURST_TYRES_ON_CLIENTS, networkId);
            }
        }
    }
}
