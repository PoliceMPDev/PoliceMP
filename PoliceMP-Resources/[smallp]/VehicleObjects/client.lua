local LPlates = {}
local PPlates = {}
local PenaltyCharges = {}
local SeizureAttachments = {}

local function SyncEntity(entity)
    local netId = ObjToNet(entity)
    SetNetworkIdExistsOnAllMachines(netId, true)
    SetNetworkIdCanMigrate(netId, false)
    NetworkSetNetworkIdDynamic(netId, true)
    FreezeEntityPosition(entity, true)
end

local function GetNearbyVehicle()
    local playerPed = PlayerPedId()
    local vehicle = GetVehiclePedIsIn(playerPed, false)

    if vehicle == 0 then
        vehicle = GetVehiclePedIsTryingToEnter(playerPed)
    end

    return vehicle
end

local function RequestPropModel(model)
    RequestModel(model)
    while not HasModelLoaded(model) do
        Wait(0)
    end
end

local function ReturnError(message)
    SetNotificationTextEntry("STRING")
    AddTextComponentString("~r~Error: ~w~" .. message)
    DrawNotification(false, true)
end

local function HasPlateAttached(vehicle)
    return LPlates[vehicle] or PPlates[vehicle] or PenaltyCharges[vehicle]
end

local function CleanupVehicleProps(vehicle)
    if LPlates[vehicle] then
        for _, obj in ipairs(LPlates[vehicle]) do
            if DoesEntityExist(obj) then
                DeleteEntity(obj)
            end
        end
        LPlates[vehicle] = nil
    end

    if PPlates[vehicle] then
        for _, obj in ipairs(PPlates[vehicle]) do
            if DoesEntityExist(obj) then
                DeleteEntity(obj)
            end
        end
        PPlates[vehicle] = nil
    end

    if PenaltyCharges[vehicle] then
        if DoesEntityExist(PenaltyCharges[vehicle]) then
            DeleteEntity(PenaltyCharges[vehicle])
        end
        PenaltyCharges[vehicle] = nil
    end

    if SeizureAttachments[vehicle] then
        for _, obj in ipairs(SeizureAttachments[vehicle]) do
            if DoesEntityExist(obj) then
                DeleteEntity(obj)
            end
        end
        SeizureAttachments[vehicle] = nil
    end
end

RegisterCommand('+xlplates', function()
    local vehicle = GetNearbyVehicle()
    if vehicle == 0 then
        ReturnError("You must inside a vehicle.")
        return
    end

    if LPlates[vehicle] then
        CleanupVehicleProps(vehicle)
        return
    end

    if HasPlateAttached(vehicle) then
        ReturnError("This vehicle already has a plate attached.")
        return
    end

    local hash = GetHashKey('prop_lplate')
    RequestPropModel(hash)
    local coords = GetEntityCoords(vehicle)

    local boneIndexFront = GetEntityBoneIndexByName(vehicle, 'windscreen')
    if boneIndexFront ~= -1 then
        local objFront = CreateObject(hash, coords.x, coords.y, coords.z, true, true, true)
        SyncEntity(objFront)
        AttachEntityToEntity(objFront, vehicle, boneIndexFront, 0.0, 0.0, -0.1, -25.0, 0.0, 180.0, true, true, false, true, 0, true)
        LPlates[vehicle] = LPlates[vehicle] or {}
        table.insert(LPlates[vehicle], objFront)
    end

    local boneIndexRear = GetEntityBoneIndexByName(vehicle, 'windscreen_r')
    if boneIndexRear ~= -1 then
        local objRear = CreateObject(hash, coords.x, coords.y, coords.z, true, true, true)
        SyncEntity(objRear)
        AttachEntityToEntity(objRear, vehicle, boneIndexRear, 0.0, 0.0, -0.1, -10.0, 0.0, 0.0, true, true, false, true, 0, true)
        table.insert(LPlates[vehicle], objRear)
    end
end, false)

RegisterCommand('+xpplates', function()
    local vehicle = GetNearbyVehicle()
    if vehicle == 0 then
        ReturnError("You must be inside a vehicle.")
        return
    end

    if PPlates[vehicle] then
        CleanupVehicleProps(vehicle)
        return
    end

    if HasPlateAttached(vehicle) then
        ReturnError("This vehicle already has a plate attached.")
        return
    end

    local hash = GetHashKey('prop_pplate')
    RequestPropModel(hash)
    local coords = GetEntityCoords(vehicle)

    local boneIndexFront = GetEntityBoneIndexByName(vehicle, 'windscreen')
    if boneIndexFront ~= -1 then
        local objFront = CreateObject(hash, coords.x, coords.y, coords.z, true, true, true)
        SyncEntity(objFront)
        AttachEntityToEntity(objFront, vehicle, boneIndexFront, 0.0, 0.0, -0.1, -25.0, 0.0, 180.0, true, true, false, true, 0, true)
        PPlates[vehicle] = PPlates[vehicle] or {}
        table.insert(PPlates[vehicle], objFront)
    end

    local boneIndexRear = GetEntityBoneIndexByName(vehicle, 'windscreen_r')
    if boneIndexRear ~= -1 then
        local objRear = CreateObject(hash, coords.x, coords.y, coords.z, true, true, true)
        SyncEntity(objRear)
        AttachEntityToEntity(objRear, vehicle, boneIndexRear, 0.0, 0.0, -0.1, -10.0, 0.0, 0.0, true, true, false, true, 0, true)
        table.insert(PPlates[vehicle], objRear)
    end
end, false)

RegisterCommand('+xpcharge', function()
    local vehicle = GetNearbyVehicle()
    if vehicle == 0 then
        ReturnError("You must be inside a vehicle.")
        return
    end

    if PenaltyCharges[vehicle] then
        CleanupVehicleProps(vehicle)
        return
    end

    if HasPlateAttached(vehicle) then
        ReturnError("This vehicle already has a plate attached.")
        return
    end

    local hash = GetHashKey('prop_penaltycharge')
    RequestPropModel(hash)
    local coords = GetEntityCoords(vehicle)
    local obj = CreateObject(hash, coords.x, coords.y, coords.z, true, true, true)
    SyncEntity(obj)
    AttachEntityToEntity(obj, vehicle, GetEntityBoneIndexByName(vehicle, 'windscreen'), 0.0, 0.10, -0.1, 30.0, 0.0, 0.0, true, true, false, true, 0, true)
    PenaltyCharges[vehicle] = obj
end, false)

CreateThread(function()
    while true do
        Wait(30)
        for vehicle, _ in pairs(LPlates) do
            if not DoesEntityExist(vehicle) then
                CleanupVehicleProps(vehicle)
            end
        end
        for vehicle, _ in pairs(PPlates) do
            if not DoesEntityExist(vehicle) then
                CleanupVehicleProps(vehicle)
            end
        end
        for vehicle, _ in pairs(PenaltyCharges) do
            if not DoesEntityExist(vehicle) then
                CleanupVehicleProps(vehicle)
            end
        end
        for vehicle, _ in pairs(SeizureAttachments) do
            if not DoesEntityExist(vehicle) then
                CleanupVehicleProps(vehicle)
            end
        end
    end
end)

RegisterCommand('+seize', function()
    local vehicle = GetNearbyVehicle()
    if vehicle == 0 then
        ReturnError("You must be inside a vehicle.")
        return
    end

    -- Check if the seizure attachment already exists
    if SeizureAttachments[vehicle] then
        CleanupVehicleProps(vehicle) -- Using CleanupVehicleProps instead of a separate function
        return
    end

    -- Check if the vehicle already has a plate attached
    if HasPlateAttached(vehicle) then
        ReturnError("This vehicle already has a seizure attachment.")
        return
    end

    local hash = GetHashKey('window') -- Window prop
    RequestPropModel(hash)
    local coords = GetEntityCoords(vehicle)

    -- Attach to the front windshield
    local boneIndexFront = GetEntityBoneIndexByName(vehicle, 'windscreen')
    if boneIndexFront ~= -1 then
        local objFront = CreateObject(hash, coords.x, coords.y, coords.z, true, true, true)
        SyncEntity(objFront)
        AttachEntityToEntity(objFront, vehicle, boneIndexFront, 0.0, -2.3, -0.6, 0.0, 0.0, 180.0, true, true, false, true, 0, true)
        
        SeizureAttachments[vehicle] = SeizureAttachments[vehicle] or {}
        table.insert(SeizureAttachments[vehicle], objFront)
    end
end, false)