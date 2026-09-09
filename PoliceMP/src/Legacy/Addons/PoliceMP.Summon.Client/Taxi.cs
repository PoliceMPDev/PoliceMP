using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using static CitizenFX.Core.Native.API;
using static PoliceMP.Summon.Client.SummonFunctions;

namespace PoliceMP.Summon.Client
{
    class Taxi : BaseScript
    {
        // Event Entities
        private static Vehicle taxiEntity;
        private static Ped ePed1;
        private static int taxiBlip;
        public static int eventPassenger;

        private static VehicleDestination vehDest;
        private static Guid unqGuid;

        // Event Flags
        public static bool eventSpawned = false;
        public static bool eventOnScene = false;
        public static bool eventSceneOver = false;
        public static bool eventWarped = false;

        private static readonly Random random = new Random();

        public static List<string> driverModels = new List<string> {
            "a_m_y_beachvesp_01", "a_f_y_eastsa_02", "g_m_y_korean_01"
        };

        public static async void Loop()
        {
            if (eventSpawned && !eventOnScene)
            {
                if (GetDistanceBetweenCoords(ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, vehDest.Position.X, vehDest.Position.Y, vehDest.Position.Z, false) < 10F)
                {
                    eventOnScene = true;
                    await taxiEntity.HonkHornAsync();
                    await taxiEntity.HonkHornAsync();
                    ClientFunctions.DisplayMessage($"~b~Officer~s~: Your Taxi has arrived.", 2500);
                    PlayAmbientSpeech1(eventPassenger, "GENERIC_BYE", "SPEECH_PARAMS_STANDARD");
                    ClearPedTasksImmediately(eventPassenger);
                    ClearPedSecondaryTask(eventPassenger);
                    TaskEnterVehicle(eventPassenger, taxiEntity.Handle, -1, 0, 3F, 1, 0);
                    await BaseScript.Delay(10000);
                    TaskWarpPedIntoVehicle(eventPassenger, taxiEntity.Handle, 0);
                }
            }
            if (eventPassenger != default(int) && eventSceneOver == false)
            {
                if (IsPedInAnyVehicle(eventPassenger, false))
                {
                    Ped player = Game.Player.Character;
                    eventSceneOver = true;
                    ePed1.PlayAmbientSpeech("GENERIC_HI");
                    TaskVehicleDriveWander(ePed1.Handle, taxiEntity.Handle, 10F, SummonFunctions.drivingStyle);

                    // Destroy entities after wait
                    Guid preLoadGuid = unqGuid;
                    await BaseScript.Delay(20000);
                    if (preLoadGuid != unqGuid)
                    {
                        return;
                    }
                    NetworkFadeOutEntity(taxiEntity.Handle, true, false);
                    await BaseScript.Delay(1000);
                    if (eventSpawned)
                    {
                        Deregister();
                    }
                }
            }
        }

        public static void AssignPedToTaxi(int pedHandle)
        {
            eventPassenger = pedHandle;
            Taxi.Summon();
        }

        private async static void Summon()
        {
            if (eventSpawned)
            {
                Deregister();
                return;
            }
            Deregister();
            unqGuid = Guid.NewGuid();
            RequestAnimDict("random@arrests");
            ShowNotification("A Taxi is in route to your location, please wait.");

            // Assets
            Ped player = Game.Player.Character;
            Vector3 playerPos = player.Position;
            var spawnLocation = playerPos + (player.ForwardVector * 75) + (player.RightVector * 75);
            taxiEntity = await World.CreateVehicle("taxi", spawnLocation);
            taxiEntity.PlaceOnNextStreet();
            string driverModel = driverModels[random.Next(driverModels.Count)];
            await ClientFunctions.LoadModelAsync(driverModel);
            ePed1 = await World.CreatePed(driverModel, taxiEntity.Position);
            ePed1.SetIntoVehicle(taxiEntity, VehicleSeat.Driver);

            // Configuration
            vehDest = await SummonFunctions.PedGetClosestSideOfRoad(player);
            TaskVehicleGotoNavmesh(ePed1.Handle, taxiEntity.Handle, vehDest.Position.X, vehDest.Position.Y, vehDest.Position.Z, 20F, SummonFunctions.drivingStyle, 0F);
            taxiBlip = AddBlipForEntity(taxiEntity.Handle);
            SetBlipColour(taxiBlip, 5);
            BeginTextCommandSetBlipName("STRING");
            AddTextComponentString("Taxi");
            EndTextCommandSetBlipName(taxiBlip);
            eventSpawned = true;

            // Radio anim
            PlaySoundFromEntity(-1, "Remote_Control_Close", player.Handle, "PI_Menu_Sounds", true, 0);
            TaskPlayAnim(player.Handle, "random@arrests", "generic_radio_chatter", 8.0F, 2.0F, -1, 50, 2.0F, false, false, false);
            if (IsEntityPlayingAnim(player.Handle, "random@arrests", "generic_radio_chatter", 3))
            {
                ClearPedSecondaryTask(player.Handle);
                SetCurrentPedWeapon(player.Handle, (uint)GetHashKey("GENERIC_RADIO_CHATTER"), true);
            }
            string streetName = World.GetStreetName(player.Position);
            ClientFunctions.DisplayMessage($"~b~Officer~s~: Taxi needed to ~r~{streetName}~s~ for someone needing a ride.", 3500);
            await BaseScript.Delay(2500);
            ClearPedTasks(player.Handle);

            // Show Warp hotkey
            Guid preLoadGuid = unqGuid;
            await BaseScript.Delay(15000);
            if (preLoadGuid != unqGuid)
            {
                return;
            }
            if (!eventOnScene && eventSpawned)
            {
                ShowNotification("Taxi taking too long/stuck? Press [F7] to warp them to you.");
            }
        }

        public static void Warp()
        {
            if (!eventWarped && eventSpawned && !eventOnScene)
            {
                taxiEntity.Position = vehDest.Position;
                taxiEntity.Heading = vehDest.Heading;
                eventWarped = true;
            }
        }

        public static void Deregister()
        {
            if (taxiEntity != null)
            {
                taxiEntity.Delete();
            }
            if (ePed1 != null)
            {
                ePed1.Delete();
            }
            if (taxiBlip != default(int))
            {
                RemoveBlip(ref taxiBlip);
            }
            if (eventSceneOver && eventPassenger != default(int))
            {
                DeleteEntity(ref eventPassenger);
            }
            eventSpawned = false;
            eventSceneOver = false;
            eventWarped = false;
            eventOnScene = false;
            Debug.WriteLine("Cleaned up old Taxi entities");
        }

        private static void ShowNotification(string message)
            => ClientFunctions.ShowToast("Taxi", message, "info");
    }
}