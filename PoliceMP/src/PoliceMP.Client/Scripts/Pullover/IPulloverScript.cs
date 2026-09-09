using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.Client.Scripts.Pullover
{
    public interface IPulloverScript
    {
        public Vehicle PulledVehicle { get; }
        public PulloverState State { get; }
        public bool PlayerIsInCopVehicle { get; }

        // Used to release vehicles before a ped fucks off
        Task ForceRelease();
    }
}