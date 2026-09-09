using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.Follow
{
    public class Follow : IAction
    {
        public Ped Subject { get; set; }
        public Ped Target { get; set; }

        public Follow(Ped subject, Ped target)
        {
            Subject = subject;
            Target = target;
        }
    }
}