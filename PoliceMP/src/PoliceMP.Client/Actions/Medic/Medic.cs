using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.Medic
{
    public class Medic : IAction
    {
        public Ped Target { get; set; }

        public Medic(Ped target)
        {
            Target = target;
        }
    }
}