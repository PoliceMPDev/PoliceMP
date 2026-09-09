-- Define the variable used to open/close the tab
local tabEnabled = false
local tabLoaded = true --false

function setRoute(x, y)
    SetNewWaypoint(tonumber(x), tonumber(y))
end

function REQUEST_NUI_FOCUS(bool, nav)
    nav = nav or nil
    SetNuiFocus(bool, bool) -- focus, cursor
    if bool == true then
        SendNUIMessage({showtab = true, nav = nav})
    else
        SendNUIMessage({hidetab = true})
    end
    return bool
end

RegisterNetEvent("fms:clnuidata")
AddEventHandler("fms:clnuidata", function(t, data)
    SendNUIMessage({
		type = t,
		data = data
	})
end)

RegisterNetEvent("fms:opentablet")
AddEventHandler("fms:opentablet", function(url)
    REQUEST_NUI_FOCUS(true, url or nil)
    tabEnabled = true
end)

RegisterNUICallback(
    "tablet-bus",
    function(data)
        -- Do tablet hide shit
        if data.load then
            tabLoaded = true
        elseif data.hide then
            SetNuiFocus(false, false) -- Don't REQUEST_NUI_FOCUS here
            tabEnabled = false
        elseif data.click then
        -- if u need click events
        end
    end
)

RegisterNUICallback('triggerserverevent', function(data)
    -- POST data gets parsed as JSON automatically
    if (data.includeCurrentCoordinates == true) then
        local x,y,z = table.unpack(GetEntityCoords(PlayerPedId()))
        data.x = x
        data.y = y
    end

    if (data.serverEvent ~= nil) then
        TriggerServerEvent(data.serverEvent, data)
    end
end)

RegisterNUICallback('updatecadcoords', function(data)
    local x,y,z = table.unpack(GetEntityCoords(PlayerPedId()))
    TriggerServerEvent("fms:updatecadcoords", { x = x, y = y })
end)

RegisterNUICallback('savetabletdimensions', function(data)
    SetResourceKvp('tablet_xoffset', data.data.xoffset)
    SetResourceKvp('tablet_yoffset', data.data.yoffset)
    SetResourceKvp('tablet_height', data.data.height)
    SetResourceKvp('tablet_width', data.data.width)
end)

RegisterNUICallback('setroute', function(data)
    if (data ~= nil and data.x ~= nil and data.y ~= nil) then
        setRoute(data.x, data.y)
    end
end)

Citizen.CreateThread(
    function()
        -- Wait for nui to load or just timeout
        local l = 0
        local timeout = false
        while not tabLoaded do
            Citizen.Wait(0)
            l = l + 1
            if l > 500 then
                tabLoaded = true --
                timeout = true
            end
        end

        if timeout == true then
            print("Failed to load tablet nui...")
        -- return ---- Quit
        end

        print("::The client lua for tablet loaded::")

        REQUEST_NUI_FOCUS(false) -- This is just in case the resources restarted whilst the NUI is focused.
        Citizen.Wait(1000)
        TriggerServerEvent("fms:gettabletglobalsettings")

        local tabletdimensions = {}
        tabletdimensions.xoffset = GetResourceKvpString("tablet_xoffset")
        tabletdimensions.yoffset = GetResourceKvpString("tablet_yoffset")
        tabletdimensions.height = GetResourceKvpString("tablet_height")
        tabletdimensions.width = GetResourceKvpString("tablet_width")
        SendNUIMessage({
            type = "loadtabletdimensions",
            data = tabletdimensions
        })
		
        while true do
            -- -- Control ID 20 is the 'Z' key by default
            -- -- 244 = M
            -- -- Use https://wiki.fivem.net/wiki/Controls to find a different key
            -- if (IsControlJustPressed(0, 244)) and GetLastInputMethod( 0 ) then
                -- tabEnabled = not tabEnabled -- Toggle tablet visible state
                -- REQUEST_NUI_FOCUS(tabEnabled)
                -- print("The tablet state is: " .. tostring(tabEnabled))
                -- Citizen.Wait(0)
            -- end
            if (tabEnabled) then
                local ped = GetPlayerPed(-1)
                DisableControlAction(0, 1, tabEnabled) -- LookLeftRight
                DisableControlAction(0, 2, tabEnabled) -- LookUpDown
                DisableControlAction(0, 24, tabEnabled) -- Attack
                DisablePlayerFiring(ped, tabEnabled) -- Disable weapon firing
                DisableControlAction(0, 142, tabEnabled) -- MeleeAttackAlternate
                DisableControlAction(0, 106, tabEnabled) -- VehicleMouseControlOverride
            end
            Citizen.Wait(0)
        end
    end
)

local x1,y1,z1 = 0
Citizen.CreateThread(function()
    while true do
        Wait(2000)

        if NetworkIsPlayerActive(PlayerId()) then
            local x,y,z = table.unpack(GetEntityCoords(PlayerPedId()))

            local dist = Vdist(x, y, z, x1, y1, z1)
            if (dist >= 0.3) then
                local var1, var2 = GetStreetNameAtCoord(x, y, z, Citizen.ResultAsInteger(), Citizen.ResultAsInteger())
                local streetname = GetStreetNameFromHashKey(var1) --exports["customareanames"]:getCustomStreetName(var1)
                local tst2 = GetStreetNameFromHashKey(var2) --exports["customareanames"]:getCustomStreetName(var2)
                if (tst2 ~= "") then
                    streetname = streetname .. " ("..tst2..")"
                end

                local speed = math.floor(GetEntitySpeed(GetPlayerPed(-1), false) * 2.236936)

                x1 = x
                y1 = y
                z1 = z
                TriggerServerEvent("fms:updateposition", x, y, z, streetname, speed)
            end
        end
    end
end)

local function freezePlayer(id, freeze)
    local player = id
    SetPlayerControl(player, not freeze, false)

    local ped = GetPlayerPed(player)

    if not freeze then
        if not IsEntityVisible(ped) then
            SetEntityVisible(ped, true)
        end

        if not IsPedInAnyVehicle(ped) then
            SetEntityCollision(ped, true)
        end

        FreezeEntityPosition(ped, false)
        --SetCharNeverTargetted(ped, false)
        SetPlayerInvincible(player, false)
    else
        if IsEntityVisible(ped) then
            SetEntityVisible(ped, false)
        end

        SetEntityCollision(ped, false)
        FreezeEntityPosition(ped, true)
        --SetCharNeverTargetted(ped, true)
        SetPlayerInvincible(player, true)
        --RemovePtfxFromPed(ped)

        if not IsPedFatallyInjured(ped) then
            ClearPedTasksImmediately(ped)
        end
    end
end

local alreadySpawned = false
function spawn(x, y, z, heading, model)
	Citizen.CreateThread(function()
		-- freeze the local player
		freezePlayer(PlayerId(), true)

		-- if the spawn has a model set
		if model ~= nil then
			RequestModel(model)

			-- load the model for this spawn
			while not HasModelLoaded(model) do
				RequestModel(model)

				Wait(0)
			end

			-- change the player model
			SetPlayerModel(PlayerId(), model)

			-- release the player model
			SetModelAsNoLongerNeeded(model)
			
			-- RDR3 player model bits
			if N_0x283978a15512b2fe then
				N_0x283978a15512b2fe(PlayerPedId(), true)
			end
		end

		-- preload collisions for the spawnpoint
		RequestCollisionAtCoord(x, y, z)

		-- spawn the player
		local ped = PlayerPedId()

		-- V requires setting coords as well
		SetEntityCoordsNoOffset(ped, x, y, z, false, false, false, true)
		SetEntityHeading(ped, heading)

		-- gamelogic-style cleanup stuff
		ClearPedTasksImmediately(ped)
		--SetEntityHealth(ped, 300)
		RemoveAllPedWeapons(ped)
		ClearPlayerWantedLevel(PlayerId())

		local time = GetGameTimer()

		while (not HasCollisionLoadedAroundEntity(ped) and (GetGameTimer() - time) < 3000) do
			Citizen.Wait(0)
		end

		-- and unfreeze the player
        freezePlayer(PlayerId(), false)
        alreadySpawned = false
	end)
end

AddEventHandler('playerSpawned', function()
	if(not alreadySpawned)then
		TriggerServerEvent("teleportOnJoin")
		alreadySpawned = true
	end
end)

RegisterNetEvent("teleportPlayerOnJoin")
AddEventHandler('teleportPlayerOnJoin', function(x, y, z, heading, model)
	spawn(tonumber(x), tonumber(y), tonumber(z), tonumber(heading), model)
end)

RegisterNetEvent("fms:setroute")
AddEventHandler('fms:setroute', setRoute)

TriggerEvent('chat:addSuggestion', '/comms', 'Toggles your radio comms group to the specified commsgroupname.', {
    { name="commsgroupname", help="Specify desired commsgroupname or Off." }
})

TriggerEvent('chat:addSuggestion', '/radio', 'Toggles your radio comms group between Off and your branch commsgroup.', {
})

RegisterCommand("disc", function(source, args)
    TriggerEvent("fms:disccheckclosestveh")
end)

RegisterCommand("updatecadcoords", function(source, args)
    local x,y,z = table.unpack(GetEntityCoords(PlayerPedId()))
    TriggerServerEvent("fms:updatecadcoords", { x = x, y = y })
end)

function clearRoute()
    SetWaypointOff()
end

RegisterNetEvent("fms:clearroute")
AddEventHandler('fms:clearroute', clearRoute)

TriggerEvent('chat:addSuggestion', '/gps', 'Sets your destination to the specified street.', {
    { name="street", help="Street to set destination to. Leave blank to clear destination." }
})

TriggerEvent('chat:addSuggestion', '/spager', 'Subscribes to pager alerts for the specified category.', {
    { name="category", help="Pager alert category to subscribe to" }
})

TriggerEvent('chat:addSuggestion', '/subpager', 'Subscribes to pager alerts for the specified category.', {
    { name="category", help="Pager alert category to subscribe to" }
})

TriggerEvent('chat:addSuggestion', '/upager', 'Unubscribes from pager alerts for the specified category.', {
    { name="category", help="Pager alert category to unsubscribe from" }
})

TriggerEvent('chat:addSuggestion', '/unsubpager', 'Unubscribes from pager alerts for the specified category.', {
    { name="category", help="Pager alert category to unsubscribe from" }
})

TriggerEvent('chat:addSuggestion', '/apager', 'Accepts the pager alert with the specified ID.', {
    { name="ID", help="Pager alert ID to accept" }
})

TriggerEvent('chat:addSuggestion', '/acceptpager', 'Accepts the pager alert with the specified ID.', {
    { name="ID", help="Pager alert ID to accept" }
})

TriggerEvent('chat:addSuggestion', '/pi', 'Shows information about your current pager status.', {
})

TriggerEvent('chat:addSuggestion', '/pagerinfo', 'Shows information about your current pager status.', {
})

TriggerEvent('chat:addSuggestion', '/channel', 'Moves you to the incident channel with the specified channelName.', {
    { name="channelName", help="The name of the incident channel to move to." }
})

TriggerEvent('chat:addSuggestion', '/p2p', 'Create P2P channel with you and the other specified users', {
    { name="users", help="Format: user1;user2;user3" }
})
