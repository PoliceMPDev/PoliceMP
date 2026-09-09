using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.AskAllOccupantsToStepOut
{
    public class AskAllOccupantsToStepOut : IAction
    {
        public Vehicle Target { get; }

        public AskAllOccupantsToStepOut(Vehicle target)
        {
            Target = target;
        }
    }
}