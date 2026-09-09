/*--------------------------------------
  % Made with ❤️ for: Rytrak Store
  % Author: Rytrak https://rytrak.fr
  % Script documentation: https://docs.rytrak.fr/scripts/advanced-extrication-system
  % Full support on discord: https://discord.gg/k22buEjnpZ
--------------------------------------*/

-- [[ Configuration ]]

Config = {
    Language = 'en', -- Language library used for the script, see the last lines to modify the text of the language. (Config.Languages)

    /*---------------------------------------------------------------------------------
                                          IMPORTANT
    If you are using ESX or QBCore, please set the UseFramework variable to true below, then follow 
    the video tutorial in our documentation to make the script compatible.
    This variable is used to remove the command to give the weapon and thus use it from your inventory.

    For ESX: https://docs.rytrak.fr/framework-compatibility/add-a-custom-weapon-on-esx
    For QBCore: https://docs.rytrak.fr/framework-compatibility/add-a-custom-weapon-on-qbcore
                                          IMPORTANT
    ---------------------------------------------------------------------------------*/
    UseFramework = false,

    Spreaders = {
        openSpeed = 0.04, -- Tool opening speed (the larger the number, the faster the speed)
        closeSpeed = 0.1, -- Tool closing speed (the larger the number, the faster the speed)
        
        damageSpeed = 0.5 -- Speed of percentage of life decrease (the larger the number, the faster the speed)
    },

    Cutters = {
        openSpeed = 0.25, -- Tool opening speed (the larger the number, the faster the speed)
        closeSpeed = 0.1, -- Tool closing speed (the larger the number, the faster the speed)

        damageSpeed = 0.75 -- Speed of percentage of life decrease (the larger the number, the faster the speed)
    },

    CutProtection = {
        delay = 4000 -- Time to install the protective cover (in ms)
    },

    LockPeopleInsideCar = { -- Blocks the vehicle and the player when an accident occurs
        enabled = false,

        minDamage = 35.0, -- Minimum damage for blocking

        requiredCutRoof = true, -- All doors and the roof must be cut away before the person can exit the vehicle

        -- Minimum number of people in a job for blocking
        ESX = false,
        QBCore = false,

        minJobsRequired = { -- Minimum job to trigger the system (works only with ESX or QBCore above)
            ['firefighter'] = 3
        }
    },

    VehicleRoof = { -- List of vehicles compatible with the roof cut-off system (see tutorial on how to make a vehicle compatible https://docs.rytrak.fr/scripts/advanced-extrication-system/adapt-a-vehicle-to-the-roof-system)
        ['asterope'] = {
            extra = 11
        },
        ['dilettante'] = {
            extra = 9,
            brokenDoorRequired = {5}
        },
        ['dubsta'] = {
            extra = 9,
            brokenDoorRequired = {5}
        },
        ['ingot'] = {
            extra = 9,
            brokenDoorRequired = {5}
        },
        ['jugular'] = {
            extra = 9
        },
        ['asbo'] = {
            extra = 9,
            brokenDoorRequired = {5}
        },
        ['tailgater'] = {
            extra = 9
        },
        ['taxi'] = {
            extra = 12
        },
        ['futo'] = {
            extra = 9
        },
        ['baller'] = {
            extra = 9,
            brokenDoorRequired = {5}
        }
    },

    CooldownDelay = 200, -- Delay in ms to avoid spamming tool usage

    HelpSphere = {
        select = {
            color = {255, 0, 0},
            opacity = 0.2
        },
        unselect = {
            color = {0, 0, 255},
            opacity = 0.2
        }
    }
}

-- https://docs.fivem.net/docs/game-references/controls/
Config.Keys = {
    ActionToolKey = 24, -- Key to operate the tool
    ActionToolKeyString = '~INPUT_ATTACK~', -- Key string to operate the tool

    ChangeModeKey = 311, -- Key to change the mode
    ChangeModeKeyString = '~INPUT_REPLAY_SHOWHOTKEY~', -- Key string to change the mode

    ProtectKey = 38, -- Key to to protecting the area with blankets
    ProtectKeyString = '~INPUT_PICKUP~' -- Key string to to protecting the area with blankets
}

-- Libraries of languages.
Config.Languages = {
    ['en'] = {
        ['weapon_spreader'] = "Spreader",
        ['weapon_cutter'] = "Cutter",
        ['weapon_glassmaster'] = "Glass master",
        ['tool'] = "Press "..Config.Keys.ChangeModeKeyString.." to change the mode\nPress "..Config.Keys.ActionToolKeyString.." to use the tool\n\nMode: {s}",
        ['toolmode_opening'] = 'clamp opening',
        ['toolmode_closure'] = 'clamp closure',
        ['cut_protection'] = "Press "..Config.Keys.ProtectKeyString.." to protect the zone",
        ['glass'] =  "Press "..Config.Keys.ActionToolKeyString.." to cut glass",
        ['helpcircle'] = "Go to the blue circle to start",
        ['startshredder'] = "You can start the spreading",
        ['startcut'] = "You can start the cutting",
        ['doorstate'] = "State of door: ",
        ['windowstate'] = "State of window: ",
        ['hingestate'] = "State of hinge: ",
        ['breakrearwindow'] = "You must cut the rear window"
    },
    ['fr'] = {
        ['weapon_spreader'] = "Écarteur",
        ['weapon_cutter'] = "Cisaille",
        ['weapon_glassmaster'] = "Coupe vitre",
        ['tool'] = "Presse "..Config.Keys.ChangeModeKeyString.." pour changer de mode\nPresse "..Config.Keys.ActionToolKeyString.." pour utiliser l'outil\n\nMode: {s}",
        ['toolmode_opening'] = 'ouvrir la pince',
        ['toolmode_closure'] = 'fermer la pince',
        ['cut_protection'] = "Presse "..Config.Keys.ProtectKeyString.." pour protéger la zone",
        ['glass'] = "Presse "..Config.Keys.ActionToolKeyString.." pour couper la vitre",
        ['helpcircle'] = "Rendez vous dans le cercle bleu pour commencer",
        ['startshredder'] = "Vous pouvez commencer l'écartement",
        ['startcut'] = "Vous pouvez commencer la coupure",
        ['doorstate'] = "État de la porte: ",
        ['windowstate'] = "État de la vitre: ",
        ['hingestate'] = "État de la charnière: ",
        ['breakrearwindow'] = "Vous devez couper la vitre arrière"
    }
}