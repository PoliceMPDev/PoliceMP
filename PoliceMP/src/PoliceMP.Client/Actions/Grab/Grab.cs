using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.Grab
{
    public class Grab : IAction
    {
        public Ped Target { get; set; }
        public bool OnlyUngrab { get; set; }

        public Grab(Ped target, bool onlyUngrab = false)
        {
            Target = target;
            OnlyUngrab = onlyUngrab;
        }
    }
}