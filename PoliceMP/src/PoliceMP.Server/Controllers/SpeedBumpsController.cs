using System.Threading.Tasks;
using CitizenFX.Core;
using Microsoft.Extensions.Options;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Options;

namespace PoliceMP.Server.Controllers
{
    public class SpeedBumpController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ILogger<SpeedBumpController> _logger;
        private readonly SpeedBumpOptions _options;

        public SpeedBumpController(ILegacyServerCommunicationsManager comms,
            ILogger<SpeedBumpController> logger,
            IOptions<SpeedBumpOptions> options)
        {
            _comms = comms;
            _logger = logger;
            _options = options.Value;

            _comms.OnRequest(SpeedBumpNetworkConstants.GetOptions, OnGetOptions);
        }

        private Task<SpeedBumpOptions> OnGetOptions(Player player)
        {
            return Task.FromResult(_options);
        }
    }
}