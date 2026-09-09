using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Extensions;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Shared.Constants;
using PoliceMP.Client.Overlays.NewNotification;
using System.Collections.Generic;

namespace PoliceMP.Client.Actions.Drugalyse
{
    public class DrugalyseHandler : ActionHandler<Drugalyse>
	{
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly IAnimationService _anims;
        private readonly ISoundService _sounds;
        private readonly IPedInfoService _pedInfo;
        private readonly ISpeechService _speech;

        public DrugalyseHandler(INewNotificationOverlay newNotificationOverlay,
            IAnimationService anims,
            ISoundService sounds,
            IPedInfoService pedInfo,
            ISpeechService speech)
		{
			_newNotificationOverlay = newNotificationOverlay;
			_anims = anims;
            _sounds = sounds;
            _pedInfo = pedInfo;
            _speech = speech;
        }

        protected override async Task<bool> Handle(Drugalyse action)
        {
            if (!action.Subject.IsNearEntity(action.Target, new Vector3(2f, 2f, 2f)))
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Drugalyser", "error", "You are not close enough to the ped.", new NewNotificationMessageContent[0]));
				return false;
            }

            if (action.Target.IsInVehicle())
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Drugalyser", "error", "You cannot drugalyse someone who is in a vehicle.", new NewNotificationMessageContent[0]));
                return false;
            }

            await DrugalysePed(action);
            return true;
        }

        private async Task DrugalysePed(Drugalyse action)
        {
            action.Target.Task.TurnTo(action.Subject);
            action.Subject.Task.TurnTo(action.Target);
            action.Subject.IsPositionFrozen = true;
            action.Target.IsPositionFrozen = true;

            _speech.Say(Game.PlayerPed, "I'm going to give you a quick drug test.");

            await Delay(1000);

            _speech.Do(action.Target, "Takes drug test.");
            _sounds.Play(Sounds.Inhaler);
            await _anims.Breathalyser(action.Subject);

            var pedInfo = await _pedInfo.GetByNetworkId(action.Target.NetworkId);
			List<NewNotificationMessageContent> messageContentList = new List<NewNotificationMessageContent>();
			messageContentList.Add(new NewNotificationMessageContent("Cocaine", pedInfo.IsOnCocaine ? "Positive" : "Negative"));
			messageContentList.Add(new NewNotificationMessageContent("Cannabis", pedInfo.IsOnCannabis ? "Positive" : "Negative"));

            if (pedInfo.IsOnCocaine || pedInfo.IsOnCannabis)
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Drugalyser", "warning", string.Empty, messageContentList.ToArray()));
			}
            else
            {
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Drugalyser", "success", string.Empty, messageContentList.ToArray()));
			}


            action.Subject.Task.ClearAll();
            action.Subject.IsPositionFrozen = false;
            action.Target.IsPositionFrozen = false;

            await action.Target.StandStillFacingPlayer();
        }
    }
}