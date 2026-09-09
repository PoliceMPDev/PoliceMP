using CitizenFX.Core;
using PoliceMP.Core.Client.Actions.Interfaces;

namespace PoliceMP.Client.Actions.AskDriverToStepOut
{
    public class AskPedToStepOut : IAction
    {
        public Ped Ped { get; }
        public Vehicle Vehicle { get; }
        public bool HandsUp { get; }
        public bool Aggressive { get; }

        public AskPedToStepOut(Ped ped, Vehicle vehicle = null, bool handsUp = false, bool aggressive = false)
        {
            Ped = ped;
            Vehicle = vehicle;
            HandsUp = handsUp;
            Aggressive = aggressive;
        }
    }
}