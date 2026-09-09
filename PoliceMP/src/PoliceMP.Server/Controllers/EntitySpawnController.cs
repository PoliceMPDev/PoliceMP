using System.Runtime.InteropServices;
//using System.Threading.Tasks;
//using CitizenFX.Core;
//using CitizenFX.Core.Native;
//using PoliceMP.Core.Server.Commands.Interfaces;
//using PoliceMP.Core.Server.Communications.Interfaces;
//using PoliceMP.Core.Server.Networking;
//using PoliceMP.Core.Shared;
//using PoliceMP.Shared.Constants;
//using PoliceMP.Shared.Models;

//namespace PoliceMP.Server.Controllers
//{
//    public class EntitySpawnController : Controller
//    {
//        private readonly ICommandManager _command;
//        private readonly ILegacyServerCommunicationsManager _comms;
//        private readonly ILogger<EntitySpawnController> _logger;

//        public EntitySpawnController(ICommandManager command, ILegacyServerCommunicationsManager comms, ILogger<EntitySpawnController> logger)
//        {
//            _command = command;
//            _comms = comms;
//            _logger = logger;
//        }

//        public override Task Started()
//        {
//            _comms.OnRequest<EntitySpawnVehicle, int>(ServerEvents.RequestSpawnEntityVehicle, OnRequestSpawnEntityVehicle);
//            _comms.OnRequest<EntitySpawnPed, int>(ServerEvents.RequestSpawnEntityPed, OnRequestSpawnEntityPed);
//            _comms.OnRequest<EntitySpawnObject, int>(ServerEvents.RequestSpawnEntityObject, OnRequestSpawnEntityObject);
//            return base.Started();
//        }

//        private async Task<int> OnRequestSpawnEntityVehicle(Player player, EntitySpawnVehicle entity)
//        {
//            var hash = (uint)API.GetHashKey(entity.Name);
            
//            var vehicle = API.CreateVehicle(hash, entity.Position.X, entity.Position.Y, entity.Position.Z, entity.Heading, true,
//                entity.MissionEntity);

//            var breaker = 25;
//            while (!API.DoesEntityExist(vehicle))
//            {
//                if(breaker == 0) break;
//                breaker--;
//                await Delay(1);
//            }

//            if (!API.DoesEntityExist(vehicle))
//            {
//                return -1;
//            }

//            //_logger.Debug($"NETWORK_GET_NETWORK_ID_FROM_ENTITY {System.Reflection.MethodBase.GetCurrentMethod().Name}");
//            var netId = API.NetworkGetNetworkIdFromEntity(vehicle);

//            return netId;
//        }
        
//        private async Task<int> OnRequestSpawnEntityPed(Player player, EntitySpawnPed entity)
//        {
//            var hash = (uint)API.GetHashKey(entity.Name);
            
//            var ped = API.CreatePed(0, hash, entity.Position.X, entity.Position.Y, entity.Position.Z, entity.Heading, true,
//                entity.ScriptHosted);

//            var breaker = 25;
//            while (!API.DoesEntityExist(ped))
//            {
//                if(breaker == 0) break;
//                breaker--;
//                await Delay(1);
//            }

//            if (!API.DoesEntityExist(ped))
//            {
//                return -1;
//            }

//            //_logger.Debug($"NETWORK_GET_NETWORK_ID_FROM_ENTITY {System.Reflection.MethodBase.GetCurrentMethod().Name}");
//            var netId = API.NetworkGetNetworkIdFromEntity(ped);

//            return netId;
//        }
        
//        private async Task<int> OnRequestSpawnEntityObject(Player player, EntitySpawnObject entity)
//        {
//            var hash = API.GetHashKey(entity.Name);
            
//            var obj = API.CreateObject(hash, entity.Position.X, entity.Position.Y, entity.Position.Z, true, entity.MissionEntity, entity.DoorFlag );

//            var breaker = 25;
//            while (!API.DoesEntityExist(obj))
//            {
//                if(breaker == 0) break;
//                breaker--;
//                await Delay(1);
//            }

//            if (!API.DoesEntityExist(obj))
//            {
//                return -1;
//            }

//            //_logger.Debug($"NETWORK_GET_NETWORK_ID_FROM_ENTITY {System.Reflection.MethodBase.GetCurrentMethod().Name}");
//            var netId = API.NetworkGetNetworkIdFromEntity(obj);

//            return netId;
//        }
//    }
//}