using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using PoliceMP.Main.Client.Actions;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Menus;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Models;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Managers
{
    /// <summary>
    ///     Handles the selecting of people.
    /// </summary>
    public class PersonSelector : BaseScript
    {
        /// <summary>
        ///     Max distance the player can be from the person before they
        ///     get unselected.
        /// </summary>
        public const float MAX_DISTANCE_FROM_SELECTED_PERSON = 5f;

        /// <summary>
        ///     Max distance the player can be from the person in order to
        ///     select them.
        /// </summary>
        public const float MAX_DISTANCE_TO_STOP_PERSON = 10f;

        /// <summary>
        ///     The currently selected person.
        /// </summary>
        public static Person SelectedPerson { get; private set; }

        /// <summary>
        ///     The last selected person.
        /// </summary>
        public static Person LastSelectedPerson { get; private set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [Tick]
        private async Task CheckPersonStillValid()
        {
            await Delay(500);

            if (SelectedPerson == null) return;

            if (!API.DoesEntityExist(SelectedPerson.EntityId()) ||
                API.IsPedDeadOrDying(SelectedPerson.EntityId(), true) ||
                API.IsPedInAnyVehicle(Game.PlayerPed.Handle, true) ||
                API.IsPedInAnyVehicle(SelectedPerson.EntityId(), true))
            {
                UnselectPerson();
                return;
            }

            // Check if the ped has moved too far away
            if (!Game.PlayerPed.IsCloseEnoughToEntity(SelectedPerson.Ped(), MAX_DISTANCE_FROM_SELECTED_PERSON))
                UnselectPerson();
        }

        /// <summary>
        ///     Checks for player input.
        /// </summary>
        //private async Task CheckControls()
        //{
        //    // Check if the player is aiming at a person
        //    if (API.IsControlPressed(0, 25))
        //    {
        //        var entity = -1;
        //        if (API.GetEntityPlayerIsFreeAimingAt(API.PlayerId(), ref entity))
        //        {
        //            if (entity == -1) return;

        //            if (API.GetEntityType(entity) != 1) return;

        //            var ped = (Ped)Entity.FromHandle(entity);
        //            if (ped == null) return;

        //            if (!API.DoesEntityExist(ped.Handle)) return;

        //            if (API.IsPedDeadOrDying(entity, true)) return;

        //            if (!ped.IsInteractable()) return;

        //            if (!Game.PlayerPed.IsCloseEnoughToEntity(ped, MAX_DISTANCE_TO_STOP_PERSON)) return;

        //            Screen.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ ~w~to stop person.");

        //            if (API.IsControlJustPressed(0, 51)) // E
        //            {
        //                if (API.IsPedInAnyVehicle(entity, false))
        //                {
        //                    API.TaskLeaveVehicle(entity, API.GetVehiclePedIsIn(entity, false), 256);
        //                    while (API.IsPedInAnyVehicle(entity, true)) await Delay(50);
        //                }

        //                TriggerEvent(ClientEvents.ACTION_LIE_DOWN, entity);
        //            }
        //        }
        //    }
        //    // Check if the player is trying to select a person.
        //    else if (SelectedPerson == null && API.IsControlJustPressed(0, 51)) // E
        //    {
        //        var ped = Game.PlayerPed.GetInteractablePedInFront();
        //        if (ped == null || ped.IsDead || !ped.IsInteractable() || ped.IsInVehicle()) return;

        //        var count = 0;
        //        var controlStillPressed = true;
        //        while (count < 2)
        //        {
        //            await Delay(200);

        //            if (!API.IsControlPressed(0, 51))
        //            {
        //                controlStillPressed = false;
        //                break;
        //            }

        //            count++;
        //        }

        //        if (controlStillPressed)
        //        {
        //            TriggerEvent(ClientEvents.ACTION_CUFF, ped.Handle);
        //            return;
        //        }

        //        var person = await PersonRetriever.TrySelectPerson(ped.Handle);
        //        if (person == null)
        //        {
        //            Screen.ShowSubtitle("~r~You can't interact with that person just now.");
        //            return;
        //        }

        //        SelectPerson(person);
        //    }
        //}

        /// <summary>
        ///     Updates the screen with help messages where appropriate.
        /// </summary>
        //[Tick]
        //private Task UpdateScreen()
        //{
        //    var ped = Game.PlayerPed.GetInteractablePedInFront();

        //    if (SelectedPerson == null && ped != null)
        //    {
        //        var isCuffed = API.IsPedCuffed(ped.Handle);
        //        Screen.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ ~w~to interact with person." +
        //                                        (isCuffed
        //                                            ? "\nHold ~INPUT_CONTEXT~ ~w~to uncuff the person."
        //                                            : "\nHold ~INPUT_CONTEXT~ ~w~to arrest the person."));
        //    }

        //    return Task.FromResult(0);
        //}

        /// <summary>
        ///     Selects the given person.
        /// </summary>
        /// <param name="person">The person.</param>
        public static void SelectPerson(Person person)
        {
            if (!person.Ped().IsInteractable())
            {
                Screen.ShowSubtitle("~r~Somebody is already interacting with this person.");
                return;
            }

            if (Resist.Check(person)) return;

            // Unselect the current person if there is one
            UnselectPerson();

            // Assign the person
            SelectedPerson = person;

            // Make sure they don't run away if they aren't playing the kneeling anim
            person.Ped().Stop();

            API.PlaySound(208, "SELECT", "HUD_MINI_GAME_SOUNDSET", false, 0, true);

            person.Ped().Task.TurnTo(Game.PlayerPed);

            MenuManager.InteractionMenu.OpenMenu();

            TriggerEvent(ClientEvents.ON_PERSON_SELECTED);

            person.Ped().SetInteractable(false);
        }

        /// <summary>
        ///     Handles the event to unselect the person.
        /// </summary>
        public static void UnselectPerson()
        {
            if (SelectedPerson == null) return;

            // Reset the selected person and remove the blip.
            var networkId = SelectedPerson.NetworkId;

            //SelectedPerson.Ped().IsPersistent = false;

            SelectedPerson.Ped().Stop();

            SelectedPerson.Ped().SetInteractable(true);

            SelectedPerson.Ped().IsCollisionEnabled = true;

            LastSelectedPerson = SelectedPerson;

            SelectedPerson = null;

            // Close menu in case it's open.
            MenuManager.InteractionMenu.CloseMenu();

            TriggerEvent(ClientEvents.ON_PERSON_UNSELECTED);
            TriggerServerEvent(ServerEvents.ON_PERSON_UNSELECTED, networkId);
        }

        [EventHandler(ClientEvents.ON_INTERACTION_MENU_CLOSED)]
        private async void OnInteractionMenuClosed()
        {
            await Delay(50);

            if (!MenuController.IsAnyMenuOpen())
                UnselectPerson();
        }
    }
}