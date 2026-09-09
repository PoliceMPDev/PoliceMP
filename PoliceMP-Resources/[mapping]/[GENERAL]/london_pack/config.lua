-- ben_config.lua
BenConfig = {}

-- ✅ Debug Mode (Set to `true` to see logs)
BenConfig.Debug = false  

-- ✅ Sound Volume (1.0 = 100%)
BenConfig.SoundVolume = 0.5

-- ✅ Chime Times (VPS Time Format HH:MM)
BenConfig.ChimeTimes = {
    {time = "01:00", sound = "https://www.youtube.com/watch?v=FqO-il3jgMs"},  -- play for 1 o'clock AM uk time
    {time = "02:00", sound = "https://www.youtube.com/watch?v=SVm9FnwHlzE"},  -- play for 2 o'clock AM uk time 
    {time = "03:00", sound = "https://www.youtube.com/watch?v=v12_3fsbZ3I"},  -- play for 3 o'clock AM uk time
    {time = "04:00", sound = "https://www.youtube.com/watch?v=Mj3oBDYOrVs"},  -- play for 4 o'clock AM uk time
    {time = "05:00", sound = "https://www.youtube.com/watch?v=GV0Z9uGOH-g"},  -- play for 5 o'clock AM uk time
    {time = "06:00", sound = "https://www.youtube.com/watch?v=cjfkc3fvcUQ"},  -- play for 6 o'clock AM uk time
    {time = "07:00", sound = "https://www.youtube.com/watch?v=RnOUBlor-pw"},  -- play for 7 o'clock AM uk time
    {time = "08:00", sound = "https://www.youtube.com/watch?v=FXAQdHPqi3k"},  -- play for 8 o'clock AM uk time
    {time = "09:00", sound = "https://www.youtube.com/watch?v=LEtF25kFZ8U"},  -- play for 9 o'clock AM uk time
    {time = "10:00", sound = "https://www.youtube.com/watch?v=VGHXDXca1cM"},  -- play for 10 o'clock AM uk time
    {time = "11:00", sound = "https://www.youtube.com/watch?v=Ds5GHqqmczM"},  -- play for 11 o'clock AM uk time
    {time = "12:00", sound = "https://www.youtube.com/watch?v=Zxt6HEjaMHg"},  -- play for 12 o'clock AM uk time
    {time = "13:00", sound = "https://www.youtube.com/watch?v=FqO-il3jgMs"},  -- play for 1 o'clock PM uk time
    {time = "14:00", sound = "https://www.youtube.com/watch?v=SVm9FnwHlzE"},  -- play for 2 o'clock PM uk time
    {time = "15:00", sound = "https://www.youtube.com/watch?v=v12_3fsbZ3I"},  -- play for 3 o'clock PM uk time  
    {time = "16:00", sound = "https://www.youtube.com/watch?v=Mj3oBDYOrVs"},  -- play for 4 o'clock PM uk time
    {time = "17:00", sound = "https://www.youtube.com/watch?v=GV0Z9uGOH-g"},  -- play for 5 o'clock PM uk time
    {time = "18:00", sound = "https://www.youtube.com/watch?v=cjfkc3fvcUQ"},  -- play for 6 o'clock PM uk time
    {time = "19:00", sound = "https://www.youtube.com/watch?v=RnOUBlor-pw"},  -- play for 7 o'clock PM uk time
    {time = "20:00", sound = "https://www.youtube.com/watch?v=FXAQdHPqi3k"},  -- play for 8 o'clock PM uk time
    {time = "21:00", sound = "https://www.youtube.com/watch?v=LEtF25kFZ8U"},  -- play for 9 o'clock PM uk time
    {time = "22:00", sound = "https://www.youtube.com/watch?v=VGHXDXca1cM"},  -- play for 10 o'clock PM uk time
    {time = "23:00", sound = "https://www.youtube.com/watch?v=Ds5GHqqmczM"},  -- play for 11 o'clock PM uk time
    {time = "00:00", sound = "https://www.youtube.com/watch?v=Zxt6HEjaMHg"},  -- play for 12 o'clock PM uk time

}



-- ✅ Big Ben Location (Where the sound plays)
BenConfig.BigBenCoords = vector3(572.1044, -1471.2594, 99.9730)

-- ✅ Distance Settings
BenConfig.PlayDistance = 300.0  -- Max distance players can hear the sound


Config = {}

-- Add Discord IDs of players allowed to use the command
Config.DiscordWhitelist = {
    "350307895570595841", -- JB
    "561903735521935364", -- Joe
    "370060187228176388", -- Boyle
    "282102097472782336", -- Avery
    "418824008067317764", -- Forbs
    "808543796870643722", -- Smallp
    "1193354082849673297" -- Wazza
    
}

Config.BridgeSpawnTrafficDelay = 5000      -- Time to wait before spawning traffic (after audio starts)
Config.BridgeOpenDelay = 5000               -- Time to wait before opening bridge (after traffic spawns)
Config.BridgeStayOpen = 30000                -- ✅ Time bridge stays fully open (30 seconds)
Config.cooldownTime = 10000                  -- Cooldown after bridge closes (10 seconds)
Config.openDuration = 30000                  -- Time bridge takes to open (match client setting if needed)
Config.scanCooldown = 10000                  -- Delay for ticket scanning (if used)




-- elevator_system.lua
ElevatorMenus = {
    [1] = { floors = {
        { name = 'Floor 68', coords = vector4(458.3935, -612.5328, 265.9955, 79.6714) },
        { name = 'Floor 69', coords = vector4(461.5558, -613.1940, 269.4970, 256.9430) },
        { name = 'Floor 72', coords = vector4(461.2177, -613.0905, 286.9939, 77.9231) }
    }},
    [2] = { floors = {
        { name = 'Floor 0', coords = vector4(470.75, -611.09, 36.72, 262.88) },
        { name = 'Floor 1', coords = vector4(506.64, -623.92, -41.96, 80.38) }
    }},
    [3] = { floors = {
        { name = 'Floor 1', coords = vector4(476.92, -619.08, -38.44, 175.98) },
        { name = 'Floor 33', coords = vector4(460.8461, -623.5603, -38.6183, 83.8121) }
    }},
    [4] = { floors = {
        { name = 'Floor 33', coords = vector4(454.8680, -615.0303, -38.6183, 80.2392) },
        { name = 'Floor 68', coords = vector4(462.4250, -607.8111, 265.9985, 308.8471) }
    }},
    [5] = { floors = {
        { name = 'G Floor', coords = vector4(458.4413, -647.8163, 28.2093, 351.0034) },
        { name = '21 Floor', coords = vector4(454.9868, -614.2839, 115.7140, 85.4669) }
    }},
    [6] = { floors = {
        { name = 'Floor 0', coords = vector4(131.4204, -731.3226, 47.0875, 110.1323) },
        { name = 'Floor 40', coords = vector4(144.1865, -747.1318, 220.1762, 6.5493) }
    }},
    [7] = { floors = {
        { name = 'Floor 0', coords = vector4(133.4638, -735.6442, 47.0875, 112.8125) },
        { name = 'Floor 40', coords = vector4(141.4765, -747.5757, 220.1778, 1.4418) }
    }},
    -- New Reception Menu (Allows players to choose any flat)
    [8] = { floors = {
        { name = 'Reception', coords = vector4(452.8548, -618.4780, 41.0375, 79.8350) },
        { name = 'Flat 1', coords = vector4(464.2458, -601.0779, 58.7629, 346.0989) },
        { name = 'Flat 2', coords = vector4(464.9820, -599.0594, 66.3571, 349.9292) },
        { name = 'Flat 3', coords = vector4(465.2615, -599.5980, 75.7370, 357.9744) },
        { name = 'Flat 4', coords = vector4(466.2035, -599.3253, 84.3801, 352.6070) },
        { name = 'Flat 5', coords = vector4(466.0155, -610.9890, 123.4503, 352.2768) },
        { name = 'Flat 6', coords = vector4(465.1064, -610.7938, 105.8299, 357.8238) },
        { name = 'Flat 7', coords = vector4(467.5832, -608.1786, 133.8262, 357.0242) },
        { name = 'Flat 8', coords = vector4(467.5741, -608.1126, 142.4454, 353.4269) },
        { name = 'Flat 9', coords = vector4(472.4020, -611.9294, 156.3231, 91.6378) },
        { name = 'Flat 10', coords = vector4(469.2406, -608.7914, 168.8998, 354.3193) },
        { name = 'Flat 11', coords = vector4(470.6384, -608.7086, 189.8442, 359.3429) },
        { name = 'Flat 12', coords = vector4(471.1239, -609.0370, 200.3453, 358.2238) }
    }},
    [9] = { floors = {
        { name = 'Floor G', coords = vector4(139.4670, -627.1714, 47.0863, 145.5166) },
        { name = 'Floor 37', coords = vector4(137.3989, -632.2646, 192.4315, 337.3337) }
    }},
    [10] = { floors = {
        { name = 'Floor G', coords = vector4(133.0629, -643.9985, 47.0862, 341.5906) },
        { name = 'Floor 37', coords = vector4(134.7975, -639.0717, 192.4315, 158.3401) }
    }}
	

    -- Repeat for all other flats...
}





