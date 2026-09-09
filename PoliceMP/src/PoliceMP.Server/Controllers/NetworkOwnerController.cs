using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers
{
    public class NetworkOwnerController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ILogger<NetworkOwnerController> _logger;

        public NetworkOwnerController(ILegacyServerCommunicationsManager comms, ILogger<NetworkOwnerController> logger)
        {
            _comms = comms;
            _logger = logger;

            _comms.On<int, bool>(ServerEvents.NetOwnerClientRequestingCarToggleLock, TogglelockCar);
        }

        private Task TogglelockCar(int vehicleNetId, bool lockStatus)
        {
            var entity = Entity.FromNetworkId(vehicleNetId);
            //var player = entity.Owner;

            if (lockStatus)
            {
                API.SetVehicleDoorsLocked(entity.Handle, 2); //locked
            }
            else
            {
                API.SetVehicleDoorsLocked(entity.Handle, 1); //Unlocked
            }            
            
            //_comms.ToClient(player, ClientEvents.SetCarLockState, vehicleNetId, lockStatus);
            return Task.CompletedTask;
        }
    }
}
