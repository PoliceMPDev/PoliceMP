using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.JailPed
{
    public class JailPed : IAction
    {
        public Ped Target { get; set; }

        public JailPed(Ped target)
        {
            Target = target;
        }
    }
}