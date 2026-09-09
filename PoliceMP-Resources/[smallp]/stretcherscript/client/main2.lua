RegisterCommand("+helpnpc", function(source, args, rawCommand)
    local playerPed = PlayerPedId()
    local playerPos = GetEntityCoords(playerPed)
    local radius = 5.0

    local nearestNPC = GetClosestPed(playerPos.x, playerPos.y, playerPos.z, radius)

    if nearestNPC and DoesEntityExist(nearestNPC) then
        local isDead = IsEntityDead(nearestNPC)
        if not isDead then
            ExecuteCommand("e medic2")
            Citizen.Wait(50) 
            ExecuteCommand("e mechanic")
            ShowNotification("Civilian is alive!")
        end
        
        Citizen.Wait(1000)

        if isDead then
            ExecuteCommand("e medic2")
            Citizen.Wait(5000)
            ShowNotification("This civilian is dead...")
            ExecuteCommand("e mechanic")
            Citizen.Wait(5000)
            ShowNotification("Putting corpse in bodybag...")
            Citizen.Wait(5000)
            local pos = GetEntityCoords(nearestNPC)
            local bodyBag = CreateObject(GetHashKey("xm_prop_body_bag"), pos.x, pos.y, pos.z, true, true, true)

            DeleteEntity(nearestNPC)
            PlaceObjectOnGroundProperly(bodyBag)
            ClearPedTasksImmediately(playerPed)
        else
            ShowNotification("Civilian is alive! Resuscitate!")
            Citizen.Wait(3000)
            ClearPedTasksImmediately(playerPed)
        end
    else
        ShowNotification("No one nearby.")
        Citizen.Wait(3000) 
    end
end, false)

function ShowNotification(text)
    SetNotificationTextEntry("STRING")
    AddTextComponentSubstringPlayerName(text)
    DrawNotification(false, false)
end

function GetClosestPed(x, y, z, radius)
    local pedHandle, ped = FindFirstPed()
    local closestPed = nil
    local closestDist = radius

    repeat
        if DoesEntityExist(ped) and not IsPedAPlayer(ped) then
            local pedPos = GetEntityCoords(ped)
            local distance = #(vector3(x, y, z) - pedPos)
            if distance < closestDist then
                closestDist = distance
                closestPed = ped
            end
        end
        found, ped = FindNextPed(pedHandle)
    until not found

    EndFindPed(pedHandle)
    return closestPed
end

RegisterCommand("+pickbodybag", function(source, args, rawCommand)
    local ped = PlayerPedId()
    local coords = GetEntityCoords(ped)

    local closestVehicle = GetClosestVehicle(coords, 3.0, GetHashKey("stretcher"), 70)

    if DoesEntityExist(closestVehicle) then
        local bodyBagModel = GetHashKey("xm_prop_body_bag")
        local bodyBag = GetClosestObjectOfType(coords, 3.0, bodyBagModel, false, false, false)

        if DoesEntityExist(bodyBag) then
            AttachEntityToEntity(
                bodyBag,                          
                closestVehicle,                   
                GetEntityBoneIndexByName(closestVehicle, "seat_dside_f"),  
                0.0, -0.3, 0.5,                    
                0.0, 0.0, 180.0,                  
                true, true, false, false, 2, true 
            )
            
            ShowNotification("Bodybag Collected!")
        else
            ShowNotification("No Bodybag Found!")
        end
    else
        ShowNotification("No Stretcher Nearby!")
    end
end, false)

-- sent bodybag to morgue

Citizen.CreateThread(function()
    local markerPos = vector3(248.71, -1359.96, 30.55) -- Marker position
    local bodyBagModel = GetHashKey("xm_prop_body_bag") -- Prop model to check for

    while true do
        Citizen.Wait(0)

        local playerPed = PlayerPedId()
        local playerCoords = GetEntityCoords(playerPed)

        DrawMarker(1, markerPos.x, markerPos.y, markerPos.z - 1, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 1.0, 1.0, 0, 0, 0, 255, false, true, 2, false, false, false, false)
        local dist = Vdist(playerCoords.x, playerCoords.y, playerCoords.z, markerPos.x, markerPos.y, markerPos.z)
        
        if dist < 2.0 then
            DisplayHelpText("Press ~r~E~s~ to send bodybag to morgue")

            if IsControlJustPressed(0, 46) then -- 'E' key is 46
                local foundProp = false
                local propCoords = nil

                for _, prop in pairs(GetAllObjects()) do
                    if GetEntityModel(prop) == bodyBagModel then
                        local propPos = GetEntityCoords(prop)
                        if Vdist(playerCoords.x, playerCoords.y, playerCoords.z, propPos.x, propPos.y, propPos.z) < 3.0 then
                            foundProp = true
                            propCoords = propPos
                            break
                        end
                    end
                end

                if foundProp then
                    TriggerServerEvent("validateAndDeleteBodyBag", propCoords)
                else
                    ShowNotification("No bodybag found on stretcher!")
                end
            end
        end
    end
end)

RegisterNetEvent("deleteBodyBag")
AddEventHandler("deleteBodyBag", function(propCoords)
    for _, prop in pairs(GetAllObjects()) do
        if GetEntityModel(prop) == GetHashKey("xm_prop_body_bag") then
            local propPos = GetEntityCoords(prop)
            if Vdist(propCoords.x, propCoords.y, propCoords.z, propPos.x, propPos.y, propPos.z) < 1.0 then
                DeleteEntity(prop)
                ShowNotification("Bodybag sent to the morgue!")
                break
            end
        end
    end
end)

RegisterNetEvent("permissionDenied")
AddEventHandler("permissionDenied", function()
    ShowNotification("You do not have permission to perform this action.")
end)

function DisplayHelpText(text)
    SetTextComponentFormat("STRING")
    AddTextComponentString(text)
    DisplayHelpTextFromStringLabel(0, false, true, -1)
end

function GetAllObjects()
    local objects = {}
    local handle, object = FindFirstObject()
    local success
    repeat
        table.insert(objects, object)
        success, object = FindNextObject(handle)
    until not success
    EndFindObject(handle)
    return objects
end
