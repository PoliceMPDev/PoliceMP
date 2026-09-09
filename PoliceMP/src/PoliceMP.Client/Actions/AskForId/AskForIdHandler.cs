using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using System.Collections.Generic;
using System.Threading.Tasks;
using PoliceMP.Client.Overlays.Legacy.IdCardOverlay;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Actions.AskForId
{
    public class AskForIdHandler : ActionHandler<AskForId>
    {
        private readonly IPedInfoService _pedInfo;
        private readonly INotificationService _notifications;
        private readonly IAnimationService _anims;
        private readonly IdCardOverlay _idCardOverlay;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ISpeechService _speech;

        public AskForIdHandler(IPedInfoService pedInfo,
            INotificationService notifications,
            IAnimationService anims,
            IdCardOverlay idCardOverlay,
            ILegacyClientCommunicationsManager comms,
            ISpeechService speech)
        {
            _pedInfo = pedInfo;
            _notifications = notifications;
            _anims = anims;
            _idCardOverlay = idCardOverlay;
            _comms = comms;
            _speech = speech;
        }

        protected override async Task<bool> Handle(AskForId action)
        {
            var pedInfo = await _pedInfo.GetByNetworkId(action.Target.NetworkId);

            if (!action.Subject.IsNearEntity(action.Target, new Vector3(2f, 2f, 2f)))
            {
                _notifications.Error("Ask for ID", "You are not close enough to the ped.");
                return false;
            }

            Game.Player.State.Set<string>(PlayerStates.LastNameFromAskForId, pedInfo.FullName);

            action.Subject.Task.TurnTo(action.Target);

            _speech.Say(Game.PlayerPed, "Have you got any ID on you?");

            await _anims.HowYouDoing(action.Subject);

            if (!action.Target.IsInVehicle())
            {
                await Script.Delay(1000);
                action.Target.Task.TurnTo(action.Subject);
            }

            await Delay(1000);

            _speech.Do(action.Target, $"Shows identification to {Game.Player.Name}.");
            await _anims.ShowId(action.Target);

            await Delay(1000);

            var drivingLicenseTypes = new List<string>();
            if (pedInfo.HasDrivingLicense) drivingLicenseTypes.Add("Car");

            _idCardOverlay.Show(new IdCard
            {
                FirstName = pedInfo.FirstName,
                LastName = pedInfo.LastName,
                DateOfBirth = pedInfo.DateOfBirth.ToString("dd/MM/yyyy"),
                Height = pedInfo.Height,
                IsMale = true,
                Type = pedInfo.HasDrivingLicense ? IdCardType.Driver : IdCardType.None,
                DrivingLicenseTypes = drivingLicenseTypes
            });

            action.Subject.Task.ClearAll();
            action.Target.SetIsNameKnown(true);
            _comms.ToClient(ClientEvents.LearnedName, action.Target.NetworkId);

            await Delay(10000);

            _idCardOverlay.Hide();

            if (!action.Target.IsInVehicle()) await action.Target.StandStillFacingPlayer();

            return true;
        }
    }
}