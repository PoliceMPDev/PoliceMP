local pluginData = {
    name = "blood_plugin",
    description = "Blood Plugin",
    author = "",
    version = "1.0"
}

RegisterClientPlugin(pluginData, function(print)

Config = {}

Config.Time = 5 -- Time in seconds how often a bloodstain should be on the floor

Config.Vehicle = false -- Should the blood stain also appear in a vehicle? Not recommended because the bloodstain has a collision

    Citizen.CreateThread(
            function()
                while true do
                    Citizen.Wait(Config.Time * 1000)
                    local ped = PlayerPedId()    
                    local coords = GetEntityCoords(ped)

                    local sum = ClientHealthBuffer.bodyParts.RIGHT_LEG.injuryAmount + ClientHealthBuffer.bodyParts.LEFT_LEG.injuryAmount + ClientHealthBuffer.bodyParts.LEFT_ARM.injuryAmount + ClientHealthBuffer.bodyParts.RIGHT_ARM.injuryAmount + ClientHealthBuffer.bodyParts.HEAD.injuryAmount + ClientHealthBuffer.bodyParts.TORSO.injuryAmount

                    if Config.Vehicle == true then
                        if sum > 0 and not IsPedInAnyVehicle(PlayerPedId(), false) then
                                local stain = CreateObject(GetHashKey("p_bloodsplat_s"), coords[1], coords[2], coords[3] - 2.0, true, true, false)
                                PlaceObjectOnGroundProperly(stain)
                                local stainCoords = GetEntityCoords(stain)
                                SetEntityCoords(stain, stainCoords[1], stainCoords[2], stainCoords[3] - 0.25)
                                SetEntityAsMissionEntity(stain, true, true)
                                SetEntityRotation(stain, -90.0, 0.0, 0.0, 2, false)
                                FreezeEntityPosition(stain, true)
                                SetEntityAsNoLongerNeeded(stain)
                        end
                    else
                        if sum > 0 then
                            local stain = CreateObject(GetHashKey("p_bloodsplat_s"), coords[1], coords[2], coords[3] - 2.0, true, true, false)
                            PlaceObjectOnGroundProperly(stain)
                            local stainCoords = GetEntityCoords(stain)
                            SetEntityCoords(stain, stainCoords[1], stainCoords[2], stainCoords[3] - 0.25)
                            SetEntityAsMissionEntity(stain, true, true)
                            SetEntityRotation(stain, -90.0, 0.0, 0.0, 2, false)
                            FreezeEntityPosition(stain, true)
                            SetEntityAsNoLongerNeeded(stain)
                        end
                    end
                end            
            end)
end)