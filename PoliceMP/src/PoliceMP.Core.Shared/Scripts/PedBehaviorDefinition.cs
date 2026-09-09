using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Core.Shared.Scripts
{
    public abstract class PedBehaviorDefinition
    {
        public bool CanMigrate { get; set; } = true;
        public int HostServerId { get; set; }
        public double MigrateTime { get; set; }
        public double NextAbleToMigrate { get; set; }
        public float HostMigrateThreshold { get; set; } = 10f;
        public int TimeToKeepControlAfterMigrationSeconds { get; set; } = 1;
    }
}
