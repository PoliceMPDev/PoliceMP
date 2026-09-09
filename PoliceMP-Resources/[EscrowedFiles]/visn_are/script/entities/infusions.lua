--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

INFUSIONS = {
    ["blood"] = {
        availableVolumes = { 100, 250, 500, 750, 1000 }, -- In ml
        cooldown = 3, -- Duration of action in s
        ivChangePerSecond = 4.1667, -- 250ml should be done in 60s. 250ml / 60s ~ 4.1667 ml/s.
        onTick = function(clientData, healthBuffer, bodyPart, ivChange) -- Will be executed every second
            -- Custom on tick logic
            healthBuffer.bloodVolume = healthBuffer.bloodVolume + ivChange
        end,
        onFinish = function(clientData, healthBuffer, bodyPart, totalVolume, givenVolume) -- Will be executed when the action is finished
            -- Custom on finish logic
            
            -- Example: Give the player a notification
        end,
        permissions_needed = { "hems","lasdoctor" }
    },
    
    ["saline"] = {
        availableVolumes = { 100, 250, 500, 750, 1000 }, -- In ml
        cooldown = 3, -- Duration of action in s
        ivChangePerSecond = 4.1667, -- 250ml should be done in 60s. 250ml / 60s ~ 4.1667 ml/s.
        onTick = function(clientData, healthBuffer, bodyPart, ivChange) -- Will be executed every second
            -- Custom on tick logic
            healthBuffer.bloodVolume = healthBuffer.bloodVolume + ivChange
        end,
        onFinish = function(clientData, healthBuffer, bodyPart, totalVolume, givenVolume) -- Will be executed when the action is finished
            -- Custom on finish logic
            
            -- Example: Give the player a notification
        end,
        permissions_needed = {"studentparamedic", "paramedic", "hart" }
    },

    
    ["propofol"] = {
        availableVolumes = { 100, 250 }, -- In ml
        cooldown = 3, -- Duration of action in s
        ivChangePerSecond = 1, -- 100ml should be done in 60s. 100ml / 60s ~ 1 ml/s.
        onTick = function(clientData, healthBuffer, bodyPart, ivChange, givenVolume) -- Will be executed every second
            if not healthBuffer.anesthesia and givenVolume >= 50 then
                healthBuffer.anesthesia = true
            end
        end,

        onFinish = function(clientData, healthBuffer, bodyPart, totalVolume) -- Will be executed when the action is finished
            if healthBuffer.anesthesia then
                healthBuffer.anesthesia = false

                if not healthBuffer.unconscious then
                    ClearPedTasks(ClientData.ped)
                end
            end
        end,
        permissions_needed = { "hems","lasdoctor" }
    },
    --["TXA"] = {
    --    availableVolumes = { 10, 25 }, -- In ml
    --    cooldown = 3, -- Duration of action in s
    --    ivChangePerSecond = 0.4, -- 250ml should be done in 60s. 250ml / 60s ~ 4.1667 ml/s.
    --    onTick = function(clientData, healthBuffer, bodyPart, ivChange) -- Will be executed every second
    --        -- Custom on tick logic
    --        healthBuffer.bloodVolume = healthBuffer.bloodVolume + ivChange
    --    end,
    --    onFinish = function(clientData, healthBuffer, bodyPart, totalVolume, givenVolume) -- Will be executed when the action is finished
    --        -- Custom on finish logic
    --        
    --        -- Example: Give the player a notification
    --    end,
    --    permissions_needed = { "advancedparamedic", "hems", "hart", "lasdoctor" }
    --},
    ["oxygen"] = {
        availableVolumes = { 15 }, -- In ml
        cooldown = 3, -- Duration of action in s
        ivChangePerSecond = 0.1667, -- 250ml should be done in 60s. 250ml / 60s ~ 4.1667 ml/s.
        onTick = function(clientData, healthBuffer, bodyPart, ivChange) -- Will be executed every second
            -- Custom on tick logic
            healthBuffer.bloodVolume = healthBuffer.bloodVolume + ivChange
        end,
        onFinish = function(clientData, healthBuffer, bodyPart, totalVolume, givenVolume) -- Will be executed when the action is finished
            -- Custom on finish logic
            
            -- Example: Give the player a notification
        end,
        permissions_needed = { "firefighter", "stjohns", "pc", "afo", "studentparamedic", "paramedic", "advancedparamedic", "hems", "hart", "lasdoctor" }
    }
}