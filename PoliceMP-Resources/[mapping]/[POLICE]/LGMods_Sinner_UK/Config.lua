Config = {}

Config.Framework = 'standalone' -- Accepted values are standalone, oEssentials, PoliceEssentials, QBCore, TMC, ESX. If you are using ESX uncomment the shared_scripts in the fxmanifest.lua

Config.AllowedJobs = { -- This is only checked when Framework is set to QBCore, TMC or ESX.
    "Job1",
    "Job2"
}

Config.ShowPrompt = true -- Show the prompt in the top left of the screen with what button to Press, will also disable the pressing of E to open the gate

Config.Items = {
    Enabled = false,
    oxInventory = false -- Set this to true to use ox_inventory
}

Config.Targetting = {
    Enabled = false, -- If you want to use the "third-eye" functionality for keypads
    Framework = "qtarget" -- The name of the targetting framework you use for 3rd Eye integration, if you use ox_target leave this as qtarget as well.
}

Config.Gate = {
    ActivationDistance = 3.0, -- The distance fromt he panels to interact with the gate.
    TimeOpen = 10000, -- How long should the gate be open, 1000 is 1 second.
}

Config.GateCode = {
    Required = true, -- If set to true, despite the user being authorised above they will need a code.
    Code = "1234" -- MAXIMUM OF 4 CHARACTERS
}

Config.Commands = {
    OpenGate = "sinnersgate", -- The command to open the gates
    ResetAlarm = "resetalarm", -- The command to reset the alarms
}

Config.Sounds = { -- This is used to determine the volume of the gate and alarm sounds for users.
    Radius = 20.0,
    Volume = 1.0 -- Maximum volume is 1.0
}

-- I would not recommend touching these options unless you know what you are doing.
Config.GateIncrement = 0.002
Config.PedGateIncrement = 0.30
