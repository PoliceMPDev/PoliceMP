using System;
using System.Collections.Generic;

namespace PoliceMP.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string SteamId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
