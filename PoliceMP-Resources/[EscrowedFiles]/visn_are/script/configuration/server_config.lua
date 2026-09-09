--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

ServerConfig = {
    -- [[ General Settings    ]] --
    m_itemsNeeded = false, -- Set this 'true', if you want that players need items to perform actions.
    m_reviveCommand = true, -- Set this 'true', if you want to enable the integrated revive command. For permissions see s_functions.lua (IsAllowedToUseReviveCommand)
    mod_revive = true,

    ace_enabled = true,




    -- [[ Custom ESX Settings ]] --
    m_esxSharedObject = {
        useExport = false, -- Set this 'true', if you want to use the ESX Shared Object instead of the old event
        resourceName = "es_extended", -- Set the resource name of the ESX (needed for export)
        eventName = "esx:getSharedObject", -- Event name of the ESX Shared Object.
    },

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

    -- [[ Custom QBCore Settings ]] --
    m_qbCoreResourceName = "qb-core", -- Set the resource name of the QBCore (needed for export)

    

    m_customInventory = { -- If you use a custom inventory
        enabled = false,
        inventory_type = "qs-inventory", -- supported: ox_inventory , qs-inventory, mf-inventory, core_inventory
    },

    -- [[ Feature Settings    ]] --
    m_ignoreItemsNeededJobs = { "ambulance" }, -- A table of jobs that ignore the that players need items to perform actions.

    m_dependUnconsciousTimeOnMedicCount = {
        enabled = false, -- Set this to 'true', if you want that the system will depend on the medic count.
        jobs = { "ambulance" }, -- A table of jobs that will count to the final count of medics.
        overwrites = { -- Keep in order: Lowest to highest!
            -- Format: [Medic count as number] = Time in seconds
            [0] = 60 * 5, -- 5 Minutes when medicCount >= 0
            [2] = ClientConfig.m_respawnConfiguration.m_respawnTime, -- Default time when medicCount >= 2
        }
    },

    m_limitMenuToJobs = { -- Limits the menu to certain jobs.
        enabled = false, -- Set this 'true', if you want that the system will limit the menu to certain jobs.
        jobs = { "ambulance" }, -- A table of jobs that are allowed to use the menu.
    },

    m_triageSystem = {
        enabled = true, -- Set this 'true', if you want that the triage system is enabled.
        jobRestriction = false, -- Set this 'true', if you want that the triage system is restricted to certain jobs.
        jobs = { "" }, -- A table of jobs that are allowed to use the triage system.
    },

    m_stateSaving = { -- This feature will save the state of the players (like injuries, blood pressure) to a file (recommend) or mysql database.
        enabled = false, -- Set this to 'true', if you want that the system will save the state of the players.
        interval = 60 * 5, -- Set this to the interval in seconds, that the system will save the state of the players.
        method = 'file', -- 'oxmysql', 'mysql-async' or 'file' (recommended)
        database = { -- Set this to the database settings, if you use oxmysql.
            table = 'users', -- Set this to the table name, where the state will be saved.
            column = 'health_state', -- The column in the table, that will be used to save the state of the players.
            identifierColumn = 'identifier' -- The column in the table, that will be used to query the player.
        },
        file = { -- Set this to the file settings, if you use file.
            path = '/database/', -- Set this to the path of the file, where the system will save the state of the players.
            type = 'message_pack', -- The type of serialization. message_pack (recommended) or json
        }
    },

    m_discordLogging = { -- This feature will log kill logs to discord.
        enabled = true,  -- Set this to 'true', if you want that the system will log messages to Discord.
        healthBuffer = true, -- Set this to 'true', if you want that the system will also log the healthBuffer on a kill.
        webhook = 'https://discord.com/api/webhooks/1345306261872312441/92DGAi1fFhEchi60JqAhx0WE_3Ei6rg1JvsZRT50l0tKTjoIZazdBZXwh77cjoQRtuNG' -- Set this to the webhook URL of your discord channel.s
    },

    -- [[ Menu Settings    ]] --
    m_showNameOfPlayerOnMenuTitle = true, -- Set this to 'true', if you want that the system will show the name of the player on the menu title. Set this to 'false', if you want that the system will not show the name of the player on the menu title.

    -- [[ Respawn Settings ]] -- 
    m_respawnConfiguration = {
        m_removeMoneyOnDeath = false, -- Set this to 'true', if you want that players lose money on death.
        m_removeItemsOnDeath = false, -- Set this to 'true', if you want that players lose items on death.
        m_removeWeaponsOnDeath = false, -- Set this to 'true', if you want that players lose weapons on death.
    },

    -- [[ Debug Settings      ]] --
    m_debugModeEnabled = false, -- Set this to 'true', if you want expanded informations about things that are going on.
    m_debugModeModules = { "items" },
}