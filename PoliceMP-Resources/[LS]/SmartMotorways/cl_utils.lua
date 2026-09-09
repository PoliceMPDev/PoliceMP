function ShowNotification(message)
    SetNotificationTextEntry("STRING")
	AddTextComponentString(message)
	DrawNotification(0,1)
end


if config.developerMode then
	TriggerEvent('chat:addSuggestion', '/'.."newsign", "Easily place a SmartMotorway sign.", {
		{name="2/3/4", help="Amount of lanes"}
	})
	RegisterCommand("newsign", function(source, args)
		local laneAmount = tonumber(args[1])
		if not (args[1] ~= nil and (laneAmount == 2 or laneAmount == 3 or laneAmount == 4)) then
			return false
		end
		RequestModel(config.signModels[laneAmount])
		while not HasModelLoaded(config.signModels[laneAmount]) do Wait(0) end
		local ped = PlayerPedId()
		local coords = GetEntityCoords(ped)
		local signProp = CreateObject(config.signModels[laneAmount], coords, true, false, false)
		while not DoesEntityExist(signProp) do Wait(0) end
		FreezeEntityPosition(signProp, true)
		SetEntityCoords(signProp, coords.x, coords.y, coords.z, true, true, true, false)
		local heading = GetEntityHeading(ped)
		SetEntityHeading(signProp, ped)
		SetModelAsNoLongerNeeded(config.signModels[laneAmount])
		PlaceObjectOnGroundProperly(signProp)

		local complete = false
		while not complete do
			local coords = GetEntityCoords(signProp)
			local heading = GetEntityHeading(signProp)
			if not IsControlReleased(0, 207) then --page down
				SetEntityCoords(signProp, coords.x, coords.y, coords.z - 0.1)
			end
				
			if not IsControlReleased(0, 208) then --page up
				SetEntityCoords(signProp, coords.x, coords.y, coords.z + 0.1)
			end


			if not IsControlReleased(0, 173) then --arrow down
				SetEntityCoords(signProp, coords.x, coords.y - 0.1, coords.z)
			end

			if not IsControlReleased(0, 172) then --arrow up
				SetEntityCoords(signProp, coords.x, coords.y + 0.1, coords.z)
			end

			if not IsControlReleased(0, 174) then --arrow left
				SetEntityCoords(signProp, coords.x - 0.1, coords.y, coords.z)
			end

			if not IsControlReleased(0, 175) then --arrow right
				SetEntityCoords(signProp, coords.x + 0.1, coords.y, coords.z)
			end

			if not IsControlReleased(0, 29) then --b rotate left
				SetEntityHeading(signProp, heading - 0.5)
			end

			if not IsControlReleased(0, 306) then --n rotate right
				
				SetEntityHeading(signProp, heading + 0.5)
			end

			if IsControlJustPressed(0, 191) then -- enter - finish
				complete = true
			end
			
			Wait(0)
		end

		local coords = GetEntityCoords(signProp)
		local heading = GetEntityHeading(signProp)
		local streetHash = GetStreetNameAtCoord(coords.x, coords.y, coords.z)
		local streetName = GetStreetNameFromHashKey(streetHash)
		ShowNotification("~b~Coords~w~: {"..coords.x..", "..coords.y..", "..coords.z.."}")
		ShowNotification("~b~Heading~w~: "..heading)
	end, false)
end