using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using Newtonsoft.Json;
using PoliceMP.Garage.Shared;
using PoliceMP.Main.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Client;
using PoliceMP.Main.Core.Shared;

/* TODO
 * Add any missing logic from current Garage system
 */

namespace PoliceMP.Garage.Client
{
    public class Main : BaseScript
    {
        private static int blipHandle;
        private static bool activeVehicle = false;
        private static bool carSpawned = false;
        private static bool camActive = false;
        private static bool disableControls = false;
        private static bool setNoRot = false;
        private static bool waitingOnRequest = false;
        private static bool waitingOnRelease = false;
        private static bool garageRunning = false;
        private static bool vehicleIsLocked = false;
        private static bool inSubMenu = false;

        private static bool isInGarage = false;

        private static Vehicle vehObj;
        private static int vehEntity;
        private static int _cam;
        private static GarageConfig config;
        private static GarageSpawn garageSpawn;
        private static OutsideSpawn outsideSpawn;
        private static Guid assignedRequest;
        private static GarageEntry entryPoint;
        private static List<int> createdBlips = new List<int> { };

        private static uint previousModelHash;

        /* Permissions */
        //Holders for whitelisting
        private static bool AFOStatus = false;
        private static bool RPUStatus = false;
        private static bool CIDStatus = false;
        private static bool NPASStatus = false;
        private static bool MPUStatus = false;
        private static bool AdminAuth = false;
        private static bool DogStatus = false;
        private static bool NHSStatus = false;
        private static bool FireStatus = false;
        private static bool ModAuth = false;
        private static bool WhitelistedMember = false;

        private static bool forcedUnlocked = false;

        private static string lockControlHelp = "~n~Press [F6] to lock - or use /lock";
        private static string unlockControlHelp = "~n~Press [F6] to unlock - or use /lock";

        public Main()
        {
            Debug.WriteLine("Starting PoliceMP.Garage...");
            EventHandlers["CarGarage:Assign"] += new Action<string>(OnAssignment);
            //API.RegisterCommand("garage", new Action(Garage), false); // quick and dirty teleport
            API.RegisterCommand("lock", new Action(LockToggle), false);
            API.RegisterCommand("lockforce", new Action(LockToggleForce), false);

            //API.RegisterCommand("garagetest", new Action(GarageTest), false); // for testing spawn points

            config = Newtonsoft.Json.JsonConvert.DeserializeObject<GarageConfig>(Properties.Resources.garage_config);

            GarageInit();
            Debug.WriteLine("Starting PoliceMP.Garage... Done!");
        }

        private async static void GarageTest()
        {
            Ped player = Game.Player.Character;
            int timesIterated = 0;
            await LoadModel("addpolmerc");
            Vehicle veh = await World.CreateVehicle("addpolmerc", player.Position, player.Heading);
            API.SetPedIntoVehicle(player.Handle, veh.Handle, -1);
            foreach (OutsideSpawn os in config.OutsideSpawns)
            {
                timesIterated++;
                Debug.WriteLine("--------------------------------------------------");
                Debug.WriteLine($"Check #{timesIterated}");
                Debug.WriteLine($"GUID: {os.Guid}");
                player.LastVehicle.Position = new Vector3(os.Location.X, os.Location.Y, os.Location.Z);
                player.LastVehicle.Heading = os.Location.Heading;
                await BaseScript.Delay(5000);
            }
            veh.Delete();
        }

        private async static void GarageInit()
        {
            await BaseScript.Delay(10000);
            Debug.WriteLine("Initializing garage...");
            foreach (GarageEntry entry in config.GarageEntry)
            {
                Blip blip = World.CreateBlip(new Vector3(entry.MarkerLocation.X, entry.MarkerLocation.Y, entry.MarkerLocation.Z));
                createdBlips.Add(blip.Handle);
                blip.Sprite = BlipSprite.Garage2;
                blip.IsShortRange = true;
                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentString("Police Garage");
                API.EndTextCommandSetBlipName(blip.Handle);
            }
            TriggerServerEvent("PoliceMPGarage:kt1Tm2D2w9TfLbGtbj2mechFz");
        }

        [EventHandler("PoliceMP:recieveSpecWhitelisting")]
        private void recieveSpecWhitelisting(bool afo, bool rpu, bool cid, bool npas, bool mpu, bool admin, bool dog, bool nhs, bool fire, bool mod, bool whitelisted)
        {
            AFOStatus = afo;
            RPUStatus = rpu;
            CIDStatus = cid;
            NPASStatus = npas;
            MPUStatus = mpu;
            AdminAuth = admin;
            DogStatus = dog;
            NHSStatus = nhs;
            FireStatus = fire;
            ModAuth = mod;
            WhitelistedMember = whitelisted;
        }

        [Tick]
        public async Task OnTick()
        {
            Ped player = Game.Player.Character;

            if (isInGarage)
            {
                API.SetPedDensityMultiplierThisFrame(0f);
                API.SetScenarioPedDensityMultiplierThisFrame(0f, 0f);
                API.SetAmbientVehicleRangeMultiplierThisFrame(0f);
                API.SetParkedVehicleDensityMultiplierThisFrame(0f);
                API.SetRandomVehicleDensityMultiplierThisFrame(0f);
                API.SetVehicleDensityMultiplierThisFrame(0f);
            }

            if (API.IsControlJustReleased(1, 167))
            {
                if (activeVehicle)
                {
                    LockToggle();
                }
                if (!activeVehicle)
                {
                    ShowNotification("You do not have an active Vehicle, pick one up at a Police Garage.");
                }
            }

            if (activeVehicle && vehicleIsLocked)
            {
                if (API.GetVehiclePedIsEntering(player.Handle) == vehEntity)
                {
                    ShowNotification($"Your vehicle is currently locked. {unlockControlHelp}");
                }
            }

            // Event for handling when a Player leaves a spot
            if (waitingOnRelease)
            {
                if (API.GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, outsideSpawn.Location.X, outsideSpawn.Location.Y, outsideSpawn.Location.Z, false) > 7.5F)
                {
                    waitingOnRelease = false;
                    TriggerServerEvent("CarGarage:Release", Game.Player.ServerId.ToString(), outsideSpawn.Guid.ToString());
                }
            }

            /*
            // Despawn Player's car if too far away
            Vector3 vehPos = API.GetEntityCoords(vehEntity, false);
            float vehDistance = API.GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, vehPos.X, vehPos.Y, vehPos.Z, true);
            if (activeVehicle && vehDistance > 750F)
            {
                DeregisterVehicle(true);
            }
            */

            // Handle locking vehicle once away from it
            if (activeVehicle && !vehicleIsLocked)
            {
                Vector3 vehPos = API.GetEntityCoords(vehEntity, false);
                float vehDistance = API.GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, vehPos.X, vehPos.Y, vehPos.Z, true);
                if (vehDistance >= 50F)
                {
                    WriteLine($"Player is {vehDistance}f away from their vehicle, locking...");
                    LockVehicle();
                }
            }

            // Handle blips, watch for destroyed vehicle
            if (activeVehicle)
            {
                if (player.IsInVehicle() && vehEntity == player.LastVehicle.Handle)
                {
                    API.SetBlipDisplay(blipHandle, 0);
                }
                else
                {
                    API.SetBlipDisplay(blipHandle, 6);
                }
            }

            // Forces the Player's Gameplay Camera to face the way their Ped is facing (always 0)
            if (setNoRot)
            {
                API.SetGameplayCamRelativeHeading(0f);
            }

            if (disableControls)
            {
                API.DisableAllControlActions(0);
                API.DisableAllControlActions(1);
            }

            // Hide HUD components, disable controls
            if (camActive)
            {
                for (int i = 0; i < 30; i++)
                {
                    API.HideHudComponentThisFrame(i);
                }
                API.HideHudAndRadarThisFrame();
                API.DisableAllControlActions(0);
                API.DisableAllControlActions(1);
            }
            // Setup marker/text in parking lot(s)
            foreach (GarageEntry entrySpot in config.GarageEntry)
            {
                API.DrawMarker(1, entrySpot.MarkerLocation.X, entrySpot.MarkerLocation.Y, entrySpot.MarkerLocation.Z, 0, 0, 0, 0, 0, 0, 2F, 2F, 3F, 0, 255, 255, 100, false, true, 2, false, null, null, false);
                float scale = 0.1F * API.GetGameplayCamFov();
                float distance = API.GetDistanceBetweenCoords(player.Position.X, player.Position.Y, player.Position.Z, entrySpot.MarkerLocation.X, entrySpot.MarkerLocation.Y, entrySpot.MarkerLocation.Z, true);

                if (distance < 10.0F)
                {
                    API.SetTextScale(0.1F * scale, 0.1F * scale);
                    API.SetTextFont(4);
                    API.SetTextProportional(true);
                    API.SetTextColour(250, 250, 250, 255);
                    API.SetTextDropshadow(1, 1, 1, 1, 255);
                    API.SetTextEdge(2, 0, 0, 0, 255);
                    API.SetTextDropShadow();
                    API.SetTextOutline();
                    API.SetTextEntry("STRING");
                    API.SetTextCentre(true);
                    API.AddTextComponentString(entrySpot.Description);
                    API.SetDrawOrigin(entrySpot.MarkerLocation.X, entrySpot.MarkerLocation.Y, entrySpot.MarkerLocation.Z + 2F, 0);
                    API.DrawText(0, 0);
                    API.ClearDrawOrigin();
                }

                if (possibleCars == null)
                {
                    return;
                }

                // Handle warping into garage
                if (distance <= 2F)
                {
                    entryPoint = entrySpot;
                    disableControls = true;
                    API.NetworkFadeOutEntity(player.Handle, true, false);
                    API.DoScreenFadeOut(250);
                    await BaseScript.Delay(1500);
                    Garage();
                    await BaseScript.Delay(1500);
                    disableControls = false;
                    API.DoScreenFadeIn(500);
                    API.NetworkFadeInEntity(player.Handle, true);

                    isInGarage = true;
                }
            }
        }

        private static void LockToggleForce()
        {
            if (!AdminAuth)
            {
                ShowNotification("You aren't an Administrator, try again when you're all grown up.");
                return;
            }
            var vehicleId = GetVehicleInFrontOfPlayer();
            if (vehicleId == -1)
            {
                ShowNotification("You need to be looking at the vehicle that you want to force lock/unlock.");
                return;
            }

            if (forcedUnlocked)
            {
                API.SetVehicleDoorsLockedForAllPlayers(vehicleId, false);
                ShowNotification("You unlock the vehicle using your superpowers.");
                forcedUnlocked = false;
            }
            else
            {
                API.SetVehicleDoorsLockedForAllPlayers(vehicleId, true);
                ShowNotification("You lock the vehicle using your superpowers.");
                forcedUnlocked = true;
            }
        }

        private static void LockToggle()
        {
            if (vehicleIsLocked)
            {
                UnlockVehicle();
            }
            else
            {
                LockVehicle();
            }
        }

        private static void LockVehicle()
        {
            if (!vehicleIsLocked)
            {
                if (!activeVehicle)
                {
                    ShowNotification("You do not have an active Vehicle, pick one up at a Police Garage.");
                    return;
                }
                API.SetVehicleDoorsLockedForAllPlayers(vehEntity, true);
                ShowNotification($"You've locked your vehicle. {unlockControlHelp}");
                vehicleIsLocked = true;
                API.BlipSiren(vehEntity);
                API.PlaySoundFromEntity(-1, "Remote_Control_Open", Game.Player.Character.Handle, "PI_Menu_Sounds", true, 0);
            }
        }
        private static void UnlockVehicle()
        {
            if (vehicleIsLocked)
            {
                if (!activeVehicle)
                {
                    ShowNotification("You do not have an active Vehicle, pick one up at a Police Garage.");
                    return;
                }
                API.SetVehicleDoorsLockedForAllPlayers(vehEntity, false);
                ShowNotification($"You've unlocked your vehicle. {lockControlHelp}");
                vehicleIsLocked = false;
                API.BlipSiren(vehEntity);
                API.PlaySoundFromEntity(-1, "Remote_Control_Open", Game.Player.Character.Handle, "PI_Menu_Sounds", true, 0);
            }
        }

        private static List<CarData> possibleCars = null;
        [EventHandler("PoliceMPGarage:ww8WnVLAzjsteULM7OU07GyzE")]
        private void RecievedCarData(string carsString)
        {
            var cars = JsonConvert.DeserializeObject<List<CarData>>(carsString);
            possibleCars = cars;
        }

        private static Menu CreateSubMenus(Menu parentMenu, string menuName, List<CarData> items, bool whitelist = true)
        {
            // Create Menu and add as submenu to parent
            Menu menu = new Menu(menuName, "Vehicle Selector");
            MenuController.AddSubmenu(parentMenu, menu);


            // Add button to parent menu and 
            MenuItem button = new MenuItem(menuName) { Label = "→→→" };
            parentMenu.AddMenuItem(button);
            MenuController.BindMenuItem(parentMenu, menu, button);

            // Check whitelist
            if (!whitelist)
            {
                button.Enabled = false;
            }

            // Populate Sub menu
            foreach (CarData item in items)
            {
                AddItemToMenu(menu, item);
            }

            // Add Event Handlers
            menu.OnItemSelect += async (_menu, _item, _index) =>
            {
                await OnItemSelect(_menu, _item, _index, items);
            };

            menu.OnIndexChange += (_menu, _oldItem, _newItem, _oldIndex, _newIndex) =>
            {
                OnIndexChange(_menu, _oldItem, _newItem, _oldIndex, _newIndex, items);
            };

            menu.OnMenuOpen += (_menu) =>
            {
                OnMenuOpen(_menu, items);
            };


            return menu;
        }

        private static void AddItemToMenu(Menu menu, CarData car)
        {
            menu.AddMenuItem(new MenuItem(car.Name));
        }

        private async void Garage()
        {
            if (garageRunning)
            {
                return;
            }

            // Populate Car Lists
            if (possibleCars == null)
            {
                Debug.WriteLine("Reconnect to server to populate Car lists");
                return;
            }

            garageRunning = true;
            // If they are already had a vehicle, destroy the old one
            if (activeVehicle)
            {
                DeregisterVehicle(true, false);
            }
            activeVehicle = false;
            carSpawned = false;
            camActive = true;

            API.DisplayHud(false);


            TriggerServerEvent("CarGarage:Request", Game.Player.ServerId.ToString(), "garage");
            waitingOnRequest = true;
            int timesWaited = 0;
            while (waitingOnRequest)
            {
                // If stuck for some reason the Garage is stuck, pick a random spot after 5 seconds
                if (timesWaited == 20)
                {
                    WriteLine("Garage seems stuck - picking a random spot!");
                    assignedRequest = config.GarageSpawns[PoliceMpRandom.Next(config.GarageSpawns.Count())].Guid;
                    waitingOnRequest = false;
                    break;
                }
                else
                {
                    timesWaited++;
                    WriteLine("Waiting on a free slot for the Garage! (Times Waited: " + timesWaited + ")");
                    await BaseScript.Delay(250);
                }
            }
            garageSpawn = FindGarageSpawn(assignedRequest);

            // Menu setup
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            Menu menu = new Menu("Police Garage", "Vehicle Selector") { Visible = true };
            MenuController.AddMenu(menu);
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);

            // Create Car Lists
            List<CarData> responseCarData = new List<CarData> { };
            List<CarData> whitelistedCarData = new List<CarData> { };
            List<CarData> rpuCarData = new List<CarData> { };
            List<CarData> arvCarData = new List<CarData> { };
            List<CarData> cidCarData = new List<CarData> { };
            List<CarData> dogCarData = new List<CarData> { };
            List<CarData> nhsCarData = new List<CarData> { };
            List<CarData> fireCarData = new List<CarData> { };
            List<CarData> adminCarData = new List<CarData> { };

            foreach (CarData car in possibleCars)
            {
                if (car.Categories.Contains("Response")) responseCarData.Add(car);
                if (car.Categories.Contains("Whitelisted")) whitelistedCarData.Add(car);
                if (car.Categories.Contains("RPU")) rpuCarData.Add(car);
                if (car.Categories.Contains("AFO")) arvCarData.Add(car);
                if (car.Categories.Contains("CID")) cidCarData.Add(car);
                if (car.Categories.Contains("DOG")) dogCarData.Add(car);
                if (car.Categories.Contains("NHS")) nhsCarData.Add(car);
                if (car.Categories.Contains("FIRE")) fireCarData.Add(car);
                if (car.Categories.Contains("Admin")) adminCarData.Add(car);
            }

            // Create Car Menus
            var menuResponse = CreateSubMenus(menu, "Response", responseCarData);
            var menuWhitelisted = CreateSubMenus(menu, "Whitelisted", whitelistedCarData, WhitelistedMember);
            var menuRPU = CreateSubMenus(menu, "RPU", rpuCarData, RPUStatus);
            var menuARV = CreateSubMenus(menu, "ARV", arvCarData, AFOStatus);
            var menuCID = CreateSubMenus(menu, "CID", cidCarData, CIDStatus);
            var menudog = CreateSubMenus(menu, "Dog Section", dogCarData, DogStatus);
            var menunhs = CreateSubMenus(menu, "NHS", nhsCarData, NHSStatus);
            var menufire = CreateSubMenus(menu, "Fire Service", fireCarData, FireStatus);
            var menuAdmin = CreateSubMenus(menu, "Admin", adminCarData, AdminAuth);

            // Setup secondary camera, move Player to Garage
            Ped player = Game.Player.Character;
            player.Position = new Vector3(garageSpawn.PlayerLocation.X, garageSpawn.PlayerLocation.Y, garageSpawn.PlayerLocation.Z);
            API.PlaceObjectOnGroundProperly(player.Handle);
            player.Heading = garageSpawn.PlayerLocation.Heading;
            Vector3 camPos = player.Position + (player.ForwardVector * 2);
            _cam = API.CreateCameraWithParams(26379945, camPos.X, camPos.Y, camPos.Z, 0F, 0.0F, API.GetEntityHeading(API.PlayerPedId()), 50.0F, true, 2);
            API.SetCamActive(_cam, true);
            API.AttachCamToEntity(_cam, player.Handle, 1F, 1F, 0F, true);
            API.RenderScriptCams(true, false, 0, false, false);
            //SpawnNewVehicle(VehicleHash.Police); // if we want to display an initial vehicle

            // Add event handler
            menu.OnMenuClose += async (_menu) =>
            {
                //WriteLine($"OnMenuClose: [{_menu}]");
                // We need this due to sub-menu close events trigger this too.
                if (_menu == menu)
                {
                    await BaseScript.Delay(50);
                    // We need this due to going from menu to sub-menu triggers this event as well
                    if (menuResponse.Visible || menuWhitelisted.Visible || menuRPU.Visible || menuARV.Visible || menuCID.Visible || menudog.Visible || menunhs.Visible || menufire.Visible || menuAdmin.Visible)
                    {
                        inSubMenu = true;
                        return;
                    }
                    else
                    {
                        inSubMenu = false;
                    }
                }
                if (!inSubMenu)
                {
                    await HandleMenuClose();
                }
                else
                {
                    // Navigate back to main menu
                    _menu.CloseMenu();
                    menu.OpenMenu();
                }
            };
        }

        private static void DeregisterVehicle(bool deleteVeh = false, bool showNotification = true)
        {
            activeVehicle = false;
            API.SetEntityAsMissionEntity(vehEntity, false, false);
            int blip = blipHandle;
            int vehicle = vehEntity;
            API.RemoveBlip(ref blip);
            API.DeleteEntity(ref vehicle);
            if (deleteVeh)
            {
                API.DeleteEntity(ref vehicle);
            }
            if (showNotification)
            {
                ShowNotification("Your vehicle is no longer available, please get another at the station.");
            }
        }

        private static void SetupVehicleBlip()
        {
            blipHandle = API.AddBlipForEntity(vehEntity);
            API.SetBlipSprite(blipHandle, 326);
            API.SetBlipColour(blipHandle, 3);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("Personal Vehicle");
            API.EndTextCommandSetBlipName(blipHandle);
        }

        private static GarageSpawn FindGarageSpawn(Guid guid)
        {
            foreach (GarageSpawn gs in config.GarageSpawns)
            {
                if (gs.Guid == guid)
                {
                    return gs;
                }
            }
            return null;
        }

        private static OutsideSpawn FindOutsideSpawn(Guid guid)
        {
            foreach (OutsideSpawn os in config.OutsideSpawns)
            {
                if (os.Guid == guid)
                {
                    return os;
                }
            }
            return null;
        }

        private static void OnAssignment(string request)
        {
            assignedRequest = new Guid(request);
            waitingOnRequest = false;
        }

        private static void OnMenuOpen(Menu _menu, List<CarData> vehicles)
        {
            _menu.RefreshIndex(0);
            MenuController.DisableMenuButtons = true;
            CarData car = vehicles.ElementAt(0);
            int oldVeh = vehEntity;
            API.DeleteVehicle(ref oldVeh);
            SpawnNewVehicle(car.Model);
        }

        private async static Task<bool> OnItemSelect(Menu _menu, MenuItem _item, int _index, List<CarData> vehicles)
        {
            MenuController.DisableMenuButtons = true;
            disableControls = true;
            //WriteLine($"OnItemSelect: [{_menu}, {_item}, {_index}]");
            Ped player = Game.Player.Character;
            carSpawned = true;
            waitingOnRequest = true;
            TriggerServerEvent("CarGarage:Request", Game.Player.ServerId.ToString(), "outside", entryPoint.Name);
            int timesWaited = 0;
            while (waitingOnRequest)
            {
                // If for some reason the Garage is stuck, pick a random spot after 5 secs.
                if (timesWaited == 20)
                {
                    WriteLine("Garage seems stuck - picking a random spot!");
                    assignedRequest = config.OutsideSpawns[PoliceMpRandom.Next(config.OutsideSpawns.Count())].Guid;
                    waitingOnRequest = false;
                    break;
                }
                else
                {
                    timesWaited++;
                    WriteLine($"Waiting on free slot ({timesWaited})");
                    await BaseScript.Delay(250);
                }
            }
            outsideSpawn = FindOutsideSpawn(assignedRequest);

            // Step 1
            API.BlipSiren(vehEntity);
            //API.SetVehicleSiren(vehEntity, true);
            //await BaseScript.Delay(1000);
            //API.SetVehicleSiren(vehEntity, false);
            disableControls = false;
            MenuController.DisableMenuButtons = false;

            // Step 2
            API.NetworkFadeOutEntity(player.Handle, true, false);
            API.DoScreenFadeOut(250);
            await BaseScript.Delay(1500);
            API.TaskWarpPedIntoVehicle(player.Handle, vehEntity, (int)VehicleSeat.Driver);
            vehObj.IsEngineRunning = true;
            API.SetEntityCoords(vehEntity, outsideSpawn.Location.X, outsideSpawn.Location.Y, outsideSpawn.Location.Z, false, false, false, false);
            API.SetEntityHeading(vehEntity, outsideSpawn.Location.Heading);
            API.SetVehRadioStation(vehEntity, "OFF");
            API.SetVehicleHasBeenOwnedByPlayer(vehEntity, true);
            API.SetEntityAsMissionEntity(vehEntity, true, true);

            // Step 3
            SetupVehicleBlip();
            _menu.CloseMenu();
            setNoRot = true;
            await BaseScript.Delay(1500);
            API.DoScreenFadeIn(500);
            API.NetworkFadeInEntity(player.Handle, true);
            TriggerServerEvent("CarGarage:Release", Game.Player.ServerId.ToString(), garageSpawn.Guid.ToString());
            CarData car = vehicles.ElementAt(_index);
            await HandleMenuClose();
            API.DisplayHud(true);
            waitingOnRelease = true;
            setNoRot = false;
            activeVehicle = true;
            garageRunning = false;
            vehicleIsLocked = false;
            isInGarage = false;
            return true;
        }

        private static void OnIndexChange(Menu _menu, MenuItem _oldItem, MenuItem _newItem, int _oldIndex, int _newIndex, List<CarData> vehicles)
        {
            //WriteLine($"OnIndexChange: [{_menu}, {_oldItem}, {_newItem}, {_oldIndex}, {_newIndex}]");
            MenuController.DisableMenuButtons = true;
            CarData car = vehicles.ElementAt(_newIndex);
            int oldVeh = vehEntity;
            API.DeleteVehicle(ref oldVeh);
            SpawnNewVehicle(car.Model);
        }

        private async static Task<bool> HandleMenuClose()
        {
            Ped player = Game.Player.Character;
            API.NetworkFadeOutEntity(player.Handle, true, false);
            API.DoScreenFadeOut(250);
            await BaseScript.Delay(1500);
            StopCamera();
            camActive = false;
            if (!carSpawned)
            {
                int oldVeh = vehEntity;
                API.DeleteVehicle(ref oldVeh);
                player.Position = new Vector3(entryPoint.OutsideLocation.X, entryPoint.OutsideLocation.Y, entryPoint.OutsideLocation.Z);
                player.Heading = entryPoint.OutsideLocation.Heading;
                TriggerServerEvent("CarGarage:Release", Game.Player.ServerId.ToString(), garageSpawn.Guid.ToString());
            }
            setNoRot = true;
            await BaseScript.Delay(1500);
            API.DoScreenFadeIn(500);
            setNoRot = false;
            garageRunning = false;
            API.NetworkFadeInEntity(player.Handle, true);
            API.DisplayHud(true);
            isInGarage = false;
            return true;
        }

        private static void StopCamera()
        {
            API.SetCamActive(_cam, false);
            API.RenderScriptCams(false, false, 0, false, false);
            API.DestroyCam(_cam, false);
            API.DestroyAllCams(true);
            MenuController.DisableMenuButtons = false;
        }

        private async static void SpawnNewVehicle(string vehicle)
        {
            if (API.GetHashKey(vehicle) == previousModelHash || previousModelHash == null)
            {
                Debug.WriteLine("Garage: Not unloading previous model as has is same or null");
            }
            else
            {
                API.SetModelAsNoLongerNeeded(previousModelHash);
                Debug.WriteLine("Garage: Unloaded previous model -" + previousModelHash.ToString() + " || New model - " + API.GetHashKey(vehicle));
            }
            previousModelHash = (uint)API.GetHashKey(vehicle);


            Ped player = Game.Player.Character;
            await LoadModel(vehicle);
            Vector3 vehPos = player.Position + (player.ForwardVector * 6) + (player.RightVector * 1);
            vehObj = new Vehicle(API.CreateVehicle((uint)API.GetHashKey(vehicle), vehPos.X, vehPos.Y, vehPos.Z, garageSpawn.Heading, true, true))
            {
                IsPersistent = true,
                IsStolen = false,
                IsWanted = false,
                NeedsToBeHotwired = false,
                PreviouslyOwnedByPlayer = true
            };
            vehEntity = vehObj.Handle;

            //Fix for the people in vehicle issue
            vehObj.Driver.Delete();
            foreach (var p in vehObj.Passengers)
            {
                p.Delete();
            }

            API.PlaceObjectOnGroundProperly(vehEntity);
            vehObj.Wash();
            vehObj.Repair();
            AddMods(vehicle, vehObj, vehEntity);
            await BaseScript.Delay(200); // give it a few moments to show the model
            MenuController.DisableMenuButtons = false;
        }

        private static void AddMods(string vehicleString, Vehicle vehObj, int vehEntity)
        {
            // Check if RPU or AFO car to install better mods
            var carCategories = new List<String>();
            foreach (var possibleCar in possibleCars)
            {
                if (possibleCar.Model == vehicleString)
                {
                    carCategories = possibleCar.Categories;
                }
            }

            if (carCategories.Contains("RPU") && RPUStatus)
            {
                vehObj.Mods.InstallModKit();
                vehObj.Mods[VehicleModType.Engine].Index = vehObj.Mods[VehicleModType.Engine].ModCount - 1;
                vehObj.Mods[VehicleModType.Brakes].Index = vehObj.Mods[VehicleModType.Brakes].ModCount - 1;
                vehObj.Mods[VehicleModType.Transmission].Index = vehObj.Mods[VehicleModType.Transmission].ModCount - 1;
            }

            else if (carCategories.Contains("ARV") && AFOStatus)
            {
                vehObj.Mods.InstallModKit();
                vehObj.Mods[VehicleModType.Engine].Index = vehObj.Mods[VehicleModType.Engine].ModCount - 2;
                vehObj.Mods[VehicleModType.Brakes].Index = vehObj.Mods[VehicleModType.Brakes].ModCount - 2;
                vehObj.Mods[VehicleModType.Transmission].Index = vehObj.Mods[VehicleModType.Transmission].ModCount - 2;

                if (vehObj.Mods[VehicleModType.Armor] != null)
                {
                    vehObj.Mods[VehicleModType.Armor].Index = vehObj.Mods[VehicleModType.Armor].ModCount - 1;
                }

                API.SetVehicleTyresCanBurst(vehEntity, false);
            }
            else if (carCategories.Contains("ARV") && carCategories.Contains("RPU") && (RPUStatus || AFOStatus))
            {
                vehObj.Mods.InstallModKit();
                vehObj.Mods[VehicleModType.Engine].Index = vehObj.Mods[VehicleModType.Engine].ModCount - 1;
                vehObj.Mods[VehicleModType.Brakes].Index = vehObj.Mods[VehicleModType.Brakes].ModCount - 1;
                vehObj.Mods[VehicleModType.Transmission].Index = vehObj.Mods[VehicleModType.Transmission].ModCount - 1;

                if (vehObj.Mods[VehicleModType.Armor] != null)
                {
                    vehObj.Mods[VehicleModType.Armor].Index = vehObj.Mods[VehicleModType.Armor].ModCount - 1;
                }

                API.SetVehicleTyresCanBurst(vehEntity, false);
            }
        }

        public static async Task<bool> LoadModel(string model)
        {
            WriteLine($"Loading Model {model}");
            // This won't trip-up on addon models, I checked
            if (!API.IsModelInCdimage((uint)API.GetHashKey(model)))
            {
                WriteLine($"ERROR! This model, {model} is not valid! This code will never work.");
            }
            else
            {
                API.RequestModel((uint)API.GetHashKey(model));
                while (!API.HasModelLoaded((uint)API.GetHashKey(model)))
                {
                    WriteLine($"Waiting for model {model} to load...");
                    await BaseScript.Delay(100);
                }
            }
            return true;
        }

        public static void WriteLine(string message)
        {
            Debug.WriteLine($"[PoliceMPCarGarage]: {message}");
        }

        /// <summary>
        ///     Gets the entity in front of the position
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="distance">The distance</param>
        /// <param name="flag">The raycast flag</param>
        /// <returns>The entity ID in front or -1 if none</returns>
        public static int GetEntityInFront(Vector3 position, float distance, int flag)
        {
            var inFront = API.GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, 0f, distance, 0f);

            // Raycast stuff
            var rayHandle = API.CastRayPointToPoint(position.X, position.Y, position.Z, inFront.X, inFront.Y, inFront.Z,
                flag, Game.PlayerPed.Handle, 0);
            var hit = false;
            var endCoords = Vector3.Zero;
            var surfaceNormal = Vector3.Zero;
            var entityHit = -1;

            API.GetRaycastResult(rayHandle, ref hit, ref endCoords, ref surfaceNormal, ref entityHit);

            return entityHit;
        }

        /// <summary>
        ///     Gets the vehicle in front of the player.
        /// </summary>
        /// <returns>The vehicle entity ID or null if none found.</returns>
        public static int GetVehicleInFrontOfPlayer()
        {
            var entity = -1;
            var distance = 2f;

            while (entity == -1)
            {
                entity = GetEntityInFront(Game.PlayerPed.Position, distance, 2);
                distance += 2f;

                if (distance >= 20f && entity == -1) break;
            }

            if (!API.DoesEntityExist(entity) || !API.IsEntityAVehicle(entity)) return -1;

            return entity;
        }

        private static void ShowNotification(string message)
            => ClientFunctions.ShowToast("Garage", message, "info");

        public class GarageConfig
        {
            public GarageEntry[] GarageEntry { get; set; }
            public GarageSpawn[] GarageSpawns { get; set; }
            public OutsideSpawn[] OutsideSpawns { get; set; }
            /*public CarData[] Cars { get; set; }*/
        }
        public class FullDestination
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float Heading { get; set; }
        }
        public class GarageEntry
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public FullDestination MarkerLocation { get; set; }
            public FullDestination OutsideLocation { get; set; }
        }
        public class GarageSpawn
        {
            public Guid Guid { get; set; }
            public float Heading { get; set; }
            public FullDestination PlayerLocation { get; set; }
        }
        public class OutsideSpawn
        {
            public Guid Guid { get; set; }
            public FullDestination Location { get; set; }
        }
        /*        public class CarData
                {
                    public string Model { get; set; }
                    public string Name { get; set; }
                    public string Category { get; set; }
                }*/
    }
}
