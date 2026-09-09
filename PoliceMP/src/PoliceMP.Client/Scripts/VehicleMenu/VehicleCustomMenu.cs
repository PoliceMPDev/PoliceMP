using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using MenuAPI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;


namespace PoliceMP.Client.Scripts.VehicleMenu
{
    public class VehicleCustomMenu : Script
    {
        private readonly INotificationService _notification;
        private readonly IPermissionService _permission;
        private readonly ICommandManager _command;
        private readonly ILogger<VehicleCustomMenu> _logger;
        private UserAces _userAces;

        public VehicleCustomMenu(ICommandManager command, INotificationService notification, IPermissionService permission, ILogger<VehicleCustomMenu> logger)
        {
            _command = command;
            _notification = notification;
            _permission = permission;
            _logger = logger;
        }
        
        protected override async Task OnStartAsync()
        {
            _userAces = await _permission.GetUserAces();
            
            _command.Register("vehicleCustom").WithHandler( () =>
            {
                if (_permission.CurrentUserRole.Branch == UserBranch.Civ)
                {
                    MenuCreate();
                    return;
                }

                if (!_userAces.IsDeveloper) return;
                MenuCreate();
            });
            
            _command.Register("extras").WithHandler(ExtraMenuCreate);
        }
        private void MenuCreate()
        {
            #region Creation Entry
            var menu = new Menu("Vehicle Mod Shop");
            MenuController.AddMenu(menu);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            if (Game.PlayerPed.CurrentVehicle == null)
            {
                _notification.Error("Vehicle Mods", "You must be in a vehicle to use this.");
                return;
            }

            if (Game.PlayerPed.CurrentVehicle.ClassType == VehicleClass.Emergency)
            {
                _notification.Error("Vehicle Mods","You cannot use this in emergency vehicles.");
            }

            var vehId = Game.PlayerPed.CurrentVehicle.Handle;
            

            #endregion

            #region Spoilers
            var spoilerListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 0); i++)
            {
                spoilerListItems.Add(i.ToString());
            }
            var menuItemSpoiler = new MenuListItem("Spoilers", spoilerListItems, 0);
            
            menu.AddMenuItem(menuItemSpoiler);
            #endregion
            
            #region Front Bumper
            var fbListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 1); i++)
            {
                fbListItems.Add(i.ToString());
            }
            var menuItemFb = new MenuListItem("Front Bumpers", fbListItems, 0);
            menu.AddMenuItem(menuItemFb);
            #endregion
            
            #region Rear Bumper
            var rbListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 2); i++)
            {
                rbListItems.Add(i.ToString());
            }
            var menuItemRb = new MenuListItem("Rear Bumpers", rbListItems, 0);
            menu.AddMenuItem(menuItemRb);
            #endregion
            
            #region Side Skirt
            var sSListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 3); i++)
            {
                sSListItems.Add(i.ToString());
            }
            var menuItemSs = new MenuListItem("Side Skirts", sSListItems, 0);
            menu.AddMenuItem(menuItemSs);
            #endregion
            
            #region Exhaust
            var exhaustListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 4); i++)
            {
                exhaustListItems.Add(i.ToString());
            }
            var menuItemEx = new MenuListItem("Exhausts", exhaustListItems, 0);
            menu.AddMenuItem(menuItemEx);
            #endregion
            
            #region Frame
            var frameListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 5); i++)
            {
                frameListItems.Add(i.ToString());
            }
            var menuItemFrame = new MenuListItem("Frames", frameListItems, 0);
            menu.AddMenuItem(menuItemFrame);
            #endregion
            
            #region Grille
            var grilleListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 6); i++)
            {
                grilleListItems.Add(i.ToString());
            }
            var menuItemGrille = new MenuListItem("Grilles", grilleListItems, 0);
            menu.AddMenuItem(menuItemGrille);
            #endregion
            
            #region Hood
            var hoodListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 7); i++)
            {
                hoodListItems.Add(i.ToString());
            }
            var menuItemHood = new MenuListItem("Hoods", hoodListItems, 0);
            menu.AddMenuItem(menuItemHood);
            #endregion
            
            #region Fender
            var fenderListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 7); i++)
            {
                fenderListItems.Add(i.ToString());
            }
            var menuItemFender = new MenuListItem("Fenders", fenderListItems, 0);
            menu.AddMenuItem(menuItemFender);
            #endregion
            
            #region Roof
            var roofListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 10); i++)
            {
                roofListItems.Add(i.ToString());
            }
            var menuItemRoof = new MenuListItem("Roofs", roofListItems, 0);
            menu.AddMenuItem(menuItemRoof);
            #endregion
            
            #region Engine
            var engineListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 11); i++)
            {
                engineListItems.Add(i.ToString());
            }
            var menuItemEngine = new MenuListItem("Engines", engineListItems, 0);
            menu.AddMenuItem(menuItemEngine);
            #endregion
            
            #region Brakes
            var brakeListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 12); i++)
            {
                brakeListItems.Add(i.ToString());
            }
            var menuItemBrake = new MenuListItem("Brakes", brakeListItems, 0);
            menu.AddMenuItem(menuItemBrake);
            #endregion
            
            #region Transmission
            var transListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 13); i++)
            {
                transListItems.Add(i.ToString());
            }
            var menuItemTrans = new MenuListItem("Transmission", transListItems, 0);
            menu.AddMenuItem(menuItemTrans);
            #endregion
            
            #region Horns
            var hornListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 14); i++)
            {
                hornListItems.Add(i.ToString());
            }
            var menuItemHorn = new MenuListItem("Horns", hornListItems, 0);
            menu.AddMenuItem(menuItemHorn);
            #endregion
            
            #region Suspension
            var susListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 15); i++)
            {
                susListItems.Add(i.ToString());
            }
            var menuItemSus = new MenuListItem("Suspension", susListItems, 0);
            menu.AddMenuItem(menuItemSus);
            #endregion
            
            #region Armour
            var armListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 16); i++)
            {
                armListItems.Add(i.ToString());
            }
            var menuItemArm = new MenuListItem("Armour", armListItems, 0);
            menu.AddMenuItem(menuItemArm);
            #endregion
            
            #region Front Wheels
            var fwListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 23); i++)
            {
                fwListItems.Add(i.ToString());
            }
            var menuItemFw = new MenuListItem("Front Wheels", fwListItems, 0);
            menu.AddMenuItem(menuItemFw);
            #endregion
            
            #region Rear Wheels
            var rwListItems = new List<string>();
            for (var i = 0; i <= API.GetNumVehicleMods(vehId, 24); i++)
            {
                rwListItems.Add(i.ToString());
            }
            var menuItemRw = new MenuListItem("Rear Wheels", rwListItems, 0);
            menu.AddMenuItem(menuItemRw);
            #endregion
            
            //Handle Changes
            menu.OnListIndexChange += async (sender, item, oldIndex, newIndex, itemIndex) =>
            {
                if (Game.PlayerPed.CurrentVehicle == null)
                {
                    menu.CloseMenu();
                    return;
                }
                
                if (item == menuItemSpoiler)
                {
                    var selectedItem = int.Parse(menuItemSpoiler.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 0, selectedItem, false);
                }
                else if (item == menuItemFb)
                {
                    var selectedItem = int.Parse(menuItemFb.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 1, selectedItem, false);
                }
                else if (item == menuItemRb)
                {
                    var selectedItem = int.Parse(menuItemRb.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 2, selectedItem, false);
                }
                else if (item == menuItemSs)
                {
                    var selectedItem = int.Parse(menuItemSs.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 3, selectedItem, false);
                }
                else if (item == menuItemEx)
                {
                    var selectedItem = int.Parse(menuItemEx.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 4, selectedItem, false);
                }
                else if (item == menuItemFrame)
                {
                    var selectedItem = int.Parse(menuItemFrame.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 5, selectedItem, false);
                }
                else if (item == menuItemGrille)
                {
                    var selectedItem = int.Parse(menuItemGrille.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 6, selectedItem, false);
                }
                else if (item == menuItemHood)
                {
                    var selectedItem = int.Parse(menuItemHood.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 7, selectedItem, false);
                }
                else if (item == menuItemFender)
                {
                    var selectedItem = int.Parse(menuItemFender.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 8, selectedItem, false);
                }
                else if (item == menuItemRoof)
                {
                    var selectedItem = int.Parse(menuItemRoof.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 10, selectedItem, false);
                }
                else if (item == menuItemEngine)
                {
                    var selectedItem = int.Parse(menuItemEngine.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 11, selectedItem, false);
                }
                else if (item == menuItemBrake)
                {
                    var selectedItem = int.Parse(menuItemBrake.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 12, selectedItem, false);
                }
                else if (item == menuItemTrans)
                {
                    var selectedItem = int.Parse(menuItemTrans.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 13, selectedItem, false);
                }
                else if (item == menuItemHorn)
                {
                    var selectedItem = int.Parse(menuItemHorn.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 14, selectedItem, false);
                }
                else if (item == menuItemSus)
                {
                    var selectedItem = int.Parse(menuItemSus.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 15, selectedItem, false);
                }
                else if (item == menuItemArm)
                {
                    var selectedItem = int.Parse(menuItemArm.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 16, selectedItem, false);
                }
                else if (item == menuItemFw)
                {
                    var selectedItem = int.Parse(menuItemFw.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 23, selectedItem, false);
                }
                else if (item == menuItemRw)
                {
                    var selectedItem = int.Parse(menuItemRw.GetCurrentSelection());
                    API.SetVehicleModKit(vehId, 0);
                    API.SetVehicleMod(vehId, 24, selectedItem, false);
                }
            };
            
            
            menu.OpenMenu();
        }

        private void ExtraMenuCreate()
        {
            #region Creation Entry
            var menu = new Menu("Vehicle Extras");
            MenuController.AddMenu(menu);
            MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;

            if (Game.PlayerPed.CurrentVehicle == null)
            {
                _notification.Error("Vehicle Extras", "You must be in a vehicle to use this.");
                return;
            }
            var vehId = Game.PlayerPed.CurrentVehicle.Handle;
            #endregion

            for (var i = 0; i < 100; i++)
            {
                if(!API.DoesExtraExist(vehId, i)) continue;
                menu.AddMenuItem(API.IsVehicleExtraTurnedOn(vehId, i)
                    ? new MenuCheckboxItem(i.ToString(), $"Toggle extra {i}", false)
                    : new MenuCheckboxItem(i.ToString(), $"Toggle extra {i}", true));
            }

            menu.OnCheckboxChange += (menu1, item, index, state) =>
            {
                API.SetVehicleExtra(vehId, int.Parse(item.Text), state);
            };
            menu.OpenMenu();
        }
    }
}