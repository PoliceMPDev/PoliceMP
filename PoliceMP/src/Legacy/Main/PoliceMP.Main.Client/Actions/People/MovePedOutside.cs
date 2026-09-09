using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Client.Extensions;
using PoliceMP.Main.Client.Managers;
using PoliceMP.Main.Client.Util;

namespace PoliceMP.Main.Client.Actions.People
{
    class MovePedOutside : BaseScript
    {

        [Command(Commands.MovePedOutside)]
        private static void MoveOutside()
        {
            Debug.WriteLine("MoveOutside");
            TriggerEvent("PoliceMPHousingClient:RequestPedExtractionLocation");
        }

        [EventHandler("PoliceMP:ExtractPedOutsideHouse")]
        private static void ExtractPedOutsideHouse(int x, int y, int z)
        {
            Debug.WriteLine("ExtractPedOutside");
            API.SetEntityCoords(PersonSelector.SelectedPerson.EntityId(), x, y, z, false, false, false, true);
        }
    }
}
