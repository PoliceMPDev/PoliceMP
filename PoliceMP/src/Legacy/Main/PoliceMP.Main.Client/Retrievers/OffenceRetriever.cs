using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Factories;
using PoliceMP.Main.Shared.Models;
using System.Collections.Generic;

namespace PoliceMP.Main.Client.Retrievers
{
    /// <summary>
    ///     Retrieves all of the offences from the server.
    /// </summary>
    public class OffenceRetriever : BaseScript
    {
        /// <summary>
        ///     The list of offences.
        /// </summary>
        private static List<Offence> _offences;

        /// <summary>
        ///     Gets all of the offences.
        /// </summary>
        /// <returns>A list of all offences.</returns>
        public static List<Offence> GetAll()
        {
            if (_offences == null || _offences.Count == 0)
                InitialLoad();

            return _offences;
        }

        /// <summary>
        ///     Called when the resource is started, used to trigger the
        ///     initial loading from the server.
        /// </summary>
        /// <param name="resourceName">The name of the resource that was started.</param>
        [EventHandler("onClientResourceStart")]
        private void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            InitialLoad();
        }

        /// <summary>
        ///     Loads the offences from the server.
        /// </summary>
        private static void InitialLoad()
        {
            if (_offences != null) return;

            TriggerServerEvent(ServerEvents.GET_ALL_OFFENCES);
        }

        /// <summary>
        ///     Receives all the offences from the server.
        /// </summary>
        /// <param name="dynOffences">The dynamic list containing all the offences.</param>
        [EventHandler(ClientEvents.RECEIVE_ALL_OFFENCES)]
        private void ReceiveAllOffences(IEnumerable<dynamic> dynOffences)
        {
            _offences = new List<Offence>();
            foreach (var dynOffence in dynOffences)
                _offences.Add(OffenceFactory.FromDynamic(dynOffence));
        }
    }
}