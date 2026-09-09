using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.CPR
{
    public class CPR : IAction
    {
        public Ped Target { get; set; }
        
        public Ped Subject { get; set; }
        public bool SkipSendEvent { get; set; }

        public CPR(Ped target, Ped subject, bool skipSendEvent)
        {
            Target = target;
            Subject = subject;
            SkipSendEvent = skipSendEvent;
        }
    }
}