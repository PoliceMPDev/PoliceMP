using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers
{
    public class TaserController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ILogger<TaserController> _logger;
        
        public TaserController(ILegacyServerCommunicationsManager comms, ILogger<TaserController> logger)
        {
            _comms = comms;
            _logger = logger;
            
            _comms.On<int, Vector3>(ServerEvents.TaserDetectSuspect, (netId, carrierLocation) =>
            {
                _comms.ToClient(ClientEvents.TaserDetectSuspect, netId, carrierLocation);
            });
            
            _comms.On<int, int>(ServerEvents.TaserSuspectDetected, (netId, suspectPlayerId) =>
            {
                _comms.ToClient(ClientEvents.TaserSuspectDetected, netId, suspectPlayerId);
            });
            
            _comms.On<int>(ServerEvents.TaserReactivateSuspect, (_, suspectPlayerId) =>
            {
                _comms.ToClient(ClientEvents.TaserReactivateSuspect, suspectPlayerId);
            });
            
            _comms.On<bool>(ServerEvents.TaserUpdateLog, (_, logStatus) =>
            {
                _comms.ToClient(ClientEvents.TaserUpdateLog, logStatus);
            });
            
            _comms.On<int, int, Vector3, int>(ServerEvents.TaserCheckLocation, (_, suspectPlayerId, carrierNetId, carrierLocation, suspectNumber) =>
            {
                _comms.ToClient(ClientEvents.TaserCheckLocation, suspectPlayerId, carrierNetId, carrierLocation, suspectNumber);
            });

            // EventHandlers["Server:CheckLocation"] += new Action<int, int, Vector3, int>((suspectPlayerId, carrierNetId, carrierLocation, suspectNumber) =>
            // {
            //     TriggerClientEvent("Client:CheckLocation", suspectPlayerId, carrierNetId, carrierLocation, suspectNumber);
            // });
            
            _comms.On<int>(ServerEvents.TaserBarbsInvalidated, (player, suspectNumber) =>
            {
                _comms.ToClient(ClientEvents.TaserBarbsInvalidated, player.Character.NetworkId, suspectNumber);
            });
            
            _comms.On<int>(ServerEvents.TaserNotifyBarbsRemoved, (_, suspectPlayerId) =>
            {
                _comms.ToClient(ClientEvents.TaserNotifyBarbsRemoved, suspectPlayerId);
            });
            
            _comms.On<string>(ServerEvents.TaserDisplayNotification, (_, messageContents) =>
            {
                _comms.ToClient(ClientEvents.TaserDisplayNotification, messageContents);
            });
        }
    }
}
