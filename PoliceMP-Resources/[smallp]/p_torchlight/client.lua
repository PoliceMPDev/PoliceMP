local animations = {
    equip = { dict = "amb@incar@male@patrol@torch@base", name = "base" },
    store = { dict = "amb@incar@male@patrol@torch@exit", name = "exit" }
}

local isEquipped = false
local syncedClients = {}  

DecorRegister("FlashlightOn", 2)

local function GetCurrentWeaponObject(ped)
    local weaponObject = GetCurrentPedWeaponEntityIndex(ped)
    if not weaponObject or weaponObject == 0 then
        return nil
    end
    return weaponObject
end

RegisterNetEvent("p_torchlight:p_AddSynClient")
AddEventHandler("p_torchlight:p_AddSynClient", function(id)
    local playerServerId = tonumber(id)
    local client = GetPlayerFromServerId(playerServerId)
    table.insert(syncedClients, client)

    if #syncedClients == 1 then
        Citizen.CreateThread(function()
            while #syncedClients > 0 do
                Citizen.Wait(0)
            end
        end)
    end

    local ped = GetPlayerPed(client)
    local weaponObject = GetCurrentWeaponObject(ped)
    if weaponObject then
        DetachEntity(weaponObject, true, true)
        local boneIndex = GetPedBoneIndex(ped, 57005) 
        AttachEntityToEntity(weaponObject, ped, boneIndex, 0.125, 0.07, -0.03, 95.0, -25.0, 0.0, true, true, false, true, 2, true)
        SetEntityVisible(weaponObject, true, true)
    end
end)

RegisterNetEvent("p_torchlight:p_RemSynClient")
AddEventHandler("p_torchlight:p_RemSynClient", function(id)
    local playerServerId = tonumber(id)
    local client = GetPlayerFromServerId(playerServerId)
    for i, v in ipairs(syncedClients) do
        if v == client then
            table.remove(syncedClients, i)
            break
        end
    end
end)

Citizen.CreateThread(function()
    local holdTime = 0
    local keyDown = false
    local keyControl = 73 -- x key
    while true do
        local ped = PlayerPedId()
        local currentWeapon = GetSelectedPedWeapon(ped)
        local flashlightHash = GetHashKey("WEAPON_FLASHLIGHT")
        
        if currentWeapon == flashlightHash then
            if IsControlPressed(0, keyControl) then
                if not keyDown then
                    keyDown = true
                    holdTime = 0
                else
                    holdTime = holdTime + 10
                    if holdTime >= 600 then 
                        if not isEquipped then
                            EquipFlashlight()
                            SetFlashLightKeepOnWhileMoving(true)
                        else
                            StoreFlashlight()
                            SetFlashLightKeepOnWhileMoving(false)
                        end
                        while IsControlPressed(0, keyControl) do
                            Citizen.Wait(10)
                        end
                        keyDown = false
                        holdTime = 0
                    end
                end
            else
                keyDown = false
                holdTime = 0
            end
        else
            keyDown = false
            holdTime = 0
        end
        Citizen.Wait(10)
    end
end)

function EquipFlashlight()
    Citizen.CreateThread(FlashlightHandler)

    local ped = PlayerPedId()
    local weaponObject = GetCurrentWeaponObject(ped)

    RequestAnimDict(animations.equip.dict)
    while not HasAnimDictLoaded(animations.equip.dict) do
        Citizen.Wait(10)
    end
    SetEntityVisible(weaponObject, false, false)
    TaskPlayAnim(ped, animations.equip.dict, animations.equip.name, 8.0, -8.0, -1, 50, -3, false, false, false)

    isEquipped = true

    Citizen.Wait(30)
    TriggerServerEvent("p_torchlight:p_SynClient")
end

function StoreFlashlight()
    local ped = PlayerPedId()
    RequestAnimDict(animations.store.dict)
    while not HasAnimDictLoaded(animations.store.dict) do
        Citizen.Wait(10)
    end
    TaskPlayAnim(ped, animations.store.dict, animations.store.name, 3.0, -3.0, -1, 50, -8, false, false, false)
    SetFlashLightKeepOnWhileMoving(true)


    Citizen.Wait(1000)
    ClearPedTasks(ped)

    isEquipped = false
    TriggerServerEvent("p_torchlight:p_PassSynClient")
end

Citizen.CreateThread(function()
    while true do
        local ped = PlayerPedId()
        local currentWeapon = GetSelectedPedWeapon(ped)
        local flashlightHash = GetHashKey("WEAPON_FLASHLIGHT")
        if isEquipped and currentWeapon ~= flashlightHash then
            StoreFlashlight()
            ClearPedTasks(ped)
        end
        Citizen.Wait(500)  
    end
end)
