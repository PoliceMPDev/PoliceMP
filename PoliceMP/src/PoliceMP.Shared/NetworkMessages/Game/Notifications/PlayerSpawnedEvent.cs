using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Game.Notifications
{
    public class PlayerSpawnedEvent : INotification
    {
        public int ServerHandle { get; set; }
        public int PedNetworkId { get; set; }
    }
}
