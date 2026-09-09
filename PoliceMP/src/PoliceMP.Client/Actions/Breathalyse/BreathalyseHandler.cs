using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Options.Interfaces;
using PoliceMP.Shared.Constants.States;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Constants;
using PoliceMP.Client.Overlays.NewNotification;
using System.Collections.Generic;

namespace PoliceMP.Client.Actions.Breathalyse
{
    public class BreathalyseHandler : ActionHandler<Breathalyse>
    {
        private readonly ILegacyClientCommunicationsManager _comms;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly IAnimationService _anims;
        private readonly ISoundService _sounds;
        private readonly IPedInfoService _pedInfo;
        private readonly IOptionsManager _optionsManager;
        private readonly ISpeechService _speech;

        public BreathalyseHandler(ILegacyClientCommunicationsManager comms,
			INewNotificationOverlay newNotificationOverlay,
            IAnimationService anims,
            ISoundService sounds,
            IPedInfoService pedInfo,
            IOptionsManager optionsManager,
            ISpeechService speech)
		{
            _comms = comms;
			_newNotificationOverlay = newNotificationOverlay;
			_anims = anims;
            _sounds = sounds;
            _pedInfo = pedInfo;
            _optionsManager = optionsManager;
            _speech = speech;
        }

        protected override async Task<bool> Handle(Breathalyse action)
        {
            if (!action.Subject.IsNearEntity(action.Target, new Vector3(2f, 2f, 2f)))
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Breathalyser", "error", "You are not close enough to the ped.", new NewNotificationMessageContent[0]));
                return false;
            }

            if (action.Target.IsInVehicle())
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Breathalyser", "error", "You cannot breathalyse someone who is in a vehicle.", new NewNotificationMessageContent[0]));
                return false;
            }

            await BreathalysePed(action);
            return true;
        }

        private async Task BreathalysePed(Breathalyse action)
        {
            action.Target.Task.TurnTo(action.Subject);
            action.Subject.Task.TurnTo(action.Target);
            action.Subject.IsPositionFrozen = true;
            action.Target.IsPositionFrozen = true;

            _speech.Say(Game.PlayerPed, "I'm going to give you a quick breathalyser test.");

            await Script.Delay(1000);

            _speech.Do(action.Target, "Takes breathalyser test.");
            _sounds.Play(Sounds.Inhaler);
            await _anims.Breathalyser(action.Subject);
            

            var pedInfo = await _pedInfo.GetByNetworkId(action.Target.NetworkId);
            if (pedInfo == null)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Breathalyser", "error", "Failed to retrieve ped information...", new NewNotificationMessageContent[0]));
                return;
            }
			List<NewNotificationMessageContent> messageContentList = new List<NewNotificationMessageContent>(); 
            messageContentList.Add(new NewNotificationMessageContent("Legal Limit: ", _optionsManager.Options.Action.BloodAlcoholLimit.ToString()));
			var builder = new StringBuilder();

            if (pedInfo.AlcoholLevel > _optionsManager.Options.Action.BloodAlcoholLimit)
			{
				messageContentList.Add(new NewNotificationMessageContent("Reading", pedInfo.AlcoholLevel.ToString()));
				messageContentList.Add(new NewNotificationMessageContent("Result", "FAIL"));
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Breathalyser", "warning", string.Empty, messageContentList.ToArray()));
			}
            else
			{
				messageContentList.Add(new NewNotificationMessageContent("Reading", pedInfo.AlcoholLevel.ToString()));
				messageContentList.Add(new NewNotificationMessageContent("Result", "PASS"));
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Breathalyser", "success", string.Empty, messageContentList.ToArray()));
			}



			action.Subject.Task.ClearAll();
            action.Subject.IsPositionFrozen = false;
            action.Target.IsPositionFrozen = false;

            action.Target.State.Set<bool>(PedStates.NameKnown, true, true);

            await action.Target.StandStillFacingPlayer();
        }
    }
}