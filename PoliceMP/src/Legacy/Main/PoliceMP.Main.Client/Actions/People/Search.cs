using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Shared.Constants;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Client.Extensions;

namespace PoliceMP.Main.Client.Actions.People
{
    public class Search : BaseScript
    {
        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Search)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_SEARCH, PersonSelector.SelectedPerson.EntityId());
        }

        [EventHandler(ClientEvents.ACTION_SEARCH)]
        private async void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            // Try and retrieve the person.
            var person = await PersonRetriever.GetPerson(pedHandle);
            if (person == null)
            {
                ClientFunctions.ShowToast( "Search", "Could not retrieve the person.", "error");
                return;
            }

            // Make sure the player is close enough.
            if (!Game.PlayerPed.IsNearEntity(person.Ped(), new Vector3(2f, 2f, 2f)))
            {
                ClientFunctions.SendErrorMessage("You are not close enough to the person.");
                return;
            }

            // Play the anims.
            SoundPlayer.PlaySound(SoundPlayer.PatDown);
            await PlayAnims(person);

            // Display the items.
            await DisplayResults(person);
        }

        private static async Task DisplayResults(Person person)
        {
            var items = person.Items;

            if (items.Count == 0 && !person.HasWeapon)
            {
                ClientFunctions.ShowToast("Person Search", "Nothing of interest was found.", "info");
                return;
            }

            var builder = new StringBuilder();

            var hasIllegalItem = person.Items.FirstOrDefault(i => !i.Legal) != null;

            builder.Append(hasIllegalItem
                ? "You found some illegal item(s):<br>"
                : "Clear<br>");

            if (person.HasWeapon)
                builder.Append($"- <span class='text-danger>{ClientFunctions.GetWeaponDisplayName(person.Weapon)}</span><br>");

            if (items.Count > 0)
                foreach (var item in items)
                    builder.Append(item.Legal ? $"- {item.Name}<br>" : $"- <span class='text-danger'>{item.Name}</span><br>");

            ClientFunctions.ShowToast("Person Search", builder.ToString(), "info");

            person.Ped().Task.ClearAll();
            person.Ped().ClearHandsupAnim();
            await Delay(500);
            person.Ped().Task.StandStill(-1);
        }

        private static async Task PlayAnims(Person person)
        {
            await person.Ped().PlayHandsupAnimAsync(true);

            person.Ped().IsPositionFrozen = true;
            person.Ped().IsCollisionEnabled = false;
            person.Ped().Heading = Game.PlayerPed.Heading;

            var offsetPositionFront = API.GetOffsetFromEntityInWorldCoords(person.Ped().Handle, 0f, -0.55f, 0f);

            var count = 0;
            while (!API.IsEntityAtCoord(Game.PlayerPed.Handle, offsetPositionFront.X, offsetPositionFront.Y,
                offsetPositionFront.Z, 0.33f, 0.33f, 0.33f, false, true, 0))
            {
                // Just teleport them to the position if it's been 5 seconds and they're still not there
                if (count == 4)
                {
                    person.Ped().Position = offsetPositionFront;
                    break;
                }

                API.TaskGoStraightToCoord(Game.PlayerPed.Handle, offsetPositionFront.X, offsetPositionFront.Y,
                    offsetPositionFront.Z, 1f, 1000, Game.PlayerPed.Handle, 0.5f);
                await Delay(1000);
                count++;
            }

            Game.PlayerPed.Heading = person.Ped().Heading;

            await Game.PlayerPed.PlayAnimAsync("anim@heists@load_box", "idle", 1.5f, flag: 0);
            await Delay(850);
            await Game.PlayerPed.PlayAnimAsync("anim@heists@box_carry@", "idle", 1.5f, flag: 0);
            await Delay(600);
            await Game.PlayerPed.PlayAnimAsync("missfam5_yoga", "start_pose", 1.5f, flag: 0);
            await Delay(750);

            var position = API.GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, 0f, -0.33f, 0f);
            API.TaskGoStraightToCoord(Game.PlayerPed.Handle, position.X, position.Y, position.Z, 1f, 1000, Game.PlayerPed.Heading, 1f);
            await Delay(1000);
            Game.PlayerPed.Heading -= 20f;

            await Game.PlayerPed.PlayAnimAsync("missbigscore2aig_7@driver", "boot_r_loop", 1.3f, flag: 1);
            await Delay(1000);
            await Game.PlayerPed.PlayAnimAsync("mini@yoga", "outro_2", 1.5f, flag: 0);
            await Delay(1500);

            API.ClearPedTasks(Game.PlayerPed.Handle);
            Game.PlayerPed.Heading += 40f;

            await Game.PlayerPed.PlayAnimAsync("missbigscore2aig_7@driver", "boot_l_loop", 1.3f, flag: 1);
            await Delay(1000);
            await Game.PlayerPed.PlayAnimAsync("mini@yoga", "outro_2", 1.5f, flag: 0);
            await Delay(1500);
            API.ClearPedTasks(Game.PlayerPed.Handle);

            Game.PlayerPed.Heading -= 20f;
            await Delay(50);

            person.Ped().IsCollisionEnabled = true;
            person.Ped().IsPositionFrozen = false;

            API.TaskStandStill(person.Ped().Handle, -1);
        }
    }
}