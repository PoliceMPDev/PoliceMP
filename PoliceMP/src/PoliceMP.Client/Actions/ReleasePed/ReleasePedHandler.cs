using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using System.Threading.Tasks;

namespace PoliceMP.Client.Actions.ReleasePed
{
    public class ReleasePedHandler : ActionHandler<ReleasePed>
    {
        private readonly INotificationService _notifications;
        private readonly ISpeechService _speech;

        public ReleasePedHandler(INotificationService notifications, ISpeechService speech)
        {
            _notifications = notifications;
            _speech = speech;
        }

        protected override Task<bool> Handle(ReleasePed action)
        {
            _speech.Say(Game.PlayerPed, "You are free to go.");

            if (action.Target.IsCuffed)
            {
                _notifications.Error("Release Ped", "You cannot release a ped who is cuffed. You should uncuff them first.");
                return Task.FromResult(false);
            }

            action.Target.Task.ClearAll();
            action.Target.CanPlayAmbientAnims(true);
            action.Target.BlockPermanentEvents = false;
            API.TaskSetBlockingOfNonTemporaryEvents(action.Target.Handle, false);
            action.Target.AlwaysKeepTask = false;

            var lastVehicle = action.Target.LastVehicle;
            if (lastVehicle != null && lastVehicle.ClassType != VehicleClass.Emergency && lastVehicle.IsDriveable)
            {
                var seat = action.Target.GetLastSeatInVehicle(lastVehicle);
                if (seat != VehicleSeat.None)
                {
                    _speech.Do(action.Target, "Gets into vehicle.");
                    action.Target.Task.EnterVehicle(lastVehicle, seat);
                    return Task.FromResult(true);
                }
            }

            _speech.SayRandomFarewell(action.Target);
            action.Target.Task.WanderAround();
            return Task.FromResult(true);
        }
    }
}