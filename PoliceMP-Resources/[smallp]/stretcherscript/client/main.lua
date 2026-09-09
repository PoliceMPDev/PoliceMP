local lit_1 = {
    {anim = "savecouch@",lib = "t_sleep_loop_couch",name = Config.Language.anim.lie_back, x = 0, y = 0.2, z = 1.1, r = 180.0},
	{anim = "amb@prop_human_seat_chair_food@male@base",lib = "base",name = Config.Language.anim.sit_right, x = 0.0, y = -0.2, z =0.55, r = -90.0},
	{anim = "amb@prop_human_seat_chair_food@male@base",lib = "base",name = Config.Language.anim.sit_left, x = 0.0, y = -0.2, z =0.55, r = 90.0},
	{anim = "amb@world_human_stupor@male_looking_left@base",lib = "base",name = Config.Language.anim.pls, x = 0.0, y = 0.3, z = 1.5, r = 180.0},
}

local lit = {
	{lit = "stretcher", distance_stop = 2.4, name = lit_1, title = Config.Language.lit_1}
}

prop_amb = false
veh_detect = 0

local stretcherHash = GetHashKey("stretcher") 
local closestVehicle = nil

Citizen.CreateThread(function()
    while true do
        local sleep = 2000    
        local pedCoords = GetEntityCoords(GetPlayerPed(-1))
        for _,i in pairs(lit) do
            local closestObject = GetClosestVehicle(pedCoords, 3.0, GetHashKey("stretcher"), 70)
        
            if DoesEntityExist(closestObject) then
                sleep = 5
                local propCoords = GetEntityCoords(closestObject)
                local propForward = GetEntityForwardVector(closestObject)
                local litCoords = (propCoords + propForward)
                local sitCoords = (propCoords + propForward * 0.1)
                local pickupCoords = (propCoords + propForward * 1.2)
                local pickupCoords2 = (propCoords + propForward * - 1.2)

                if GetDistanceBetweenCoords(pedCoords, litCoords, true) <= 5.0 then
                    if GetDistanceBetweenCoords(pedCoords, sitCoords, true) <= 2.0 then
                        if IsControlJustPressed(0, Config.Press.do_action) then
                            TriggerEvent('', '')
                        end
                    elseif IsEntityAttachedToEntity(closestObject, GetPlayerPed(-1)) == false and not IsEntityPlayingAnim(PlayerPedId(), 'anim@heists@box_carry@', 'idle', 3) then
                        if GetDistanceBetweenCoords(pedCoords, pickupCoords, true) <= 0.5 then
                            hintToDisplay(Config.Language.take_bed)
                            if IsControlJustPressed(0, Config.Press.take_bed) then
                                SetVehicleExtra(closestObject, 1, 0)
                                SetVehicleExtra(closestObject, 2, 1)
                                prop_exist = 0
                                prendre(closestObject)
                            end
                        end
                        
                        if GetDistanceBetweenCoords(pedCoords, pickupCoords2, true) <= 1.5 then
                            hintToDisplay(Config.Language.take_bed)
                            if IsControlJustPressed(0, Config.Press.take_bed) then
                                SetVehicleExtra(closestObject, 1, 0)
                                SetVehicleExtra(closestObject, 2, 1)
                                prop_exist = 0
                                prendre(closestObject)
                            end
                        end
                    end
                end

                -- Handling /commands for interaction instead of WarMenu
                -- Seat right
                RegisterCommand("sitright", function()
					local ped = GetPlayerPed(-1)
					local coords = GetEntityCoords(ped)
					local closestVehicle = GetClosestVehicle(coords, 5.0, GetHashKey("stretcher"), 70)
				
					if DoesEntityExist(closestVehicle) then
						local sit = lit_1[2]
						LoadAnim(sit.anim)
						AttachEntityToEntity(ped, closestVehicle, GetEntityBoneIndexByName(closestVehicle, "seat_dside_f"), sit.x, sit.y, sit.z, 0.0, 0.0, sit.r, false, false, false, false, 2, true)
						TaskPlayAnim(ped, sit.anim, sit.lib, 8.0, 8.0, -1, 1, 0, false, false, false)
					end
				end)
				
				RegisterCommand("sitleft", function()
					local ped = GetPlayerPed(-1)
					local coords = GetEntityCoords(ped)
					local closestVehicle = GetClosestVehicle(coords, 5.0, GetHashKey("stretcher"), 70)
				
					if DoesEntityExist(closestVehicle) then
						local sit = lit_1[3]
						LoadAnim(sit.anim)
						AttachEntityToEntity(ped, closestVehicle, GetEntityBoneIndexByName(closestVehicle, "seat_dside_f"), sit.x, sit.y, sit.z, 0.0, 0.0, sit.r, false, false, false, false, 2, true)
						TaskPlayAnim(ped, sit.anim, sit.lib, 8.0, 8.0, -1, 1, 0, false, false, false)
					end
				end)
				
				RegisterCommand("laybed", function()
					local ped = GetPlayerPed(-1)
					local coords = GetEntityCoords(ped)
					local closestVehicle = GetClosestVehicle(coords, 5.0, GetHashKey("stretcher"), 70)
				
					if DoesEntityExist(closestVehicle) then
						local lie = lit_1[1]
						LoadAnim(lie.anim)
						AttachEntityToEntity(ped, closestVehicle, GetEntityBoneIndexByName(closestVehicle, "seat_dside_f"), lie.x, lie.y, lie.z, 0.0, 0.0, lie.r, false, false, false, false, 2, true)
						TaskPlayAnim(ped, lie.anim, lie.lib, 8.0, 8.0, -1, 1, 0, false, false, false)
					end
				end)

				RegisterCommand("sitbed", function()
					local ped = GetPlayerPed(-1)
					local coords = GetEntityCoords(ped)
					local closestVehicle = GetClosestVehicle(coords, 5.0, GetHashKey("stretcher"), 70)
				
					if DoesEntityExist(closestVehicle) then
						local lie = lit_1[4]
						LoadAnim(lie.anim)
						AttachEntityToEntity(ped, closestVehicle, GetEntityBoneIndexByName(closestVehicle, "seat_dside_f"), lie.x, lie.y, lie.z, 0.0, 0.0, lie.r, false, false, false, false, 2, true)
						TaskPlayAnim(ped, lie.anim, lie.lib, 8.0, 8.0, -1, 1, 0, false, false, false)
					end
				end)

                -- Additional Commands
                RegisterCommand('togglebed', function()
                    if IsVehicleExtraTurnedOn(closestObject, 11) == false then
                        SetVehicleExtra(closestObject, 11, 0)
                        SetVehicleExtra(closestObject, 12, 1)
                    else
                        SetVehicleExtra(closestObject, 11, 1)
                        SetVehicleExtra(closestObject, 12, 0)
                    end
                end, false)

                RegisterCommand('outbed', function()
                    DetachEntity(GetPlayerPed(-1), true, true)
                    local x, y, z = table.unpack(GetEntityCoords(closestObject) + GetEntityForwardVector(closestObject) * -i.distance_stop)
                    SetEntityCoords(GetPlayerPed(-1), x, y, z)
                end, false)

                RegisterCommand('delstretcher', function()
                    local playerPed = PlayerPedId()
                    local playerPos = GetEntityCoords(playerPed)
                    local closestVehicle = GetClosestVehicle(playerPos.x, playerPos.y, playerPos.z, 5.0, 0, 70)
                
                    if DoesEntityExist(closestVehicle) then
                        local vehicleModel = GetEntityModel(closestVehicle)
                        if vehicleModel == stretcherHash then
                            if NetworkHasControlOfEntity(closestVehicle) then
                                DeleteEntity(closestVehicle)
                                closestVehicle = nil
                                hintToDisplay("Stretcher deleted.")
                            else
                                NetworkRequestControlOfEntity(closestVehicle)
                                while not NetworkHasControlOfEntity(closestVehicle) do
                                    Citizen.Wait(0)
                                end
                                DeleteEntity(closestVehicle)
                                closestVehicle = nil
                                hintToDisplay("Stretcher deleted.")
                            end
                        else
                            hintToDisplay("No stretcher to delete!")
                        end
                    else
                        hintToDisplay("No stretcher to delete!")
                    end
                end, false)
            end
        end
        Citizen.Wait(sleep)
    end
end)


Citizen.CreateThread(function()
	prop_exist = 0
	while true do
		for _,g in pairs(Config.Hash) do
			local closestObject = GetClosestVehicle(GetEntityCoords(GetPlayerPed(-1)), 7.0, GetHashKey(g.hash), 18)
			if closestObject ~= 0 then
				veh_detect = closestObject
				veh_detection = g.detection
				prop_depth = g.depth
				prop_height = g.height
			end
		end
		if prop_amb == false then
			if GetVehiclePedIsIn(GetPlayerPed(-1)) == 0 then
				if DoesEntityExist(veh_detect) then
					local coords = GetEntityCoords(veh_detect) + GetEntityForwardVector(veh_detect) * - veh_detection
					local coords_spawn = GetEntityCoords(veh_detect) + GetEntityForwardVector(veh_detect) * - (veh_detection + 4.0)
					if GetDistanceBetweenCoords(GetEntityCoords(GetPlayerPed(-1)), coords.x , coords.y, coords.z, true) <= 1.0 then
						if not IsEntityPlayingAnim(PlayerPedId(), 'anim@heists@box_carry@', 'idle', 3) then
							hintToDisplay(Config.Language.out_vehicle_bed)
							for _,m in pairs(lit) do
								local prop = GetClosestObjectOfType(GetEntityCoords(GetPlayerPed(-1)), 9.0, GetHashKey(m.lit))
								if prop ~= 0 then 
									prop_exist = prop
								end
							end
							if IsEntityAttachedToEntity(prop, GetPlayerPed(-1)) ~= 0 or prop ~= 0 then
								if IsControlJustPressed(0, Config.Press.out_vehicle_bed) then
									-- Trigger server-side event to check permission
									TriggerServerEvent('checkParamedicPermission', GetPlayerServerId(PlayerId()), coords_spawn)
								end
							end
						end
					end
				end
			end
		end
		Citizen.Wait(0)
	end
end)

RegisterNetEvent('spawnStretcher')
AddEventHandler('spawnStretcher', function(coords_spawn)
    while not HasModelLoaded("stretcher") do
        RequestModel("stretcher")
        Citizen.Wait(1)
    end
    local object = CreateVehicle(GetHashKey("stretcher"), coords_spawn, true, true)
    SetVehicleExtra(object, 1, 0)
    SetVehicleExtra(object, 2, 1)
    SetVehicleExtra(object, 12, 1)
    SetVehicleExtra(object, 11, 0)
    SetEntityHeading(GetPlayerPed(-1), GetEntityHeading(GetPlayerPed(-1)) - 180.0)
    SetVehicleEngineOn(object, 1, 1, 1)
    prendre(object, vehicle)
end)



function prendre(propObject, hash)
	NetworkRequestControlOfEntity(propObject)

	LoadAnim("anim@heists@box_carry@")

	AttachEntityToEntity(propObject, GetPlayerPed(-1), GetPlayerPed(-1), -0.05, 1.3, -0.345 , 180.0, 180.0, 180.0, 0.0, false, false, true, false, 2, true)
		---
	while IsEntityAttachedToEntity(propObject, GetPlayerPed(-1)) do

		Citizen.Wait(5)

		if not IsEntityPlayingAnim(PlayerPedId(), 'anim@heists@box_carry@', 'idle', 3) then
			TaskPlayAnim(PlayerPedId(), 'anim@heists@box_carry@', 'idle', 8.0, 8.0, -1, 50, 0, false, false, false)
		end

		if IsPedDeadOrDying(GetPlayerPed(-1)) then
			ClearPedTasksImmediately(GetPlayerPed(-1))
			SetVehicleExtra(propObject, 1, 1)
			SetVehicleExtra(propObject, 2, 0)
			DetachEntity(propObject, true, true)
		end
		if GetDistanceBetweenCoords(GetEntityCoords(GetPlayerPed(-1)), GetEntityCoords(veh_detect), true) <= 7.0 then
			hintToDisplay(Config.Language.in_vehicle_bed)
			if (IsControlJustPressed(0, Config.Press.in_vehicle_bed)) then
				ClearPedTasksImmediately(GetPlayerPed(-1))
				SetVehicleExtra(propObject, 1, 1)
				SetVehicleExtra(propObject, 2, 0)

				DetachEntity(propObject, true, true)
				prop_amb = true

				in_ambulance(propObject, veh_detect, prop_depth, prop_height)
			end
		else
			hintToDisplay(Config.Language.release_bed)
		end

		if IsControlJustPressed(0, Config.Press.release_bed) then
            local playerPed = PlayerPedId()
            local playerCoords = GetEntityCoords(playerPed)
        
            local nearbyVehicle = false
            local vehicles = GetGamePool("CVehicle") -- Gets all vehicles
        
            for _, vehicle in ipairs(vehicles) do
                local vehModel = GetEntityModel(vehicle)
        
                -- Check if it's NOT a stretcher
                if vehModel ~= GetHashKey("stretcher") then
                    local vehCoords = GetEntityCoords(vehicle)
                    local distance = #(playerCoords - vehCoords)
        
                    if distance < 4.0 then
                        nearbyVehicle = true
                        break
                    end
                end
            end
        
            if nearbyVehicle then
                hintToDisplay("You cannot drop this near a vehicle!")
            else
                -- Allow dropping
                ClearPedTasksImmediately(playerPed)
                SetVehicleExtra(propObject, 1, 1)
                SetVehicleExtra(propObject, 2, 0)
                DetachEntity(propObject, true, false)
                SetVehicleOnGroundProperly(propObject)
            end
        end 
    end
end

function in_ambulance(propObject, amb, depth, height)
	veh_detect = 0
	NetworkRequestControlOfEntity(amb)

	AttachEntityToEntity(propObject, amb, 0.0, 0.0, depth, height, 0.0, 0.0, 0.0, 0.0, false, false, true, false, 2, true)
	
	while IsEntityAttachedToEntity(propObject, amb) do
		Citizen.Wait(5)

		if GetVehiclePedIsIn(GetPlayerPed(-1)) == 0 then
			if GetDistanceBetweenCoords(GetEntityCoords(GetPlayerPed(-1)), GetEntityCoords(amb), true) <= 7.0 then
				hintToDisplay(Config.Language.out_vehicle_bed)
				if IsControlJustPressed(0, Config.Press.out_vehicle_bed) then
					DetachEntity(propObject, true, true)
					prop_amb = false
					SetEntityHeading(GetPlayerPed(-1), GetEntityHeading(GetPlayerPed(-1)) - 180.0)
					SetVehicleExtra(propObject, 1, 0)
					SetVehicleExtra(propObject, 2, 1)
					prendre(propObject)
				end
			end
		end
	end
end

function LoadAnim(dict)
	while not HasAnimDictLoaded(dict) do
		RequestAnimDict(dict)
		Citizen.Wait(1)
	end
end

function hintToDisplay(text)
    SetTextComponentFormat("STRING")
    AddTextComponentString(text)
    DisplayHelpTextFromStringLabel(0, 0, 1, -1)
end

function ShowNotification( text )
    SetNotificationTextEntry( "STRING" )
    AddTextComponentString( text )
    DrawNotification( false, false )
end

function DrawText3D(coords, text, size)

    local onScreen,_x,_y=World3dToScreen2d(coords.x,coords.y,coords.z + 1.0)
    local px,py,pz=table.unpack(GetGameplayCamCoords())
    
    SetTextScale(0.35, 0.35)
    SetTextFont(4)
    SetTextProportional(1)
    SetTextColour(255, 255, 255, 215)
    SetTextEntry("STRING")
    SetTextCentre(1)
    AddTextComponentString(text)
    DrawText(_x,_y)
    local factor = (string.len(text)) / 370
    DrawRect(_x,_y+0.0125, 0.015+ factor, 0.03, 41, 11, 41, 68)
end


function GetVehiclesInArea(coords, radius)
    local vehicles = {}
    local handle, vehicle = FindFirstVehicle()
    local success

    repeat
        if #(GetEntityCoords(vehicle) - coords) <= radius then
            table.insert(vehicles, vehicle)
        end
        success, vehicle = FindNextVehicle(handle)
    until not success

    EndFindVehicle(handle)
    return vehicles
end

RegisterCommand("+inhelimed", function(source, args, rawCommand)
    local ped = PlayerPedId() 
    local playerCoords = GetEntityCoords(ped)   

    local stretcher = GetClosestVehicleOfType(playerCoords, 10.0, `stretcher`)
    if not stretcher then
        Notify("No stretcher found nearby.")
        return
    end

    local helimed = GetClosestVehicleOfType(playerCoords, 10.0, `GLAAA`) or GetClosestVehicleOfType(playerCoords, 10.0, `GLAAB`)
    if not helimed then
        Notify("No Helimed found!")
        return
    end

    local bootBone = GetEntityBoneIndexByName(helimed, "seat_pside_r")
    if bootBone == -1 then
        return
    end

    AttachEntityToEntity(stretcher, helimed, bootBone, -0.65, -0.2, -0.45, 0.0, 0.0, 0.0, false, false, true, false, 2, true)
    attachedStretcher = stretcher  
    Notify("Stretcher attached to Helimed.")

    SetVehicleExtra(stretcher, 1, 1) 
    SetVehicleExtra(stretcher, 2, 0) 

    ClearPedTasksImmediately(ped)  
    Citizen.Wait(500)  
    ClearPedTasks(ped)  

    TaskClearLookAt(ped)

    Citizen.Wait(500)  
end, false)

RegisterCommand("+outhelimed", function(source, args, rawCommand)
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)
    local closestVehicle = GetClosestVehicle(playerCoords.x, playerCoords.y, playerCoords.z, 10.0, 0, 70)

    if closestVehicle and closestVehicle ~= 0 then
        local vehicleModel = GetEntityModel(closestVehicle)
        local vehicleName = GetDisplayNameFromVehicleModel(vehicleModel)

        if string.match(vehicleName:lower(), "stretcher") then
            local vehicleCoords = GetEntityCoords(closestVehicle)
            local vehicleHeading = GetEntityHeading(closestVehicle)

            DetachEntity(closestVehicle, true, true)

            local offsetDistance = 3.0  -- 5 feet (approximately 1.5 meters)
            local offsetX = vehicleCoords.x + (math.sin(math.rad(vehicleHeading + 180)) * offsetDistance)  -- 90 degrees for left side
            local offsetY = vehicleCoords.y + (math.cos(math.rad(vehicleHeading + 90)) * offsetDistance)  -- 90 degrees for left side
            local offsetZ = vehicleCoords.z - 1.0  -- Lower the stretcher 0.5 units (adjust this value as needed)

            SetEntityCoordsNoOffset(closestVehicle, offsetX, offsetY, offsetZ, true, true, true)

            TriggerEvent('notification', 'Stretcher removed from Helimed!')
        else
            TriggerEvent('notification', 'No stretcher found nearby!')
        end
    else
        TriggerEvent('notification', 'No Helimed found!')
    end
end, false)


RegisterNetEvent('notification')
AddEventHandler('notification', function(msg)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(msg)
    DrawNotification(false, true)
end)


function GetClosestVehicleOfType(coords, radius, modelHash)
    local vehicles = GetGamePool("CVehicle") 
    local closestVehicle = nil
    local minDistance = radius

    for _, vehicle in ipairs(vehicles) do
        if DoesEntityExist(vehicle) and GetEntityModel(vehicle) == modelHash then
            local vehicleCoords = GetEntityCoords(vehicle)
            local distance = #(coords - vehicleCoords)
            if distance < minDistance then
                minDistance = distance
                closestVehicle = vehicle
            end
        end
    end

    return closestVehicle
end

function Notify(message)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(false, true)
end
