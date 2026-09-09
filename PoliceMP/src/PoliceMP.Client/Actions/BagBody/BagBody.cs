using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.BagBody
{
    public class BagBody : IAction
    {
        public Ped Target { get; }

        public BagBody(Ped target)
        {
            Target = target;
        }
    }
}