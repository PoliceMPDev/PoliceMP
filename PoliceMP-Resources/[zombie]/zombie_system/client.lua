local Shooting = false
local Running = false
local isGrowlPlaying = false
local zoneBlips = {}

local SafeZones = {
    {name = "bishopgate", x = 450.5966, y = -998.9636, z = 28.4284, radius = 80.0},
    {name = "sandy", x = 1853.6666, y = 3688.0222, z = 33.2777, radius = 40.0},
    {name = "paleto", x = -460.5692, y = 6001.378, z = 31.34046, radius = 40.0}
}

local safeZonesEnabled = {
    bishopgate = false,
    sandy = false,
    paleto = false
}

-- Manage SafeZone blips based on active state
RegisterNetEvent("zombies:updateSafeZoneState")
AddEventHandler("zombies:updateSafeZoneState", function(zoneName, state)
    safeZonesEnabled[zoneName] = state

    -- Find the zone by name
    for _, zone in pairs(SafeZones) do
        if zone.name == zoneName then
            -- Remove existing blip if it exists
            if zoneBlips[zoneName] then
                RemoveBlip(zoneBlips[zoneName])
                zoneBlips[zoneName] = nil
            end

            -- Create blip if enabled
            if state then
                local blip = AddBlipForRadius(zone.x, zone.y, zone.z, zone.radius)
                SetBlipHighDetail(blip, true)
                SetBlipColour(blip, 2)
                SetBlipAlpha(blip, 128)
                zoneBlips[zoneName] = blip
            end
        end
    end
end)

function GetClosestPlayerToZombie(zombieCoords)
    local closestPlayer = nil
    local closestDistance = -1

    for _, playerId in ipairs(GetActivePlayers()) do
        local ped = GetPlayerPed(playerId)
        if ped and ped ~= 0 and not IsPedDeadOrDying(ped, true) then
            local dist = #(GetEntityCoords(ped) - zombieCoords)
            if closestDistance == -1 or dist < closestDistance then
                closestDistance = dist
                closestPlayer = ped
            end
        end
    end

    return closestPlayer, closestDistance
end

DecorRegister('RegisterZombie', 2)
DecorRegister('ZombieScheduledForCleanup', 2)

AddRelationshipGroup('ZOMBIE')
SetRelationshipBetweenGroups(0, GetHashKey('ZOMBIE'), GetHashKey('PLAYER'))
SetRelationshipBetweenGroups(5, GetHashKey('PLAYER'), GetHashKey('ZOMBIE'))

function IsPlayerShooting()
    return Shooting
end

function IsPlayerRunning()
    return Running
end

function PlayZombieSound(type)
    SendNUIMessage({ type = type })
end

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        SetPedDensityMultiplierThisFrame(1.0)
        SetScenarioPedDensityMultiplierThisFrame(1.0, 1.0)
        SetRandomVehicleDensityMultiplierThisFrame(0.0)
        SetParkedVehicleDensityMultiplierThisFrame(0.0)
        SetVehicleDensityMultiplierThisFrame(0.0)
    end
end)

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)
        if IsPedShooting(PlayerPedId()) then
            Shooting = true
            Citizen.Wait(5000)
            Shooting = false
        end

        if IsPedSprinting(PlayerPedId()) or IsPedRunning(PlayerPedId()) then
            if Running == false then Running = true end
        else
            if Running == true then Running = false end
        end
    end
end)

Citizen.CreateThread(function()
    for _, zone in pairs(SafeZones) do
        TriggerEvent("zombies:updateSafeZoneState", zone.name, safeZonesEnabled[zone.name])
    end
end)

-- Zombie AI and safe zone enforcement
Citizen.CreateThread(function()
    while true do
        Citizen.Wait(0)

        -- Remove zombies inside safe zones
        for _, zone in pairs(SafeZones) do
            if safeZonesEnabled[zone.name] then
                local Handler, Zombie = FindFirstPed()
                local Success = true
                repeat
                    if IsPedHuman(Zombie) and not IsPedAPlayer(Zombie) and not IsPedDeadOrDying(Zombie, true) then
                        local pedcoords = GetEntityCoords(Zombie)
                        local zonecoords = vector3(zone.x, zone.y, zone.z)
                        local distance = #(zonecoords - pedcoords)
                        if distance <= zone.radius then
                            DeleteEntity(Zombie)
                        end
                    end
                    Success, Zombie = FindNextPed(Handler)
                until not Success
                EndFindPed(Handler)
            end
        end

        -- Zombie AI logic
        local Handler, Zombie = FindFirstPed()
        local Success = true
        local anyZombieNear = false

        repeat
            Citizen.Wait(10)
            if IsPedHuman(Zombie) and not IsPedAPlayer(Zombie) then
                if not IsPedDeadOrDying(Zombie, true) then
                    if not DecorExistOn(Zombie, 'RegisterZombie') then
                        ClearPedTasks(Zombie)
                        ClearPedSecondaryTask(Zombie)
                        ClearPedTasksImmediately(Zombie)
                        TaskWanderStandard(Zombie, 10.0, 10)
                        SetPedRelationshipGroupHash(Zombie, 'ZOMBIE')
                        ApplyPedDamagePack(Zombie, 'BigHitByVehicle', 0.0, 1.0)
                        SetEntityHealth(Zombie, 200)

                        RequestAnimSet('move_m@drunk@verydrunk')
                        while not HasAnimSetLoaded('move_m@drunk@verydrunk') do
                            Citizen.Wait(0)
                        end
                        SetPedMovementClipset(Zombie, 'move_m@drunk@verydrunk', 1.0)

                        SetPedConfigFlag(Zombie, 100, false)
                        DecorSetBool(Zombie, 'RegisterZombie', true)
                    end

                    SetPedRagdollBlockingFlags(Zombie, 1)
                    SetPedCanRagdollFromPlayerImpact(Zombie, false)
                    SetPedSuffersCriticalHits(Zombie, true)
                    SetPedEnableWeaponBlocking(Zombie, true)
                    DisablePedPainAudio(Zombie, true)
                    StopPedSpeaking(Zombie, true)
                    SetPedDiesWhenInjured(Zombie, false)
                    StopPedRingtone(Zombie)
                    SetPedMute(Zombie)
                    SetPedIsDrunk(Zombie, true)
                    SetPedConfigFlag(Zombie, 166, false)
                    SetPedConfigFlag(Zombie, 170, false)
                    SetBlockingOfNonTemporaryEvents(Zombie, true)
                    SetPedCanEvasiveDive(Zombie, false)
                    RemoveAllPedWeapons(Zombie, true)

                    local PedCoords = GetEntityCoords(Zombie)
                    local targetPed, Distance = GetClosestPlayerToZombie(PedCoords)

                    local DistanceTarget
                    if IsPlayerShooting() then
                        DistanceTarget = 120.0
                    elseif IsPlayerRunning() then
                        DistanceTarget = 50.0
                    else
                        DistanceTarget = 20.0
                    end

                    if targetPed and Distance <= DistanceTarget and not IsPedInAnyVehicle(targetPed, false) then
                        anyZombieNear = true
                        TaskGoToEntity(Zombie, targetPed, -1, 0.0, 100.0, 1073741824, 0)
                    end

                    if Distance <= 1.3 then
                        if not IsPedRagdoll(Zombie) and not IsPedGettingUp(Zombie) then
                            local health = GetEntityHealth(PlayerPedId())
                            if health == 0 then
                                ClearPedTasks(Zombie)
                                TaskWanderStandard(Zombie, 10.0, 10)
                            else
                                RequestAnimSet('melee@unarmed@streamed_core_fps')
                                while not HasAnimSetLoaded('melee@unarmed@streamed_core_fps') do
                                    Citizen.Wait(10)
                                end
                                TaskPlayAnim(Zombie, 'melee@unarmed@streamed_core_fps', 'ground_attack_0_psycho', 8.0, 1.0, -1, 48, 0.001, false, false, false)
                                PlayZombieSound('zombie_attack')
                                ApplyDamageToPed(targetPed, 10, false)
                            end
                        end
                    end
                elseif not DecorExistOn(Zombie, 'ZombieScheduledForCleanup') then
                    DecorSetBool(Zombie, 'ZombieScheduledForCleanup', true)
                    local zombieEntity = Zombie
                    Citizen.CreateThread(function()
                        Citizen.Wait(15000)
                        if DoesEntityExist(zombieEntity) then
                            DeleteEntity(zombieEntity)
                        end
                    end)
                end

                if not NetworkGetEntityIsNetworked(Zombie) then
                    DeleteEntity(Zombie)
                end
            end
            Success, Zombie = FindNextPed(Handler)
        until not Success
        EndFindPed(Handler)

        if anyZombieNear and not isGrowlPlaying then
            isGrowlPlaying = true
            PlayZombieSound('zombie_near_start')
        elseif not anyZombieNear and isGrowlPlaying then
            isGrowlPlaying = false
            PlayZombieSound('zombie_near_stop')
        end
    end
end)

local ambientModels = {
    "a_m_m_business_01",
    "a_m_y_business_02",
    "a_f_y_business_02",
    "a_m_y_beach_01",
    "a_f_y_beach_01",
    "a_m_y_genstreet_01",
    "a_f_y_genhot_01",
    "a_f_y_hipster_01",
    "a_m_y_hipster_01",
    "a_m_y_hiker_01",
    "a_f_m_tourist_01",
    "a_f_y_tourist_01",
    "a_m_y_runner_01",
    "a_m_m_og_boss_01",
    "a_m_y_skater_01",
    "a_m_y_roadcyc_01",
    "a_f_y_scdressy_01",
    "a_f_y_tennis_01",
    "a_f_y_runner_01",
    "a_m_y_epsilon_01",
    "a_m_m_soucent_02",
    "a_f_m_bodybuild_01",
    "a_f_y_yoga_01"
}

RegisterNetEvent("zombies:spawnAmbientPeds")
AddEventHandler("zombies:spawnAmbientPeds", function(count)
    local playerPed = PlayerPedId()
    local playerCoords = GetEntityCoords(playerPed)

    for i = 1, count do
        local modelName = ambientModels[math.random(#ambientModels)]
        local modelHash = GetHashKey(modelName)
        RequestModel(modelHash)
        while not HasModelLoaded(modelHash) do Wait(0) end

        local offsetX = math.random(-5, 5)
        local offsetY = math.random(-5, 5)
        local spawnPos = vector3(playerCoords.x + offsetX, playerCoords.y + offsetY, playerCoords.z)

        local ped = CreatePed(4, modelHash, spawnPos.x, spawnPos.y, spawnPos.z, math.random(0, 360), true, true)
		SetEntityAsMissionEntity(ped, true, false)
		SetPedCanRagdoll(ped, true)
		SetPedRandomComponentVariation(ped, 0)
		TaskWanderStandard(ped, 10.0, 10)
		SetModelAsNoLongerNeeded(modelHash)

    end
end)

RegisterNetEvent("zombies:alertAll")
AddEventHandler("zombies:alertAll", function()
    SendNUIMessage({ type = "zombie_alert" })
end)
