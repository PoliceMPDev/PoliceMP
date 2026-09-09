Config = Config or {}
local configFile = LoadResourceFile(GetCurrentResourceName(), '/config.lua')
if configFile then
    load(configFile)()
else
    print("Config file not found.")
end

_menuPool = NativeUI.CreatePool()
mainMenu = NativeUI.CreateMenu(Config.MenuTitle, Config.MenuSubtitle, Config.MenuPositionX, Config.MenuPositionY)
_menuPool:Add(mainMenu)
local raw = LoadResourceFile(GetCurrentResourceName(), 'weapons.json')
local data = json.decode(raw)
local SubMenus = {}; Items = {}

SubMenus[mainMenu] = mainMenu

function CreateWeaponMenu(menu)
    -- Step 1: Build a sortable list of weapons
    local weaponList = {}
    for WeaponName, WeaponData in pairs(data) do
        table.insert(weaponList, {
            key = WeaponName,
            label = WeaponData.label or WeaponName,
            data = WeaponData
        })
    end

    -- Step 2: Sort alphabetically by label
    table.sort(weaponList, function(a, b)
        return a.label:lower() < b.label:lower()
    end)

    -- Step 3: Loop sorted list and build menu
    for _, weapon in ipairs(weaponList) do
        local WeaponName = weapon.key
        local WeaponData = weapon.data

        -- Handle categories
        local submenu_cat
        if WeaponData.category and not SubMenus[WeaponData.category] then
            SubMenus[WeaponData.category] = _menuPool:AddSubMenu(menu, WeaponData.category, "", Config.MenuPositionX, Config.MenuPositionY)
            submenu_cat = SubMenus[WeaponData.category]
        elseif WeaponData.category then
            submenu_cat = SubMenus[WeaponData.category]
        else
            submenu_cat = menu
        end

        -- Weapon invalid
        if not IsWeaponValid(GetHashKey(WeaponName)) then
            local Unavailable = NativeUI.CreateItem("~m~" .. WeaponData.label, "Weapon unavailable")
            Unavailable:SetRightBadge(BadgeStyle.Lock)
            submenu_cat:AddItem(Unavailable)
            table.insert(Items, {Unavailable, WeaponName, false})
        else
            -- Create sub menu for weapon
            if not SubMenus[WeaponName] then
                SubMenus[WeaponName] = _menuPool:AddSubMenu(submenu_cat, WeaponData.label, "", Config.MenuPositionX, Config.MenuPositionY)
            end

            -- Equip/Remove
            local Spawn = NativeUI.CreateItem("~r~Equip/Remove " .. WeaponData.label, "Add or remove this weapon to/from your inventory.")
            Spawn:SetLeftBadge(BadgeStyle.Gun)
            SubMenus[WeaponName]:AddItem(Spawn)
            table.insert(Items, {Spawn, WeaponName})

            -- Refill
            local Refill = NativeUI.CreateItem("Refill ammo", "Get max ammo for this weapon.")
            Refill:SetLeftBadge(BadgeStyle.Ammo)
            SubMenus[WeaponName]:AddItem(Refill)
            table.insert(Items, {Refill, WeaponName, "refill"})

            -- Attachments
            for attachments_key, attachments_value in pairs(WeaponData.attachments) do
                if attachments_value and #attachments_value > 0 then
                    SubMenus[WeaponName .. "_" .. attachments_key] = _menuPool:AddSubMenu(SubMenus[WeaponName], attachments_key, "", Config.MenuPositionX, Config.MenuPositionY)

                    for _, attach_item in ipairs(attachments_value) do
                        local attach = NativeUI.CreateItem(attach_item.label, "Add or remove attachment to/from your weapon.")
                        SubMenus[WeaponName .. "_" .. attachments_key]:AddItem(attach)
                        table.insert(Items, {attach, WeaponName, attach_item.value})
                    end
                end
            end
        end
    end

    -- Selection handler
    for _, SubMenu in pairs(SubMenus) do
        SubMenu.OnItemSelect = function(Sender, Item, Index)
            for _, Value in pairs(Items) do
                if Item == Value[1] then
                    if Value[3] ~= nil then
                        if Value[3] == false then
                            ShowNotification("No such weapon exists")
                        elseif Value[3] == "refill" then
                            local _, ammo = GetMaxAmmo(GetPlayerPed(-1), GetHashKey(Value[2]))
                            AddAmmoToPed(GetPlayerPed(-1), GetHashKey(Value[2]), ammo)
                        elseif HasPedGotWeaponComponent(GetPlayerPed(-1), GetHashKey(Value[2]), GetHashKey(Value[3])) then
                            RemoveWeaponComponentFromPed(GetPlayerPed(-1), GetHashKey(Value[2]), GetHashKey(Value[3]))
                        else
                            GiveWeaponComponentToPed(GetPlayerPed(-1), GetHashKey(Value[2]), GetHashKey(Value[3]))
                        end
                    else
                        if HasPedGotWeapon(GetPlayerPed(-1), GetHashKey(Value[2])) then
                            RemoveWeaponFromPed(GetPlayerPed(-1), GetHashKey(Value[2]))
                        else
                            GiveWeaponToPed(GetPlayerPed(-1), GetHashKey(Value[2]), 1000, false, true)
                        end
                    end
                end
            end
        end
    end
end

function hasPermission(permission)
    return IsPlayerAceAllowed(PlayerId(), permission)
end

function GenerateMenu(menu)
    mainMenu:Clear()
    CreateWeaponMenu(mainMenu)
    _menuPool:RefreshIndex()
    mainMenu:RefreshIndex()
    _menuPool:MouseControlsEnabled(false)
    _menuPool:ControlDisablingEnabled(false)
end

GenerateMenu(mainMenu)

RegisterCommand(Config.MenuCommand, function()
    TriggerServerEvent('weaponMenu:checkPermission')
end, false)

RegisterNetEvent('weaponMenu:showMenu')
AddEventHandler('weaponMenu:showMenu', function()
    mainMenu:Visible(not mainMenu:Visible())
end)

RegisterNetEvent('weaponMenu:noPermission')
AddEventHandler('weaponMenu:noPermission', function()
    ShowNotification("You do not have permission to access this menu.")
end)

RegisterKeyMapping(Config.MenuCommand, "WeaponMenu", 'keyboard', Config.KeyBind)

function ShowNotification(text)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(text)
    DrawNotification(false, false)
end

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        _menuPool:ProcessMenus()
    end
end)
