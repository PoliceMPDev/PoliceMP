using System.Drawing.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.TwitterChatScript
{
    public class TwitterChatScript : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<TwitterChatScript> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommandManager _commands;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly ITickManager _ticks;

        private UserAces _userAces;

        public TwitterChatScript(ILogger<TwitterChatScript> logger, ITickManager ticks, IPermissionService permissionService, ICommandManager commands, ILegacyClientCommunicationsManager comms, INewNotificationOverlay newNotificationOverlay)
        {
            _logger = logger;
            _permissionService = permissionService;
            _commands = commands;
            _comms = comms;
			_newNotificationOverlay = newNotificationOverlay;
			_ticks = ticks;
        }

        protected override async Task OnStartAsync()
        {
            _userAces = await _permissionService.GetUserAces();

            _commands.Register("intel").HasGreedyArgs().WithHandler((message) =>
            {
                var currentUserRole = _permissionService.CurrentUserRole;

                if (currentUserRole.Branch != UserBranch.Civ && currentUserRole.Division != UserDivision.Cid && !_userAces.IsDeveloper && !_userAces.IsAdmin)
                {
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Intel", "error", "Intel can only be used by Civilians!", new NewNotificationMessageContent[0]));
					return;
                }

                if (string.IsNullOrEmpty(message) || message == "" || message == string.Empty)
                {
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Intel", "error", "Command Error - /intel [Intel Message]", new NewNotificationMessageContent[0]));
					return;
                }
                _comms.ToServer(ServerEvents.SendTweetCommandToServer, message);
				_newNotificationOverlay.SendNotification(new NewNotificationMessage("Intel", "success", "Your intel has been posted!", new NewNotificationMessageContent[0]));
			});
        }
    }
}