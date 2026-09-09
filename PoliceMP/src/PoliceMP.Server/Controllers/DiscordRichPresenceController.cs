using CitizenFX.Core;
using Microsoft.Extensions.Options;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Options;
using System.Threading.Tasks;

namespace PoliceMP.Server.Controllers
{
    public class DiscordRichPresenceController : Controller
    {
        private readonly ILogger<DiscordRichPresenceController> _logger;
        private readonly DiscordRichPresenceOptions _options;

        public DiscordRichPresenceController(ILegacyServerCommunicationsManager comms,
            ILogger<DiscordRichPresenceController> logger,
            IOptions<DiscordRichPresenceOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            comms.OnRequest(ServerEvents.DiscordRichPresenceGetOptions, OnGetOptions);
        }

        private Task<DiscordRichPresenceOptions> OnGetOptions(Player player)
        {
            _logger.Trace($"Player \"{player.Name}\" requested Discord Rich Presence options.");
            return Task.FromResult(_options);
        }
    }
}