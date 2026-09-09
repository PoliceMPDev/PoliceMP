using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Scripts.CivVehicleContents
{
    public class CivVehicleContents : Script
    {
        private readonly ICommandManager _commandManager;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<CivVehicleContents> _logger;

        #region Item Dictionaries

        private Dictionary<string, int> vehicleSearchPossibleItems = new Dictionary<string, int>
        {
            { "Nothing of Interest", 1 },
            { "Smart Phone", 2 },
            { "Burner Phone", 4 },
            { "Wallet", 8 },
            { "Car Keys", 16 },
            { "House Keys", 32 },
            { "Purse", 64 },
            { "Handbag", 128 },
            { "Laptop", 256 },
            { "Electronic Devices", 512 },
            { "Sunglasses", 1024 },
            { "Torch", 2048 },
            { "Camera", 4096 },
            { "Stolen IDs", 8192 },
            { "Fake IDs", 16384 },
            { "Stolen Mail", 32768 },
            { "Stolen Checks", 65536 },
            { "Duct Tape", 131072 },
            { "Plastic Sheet", 262144 },
            { "Lock Pick", 524288 },
            { "Counterfeit Money", 1048576 },
            { "Bloody Hammer", 2097152 },
            { "Documentation", 4194304 },
            { "Parking Ticket", 8388608 },
            { "Food Wrappers", 16777216 },
            { "Waste Bags", 33554432 },
            { "Shovel", 67108864 }
        };

        private Dictionary<string, int> vehicleSearchPossibleItems2 =
            new
                Dictionary<string, int> // ENSURE THIS DICTIONARY IS UP TO DATE/IDENTICAL WITH THE ONE IN `SearchVehicleHandler.cs`
                {
                    { "Knife", 1 },
                    { "Switchblade", 2 },
                    { "Machete", 4 },
                    { "Handgun", 8 },
                    { "Semi-Automatic Rifle", 16 },
                    { "Fully-Automatic Rifle", 32 },
                    { "Magazines", 64 },
                    { "Loose Ammo", 128 },
                    { "Weapon Parts", 256 },
                    { "Weapon Casing", 512 },
                    { "Cocaine Parcel", 1024 },
                    { "Cannabis Grinder", 2048 },
                    { "Spliff", 4096 },
                    { "Briefcase of Drugs", 8192 },
                    { "Small Bags of Drugs", 16384 },
                    { "Packet of Cigarettes", 32768 },
                    { "Packet of Tobacco", 65536 },
                    { "Vape", 131072 },
                    { "Chewing Gum", 262144 },
                    { "Chocolate Bar", 524288 },
                    { "Crisps", 1048576 },
                    { "Kinder Eggs", 2097152 },
                    { "Cash £200", 4194304 },
                    { "Cash £500", 8388608 },
                    { "Cash £2000", 16777216 },
                    { "Cash £10000", 33554432 },
                };

        private Dictionary<string, int> vehicleSearchPossibleItems3 =
            new
                Dictionary<string, int> // ENSURE THIS DICTIONARY IS UP TO DATE/IDENTICAL WITH THE ONE IN `SearchVehicleHandler.cs`
                {
                    { "Gift Card", 1 },
                    { "Key Maker", 2 },
                    { "Vehicle Hacking Device", 4 },
                    { "Fake Pistol", 8 },
                    { "Credit Card Reader", 16 },
                    { "Balaclava", 32 },
                    { "Ski Mask", 64 },
                    { "Power Drill", 128 },
                    { "Opened Bottle", 256 },
                    { "Empty Cans", 512 },
                    { "Stolen Jewellery", 1024 },
                    { "Gloves", 2048 },
                    { "Tracking Device", 4096 },
                    { "Blueprints", 8192 },
                    { "Black Market Catalogue", 16384 },
                    { "Number Plates", 32768 },
                    { "Radio Scanner", 65536 },
                };

        private Dictionary<string, int> vehicleSearchPossibleItemsSenior =
            new
                Dictionary<string, int> // ENSURE THIS DICTIONARY IS UP TO DATE/IDENTICAL WITH THE ONE IN `SearchVehicleHandler.cs`
                {
                    { "C55", 1 },
                    { "CHEMTEX", 2 },
                    { "Trinitrotoluene (TNT)", 4 },
                    { "Nitrocellulose", 8 },
                    { "Gunpowder", 16 },
                    { "Oxidisers", 32 },
                    { "RDX", 64 },
                    { "Nitroglycerine", 128 },
                    { "Hexamethylene Triperoxide Diamine (HMTD)", 256 },
                    { "Triacetone Triperoxide (TATP)", 512 },
                    { "Nitrate Fertilizers", 1024 },
                    { "Household Chemicals", 2048 }
                };

        #endregion Item Dictionaries


        public CivVehicleContents(ICommandManager commandManager, ILogger<CivVehicleContents> logger,
            INewNotificationOverlay newNotificationOverlay, IPermissionService permissionService, ITickManager ticks,
            IPlayerService playerService, ILegacyClientCommunicationsManager comms, ICommonFunctionsService common,
            IFeatureService featureService)
        {
            _commandManager = commandManager;
            _newNotificationOverlay = newNotificationOverlay;
            _permissionService = permissionService;
            _ticks = ticks;
            _commandManager = commandManager;
            _logger = logger;
            _permissionService = permissionService;
        }


        protected override async Task OnStartAsync()
        {
            API.DecorRegister("PoliceMP_Vehicle_Inventory1", 3);
            API.DecorRegister("PoliceMP_Vehicle_Inventory2", 3);
            API.DecorRegister("PoliceMP_Vehicle_Inventory3", 3);
            API.DecorRegister("PoliceMP_Vehicle_Inventory4", 3);
            API.DecorRegister("PoliceMP_Ped_Inventory1", 3);
            API.DecorRegister("PoliceMP_Ped_Inventory2", 3);
            API.DecorRegister("PoliceMP_Ped_Inventory3", 3);
            API.DecorRegister("PoliceMP_Ped_Inventory4", 3);

            API.DecorRegister("PoliceMP_Ped_BeingSearched", 2);

            _ticks.On(CheckBeingSearched);
            _ticks.Off(OnBeingSearched); // Keep off pls as its acting as a void that I can await

            _commandManager.Register("cvbsc").WithHandler(OnVehicleBootContents);
            _commandManager.Register("cpbsc").WithHandler(OnPlayerBodyContents);
        }

        private async Task OnBeingSearched()
        {
            var player = Game.PlayerPed.Handle;
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Person Search", "success",
                $"You are currently being searched!"));
            API.FreezeEntityPosition(player, true);
            API.ExecuteCommand("e t");
            await Delay(4000);
            API.FreezeEntityPosition(player, false);
            API.ExecuteCommand("e c");
            await Delay(200);
        }

        private async Task CheckBeingSearched()
        {
            await Delay(1000);
            var player = Game.PlayerPed.Handle;
            var currentUserRole = _permissionService.CurrentUserRole;
            if (_permissionService.CurrentUserRole == null) return;
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Civ) return;


            var beingSearched = API.DecorGetBool(player, "PoliceMP_Ped_BeingSearched");
            if (!beingSearched) return;
            await OnBeingSearched();
            API.DecorSetBool(player, "PoliceMP_Ped_BeingSearched", false);
        }


        private async void OnVehicleBootContents()
        {
            var player = Game.PlayerPed.Handle;
            var playerPed = Game.PlayerPed;
            var currentUserRole = _permissionService.CurrentUserRole;
            var vehicle = API.GetVehiclePedIsIn(player, true);
            if (!API.IsPedInAnyVehicle(player, false)) vehicle = playerPed.LastVehicle.Handle;

            if (currentUserRole.Branch != UserBranch.Civ) return;
            var _userAces = await _permissionService.GetUserAces();

            MenuController.CloseAllMenus();

            var civVehicleSearchMenu = new Menu("Vehicle Contents", "Items found during search!");
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.AddMenu(civVehicleSearchMenu);

            await Delay(0);
            civVehicleSearchMenu.OpenMenu();

            var clearAll = new MenuItem("Clear All Items", "Clear vehicle contents")
            {
                LeftIcon = MenuItem.Icon.TICK
            };
            civVehicleSearchMenu.AddMenuItem(clearAll);
            var showAll = new MenuItem("Preview Current Items", "Preview vehicle contents")
            {
                LeftIcon = MenuItem.Icon.TICK
            };
            civVehicleSearchMenu.AddMenuItem(showAll);
            var saveItems = new MenuItem("Save Items")
            {
                LeftIcon = MenuItem.Icon.TICK
            };
            civVehicleSearchMenu.AddMenuItem(saveItems);

            foreach (var item in vehicleSearchPossibleItems)
            {
                var menuItem = new MenuCheckboxItem(item.Key, "Add to vehicle contents");
                civVehicleSearchMenu.AddMenuItem(menuItem);
            }

            foreach (var item in vehicleSearchPossibleItems2)
            {
                var menuItem = new MenuCheckboxItem(item.Key, "Add to vehicle contents");
                civVehicleSearchMenu.AddMenuItem(menuItem);
            }

            foreach (var item in vehicleSearchPossibleItems3)
            {
                var menuItem = new MenuCheckboxItem(item.Key, "Add to vehicle contents");
                civVehicleSearchMenu.AddMenuItem(menuItem);
            }

            if (_userAces.IsSeniorCiv)
            {
                foreach (var item in vehicleSearchPossibleItemsSenior)
                {
                    var menuItem = new MenuCheckboxItem(item.Key, "Add to vehicle contents");
                    civVehicleSearchMenu.AddMenuItem(menuItem);
                }
            }

            int alreadySelectedEncoded1 = API.DecorGetInt(vehicle, "PoliceMP_Vehicle_Inventory1");
            int alreadySelectedEncoded2 = API.DecorGetInt(vehicle, "PoliceMP_Vehicle_Inventory2");
            int alreadySelectedEncoded3 = API.DecorGetInt(vehicle, "PoliceMP_Vehicle_Inventory3");
            int alreadySelectedEncoded4 = API.DecorGetInt(vehicle, "PoliceMP_Vehicle_Inventory4");

            foreach (var menuItem in civVehicleSearchMenu.GetMenuItems())
            {
                if (menuItem is MenuCheckboxItem checkboxItem)
                {
                    if (vehicleSearchPossibleItems.TryGetValue(checkboxItem.Text, out int itemValue))
                    {
                        checkboxItem.Checked = (alreadySelectedEncoded1 & itemValue) != 0;
                    }
                    else if (vehicleSearchPossibleItems2.TryGetValue(checkboxItem.Text, out itemValue))
                    {
                        checkboxItem.Checked = (alreadySelectedEncoded2 & itemValue) != 0;
                    }
                    else if (vehicleSearchPossibleItems3.TryGetValue(checkboxItem.Text, out itemValue))
                    {
                        checkboxItem.Checked = (alreadySelectedEncoded4 & itemValue) != 0;
                    }
                    else if (vehicleSearchPossibleItemsSenior.TryGetValue(checkboxItem.Text, out itemValue))
                    {
                        checkboxItem.Checked = (alreadySelectedEncoded3 & itemValue) != 0;
                    }
                }
            }

            civVehicleSearchMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (menu != civVehicleSearchMenu) return;

                if (item == clearAll)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Contents Preview",
                        "success", $"Cleared all items!"));
                    MenuController.CloseAllMenus();
                    API.DecorSetInt(vehicle, "PoliceMP_Vehicle_Inventory1", 0);
                    API.DecorSetInt(vehicle, "PoliceMP_Vehicle_Inventory2", 0);
                    API.DecorSetInt(vehicle, "PoliceMP_Vehicle_Inventory3", 0);
                    API.DecorSetInt(vehicle, "PoliceMP_Vehicle_Inventory4", 0);

                    await Delay(250);
                    OnVehicleBootContents();
                    return;
                }

                if (item == showAll)
                {
                    var encodedValueSet1 = API.DecorGetInt(vehicle, "PoliceMP_Vehicle_Inventory1");
                    var encodedValueSet2 = API.DecorGetInt(vehicle, "PoliceMP_Vehicle_Inventory2");
                    var encodedValueSet3 = API.DecorGetInt(vehicle, "PoliceMP_Vehicle_Inventory3");
                    var encodedValueSet4 = API.DecorGetInt(vehicle, "PoliceMP_Vehicle_Inventory4");

                    List<string> decodedItems = new List<string>();

                    foreach (var vehicleItem in vehicleSearchPossibleItems)
                    {
                        if ((encodedValueSet1 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in vehicleSearchPossibleItems2)
                    {
                        if ((encodedValueSet2 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in vehicleSearchPossibleItems3)
                    {
                        if ((encodedValueSet4 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in vehicleSearchPossibleItemsSenior)
                    {
                        if ((encodedValueSet3 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    if (decodedItems.Count == 0)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Contents Preview",
                            "error", "No items have been selected."));
                        return;
                    }

                    var decodedString = String.Join(", ", decodedItems);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Contents Preview",
                        "success", $"Items: {decodedString}"));
                }

                if (item == saveItems)
                {
                    int encodedValueSet1 = 0;
                    int encodedValueSet2 = 0;
                    int encodedValueSet3 = 0;
                    int encodedValueSet4 = 0;

                    foreach (var menuItem in civVehicleSearchMenu.GetMenuItems())
                    {
                        if (menuItem is MenuCheckboxItem checkboxItem && checkboxItem.Checked)
                        {
                            if (vehicleSearchPossibleItems.TryGetValue(checkboxItem.Text, out int itemValue))
                            {
                                encodedValueSet1 |= itemValue;
                            }
                            else if (vehicleSearchPossibleItems2.TryGetValue(checkboxItem.Text, out itemValue))
                            {
                                encodedValueSet2 |= itemValue;
                            }
                            else if (vehicleSearchPossibleItems3.TryGetValue(checkboxItem.Text, out itemValue))
                            {
                                encodedValueSet4 |= itemValue;
                            }
                            else if (vehicleSearchPossibleItemsSenior.TryGetValue(checkboxItem.Text, out itemValue))
                            {
                                encodedValueSet3 |= itemValue;
                            }
                        }
                    }

                    if (vehicle != 0)
                    {
                        API.DecorSetInt(vehicle, "PoliceMP_Vehicle_Inventory1", encodedValueSet1);
                        API.DecorSetInt(vehicle, "PoliceMP_Vehicle_Inventory2", encodedValueSet2);
                        API.DecorSetInt(vehicle, "PoliceMP_Vehicle_Inventory3", encodedValueSet3);
                        API.DecorSetInt(vehicle, "PoliceMP_Vehicle_Inventory4", encodedValueSet4);
                    }

                    MenuController.CloseAllMenus();
                    await Delay(100);
                    civVehicleSearchMenu.OpenMenu();
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Contents Preview",
                        "success", $"Saved all items!"));
                }
            };
        }


        private async void OnPlayerBodyContents()
        {
            var currentUserRole = _permissionService.CurrentUserRole;
            var playerPed = Game.PlayerPed.Handle;

            if (currentUserRole.Branch != UserBranch.Civ) return;
            var _userAces = await _permissionService.GetUserAces();

            MenuController.CloseAllMenus();

            var civPersonSearchMenu = new Menu("Person Contents", "Items found during search!");
            MenuController.EnableMenuToggleKeyOnController = false;
            MenuController.MenuToggleKey = (Control)(-1);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
            MenuController.AddMenu(civPersonSearchMenu);

            await Delay(0);
            civPersonSearchMenu.OpenMenu();

            var clearAll = new MenuItem("Clear All Items", "Clear person contents")
            {
                LeftIcon = MenuItem.Icon.TICK
            };
            civPersonSearchMenu.AddMenuItem(clearAll);
            var showAll = new MenuItem("Preview Current Items", "Preview person contents")
            {
                LeftIcon = MenuItem.Icon.TICK
            };
            civPersonSearchMenu.AddMenuItem(showAll);
            var saveItems = new MenuItem("Save Items")
            {
                LeftIcon = MenuItem.Icon.TICK
            };
            civPersonSearchMenu.AddMenuItem(saveItems);

            foreach (var item in vehicleSearchPossibleItems)
            {
                var menuItem = new MenuCheckboxItem(item.Key, "Add to person contents");
                civPersonSearchMenu.AddMenuItem(menuItem);
            }

            foreach (var item in vehicleSearchPossibleItems2)
            {
                var menuItem = new MenuCheckboxItem(item.Key, "Add to person contents");
                civPersonSearchMenu.AddMenuItem(menuItem);
            }

            if (_userAces.IsSeniorCiv)
            {
                foreach (var item in vehicleSearchPossibleItemsSenior)
                {
                    var menuItem = new MenuCheckboxItem(item.Key, "Add to person contents");
                    civPersonSearchMenu.AddMenuItem(menuItem);
                }
            }

            int alreadySelectedEncoded1 = API.DecorGetInt(playerPed, "PoliceMP_Ped_Inventory1");
            int alreadySelectedEncoded2 = API.DecorGetInt(playerPed, "PoliceMP_Ped_Inventory2");
            int alreadySelectedEncoded3 = API.DecorGetInt(playerPed, "PoliceMP_Ped_Inventory3");
            int alreadySelectedEncoded4 = API.DecorGetInt(playerPed, "PoliceMP_Ped_Inventory4");

            foreach (var menuItem in civPersonSearchMenu.GetMenuItems())
            {
                if (menuItem is MenuCheckboxItem checkboxItem)
                {
                    if (vehicleSearchPossibleItems.TryGetValue(checkboxItem.Text, out int itemValue))
                    {
                        checkboxItem.Checked = (alreadySelectedEncoded1 & itemValue) != 0;
                    }
                    else if (vehicleSearchPossibleItems2.TryGetValue(checkboxItem.Text, out itemValue))
                    {
                        checkboxItem.Checked = (alreadySelectedEncoded2 & itemValue) != 0;
                    }
                    else if (vehicleSearchPossibleItems3.TryGetValue(checkboxItem.Text, out itemValue))
                    {
                        checkboxItem.Checked = (alreadySelectedEncoded4 & itemValue) != 0;
                    }
                    else if (vehicleSearchPossibleItemsSenior.TryGetValue(checkboxItem.Text, out itemValue))
                    {
                        checkboxItem.Checked = (alreadySelectedEncoded3 & itemValue) != 0;
                    }
                }
            }

            civPersonSearchMenu.OnItemSelect += async (menu, item, index) =>
            {
                if (menu != civPersonSearchMenu) return;

                if (item == clearAll)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Body Contents Preview",
                        "success", $"Cleared all items!"));
                    MenuController.CloseAllMenus();
                    API.DecorSetInt(playerPed, "PoliceMP_Ped_Inventory1", 0);
                    API.DecorSetInt(playerPed, "PoliceMP_Ped_Inventory2", 0);
                    API.DecorSetInt(playerPed, "PoliceMP_Ped_Inventory3", 0);
                    API.DecorSetInt(playerPed, "PoliceMP_Ped_Inventory4", 0);

                    await Delay(250);
                    OnPlayerBodyContents();
                    return;
                }

                if (item == showAll)
                {
                    var encodedValueSet1 = API.DecorGetInt(playerPed, "PoliceMP_Ped_Inventory1");
                    var encodedValueSet2 = API.DecorGetInt(playerPed, "PoliceMP_Ped_Inventory2");
                    var encodedValueSet3 = API.DecorGetInt(playerPed, "PoliceMP_Ped_Inventory3");
                    var encodedValueSet4 = API.DecorGetInt(playerPed, "PoliceMP_Ped_Inventory4");

                    List<string> decodedItems = new List<string>();

                    foreach (var vehicleItem in vehicleSearchPossibleItems)
                    {
                        if ((encodedValueSet1 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in vehicleSearchPossibleItems2)
                    {
                        if ((encodedValueSet2 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in vehicleSearchPossibleItems3)
                    {
                        if ((encodedValueSet4 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    foreach (var vehicleItem in vehicleSearchPossibleItemsSenior)
                    {
                        if ((encodedValueSet3 & vehicleItem.Value) != 0)
                        {
                            decodedItems.Add(vehicleItem.Key);
                        }
                    }

                    if (decodedItems.Count == 0)
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Body Contents Preview",
                            "error", "No items have been selected."));
                        return;
                    }

                    var decodedString = String.Join(", ", decodedItems);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Body Contents Preview",
                        "success", $"Items: {decodedString}"));
                }

                if (item == saveItems)
                {
                    int encodedValueSet1 = 0;
                    int encodedValueSet2 = 0;
                    int encodedValueSet3 = 0;
                    int encodedValueSet4 = 0;

                    foreach (var menuItem in civPersonSearchMenu.GetMenuItems())
                    {
                        if (menuItem is MenuCheckboxItem checkboxItem && checkboxItem.Checked)
                        {
                            if (vehicleSearchPossibleItems.TryGetValue(checkboxItem.Text, out int itemValue))
                            {
                                encodedValueSet1 |= itemValue;
                            }
                            else if (vehicleSearchPossibleItems2.TryGetValue(checkboxItem.Text, out itemValue))
                            {
                                encodedValueSet2 |= itemValue;
                            }
                            else if (vehicleSearchPossibleItems3.TryGetValue(checkboxItem.Text, out itemValue))
                            {
                                encodedValueSet4 |= itemValue;
                            }
                            else if (vehicleSearchPossibleItemsSenior.TryGetValue(checkboxItem.Text, out itemValue))
                            {
                                encodedValueSet3 |= itemValue;
                            }
                        }
                    }

                    if (playerPed != 0)
                    {
                        API.DecorSetInt(playerPed, "PoliceMP_Ped_Inventory1", encodedValueSet1);
                        API.DecorSetInt(playerPed, "PoliceMP_Ped_Inventory2", encodedValueSet2);
                        API.DecorSetInt(playerPed, "PoliceMP_Ped_Inventory3", encodedValueSet3);
                        API.DecorSetInt(playerPed, "PoliceMP_Ped_Inventory4", encodedValueSet4);
                    }

                    MenuController.CloseAllMenus();
                    await Delay(100);
                    civPersonSearchMenu.OpenMenu();
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Body Contents Preview",
                        "success", $"Saved all items!"));
                }
            };
        }
    }
}