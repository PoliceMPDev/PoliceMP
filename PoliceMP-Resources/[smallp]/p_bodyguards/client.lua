function EnsurePedOwnership(ped)
    local timeout = 0
    while not NetworkHasControlOfEntity(ped) and timeout < 50 do -- Max 500ms
        NetworkRequestControlOfEntity(ped)
        Citizen.Wait(10)
        timeout = timeout + 1
    end
    return NetworkHasControlOfEntity(ped)
end

RegisterNetEvent("bodyguard:spawnGuards", function()
    local BodyGuardSkinID = GetHashKey(Bodyguard.GuardSkin)
    local playerPed = PlayerPedId()
    local playerPosition = GetOffsetFromEntityInWorldCoords(playerPed, 0.0, 5.0, 0.0)
    local playerGroup = GetPedGroupIndex(playerPed)

    -- ✅ Force player as group leader
    SetPedAsGroupLeader(playerPed, playerGroup)
    SetGroupSeparationRange(playerGroup, 999.0)

    if not Bodyguard.SpawnMultiple then
        UnloadBodyguard()
    end

    RequestModel(BodyGuardSkinID)
    while not HasModelLoaded(BodyGuardSkinID) do
        Citizen.Wait(10)
    end

    local spawnCount = Bodyguard.GuardAmount + 1
    for i = 1, spawnCount do
        local ped = CreatePed(26, BodyGuardSkinID, playerPosition.x + (i * 1.5), playerPosition.y, playerPosition.z, 1.0, true, true)

        -- ✅ Ensure network ownership
        EnsurePedOwnership(ped)
        local netId = NetworkGetNetworkIdFromEntity(ped)
        SetNetworkIdExistsOnAllMachines(netId, true)
        SetNetworkIdCanMigrate(netId, false)

        Bodyguard.Guards[i] = ped

        -- ✅ Set up ped
        SetPedCanSwitchWeapon(ped, false)
        SetPedAsGroupMember(ped, playerGroup)
        SetPedNeverLeavesGroup(ped, true)
        SetEntityInvincible(ped, Bodyguard.SetInvincible)

        if Bodyguard.GiveWeapon then
            GiveWeaponToPed(ped, GetHashKey(Bodyguard.GuardWeapon), 100, true, true)
            local playerWeapon = GetSelectedPedWeapon(playerPed)
            if playerWeapon == GetHashKey("WEAPON_UNARMED") then
                SetCurrentPedWeapon(ped, GetHashKey("WEAPON_UNARMED"), true)
            end
        end
    end

    SetModelAsNoLongerNeeded(BodyGuardSkinID)
end)

RegisterNetEvent("bodyguard:clearGuards", function()
    for k, guard in pairs(Bodyguard.Guards) do
        if DoesEntityExist(guard) then
            DeletePed(guard)
        end
        Bodyguard.Guards[k] = nil
    end
end)

-- ✅ Background: Reassert ownership if dropped
Citizen.CreateThread(function()
    while true do
        for _, guard in pairs(Bodyguard.Guards) do
            if guard and DoesEntityExist(guard) and not NetworkHasControlOfEntity(guard) then
                NetworkRequestControlOfEntity(guard)
            end
        end
        Citizen.Wait(3000)
    end
end)

-- ✅ Background: Re-add to group if kicked
Citizen.CreateThread(function()
    while true do
        local playerPed = PlayerPedId()
        local playerGroup = GetPedGroupIndex(playerPed)

        for _, guard in pairs(Bodyguard.Guards) do
            if guard and DoesEntityExist(guard) and not IsPedInGroup(guard) then
                SetPedAsGroupMember(guard, playerGroup)
                SetPedNeverLeavesGroup(guard, true)
            end
        end

        Citizen.Wait(3000)
    end
end)
