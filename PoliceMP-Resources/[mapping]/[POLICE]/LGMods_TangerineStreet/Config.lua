Config = {}

Config.Framework = 'standalone' -- Accepted values are standalone, oEssentials, PoliceEssentials, QBCore, ESX, TMC.
-- If you are using the old ESX version, make this "ESX-Legacy"

Config.AllowedJobs = { -- This is only checked when Framework is set to QBCore, ESX or TMC.
    "police",
    "Job2"
}

Config.ShowPrompt = true -- Show the prompt in the top left of the screen with what button to Press, will also disable the pressing of E to open the gate

Config.TargettingEnabled = false -- If you want to use the "third-eye" functionality for Panic Strips
Config.Target = "qtarget" -- The name of the targetting framework you use for 3rd Eye integration

Config.CodeRequired = true -- If set to true, despite the user being authorised above they will need a code.
Config.GateCode = "1234" -- MAXIMUM OF 4 CHARACTERS

Config.GateCommand = "tangerinegate"

Config.GateIncrement = 0.001
Config.PedGateIncrement = 0.15
Config.SoundRadius = 20.0
Config.MaxVolume = 1.0
