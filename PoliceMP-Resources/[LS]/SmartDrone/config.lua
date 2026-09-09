main = {
    models = {
        --["Drone Name"] = `model_name`,
        ["Drone1"] = `dji_inspire`,
        ["Drone2"] = `dji_matrice`,
    },
    defaultDroneType = "Drone1", -- This must match an above option. If no drone type is specified, this will be used
    tabletModel = `prop_cs_tablet`,
    dronePadModel = `prop_dronepad`,
    useTopNotifications = false,
    blockVehiclesBeingDriven = true,

    enableInvincible = false, -- This will determine whether we prevent the ped from dying on drone crash. You may want to disable this if you have a sensitive anticheat.

    -- Controls:
    spawnDroneInFrontOffSet = {0.0, 4.0, 0.0},
    cameraPosition = {0.0, 0.35, 0.01},
    controlDroneKey = {0, 80}, -- https://docs.fivem.net/docs/game-references/controls/ -- default Left ALT
    nightVisionKey = {0, 212},   
    thermalImagingKey = {0, 214},
    -- Camera Rotations:
    rotatingUpKey = {0, 172},
    rotatingDownKey = {0, 173},
    rotatingLeftKey = {0, 174},
    rotatingRightKey = {0, 175},
    toggleSearchLight = {0, 121},

    enableNightVision = true,
    enableThermalVision = true,

    disableLandingPad = false,

    maximumDroneDistance = 900.0, -- If you are experiencing issues, reduce this number.

    -- We do not recommend editing these sections unless you are familiar with spotlights in GtaV
    searchLight = {
        brightness = 70.0,
        distance = 70.0,
        hardness = 4.3,
        radius = 25.0,
        falloff = 28.6,

        onByDefaultHours = {18, 6}, -- between 6pm and 6am, spotlight turns on automatically. Left hour should be higher (24hr clock)
        switchColourKey = {0, 208},
        
        enableMultiColours = true,
        
        colours = {
            {221, 221, 221}, -- white, default
            {0, 0, 255}, -- blue
            {255, 0, 0}, -- red
        }
    },

    noFlyZoneColour = 1, --https://docs.fivem.net/docs/game-references/blips/#BlipColors
    noFlyZones = {
        ["MilitaryBase"] = {
            coords = vector3(-2110.33, 3054.58, 32.82),
            radius = 400.0,
        },
    }
}

droneCommand = {
    enabled = true,
    itemEnabled = false, -- The item's names are the names found in main.models. (By default, drone1 and drone2)
    commandName = "drone",

    acePermissions = {
        enabled = true,
        -- This enables ace permissions on the drone command
    },
    -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
    ESX = {
        enabled = false,
        checkJob = {
            enabled = false, -- Enable this to use ESX job check
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
    droneCommandHelp = "Setup a drone",
    droneCommandParameterOne = "Drone Type",
    noPermission = "~r~Error~w~: You do not have permission to access this command.",
    invalidAction = "~r~Error~w~: That action is invalid.",
    invalidDroneType = "~r~Error~w~: Invalid drone type.",
    noFlyZone = "~r~Warning~w~: Leave the no fly zone immediately.",
    loosingConnection = "~r~Warning~w~: Head back, you are loosing connection.",
}