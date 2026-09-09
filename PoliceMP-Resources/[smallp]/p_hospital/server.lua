local occupiedBeds = {}
local hospitals = {
    {
        name = "St Thomas (East Wing)",
        checkinPos = vector3(348.5533, -580.2629, 28.80111),
        beds = {
            vector4(348.2198, -563.4766, 28.72371, -110.127),
            vector4(349.417, -559.6983, 28.7237, -108.8715),
            vector4(350.9783, -555.7922, 28.7237, -108.8689),
            vector4(355.3325, -557.304, 28.72368, 71.23456),
            vector4(353.8598, -561.2361, 28.7237, 68.54893)
        },
        healingEnabled = false
    },
    {
        name = "St Thomas (West Wing)",
        checkinPos = vector3(310.4552, -566.1929, 43.39661),
        beds = {
            vector4(352.0998, -575.0886, 43.3127, -158.1785),
            vector4(355.2678, -581.8766, 43.31274, 20.81948),
            vector4(358.9859, -580.1169, 43.31274, 19.44991),
            vector4(355.8441, -573.4001, 43.31274, -157.1367)
        },
        healingEnabled = false
    },
    {
        name = "Sandy Shores",
        checkinPos = vector3(1826.888, 3682.826, 34.28053),
        beds = {
            vector4(1826.306, 3668.599, 34.20523, 22.58408),
            vector4(1829.263, 3670.081, 34.20523, 32.5957),
            vector4(1823.419, 3666.85, 34.20523, 30.67315),
            vector4(1823.335, 3666.907, 34.20523, 29.76343),
            vector4(1819.621, 3673.261, 34.20523, -151.9968),
            vector4(1825.713, 3676.6, 34.20523, -153.4929)
        },
        healingEnabled = false
    },
    {
        name = "Victoria Medical",
        checkinPos = vector3(-250.6461, 6327.0, 32.45868),
        beds = {
            vector4(-252.3185, 6314.719, 32.38035, 138.7057),
            vector4(-250.1778, 6312.584, 32.38037, 137.7644),
            vector4(-258.5576, 6308.489, 32.38035, -46.8057),
            vector4(-260.7281, 6310.744, 32.38035, -38.62259),
            vector4(-256.4973, 6306.41, 32.38034, -44.53598)
        },
        healingEnabled = false
    },
    {
        name = "Mount Zonah",
        checkinPos = vector3(-437.9384, -324.9686, 34.9108),
        beds = {
            vector4(-459.0488, -279.6323, 34.83508, -154.7515),
            vector4(-462.7575, -281.2084, 34.83508, -152.2756),
            vector4(-455.1148, -278.0808, 34.83508, -159.9308),
            vector4(-448.3271, -283.695, 34.83318, 21.877),
            vector4(-451.451, -285.1343, 34.83321, 17.57132),
            vector4(-454.8612, -286.4753, 34.83322, 24.54674),
            vector4(-460.2418, -288.6709, 34.83321, 17.89731),
            vector4(-463.682, -290.106, 34.83321, 21.59733),
            vector4(-466.8876, -291.3628, 34.83509, 24.34126),
            vector4(-470.0132, -284.1581, 34.83509, -158.451),
            vector4(-466.4977, -282.6896, 34.83508, -158.854)
        },
        healingEnabled = false
    },
    {
        name = "Royal London",
        checkinPos = vector3(1129.528, -1535.839, 35.03271),
        beds = {
            vector4(1124.823, -1563.181, 34.9583, -4.947848),
            vector4(1124.892, -1554.688, 34.95829, -176.2631),
            vector4(1121.42, -1563.06, 34.95827, -0.11300),
            vector4(1121.40, -1554.689, 34.95829, 177.4816),
            vector4(1117.852, -1554.541, 34.95829, 179.7513),
            vector4(1117.907, -1563.305, 34.95829, -2.238376)
        },
        healingEnabled = false
    },
    {
        name = "Royal Crusade",
        checkinPos = vector3(294.0645, -1454.602, 29.97171),
        beds = {
            vector4(297.228, -1461.202, 29.87913, -39.4981),
            vector4(300.849, -1464.224, 29.87781, -43.24348),
            vector4(304.5696, -1467.44, 29.87468, -45.33517)
        },
        healingEnabled = false
    }
}

function IsParamedic(playerId)
    return IsPlayerAceAllowed(playerId, "nhs.paramedic") 
        or IsPlayerAceAllowed(playerId, "nhs.lasStudent") 
        or IsPlayerAceAllowed(playerId, "nhs.hems")
        or IsPlayerAceAllowed(playerId, "nhs.hart")
end

function TriggerClientEventExcludingSource(eventName, source, ...)
    for _, playerId in ipairs(GetPlayers()) do
        if tonumber(playerId) ~= source then
            TriggerClientEvent(eventName, playerId, ...)
        end
    end
end

RegisterCommand("+xhcheckin", function(source)
    if not IsParamedic(source) then
        TriggerClientEvent("hospital:drawNotification", source, "~r~You are not authorised to check in.")
        return
    end

    local ped = GetPlayerPed(source)
    local coords = GetEntityCoords(ped)

    local foundHospital = nil

    for i, hospital in ipairs(hospitals) do
        if #(coords - hospital.checkinPos) < 3.0 then
            foundHospital = i
            break
        end
    end

    if foundHospital then
        local hospital = hospitals[foundHospital]
        if hospital.healingEnabled then
            TriggerClientEvent("hospital:drawNotification", source, "~r~Healing beds are already enabled here.")
            return
        end

        hospital.healingEnabled = true

        TriggerClientEventExcludingSource("hospital:updateHealingState", source, hospital.name, true, hospital.beds)
        TriggerClientEvent("hospital:drawNotification", source, "Healing beds enabled at " .. hospital.name .. ".")

        SetTimeout(3 * 60 * 1000, function()
            hospital.healingEnabled = false
            TriggerClientEvent("hospital:updateHealingState", -1, hospital.name, false, hospital.beds)            
        end)
    else
        TriggerClientEvent("hospital:drawNotification", source, "~r~You are not near any Reception to check in.")
    end
end, false)

function SetBedOccupied(hospitalName, bedIndex, occupied, player)
    if not occupiedBeds[hospitalName] then
        occupiedBeds[hospitalName] = {}
    end

    if occupied then
        occupiedBeds[hospitalName][bedIndex] = player
    else
        occupiedBeds[hospitalName][bedIndex] = nil
    end

    TriggerClientEvent("hospital:updateBedStatus", -1, hospitalName, bedIndex, occupied ~= nil)
end

function IsBedOccupied(hospitalName, bedIndex)
    if not occupiedBeds[hospitalName] then return false end
    return occupiedBeds[hospitalName][bedIndex] or false
end

RegisterNetEvent("hospital:requestUseBed", function(hospitalName, bedIndex)
    local src = source

    if occupiedBeds[hospitalName] and occupiedBeds[hospitalName][bedIndex] then
        TriggerClientEvent("hospital:bedOccupied", src)
    else
        SetBedOccupied(hospitalName, bedIndex, true, src)
        TriggerClientEvent("hospital:startHealing", src)
    end
end)

RegisterNetEvent("hospital:freeBed", function(hospitalName, bedIndex)
    SetBedOccupied(hospitalName, bedIndex, false)
end)


RegisterNetEvent("hospital:requestCheckinLocations", function()
    local checkins = {}
    for _, hospital in ipairs(hospitals) do
        table.insert(checkins, { x = hospital.checkinPos.x, y = hospital.checkinPos.y, z = hospital.checkinPos.z })
    end
    TriggerClientEvent("hospital:setCheckinLocations", source, checkins)
end)

AddEventHandler("playerDropped", function(reason)
    local src = source

    for hospitalName, beds in pairs(occupiedBeds) do
        for bedIndex, player in pairs(beds) do
            if player == src then
                --print("[Hospital] Player " .. src .. " disconnected, freeing bed " .. bedIndex .. " at " .. hospitalName)
                SetBedOccupied(hospitalName, bedIndex, false)
            end
        end
    end
end)
