/*--------------------------------------
  % Made with ❤️ for: Rytrak Store
  % Author: Rytrak https://rytrak.fr
  % Script documentation: https://docs.rytrak.fr/scripts/firefighter-scba-system
  % Full support on discord: https://discord.gg/k22buEjnpZ
--------------------------------------*/

-- [[ Configuration ]]

Config = {
    Language = 'en', -- Language library used for the script, see the last lines to modify the text of the language. (Config.Languages)

    UseOutdatedVersion = false, -- Enable this parameter to suppress alert messages in the console if you wish to use an older version of the script.
    
    ESX = { -- ESX compatibility (you can modify this function on cl_utils.lua)
        enabled = false,
        jobs = {
            'firefighter',
            'ems',
        }
    },

    QB = { -- QB compatibility (you can modify this function on cl_utils.lua)
        enabled = false, -- Activate or not the QB system
        jobs = { -- Job for which the grab ped can be used
            'firefighter',
            'ems',
        }
    },

    CommandEnabled = false, -- Activate or not the command to use the SCBA script other than an EUP (You can adjust the SCBA usage from the Command - 'scba', - Command to use the SCBA without the cup.
    Command = 'scba', -- Command to use the SCBA without the eup.
    
    DisablePutMask = false, -- Disable or not the key to link or untie the SCBA (often used to link a script to lock other than a key) (used with the SetSCBA() export)

    EUP = {
        Untie = {
            [`mp_m_freemode_01`] = { -- Male
                [8] = { -- 0: Face\ 1: Mask\ 2: Hair\ 3: Torso\ 4: Leg\ 5: Parachute / bag\ 6: Shoes\ 7: Accessory\ 8: Undershirt\ 9: Kevlar\ 10: Badge\ 11: Torso 2
                    component = 214
                }
            },
            [`mp_f_freemode_01`] = { -- Female
                [8] = {
                    component = 214
                }
            }
        },
        Link = {
            [`mp_m_freemode_01`] = { -- Male
                [8] = {
                    component = 213
                }
            },
            [`mp_f_freemode_01`] = { -- Female
                [8] = {
                    component = 213
                }
            }
        }
    },

    UntieComponent = 15, -- Component when you remove the SCBA.

    RechargeVehicle = { -- List of vehicles to recharge your SCBA.
        `addpolfirevan`,
        `CSU`,
        `LfbBasu`,
        
        
    },
    RechargeCoords = {
        vector3(1164.67, -1494.17, 34.85),
        vector3(1188.5, -1472.38, 34.69),
        vector3(-583.4959, -2162.5449, 6.9272)
        
    }, -- Coordinated to recharge your SCBA other than on a vehicle.
    RechargeRadius = 3.0, -- Distance from the coordinates below to recharge your SCBA other than on a vehicle.

    TakeCar = { -- Gives the player the possibility to take his SCBA in vehicles
        `firetruk`,
        `fire11`,
        `lfb1`,
        `lfb2`,
        `lfb5`,
        `lfb8`,
        `addpolfirevan`,
        `addpolfiretruk`,
        `electriclfb`,
        `reserve2`,
        `lfb10`,
        `lfb_training`,
        `manrural`,
        `l200rescue`,
        `l200rescue1`,
        `mprv`,
        `NBFiretruck`,
        `NBFiretruck1`,
        `2004scaniacroy`,
        `2004scaniamill`,
        `2004scaniarom`,
        `fire17`,
        `fire18`,
        `fire16`,
        `reserve3`,
        `reserve4`,
        `hart1`,
        '25hart4',
        '25hart1',
        't61kom',
        
    },
    DriverTake = false, -- Allows the driver to retrieve the SCBA from the vehicle list above

    TakeCoords = {
        vector3(1200.25, -1487.31, 35.14),
        vector3(1199.22, -1473.9, 34.69)
    }, -- Coordinated to retrieve an SCBA. (disabled when using CommandEnabled)
    TakeRadius = 3.0, -- Distance from the coordinates to retrieve an SCBA. (disabled when using CommandEnabled)

    RechargeSpeed = 0.25, -- Speed to recharge the pressure.
    
    Pressure = 300, -- Pressure when the SCBA is first used. (Max: 400)
    PressureAnim = true, -- Animation of the gauge.

    VisualEffect = 'DefaultColorCode', -- Visual effect when you have the SCBA on your face. (https://wiki.rage.mp/index.php?title=Timecycle_Modifiers)

    UseInVehicle = false, -- Use the SCBA in a vehicle.

    DecreaseTime = { -- Every x millisecond the DecreaseDetails will decrease.
        Sprinting = 100, -- When you make a physical effort.
        Walking = 200, -- When you walk.
        Still = 300 -- When you do nothing.
    },

    DecreaseDetails = { -- The level at wich the pressure drops.
        Sprinting = 0.2, -- When you make a physical effort.
        Walking = 0.17, -- When you walk
        Still = 0.12 -- When you do nothing
    },

    AlarmSound = {50, 150}, -- List of pressure alerts.

    Volume = 0.3, -- Volume of sounds. (You can modify the sounds from the 'html' directory)

    SCBASyncSound = false, -- Sound of the SCBA synchronized to the other players.

    VoiceEffect = true, -- Muffled voice when the player is wearing the SCBA (only if you use pma-voice) (FOLLOW OUR DOCUMENTATION FOR THIS: https://docs.rytrak.fr/scripts/firefighter-scba-system/general-configuration#config.voiceeffect)
}

Config.FireDamage = {
    SmartFires = { -- Make compatible or not the Smart Fires script of London Studios, if yes, the basic fire will not be detected anymore.
        enabled = true,
        resourceName = 'SmartFires' -- Resource name
    },

    CoughInInterior = true, -- Cough when you are near a fire and you don't have SCBA on you interior.
    CoughInExterior = true, -- Cough when you are near a fire and you don't have SCBA on you exterior.
    DamageNearFire = 4, -- Damage when you are near a fire. (You can modify the damage function from the 'cl_utils.lua' file)
    RadiusGetFire = 9.0, -- Distance you are required to cough near a fire.
    MinimumFire = 5, -- Detection of the size of the fire to cough.
    CoughSound = true, -- Activate or not the local sounds when you cough.
    CoughDelay = 5000, -- Time to cough.
    CoughSync = false, -- Synchronize the sound when you cough with the other players around you.

    -- EUP of the oxygen mask.
    EvacuationEnable = true, -- Activate or not the oxygen mask eup.
    EvacuationMask = { -- You can use this free resources: https://www.gta5-mods.com/player/avon-m50-gas-mask
        [`mp_m_freemode_01`] = { -- Male
            [1] = { -- 0: Face\ 1: Mask\ 2: Hair\ 3: Torso\ 4: Leg\ 5: Parachute / bag\ 6: Shoes\ 7: Accessory\ 8: Undershirt\ 9: Kevlar\ 10: Badge\ 11: Torso 2
                component = 228
            }
        },
        [`mp_f_freemode_01`] = { -- Female
            [1] = {
                component = 228
            }
        }
    }
}

-- Design configuration
Config.NUI = {
    Design = {
        NUI = true, -- Activate or not the design at the bottom right of the screen.
        Text = true -- Activate or not the text at the bottom of the screen.
    },

    -- Size of the design at the bottom right of the screen.
    SizeNUI = {
        screenX = 0.905,
        screenY = 0.83
    },

    VisibleHint = true, -- Activate or not the notification in top left. (You can modify hint function in 'cl_utils.lua' file)
    HintSound = false -- Activate or not the notification in top left sound. (You can modify hint function in 'cl_utils.lua' file)
}

-- https://docs.fivem.net/docs/game-references/controls/
Config.Keys = {
    PutMaskKey = 45, -- Key to set or remove the SCBA.
    PutMaskKeyString = '~INPUT_RELOAD~', -- Name of the key to set or remove the SCBA.

    VerifyKey = 86, -- Key to verify the SCBA.
    VerifyKeyString = '~INPUT_VEH_HORN~', -- Name of the key to verify the SCBA.

    ReloadKey = 47, -- Key to reload the SCBA, give a oxygen mask and retrieve a oxygen mask.
    ReloadKeyString = '~INPUT_DETONATE~', -- Name of the key to reload the SCBA, give a oxygen mask and retrieve a oxygen mask.
}

-- Libraries of languages.
Config.Languages = {
    ['en'] = {
        ['takescba'] = 'To take a SCBA press '..Config.Keys.PutMaskKeyString,
        ['dropscba'] = 'To deposit your SCBA press '..Config.Keys.PutMaskKeyString,
        ['giveoxygen'] = 'To give a oxygen mask press '..Config.Keys.ReloadKeyString,
        ['returnoxygen'] = 'To retrieve a oxygen mask press '..Config.Keys.ReloadKeyString,
        ['link'] = 'To put a SCBA press '..Config.Keys.PutMaskKeyString,
        ['untie'] = 'To untie a SCBA press '..Config.Keys.PutMaskKeyString,
        ['verify'] = 'To verify your SCBA press '..Config.Keys.VerifyKeyString,
        ['reload'] = 'To reload your SCBA press '..Config.Keys.ReloadKeyString,
        ['pressure'] = 'Pressure: ~r~{s} bar'
    },
    ['fr'] = {
        ['takescba'] = 'Pour prendre votre ARI presse '..Config.Keys.PutMaskKeyString,
        ['dropscba'] = 'Pour déposer votre ARI presse '..Config.Keys.PutMaskKeyString,
        ['giveoxygen'] = 'Pour donner un masque presse '..Config.Keys.ReloadKeyString,
        ['returnoxygen'] = 'Pour reprendre le masque presse '..Config.Keys.ReloadKeyString,
        ['link'] = 'Pour capler votre ARI presse '..Config.Keys.PutMaskKeyString,
        ['untie'] = 'Pour décapler votre ARI presse '..Config.Keys.PutMaskKeyString,
        ['verify'] = 'Pour vérifier votre ARI presse '..Config.Keys.VerifyKeyString,
        ['reload'] = 'Pour recharger votre ARI presse '..Config.Keys.ReloadKeyString,
        ['pressure'] = 'Pression: ~r~{s} bar'
    }
}