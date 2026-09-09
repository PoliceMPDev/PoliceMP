using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions.AskAllOccupantsToStepOut
{
    public class AskAllOccupantsToStepOutHandler : ActionHandler<AskAllOccupantsToStepOut>
    {
        private readonly ISpeechService _speech;

        public AskAllOccupantsToStepOutHandler(ISpeechService speech)
        {
            _speech = speech;
        }

        protected override async Task<bool> Handle(AskAllOccupantsToStepOut action)
        {
            _speech.Say(Game.PlayerPed, "Can everyone step out of the vehicle for me please?");

            foreach (var occupant in action.Target.Occupants)
            {
                occupant.Task.LeaveVehicle();
                _speech.Do(occupant, "Gets out of the vehicle.");
                await occupant.StandStillFacingPlayer();
            }

            var timeOut = DateTime.Now;
            while ((DateTime.Now - timeOut).TotalSeconds <= 5 && action.Target.Occupants.Length > 0)
            {
                await Delay(500);
            }

            return true;
        }
    }
}