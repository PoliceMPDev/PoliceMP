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
    class PrisonerTransport
    {

        public static bool eventSpawned = false;
        private static bool eventOnScene = false;
        private static bool eventWarped = false;

        private static Vehicle truckEntity;
        private static Ped ePed1;
        private static Vector3 vehDest;
        private static int truckBlip;
        private static int eventPassenger;

        private static Guid unqGuid;

        private static List<string> transportModels = new List<string> {
            "addpolfordt"
        };
        private static List<string> driverModels = new List<string> {
            "s_m_y_cop_01", "s_f_y_cop_01"
        };
        private static List<string> randomMessages = new List<string> {
            "~b~Transport Officer~s~: I got my taser nice and charged up!", "~b~Transport Officer~s~: Where's the prisoner? Let's go...",
            "~b~Transport Officer~s~: This city's traffic is unreal. Let's hurry this up!", "~b~Transport Officer~s~: Transport on scene! Who's ready to go to jail?"
        };
        private static List<string> grabMessages = new List<string> {
            "~b~Transport Officer~s~: Let's go, now!", "~b~Transport Officer~s~: Van is nice and warm for ya!",
            "~b~Transport Officer~s~: We'll make sure to frisk you back at the station.", "~b~Transport Officer~s~: Haven't I transported you before?"
        };
        private static List<string> leaveMessages = new List<string> {
            "~b~Transport Officer~s~: Don't make a mess back there!", "~b~Transport Officer~s~: I can't wait for shift end...",
            "~b~Transport Officer~s~: We've got another to pick up on the way.", "~b~Transport Officer~s~: This job is getting old!"
        };
        private static readonly Random random = new Random();

        public static async void Loop()
        {
            if (eventSpawned && !eventOnScene)
            {
                if (API.GetDistanceBetweenCoords(ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, vehDest.X, vehDest.Y, vehDest.Z, false) < 10F)
                {
                    Ped player = Game.Player.Character;
                    Ped passenger = (Ped)Entity.FromHandle(eventPassenger);
                    if (!passenger.Exists())
                    {
                        ShowNotification("Unable to locate Prisoner, please try again.");
                        Deregister();
                        return;
                    }
                    eventOnScene = true;
                    API.BlipSiren(truckEntity.Handle);
                    ClientFunctions.DisplayMessage(randomMessages[random.Next(randomMessages.Count())], 2500);
                    // Run to Officer & wait until near
                    while (API.GetDistanceBetweenCoords(passenger.Position.X, passenger.Position.Y, passenger.Position.Z, ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, false) > 5F)
                    {
                        API.TaskGoToEntity(ePed1.Handle, eventPassenger, -1, 5F, 2F, 1073741824, 0);
                        Debug.WriteLine("Transporter pathfinding to Prisoner");
                        await BaseScript.Delay(500);
                    }
                    ClientFunctions.DisplayMessage(grabMessages[random.Next(grabMessages.Count())], 2500);

                    // Wait until Transporter is near Van
                    Vector3 walkLocation = truckEntity.Position - (truckEntity.ForwardVector * 4);
                    API.TaskGoToEntity(ePed1.Handle, eventPassenger, -1, 2F, 1.25F, 1073741824, 0);
                    API.TaskGoToCoordAnyMeans(eventPassenger, walkLocation.X, walkLocation.Y, walkLocation.Z, 1.25F, 0, false, 786603, 2F);
                    int timesWaited = 0;
                    while (API.GetDistanceBetweenCoords(ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, truckEntity.Position.X, truckEntity.Position.Y, truckEntity.Position.Z, false) > 5F)
                    {
                        timesWaited++;
                        if (IsPedJackingOff(ePed1.Handle))
                        {
                            //API.TaskGoToCoordAnyMeans(ePed1.Handle, walkLocation.X, walkLocation.Y, walkLocation.Z, 1F, 0, false, 786603, 0F);
                            //API.TaskFollowNavMeshToCoord(ePed1.Handle, walkLocation.X, walkLocation.Y, walkLocation.Z, 1F, -1, 1F, false, 0F);
                            API.TaskGoToEntity(ePed1.Handle, eventPassenger, -1, 2F, 1.25F, 1073741824, 0);
                        }
                        if (IsPedJackingOff(eventPassenger))
                        {
                            API.TaskGoToCoordAnyMeans(eventPassenger, walkLocation.X, walkLocation.Y, walkLocation.Z, 1.25F, 0, false, 786603, 2F);
                            //API.TaskGoToEntity(eventPassenger, ePed1.Handle, -1, 2F, 1F, 1073741824, 0);
                        }
                        Debug.WriteLine("Transporter/Prisoner pathfinding to Van");
                        await BaseScript.Delay(500);

                        // Don't wait forever in case the area is blocked
                        if (timesWaited >= 30)
                        {
                            //Debug.WriteLine("Warping Transport/Prisoner into vehicle (taking too long)...");
                            ePed1.Task.WarpIntoVehicle(truckEntity, VehicleSeat.Driver);
                            passenger.Task.WarpIntoVehicle(truckEntity, VehicleSeat.LeftRear);
                            await BaseScript.Delay(500);
                            break;
                        }
                    }

                    // Once near Van, open doors and get in
                    API.SetPedAsGroupMember(eventPassenger, API.GetPedGroupIndex(ePed1.Handle));
                    API.SetVehicleDoorOpen(truckEntity.Handle, 2, false, false);
                    API.SetVehicleDoorOpen(truckEntity.Handle, 3, false, false);
                    passenger.Task.EnterVehicle(truckEntity, VehicleSeat.LeftRear, 5000, 1F, 1);

                    // Get in said Van & drive into the sunset
                    int enterAttempts = 0;
                    const int maxEnterAttempts = 60; // 10 seconds
                    const int delayPerAttemptMs = 250;

                    while (passenger.Exists() && !passenger.IsInVehicle() && eventSpawned)
                    {
                        passenger.Task.EnterVehicle(truckEntity, VehicleSeat.LeftRear, 5000, 1F, 1);
                        //Debug.WriteLine("Waiting until Prisoner is in Vehicle");
                        await BaseScript.Delay(delayPerAttemptMs);
                        enterAttempts++;

                        if (enterAttempts >= maxEnterAttempts)
                        {
                            Debug.WriteLine("Timeout reached: Warping prisoner into vehicle.");
                            break;
                        }
                    }

                    if (passenger.Exists() && !passenger.IsInVehicle())
                    {
                        passenger.Task.WarpIntoVehicle(truckEntity, VehicleSeat.LeftRear);
                        await BaseScript.Delay(250);
                        Debug.WriteLine("Prisoner warped into vehicle.");
                    }
                    else if (!passenger.Exists())
                    {
                        Debug.WriteLine("Prisoner ped no longer exists. Aborting entry.");
                    }

                    ePed1.Task.EnterVehicle(truckEntity, VehicleSeat.Driver, 5000, 2F, 1);
                    ePed1.PlayAmbientSpeech("GENERIC_BYE");
                    API.SetVehicleDoorShut(truckEntity.Handle, 2, false);
                    API.SetVehicleDoorShut(truckEntity.Handle, 3, false);
                    await BaseScript.Delay(5000);
                    passenger.PlayAmbientSpeech("GENERIC_CURSE_MED");

                    // Leave
                    API.TaskVehicleDriveWander(ePed1.Handle, truckEntity.Handle, 10F, 319);
                    await BaseScript.Delay(2500);
                    API.BlipSiren(truckEntity.Handle);
                    ClientFunctions.DisplayMessage(leaveMessages[random.Next(leaveMessages.Count())], 2500);

                    // Destroy entities after wait
                    Guid preLoadGuid = unqGuid;
                    await BaseScript.Delay(17500);
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

        private static bool IsPedJackingOff(int pedHandle)
        {
            return API.IsPedFleeing(pedHandle) || API.IsPedStopped(pedHandle) || API.IsPedShooting(pedHandle) || API.IsPedSwimming(pedHandle);
        }

        public static void AssignPedToTransport(int pedHandle)
        {
            Ped ped = (Ped)Entity.FromHandle(pedHandle);
            if (ped == null || !ped.Exists())
            {
                ShowNotification("Unable to locate Prisoner, please try again.");
                return;
            }
            if (!ped.IsCuffed)
            {
                ShowNotification("Please ensure the Prisoner is cuffed before calling for Transport.");
                return;
            }
            eventPassenger = pedHandle;
            Summon();
        }

        public async static void Summon()
        {
            if (eventSpawned)
            {
                Deregister();
                return;
            }
            unqGuid = Guid.NewGuid();
            API.RequestAnimDict("random@arrests");
            ShowNotification("A Transport Unit is in route to your location, please wait.");

            // Assets
            Ped player = Game.Player.Character;
            Vector3 playerPos = player.Position;
            Vector3 spawnPos = Vector3.Zero;
            API.GetNthClosestVehicleNode(playerPos.X, playerPos.Y, playerPos.Z, 75, ref spawnPos, 0, 0, 0);
            truckEntity = await World.CreateVehicle(transportModels[random.Next(transportModels.Count)], spawnPos);
            API.SetEntityHeading(truckEntity.Handle, truckEntity.GetHeadingToEntity(player));
            ePed1 = await World.CreatePed(driverModels[random.Next(driverModels.Count)], spawnPos);
            ePed1.SetIntoVehicle(truckEntity, VehicleSeat.Driver);
            //SetPedRelationshipGroupHash(ePed1.Handle, PoliceMPSummon.civHash);

            // Configuration
            API.GetNthClosestVehicleNode(playerPos.X, playerPos.Y, playerPos.Z, 1, ref vehDest, 0, 0, 0);
            API.TaskVehicleGotoNavmesh(ePed1.Handle, truckEntity.Handle, vehDest.X, vehDest.Y, vehDest.Z, 20F, SummonFunctions.drivingStyle, 0F);
            truckBlip = API.AddBlipForEntity(truckEntity.Handle);
            API.SetBlipColour(truckBlip, 17);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("Prisoner Transport");
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
            ClientFunctions.DisplayMessage($"~b~Officer~s~: Prisoner Transport needed to ~r~{streetName}~s~.", 3500);
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
                ShowNotification("Prisoner Transport taking too long/stuck? Press [F7] to warp them to you.");
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
            Ped passenger = (Ped)Entity.FromHandle(eventPassenger);
            if (passenger.Exists())
            {
                passenger.Delete();
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
            Debug.WriteLine("Cleaned up old Prisoner Transport entities");
        }

        private static void ShowNotification(string message)
            => ClientFunctions.ShowToast("Prisoner Transport", message, "info");
    }
}
