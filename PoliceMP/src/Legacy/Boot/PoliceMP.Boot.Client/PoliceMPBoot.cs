using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;

namespace PoliceMP.Boot.Client
{
    public class PoliceMPBoot : BaseScript
    {
        private Menu bootmenu = new Menu("Vehicle Boot", "");

        //Holders for whitelisting
        private bool AFOStatus = false;

        private bool RPUStatus = false;
        private bool CIDStatus = false;
        private bool NPASStatus = false;
        private bool MPUStatus = false;
        private bool AdminAuth = false;
        private bool DogStatus = false;
        private bool NHSStatus = false;
        private bool FireStatus = false;
        private bool ModAuth = false;
        private bool WhitelistedMember = false;

        private bool inputDisabled = false;

        private const string DOGMODEL = "a_c_shepherd"; //a_c_shepherd

        private bool dogout = false;
        private Ped dog;

        private WeaponHash FlashBangHash = (WeaponHash)API.GetHashKey("WEAPON_FLASHBANG");

        public PoliceMPBoot()
        {
            Tick += OnTick;
            Tick += DisableChecker;

            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.AddMenu(bootmenu);
            MenuController.AddMenu(dogMenu);

            DogMenuOptions();
        }

        //All whitelisting recieved for now, functionality for other units at a later date.
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

        private async Task DisableChecker()
        {
            if (!inputDisabled) return;

            if (!bootmenu.Visible) return;

            API.DisableAllControlActions(0);
            API.DisableAllControlActions(1);
        }

        private async Task OnTick()
        {
            int vehicleId = GetVehicleInFrontOfPlayer();
            Vehicle v = (Vehicle)Entity.FromHandle(vehicleId);

            //Do not continue if no vehicle
            if (vehicleId == -1) return;

            if (!v.IsNearEntity(Game.PlayerPed, new Vector3(3f, 3f, 3f))) { return; }

            //Checking emergency class
            if (API.GetVehicleClass(vehicleId) != 18) { return; }

            var door = GetVehicleDoorEntityIsLookingAt(Game.PlayerPed.Handle, vehicleId);
            if (door != VehicleDoorIndex.Trunk) { return; }

            if (bootmenu.Visible) { return; }

            Screen.DisplayHelpTextThisFrame("Press  ~INPUT_MP_TEXT_CHAT_TEAM~ ~w~to access the vehicle boot.");
            if (API.IsInputDisabled(2) && Game.IsControlPressed(0, (Control)246))
            {
                inputDisabled = true;
                Game.PlayerPed.Task.PlayAnimation("missexile3", "ex03_dingy_search_case_base_michael");
                API.SetVehicleDoorOpen(v.Handle, 5, false, false);

                if (v.Model.Hash == API.GetHashKey("addpolbmw5arv"))
                {
                    API.SetVehicleDoorOpen(v.Handle, 4, false, false);
                }

                reloadMenu();
                bootmenu.OpenMenu();

                bootmenu.OnMenuClose += (menu) =>
                {
                    inputDisabled = false;
                    Game.PlayerPed.Task.StandStill(1);
                    API.SetVehicleDoorShut(v.Handle, 5, false);
                    if (v.Model.Hash == API.GetHashKey("addpolbmw5arv"))
                    {
                        API.SetVehicleDoorShut(v.Handle, 4, false);
                    }
                };
            }
        }

        private void reloadMenu()
        {
            bootmenu.ClearMenuItems();
            var fa = new MenuItem("First Aid");
            var ar = new MenuItem("Body Armour");
            var keyur = new MenuItem("Unrack Enforcer");
            var keyr = new MenuItem("Rack Enforcer");
            var op1 = new MenuItem("Unrack Baton Gun");
            var op2 = new MenuItem("Rack Baton Gun");
            var op3 = new MenuItem("Unrack G36C");
            var op4 = new MenuItem("Rack G36C");
            var op5 = new MenuItem("Unrack MP5");
            var op6 = new MenuItem("Rack MP5");
            var op7 = new MenuItem("Grab Flashbangs (x5)");
            var op8 = new MenuItem("Store Flashbangs");
            var getdogout = new MenuItem("Deploy Dog");
            var putdogin = new MenuItem("Cage Dog");

            bootmenu.AddMenuItem(fa);
            bootmenu.AddMenuItem(ar);

            if (Game.PlayerPed.Weapons.HasWeapon(WeaponHash.GolfClub))
            {
                bootmenu.AddMenuItem(keyr);
            }
            else
            {
                bootmenu.AddMenuItem(keyur);
            }

            if (AFOStatus)
            {
                if (Game.PlayerPed.Weapons.HasWeapon(WeaponHash.PumpShotgun))
                {
                    bootmenu.AddMenuItem(op2);
                }
                else
                {
                    bootmenu.AddMenuItem(op1);
                }

                if (Game.PlayerPed.Weapons.HasWeapon(WeaponHash.SpecialCarbine))
                {
                    bootmenu.AddMenuItem(op4);
                }
                else
                {
                    bootmenu.AddMenuItem(op3);
                }

                if (Game.PlayerPed.Weapons.HasWeapon(WeaponHash.SMG))
                {
                    bootmenu.AddMenuItem(op6);
                }
                else
                {
                    bootmenu.AddMenuItem(op5);
                }

                if (Game.PlayerPed.Weapons.HasWeapon(FlashBangHash))
                {
                    bootmenu.AddMenuItem(op8);
                }
                else
                {
                    bootmenu.AddMenuItem(op7);
                }
            }

            if (DogStatus)
            {
                if (dogout)
                {
                    bootmenu.AddMenuItem(putdogin);
                }
                else
                {
                    bootmenu.AddMenuItem(getdogout);
                }
            }

            bootmenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == fa) Game.PlayerPed.Health = 100;
                if (item == ar) if (AFOStatus) { Game.PlayerPed.Armor = 100; } else { Game.PlayerPed.Armor = 50; }
                if (item == keyur) Game.PlayerPed.Weapons.Give(WeaponHash.GolfClub, 100, true, true);
                if (item == keyr) Game.PlayerPed.Weapons.Remove(WeaponHash.GolfClub);
                if (item == op1) Game.PlayerPed.Weapons.Give(WeaponHash.PumpShotgun, 100, true, true);
                if (item == op2) Game.PlayerPed.Weapons.Remove(WeaponHash.PumpShotgun);
                if (item == op3) Game.PlayerPed.Weapons.Give(WeaponHash.SpecialCarbine, 100, true, true);
                if (item == op4) Game.PlayerPed.Weapons.Remove(WeaponHash.SpecialCarbine);
                if (item == op5) Game.PlayerPed.Weapons.Give(WeaponHash.SMG, 100, true, true);
                if (item == op6) Game.PlayerPed.Weapons.Remove(WeaponHash.SMG);
                if (item == op7) Game.PlayerPed.Weapons.Give(FlashBangHash, 5, true, true);
                if (item == op8) Game.PlayerPed.Weapons.Remove(FlashBangHash);
                if (item == getdogout) { GetDog(); }
                if (item == putdogin) { ReturnDog(); }
                bootmenu.CloseMenu();
            };
        }

        public static bool IsEntityAtVehicleDoor(int entityId, int vehicleId, string boneName)
        {
            var doorPos =
                API.GetWorldPositionOfEntityBone(vehicleId, API.GetEntityBoneIndexByName(vehicleId, boneName));
            var entityPos = API.GetEntityCoords(entityId, true);

            var distance = API.GetDistanceBetweenCoords(entityPos.X, entityPos.Y, entityPos.Z, doorPos.X, doorPos.Y,
                doorPos.Z, true);

            return distance < 1f;
        }

        public static VehicleDoorIndex GetVehicleDoorEntityIsLookingAt(int entityId, int vehicleId)
        {
            if (IsEntityAtVehicleDoor(entityId, vehicleId, "door_dside_f"))
                return VehicleDoorIndex.FrontLeftDoor;

            if (IsEntityAtVehicleDoor(entityId, vehicleId, "door_dside_r"))
                return VehicleDoorIndex.BackLeftDoor;

            if (IsEntityAtVehicleDoor(entityId, vehicleId, "door_pside_f"))
                return VehicleDoorIndex.FrontRightDoor;

            if (IsEntityAtVehicleDoor(entityId, vehicleId, "door_pside_r"))
                return VehicleDoorIndex.BackRightDoor;

            return VehicleDoorIndex.Trunk;
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

        /// <summary>
        ///     Gets the vehicle in front of the player within a specified
        ///     heading.
        /// </summary>
        /// <param name="heading">The heading.</param>
        /// <returns>The vehicle entity id.</returns>
        public static int GetVehicleInFrontOfPlayer(float heading)
        {
            var vehicleId = GetVehicleInFrontOfPlayer();
            if (vehicleId == -1) return vehicleId;

            var vehicle = (Vehicle)Entity.FromHandle(vehicleId);
            var playerHeading = Game.PlayerPed.Heading;

            if (Math.Abs(vehicle.Heading - playerHeading) <= heading)
                return vehicleId;

            if (Math.Abs(vehicle.Heading - 180f - playerHeading) <= heading)
                return vehicleId;

            return -1;
        }

        private string[] dognames = { "Molly", "Alwyn", "Alsha", "Lala", "Eldon", "Lewis", "Tom", "weesav", "Barkus Maximus", "X Æ A-Xii" };

        private Menu dogMenu = new Menu("Dog Interactions", "LShift + X for send the dog.");

        private void DogMenuOptions()
        {
            var follow = new MenuItem("Follow");
            var wait = new MenuItem("Sit");
            var search = new MenuItem("Search Area");
            var hs = new MenuItem("Hide and Seek");
            var bark = new MenuItem("Bark");

            dogMenu.AddMenuItem(follow);
            dogMenu.AddMenuItem(wait);
            dogMenu.AddMenuItem(search);
            dogMenu.AddMenuItem(hs);
            dogMenu.AddMenuItem(bark);

            dogMenu.OnItemSelect += (menu, item, index) =>
            {
                if (item == follow) DogFollow();
                if (item == wait) DogWait();
                if (item == search) SearchArea();
                if (item == hs) HideAndSeek();
                if (item == bark) DogBark();
                bootmenu.CloseMenu();
            };
        }

        private async Task GetDog()
        {
            if (dogout) { return; }

            API.RequestModel((uint)API.GetHashKey(DOGMODEL));
            while (!API.HasModelLoaded((uint)API.GetHashKey(DOGMODEL)))
            {
                await Delay(100);
            }
            dog = await World.CreatePed(DOGMODEL, Game.PlayerPed.Position, 0F);

            Random r = new Random();
            string dogname = dognames[r.Next(dognames.Length)];
            TriggerEvent("PoliceMP:ShowNotification", "PoliceMP",
                $"<span class='text-primary'>PD {dogname}</span> has been deployed, press <span class='text-warning'>Shift + D</span> for dog menu.",
                "info");

            blip = dog.AttachBlip();
            blip.Sprite = (BlipSprite)442;
            blip.Color = BlipColor.MichaelBlue;
            blip.Name = "Police Dog";

            API.SetPedComponentVariation(dog.Handle, 0, 0, 2, 1);
            API.SetEntityAsMissionEntity(dog.Handle, true, true);
            API.SetPedAsCop(dog.Handle, true);

            uint hash = (uint)API.GetPedRelationshipGroupHash(Game.PlayerPed.Handle);
            API.SetPedRelationshipGroupHash(Game.PlayerPed.Handle, hash);

            SyncDogToAllClients();

            dogout = true;
            DogFollow();
        }

        private void ReturnDog()
        {
            if (!dogout) { return; }

            if (dog == null) { return; }

            ClearTasks();

            blip.Delete();

            dog.Delete();

            dogout = false;
        }

        private async Task DogFollow()
        {
            await ClearTasks();
            API.TaskFollowToOffsetOfEntity(dog.Handle, Game.PlayerPed.Handle, 1f, 1f, 0f, 10f, -1, -1, true);
        }

        private bool shouldsit = false;

        private async Task DogWait()
        {
            await ClearTasks();
            shouldsit = true;
        }

        [Tick]
        private async Task Sit()
        {
            if (!shouldsit) { return; }
            dog.Task.PlayAnimation("creatures@rottweiler@amb@world_dog_sitting@base", "base");
        }

        private async Task DogBark()
        {
            await ClearTasks();
            API.TaskStartScenarioInPlace(dog.Handle, "WORLD_DOG_BARKING_ROTTWEILER", 0, true);
        }

        private async Task SearchArea()
        {
            await ClearTasks();
            Vector3 p = Game.PlayerPed.Position;
            API.TaskWanderInArea(dog.Handle, p.X, p.Y, p.Z, 15f, 0, 0);
        }

        private async Task HideAndSeek()
        {
            ClearTasks();
            API.TaskSeekCoverFromPed(dog.Handle, Game.PlayerPed.Handle, 60, true);
        }

        private async Task ClearTasks()
        {
            shouldsit = false;
            chasingSuspect = false;
            API.ClearPedTasks(dog.Handle);
        }

        private Blip blip;

        //bool shouldChase = false;
        [Tick]
        private async Task DogCheck()
        {
            if (!dogout) { return; }

            if (API.IsControlPressed(0, 21) && API.IsControlJustPressed(0, 30))
            {
                if (dogMenu.Visible) { return; }
                dogMenu.OpenMenu();
            }

            if (API.IsControlPressed(0, (int)Control.VehicleDropProjectile) && API.IsControlPressed(0, (int)Control.Sprint))
            {
                if (dogMenu.Visible) { return; }

                int suspectEntityHandle = 0;
                API.GetEntityPlayerIsFreeAimingAt(Game.Player.Handle, ref suspectEntityHandle);

                if (API.IsPedAPlayer(suspectEntityHandle)) { return; }
                if (API.IsPedDeadOrDying(suspectEntityHandle, true)) { return; }

                suspectEntity = (Ped)Ped.FromHandle(suspectEntityHandle);
                API.NetworkRequestControlOfNetworkId(suspectEntity.NetworkId);
                API.SetNetworkIdCanMigrate(suspectEntity.NetworkId, false);

                if (API.IsEntityAPed(suspectEntity.Handle))
                {
                    ClearTasks();
                    API.TaskFollowToOffsetOfEntity(dog.Handle, suspectEntity.Handle, 0f, 0f, 0f, 10f, -1, -1, true);

                    if (!suspectEntity.IsFleeing)
                    {
                        suspectEntity.Task.WanderAround();
                    }

                    API.SetEntityAsMissionEntity(suspectEntity.Handle, true, true);
                    chasingSuspect = true;
                    Screen.ShowSubtitle("Woof Woof!");
                }
            }
        }

        private bool chasingSuspect = false;
        private Ped suspectEntity;

        [Tick]
        private async Task CheckSuspect()
        {
            if (!chasingSuspect) { return; }

            if (API.GetDistanceBetweenCoords(dog.Position.X, dog.Position.Y, dog.Position.Z, suspectEntity.Position.X, suspectEntity.Position.Y, suspectEntity.Position.Z, true) <= 5f)
            {
                API.NetworkRequestControlOfNetworkId(suspectEntity.NetworkId);
                API.SetNetworkIdCanMigrate(suspectEntity.NetworkId, true);
                suspectEntity.Task.ClearAllImmediately();
                suspectEntity.Task.StandStill(-1);
                API.TaskStartScenarioInPlace(suspectEntity.Handle, "WORLD_HUMAN_SUNBATHE", 0, true);
            }

            if (API.GetDistanceBetweenCoords(dog.Position.X, dog.Position.Y, dog.Position.Z, suspectEntity.Position.X, suspectEntity.Position.Y, suspectEntity.Position.Z, true) <= 1f)
            {
                API.NetworkRequestControlOfNetworkId(dog.NetworkId);
                chasingSuspect = false;
                DogWait();
            }
        }

        private void SyncDogToAllClients()
        {
            for (int i = 0; i <= 256; i++)
            {
                if (API.NetworkIsPlayerActive(i))
                {
                    API.SetNetworkIdSyncToPlayer(dog.NetworkId, i, true);
                }
            }
        }

        [EventHandler("PoliceMP:DogSectionUpdateClient")]
        private void DogSectionUpdate(int _pl, int _switchToNet)
        {
            if (_pl == Game.Player.Handle) { return; }

            Entity e;
            try
            {
                e = Entity.FromNetworkId(_switchToNet);
                API.ChangePlayerPed(_pl, e.Handle, false, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DOG SECTION ISSUE: " + ex);
            }
        }
    }
}