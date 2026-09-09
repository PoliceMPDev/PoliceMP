local hasDroppedHat = false
local droppedHat = nil
local cachedHat = { drawable = -1, texture = -1 }
local lastHealth = nil

CreateThread(function()
    while true do
        Wait(500)
        local ped = PlayerPedId()
        local hat = GetPedPropIndex(ped, 0)

        if hat ~= -1 then
            cachedHat.drawable = hat
            cachedHat.texture = GetPedPropTextureIndex(ped, 0)
        end

        if lastHealth == nil then
            lastHealth = GetEntityHealth(ped)
        end

        local currentHealth = GetEntityHealth(ped)

        if currentHealth < lastHealth then
            if cachedHat.drawable ~= -1 and not hasDroppedHat then
                local attackers = GetGamePool("CPed")
                local isMelee = false

                for _, otherPed in ipairs(attackers) do
                    if otherPed ~= ped and not IsEntityDead(otherPed) and HasEntityClearLosToEntity(ped, otherPed, 17) then
                        if #(GetEntityCoords(otherPed) - GetEntityCoords(ped)) < 2.5 and IsPedInMeleeCombat(otherPed) then
                            isMelee = true
                            break
                        end
                    end
                end

                if isMelee then
                    DetectDroppedHat()
                end
            end

            lastHealth = currentHealth
            Wait(3000)
        else
            lastHealth = currentHealth
        end
    end
end)

function DetectDroppedHat()
    local ped = PlayerPedId()

    CreateThread(function()
        local searchTime = GetGameTimer() + 10000 -- search for 10 seconds max

        while hasDroppedHat == false do
            Wait(500)

            if GetPedPropIndex(ped, 0) ~= -1 then
                -- player put hat back on → stop
                return
            end

            local nearbyObjects = GetGamePool("CObject")
            local coords = GetEntityCoords(ped)

            local found = false

            for _, obj in ipairs(nearbyObjects) do
                if DoesEntityExist(obj) and not IsEntityAttached(obj) then
                    local objCoords = GetEntityCoords(obj)
                    local dist = #(coords - objCoords)

                    if dist < 2.0 then
                        droppedHat = obj
                        hasDroppedHat = true
                        found = true
                        return
                    end
                end
            end

            -- if not found after 10 seconds
            if GetGameTimer() > searchTime and not found then
                ShowNotification("Your hat / helmet has rolled into a ditch. Can't be found.")
                return
            end
        end
    end)
end

CreateThread(function()
    while true do
        Wait(0)

        if hasDroppedHat and droppedHat and DoesEntityExist(droppedHat) then
            local ped = PlayerPedId()
            local coords = GetEntityCoords(ped)
            local hatCoords = GetEntityCoords(droppedHat)

            if #(coords - hatCoords) < 2.0 then
                Draw3DText(hatCoords.x, hatCoords.y, hatCoords.z + 0.3, "~g~[E]~w~ Pick up hat")

                if IsControlJustReleased(0, 38) then
                    TriggerServerEvent("hat:pickup", GetPlayerServerId(PlayerId()), cachedHat.drawable, cachedHat.texture)
                end
            end
        end
    end
end)

function Draw3DText(x, y, z, text)
    local onScreen, _x, _y = World3dToScreen2d(x, y, z)
    local camCoords = GetGameplayCamCoords()
    local dist = #(vector3(x, y, z) - camCoords)
    local scale = (1 / dist) * 2.0 * (1 / GetGameplayCamFov()) * 100

    if onScreen then
        SetTextScale(0.25 * scale, 0.25 * scale)
        SetTextFont(0)
        SetTextProportional(1)
        SetTextOutline()
        SetTextColour(255, 255, 255, 255)
        SetTextCentre(true)
        BeginTextCommandDisplayText("STRING")
        AddTextComponentSubstringPlayerName(text)
        EndTextCommandDisplayText(_x, _y)
    end
end

-- 👇 Picker only → equip hat + animation
RegisterNetEvent("hat:pickupEquip")
AddEventHandler("hat:pickupEquip", function(drawable, texture)
    local ped = PlayerPedId()

    RequestAnimDict("random@domestic")
    while not HasAnimDictLoaded("random@domestic") do Wait(10) end
    TaskPlayAnim(ped, "random@domestic", "pickup_low", 8.0, -8.0, 1000, 0, 0, false, false, false)

    Wait(1000)
    RequestAnimDict("mp_masks@standard_car@ds@")
    while not HasAnimDictLoaded("mp_masks@standard_car@ds@") do Wait(10) end
    TaskPlayAnim(ped, "mp_masks@standard_car@ds@", "put_on_mask", 8.0, 8.0, 600, 51, 1.0, false, false, false)
    Wait(600)

    SetPedPropIndex(ped, 0, drawable, texture, true)

    if droppedHat and DoesEntityExist(droppedHat) then
        SetEntityAsMissionEntity(droppedHat, true, true)
        for i = 1, 10 do
            DeleteEntity(droppedHat)
            Wait(100)
            if not DoesEntityExist(droppedHat) then
                break
            end
        end
    end

    droppedHat = nil
    hasDroppedHat = false
end)

-- 👇 Everyone → pickup animation only
RegisterNetEvent("hat:pickupAnim")
AddEventHandler("hat:pickupAnim", function(sourceId)
    local pickerPlayer = GetPlayerFromServerId(sourceId)
    local pickerPed = GetPlayerPed(pickerPlayer)
    if not DoesEntityExist(pickerPed) then return end

    RequestAnimDict("random@domestic")
    while not HasAnimDictLoaded("random@domestic") do Wait(10) end
    TaskPlayAnim(pickerPed, "random@domestic", "pickup_low", 8.0, -8.0, 1000, 0, 0, false, false, false)
end)

RegisterNetEvent("hat:forceStart")
AddEventHandler("hat:forceStart", function()
    if _G.hatScriptStarted then return end
    _G.hatScriptStarted = true
end)

function ShowNotification(msg)
    SetNotificationTextEntry("STRING")
    AddTextComponentSubstringPlayerName(msg)
    DrawNotification(false, false)
end
