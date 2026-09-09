using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.CollectDNA
{
    public class CollectDNA : IAction
    {
        public Ped Target { get; }

        public CollectDNA(Ped target)
        {
            Target = target;
        }
    }
}