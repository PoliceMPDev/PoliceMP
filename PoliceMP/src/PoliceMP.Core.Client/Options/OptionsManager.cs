using System.Threading.Tasks;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Options.Interfaces;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Core.Client.Options
{
    public class OptionsManager : IOptionsManager
    {
        public PoliceMP.Shared.Options.Options Options { get; private set; }

        private readonly ILegacyClientCommunicationsManager _comms;

        public OptionsManager(ILegacyClientCommunicationsManager comms)
        {
            _comms = comms;
        }

        public async Task Initialise()
        {
            Options = await _comms.Request<PoliceMP.Shared.Options.Options>(ServerEvents.GetOptions);
        }
    }
}