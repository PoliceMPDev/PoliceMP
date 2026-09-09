using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Client.Models;
using PoliceMP.Main.Shared.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Managers
{
    /// <summary>
    ///     Manages the people that have been arrested by the user.
    /// </summary>
    public class ArrestManager : BaseScript
    {
        /// <summary>
        ///     All the people that the player has arrested (that haven't been taken away by
        ///     prisoner transport or booked in at the station).
        /// </summary>
        private readonly List<ArrestedPerson> _arrestedPeople;

        /// <summary>
        ///     Create a new instance of the Arrests class.
        /// </summary>
        public ArrestManager()
        {
            _arrestedPeople = new List<ArrestedPerson>();
        }

        /// <summary>
        ///     Adds an arrested person.
        /// </summary>
        /// <param name="entityId">The entity ID of the arrested person.</param>
        [EventHandler(ClientEvents.ADD_ARRESTED_PERSON)]
        private void AddArrestedPerson(int entityId)
        {
            if (_arrestedPeople.FirstOrDefault(a => a.EntityId == entityId) != null) return;

            var blip = API.AddBlipForEntity(entityId);

            API.SetBlipSprite(blip, 143);
            API.SetBlipAlpha(blip, 255);
            API.SetBlipColour(blip, 3);
            API.SetBlipScale(blip, 0.5f);

            var arrestedPerson = new ArrestedPerson(entityId, blip);

            _arrestedPeople.Add(arrestedPerson);
        }

        /// <summary>
        ///     Removes an arrested person.
        /// </summary>
        /// <param name="entityId">The entity ID of the arrested person.</param>
        [EventHandler(ClientEvents.REMOVE_ARRESTED_PERSON)]
        private void RemoveArrestedPerson(int entityId)
        {
            var arrestedPerson = _arrestedPeople.FirstOrDefault(a => a.EntityId == entityId);
            if (arrestedPerson == null) return;

            // Remove the blip
            var blip = arrestedPerson.BlipId;
            if (API.DoesBlipExist(blip)) API.RemoveBlip(ref blip);

            _arrestedPeople.Remove(arrestedPerson);
        }

        /// <summary>
        ///     Called every half a second.
        /// </summary>
        [Tick]
        private async Task OnTick()
        {
            // Wait half a second.
            await Delay(500);

            foreach (var person in _arrestedPeople)
            {
                // Check if the person still exists or is dead.
                if (!API.DoesEntityExist(person.EntityId) || API.IsEntityDead(person.EntityId))
                {
                    RemoveArrestedPerson(person.EntityId);
                    continue;
                }

                // Check to make sure the person is still playing the cuffed anim.
                if (!API.IsEntityPlayingAnim(person.EntityId, "mp_arresting", "idle", 3))
                {
                    await Delay(500);

                    var ped = (Ped)Entity.FromHandle(person.EntityId);
                    await ped.Task.PlayAnimation("mp_arresting", "idle", 8f, -8f, -1, (AnimationFlags)49, 0);
                }
            }
        }

        [Tick]
        private async Task OnLongTick()
        {
            // Because FiveM is a prick, check every 10 seconds to ensure they're playing the anim
            await Delay(10000);

            foreach (var person in _arrestedPeople)
            {
                // Check if the person still exists or is dead.
                if (!API.DoesEntityExist(person.EntityId) || API.IsEntityDead(person.EntityId))
                {
                    RemoveArrestedPerson(person.EntityId);
                    continue;
                }

                await Delay(500);

                var ped = (Ped)Entity.FromHandle(person.EntityId);
                await ped.Task.PlayAnimation("mp_arresting", "idle", 8f, -8f, -1, (AnimationFlags)49, 0);
            }
        }
    }
}