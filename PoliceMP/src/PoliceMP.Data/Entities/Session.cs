using System;

namespace PoliceMP.Data.Entities
{
    public class Session
    {
        public int SessionId { get; set; }

        public string PlayerName { get; set; }
        public string IpAddress { get; set; }
        public string License { get; set; }
        public string Discord { get; set; }
        public string Xbl { get; set; }
        public string LiveId { get; set; }

        public string DisconnectedReason { get; set; }

        public DateTime ConnectedAt { get; set; }
        public DateTime DisconnectedAt { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
