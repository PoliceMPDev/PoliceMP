using System;
using System.ComponentModel.DataAnnotations;

namespace PoliceMP.Data.Entities
{
    public class UserActivity
    {
        [Key]
        public int UserId { get; set; }
        public string SteamId { get; set; }
        public string SteamName { get; set; }
        public int TotalMinutes { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsMod { get; set; }
    }
}