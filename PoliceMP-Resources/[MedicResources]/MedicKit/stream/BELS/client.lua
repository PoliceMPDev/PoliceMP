----- Global Variables 

local spawnedProp = nil
local dropable = false
local selectedItem = 0

----- Commands

Citizen.CreateThread(function()
    for i = 1, #Commands do
        RegisterCommand(Commands[i].commandName, function()
            if spawnedProp ~= nil then return notify("You have already spawned a Prop, you must delete it first.") end
            toggleEUP(Commands[i].SlotID, Commands[i].ModelID, i)
        end)
    end
end)

RegisterKeyMapping("OG:ToggleProp", "Drop BELS Kit", "KEYBOARD", "e")
RegisterCommand("OG:ToggleProp", function()
    if dropable or spawnedProp ~= nil then return toggleProp() end 
end, false)


----- Toggling Functions

function toggleEUP(slotId, modelId, i)
    local Ped = PlayerPedId()
    local eup = GetPedDrawableVariation(Ped, slotId)

    local VehicleWhitelist = CheckVehicle(i)

    if VehicleWhitelist[1] ~= "Found" then return notify(VehicleWhitelist) end -- Stops Function if Vehicle is not found and replies with the relevant message.

    SetVehicleDoorOpen(VehicleWhitelist[2], 5, false, false)

    LoadAnim('clothingtie')
    TaskPlayAnim(Ped, 'clothingtie', 'try_tie_negative_a', 8.0, 1.0, -1, 2, 0, 0, 0, 0)
    Wait(1000)

    if eup == modelId then
        dropable = false
        selectedItem = 0
        SetPedComponentVariation(Ped, slotId, 0, 0, 0)
    else
        dropable = true
        selectedItem = i
        SetPedComponentVariation(Ped, slotId, modelId, 0, 0)
    end

	ClearPedTasksImmediately(Ped)
    SetVehicleDoorShut(VehicleWhitelist[2], 5, false)
end

function toggleProp()
    local Ped = PlayerPedId()
    
    LoadAnim('anim@mp_snowball')
    TaskPlayAnim(Ped, 'anim@mp_snowball', 'pickup_snowball', 8.0, 1.0, -1, 2, 0, 0, 0, 0)
    Wait(1200)
    ClearPedTasksImmediately(Ped)

    if spawnedProp ~= nil then -- Delete The Prop
        
        DeleteObject(spawnedProp)
        SetPedComponentVariation(Ped, Commands[selectedItem].SlotID, Commands[selectedItem].ModelID, 0, 0)
        
        spawnedProp = nil
        dropable = true

        alert("Press ~INPUT_WEAPON_SPECIAL_TWO~ to Drop the BELS Kit")

    else -- Spawn The prop

        RequestModel(Commands[selectedItem].prop)
        while not HasModelLoaded(Commands[selectedItem].prop) do
            Citizen.Wait(1)
        end

        local x,y,z = table.unpack(GetEntityCoords(PlayerPedId()))
        spawnedProp = CreateObject(GetHashKey(Commands[selectedItem].prop), x, y, z, true, false, false);
        PlaceObjectOnGroundProperly(spawnedProp)
        FreezeEntityPosition(spawnedProp, 1)
        SetEntityCollision(spawnedProp, true, true)
        SetPedComponentVariation(Ped, Commands[selectedItem].SlotID, 0, 0, 0)

        dropable = false
        alert("Press ~INPUT_WEAPON_SPECIAL_TWO~ to Pickup the BELS Kit")

    end
end

RegisterCommand("DeleteProp", function()
    DeleteObject(spawnedProp)
    spawnedProp = nil
end)

----- Other Functions

function LoadAnim(dict)
    while not HasAnimDictLoaded(dict) do
        RequestAnimDict(dict)
        Citizen.Wait(1)
    end
end

function CheckVehicle(i)
	local Vehicle = GetVehicleInFront()
	if Vehicle ~= 0 then 
        for k,v in pairs(Commands[i].AllowedVehicles) do
            local model = v
            local hash = GetHashKey(model)
            if IsVehicleModel(Vehicle, hash) and not hash_text then
                return {"Found", Vehicle}
            end
        end
        return "~r~Vehicle not found in config"
    end
    return "~r~You are not near a vehicle"
end

function GetVehicleInFront()
	local player = PlayerPedId()
    local pos = GetEntityCoords(player)
    local entityWorld = GetOffsetFromEntityInWorldCoords(player, 0.0, 2.0, 0.0)
    local rayHandle = CastRayPointToPoint(pos.x, pos.y, pos.z, entityWorld.x, entityWorld.y, entityWorld.z, 30, player, 0)
    local _, _, _, _, result = GetRaycastResult(rayHandle)
    return result
end


----- Notifications


function alert(msg)
    SetTextComponentFormat("STRING")
    AddTextComponentString(msg)
    DisplayHelpTextFromStringLabel(0,0,1,4000)
end

function notify(msg)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(msg)
    DrawNotification(true, false)
end