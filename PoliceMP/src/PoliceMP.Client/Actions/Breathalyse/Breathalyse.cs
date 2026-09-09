using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.Breathalyse
{
    public class Breathalyse : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }

        public Breathalyse(Ped subject, Ped target)
        {
            Subject = subject;
            Target = target;
        }
    }
}