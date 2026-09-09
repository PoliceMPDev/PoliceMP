using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.AskForId
{
    public class AskForId : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }

        public AskForId(Ped subject, Ped target)
        {
            Subject = subject;
            Target = target;
        }
    }
}