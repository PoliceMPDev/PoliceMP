using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface ICommonFunctionsService
    {
        public Task<bool> RequestNetworkEntityControl(int networkId, int timeout = 10, [CallerMemberName] string callerName = "");

        public Task<bool> RequestModelToBeLoaded(string model, int attempts = 10,
            [CallerMemberName] string callerName = "");

        public string TildeStripper(string inputString);
    }
}