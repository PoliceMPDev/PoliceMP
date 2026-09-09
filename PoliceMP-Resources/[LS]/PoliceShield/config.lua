main = {
    commandName = "shield",
    commandEnabled = true,
    commandChatSuggestion = "Use a police shield",
    parameterType = "Shield Type", -- Use this to change to a different language
    enableDeveloperCommand = true, -- Toggle the developer command (/shieldposition) which can assist in positioning the shields
    networkedWhenDeveloping = true, -- Toggle whether the shield is synced to other players when using the shieldposition command
    developerBoneIndex = 24818, -- The bone index to attach the shield to when using the shield position command
    developerCollision = true, -- Whether collision should be enabled when using the shield position command
    offSetMovement = 0.009,
    rotationMovement = 0.7,
    developerAnimDict = "combat@combat_reactions@pistol_1h_hillbilly",
    developerAnimName = "180",
    defaultShield = "long2",
}

translations = {
    noArguments = "~r~Error~w~: You must specify the shield type",
    invalidShieldType = "~r~Error~w~: You must specify the shield type",
    outsideVehicle = "~r~Error~w~: You must be outside a vehicle",
    shieldRemoved = "~g~Success~w~: Shield removed",
    invalidShieldModel = "Invalid Shield Model - Usage /shieldposition [model]",
    noItem = "~r~Error:~w~: You must have a valid shield item",
}

Notifications = {
    Enabled = true,
    Framework = {
        QBCore = false, 
        QBX = false,
        ESX = false,
        vRP = false, 
        okok = false, -- https://okok.tebex.io/package/4724993
    }
}

Inventory = {
    oxInventory = false,
    quasarInventory = false,
    coreInventory = false,
    ESX = false,
    vRP = false,
    qbCore = false,
}


Permissions = {
    EnablePermissions = false,
    AcePermissions = {
        Enabled = false,
        -- This enables ace permissions on the shield command
    },
    -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
    ESX = {
        Enabled = false,
        CheckJob = {
            Enabled = false, -- Enable this to use ESX job check
            Jobs = {"police"} -- A user can have any of the following jobs, allowing you to add multiple
        }
    },
    -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
    vRP = {
        Enabled = false,
        CheckGroup = {
            Enabled = false, -- Enable this to use vRP group check
            Groups = {"police"}, -- A user can have any of the following groups, meaning you can add different jobs
        },
        CheckPermission = {
            Enabled = false, -- Enable this to use vRP permission check
            Permissions = {"police"} -- A user can have any of the following permissions, allowing you to add multiple
        },
    },
    -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
    QBCore = {
        Enabled = false,
        CheckJob = {
            Enabled = false, -- Enable this to use QBCore job check
            Jobs = {"police"}, -- A user can have any of the following jobs, meaning you can add different jobs
        },
        CheckPermission = {
            Enabled = false, -- Enable this to use QBCore permission check
            Permissions = {"police"}, -- A user can have any of the following permissions, allowing you to add multiple
        },
    },
    QBX = {
        Enabled = false,
        CheckJob = {
            Enabled = false, -- Enable this to use QBX job check
            Jobs = {"police"}, -- A user can have any of the following jobs, meaning you can add different jobs
        }
    },
}

-- Shield Names must be one word for command usage
-- eg /shield ballistic

shields = {
    {
        name = "firearms",
        model = `bv_shield1`,
        offSet = {-0.081, 0.432, 0.0},
        rotation = {0.7, 0.0, 180.6},
        boneIndex = -1,
        collision = true,
        animDict = "combat@combat_reactions@pistol_1h_hillbilly",
        animName = "180",
        allowShooting = true,
    },
    {
        name = "ctsfo",
        model = `bv_shield2`,
        offSet = {0.0, 0.387, 0.0},
        rotation = {0.0, 0.0, 185.5},
        boneIndex = -1,
        collision = true,
        animDict = "combat@combat_reactions@pistol_1h_hillbilly",
        animName = "180",
        allowShooting = true,
    },
    {
        name = "small",
        model = `bv_shield3`,
        offSet = {0.0, 0.288, 0.0},
        rotation = {0.0, 0.0, 163.1},
        boneIndex = -1,
        collision = true,
        animDict = "combat@combat_reactions@pistol_1h_hillbilly",
        animName = "180",
        allowShooting = true,
    },
    {
        name = "long",
        model = `bv_shield4`,
        offSet = {0.045, 0.405, -0.108},
        rotation = {0.7, -0.7, 171.5},
        boneIndex = -1,
        collision = true,
        animDict = "combat@combat_reactions@pistol_1h_hillbilly",
        animName = "180",
        allowShooting = true,
    },
    {
        name = "ctsfo2",
        model = `prop_shield_one`,
        offSet = {-0.59, 0.29, 0.15},
        rotation = {0.16, 79.04, 41.39},
        boneIndex = 24818,
        collision = true,
        animDict = "combat@combat_reactions@pistol_1h_hillbilly",
        animName = "180",
        allowShooting = true,
    },
    {
        name = "firearms2",
        model = `prop_shield_two`,
        offSet = {-0.59, 0.29, 0.15},
        rotation = {0.16, 79.04, 41.39},
        boneIndex = 24818,
        collision = true,
        animDict = "combat@combat_reactions@pistol_1h_hillbilly",
        animName = "180",
        allowShooting = true,
    },
    {
        name = "long2",
        model = `prop_shield_three`,
        offSet = {-1.2, 0.31, -0.05},
        rotation = {1.0, 74.39, 41.39},
        boneIndex = 24818,
        collision = true,
        animDict = "combat@combat_reactions@pistol_1h_hillbilly",
        animName = "180",
        allowShooting = true,
    },
    {
        name = "short2",
        model = `prop_shield_four`,
        offSet = {-0.68, 0.22, 0.15},
        rotation = {0.52, 79.28, 41.39},
        boneIndex = 24818,
        collision = true,
        animDict = "combat@combat_reactions@pistol_1h_hillbilly",
        animName = "180",
        allowShooting = true,
    },
}