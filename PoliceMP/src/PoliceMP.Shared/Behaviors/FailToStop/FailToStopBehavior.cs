using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.FailToStop
{
    public enum FailToStopBehaviorState
    {
        Fleeing,
        GiveUp
    }

    /// <summary>
    /// Behavior used for peds running from police
    /// </summary>
    public class FailToStopBehavior : PedBehaviorDefinition
    {
        public double StartTime { get; set; }
        public FailToStopBehaviorState State { get; set; }
        public bool ShouldDecamp { get; set; }
        public int DrivingStyle { get; set; }
        public bool HasDecamped { get; set; }
        public bool StopForVehicles { get; set; }
        public bool Reckless { get; set; }
        public bool DetectedTpac { get; set; }
        public bool Dazed { get; set; }
    }
}