using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Retrievers;
using PoliceMP.Main.Client.Util;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions.People
{
    /// <summary>
    ///     Used to cuff people.
    /// </summary>
    public class Cuff : BaseScript
    {
        // private static readonly Dictionary<int, int> _cuffObjects = new Dictionary<int, int>();

        /// <summary>
        ///     The command used to execute the action.
        /// </summary>
        [Command(Commands.Cuff)]
        private void Command()
        {
            if (PersonSelector.SelectedPerson == null)
            {
                ClientFunctions.SendErrorMessage("You need to select a person to do this.");
                return;
            }

            TriggerEvent(ClientEvents.ACTION_CUFF, PersonSelector.SelectedPerson.EntityId());
        }

        /// <summary>
        ///     Handles the CUFF_PERSON event.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        [EventHandler(ClientEvents.ACTION_CUFF)]
        private async void Execute(int pedHandle)
        {
            // Check action cooldown.
            if (!ActionCooldown.Check()) return;

            // Try to retrieve the person.
            var person = await PersonRetriever.GetPerson(pedHandle);
            if (person == null)
            {
                ClientFunctions.SendErrorMessage("Could not retrieve the person.");
                return;
            }

            // Check if the player is close enough to the person.
            if (!Game.PlayerPed.IsNearEntity(person.Ped(), new Vector3(2f, 2f, 2f)))
            {
                ClientFunctions.SendErrorMessage("You are not close enough to the person.");
                return;
            }

            // Check if the person is already cuffed, if so uncuff them
            if (person.Ped().IsCuffed)
                await UncuffPerson(person.Ped());
            else
                await CuffPerson(person.Ped());

            person.Ped().Stop();
            person.Ped().IsCollisionEnabled = true;
        }

        /// <summary>
        ///     Makes the player cuff the person.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        private static async Task CuffPerson(Ped ped)
        {
            Screen.ShowSubtitle(
                "~b~You: ~w~I am placing you under arrest. You do not have to say anything. But, it may harm your defence if you do not mention when questioned something which you later rely on in court. Anything you do say may be given in evidence.",
                10000);

            ped.CanRagdoll = false;

            ped.Weapons.RemoveAll();
            API.SetEnableHandcuffs(ped.Handle, true);

            await GetInPosition(ped);

            // Attach the ped to the player
            ped.AttachTo(Game.PlayerPed, new Vector3(0f, 1f, 0f), Vector3.Zero);
            Game.PlayerPed.IsPositionFrozen = true;

            // Make them play the anims
            Game.PlayerPed.Task.PlayAnimation("mp_arresting", "a_uncuff");
            await ped.Task.PlayAnimation("mp_arresting", "idle", 8f, -8f, -1, (AnimationFlags)49, 0);

            SoundPlayer.PlaySound(SoundPlayer.Cuff);

            // Detach the ped from the player
            ped.Detach();

            // Add the cuff objects
            /* var boneIndex = API.GetPedBoneIndex(ped.Handle, 18905);
             int obj;
             if (API.IsPedMale(ped.Handle))
             {
                 obj = API.CreateObject(API.GetHashKey("p_cs_cuffs_02_s"), 0f, 0f, 0f, true, true, true);
                 API.AttachEntityToEntity(obj, ped.Handle, boneIndex, 0.005f, 0.062f, 0.03f, -15f, 90f, 112f,
                     true, true,
                     false, false, 1, true);
                 API.SetEntityCollision(obj, false, false);
             }

             else
             {
                 obj = API.CreateObject(API.GetHashKey("prop_cs_cuffs_01"), 0f, 0f, 0f, true, true, true);
                 API.AttachEntityToEntity(obj, ped.Handle, boneIndex, -0.04f, 0.04f, 0.03f, -60f, 50f, -60f, true,
                     true,
                     false, false, 1, true);
                 API.SetEntityCollision(obj, false, false);
             }

             if (obj != -1) _cuffObjects.Add(ped.Handle, obj);*/

            Game.PlayerPed.IsPositionFrozen = false;
            ped.CanRagdoll = true;

            // Add to the arrested people list.
            TriggerEvent(ClientEvents.ADD_ARRESTED_PERSON, ped.Handle);

            ped.IsCollisionEnabled = true;
                        ped.Detach();
           
            //testing woody
            ped.Detach();

        }

        /// <summary>
        ///     Makes the player uncuff the person.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        private static async Task UncuffPerson(Ped ped)
        {
            Screen.ShowSubtitle("~b~You: ~w~Looks like it's your lucky day...");

            await GetInPosition(ped);

            ped.AttachTo(Game.PlayerPed, new Vector3(0f, 0.6f, 0f));
            Game.PlayerPed.IsPositionFrozen = true;
            Game.PlayerPed.Task.PlayAnimation("mp_arresting", "a_uncuff");

            SoundPlayer.PlaySound(SoundPlayer.Cuff);

            ped.CanRagdoll = true;

            API.SetEnableHandcuffs(ped.Handle, false);
            TriggerEvent(ClientEvents.REMOVE_ARRESTED_PERSON, ped.Handle);

            ped.Detach();

            Game.PlayerPed.IsPositionFrozen = false;


            //woody changed for delay increase
            await Delay(3000);

            ped.BlockPermanentEvents = false;
            ped.Task.ClearAll();
            ped.Task.ClearAnimation("mp_arresting", "idle");
            ped.Task.StandStill(-1);

            /* var obj = _cuffObjects[ped.Handle];
             API.DetachEntity(obj, false, false);
             API.DeleteEntity(ref obj);
             _cuffObjects.Remove(ped.Handle);*/

            ped.IsCollisionEnabled = true;

            //woody testing
            ped.Detach();

        }

        private static async Task GetInPosition(Ped ped)
        {
            // Make ped face the way the player is.
            ped.Task.AchieveHeading(Game.PlayerPed.Heading);
            ped.Task.StandStill(-1);

            await Delay(1000);
        }
    }
}