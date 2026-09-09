using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.WarnPed
{
    public class WarnPed : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }

        public WarnPed(Ped subject, Ped target)
        {
            Subject = subject;
            Target = target;
        }
    }
}