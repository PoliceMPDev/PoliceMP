local isRadarExtended = false


function setBlipParams(blip, playerName, currentRole)

	local prefix = 'PLAYER'

	if currentRole.Branch == 0 then
		SetBlipColour(blip, 38)

		if currentRole.Division == 0 then
			prefix = 'MET'
		elseif currentRole.Division == 1 then
			prefix = 'AFO'
		elseif currentRole.Division == 2 then
			prefix = 'CID'
		elseif currentRole.Division == 3 then
			prefix = 'DSU'
		elseif currentRole.Division == 4 then
			prefix = 'ERT'
		elseif currentRole.Division == 5 then
			prefix = 'NPAS'
		elseif currentRole.Division == 6 then
			prefix = 'RTPC'
		else
			prefix = 'MET'
		end

	elseif currentRole.Branch == 1 then
		SetBlipColour(blip, 1)
		prefix = 'LFB'
	elseif currentRole.Branch == 2 then
		SetBlipColour(blip, 25)
		prefix = 'LHS'
	elseif currentRole.Branch == 3 then
		SetBlipColour(blip, 0)
		prefix = 'CIV'
	elseif currentRole.Branch == 4 then
		SetBlipColour(blip, 5)
		prefix = 'MOTORWAYS'
	end

	BeginTextCommandSetBlipName("STRING")
	AddTextComponentString('['..prefix..'] '..playerName)
	EndTextCommandSetBlipName(blip)
  end

Citizen.CreateThread(function()

	while true do

		Wait( 1 )

		-- show blips
		for _, id in ipairs(GetActivePlayers()) do
			
			if GetPlayerPed(id) ~= GetPlayerPed(-1) then
				local serverId = GetPlayerServerId(id);
				local player = Player(serverId);
				local ped = GetPlayerPed(id);
				local blip = GetBlipFromEntity( ped )


				if player.state.pmphideblips == true or player.state.pmphideblips == 'true' then
					Citizen.InvokeNative( 0x63BB75ABEDC1F6A0, headId, 7, false ) -- Remove wanted sprite
					Citizen.InvokeNative( 0x63BB75ABEDC1F6A0, headId, 9, false ) -- Remove speaking sprite
					RemoveBlip( blip )
				else

					local currentRole = json.decode(player.state.currentRole);

					if currentRole ~= nil then
						-- BLIP STUFF --

						if not DoesBlipExist( blip ) then -- Add blip and create head display on player
							blip = AddBlipForEntity( ped )
							SetBlipCategory( blip, currentRole.Branch + 100 )
							SetBlipSprite( blip, 1 )

							Citizen.InvokeNative( 0x5FBCA48327B914DF, blip, true ) -- Player Blip indicator

						else -- update blip

							veh = GetVehiclePedIsIn( ped, false )
							blipSprite = GetBlipSprite( blip )

							if not GetEntityHealth( ped ) then -- dead

								if blipSprite ~= 274 then

									SetBlipSprite( blip, 274 )
									Citizen.InvokeNative( 0x5FBCA48327B914DF, blip, false ) -- Player Blip indicator

								end

							elseif veh then

								vehClass = GetVehicleClass( veh )
								vehModel = GetEntityModel( veh )
								
								if vehClass == 15 then -- jet

									if blipSprite ~= 422 then

										SetBlipSprite( blip, 422 )
										Citizen.InvokeNative( 0x5FBCA48327B914DF, blip, false ) -- Player Blip indicator

									end

								elseif vehClass == 16 then -- plane

									if vehModel == GetHashKey( "besra" ) or vehModel == GetHashKey( "hydra" )
										or vehModel == GetHashKey( "lazer" ) then -- jet

										if blipSprite ~= 424 then

											SetBlipSprite( blip, 424 )
											Citizen.InvokeNative( 0x5FBCA48327B914DF, blip, false ) -- Player Blip indicator

										end

									elseif blipSprite ~= 423 then

										SetBlipSprite( blip, 423 )
										Citizen.InvokeNative (0x5FBCA48327B914DF, blip, false ) -- Player Blip indicator

									end

								elseif vehClass == 14 then -- boat

									if blipSprite ~= 427 then

										SetBlipSprite( blip, 427 )
										Citizen.InvokeNative( 0x5FBCA48327B914DF, blip, false ) -- Player Blip indicator

									end

								elseif vehModel == GetHashKey( "insurgent" ) or vehModel == GetHashKey( "insurgent2" )
								or vehModel == GetHashKey( "limo2" ) then -- insurgent (+ turreted limo cuz limo blip wont work)

									if blipSprite ~= 426 then

										SetBlipSprite( blip, 426 )
										Citizen.InvokeNative( 0x5FBCA48327B914DF, blip, false ) -- Player Blip indicator

									end

								elseif vehModel == GetHashKey( "rhino" ) then -- tank

									if blipSprite ~= 421 then

										SetBlipSprite( blip, 421 )
										Citizen.InvokeNative( 0x5FBCA48327B914DF, blip, false ) -- Player Blip indicator

									end

								elseif blipSprite ~= 1 then -- default blip

									SetBlipSprite( blip, 1 )
									Citizen.InvokeNative( 0x5FBCA48327B914DF, blip, true ) -- Player Blip indicator

								end

								-- Show number in case of passangers
								passengers = GetVehicleNumberOfPassengers( veh )

								if passengers then

									if not IsVehicleSeatFree( veh, -1 ) then

										passengers = passengers + 1

									end

									ShowNumberOnBlip( blip, passengers )

								else

									HideNumberOnBlip( blip )

								end

							else

								-- Remove leftover number
								HideNumberOnBlip( blip )

								if blipSprite ~= 1 then -- default blip

									SetBlipSprite( blip, 1 )
									Citizen.InvokeNative( 0x5FBCA48327B914DF, blip, true ) -- Player Blip indicator

								end

							end

							SetBlipRotation( blip, math.ceil( GetEntityHeading( veh ) ) ) -- update rotation
							-- SetBlipNameToPlayerName( blip, id ) -- update blip name
							SetBlipScale( blip,  0.85 ) -- set scale
							setBlipParams(blip, GetPlayerName(id), currentRole)

							-- set player alpha
							if IsPauseMenuActive() then

								SetBlipAlpha( blip, 255 )

							else

								x1, y1 = table.unpack( GetEntityCoords( GetPlayerPed( -1 ), true ) )
								x2, y2 = table.unpack( GetEntityCoords( ped, true ) )
								distance = ( math.floor( math.abs( math.sqrt( ( x1 - x2 ) * ( x1 - x2 ) + ( y1 - y2 ) * ( y1 - y2 ) ) ) / -1 ) ) + 900
								-- Probably a way easier way to do this but whatever im an idiot

								if distance < 0 then

									distance = 0

								elseif distance > 255 then

									distance = 255

								end

								SetBlipAlpha( blip, distance )

							end

						end
					end
				end
			end

		end

	end

end)