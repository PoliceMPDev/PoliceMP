--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

ClientConfig = {
    -- [[ Language System  ]] -- 
    -- All language codes available in the languages folder.
    m_languageCode = "en", -- The language code.


-- add_ace group.stjohns "medical1" allow
-- add_ace group.clinicaltrainer "medical2" allow
-- add_ace group.clinicalTL "medical3" allow
-- add_ace group.hart "medical4" allow
-- add_ace group.HEMSTrainer "medical5" allow
-- add_ace group.HEMSTL "medical6" allow
-- add_ace group.HEMS "medical7" allow
-- add_ace group.paramedic "medical8" allow
-- add_ace group.lasAdvPara "medical9" allow
-- add_ace group.lasDoctor "medical10" allow
-- add_ace group.las.sectionleader "medical11" allow
-- add_ace group.CMO "medical12" allow
-- add_ace group.DOO "medical13" allow
-- add_ace group.normaldev "medical14" allow
-- add_ace group.admin "medical15" allow

    AcePermission1 = "Medical1", -- stjohns
    AcePermission2 = "Medical2", -- clinicaltrainer
    AcePermission3 = "Medical3", -- clinicalTL
    AcePermission4 = "Medical4", -- hart
    AcePermission5 = "Medical5", -- HEMSTrainer
    AcePermission6 = "Medical6", -- HEMSTL
    AcePermission7 = "Medical7", -- HEMS
    AcePermission8 = "Medical8", -- Student paramedic
    AcePermission9 = "Medical9", -- Paramedic
    AcePermission10 = "Medical10", -- Advanced paramedic
    AcePermission11 = "Medical11", -- lasDoctor
    AcePermission12 = "Medical12", -- Section Leader
    AcePermission13 = "Medical13", -- CMO
    AcePermission14 = "Medical14", -- DOO
    AcePermission15 = "Medical15", -- normaldev
    AcePermission16 = "Medical16", -- admin
    AcePermission17 = "Medical17", -- PC
    AcePermission18 = "Medical18", -- AFO


    -- [[ General Settings ]] --
    -- General settings about this script.
    m_damageEnableCooldown = 3000, -- After this time in ms, the damage system will be enabled (useful when player dies on spawn for example).
    m_maxInteractionDistance = 2.0, -- Measured in gta distance units
    m_lastDamageCooldown = 210, -- The cooldown when the next damage can be handled after damage has happended (recommend to leave it at default value. | also in ~ms)
    m_nearbyPlayerDistance = 4.0, -- The distance in gta units, when a player is considered nearby.
    m_vehicleScanRadius = 6.0, -- The vehicle scan radius in gta units.
    m_disabledWeapons = { GetHashKey("WEAPON_PLASMAP") }, -- The weapons that will be ignored by the damage system.
    m_ox_target_support = false, -- If true, the ox target system will be enabled. (see: c_functions.lua line 505)
    m_update_queue_interval = 250, -- The interval in ms, when the health buffer update queue will be executed.

    -- [[ Keys            ]] --
    m_configurableKeys = {
        m_useInputMapper = true, -- Set this to 'true' to use RegisterKeyMapping instead of IsControlJustPressed.
                                 -- This will allow you to change the keybinds in the controls menu.
                                 -- (WARNING: When enabled, DisableControlAction will not work anymore, so there may be issues with other scripts.)
                                 -- (To avoid issues with other scripts, implement our exports in the scripts for example to block opening the inventory while being dead.)
        keys = {
            -- enable: true/false -- When disabled only the commands will work. (Only when using input mapper)
            -- To disable keybind entirely, set m_useInputMapper to false and set enabled to false on each action.
            -- control_id: https://docs.fivem.net/docs/game-references/controls/)
            -- control_key: https://docs.fivem.net/docs/game-references/input-mapper-parameter-ids/

            OPEN_SELF_MENU = { enabled = true, control_id = 246, input_parameter = "Y" }, -- Y Key 
            OPEN_OTHER_MENU = { enabled = true, control_id = 74, input_parameter = "H" }, -- H Key
            EMERGENCY_DISPATCH = { enabled = true, control_id = 47, input_parameter = "G" }, -- G Key
            CANCEL_INTERACTION = { enabled = true, control_id = 73, input_parameter = "X" }, -- X Key
            MANUAL_RESPAWN = { enabled = true, control_id = 311, input_parameter = "K" } -- K Key
        },
    },

    -- [[ Menu Settings    ]] --
    -- Settings about the menu.
    m_onlyShowActionsIfPlayerHasRequiredItems = false, -- Set this to 'true' to only show actions, if the player has the required items. (could affect performance)

    -- [[ Feature Settings ]] --
    -- Settings about the features, here you can enable or disable some features.
    m_enableSewings = true, -- Enable this to enable the sewing system.
    m_sewingBloodLoss = 0.1, -- The amount of blood loss when having a sewed wound.
    m_blurScreenOnHighBloodLoss = true, -- Enable this to blur the screen when high blood loss is detected. (bloodVolume <= 4200ml)
    m_allowManualRespawnWhileBeingUnconscious = true, -- Set this to 'true' to allow manual respawn after time has expired. (disables automatic respawn)

    m_limpingFeature = { -- Enable this to make the player limp if the player has a injury that should make him limp.
        enabled = true,
        ragdollWhileRunning = {
            enabled = true, -- Enable this to enable the ragdoll while running if the player has a injury that should make him limp.
            chance = 25, -- The chance in percent.
            ragdollType = 3, -- The ragdoll type.
            time = 2000, -- The time in ms.
        }
    },

    m_showTriage3dMarkers = true, -- Enable this to show the triage markers in 3d.
    m_triage3dMarkersDistance = 7.0, -- The distance in gta units, when the triage markers will be shown.
    m_update_triage_info_interval = 1000, -- The interval in ms, when the triage info will be updated.

    m_respawnOnCriticalBloodVolume = false, -- Enable this to respawn the player when the blood volume is critical.



    -- [[ BETA FEATURES ]] -- (Issues may occur)



    m_lowerHeartRatePerSecondOnUncounscious = 0.06, -- The amount of the lowering heart rate per second, when the player is unconscious. | 0.0 to disable
    m_lowerHeartRatePerSecondOnUncounsciousNonRecoveryMode = 4.54, -- The amount of the lowering heart rate per second, when the player is unconscious and non recovery mode. | 0.0 to disable
    m_nonRecoveryModeOnZeroHeartRateSince = 60000 * 15, -- The time in ms, when the player will go into "non-recovery-mode" when the heart rate is 0. (After 3 minutes of zero heart rate player has a fatal brain function loss) | 0 to disable!
    m_nonRecoveryModeOnFatalInjury = {  -- The non-recovery mode on injury settings.
        body_parts = { "HEAD", "TORSO" }, -- The body parts that will trigger the non-recovery mode on fatal injury. 
        chance = 0.03, -- The chance in percent, when the player will go into "non-recovery-mode" when the player got a injury. | 0.0 to disable this feature
        injuries = { -- The injuries that will be checked
            "avulsion",
            "velocity_wound"
        }
    },

    -- [[ BETA FEATURES END ]] -- 

    m_bodybagUnconsciousTime = 60, -- The time in seconds, when the player will be unconscious after being put in a bodybag.

    m_enableWeaponAimShakeOnArmInjury = { -- Shakes the screen when aiming and having an arm injury.
        enabled = true, -- Enable this to enable the weapon aim shake on arm injury. (beta)
        painFactor = 0.1, -- The pain factor. (ClientHealthBuffer.painLevel * ClientConfig.m_enableWeaponAimShakeOnArmInjury.painFactor)
        shakeType = "SMALL_EXPLOSION_SHAKE", -- See for more: https://docs.fivem.net/natives/?_0xFD55E49555E017CF
    },

    m_weaponDisableAfterBeingRevived = { -- Disable weapons after being revived.
        enabled = false, -- Enable this to disable weapons after being revived.
        time = 30000, -- The time in ms.
    },

    m_emergencyDispatch = { -- Enable this to enable the emergency dispatch system. (Button press while being dead to alert emergency services)
        enabled = false, -- Enable this to enable the emergency dispatch system (or disable it :D).
        cooldown = 5, -- The cooldown in seconds.
        phoneConfiguration = "woodyspanic", -- The default phone configuration. ("esx_phone", "visn_phone", "gcphone", "dphone", "roadphone", "qs-smartphone", "gksphone", "emergencydispatch", "custom" -> edit in helpers/c_functions.lua|l:244)
        receivers = { "" } -- The jobs that will receive a message/notification when you alert the emergency dispatch.
    },

    m_spawnGameObjects = { -- Enable this to enable the spawn game objects feature (bandages on ground etc).
        enabled = true, -- WARNING: When enabled be careful with restarting this script, since it will crash the game on every client because of the custom streamed props.
        --@todo Woody - Change
        lifetime = 60000 * 5, -- The timeout in ms. When the timeout is exceeded the game objects will be removed.
        limit = 50 -- The limit of game objects that can be spawned.s
    },


    m_delayAnimationStart = true, -- Delay the animation start of the unconscious animation. Deaths will look smoother with this. (WARNING: If the resource "spawnmanager" is used, then the autospawn feature needs to be disabled!)




    -- [[ Screen Effects   ]] --
    -- Settings about the screen effects.
    m_enabledScreenEffects = { "bleeding", "pain" }, -- The enabled screen effects. ("bleeding", "pain")




    -- [[  Controls        ]] --
    -- Settings about the controls.
    m_enabledControlActionsWhenUnconscious = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                          11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                                          21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
                                          31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
                                          41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
                                          51, 52, 53, 54, 55, 56, 57, 58, 59, 60,
                                          61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
                                          71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
                                          81, 82, 83, 84, 85, 86, 87, 88, 89, 90,
                                          91, 92, 93, 94, 95, 96, 97, 98, 99, 100,
                                          101, 102, 103, 104, 105, 106, 107, 108, 109, 110,
                                          111, 112, 113, 114, 115, 116, 117, 118, 119, 120,
                                          121, 122, 123, 124, 125, 126, 127, 128, 129, 130,
                                          131, 132, 133, 134, 135, 136, 137, 138, 139, 140,
                                          141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
                                          151, 152, 153, 154, 155, 156, 157, 158, 159, 160,
                                          161, 162, 163, 164, 165, 166, 167, 168, 169, 170,
                                          171, 172, 173, 174, 175, 176, 177, 178, 179, 180,
                                          181, 182, 183, 184, 185, 186, 187, 188, 189, 190,
                                          191, 192, 193, 194, 195, 196, 197, 198, 199, 200,
                                          201, 202, 203, 204, 205, 206, 207, 208, 209, 210,
                                          211, 212, 213, 214, 215, 216, 217, 218, 219, 220,
                                          221, 222, 223, 224, 225, 226, 227, 228, 229, 230,
                                          231, 232, 233, 234, 235, 236, 237, 238, 239, 240,
                                          241, 242, 243, 244, 245, 246, 247, 248, 249, 250,
                                          251, 252, 253, 254, 255, 256, 257, 258, 259, 260,
                                          261, 262, 263, 264, 265, 266, 267, 268, 269, 270,
                                          271, 272, 273, 274, 275, 276, 277, 278, 279, 280,
                                          281, 282, 283, 284, 285, 286, 287, 288, 289, 290,
                                          291, 292, 293, 294, 295, 296, 297, 298, 299, 300,
                                          301, 302, 303, 304, 305, 306, 307, 308, 309, 310,
                                          311, 312, 313, 314, 315, 316, 317, 318, 319, 320,
                                          321, 322, 323, 324, 325, 326, 327, 328, 329, 330,
                                          331, 332, 333, 334, 335, 336, 337, 338, 339, 340,
                                          341, 342, 343, 344, 345, 346, 347, 348, 349, 350,
                                          351, 352, 353, 354, 355, 356, 357, 358, 359, 360},
                                          
 -- The enabled control actions when unconscious.
    m_enabledControlActionsWhenCarrying = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
    11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
    21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
    31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
    41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
    51, 52, 53, 54, 55, 56, 57, 58, 59, 60,
    61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
    71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
    81, 82, 83, 84, 85, 86, 87, 88, 89, 90,
    91, 92, 93, 94, 95, 96, 97, 98, 99, 100,
    101, 102, 103, 104, 105, 106, 107, 108, 109, 110,
    111, 112, 113, 114, 115, 116, 117, 118, 119, 120,
    121, 122, 123, 124, 125, 126, 127, 128, 129, 130,
    131, 132, 133, 134, 135, 136, 137, 138, 139, 140,
    141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
    151, 152, 153, 154, 155, 156, 157, 158, 159, 160,
    161, 162, 163, 164, 165, 166, 167, 168, 169, 170,
    171, 172, 173, 174, 175, 176, 177, 178, 179, 180,
    181, 182, 183, 184, 185, 186, 187, 188, 189, 190,
    191, 192, 193, 194, 195, 196, 197, 198, 199, 200,
    201, 202, 203, 204, 205, 206, 207, 208, 209, 210,
    211, 212, 213, 214, 215, 216, 217, 218, 219, 220,
    221, 222, 223, 224, 225, 226, 227, 228, 229, 230,
    231, 232, 233, 234, 235, 236, 237, 238, 239, 240,
    241, 242, 243, 244, 245, 246, 247, 248, 249, 250,
    251, 252, 253, 254, 255, 256, 257, 258, 259, 260,
    261, 262, 263, 264, 265, 266, 267, 268, 269, 270,
    271, 272, 273, 274, 275, 276, 277, 278, 279, 280,
    281, 282, 283, 284, 285, 286, 287, 288, 289, 290,
    291, 292, 293, 294, 295, 296, 297, 298, 299, 300,
    301, 302, 303, 304, 305, 306, 307, 308, 309, 310,
    311, 312, 313, 314, 315, 316, 317, 318, 319, 320,
    321, 322, 323, 324, 325, 326, 327, 328, 329, 330,
    331, 332, 333, 334, 335, 336, 337, 338, 339, 340,
    341, 342, 343, 344, 345, 346, 347, 348, 349, 350,
    351, 352, 353, 354, 355, 356, 357, 358, 359, 360 }, -- The enabled control actions when carrying.
    m_disabledControlGroups = { 0, 1, 2 }, -- The disabled control groups.

    -- [[ Respawn Settings ]] --
    -- Settings about the respawn. (WARNING: When m_dependUnconsciousTimeOnMedicCount is enabled, the respawn time here will be overwritten. See: server_config.lua)
    m_respawnConfiguration = { -- The respawn configuration.
        m_respawnTime = 1800, -- The default bleedout/death time in seconds (can be extended trough cpr...).
        --Woody change before
        m_respawnLocations = -- The nearest location to the player will be selected.
        {
            { x = 348.35, y = -563.7, z = 29.72, heading = 246.58 } -- The default respawn location.
        }
    },

    -- [[ Default Settings ]] --
    -- Settings about the default settings. (recommend to leave at default)
    m_defaultValues = {
        HEART_RATE = 80, -- The default heart rate.
        BLOOD_VOLUME = 6000, -- Blood volume in milliliters
        PERIPH_RES = 100, -- Peripheral resistance
        IV_CHANGE_PER_SEC = 4.1667, -- 250ml should be done in 60s. 250ml / 60s ~ 4.1667 ml/s.
        BANDAGE_REOPENING_CHANCE = 0.1, -- Bandage reopening chance.
        BANDAGE_REOPENING_MIN_DELAY = 120, -- Bandage reopening min delay in seconds.
        BANDAGE_REOPENING_MAX_DELAY = 200, -- Bandage reopening max delay in seconds.
    },



    -- [[ Debug Settings   ]] --
    -- Settings about the debug.
    m_debugModeEnabled = false, -- Enable this to get debug informations (sometimes useful, sometimes not :/)
    m_debugModeModules = { -- Enable this to get debug informations for the specified modules.
        "injuries",
        "infusions",
        "bandages",
        "medications",
        "interactions",
        "state_bags",
    }


    
}
