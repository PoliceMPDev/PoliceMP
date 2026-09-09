using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.ToggleSirens
{
    public class ToggleSirens : IAction
    {
        public Ped Player { get; }
        public Vehicle Vehicle { get; }

        public ToggleSirens(Ped player, Vehicle vehicle)
        {
            Player = player;
            Vehicle = vehicle;
        }
    }
}