using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants.States;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Shared.Extensions;

namespace PoliceMP.Client.Actions.RunPlate
{
    public class RunPlateHandler : ActionHandler<RunPlate>
    {
        private readonly IVehicleInfoService _vehicleInfoService;
        private readonly INotificationService _notifications;
        private readonly ISpeechService _speech;

        public RunPlateHandler(IVehicleInfoService vehicleInfoService,
            INotificationService notifications,
            ISpeechService speech)
        {
            _vehicleInfoService = vehicleInfoService;
            _notifications = notifications;
            _speech = speech;
        }

        protected override async Task<bool> Handle(RunPlate action)
        {
            string plate;
            if (string.IsNullOrEmpty(action.Plate))
            {
                string defaultText = Game.Player.State.Get<string>(PlayerStates.LastPlateFromPullover);
                defaultText = defaultText?.Replace("\"", "");
                plate = await Game.GetUserInput(WindowTitle.PM_NAME_CHALL, defaultText, 40);
                if (string.IsNullOrEmpty(plate))
                {
                    _notifications.Error("Police Database", "You must enter a plate.");
                    return false;
                }
            }
            else
            {
                plate = action.Plate;
            }

            _speech.Say(Game.PlayerPed, $"Control, can I get a check for '{plate}' please?");
            await Delay(2000);

            var vehicle = World.GetAllVehicles()
                .FirstOrDefault(x => x.GetPlateText().EqualsIgnoreCase(plate));
            if (vehicle == null)
            {
                _notifications.Error("Police Database", $"No vehicle could be found with plate " +
                                                        $"<span class='text-warning'>{plate}</span>.");
                return true;
            }

            var vehicleInfo = await _vehicleInfoService.GetByNetworkId(vehicle.NetworkId,
                plate,
                vehicle.GetDriverPedNetworkId());
            if (vehicleInfo == null || vehicleInfo.NetworkId < 1)
            {
                _notifications.Error("Police Database", $"No vehicle could be found with plate " +
                                                       $"<span class='text-warning'>{plate}</span>.");
                return true;
            }

            var builder = new StringBuilder();

            builder.Append($"Match found for <span class='text-warning'>{plate.ToUpper()}</span><br><br>");

            builder.Append($"<b>VIN:</b> {vehicleInfo.VIN}<br>");
            builder.Append($"<b>Model:</b> {API.GetLabelText(vehicle.DisplayName)}<br>");
            builder.Append($"<b>Class:</b> {vehicle.ClassType}<br>");
            builder.Append($"<b>Owner:</b> {vehicleInfo.OwnerName}<br>");

            builder.Append(!vehicleInfo.IsMotExpired
                ? $"<b>MOT:</b> <span class='text-success'>Valid</span> <small>({vehicleInfo.MotExpiryDate:dd-MMM-yy})</small><br>"
                : $"<b>MOT:</b> <span class='text-danger'>Expired</span> <small>({vehicleInfo.MotExpiryDate:dd-MMM-yy})</small><br>");

            builder.Append(!vehicleInfo.IsTaxExpired
                ? $"<b>Tax:</b> <span class='text-success'>Valid</span> <small>({vehicleInfo.TaxExpiryDate:dd-MMM-yy})</small><br>"
                : $"<b>Tax:</b> <span class='text-danger'>Expired</span> <small>({vehicleInfo.TaxExpiryDate:dd-MMM-yy})</small><br>");

            builder.Append(!vehicleInfo.IsInsuranceExpired
                ? $"<b>Insurance:</b> <span class='text-success'>Valid</span> <small>({vehicleInfo.InsuranceExpiryDate:dd-MMM-yy})</small><br>"
                : $"<b>Insurance:</b> <span class='text-danger'>Expired</span> <small>({vehicleInfo.InsuranceExpiryDate:dd-MMM-yy})</small><br>");


            if (vehicleInfo.Markers.Any())
            {
                builder.Append("<br><b>Markers:</b><br>");
                foreach (string marker in vehicleInfo.Markers)
                    builder.Append($"- <span class='text-danger'>{marker}</span></span><br>");
            }

            _notifications.Info("Police Database", builder.ToString());
            return true;
        }
    }
}