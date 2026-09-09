using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;

namespace PoliceMP.Summon.Client
{
    class TowTruck
    {
        public static bool eventSpawned = false;
        private static bool eventOnScene = false;
        private static bool eventWarped = false;

        private static Vehicle truckEntity;
        private static Ped ePed1;
        private static Vector3 vehDest;
        private static int truckBlip;
        private static int targetVehicleId;

        private static Guid unqGuid;

        private static List<string> towtruckModels = new List<string> {
            "towtruck", // "towtruck2" // towtruck2 is a non-flatbed model
        };
        private static List<string> driverModels = new List<string> {
            "s_m_m_autoshop_01", "s_m_y_construct_01", "s_m_y_construct_02"
        };
        private static List<VehicleClass> untowableClasses = new List<VehicleClass>
        {
            VehicleClass.Boats, VehicleClass.Emergency, VehicleClass.Helicopters, VehicleClass.Service,
            VehicleClass.Military, VehicleClass.Planes, VehicleClass.Trains
        };
        private static List<string> randomMessages = new List<string> {
            "~b~Tow Truck Driver~s~: I got this one officer! We'll treat it real nice, we promise.", "~b~Tow Truck Driver~s~: Man, this thing is hammered!",
            "~b~Tow Truck Driver~s~: The traffic was the worst! I hate this city...", "~b~Tow Truck Driver~s~: Dang, that thing looks heavy..."
        };
        private static readonly Random random = new Random();

        public static async void Loop()
        {
            if (eventSpawned && !eventOnScene)
            {
                if (API.GetDistanceBetweenCoords(ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, vehDest.X, vehDest.Y, vehDest.Z, false) < 10F)
                {
                    // Make sure Vehicle still exists
                    Vehicle vehicle = (Vehicle)Entity.FromHandle(targetVehicleId);
                    if (vehicle == null || !vehicle.Exists())
                    {
                        ShowNotification("The Vehicle could not be towed. Please try again if needed.");
                        Deregister();
                        return;
                    }

                    eventOnScene = true;
                    await truckEntity.HonkHornAsync();
                    ClientFunctions.DisplayMessage(randomMessages[random.Next(randomMessages.Count())], 2500);
                    API.PlayAmbientSpeech1(ePed1.Handle, "GENERIC_BYE", "SPEECH_PARAMS_SHOUTED");
                    vehicle.AttachTo(truckEntity, new Vector3(0, -1F, 0.75F));
                    vehicle.Rotation = new Vector3(0, 0, 20F);
                    API.SetVehicleDoorsShut(targetVehicleId, true);
                    foreach (Ped occupant in vehicle.Occupants)
                    {
                        occupant.Task.LeaveVehicle(LeaveVehicleFlags.WarpOut);
                    }
                    API.TaskVehicleDriveWander(ePed1.Handle, truckEntity.Handle, 10F, SummonFunctions.drivingStyle);

                    // Destroy entities after wait
                    Guid preLoadGuid = unqGuid;
                    await BaseScript.Delay(20000);
                    if (preLoadGuid != unqGuid)
                    {
                        return;
                    }
                    API.NetworkFadeOutEntity(truckEntity.Handle, true, false);
                    await BaseScript.Delay(1000);
                    if (eventSpawned)
                    {
                        Deregister();
                    }
                }
            }
        }

        public async static void Summon(int vehicleId)
        {
            Debug.WriteLine("testy test test");
            if (eventSpawned)
            {
                Deregister();
                return;
            }

            var targetVehicle = vehicleId != default ? new Vehicle(vehicleId) : Game.PlayerPed.GetVehicleInFront();

            if (targetVehicle == null)
            {
                ShowNotification("Unable to locate the Vehicle to be towed, please move closer and face it or try again.");
                return;
            }
            targetVehicleId = targetVehicle.Handle;
            if (targetVehicle.Occupants.Length > 0)
            {
                ShowNotification("The Vehicle must be empty, please remove any occupants and try again.");
                return;
            }
            if (untowableClasses.Contains(targetVehicle.ClassType))
            {
                ShowNotification("This Vehicle cannot be towed, please try a different Vehicle.");
                return;
            }
            if (targetVehicle.IsExplosionProof)
            {
                ShowNotification("This Vehicle has already had a Tow Truck called for it, please try a different Vehicle.");
                return;
            }
            targetVehicle.IsExplosionProof = true;
            targetVehicle.IsDriveable = false;
            targetVehicle.LockStatus = VehicleLockStatus.Locked;

            unqGuid = Guid.NewGuid();
            API.RequestAnimDict("random@arrests");
            ShowNotification("A Tow Truck is in route to your location, please wait.");

            // Assets
            Ped player = Game.Player.Character;
            Vector3 playerPos = player.Position;
            Vector3 spawnPos = Vector3.Zero;
            float spawnHdg = 0F;
            int unk1 = 0;

            API.GetNthClosestVehicleNodeWithHeading(playerPos.X, playerPos.Y, playerPos.Z, 75, ref spawnPos, ref spawnHdg, ref unk1, 0, 0, 0);
            truckEntity = await World.CreateVehicle(towtruckModels[random.Next(towtruckModels.Count)], spawnPos, spawnHdg);
            ePed1 = await World.CreatePed(driverModels[random.Next(driverModels.Count)], spawnPos);
            ePed1.SetIntoVehicle(truckEntity, VehicleSeat.Driver);

            // Configuration
            API.GetNthClosestVehicleNode(playerPos.X, playerPos.Y, playerPos.Z, 1, ref vehDest, 0, 0, 0);
            API.TaskVehicleGotoNavmesh(ePed1.Handle, truckEntity.Handle, vehDest.X, vehDest.Y, vehDest.Z, 20F, SummonFunctions.drivingStyle, 0F);
            API.SetDriveTaskDrivingStyle(ePed1.Handle, SummonFunctions.drivingStyle);
            API.SetDriverAbility(ePed1.Handle, 1F);
            truckBlip = API.AddBlipForEntity(truckEntity.Handle);
            API.SetBlipColour(truckBlip, 28);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("Tow Truck");
            API.EndTextCommandSetBlipName(truckBlip);
            eventSpawned = true;

            // Radio anim
            API.PlaySoundFromEntity(-1, "Remote_Control_Close", player.Handle, "PI_Menu_Sounds", true, 0);
            API.TaskPlayAnim(player.Handle, "random@arrests", "generic_radio_chatter", 8.0F, 2.0F, -1, 50, 2.0F, false, false, false);
            if (API.IsEntityPlayingAnim(player.Handle, "random@arrests", "generic_radio_chatter", 3))
            {
                API.ClearPedSecondaryTask(player.Handle);
                API.SetCurrentPedWeapon(player.Handle, (uint)API.GetHashKey("GENERIC_RADIO_CHATTER"), true);
            }

            // Show message
            string streetName = World.GetStreetName(player.Position);
            string vehicleName = targetVehicle.LocalizedName;
            string numberPlate = API.GetVehicleNumberPlateText(targetVehicleId);
            ClientFunctions.DisplayMessage($"~b~Officer~s~: Tow Truck needed to ~r~{streetName}~s~ for a ~r~{vehicleName}~s~, Plate # ~r~{numberPlate}~s~.", 3500);
            await BaseScript.Delay(2500);
            API.ClearPedTasks(player.Handle);

            // Show Warp hotkey
            Guid preLoadGuid = unqGuid;
            await BaseScript.Delay(15000);
            if (preLoadGuid != unqGuid)
            {
                return;
            }
            if (!eventOnScene && eventSpawned)
            {
                ShowNotification("Tow Truck taking too long/stuck? Press [F7] to warp them to you.");
            }
        }

        public static void Warp()
        {
            if (!eventWarped && eventSpawned && !eventOnScene)
            {
                truckEntity.Position = vehDest;
                eventWarped = true;
                ePed1.Task.ClearAll();
            }
        }

        private static void Deregister()
        {
            if (targetVehicleId != default(int) && targetVehicleId != -1)
            {
                Vehicle vehicle = (Vehicle)Entity.FromHandle(targetVehicleId);
                // .Exists() was causing a un-set object exception when the Vehicle gets removed
                if (vehicle != null /*|| vehicle.Exists()*/)
                {
                    vehicle.IsExplosionProof = false;
                    vehicle.IsDriveable = true;
                    vehicle.LockStatus = VehicleLockStatus.Unlocked;
                    targetVehicleId = -1;
                }
            }
            if (truckEntity != null)
            {
                truckEntity.Delete();
            }
            if (ePed1 != null)
            {
                ePed1.Delete();
            }
            if (truckBlip != default(int))
            {
                API.RemoveBlip(ref truckBlip);
            }
            eventSpawned = false;
            eventWarped = false;
            eventOnScene = false;
            Debug.WriteLine("Cleaned up old Tow Truck entities");
        }

        private static void ShowNotification(string message)
            => ClientFunctions.ShowToast("Tow Truck", message, "info");
    }
}
