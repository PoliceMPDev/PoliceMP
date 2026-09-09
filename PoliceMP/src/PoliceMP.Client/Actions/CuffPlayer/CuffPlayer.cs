using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.CuffPlayer
{
    public class CuffPlayer : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }

        public CuffPlayer(Ped target)
        {
            Target = target;
        }
    }
}
