using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.Cuff
{
    public class Cuff : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }
        
        public bool SkipSendEvent { get; set; } 

        public Cuff(Ped subject, Ped target, bool skipSendEvent)
        {
            Subject = subject;
            Target = target;
            SkipSendEvent = skipSendEvent;
        }
    }
}