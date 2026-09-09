local bac = nil
local display = false
local prop = nil -- Store the prop reference

function SetDisplay(bool)
    display = bool
    SetNuiFocus(bool, bool)
    SendNUIMessage({
        type = "ui",
        status = bool,
    })
end

function LoadAnimDict(dict)
    RequestAnimDict(dict)
    while not HasAnimDictLoaded(dict) do
        Citizen.Wait(10)
    end
end

function AttachProp(propName, boneID, x, y, z, rx, ry, rz)
    local ped = PlayerPedId()
    prop = CreateObject(GetHashKey(propName), 0, 0, 0, true, true, true)
    AttachEntityToEntity(prop, ped, GetPedBoneIndex(ped, boneID), x, y, z, rx, ry, rz, true, true, false, true, 1, true)
end

function RemoveProp()
    if DoesEntityExist(prop) then
        DeleteEntity(prop)
        prop = nil
    end
end

RegisterNUICallback("exit", function(data)
    SetDisplay(false)
    SendNUIMessage({
        type = "data",
        bac = '0.00',
        textColor = '--color-black'
    })

    -- Clean up animation and prop when NUI closes
    ClearPedTasks(PlayerPedId())
    RemoveProp()
end)

RegisterNUICallback("startBac", function(data)
    local target = GetClosestPlayerRadius(2.0)
    if target == nil then 
        Notify("Get closer to a player!") 
        return 
    end
    TriggerServerEvent('breathalyzer.server:doBacTest', GetPlayerServerId(target))
    Notify('Breathalyzer request sent to ~g~' .. GetPlayerName(target))
end)

Citizen.CreateThread(function()
    while display do
        Citizen.Wait(0)
        --DisableControlAction(0, 1, display)
        --DisableControlAction(0, 2, display)
        DisableControlAction(0, 142, display)
        DisableControlAction(0, 18, display)
        DisableControlAction(0, 322, display)
        DisableControlAction(0, 106, display)
    end
end)

RegisterCommand("breathalyser", function(source, args)
    SetDisplay(true)
    local ped = PlayerPedId()

    LoadAnimDict("amb@world_human_mobile_film_shocking@male@base")
    TaskPlayAnim(ped, "amb@world_human_mobile_film_shocking@male@base", "base", 8.0, -8.0, -1, 49, 0, false, false, false)

    AttachProp("porp_breatha", 57005, 0.15, 0.03, -0.05, 75.0, 45.0, 150.0)
end)

RegisterNetEvent('breathalyzer.client:requestBac')
AddEventHandler('breathalyzer.client:requestBac', function(leo, target)
    local accepted = nil
    Notify(GetPlayerName(GetPlayerFromServerId(leo)) .. " wants to breathalyse you.")
    Notify("Accept [~g~Y~w~] Refuse [~r~N~w~]")

    Citizen.CreateThread(function()
        while accepted == nil do
            Citizen.Wait(0)
            if IsControlJustReleased(1, 246) then -- Accept
                accepted = true
                TriggerServerEvent('breathalyzer.server:acceptedBac', leo, target)
                local result = KeyboardInput('BAC Level (Legal limit is 0.35):', 4)

                if result and tonumber(result) then
                    local bac = tonumber(result)
                    TriggerServerEvent('breathalyzer.server:returnBac', bac, leo)
                else
                    Notify("Invalid input. BAC level must be a valid number.")
                    TriggerServerEvent('breathalyzer.server:refusedBac', leo, target)
                end
            end
            if IsControlJustReleased(1, 249) then -- Refuse
                accepted = false
                TriggerServerEvent('breathalyzer.server:refusedBac', leo, target)
            end
        end
    end)
end)


RegisterNetEvent('breathalyzer.client:displayBac')
AddEventHandler('breathalyzer.client:displayBac', function(bac, color)
    SendNUIMessage({
        type = "data",
        bac = bac,
        textColor = color
    })
end)

RegisterNetEvent('breathalyzer.client:bacRefused')
AddEventHandler('breathalyzer.client:bacRefused', function(target)
    SetDisplay(false)
    SendNUIMessage({
        type = "data",
        bac = '0.00',
        textColor = '--color-black'
    })
    ClearPedTasks(PlayerPedId())
    RemoveProp()
    Notify(GetPlayerName(GetPlayerFromServerId(target)) .. " has ~r~refused~w~ the BAC test!")
end)

RegisterNetEvent('breathalyzer.client:acceptedBac')
AddEventHandler('breathalyzer.client:acceptedBac', function(target)
    Notify("Testing ~g~" .. GetPlayerName(GetPlayerFromServerId(target)) .. "'s ~w~blood alcohol content...")

    
end)
