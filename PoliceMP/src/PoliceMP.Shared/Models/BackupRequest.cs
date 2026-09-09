using System;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Shared.Models
{
    public class BackupRequest
    {
        public string Id { get; set; }
        public BackupType Type { get; set; }
        public int Player { get; set; }
        public int Ped { get; set; }
        public int NetworkId { get; set; }
        public string Name { get; set; }
        public float CoordsX { get; set; }
        public float CoordsY { get; set; }
        public float CoordsZ { get; set; }
        public string Location { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public int? AreaBlip { get; set; }
            
        public BackupRequest()
        {
            Id = Guid.NewGuid().ToString("N");
        }
    }

}