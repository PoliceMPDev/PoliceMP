local previewVeh = nil
local currentHeading = 0.0
local offset = vector3(0.0, 2.0, 0.0)
local previewing = false
local modelName = "adder"
local rerouteBlips = {} -- [id] = {vehicle, blip}

-- Triggered by server if player is permitted
RegisterNetEvent("reroute:startPreview", function()
    if previewing then return end

    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)
    local heading = GetEntityHeading(playerPed)

    RequestModel(modelName)
    while not HasModelLoaded(modelName) do Wait(0) end

    previewVeh = CreateVehicle(GetHashKey(modelName), playerCoords + offset, heading, false, false)
    SetEntityAlpha(previewVeh, 0, false)
    SetEntityVisible(previewVeh, false, false)
    SetEntityCollision(previewVeh, false, false)
    SetEntityNoCollisionEntity(previewVeh, PlayerPedId(), false)
    SetEntityCompletelyDisableCollision(previewVeh, true, false)
    SetEntityInvincible(previewVeh, true)
    FreezeEntityPosition(previewVeh, true)
    SetVehicleUndriveable(previewVeh, true)
    SetVehicleDoorsLocked(previewVeh, 4)
    FreezeEntityPosition(PlayerPedId(), true)
    
    previewing = true
    currentHeading = heading

    CreateThread(function()
        while previewing do
            Wait(0)

            -- Disable climbing/jumping
            DisableControlAction(0, 22, true) -- Jump (space)
            DisableControlAction(0, 44, true) -- Cover (Q)

            ShowRouteSubtitle(
                "~c~Q/E~s~ Up/Down  ~c~NUM 7/9~s~ Rotate" ..
                "~n~~c~NUM 8/5/4/6~s~ Move (F/B/L/R)" ..
                "~n~~c~ENTER~s~ Place Node   ~c~ESC~s~ Cancel", 0)

            if IsControlPressed(0, 111) then offset = offset + vector3(0.0, 0.1, 0.0) end
            if IsControlPressed(0, 112) then offset = offset - vector3(0.0, 0.1, 0.0) end
            if IsControlPressed(0, 108) then offset = offset - vector3(0.1, 0.0, 0.0) end
            if IsControlPressed(0, 109) then offset = offset + vector3(0.1, 0.0, 0.0) end
            if IsControlPressed(0, 44)  then offset = offset + vector3(0.0, 0.0, 0.1) end
            if IsControlPressed(0, 38)  then offset = offset - vector3(0.0, 0.0, 0.1) end

            if IsControlPressed(0, 117) then currentHeading = currentHeading - 1.0 end
            if IsControlPressed(0, 118) then currentHeading = currentHeading + 1.0 end

            local updatedPos = GetEntityCoords(PlayerPedId()) + offset
            SetEntityCoordsNoOffset(previewVeh, updatedPos.x, updatedPos.y, updatedPos.z, true, true, true)
            SetEntityHeading(previewVeh, currentHeading)

            local minDim, maxDim = GetModelDimensions(GetEntityModel(previewVeh))
            DrawWireBox(updatedPos, currentHeading, minDim, maxDim, 255, 0, 0, 150)

            if IsControlJustPressed(0, 191) then
                TriggerServerEvent("reroute:placeVehicle", GetEntityCoords(previewVeh), GetEntityHeading(previewVeh))
                DeleteEntity(previewVeh)
                FreezeEntityPosition(PlayerPedId(), false)
                previewing = false
            end

            if IsControlJustPressed(0, 322) then
                DeleteEntity(previewVeh)
                FreezeEntityPosition(PlayerPedId(), false)
                previewing = false
            end
        end
    end)
end)

RegisterNetEvent("reroute:attemptDeleteClosest", function()
    local playerCoords = GetEntityCoords(PlayerPedId())
    local closestId, closestVeh, closestDist = nil, nil, 5.0

    for id, data in pairs(rerouteBlips) do
        if data.vehicle and DoesEntityExist(data.vehicle) then
            local dist = #(GetEntityCoords(data.vehicle) - playerCoords)
            if dist < closestDist then
                closestDist = dist
                closestVeh = data.vehicle
                closestId = id
            end
        end
    end

    if closestId then
        DeleteEntity(closestVeh)
        TriggerServerEvent("reroute:removeVehicleById", closestId)
        ShowRouteNotification("~g~Reroute node deleted.")
    else
        ShowRouteNotification("~r~No reroute node found nearby.")
    end
end)

RegisterNetEvent("reroute:createPlacedVehicle", function(coords, heading, id)
    local model = `adder`
    RequestModel(model)
    while not HasModelLoaded(model) do Wait(0) end

    local veh = CreateVehicle(model, coords.x, coords.y, coords.z, heading, true, false)
    SetEntityAlpha(veh, 0, false)
    SetEntityVisible(veh, false, false)
    SetEntityCollision(veh, false, false)
    SetEntityNoCollisionEntity(veh, PlayerPedId(), false)
    SetEntityCompletelyDisableCollision(veh, true, false)
    SetEntityInvincible(veh, true)
    FreezeEntityPosition(veh, true)
    SetEntityAsMissionEntity(veh, true, true)
    SetEntityHeading(veh, heading)
    SetVehicleUndriveable(veh, true)
    SetVehicleDoorsLocked(veh, 4)
    ShowRouteNotification("~g~Reroute Node placed")

    rerouteBlips[id] = rerouteBlips[id] or {}
    rerouteBlips[id].vehicle = veh
end)

RegisterNetEvent("reroute:createRerouteBlip", function(coords, id)
    local blip = AddBlipForCoord(coords.x, coords.y, coords.z)
    SetBlipSprite(blip, 237)
    SetBlipDisplay(blip, 4)
    SetBlipScale(blip, 0.7)
    SetBlipColour(blip, 5)
    SetBlipAlpha(blip, 180)
    BeginTextCommandSetBlipName("STRING")
    AddTextComponentString("Reroute Blocker")
    EndTextCommandSetBlipName(blip)

    rerouteBlips[id] = rerouteBlips[id] or {}
    rerouteBlips[id].blip = blip
end)

RegisterNetEvent("reroute:removeRerouteBlip", function(id)
    local data = rerouteBlips[id]
    if data then
        if data.blip then RemoveBlip(data.blip) end
        rerouteBlips[id] = nil
    end
end)

RegisterNetEvent("reroute:deleteRerouteVehicle", function(id)
    local data = rerouteBlips[id]
    if data then
        if data.vehicle and DoesEntityExist(data.vehicle) then
            DeleteEntity(data.vehicle)
        end
        if data.blip then
            RemoveBlip(data.blip)
        end
        rerouteBlips[id] = nil
    end
end)

RegisterNetEvent("reroute:notify", function(msg)
    ShowRouteNotification(msg)
end)

-- Cleanup loop
CreateThread(function()
    while true do
        Wait(10000)
        for id, data in pairs(rerouteBlips) do
            if data.vehicle and not DoesEntityExist(data.vehicle) then
                if data.blip then RemoveBlip(data.blip) end
                rerouteBlips[id] = nil
            end
        end
    end
end)

-- Helpers
function ShowRouteSubtitle(msg, duration)
    BeginTextCommandPrint("STRING")
    AddTextComponentSubstringPlayerName(msg)
    EndTextCommandPrint(duration or 1000, true)
end

function ShowRouteNotification(msg)
    BeginTextCommandThefeedPost("STRING")
    AddTextComponentSubstringPlayerName(msg)
    EndTextCommandThefeedPostTicker(false, false)
end

function DrawWireBox(center, heading, minDim, maxDim, r, g, b, a)
    local function rotateOffset(vec, h)
        local rad = math.rad(h)
        local cos = math.cos(rad)
        local sin = math.sin(rad)
        return vector3(
            vec.x * cos - vec.y * sin,
            vec.x * sin + vec.y * cos,
            vec.z
        )
    end

    local corners = {}
    for _, x in ipairs({minDim.x, maxDim.x}) do
        for _, y in ipairs({minDim.y, maxDim.y}) do
            for _, z in ipairs({minDim.z, maxDim.z}) do
                local rotated = rotateOffset(vector3(x, y, z), heading)
                table.insert(corners, center + rotated)
            end
        end
    end

    local lines = {
        {1,2}, {1,3}, {1,5},
        {2,4}, {2,6},
        {3,4}, {3,7},
        {4,8},
        {5,6}, {5,7},
        {6,8},
        {7,8}
    }

    local offsetDirs = {
        vector3(0.01, 0.00, 0.00),
        vector3(-0.01, 0.00, 0.00),
        vector3(0.00, 0.01, 0.00),
        vector3(0.00, -0.01, 0.00),
    }

    for _, pair in ipairs(lines) do
        local start = corners[pair[1]]
        local stop = corners[pair[2]]
        DrawLine(start, stop, r, g, b, a)
        for _, dir in ipairs(offsetDirs) do
            DrawLine(start + dir, stop + dir, r, g, b, a)
        end
    end
end
