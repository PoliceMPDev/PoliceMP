using CitizenFX.Core;
using System;

namespace PoliceMP.Callouts.Server.Models
{
    public class CalloutPlayer
    {
        public Player Player { get; private set; }
        public bool IsHost { get; set; }
        public bool HasArrived { get; set; }
        public bool EntitiesExistOnClient { get; set; }
        public bool WithinRange { get; set; }
        public DateTime TimeOfArrival { get; set; }

        public CalloutPlayer(Player player)
        {
            Player = player;
            IsHost = false;
            HasArrived = false;
            EntitiesExistOnClient = false;
        }
    }
}
