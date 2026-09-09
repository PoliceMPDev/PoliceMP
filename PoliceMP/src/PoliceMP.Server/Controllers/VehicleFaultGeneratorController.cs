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
    public class VehicleFaultGeneratorController : Controller
    {
        private readonly ILogger<VehicleFaultGeneratorController> _logger;
        private readonly VehicleFaultGeneratorOptions _options;

        public VehicleFaultGeneratorController(ILegacyServerCommunicationsManager comms,
            IOptions<VehicleFaultGeneratorOptions> options,
            ILogger<VehicleFaultGeneratorController> logger)
        {
            _options = options.Value;
            _logger = logger;
            comms.OnRequest(ServerEvents.VehicleFaultGeneratorGetOptions, OnGetOptions);
        }

        private Task<VehicleFaultGeneratorOptions> OnGetOptions(Player player)
        {
            _logger.Trace($"Player \"{player.Name}\" requested Vehicle Fault Generator options.");
            return Task.FromResult(_options);
        }
    }
}