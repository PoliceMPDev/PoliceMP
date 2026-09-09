using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Options;
using System;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.DiscordRichPresence
{
    public class DiscordRichPresence : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private DiscordRichPresenceOptions _options;
        private readonly ITickManager _ticks;

        public DiscordRichPresence(ILegacyClientCommunicationsManager comms, ITickManager ticks)
        {
            _comms = comms;
            _ticks = ticks;
        }

        protected override async Task OnStartAsync()
        {
            _options = await _comms.Request<DiscordRichPresenceOptions>(ServerEvents.DiscordRichPresenceGetOptions);
            _ticks.On(DiscordRichPresenceTick);
        }

        private async Task DiscordRichPresenceTick()
        {
            API.SetDiscordAppId(_options.AppId);
            API.SetDiscordRichPresenceAsset(_options.Asset);
            API.SetDiscordRichPresenceAssetText(_options.AssetText);
            API.SetDiscordRichPresenceAssetSmall(_options.AssetSmall);
            API.SetDiscordRichPresenceAssetSmallText(_options.AssetSmallText);
            API.SetRichPresence(GetRichPresenceText());

            _options.ActionButtons.ForEach(actionButton =>
            {
                API.SetDiscordRichPresenceAction(
                    actionButton.Index,
                    actionButton.Label,
                    actionButton.Url);
            });

            await Delay(TimeSpan.FromSeconds(30));
        }

        private string GetRichPresenceText()
        {
            string street = Game.PlayerPed.GetCurrentStreet();
            var currentVehicle = Game.PlayerPed.CurrentVehicle;

            string text = currentVehicle == null
                ? $"On foot at {street}."
                : $"Driving on {street}.";

            return text;
        }
    }
}