using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Callouts.Shared.Events;
using System.Collections.Generic;
using System.Text;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Callouts.Client
{
    // Needs renamed or split out because it handles more than just spawning
    // A lot of these could go in PoliceMP.Main.Core.Client
    public class SpawnHandler : BaseScript
    {
        [EventHandler(ClientEvents.SPAWN_PED)]
        private async void SpawnPed(int calloutId, string pedKey, uint pedHash, Vector3 location, uint weaponHash)
        {
            var ped = await World.CreatePed(new Model((PedHash)pedHash), location);
            ped.IsPersistent = true;
            ped.Weapons.Give((WeaponHash)weaponHash, 500, true, true);
            API.SetNetworkIdCanMigrate(ped.NetworkId, true);
            API.DecorSetInt(ped.Handle, "CalloutId", calloutId);
            API.NetworkRequestControlOfNetworkId(ped.NetworkId);
            
            int controlCount = 0;
            
            while (!API.NetworkHasControlOfEntity(ped.NetworkId) && controlCount < 10)
            {
                API.NetworkRequestControlOfNetworkId(ped.NetworkId);
                controlCount++;
                await Delay(10);
            }

            if (controlCount >= 10)
            {
                Debug.WriteLine($"Unable to get control of entity {ped.NetworkId}");
                return;
            }
            
            API.SetEntityAsMissionEntity(ped.Handle, true, true);
            API.SetNetworkIdExistsOnAllMachines(ped.NetworkId, true);
            for (int i = 0; i <= 256; i++)
            {
                API.SetNetworkIdSyncToPlayer(ped.NetworkId, API.GetPlayerFromServerId(i), true);
            }

            //API.NetworkSetEntityVisibleToNetwork(ped.Handle, false);

            ped.Rotation = new Vector3(PoliceMpRandom.Next(0, 180), PoliceMpRandom.Next(0, 180), PoliceMpRandom.Next(0, 180));

            ServerEventAPI.ClientSpawnedPed(calloutId, pedKey, ped.NetworkId);
            SetPedRandomIdleAnim(ped.NetworkId);
        }

        [EventHandler(ClientEvents.SET_PED_DRIVE_TO_FAST)]
        private void SetPedDriveToFast(int pedNetworkId, int vehicleNetworkId, Vector3 position)
        {
            var ped = (Ped)Entity.FromNetworkId(pedNetworkId);
            var vehicle = (Vehicle)Entity.FromNetworkId(vehicleNetworkId);
            if (ped == null || vehicle == null) return;

            ped.Task.DriveTo(vehicle, position, 30f, 400f, 787004);
        }

        [EventHandler(ClientEvents.APPLY_PED_DAMAGE)]
        private void ApplyPedDamage(int pedNetworkId, int damage)
        {
            var ped = (Ped)Entity.FromNetworkId(pedNetworkId);
            if (ped == null) return;

            ped.ApplyDamage(damage);
        }

        [EventHandler(ClientEvents.SET_PED_FLEE_FROM_PLAYER)]
        private void SetPedFleeFromPlayer(int pedNetworkId)
        {
            var ped = (Ped)Entity.FromNetworkId(pedNetworkId);
            if (ped == null) return;

            ped.Task.ReactAndFlee(Game.PlayerPed);
        }

        [EventHandler(ClientEvents.SET_PED_ATTACK_PLAYER)]
        private void SetPedAttackPlayer(int pedNetworkId)
        {
            var ped = (Ped)Entity.FromNetworkId(pedNetworkId);
            if (ped == null) return;

            ped.Task.FightAgainst(Game.PlayerPed);
        }

        [EventHandler(ClientEvents.SET_PED_ATTACK_PED)]
        private void SetPedAttackPed(int pedNetworkId1, int pedNetworkId2)
        {
            var ped1 = (Ped)Entity.FromNetworkId(pedNetworkId1);
            var ped2 = (Ped)Entity.FromNetworkId(pedNetworkId2);
            if (ped1 == null || ped2 == null) return;

            ped1.Task.FightAgainst(ped2);
            ped2.Task.FightAgainst(ped1);
        }

        [EventHandler(ClientEvents.SET_PED_TASK_WANDER)]
        private void SetPedTaskWander(int pedNetworkId)
        {
            var ped = (Ped)Entity.FromNetworkId(pedNetworkId);
            if (ped == null) return;

            ped.Task.WanderAround();
        }

        [EventHandler(ClientEvents.SET_PED_TASK_DRIVE_WANDER)]
        private void SetPedTaskDriveWander(int pedNetworkId, int vehicleNetworkId)
        {
            var ped = (Ped)Entity.FromNetworkId(pedNetworkId);
            var vehicle = (Vehicle)Entity.FromNetworkId(vehicleNetworkId);
            if (ped == null || vehicle == null) return;

            API.TaskVehicleDriveWander(ped.Handle, vehicle.Handle, 15f, 447);
        }

        [EventHandler(ClientEvents.SPAWN_VEHICLE)]
        private async void SpawnVehicle(int calloutId,
            string vehicleKey,
            string plate,
            uint vehicleHash,
            Vector3 location,
            int colour,
            float bodyHealth,
            float engineHealth,
            float fuelTankHealth)
        {
            var vehicle = await World.CreateVehicle(new Model((VehicleHash)vehicleHash), location);
            if (string.IsNullOrEmpty(plate)) plate = GeneratePlate();
            API.SetVehicleNumberPlateText(vehicle.Handle, plate); // make sure the random fault generator doesn't fuck this
            vehicle.IsPersistent = true;
            API.SetNetworkIdCanMigrate(vehicle.NetworkId, true);
            API.NetworkRequestControlOfNetworkId(vehicle.NetworkId);
            API.SetNetworkIdExistsOnAllMachines(vehicle.NetworkId, true);
            foreach (var player in Players)
                API.SetNetworkIdSyncToPlayer(vehicle.NetworkId, player.Handle, true);

            API.DecorSetInt(vehicle.Handle, "CalloutId", calloutId);
            vehicle.Mods.PrimaryColor = (VehicleColor)colour;
            vehicle.BodyHealth = bodyHealth;
            vehicle.EngineHealth = engineHealth;
            vehicle.PetrolTankHealth = fuelTankHealth;

            vehicle.Rotation = new Vector3(0f, 0f, PoliceMpRandom.Next(0, 360));

            ServerEventAPI.ClientSpawnedVehicle(calloutId, vehicleKey, vehicle.NetworkId, plate);
        }

        [EventHandler(ClientEvents.SET_PED_INTO_VEHICLE)]
        private void SetPedIntoVehicle(int pedNetworkId, int vehicleNetworkId, int seat)
        {
            var vehicle = (Vehicle)Entity.FromNetworkId(vehicleNetworkId);
            var ped = (Ped)Entity.FromNetworkId(pedNetworkId);

            if (vehicle == null || ped == null) return;

            if (seat != (int)VehicleSeat.Driver)
                seat = (int)VehicleSeat.Any;
            ped.SetIntoVehicle(vehicle, (VehicleSeat)seat);
        }

        [EventHandler(ClientEvents.SPAWN_RANDOM_PED)]
        private void SpawnRandomPed(int calloutId, Vector3 location)
        {
            var ped = World.CreateRandomPed(location);
            ped.IsPersistent = true;
            API.DecorSetInt(ped.Handle, "CalloutId", calloutId);
        }

        [EventHandler(ClientEvents.SPAWN_RANDOM_VEHICLE)]
        private async void SpawnRandomVehicle(int calloutId, string vehicleKey, Vector3 location)
        {
            var vehicle = await World.CreateRandomVehicle(location);
            vehicle.IsPersistent = true;
            API.DecorSetInt(vehicle.Handle, "CalloutId", calloutId);

            ServerEventAPI.ClientSpawnedVehicle(calloutId, vehicleKey, vehicle.NetworkId, "");
        }

        [EventHandler(ClientEvents.SET_PED_ANIM)]
        private void SetPedAnim(int networkId, string animDict, string animName)
        {
            var ped = (Ped)Entity.FromNetworkId(networkId);
            if (ped == null) return;

            ped.Task.Cower(-1);
        }

        [EventHandler(ClientEvents.SET_PED_RANDOM_IDLE_ANIM)]
        private void SetPedRandomIdleAnim(int networkId)
        {
            var ped = (Ped)Entity.FromNetworkId(networkId);
            if (ped == null) return;

            var index = PoliceMpRandom.Next(0, 6);

            if (index == 0) ped.Task.UseMobilePhone();
            else if (index == 1) ped.Task.StartScenario("WORLD_HUMAN_AA_SMOKE", ped.Position);
            else if (index == 2) ped.Task.StartScenario("WORLD_HUMAN_DRINKING", ped.Position);
            else if (index == 3) ped.Task.StartScenario("WORLD_HUMAN_HANG_OUT_STREET", ped.Position);
            else if (index == 4) ped.Task.StartScenario("WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT", ped.Position);
            else if (index == 5) ped.Task.StartScenario("WORLD_HUMAN_AA_COFFEE", ped.Position);
        }

        [EventHandler(ClientEvents.SET_VEHICLE_PHYSICAL_DAMAGE)]
        private void SetVehiclePhysicalDamage(int networkId)
        {
            var vehicle = (Vehicle)Entity.FromNetworkId(networkId);
            if (vehicle == null) return;
            for (var i = 0; i < 100; i++)
            {
                API.SetVehicleDamage(vehicle.Handle,
                    PoliceMpRandom.Next(-2, 2),
                    PoliceMpRandom.Next(-2, 2),
                    PoliceMpRandom.Next(-2, 2),
                    5000f,
                    50f,
                    true);
            }
        }

        [EventHandler(ClientEvents.SET_VEHICLE_ROTATION)]
        private void SetVehicleRotation(int networkId, float rotation)
        {
            var vehicle = (Vehicle)Entity.FromNetworkId(networkId);
            if (vehicle == null) return;

            vehicle.Rotation = new Vector3(0f, 0f, rotation);
        }

        [EventHandler(ClientEvents.GET_NEXT_POSITION_ON_STREET)]
        private void GetNextPositionOnStreet(string requestGuid, List<dynamic> args)
        {
            var nextPosition = ClientFunctions.GetNextPositionOnStreet(Game.PlayerPed.Position, 200, 500);
            var jsonString = JsonConvert.SerializeObject(nextPosition);
            TriggerServerEvent(ServerEvents.RECEIVE_REQUEST_RESULT, requestGuid, jsonString);
        }

        [EventHandler(ClientEvents.GET_NEXT_POSITION_ON_SIDEWALK)]
        private void GetNextPositionOnSidewalk(string requestGuid, List<dynamic> args)
        {
            var nextPosition = World.GetNextPositionOnSidewalk(Game.PlayerPed.Position);
            var jsonString = JsonConvert.SerializeObject(nextPosition);
            TriggerServerEvent(ServerEvents.RECEIVE_REQUEST_RESULT, requestGuid, jsonString);
        }

        private static string GeneratePlate()
        {
            int[] yearStart = { 0, 1, 5, 6 };
            var builder = new StringBuilder();

            builder.Append(GetLetter().ToString().ToUpper());
            builder.Append(GetLetter().ToString().ToUpper());
            builder.Append(yearStart[PoliceMpRandom.Next(yearStart.Length - 1)]);
            builder.Append(PoliceMpRandom.Next(9));
            builder.Append(" ");
            builder.Append(GetLetter().ToString().ToUpper());
            builder.Append(GetLetter().ToString().ToUpper());
            builder.Append(GetLetter().ToString().ToUpper());

            return builder.ToString();
        }

        private static char GetLetter()
        {
            var num = PoliceMpRandom.Next(26);
            var let = (char)('a' + num);
            return let;
        }

        [EventHandler(ClientEvents.REMOVE_PED_ELEGANTLY)]
        private void RemovePedElegantly(int pedNetworkId)
        {
            API.RemovePedElegantly(ref pedNetworkId);
        }

        [EventHandler(ClientEvents.RUN_GANG_WAR)]
        private async void RunGangWar(string pgroupString, string ggroupString)
        {
            try
            {
                if (string.IsNullOrEmpty(pgroupString) || string.IsNullOrEmpty(ggroupString))
                {
                    Debug.WriteLine("pgroup or ggorup string is empty!");
                    Debug.WriteLine($"pgroup: {pgroupString}");
                    Debug.WriteLine($"ggroup: {ggroupString}");
                    return;
                }
                
                var pgroupNetworkIds = JsonConvert.DeserializeObject<List<int>>(pgroupString);
                var ggroupNetworkIds = JsonConvert.DeserializeObject<List<int>>(ggroupString);

                if (pgroupNetworkIds == null || ggroupNetworkIds == null)
                {
                    Debug.WriteLine($"pggroup or ggroup network IDs are null!");
                    Debug.WriteLine($"pgroup: {pgroupString}");
                    Debug.WriteLine($"ggroup: {ggroupString}");
                    return;
                }

                uint phashKey = (uint)API.GetHashKey("pmppgroup");
                API.AddRelationshipGroup("pmppgroup", ref phashKey);

                uint ghashKey = (uint)API.GetHashKey("pmpggroup");
                API.AddRelationshipGroup("pmpggroup", ref ghashKey);

                API.SetRelationshipBetweenGroups(5, phashKey, ghashKey);
                API.SetRelationshipBetweenGroups(5, ghashKey, phashKey);
                API.SetRelationshipBetweenGroups(5, ghashKey, (uint)API.GetHashKey("PLAYER"));
                API.SetRelationshipBetweenGroups(5, phashKey, (uint)API.GetHashKey("PLAYER"));

                // Construct list of pgroupPeds
                var pgroupPeds = new List<Ped>();
                foreach (var pedId in pgroupNetworkIds)
                {
                    if (!API.NetworkDoesEntityExistWithNetworkId(pedId))
                    {
                        Debug.WriteLine("This entity doesn't exist!");
                        continue;
                    }
                    
                    var ped = (Ped)Entity.FromNetworkId(pedId);

                    if (ped == null) continue;

                    if (API.GetEntityType(ped.Handle) != 1)
                    {
                        Debug.WriteLine("Entity is not a ped!");
                        continue;
                    }
                    
                    pgroupPeds.Add(ped);

                    API.NetworkRequestControlOfEntity(ped.Handle);

                    int requestCount = 0;
                    
                    while (!API.NetworkHasControlOfEntity(ped.Handle) && requestCount < 10)
                    {
                        API.NetworkRequestControlOfEntity(ped.Handle);
                        await Delay(10);
                        requestCount++;
                    }

                    if (requestCount >= 10)
                    {
                        Debug.WriteLine($"Unable to request control of entity {ped.Handle}.");
                        continue;
                    }
                    
                    API.SetPedRelationshipGroupHash(ped.Handle, phashKey);
                    API.TaskCombatHatedTargetsAroundPed(ped.Handle, 1000, 0);
                }

                // Construct list of ggroupPeds
                var ggroupPeds = new List<Ped>();
                foreach (var pedId in ggroupNetworkIds)
                {
                    if (!API.NetworkDoesEntityExistWithNetworkId(pedId))
                    {
                        Debug.WriteLine("This entity doesn't exist!");
                        continue;
                    }
                    
                    var ped = (Ped)Entity.FromNetworkId(pedId);

                    if (ped == null) continue;
                    
                    if (API.GetEntityType(ped.Handle) != 1)
                    {
                        Debug.WriteLine("Entity is not a ped!");
                        continue;
                    }
                    
                    ggroupPeds.Add(ped);
                    
                    API.NetworkRequestControlOfEntity(ped.Handle);

                    int requestCount = 0;
                    
                    while (!API.NetworkHasControlOfEntity(ped.Handle) && requestCount < 10)
                    {
                        API.NetworkRequestControlOfEntity(ped.Handle);
                        await Delay(10);
                        requestCount++;
                    }

                    if (requestCount >= 10)
                    {
                        Debug.WriteLine($"Unable to request control of entity {ped.Handle}.");
                        continue;
                    }
                    
                    API.SetPedRelationshipGroupHash(ped.Handle, ghashKey);
                    API.TaskCombatHatedTargetsAroundPed(ped.Handle, 1000, 0);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
        [EventHandler(ClientEvents.RUN_BANK_HEIST)]
        private void RunBankHeist(string badPedString, string civPedString)
        {
            Debug.WriteLine(badPedString);
            Debug.WriteLine(civPedString);

            var badPedNetworkIds = JsonConvert.DeserializeObject<List<int>>(badPedString);
            var civPedNetworkIds = JsonConvert.DeserializeObject<List<int>>(civPedString);

            uint badHashKey = (uint)API.GetHashKey("pmpbggroup");
            API.AddRelationshipGroup("pmpbggroup", ref badHashKey);
            API.SetRelationshipBetweenGroups(5, badHashKey, (uint)API.GetHashKey("PLAYER"));
            API.SetRelationshipBetweenGroups(5, (uint)API.GetHashKey("PLAYER"), badHashKey);

            foreach (var pedId in badPedNetworkIds)
            {
                if (pedId == 0)
                {
                    continue;
                }
                var ped = (Ped)Entity.FromNetworkId(pedId);
                API.SetPedRelationshipGroupHash(ped.Handle, badHashKey);
            }

            foreach (var pedId in civPedNetworkIds)
            {
                if (pedId == 0)
                {
                    continue;
                }
                var ped = (Ped)Entity.FromNetworkId(pedId);
                API.TaskCower(ped.Handle, 9999);
            }
        }
    }
}
