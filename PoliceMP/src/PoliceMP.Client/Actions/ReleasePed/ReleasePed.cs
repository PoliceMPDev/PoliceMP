using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.ReleasePed
{
    public class ReleasePed : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }

        public ReleasePed(Ped subject, Ped target)
        {
            Subject = subject;
            Target = target;
        }
    }
}