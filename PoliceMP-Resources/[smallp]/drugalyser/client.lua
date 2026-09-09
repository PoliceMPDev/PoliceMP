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
        bac = 'Testing..',
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
    TriggerServerEvent('drugalyser.server:doBacTest', GetPlayerServerId(target))
    Notify('Drugalyser request sent to ~g~' .. GetPlayerName(target))
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

RegisterCommand("drugalyser", function(source, args)
    SetDisplay(true)
    -- Animation and prop handling
    local ped = PlayerPedId()

    -- Load animation dictionary and play animation
    LoadAnimDict("amb@world_human_mobile_film_shocking@male@base")
    TaskPlayAnim(ped, "amb@world_human_mobile_film_shocking@male@base", "base", 8.0, -8.0, -1, 49, 0, false, false, false)

    -- Attach the inhaler prop
    AttachProp("v_ret_gc_pen2", 57005, 0.14, 0.05, -0.05, 45.0, 45.0, 0.0) -- Adjust position as needed
end)

RegisterNetEvent('drugalyser.client:requestBac')
AddEventHandler('drugalyser.client:requestBac', function(leo, target)
    local accepted = nil
    Notify(GetPlayerName(GetPlayerFromServerId(leo)) .. " wants to drugalyse you.")
    Notify("Accept [~g~Y~w~] Refuse [~r~N~w~]")

    Citizen.CreateThread(function()
        while accepted == nil do
            Citizen.Wait(0)
            if IsControlJustReleased(1, 246) then -- Accept
                accepted = true
                TriggerServerEvent('drugalyser.server:acceptedBac', leo, target)
                local result = KeyboardInput('Options: Clear, Cocaine, Cannabis, Opiates, Benzo, Meth', 10)
                if result and result:trim() ~= "" then
                    bac = result
                    TriggerServerEvent('drugalyser.server:returnBac', bac, leo)
                else
                    bac = nil
                    Notify("No input provided. Drugalyser result was not recorded.")
                end
            end
            if IsControlJustReleased(1, 249) then -- Refuse
                accepted = false
                TriggerServerEvent('drugalyser.server:refusedBac', leo, target)
            end
        end
    end)
end)

-- Utility function to trim spaces from a string
function string.trim(s)
    return s:match("^%s*(.-)%s*$")
end


RegisterNetEvent('drugalyser.client:displayBac')
AddEventHandler('drugalyser.client:displayBac', function(bac)
    SendNUIMessage({
        type = "data",
        bac = bac,
        textColor = '--color-black' 
    })
end)


RegisterNetEvent('drugalyser.client:bacRefused')
AddEventHandler('drugalyser.client:bacRefused', function(target)
    SetDisplay(false)
    SendNUIMessage({
        type = "data",
        bac = 'Testing..',
        textColor = '--color-black'
    })
    ClearPedTasks(PlayerPedId())
    RemoveProp()
    Notify(GetPlayerName(GetPlayerFromServerId(target)) .. " has ~r~refused~w~ the drug test!")
end)

RegisterNetEvent('drugalyser.client:acceptedBac')
AddEventHandler('drugalyser.client:acceptedBac', function(target)
    Notify("Swabbing ~g~" .. GetPlayerName(GetPlayerFromServerId(target)) .. " ~w~for any trace of drugs...")

    
end)
