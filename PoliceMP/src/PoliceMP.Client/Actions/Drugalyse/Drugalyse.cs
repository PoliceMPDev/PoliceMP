using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.Drugalyse
{
    public class Drugalyse : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }

        public Drugalyse(Ped subject, Ped target)
        {
            Subject = subject;
            Target = target;
        }
    }
}