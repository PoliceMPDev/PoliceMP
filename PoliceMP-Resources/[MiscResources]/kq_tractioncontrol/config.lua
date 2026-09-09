Config = {}

-- This enabled additional debug commands and logs
Config.debug = false

Config.traction = {
    -- The amount of grip that will be reduced during drift (higher = less grip)
    -- (Values between 0 & 60 = more realistic)
    -- (Values between 60 & 100 = more drifty)
    -- Values between 0 and 100
    reduceGrip = 50,

    -- How easily the vehicle will enter into drift (Higher numbers = more traction loss required for it to enter drift, aka, lower numbers will be more drifty)
    -- Values between 0.0 and 2.0
    driftThreshold = 1.0,
}

-- Whether or not to display that the traction control is turned off on the dashboard of the car
Config.showOnDashboards = true

-- Size of the 'ESP OFF' text on the dashboard
Config.fontScale = 0.15

----------------------------------------------------------------------------------------------
--- WHITE/BLACKLISTING
----------------------------------------------------------------------------------------------

-- Disallow certain vehicles or classes of vehicles from toggling the traction control
Config.blacklist = {
    -- List of models that should not be able to toggle the traction control
    vehiclesEnabled = true,
    vehicles = {
      'police',
      'taxi',
    },
    
    -- Classes that should not be allowed to disable traction control (numerical values) https://docs.fivem.net/natives/?_0x29439776AAA00A62
    classesEnabled = true,
    classes = {
        8, 10, 11, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22
    },
}

-- Opposite of the blacklist. Only allows certain models/vehicle models to toggle their traction control
Config.whitelist = {
    vehiclesEnabled = false,
    -- List of models that should be able to toggle the traction control
    vehicles = {
      'futo',
      'futo2',
      'rt3000',
      'yosemite2',
      'comet6',
      'gauntlet4',
      'euros',
      'zr350',
      'dominator8',
    },
    
    classesEnabled = false,
    -- The only classes that should be allowed to disable traction control (numerical values) https://docs.fivem.net/natives/?_0x29439776AAA00A62
    classes = {
        3, 4, 5, 6
    },
}

-- Whether or not to apply a slight boost to vehicles engine power while mid drift (Helps lower powered vehicles stay in drift) (1.0 is the base car power)
Config.driftBoost = {
    enabled = true,
    power = 1.75
}

-- https://docs.fivem.net/docs/game-references/controls/
-- Use the input index for the "input" value
Config.keybinds = {
    toggle = {
        label = '[',
        input = 100,
        holdDuration = 1000,
    }
}