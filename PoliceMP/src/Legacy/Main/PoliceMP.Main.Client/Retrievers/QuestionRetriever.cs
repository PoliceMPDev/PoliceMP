using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Factories;
using PoliceMP.Main.Shared.Models;
using System.Collections.Generic;

namespace PoliceMP.Main.Client.Retrievers
{
    /// <summary>
    ///     Retrieves all of the questions from the server.
    /// </summary>
    public class QuestionRetriever : BaseScript
    {
        /// <summary>
        ///     The list of questions.
        /// </summary>
        private static List<Question> _questions;

        /// <summary>
        ///     Gets all of the questions.
        /// </summary>
        /// <returns>A list of all questions.</returns>
        public static List<Question> GetAll()
        {
            if (_questions == null || _questions.Count == 0)
                InitialLoad();

            return _questions;
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
        ///     Loads the questions from the server.
        /// </summary>
        private static void InitialLoad()
        {
            if (_questions != null) return;

            TriggerServerEvent(ServerEvents.GET_ALL_QUESTIONS);
        }

        /// <summary>
        ///     Receives all the questions from the server.
        /// </summary>
        /// <param name="dynQuestions">The dynamic list containing all the questions.</param>
        [EventHandler(ClientEvents.RECEIVE_ALL_QUESTIONS)]
        private void ReceiveAllOffences(IEnumerable<dynamic> dynQuestions)
        {
            _questions = new List<Question>();
            foreach (var dynQuestion in dynQuestions)
                _questions.Add(QuestionFactory.FromDynamic(dynQuestion));
        }
    }
}