using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Constants;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers
{
    public class RoadManagementController : Controller
    {
        private const string PROP_CATEGORIES_FILE = "PropCategories.json";
        private const string PROP_ITEMS_FILE = "PropItems.json";

        #region Services

        private readonly ILogger<RoadManagementController> _logger;
        private readonly ILegacyServerCommunicationsManager _comms;

        #endregion Services

        #region Variables

        private List<PropCategory> PropCategories;
        private List<PropItem> PropItems;
        private List<RoadManagementItem> _roadManagementItems = new();
        private Dictionary<Core.Shared.Models.PmpVector3, int> _roadSpeedZones = new();

        #endregion Variables

        public RoadManagementController(ILogger<RoadManagementController> logger,
            ILegacyServerCommunicationsManager comms, IFiveEventManager fiveEvents)
        {
            _logger = logger;
            _comms = comms;
            // fiveEvents.On<Player, string>(FiveEvents.PlayerJoining, OnPlayerJoining);
        }

        public override async Task Started()
        {
            _comms.On<RoadManagementItem>(ServerEvents.CreateRoadPropOnServer, OnCreateRoadPropOnServer);
            _comms.On(ServerEvents.ClearRoadPropOnServer, OnClearRoadPropOnServer);
            _comms.OnRequest<List<PropCategory>>(ServerEvents.GetPropData, OnRequestPropData);
            _comms.On<int>(ServerEvents.RoadsDeleteProp, OnServerDeleteProp);

            var propCategoriesJson = await TextFromFileAsync(PROP_CATEGORIES_FILE);
            var propItemsJson = await TextFromFileAsync(PROP_ITEMS_FILE);

            PropCategories = JsonConvert.DeserializeObject<List<PropCategory>>(propCategoriesJson);
            PropItems = JsonConvert.DeserializeObject<List<PropItem>>(propItemsJson);

            foreach (PropCategory pc in PropCategories)
            {
                pc.Props.Clear();
            }

            if (PropItems.Count != 0)
            {
                PropItems.Sort((x, y) => string.Compare(x.PropName, y.PropName));

                foreach (PropItem pi in PropItems)
                {
                    if (pi.PropCategories.Count() == 0)
                    {
                        _logger.Warn(pi.PropName + " does not have any categories defined.");
                    }

                    bool validcat = false;
                    foreach (PropCategory pc in PropCategories)
                    {
                        if (pi.PropCategories.Contains(pc.CategoryName))
                        {
                            try
                            {
                                pc.Props.Add(pi);
                                validcat = true;
                            }
                            catch (Exception ex)
                            {
                                _logger.Warn(ex.ToString());
                            }
                        }
                    }

                    if (!validcat)
                    {
                        _logger.Warn(pi.PropName + " does not have any valid categories defined.");
                    }
                }
            }
            else
            {
                _logger.Warn("ERROR in props config.");
            }
        }

        private Task OnServerDeleteProp(int propDelete)
        {
            var entity = API.NetworkGetEntityFromNetworkId(propDelete);
            API.DeleteEntity(entity);
            return Task.FromResult(0);
        }

        private Task OnPlayerJoining([FromSource] Player player,
            string oldId)
        {
            foreach (var roadManagementItem in _roadManagementItems.ToArray())
            {
                if (roadManagementItem.Speed > 0)
                {
                    _comms.ToClient(player, ServerEvents.UpdateSpeedAtPosition, roadManagementItem.Position,
                        roadManagementItem.Speed);
                }

                if (roadManagementItem.Speed == 0)
                {
                    _comms.ToClient(player, ServerEvents.UpdateNodeAtPosition, roadManagementItem.NodePosition, false);
                }
            }

            return Task.CompletedTask;
        }

        private Task<List<PropCategory>> OnRequestPropData(Player player)
        {
            List<PropCategory> AllowedProps = new List<PropCategory>();
            foreach (PropCategory pc in PropCategories)
            {
                if (pc.AceGroupsRequired.Length == 0)
                {
                    AllowedProps.Add(pc);
                    continue;
                }

                if (pc.AceGroupsRequired.Any())
                {
                    if (pc.AceGroupsRequired.All(s => API.IsPlayerAceAllowed(player.Handle, s)))
                    {
                        AllowedProps.Add(pc);
                    }
                }
            }

            AllowedProps.Sort((x, y) => string.Compare(x.CategoryName, y.CategoryName));

            //_logger.Debug($"Returning {AllowedProps.Count} prop data entries to player [{player.Name}]");
            return Task.FromResult(AllowedProps);
        }

        private async Task OnClearRoadPropOnServer(Player player)
        {
            lock (_roadManagementItems)
            {
                var position = new Core.Shared.Models.PmpVector3(player.Character.Position.X, player.Character.Position.Y,
                    player.Character.Position.Z);
                RoadManagementItem? roadManagementItem = null;
                var distance = 2f;
                foreach (var roadItem in _roadManagementItems)
                {
                    var propDistance = Core.Shared.Models.PmpVector3.Distance(position, roadItem.Position);
                    _logger.Trace($"Distance: {distance}");
                    if (propDistance > distance) continue;
                    distance = propDistance;
                    roadManagementItem = roadItem;
                }

                if (roadManagementItem == null) return;

                if (roadManagementItem.Speed >= 0)
                {
                    if (roadManagementItem.Speed == 0)
                    {
                        var stopConeCount = _roadManagementItems
                            .Count(x => x.NodePosition == roadManagementItem.NodePosition);

                        _logger.Debug($"Stop Cone Count: {stopConeCount}");

                        if (stopConeCount == 1)
                        {
                            //Last Stop Cone so can re-open road!
                            _comms.ToClient(ServerEvents.UpdateNodeAtPosition, roadManagementItem.NodePosition, true);
                        }
                    }

                    _comms.ToClient(ServerEvents.UpdateSpeedAtPosition, roadManagementItem.Position, -1);
                    var ratEntity = (Ped)Entity.FromNetworkId(roadManagementItem.RatNetId);
                    if (API.DoesEntityExist(ratEntity.Handle))
                    {
                        API.DeleteEntity(ratEntity.Handle);
                    }
                }

                var prop = (Prop)Entity.FromNetworkId(roadManagementItem.ObjectNetId);
                if (API.DoesEntityExist(prop.Handle))
                {
                    API.DeleteEntity(prop.Handle);
                }

                _roadManagementItems.Remove(roadManagementItem);
            }
        }

        private async Task OnCreateRoadPropOnServer(Player player, RoadManagementItem roadManagementItem)
        {
            var propModel = API.CreateObjectNoOffset((uint)roadManagementItem.ModelHash, roadManagementItem.Position.X,
                roadManagementItem.Position.Y, roadManagementItem.Position.Z, true, false, false);

            var prop = new Prop(propModel);

            prop.Rotation = new Vector3(roadManagementItem.Rotation.X, roadManagementItem.Rotation.Y,
                roadManagementItem.Rotation.Z);

            var tryCount = 0;
            while (prop.Owner == null && tryCount < 10)
            {
                tryCount++;
                await BaseScript.Delay(100);
            }

            if (prop.Owner == null)
            {
                _logger.Error("Unable to get the Prop Owner");
                API.DeleteEntity(prop.Handle);
                return;
            }

            API.FreezeEntityPosition(prop.Handle, true);

            roadManagementItem.PlacedByNetId = player.Character.NetworkId;
            roadManagementItem.ObjectNetId = prop.NetworkId;

            if (roadManagementItem.Speed >= 0)
            {
                if (roadManagementItem.Speed == 0)
                {
                    var nearestRoadNode =
                        await _comms.Request<Core.Shared.Models.PmpVector3>(prop.Owner, ServerEvents.FetchNodePosition,
                            prop.Position);
                    _logger.Debug($"Nearest Road Node: {nearestRoadNode}");
                    roadManagementItem.NodePosition = nearestRoadNode;
                    _comms.ToClient(ServerEvents.UpdateNodeAtPosition, nearestRoadNode, false);
                }

                // Create Rat to avoid prop
                var ratHandle = API.CreatePed(1, (uint)API.GetHashKey("A_C_Rat"), prop.Position.X, prop.Position.Y,
                    prop.Position.Z - 1f, prop.Rotation.Z, true, false);
                var rat = (Ped)Entity.FromHandle(ratHandle);
                API.FreezeEntityPosition(ratHandle, true);
                rat.State.Set($"{rat.NetworkId}", "invisible", true);
                roadManagementItem.RatNetId = rat.NetworkId;
                _comms.ToClient(ServerEvents.UpdateSpeedAtPosition, roadManagementItem.Position,
                    roadManagementItem.Speed);
            }

            lock (_roadManagementItems)
            {
                _roadManagementItems.Add(roadManagementItem);
            }
        }

        private async Task<string> TextFromFileAsync(string file)
        {
            try
            {
                using (var reader = new StreamReader(file))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}