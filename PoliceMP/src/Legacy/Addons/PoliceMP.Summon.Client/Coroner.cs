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
    class Coroner
    {
        // Event Entities
        private static int cPedsIterated = 0;
        private static Vehicle cVanEntity;
        private static Ped cVanPed1;
        private static Ped cVanPed2;
        private static int cVanBlip;
        private static int cActivePedBlip;

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
        public static List<string> ped1WelcomeMessages = new List<string> {
            "~b~Coroner~s~: Wow, this is a dirty one...", "~b~Coroner~s~: Oh man, we never get the easy ones!"
        };
        public static List<string> ped2WelcomeMessages = new List<string> {
            "~b~Medical Assistant~s~: I'll grab the clipboard, you grab the body!", "~b~Medical Assistant~s~: I'll uh... grab my clipboard I guess."
        };
        public static List<string> progressMessages = new List<string> {
            "~b~Coroner~s~: Oh dear, this person really got a whopping.", "~b~Coroner~s~: Poor person, let's go ahead and bag them up."
        };
        public static List<string> randomMessages = new List<string> {
            "~b~Medical Examiner~s~: Oof, I really shouldn't have eaten any lunch.", "~b~Medical Examiner~s~: Man, I drank way too much last night for this."
        };

        public async static void Loop()
        {
            if (eventSpawned)
            {
                Ped player = Game.Player.Character;

                // Check if the get-out event needs to happen
                if (!eventOnScene && GetDistanceBetweenCoords(cVanPed1.Position.X, cVanPed1.Position.Y, cVanPed1.Position.Z, vehDest.Position.X, vehDest.Position.Y, vehDest.Position.Z, false) < 15)
                {
                    RequestAnimDict("CODE_HUMAN_MEDIC_TIME_OF_DEATH");
                    RequestAnimDict("CODE_HUMAN_MEDIC_TEND_TO_DEAD");
                    eventOnScene = true;
                    ShowNotification("The Coroner has arrived, ensure the scene is secure.");
                    BaseScript.TriggerEvent("PoliceMPFirstAid:Client:Scene", true);
                    if (!eventWarped)
                    {
                        ClearPedTasks(cVanPed1.Handle);
                        SetVehicleForwardSpeed(cVanEntity.Handle, 0F);
                        cVanPed1.Task.DriveTo(cVanEntity, cVanEntity.Position, 0F, 0F, 262972);
                        TaskLeaveVehicle(cVanPed1.Handle, cVanEntity.Handle, 0);
                        TaskLeaveVehicle(cVanPed2.Handle, cVanEntity.Handle, 0);
                    }

                    // Prepare Scenario
                    ClientFunctions.DisplayMessage(ped1WelcomeMessages[random.Next(ped1WelcomeMessages.Count)], 2500);
                    await BaseScript.Delay(2500);
                    ClientFunctions.DisplayMessage(ped2WelcomeMessages[random.Next(ped2WelcomeMessages.Count)], 2500);
                    if (!eventWarped)
                    {
                        TaskGoToEntity(cVanPed1.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                        TaskGoToEntity(cVanPed2.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                    }

                    // Wait until assistant is near player
                    while (GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, cVanPed2.Position.X, cVanPed2.Position.Y, cVanPed2.Position.Z, false) > 5F)
                    {
                        Debug.WriteLine("Assistant pathfinding to Player");
                        await BaseScript.Delay(500);
                    }
                    TaskStartScenarioAtPosition(cVanPed2.Handle, "CODE_HUMAN_MEDIC_TIME_OF_DEATH", cVanPed2.Position.X, cVanPed2.Position.Y, cVanPed2.Position.Z, cVanPed2.Heading, -1, false, false);
                }
                if (!eventSceneStarted && eventOnScene)
                {
                    // Wait until Coroner is near player
                    if (GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, cVanPed1.Position.X, cVanPed1.Position.Y, cVanPed1.Position.Z, false) < 5F)
                    {
                        eventSceneStarted = true;
                        ClearPedTasks(cVanPed1.Handle);
                        cVanPed1.PlayAmbientSpeech("GENERIC_HI");
                        ClientFunctions.DisplayMessage("~b~Coroner~s~: Can you show me to the location?", 2500);
                        await ClientFunctions.LoadModelAsync("xm_prop_body_bag");
                        Ped[] peds = ClientFunctions.GetAllDeadPeds();
                        foreach (var ped in peds)
                        {
                            if (ped.Money == 999)
                            {
                                continue;
                            }
                            ped.Money = 999;
                            cPedsIterated += 1;
                            // Loop through bodies close to player
                            if (GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, ped.Position.X, ped.Position.Y, ped.Position.Z, false) < 20F)
                            {
                                if (ped.IsHuman && !ped.IsInVehicle())
                                {
                                    // Step 1
                                    int pedHandle = ped.Handle;
                                    int blip = AddBlipForEntity(ped.Handle);
                                    cActivePedBlip = blip;
                                    SetBlipColour(blip, 39);
                                    BeginTextCommandSetBlipName("STRING");
                                    AddTextComponentString("Body");
                                    EndTextCommandSetBlipName(blip);
                                    TaskGoToEntity(cVanPed1.Handle, ped.Handle, -1, 1F, 3F, 1073741824, 0);
                                    while (GetDistanceBetweenCoords(cVanPed1.Position.X, cVanPed1.Position.Y, cVanPed1.Position.Z, ped.Position.X, ped.Position.Y, ped.Position.Z, false) > 2.0F)
                                    {
                                        Debug.WriteLine("Coroner pathfinding to a body...");
                                        await BaseScript.Delay(500);
                                    }

                                    // Step 2
                                    ClearPedTasks(cVanPed1.Handle);
                                    cVanPed1.FacePed(ped);
                                    TaskStartScenarioAtPosition(cVanPed1.Handle, "CODE_HUMAN_MEDIC_TEND_TO_DEAD", cVanPed1.Position.X, cVanPed1.Position.Y, cVanPed1.Position.Z, cVanPed1.Heading, -1, false, false);
                                    WeaponHash deathCause = (WeaponHash)GetPedCauseOfDeath(ped.Handle);
                                    await BaseScript.Delay(5000);

                                    // Step 3
                                    ClientFunctions.DisplayMessage(progressMessages[random.Next(progressMessages.Count)], 2500);
                                    var bagObj = CreateObject(GetHashKey("xm_prop_body_bag"), ped.Position.X, ped.Position.Y, ped.Position.Z + 2F, true, true, true);
                                    PlaceObjectOnGroundProperly(bagObj);
                                    DeletePed(ref pedHandle);
                                    await BaseScript.Delay(5000);

                                    // Step 4
                                    DeathChance deathChance = SummonFunctions.GetDeathCauseFromHash(deathCause);
                                    if (deathChance.DeathCause != "an unknown reason")
                                    {
                                        ClientFunctions.DisplayMessage($"~b~Coroner~s~: Wounds seem to be consistent with being killed by {deathChance.DeathCause}.", 2500);
                                    }
                                    else
                                    {
                                        ClientFunctions.DisplayMessage("~b~Coroner~s~: The body was too badly injured to identify the death cause right now.", 2500);
                                        Debug.WriteLine($"Unable to find WeaponHash: {deathCause} - please report this to the developers if it's not 0.");
                                    }
                                    await BaseScript.Delay(5000);
                                    ClearPedTasks(cVanPed1.Handle);
                                    RemoveBlip(ref blip);
                                    DeleteEntity(ref bagObj);
                                    await BaseScript.Delay(500);
                                }
                            }
                        }

                        // Check if we actually found any bodies
                        if (cPedsIterated == 0)
                        {
                            ShowNotification("Unable to locate any corpses, try to stand closer and call the Coroner again.");

                        }

                        // Wrap up scenario
                        cVanPed1.PlayAmbientSpeech("GENERIC_BYE");
                        ClearPedTasksImmediately(cVanPed2.Handle);
                        ClientFunctions.DisplayMessage("~b~Coroner~s~: Thank you Officer for securing the scene. We're done here!", 2500);
                        BaseScript.TriggerEvent("PoliceMPFirstAid:Client:Scene", false);
                        TaskVehicleDriveWander(cVanPed1.Handle, cVanEntity.Handle, 15F, SummonFunctions.drivingStyle);
                        cVanPed2.Task.EnterVehicle(cVanEntity, VehicleSeat.Passenger, 5000, 5F);
                        eventSceneOver = true;
                        Guid preLoadGuid = unqGuid;
                        await BaseScript.Delay(10000);
                        if (preLoadGuid != unqGuid)
                        {
                            return;
                        }
                        ClientFunctions.DisplayMessage(randomMessages[random.Next(randomMessages.Count)], 2500);
                        if (!cVanPed2.IsInVehicle())
                        {
                            cVanPed2.Task.WarpIntoVehicle(cVanEntity, VehicleSeat.Passenger);
                        }

                        // Destroy entities after wait
                        preLoadGuid = unqGuid;
                        await BaseScript.Delay(20000);
                        if (preLoadGuid != unqGuid)
                        {
                            return;
                        }
                        NetworkFadeOutEntity(cVanEntity.Handle, true, false);
                        await BaseScript.Delay(1000);
                        if (eventSpawned)
                        {
                            Deregister();
                        }
                    }
                }
                // Sometimes they fight w/ people (this can be remove once I figure out how to make them passive)
                if (cVanPed1.IsInCombat)
                {
                    ClearPedTasksImmediately(cVanPed1.Handle);
                    var randomSpot = player.Position + (player.ForwardVector * 75) + (player.RightVector * 75);
                    cVanPed1.Task.DriveTo(cVanEntity, randomSpot, 15F, 20F, 262972);
                }
                if (cVanPed2.IsInCombat)
                {
                    ClearPedTasksImmediately(cVanPed2.Handle);
                    cVanPed2.Task.EnterVehicle(cVanEntity, VehicleSeat.Passenger, 5000, 5F);
                    await BaseScript.Delay(10000);
                    if (!cVanPed2.IsInVehicle())
                    {
                        cVanPed2.Task.WarpIntoVehicle(cVanEntity, VehicleSeat.Passenger);
                    }
                }
                // Update Driver w/ Player's location always
                if (!eventOnScene && !eventWarped)
                {
                    cVanPed1.Task.DriveTo(cVanEntity, Game.Player.Character.Position, 10F, 20F, 262972);
                }
            }
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
            ShowNotification("A Coroner is in route to your location, please wait.");

            // Assets
            Ped player = Game.Player.Character;
            var spawnLocation = player.Position + (player.ForwardVector * 75) + (player.RightVector * 75);
            cVanEntity = await World.CreateVehicle("burrito3", spawnLocation);
            await ClientFunctions.LoadModelAsync("s_m_m_doctor_01");
            cVanPed1 = await World.CreatePed("s_m_m_doctor_01", cVanEntity.Position, 0F);
            cVanPed1.SetIntoVehicle(cVanEntity, VehicleSeat.Driver);
            //SetPedRelationshipGroupHash(cVanPed1.Handle, PoliceMPSummon.civHash);
            await ClientFunctions.LoadModelAsync("s_m_m_scientist_01");
            cVanPed2 = await World.CreatePed("s_m_m_scientist_01", cVanEntity.Position, 0F);
            cVanPed2.SetIntoVehicle(cVanEntity, VehicleSeat.Passenger);

            // Configuration
            cVanPed1.AlwaysKeepTask = true; // does this do anything?
            cVanPed2.AlwaysKeepTask = true; // does this do anything?
            cVanPed1.CanBeTargetted = false;
            cVanPed2.CanBeTargetted = false;
            cVanEntity.Mods.PrimaryColor = VehicleColor.MetallicBlack;
            cVanEntity.Mods.LicensePlate = $"SA C {random.Next(10)}";
            cVanEntity.Mods.LicensePlateStyle = LicensePlateStyle.BlueOnWhite3;
            cVanEntity.PlaceOnNextStreet();
            vehDest = await SummonFunctions.PedGetClosestSideOfRoad(player);
            //cVanPed1.Task.DriveTo(cVanEntity, vehDest.Position, 15F, 20F, 262972);
            TaskVehicleGotoNavmesh(cVanPed1.Handle, cVanEntity.Handle, vehDest.Position.X, vehDest.Position.Y, vehDest.Position.Z, 20F, SummonFunctions.drivingStyle, 0F);
            cVanBlip = AddBlipForEntity(cVanEntity.Handle);
            SetBlipColour(cVanBlip, 40);
            BeginTextCommandSetBlipName("STRING");
            AddTextComponentString("Coroner");
            EndTextCommandSetBlipName(cVanBlip);
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
            ClientFunctions.DisplayMessage($"~b~Officer~s~: Coroner needed to ~r~{streetName}~s~ for a body.", 3500);
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
                ShowNotification("Coroner taking too long/stuck? Press [F7] to warp them to you.");
            }
        }

        public async static void Warp()
        {
            if (eventSpawned && !eventSceneStarted && cVanPed1.IsInVehicle())
            {
                ClearPedTasks(cVanPed1.Handle);
                ClearPedTasks(cVanPed2.Handle);
                Ped player = Game.Player.Character;
                cVanEntity.Position = vehDest.Position;
                cVanEntity.Speed = 0;
                cVanEntity.PlaceOnNextStreet();
                await BaseScript.Delay(500);
                cVanEntity.PlaceOnGround();
                eventWarped = true;

                // In case they warp far away (if player is not near a road)
                TaskLeaveVehicle(cVanPed1.Handle, cVanEntity.Handle, 0);
                TaskLeaveVehicle(cVanPed2.Handle, cVanEntity.Handle, 0);
                TaskGoToEntity(cVanPed1.Handle, player.Handle, -1, 5F, 4F, 1073741824, 0);
                TaskGoToEntity(cVanPed2.Handle, player.Handle, -1, 5F, 4F, 1073741824, 0);
            }
            else
            {
                Debug.WriteLine("Could not detect a spawned Coroner");
            }
        }

        public static void Deregister()
        {
            if (cVanEntity != null)
            {
                cVanEntity.Delete();
            }
            if (cVanPed1 != null)
            {
                cVanPed1.Delete();
            }
            if (cVanPed2 != null)
            {
                cVanPed2.Delete();
            }
            if (cVanBlip != default(int))
            {
                RemoveBlip(ref cVanBlip);
            }
            if (cActivePedBlip != default(int))
            {
                RemoveBlip(ref cActivePedBlip);
            }
            eventSpawned = false;
            eventOnScene = false;
            eventSceneStarted = false;
            eventSceneOver = false;
            eventWarped = false;
            cPedsIterated = 0;
            Debug.WriteLine("Cleaned up old Coroner entities");
        }

        private static void ShowNotification(string message) 
            => ClientFunctions.ShowToast("Coroner", message, "info");
    }
}
