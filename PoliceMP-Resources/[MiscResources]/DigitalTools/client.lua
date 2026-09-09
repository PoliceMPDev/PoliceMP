local EmoteList = EmoteList
local previewClone = nil
local placingClone = false
local offset = vector3(0.0, 0.0, 0.0)
local currentHeading = 0.0
local placedClones = {}
local cachedData = {}

function ShowCloneSubtitle(msg, duration)
    BeginTextCommandPrint("STRING")
    AddTextComponentSubstringPlayerName(msg)
    EndTextCommandPrint(duration or 1000, true)
end

RegisterNetEvent("clone:requestData")
AddEventHandler("clone:requestData", function(animKey, targetPlayer, gunClone)
    local ped = PlayerPedId()
    local model = GetEntityModel(ped)
    local coords = GetEntityCoords(ped)
    local heading = GetEntityHeading(ped)

    local appearance = {}
    for i = 0, 11 do
        appearance[i] = {
            drawable = GetPedDrawableVariation(ped, i),
            texture = GetPedTextureVariation(ped, i)
        }
    end

    local headBlendData = nil
    local success, s1, s2, s3, sk1, sk2, sk3, sm, skm, tm = GetPedHeadBlendData(ped)
    if success then
        headBlendData = {
            shapeFirst = s1, shapeSecond = s2, shapeThird = s3,
            skinFirst = sk1, skinSecond = sk2, skinThird = sk3,
            shapeMix = sm, skinMix = skm, thirdMix = tm
        }
    end

    local hairColor, hairHighlight = 0, 0
    local eyeColor = GetPedEyeColor(ped)

    local overlays = {}
    for i = 0, 12 do
        local index = GetPedHeadOverlayValue(ped, i)
        overlays[i] = {
            index = index,
            opacity = 1.0,
            colorType = 1,
            color = 0,
            highlight = 0
        }
    end

    local props = {}
    for i = 0, 7 do
        props[i] = {
            drawable = GetPedPropIndex(ped, i),
            texture = GetPedPropTextureIndex(ped, i)
        }
    end

    local animDict, animName = nil, nil
    if animKey and EmoteList[animKey] then
        animDict = EmoteList[animKey].dict
        animName = EmoteList[animKey].name
    end

    -- Store all for use when networked ped is created
    cachedData = {
        model = model,
        coords = coords,
        heading = heading,
        appearance = appearance,
        headBlend = headBlendData,
        hairColor = hairColor,
        hairHighlight = hairHighlight,
        eyeColor = eyeColor,
        overlays = overlays,
        props = props,
        animDict = animDict,
        animName = animName
    }

    local spawnX = coords.x
    local groundZ = coords.z
    local success, z = GetGroundZFor_3dCoord(spawnX, coords.y, coords.z + 5.0, 0)
    if success then groundZ = z end

    if targetPlayer then
        TriggerServerEvent("clone:createNetworkedPedFor", targetPlayer, model, vector3(spawnX, coords.y, groundZ), heading, cachedData, gunClone)
    else
        TriggerServerEvent("clone:createNetworkedPed", model, vector3(spawnX, coords.y, groundZ), heading)
    end
end)

RegisterNetEvent("clone:networkedPedCreated")
AddEventHandler("clone:networkedPedCreated", function(netId)
    local clone = NetworkGetEntityFromNetworkId(netId)
    if not DoesEntityExist(clone) then return end

    -- Apply all visual data
    local data = cachedData
    if data.headBlend then
        SetPedHeadBlendData(clone,
            data.headBlend.shapeFirst, data.headBlend.shapeSecond, data.headBlend.shapeThird,
            data.headBlend.skinFirst, data.headBlend.skinSecond, data.headBlend.skinThird,
            data.headBlend.shapeMix, data.headBlend.skinMix, data.headBlend.thirdMix, false
        )
    end

    SetPedHairColor(clone, data.hairColor, data.hairHighlight)
    SetPedEyeColor(clone, data.eyeColor)

    for i = 0, 12 do
        local o = data.overlays[i]
        SetPedHeadOverlay(clone, i, o.index, o.opacity)
        SetPedHeadOverlayColor(clone, i, o.colorType, o.color, o.highlight)
    end

    for i = 0, 11 do
        SetPedComponentVariation(clone, i, data.appearance[i].drawable, data.appearance[i].texture, 0)
    end

    for i = 0, 7 do
        local p = data.props[i]
        if p and p.drawable ~= -1 then
            SetPedPropIndex(clone, i, p.drawable, p.texture, true)
        else
            ClearPedProp(clone, i)
        end
    end

    FreezeEntityPosition(clone, false)
    SetEntityInvincible(clone, false)
    SetBlockingOfNonTemporaryEvents(clone, true)

    if data.animDict and data.animName then
        RequestAnimDict(data.animDict)
        while not HasAnimDictLoaded(data.animDict) do Wait(0) end
        TaskPlayAnim(clone, data.animDict, data.animName, 8.0, -8.0, -1, 1, 0, false, false, false)
    end

    SetEntityAlpha(clone, 150, false)
    previewClone = clone
    placingClone = true
    currentHeading = data.heading
    offset = vector3(1.0, 0.0, 0.0)
end)

CreateThread(function()
    while true do
        Wait(0)
        if placingClone and previewClone then
            local baseCoords = GetEntityCoords(PlayerPedId())
            local finalPos = baseCoords + offset
            SetEntityCoordsNoOffset(previewClone, finalPos.x, finalPos.y, finalPos.z, false, false, false)
            SetEntityHeading(previewClone, currentHeading)

            ShowCloneSubtitle(
            "~c~Q/E~s~ Up/Down  ~c~NUM 7/9~s~ Rotate" ..
            "~n~~c~NUM 8/5/4/6~s~ Move (F/B/L/R)" ..
            "~n~~c~ENTER~s~ Place Clone" ..
            "   ~c~ESC~s~ Cancel", 0)

            if IsControlPressed(0, 111) then offset = offset + vector3(0.0, 0.1, 0.0) end
            if IsControlPressed(0, 112) then offset = offset - vector3(0.0, 0.1, 0.0) end
            if IsControlPressed(0, 108) then offset = offset - vector3(0.1, 0.0, 0.0) end
            if IsControlPressed(0, 109) then offset = offset + vector3(0.1, 0.0, 0.0) end
            if IsControlPressed(0, 44)  then offset = offset + vector3(0.0, 0.0, 0.1) end
            if IsControlPressed(0, 38)  then offset = offset - vector3(0.0, 0.0, 0.1) end
            if IsControlPressed(0, 117) then currentHeading = currentHeading - 1.0 end
            if IsControlPressed(0, 118) then currentHeading = currentHeading + 1.0 end

            if IsControlJustPressed(0, 191) then -- ENTER
                FreezeEntityPosition(previewClone, false)
                SetEntityAlpha(previewClone, 255, false)

                local finalCoords = GetEntityCoords(previewClone)
                local finalHeading = GetEntityHeading(previewClone)
                local netId = NetworkGetNetworkIdFromEntity(previewClone)
                SetNetworkIdCanMigrate(netId, true)

                TriggerServerEvent("clone:syncPlacedClone", netId, finalCoords, finalHeading,
                    cachedData.appearance, cachedData.headBlend, cachedData.hairColor,
                    cachedData.hairHighlight, cachedData.eyeColor, cachedData.overlays, cachedData.props)

                table.insert(placedClones, previewClone)
                placingClone = false
                previewClone = nil
                offset = vector3(0.0, 0.0, 0.0)
            end

            if IsControlJustPressed(0, 322) then -- ESC
                DeleteEntity(previewClone)
                placingClone = false
                previewClone = nil
                offset = vector3(0.0, 0.0, 0.0)
            end
        end
    end
end)

RegisterNetEvent("clone:createNetworkedPedClient")
AddEventHandler("clone:createNetworkedPedClient", function(model, coords, heading)
    RequestModel(model)
    while not HasModelLoaded(model) do Wait(0) end

    local ped = CreatePed(4, model, coords.x, coords.y, coords.z, heading, true, true)
    local netId = NetworkGetNetworkIdFromEntity(ped)
    SetNetworkIdCanMigrate(netId, true)
    SetEntityAsMissionEntity(ped, true, true)

    -- Continue setup using the existing logic:
    TriggerEvent("clone:networkedPedCreated", netId)
end)

RegisterNetEvent("clone:placeClone")
AddEventHandler("clone:placeClone", function(netId, coords, heading, appearance, headBlend, hairColor, hairHighlight, eyeColor, overlays, props)
    local clone = NetworkGetEntityFromNetworkId(netId)
    if not DoesEntityExist(clone) then return end

    -- Apply position
    SetEntityCoordsNoOffset(clone, coords.x, coords.y, coords.z, false, false, false)
    SetEntityHeading(clone, heading)
    FreezeEntityPosition(clone, false)
    SetEntityAlpha(clone, 255, false)

    -- Apply head blend
    if headBlend then
        SetPedHeadBlendData(clone,
            headBlend.shapeFirst, headBlend.shapeSecond, headBlend.shapeThird,
            headBlend.skinFirst, headBlend.skinSecond, headBlend.skinThird,
            headBlend.shapeMix, headBlend.skinMix, headBlend.thirdMix, false
        )
    end

    Wait(50)
    SetPedHairColor(clone, hairColor, hairHighlight)
    SetPedEyeColor(clone, eyeColor)

    for i = 0, 12 do
        local data = overlays[i]
        SetPedHeadOverlay(clone, i, data.index, data.opacity)
        SetPedHeadOverlayColor(clone, i, data.colorType, data.color, data.highlight)
    end

    for i = 0, 11 do
        SetPedComponentVariation(clone, i, appearance[i].drawable, appearance[i].texture, 0)
    end

    for i = 0, 7 do
        local prop = props[i]
        if prop and prop.drawable ~= -1 then
            SetPedPropIndex(clone, i, prop.drawable, prop.texture, true)
        else
            ClearPedProp(clone, i)
        end
    end
end)

RegisterNetEvent("clone:clearAll")
AddEventHandler("clone:clearAll", function()
    for _, ped in ipairs(placedClones) do
        if DoesEntityExist(ped) then
            DeleteEntity(ped)
        end
    end
    placedClones = {}
end)

RegisterNetEvent("clone:createNetworkedPedClientFor")
AddEventHandler("clone:createNetworkedPedClientFor", function(model, coords, heading, data, gunClone)
    RequestModel(model)
    while not HasModelLoaded(model) do Wait(0) end

local ped = CreatePed(4, model, coords.x, coords.y, coords.z, heading, true, true)
SetEntityAsMissionEntity(ped, true, true)

local netId = NetworkGetNetworkIdFromEntity(ped)
SetNetworkIdCanMigrate(netId, false)
SetEntityCanBeDamaged(ped, true)
SetBlockingOfNonTemporaryEvents(ped, true)
SetPedKeepTask(ped, true)

-- Optional: force local control
if not NetworkHasControlOfEntity(ped) then
    NetworkRequestControlOfEntity(ped)
    local timeout = GetGameTimer() + 1000
    while not NetworkHasControlOfEntity(ped) and GetGameTimer() < timeout do
        Wait(10)
    end
end


    -- Cache the received data for placement system
    cachedData = data

    -- Apply appearance
    if data.headBlend then
        SetPedHeadBlendData(ped,
            data.headBlend.shapeFirst, data.headBlend.shapeSecond, data.headBlend.shapeThird,
            data.headBlend.skinFirst, data.headBlend.skinSecond, data.headBlend.skinThird,
            data.headBlend.shapeMix, data.headBlend.skinMix, data.headBlend.thirdMix, false
        )
    end

    SetPedHairColor(ped, data.hairColor, data.hairHighlight)
    SetPedEyeColor(ped, data.eyeColor)

    for i = 0, 12 do
        local o = data.overlays[i]
        SetPedHeadOverlay(ped, i, o.index, o.opacity)
        SetPedHeadOverlayColor(ped, i, o.colorType, o.color, o.highlight)
    end

    for i = 0, 11 do
        SetPedComponentVariation(ped, i, data.appearance[i].drawable, data.appearance[i].texture, 0)
    end

    for i = 0, 7 do
        local p = data.props[i]
        if p and p.drawable ~= -1 then
            SetPedPropIndex(ped, i, p.drawable, p.texture, true)
        else
            ClearPedProp(ped, i)
        end
    end

    FreezeEntityPosition(ped, false)
    SetEntityInvincible(ped, false)
    SetBlockingOfNonTemporaryEvents(ped, true)

    if data.animDict and data.animName then
        RequestAnimDict(data.animDict)
        while not HasAnimDictLoaded(data.animDict) do Wait(0) end
        TaskPlayAnim(ped, data.animDict, data.animName, 8.0, -8.0, -1, 1, 0, false, false, false)
    end

if gunClone then
    -- Weapon + group setup
    GiveWeaponToPed(ped, `WEAPON_SPECIALCARBINE`, 250, false, true)
    SetPedAsGroupMember(ped, GetPedGroupIndex(PlayerPedId()))
    SetPedCombatAttributes(ped, 5, true)
    SetPedCombatAttributes(ped, 12, true)
    SetPedCombatAttributes(ped, 42, true)
    SetPedCombatAttributes(ped, 43, true)
    SetPedCombatAttributes(ped, 44, true)
    SetPedCombatAbility(ped, 1)
    SetPedCombatRange(ped, 3)
    
    -- Fix relationship so clone doesn't shoot you
    SetPedRelationshipGroupHash(ped, `PLAYER`)
    SetRelationshipBetweenGroups(1, `PLAYER`, `PLAYER`)
    SetRelationshipBetweenGroups(1, GetHashKey("PLAYER"), GetPedRelationshipGroupHash(ped))
    SetRelationshipBetweenGroups(1, GetPedRelationshipGroupHash(ped), GetHashKey("PLAYER"))

    -- AI logic: defend and assist
    CreateThread(function()
        local playerPed = PlayerPedId()

        while DoesEntityExist(ped) do
            Wait(500)

            local _, playerTarget = GetEntityPlayerIsFreeAimingAt(PlayerId())
            local inCombat = IsPedInCombat(playerPed, 0)
            local nearbyPeds = GetGamePool("CPed")

            for _, target in ipairs(nearbyPeds) do
                if DoesEntityExist(target) and target ~= ped and not IsPedDeadOrDying(target) then
                    local rel = GetRelationshipBetweenPeds(ped, target)

                    local shouldAttack =
                        IsPedInCombat(target, ped) or
                        IsPedInCombat(target, playerPed) or
                        (playerTarget == target and IsPlayerFreeAiming(PlayerId())) or
                        (inCombat and rel == 5)

                    if shouldAttack then
                        TaskCombatPed(ped, target, 0, 16)
                        break
                    end
                end
            end
        end
    end)
end -- ✅ Closes if gunClone

-- ✅ This always runs for placement preview
SetEntityAlpha(ped, 150, false)
previewClone = ped
placingClone = true
currentHeading = heading
offset = vector3(1.0, 0.0, 0.0)
end)