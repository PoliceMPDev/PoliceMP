Config = {}

Config.Targetting = 'ox-target' -- Supports ox-target, qb-target and qtarget

Config.Commands = {} -- Intentionally empty, do not populate.
Config.Commands.Enabled = true
Config.Commands.PrimaryBag = "spawnPrimaryBag"
Config.Commands.ALSBag = "spawnALSBag"
Config.Commands.O2Canister = "spawnO2Canister"
Config.Commands.Stryker = "spawnStryker"
Config.Commands.FireBag = "spawnFireBag"

Config.Exports = {} -- Intentionally empty, do not populate.
Config.Exports.Enabled = true
Config.Exports.PrimaryBag = "spawnPrimaryBag"
Config.Exports.ALSBag = "spawnALSBag"
Config.Exports.O2Canister = "spawnO2Canister"
Config.Exports.Stryker = "spawnStryker"
Config.Exports.FireBag = "spawnFireBag"

Config.Placing = {} -- Intentionally empty, do not populate.
Config.Placing.Enabled = true
Config.Placing.DefaultKeybind = "E"

Config.Sounds = {} -- Intentionally empty, do not populate.
Config.Sounds.BagOpen = function()
    TriggerServerEvent("InteractSound_SV:PlayWithinDistance", 3.0, "bagOpen", 1.0)
end

Config.Sounds.BagClose = function()
    TriggerServerEvent("InteractSound_SV:PlayWithinDistance", 3.0, "bagClose", 1.0)
end
