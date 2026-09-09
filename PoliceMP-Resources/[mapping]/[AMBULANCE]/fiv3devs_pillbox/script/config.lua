Config = {}

-- Common Settings
Config.Target = 'ox_target' -- Can be 'ox_target', 'qb_target' or false (false will disable chairs and beds, only leaving the elevator, MRI, X-RAY working)
Config.isCustomFont = false -- Use a custom font if needed
Config.fontName = 'fontname' -- Custom font name if isCustomFont is true, font needs to be streamed

-- Seat Settings
Config.Seats = true -- enable seating inside the hospital exclusively for custom chairs. Original game chairs can utilize your own script.
Config.Beds = true -- enable laying inside the hospital exclusively for custom beds. Original game beds can utilize your own script.
Config.OxTextLay = 'Lay Down' -- text to display for ox_target or qb_target
Config.OxTextGetUp = 'Get Up'
Config.OxTextSit = 'Sit Down'
Config.OxTextStand = 'Stand Up'
Config.TextSeats = '[~b~E~w~] Sit' -- text to display for when Config.Target is set to false (DISABLED)
Config.Text = '[~b~E~w~] Lay'
Config.StopKey = 73 -- key to stand up

-- MRI and X-RAY Settings
Config.EnableMRI = true -- enable the MRI and X-RAY machine
Config.OxTextLayMri = 'Lay Down'
Config.OxTextStartMri = 'Start MRI'
Config.OxTextStartXray = 'Start XRay'
Config.OxTextGetUpMri = 'Get Up'
Config.LayKey = 38 -- only for when Config.Target is set to false
Config.MRIKey = 23 -- key for MRI and X-RAY
Config.TextMRI = '[~b~F~w~] Start MRI'
Config.TextXRAY = '[~b~F~w~] Start X-RAY'
Config.syncInterval = 50 -- interval for syncing the MRI animation in milliseconds (lower values improve visual quality but decrease performance)

-- Elevator Settings
Config.EnableElevator = true -- enable or disable the elevator
Config.OxText = 'Elevator' -- text to display for ox_target or qb_target
Config.TextElevator = 'Press [~g~E~w~] to use.' -- text to display for when Config.Target is set to false
Config.ElevatorKey = 38 -- key for the elevator when target is disabled
Config.EnabledUnderground = true -- enable or disable the elevator's underground/offices floors
Config.EnabledOffices = true
Config.EnableJobs = false -- enable job restrictions for ESX or QBCore
Config.RestrictedFloors = { -- which floors to restric and for which jobs (names for floors are: lower, main, offices, roof, er, rec)
    ["lower"] = {'ambulance', 'police'},
    ["offices"] = {'ambulance', 'police'},
}
Config.DisabledTextSubject = 'Sorry!' -- in case Config.EnableJobs is set to true, this is the message that will appear for players trying to access that floor. 
Config.DisabledText = 'This floor is currently unavailable.'
Config.AccessText = 'You are not authorized to access this floor.'
Config.DisabledTextName = 'Pillbox Hill Medical Center'