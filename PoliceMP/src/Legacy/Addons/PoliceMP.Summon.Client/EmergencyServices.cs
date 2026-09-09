using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Client.Extensions;
using static CitizenFX.Core.Native.API;
using static PoliceMP.Summon.Client.SummonFunctions;
using CitizenFX.Core.Native;

namespace PoliceMP.Summon.Client
{
    class EmergencyServices
    {
        // Event Entities
        private static int pedsIterated = 0;
        private static Vehicle _vehicle;
        private static Ped ePed1;
        private static Ped ePed2;
        private static int ambulanceBlip;
        private static int eActivePedBlip;

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
            "~b~EMT~s~: Looks like a nasty one...", "~b~EMT~s~: Oh man, we never get the easy ones!"
        };
        public static List<string> ped2WelcomeMessages = new List<string> {
            "~b~Paramedic~s~: Grab the medical bag, let's see what we can do.", "~b~Paramedic~s~: Did you remember extra bandages?"
        };
        public static List<string> progressMessages = new List<string> {
            "~b~Paramedic~s~: Come on you can make it! Stick with me!", "~b~Paramedic~s~: Oh man, they are bleeding so much! Help me!",
            "~b~EMT~s~: Here, I'll hold pressure on this wound.", "~b~EMT~s~: Want me to grab the defibrillator?"
        };
        public static List<string> successMessages = new List<string>
        {
            "~b~Paramedic~s~: Come on, take it slow, are you okay?", "~b~Paramedic~s~: Hey! They're awake!",
            "~b~EMT~s~: Here, I'll help you stand up.", "~b~EMT~s~: Officer, we did it!"
        };
        public static List<string> failMessages = new List<string>
        {
            "~b~Paramedic~s~: I'm sorry, there was nothing we could do.", "~b~Paramedic~s~: Officer, I'm afraid they didn't make it."
        };
        public static List<string> randomMessages = new List<string> {
            "~b~EMT~s~: Can we stop by the Pub?", "~b~EMT~s~: I'm hungry after that - time for lunch?"
        };

        private static Dictionary<int, string> drawableToVoice = new Dictionary<int, string>
        {
            { 0, "s_m_m_paramedic_01_latino_mini_01" },
            { 1, "s_m_m_paramedic_01_black_mini_01" },
            { 2, "s_m_m_paramedic_01_white_mini_01" }
        };

        public async static void Loop()
        {
            if (eventSpawned)
            {
                Ped player = Game.Player.Character;

                // Check if the get-out event needs to happen
                if (!eventOnScene && GetDistanceBetweenCoords(ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, vehDest.Position.X, vehDest.Position.Y, vehDest.Position.Z, false) < 15)
                {
                    _vehicle.IsSirenActive = false;
                    RequestAnimDict("CODE_HUMAN_MEDIC_KNEEL");
                    RequestAnimDict("CODE_HUMAN_MEDIC_TEND_TO_DEAD");
                    RequestAnimDict("move_ped_crouched");
                    eventOnScene = true;
                    BaseScript.TriggerEvent("PoliceMPFirstAid:Client:Scene", true);
                    ShowNotification("EMS has arrived, ensure the scene is secure.");
                    if (!eventWarped)
                    {
                        ClearPedTasks(ePed1.Handle);
                        SetVehicleForwardSpeed(_vehicle.Handle, 0F);
                        ePed1.Task.DriveTo(_vehicle, _vehicle.Position, 0F, 0F, 262972);
                        TaskLeaveVehicle(ePed1.Handle, _vehicle.Handle, 0);
                        TaskLeaveVehicle(ePed2.Handle, _vehicle.Handle, 0);
                    }

                    // Prepare Scenario
                    ClientFunctions.DisplayMessage(ped1WelcomeMessages[random.Next(ped1WelcomeMessages.Count)], 2500);
                    await BaseScript.Delay(2500);
                    ClientFunctions.DisplayMessage(ped2WelcomeMessages[random.Next(ped2WelcomeMessages.Count)], 2500);
                    if (!eventWarped)
                    {
                        TaskGoToEntity(ePed1.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                        TaskGoToEntity(ePed2.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                        BlipSiren(_vehicle.Handle);
                        ePed1.PlayAmbientSpeech("GENERIC_HI", SpeechModifier.ForceMegaphone);
                    }
                    PlayAmbientSpeechWithVoice(ePed1.Handle, "MEDIC_EXIT_AMBULANCE", drawableToVoice[GetPedDrawableVariation(ePed1.Handle, 0)], "SPEECH_PARAMS_STANDARD", false);
                    while (IsAmbientSpeechPlaying(ePed1.Handle))
                    {
                        await BaseScript.Delay(100);
                    }
                    PlayAmbientSpeechWithVoice(ePed2.Handle, "MEDIC_EXIT_AMBULANCE", drawableToVoice[GetPedDrawableVariation(ePed2.Handle, 0)], "SPEECH_PARAMS_STANDARD", false);
                }
                if (!eventSceneStarted && eventOnScene)
                {
                    // Wait until Ped is near player
                    if (GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, false) < 5F)
                    {
                        eventSceneStarted = true;
                        ClearPedTasks(ePed1.Handle);
                        ClientFunctions.DisplayMessage("~b~Paramedic~s~: Can you show me to the location?", 2500);
                        Ped[] peds = ClientFunctions.GetAllDeadPeds();
                        foreach (var ped in peds)
                        {
                            if (ped.Money == 999)
                            {
                                continue;
                            }
                            ped.Money = 999;
                            pedsIterated += 1;
                            // Loop through bodies close to player
                            if (GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, ped.Position.X, ped.Position.Y, ped.Position.Z, false) < 20F)
                            {
                                if (ped.IsHuman && !ped.IsInVehicle())
                                {
                                    // Step 1
                                    int pedHandle = ped.Handle;
                                    int blip = AddBlipForEntity(ped.Handle);
                                    eActivePedBlip = blip;
                                    SetBlipColour(blip, 39);
                                    BeginTextCommandSetBlipName("STRING");
                                    AddTextComponentString("Patient");
                                    EndTextCommandSetBlipName(eActivePedBlip);
                                    TaskGoToEntity(ePed1.Handle, ped.Handle, -1, 1.5F, 2F, 1073741824, 0);
                                    TaskGoToEntity(ePed2.Handle, ped.Handle, -1, 1.5F, 2F, 1073741824, 0);
                                    while (GetDistanceBetweenCoords(ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, ped.Position.X, ped.Position.Y, ped.Position.Z, false) > 2.0F)
                                    {
                                        TaskGoToEntity(ePed1.Handle, ped.Handle, -1, 1.5F, 2F, 1073741824, 0);
                                        Debug.WriteLine("EMT pathfinding to a body...");
                                        await BaseScript.Delay(500);
                                    }
                                    while (GetDistanceBetweenCoords(ePed2.Position.X, ePed2.Position.Y, ePed2.Position.Z, ped.Position.X, ped.Position.Y, ped.Position.Z, false) > 2.0F)
                                    {
                                        TaskGoToEntity(ePed2.Handle, ped.Handle, -1, 1.5F, 2F, 1073741824, 0);
                                        Debug.WriteLine("Paramedic pathfinding to a body...");
                                        await BaseScript.Delay(500);
                                    }

                                    // Step 2
                                    ClearPedTasks(ePed1.Handle);
                                    ClearPedTasks(ePed2.Handle);
                                    ePed1.FacePed(ped);
                                    ePed2.FacePed(ped);
                                    PlayAmbientSpeechWithVoice(ePed1.Handle, "MEDIC_ARRIVE_AT_BODY", drawableToVoice[GetPedDrawableVariation(ePed1.Handle, 0)], "SPEECH_PARAMS_STANDARD", false);
                                    TaskStartScenarioAtPosition(ePed1.Handle, "CODE_HUMAN_MEDIC_TEND_TO_DEAD", ePed1.Position.X, ePed1.Position.Y, ePed1.Position.Z, ePed1.Heading, -1, false, false);
                                    TaskStartScenarioAtPosition(ePed2.Handle, "CODE_HUMAN_MEDIC_KNEEL", ePed2.Position.X, ePed2.Position.Y, ePed2.Position.Z, ePed2.Heading, -1, false, false);
                                    WeaponHash deathCause = (WeaponHash)GetPedCauseOfDeath(ped.Handle);
                                    await BaseScript.Delay(5000);

                                    // Step 3
                                    PlayAmbientSpeechWithVoice(ePed2.Handle, "MEDIC_ARRIVE_AT_BODY", drawableToVoice[GetPedDrawableVariation(ePed2.Handle, 0)], "SPEECH_PARAMS_STANDARD", false);
                                    ClientFunctions.DisplayMessage(progressMessages[random.Next(progressMessages.Count)], 2500);
                                    DeathChance deathChance = SummonFunctions.GetDeathCauseFromHash(deathCause);
                                    int resurrectChance = random.Next(15, 100);
                                    int randomChance = random.Next(100) + deathChance.DeathChanceAmount;
                                    Debug.WriteLine($"Ped given a {randomChance} / {resurrectChance}% chance to live");
                                    await BaseScript.Delay(7500);

                                    // Step 4
                                    if (randomChance >= resurrectChance)
                                    {
                                        ped.Money = 0;
                                        PlayAmbientSpeechWithVoice(ePed1.Handle, "MEDIC_REALISE_VICTIM_DEAD", drawableToVoice[GetPedDrawableVariation(ePed1.Handle, 0)], "SPEECH_PARAMS_STANDARD", false);
                                        ClientFunctions.DisplayMessage(failMessages[random.Next(failMessages.Count)], 2500);
                                    }
                                    else
                                    {
                                        PlayAmbientSpeechWithVoice(ePed1.Handle, "GENERIC_SHOCKED_MED", drawableToVoice[GetPedDrawableVariation(ePed1.Handle, 0)], "SPEECH_PARAMS_STANDARD", false);
                                        ClientFunctions.DisplayMessage(successMessages[random.Next(successMessages.Count)], 2500);
                                        ped.Resurrect();
                                        FreezeEntityPosition(ped.Handle, true);
                                        FreezePedCameraRotation(ped.Handle);
                                        ClearPedTasksImmediately(ped.Handle);
                                        SetPedMovementClipset(ped.Handle, "move_ped_crouched", 0.25F);
                                    }
                                    await BaseScript.Delay(5000);

                                    // Step 5
                                    if (ped.IsAlive)
                                    {
                                        FreezeEntityPosition(ped.Handle, false);
                                        ResetPedMovementClipset(ped.Handle, 0);
                                        ped.Task.WanderAround();
                                    }
                                    else
                                    {
                                        PlayAmbientSpeechWithVoice(ePed2.Handle, "MEDIC_REFLECT_ON_DEATH", drawableToVoice[GetPedDrawableVariation(ePed2.Handle, 0)], "SPEECH_PARAMS_STANDARD", false);
                                    }
                                    ClearPedTasks(ePed1.Handle);
                                    ClearPedTasks(ePed2.Handle);
                                    RemoveBlip(ref blip);
                                    await BaseScript.Delay(1000);
                                }
                            }
                        }

                        // Check if we actually found any bodies
                        if (pedsIterated == 0)
                        {
                            ShowNotification("Unable to locate any corpses, try to stand closer and call EMS again.");
                        }

                        // Wrap up scenario
                        ClearPedTasksImmediately(ePed2.Handle);
                        ClientFunctions.DisplayMessage("~b~Paramedic~s~: Thank you Officer for securing the scene. We're done here!", 2500);
                        BaseScript.TriggerEvent("PoliceMPFirstAid:Client:Scene", false);
                        TaskVehicleDriveWander(ePed1.Handle, _vehicle.Handle, 15F, SummonFunctions.drivingStyle);
                        ePed2.Task.EnterVehicle(_vehicle, VehicleSeat.Passenger, 5000, 5F);
                        eventSceneOver = true;
                        PlayAmbientSpeechWithVoice(ePed1.Handle, "MEDIC_RETURN_TO_AMBULANCE_STATE", drawableToVoice[GetPedDrawableVariation(ePed1.Handle, 0)], "SPEECH_PARAMS_STANDARD", false);
                        while (IsAmbientSpeechPlaying(ePed1.Handle))
                        {
                            await BaseScript.Delay(100);
                        }
                        PlayAmbientSpeechWithVoice(ePed2.Handle, "MEDIC_RETURN_TO_AMBULANCE_RESP", drawableToVoice[GetPedDrawableVariation(ePed2.Handle, 0)], "SPEECH_PARAMS_STANDARD", false);
                        Guid preLoadGuid = unqGuid;
                        await BaseScript.Delay(10000);
                        if (preLoadGuid != unqGuid)
                        {
                            return;
                        }
                        ClientFunctions.DisplayMessage(randomMessages[random.Next(randomMessages.Count)], 2500);
                        BlipSiren(_vehicle.Handle);
                        if (!ePed2.IsInVehicle())
                        {
                            ePed2.Task.WarpIntoVehicle(_vehicle, VehicleSeat.Passenger);
                        }

                        // Destroy entities after wait
                        preLoadGuid = unqGuid;
                        await BaseScript.Delay(20000);
                        if (preLoadGuid != unqGuid)
                        {
                            return;
                        }
                        NetworkFadeOutEntity(_vehicle.Handle, true, false);
                        await BaseScript.Delay(1000);
                        if (eventSpawned)
                        {
                            Deregister();
                        }
                    }
                }
                // Sometimes they fight w/ people (this can be remove once I figure out how to make them passive)
                if (eventSceneOver && ePed1.IsInCombat)
                {
                    ClearPedTasksImmediately(ePed1.Handle);
                    var randomSpot = player.Position + (player.ForwardVector * 75) + (player.RightVector * 75);
                    ePed1.Task.DriveTo(_vehicle, randomSpot, 15F, 20F, 262972);
                }
                if (eventSceneOver && ePed2.IsInCombat)
                {
                    ClearPedTasksImmediately(ePed2.Handle);
                    ePed2.Task.EnterVehicle(_vehicle, VehicleSeat.Passenger, 5000, 5F);
                    await BaseScript.Delay(10000);
                    if (!ePed2.IsInVehicle())
                    {
                        ePed2.Task.WarpIntoVehicle(_vehicle, VehicleSeat.Passenger);
                    }
                }
                // Update Driver w/ Player's location always
                if (!eventOnScene && !eventWarped)
                {
                    ePed1.Task.DriveTo(_vehicle, vehDest.Position, 10F, 20F, 262972);
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
            ShowNotification("An ambulance is in route to your location, please wait.");
            await ClientFunctions.LoadModelAsync("s_m_m_paramedic_01");

            // Assets
            Ped player = Game.Player.Character;
            var spawnLocation = player.Position + (player.ForwardVector * 75) + (player.RightVector * 75);

            var vanModel = new Model(VehicleHash.Ambulance);

            _vehicle = await World.CreateVehicle(vanModel, spawnLocation);

            ePed1 = await World.CreatePed("s_m_m_paramedic_01", _vehicle.Position, 0F);
            ePed1.SetIntoVehicle(_vehicle, VehicleSeat.Driver);
            //SetPedRelationshipGroupHash(ePed1.Handle, PoliceMPSummon.civHash);
            ePed2 = await World.CreatePed("s_m_m_paramedic_01", _vehicle.Position, 0F);
            ePed2.SetIntoVehicle(_vehicle, VehicleSeat.Passenger);
            //SetPedRelationshipGroupHash(ePed2.Handle, PoliceMPSummon.civHash);

            // Configuration
            _vehicle.IsSirenActive = true;
            ePed1.AlwaysKeepTask = true; // does this do anything?
            ePed2.AlwaysKeepTask = true; // does this do anything?
            ePed1.CanBeTargetted = false;
            ePed2.CanBeTargetted = false;
            _vehicle.Mods.LicensePlate = $"SA EMS {random.Next(10)}";
            _vehicle.Mods.LicensePlateStyle = LicensePlateStyle.BlueOnWhite3;
            _vehicle.PlaceOnNextStreet();
            vehDest = await SummonFunctions.PedGetClosestSideOfRoad(player);
            TaskVehicleGotoNavmesh(ePed1.Handle, _vehicle.Handle, vehDest.Position.X, vehDest.Position.Y, vehDest.Position.Z, 20F, SummonFunctions.drivingStyle, 0F);
            ambulanceBlip = AddBlipForEntity(_vehicle.Handle);
            SetBlipColour(ambulanceBlip, 12);
            BeginTextCommandSetBlipName("STRING");
            AddTextComponentString("Ambulance");
            EndTextCommandSetBlipName(ambulanceBlip);
            eventSpawned = true;

            API.SetNetworkIdCanMigrate(ePed1.NetworkId, false);
            API.SetNetworkIdCanMigrate(ePed2.NetworkId, false);
            API.SetNetworkIdCanMigrate(_vehicle.NetworkId, false);

            // Radio anim
            PlaySoundFromEntity(-1, "Remote_Control_Close", player.Handle, "PI_Menu_Sounds", true, 0);
            TaskPlayAnim(player.Handle, "random@arrests", "generic_radio_chatter", 8.0F, 2.0F, -1, 50, 2.0F, false, false, false);
            if (IsEntityPlayingAnim(player.Handle, "random@arrests", "generic_radio_chatter", 3))
            {
                ClearPedSecondaryTask(player.Handle);
                SetCurrentPedWeapon(player.Handle, (uint)GetHashKey("GENERIC_RADIO_CHATTER"), true);
            }
            string streetName = World.GetStreetName(player.Position);
            ClientFunctions.DisplayMessage($"~b~Officer~s~: EMS needed to ~r~{streetName}~s~.", 3500);
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
                ShowNotification("EMS taking too long/stuck? Press [F7] to warp them to you.");
            }
        }

        public async static void Warp()
        {
            if (eventSpawned && !eventSceneStarted && ePed1.IsInVehicle())
            {
                BlipSiren(_vehicle.Handle);
                ePed1.PlayAmbientSpeech("GENERIC_HI", SpeechModifier.ForceMegaphone);
                ClearPedTasks(ePed1.Handle);
                ClearPedTasks(ePed2.Handle);
                Ped player = Game.Player.Character;
                _vehicle.Position = vehDest.Position;
                _vehicle.Speed = 0;
                _vehicle.PlaceOnNextStreet();
                await BaseScript.Delay(500);
                _vehicle.PlaceOnGround();
                eventWarped = true;

                // In case they warp far away (if player is not near a road)
                TaskLeaveVehicle(ePed1.Handle, _vehicle.Handle, 0);
                TaskLeaveVehicle(ePed2.Handle, _vehicle.Handle, 0);
                TaskGoToEntity(ePed1.Handle, player.Handle, -1, 5F, 4F, 1073741824, 0);
                TaskGoToEntity(ePed2.Handle, player.Handle, -1, 5F, 4F, 1073741824, 0);
            }
            else
            {
                Debug.WriteLine("Could not detect spawned EMS");
            }
        }

        public static void Deregister()
        {
            if (_vehicle != null)
            {
                _vehicle.Delete();
            }
            if (ePed1 != null)
            {
                ePed1.Delete();
            }
            if (ePed2 != null)
            {
                ePed2.Delete();
            }
            if (ambulanceBlip != default(int))
            {
                RemoveBlip(ref ambulanceBlip);
            }
            if (eActivePedBlip != default(int))
            {
                RemoveBlip(ref eActivePedBlip);
            }
            eventSpawned = false;
            eventOnScene = false;
            eventSceneStarted = false;
            eventSceneOver = false;
            eventWarped = false;
            pedsIterated = 0;
            Debug.WriteLine("Cleaned up old EMS entities");
        }

        private static void ShowNotification(string message)
            => ClientFunctions.ShowToast("Emergency Services", message, "info");
    }
}
