using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Mediator;

namespace PoliceMP.Shared.NetworkMessages.Game.Notifications
{
    public class PlayerDroppedEvent : INotification
    {
        public int ServerHandle { get; set; }
        public string PlayerName { get; set; }
    }
}
