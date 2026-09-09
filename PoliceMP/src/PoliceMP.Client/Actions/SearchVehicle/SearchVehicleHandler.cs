using System.Linq;
using System.Text;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants;
using PoliceMP.Client.Overlays.NewNotification;
using System.Collections.Generic;
using CitizenFX.Core.Native;
using System;
using PoliceMP.Client.Scripts.CivVehicleContents;

namespace PoliceMP.Client.Actions.SearchVehicle
{
    public class SearchVehicleHandler : ActionHandler<SearchVehicle>
    {
        private readonly ISoundService _sounds;
        private readonly ISpeechService _speech;
        private readonly IVehicleInfoService _vehicleInfoService;
		private readonly INewNotificationOverlay _newNotificationOverlay;

        public SearchVehicleHandler(ISoundService sounds,
            ISpeechService speech, 
            IVehicleInfoService vehicleInfoService,
			INewNotificationOverlay newNotificationOverlay)
        {
            _sounds = sounds;
            _speech = speech;
            _vehicleInfoService = vehicleInfoService;
			_newNotificationOverlay = newNotificationOverlay;
		}

        protected override async Task<bool> Handle(SearchVehicle action)
        {
            if (action.Target.ClassType == VehicleClass.Emergency) return false;

            if (action.Target.LockStatus == VehicleLockStatus.CanBeBrokenInto)
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Search", "error", "The vehicle is locked. You need to break into it first.", new NewNotificationMessageContent[0]));
				return false;
            }

            if (action.Target.Occupants.Length > 0)
			{
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Search", "error", "You should ask all occupants to leave the vehicle first.", new NewNotificationMessageContent[0]));
                return false;
            }

            foreach (var vehicleDoor in action.Target.Doors)
            {
                vehicleDoor.Open();
            }

            _speech.Do(Game.PlayerPed, $"Searches the {action.Target.LocalizedName}.");

            Game.PlayerPed.IsPositionFrozen = true;
            action.Target.IsDriveable = false;

            _sounds.Play(Sounds.SearchCar);
            Game.PlayerPed.Task.PlayAnimation("missexile3", "ex03_dingy_search_case_base_michael");

            int ownerNetworkId = action.Target.Driver == null ? -1 : action.Target.Driver.Handle;
            var vehicleInfo = await _vehicleInfoService.GetByNetworkId(
                action.Target.NetworkId,
                action.Target.GetPlateText(),
                ownerNetworkId);
            await Delay(5000);

            Game.PlayerPed.IsPositionFrozen = false;
            action.Target.IsDriveable = true;

            foreach (var vehicleDoor in action.Target.Doors)
            {
                vehicleDoor.Close();
            }

            if (API.DecorExistOn(action.Target.Handle, "PoliceMP_Vehicle_Inventory1") || API.DecorExistOn(action.Target.Handle, "PoliceMP_Vehicle_Inventory2") || API.DecorExistOn(action.Target.Handle, "PoliceMP_Vehicle_Inventory3") || API.DecorExistOn(action.Target.Handle, "PoliceMP_Vehicle_Inventory4"))
            {

                var encodedValueSet1 = API.DecorGetInt(action.Target.Handle, "PoliceMP_Vehicle_Inventory1");
                var encodedValueSet2 = API.DecorGetInt(action.Target.Handle, "PoliceMP_Vehicle_Inventory2");
                var encodedValueSet3 = API.DecorGetInt(action.Target.Handle, "PoliceMP_Vehicle_Inventory3");
                var encodedValueSet4 = API.DecorGetInt(action.Target.Handle, "PoliceMP_Vehicle_Inventory4");

                var skip = false;
                if (encodedValueSet1 == 0 && encodedValueSet2 == 0 && encodedValueSet3 == 0 && encodedValueSet4 == 0) skip = true;

                if (!skip)
                {
                    List<string> decodedItems = new List<string>();

                    foreach (var vehicleItem in CivSearchDictionaries.vehicleSearchPossibleItems)
                    {
                        if ((encodedValueSet1 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in CivSearchDictionaries.vehicleSearchPossibleItems2)
                    {
                        if ((encodedValueSet2 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in CivSearchDictionaries.vehicleSearchPossibleItems3)
                    {
                        if ((encodedValueSet4 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in CivSearchDictionaries.vehicleSearchPossibleItemsSenior)
                    {
                        if ((encodedValueSet3 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    var decodedString = String.Join(", ", decodedItems);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Search", "success", $"Items Found: {decodedString}!", new NewNotificationMessageContent[0]));
                    return true;
                }
            }

            if (vehicleInfo == null || !vehicleInfo.Items.Any())
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Search", "success", "Found nothing of interest.", new NewNotificationMessageContent[0]));
				return true;
            }

            var builder = new StringBuilder(); 
            List<NewNotificationMessageContent> messageContentList = new List<NewNotificationMessageContent>();

            if (vehicleInfo.HasIllegalItems)
			{
				messageContentList.Add(new NewNotificationMessageContent("Outcome: ", "Illegal items found"));
				messageContentList.Add(new NewNotificationMessageContent("Items found: ", string.Join(", ", vehicleInfo.Items.Select(item => item.Name))));
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle search", "warning", string.Empty, messageContentList.ToArray()));
			}
            else
			{
				messageContentList.Add(new NewNotificationMessageContent("Outcome: ", "No illegal items found"));
				messageContentList.Add(new NewNotificationMessageContent("Items found: ", string.Join(", ", vehicleInfo.Items.Select(item => item.Name))));
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle search", "success", string.Empty, messageContentList.ToArray()));
			}
            return true;
        }
    }
}