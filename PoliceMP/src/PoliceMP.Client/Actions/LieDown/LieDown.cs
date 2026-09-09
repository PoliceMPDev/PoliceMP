using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.LieDown
{
    public class LieDown : IAction
    {
        public Ped Target { get; set; }

        public LieDown(Ped target)
        {
            Target = target;
        }
    }
}