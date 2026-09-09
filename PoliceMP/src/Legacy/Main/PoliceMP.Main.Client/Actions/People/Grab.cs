using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Shared.Events;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Client.Extensions;

namespace PoliceMP.Main.Client.Actions.People
{
    public class Grab : BaseScript
    {
        private static Ped _grabbedPed;

        [Tick]
        private Task OnTick()
        {
            if (_grabbedPed == null) return Task.FromResult(0);

            if (!API.NetworkHasControlOfNetworkId(_grabbedPed.NetworkId))
            {
                API.NetworkRequestControlOfNetworkId(_grabbedPed.NetworkId);
            }

            var posFront = _grabbedPed.GetOffsetPosition(new Vector3(20f, 0f, 0f));

            if (Game.PlayerPed.IsWalking)
                API.TaskGoStraightToCoord(_grabbedPed.Handle, posFront.X, posFront.Y, posFront.Z, 1f, -1,
                    Game.PlayerPed.Heading, 1f);
            else if (Game.PlayerPed.IsRunning)
                API.TaskGoStraightToCoord(_grabbedPed.Handle, posFront.X, posFront.Y, posFront.Z, 2f, -1,
                    Game.PlayerPed.Heading, 1f);
            else if (Game.PlayerPed.IsSprinting)
                API.TaskGoStraightToCoord(_grabbedPed.Handle, posFront.X, posFront.Y, posFront.Z, 3f, -1,
                    Game.PlayerPed.Heading, 1f);
            else
                _grabbedPed.Task.StandStill(-1);

            return Task.FromResult(0);
        }

        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Grab)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_GRAB, PersonSelector.SelectedPerson.EntityId());
        }

        /// <summary>
        ///     Handles the GRAB_PERSON event.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        [EventHandler(ClientEvents.ACTION_GRAB)]
        private async void Execute(int pedHandle)
        {
            if (!ActionCooldown.Check()) return;

            if (_grabbedPed != null)
            {
                UngrabPed();
                return;
            }

            var ped = (Ped)Entity.FromHandle(pedHandle);

            if (!Game.PlayerPed.IsNearEntity(ped, new Vector3(2f, 2f, 2f)))
            {
                ClientFunctions.SendErrorMessage("You are not close enough to the person.");
                UngrabPed();

                return;
            }

            await GrabPed(ped);

            ped.IsCollisionEnabled = true;
        }

        private static void UngrabPed()
        {
            if (_grabbedPed == null) return;

            _grabbedPed.Detach();
            _grabbedPed.IsInvincible = false;

            _grabbedPed.Task.ClearAll();
            Game.PlayerPed.Task.ClearAll();

            Screen.ShowSubtitle("~b~You: ~w~Stay here.");

            _grabbedPed.Task.StandStill(-1);
            
            //woody testing
            _grabbedPed.Detach();


            API.SetEntityCollision(_grabbedPed.Handle, true, true);

            _grabbedPed = null;
        }

        private static async Task GrabPed(Ped ped)
        {
            var position = ped.GetOffsetPosition(new Vector3(0f, -1f, 0f));
            Game.PlayerPed.Task.GoTo(position);

            await Delay(1000);

            Game.PlayerPed.Task.AchieveHeading(ped.Heading);

            await Delay(1000);

            ped.AttachTo(Game.PlayerPed, new Vector3(-0.2f, 0.4f, 0f));

            await Game.PlayerPed.PlayGrabPedAnimAsync(1f);

            ped.IsInvincible = true;
            ped.Task.StandStill(-1);

            _grabbedPed = ped;


            Screen.ShowSubtitle("~b~You: ~w~Let me walk you over here...");
        }

        [EventHandler(ClientEvents.ON_PERSON_UNSELECTED)]
        private void OnPersonUnselected()
        {
            UngrabPed();
        }
    }
}