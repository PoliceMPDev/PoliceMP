using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Main.Client.Actions.People
{
    /// <summary>
    ///     Used to put people into cells.
    /// </summary>
    public class Book : BaseScript
    {
        /// <summary>
        ///     The location where the player must be to book the person.
        /// </summary>
        private static readonly Vector3 _bookLocation = new Vector3(473.73f, -1008.73f, 26.27f);

        /// <summary>
        ///     Locations for inside the cells.
        /// </summary>
        ///
        /*
        private static readonly Vector3[] _cells =
        {
            new Vector3(459.45f, -1001.45f, 24.91f),
            new Vector3(459.12f, -997.86f, 24.91f),
            new Vector3(460.26f, -994.36f, 24.91f)
        };
        */

        /// <summary>
        ///     Add a blip for the cells location.
        /// </summary>
        public Book()
        {
            var blip = World.CreateBlip(_bookLocation);
            blip.Sprite = BlipSprite.Custody; // or 188
            blip.IsShortRange = true;
        }

        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Book)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_BOOK, PersonSelector.SelectedPerson.EntityId());
        }

        /// <summary>
        ///     Handles the BOOK_PERSON event.
        /// </summary>
        /// <param name="entityId">Ped entity ID.</param>
        [EventHandler(ClientEvents.ACTION_BOOK)]
        private async void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            // Try and retrieve the person.
            var person = await PersonRetriever.GetPerson(pedHandle);
            if (person == null)
            {
                ClientFunctions.SendErrorMessage("Could not retrieve the person.");
                return;
            }

            // Check if the player and ped are close enough.
            if (!IsPlayerAndPedAtCells(person.Ped()))
            {
                ClientFunctions.SendErrorMessage("You both need to be at the cells to book the person.");
                return;
            }

            ClientFunctions.ShowToast(
                "Police Cells",
                "The ped is now being booked in...",
                "info");
            await EnterCell(person);
        }

        private static bool IsPlayerAndPedAtCells(Ped ped)
        {
            return IsAtCells(ped) && IsAtCells(Game.PlayerPed);
        }

        /// <summary>
        ///     Makes the person enter the cell and waits before deleting them.
        /// </summary>
        /// <param name="person">The person.</param>
        private static async Task EnterCell(Person person)
        {
            person.Ped().IsPositionFrozen = true;
            person.Ped().Task.StandStill(-1);
            person.Ped().SetInteractable(false);
            
            ClientFunctions.ShowToast(
                "Police Cells",
                $"You have just booked <span class='text-danger'>{person.FullName}</span>.",
                "info");

            await GiveReward(person);

            TriggerEvent(ClientEvents.REMOVE_ARRESTED_PERSON, person.EntityId());

            await Delay(10000);

            API.NetworkFadeOutEntity(person.EntityId(), true, false);
            Entity.FromHandle(person.EntityId()).Delete();
        }

        /// <summary>
        ///     Checks to see if the entity is at the cells.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        /// <returns>Whether the entity is at the cells.</returns>
        private static bool IsAtCells(Ped ped)
        {
            if (API.IsEntityAtCoord(ped.Handle, _bookLocation.X, _bookLocation.Y, _bookLocation.Z, 10f, 2f, 10f, false,
                true, 0))
                return true;
            return false;
        }

        private static Task GiveReward(Person person)
        {
            if (person.HasIllegalItems || person.OnAnyDrugs || person.HasMarkers || person.HasWarrants ||
                person.AlcoholLevel > 0.1f)
            {
                TriggerServerEvent(ServerEvents.GIVE_EXPERIENCE, 100);
            }

            return Task.FromResult(0);
        }
    }
}