main = {
    model = `bombrobot`,
    tabletModel = `prop_cs_tablet`,
    pedModel = `s_m_m_bouncer_01`,
    freezePedInUse = true, -- Freeze the ped when the EOD camera is in use
}

-- Find controls here: https://docs.fivem.net/docs/game-references/controls/

keys = {
    forward = {0, 172},
    backward = {0, 173},
    left = {0, 174},
    right = {0, 175},
    camera = {0, 191},
    nightVision = {0, 212},   
    thermalImaging = {0, 214},
    explosion = {0, 208},
    cancelExplosion = {0, 207},
    hose = {1, 121},
}

logging = {
    enabled = true,
    displayName = "EOD",
    colour = 31487,
    title = "**New EOD Log**",
    icon = "https://i.imgur.com/UDlerwZ.png",
    footerIcon = "https://i.imgur.com/n3n7JNW.png",
    dateFormat = "%d-%m-%Y %H:%M:%S", 
}

permissions = {

    acePermissions = {
            enabled = true,
            permission = "use.eod"
    },

    -- We've added vRP integration. All you need to do is enable it below
    -- Then, configure if you wish to check for groups or permissions, or even both
    -- If you want to add further vRP integration, edit sv_eod.lua
    vRP = {
        enabled = false,
        checkGroup = {
            enabled = true, -- Enable this to use vRP group check
            groups = {"police", "emergency", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
        },
        checkPermission = {
            enabled = false, -- Enable this to use vRP permission check
            permissions = {"police.menu", "player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
        },
    },

    -- We've added ESX integration. All you need to do is enable it below
    -- Then, configure which jobs you want to check for
    -- If you want to add further ESX integration, edit sv_eod.lua
    ESX = {
        enabled = false,
        checkJob = {
            enabled = true, -- Enable this to use ESX job check
            jobs = {"police"} -- A user can have any of the following jobs, allowing you to add multiple
        }
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