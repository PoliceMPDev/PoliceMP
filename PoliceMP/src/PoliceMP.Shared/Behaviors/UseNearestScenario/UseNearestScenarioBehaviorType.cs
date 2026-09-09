using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Shared.Behaviors.UseNearestScenario
{
    public class UseNearestScenarioBehavior : PedBehaviorDefinition
    {
        public UseNearestScenarioBrainState State { get; set; }
    }
}