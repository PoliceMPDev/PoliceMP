using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.RoadManagement.Shared;

namespace PoliceMP.RoadManagement.Client
{
    public class RoadblocksManager : BaseScript
    {
        private static readonly int MAX_OBJECTS_NUM = 15;
        private static readonly float MAX_DISTANCE_FROM_OBJECT = 100f;
        private static readonly string SPIKES_MODEL = "P_ld_stinger_s";
        private static Dictionary<string, string> _objects = new Dictionary<string, string>
        {
            // Spikes
            { "spikes", SPIKES_MODEL },

            // Cones
            { "cone1", "prop_mp_cone_01" },
            { "cone2", "prop_roadcone01a" },
            
            // Barriers
            { "barrier1", "prop_mp_barrier_02b" },
            { "barrier2", "prop_mp_arrow_barrier_01" },
            { "barrier3", "prop_barrier_wat_03a" },
            { "barrier4", "prop_barrier_work05" },
            { "barrier5", "prop_mc_conc_barrier_01" },

            { "bigbarrier1", "prop_barier_conc_05b" },
            { "bigbarrier2", "prop_barier_conc_05c" },

            { "tent", "prop_skid_tent_cloth" },
        };

        private static List<SpawnedObject> _spawnedObjects = new List<SpawnedObject>(MAX_OBJECTS_NUM); // Preallocate MAX_OBJECTS_NUM to ensure that malloc is called once under the hood.
        private static SpawnedObject _grabbedObject = null;
        private static List<int> _recentlySpikedVehicleIds = new List<int>();
        // List of known tyres used on the car
        private static List<int> tyres = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

        public RoadblocksManager()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        [Tick]
        private async Task OnTick()
        {
            // If the player is grabbing an object, 
            if (_grabbedObject != null)
            {
                if (API.IsControlPressed(0, 39)) // [
                {
                    Vector3 rotation = API.GetEntityRotation(_grabbedObject.Id, 0);
                    API.AttachEntityToEntity(_grabbedObject.Id, API.GetPlayerPed(-1), 11816, 0f, -1f, -0.6f, 0f, 0f, rotation.Z + 1f, false, false, false, false, 2, false);
                }
                else if (API.IsControlPressed(0, 40)) // ]
                {
                    Vector3 rotation = API.GetEntityRotation(_grabbedObject.Id, 0);
                    API.AttachEntityToEntity(_grabbedObject.Id, API.GetPlayerPed(-1), 11816, 0f, -1f, -0.6f, 0f, 0f, rotation.Z - 1f, false, false, false, false, 2, false);
                }
            }

            
            if (_spawnedObjects.Count > 0)
            {
                foreach (var spawnedObject in _spawnedObjects)
                {
                    // Handle periodic cleanup of objects
                    var playerPedPostion = Game.PlayerPed.Position;
                    var objectPostion = API.GetEntityCoords(spawnedObject.Id, false);
                    if (MAX_DISTANCE_FROM_OBJECT < API.GetDistanceBetweenCoords(objectPostion.X, objectPostion.Y, objectPostion.Z, playerPedPostion.X, playerPedPostion.Y, playerPedPostion.Z, false))
                    {
                        RemoveObject(spawnedObject);
                        // Need to break here as it will invalidate the iterators for the _spawnedObjects list being used in the foreach loop.
                        break;
                    }

                    // Handle the spikes
                    if (spawnedObject.Name == SPIKES_MODEL && spawnedObject != _grabbedObject)
                    {
                        var spikesPosition = objectPostion;
                        foreach (var vehicle in World.GetAllVehicles())
                        {
                            // If driven by player return and do nothing as to not bug out TODO: Make this work on a player car
                            if (API.IsPedAPlayer(vehicle.Driver.Handle)) { continue; }
                            // If the vehicle was spiked in the last 3 seconds return. This is to reduce network usage.
                            if(_recentlySpikedVehicleIds.Contains(vehicle.NetworkId)) { continue; }

                            var distance = API.GetDistanceBetweenCoords(spikesPosition.X, spikesPosition.Y, spikesPosition.Z, vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, false);

                            if (distance < 3.5)
                            {
                                Debug.WriteLine("Event sent to server netId:" + vehicle.NetworkId);
                                for (int i = 0; i <= 256; i++)
                                {
                                    API.SetNetworkIdSyncToPlayer(vehicle.NetworkId, API.GetPlayerFromServerId(i), true);
                                    API.SetNetworkIdSyncToPlayer(vehicle.Driver.NetworkId, API.GetPlayerFromServerId(i), true);
                                }
                                TriggerServerEvent(ServerEvents.BURST_TYRES_ON_ALL_CLIENTS, vehicle.NetworkId);
                                _recentlySpikedVehicleIds.Add(vehicle.NetworkId);
                                RemoveRecentlySpikedInThreeSeconds(vehicle.NetworkId);
                            }
                        }
                    }
                }
            }
        }

        private async void RemoveRecentlySpikedInThreeSeconds(int _networkId)
        {
            await Delay(3000);
            _recentlySpikedVehicleIds.Remove(_networkId);
        }

        [EventHandler("RoadManagement:BurstTyres")]
        private void BurstNetIdForVehiclesTyres(int _networkId)
        {
            Debug.WriteLine("Event recieved from server netId:" + _networkId);
            if (API.NetworkDoesNetworkIdExist(_networkId))
            {
                Debug.WriteLine("Network Id exists: " + _networkId);
                API.NetworkRequestControlOfNetworkId(_networkId);
                if (API.NetworkHasControlOfNetworkId(_networkId))
                {
                    Debug.WriteLine("Client has control of ID: " + _networkId);
                    SpikeStripEffects((Vehicle)Entity.FromNetworkId(_networkId));
                }
            }
            
        }

        private void SpikeStripEffects(Vehicle vehicle)
        {
            Debug.WriteLine("Attempting to spike!");
            // List of known tyres used on the car
            var tyres = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
            foreach (var tyreIndex in tyres)
            {
                if (!API.IsVehicleTyreBurst(vehicle.Handle, tyreIndex, true) && !API.IsVehicleTyreBurst(vehicle.Handle, tyreIndex, false))
                {
                    API.SetVehicleTyreBurst(vehicle.Handle, tyreIndex, true, 1000);
                    API.SetVehicleForwardSpeed(vehicle.Handle, vehicle.Speed * 0.9f);
                }
            }
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            API.RegisterCommand("rb", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (_grabbedObject != null)
                {
                    SendMessage("Put down the object you are holding first.");
                    return;
                }

                if (args.Count > 0)
                {
                    string name = args[0].ToString();
                    if (_objects.ContainsKey(name))
                    {
                        SpawnObject(_objects[name]);
                    }
                }
            }), false);

            API.RegisterCommand("rb_delete", new Action<int, List<object>, string>((source, args, raw) =>
            {
                foreach (string objectName in _objects.Values)
                {
                    DeleteObject(objectName);
                }
            }), false);

            API.RegisterCommand("rb_deleteall", new Action<int, List<object>, string>((source, args, raw) =>
            {
                DeleteAllObjects();
            }), false);

            API.RegisterCommand("rb_place", new Action<int, List<object>, string>((source, args, raw) =>
            {
                PlaceObject();
            }), false);

            API.RegisterCommand("rb_grab", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (_grabbedObject != null)
                {
                    SendMessage("Put down the object you are holding first.");
                    return;
                }

                GrabObject();
            }), false);

            API.RegisterCommand("rb_list", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var builder = new StringBuilder();
                foreach (string objectKey in _objects.Keys)
                {
                    builder.Append($"{objectKey}, ");
                }

                SendMessage(builder.ToString());
            }), false);
        }

        private void SendMessage(string message)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 0, 0, 100 },
                args = new[] { "[Road Management]", message }
            });
        }

        private void DeleteObject(string name)
        {
            var hash = (uint)API.GetHashKey(name);
            var playerPos = API.GetEntityCoords(API.GetPlayerPed(-1), true);

            if (API.DoesObjectOfTypeExistAtCoords(playerPos.X, playerPos.Y, playerPos.Z, 0.9f, hash, true))
            {
                var obj = API.GetClosestObjectOfType(playerPos.X, playerPos.Y, playerPos.Z, 0.9f, hash, false, false, false);
                API.DeleteObject(ref obj);
                SendMessage($"{name} was deleted.");

                _spawnedObjects.RemoveAll(x => x.Id == obj);
            }
        }

        private void GrabObject()
        {
            if (_grabbedObject != null) return;

            var playerPos = API.GetEntityCoords(API.GetPlayerPed(-1), true);

            foreach (string objName in _objects.Values)
            {
                var hash = (uint)API.GetHashKey(objName);

                if (API.DoesObjectOfTypeExistAtCoords(playerPos.X, playerPos.Y, playerPos.Z, 0.9f, hash, true))
                {
                    var obj = API.GetClosestObjectOfType(playerPos.X, playerPos.Y, playerPos.Z, 0.9f, hash, false, false, false);

                    API.AttachEntityToEntity(obj, API.GetPlayerPed(-1), 11816, 0f, -1f, -0.6f, 0f, 0f, 0f, false, false, false, false, 2, false);

                    var spawnedObject = _spawnedObjects.Where(x => x.Id == obj).FirstOrDefault();
                    _grabbedObject = spawnedObject;
                }
            }
        }

        private void DeleteAllObjects()
        {
            foreach (var spawnedObject in _spawnedObjects)
            {
                int obj = spawnedObject.Id;
                API.DeleteObject(ref obj);
            }

            _spawnedObjects.Clear();
            _grabbedObject = null;
        }

        private void SpawnObject(string name)
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                SendMessage($"Cannot spawn road management objects inside of a vehicle!");
                return;
            }

            if (_spawnedObjects.Count >= MAX_OBJECTS_NUM)
            {
                SendMessage($"Cannot spawn more than {MAX_OBJECTS_NUM} objects at any one time");
                return;
            }

            var playerId = API.GetPlayerPed(-1);
            var playerPos = API.GetEntityCoords(playerId, true);
            var heading = API.GetEntityHeading(playerId);
            var hash = (uint)API.GetHashKey(name);
            API.RequestModel(hash);

            int objectId = API.CreateObject((int)hash, playerPos.X, playerPos.Y, playerPos.Z, true, true, true);
            API.AttachEntityToEntity(objectId, playerId, 11816, 0f, -1f, -0.6f, 0f, 0f, 0f, false, false, false, false, 2, false);

            var spawnedObject = new SpawnedObject() { Id = objectId, Name = name };
            _spawnedObjects.Add(spawnedObject);
            _grabbedObject = spawnedObject;

            /*Tick += OnTick;*/
            if (spawnedObject.Name == SPIKES_MODEL)
            {
                DeleteSpikesThread(spawnedObject);
            }

            SendMessage($"{name} was spawned. Use the [ and ] keys to rotate.");
        }

        private async void DeleteSpikesThread(SpawnedObject spikes)
        {
            // In 1 min delete spikes to help with potential server lag
            await Delay(60000);
            RemoveObject(spikes);
        }

        // Not a menu action
        private void RemoveObject(SpawnedObject obj)
        {
            _spawnedObjects.Remove(obj);
            if (_grabbedObject == obj)
            {
                _grabbedObject = null;
            }
            // Actually remove the object
            int objId = obj.Id;
            API.DeleteObject(ref objId);
        }

        private void PlaceObject()
        {
            if (_grabbedObject != null)
            {
                API.DetachEntity(_grabbedObject.Id, true, false);
                API.PlaceObjectOnGroundProperly(_grabbedObject.Id);
                API.FreezeEntityPosition(_grabbedObject.Id, true);

                var hash = (uint) API.GetHashKey(_grabbedObject.Name);
                var minDimension = new Vector3();
                var maxDimension = new Vector3();
                API.GetModelDimensions(hash, ref minDimension, ref maxDimension);

                var dimension = maxDimension - minDimension;
                Vector3 position = API.GetEntityCoords(_grabbedObject.Id, true);
                
                int navmesh = API.AddNavmeshBlockingObject(position.X, position.Y, position.Z, dimension.X + 10f, dimension.Y + 10f, dimension.Z + 10f, 20f, true, _grabbedObject.Id);
                _grabbedObject = null;

                /*Tick -= OnTick;*/

                SendMessage("Object was placed.");
            }            
        }
    }
}
