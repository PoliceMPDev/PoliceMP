Config = {}

-- Distance threshold for TCAS alert in meters
Config.TCASAlertDistance = 500.0

-- MP3 URLs for alert sounds
Config.TCASAlertSoundURL = 'https://mcdn.podbean.com/mf/web/kex92rj4qfgqmfvx/p_30420923_215.mp3'
Config.ClearOfConflictSoundURL = 'https://mcdn.podbean.com/mf/web/4d3ymygmh7kkf2bh/p_30420905_201.mp3'

-- Audio settings
Config.Volume = 0.3  -- 0.0 to 1.0
Config.Loop = false  -- Should the sound loop
Config.PlayOnVehicle = true  -- Play sound relative to vehicle

-- Debug options
Config.Debug = false  -- Toggle debug mode

-- Postal settings
Config.PostalList = {
    ["200"] = true, -- Skyscraper Zone
    ["396"] = true,
    ["397"] = true,
    ["398"] = true,
    ["399"] = true,
    ["392"] = true,
    ["393"] = true,
    ["383"] = true,
    ["382"] = true,
    ["390"] = true,
    ["389"] = true,
    ["1003"] = true, -- Zancudo
    ["1002"] = true,
    ["947"] = true, -- Prison
}

Config.PostalAlertSoundURL = 'https://static.wixstatic.com/mp3/772067_4718dd0f3cd148d48b13abff9ac9aa63.mp3' -- URL for postal alert sound
Config.PostalAlertDuration = 5000 -- Duration for postal alert notification in milliseconds
