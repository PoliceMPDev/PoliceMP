main = {
    acePermissionsEnabled = true,
    commandName = "tripod",
    basketCommand = "basket",
    defaultPermission = true, -- Default permission to go up and down the rope (not to spawn one in), see server file for event to trigger
    usageDistance = 5.0,
    tripodModel = `prop_tripod`,
    ropeModel = `prop_cs_30m_rope`,
    basketModel = `prop_basket`,
    usageKey = {0, 23}, -- Find controls list here: https://docs.fivem.net/docs/game-references/controls/
    animDict = "missrappel",
    animName = "rappel_walk",
    upKey = {1, 32},
    downKey = {0, 33},
    speed = 0.25,
    climbOffSet = -2.4,
}

-- These are the job checks for the /basket and /tripod commands. Enabling permissions will have effect for both commands.
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

-- We do not recommend editing the below section
offSets = {
    [1] = {-2.0, 0.0, -27.61},
    [2] = {0.0, 0.0, -30.0},
    [3] = {0.1, 0.7, 0.0},
}

rotations = {
    [3] = {0.0, 0.0, 90.0},
}

-- This allows you to translate the resource into another language
translations = {
    invalidArgs = "~b~RopeRescue~w~: Use /tripod setup or /tripod remove",
    commandSuggestion = "Setup or remove a rope rescue tripod",
    help = "setup/remove",
    setup = "setup",
    remove = "remove",
    basketCommandSuggestion = "Put nearest player in a basket, or just carry one",
    tripodRemoved = "~b~RopeRescue~w~: This tripod has been removed.",
    tripodError = "~r~RopeRescue~w~: You are unable to setup a tripod here.",
    tripodSetup = "~b~RopeRescue~w~: Tripod Setup.",
    noPerms = "~b~RopeRescue~w~: Access denied.",
    noPlayerFound = "~b~RopeRescue~w~: No nearby player found.",
    pressToRappel = "Press ~INPUT_ENTER~ to ~b~rappel~w~.",
    pressToStopRappel = "Press ~INPUT_ENTER~ to stop ~b~rappelling~w~.",
}