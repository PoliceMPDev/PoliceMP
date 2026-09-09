main = {
    itemEnabled = true,
    itemName = "throwbag",
    weapon = `weapon_throwline`,
    commandName = "throwbag",
    acePermissionsEnabled = true,
    ropeModel = `prop_cs_20m_rope`,
    pullInSpeed = 100, -- Every 100 ms it pulls in
    animDict = "rcmlastone2leadinout",
    animName = "sas_idle_sit",
    enableAnimation = true, -- Enables the animation after the player is no longer swimming
    animDict2 = "missprologueig_4@hold_head_base",
    animName2 = "hold_head_loop_base_brad",
    allowAllObjects = false, -- This allows all objects to be pulled in by the throwbag (not just people)
}

-- These are the job checks for the /throwbag command
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
    commandSuggestion = "Use a water rescue throwbag",
    outsideVehicle = "~r~Error~w~: You must not be inside a vehicle",
    noPlayerFound = "~r~Error~w~: No player found",
    playerFound = "~g~Success~w~: Throw bag deployed",
    ropeWinding = "~g~Success~w~: Rope is now winding",
}