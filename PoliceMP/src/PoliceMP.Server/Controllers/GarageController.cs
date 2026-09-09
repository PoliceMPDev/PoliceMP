using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Shared.Communications.Interfaces;

namespace PoliceMP.Server.Controllers
{
    class GarageController : Controller
    {
        private const string GARAGE_POSITIONS_FILE = "Garages.json";
        private const string GARAGE_CATEGORIES_FILE = "GarageCategories.json";
        private const string GARAGE_VEHICLES_FILE = "GarageVehicles.json";

        private readonly ILogger<GarageController> _logger;
        private readonly ICommandManager _commands;
        private readonly ILegacyServerCommunicationsManager _comms;

        private List<GarageEntity> Garages;
        private List<GarageCategory> GarageData;
        private List<GarageVehicles> GarageVehicles;

        /// <summary>
        /// List of Personal Vehicles <PlayerHandle, VehNetId>
        /// </summary>
        //private Dictionary<string, int> PersonalVehicles = new();
        
        public GarageController(ILogger<GarageController> logger,
            ICommandManager commands,
            ILegacyServerCommunicationsManager comms)
        {
            _logger = logger;
            _commands = commands;
            _comms = comms;
        }

        public override async Task Started()
        {
            Garages = JsonConvert.DeserializeObject<List<GarageEntity>>(await TextFromFileAsync(GARAGE_POSITIONS_FILE));
            GarageData = JsonConvert.DeserializeObject<List<GarageCategory>>(await TextFromFileAsync(GARAGE_CATEGORIES_FILE));
            GarageVehicles = JsonConvert.DeserializeObject<List<GarageVehicles>>(await TextFromFileAsync(GARAGE_VEHICLES_FILE));

            foreach (GarageCategory gc in GarageData)
            {
                gc.Vehicles.Clear();
            }

            if (GarageVehicles.Count != 0)
            {
                GarageVehicles.Sort((x, y) => string.Compare(x.VehicleName, y.VehicleName));

                foreach (GarageVehicles gv in GarageVehicles)
                {
                    if (gv.GarageCategory.Count() == 0)
                    {
                        _logger.Warn(gv.VehicleClass + " does not have any categories defined.");
                    }

                    bool validcat = false;
                    foreach (GarageCategory gd in GarageData)
                    {
                        if (gv.GarageCategory.Contains(gd.CategoryName))
                        {
                            try
                            {
                                gd.Vehicles.Add(gv);
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
                        _logger.Warn(gv.VehicleClass + " does not have any valid categories defined.");
                    }
                }
            }
            else
            {
                _logger.Warn("ERROR in vehicles config.");
            }

            _comms.OnRequest<List<GarageEntity>>(ServerEvents.GetGaragePoints, OnRequestGaragePoints);
            _comms.OnRequest<List<GarageCategory>>(ServerEvents.GetGarageData, OnRequestGarageData);
            _comms.OnRequest<string, int>(ServerEvents.GetSirenType, OnRequestSirenType);
            //_comms.On<Player, int>(ServerEvents.OnGarageVehicleCreated, OnGarageVehicleCreated);

            // Finished loading
            return;
        }

        /*
        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            if (player.Character == null) return;
            var doesContain = PersonalVehicles.TryGetValue(player.Handle, out int oldNetId);
            if (!doesContain) return;
            
            _logger.Debug($"{player.Name} has left the server. A Personal Vehicle has been found.");
            
            var oldVehicle = (Vehicle) Entity.FromNetworkId(oldNetId);
            if (API.DoesEntityExist(oldVehicle.Handle))
            {
                API.DeleteEntity(oldVehicle.Handle);
                _logger.Debug("Personal Vehicle has been found and deleted!");
            }
            PersonalVehicles.Remove(player.Handle);
        }
        */

        /*private Task OnGarageVehicleCreated(Player player, int vehicleNetId)
        {
            lock (PersonalVehicles)
            {
                var doesContain = PersonalVehicles.TryGetValue(player.Handle, out var oldNetId);
                if (doesContain)
                {
                    var oldVehicle = Entity.FromNetworkId(oldNetId);
                    if (oldVehicle != null)
                    {
                        if (API.DoesEntityExist(oldVehicle.Handle))
                        {
                            API.DeleteEntity(oldVehicle.Handle);
                        }
                    }
                    PersonalVehicles.Remove(player.Handle);
                }

                PersonalVehicles.Add(player.Handle, vehicleNetId);
            }
            
            return Task.CompletedTask;
        }
        */

        private Task<int> OnRequestSirenType(Player player, string vehicle)
        {
            //_logger.Debug($"$Checking siren type for {vehicle}");
            foreach (var v in GarageVehicles)
            {
                if (v.VehicleClass != vehicle) continue;
               //_logger.Debug($"$Siren type code is {v.SirenTypeCode}");
                return Task.FromResult(v.SirenTypeCode);
            }
            return Task.FromResult(0);
        }

        private Task<List<GarageEntity>> OnRequestGaragePoints(Player player)
        {
            List<GarageEntity> AllowedGarages = new List<GarageEntity>();
            foreach (GarageEntity ge in Garages)
            {
                if (ge.AceGroupsRequired.Length == 0)
                {
                    AllowedGarages.Add(ge);
                    continue;
                }

                if (ge.AceGroupsRequired.Any(rg => API.IsPlayerAceAllowed(player.Handle, rg)))
                {
                    AllowedGarages.Add(ge);
                }
            }

            //_logger.Debug($"Returning {AllowedGarages.Count} garage points to player [{player.Name}]");
            return Task.FromResult(AllowedGarages);
        }

        private Task<List<GarageCategory>> OnRequestGarageData(Player player)
        {
            List<GarageCategory> AllowedGarages = new List<GarageCategory>();
            foreach (GarageCategory gc in GarageData)
            {
                if (gc.AceGroupsRequired.Length == 0 && gc.OptionalAceGroups.Length == 0)
                {
                    AllowedGarages.Add(gc);
                    continue;
                }

                if (gc.OptionalAceGroups.Any())
                {
                    if (gc.AceGroupsRequired.Any())
                    {
                        if (gc.AceGroupsRequired.All(s => API.IsPlayerAceAllowed(player.Handle, s)))
                        {
                            if (gc.OptionalAceGroups.Any(s => API.IsPlayerAceAllowed(player.Handle, s)))
                            {
                                AllowedGarages.Add(gc);
                            }
                        }
                    }
                    else
                    {
                        if (gc.OptionalAceGroups.Any(s => API.IsPlayerAceAllowed(player.Handle, s)))
                        {
                            AllowedGarages.Add(gc);
                        }
                    }
                }
                else
                {
                    if (gc.AceGroupsRequired.All(rg => API.IsPlayerAceAllowed(player.Handle, rg)))
                    {
                        if (AllowedGarages.Contains(gc)) continue;
                        AllowedGarages.Add(gc);
                    }
                }
                
            }

            //_logger.Debug($"Returning {AllowedGarages.Count} garage data entries to player [{player.Name}]");
            return Task.FromResult(AllowedGarages);
        }

        private async Task<string> TextFromFileAsync(string path)
        {
            try
            {
                using (var reader = File.OpenText(path))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"Failed to read {path}: {ex.Message}");
                return null;
            }
        }
    }
}
