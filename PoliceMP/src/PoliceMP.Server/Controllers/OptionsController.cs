using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Server.Options.Interfaces;
using PoliceMP.Shared.Constants;
using System.Threading.Tasks;

namespace PoliceMP.Server.Controllers
{
    public class OptionsController : Controller
    {
        private readonly IOptionsManager _options;
        private readonly ILegacyServerCommunicationsManager _comms;

        public OptionsController(IOptionsManager options, ILegacyServerCommunicationsManager comms)
        {
            _options = options;
            _comms = comms;

            _comms.OnRequest<Shared.Options.Options>(ServerEvents.GetOptions, OnGetOptions);
        }

        private Task<Shared.Options.Options> OnGetOptions(Player player)
        {
            return Task.FromResult(_options.Options);
        }
    }
}
