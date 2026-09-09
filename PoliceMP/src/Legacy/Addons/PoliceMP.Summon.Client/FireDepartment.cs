using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using PoliceMP.Main.Core.Client;
using static CitizenFX.Core.Native.API;
using static PoliceMP.Summon.Client.SummonFunctions;
using CitizenFX.Core.Native;

namespace PoliceMP.Summon.Client
{
    class FireDepartment
    {
        // Event Entities
        private static int firesIterated = 0;
        private static Vehicle firetruckEntity;
        private static Ped ePed1;
        private static Ped ePed2;
        private static Ped ePed3;
        private static Ped ePed4;
        private static int firetruckBlip;

        private static Dictionary<int, bool> pedBusy = new Dictionary<int, bool>();
        private static VehicleDestination vehDest;
        private static Guid unqGuid;

        // Event Flags
        public static bool eventSpawned = false;
        public static bool eventOnScene = false;
        public static bool eventWarped = false;
        public static bool eventSceneStarted = false;
        public static bool eventSceneOver = false;

        private static readonly Random random = new Random();

        // Event Messages
        public static List<string> welcomeMessages = new List<string> {
            "~b~Fireman~s~: Another fire to destroy!", "~b~Fireman~s~: Hope everyone brought their goggles!"
        };
        public static List<string> progressMessages = new List<string> {
            "~b~Fireman~s~: Oh yes! It's going out!", "~b~Fireman~s~: Come on, let's get it done, it's going down!"
        };
        public static List<string> randomMessages = new List<string> {
            "~b~Fireman~s~: Oh man, I forgot my extinguisher!", "~b~Fireman~s~: Let's hit the lunch spot!"
        };

        public async static void Loop()
        {
            if (eventSpawned)
            {
                Ped player = Game.Player.Character;

                // Check if the get-out event needs to happen
                if (!eventOnScene && GetDistanceBetweenCoords(ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, vehDest.Position.X, vehDest.Position.Y, vehDest.Position.Z, false) < 15)
                {
                    firetruckEntity.IsSirenActive = false;
                    eventOnScene = true;
                    ShowNotification("The Firetruck has arrived, ensure the scene is secure.");
                    if (!eventWarped)
                    {
                        ClearPedTasks(ePed1.Handle);
                        SetVehicleForwardSpeed(firetruckEntity.Handle, 0F);
                        ePed1.Task.DriveTo(firetruckEntity, firetruckEntity.Position, 0F, 0F, 262972);
                        TaskEveryoneLeaveVehicle(firetruckEntity.Handle);
                    }

                    // Prepare Scenario
                    ClientFunctions.DisplayMessage(welcomeMessages[random.Next(welcomeMessages.Count)], 2500);
                    if (!eventWarped)
                    {
                        TaskGoToEntity(ePed1.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                        //TaskGoToEntity(ePed2.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                        TaskGotoEntityOffsetXy(ePed2.Handle, player.Handle, -1, 3, 3, 0, 0, true);
                        TaskGotoEntityOffsetXy(ePed3.Handle, player.Handle, -1, 6, 6, 0, 0, true);
                        TaskGotoEntityOffsetXy(ePed4.Handle, player.Handle, -1, 10, 10, 0, 0, true);
                        BlipSiren(firetruckEntity.Handle);
                        await BaseScript.Delay(500);
                        ePed1.PlayAmbientSpeech("GENERIC_HI", SpeechModifier.ForceMegaphone);
                    }
                }
                if (!eventSceneStarted && eventOnScene)
                {
                    // Wait until Ped is near player
                    if (GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, false) < 5F)
                    {
                        eventSceneStarted = true;
                        ClearPedTasks(ePed1.Handle);
                        ClientFunctions.DisplayMessage("~b~Fireman~s~: Can you show me to the fire?", 2500);
                        await ClientFunctions.LoadModelAsync("prop_devin_box_dummy_01");
                        PedExtinguishFires(ePed1);
                        PedExtinguishFires(ePed2);
                        PedExtinguishFires(ePed3);
                        PedExtinguishFires(ePed4);
                        await BaseScript.Delay(500);
                        while (eventSpawned && (pedBusy[ePed1.Handle] || pedBusy[ePed2.Handle] || pedBusy[ePed3.Handle] || pedBusy[ePed4.Handle]))
                        {
                            if (!pedBusy[ePed1.Handle])
                            {
                                TaskGoToEntity(ePed1.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                            }
                            if (!pedBusy[ePed2.Handle])
                            {
                                TaskGoToEntity(ePed2.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                            }
                            if (!pedBusy[ePed3.Handle])
                            {
                                TaskGoToEntity(ePed3.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                            }
                            if (!pedBusy[ePed4.Handle])
                            {
                                TaskGoToEntity(ePed4.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                            }
                            //Debug.WriteLine($"Firemen busy: #1: {pedBusy[ePed1.Handle]}; #2: {pedBusy[ePed2.Handle]}; #3: {pedBusy[ePed3.Handle]}; #4: {pedBusy[ePed4.Handle]}");
                            await BaseScript.Delay(500);
                        }

                        // Check if we actually found any fires
                        if (firesIterated == 0)
                        {
                            ShowNotification("Unable to locate any fires, try to stand closer and call the Firetruck again.");
                        }

                        // Wrap up scenario
                        ClearPedTasksImmediately(ePed1.Handle);
                        ClearPedTasksImmediately(ePed2.Handle);
                        ClearPedTasksImmediately(ePed3.Handle);
                        ClearPedTasksImmediately(ePed4.Handle);
                        ClientFunctions.DisplayMessage("~b~Fireman~s~: Thank you Officer for securing the scene. We're done here!", 2500);
                        TaskVehicleDriveWander(ePed1.Handle, firetruckEntity.Handle, 15F, SummonFunctions.drivingStyle);
                        ePed2.Task.EnterVehicle(firetruckEntity, VehicleSeat.Passenger, 5000, 5F);
                        ePed3.Task.EnterVehicle(firetruckEntity, VehicleSeat.LeftRear, 5000, 5F);
                        ePed4.Task.EnterVehicle(firetruckEntity, VehicleSeat.RightRear, 5000, 5F);
                        eventSceneOver = true;
                        Guid preLoadGuid = unqGuid;
                        preLoadGuid = unqGuid;
                        await BaseScript.Delay(10000);
                        if (preLoadGuid != unqGuid)
                        {
                            return;
                        }
                        if (preLoadGuid != unqGuid)
                        {
                            return;
                        }
                        ClientFunctions.DisplayMessage(randomMessages[random.Next(randomMessages.Count)], 2500);
                        BlipSiren(firetruckEntity.Handle);
                        if (!ePed2.IsInVehicle())
                        {
                            ePed2.Task.WarpIntoVehicle(firetruckEntity, VehicleSeat.Passenger);
                        }
                        if (!ePed3.IsInVehicle())
                        {
                            ePed3.Task.WarpIntoVehicle(firetruckEntity, VehicleSeat.LeftRear);
                        }
                        if (!ePed4.IsInVehicle())
                        {
                            ePed4.Task.WarpIntoVehicle(firetruckEntity, VehicleSeat.RightRear);
                        }

                        // Destroy entities after wait
                        preLoadGuid = unqGuid;
                        preLoadGuid = unqGuid;
                        await BaseScript.Delay(20000);
                        if (preLoadGuid != unqGuid)
                        {
                            return;
                        }
                        if (preLoadGuid != unqGuid)
                        {
                            return;
                        }
                        NetworkFadeOutEntity(firetruckEntity.Handle, true, false);
                        await BaseScript.Delay(1000);
                        if (eventSpawned)
                        {
                            Deregister();
                        }
                    }
                }
            }
        }

        private async static void PedExtinguishFires(Ped fireman)
        {
            Ped player = Game.Player.Character;
            Vector3 firePos = fireman.Position;
            pedBusy.Add(fireman.Handle, true);
            while (eventSpawned && HasFireClose(fireman, ref firePos))
            {
                firesIterated += 1;
                var fireAimEntity = CreateObject(GetHashKey("prop_devin_box_dummy_01"), firePos.X, firePos.Y, firePos.Z, true, true, true);
                PlaceObjectOnGroundProperly(fireAimEntity);
                int fireBlip = AddBlipForEntity(fireAimEntity);
                SetBlipColour(fireBlip, 6);
                BeginTextCommandSetBlipName("STRING");
                AddTextComponentString("Detected Fire");
                EndTextCommandSetBlipName(fireBlip);
                TaskGoToEntity(fireman.Handle, fireAimEntity, -1, 1.5F, 2.0F, 1073741824, 0);
                int timeSpentPathfinding = 0;
                while (timeSpentPathfinding < 10 && eventSpawned && GetDistanceBetweenCoords(fireman.Position.X, fireman.Position.Y, fireman.Position.Z, firePos.X, firePos.Y, firePos.Z, false) > 2.0F)
                {
                    timeSpentPathfinding += 1;
                    float distance = GetDistanceBetweenCoords(fireman.Position.X, fireman.Position.Y, fireman.Position.Z, firePos.X, firePos.Y, firePos.Z, false);
                    TaskGoToEntity(fireman.Handle, fireAimEntity, -1, 1.5F, 2.0F, 1073741824, 0);
                    //Debug.WriteLine($"Firefighter pathfinding to fire... (Have {distance} to go, I've spent {timeSpentPathfinding} secs pathfinding!");
                    await BaseScript.Delay(1000);
                }
                TaskShootAtEntity(fireman.Handle, fireAimEntity, -1, (uint)FiringPattern.FullAuto);
                int timeSpentPuttingFireOut = 0;
                while (timeSpentPuttingFireOut < 10 && eventSpawned && CoordHasFire(firePos))
                {
                    timeSpentPuttingFireOut += 1;
                    //Debug.WriteLine($"Fireman is fighting a fire for {timeSpentPuttingFireOut} secs!");
                    //TaskShootAtEntity(fireman.Handle, fireAimEntity, -1, (uint)FiringPattern.FullAuto);
                    await BaseScript.Delay(1000);
                }
                RemoveBlip(ref fireBlip);
                //Debug.WriteLine("Done w/ this fire");
                DeleteEntity(ref fireAimEntity);
                // Wander around a bit randomly (so they don't all stack up)
                if (timeSpentPuttingFireOut > 5)
                {
                    firePos = new Vector3(firePos.X + random.Next(5), firePos.Y + random.Next(5), firePos.Z);
                    fireAimEntity = CreateObject(GetHashKey("prop_devin_box_dummy_01"), firePos.X, firePos.Y, firePos.Z, true, true, true);
                    PlaceObjectOnGroundProperly(fireAimEntity);
                    TaskGoToEntity(fireman.Handle, fireAimEntity, -1, 2.0F, 2.0F, 1073741824, 0);
                    int timeSpentWandering = 0;
                    while (timeSpentWandering < 5 && eventSpawned && GetDistanceBetweenCoords(fireman.Position.X, fireman.Position.Y, fireman.Position.Z, firePos.X, firePos.Y, firePos.Z, false) > 1.5F)
                    {
                        timeSpentWandering += 1;
                        float distance = GetDistanceBetweenCoords(fireman.Position.X, fireman.Position.Y, fireman.Position.Z, firePos.X, firePos.Y, firePos.Z, false);
                        //TaskGoToEntity(fireman.Handle, fireAimEntity, -1, 2.0F, 2.0F, 1073741824, 0);
                        //Debug.WriteLine($"Firefighter is wandering around... (Have {distance} to go, been wandering for {timeSpentWandering} secs)");
                        await BaseScript.Delay(1000);
                    }
                    DeleteEntity(ref fireAimEntity);
                }
                ClearPedTasksImmediately(fireman.Handle);
            }
            pedBusy[fireman.Handle] = false;
            //Debug.WriteLine($"Setting fireman as NOT Active, result: {pedBusy[fireman.Handle]}");
        }

        private static void ConfigureFirePed(Ped ped, bool disableGravity = false, bool giveExtinguisher = false)
        {
            //SetPedRelationshipGroupHash(ped.Handle, (uint) GetHashKey("FIREMAN"));
            SetPedPathAvoidFire(ped.Handle, true);
            ped.IsFireProof = true;
            ped.IsInvincible = true;
            ped.CanBeTargetted = false;
            if (disableGravity)
            {
                ped.HasGravity = false;
            }
            if (giveExtinguisher)
            {
                GiveWeaponToPed(ped.Handle, (uint)GetHashKey("WEAPON_FIREEXTINGUISHER"), 999, false, true);
            }
        }

        private static bool HasFireClose(Ped fireman, ref Vector3 firePos)
        {
            Vector3 originalPos = firePos;
            if (GetClosestFirePos(ref firePos, originalPos.X, originalPos.Y, originalPos.Z))
            {
                float distance = GetDistanceBetweenCoords(fireman.Position.X, fireman.Position.Y, fireman.Position.Z, firePos.X, firePos.Y, firePos.Z, false);
                //Debug.WriteLine($"Found a fire {distance} away");
                if (distance < 20F)
                {
                    return true;
                }
                else
                {
                    //Debug.WriteLine("Found a fire, but too far away for this summon.");
                    firePos = originalPos;
                }
            }
            return false;
        }

        private static bool CoordHasFire(Vector3 pos)
        {
            Vector3 originalPos = pos;
            if (GetClosestFirePos(ref pos, originalPos.X, originalPos.Y, originalPos.Z))
            {
                float distance = GetDistanceBetweenCoords(originalPos.X, originalPos.Y, originalPos.Z, pos.X, pos.Y, pos.Z, false);
                if (distance < 0.5F)
                {
                    return true;
                }
            }
            return false;
        }

        public async static void Summon()
        {
            if (eventSpawned)
            {
                Deregister();
                return;
            }
            Deregister();
            unqGuid = Guid.NewGuid();
            RequestAnimDict("random@arrests");
            ShowNotification("A Firetruck is in route to your location, please wait.");
            await ClientFunctions.LoadModelAsync("s_m_y_fireman_01");

            // Assets
            Ped player = Game.Player.Character;
            var spawnLocation = player.Position + (player.ForwardVector * 75) + (player.RightVector * 75);
            firetruckEntity = await World.CreateVehicle("firetruk", spawnLocation);
            firetruckEntity.IsInvincible = true;
            ePed1 = await World.CreatePed("s_m_y_fireman_01", firetruckEntity.Position, 0F);
            ePed1.SetIntoVehicle(firetruckEntity, VehicleSeat.Driver);
            ConfigureFirePed(ePed1, false, true);
            ePed2 = await World.CreatePed("s_m_y_fireman_01", firetruckEntity.Position, 0F);
            ePed2.SetIntoVehicle(firetruckEntity, VehicleSeat.Passenger);
            ConfigureFirePed(ePed2, false, true);
            ePed3 = await World.CreatePed("s_m_y_fireman_01", firetruckEntity.Position, 0F);
            ePed3.SetIntoVehicle(firetruckEntity, VehicleSeat.Any);
            ConfigureFirePed(ePed3, false, true);
            ePed4 = await World.CreatePed("s_m_y_fireman_01", firetruckEntity.Position, 0F);
            ePed4.SetIntoVehicle(firetruckEntity, VehicleSeat.Any);
            ConfigureFirePed(ePed4, false, true);

            API.SetNetworkIdCanMigrate(ePed1.NetworkId, false);
            API.SetNetworkIdCanMigrate(ePed2.NetworkId, false);
            API.SetNetworkIdCanMigrate(ePed3.NetworkId, false);
            API.SetNetworkIdCanMigrate(ePed4.NetworkId, false);
            API.SetNetworkIdCanMigrate(firetruckEntity.NetworkId, false);

            // Configuration
            firetruckEntity.IsSirenActive = true;
            firetruckEntity.Mods.LicensePlate = $"SA FD {random.Next(10)}";
            firetruckEntity.Mods.LicensePlateStyle = LicensePlateStyle.BlueOnWhite3;
            firetruckEntity.PlaceOnNextStreet();
            //ePed1.Task.DriveTo(firetruckEntity, player.Position, 15F, 20F, 262972);
            vehDest = await SummonFunctions.PedGetClosestSideOfRoad(player);
            //ePed1.Task.DriveTo(firetruckEntity, vehDest.Position, 15F, 20F, 262972);
            TaskVehicleGotoNavmesh(ePed1.Handle, firetruckEntity.Handle, vehDest.Position.X, vehDest.Position.Y, vehDest.Position.Z, 20F, SummonFunctions.drivingStyle, 0F);
            firetruckBlip = AddBlipForEntity(firetruckEntity.Handle);
            SetBlipColour(firetruckBlip, 1);
            BeginTextCommandSetBlipName("STRING");
            AddTextComponentString("Firetruck");
            EndTextCommandSetBlipName(firetruckBlip);
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
            ClientFunctions.DisplayMessage($"~b~Officer~s~: Fire Department needed to ~r~{streetName}~s~.", 3500);
            await BaseScript.Delay(2500);
            ClearPedTasks(player.Handle);

            // Show Warp hotkey
            await BaseScript.Delay(15000);
            if (!eventOnScene && eventSpawned)
            {
                ShowNotification("Firetruck taking too long/stuck? Press [F7] to warp them to you.");
            }
        }

        public async static void Warp()
        {
            if (eventSpawned && !eventSceneStarted && ePed1.IsInVehicle())
            {
                BlipSiren(firetruckEntity.Handle);
                await BaseScript.Delay(500);
                ePed1.PlayAmbientSpeech("GENERIC_HI", SpeechModifier.ForceMegaphone);
                ClearPedTasks(ePed1.Handle);
                ClearPedTasks(ePed2.Handle);
                ClearPedTasks(ePed3.Handle);
                ClearPedTasks(ePed4.Handle);
                Ped player = Game.Player.Character;
                firetruckEntity.Position = vehDest.Position;
                firetruckEntity.Speed = 0;
                firetruckEntity.PlaceOnNextStreet();
                await BaseScript.Delay(500);
                firetruckEntity.PlaceOnGround();
                eventWarped = true;

                // In case they warp far away (if player is not near a road)
                TaskEveryoneLeaveVehicle(firetruckEntity.Handle);
                TaskGoToEntity(ePed1.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                //TaskGoToEntity(ePed2.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                TaskGoStraightToCoord(ePed2.Handle, player.Position.X + 2F, player.Position.Y, player.Position.Z, 3F, -1, 0F, 0F);
                TaskGoStraightToCoord(ePed3.Handle, player.Position.X, player.Position.Y + 2F, player.Position.Z, 3F, -1, 0F, 0F);
                TaskGoStraightToCoord(ePed4.Handle, player.Position.X + 2F, player.Position.Y + 2F, player.Position.Z, 3F, -1, 0F, 0F);
            }
            else
            {
                Debug.WriteLine("Could not detect spawned Firetruck");
            }
        }

        public static void Deregister()
        {
            if (firetruckEntity != null)
            {
                firetruckEntity.Delete();
            }
            if (ePed1 != null)
            {
                ePed1.Delete();
            }
            if (ePed2 != null)
            {
                ePed2.Delete();
            }
            if (ePed3 != null)
            {
                ePed3.Delete();
            }
            if (ePed4 != null)
            {
                ePed4.Delete();
            }
            if (firetruckBlip != default(int))
            {
                RemoveBlip(ref firetruckBlip);
            }
            eventSpawned = false;
            eventOnScene = false;
            eventSceneStarted = false;
            eventSceneOver = false;
            eventWarped = false;
            firesIterated = 0;
            Debug.WriteLine("Cleaned up old Fire entities");
        }

        private static void ShowNotification(string message)
            => ClientFunctions.ShowToast("Fire Department", message, "info");
    }
}
