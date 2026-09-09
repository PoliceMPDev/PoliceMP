using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.Kneel
{
    public class Kneel : IAction
    {
        public Ped Target { get; set; }

        public Kneel(Ped target)
        {
            Target = target;
        }
    }
}