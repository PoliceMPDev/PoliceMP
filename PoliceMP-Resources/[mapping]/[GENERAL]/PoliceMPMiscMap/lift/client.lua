key_to_teleport = 38

positions = {
    --[[
    {{Teleport1 X, Teleport1 Y, Teleport1 Z, Teleport1 Heading}, {Teleport2 X, Teleport 2Y, Teleport 2Z, Teleport2 Heading}, {Red, Green, Blue}, "Text for Teleport"}
    ]]
    {{-442.16, -342.95, 34.01, 151.47}, {-442.29, -343.41, 41.43, 343.85},{36,237,157}, "Press ~INPUT_PICKUP~ to go up or down"}, -- Zonah Ground Lift
    {{-491.07, -329.91, 41.32, 168.46}, {-490.9, -329.81, 68.52, 176.06},{255, 157, 0}, "Press ~INPUT_PICKUP~ to go up or down"}, -- Zonah Ground Lift
    {{-493.88119506836,-329.29666137695,41.32067489624}, {-421.0302734375,-345.56289672852,23.229343414307},{36,237,157}, "Press ~INPUT_PICKUP~ to go up or down"}, -- Zonah Ground Lift
    {{-493.16830444336,-371.73919677734,23.229343414307}, {-435.80999755859,-357.5983581543,33.910709381104},{36,237,157}, "Press ~INPUT_PICKUP~ to go up or down"}, -- Zonah Ground Lift
    {{-448.0234, -297.2674, 77.32784}, {-454.7437, -289.4384, 33.91082},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Hospital"}, -- MT Zona helipad )
    
    -----
    {{923.0092, 47.05838, 80.10636}, {964.2498, 58.98241, 111.553},{36,237,157}, "Press ~INPUT_PICKUP~ to go on/off Casino Roof"}, -- Casino ground to roof 
    {{-677.2299, -2458.993, 12.94439}, {-1569.34, -3017.241, -75.40616},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Nightclub"}, -- LSIA Nightclub 
    {{1182.289, -3322.102, 5.028763}, {1065.829,-3183.313,-40.16345},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Warehouse"}, -- Weed Farm at Port 
    {{1183.997, -3168.104, 6.118019}, {1102.739,-3195.953,-39.99347},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Warehouse"}, -- Cocaine Lockup at Port 
    {{-1371, -503.6255, 32.15739}, {-1395.203, -479.6846, 71.04211},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Office Block"}, -- Maze Bank West Office 
    {{-116.2306, -604.8462, 35.28072}, {-140.7948, -617.8311, 167.8204},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Office Block"}, -- Arcadius Business Centre Office
    {{-68.65796, -800.895, 43.22729}, {-77.46159, -829.2974, 242.3858},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Office Block"}, -- Maze Tower Office 
    {{-1581.813, -557.4393, 33.95288}, {-1580.113, -562.4185, 107.5229},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Office Block"}, -- LOM Office 
    {{-774.0416, 310.4376, 84.69814}, {-781.7474, 326.2743, 175.8037},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Apartment"}, -- 501 Eclipse Towers Apartment 
    {{-60.63583, -615.8149, 35.96332}, {-17.36568, -588.2874, 89.11486},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Apartment"}, -- Integrity Way Apartment 395 
    {{-1441.904, -546.3289, 33.74182}, {-1451.891, -523.1708, 55.92904},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Apartment"}, -- Del Perro Heights Apartment 627 
    {{-937.7429, -379.7528, 37.9613}, {-913.1727, -365.4557, 113.2748},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Apartment"}, -- Richard Majestic Apartment 665 
   -- {{138.0231, -765.4586, 234.1522}, {135.2293, -764.5824, 45.75201},{36,237,157}, "Press ~INPUT_PICKUP~ to go up or down FIB"}, -- FIB Building 
   -- {{140.8344, -766.543, 45.75204}, {121.5451, -725.978, 254.1521},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Fire "}, -- FIB Fire Floor 

    {{785.7756, -1793.056, 29.87272}, {1026.425, -3101.559, -39.99994},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Warehouse"}, -- Large Warehouse
    -- {{757.0939,-1400.342, 25.53074}, {1072.304, -3102.627, -39.99994},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Warehouse"}, -- Medium Warehouse
    {{1018.545, -2515.695, 27.30198}, {1104.437, -3099.406, -39.99994},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Warehouse"}, -- Small Warehouse
    {{-1097.861, -848.1342, 3.884064}, {-1097.79, -848.4437, 12.68696},{36,237,157}, "Press ~INPUT_PICKUP~ to go up or down Vespucci"}, -- Vespucci Police Station
    {{-1389.256, -585.6901, 29.22098}, {-1391.6, -591.9924, 29.31956},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Nightclub"}, -- 628 Nightclub
    {{240.6268, -1379.6031, 32.7418}, {247.0954, -1372.1021, 23.5378},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Hospital"}, -- Former DVLA (Hospital Postal 140 )
    {{334.1679, -1433.499,  45.51108}, {313.8053, -1467.335, 28.97168},{36,237,157}, "Press ~INPUT_PICKUP~ to enter/exit Hospital"}, -- Royal Crusade helipad )
   

}
-----------------------------------------------------------------------------
-------------------------DO NOT EDIT BELOW THIS LINE-------------------------
-----------------------------------------------------------------------------

local player = GetPlayerPed(-1)

Citizen.CreateThread(function ()
    while true do
        Citizen.Wait(5)
        local player = GetPlayerPed(-1)
        local playerLoc = GetEntityCoords(player)

        for _,location in ipairs(positions) do
            teleport_text = location[4]
            loc1 = {
                x=location[1][1],
                y=location[1][2],
                z=location[1][3],
                heading=location[1][4]
            }
            loc2 = {
                x=location[2][1],
                y=location[2][2],
                z=location[2][3],
                heading=location[2][4]
            }
            Red = location[3][1]
            Green = location[3][2]
            Blue = location[3][3]

            DrawMarker(1, loc1.x, loc1.y, loc1.z, 0, 0, 0, 0, 0, 0, 1.501, 1.5001, 0.5001, Red, Green, Blue, 200, 0, 0, 0, 0)
            DrawMarker(1, loc2.x, loc2.y, loc2.z, 0, 0, 0, 0, 0, 0, 1.501, 1.5001, 0.5001, Red, Green, Blue, 200, 0, 0, 0, 0)

            if CheckPos(playerLoc.x, playerLoc.y, playerLoc.z, loc1.x, loc1.y, loc1.z, 2) then 
                alert(teleport_text)
                
                if IsControlJustReleased(1, key_to_teleport) then
                    if IsPedInAnyVehicle(player, true) then
                        SetEntityCoords(GetVehiclePedIsUsing(player), loc2.x, loc2.y, loc2.z)
                        SetEntityHeading(GetVehiclePedIsUsing(player), loc2.heading)
                    else
                        SetEntityCoords(player, loc2.x, loc2.y, loc2.z)
                        SetEntityHeading(player, loc2.heading)
                    end
                end

            elseif CheckPos(playerLoc.x, playerLoc.y, playerLoc.z, loc2.x, loc2.y, loc2.z, 2) then
                alert(teleport_text)

                if IsControlJustReleased(1, key_to_teleport) then
                    if IsPedInAnyVehicle(player, true) then
                        SetEntityCoords(GetVehiclePedIsUsing(player), loc1.x, loc1.y, loc1.z)
                        SetEntityHeading(GetVehiclePedIsUsing(player), loc1.heading)
                    else
                        SetEntityCoords(player, loc1.x, loc1.y, loc1.z)
                        SetEntityHeading(player, loc1.heading)
                    end
                end
            end            
        end
    end
end)

function CheckPos(x, y, z, cx, cy, cz, radius)
    local t1 = x - cx
    local t12 = t1^2

    local t2 = y-cy
    local t21 = t2^2

    local t3 = z - cz
    local t31 = t3^2

    return (t12 + t21 + t31) <= radius^2
end

function alert(msg)
    SetTextComponentFormat("STRING")
    AddTextComponentString(msg)
    DisplayHelpTextFromStringLabel(0,0,1,-1)
end