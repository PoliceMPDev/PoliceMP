using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;
using PoliceMP.Shared.NetworkMessages.XPSystem.Notifications;
namespace PoliceMP.Client.Scripts.XPSystem
{
    public class XPSystem : Script
    {
        private const string cacheString = "XPSystem";
        
        private readonly ILogger<XPSystem> _logger;
        private readonly INewNotificationOverlay _newNotificationOverlay;

        public XPSystem(ILogger<XPSystem> logger, IClientCommunicationsManager comms, INewNotificationOverlay newNotificationOverlay)
        {
            _logger = logger;
            _newNotificationOverlay = newNotificationOverlay;

            comms.AddNotificationHandler<SetXPEvent>(SetXP);
            comms.AddNotificationHandler<XPIncreasedEvent>(IncreaseXP);
            comms.AddNotificationHandler<XPDecreasedEvent>(DecreaseXP);
        }

        public Task SetXP(SetXPEvent @event)
        {
            BaseScript.TriggerEvent("XNL_NET:XNL_SetInitialXPLevels", @event.XpValue, false, false);
            
            return Task.FromResult(0);
        }

        public Task IncreaseXP(XPIncreasedEvent @event)
        {
            _logger.Debug($"XP increase by {@event.XpIncrease}");
            
            // Tell XP bar about increase
            BaseScript.TriggerEvent("XNL_NET:AddPlayerXP", @event.XpIncrease);
            
            // Show on screen notification with reason why
            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                $"Gained {@event.XpIncrease} XP", "success",
                $"You have gained {@event.XpIncrease} XP for {@event.Reason}",
                new NewNotificationMessageContent[0]));

            return Task.FromResult(0);
        }

        public Task DecreaseXP(XPDecreasedEvent @event)
        {
            _logger.Debug($"XP decrease by {@event.XpDecrease}");
            
            // Tell XP bar about decrease
            BaseScript.TriggerEvent("XNL_NET:RemovePlayerXP", @event.XpDecrease);
            
            // Show on screen notification with reason why
            _newNotificationOverlay.SendNotification(new NewNotificationMessage(
                $"Lost {@event.XpDecrease} XP", "error",
                $"You have lost {@event.XpDecrease} XP for {@event.Reason}",
                new NewNotificationMessageContent[0]));
            
            return Task.FromResult(0);
        }

        protected int GetCurrentXPValue()
        {
            return API.GetResourceKvpInt(cacheString);
        }

        protected void SetCurrentXPValue(int xpValue)
        {
            API.SetResourceKvpInt(cacheString, xpValue);
        }
    }
}