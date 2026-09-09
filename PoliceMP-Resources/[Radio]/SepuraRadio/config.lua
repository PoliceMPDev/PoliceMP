Config = {}

-- ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
-- ─██████████████─██████████████─██████──────────██████─██████████████─████████████████───██████████████─██████─────────
-- ─██░░░░░░░░░░██─██░░░░░░░░░░██─██░░██████████──██░░██─██░░░░░░░░░░██─██░░░░░░░░░░░░██───██░░░░░░░░░░██─██░░██─────────
-- ─██░░██████████─██░░██████████─██░░░░░░░░░░██──██░░██─██░░██████████─██░░████████░░██───██░░██████░░██─██░░██─────────
-- ─██░░██─────────██░░██─────────██░░██████░░██──██░░██─██░░██─────────██░░██────██░░██───██░░██──██░░██─██░░██─────────
-- ─██░░██─────────██░░██████████─██░░██──██░░██──██░░██─██░░██████████─██░░████████░░██───██░░██████░░██─██░░██─────────
-- ─██░░██──██████─██░░░░░░░░░░██─██░░██──██░░██──██░░██─██░░░░░░░░░░██─██░░░░░░░░░░░░██───██░░░░░░░░░░██─██░░██─────────
-- ─██░░██──██░░██─██░░██████████─██░░██──██░░██──██░░██─██░░██████████─██░░██████░░████───██░░██████░░██─██░░██─────────
-- ─██░░██──██░░██─██░░██─────────██░░██──██░░██████░░██─██░░██─────────██░░██──██░░██─────██░░██──██░░██─██░░██─────────
-- ─██░░██████░░██─██░░██████████─██░░██──██░░░░░░░░░░██─██░░██████████─██░░██──██░░██████─██░░██──██░░██─██░░██████████─
-- ─██░░░░░░░░░░██─██░░░░░░░░░░██─██░░██──██████████░░██─██░░░░░░░░░░██─██░░██──██░░░░░░██─██░░██──██░░██─██░░░░░░░░░░██─
-- ─██████████████─██████████████─██████──────────██████─██████████████─██████──██████████─██████──██████─██████████████─
-- ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

Config.Debug = false -- Make this true if you require support or asked by OfficerGalvin.

Config.Framework = 'Standalone' -- Standalone, ESX, QB, ESX Legacy.
Config.VoiceResource = 'PMA' -- PMA or Mumble.
Config.VoiceResourceName = 'pma-voice' -- The resource name of your selected voice resource.
Config.SetMumbleRadioName = true -- If this is true AND YOUR USING MUMBLE, it will set the users Radio Name as there setup Identifier.

Config.Frequencies = {
    ['PAN LONDON'] = {
        'MET DESP SOUTH', -- This is a channel that is used for MET assets operating in the South of the City (Map will reflect this with a line on the map - but In the city itself)
        'MET DESP NORTH', -- This is a channel that is used for MET assets operating in the NORTH of the City (Map will reflect this with a line on the map - Sandy, paleto etc)
        'LAS OPS', -- New home for LAS, where all units will come and shout for intial response to any job
        'LFB OPS', -- New home for LFB, where all units will come and shout for intial response to any job
        'HIGHWAYS OPS', -- New home for Highways, where all units will come and shout for intial response to any job
        
    },
    ['MET OPS'] = { -- Channels are used for spontaneous police jobs ONLY (this replaces the current incident & pursuit channels)
        'TAC-OP 01',
        'TAC-OP 02',
        'TAC-OP 03',
        'TAC-OP 04',
        'TAC-OP 05',
        'TAC-OP 06',
        'TAC-OP 07',
        'TAC-OP 08',
        'TAC-OP 09',
        'TAC-OP 10',
        'TAC-OP 11',
        'TAC-OP 12',
        'TAC-OP 13',
        'TAC-OP 14',
        'TAC-OP 15'
    },
    ['FIREARMS IO'] = { -- Channels are used for spontaneous firearms jobs (this replaces the current firearms channels)
        'FIREARMS 01',
        'FIREARMS 02',
        'FIREARMS 03',
        'FIREARMS 04',
        'FIREARMS 05',
        'PADP OPS', -- New channel for PADP operations 
        'CTSFO OPS'-- New channel for CT operations 
    },
    ['INTEROPS'] = { -- Channels are used for spontaneous jobs that from the outset require a mulit agency response.
        'INCIDENT 01',
        'INCIDENT 02',
        'INCIDENT 03',
        'INCIDENT 04',
        'INCIDENT 05',
        'METHANE 01', -- Methane channels are specifically for declared METHANES by FIM / relevent command 
        'METHANE 02',
        'METHANE 03'
    },
    ['LAS'] = {
        'HEMS AIR DESK', -- New home for HEMS to operate out of, this is where LAS come to request HEMS - HEMS then move to a relevent channel (Ie TAC OPS 1 or METHANE 01) & return after St2
        'HART DESK', -- New home for HART to operate out of, this is where LAS come to requesting HART - HART then move to a relevent channel (Ie TAC OPS 1 or METHANE 01) & return after St2
        'ALPHA 01', -- Alpha channels are used by discretion of LAS
        'ALPHA 02',
        'ALPHA 03'
    },
    ['LFB'] = {
        'INCIDENT 01', -- Used for LFB jobs ie (structure fire comms / appliance groups etc)
        'INCIDENT 02',
        'INCIDENT 03',
        'INCIDENT 04',
        'INCIDENT 05',
        'OPS ASSURANCE 01', -- New channel to allow LFB command to communicate for LFB only jobs
        'OPS ASSURANCE 02',
        'OPS ASSURANCE 03'
    },
    ['MISC'] = {
        'TRAINING 01', -- As it says on the tin, college or divisonal training 
        'TRAINING 02',
        'TRAINING 03',
        'TRAINING 04',
        'TRAINING 05',
        'EVENT 01', -- Events etc
        'EVENT 02',
        'EVENT 03'
    },
}

Config.HaveOptionToShowSpeaker = true -- If this is true, the radio will show up the speaker on the Talk Group and have the option in the Settings.
Config.DefaultShowSpeaker = false  -- If this is true, on the Radio Settings, it will default to "Yes" when showing the speaker for all users. This is for the radio pop-up when someone is speaking. 

Config.RadioCommandName = 'radio1' -- If you are using Standalone, set this here.

-- When someone turns on the radio, they will connect to the channel below.
Config.InitalFolder = 'PAN LONDON'   -- Key set in Frequencies
Config.InitalTalkGroup = 'MET DESP SOUTH' -- Value of that Key.

Config.DisplayPanicCommand = true -- If you want the panic command to display.
Config.PanicCommandName = 'panic'   -- If the above is true, this is the Command Name for it.
Config.DisplayPanicChatMessage = true
Config.PlayPanicSound = true

-- ───────────────────────────────────────────────────────────────────────────────────────
-- ─██████████████─██████████████─████████──████████────██████████████───██████████████───
-- ─██░░░░░░░░░░██─██░░░░░░░░░░██─██░░░░██──██░░░░██────██░░░░░░░░░░██───██░░░░░░░░░░██───
-- ─██░░██████████─██░░██████████─████░░██──██░░████────██░░██████░░██───██░░██████░░██───
-- ─██░░██─────────██░░██───────────██░░░░██░░░░██──────██░░██──██░░██───██░░██──██░░██───
-- ─██░░██████████─██░░██████████───████░░░░░░████──────██░░██──██░░██───██░░██████░░████─
-- ─██░░░░░░░░░░██─██░░░░░░░░░░██─────██░░░░░░██────────██░░██──██░░██───██░░░░░░░░░░░░██─
-- ─██░░██████████─██████████░░██───████░░░░░░████──────██░░██──██░░██───██░░████████░░██─
-- ─██░░██─────────────────██░░██───██░░░░██░░░░██──────██░░██──██░░██───██░░██────██░░██─
-- ─██░░██████████─██████████░░██─████░░██──██░░████────██░░██████░░████─██░░████████░░██─
-- ─██░░░░░░░░░░██─██░░░░░░░░░░██─██░░░░██──██░░░░██────██░░░░░░░░░░░░██─██░░░░░░░░░░░░██─
-- ─██████████████─██████████████─████████──████████────████████████████─████████████████─
-- ───────────────────────────────────────────────────────────────────────────────────────

Config.InventoryItem = 'radio' -- The name of the inventory item you make, you do not need to Register its usage, we do that.
Config.RequireJob = false -- Make this true if the user must have a job to use this radio. You will set the job(s) in the table below.
Config.AllowedJobs = { -- The required job to have to use the radio, to appear in Contacts & view Panic Buttons. If the above bool is not true, it will just default to being allowed. 
    'Police',
    'Ambulance',
    'Fire'
}

-- ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
-- ─████████████───██████████─██████████████─██████████████─██████████████─████████████████───████████████──────██████████████─██████████████─██████████─
-- ─██░░░░░░░░████─██░░░░░░██─██░░░░░░░░░░██─██░░░░░░░░░░██─██░░░░░░░░░░██─██░░░░░░░░░░░░██───██░░░░░░░░████────██░░░░░░░░░░██─██░░░░░░░░░░██─██░░░░░░██─
-- ─██░░████░░░░██─████░░████─██░░██████████─██░░██████████─██░░██████░░██─██░░████████░░██───██░░████░░░░██────██░░██████░░██─██░░██████░░██─████░░████─
-- ─██░░██──██░░██───██░░██───██░░██─────────██░░██─────────██░░██──██░░██─██░░██────██░░██───██░░██──██░░██────██░░██──██░░██─██░░██──██░░██───██░░██───
-- ─██░░██──██░░██───██░░██───██░░██████████─██░░██─────────██░░██──██░░██─██░░████████░░██───██░░██──██░░██────██░░██████░░██─██░░██████░░██───██░░██───
-- ─██░░██──██░░██───██░░██───██░░░░░░░░░░██─██░░██─────────██░░██──██░░██─██░░░░░░░░░░░░██───██░░██──██░░██────██░░░░░░░░░░██─██░░░░░░░░░░██───██░░██───
-- ─██░░██──██░░██───██░░██───██████████░░██─██░░██─────────██░░██──██░░██─██░░██████░░████───██░░██──██░░██────██░░██████░░██─██░░██████████───██░░██───
-- ─██░░██──██░░██───██░░██───────────██░░██─██░░██─────────██░░██──██░░██─██░░██──██░░██─────██░░██──██░░██────██░░██──██░░██─██░░██───────────██░░██───
-- ─██░░████░░░░██─████░░████─██████████░░██─██░░██████████─██░░██████░░██─██░░██──██░░██████─██░░████░░░░██────██░░██──██░░██─██░░██─────────████░░████─
-- ─██░░░░░░░░████─██░░░░░░██─██░░░░░░░░░░██─██░░░░░░░░░░██─██░░░░░░░░░░██─██░░██──██░░░░░░██─██░░░░░░░░████────██░░██──██░░██─██░░██─────────██░░░░░░██─
-- ─████████████───██████████─██████████████─██████████████─██████████████─██████──██████████─████████████──────██████──██████─██████─────────██████████─
-- ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

Config.UsingBagderDiscordAPI = false -- Set this to true to use BadgerDiscordAPI checks on who can use this command / item.
Config.BaderDiscordAPIFolderName = "Badger_Discord_API" -- The name of the folder (Allows for Export).
Config.BagderDiscordAPIDiscordRoleIDs = { -- The Discord ID's that are allowed to use this.
    "902321769959030846",
    "108717881968197663",
    "908717881968197662"
}
-- ──────────────────────────────────────────────────────────────────────────────────────────────────────────────
-- ─██████████████─██████──────────██████─██████████████────██████████████─██████──────────██████─██████████████─
-- ─██░░░░░░░░░░██─██░░██████████████░░██─██░░░░░░░░░░██────██░░░░░░░░░░██─██░░██████████████░░██─██░░░░░░░░░░██─
-- ─██░░██████████─██░░░░░░░░░░░░░░░░░░██─██░░██████████────██░░██████████─██░░░░░░░░░░░░░░░░░░██─██░░██████░░██─
-- ─██░░██─────────██░░██████░░██████░░██─██░░██────────────██░░██─────────██░░██████░░██████░░██─██░░██──██░░██─
-- ─██░░██████████─██░░██──██░░██──██░░██─██░░██████████────██░░██─────────██░░██──██░░██──██░░██─██░░██████░░██─
-- ─██░░░░░░░░░░██─██░░██──██░░██──██░░██─██░░░░░░░░░░██────██░░██─────────██░░██──██░░██──██░░██─██░░░░░░░░░░██─
-- ─██░░██████████─██░░██──██████──██░░██─██████████░░██────██░░██─────────██░░██──██████──██░░██─██░░██████░░██─
-- ─██░░██─────────██░░██──────────██░░██─────────██░░██────██░░██─────────██░░██──────────██░░██─██░░██──██░░██─
-- ─██░░██─────────██░░██──────────██░░██─██████████░░██────██░░██████████─██░░██──────────██░░██─██░░██──██░░██─
-- ─██░░██─────────██░░██──────────██░░██─██░░░░░░░░░░██────██░░░░░░░░░░██─██░░██──────────██░░██─██░░██──██░░██─
-- ─██████─────────██████──────────██████─██████████████────██████████████─██████──────────██████─██████──██████─
-- ──────────────────────────────────────────────────────────────────────────────────────────────────────────────

-- ONLY READ THIS SECTION IF YOU ARE USING AND HAVE PURCHASED ALBO FMS OR OMSOLUTIONS CMA (AVALIABLE AT https://albo1125.com/fms & https://omsolutions.co.uk/)

Config.FMS = false -- Make this true if using FMS.
Config.CMA = false -- Make this true if using CMA.

Config.ChannelGroupFolders = { -- These folders will make Teamspeak actions (Assign the Channel Group via FMS / CMA's exports). 
    'SOUTH EAST'
}
Config.MoveChannelFolders = { -- These folders will make Teamspeak actions (Move the player to the channel, via FMS / CMA's exports).
    'MET-MP'
}

Config.CanCivilianAccessRadio = true -- If you make this false, then anyone can use the Radio, will see Panic Buttons & will be in Contacts.
Config.CivilianPatrolBranchName = 'Civilian' -- Please note currentPatrolBranch and currentPatrolBranchDivision are based on 1) the player’s currently booked on unit on CAD or 2) the player’s duty assignment for the currently active patrol.