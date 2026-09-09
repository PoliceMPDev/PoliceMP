local FUEL_DECOR = "_ANDY_FUEL_DECORE_"
local nozzleDropped = false
local holdingNozzle = false
local nozzleInVehicle = false
local nozzle
local rope
local vehicleFueling
local usedPump
local pumpCoords
local wastingFuel = false
local usingCan = false
local nearTank = false

-- Nozzle Z position based on vehicle class.
local nozzleBasedOnClass = {
    0.65, -- Compacts
    0.65, -- Sedans
    0.85, -- SUVs
    0.6, -- Coupes
    0.55, -- Muscle
    0.6, -- Sports Classics
    0.6, -- Sports
    0.55, -- Super
    0.12, -- Motorcycles
    0.8, -- Off-road
    0.7, -- Industrial
    0.6, -- Utility
    0.7, -- Vans
    0.0, -- Cycles
    0.0, -- Boats
    0.0, -- Helicopters
    0.0, -- Planes
    0.6, -- Service
    0.65, -- Emergency
    0.65, -- Military
    0.75, -- Commercial
    0.0 -- Trains
}

-- Setting the electric vehicle config keys to hashes.
for _, vehHash in pairs(config.electricVehicles) do
    config.electricVehicles[vehHash] = vehHash
end

-- Get the fuel of a vehicle, which is set to an entity.
function GetFuel(vehicle)
    if not DecorExistOn(vehicle, FUEL_DECOR) then
        return GetVehicleFuelLevel(vehicle)
    end
	return DecorGetFloat(vehicle, FUEL_DECOR)
end

-- returns pump position if a player is near it.
function nearPump(coords)
    local entity = nil
    for hash in pairs(config.pumpModels) do
        entity = GetClosestObjectOfType(coords.x, coords.y, coords.z, 0.8, hash, true, true, true)
        if entity ~= 0 then 
           -- print("Found entity:", entity, "Hash:", GetEntityModel(entity)) -- Debug log
            break 
        end
    end
    if entity and entity ~= 0 then
        --print("Entity detected with hash:", GetEntityModel(entity)) -- Debug log
    else
        --print("No entity detected at location") -- Debug log
    end
    if config.pumpModels[GetEntityModel(entity)] then
        return GetEntityCoords(entity), entity
    end
end

-- Draws 3D text on coords.
function DrawText3D(x, y, z, text)
    local onScreen, _x, _y = World3dToScreen2d(x, y, z)
    local pX, pY, pZ = table.unpack(GetGameplayCamCoords())
    SetTextScale(0.4, 0.4)
    SetTextFont(4)
    SetTextProportional(1)
    SetTextEntry("STRING")
    SetTextCentre(true)
    SetTextColour(255, 255, 255, 255)
    SetTextOutline()
    AddTextComponentString(text)
    DrawText(_x, _y)
end

-- used to load the filling manually animation.
function LoadAnimDict(dict)
	if not HasAnimDictLoaded(dict) then
		RequestAnimDict(dict)
		while not HasAnimDictLoaded(dict) do
			Wait(1)
		end
	end
end

function PlayUISound(sound)
    SendNUIMessage({
        Type = "playSound",
        File = sound
    })
end

-- Used to play the effect of pouring fuel from the nozzle.
function PlayEffect(pdict, pname)
    CreateThread(function()
        local position = GetOffsetFromEntityInWorldCoords(nozzle, 0.0, 0.28, 0.17)
        UseParticleFxAssetNextCall(pdict)
        local pfx = StartParticleFxLoopedAtCoord(pname, position.x, position.y, position.z, 0.0, 0.0, GetEntityHeading(nozzle), 1.0, false, false, false, false)
        Wait(100)
        StopParticleFxLooped(pfx, 0)
    end)
end

function vehicleInFront()
    local entity = nil
    local offset = GetOffsetFromEntityInWorldCoords(ped, 0.0, 2.0, 0.0)
    local rayHandle = CastRayPointToPoint(pedCoords.x, pedCoords.y, pedCoords.z - 1.3, offset.x, offset.y, offset.z, 10, ped, 0)
    local _, _, _, _, entity = GetRaycastResult(rayHandle)

    if IsEntityAVehicle(entity) then
        local vehModel = GetEntityModel(entity)
        local vehCoords = GetEntityCoords(entity) -- Get vehicle's coordinates

        -- Check if the vehicle is electric
        if config.electricVehicles[vehModel] then
            -- Show error message above the electric vehicle
            CreateThread(function()
                local startTime = GetGameTimer()
                while GetGameTimer() - startTime < 5000 do -- Display for 5 seconds
                    DrawText3D(vehCoords.x, vehCoords.y, vehCoords.z + 1.2, "~r~This is an electric vehicle!")
                    Wait(0)
                end
            end)
            return nil -- Block interaction
        end
        
        return entity -- Return vehicle if it's NOT electric
    end
end

-- Create nozzle, rope and attach them to the player.
function grabNozzleFromPump()
    LoadAnimDict("anim@am_hold_up@male")
    TaskPlayAnim(ped, "anim@am_hold_up@male", "shoplift_high", 2.0, 8.0, -1, 50, 0, 0, 0, 0)
    Wait(300)

    PlayUISound("pickup")

    nozzle = CreateObject(`prop_cs_fuel_nozle`, 0, 0, 0, true, true, true)
    AttachEntityToEntity(nozzle, ped, GetPedBoneIndex(ped, 0x49D9), 0.11, 0.02, 0.02, -80.0, -90.0, 15.0, true, true, false, true, 1, true)
    RopeLoadTextures()
    while not RopeAreTexturesLoaded() do
        Wait(0)
    end
    RopeLoadTextures()
    while not pump do
        Wait(0)
    end
    rope = AddRope(pump.x, pump.y, pump.z, 0.0, 0.0, 0.0, 3.0, 1, 1000.0, 0.0, 1.0, false, false, false, 1.0, true)
    while not rope do
        Wait(0)
    end
    ActivatePhysics(rope)
    Wait(50)
    local nozzlePos = GetEntityCoords(nozzle)
    nozzlePos = GetOffsetFromEntityInWorldCoords(nozzle, 0.0, -0.033, -0.195)
    AttachEntitiesToRope(rope, pumpHandle, nozzle, pump.x, pump.y, pump.z + 1.45, nozzlePos.x, nozzlePos.y, nozzlePos.z, 5.0, false, false, nil, nil)
    nozzleDropped = false
    holdingNozzle = true
    nozzleInVehicle = false
    vehicleFueling = false
    usedPump = pumpHandle
end

-- attach the nozzle to the player.
function grabExistingNozzle()
    AttachEntityToEntity(nozzle, ped, GetPedBoneIndex(ped, 0x49D9), 0.11, 0.02, 0.02, -80.0, -90.0, 15.0, true, true, false, true, 1, true)
    PlayUISound("unmount")
    nozzleDropped = false
    holdingNozzle = true
    nozzleInVehicle = false
    vehicleFueling = false
end

-- attach nozzle to vehicle.
function putNozzleInVehicle(vehicle, ptankBone, isBike, dontClear, newTankPosition)
    TriggerServerEvent("forcePassengerExit", NetworkGetNetworkIdFromEntity(vehicle))
    Wait(500)
    if config.electricVehicles[GetEntityModel(vehicle)] then
        local vehicleCoords = GetEntityCoords(vehicle)
        return
    end

    if isBike then
        AttachEntityToEntity(nozzle, vehicle, ptankBone, 0.0 + newTankPosition.x, -0.2 + newTankPosition.y, 0.2 + newTankPosition.z, -80.0, 0.0, 0.0, true, true, false, false, 1, true)
    else
        AttachEntityToEntity(nozzle, vehicle, ptankBone, -0.25 + newTankPosition.x, -0.20 + newTankPosition.y, 0.55 + newTankPosition.z, -125.0, -90.0, -90.0, true, true, false, false, 1, true)
    end
    if not dontClear and IsEntityPlayingAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 3) then
        ClearPedTasks(ped)
    end
    PlayUISound("place")
    nozzleDropped = false
    holdingNozzle = false
    nozzleInVehicle = true
    wastingFuel = false
    vehicleFueling = vehicle
end

-- detach nozzle from everything and hide ui.
function dropNozzle()
    DetachEntity(nozzle, true, true)
    nozzleDropped = true
    holdingNozzle = false
    nozzleInVehicle = false
    vehicleFueling = false
end

-- delete nozzle and rope
function returnNozzleToPump()
    DeleteEntity(nozzle)
    PlayUISound("place")
    RopeUnloadTextures()
    DeleteRope(rope)
    nozzleDropped = false
    holdingNozzle = false
    nozzleInVehicle = false
    vehicleFueling = false
end

-- Get important information.
CreateThread(function()
    while true do
        ped = PlayerPedId()
        pedCoords = GetEntityCoords(ped)
        pump, pumpHandle = nearPump(pedCoords)
        veh = GetVehiclePedIsIn(ped, true)
        Wait(500)
    end
end)

-- Draws 3D text on coords.
function DrawText3D(x, y, z, text)
    local onScreen, _x, _y = World3dToScreen2d(x, y, z)
    local pX, pY, pZ = table.unpack(GetGameplayCamCoords())
    SetTextScale(0.4, 0.4)
    SetTextFont(4)
    SetTextProportional(1)
    SetTextEntry("STRING")
    SetTextCentre(true)
    SetTextColour(255, 255, 255, 255)
    SetTextOutline()
    AddTextComponentString(text)
    DrawText(_x, _y)
end

local function vehicleIsFueling()
    if config.electricVehicles[GetEntityModel(vehicleFueling)] then
        local vehicleCoords = GetEntityCoords(vehicleFueling)     
        vehicleFueling = false
        return
    end

    local classMultiplier = config.vehicleClasses[GetVehicleClass(vehicleFueling)]
    
    while vehicleFueling do
        local fuel = GetFuel(vehicleFueling)
        if not DoesEntityExist(vehicleFueling) then
            dropNozzle()
            vehicleFueling = false
            break
        end
        
        if not NetworkHasControlOfEntity(vehicleFueling) then
            NetworkRequestControlOfEntity(vehicleFueling)
            Wait(100)
        end

        fuel = fuel + classMultiplier * 25
        if fuel >= 100 then
            fuel = 100.0
            vehicleFueling = false
        end

        SetVehicleFuelLevel(vehicleFueling, fuel)
        --DecorSetFloat(vehicleFueling, "_ANDY_FUEL_DECORE_", fuel) 
        
        Wait(600)
    end
end

-- Separate thread to continuously draw fuel text while refueling
CreateThread(function()
    while true do
        Wait(0) -- Runs every frame for smooth text rendering
        if vehicleFueling then
            local vehicleCoords = GetEntityCoords(vehicleFueling)
            DrawText3D(vehicleCoords.x, vehicleCoords.y, vehicleCoords.z + 1.2, string.format("Fuel: %.1f%%", GetFuel(vehicleFueling)))
        end
    end
end)

-- Refuel the vehicle.
CreateThread(function()
    while true do
        Wait(2000)
        if vehicleFueling then
            vehicleIsFueling()
        end
    end
end)

-- Grabbing and returning the nozzle from the pump.
CreateThread(function()
    local wait = 500
    while true do
        Wait(wait)
        if pump then
            wait = 0
            if not holdingNozzle and not nozzleInVehicle and not nozzleDropped then
                DrawText3D(pump.x, pump.y, pump.z + 1.2, "[E] Grab Nozzle")
                if IsControlJustPressed(0, 51) then
                    grabNozzleFromPump()
                    Wait(1000)
                    ClearPedTasks(ped)
                end
            elseif holdingNozzle and not nearTank and pumpHandle == usedPump then
                DrawText3D(pump.x, pump.y, pump.z + 1.2, "[E] Return Nozzle")
                if IsControlJustPressed(0, 51) then
                    LoadAnimDict("anim@am_hold_up@male")
                    TaskPlayAnim(ped, "anim@am_hold_up@male", "shoplift_high", 2.0, 8.0, -1, 50, 0, 0, 0, 0)
                    Wait(300)
                    returnNozzleToPump()
                    Wait(1000)
                    ClearPedTasks(ped)
                end
            end
        else
            wait = 500
        end
    end
end)

-- Attaching and taking the nozzle form the vehicle, and dropping the nozzle form the player or vehicle.
CreateThread(function()
    local wait = 500
    while true do
        Wait(wait)
        if holdingNozzle or nozzleInVehicle or nozzleDropped then
            wait = 0

            -- drop the nozzle and remove it if it's far away from the pump.
            if pump then
                pumpCoords = GetEntityCoords(usedPump)
            end
            if nozzle and pumpCoords then
                nozzleLocation = GetEntityCoords(nozzle)
                if #(pumpCoords - pedCoords) < 3.0 then
                    
                else
                    
                end
                if #(nozzleLocation - pumpCoords) > 6.0 then
                    dropNozzle()
                elseif #(pumpCoords - pedCoords) > 100.0 then
                    returnNozzleToPump()
                end
                if nozzleDropped and #(nozzleLocation - pedCoords) < 1.5 then
                    DrawText3D(nozzleLocation.x, nozzleLocation.y, nozzleLocation.z, "[E] Pickup Nozzle")
                    if IsControlJustPressed(0, 51) then
                        LoadAnimDict("anim@mp_snowball")
                        TaskPlayAnim(ped, "anim@mp_snowball", "pickup_snowball", 2.0, 8.0, -1, 50, 0, 0, 0, 0)
                        Wait(700)
                        grabExistingNozzle()
                        ClearPedTasks(ped)
                    end
                end
            end

            local veh = vehicleInFront()

            -- Animations for manually fueling and effect for sparying fuel.
            if holdingNozzle and nozzle then
                DisableControlAction(0, 25, true)
                DisableControlAction(0, 24, true)
                if IsDisabledControlPressed(0, 24) then
                    if veh and tankPosition and #(pedCoords - tankPosition) < 3.0 then
                        if not IsEntityPlayingAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 3) then
                            LoadAnimDict("timetable@gardener@filling_can")
                            TaskPlayAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 2.0, 8.0, -1, 50, 0, 0, 0, 0)
                        end
                        wastingFuel = false
                        vehicleFueling = veh
                    else
                        if IsEntityPlayingAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 3) then
                            vehicleFueling = false
                            ClearPedTasks(ped)
                        end
                        if nozzleLocation then
                            wastingFuel = true
                            PlayEffect("core", "veh_trailer_petrol_spray")
                        end
                    end
                else
                    if IsEntityPlayingAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 3) then
                        vehicleFueling = false
                        ClearPedTasks(ped)
                    end
                    wastingFuel = false
                end
            end

            -- attaching and taking the nozzle from the vehicle.
            if veh then
                local model = GetEntityModel(veh)
                local vehName = GetDisplayNameFromVehicleModel(model)
                local vehClass = GetVehicleClass(veh)
            
                --print(('[DEBUG] Vehicle: %s (Hash: %s), Class: %s'):format(vehName, model, vehClass))
            
                local zPos = nozzleBasedOnClass[vehClass + 1] or 0.0
                local isBike = false
                local nozzleModifiedPosition = { x = 0.0, y = 0.0, z = 0.0 }
                local textModifiedPosition = { x = 0.0, y = 0.0, z = 0.0 }
            
                -- Manual overrides for bikes that are classed as emergency
                local bikeOverrides = {
                    [GetHashKey("addpoloffroadbike")] = true,
                    [GetHashKey("addpoloffroadbike1")] = true,
                    [GetHashKey("addpolbmwbike")] = true,
                    [GetHashKey("addpolpolicebumk")] = true,
                    [GetHashKey("addpolpolicebmk")] = true
                }               
            
                -- Helper to get closest bone and name
                local function getClosestTankBone(vehicle, boneNames)
                    local vehPos = GetEntityCoords(vehicle)
                
                    -- Priority: if petrolcap is found, use it
                    local petrolcapIndex = GetEntityBoneIndexByName(vehicle, "petrolcap")
                    if petrolcapIndex ~= -1 then
                        --print("[DEBUG] Forcing use of petrolcap bone")
                        return petrolcapIndex, "petrolcap"
                    end
                
                    -- Otherwise, find closest
                    local closestBone = -1
                    local closestName = nil
                    local minDistance = 1000.0
                
                    for _, bone in ipairs(boneNames) do
                        local boneIndex = GetEntityBoneIndexByName(vehicle, bone)
                        if boneIndex ~= -1 then
                            local boneWorld = GetWorldPositionOfEntityBone(vehicle, boneIndex)
                            local dist = #(boneWorld - vehPos)
                            if dist < minDistance then
                                closestBone = boneIndex
                                closestName = bone
                                minDistance = dist
                                --print(('[DEBUG] Bone %s found at distance %.2f'):format(bone, dist))
                            end
                        end
                    end
                
                    return closestBone, closestName
                end                
            
                local tankBone, tankBoneName
            
                if vehClass == 8 or bikeOverrides[model] then
                    isBike = true
                    tankBone, tankBoneName = getClosestTankBone(veh, {
                        "petrolcap", "petroltank", "engine"
                    })
            
                    -- Default bike offsets
                    nozzleModifiedPosition = { x = 0.0, y = 0.0, z = 0.0 }
                    textModifiedPosition = { x = 0.1, y = 0.05, z = 0.1 }
            
                    -- Special case: petrolcap on bikes
                    if tankBoneName == "petrolcap" then
                        nozzleModifiedPosition = { x = 0.0, y = 0.0, z = 0.0 }
                        --print("[DEBUG] Applied petrolcap nozzle offset for bike")
                    end
            
                elseif vehClass ~= 13 and not config.electricVehicles[model] then
                    tankBone, tankBoneName = getClosestTankBone(veh, {
                        "petrolcap", "petroltank_l", "hub_lr", "handle_dside_r", "seat_dside_r"
                    })
            
                    if tankBone ~= -1 then
                        if tankBoneName == "handle_dside_r" then
                            nozzleModifiedPosition = { x = 0.1, y = -0.5, z = -0.6 }
                            textModifiedPosition = { x = 0.55, y = 0.1, z = -0.2 }
                        elseif tankBoneName == "seat_dside_r" then
                            nozzleModifiedPosition = { x = -0.4, y = -0.5, z = -0.3 }
                            textModifiedPosition = { x = -0.4, y = -0.5, z = -0.3 }
                        elseif tankBoneName == "petrolcap" then
                            nozzleModifiedPosition = { x = 0.0, y = -0.2, z = -0.1 }
                            --print("[DEBUG] Applied petrolcap nozzle offset for vehicle")
                        end
                    end
                end
            
                if tankBone ~= -1 then
                    tankPosition = GetWorldPositionOfEntityBone(veh, tankBone)
                    if tankPosition and #(pedCoords - tankPosition) < 3.0 then
                        if not nozzleInVehicle and holdingNozzle then
                            nearTank = true
                            DrawText3D(
                                tankPosition.x + textModifiedPosition.x,
                                tankPosition.y + textModifiedPosition.y,
                                tankPosition.z + zPos + textModifiedPosition.z,
                                "[E] Attach Nozzle"
                            )
                            if IsControlJustPressed(0, 51) then
                                LoadAnimDict("timetable@gardener@filling_can")
                                TaskPlayAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 2.0, 8.0, -1, 50, 0, 0, 0, 0)
                                Wait(300)
                                putNozzleInVehicle(veh, tankBone, isBike, true, nozzleModifiedPosition)
                                Wait(300)
                                ClearPedTasks(ped)
                            end
                        elseif nozzleInVehicle then
                            DrawText3D(
                                tankPosition.x + textModifiedPosition.x,
                                tankPosition.y + textModifiedPosition.y,
                                tankPosition.z + zPos + textModifiedPosition.z,
                                "[E] Detach Nozzle"
                            )
                            if IsControlJustPressed(0, 51) then
                                LoadAnimDict("timetable@gardener@filling_can")
                                TaskPlayAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 2.0, 8.0, -1, 50, 0, 0, 0, 0)
                                Wait(300)
                                grabExistingNozzle()
                                Wait(300)
                                ClearPedTasks(ped)
                            end
                        end
                    end
                else
                    --print("[DEBUG] No valid tank bone found for this vehicle.")
                end
            else
                nearTank = false
            end                                   
        else
            wait = 500
        end
    end
end)

-- refueling using jerry can.
CreateThread(function()
    local wait = 500
    while true do
        Wait(wait)
        if GetSelectedPedWeapon(ped) == 883325847 and not holdingNozzle and not nozzleInVehicle then
            wait = 0
            local veh = vehicleInFront()
            if veh and not config.electricVehicles[GetEntityModel(veh)] then
                local vehClass = GetVehicleClass(veh)
                local zPos = nozzleBasedOnClass[vehClass + 1]
                local can = GetAmmoInPedWeapon(ped, 883325847)
                local distance = 1.2
                
                if vehClass == 8 and vehClass ~= 13 and not config.electricVehicles[GetHashKey(veh)] then
                    tankBone = GetEntityBoneIndexByName(veh, "petroltank")
                    if tankBone == -1 then
                        tankBone = GetEntityBoneIndexByName(veh, "engine")
                    end
                elseif vehClass == 14 and not config.electricVehicles[GetHashKey(veh)] then
                    tankBone = GetEntityBoneIndexByName(veh, "engine")
                    if tankBone == -1 then
                        tankBone = GetEntityBoneIndexByName(veh, "bodyshell")
                    else
                        distance = 2.0
                    end
                elseif vehClass ~= 13 and not config.electricVehicles[GetHashKey(veh)] then
                    tankBone = GetEntityBoneIndexByName(veh, "petroltank_l")
                    if tankBone == -1 then
                        tankBone = GetEntityBoneIndexByName(veh, "hub_lr")
                    end
                end
                tankPosition = GetWorldPositionOfEntityBone(veh, tankBone)
                if tankPosition and #(pedCoords - tankPosition) < distance then
                    local fuel = GetFuel(veh)
                    DrawText3D(tankPosition.x, tankPosition.y, tankPosition.z + zPos, math.floor(fuel) .. "% refuel [E]")
                    local ammo = GetAmmoInPedWeapon(ped, 883325847)
                    if IsControlPressed(0, 51) and ammo > 0 then
                        if not IsEntityPlayingAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 3) then
                            LoadAnimDict("timetable@gardener@filling_can")
                            TaskPlayAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 2.0, 8.0, -1, 50, 0, 0, 0, 0)
                        elseif can and DoesEntityExist(veh) then
                            SetPedAmmo(ped, 883325847, ammo - 3)
                            vehicleFueling = veh
                            usingCan = true
                        end
                    else
                        vehicleFueling = false
                        usingCan = false
                        if IsEntityPlayingAnim(ped, "timetable@gardener@filling_can", "gar_ig_5_filling_can", 3) then
                            ClearPedTasks(ped)
                        end
                    end
                end
            end
        else
            wait = 500
        end
    end
end)

-- spawn pumps on the map.
CreateThread(function()
    for _, pumps in pairs(config.addPumps) do
        local pumpObject = CreateObject(GetHashKey(pumps.hash), pumps.x, pumps.y, pumps.z - 1.0, true, true, true)

        if DoesEntityExist(pumpObject) then
            SetEntityHeading(pumpObject, pumps.heading) -- Set the heading
            FreezeEntityPosition(pumpObject, true) -- Optional: Prevents it from moving
            print("Spawned pump:", pumpObject, "Model Hash:", GetEntityModel(pumpObject), "Heading:", pumps.heading)
        else
            print("Failed to spawn pump at:", pumps.x, pumps.y, pumps.z)
        end
    end
end)

-- Register the fuel decor
CreateThread(function()
    DecorRegister(FUEL_DECOR, 1)
end)

RegisterNetEvent("notifyPlayer")
AddEventHandler("notifyPlayer", function(msg, color)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(msg)
    DrawNotification(false, true)
end)
