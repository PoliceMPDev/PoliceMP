using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using Newtonsoft.Json;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.CharCustom
{
    public class CharCustomScript : Script, ICharCustom
    {
        private readonly ILogger<CharCustomScript> _logger;
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ICommandManager _commands;
        private readonly IPermissionService _permission;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private readonly IFeatureService _featureService;
        private readonly IExportsAccessor _exportsAccessor;

        private readonly string _kvp = "SAVED_OUTFITS_KVP_V2";
        private UserAces _userAccess;

        private List<PedOutfit> _loadedOutfits = new List<PedOutfit>();
        private List<Menu> _categoryMenus = new List<Menu>();

        public CharCustomScript(ILogger<CharCustomScript> logger, ITickManager ticks,
            ILegacyClientCommunicationsManager comms,
            ICommandManager commands, IPermissionService permission, INewNotificationOverlay newNotificationOverlay,
            IFeatureService featureService, IExportsAccessor exportsAccessor)
        {
            _logger = logger;
            _ticks = ticks;
            _comms = comms;
            _commands = commands;
            _permission = permission;
            _newNotificationOverlay = newNotificationOverlay;
            _featureService = featureService;
            _exportsAccessor = exportsAccessor;
        }

        protected override async Task OnStartAsync()
        {
            #region Outfit Menu

            _commands.Register("outfitsmenu").WithHandler(async () =>
            {
                MenuController.CloseAllMenus();

                var outfitsMenu = new Menu("Outfits Menu", "Edit your saved outfits");
                MenuController.EnableMenuToggleKeyOnController = false;
                MenuController.MenuToggleKey = (Control)(-1);
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                MenuController.AddMenu(outfitsMenu);

                await Delay(1);
                outfitsMenu.OpenMenu();

                var display = new MenuItem("Display Outfits", "Display all saved outfits");
                var save = new MenuItem("Save Outfit", "Save current outfit");
                var remove = new MenuItem("Remove Outfit", "Remove an outfit");
                var replace = new MenuItem("Replace Outfit", "Replace an outfit with current");
                var rename = new MenuItem("Rename Outfit", "Rename a saved outfit");
                var edit = new MenuItem("Edit Outfit", "Open custom menu (requires access)");

                outfitsMenu.AddMenuItem(display);
                outfitsMenu.AddMenuItem(save);
                outfitsMenu.AddMenuItem(remove);
                outfitsMenu.AddMenuItem(replace);
                outfitsMenu.AddMenuItem(rename);
                outfitsMenu.AddMenuItem(edit);

                outfitsMenu.OpenMenu();


                outfitsMenu.OnItemSelect += async (menu, item, index) =>
                {
                    if (menu != outfitsMenu) return;
                    if (item == display) API.ExecuteCommand("displayoutfits");
                    if (item == edit) API.ExecuteCommand("customchar");

                    if (item == save)
                    {
                        API.AddTextEntry("FMMC_KEY_TIP1", "Enter Outfit Name:");
                        API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "Cool Name", "", "", "", 30);

                        API.UpdateOnscreenKeyboard();

                        while (API.UpdateOnscreenKeyboard() == 0)
                        {
                            await Delay(100);
                            API.UpdateOnscreenKeyboard();
                        }

                        if (API.UpdateOnscreenKeyboard() == 1)
                        {
                            string outfitName = API.GetOnscreenKeyboardResult();
                            API.ExecuteCommand($"saveoutfit \"{outfitName}\"");
                        }
                    }

                    if (item == rename)
                    {
                        API.AddTextEntry("FMMC_KEY_TIP1", "Enter Index to Rename:");
                        API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "0", "", "", "", 30);

                        API.UpdateOnscreenKeyboard();

                        while (API.UpdateOnscreenKeyboard() == 0)
                        {
                            await Delay(100);
                            API.UpdateOnscreenKeyboard();
                        }

                        if (API.UpdateOnscreenKeyboard() == 1)
                        {
                            string renameIndexStr = API.GetOnscreenKeyboardResult();
                            int.TryParse(renameIndexStr, out int renameIndex);
                            API.ExecuteCommand($"renameoutfit {renameIndex}");
                        }
                    }

                    if (item == remove)
                    {
                        API.AddTextEntry("FMMC_KEY_TIP1", "Enter Index to Remove:");
                        API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "0", "", "", "", 10);

                        API.UpdateOnscreenKeyboard();

                        while (API.UpdateOnscreenKeyboard() == 0)
                        {
                            await Delay(100);
                            API.UpdateOnscreenKeyboard();
                        }

                        if (API.UpdateOnscreenKeyboard() == 1)
                        {
                            string removeIndexStr = API.GetOnscreenKeyboardResult();
                            int.TryParse(removeIndexStr, out int removeIndex);
                            API.ExecuteCommand($"removeoutfit {removeIndex}");
                        }
                    }

                    if (item == replace)
                    {
                        API.AddTextEntry("FMMC_KEY_TIP1", "Enter Index to Replace:");
                        API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP1", "", "0", "", "", "", 10);

                        API.UpdateOnscreenKeyboard();

                        while (API.UpdateOnscreenKeyboard() == 0)
                        {
                            await Delay(100);
                            API.UpdateOnscreenKeyboard();
                        }

                        if (API.UpdateOnscreenKeyboard() == 1)
                        {
                            string replaceIndexStr = API.GetOnscreenKeyboardResult();
                            int.TryParse(replaceIndexStr, out int replaceIndex);
                            API.ExecuteCommand($"replaceoutfit {replaceIndex}");
                        }
                    }

                    outfitsMenu.CloseMenu();
                };
            });

            #endregion Outfit Menu

            _comms.On("client:delete:kvp", new Action(() =>
            {
                API.DeleteResourceKvp("SAVED_OUTFITS_KVP_V2");

                // Clear in-memory list as well
                lock (_loadedOutfits)
                {
                    _loadedOutfits.Clear();
                }

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "success",
                    "Your saved outfits have been cleared by SM/DEV.", new NewNotificationMessageContent[0]));
            }));

            _comms.On<string>("client:notify:kvpSuccess",
                (message) =>
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "success", message,
                        new NewNotificationMessageContent[0]));
                });

            _comms.On<string>("client:notify:kvpError",
                (message) =>
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error", message,
                        new NewNotificationMessageContent[0]));
                });

            _commands.Register("customchar").WithHandler(async () =>
            {
                _userAccess = _permission.GetUserAces().Result;
                if (!_userAccess.IsDeveloper && !_userAccess.IsCivTrained && !_userAccess.IsProDonator &&
                    !_userAccess.IsBandFour) return;
                var currentUserRole = _permission.CurrentUserRole;
                if (!_userAccess.IsDeveloper && !_userAccess.IsProDonator && !_userAccess.IsBandFour &&
                    currentUserRole.Branch != UserBranch.Civ) return;

                _logger.Debug("Opening custom char menu.");

                dynamic config = new ExpandoObject();
                config.ped = false;
                config.headBlend = true;
                config.faceFeatures = true;
                config.headOverlays = true;
                config.components = false;
                config.props = false;
                config.allowExit = true;
                config.tattoos = true;

                if (_userAccess.IsCivTrained && currentUserRole.Branch == UserBranch.Civ)
                {
                    config.ped = true;
                    config.components = false;
                    config.props = false;
                }

                if (_userAccess.IsSeniorCiv && currentUserRole.Branch == UserBranch.Civ || _userAccess.IsBandFour)
                {
                    if (_featureService.IsFeatureEnabled(FeatureToggle.AllowCustomCharClothing))
                    {
                        config.ped = true;
                        config.headBlend = true;
                        config.faceFeatures = true;
                        config.headOverlays = true;
                        config.components = true;
                        config.props = true;
                    }
                    else
                    {
                        config.ped = true;
                        config.headBlend = true;
                        config.faceFeatures = true;
                        config.headOverlays = true;
                        config.components = false;
                        config.props = false;
                    }
                }

                if (_userAccess.IsAdmin || _userAccess.IsDeveloper)
                {
                    config.ped = true;
                    config.headBlend = true;
                    config.faceFeatures = true;
                    config.headOverlays = true;
                    config.components = true;
                    config.props = true;
                }

                _exportsAccessor.Exports["fivem-appearance"]
                    .startPlayerCustomization(new Action<object>(PlayerAppearance), config);
            });

            _commands.Register("replaceoutfit").WithHandler(async (args) =>
            {
                var aces = await _permission.GetUserAces();
                if (!aces.IsDeveloper && !aces.IsProDonator && !aces.IsCivTrained && !aces.IsBandFour) return;

                int index;
                if (!int.TryParse(args, out index) || index < 0)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "Please provide a valid outfit index to replace. The index is next to the name, e.g '[6]'",
                        new NewNotificationMessageContent[0]));
                    return;
                }

                var outfits = await FetchAllOutfits();

                if (index >= outfits.Count)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "Please provide a valid outfit index to replace. The index is next to the name, e.g '[6]'",
                        new NewNotificationMessageContent[0]));
                    return;
                }

                var replacedOutfit = outfits[index];
                outfits.RemoveAt(index);
                var name = replacedOutfit.Name;

                await SaveCurrentSelectionAtIndex(name, index);
                await SaveToCache();

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "success",
                    $"You've replaced your '{replacedOutfit.Name}' outfit.", new NewNotificationMessageContent[0]));
            });


            _commands.Register("saveoutfit").WithHandler(async (args) =>
            {
                var aces = await _permission.GetUserAces();
                if (!aces.IsDeveloper && !aces.IsProDonator && !aces.IsCivTrained && !aces.IsBandFour) return;

                var name = args;
                API.AddTextEntry("FMMC_KEY_TIP2", "Enter Outfit Category:");
                API.DisplayOnscreenKeyboard(0, "FMMC_KEY_TIP2", "", "Default", "", "", "", 30);

                while (API.UpdateOnscreenKeyboard() == 0)
                {
                    await Delay(100);
                }

                string category = "Default";
                if (API.UpdateOnscreenKeyboard() == 1)
                {
                    category = API.GetOnscreenKeyboardResult();
                }

                if (name.Length <= 0)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "Your outfit must be saved with a name!", new NewNotificationMessageContent[0]));
                    return;
                }

                await SaveCurrentSelection(name, category);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "success",
                    $"Outfit '{name}' has been saved!", new NewNotificationMessageContent[0]));
            });

            _commands.Register("loadoutfit").WithHandler(async (args) =>
            {
                var aces = await _permission.GetUserAces();
                if (!aces.IsDeveloper && !aces.IsProDonator && !aces.IsCivTrained && !aces.IsBandFour) return;

                string name = args;
                int index;
                Int32.TryParse(name, out index);
                if (name.Length <= 0 || index < 0)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "Please load a valid number!", new NewNotificationMessageContent[0]));
                    return;
                }

                await LoadByIndex(index);
            });

            _commands.Register("removeoutfit").WithHandler(async (args) =>
            {
                var aces = await _permission.GetUserAces();
                if (!aces.IsDeveloper && !aces.IsProDonator && !aces.IsCivTrained && !aces.IsBandFour) return;

                int index;
                if (!int.TryParse(args, out index) || index < 0)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "Please provide a valid outfit index to remove. The index is next to the name, e.g '[6]'",
                        new NewNotificationMessageContent[0]));
                    return;
                }

                var outfits = await FetchAllOutfits();

                if (index >= outfits.Count)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "Please provide a valid outfit index to remove. The index is next to the name, e.g '[6]'",
                        new NewNotificationMessageContent[0]));
                    return;
                }

                var removedOutfit = outfits[index];
                outfits.RemoveAt(index);

                await SaveToCache();

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "success",
                    $"You've removed your '{removedOutfit.Name}' outfit.", new NewNotificationMessageContent[0]));
            });

            _commands.Register("displayoutfits").WithHandler(async () =>
            {
                var aces = await _permission.GetUserAces();
                if (!aces.IsDeveloper && !aces.IsProDonator && !aces.IsCivTrained && !aces.IsBandFour) return;

                var alloutfits = await FetchAllOutfits();

                var playerPed = Game.PlayerPed.Model;
                var isMale = playerPed == (uint)PedHash.FreemodeMale01;

                var outfits = new List<PedOutfit>();
                foreach (var outfit in alloutfits)
                {
                    if (isMale && outfit.MaleOutfit) outfits.Add(outfit);
                    if (!isMale && !outfit.MaleOutfit) outfits.Add(outfit);
                }

                if (!outfits.Any())
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "You have no saved outfits!", new NewNotificationMessageContent[0]));
                    return;
                }

                var outfitMenu = new Menu("Outfits");
                MenuController.EnableMenuToggleKeyOnController = false;
                MenuController.MenuToggleKey = (Control)(-1);
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                MenuController.AddMenu(outfitMenu);

                await Delay(0);
                outfitMenu.OpenMenu();

                var grouped =
                    outfits.GroupBy(o => string.IsNullOrWhiteSpace(o.Category) ? "Uncategorised" : o.Category);

                _categoryMenus.Clear(); 

                foreach (var categoryGroup in grouped)
                {
                    var categoryMenu = new Menu(categoryGroup.Key, $"Outfits in '{categoryGroup.Key}'");
                    _categoryMenus.Add(categoryMenu);

                    MenuController.AddSubmenu(outfitMenu, categoryMenu);

                    foreach (var outfit in categoryGroup)
                    {
                        var position = alloutfits.IndexOf(outfit);
                        categoryMenu.AddMenuItem(new MenuItem($"[{position}] {outfit.Name}")
                        {
                            ItemData = outfit
                        });
                    }

                    var mainMenuItem = new MenuItem($"{categoryGroup.Key} ({categoryGroup.Count()})");
                    outfitMenu.AddMenuItem(mainMenuItem);
                    MenuController.BindMenuItem(outfitMenu, categoryMenu, mainMenuItem);

                    categoryMenu.OnItemSelect += (menu, item, index) =>
                    {
                        var pedOutfit = (PedOutfit)item.ItemData;
                        LoadOutfit(pedOutfit);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "success",
                            $"You've loaded your '{pedOutfit.Name}' Outfit", new NewNotificationMessageContent[0]));
                        menu.CloseMenu();
                    };
                }
            });

            _commands.Register("renameoutfit").WithHandler(async (args) =>
            {
                var aces = await _permission.GetUserAces();
                if (!aces.IsDeveloper && !aces.IsProDonator && !aces.IsCivTrained && !aces.IsBandFour) return;

                if (!int.TryParse(args, out int index))
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "Please provide a valid outfit index. You can view your outfits index in the displayoutfits menu!",
                        new NewNotificationMessageContent[0]));
                    return;
                }

                API.AddTextEntry("OutfitKeyboardTitleText", "INSERT NEW OUTFIT NAME");
                API.DisplayOnscreenKeyboard(0, "OutfitKeyboardTitleText", "", "", "", "", "", 30);

                API.UpdateOnscreenKeyboard();

                while (API.UpdateOnscreenKeyboard() == 0)
                {
                    await Delay(10);
                    API.UpdateOnscreenKeyboard();
                }

                if (API.UpdateOnscreenKeyboard() != 1)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "Error saving your input, please try again!", new NewNotificationMessageContent[0]));
                    return;
                }

                string newName = API.GetOnscreenKeyboardResult();

                lock (_loadedOutfits)
                {
                    if (index >= 0 && index < _loadedOutfits.Count)
                    {
                        _loadedOutfits[index].Name = newName;
                        SaveToCache();
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "success",
                            $"Outfit '{index}' has been renamed to '{newName}'!",
                            new NewNotificationMessageContent[0]));
                    }
                    else
                    {
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                            $"No outfit found at index '{index}'. You can view your outfits index in the displayoutfits menu!",
                            new NewNotificationMessageContent[0]));
                    }
                }
            });

            _commands.Register("removecategory").WithHandler(async (args) =>
            {
                var aces = await _permission.GetUserAces();
                if (!aces.IsDeveloper && !aces.IsProDonator && !aces.IsCivTrained && !aces.IsBandFour) return;

                string categoryToRemove = args.Trim();
                if (string.IsNullOrEmpty(categoryToRemove))
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        "Please enter a category name to remove.", new NewNotificationMessageContent[0]));
                    return;
                }

                await LoadFromCache();

                int beforeCount = _loadedOutfits.Count;

                lock (_loadedOutfits)
                {
                    _loadedOutfits.RemoveAll(o =>
                        string.Equals(o.Category, categoryToRemove, StringComparison.OrdinalIgnoreCase));
                }

                int afterCount = _loadedOutfits.Count;
                int removed = beforeCount - afterCount;

                await SaveToCache();

                if (removed > 0)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "success",
                        $"Removed {removed} outfits from category '{categoryToRemove}'.",
                        new NewNotificationMessageContent[0]));
                }
                else
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                        $"No outfits found in category '{categoryToRemove}'.", new NewNotificationMessageContent[0]));
                }
            });

            _commands.Register("clearoutfits").WithHandler(async () =>
            {
                var aces = await _permission.GetUserAces();
                if (!aces.IsDeveloper && !aces.IsProDonator && !aces.IsCivTrained && !aces.IsBandFour) return;
                ClearAllOutfits();
            });
        }

        private void PlayerAppearance(object appearance)
        {
            Game.PlayerPed.GiveDefaultEquipment(_permission.CurrentUserRole, _userAccess);
        }

        //Save to cache
        private async Task SaveToCache()
        {
            lock (_loadedOutfits)
            {
                var str = JsonConvert.SerializeObject(_loadedOutfits);
                API.SetResourceKvp(_kvp, str);
                _logger.Debug($"{_loadedOutfits.Count} outfits saved to cache.");
            }
        }

        //Load from cache
        private async Task LoadFromCache()
        {
            lock (_loadedOutfits)
            {
                var str = API.GetResourceKvpString(_kvp);

                if (str == null)
                {
                    _logger.Debug("There are no saved outfits!");
                    return;
                }

                _logger.Debug($"JSON: {str}");
                _loadedOutfits = JsonConvert.DeserializeObject<List<PedOutfit>>(str);
                _logger.Debug($"{_loadedOutfits.Count} outfits loaded from cache.");
            }
        }

        //Save current selection
        private async Task SaveCurrentSelection(string name, string category)

        {
            await FetchAllOutfits();

            var playerPed = Game.PlayerPed.Model;
            bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

            PedOutfit outfit = new PedOutfit()
            {
                Name = name,
                Category = category,
                AceGroupsRequired = null,
                MaleOutfit = isMale,
                HEAD = GetCombination(0),
                BERD = GetCombination(1),
                //HAIR = GetCombination(2), Removed Hair from saving due to it saving with "HatHair"
                UPPR = GetCombination(3),
                LOWR = GetCombination(4),
                HAND = GetCombination(5),
                FEET = GetCombination(6),
                TEEF = GetCombination(7),
                ACCS = GetCombination(8),
                TASK = GetCombination(9),
                DECL = GetCombination(10),
                JBIB = GetCombination(11),
                headProp = GetPropCombination(0),
                EYES = GetPropCombination(1),
                EARS = GetPropCombination(2),
                MOUTH = GetPropCombination(3),
                LEFT_HAND = null,
                RIGHT_HAND = null,
                LEFT_WRIST = null,
                RIGHT_WRIST = null,
                HIP = null,
                LEFT_FOOT = null,
                RIGHT_FOOT = null,
                UNK_604819740 = null,
                UNK_2358626934 = null,
            };
            lock (_loadedOutfits)
            {
                _loadedOutfits.Add(outfit);
            }

            await SaveToCache();
        }

        #region Save Outfit at Specific Index

        private async Task SaveCurrentSelectionAtIndex(string name, int index)
        {
            var existingOutfits = await FetchAllOutfits();
            var existingOutfit = existingOutfits.ElementAtOrDefault(index);

            if (existingOutfit == null)
                return; // Or show a notification if needed

            var playerPed = Game.PlayerPed.Model;
            bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

            PedOutfit outfit = new PedOutfit()
            {
                Name = name,
                AceGroupsRequired = null,
                MaleOutfit = isMale,
                Category = existingOutfit.Category ?? "Default",
                HEAD = GetCombination(0),
                BERD = GetCombination(1),
                UPPR = GetCombination(3),
                LOWR = GetCombination(4),
                HAND = GetCombination(5),
                FEET = GetCombination(6),
                TEEF = GetCombination(7),
                ACCS = GetCombination(8),
                TASK = GetCombination(9),
                DECL = GetCombination(10),
                JBIB = GetCombination(11),
                headProp = GetPropCombination(0),
                EYES = GetPropCombination(1),
                EARS = GetPropCombination(2),
                MOUTH = GetPropCombination(3),
            };

            lock (_loadedOutfits)
            {
                _loadedOutfits[index] = outfit;
            }

            await SaveToCache();
        }

        #endregion

        public Component GetCombination(int id)
        {
            var ped = Game.PlayerPed.Handle;
            Component component = new Component()
            {
                DrawableID = API.GetPedDrawableVariation(ped, id),
                TextureID = API.GetPedTextureVariation(ped, id),
                PaletteID = API.GetPedPaletteVariation(ped, id)
            };
            return component;
        }

        public Component GetPropCombination(int id)
        {
            var ped = Game.PlayerPed.Handle;
            Component component = new Component()
            {
                DrawableID = API.GetPedPropIndex(ped, id),
                TextureID = API.GetPedPropTextureIndex(ped, id),
                PaletteID = 0
            };
            return component;
        }

        //Load by index
        private async Task LoadByIndex(int index)
        {
            await LoadFromCache();

            if (_loadedOutfits.Count <= 0)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                    "You currently have no saved outfits.", new NewNotificationMessageContent[0]));
                return;
            }

            lock (_loadedOutfits)
            {
                int c = -1;
                foreach (var outfit in _loadedOutfits)
                {
                    c++;
                    if (index != c) continue;
                    LoadOutfit(outfit);
                    return;
                }

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Outfits", "error",
                    "No outfit exists with the ID provided.", new NewNotificationMessageContent[0]));
            }
        }

        public void LoadOutfit(PedOutfit outfit)
        {
            int handle = Game.PlayerPed.Handle;

            //Set component variations
            API.SetPedComponentVariation(handle, 0, outfit.HEAD.DrawableID, outfit.HEAD.TextureID,
                outfit.HEAD.PaletteID);
            API.SetPedComponentVariation(handle, 1, outfit.BERD.DrawableID, outfit.BERD.TextureID,
                outfit.BERD.PaletteID);
            //API.SetPedComponentVariation(handle, 2, outfit.HAIR.DrawableID, outfit.HAIR.TextureID, outfit.HAIR.PaletteID);
            //_logger.Debug($"{outfit.HAIR.DrawableID}, {outfit.HAIR.TextureID}, {outfit.HAIR.PaletteID}");
            API.SetPedComponentVariation(handle, 3, outfit.UPPR.DrawableID, outfit.UPPR.TextureID,
                outfit.UPPR.PaletteID);
            API.SetPedComponentVariation(handle, 4, outfit.LOWR.DrawableID, outfit.LOWR.TextureID,
                outfit.LOWR.PaletteID);
            API.SetPedComponentVariation(handle, 5, outfit.HAND.DrawableID, outfit.HAND.TextureID,
                outfit.HAND.PaletteID);
            API.SetPedComponentVariation(handle, 6, outfit.FEET.DrawableID, outfit.FEET.TextureID,
                outfit.FEET.PaletteID);
            API.SetPedComponentVariation(handle, 7, outfit.TEEF.DrawableID, outfit.TEEF.TextureID,
                outfit.TEEF.PaletteID);
            API.SetPedComponentVariation(handle, 8, outfit.ACCS.DrawableID, outfit.ACCS.TextureID,
                outfit.ACCS.PaletteID);
            API.SetPedComponentVariation(handle, 9, outfit.TASK.DrawableID, outfit.TASK.TextureID,
                outfit.TASK.PaletteID);
            API.SetPedComponentVariation(handle, 10, outfit.DECL.DrawableID, outfit.DECL.TextureID,
                outfit.DECL.PaletteID);
            API.SetPedComponentVariation(handle, 11, outfit.JBIB.DrawableID, outfit.JBIB.TextureID,
                outfit.JBIB.PaletteID);

            //Set prop variaitons
            API.SetPedPropIndex(handle, 0, outfit.headProp.DrawableID, outfit.headProp.TextureID, true);
            API.SetPedPropIndex(handle, 1, outfit.EYES.DrawableID, outfit.EYES.TextureID, true);
            API.SetPedPropIndex(handle, 2, outfit.EARS.DrawableID, outfit.EARS.TextureID, true);
            API.SetPedPropIndex(handle, 3, outfit.MOUTH.DrawableID, outfit.MOUTH.TextureID, true);
        }

        //Display all saves
        private async Task DisplayAllSaves()
        {
            await LoadFromCache();
            lock (_loadedOutfits)
            {
                if (_loadedOutfits.Count == 0)
                {
                    _logger.Debug("No outfits have been saved.");
                    return;
                }

                var c = 0;
                foreach (var outfit in _loadedOutfits)
                {
                    _logger.Debug($"{outfit.Name} {c}");
                    c++;
                }
            }
        }

        private async Task<List<PedOutfit>> FetchAllOutfits()
        {
            await LoadFromCache();

            lock (_loadedOutfits)
            {
                return _loadedOutfits;
            }
        }

        private async Task ClearAllOutfits()
        {
            lock (_loadedOutfits)
            {
                _loadedOutfits.Clear();
            }

            await SaveToCache();
            await LoadFromCache();
        }
    }
}