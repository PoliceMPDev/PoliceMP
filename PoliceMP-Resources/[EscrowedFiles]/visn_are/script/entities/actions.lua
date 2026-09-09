--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions.
--]]
local permissions = {}

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

    local _defaultAnimationData = { lib = "anim@heists@narcotics@funding@gang_idle", name = "gang_chatting_idle01" }


    AVAILABLE_ACTIONS = {
        ["diagnoses"] = {
            {
                name = "CHECK_CONSCIOUSNESS",
                exclusiveBodyParts = { "HEAD" },
                cooldown = 3000,
                animation = _defaultAnimationData,
                hideOnSelf = true,
                log = function(bodyPart, result)
                    
                    if result then
                        result = TranslateText("LOG_PATIENT_CONSCIOUS")
                    else
                        result = TranslateText("LOG_PATIENT_UNCONSCIOUS")
                    end

                    return TranslateText("LOG_CONSCIOUSNESS_CHECKED", TranslateText(bodyPart), result, GetPlayerName(PlayerId()))
                end,
                condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    if not healthBuffer.unconscious and not healthBuffer.anesthesia then
                        ShowNotification(TranslateText("PATIENT_IS_ALIVE"))
                        callback(true)
                        return
                    end

                    ShowNotification(TranslateText("PATIENT_NOT_ALIVE"))
                    callback(false)
                end
            },

            {
                name = "CHECK_PULSE",
                cooldown = 12000,
                animation = _defaultAnimationData,
                log = function(bodyPart, result)
                    return TranslateText("LOG_PULSE_MEASURED", TranslateText(bodyPart), result, GetPlayerName(PlayerId()))
                end,
                condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    local heartRate = RoundValue(GetHeartRate(healthBuffer, bodyPart))
                    ShowNotification(TranslateText("PULSE_MEASURED", heartRate))
                    callback(heartRate)
                end
            },
            {
                name = "CHECK_TEMPERATURE",
                cooldown = 12000,
                animation = _defaultAnimationData,
                log = function(bodyPart, result)
                    return TranslateText("LOG_TEMPERATURE_MEASURED", GetPlayerName(PlayerId()), TranslateText(bodyPart), result)
                end,
                condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    local temperature = math.random(36, 39) .. '.' .. math.random(1, 9)
                    ShowNotification(TranslateText("TEMPERATURE_MEASURED", temperature))
                    callback(temperature)
                end
            },

            {
                name = "CHECK_BLOOD_PRESSURE",
                cooldown = 12000,
                animation = _defaultAnimationData,
                log = function(bodyPart, result)
                    return TranslateText("LOG_BLOOD_PRESSURE_MEASURED", TranslateText(bodyPart), result[1], result[2],  GetPlayerName(PlayerId()))
                end,
                condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    local pressureH, pressureL = GetBloodPressure(healthBuffer, bodyPart)
                    ShowNotification(TranslateText("BLOOD_PRESSURE_MEASURED", pressureH, pressureL))
                    callback({ pressureH, pressureL })
                end
            }
        
        },

        ["bandages"] = {
            {
                name = "APPLY_TOURNIQUET",
                exclusiveBodyParts = { "LEFT_ARM", "RIGHT_ARM", "LEFT_LEG", "RIGHT_LEG"},
                cooldown = 5000,
                requiredItem = {
                    -- name = "tourniquet",
                    -- count = 1
                },
                animation = _defaultAnimationData,
                log = function(bodyPart, result)
                    return TranslateText("LOG_TOURNIQUET_APPLIED", TranslateText(bodyPart),  GetPlayerName(PlayerId()))
                end,
                condition = function(healthBuffer, bodyPart) return not healthBuffer.bodyParts[bodyPart].tourniquetApplied and not healthBuffer.bodybag end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    if healthBuffer.bodyParts[bodyPart].tourniquetApplied then return end
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_APPLY_TOURNIQUET, TempAuthToken, clientData.source, bodyPart)
                    callback(true)
                end
            },

            {
                name = "REMOVE_TOURNIQUET",
                exclusiveBodyParts = { "LEFT_ARM", "RIGHT_ARM", "LEFT_LEG", "RIGHT_LEG" },
                cooldown = 5000,
                animation = _defaultAnimationData,
                log = function(bodyPart, result)
                    return TranslateText("LOG_TOURNIQUET_REMOVED", TranslateText(bodyPart),  GetPlayerName(PlayerId()))
                end,
                condition = function(healthBuffer, bodyPart) return healthBuffer.bodyParts[bodyPart].tourniquetApplied and not healthBuffer.bodybag end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    if not healthBuffer.bodyParts[bodyPart].tourniquetApplied then return end
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_REMOVE_TOURNIQUET, TempAuthToken, clientData.source, bodyPart)
                    callback(true)
                end
            }
        },

        ["cpr"] = {
            {
                name = "APPLY_CPR",
                cooldown = 30000,
                animation = {
                    lib = "mini@cpr@char_a@cpr_str",
                    name = "cpr_pumpchest"
                },
                hideOnSelf = true,
                log = function(bodyPart, result)
                    return TranslateText("LOG_CPR_APPLIED", GetPlayerName(PlayerId()))
                end,
                condition = function(healthBuffer, bodyPart) return (healthBuffer.unconscious or healthBuffer.anesthesia) and not healthBuffer.bodybag end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_APPLY_CPR, TempAuthToken, clientData.source, bodyPart)
                    callback(true)
                end
            },
            {
                name = "APPLY_DEFIBRILLATOR",
                cooldown = 9000,
                animation = _defaultAnimationData,
                hideOnSelf = true,
                sound = {
                    name = "defibrillator",
                    distance = 8
                },
                requiredItem = {
                    -- name = "defibrillator",
                    -- count = 1
                },
                log = function(bodyPart, result)
                    return TranslateText("LOG_DEFIBIRLATOR_APPLIED", GetPlayerName(PlayerId()))
                end,
                condition = function(healthBuffer, bodyPart)
                    return (healthBuffer.unconscious or healthBuffer.anesthesia) and not healthBuffer.bodybag 
                end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_APPLY_DEFIBRILLATOR, TempAuthToken, clientData.source, bodyPart)
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_CLEAR_INJURY, TempAuthToken, clientData.source, bodyPart, {"electrocution", "heart_attack", "cardio_disturbance"})

                    callback(true)
                end           
            },
            {
                name = "START_ECG",
                cooldown = 6000,
                animation = _defaultAnimationData,
                hideOnSelf = true,
                exclusiveBodyParts = { "TORSO" },
                requiredItem = {
                    -- name = "ecg_monitor",
                    -- count = 1,
                },
                condition = function(healthBuffer, bodyPart) return not healthBuffer.activeECG end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_START_ECG, TempAuthToken, clientData.source)
                    callback(true)
                end
            },

            {
                name = "STOP_ECG",
                cooldown = 6000,
                animation = _defaultAnimationData,
                hideOnSelf = true,
                exclusiveBodyParts = { "TORSO" },
                condition = function(healthBuffer, bodyPart) return healthBuffer.activeECG end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_STOP_ECG, TempAuthToken, clientData.source)
                    callback(true)
                end
            }
        },

        ["carry"] = {
            {
                name = "CARRY",
                cooldown = 4000,
                animation = _defaultAnimationData,
                hideOnSelf = true,
                -- condition = function(healthBuffer, bodyPart) return (healthBuffer.unconscious or healthBuffer.anesthesia) end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    if ClientConfig.m_disableDefaultUnconsciousAnimation then 
                        LogWarning("Carry is currently not available with m_disableDefaultUnconsciousAnimation enabled.")
                        return
                    end

                    CarryPlayer(clientData, healthBuffer)
                    callback(true)
                end
            },
            {
                name = "PUT_IN_VEHICLE",
                cooldown = 3000,
                animation = _defaultAnimationData,
                hideOnSelf = false,
                -- condition = function(healthBuffer, bodyPart) return (healthBuffer.unconscious or healthBuffer.anesthesia) end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_PUT_IN_VEHICLE, TempAuthToken, clientData.source)
                    callback(true)
                end
            },
            {
                name = "PULL_OUT_VEHICLE",
                cooldown = 3000,
                animation = _defaultAnimationData,
                hideOnSelf = false,
                -- condition = function(healthBuffer, bodyPart) return (healthBuffer.unconscious or healthBuffer.anesthesia) end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_PULL_OUT_VEHICLE, TempAuthToken, clientData.source)
                    callback(true)
                end
            },
            {
                name = "PUT_IN_BODYBAG",
                cooldown = 12000,
                animation = _defaultAnimationData,
                hideOnSelf = false,
                condition = function(healthBuffer, bodyPart) return (healthBuffer.unconscious or healthBuffer.anesthesia) and not healthBuffer.bodybag end,
                log = function(bodyPart, result)
                    return TranslateText("LOG_PUT_IN_BODYBAG", TranslateText(bodyPart), GetPlayerName(source))
                end,
                action = function(clientData, healthBuffer, bodyPart, callback)
                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_PUT_IN_BODYBAG, TempAuthToken, clientData.source)
                    callback(true)
                end
            }
            
        },

        ["syringes"] = {}, -- Don't touch
        ["infusions"] = {}, -- Don't touch
        ["oxygen"] = {}, -- Don't touch
        ["doctor"] = {} -- Don't touch
    }

    function GetAvailableActionsLocal(isSelf, category, bodyPart, healthBuffer)
        if not AVAILABLE_ACTIONS[category] then LogWarning("Tried to index ", category, " in available actions but category wasn't found.") return {} end

        local finalList = {}

        local items = PlayerItems
        for k, v in pairs(items) do
            if type(v) == "number" then
                if v > 0 then items[k] = v end
            elseif type(v) == "table" then
                if v.count > 0 then items[v.name] = v.count end
            end
        end

        for _, v in pairs(AVAILABLE_ACTIONS[category]) do
            local hasItem = not v.requiredItem or (v.requiredItem and items[v.requiredItem.name])
            local notHidden = not v.hideOnSelf or (v.hideOnSelf and not isSelf)
            local includedBodyPart = not v.exclusiveBodyParts or v.exclusiveBodyParts and TableContains(v.exclusiveBodyParts, bodyPart)
            local condition = not v.condition or (v.condition and v.condition(healthBuffer, bodyPart))
            
            if not ClientConfig.m_onlyShowActionsIfPlayerHasRequiredItems then
                if notHidden and includedBodyPart and condition then
                    table.insert(finalList, v.name)
                end
            else
                if hasItem and notHidden and includedBodyPart and condition then
                    table.insert(finalList, v.name)
                end
            end
        end

        return finalList
    end

    function tableCount(table)
        local count = 0
        for k, v in pairs(table) do
            count = count + 1
        end
        return count
    end

    function LoadActionsFromEntities()
        -- Check that permissions have loaded, otherwise delay
        while (tableCount(permissions) == 0) do
            -- ShowNotification("Too Fast! Sleeping 1 sec...")
            Citizen.Wait(3000)
        end
        --

        -- Debug to tell us when the table is rendering
        -- ShowNotification("LoadActionsFromEntities")

        -- Table rendering
        for k, v in PairsByKeys(BANDAGES) do
            for _, v1 in pairs(v.permissions_needed or {}) do
                if TableContains(permissions, v1) then
                    if v.clears_injuries ~= nil then
                        table.insert(AVAILABLE_ACTIONS["bandages"],{
                            name = "APPLY_" .. string.upper(k),
                            cooldown = v.cooldown * 1000,
                            animation = _defaultAnimationData,
                            -- requiredJobs = v.requiredJobs or nil,
                            requiredItem = {
                                -- name = k,
                                -- count = 1
                            },
                            log = function(bodyPart, result)
                                return TranslateText("LOG_BANDAGE_APPLIED", TranslateText(string.upper(k)), TranslateText(bodyPart), GetPlayerName(PlayerId()))
                            end,
                            condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                            action = function(clientData, healthBuffer, bodyPart, callback)
                                TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_CLEAR_INJURY, TempAuthToken, clientData.source, bodyPart, v.clears_injuries)
                                callback(true)
                            end
                        })
                    else
                        table.insert(AVAILABLE_ACTIONS["bandages"],{
                            name = "APPLY_" .. string.upper(k),
                            cooldown = v.cooldown * 1000,
                            animation = _defaultAnimationData,
                            -- requiredJobs = v.requiredJobs or nil,
                            requiredItem = {
                                -- name = k,
                                -- count = 1
                            },
                            log = function(bodyPart, result)
                                return TranslateText("LOG_BANDAGE_APPLIED", TranslateText(string.upper(k)), TranslateText(bodyPart), GetPlayerName(PlayerId()))
                            end,
                            condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                            action = function(clientData, healthBuffer, bodyPart, callback)
                                TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_APPLY_BANDAGE, TempAuthToken, clientData.source, bodyPart, k)
                                callback(true)
                            end
                        })
                    end
                    break
                end
            end
        end

        for k2, v2 in pairs(INFUSIONS) do
            if k2 ~= "oxygen" then
                for _2, v3 in pairs(v2.permissions_needed or {}) do
                    if TableContains(permissions, v3) then
                        for _3, volume in pairs(v2.availableVolumes) do
                            table.insert(AVAILABLE_ACTIONS["infusions"], {
                                name = "APPLY_" .. string.upper(k2) .. "_" .. volume,
                                cooldown = v2.cooldown * 1000,
                                animation = _defaultAnimationData,
                                requiredJobs = nil,
                                requiredItem = {},
                                log = function(bodyPart, result)
                                    return TranslateText("LOG_INFUSION_APPLIED", TranslateText(string.upper(k2)), volume, TranslateText(bodyPart), GetPlayerName(PlayerId()))
                                end,
                                condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                                action = function(clientData, healthBuffer, bodyPart, callback)
                                    TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_APPLY_INFUSION, TempAuthToken, clientData.source, bodyPart, k2, volume)
                                    callback(true)
                                end
                            })
                        end
                        break
                    end
                end
            end
        end

        for a, c in pairs(OXYGEN) do
            for _, v in pairs(c.permissions_needed or {}) do
                if TableContains(permissions, v) then
                    for _2, volume in pairs(c.availableVolumes) do
                        table.insert(AVAILABLE_ACTIONS["oxygen"], {
                            name = "APPLY_" .. string.upper(a) .. "_" .. volume,
                            cooldown = c.cooldown * 1000,
                            animation = _defaultAnimationData,
                            requiredJobs = nil,
                            requiredItem = {},
                            log = function(bodyPart, result)
                                return TranslateText("LOG_OXYGEN_APPLIED", GetPlayerName(PlayerId()))
                            end,
                            condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                            action = function(clientData, healthBuffer, bodyPart, callback)
                                TriggerServerEvent("visn_are:test123", TempAuthToken, clientData.source, bodyPart, a, volume)
                                callback(true)
                            end
                        })
                    end
                    break
                end
            end
        end

        for k4, v4 in PairsByKeys(MEDICATIONS) do
            for _, v5 in pairs(v4.permissions_needed or {}) do
                if TableContains(permissions, v5) then
                    if v4.clears_injuries ~= nil then
                        table.insert(AVAILABLE_ACTIONS["syringes"], {
                            name = "INJECT_" .. string.upper(k4),
                            exclusiveBodyParts = { "HEAD", "TORSO", "LEFT_ARM", "RIGHT_ARM", "LEFT_LEG", "RIGHT_LEG" },
                            cooldown = 5000,
                            animation = _defaultAnimationData,
                            -- requiredJobs = v.requiredJobs or nil, woody
                            requiredJobs = nil,
                            requiredItem = {
                                -- name = k, woody
                                -- count = 1
                            },
                            log = function(bodyPart, result)
                                return TranslateText("LOG_MEDICATION_INJECTED", TranslateText(string.upper(k4)), TranslateText(bodyPart), GetPlayerName(PlayerId()))
                            end,
                            condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                            action = function(clientData, healthBuffer, bodyPart, callback)
                                TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_CLEAR_INJURY, TempAuthToken, clientData.source, bodyPart, v4.clears_injuries)
                                callback(true)
                            end
                        })
                    else
                        table.insert(AVAILABLE_ACTIONS["syringes"], {
                            name = "INJECT_" .. string.upper(k4),
                            exclusiveBodyParts = { "HEAD", "TORSO", "LEFT_ARM", "RIGHT_ARM", "LEFT_LEG", "RIGHT_LEG" },
                            cooldown = 5000,
                            animation = _defaultAnimationData,
                            -- requiredJobs = v.requiredJobs or nil, woody
                            requiredJobs = nil,
                            requiredItem = {
                                -- name = k, woody
                                -- count = 1
                            },
                            log = function(bodyPart, result)
                                return TranslateText("LOG_MEDICATION_INJECTED", TranslateText(string.upper(k4)), TranslateText(bodyPart), GetPlayerName(PlayerId()))
                            end,
                            condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                            action = function(clientData, healthBuffer, bodyPart, callback)
                                TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_INJECT_MEDICATION, TempAuthToken, clientData.source, bodyPart, k4)
                                callback(true)
                            end
                        })
                    end
                    break
                end
            end
        end

        for k6, v6 in PairsByKeys(DOCTOR) do
            for _, v7 in pairs(v6.permissions_needed or {}) do
                if TableContains(permissions, v7) then
                    if v6.clears_injuries ~= nil then
                        table.insert(AVAILABLE_ACTIONS["doctor"],{
                            name = "APPLY_" .. string.upper(k6),
                            cooldown = v6.cooldown * 1000,
                            animation = _defaultAnimationData,
                            -- requiredJobs = v.requiredJobs or nil,
                            requiredItem = {
                                -- name = k,
                                -- count = 1
                            },
                            log = function(bodyPart, result)
                                if k6 == "esa" then
                                    return TranslateText("LOG_ESA", GetPlayerName(PlayerId()))
                                end
                                if k6 == "needle_decompression" then
                                    return TranslateText("LOG_NEEDLE_DECOMPRESSION", GetPlayerName(PlayerId()))
                                end
                                if k6 == "reset_fracture" then
                                    return TranslateText("LOG_RESET_FRACTURE", TranslateText(bodyPart), GetPlayerName(PlayerId()))
                                end

                                return TranslateText("LOG_BANDAGE_APPLIED", TranslateText(string.upper(k6)), TranslateText(bodyPart), GetPlayerName(PlayerId()))
                            end,
                            condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                            action = function(clientData, healthBuffer, bodyPart, callback)
                                TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_CLEAR_INJURY, TempAuthToken, clientData.source, bodyPart, v6.clears_injuries)
                                callback(true)
                            end
                        })
                    elseif k6 == "surgical_kit" then
                        table.insert(AVAILABLE_ACTIONS["doctor"],{
                            name = "APPLY_" .. string.upper(k6),
                            cooldown = v6.cooldown * 1000,
                            animation = _defaultAnimationData,
                            -- requiredJobs = v.requiredJobs or nil,
                            requiredItem = {
                                -- name = k,
                                -- count = 1
                            },
                            log = function(bodyPart, result)
                                return TranslateText("LOG_BANDAGE_APPLIED", TranslateText(string.upper(k6)), TranslateText(bodyPart), GetPlayerName(PlayerId()))
                            end,
                            condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                            action = function(clientData, healthBuffer, bodyPart, callback)
                                TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_APPLY_SURGICAL_KIT, TempAuthToken, clientData.source, bodyPart)
                                callback(true)
                            end
                        })
                    else
                        table.insert(AVAILABLE_ACTIONS["doctor"],{
                            name = "APPLY_" .. string.upper(k6),
                            cooldown = v6.cooldown * 1000,
                            animation = _defaultAnimationData,
                            -- requiredJobs = v.requiredJobs or nil,
                            requiredItem = {
                                -- name = k,
                                -- count = 1
                            },
                            log = function(bodyPart, result)
                                if k6 == "ammuputation" then
                                    return TranslateText("LOG_AMPUTATION", TranslateText(bodyPart), GetPlayerName(PlayerId()))
                                end
                                if k6 == "lollypop" then
                                    return TranslateText("LOG_LOLLYPOP", GetPlayerName(PlayerId()))
                                end
                                if k6 == "peppapig" then
                                    return TranslateText("LOG_PEPPAPIG", GetPlayerName(PlayerId()))
                                end
                                if k6 == "sticker" then
                                    return TranslateText("LOG_STICKER", GetPlayerName(PlayerId()))
                                end

                                return TranslateText("LOG_BANDAGE_APPLIED", TranslateText(string.upper(k6)), TranslateText(bodyPart), GetPlayerName(PlayerId()))
                            end,
                            condition = function(healthBuffer, bodyPart) return not healthBuffer.bodybag end,
                            action = function(clientData, healthBuffer, bodyPart, callback)
                                TriggerServerEvent(ENUM_EVENT_TYPES.EVENT_APPLY_BANDAGE, TempAuthToken, clientData.source, bodyPart, k6)
                                callback(true)
                            end
                        })
                    end
                    break
                end
            end
        end
        
    end
end