using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using PoliceMP.Main.Core.Client;

namespace PoliceMP.Toolbox.Client
{
    public class PoliceMPToolbox : BaseScript
    {
        public Menu toolbox = new Menu("PoliceMP Toolbox", "Everything you may need for your duty");

        private int prevModelHash;

        private List<PickupType> PickupList = new List<PickupType>();
        private List<WeaponHash> forbiddenWeapons = new List<WeaponHash>();

        private Vector3 _playerSpawnPosition = Vector3.Zero;

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

        [EventHandler("onClientResourceStart")]
        public void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;

            Debug.WriteLine("PoliceMPToolbox ctor");
            TriggerServerEvent("PoliceMP:requestingSpecWhitelisting", false);

            //Weapon Hashes
            forbiddenWeapons.Add(WeaponHash.AdvancedRifle);
            forbiddenWeapons.Add(WeaponHash.APPistol);
            forbiddenWeapons.Add(WeaponHash.AssaultRifle);
            forbiddenWeapons.Add(WeaponHash.AssaultRifleMk2);
            forbiddenWeapons.Add(WeaponHash.AssaultShotgun);
            forbiddenWeapons.Add(WeaponHash.AssaultSMG);
            forbiddenWeapons.Add(WeaponHash.Ball);
            forbiddenWeapons.Add(WeaponHash.Bat);
            //forbiddenWeapons.Add(WeaponHash.BattleAxe);
            forbiddenWeapons.Add(WeaponHash.Bottle);
            forbiddenWeapons.Add(WeaponHash.BullpupRifle);
            forbiddenWeapons.Add(WeaponHash.BullpupRifleMk2);
            forbiddenWeapons.Add(WeaponHash.BullpupShotgun);
            forbiddenWeapons.Add(WeaponHash.BZGas);
            forbiddenWeapons.Add(WeaponHash.CarbineRifle);
            forbiddenWeapons.Add(WeaponHash.CarbineRifleMk2);
            forbiddenWeapons.Add(WeaponHash.CombatMG);
            forbiddenWeapons.Add(WeaponHash.CombatMGMk2);
            forbiddenWeapons.Add(WeaponHash.CombatPDW);
            //forbiddenWeapons.Add(WeaponHash.CombatPistol);
            forbiddenWeapons.Add(WeaponHash.CompactGrenadeLauncher);
            forbiddenWeapons.Add(WeaponHash.CompactRifle);
            forbiddenWeapons.Add(WeaponHash.Crowbar);
            forbiddenWeapons.Add(WeaponHash.Dagger);
            forbiddenWeapons.Add(WeaponHash.DoubleAction);
            forbiddenWeapons.Add(WeaponHash.DoubleBarrelShotgun);
            //forbiddenWeapons.Add(WeaponHash.FireExtinguisher);
            forbiddenWeapons.Add(WeaponHash.Firework);
            forbiddenWeapons.Add(WeaponHash.Flare);
            forbiddenWeapons.Add(WeaponHash.FlareGun);
            //forbiddenWeapons.Add(WeaponHash.Flashlight);
            //forbiddenWeapons.Add(WeaponHash.GolfClub);
            forbiddenWeapons.Add(WeaponHash.Grenade);
            forbiddenWeapons.Add(WeaponHash.GrenadeLauncher);
            forbiddenWeapons.Add(WeaponHash.GrenadeLauncherSmoke);
            forbiddenWeapons.Add(WeaponHash.Gusenberg);
            forbiddenWeapons.Add(WeaponHash.Hammer);
            forbiddenWeapons.Add(WeaponHash.Hatchet);
            forbiddenWeapons.Add(WeaponHash.HeavyPistol);
            forbiddenWeapons.Add(WeaponHash.HeavyShotgun);
            forbiddenWeapons.Add(WeaponHash.HeavySniper);
            //forbiddenWeapons.Add(WeaponHash.HeavySniperMk2);
            forbiddenWeapons.Add(WeaponHash.HomingLauncher);
            forbiddenWeapons.Add(WeaponHash.Knife);
            forbiddenWeapons.Add(WeaponHash.KnuckleDuster);
            forbiddenWeapons.Add(WeaponHash.Machete);
            forbiddenWeapons.Add(WeaponHash.MachinePistol);
            forbiddenWeapons.Add(WeaponHash.MarksmanPistol);
            //forbiddenWeapons.Add(WeaponHash.MarksmanRifle);
            forbiddenWeapons.Add(WeaponHash.MarksmanRifleMk2);
            forbiddenWeapons.Add(WeaponHash.MG);
            //forbiddenWeapons.Add(WeaponHash.MicroSMG);
            forbiddenWeapons.Add(WeaponHash.Minigun);
            forbiddenWeapons.Add(WeaponHash.MiniSMG);
            forbiddenWeapons.Add(WeaponHash.Molotov);
            //forbiddenWeapons.Add(WeaponHash.Musket);
            //forbiddenWeapons.Add(WeaponHash.Nightstick);
            forbiddenWeapons.Add(WeaponHash.NightVision);
            //forbiddenWeapons.Add(WeaponHash.Parachute);
            forbiddenWeapons.Add(WeaponHash.PetrolCan);
            forbiddenWeapons.Add(WeaponHash.PipeBomb);
            //forbiddenWeapons.Add(WeaponHash.Pistol);
            //forbiddenWeapons.Add(WeaponHash.Pistol50);
            forbiddenWeapons.Add(WeaponHash.PistolMk2);
            forbiddenWeapons.Add(WeaponHash.PoolCue);
            forbiddenWeapons.Add(WeaponHash.ProximityMine);
            //forbiddenWeapons.Add(WeaponHash.PumpShotgun);
            //forbiddenWeapons.Add(WeaponHash.PumpShotgunMk2);
            forbiddenWeapons.Add(WeaponHash.Railgun);
            forbiddenWeapons.Add(WeaponHash.RayCarbine);
            forbiddenWeapons.Add(WeaponHash.RayMinigun);
            forbiddenWeapons.Add(WeaponHash.RayPistol);
            forbiddenWeapons.Add(WeaponHash.Revolver);
            forbiddenWeapons.Add(WeaponHash.RevolverMk2);
            forbiddenWeapons.Add(WeaponHash.RPG);
            //forbiddenWeapons.Add(WeaponHash.SawnOffShotgun);
            //forbiddenWeapons.Add(WeaponHash.SMG);
            forbiddenWeapons.Add(WeaponHash.SMGMk2);
            forbiddenWeapons.Add(WeaponHash.SmokeGrenade);
            //forbiddenWeapons.Add(WeaponHash.SniperRifle);
            forbiddenWeapons.Add(WeaponHash.Snowball);
            //forbiddenWeapons.Add(WeaponHash.SNSPistol);
            forbiddenWeapons.Add(WeaponHash.SNSPistolMk2);
            //forbiddenWeapons.Add(WeaponHash.SpecialCarbine);
            forbiddenWeapons.Add(WeaponHash.SpecialCarbineMk2);
            forbiddenWeapons.Add(WeaponHash.StickyBomb);
            forbiddenWeapons.Add(WeaponHash.StoneHatchet);
            //forbiddenWeapons.Add(WeaponHash.StunGun);
            forbiddenWeapons.Add(WeaponHash.SweeperShotgun);
            forbiddenWeapons.Add(WeaponHash.SwitchBlade);
            //forbiddenWeapons.Add(WeaponHash.Unarmed);
            //forbiddenWeapons.Add(WeaponHash.VintagePistol);
            forbiddenWeapons.Add(WeaponHash.Wrench);

            //Pickups
            PickupList.Add(PickupType.AmmoBulletMP);
            PickupList.Add(PickupType.AmmoGrenadeLauncher);
            PickupList.Add(PickupType.AmmoGrenadeLauncherMP);
            PickupList.Add(PickupType.AmmoMG);
            PickupList.Add(PickupType.AmmoMinigun);
            PickupList.Add(PickupType.AmmoMissileMP);
            PickupList.Add(PickupType.AmmoPistol);
            PickupList.Add(PickupType.AmmoRifle);
            PickupList.Add(PickupType.AmmoRPG);
            PickupList.Add(PickupType.AmmoShotgun);
            PickupList.Add(PickupType.AmmoSMG);
            PickupList.Add(PickupType.AmmoSniper);
            PickupList.Add(PickupType.Armour);
            PickupList.Add(PickupType.CustomScript);
            PickupList.Add(PickupType.Health);
            PickupList.Add(PickupType.HealthSnack);
            PickupList.Add(PickupType.MoneyCase);
            PickupList.Add(PickupType.MoneyDepBag);
            PickupList.Add(PickupType.MoneyMedBag);
            PickupList.Add(PickupType.MoneyPaperBag);
            PickupList.Add(PickupType.MoneyPurse);
            PickupList.Add(PickupType.MoneySecurityCase);
            PickupList.Add(PickupType.MoneyVariable);
            PickupList.Add(PickupType.MoneyWallet);
            PickupList.Add(PickupType.Parachute);
            PickupList.Add(PickupType.PortableCrateUnfixed);
            PickupList.Add(PickupType.PortablePackage);
            PickupList.Add(PickupType.VehicleCustomScript);
            PickupList.Add(PickupType.VehicleHealth);
            PickupList.Add(PickupType.VehicleWeaponAPPistol);
            PickupList.Add(PickupType.VehicleWeaponCombatPistol);
            PickupList.Add(PickupType.VehicleWeaponGrenade);
            PickupList.Add(PickupType.VehicleWeaponMicroSMG);
            PickupList.Add(PickupType.VehicleWeaponMolotov);
            PickupList.Add(PickupType.VehicleWeaponPistol);
            PickupList.Add(PickupType.VehicleWeaponSawnoffShotgun);
            PickupList.Add(PickupType.VehicleWeaponSmokeGrenade);
            PickupList.Add(PickupType.VehicleWeaponStickyBomb);
            PickupList.Add(PickupType.WeaponAdvancedRifle);
            PickupList.Add(PickupType.WeaponAPPistol);
            PickupList.Add(PickupType.WeaponAssaultRifle);
            PickupList.Add(PickupType.WeaponAssaultShotgun);
            PickupList.Add(PickupType.WeaponBat);
            PickupList.Add(PickupType.WeaponBottle);
            PickupList.Add(PickupType.WeaponBullpupRifle);
            PickupList.Add(PickupType.WeaponCarbineRifle);
            PickupList.Add(PickupType.WeaponCombatMG);
            PickupList.Add(PickupType.WeaponCombatPistol);
            PickupList.Add(PickupType.WeaponCrowbar);
            PickupList.Add(PickupType.WeaponGolfclub);
            PickupList.Add(PickupType.WeaponGrenade);
            PickupList.Add(PickupType.WeaponGrenadeLauncher);
            PickupList.Add(PickupType.WeaponHeavyPistol);
            PickupList.Add(PickupType.WeaponHeavySniper);
            PickupList.Add(PickupType.WeaponKnife);
            PickupList.Add(PickupType.WeaponMG);
            PickupList.Add(PickupType.WeaponMicroSMG);
            PickupList.Add(PickupType.WeaponMinigun);
            PickupList.Add(PickupType.WeaponMolotov);
            PickupList.Add(PickupType.WeaponNightstick);
            PickupList.Add(PickupType.WeaponPetrolCan);
            PickupList.Add(PickupType.WeaponPistol);
            PickupList.Add(PickupType.WeaponPumpShotgun);
            PickupList.Add(PickupType.WeaponRPG);
            PickupList.Add(PickupType.WeaponSawnoffShotgun);
            PickupList.Add(PickupType.WeaponSMG);
            PickupList.Add(PickupType.WeaponSmokeGrenade);
            PickupList.Add(PickupType.WeaponSniperRifle);
            PickupList.Add(PickupType.WeaponSNSPistol);
            PickupList.Add(PickupType.WeaponSpecialCarbine);
            PickupList.Add(PickupType.WeaponStickyBomb);

            Game.PlayerPed.CanBeDraggedOutOfVehicle = false;
            CreateMenus();

            Game.PlayerPed.Weapons.RemoveAll();
            API.SetPedAsCop(API.PlayerPedId(), true);

            prevModelHash = Game.PlayerPed.GetHashCode();

            TriggerEvent("PoliceMP:ShowNotification", "PoliceMP",
                "You are now on duty as a Police Officer.<br><br> " +
                "Please take clothing from the lockers and vehicles from the garage outside. " +
                "Equipment will automatically be given to you.", "info");

            Game.PlayerPed.Health = 100;
            if (AFOStatus) { Game.PlayerPed.Armor = 100; } else { Game.PlayerPed.Armor = 50; }

            Game.PlayerPed.Weapons.Give(WeaponHash.FireExtinguisher, 100, true, true);
            API.SetPedInfiniteAmmo(Game.PlayerPed.Handle, true, (uint)WeaponHash.FireExtinguisher);
            Game.PlayerPed.Weapons.Give(WeaponHash.Flashlight, 100, true, true);
            Game.PlayerPed.Weapons.Give(WeaponHash.Nightstick, 100, true, true);
            Game.PlayerPed.Weapons.Give(WeaponHash.VintagePistol, 100, true, true);
            Game.PlayerPed.Weapons.Give(WeaponHash.StunGun, 100, true, true);

            if (AFOStatus)
            {
                Game.PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);
            }
        }

        private void CreateMenus()
        {
            //Menu Header
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.AddMenu(toolbox);

            //Main menu
            var fix = new MenuItem("Fix Vehicle", "Fix your vehicle");
            var roads = new MenuItem("Roads Management", "Display the roads management menu") { Label = "→→→" };
            var topd = new MenuItem("Go To Station", "Immediatly teleport to main police station");
            var colour = new MenuItem("Colour Car");
            var help = new MenuItem("Help", "Display help messages");
            var discord = new MenuItem("Discord", "Display discord address");

            toolbox.AddMenuItem(fix);
            toolbox.AddMenuItem(topd);
            toolbox.AddMenuItem(colour);
            toolbox.AddMenuItem(help);
            toolbox.AddMenuItem(discord);

            toolbox.OnItemSelect += (menu, item, index) =>
            {
                if (item == fix) API.ExecuteCommand("fix");
                if (item == topd) API.ExecuteCommand("topd");
                if (item == colour) API.ExecuteCommand("colourme");
                if (item == help) API.ExecuteCommand("help");
                if (item == discord) API.ExecuteCommand("discord");
            };
        }

        [Tick]
        private Task WhosTalking()
        {
            API.DistantCopCarSirens(false);

            bool currentlyTalking = false;
            string outputtext = "";

            foreach (Player p in Players)
            {
                if (API.NetworkIsPlayerTalking(p.Handle))
                {
                    if (!currentlyTalking)
                    {
                        outputtext += "~s~Currently Talking: ~n~";
                        currentlyTalking = true;
                    }
                    if (API.GetDistanceBetweenCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, p.Character.Position.X, p.Character.Position.Y, p.Character.Position.Z, true) >= 9f)
                    {
                        outputtext += "~p~[RADIO] ~b~" + p.Name + "~n~";
                    }
                    else
                    {
                        outputtext += "~b~" + p.Name + "~n~";
                    }
                }
                if (currentlyTalking)
                {
                    DrawTextOnScreen(outputtext, 0.5f, 0.00f, 0.5f, Alignment.Center, 6, false);
                }
            }

            return Task.FromResult(0);
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

            if (AFOStatus)
            {
                Game.PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);
            }
        }

        [Tick]
        private Task WantedHandler()
        {
            if (API.GetPlayerWantedLevel(API.PlayerId()) != 0)
            {
                API.SetPlayerWantedLevel(API.PlayerId(), 0, false);
                API.SetPlayerWantedLevelNow(API.PlayerId(), false);
                API.SetPlayerWantedLevelNoDrop(API.PlayerId(), 0, false);
            }

            for (int i = 1; i == 16; i++)
            {
                API.EnableDispatchService(i, false);
            }

            return Task.FromResult(0);
        }

        [Tick]
        private Task OpenMenuCheck()
        {
            if (toolbox.Visible || toolbox.Visible || MenuController.IsAnyMenuOpen()) { return Task.FromResult(0); }
            if (API.IsInputDisabled(2) && Game.IsControlJustPressed(0, (Control)288))
            {
                toolbox.OpenMenu();
            }

            return Task.FromResult(0);
        }

        [Tick]
        private async Task WeaponManager()
        {
            if (Game.PlayerPed.GetHashCode() != prevModelHash)
            {
                prevModelHash = Game.PlayerPed.GetHashCode();
                Game.PlayerPed.Weapons.Give(WeaponHash.FireExtinguisher, 100, true, true);
                API.SetPedInfiniteAmmo(Game.PlayerPed.Handle, true, (uint)WeaponHash.FireExtinguisher);
                Game.PlayerPed.Weapons.Give(WeaponHash.Flashlight, 100, true, true);
                Game.PlayerPed.Weapons.Give(WeaponHash.Nightstick, 100, true, true);
                //Game.PlayerPed.Weapons.Give(WeaponHash.VintagePistol, 100, true, true);

                if (WhitelistedMember)
                {
                    Game.PlayerPed.Weapons.Give(WeaponHash.StunGun, 100, true, true);
                }
                
                if (AFOStatus)
                {
                    //Game.PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 100, true, true);
                }
            }

            foreach (PickupType pt in PickupList)
            {
                uint ph = (uint)API.GetHashKey(pt.ToString());
                Vector3 pos = Game.PlayerPed.Position;
                if (API.IsPickupWithinRadius(ph, pos.X, pos.Y, pos.Z, 20f))
                {
                    API.RemoveAllPickupsOfType(ph);
                }
            }

            foreach (WeaponHash forbidenHash in forbiddenWeapons)
            {
                if (Game.PlayerPed.Weapons.HasWeapon(forbidenHash))
                {
                    Game.PlayerPed.Weapons.Remove(forbidenHash);
                }
            }

            if (!AFOStatus)
            {
                Game.PlayerPed.Weapons.Remove(WeaponHash.CombatPistol);
                Game.PlayerPed.Weapons.Remove(WeaponHash.PumpShotgun);
                Game.PlayerPed.Weapons.Remove(WeaponHash.SpecialCarbine);
                Game.PlayerPed.Weapons.Remove(WeaponHash.SMG);
                Game.PlayerPed.Weapons.Remove(WeaponHash.SniperRifle);
                Game.PlayerPed.Weapons.Remove(WeaponHash.SNSPistol);
                Game.PlayerPed.Weapons.Remove(WeaponHash.Pistol);
                Game.PlayerPed.Weapons.Remove(WeaponHash.SawnOffShotgun);
                Game.PlayerPed.Weapons.Remove(WeaponHash.MarksmanRifle);
            }

            await Delay(0);
        }

        [Command("whitelist")]
        private void whitelist()
        {
            Debug.WriteLine("requesting");
            TriggerServerEvent("PoliceMP:requestingSpecWhitelisting", true);
        }

        [Command("fix")]
        private void fixVehicle()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Game.PlayerPed.CurrentVehicle.Repair();
                Game.PlayerPed.CurrentVehicle.Wash();
                Game.PlayerPed.CurrentVehicle.Speed = 0;
            }
        }

        [Command("topd")]
        private Task GoToPD()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                if (Game.PlayerPed.CurrentVehicle.Driver == Game.PlayerPed)
                {
                    Game.PlayerPed.CurrentVehicle.Delete();
                }
            }
            API.DoScreenFadeOut(100);
            API.NetworkFadeOutEntity(Game.PlayerPed.Handle, true, false);

            var positionZero = _playerSpawnPosition == Vector3.Zero;
            var spawnPosition = positionZero ? new Vector3(427, -980, 30) : _playerSpawnPosition;
            API.SetEntityCoords(Game.PlayerPed.Handle, spawnPosition.X, spawnPosition.Y, spawnPosition.Z, false, false, false, false);

            API.NetworkFadeInEntity(Game.PlayerPed.Handle, true);
            API.DoScreenFadeIn(100);
            return Task.FromResult(0);
        }

        [EventHandler("PoliceMP:SetPlayerSpawnLocation")]
        private void SetPlayerSpawnPosition(float x, float y, float z)
        {
            _playerSpawnPosition = new Vector3(x, y, z);
        }

        [Command("help")]
        private void help()
        {
            ClientFunctions.ShowToast("General Info",
                "Skin can be changed in the locker room <br>" +
                "Vehicles are available in the menu outside <br><br>" +
                "Controls: <br>" +
                "<span class='text-warning'>E</span> = Interaction Key <br>" +
                "<span class='text-warning'>SHIFT</span> = Stop Vehicle <br>" +
                "<span class='text-warning'>M</span> = Vehicle Menu <br>" +
                "<span class='text-warning'>F1</span> = Police Toolbox <br>" +
                "<span class='text-warning'>F2</span> = Police Backup <br>" +
                "<span class='text-warning'>G</span> = Police Radio <br>" +
                "<span class='text-warning'>Q, Alt, (1,2,3)</span> = ELS MAIN CONTROLS <br>" +
                "<span class='text-warning'>Y, C</span> = Speedgun (Vintage Pistol) Controls", "info");
        }

        [Command("discord")]
        private void discord()
        {
            ClientFunctions.ShowToast("Discord", "Discord is available at: discord.policemp.com or https://discord.gg/7YZ42gk", "info");
        }

        //List of all the colours that should be accepted.
        //If this needs edited: https://pastebin.com/pwHci0xK
        private int[] AcceptedColours =
        {
            0,      //Classic - Black
            147,    //Classic - Carbon Black
            1,      //Classic - Graphite
            2,      //Classic - Black Steel
            4,      //Classic - Silver
            8,      //Classic - Stone Silver
            9,      //Classic - Midnight Silver
            27,     //Classic - Red
            141,    //Classic - Midnight Blue
            62,     //Classic - Dark Blue
            63,     //Classic - Saxon Blue
            64,     //Classic - Diamond Blue
            101,    //Classic - Bison Brown
            105,    //Classic - Sandy Brown
            12,     //Matte - Black
            13,     //Matte - Gray
            14,     //Matte - Light Grey
            83,     //Matte - Blue
            82,     //Matte - Dark Blue
            84,     //Matte - Midnight Blue
            39,     //Matte - Red
            40,     //Matte - Dark Red
        };

        [Command("colourme")]
        private void ColourMe()
        {
            if (AdminAuth || RPUStatus || AFOStatus || CIDStatus || DogStatus)
            {
                if (API.IsPedInAnyPoliceVehicle(Game.PlayerPed.Handle))
                {
                    if (Game.PlayerPed.CurrentVehicle.Driver == Game.PlayerPed)
                    {
                        Random rand = new Random();
                        int r = rand.Next(0, AcceptedColours.Length);
                        API.SetVehicleColours(Game.PlayerPed.CurrentVehicle.Handle, r, r);
                    }
                    else
                    {
                        ClientFunctions.ShowToast("Toolbox",
                            "You must be the driver of a vehicle to use this command.",
                            "error");
                    }
                }
                else
                {
                    ClientFunctions.ShowToast("Toolbox",
                        "You must be in a vehicle to use this command.",
                        "error");
                }
            }
        }

        public static void ShowAdvancedNotification(string text, string title, string subtitle, string icon, int type, bool flash = false)
        {
            API.SetNotificationTextEntry("STRING");
            API.AddTextComponentString(text);
            API.SetNotificationMessage(icon, icon, flash, type, title, subtitle);
            API.DrawNotification(false, true);
        }

        private static void DrawTextOnScreen(string text, float xPosition, float yPosition, float size, Alignment justification, int font, bool disableTextOutline)
        {
            API.SetTextFont(font);
            API.SetTextScale(1.0f, size);
            if (justification == CitizenFX.Core.UI.Alignment.Right)
            {
                API.SetTextWrap(0f, xPosition);
            }
            API.SetTextJustification((int)justification);
            if (!disableTextOutline) { API.SetTextOutline(); }
            API.BeginTextCommandDisplayText("STRING");
            API.AddTextComponentSubstringPlayerName(text);
            API.EndTextCommandDisplayText(xPosition, yPosition);
        }

        [Tick]
        private Task VehicleCheck()
        {
            API.SetParkedVehicleDensityMultiplierThisFrame(1);
            API.SetPedDensityMultiplierThisFrame(1);
            API.SetRandomVehicleDensityMultiplierThisFrame(1);
            API.SetScenarioPedDensityMultiplierThisFrame(1, 1);
            API.SetSomeVehicleDensityMultiplierThisFrame(1);
            API.SetVehicleDensityMultiplierThisFrame(1);

            return Task.FromResult(0);
        }
    }
}