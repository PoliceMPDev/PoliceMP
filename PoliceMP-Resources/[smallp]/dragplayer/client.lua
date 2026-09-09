local dragging_data = {
    InProgress = false,
    target = -1,
    Anim = {
        dict = "combat@drag_ped@",
        start = "injured_pickup_back_",
        loop = "injured_drag_",
        ending = "injured_putdown_"
    }
}

local function HelpNotification(text)
    SetTextComponentFormat("STRING")
    AddTextComponentString(text)
    DisplayHelpTextFromStringLabel(0, 0, 1, -1)
end

local function Notification(text)
    AddTextEntry('notify', text)
    SetNotificationTextEntry('notify')
    DrawNotification(false, true)
end

local function GetClosestPlayer(radius)
    local players = GetActivePlayers()
    local closestDistance = -1
    local closestPlayer = -1
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)

    for _, playerId in ipairs(players) do
        local targetPed = GetPlayerPed(playerId)
        if targetPed ~= playerPed then
            local targetCoords = GetEntityCoords(targetPed)
            local distance = #(targetCoords - playerCoords)
            if closestDistance == -1 or closestDistance > distance then
                closestPlayer = playerId
                closestDistance = distance
            end
        end
    end
    if closestDistance ~= -1 and closestDistance <= radius then
        return closestPlayer
    else
        return nil
    end
end

local function LoadAnimDict(animDict)
    if not HasAnimDictLoaded(animDict) then
        RequestAnimDict(animDict)
        while not HasAnimDictLoaded(animDict) do
            Wait(0)
        end        
    end
    return animDict
end

function PlayAnim(type, desinence)
    local duration = nil
    if type == "loop" then duration = -1 elseif type == "start" then duration = 6000 elseif type == "ending" then duration = 5000 end

    LoadAnimDict(dragging_data.Anim.dict)
    TaskPlayAnim(PlayerPedId(), dragging_data.Anim.dict, dragging_data.Anim[type] .. desinence, 8.0, -8.0, duration, 33, 0, 0, 0, 0)

    if duration ~= -1 then
        Wait(duration)
        ClearPedTasks(PlayerPedId())
    end
end

function WaitControlsInteractions()
    Citizen.CreateThread(function()
        while true do
            HelpNotification("~INPUT_VEH_DUCK~ to drop the body")
            if IsControlJustPressed(1, 323) then -- X
                DragClosest()
                return
            end
            Wait(5)
        end
    end)
end    

--RegisterCommand("dp", function()
--     TriggerServerEvent("pmp_DragPeople:checkDragPermission")
--end)

RegisterNetEvent("pmp_DragPeople:startDrag")
AddEventHandler("pmp_DragPeople:startDrag", function()
    DragClosest()
end)

function DragClosest()
    local player = PlayerPedId()

    if not dragging_data.InProgress then
        local closestPlayer = GetClosestPlayer(1)
        local Ped_ClosestPlayer = GetPlayerPed(closestPlayer)

        if closestPlayer then
            local target = GetPlayerServerId(closestPlayer)
            if target ~= -1 then
                dragging_data.InProgress = true
                dragging_data.target = target

                TriggerServerEvent("pmp_DragPeople:sync", target)
                PlayAnim("start", "plyr")
                PlayAnim("loop", "plyr")
                WaitControlsInteractions()
            else
                Notification("~r~No one nearby to drag!")
            end
        else
            Notification("~r~No one nearby to drag!")
        end
    else
        local target_ped = GetPlayerPed(GetPlayerFromServerId(dragging_data.target))

        TriggerServerEvent("pmp_DragPeople:stop", dragging_data.target)
        
        DetachEntity(PlayerPedId(), true, false)
        PlayAnim("ending", "plyr")
        ClearPedTasks(target_ped)

        dragging_data.InProgress = false
        dragging_data.target = 0
    end
end

RegisterNetEvent("pmp_DragPeople:syncTarget")
AddEventHandler("pmp_DragPeople:syncTarget", function(target)
    local target_ped = GetPlayerPed(GetPlayerFromServerId(target))
    local player     = PlayerPedId()

    dragging_data.InProgress = true

    SetEntityCoords(player, GetOffsetFromEntityInWorldCoords(target_ped, 0.0, 1.2, -1.0))
    SetEntityHeading(player, GetEntityHeading(target_ped))
    PlayAnim("start", "ped")
    ClearPedTasks(player)

    AttachEntityToEntity(player, target_ped, 1816, 4103, 0.48, 0.0, 0.0, 0.0, 0.0, 0.0)
    PlayAnim("loop", "ped")

    Citizen.CreateThread(function()
        while dragging_data.InProgress do
            DisableAllControlActions(0)

            EnableControlAction(0, 1, true)   -- LookLeftRight (Mouse)
            EnableControlAction(0, 2, true)   -- LookUpDown (Mouse)
            EnableControlAction(0, 245, true) -- Enable text chat
                                
            Wait(0)
        end
    end)
end)

RegisterNetEvent("pmp_DragPeople:cl_stop")
AddEventHandler("pmp_DragPeople:cl_stop", function(_target)
    _target = GetPlayerPed(GetPlayerFromServerId(_target))
    dragging_data.InProgress = false

    DetachEntity(PlayerPedId(), true, false)
    SetEntityCoords(PlayerPedId(), GetOffsetFromEntityInWorldCoords(_target, 0.0, 0.4, -1.0))
    PlayAnim("ending", "ped")

    Citizen.CreateThread(function()
        Wait(500) -- Small delay to ensure animation completes before re-enabling controls
        EnableAllControlActions(0)
    end)
end)

RegisterNetEvent("pmp_DragPeople:sendErrorNotification")
AddEventHandler("pmp_DragPeople:sendErrorNotification", function(message)
    AddTextEntry('notify_error', message)
    SetNotificationTextEntry('notify_error')
    DrawNotification(false, true)
end)
