--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

OXYGEN = {
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