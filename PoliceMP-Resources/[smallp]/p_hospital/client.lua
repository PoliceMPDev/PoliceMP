local hospitalBeds = {}
local isHealing = false
local hospitalCheckins = {}
local occupiedBeds = {}

RegisterNetEvent("hospital:setCheckinLocations", function(checkins)
    hospitalCheckins = checkins
end)

RegisterNetEvent("hospital:updateHealingState", function(hospitalName, enabled, beds)
    if enabled then
        hospitalBeds[hospitalName] = beds
    else
        hospitalBeds[hospitalName] = nil
    end
end)

RegisterNetEvent("hospital:drawNotification", function(message)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(message)
    DrawNotification(false, true)
end)

CreateThread(function()
    while true do
        Wait(0)

        if isHealing then
            goto continue
        end

        for _, pos in ipairs(hospitalCheckins) do
            DrawMarker(30, pos.x, pos.y, pos.z + 0.5 - 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.5, 0.5, 0.5, 255, 255, 255, 200, true, true, 2, false, nil, nil, false)
        end      

        local playerPed = PlayerPedId()
        local playerCoords = GetEntityCoords(playerPed)

        for hospitalName, beds in pairs(hospitalBeds) do
            for bedIndex, pos in ipairs(beds) do
                local isOccupied = occupiedBeds[hospitalName] and occupiedBeds[hospitalName][bedIndex]

                if #(playerCoords - vector3(pos.x, pos.y, pos.z)) < 2.5 then
                    if isOccupied then
                        DrawText3D(pos.x, pos.y, pos.z + 0.5, "~r~Occupied")
                    else
                        DrawText3D(pos.x, pos.y, pos.z + 0.5, "~g~[E]~w~ Start Healing")

                        if IsControlJustPressed(0, 38) then -- E key
                            if IsEntityAttached(playerPed) then
                                DetachEntity(playerPed, true, true)
                                ClearPedTasksImmediately(playerPed)
                                Wait(200)
                            end

                            pendingBedPos = pos
                            TriggerServerEvent("hospital:requestUseBed", hospitalName, bedIndex)
                        end
                    end
                end
            end
        end

        ::continue::
    end
end)

function StartHealingProcess(bedPos)
    local ped = PlayerPedId()

    isHealing = true

    SetEntityCoords(ped, bedPos.x, bedPos.y, bedPos.z, false, false, false, true)
    SetEntityHeading(ped, bedPos.w)
    FreezeEntityPosition(ped, true)

    ExecuteCommand("e passout3")

    local healTime = 120 -- 2 minutes

    CreateThread(function()
        while healTime > 0 and isHealing do
            if healTime % 30 == 0 then
                TriggerEvent("hospital:drawNotification", "Healing... " .. healTime .. " seconds remaining.")
            end
            Wait(1000)
            healTime = healTime - 1
        end
    end)

    SetTimeout(2 * 60 * 1000, function()
        if isHealing then
            SetEntityHealth(ped, GetEntityMaxHealth(ped))
            ExecuteCommand("+trsac")
            FreezeEntityPosition(ped, false)
            TriggerEvent("hospital:drawNotification", "You have been fully healed.")

            isHealing = false

            -- Free the bed
            local hospitalName = GetHospitalNameByBed(bedPos)
            local bedIndex = GetBedIndexByPos(bedPos)

            if hospitalName and bedIndex then
                TriggerServerEvent("hospital:freeBed", hospitalName, bedIndex)
            end
        end
    end)
end

function DrawText3D(x, y, z, text)
    SetTextScale(0.35, 0.35)
    SetTextFont(4)
    SetTextProportional(1)
    SetTextColour(255, 255, 255, 215)
    SetTextEntry("STRING")
    SetTextCentre(1)
    AddTextComponentString(text)
    SetDrawOrigin(x, y, z, 0)
    DrawText(0.0, 0.0)
    ClearDrawOrigin()
end

CreateThread(function()
    Wait(2000)
    TriggerServerEvent("hospital:requestCheckinLocations")
end)

RegisterNetEvent("hospital:updateBedStatus", function(hospitalName, bedIndex, occupied)
    if not occupiedBeds[hospitalName] then
        occupiedBeds[hospitalName] = {}
    end
    occupiedBeds[hospitalName][bedIndex] = occupied
end)

RegisterNetEvent("hospital:bedOccupied", function()
    TriggerEvent("hospital:drawNotification", "~r~That bed is currently occupied.")
end)

RegisterNetEvent("hospital:startHealing", function()
    StartHealingProcess(pendingBedPos)
end)

function GetHospitalNameByBed(bedPos)
    for hospitalName, beds in pairs(hospitalBeds) do
        for i, pos in ipairs(beds) do
            if pos.x == bedPos.x and pos.y == bedPos.y and pos.z == bedPos.z then
                return hospitalName
            end
        end
    end
    return nil
end

function GetBedIndexByPos(bedPos)
    for hospitalName, beds in pairs(hospitalBeds) do
        for i, pos in ipairs(beds) do
            if pos.x == bedPos.x and pos.y == bedPos.y and pos.z == bedPos.z then
                return i
            end
        end
    end
    return nil
end
