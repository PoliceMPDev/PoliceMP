using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.ObservePed
{
    public class ObservePed : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }

        public ObservePed(Ped subject, Ped target)
        {
            Subject = subject;
            Target = target;
        }
    }
}