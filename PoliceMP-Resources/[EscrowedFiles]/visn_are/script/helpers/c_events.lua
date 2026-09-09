--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]
local permissions = {}

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_TOURNIQUET, function(bodyPart, authToken)
    if authToken ~= TempAuthToken then return end
    if ClientHealthBuffer.bodyParts[bodyPart].tourniquetApplied then return end

    ClientHealthBuffer.bodyParts[bodyPart].tourniquetApplied = true
    ClientHealthBuffer.bodyParts[bodyPart].tourniquetAppliedTime = GetGameTimer()
    ReportHealthBufferUpdate()
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_REMOVE_TOURNIQUET, function(bodyPart, authToken)
    if not ClientHealthBuffer.bodyParts[bodyPart].tourniquetApplied then return end
    ClientHealthBuffer.bodyParts[bodyPart].tourniquetApplied = false
    ClientHealthBuffer.bodyParts[bodyPart].tourniquetAppliedTime = 0

    for k, v in ipairs(ClientHealthBuffer.bodyParts[bodyPart].blocked_medications) do
        table.remove(ClientHealthBuffer.bodyParts[bodyPart].blocked_medications, k)
        AddMedicationLocal(bodyPart, v)
    end

    ReportHealthBufferUpdate()
end)



RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_INJECT_MEDICATION, function(bodyPart, medication)
    -- if authToken ~= TempAuthToken then return end
    -- if medication == "adrenaline" or medication == "epinephrine" or medication == "midazolam"  or medication == "ramipril" or medication == "prochlorperazine" or medication == "diazepam" or medication == "methlyene_blue" or medication == "amlodipine" or medication == "nitroglycerin" or medication == "lorazepam" or medication == "amiodarone" or medication == "metoprolol" or medication == "tramadol" or medication == "lidocaine" or medication == "ketamine" or medication == "amoxicillin" or medication == "ciprofloxacin" then SpawnGameObject("visn_injector_epinephrine", ClientData.coords.x, ClientData.coords.y, ClientData.coords.z - 0.95) end
    AddMedicationLocal(bodyPart, medication)
end)




RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_BREAK_BONE, function(bodyPart)
    ApplyDamageToPed(-1, 100, true)
    ReportHealthBufferUpdate()
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_CLEAR_INJURY, function(bodyPart, authToken, injuries)
    for k, v in pairs(ClientHealthBuffer.bodyParts[bodyPart].injuries) do
        for k2, v2 in pairs(injuries) do
            if k == v2 then
                ClientHealthBuffer.bodyParts[bodyPart].injuries[k] = nil
                ClientHealthBuffer.bodyParts[bodyPart].injuryAmount = math.max(0, ClientHealthBuffer.bodyParts[bodyPart].injuryAmount - 1)
            end
        end
    end

    ReportHealthBufferUpdate()
end)


RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_BANDAGE, function(bodyPart, bandageType, authToken)
    -- ShowNotification("Applying Bandage: " .. bandageType)
    -- ShowNotification("Body Part: " .. bodyPart)
    -- if authToken ~= TempAuthToken then return end
    local targetWound, effectiveness = FindMostEffectiveWound(ClientHealthBuffer, bandageType, bodyPart)

    -- ShowNotification("Effectiveness: " .. effectiveness)
    -- ShowNotification(dump(targetWound))
    if effectiveness == -1 or not targetWound then -- No wounds on this body part
        SpawnGameObject("visn_bandage", ClientData.coords.x, ClientData.coords.y, ClientData.coords.z - 0.95)
        return
    end

    local woundIndex, finalWound = FindInjuryInHealthBuffer(ClientHealthBuffer, bodyPart, targetWound.id)
    -- ShowNotification("woundIndex: " .. woundIndex)
    -- ShowNotification(dump(finalWound))
    if woundIndex == -1 or not finalWound then return end

    local amount = GetInjuryLevelAsNumber(finalWound.level)
    local impact = math.min(effectiveness, amount)
    local woundWillOpenAgain = false

    amount = amount - impact
    finalWound.level = GetInjuryLevelFromNumber(amount)
    SpawnGameObject("visn_bandage_blood", ClientData.coords.x, ClientData.coords.y, ClientData.coords.z - 0.95)

    -- ShowNotification("New Amount: " .. amount)
    if amount <= 0 or finalWound.level == "none" and not woundWillOpenAgain then

        if ClientConfig.m_enableSewings then
            woundWillOpenAgain = HandleBandageOpening(bodyPart, targetWound.key, bandageType, targetWound.id)
        end

        if not woundWillOpenAgain then
            ClientHealthBuffer.bodyParts[bodyPart].injuries[targetWound.key] = nil
            ClientHealthBuffer.bodyParts[bodyPart].injuryAmount = math.max(0, ClientHealthBuffer.bodyParts[bodyPart].injuryAmount - 1)
            if bodyPart == "LEFT_LEG" or bodyPart == "RIGHT_LEG" then ResetPedMovementClipset(ClientData.ped, 0.0) end
        end
    end

    ReportHealthBufferUpdate()
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_SURGICAL_KIT, function(bodyPart, authToken)
    -- if authToken ~= TempAuthToken then return end
    for k, v in pairs(ClientHealthBuffer.bodyParts[bodyPart].injuries) do
        if v.needSewing then
            ClientHealthBuffer.bodyParts[bodyPart].injuries[k] = nil
            ClientHealthBuffer.bodyParts[bodyPart].injuryAmount = math.max(0, ClientHealthBuffer.bodyParts[bodyPart].injuryAmount - 1)
        end
    end

    SpawnGameObject("visn_gloves", ClientData.coords.x, ClientData.coords.y, ClientData.coords.z - 0.95)
    ReportHealthBufferUpdate()
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_INFUSION, function(bodyPart, infusion, volume, authToken)
    -- if authToken ~= TempAuthToken then return end

    for i, v in ipairs(ClientHealthBuffer.infusions) do
        if v.bodyPart == bodyPart and v.name == infusion then
            ClientHealthBuffer.infusions[i].remainingVolume = ClientHealthBuffer.infusions[i].remainingVolume + volume
            ClientHealthBuffer.infusions[i].totalVolume = ClientHealthBuffer.infusions[i].totalVolume + volume
            ReportHealthBufferUpdate()
            LogDebug(TableContains(ClientConfig.m_debugModeModules, "infusions"), "Infusion (" .. infusion .. "; " .. volume .. "ml) updated at " .. bodyPart .. ".")
            return
        end
    end

    table.insert(ClientHealthBuffer.infusions, {
        name = infusion,
        remainingVolume = volume,
        totalVolume = volume,
        bodyPart = bodyPart
    })

    LogDebug(TableContains(ClientConfig.m_debugModeModules, "infusions"), "Infusion (" .. infusion .. "; " .. volume .. "ml) applied at " .. bodyPart .. ".")
    ReportHealthBufferUpdate()
end)

RegisterNetEvent("visn_are:test123", function(bodyPart, OXYGEN, volume, authToken)
    -- if authToken ~= TempAuthToken then return end

    for i, v in ipairs(ClientHealthBuffer.infusions) do
        if v.bodyPart == bodyPart and v.name == OXYGEN then
            ClientHealthBuffer.infusions[i].remainingVolume = ClientHealthBuffer.infusions[i].remainingVolume + volume
            ClientHealthBuffer.infusions[i].totalVolume = ClientHealthBuffer.infusions[i].totalVolume + volume
            ReportHealthBufferUpdate()
            LogDebug(TableContains(ClientConfig.m_debugModeModules, "infusions"), "Infusion (" .. OXYGEN .. "; " .. volume .. "ml) updated at " .. bodyPart .. ".")
            return
        end
    end

    

    table.insert(ClientHealthBuffer.infusions, {
        name = OXYGEN,
        remainingVolume = volume,
        totalVolume = volume,
        bodyPart = bodyPart
    })

    LogDebug(TableContains(ClientConfig.m_debugModeModules, "infusions"), "Infusion (" .. OXYGEN .. "; " .. volume .. "ml) applied at " .. bodyPart .. ".")
    ReportHealthBufferUpdate()
end)

local cprInjuryTimer = nil

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_CPR, function(bodyPart, authToken)
    -- Uncomment to use token security
    -- if authToken ~= TempAuthToken then return end

    if not ClientHealthBuffer.unconscious then return end

    local cprSuccessful = false

    if not ClientHealthBuffer.non_recovery_mode then
        if math.random(0, 100) >= 50 then
            local boost = math.random(3, 30)
            ClientHealthBuffer.heartRate = ClientHealthBuffer.heartRate + boost

            if boost > 0 then
                cprSuccessful = true
            end
        end
    end

    -- If CPR was successful, cancel any previous injury timer
    if cprSuccessful then
        if cprInjuryTimer and cprInjuryTimer.active then
            cprInjuryTimer.cancel = true
            cprInjuryTimer = nil
        end
        return -- exit early, no injury timer needed
    end

    -- If CPR failed and no timer is running, start injury fallback timer
    if not cprInjuryTimer or not cprInjuryTimer.active then
        cprInjuryTimer = { active = true, cancel = false }

        CreateThread(function()
            Wait(900000) 

            if cprInjuryTimer.cancel then
                cprInjuryTimer = nil
                return
            end

            local torso = ClientHealthBuffer.bodyParts["TORSO"]
            local head = ClientHealthBuffer.bodyParts["HEAD"]

            local torsoInjured = torso and (torso.injuryAmount or 0) > 0
            local headInjured = head and (head.injuryAmount or 0) > 0

            if torsoInjured or headInjured then
                TriggerEvent("addinjury", { "iwf", "TORSO", "large" })
            end

            cprInjuryTimer = nil
        end)
    end

    ClientHealthBuffer.unconsciousTimeUntilDeath = ClientHealthBuffer.unconsciousTimeUntilDeath + 150
    ReportHealthBufferUpdate()

end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_DEFIBRILLATOR, function(bodyPart, authToken)
    -- if authToken ~= TempAuthToken then return end
    if not ClientHealthBuffer.unconscious then return end
    if math.random(0, 100) > 20 then return end  -- Changed 50 to 30 for a 30% chance woody (Forbs Request)
    if ClientHealthBuffer.non_recovery_mode then return end




    
    ClientHealthBuffer.heartRate = ClientHealthBuffer.heartRate + math.random(0, 55)
    ReportHealthBufferUpdate()
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_APPLY_EMERGENCY_REVIVE_KIT, function(bodyPart, authToken)
    if authToken ~= TempAuthToken then return end

    SpawnGameObject("visn_gloves", ClientData.coords.x, ClientData.coords.y, ClientData.coords.z - 0.95)
    ResetHealthBuffer()
    ReportHealthBufferUpdate()
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_RESPAWN_PLAYER, function(authToken)
    -- if authToken ~= TempAuthToken then return end
    if not ClientHealthBuffer then return end

    local nearestLocation, nearestDistance = nil, 9000000
    for _, v in pairs(ClientConfig.m_respawnConfiguration.m_respawnLocations) do
        local distance = #(ClientData.coords - vector3(v.x, v.y, v.z))
        if distance < nearestDistance then
            nearestLocation = v
            nearestDistance = distance
        end
    end

    RespawnPed(ClientData.ped, { x = nearestLocation.x, y = nearestLocation.y, z = nearestLocation.z }, nearestLocation.hospital)
    RemoveAllPedWeapons(ClientData.ped, true)

    SetUnconsciousState(false)
    ResetHealthBuffer()

    if CarryAnimationData ~= nil then
        ClearPedTasks(ClientData.ped)
        DetachEntity(ClientData.ped, true, false)
        CarryAnimationData = nil
    end
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_SAVE_LOG, function(logs, authToken)
    -- if authToken ~= TempAuthToken then return end
    if not ClientHealthBuffer then return end

    ClientHealthBuffer.logs = logs
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_PUT_IN_VEHICLE, function(authToken)
    if authToken ~= TempAuthToken then return end
    if not ClientHealthBuffer then return end
    if not ClientHealthBuffer.unconscious and not ClientHealthBuffer.anesthesia then return end

    local vehicle, distance = GetCustomClosestVehicle(ClientData.coords)
    if distance > ClientConfig.m_vehicleScanRadius then return end
    if not IsEntityAVehicle(vehicle) then return end
    local maxPassengers = GetVehicleMaxNumberOfPassengers(vehicle)
    local freeSeat = -1

    local startIndex = 0
    if maxPassengers >= 3 then startIndex = 1 end

    for i = startIndex, (maxPassengers - 1) - startIndex do
        if IsVehicleSeatFree(vehicle, i) then
            freeSeat = i
            break
        end
    end

    if freeSeat then
        TaskWarpPedIntoVehicle(ClientData.ped, vehicle, freeSeat)
    end
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_PUT_IN_BODYBAG, function(authToken)
    -- if authToken ~= TempAuthToken then return end
    if not ClientHealthBuffer then return end
    if not ClientHealthBuffer.unconscious and not ClientHealthBuffer.anesthesia then return end

    SetEntityAlpha(PlayerPedId(), 0, false)
    SetEntityVisible(PlayerPedId(), false, false)

    RequestModel(GetHashKey("xm_prop_body_bag"))
    while not HasModelLoaded(GetHashKey("xm_prop_body_bag")) do
        Citizen.Wait(0)
    end

    local bodyBag = CreateObject(GetHashKey("xm_prop_body_bag"), ClientData.coords.x, ClientData.coords.y, ClientData.coords.z, true, true, false)
    AttachEntityToEntity(bodyBag, ClientData.ped, 0, -0.2, 0.75, -0.2, 0.0, 0.0, 0.0, false, false, false, false, 20, false)
    BodyBagObject = bodyBag

    ClientHealthBuffer.bodybag = true
    ClientHealthBuffer.unconsciousTimeUntilDeath = math.min(ClientHealthBuffer.unconsciousTimeUntilDeath, ClientConfig.m_bodybagUnconsciousTime)
    ClientHealthBuffer.activeECG = false

    SendNUIMessage({
        payload = "showManualRespawnText",
        payloadData = {
            value = true,
            key = ClientConfig.m_configurableKeys.m_useInputMapper
            and GetControlStringFromKeyMapping("manual_respawn")
            or GetControlStringFromKeyMappingNoInputMapper(ClientConfig.m_configurableKeys.keys.MANUAL_RESPAWN.control_id)
        }
    })
    AllowManualRespawn = true

    ReportHealthBufferUpdate()
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_START_ECG, function(authToken)
    if not ClientHealthBuffer then return end

    ClientHealthBuffer.activeECG = true
    ReportHealthBufferUpdate()
end)



RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_STOP_ECG, function(authToken)
    -- if authToken ~= TempAuthToken then return end
    if not ClientHealthBuffer then return end

    ClientHealthBuffer.activeECG = false
    ReportHealthBufferUpdate()
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_PULL_OUT_VEHICLE, function(authToken)
    -- if authToken ~= TempAuthToken then return end
    if not ClientHealthBuffer then return end
    if not ClientHealthBuffer.unconscious and not ClientHealthBuffer.anesthesia then return end
    if not IsPedSittingInAnyVehicle(ClientData.ped) then return end

    local vehicle = GetVehiclePedIsIn(ClientData.ped, false)
    TaskLeaveVehicle(ClientData.ped, vehicle, 16)
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_SYNC_LOCAL, function(authToken, animationData)
    -- if authToken ~= TempAuthToken then return end
    RequestAnimDict(animationData.lib)
    while not HasAnimDictLoaded(animationData.lib) do Citizen.Wait(0) end

    TaskPlayAnim(ClientData.ped, animationData.lib, animationData.name, 8.0, -8.0, animationData.length, animationData.flag, 0, false, false, false)
    CarryAnimationData = {
        lib = animationData.lib,
        name = animationData.name,
        flag = animationData.flag
    }
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_SYNC_TARGET, function(authToken, targetSource, animationData)
    -- if authToken ~= TempAuthToken then return end
    local targetPed = GetPlayerPed(GetPlayerFromServerId(targetSource))
    if not targetPed or targetPed == -1 then return end

    RequestAnimDict(animationData.lib)
    while not HasAnimDictLoaded(animationData.lib) do Citizen.Wait(0) end
    if animationData.zRotation == nil then animationData.zRotation = 180.0 end
    if animationData.flag == nil then animationData.flag = 0 end

    AttachEntityToEntity(ClientData.ped, targetPed, 0, animationData.xDistance, animationData.yDistance, animationData.zDistance, 0.5, 0.5, animationData.zRotation, false, false, false, false, 2, false)
    TaskPlayAnim(ClientData.ped, animationData.lib, animationData.name, 8.0, -8.0, animationData.length, animationData.flag, 0, false, false, false)
    CarryAnimationData = {
        lib = animationData.lib,
        name = animationData.name,
        flag = animationData.flag
    }
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_STOP_CARRY, function(authToken)
    -- if authToken ~= TempAuthToken then return end

    ClearPedTasks(ClientData.ped)
    DetachEntity(ClientData.ped, true, false)
    CarryAnimationData = nil
end)

RegisterNetEvent(ENUM_EVENT_TYPES.EVENT_SET_HEALTH_BUFFER, function(authToken, healthBuffer)
    -- if authToken ~= TempAuthToken then return end

    for k, v in pairs(healthBuffer) do
        if k ~= "unconscious" then
            ClientHealthBuffer[k] = v
        end
    end

    if healthBuffer.unconscious == true then
        SetUnconsciousState(true)

    end
end)

RegisterNetEvent(ENUM_EVENT_TYPES.PROCEDURE_UPDATE_TRIAGE, function(authToken, triageSelection)
    -- if authToken ~= TempAuthToken then return end
    if not ClientHealthBuffer then return end

    if triageSelection == "black" then
        if not ClientHealthBuffer.unconscious then return end
    end

    SendNUIMessage({
        payload = "showManualRespawnText",
        payloadData = {
            value = ClientConfig.m_configurableKeys.keys.MANUAL_RESPAWN.enabled and triageSelection == "black",
            key = ClientConfig.m_configurableKeys.m_useInputMapper
            and GetControlStringFromKeyMapping("manual_respawn")
            or GetControlStringFromKeyMappingNoInputMapper(ClientConfig.m_configurableKeys.keys.MANUAL_RESPAWN.control_id)
        }
    })

    AllowManualRespawn = triageSelection == "black"
    ClientHealthBuffer.triageSelection = triageSelection
    ReportHealthBufferUpdate()
end)


RegisterNetEvent("visn_are:resetHealthBuffer", function()
    ResetHealthBuffer()
end)


if not IsDuplicityVersion() then 
    -- Sorts out perms, don't touch without FunnyHat's permission!
    RegisterNetEvent("visn_are.returnPerms")
    AddEventHandler("visn_are.returnPerms", function(permsYouHave)
        -- Server responded with our perms
        -- ShowNotification("visn: server -> client with perms")

        for k, v in pairs(permsYouHave) do
            table.insert(permissions, v)
        end

        -- DEBUG POPUP FOR TESTING, COMMENT OUT BEFORE GOING LIVE
        for k, v in pairs(permissions) do
            -- ShowNotification("YOU HAVE " .. v)
        end
        
    end)
    Citizen.CreateThread(function()
        -- Ask the server what perms we got
        -- ShowNotification("visn: client -> server ask for perms")
        TriggerServerEvent("visn_are.getPerms")
    end)
    --
end



RegisterCommand("addinjury", function(source, args, rawCommand)
    local hasPerm = false
    for k,v in pairs(permissions) do
        if v == "civ" then
            hasPerm = true
            break
        end
    end
    if not hasPerm then
        ShowNotification("You can't do this, Dr Samurai will Smite you!")
        return
    end

    local success, errorMessage = pcall(function()
        if #args < 3 then
            -- If there are not enough arguments, you can handle it here.
            -- Maybe print an error message or return.
            ShowNotification("Usage: /addinjury [injury] [bodyPart] [level]")
            return
        end

        local injuryName = args[1]
        if INJURIES[injuryName] == nil then
            ShowNotification("Invalid injury")
            return
        end

        local bodyPart = args[2]
        if not ValidateBodyPart(bodyPart) then
            ShowNotification("Invalid body part: HEAD, TORSO, LEFT_ARM, RIGHT_ARM, LEFT_LEG, RIGHT_LEG")
            return
        end

        local level = args[3]
        if level ~= "minor" and level ~= "medium" and level ~= "large" then
            ShowNotification("Invalid level: minor, medium, large")
            return
        end

        local unconscious = false
        if level == "large" then
            unconscious = true
        end

        local injury = { key = injuryName, bodyPart = bodyPart, level = level }
        AddInjury(injury)
        SetInjuryLevel(bodyPart, injuryName, level)
        SetUnconsciousState(unconscious)

        -- Electricution stops the heart
        if injuryName == 'electrocution' or injuryName == 'heart_attack' or injuryName == 'cardio_disturbance' then
            ShowNotification("UpdateHeartRate")
            UpdateHeartRate(9999)
        end

        ReportHealthBufferUpdate()

    end)
    
end, false)

-- When you click on an option in the Medical UI...
RegisterNUICallback('NUIEventTriggerAction', function(data)
    -- If we defib, play our custom animation
    if data["name"] == "APPLY_DEFIBRILLATOR" then
        Citizen.Wait(300)
        ExecuteCommand("e medic2")
        Citizen.Wait(3000)
        ExecuteCommand("e handsup")
        Citizen.Wait(5000)
        ExecuteCommand("e handsup")
        return
    end

    if string.starts(data["name"], "INJECT_") then
        ExecuteCommand("e syringe")
        return
    end
    
    if data["name"] == "APPLY_SURGICAL_KIT" then
        ExecuteCommand("e surgery")
        return
    end

    if string.starts(data["name"], "APPLY_CPR") then
        ExecuteCommand("e medic2")
        Citizen.Wait(1200)
        ExecuteCommand("e cpr2")
        return
    end

    if string.starts(data["name"], "APPLY_") then
        ExecuteCommand("e medic2")
        Citizen.Wait(1200)
        ExecuteCommand("e bandage1")
        return
    end

    if data["name"] == "CHECK_TEMPERATURE" then
        ExecuteCommand("e medic2")
        Citizen.Wait(3000)
        ExecuteCommand("e thermo")
        return
    end
end)

RegisterNetEvent("addinjury", function(args)
    local success, errorMessage = pcall(function()
        if #args < 3 then
            -- If there are not enough arguments, handle the error
            ShowNotification("Usage: /addinjury [injury] [bodyPart] [level]")
            return
        end

        local injuryName = args[1]
        if INJURIES[injuryName] == nil then
            ShowNotification("Invalid injury")
            return
        end

        local bodyPart = args[2]
        if not ValidateBodyPart(bodyPart) then
            ShowNotification("Invalid body part: HEAD, TORSO, LEFT_ARM, RIGHT_ARM, LEFT_LEG, RIGHT_LEG")
            return
        end

        local level = args[3]
        if level ~= "minor" and level ~= "medium" and level ~= "large" then
            ShowNotification("Invalid level: minor, medium, large")
            return
        end

        local unconscious = false
        if level == "large" then
            unconscious = true
        end

        local injury = { key = injuryName, bodyPart = bodyPart, level = level }
        AddInjury(injury)
        SetInjuryLevel(bodyPart, injuryName, level)
        SetUnconsciousState(unconscious)

        -- Electricution stops the heart
        if injuryName == 'electrocution' or injuryName == 'heart_attack' or injuryName == 'cardio_disturbance' then
            ShowNotification("UpdateHeartRate")
            UpdateHeartRate(9999)
        end

        ReportHealthBufferUpdate()

    end)
end)

RegisterNetEvent("applyMultipleInjuries", function(injuryList)
    for _, injury in ipairs(injuryList) do
        local injuryName, bodyPart, severity = injury[1], injury[2], injury[3]
        
        if INJURIES[injuryName] and ValidateBodyPart(bodyPart) and (severity == "minor" or severity == "medium" or severity == "large") then
            AddInjury({ key = injuryName, bodyPart = bodyPart, level = severity })
            SetInjuryLevel(bodyPart, injuryName, severity)

            if severity == "large" then
                SetUnconsciousState(true)
            end

            if injuryName == 'electrocution' or injuryName == 'heart_attack' or injuryName == 'cardio_disturbance' then
                ShowNotification("UpdateHeartRate")
                UpdateHeartRate(9999)
            end
        end
    end

    ReportHealthBufferUpdate()
end)

local limbParts = { "LEFT_ARM", "RIGHT_ARM", "LEFT_LEG", "RIGHT_LEG" }

RegisterNetEvent("rtcsevere", function()
    local numLimbInjuries = math.random(3, 5) -- Apply 3-5 limb injuries
    local numTorsoInjuries = math.random(1, 3) -- Apply 1-3 torso injuries
    local usedLimbParts = {} -- Track limbs already injured
    local limbCount = 0
    local injuriesToApply = {}

    local limbInjuries = { "abrasion", "crush", "cut", "laceration", "puncture_wound", "impalement", "internal_bleeding", "open_fracture", "dislocation", "compartmentsyndrome" }
    local torsoInjuries = { "abrasion", "crush", "cut", "laceration", "puncture_wound", "impalement", "broken_pelvis", "internal_bleeding", "punctured_lung", "hemothorax", "tension_pneumothorax" }
    local severityLevels = { "medium", "large" } -- Severe injuries should be medium or large severity

    -- Apply injuries to limbs
    for i = 1, numLimbInjuries do
        if limbCount >= #limbParts then break end -- Prevent infinite loop

        local selectedInjury = limbInjuries[math.random(#limbInjuries)]
        local selectedSeverity = severityLevels[math.random(#severityLevels)]
        local selectedBodyPart

        local attempts = 0
        repeat
            selectedBodyPart = limbParts[math.random(#limbParts)]
            attempts = attempts + 1
        until usedLimbParts[selectedBodyPart] == nil or attempts > 10

        if attempts <= 10 then -- Only apply if a valid limb was found
            usedLimbParts[selectedBodyPart] = true -- Mark limb as used
            limbCount = limbCount + 1
            table.insert(injuriesToApply, { selectedInjury, selectedBodyPart, selectedSeverity })
        end
    end

    for i = 1, numTorsoInjuries do
        local selectedInjury = torsoInjuries[math.random(#torsoInjuries)]
        local selectedSeverity = severityLevels[math.random(#severityLevels)]
        table.insert(injuriesToApply, { selectedInjury, "TORSO", selectedSeverity })
    end

    if TriggerEvent then
        TriggerEvent("applyMultipleInjuries", injuriesToApply)
    else
        --print("[ERROR] applyMultipleInjuries event is missing!")
    end
end)

RegisterNetEvent("rtcminor", function()
    local numLimbInjuries = math.random(1, 3) -- Apply 1-3 limb injuries
    local numTorsoInjuries = math.random(0, 2) -- Apply 0-2 torso injuries
    local usedLimbParts = {} -- Track limbs already injured
    local injuriesToApply = {}

    local minorInjuries = { "abrasion", "contusion", "cut", "laceration" }
    local specificInjuries = {
        { "broken_arm", { "LEFT_ARM", "RIGHT_ARM" } },
        { "broken_leg", { "LEFT_LEG", "RIGHT_LEG" } },
        { "broken_neck", { "HEAD" } },
        { "headache", { "HEAD" } }
    }

    -- Apply minor injuries to limbs
    for i = 1, numLimbInjuries do
        local selectedInjury = minorInjuries[math.random(#minorInjuries)]
        local selectedSeverity = "minor" -- Always minor severity for RTC Minor
        local selectedBodyPart

        repeat
            selectedBodyPart = limbParts[math.random(#limbParts)]
        until not usedLimbParts[selectedBodyPart]

        usedLimbParts[selectedBodyPart] = true -- Mark limb as used
        table.insert(injuriesToApply, { selectedInjury, selectedBodyPart, selectedSeverity })
    end

    -- Apply minor injuries to torso
    for i = 1, numTorsoInjuries do
        local selectedInjury = minorInjuries[math.random(#minorInjuries)]
        local selectedSeverity = "minor"

        table.insert(injuriesToApply, { selectedInjury, "TORSO", selectedSeverity })
    end

    -- Apply specific injuries based on required body part
    for _, injuryData in ipairs(specificInjuries) do
        if math.random(1, 3) == 1 then -- 33% chance to apply each specific injury
            local injuryName = injuryData[1]
            local possibleParts = injuryData[2]
            local selectedPart = possibleParts[math.random(#possibleParts)]

            table.insert(injuriesToApply, { injuryName, selectedPart, "minor" })
        end
    end

    -- Apply all injuries in one go
    TriggerEvent("applyMultipleInjuries", injuriesToApply)

end)

RegisterNetEvent("gswsevere", function()
    local numGeneralGSWInjuries = math.random(2, 4) -- Apply 2-4 injuries (any body part)
    local numTorsoInjuries = math.random(2, 3) -- Apply 2-3 torso injuries
    local hasHeadInjury = math.random(1, 3) == 1 -- 33% chance for TBI
    local hasBlockedAirways = math.random(1, 3) == 1 -- 33% chance for blocked airways
    local usedLimbParts = {} -- Track limbs already injured
    local injuriesToApply = {}
    local generalGSWInjuries = { "avulsion", "velocity_wound", "internal_bleeding", "degloved_wound" }
    local torsoGSWInjuries = { "internal_bleeding", "sucking_chest_wound", "puncturedlung", "hemothorax", "tension_pneumothorax", "cardiac_tamponade" }
    local limbOnlyInjury = "loss_of_circulation"
    local headInjuries = { "tbi" } -- TBI always applies to HEAD

    -- Apply general GSW injuries to random body parts (limbs or torso)
    for i = 1, numGeneralGSWInjuries do
        local selectedInjury = generalGSWInjuries[math.random(#generalGSWInjuries)]
        local selectedSeverity = "large"
        local selectedBodyPart

        repeat
            selectedBodyPart = (math.random(1, 2) == 1) and limbParts[math.random(#limbParts)] or "TORSO" -- 50% chance to be on torso
        until not usedLimbParts[selectedBodyPart]

        usedLimbParts[selectedBodyPart] = true -- Mark limb as used
        table.insert(injuriesToApply, { selectedInjury, selectedBodyPart, selectedSeverity })

    end

    -- Apply torso-specific injuries
    for i = 1, numTorsoInjuries do
        local selectedInjury = torsoGSWInjuries[math.random(#torsoGSWInjuries)]
        local selectedSeverity = "large"

        table.insert(injuriesToApply, { selectedInjury, "TORSO", selectedSeverity })
    end

    -- Apply loss_of_circulation to limbs only
    if math.random(1, 2) == 1 then -- 50% chance to apply
        local selectedLimb = limbParts[math.random(#limbParts)]
        table.insert(injuriesToApply, { limbOnlyInjury, selectedLimb, "large" })
    end

    -- Apply head injuries
    if hasHeadInjury then
        table.insert(injuriesToApply, { "tbi", "HEAD", "large" })
    end

    -- Apply blocked airways to HEAD if triggered
    if hasBlockedAirways then
        table.insert(injuriesToApply, { "blocked_airways", "HEAD", "large" })
    end

    -- Apply all injuries in one go
    TriggerEvent("applyMultipleInjuries", injuriesToApply)

end)

RegisterNetEvent("gswminor", function()
    local numGeneralGSWInjuries = math.random(1, 3) -- Apply 1-3 injuries (any body part)
    local numTorsoInjuries = math.random(1, 2) -- Apply 1-2 torso injuries
    local hasHeadInjury = math.random(1, 3) == 1 -- 33% chance for TBI
    local usedLimbParts = {} -- Track limbs already injured
    local injuriesToApply = {}

    local generalGSWInjuries = { "avulsion", "velocity_wound", "internal_bleeding", "degloved_wound" }
    local torsoGSWInjuries = { "sucking_chest_wound", "puncturedlung" }
    local headInjury = "tbi" -- TBI applies only to HEAD

    -- Apply general GSW injuries to random body parts (limbs or torso)
    for i = 1, numGeneralGSWInjuries do
        local selectedInjury = generalGSWInjuries[math.random(#generalGSWInjuries)]
        local selectedSeverity = "minor"
        local selectedBodyPart

        repeat
            selectedBodyPart = (math.random(1, 2) == 1) and limbParts[math.random(#limbParts)] or "TORSO" -- 50% chance to be on torso
        until not usedLimbParts[selectedBodyPart]

        usedLimbParts[selectedBodyPart] = true -- Mark limb as used
        table.insert(injuriesToApply, { selectedInjury, selectedBodyPart, selectedSeverity })
    end

    -- Apply torso-specific injuries
    for i = 1, numTorsoInjuries do
        local selectedInjury = torsoGSWInjuries[math.random(#torsoGSWInjuries)]
        local selectedSeverity = "minor"
        table.insert(injuriesToApply, { selectedInjury, "TORSO", selectedSeverity })
    end

    -- Apply head injury (TBI) if triggered
    if hasHeadInjury then
        table.insert(injuriesToApply, { headInjury, "HEAD", "minor" })
    end

    -- Apply all injuries in one go
    TriggerEvent("applyMultipleInjuries", injuriesToApply)
end)

RegisterNetEvent("firesevere", function()
    local numGeneralFireInjuries = math.random(2, 4) -- Apply 2-4 injuries (any body part)
    local numLimbInjuries = math.random(1, 2) -- Apply 1-2 limb-specific injuries
    local numTorsoInjuries = math.random(1, 2) -- Apply 1-2 torso injuries
    local hasHeadInjury = math.random(1, 2) == 1 -- 50% chance for head injuries
    local hasHeatStroke = math.random(1, 2) == 1 -- 50% chance for heat stroke
    local usedLimbParts = {} -- Track limbs already injured
    local injuriesToApply = {}

    local generalFireInjuries = { "laceration", "burn_injury" }
    local limbSpecificInjury = "compartmentsyndrome" -- Only applies to arms and legs
    local torsoSpecificInjuries = { "cardio_disturbance" }
    local headInjuries = { "asthma_attack", "smoke_inhalation", "partial_blockage" }

    -- Apply general fire injuries to random body parts
    for i = 1, numGeneralFireInjuries do
        local selectedInjury = generalFireInjuries[math.random(#generalFireInjuries)]
        local selectedSeverity = (math.random(1, 2) == 1) and "medium" or "large"
        local selectedBodyPart

        repeat
            selectedBodyPart = limbParts[math.random(#limbParts)]
        until not usedLimbParts[selectedBodyPart]

        usedLimbParts[selectedBodyPart] = true
        table.insert(injuriesToApply, { selectedInjury, selectedBodyPart, selectedSeverity })
    end

    -- Apply limb-specific injuries (compartment syndrome)
    for i = 1, numLimbInjuries do
        local selectedLimb = limbParts[math.random(#limbParts)]
        local selectedSeverity = (math.random(1, 2) == 1) and "medium" or "large"
        table.insert(injuriesToApply, { limbSpecificInjury, selectedLimb, selectedSeverity })
    end

    -- Apply torso-specific injuries
    for i = 1, numTorsoInjuries do
        local selectedInjury = torsoSpecificInjuries[math.random(#torsoSpecificInjuries)]
        local selectedSeverity = (math.random(1, 2) == 1) and "medium" or "large"
        table.insert(injuriesToApply, { selectedInjury, "TORSO", selectedSeverity })
    end

    -- Apply heat stroke (50% chance)
    if hasHeatStroke then
        local selectedSeverity = (math.random(1, 2) == 1) and "medium" or "large"
        table.insert(injuriesToApply, { "heat_stroke", "TORSO", selectedSeverity })
    end

    -- Apply head injuries (50% chance)
    if hasHeadInjury then
        local selectedHeadInjury = headInjuries[math.random(#headInjuries)]
        local selectedSeverity = (math.random(1, 2) == 1) and "medium" or "large"
        table.insert(injuriesToApply, { selectedHeadInjury, "HEAD", selectedSeverity })
    end

    -- Apply all injuries in one go
    TriggerEvent("applyMultipleInjuries", injuriesToApply)
end)

RegisterNetEvent("fireminor", function()
    local numGeneralBurns = math.random(1, 3) -- Apply 1-3 burn injuries (random body parts)
    local hasHeadInjury = math.random(1, 2) == 1 -- 50% chance for a head injury
    local injuriesToApply = {}

    local headInjuries = { "smoke_inhalation", "heat_stroke", "partial_blockage", "asthma_attack" }

    -- Apply burn injuries to random body parts
    for i = 1, numGeneralBurns do
        local selectedBodyPart = (math.random(1, 2) == 1) and limbParts[math.random(#limbParts)] or "TORSO"
        table.insert(injuriesToApply, { "burn_injury", selectedBodyPart, "minor" })
    end

    -- Apply head injuries (50% chance)
    if hasHeadInjury then
        local selectedHeadInjury = headInjuries[math.random(#headInjuries)]
        table.insert(injuriesToApply, { selectedHeadInjury, "HEAD", "minor" })
    end

    -- Apply all injuries in one go
    TriggerEvent("applyMultipleInjuries", injuriesToApply)
end)

RegisterNetEvent("fallsevere", function()
    local numMediumInjuries = math.random(2, 4) -- Apply 2-4 medium injuries
    local numLargeInjuries = math.random(2, 3) -- Apply 2-3 large injuries
    local injuriesToApply = {}

    local mediumGeneralInjuries = { "abrasion", "contusion", "cut", "laceration", "internal_bleeding" }
    local mediumTorsoInjuries = { "broken_pelvis", "broken_ribs" }
    local mediumLegInjuries = { "mid_femur_fracture", "open_fracture", "broken_leg", "dislocation" }
    local mediumHeadInjuries = { "tbi", "broken_neck" }

    local largeGeneralInjuries = { "abrasion", "cut", "contusion", "laceration", "internal_bleeding" }
    local largeLegInjuries = { "loss_of_circulation", "broken_leg", "broken_toe", "dislocation", "open_fracture" }
    local largeArmInjuries = { "loss_of_circulation", "broken_arm", "broken_finger", "dislocation", "open_fracture" }
    local largeTorsoInjuries = { "broken_ribs", "broken_pelvis", "puncturedlung", "blunt_chest_trauma", "cardiac_tamponade", "hemothorax", "tension_pneumothorax" }
    local largeHeadInjuries = { "tbi", "broken_neck" }

    -- Apply medium severity injuries
    for i = 1, numMediumInjuries do
        local selectedCategory = math.random(1, 4) -- Randomly choose category
        local selectedInjury, selectedBodyPart

        if selectedCategory == 1 then
            selectedInjury = mediumGeneralInjuries[math.random(#mediumGeneralInjuries)]
            selectedBodyPart = (math.random(1, 2) == 1) and limbParts[math.random(#limbParts)] or "TORSO"
        elseif selectedCategory == 2 then
            selectedInjury = mediumTorsoInjuries[math.random(#mediumTorsoInjuries)]
            selectedBodyPart = "TORSO"
        elseif selectedCategory == 3 then
            selectedInjury = mediumLegInjuries[math.random(#mediumLegInjuries)]
            selectedBodyPart = (math.random(1, 2) == 1) and "LEFT_LEG" or "RIGHT_LEG"
        else
            selectedInjury = mediumHeadInjuries[math.random(#mediumHeadInjuries)]
            selectedBodyPart = "HEAD"
        end

        table.insert(injuriesToApply, { selectedInjury, selectedBodyPart, "medium" })
    end

    -- Apply large severity injuries
    for i = 1, numLargeInjuries do
        local selectedCategory = math.random(1, 4) -- Randomly choose category
        local selectedInjury, selectedBodyPart

        if selectedCategory == 1 then
            selectedInjury = largeGeneralInjuries[math.random(#largeGeneralInjuries)]
            selectedBodyPart = (math.random(1, 2) == 1) and limbParts[math.random(#limbParts)] or "TORSO"
        elseif selectedCategory == 2 then
            selectedInjury = largeLegInjuries[math.random(#largeLegInjuries)]
            selectedBodyPart = (math.random(1, 2) == 1) and "LEFT_LEG" or "RIGHT_LEG"
        elseif selectedCategory == 3 then
            selectedInjury = largeArmInjuries[math.random(#largeArmInjuries)]
            selectedBodyPart = (math.random(1, 2) == 1) and "LEFT_ARM" or "RIGHT_ARM"
        elseif selectedCategory == 4 then
            selectedInjury = largeTorsoInjuries[math.random(#largeTorsoInjuries)]
            selectedBodyPart = "TORSO"
        else
            selectedInjury = largeHeadInjuries[math.random(#largeHeadInjuries)]
            selectedBodyPart = "HEAD"
        end

        table.insert(injuriesToApply, { selectedInjury, selectedBodyPart, "large" })
    end

    -- Apply all injuries in one go
    TriggerEvent("applyMultipleInjuries", injuriesToApply)
end)

RegisterNetEvent("fallminor", function()
    local numGeneralFallInjuries = math.random(1, 3) -- Apply 1-3 general injuries
    local numLimbInjuries = math.random(1, 2) -- Apply 1-2 limb-specific injuries
    local injuriesToApply = {}

    local generalFallInjuries = { "abrasion", "contusion", "cut", "laceration" }
    local legInjuries = { "broken_leg", "broken_toe", "dislocation" }
    local armInjuries = { "broken_arm", "broken_finger", "dislocation" }

    -- Apply general fall injuries to random body parts
    for i = 1, numGeneralFallInjuries do
        local selectedBodyPart = (math.random(1, 2) == 1) and limbParts[math.random(#limbParts)] or "TORSO"
        local selectedInjury = generalFallInjuries[math.random(#generalFallInjuries)]
        table.insert(injuriesToApply, { selectedInjury, selectedBodyPart, "minor" })
    end

    -- Apply limb-specific injuries (arms & legs)
    for i = 1, numLimbInjuries do
        local selectedCategory = math.random(1, 2) -- 1 = Leg, 2 = Arm
        local selectedInjury, selectedBodyPart

        if selectedCategory == 1 then
            selectedInjury = legInjuries[math.random(#legInjuries)]
            selectedBodyPart = (math.random(1, 2) == 1) and "LEFT_LEG" or "RIGHT_LEG"
        else
            selectedInjury = armInjuries[math.random(#armInjuries)]
            selectedBodyPart = (math.random(1, 2) == 1) and "LEFT_ARM" or "RIGHT_ARM"
        end

        table.insert(injuriesToApply, { selectedInjury, selectedBodyPart, "minor" })
    end

    -- Apply all injuries in one go
    TriggerEvent("applyMultipleInjuries", injuriesToApply)
end)
