using System.Linq;
using System.Text;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants.States;
using System.Threading.Tasks;
using PoliceMP.Client.Overlays.NewNotification;
using System.Collections.Generic;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Actions.RunName
{
    public class RunNameHandler : ActionHandler<RunName>
    {
        private readonly IPedInfoService _pedInfoService;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly ISpeechService _speech;

        public RunNameHandler(IPedInfoService pedInfoService,
			INewNotificationOverlay newNotificationOverlay,
            ISpeechService speech)
        {
            _pedInfoService = pedInfoService;
			_newNotificationOverlay = newNotificationOverlay;
			_speech = speech;
        }

        protected override async Task<bool> Handle(RunName action)
        {
            string name;
            if (string.IsNullOrEmpty(action.Name))
            {
                string defaultText = Game.Player.State.Get<string>(PlayerStates.LastNameFromAskForId).Replace("\"", "");
                name = await Game.GetUserInput(WindowTitle.PM_NAME_CHALL, defaultText, 40);
                if (string.IsNullOrEmpty(name))
                {
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Police Database", "error", "You must enter a name.", new NewNotificationMessageContent[0]));
					return false;
                }
            }
            else
            {
                name = action.Name;
            }

            _speech.Say(Game.PlayerPed, $"Control, can I get a check for '{name}' please?");
            await Delay(2000);

            var pedInfo = await _pedInfoService.GetByName(name);
            if (pedInfo == null || pedInfo.NetworkId < 1)
			{
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Police Database", "error", $"There was no match for a person named: {name}", new NewNotificationMessageContent[0]));
                return true;
            }
			List<NewNotificationMessageContent> messageContentList = new List<NewNotificationMessageContent>();

			messageContentList.Add(new NewNotificationMessageContent("Name", pedInfo.FullName));
			messageContentList.Add(new NewNotificationMessageContent("DoB", pedInfo.DateOfBirth.ToString("dd'/'MM'/'yyyy")));
			messageContentList.Add(new NewNotificationMessageContent("Address", pedInfo.Street));
			messageContentList.Add(new NewNotificationMessageContent("Driving License", pedInfo.HasDrivingLicense ? "Valid" : "None"));


            if (pedInfo.HasDrivingLicense)
				messageContentList.Add(new NewNotificationMessageContent("Points", pedInfo.DrivingLicensePoints.ToString()));


            if (pedInfo.IsBannedFromDriving)
				messageContentList.Add(new NewNotificationMessageContent("Banned from driving", "Yes"));

            if (pedInfo.Warrants.Any())
			{
				messageContentList.Add(new NewNotificationMessageContent("Warrants", string.Join(", ", pedInfo.Warrants)));
            }

            if (pedInfo.CriminalMarkers.Any())
			{
				messageContentList.Add(new NewNotificationMessageContent("Markers", string.Join(", ", pedInfo.CriminalMarkers)));
            }

            if (pedInfo.Charges.Any())
			{
				messageContentList.Add(new NewNotificationMessageContent("Previous Convictions", string.Join(", ", pedInfo.Charges)));
            }
			_newNotificationOverlay.SendNotification(new NewNotificationMessage("Police Database", "info", string.Empty, messageContentList.ToArray()));

            return true;
        }
    }
}