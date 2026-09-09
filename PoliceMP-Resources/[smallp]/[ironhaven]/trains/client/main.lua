local freight = nil
local isfreightStopped = false
local metro = nil
local isMetroStopped = false
local oldBlip = nil
local oldMetroBlip = nil
local isInMetro = false
local metroPosCur = nil
local metroVelo = nil
local metroBlip = nil
local clientSideMetro = nil
local canGetInMetro = false
local alreadySet = false
local metroPosWhenStopped = nil
local isCooldown = false
local isMetroNearPlayer = false
local stoppedClient = false
local isHost = false
local alreadySpawned = false

--local metro2 = nil
local isMetro2Stopped = false
local isInMetro2 = false
local metro2PosCur = nil
local metro2Velo = nil
local metro2Blip = nil
local clientSideMetro2 = nil
local canGetInMetro2 = false
local metro2PosWhenStopped = nil
local isMetro2NearPlayer = false
local stoppedClient2 = false
local alreadySet2 = false
local isCooldown2 = false

local metroBlip = nil
local freightBlip = nil
local metroEntityBlip = nil
local metro2Blip = nil
local oldMetro2Blip = nil
local metro2EntityBlip = nil

local isFreightStopped = false
local freightClientStopped = false
local freightPosWhenStopped = nil
local freightAlreadySet = false
local canGetInFreight = false
local isFreightNearPlayer = false
local isCooldownFreight = false
local isInFreight = false

function loadTrainModels()
    local trainsAndCarriages = {
        'freight', 'metrotrain', 'freightcont1', 'freightcar',
        'freightcar2', 'freightcont2', 'tankercar', 'freightgrain'
    }

    for _, vehicleName in ipairs(trainsAndCarriages) do
        local modelHashKey = GetHashKey(vehicleName)
        RequestModel(modelHashKey)
        while not HasModelLoaded(modelHashKey) do
            Citizen.Wait(500)
        end
    end
end

SetTrainsForceDoorsOpen(false)

RegisterNetEvent("k_trains:SpawnHostTrains")
AddEventHandler("k_trains:SpawnHostTrains", function()
    --Variable Reset
    freight = nil
    isfreightStopped = false
    metro = nil
    isMetroStopped = false
    isInMetro = false
    metroPosCur = nil
    metroVelo = nil
    clientSideMetro = nil
    canGetInMetro = false
    alreadySet = false
    metroPosWhenStopped = nil
    isCooldown = false
    isMetroNearPlayer = false
    stoppedClient = false

    --metro2 = nil
    isMetro2Stopped = false
    isInMetro2 = false
    metro2PosCur = nil
    metro2Velo = nil
    metro2Blip = nil
    clientSideMetro2 = nil
    canGetInMetro2 = false
    metro2PosWhenStopped = nil
    isMetro2NearPlayer = false
    stoppedClient2 = false
    alreadySet2 = false
    isCooldown2 = false

    metroBlip = nil
    freightBlip = nil
    metroEntityBlip = nil
    metro2Blip = nil
    oldMetro2Blip = nil
    metro2EntityBlip = nil

    isFreightStopped = false
    freightClientStopped = false
    freightPosWhenStopped = nil
    freightAlreadySet = false
    canGetInFreight = false
    isFreightNearPlayer = false
    isCooldownFreight = false
    isInFreight = false

    --Set that this client is the host
    isHost = false

    Citizen.Wait(500)
    loadTrainModels()

    local randomDir = math.random(0, 1)
    if (randomDir >= 0.5) then
        randomDir = true
    else
        randomDir = false
    end

    local randomTrain = math.random(1, #Config.FreightSpawns)
    local randomTrainLocation = Config.FreightSpawns[randomTrain]
    freight = CreateMissionTrain(23, randomTrainLocation[1], randomTrainLocation[2],
        randomTrainLocation[3], randomDir)
    SetTrainCruiseSpeed(freight, Config.FreightSpeed)
    SetEntityAsMissionEntity(freight, true, true)
    SetEntityInvincible(freight, true)

    Citizen.Wait(500)

    metro = CreateMissionTrain(27, 40.2, -1201.3, 31.0, false)
    SetTrainCruiseSpeed(metro, Config.MetroSpeed)
    SetEntityAsMissionEntity(metro, true, true)
    SetEntityInvincible(metro, true)
    SetTrainsForceDoorsOpen(false)

    --[[metro2 = CreateMissionTrain(27, -618.0, -1476.8, 16.2, false)
    SetTrainCruiseSpeed(metro2, Config.MetroSpeed)
    SetEntityAsMissionEntity(metro2, true, true)
    SetEntityInvincible(metro2, true)
    SetTrainsForceDoorsOpen(false)]]--

    repeat
        Citizen.Wait(0)
        SetTrainCruiseSpeed(metro, Config.MetroSpeed)
    until GetEntitySpeed(metro) > 0.0

    --[[repeat
        Citizen.Wait(0)
        SetTrainCruiseSpeed(metro2, Config.MetroSpeed)
    until GetEntitySpeed(metro2) > 0.0]]--


    Citizen.Wait(2000)
end)



Citizen.CreateThread(function()
    while true do
        Citizen.Wait(20)
        if (DoesEntityExist(freight) and not isfreightStopped) then
            local freightPos = GetEntityCoords(freight)
            for k, v in pairs(Config.FreightStations) do
                if (GetDistanceBetweenCoords(v.trainStop.x, v.trainStop.y, v.trainStop.z, freightPos.x, freightPos.y, freightPos.z, false) <= 10) then
                    SetTrainCruiseSpeed(freight, 0)
                    while GetEntitySpeed(freight) > 0 do
                        Citizen.Wait(0)
                    end
                    isFreightStopped = true
                    Citizen.Wait(Config.FreightWaitTime * 1000)
                    SetTrainCruiseSpeed(freight, Config.FreightSpeed)
                    isFreightStopped = false
                end
            end
        end
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(20)

        if (DoesEntityExist(metro)) then
            if (not isMetroStopped) then
                local metroPos = GetEntityCoords(metro)
                for k, v in pairs(Config.MetroStations) do
                    if (GetDistanceBetweenCoords(v.trainStop.x, v.trainStop.y, v.trainStop.z, metroPos.x, metroPos.y, metroPos.z, false) <= 2) then
                        StopAndStartTrainWhenInStation(true)
                    end
                end
            end
        end
    end
end)

--[[Citizen.CreateThread(function()
    while true do
        Citizen.Wait(20)
        if (DoesEntityExist(metro2)) then
            if (not isMetro2Stopped) then
                local metro2Pos = GetEntityCoords(metro2)
                for k, v in pairs(Config.MetroStations) do
                    if (GetDistanceBetweenCoords(v.trainStop.x, v.trainStop.y, v.trainStop.z, metro2Pos.x, metro2Pos.y, metro2Pos.z, false) <= 2) then
                        StopAndStartTrainWhenInStation(false)
                    end
                end
            end
        end
    end
end)]]--

function StopAndStartTrainWhenInStation(isMetro1)
    local train = isMetro1 and metro or metro2

    if train then
        -- Stop the train
        SetEntityVelocity(train, 0.0, 0.0, 0.0)
        NetworkRequestControlOfEntity(train)
        repeat
            Citizen.Wait(0)
            NetworkRequestControlOfEntity(train)
        until NetworkHasControlOfEntity(train)

        SetTrainCruiseSpeed(train, 0.0)
        Citizen.Wait(100)
        local stoppedTimer = GetGameTimer()

        -- Ensure the train is fully stopped
        repeat
            SetTrainCruiseSpeed(train, 0.0)
            Citizen.Wait(0)
        until GetEntitySpeed(train) <= 0

        -- Smoothly open train doors
        OpenTrainDoors(train)

        -- Wait for passengers to get in/out
        local stopDuration = Config.MetroWaitTime * 1000
        while (GetGameTimer() - stoppedTimer < stopDuration) do
            Citizen.Wait(0)
        end

        -- Smoothly close train doors
        CloseTrainDoors(train)

        -- Restart the train
        Citizen.Wait(100)
        SetTrainCruiseSpeed(train, Config.MetroSpeed)

        if isMetro1 then
            isMetroStopped = false
        else
            isMetro2Stopped = false
        end
    end
end

function OpenTrainDoors(train)
    if not DoesEntityExist(train) then return end

    local trainNetId = NetworkGetNetworkIdFromEntity(train)
    TriggerServerEvent('Train:SyncDoors', trainNetId, true)

    -- Instantly open the doors
    SetTrainDoorOpenRatio(train, 1, 1.0) -- Front left door
    SetTrainDoorOpenRatio(train, 3, 1.0) -- Front right door
end

function CloseTrainDoors(train)
    if not DoesEntityExist(train) then return end

    local trainNetId = NetworkGetNetworkIdFromEntity(train)
    TriggerServerEvent('Train:SyncDoors', trainNetId, false)

    -- Instantly close the doors
    SetTrainDoorOpenRatio(train, 1, 0.0) -- Front left door
    SetTrainDoorOpenRatio(train, 3, 0.0) -- Front right door
end

RegisterNetEvent('Train:SyncDoors')
AddEventHandler('Train:SyncDoors', function(trainNetId, isOpening)
    local train = NetworkGetEntityFromNetworkId(trainNetId)
    if not DoesEntityExist(train) then return end

    if isOpening then
        -- Instantly open the doors
        SetTrainDoorOpenRatio(train, 1, 1.0) -- Front left door
        SetTrainDoorOpenRatio(train, 3, 1.0) -- Front right door
    else
        -- Instantly close the doors
        SetTrainDoorOpenRatio(train, 1, 0.0) -- Front left door
        SetTrainDoorOpenRatio(train, 3, 0.0) -- Front right door
    end
end)

AddEventHandler('onResourceStop', function(name)
    if (GetCurrentResourceName() == name) then
        deleteTrain()
    end
end)

RegisterNetEvent("k_trains:changedHost")
AddEventHandler("k_trains:changedHost", function()
    deleteTrain()
end)

function deleteTrain()
    if (freight ~= nil) then
        DeleteMissionTrain(freight)
    end

    if (metro ~= nil) then
        DeleteMissionTrain(metro)
    end

    if (metro2 ~= nil) then
        DeleteMissionTrain(metro2)
    end
end

--[[RegisterNetEvent("k_trains:ShowEnterText")
AddEventHandler("k_trains:ShowEnterText",
    function(stopped, metroCoords, metro2Coords, stopped2, isFreightStopped, freightPos)
        stoppedClient2 = stopped2
        stoppedClient = stopped
        freightClientStopped = isFreightStopped
        metroPosWhenStopped = metroCoords
        metro2PosWhenStopped = metro2Coords
        freightPosWhenStopped = freightPos
        if (not alreadySet2) then
            canGetInMetro2 = stopped2
        end
        if (not alreadySet) then
            canGetInMetro = stopped
        end
        if (not freightAlreadySet) then
            canGetInFreight = isFreightStopped
        end
        if (canGetInMetro) then
            alreadySet = true
        end
        if (canGetInMetro2) then
            alreadySet2 = true
        end
        if (canGetInFreight) then
            freightAlreadySet = true
        end
    end)]]--

CreateThread(function()
    while true do
        Wait(0)
        if alreadySet then
            Citizen.Wait(Config.MetroWaitTime * 1000)
            canGetInMetro = false
            Citizen.Wait(2000)
            alreadySet = false
        end
    end
end)


CreateThread(function()
    while true do
        Wait(0)
        if freightAlreadySet then
            Citizen.Wait(Config.FreightWaitTime * 1000)
            canGetInFreight = false
            Citizen.Wait(2000)
            freightAlreadySet = false
        end
    end
end)

--[[CreateThread(function()
    while true do
        Wait(0)
        if alreadySet2 then
            Citizen.Wait(Config.MetroWaitTime * 1000)
            canGetInMetro2 = false
            Citizen.Wait(2000)
            alreadySet2 = false
        end
    end
end)]]--

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(50)

        local coordA = GetEntityCoords(GetPlayerPed(-1), 1)
        local coordB = GetOffsetFromEntityInWorldCoords(GetPlayerPed(-1), 0.0, 3.0, 0.0)
        local metroWithRaycast = getVehicleInDirection(coordA, coordB)
        if DoesEntityExist(metroWithRaycast) and (clientSideMetro == nil or clientSideMetro2 == nil) then
            if GetEntityModel(metroWithRaycast) == GetHashKey("metrotrain") then
                if (clientSideMetro == nil and metroWithRaycast ~= clientSideMetro2) then
                    clientSideMetro = metroWithRaycast
                elseif (metroWithRaycast ~= clientSideMetro) then
                    clientSideMetro2 = metroWithRaycast
                end
            end
        end

        if (metroPosWhenStopped ~= nil and stoppedClient and #(GetEntityCoords(GetPlayerPed(-1)) - metroPosWhenStopped) <= Config.MetroInteractionRadius) then
            isMetroNearPlayer = true
        else
            isMetroNearPlayer = false
        end

        --[[if (metro2PosWhenStopped ~= nil and stoppedClient2 and #(GetEntityCoords(GetPlayerPed(-1)) - metro2PosWhenStopped) <= Config.MetroInteractionRadius) then
            isMetro2NearPlayer = true
        else
            isMetro2NearPlayer = false
        end]]--

        if (freightPosWhenStopped ~= nil and isFreightStopped and #(GetEntityCoords(GetPlayerPed(-1)) - freightPosWhenStopped) <= Config.FreightInteractionRadius) then
            isFreightNearPlayer = true
        else
            isFreightNearPlayer = false
        end


        if (isInMetro) then
            if (not stoppedClient and metroPosCur ~= nil and #(GetEntityCoords(GetPlayerPed(-1)) - metroPosCur) >= Config.MetroInteractionRadius) then
                if (Config.ShouldBeTeleportedBack) then
                    SetEntityCoords(GetPlayerPed(-1), metroPosCur.x, metroPosCur.y, metroPosCur.z + 1.0, false, false,
                        false, false)
                    SetEntityVelocity(GetPlayerPed(-1), metroVelo.x, metroVelo.y, metroVelo.z)
                    ExecuteCommand("+trsac")
                else
                    isInMetro = false
                end
            end
        end

        --[[if (isInMetro2) then
            if (not stoppedClient2 and metro2PosCur ~= nil and #(GetEntityCoords(GetPlayerPed(-1)) - metro2PosCur) >= Config.MetroInteractionRadius) then
                if (Config.ShouldBeTeleportedBack) then
                    SetEntityCoords(GetPlayerPed(-1), metro2PosCur.x, metro2PosCur.y, metro2PosCur.z + 1.0, false, false,
                        false, false)
                    SetEntityVelocity(GetPlayerPed(-1), metro2Velo.x, metro2Velo.y, metro2Velo.z)
                    ExecuteCommand("+trsac")
                else
                    isInMetro2 = false
                end
            end
        end]]--
    end
end)

function DisplayHelpText(text)
    BeginTextCommandDisplayHelp("STRING")
    AddTextComponentSubstringPlayerName(text)
    EndTextCommandDisplayHelp(0, false, true, -1)
end

function message(lineOne, lineTwo, lineThree, duration)
    BeginTextCommandDisplayHelp("THREESTRINGS")
    AddTextComponentSubstringPlayerName(lineOne)
    AddTextComponentSubstringPlayerName(lineTwo or "")
    AddTextComponentSubstringPlayerName(lineThree or "")
    EndTextCommandDisplayHelp(0, false, true, duration or 5000)
end

function getNearestTrainStation()
    local least = vector3(0.0, 0.0, 0.0)
    local leastDist = math.maxinteger
    local playerCoords = GetEntityCoords(GetPlayerPed(-1))
    for k, v in ipairs(Config.MetroStations) do
        if (#(playerCoords - v.exitCoord) <= leastDist) then
            least = v.exitCoord
            leastDist = #(playerCoords - v.exitCoord)
        end
    end
    return least
end

function getNearestFreightStation()
    local least = vector3(0.0, 0.0, 0.0)
    local leastDist = math.maxinteger
    local playerCoords = GetEntityCoords(GetPlayerPed(-1))
    for k, v in ipairs(Config.FreightStations) do
        if (#(playerCoords - v.exitCoord) <= leastDist) then
            least = v.exitCoord
            leastDist = #(playerCoords - v.exitCoord)
        end
    end
    return least
end

function getVehicleInDirection(coordFrom, coordTo)
    local rayHandle = CastRayPointToPoint(coordFrom.x, coordFrom.y, coordFrom.z, coordTo.x, coordTo.y, coordTo.z, 10,
        GetPlayerPed(-1), 0)
    local a, b, c, d, vehicle = GetRaycastResult(rayHandle)
    return vehicle
end --Thanks to TheNickoos for this funtion

RegisterNetEvent('k_trains:NotifyHost')
AddEventHandler('k_trains:NotifyHost', function()
    TriggerServerEvent("k_trains:playerHostLeft")
end)


RegisterNetEvent("k_trains:stopTrains")
AddEventHandler("k_trains:stopTrains", function()
    SetEntityVelocity(metro, 0.0, 0.0, 0.0)
    SetTrainCruiseSpeed(metro, 0.0)
    isMetroStopped = true

    --[[SetEntityVelocity(metro2, 0.0, 0.0, 0.0)
    SetTrainCruiseSpeed(metro2, 0.0)
    isMetro2Stopped = true]]--


    SetTrainCruiseSpeed(freight, 0)
    while GetEntitySpeed(freight) > 0 do
        Citizen.Wait(0)
    end
    isFreightStopped = true
end)

RegisterNetEvent("k_trains:startTrains")
AddEventHandler("k_trains:startTrains", function()
    SetTrainCruiseSpeed(metro, Config.MetroSpeed)
    isMetroStopped = false

    --[[SetTrainCruiseSpeed(metro2, Config.MetroSpeed)
    isMetro2Stopped = false]]--

    SetTrainCruiseSpeed(freight, Config.FreightSpeed)
    isFreightStopped = false
end)

RegisterNetEvent("ShowNotification")
AddEventHandler("ShowNotification", function(message)
    -- Default GTA V notification
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(false, true)
end)

local isInSeat = false
local nearestTrain = nil
local targetSeatIndex = nil -- -1 for driver seat, 0 for front passenger seat
local seatBoneNames = {
    ["driver"] = "seat_dside_f" -- Driver's seat bone name
}

-- Enter a seat in the train
function EnterSeat(train, seatIndex)
    if train and DoesEntityExist(train) then
        local playerPed = PlayerPedId()
        TaskEnterVehicle(playerPed, train, 2000, seatIndex, 1.0, 1, 0)
        isInSeat = true
        nearestTrain = train
        targetSeatIndex = seatIndex
    end
end

-- Exit the current seat
function ExitSeat()
    if isInSeat then
        local playerPed = PlayerPedId()
        TaskLeaveVehicle(playerPed, nearestTrain, 0)
        isInSeat = false
        nearestTrain = nil
        targetSeatIndex = nil
    end
end

-- Get the node position for a specific seat
function GetSeatNodePosition(train, seatType)
    local boneName = seatBoneNames[seatType]
    if not boneName then return nil end

    local boneIndex = GetEntityBoneIndexByName(train, boneName)
    if boneIndex ~= -1 then
        return GetWorldPositionOfEntityBone(train, boneIndex)
    end
    return nil
end

-- Check for the nearest train seat
function GetNearestTrainSeat(coords, radius)
    local closestTrain = nil
    local closestSeat = nil
    local closestDistance = radius

    for _, train in pairs({metro, metro2, freight}) do
        if train and DoesEntityExist(train) then
            for seatType, _ in pairs(seatBoneNames) do
                local seatCoords = GetSeatNodePosition(train, seatType)
                if seatCoords then
                    local distance = #(coords - seatCoords)
                    if distance <= closestDistance then
                        closestTrain = train
                        closestSeat = seatType
                        closestDistance = distance
                    end
                end
            end
        end
    end

    return closestTrain, closestSeat
end

-- Main loop for interaction
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)

        local playerPed = PlayerPedId()
        local playerCoords = GetEntityCoords(playerPed)

        if not isInSeat then
            local seatType = nil
            nearestTrain, seatType = GetNearestTrainSeat(playerCoords, 2.5) -- Check within 2.5 meters of seats
            if nearestTrain and seatType then
                local seatMessage = seatType == "driver" and "Press ~INPUT_ENTER~ to enter the driver's seat."
                    or "Press ~INPUT_ENTER~ to enter the front passenger seat."
                DisplayHelpText(seatMessage)

                if IsControlJustPressed(1, 23) then -- F key
                    local seatIndex = seatType == "driver" and -1 or 0
                    EnterSeat(nearestTrain, seatIndex)
                end
            end
        else
            DisplayHelpText("Press ~INPUT_ENTER~ to exit the seat.")
            if IsControlJustPressed(1, 23) then -- F key
                ExitSeat()
            end
        end
    end
end)

-- Display help text
function DisplayHelpText(text)
    BeginTextCommandDisplayHelp("STRING")
    AddTextComponentSubstringPlayerName(text)
    EndTextCommandDisplayHelp(0, false, true, -1)
end

RegisterNetEvent("k_trains:checkAndDeleteTrains")
AddEventHandler("k_trains:checkAndDeleteTrains", function()
    -- Check and delete freight train
    if freight ~= nil and DoesEntityExist(freight) then
        if not NetworkHasControlOfEntity(freight) then
            NetworkRequestControlOfEntity(freight)
            Citizen.Wait(500)
        end
        if NetworkHasControlOfEntity(freight) then
            print("Deleting orphaned freight train.")
            DeleteMissionTrain(freight)
            freight = nil
        end
    end

    -- Check and delete metro train
    if metro ~= nil and DoesEntityExist(metro) then
        if not NetworkHasControlOfEntity(metro) then
            NetworkRequestControlOfEntity(metro)
            Citizen.Wait(500)
        end
        if NetworkHasControlOfEntity(metro) then
            print("Deleting orphaned metro train.")
            DeleteMissionTrain(metro)
            metro = nil
        end
    end

    -- Check and delete metro2 train
    --[[if metro2 ~= nil and DoesEntityExist(metro2) then
        if not NetworkHasControlOfEntity(metro2) then
            NetworkRequestControlOfEntity(metro2)
            Citizen.Wait(500)
        end
        if NetworkHasControlOfEntity(metro2) then
            print("Deleting orphaned metro2 train.")
            DeleteMissionTrain(metro2)
            metro2 = nil
        end
    end]]--
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(100) 
        if freight then
            CheckAndStopTrain(freight, Config.FreightSpeed)
        end
        if metro then
            CheckAndStopTrain(metro, Config.MetroSpeed)
        end
        --[[if metro2 then
            CheckAndStopTrain(metro2, Config.MetroSpeed)
        end]]--
    end
end)

function CheckAndStopTrain(train, speed)
    if not DoesEntityExist(train) then return end

    local trainCoords = GetEntityCoords(train)
    local forwardVector = GetEntityForwardVector(train)
    local rayStart = trainCoords + forwardVector * 5.0 -- Start 5 meters ahead
    local rayEnd = trainCoords + forwardVector * 35.0 -- Check 35 meters ahead
    local rayHandle = StartShapeTestCapsule(rayStart.x, rayStart.y, rayStart.z, rayEnd.x, rayEnd.y, rayEnd.z, 5.0, 10, train, 7)
    local _, hit, _, _, entityHit = GetShapeTestResult(rayHandle)

    if hit and DoesEntityExist(entityHit) and IsEntityAVehicle(entityHit) then
        SetTrainCruiseSpeed(train, 0.0)
        Citizen.Wait(100)

        while true do
            trainCoords = GetEntityCoords(train)
            forwardVector = GetEntityForwardVector(train)
            rayStart = trainCoords + forwardVector * 5.0 -- Update start position
            rayEnd = trainCoords + forwardVector * 35.0 -- Update end position

            rayHandle = StartShapeTestCapsule(rayStart.x, rayStart.y, rayStart.z, rayEnd.x, rayEnd.y, rayEnd.z, 5.0, 10, train, 7)
            _, hit, _, _, entityHit = GetShapeTestResult(rayHandle)

            if not hit or not DoesEntityExist(entityHit) or not IsEntityAVehicle(entityHit) then
                break
            end
            Citizen.Wait(100)
        end

        SetTrainCruiseSpeed(train, speed)
    end
end

RegisterNetEvent("k_trains:clearTrainBlips")
AddEventHandler("k_trains:clearTrainBlips", function()
    if metroBlip then RemoveBlip(metroBlip) end
    if freightBlip then RemoveBlip(freightBlip) end
    if oldBlip then RemoveBlip(oldBlip) end
    if oldMetroBlip then RemoveBlip(oldMetroBlip) end
    if metro2Blip then RemoveBlip(metro2Blip) end
end)

local freightBlip, metroBlip, metro2Blip = nil, nil, nil

RegisterNetEvent("train:syncBlips")
AddEventHandler("train:syncBlips", function(blipPositions)

    -- Ensure `blipPositions` is initialized
if type(blipPositions) ~= "table" then
    blipPositions = {}
end

-- Update freight blip
if blipPositions.freight and type(blipPositions.freight) == "vector3" then
    if not freightBlip then
        -- Create the blip for freight
        freightBlip = AddBlipForCoord(blipPositions.freight.x, blipPositions.freight.y, blipPositions.freight.z)
        SetBlipSprite(freightBlip, 86) -- Train icon
        SetBlipColour(freightBlip, 47) -- Yellow
        SetBlipScale(freightBlip, 0.5)
        BeginTextCommandSetBlipName("STRING")
        AddTextComponentString("Freight Train")
        EndTextCommandSetBlipName(freightBlip)
    else
        -- Update the blip coordinates
        SetBlipCoords(freightBlip, blipPositions.freight.x, blipPositions.freight.y, blipPositions.freight.z)
    end
elseif freightBlip then
    -- Remove the blip if freight no longer exists
    RemoveBlip(freightBlip)
    freightBlip = nil
end


    -- Update metro blip
    if blipPositions.metro then
        if not metroBlip then
            metroBlip = AddBlipForCoord(blipPositions.metro.x, blipPositions.metro.y, blipPositions.metro.z)
            SetBlipSprite(metroBlip, 78)
            SetBlipColour(metroBlip, 2) -- Green
            SetBlipScale(metroBlip, 0.5)
            BeginTextCommandSetBlipName("STRING")
            AddTextComponentString("Metro Train")
            EndTextCommandSetBlipName(metroBlip)
        else
            SetBlipCoords(metroBlip, blipPositions.metro.x, blipPositions.metro.y, blipPositions.metro.z)
        end
    elseif metroBlip then
        RemoveBlip(metroBlip)
        metroBlip = nil
    end

    -- Update metro2 blip
    --[[if blipPositions.metro2 then
        if not metro2Blip then
            metro2Blip = AddBlipForCoord(blipPositions.metro2.x, blipPositions.metro2.y, blipPositions.metro2.z)
            SetBlipSprite(metro2Blip, 78)
            SetBlipColour(metro2Blip, 2) -- Green
            SetBlipScale(metro2Blip, 0.5)
            BeginTextCommandSetBlipName("STRING")
            AddTextComponentString("Metro 2")
            EndTextCommandSetBlipName(metro2Blip)
        else
            SetBlipCoords(metro2Blip, blipPositions.metro2.x, blipPositions.metro2.y, blipPositions.metro2.z)
        end
    elseif metro2Blip then
        RemoveBlip(metro2Blip)
        metro2Blip = nil
    end]]--
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(1000) -- Update every second
        if freight and DoesEntityExist(freight) and metro and DoesEntityExist(metro) and metro2 and DoesEntityExist(metro2) then
            local freightPos = GetEntityCoords(freight)
            local metroPos = GetEntityCoords(metro)
            local metro2Pos = GetEntityCoords(metro2)
            
            -- Trigger server event to update blips
            TriggerServerEvent("train:updateBlips", freightPos, metroPos, metro2Pos)
        end
    end
end)

RegisterNetEvent("train:removeBlips")
AddEventHandler("train:removeBlips", function()
    -- Remove existing blips
    if freightBlip then 
        RemoveBlip(freightBlip) 
        freightBlip = nil 
    end
    if metroBlip then 
        RemoveBlip(metroBlip) 
        metroBlip = nil 
    end
    if metro2Blip then 
        RemoveBlip(metro2Blip) 
        metro2Blip = nil 
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(1000) -- Update every second
        if freight and DoesEntityExist(freight) then
            local freightPos = GetEntityCoords(freight)
            TriggerServerEvent("train:updateBlipPosition", "freight", freightPos)
        end
        if metro and DoesEntityExist(metro) then
            local metroPos = GetEntityCoords(metro)
            TriggerServerEvent("train:updateBlipPosition", "metro", metroPos)
        end
        --[[if metro2 and DoesEntityExist(metro2) then
            local metro2Pos = GetEntityCoords(metro2)
            TriggerServerEvent("train:updateBlipPosition", "metro2", metro2Pos)
        end]]--
    end
end)

local currentMetroSpeed = 0.0
local currentFreightSpeed = 0.0
local maxMetroSpeed = Config.MetroSpeed or 50.0
local maxFreightSpeed = Config.FreightSpeed or 80.0

RegisterNetEvent("k_trains:accelerateMetro")
AddEventHandler("k_trains:accelerateMetro", function()
    if DoesEntityExist(metro) then
        currentMetroSpeed = math.min(currentMetroSpeed + 1.0, maxMetroSpeed)
        SetTrainCruiseSpeed(metro, currentMetroSpeed)
    end
end)

RegisterNetEvent("k_trains:brakeMetro")
AddEventHandler("k_trains:brakeMetro", function()
    if DoesEntityExist(metro) then
        currentMetroSpeed = math.max(currentMetroSpeed - 2.0, 0.0)
        SetTrainCruiseSpeed(metro, currentMetroSpeed)
    end
end)

RegisterNetEvent("k_trains:reverseMetro")
AddEventHandler("k_trains:reverseMetro", function()
    if DoesEntityExist(metro) then
        currentMetroSpeed = math.max(currentMetroSpeed - 1.0, -maxMetroSpeed / 2) -- Half speed for reverse
        SetTrainCruiseSpeed(metro, currentMetroSpeed)
    end
end)

RegisterNetEvent("k_trains:accelerateFreight")
AddEventHandler("k_trains:accelerateFreight", function()
    if DoesEntityExist(freight) then
        currentFreightSpeed = math.min(currentFreightSpeed + 1.0, maxFreightSpeed)
        SetTrainCruiseSpeed(freight, currentFreightSpeed)
    end
end)

RegisterNetEvent("k_trains:brakeFreight")
AddEventHandler("k_trains:brakeFreight", function()
    if DoesEntityExist(freight) then
        currentFreightSpeed = math.max(currentFreightSpeed - 2.0, 0.0)
        SetTrainCruiseSpeed(freight, currentFreightSpeed)
    end
end)

RegisterNetEvent("k_trains:reverseFreight")
AddEventHandler("k_trains:reverseFreight", function()
    if DoesEntityExist(freight) then
        currentFreightSpeed = math.max(currentFreightSpeed - 1.0, -maxFreightSpeed / 2) -- Half speed for reverse
        SetTrainCruiseSpeed(freight, currentFreightSpeed)
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        local playerPed = PlayerPedId()

        -- 🚄 Control Metro
        if IsPedInVehicle(playerPed, metro, false) then
            if GetPedInVehicleSeat(metro, -1) == playerPed then
                if IsControlPressed(0, 32) then -- W key
                    TriggerEvent("k_trains:accelerateMetro")
                end
                if IsControlPressed(0, 33) then -- S key
                    TriggerEvent("k_trains:brakeMetro")
                end
                if IsControlPressed(0, 45) then -- R key
                    TriggerEvent("k_trains:reverseMetro")
                end
            end
        end

        if IsPedInVehicle(playerPed, freight, false) then
            if GetPedInVehicleSeat(freight, -1) == playerPed then
                if IsControlPressed(0, 32) then -- W key
                    TriggerEvent("k_trains:accelerateFreight")
                end
                if IsControlPressed(0, 33) then -- S key
                    TriggerEvent("k_trains:brakeFreight")
                end
                if IsControlPressed(0, 45) then -- R key
                    TriggerEvent("k_trains:reverseFreight")
                end
            end
        end
    end
end)

local areDoorsOpen = false -- Tracks door state

RegisterNetEvent("k_trains:toggleDoors")
AddEventHandler("k_trains:toggleDoors", function(train)
    if DoesEntityExist(train) then
        if areDoorsOpen then
            CloseTrainDoors(train)
        else
            OpenTrainDoors(train)
        end
        areDoorsOpen = not areDoorsOpen
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        local playerPed = PlayerPedId()

        -- Control Metro
        if IsPedInVehicle(playerPed, metro, false) then
            if GetPedInVehicleSeat(metro, -1) == playerPed then
                if IsControlJustPressed(0, 73) then -- X key
                    TriggerEvent("k_trains:toggleDoors", metro)
                end
            end
        end

        -- Control Freight
        if IsPedInVehicle(playerPed, freight, false) then
            if GetPedInVehicleSeat(freight, -1) == playerPed then
                if IsControlJustPressed(0, 73) then -- X key
                    TriggerEvent("k_trains:toggleDoors", freight)
                end
            end
        end
    end
end)
