using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.StandUp
{
    public class StandUp : IAction
    {
        public Ped Target { get; set; }

        public StandUp(Ped target)
        {
            Target = target;
        }
    }
}