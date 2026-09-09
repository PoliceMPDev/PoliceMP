using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.Tailgate
{
    public class TailgateBehavior : PedBehaviorDefinition
    {
        public int TargetVehicleNetworkId { get; set; }
        public int MinDistance { get; set; }
    }
}