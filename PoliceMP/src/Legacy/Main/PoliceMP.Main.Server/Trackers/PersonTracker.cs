using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Factories;
using PoliceMP.Main.Shared.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PoliceMP.Main.Server.Trackers
{
    /// <summary>
    ///     Tracks all the Person objects.
    /// </summary>
    public class PersonTracker : BaseScript
    {
        /// <summary>
        ///     The list of all the people that have been created.
        /// </summary>
        private static readonly List<Person> _people = new List<Person>();

        /// <summary>
        ///     The list of all the people that are currently selected by the players.
        /// </summary>
        private static readonly List<int> _selectedPeople = new List<int>();

        /// <summary>
        ///     Adds a Person to the list.
        /// </summary>
        /// <param name="person">The person to add.</param>
        public static void AddPerson(Person person)
        {
            if (!_people.Contains(person))
                _people.Add(person);
        }

        /// <summary>
        ///     Gets a Person from the list.
        /// </summary>
        /// <param name="networkId">The person's network ID.</param>
        /// <returns>The Person or null.</returns>
        public static Person GetPerson(int networkId)
        {
            return _people.FirstOrDefault(p => p.NetworkId == networkId);
        }

        /// <summary>
        ///     Handles the event to get a person by their name.
        /// </summary>
        /// <param name="player">The source player.</param>
        /// <param name="name">The person name.</param>
        [EventHandler(ServerEvents.GET_PERSON_BY_NAME)]
        private void GetPersonByName([FromSource] Player player, string name)
        {
            var person = _people.FirstOrDefault(p => p.FullName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (person == null)
                SendRetrievalErrorToClient(player);
            else
                SendPersonToClient(player, person, false);


            var json = JsonConvert.SerializeObject(person);
            player.TriggerEvent(ClientEvents.RECEIVE_SERVER_REQUEST, json);
        }

        [EventHandler(ServerEvents.ADD_PERSON_MANUAL)]
        private void AddPersonManual(int networkId,
            string firstName,
            string lastName,
            long dateOfBirthBinary,
            float alcoholLevel,
            bool onCocaine,
            bool onCannabis,
            bool onHeroin,
            bool onEcstasy,
            bool hasDrivingLicense,
            bool isBannedFromDriving,
            int drivingLicensePoints,
            bool wearsSeatbelt,
            List<dynamic> items,
            List<dynamic> warrants,
            List<dynamic> charges,
            List<dynamic> markers)
        {
            var dateOfBirth = DateTime.FromBinary(dateOfBirthBinary);
            var person = PersonFactory.Custom(networkId,
                firstName,
                lastName,
                dateOfBirth,
                alcoholLevel,
                onCocaine,
                onCannabis,
                onHeroin,
                onEcstasy,
                hasDrivingLicense,
                isBannedFromDriving,
                drivingLicensePoints,
                wearsSeatbelt,
                items.Cast<string>().ToArray(),
                warrants.Cast<string>().ToArray(),
                charges.Cast<string>().ToArray(),
                markers.Cast<string>().ToArray());

            _people.Add(person);
        }

        /// <summary>
        ///     Gets a random person. This will create one if one doesn't
        ///     exist for the specified network ID.
        /// </summary>
        /// <param name="player">The source player.</param>
        /// <param name="networkId">The ped network ID.</param>
        /// <param name="gender">The ped gender.</param>
        [EventHandler(ServerEvents.GET_RANDOM_PERSON)]
        private void GetRandomPerson([FromSource] Player player, int networkId, bool gender)
        {
            var person = _people.FirstOrDefault(p => p.NetworkId == networkId);
            if (person == null)
            {
                person = PersonFactory.Random(networkId, gender);
                _people.Add(person);
            }

            SendPersonToClient(player, person);
        }

        /// <summary>
        ///     Gets a person that is fully legal ie no drugs or items.
        ///     This is used for peds like cops.
        /// </summary>
        /// <param name="player">The source player.</param>
        /// <param name="networkId">The ped network ID.</param>
        /// <param name="gender">The ped gender.</param>
        [EventHandler(ServerEvents.GET_LEGAL_PERSON)]
        private void GetLegalPerson([FromSource] Player player, int networkId, bool gender)
        {
            var person = _people.FirstOrDefault(p => p.NetworkId == networkId);
            if (person == null)
            {
                person = PersonFactory.Legal(networkId, gender);
                _people.Add(person);
            }

            SendPersonToClient(player, person);
        }

        /// <summary>
        ///     Handles the client event when a person is unselected so it can
        ///     be removed from the selected people list.
        /// </summary>
        /// <param name="player">The source player.</param>
        /// <param name="networkId">The ped network ID.</param>
        [EventHandler(ServerEvents.ON_PERSON_UNSELECTED)]
        private void OnPersonUnselected([FromSource] Player player, int networkId)
        {
            if (_selectedPeople.Contains(networkId))
                _selectedPeople.Remove(networkId);
        }

        /// <summary>
        ///     Sends an error to the client when retrieval failed.
        /// </summary>
        /// <param name="player">The player client.</param>
        private void SendRetrievalErrorToClient(Player player)
        {
            player.TriggerEvent(ClientEvents.RECEIVE_PERSON_ERROR);
        }

        /// <summary>
        ///     Sends a person to the client.
        /// </summary>
        /// <param name="player">The player client.</param>
        /// <param name="person">The person.</param>
        /// <param name="isSelecting">Whether the player is selecting the person.</param>
        private void SendPersonToClient(Player player, Person person, bool isSelecting = true)
        {
            var isAlreadySelected = false;

            if (_selectedPeople.Contains(person.NetworkId))
                isAlreadySelected = true;
            else if (isSelecting)
                _selectedPeople.Add(person.NetworkId);

            var dynPerson = PersonFactory.ToDynamic(person);
            var dict = new ExpandoObject() as IDictionary<string, dynamic>;

            dict.Add("1", dynPerson);
            player.TriggerEvent(ClientEvents.RECEIVE_PERSON, dict, isAlreadySelected);
        }
    }
}