using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Factories;
using PoliceMP.Main.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Retrievers
{
    /// <summary>
    ///     Retrieves people from the server.
    /// </summary>
    public class PersonRetriever : BaseScript
    {
        /// <summary>
        ///     The latest retrieved person.
        /// </summary>
        private static Person _retrievedPerson;

        /// <summary>
        ///     Whether the person is already selected.
        /// </summary>
        private static bool _isAlreadySelected;

        /// <summary>
        ///     Whether there has been a retrieval error.
        /// </summary>
        private static bool _retrievalError;

        /// <summary>
        ///     Tries to select a person. Will return null if already selected
        ///     by another player.
        /// </summary>
        /// <param name="entityId">The ped entity Id.</param>
        /// <returns>The person.</returns>
        public static async Task<Person> TrySelectPerson(int entityId)
        {
            var person = await GetPerson(entityId);

            return _isAlreadySelected ? null : person;
        }

        /// <summary>
        ///     Gets a person by their name.
        /// </summary>
        /// <param name="name">The person name.</param>
        /// <returns>The person.</returns>
        public static async Task<Person> GetPerson(string name)
        {
            TriggerServerEvent(ServerEvents.GET_PERSON_BY_NAME, name);

            var count = 0;
            while (_retrievedPerson == null ||
                   !_retrievedPerson.FullName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                await Delay(100);

                if (_retrievalError)
                    return null;

                if (count >= 10000) return null;
                count++;
            }

            _retrievalError = false;
            _isAlreadySelected = false;


            // var person = await ServerRequester.Request<Person>(ServerEvents.GET_PERSON_BY_NAME, name);

            return _retrievedPerson;
        }

        /// <summary>
        ///     Gets a person by their entity id.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="personType">The person type (used for gen if the person doesn't exist)</param>
        /// <returns>The person.</returns>
        public static async Task<Person> GetPerson(int entityId, PersonType personType = PersonType.Random)
        {
            if (!API.DoesEntityExist(entityId))
            {
                Debug.WriteLine($"[PersonRetriever] Invalid entity ID: {entityId}");
                return null;
            }

            var networkId = API.NetworkGetNetworkIdFromEntity(entityId);

            _isAlreadySelected = false;

            if (_retrievedPerson != null && _retrievedPerson.NetworkId == networkId)
                return _retrievedPerson;

            var gender = API.IsPedMale(entityId);

            switch (personType)
            {
                case PersonType.Legal:
                    TriggerServerEvent(ServerEvents.GET_LEGAL_PERSON, networkId, gender);
                    break;

                case PersonType.Random:
                    TriggerServerEvent(ServerEvents.GET_RANDOM_PERSON, networkId, gender);
                    break;

                default:
                    TriggerServerEvent(ServerEvents.GET_RANDOM_PERSON, networkId, gender);
                    break;
            }

            var count = 0;
            while (_retrievedPerson == null || _retrievedPerson.NetworkId != networkId)
            {
                await Delay(100);

                if (count >= 10000) return null;
                count++;
            }

            if (personType == PersonType.Random)
            {
                if (_retrievedPerson.AlcoholLevel >= 0.04f)
                {
                    await _retrievedPerson.Ped().SetDrunkAsync();
                    API.SetDriveTaskDrivingStyle(_retrievedPerson.Ped().Handle, 262663);
                    API.SetPedConfigFlag(_retrievedPerson.Ped().Handle, 100, true); // 100: PED_FLAG_DRUNK
                }

                if (_retrievedPerson.HasWeapon)
                    API.GiveWeaponToPed(_retrievedPerson.EntityId(), (uint)API.GetHashKey(_retrievedPerson.Weapon), 1,
                        true, false);
            }

            return _retrievedPerson;
        }

        /// <summary>
        ///     Receives a person from the server.
        /// </summary>
        /// <param name="input">The person dynamic object.</param>
        /// <param name="isAlreadySelected">Whether the person is already selected by another player.</param>
        [EventHandler(ClientEvents.RECEIVE_PERSON)]
        private void ReceivePerson(dynamic input, bool isAlreadySelected)
        {
            foreach (KeyValuePair<string, dynamic> item in input)
            {
                _retrievedPerson = PersonFactory.FromDynamic(item.Value);
                if (_retrievedPerson == null)
                    this.SendChatMessage("Error receiving the person. Please contact the admin.");
                _isAlreadySelected = isAlreadySelected;
                return;
            }
        }

        /// <summary>
        ///     Handles the event to receive an retrieval error from the server.
        /// </summary>
        [EventHandler(ClientEvents.RECEIVE_PERSON_ERROR)]
        private void ReceivePersonError()
        {
            _retrievalError = true;
        }
    }

    /// <summary>
    ///     The person type.
    /// </summary>
    public enum PersonType
    {
        Random,
        Legal
    }
}