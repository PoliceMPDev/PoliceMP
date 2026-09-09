local nearestDJName = ""

local function openPanelDJ()
    local input = lib.inputDialog('P13 DJ', {
        'Enter Song URL', 
        'Enter Volume (0.0 - 1.0)', 
        'Enter Range'
    })

    if not input then 
        TriggerEvent('chat:addMessage', { args = { '^1[P13]', 'No input provided!' } })
        return
    end

    local input_one = input[1] or ""
    local input_two = tonumber(input[2]) or 0.1  -- Default volume to 0.1
    local input_three = tonumber(input[3]) or 20 -- Default distance to 20

    TriggerServerEvent('smallp_executeDJmenu', input_one, input_two, input_three)
end

local function CheckNearbyDJ()
    local minDistance = math.huge
    local plyCoords = GetEntityCoords(PlayerPedId(), false)
    for _, dj in pairs(Config.DJpanels) do
        for i = 1, #dj.coords, 1 do
            local djDistance = #(vector3(dj.coords[i].x, dj.coords[i].y, dj.coords[i].z) - plyCoords)
            if djDistance < minDistance then
                minDistance = djDistance
                nearestDJName = dj.dj_name
            end
        end
    end
end

local function destroyMusic()
    TriggerServerEvent('smallp_destroyMusic') -- Fixed typo
end

RegisterNetEvent('smallp_play_music', function(input_one, input_two, input_three, tokenizer)
    if tokenizer == Config.TokenizerToken then
        if not input_three then
            TriggerEvent('chat:addMessage', { args = { '^1[P13]', 'Range is missing!' } })
        else
            local xSound = exports["xsound"]
            CheckNearbyDJ()
            Wait(100)
            destroyMusic()
            Wait(500)

            local nearestCoords
            for _, dj in pairs(Config.DJpanels) do
                if dj.dj_name == nearestDJName then
                    nearestCoords = dj.coords[1] -- Assuming one coordinate per DJ panel
                end
            end

            if nearestCoords then
                xSound:PlayUrlPos(nearestDJName, input_one, input_two, vector3(nearestCoords.x, nearestCoords.y, nearestCoords.z), false)
                xSound:Distance(nearestDJName, tonumber(input_three))
            else
                TriggerEvent('chat:addMessage', { args = { '^1[P13]', 'Unable to locate nearest DJ panel!' } })
            end
        end
    else
        TriggerEvent('chat:addMessage', { args = { '^1[P13]', 'Invalid Tokenizer!' } })
    end
end)

RegisterNetEvent('smallp_destroyMusic', function(djName)
    local xSound = exports["xsound"]
    if xSound:isPlaying(djName) then
        print("🛑 Stopping music for:", djName)
        xSound:Destroy(djName)
    else
        print("⚠️ No music playing for", djName, "- Ignoring destroy request")
    end
end)

local hasPermission = false

RegisterNetEvent('smallp_returnPermission', function(allowed)
    hasPermission = allowed
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(10000) -- Reduced spam, checking every 10 seconds
        TriggerServerEvent('smallp_checkPermission')
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(5)
        local playerCoords = GetEntityCoords(PlayerPedId(), false)

        for _, dj in pairs(Config.DJpanels) do
            for i = 1, #dj.coords, 1 do
                local djCoords = vector3(dj.coords[i].x, dj.coords[i].y, dj.coords[i].z)
                local distance = #(djCoords - playerCoords)

                if distance < 3 and hasPermission and IsControlJustReleased(0, 46) then
                    openPanelDJ()
                end

                if distance < 5 and not dj.isInZone and hasPermission then
                    lib.showTextUI('Press [E] to open Music Panel')
                    dj.isInZone = true
                elseif distance >= 5 and dj.isInZone then
                    dj.isInZone = false
                    lib.hideTextUI()
                end
            end
        end
    end
end)

local hasPermission = false
local activeVehicleMusic = {}

RegisterNetEvent('smallp_returnPermission', function(allowed)
    hasPermission = allowed
end)

-- Request permission check every 10s
CreateThread(function()
    while true do
        Wait(10000)
        TriggerServerEvent('smallp_checkPermission')
    end
end)

RegisterCommand("carmusicstart", function()
    if not hasPermission then
        --TriggerEvent('chat:addMessage', { args = { '^1[P13]', '🚫 You do not have permission to use this command!' } })
        return
    end

    local ped = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(ped, false)
    if vehicle == 0 then
        --TriggerEvent('chat:addMessage', { args = { '^1[P13]', '🚗 You are not in a vehicle!' } })
        return
    end

    local input = lib.inputDialog('Vehicle Music', {
        'Enter Song URL',
        'Enter Volume (0.0 - 1.0)',
        'Enter Range'
    })
    if not input then
        --TriggerEvent('chat:addMessage', { args = { '^1[P13]', '❌ No input provided!' } })
        return
    end

    local url = input[1]
    local volume = tonumber(input[2]) or 0.1
    local range = tonumber(input[3]) or 20.0
    local vehicleNetId = VehToNet(vehicle)

    TriggerServerEvent('smallp_startVehicleMusic', url, volume, range, vehicleNetId)
end, false)

RegisterCommand("carmusicstop", function()
    if not hasPermission then
        --TriggerEvent('chat:addMessage', { args = { '^1[P13]', '🚫 You do not have permission to use this command!' } })
        return
    end

    if not currentVehicleMusic then
        --TriggerEvent('chat:addMessage', { args = { '^1[P13]', '⚠️ No music is currently playing.' } })
        return
    end

    local xSound = exports["xsound"]
    if xSound:isPlaying(currentVehicleMusic.name) then
        xSound:Destroy(currentVehicleMusic.name)
        --TriggerEvent('chat:addMessage', { args = { '^1[P13]', '🛑 Vehicle music stopped!' } })
    end

    currentVehicleMusic = nil
end, false)

CreateThread(function()
    while true do
        Wait(500)

        for netId, data in pairs(activeVehicleMusic) do
            local vehicle = data.vehicle
            if DoesEntityExist(vehicle) then
                local coords = GetEntityCoords(vehicle)
                exports["xsound"]:Position(data.name, vector3(coords.x, coords.y, coords.z))
            else
                if exports["xsound"]:isPlaying(data.name) then
                    exports["xsound"]:Destroy(data.name)
                    --TriggerEvent('chat:addMessage', {args = { '^1[P13]', '🛑 Vehicle ' .. netId .. ' despawned — music stopped.' } })
                end
                activeVehicleMusic[netId] = nil
            end
        end
    end
end)

RegisterNetEvent('smallp_playVehicleMusic', function(url, volume, range, vehicleNetId, token)
    if token ~= Config.TokenizerToken then return end

    local vehicle = NetToVeh(vehicleNetId)
    if not DoesEntityExist(vehicle) then return end

    local vehicleName = "vehicleMusic_" .. vehicleNetId
    local coords = GetEntityCoords(vehicle)

    exports["xsound"]:PlayUrlPos(vehicleName, url, volume, vector3(coords.x, coords.y, coords.z), true)
    exports["xsound"]:Distance(vehicleName, range)

    -- ✅ Store tracking info for ALL players
    activeVehicleMusic[vehicleNetId] = {
    name = vehicleName,
    vehicle = vehicle
}
end)
