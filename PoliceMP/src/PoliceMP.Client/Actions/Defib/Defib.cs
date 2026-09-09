using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.Defib
{
    public class Defib : IAction
    {
        public Ped Target { get; set; }
        public Ped Subject { get; }
        
        public bool SkipSendEvent { get; set; }

        public Defib(Ped target, Ped subject, bool skipSendEvent)
        {
            Target = target;
            Subject = subject;
            SkipSendEvent = skipSendEvent;
        }
    }
}