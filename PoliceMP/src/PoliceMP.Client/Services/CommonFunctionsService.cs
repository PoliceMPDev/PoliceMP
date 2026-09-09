using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Services
{
    public class CommonFunctionsService : Script, ICommonFunctionsService
    {
        private readonly ILogger<CommonFunctionsService> _logger;

        public CommonFunctionsService(ILogger<CommonFunctionsService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Requesting the player control of a network ID
        /// </summary>
        /// <param name="networkId">NetworkID you want control of</param>
        /// <param name="timeout">Timeout or number of attempts to gain control (50ms between attempts)</param>
        /// <param name="callerName">The resource caller</param>
        /// <returns>True = Control gained   False = Control cannot be gained or other issue.</returns>
        public async Task<bool> RequestNetworkEntityControl(int networkId, int timeout = 10, [CallerMemberName] string callerName = "")
        {
            if (!API.NetworkDoesEntityExistWithNetworkId(networkId))
            {
                _logger.Debug($"{callerName} : NetworkID {networkId} does not exist.");
                return false;
            }

            if (API.NetworkHasControlOfNetworkId(networkId)) return true;

            var breaker = 0;
            while (!API.NetworkHasControlOfNetworkId(networkId) && breaker <= timeout)
            {
                breaker++;
                if (!API.NetworkDoesEntityExistWithNetworkId(networkId))
                {
                    _logger.Debug($"{callerName} : NetworkID {networkId} no longer exists during control attempt.");
                    return false;
                }
                API.NetworkRequestControlOfNetworkId(networkId);
            }
            if (breaker > timeout)
            {
                _logger.Debug($"{callerName} : NetworkID {networkId} control could not be gained after {timeout} attempts.");
                return false;
            }

            if (API.NetworkHasControlOfNetworkId(networkId)) return true;
            _logger.Debug($"{callerName} : NetworkID {networkId} something broke when requesting.");
            return false;
        }

        /// <summary>
        /// Requests a model to be loaded.
        /// </summary>
        /// <param name="model">Model name</param>
        /// <param name="attempts">How many attempts to load model (default 10)</param>
        /// <param name="callerName">The resource caller</param>
        /// <returns>If the model loaded successfully</returns>
        public async Task<bool> RequestModelToBeLoaded(string model, int attempts = 10, [CallerMemberName] string callerName = "")
        {
            var hash = (uint)API.GetHashKey(model);
            if (!API.IsModelInCdimage(hash))
            {
                _logger.Debug($"{callerName} : Model {model} is not valid.");
                return false;
            }
            API.RequestModel(hash);
            var breaker = 0;
            while (!API.HasModelLoaded(hash))
            {
                if (breaker == attempts)
                {
                    _logger.Debug($"{callerName} : Model {model} is valid but could not be obtained after {attempts} attempts.");
                    return false;
                }
                breaker++;
                API.RequestModel(hash);
            }
            return breaker <= attempts && API.HasModelLoaded(hash);
        }

        public string TildeStripper(string inputString)
        {
            var sanitised = inputString.Replace("~", " ");
            return sanitised;
        }
    }
}