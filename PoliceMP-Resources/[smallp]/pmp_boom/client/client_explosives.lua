local GT = nil
RegisterNetEvent('PoliceMP:Boom:Login')
AddEventHandler('PoliceMP:Boom:Login', function(_GT)
	GT = _GT
end)

local function LoadParticleDictionary(dictionary)
    if not HasNamedPtfxAssetLoaded(dictionary) then
		RequestNamedPtfxAsset(dictionary)
		while not HasNamedPtfxAssetLoaded(dictionary) do
			Citizen.Wait(50)
		end
	end
end

local function StartParticleAtCoord(pfx_dict, pfx_name, pfx_looped, coords, rot, scale, alpha, color, duration)
	LoadParticleDictionary(pfx_dict)
	UseParticleFxAssetNextCall(pfx_dict)
	SetPtfxAssetNextCall(pfx_dict)

	local particleHandle
	if pfx_looped then
		particleHandle = StartParticleFxLoopedAtCoord(pfx_name, coords.x, coords.y, coords.z, rot.x, rot.y, rot.z, scale or 1.0)

		if color then
			SetParticleFxLoopedColour(particleHandle, color.r, color.g, color.b, false)
		end

		SetParticleFxLoopedAlpha(particleHandle, alpha or 10.0)

		if duration then
			Citizen.Wait(duration)
			StopParticleFxLooped(particleHandle, 0)
		end
	else
		SetParticleFxNonLoopedAlpha(alpha or 10.0)

		if color then
			SetParticleFxNonLoopedColour(color.r, color.g, color.b)
		end

		StartParticleFxNonLoopedAtCoord(pfx_name, coords.x, coords.y, coords.z, rot.x, rot.y, rot.z, scale or 1.0)
	end

	return particleHandle
end

local function SimulateExplosionDamage(origin)
	local player_ped = PlayerPedId()
	local player_coords = GetEntityCoords(player_ped)
	local distance_from_origin = #(player_coords - origin)
	if distance_from_origin <= endpoint.explosion_damage_range then
		local damage_to_player = math.floor(endpoint.explosion_damage_range - distance_from_origin)
		SetEntityHealth(player_ped, GetEntityHealth(player_ped) - damage_to_player)
		if distance_from_origin <= endpoint.explosion_damage_range / 2 then
			local explosion_vector = GetEntityForwardVector(player_ped)
			SetPedToRagdollWithFall(player_ped, 5000, 5000, 0, explosion_vector, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
		end
	end
end

RegisterNetEvent('PoliceMP:Boom:ExplodeAtCoordinate')
AddEventHandler('PoliceMP:Boom:ExplodeAtCoordinate', function(explosive_type, coordinate)
	local player_ped = PlayerPedId()
	local player_coords = GetEntityCoords(player_ped)

	if #(player_coords - coordinate) <= endpoint.max_range then
		if not explosive_type or explosive_type == "mechanical" then
			--local temporary_sound_id = GetSoundId()
			--PlaySoundFromCoord(temporary_sound_id, "Jet_Explosions", coordinate.x, coordinate.y, coordinate.z, "exile_1", 0, 10, 0)
			StartParticleAtCoord("des_gas_station", "ent_ray_paleto_gas_explosion", true, vector3(coordinate.x, coordinate.y, coordinate.z), vector3(0,0,0), 1.0,  10.0, {r=255,g=255,b=255}, 300)
			AddExplosion(coordinate.x, coordinate.y, coordinate.z, 81, 10.0, true, false, 1.0) -- Add in-game explosion
			if endpoint.explosion_damage then
				SimulateExplosionDamage(coordinate)
			end
			StartParticleAtCoord("scr_agencyheistb", "scr_agency3b_linger_smoke", true, vector3(coordinate.x, coordinate.y, coordinate.z), vector3(0,0,0), 3.0,  10.0, {r=255,g=255,b=255}, 10000)
			--StopSound(temporary_sound_id)
			--ReleaseSoundId(temporary_sound_id)
		end
	end
end)

local function RequestAnimation(animation_dir)
	while not HasAnimDictLoaded(animation_dir) do 
		Citizen.Wait(100)
		RequestAnimDict(animation_dir)
	end
end

local function PlayPlantingAnimation()
	RequestAnimation(endpoint.plant_animation[1])
	TaskPlayAnim(PlayerPedId(), endpoint.plant_animation[1], endpoint.plant_animation[2], 8.00, -8.00, 1500, 1, 0, false, false, false)
end

local function GetPinNumber()
	AddTextEntry("NM_EXPL_PIN", "Enter Pin Number")
	DisplayOnscreenKeyboard(1, "NM_EXPL_PIN", "", "", "", "", "", 30)

	while UpdateOnscreenKeyboard() == 0 do
		DisableAllControlActions(0)
		Citizen.Wait(0)
	end

	if GetOnscreenKeyboardResult() then
		return tonumber(GetOnscreenKeyboardResult())
	end

	return 0
end

local function Draw3D(coords, text, size, font)
	coords = vector3(coords.x, coords.y, coords.z)
	
	local camCoords = GetGameplayCamCoords()
	local distance = #(coords - camCoords)
	
	if not size then size = 1 end
	if not font then font = 0 end
	
	local scale = (size / distance) * 2
	local fov = (1 / GetGameplayCamFov()) * 100
	scale = scale * fov
	
	SetTextScale(0.0 * scale, 0.60 * scale)
	SetTextFont(font)
	SetTextColour(255, 255, 255, 255)
	SetTextDropshadow(0, 0, 0, 0, 255)
	SetTextDropShadow()
	SetTextOutline()
	SetTextCentre(true)
	
	SetDrawOrigin(coords, 0)
	BeginTextCommandDisplayText('STRING')
	AddTextComponentSubstringPlayerName(text)
	EndTextCommandDisplayText(0.0, 0.0)
	ClearDrawOrigin()
end

local placeable_hash = GetHashKey(endpoint.placeable_object)
function GetClosestExplosive()
	local player_ped = PlayerPedId()
	local player_coords = GetEntityCoords(player_ped)
	local closest_explosive = GetClosestObjectOfType(player_coords.x, player_coords.y, player_coords.z, 5.0, placeable_hash, 0, 0, 0)

	if closest_explosive and DoesEntityExist(closest_explosive) then
		local entity = Entity(closest_explosive)
		if entity.state and entity.state.explosive_timer then
			return closest_explosive
		end
	end

	return nil
end

Citizen.CreateThread(function()
	while true do
		local sleep_timer = 1000

		local player_ped = PlayerPedId()
		local player_coords = GetEntityCoords(player_ped)
		local closest_explosive = GetClosestObjectOfType(player_coords.x, player_coords.y, player_coords.z, 4.0, placeable_hash, 0, 0, 0)
		if closest_explosive and DoesEntityExist(closest_explosive) then
			local entity = Entity(closest_explosive)
			if entity.state and entity.state.explosive_timer then
				sleep_timer = 0
				local explosive_coords = GetEntityCoords(closest_explosive)

				local draw_string = endpoint.default_defuse_text
				if endpoint.draw_timer or endpoint.draw_pin then
					if endpoint.draw_timer then
						draw_string = ("%s~r~%s\n"):format(draw_string, entity.state.explosive_timer)
					end
					if endpoint.draw_pin then
						draw_string = ("%s~y~Defuse Code: ~s~%s"):format(draw_string, entity.state.explosive_code)
					end
				end

				Draw3D(vector3(explosive_coords.x, explosive_coords.y, explosive_coords.z + 0.3), draw_string, 0.5)
			end
		end

		Citizen.Wait(sleep_timer)
	end
end)

local function StartDefusing()
	local explosive_to_defuse = GetClosestExplosive()
	if explosive_to_defuse then
		SetNuiFocus(true, true)
		SendNUIMessage({ module = "pmp_boom", event_call = "defuse:toggle_ui:on" })
	end
end

RegisterNUICallback('attempt', function(data, cb)
	local pin = data.value
	local closest_explosive = GetClosestExplosive()
	local entity = Entity(closest_explosive)

	if tostring(entity.state.explosive_code) == pin or pin == "true" then
		TriggerServerEvent('PoliceMP:Boom:Defuse',
			{ P1 = GT, explosive_id = entity.state.explosive_id })
	else
		if endpoint.explode_on_incorrect then
			TriggerServerEvent('PoliceMP:Boom:Explode',
				{ P1 = GT, explosive_id = entity.state.explosive_id })
		end
	end

	PlayPlantingAnimation()

	SetNuiFocus(false, false)
	SendNUIMessage({ module = "pmp_boom", event_call = "defuse:toggle_ui:off" })
end)

local function StartPlanting(explosive_type)
	local pin_number = GetPinNumber()
	if not pin_number then
		pin_number = math.random(0000,9999)
	end

	PlayPlantingAnimation()

	TriggerServerEvent('PoliceMP:Boom:Plant',
		{ P1 = GT, pin_code = pin_number, explosive_type = explosive_type, explosive_location = GetEntityCoords(PlayerPedId()) })
end

RegisterNUICallback('closeUI', function(data, cb)
    SetNuiFocus(false, false)
    cb('ok')  
end)

local function DoAPrint(args)
    local message = 'Bomb located'
    DisplayNotification(message)
end

function DisplayNotification(text)
    SetNotificationTextEntry("STRING")
    AddTextComponentSubstringPlayerName(text)
    DrawNotification(false, false)
end

RegisterNetEvent('pmp_boom:SetupDigiScanner', function(location)
    exports['bomb_scanner']:SetupDigiScanner(location, {
        event = DoAPrint,
		isAction = true,
        interact = {
            interactKey = 38, 
            interactMessage = 'Disarm the explosive', 
        }
    })
end)


exports('StartPlanting', StartPlanting)
exports('StartDefusing', StartDefusing)