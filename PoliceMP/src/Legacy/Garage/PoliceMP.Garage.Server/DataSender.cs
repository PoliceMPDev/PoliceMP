using CitizenFX.Core;
using PoliceMP.Garage.Shared;

namespace PoliceMP.Garage.Server
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
        [EventHandler(ServerEvents.GET_ALL_CARDATA)]
        private void GetAllCarData([FromSource] Player player)
        {
            var carData = CarReader.All();
            player.TriggerEvent(ClientEvents.RECEIVE_ALL_CARDATA, carData);
        }
    }
}