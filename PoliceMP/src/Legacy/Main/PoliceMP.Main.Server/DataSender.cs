using CitizenFX.Core;
using PoliceMP.Main.Shared.Events;
using PoliceMP.Main.Shared.Readers;

namespace PoliceMP.Main.Server
{
    /// <summary>
    ///     This class sends the data in JSON files to the client
    ///     seeing as the client can't load the files itself
    /// </summary>
    public class DataSender : BaseScript
    {
        /// <summary>
        ///     Triggered by the client so they can receive all the offences.
        /// </summary>
        /// <param name="player">The source player.</param>
        [EventHandler(ServerEvents.GET_ALL_OFFENCES)]
        private void GetAllOffences([FromSource] Player player)
        {
            var offences = OffenceReader.All();
            player.TriggerEvent(ClientEvents.RECEIVE_ALL_OFFENCES, offences);
        }

        /// <summary>
        ///     Triggered by the client so they can receive all the questions.
        /// </summary>
        /// <param name="player">The source player.</param>
        [EventHandler(ServerEvents.GET_ALL_QUESTIONS)]
        private void GetAllQuestions([FromSource] Player player)
        {
            var questions = QuestionReader.All();
            player.TriggerEvent(ClientEvents.RECEIVE_ALL_QUESTIONS, questions);
        }
    }
}