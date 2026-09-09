using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Shared.Events;

namespace PoliceMP.Main.Client.Actions.People
{
    public class Warning : BaseScript
    {
        [Command("warning")]
        private void Command()
        {
            var pedHandle = -1;
            if (PersonSelector.SelectedPerson != null)
                pedHandle = PersonSelector.SelectedPerson.EntityId();
            else if (CarSelector.SelectedCar != null && CarSelector.SelectedCar.Vehicle().Driver != null)
                pedHandle = CarSelector.SelectedCar.Vehicle().Driver.Handle;

            if (pedHandle == -1)
            {
                ClientFunctions.SendErrorMessage("You have no selected person or car.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_WARNING, pedHandle);
        }

        [EventHandler(ClientEvents.ACTION_WARNING)]
        private async void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            var ped = (Ped)Entity.FromHandle(pedHandle);
            if (!Entity.Exists(ped))
            {
                ClientFunctions.SendErrorMessage("The ped entity did not exist.");
                return;
            }

            Screen.ShowSubtitle("~b~You: ~w~I'm going to let you off with a warning. Don't do that again.");

            Game.PlayerPed.Task.TurnTo(ped);

            await Delay(500);

            Game.PlayerPed.Task.PlayAnimation("gestures@f@standing@casual", "gesture_point");

            await Delay(1000);

            ped.Task.PlayAnimation("anim@mp_player_intcelebrationfemale@face_palm",
                "face_palm");
        }
    }
}