main = {
    commandName = "watermonitor",
    acePermissionsEnabled = true,
    monitorModel = `prop_water_monitor`,
    usageDistance = 5.0, -- Distance to turn it on/off
    pitchDistance = 5.0, -- Must be 2m away to adjust the pitch (leave this recommended)
    toggleKey = {0, 38}, -- https://docs.fivem.net/docs/game-references/controls/
    cooldown = 3, -- 3 seconds between turning on/off to prevent spam
    defaultHasPermission = true, -- Determines whether someone can toggle on/off a water monitor and adjust pitch by default. See server event to trigger if false.
    upKey = {0, 172},
    downKey = {0, 173},
    toggleOffset = {0.0, 1.5, 0.0},
    toggleOffset2 = {0.10, 0.22, 0.70},
}

-- These are the job checks for the /watermonitor command
inventory = {
    itemEnabled = false,
    oxInventory = false,
    qbInventory = false,
    esxInventory = false,
    vRPInventory = false,
    quasarInventory = false, -- Enable QB/ESX below aswell
    coreInventory = false, -- Enable QB/ESX below aswell
}

jobCheck = {
    ESX = {
        enabled = false,
        checkJob = {
            enabled = true, -- Enable this to use ESX job check
            jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
        }
    },
    -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
    vRP = {
        enabled = false,
        checkGroup = {
            enabled = false, -- Enable this to use vRP group check
            groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
        },
        checkPermission = {
            enabled = false, -- Enable this to use vRP permission check
            permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
        },
    },
    -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
    QBCore = {
        enabled = false,
        checkJob = {
            enabled = false, -- Enable this to use QBCore job check
            jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
        },
        checkPermission = {
            enabled = false, -- Enable this to use QBCore permission check
            permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
        },
    },
}

translations = {
    setup = "setup",
    remove = "remove",
    commandSuggestion = "Setup or remove a water monitor",
    commandHelp = "Setup or remove",
    outsideVehicle = "~r~Error~w~: You must not be inside a vehicle",
    monitorRemoved = "~g~Success~w~: Water monitor removed",
    monitorSetup = "~g~Success~w~: Water monitor setup",
    noMonitorFound = "~r~Error~w~: No water monitor found",
    press = "Press",
    toUse = "to toggle this ~b~water monitor",
    keyHelp = "INPUT_PICKUP",
    monitorToggled = "~g~Success~w~: Water monitor toggled",
    commandError = "~r~Error~w~: Use /watermonitor setup or /watermonitor remove",
    monitorActive = "~r~Error~w~: This monitor is active and cannot be removed",
    noSupplyLineFound = "~r~Error~w~: No active supply line found to enable this water monitor", -- Only works if you have the supply line resource
}
