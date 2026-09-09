using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using System;
using System.Threading.Tasks;
using PoliceMP.Client.Extensions;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Client.Overlays.NewNotification;

namespace PoliceMP.Client.Scripts.Afk
{
    public class AfkScript : Script
    {
        private readonly ILogger<AfkScript> _logger;
        private readonly INotificationService _notification;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ITickManager _ticks;
		private readonly INewNotificationOverlay _newNotificationOverlay;

		private const string PlayTimeKvpMinutes = "PoliceMp:Stats:PlayTime:Minutes";
        private const string PlayTimeKvpHours = "PoliceMp:Stats:PlayTime:Hours";
        private const string AfkTimeKvpMinutes = "PoliceMp:Stats:AfkTime:Minutes";
        private const string AfkTimeKvpHours = "PoliceMp:Stats:AfkTime:Hours";

        private DateTime LastAfkTime = DateTime.MinValue;
        private DateTime LastTimeCheck = DateTime.MinValue;
        private Vector3 LastPosition = new Vector3(0, 0, 0);

        private const int _afkTime = 10;
        private bool TimeCheckActive = false;

        public AfkScript(ILogger<AfkScript> logger,
            INotificationService notification,
            ILegacyClientCommunicationsManager comms,
            ITickManager ticks,
			INewNotificationOverlay newNotificationOverlay)
        {
            _logger = logger;
            _notification = notification;
			_comms = comms;
            _ticks = ticks;
			_newNotificationOverlay = newNotificationOverlay;

			_comms.OnRequest<string, int[]>(ClientEvents.FetchAfkTimeData, OnTimeRequest);
        }

        protected override Task OnStartAsync()
        {
            _logger.Debug("Started AFK System");
            string playTimeMinutesString = API.GetResourceKvpString(PlayTimeKvpMinutes);
            string playTimeHoursString = API.GetResourceKvpString(PlayTimeKvpHours);

            int playTimeMinutes = 0;
            int playTimeHours = 0;

            if (!string.IsNullOrEmpty(playTimeMinutesString))
            {
                playTimeMinutes = int.Parse(playTimeMinutesString);
            }

            if (!string.IsNullOrEmpty(playTimeHoursString))
            {
                playTimeHours = int.Parse(playTimeHoursString);
            }

			_newNotificationOverlay.SendNotification(new NewNotificationMessage("Play time", "info", $"Total Playtime\n{playTimeHours:D2}:{playTimeMinutes:D2}", new NewNotificationMessageContent[0]));
			TimeCheckActive = true;

            _ticks.On(AfkScriptTick);

            return Task.FromResult(0);
        }

        private Task AfkScriptTick()
        {
            if (!TimeCheckActive) return Task.FromResult(0);

            if (LastTimeCheck == DateTime.MinValue)
            {
                LastTimeCheck = DateTime.Now;
                return Task.FromResult(0);
            }

            if (DateTime.Compare(DateTime.Now, LastTimeCheck.AddMinutes(1)) <= 0) return Task.FromResult(0);

            LastTimeCheck = DateTime.Now;

            //_logger.Debug("Starting AFK Check");

            Vector3 playerPosition = Game.PlayerPed.Position;

            string playTimeMinsString = API.GetResourceKvpString(PlayTimeKvpMinutes);
            string playTimeHrsString = API.GetResourceKvpString(PlayTimeKvpHours);
            string afkTimeMinutesString = API.GetResourceKvpString(AfkTimeKvpMinutes);
            string afkTimeHoursString = API.GetResourceKvpString(AfkTimeKvpHours);

            bool playTimeMinsParse = int.TryParse(playTimeMinsString, out int playTimeMins);
            if (!playTimeMinsParse)
            {
                playTimeMins = 0;
                API.SetResourceKvp(PlayTimeKvpMinutes, playTimeMins.ToString());
            }
            bool playTimeHrsParse = int.TryParse(playTimeHrsString, out int playTimeHrs);
            if (!playTimeHrsParse)
            {
                playTimeHrs = 0;
                API.SetResourceKvp(PlayTimeKvpHours, playTimeHrs.ToString());
            }
            bool afkTimeMinsParse = int.TryParse(afkTimeMinutesString, out int afkTimeMinutes);
            if (!afkTimeMinsParse)
            {
                afkTimeMinutes = 0;
                API.SetResourceKvp(AfkTimeKvpMinutes, afkTimeMinutes.ToString());
            }
            bool afkTimeHrsParse = int.TryParse(afkTimeHoursString, out int afkTimeHours);
            if (!afkTimeHrsParse)
            {
                afkTimeHours = 0;
                API.SetResourceKvp(AfkTimeKvpHours, afkTimeHours.ToString());
            }

            if (playTimeMins == 59)
            {
                //_logger.Debug("Playtime Minutes = 59");
                playTimeMins = 0;
                playTimeHrs += 1;
            }
            else playTimeMins += 1;

            if (LastAfkTime == DateTime.MinValue || LastPosition == new Vector3(0, 0, 0))
            {
                //_logger.Debug("No data");
                LastAfkTime = DateTime.Now;
                LastPosition = playerPosition;
                return Task.FromResult(0);
            }

            if (LastPosition.Distance(playerPosition) > 2.5)
            {
                //_logger.Debug("Player Moved");
                LastPosition = playerPosition;
                LastAfkTime = DateTime.Now;
                API.SetResourceKvp(PlayTimeKvpMinutes, playTimeMins.ToString());
                API.SetResourceKvp(PlayTimeKvpHours, playTimeHrs.ToString());
                //_logger.Debug($"New Playtime: {playTimeHrs}:{playTimeMins}");
                return Task.FromResult(0);
            }

            if (DateTime.Compare(DateTime.Now, LastAfkTime.AddMinutes(_afkTime)) <= 0)
            {
                API.SetResourceKvp(PlayTimeKvpMinutes, playTimeMins.ToString());
                API.SetResourceKvp(PlayTimeKvpHours, playTimeHrs.ToString());
                //_logger.Debug($"New Playtime: {playTimeHrs}:{playTimeMins}");
                return Task.FromResult(0);
            }

            //_logger.Debug("Player AFK");

            // Not moved for 10 minutes

            if (afkTimeMinutes == 59)
            {
                afkTimeMinutes = 0;
                afkTimeHours += 1;
            }
            else afkTimeMinutes += 1;

            API.SetResourceKvp(AfkTimeKvpMinutes, afkTimeMinutes.ToString());
            API.SetResourceKvp(AfkTimeKvpHours, afkTimeHours.ToString());
            //_logger.Debug($"New AFK Time: {afkTimeHours}:{afkTimeMinutes}");
            return Task.FromResult(0);
        }

        private Task<int[]> OnTimeRequest(string s)
        {
            string playTimeMinsString = API.GetResourceKvpString(PlayTimeKvpMinutes);
            string playTimeHrsString = API.GetResourceKvpString(PlayTimeKvpHours);
            string afkTimeMinutesString = API.GetResourceKvpString(AfkTimeKvpMinutes);
            string afkTimeHoursString = API.GetResourceKvpString(AfkTimeKvpHours);

            bool playTimeMinsParse = int.TryParse(playTimeMinsString, out int playTimeMins);
            if (!playTimeMinsParse)
            {
                playTimeMins = 0;
                API.SetResourceKvp(PlayTimeKvpMinutes, playTimeMins.ToString());
            }
            bool playTimeHrsParse = int.TryParse(playTimeHrsString, out int playTimeHrs);
            if (!playTimeHrsParse)
            {
                playTimeHrs = 0;
                API.SetResourceKvp(PlayTimeKvpHours, playTimeHrs.ToString());
            }
            bool afkTimeMinsParse = int.TryParse(afkTimeMinutesString, out int afkTimeMinutes);
            if (!afkTimeMinsParse)
            {
                afkTimeMinutes = 0;
                API.SetResourceKvp(AfkTimeKvpMinutes, afkTimeMinutes.ToString());
            }
            bool afkTimeHrsParse = int.TryParse(afkTimeHoursString, out int afkTimeHours);
            if (!afkTimeHrsParse)
            {
                afkTimeHours = 0;
                API.SetResourceKvp(AfkTimeKvpHours, afkTimeHours.ToString());
            }

            int[] timeData = new int[]
            {
                playTimeHrs,
                playTimeMins,
                afkTimeHours,
                afkTimeMinutes
            };

            return Task.FromResult(timeData);
        }
    }
}