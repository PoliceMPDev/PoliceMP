using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions
{
    public class BootEnter : IAction
    {
        public Vehicle Target { get; }

        public BootEnter(Vehicle target)
        {
            Target = target;
        }
    }
}
