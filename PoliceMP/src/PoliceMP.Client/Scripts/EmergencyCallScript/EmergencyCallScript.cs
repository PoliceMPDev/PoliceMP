using System.Drawing.Text;
using System.Linq;
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

namespace PoliceMP.Client.Scripts.EmergencyCallScript
{
    public class EmergencyCallScript : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<EmergencyCallScript> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommandManager _commands;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly ITickManager _ticks;

        public EmergencyCallScript(ILogger<EmergencyCallScript> logger, ITickManager ticks, IPermissionService permissionService, ICommandManager commands, ILegacyClientCommunicationsManager comms, INewNotificationOverlay newNotificationOverlay)
        {
            _logger = logger;
            _permissionService = permissionService;
            _commands = commands;
            _comms = comms;
			_newNotificationOverlay = newNotificationOverlay;
			_ticks = ticks;
        }

        private UserAces _userAces;
        private bool usingForceUpdate = false;

        protected override async Task OnStartAsync()
        {
            _userAces = await _permissionService.GetUserAces();

            // 999 Calls
            _commands.Register("text999").WithHandler(async () =>
            {
                var currentUserRole = _permissionService.CurrentUserRole;

                if (currentUserRole.Branch != UserBranch.Civ && !_userAces.IsDeveloper && !_userAces.IsAdmin)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emergency Call Center", "error", "999 Calls can only be made by Civilians!", new NewNotificationMessageContent[0]));
                    return;
                }

                // Postal input with numeric validation
                string postal;
                while (true)
                {
                    postal = await GetUserInput("Enter Postal Code", "", 10);
                    if (string.IsNullOrWhiteSpace(postal)) return;

                    if (postal.All(char.IsDigit)) break;

                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emergency Call Center", "error", "Postal must contain numbers only.", new NewNotificationMessageContent[0]));
                }

                // Division input
                string division = await GetUserInput("Enter Targeted Division", "", 20);
                if (string.IsNullOrWhiteSpace(division)) return;

                // Message input with 200 char limit
                string message = await GetUserInput("Describe the Emergency", "", 200);
                if (string.IsNullOrWhiteSpace(message))
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emergency Call Center", "error", "Message cannot be empty.", new NewNotificationMessageContent[0]));
                    return;
                }

                // Final formatted message
                string finalMessage = $"📍 {postal} | 🛂 {division} | 💬 {message}";

                _comms.ToServer(ServerEvents.Send999CallToServer, finalMessage);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emergency Call Center", "success", "Your 999 call has been sent!", new NewNotificationMessageContent[0]));
                API.PlaySoundFrontend(-1, "Hang_Up", "Phone_SoundSet_Michael", false);
                await Delay(500);
                API.PlaySoundFrontend(-1, "Hang_Up", "Phone_SoundSet_Michael", false);
            });

            // 101 Calls
            _commands.Register("text101").WithHandler(async () =>
            {
                var currentUserRole = _permissionService.CurrentUserRole;

                if (currentUserRole.Branch != UserBranch.Civ && !_userAces.IsDeveloper && !_userAces.IsAdmin)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emergency Call Center", "error", "101 Calls can only be made by Civilians!", new NewNotificationMessageContent[0]));
                    return;
                }

                string postal;
                while (true)
                {
                    postal = await GetUserInput("Enter Postal Code", "", 10);
                    if (string.IsNullOrWhiteSpace(postal)) return;

                    if (postal.All(char.IsDigit)) break;

                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emergency Call Center", "error", "Postal must contain numbers only.", new NewNotificationMessageContent[0]));
                }

                string division = await GetUserInput("Enter Targeted Division", "", 20);
                if (string.IsNullOrWhiteSpace(division)) return;

                string message = await GetUserInput("Describe the Situation", "", 200);
                if (string.IsNullOrWhiteSpace(message))
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emergency Call Center", "error", "Message cannot be empty.", new NewNotificationMessageContent[0]));
                    return;
                }

                string finalMessage = $"📍 {postal} | 🛂 {division} | 💬 {message}";

                _comms.ToServer(ServerEvents.Send101CallToServer, finalMessage);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emergency Call Center", "success", "Your 101 call has been sent!", new NewNotificationMessageContent[0]));
                API.PlaySoundFrontend(-1, "Hang_Up", "Phone_SoundSet_Michael", false);
                await Delay(500);
                API.PlaySoundFrontend(-1, "Hang_Up", "Phone_SoundSet_Michael", false);
            });

            // FORCE INFO - ANNOUNCEMENTS
            _commands.Register("forceinfo").HasGreedyArgs().WithHandler(async (message) =>
            {
                if (usingForceUpdate)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Force Information", "error", "You have updated Force Information recently, await until 5 Mins have passed!", new NewNotificationMessageContent[0]));
                    return;
                }

                var currentUserRole = _permissionService.CurrentUserRole;

                if (currentUserRole.Branch == UserBranch.Civ)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Force Information", "error", "Only Police/LHS/LFRS/Highways England can send out Force Information Updates!", new NewNotificationMessageContent[0]));
                    return;
                }

                if (string.IsNullOrEmpty(message) || message == "" || message == string.Empty)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Force Information", "error", "/forceinfo [Force Information Message]", new NewNotificationMessageContent[0]));
                    return;
                }

                usingForceUpdate = true;

                var div = "null";
                if (currentUserRole.Branch == UserBranch.Police) div = "Police";
                if (currentUserRole.Branch == UserBranch.Nhs) div = "Nhs";
                if (currentUserRole.Branch == UserBranch.Fire) div = "Fire";
                if (currentUserRole.Branch == UserBranch.Highways) div = "Highways";
                if (currentUserRole.Branch == UserBranch.Control) div = "Control";


                message = message.Replace("#", @"\#");
                _comms.ToServer(ServerEvents.SendForceInfoToServer, message, div);

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Force Information", "success", "You have sent a force information update!", new NewNotificationMessageContent[0]));
                API.PlaySoundFrontend(-1, "Hang_Up", "Phone_SoundSet_Michael", false);
                Delay(500);
                API.PlaySoundFrontend(-1, "Hang_Up", "Phone_SoundSet_Michael", false);

                if (_userAces.IsBandThree || _userAces.IsBandFour || _userAces.IsDeveloper || _userAces.IsAdmin)
                {
                    usingForceUpdate = false;
                    return;
                }

                await Delay(300000); // 5 MIN COOLDOWN
                usingForceUpdate = false;

            });
        }
        
        private async Task<string> GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            API.AddTextEntry("FMMC_KEY_TIP1", windowTitle);
            API.DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP1", "", defaultText, "", "", "", maxLength);

            while (API.UpdateOnscreenKeyboard() == 0)
            {
                await Delay(0);
            }

            if (API.UpdateOnscreenKeyboard() == 1)
            {
                return API.GetOnscreenKeyboardResult();
            }

            return null;
        }
    }
}