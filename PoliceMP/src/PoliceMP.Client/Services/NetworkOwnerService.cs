using CitizenFX.Core;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Services
{
    public class NetworkOwnerService : Script
    {
        private readonly ILogger<NetworkOwnerService> _logger;
        private readonly ILegacyClientCommunicationsManager _comms;

        public NetworkOwnerService(ILegacyClientCommunicationsManager comms, ILogger<NetworkOwnerService> logger)
        {
            _comms = comms;
            _logger = logger;

            _comms.On<int, bool>(ClientEvents.SetCarLockState, SetCarLockState);
        }

        private void SetCarLockState(int vehicleNetId, bool lockStatus)
        {
            var entity = (Vehicle)Entity.FromNetworkId(vehicleNetId);

            entity.LockStatus = lockStatus ? VehicleLockStatus.Locked : VehicleLockStatus.Unlocked;
        }
    }
}

