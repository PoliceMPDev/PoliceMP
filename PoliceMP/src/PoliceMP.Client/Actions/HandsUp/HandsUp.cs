using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.HandsUp
{
    public class HandsUp : IAction
    {
        public Ped Target { get; set; }

        public HandsUp(Ped target)
        {
            Target = target;
        }
    }
}