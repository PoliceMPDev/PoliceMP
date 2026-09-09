-- Please read our documentation before configuring
-- https://docs.londonstudios.net/
-- If you are still having issues, contact us

-- Discord link: https://discord.gg/AtPt9ND

main = {
    oxInventory = false, -- To use the lighter :-)
    lighterFireSize = 3.0,
    lighterOffset = {x = 0.0, y = 0.0, z = 3.0}, -- You can change these values to change the offset of the fire spawned from the actual item itself
    fireSpawnDistance = 200.0, -- This is the distance the player must be within to a fire to spawn in (for performance)
    smokeSpawnDistance = 250.0,-- This is the distance the player must be within to smoke to spawn in (for performance)
    usingHoseLS = true, -- Also enable this if you are using SmartHose or another hose system that works similar.
    distanceToSpawnFiresInFront = 5.5, -- Distance to spawn fires in front (to avoid player being damaged initially)
    maxWidthOfFiresMultipler = 1.2, -- Max width of fires. E.g, a 15 flame fire would have a maximum width of 22.5m (rounded to nearest integer)
    maximumFinalWidth = 20.0, -- This is the maximum width, incase the multiplier leads the maximum to be too big
    foam = {
        enabled = true,-- This will enable foam mode, allowing you to use /foam to spray foam through the hose (particles are the same, but fires will respond differently)
        -- Foam mode can only be enabled if you are using Hose LS above
    },
    minimumSizeToExtinguish = 0.2, -- This is the minimum size a fire can be whilst being put out before the script removes it completely

    debugMode = false,

    useMythicNotify = false,

    distanceToExtinguish = 8.0,

    explosionFires = {
        startFireOnExplosion = false, -- This will start a fire when an explosion occurs
        fireSize = 2.0, -- This is the size of the fire that will be spawned from an explosion
        fireType = "normal", -- This is the type of fire that will be spawned from an explosion
        fireSpreadable = true, -- This will allow the fire to spread
        ignoreFireTypes = {1}, -- These explosion types wont start a smart fires fire - fire types here; https://altv.stuyk.com/docs/articles/tables/explosions.html
    },

    societyPayments = {
        qbManagement = false,
        qbBossMenu = false,
        esxAddonAccount = false,
        societyName = "firefighters",
        amountPerFire = {3000, 7000} -- Random number between 3k and 7k
    },
    -- This section allows you to setup spreadable fires
    spreadableFires = {
        automaticFires = false, -- This will allow automatic fires to spread
        manualFires = true, -- This will set whether manual fires will spread by default (unless disabled manually when running the command)
        spreadTimer = 120, -- Fires will spread every 4 minutes (240 seconds)
        spreadDistance = 5.0, -- Distance fires can spread
    },

    automaticFires = {
        enabled = true,
        toggledOnInitially = true,
        enableLocationCommand = {
            enabled = false, -- The command will give you your current location to insert here, if enabled
            commandName = "mylocation",
            locationColour = "~b~", -- blue
        },
        -- This will enable the area of patrol settings, allowing you to choose an area of patrol where automatic fires will spawn, live in game
        -- If this setting is not enabled, you can still categorise coordinates and the script will just pick a random one out of any category
        enableAreaOfPatrolSettings = {
            enabled = false,
            defaultAreaOfPatrol = "city",
            setAreaOfPatrolCommand = {
                enabled = true,
                commandName = "setfiresaop",
                acePermissions = {
                    enabled = false,
                    -- This enables ace permissions on the toggle automatic fires command
                },
                -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
                ESX = {
                    enabled = false,
                    checkJob = {
                        enabled = true, -- Enable this to use ESX job check
                        jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
                    }
                },
                -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
                vRP = {
                    enabled = false,
                    checkGroup = {
                        enabled = false, -- Enable this to use vRP group check
                        groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
                    },
                    checkPermission = {
                        enabled = false, -- Enable this to use vRP permission check
                        permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
                    },
                },
                -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
                QBCore = {
                    enabled = false,
                    checkJob = {
                        enabled = false, -- Enable this to use QBCore job check
                        jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
                    },
                    checkPermission = {
                        enabled = false, -- Enable this to use QBCore permission check
                        permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
                    },
                },
            },
            -- This command will toggle all areas of patrol on or off
            -- When toggled on, this means that fires will spawn in all areas of patrol
            toggleAllAreasOfPatrolCommand = {
                enabled = true,
                commandName = "togglefiresaop",
                acePermissions = {
                    enabled = false,
                    -- This enables ace permissions on the toggle automatic fires command
                },
                -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
                ESX = {
                    enabled = false,
                    checkJob = {
                        enabled = true, -- Enable this to use ESX job check
                        jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
                    }
                },
                -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
                vRP = {
                    enabled = false,
                    checkGroup = {
                        enabled = false, -- Enable this to use vRP group check
                        groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
                    },
                    checkPermission = {
                        enabled = false, -- Enable this to use vRP permission check
                        permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
                    },
                },
                -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
                QBCore = {
                    enabled = false,
                    checkJob = {
                        enabled = false, -- Enable this to use QBCore job check
                        jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
                    },
                    checkPermission = {
                        enabled = false, -- Enable this to use QBCore permission check
                        permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
                    },
                },
            },
        },
        
        -- Use lowercase for location categories
        locations = {
            ["city"] = {
                { coords = vector3(-292.85, 102.69, 68.26), description = "Garbagecan Eclipse Blvd", type = "normal2", size = 2.0, numberOfFlames = 1 }, -- If type and size is not defined, random will be picked
                { coords = vector3(64.25, -399.12, 39.92), description = "Garbagecan Alta Street", type = "normal2", size = 2.0, numberOfFlames = 1 }, 
                { coords = vector3(-27.33, -80.09, 57.25), description = "Garbagecan LagunaPlace", type = "normal2", size = 2.0, numberOfFlames = 1 },
                { coords = vector3(682.79, 120.06, 80.75), description = "power generator", type = "electrical", size = 10.0, numberOfFlames = 3 },
                { coords = vector3(212.01, -168.09, 56.34), description = "Garbagecan Alta Place", type = "normal", size = 2.0, numberOfFlames = 1 },
                { coords = vector3(-498.85, -1030.63, 29.13), description = "construction site fire", type = "bonfire", size = 8.0, numberOfFlames = 1 },
                { coords = vector3(-19.63, -198.12, 52.37), description = "BalvenueStreet Fire", type = "normal2", size = 3.0, numberOfFlames = 1 },
                
                
                { coords = vector3(621.00, 268.68, 103.08), description = "Gas Station Garbage", type = "bonfire", size = 8.0, numberOfFlames = 1 },


                { coords = vector3(609.28, 145.26, 97.95), description = "Electricitysecuritiy case", type = "electrical", size = 3.0, numberOfFlames = 1 },

                { coords = vector3(-185.51, -814.36, 31.77), description = "Cubepark building", type = "bonfire", size = 2.0, numberOfFlames = 1 },


                { coords = vector3(989.45, -138.61, 73.09), description = "Lost MC", type = "bonfire", size = 3.0, numberOfFlames = 1 },


                { coords = vector3(-1172.47, -1572.56, 4.66), description = "Smoke on the Water", type = "normal", size = 5.0, numberOfFlames = 2 },


                { coords = vector3(-1099.45, -1660.58, 7.36), description = "San Andreas Building Site", type = "bonfire", size = 4.0, numberOfFlames = 4},




                { coords = vector3(-1629.55, -798.92, 10.20), description = "Del Perro Beach Building Site", type = "chemical", size = 10.0, numberOfFlames = 1},




                { coords = vector3(-2096.52, -316.86, 13.02), description = "Pacific Bluffs Petrol Station", type = "chemical", size = 10.0, numberOfFlames = 6},




                { coords = vector3(-1697.22, -279.02, 51.88), description = "Pacific Bluffs Church Bins", type = "normal", size = 3.0, numberOfFlames = 1},





                { coords = vector3(-1450.35, -237.43, 49.81), description = "Morningwood Ponsobys", type = "bonfire", size = 5.0, numberOfFlames = 1},





                { coords = vector3(-1712.50, -186.96, 58.04), description = "University Bin", type = "normal", size = 8.0, numberOfFlames = 2},




                
                { coords = vector3(-2283.01, 362.91, 174.60), description = "Kortz Center Vending Machines", type = "electrical", size = 2.0, numberOfFlames = 1},



                { coords = vector3(-709.85, 2940.48, 22.92), description = "Richman Wildfire", type = "bonfire", size = 7.0, numberOfFlames = 1},




                { coords = vector3(-1158.61, -521.57, 32.83), description = "Backlot Studios Catering Van", type = "chemical", size = 2.0, numberOfFlames = 1},

                { coords = vector3(-1117.03, -503.06, 35.80), description = "Backlot Studios Catering Van", type = "chemical", size = 2.0, numberOfFlames = 1},




                { coords = vector3(-1282.72, -1117.12, 6.999), description = "Vespucci Canals Barbers", type = "bonfire", size = 1.0, numberOfFlames = 1},



                { coords = vector3(-921.88, -353.22, 38.96), description = "Portola Trinity MC - Bin", type = "normal", size = 7.0, numberOfFlames = 6},



                { coords = vector3(-796.41, -117.72, 19.05), description = "Rockford Hills - Underground Tracks", type = "electrical", size = 5.0, numberOfFlames = 6},



                { coords = vector3(-833.76, -123.75, 28.22), description = "Rockford Hills - Underground Bin", type = "electrical", size = 3.0, numberOfFlames = 3},

                { coords = vector3(-849.47, -134.19, 28.18), description = "Rockford Hills - Underground Bin", type = "electrical", size = 3.0, numberOfFlames = 3},



                { coords = vector3(-711.49, -912.12, 19.26), description = "Little Seoul - Petrol Station (Arson Attack)", type = "bonfire", size = 4.0, numberOfFlames = 1},



                { coords = vector3(-829.29, -1260.08, 5.00), description = "La Puerta/Tackle Street - Dumpster", type = "normal", size = 2.0, numberOfFlames = 2},






                { coords = vector3(-118.71, -2209.93, 7.81), description = "Banning - Globe Oil", type = "electrical", size = 4.0, numberOfFlames = 1},
                { coords = vector3(-112.43, -2215.21, 7.81), description = "Banning - Globe Oil", type = "normal", size = 6.0, numberOfFlames = 1},
                { coords = vector3(-116.49, -2211.08, 7.86), description = "Banning - Globe Oil", type = "normal", size = 10.0, numberOfFlames = 1},






                { coords = vector3(-1266.85, -2727.8789, 13.94), description = "LSIA - Jet Engine", type = "electrical", size = 4.0, numberOfFlames = 3},





                { coords = vector3(-897.30, -2754.96, 16.39), description = "LSIA - Dumpsters", type = "bonfire", size = 3.0, numberOfFlames = 1},


                { coords = vector3(-622.48, -229.67, 38.05), description = "Rockford Drive - Vangelico", type = "electrical", size = 6.0, numberOfFlames = 1},






                { coords = vector3(-929.18, -748.83, 19.84), description = "Little Seoul - BB Courts Bins", type = "normal", size = 10.0, numberOfFlames = 1},



                { coords = vector3(-525.64, -1210.91, 18.18), description = "Little Seoul - Petrol Station", type = "bonfire", size = 6.0, numberOfFlames = 1},



                { coords = vector3(-722.7173, -1496.08, 5.00), description = "La Puerta - Helipads", type = "chemical", size = 6.0, numberOfFlames = 2},


                { coords = vector3(-661.2731, -937.1542, 21.8292), description = "Little Seoul - Ammunation", type = "normal", size = 10.0, numberOfFlames = 2},
            },
            ["sandy"] = {
                { coords = vector3(1154.1, 2393.6, 57.81), description = "Redwood Track", type = "electrical", numberOfFlames = 4 },
				{ coords = vector3(849.78, 2420.47, 54.7), description = "Redwood Track 2", type = "electrical", numberOfFlames = 4 },
				{ coords = vector3(367.96, 2563.13, 43.52), description = "Harmony Trailer Park", type = "normal", size = 3.0, numberOfFlames = 6 },





				{ coords = vector3(206.14, 2476.63, 55.71), description = "Harmony 24/7", type = "normal3", size = 3.0, numberOfFlames = 1 },



				{ coords = vector3(973.53, 2722.57, 39.5), description = "Grand Senora Desert Home 1", type = "normal", size = 5.0, numberOfFlames = 3 },




				{ coords = vector3(2057.02, 3187.56, 45.68), description = "Grand Senora Desert Small Junkyard", type = "bonfire", size = 2.0, numberOfFlames = 3 },



				{ coords = vector3(1595.96, 3587.69, 38.77), description = "GSD Deserted Motel", type = "normal3", numberOfFlames = 10 },



				{ coords = vector3(1533.03, 3698.53, 34.61), description = "GSD Wrecked Trailer", type = "bonfire", numberOfFlames = 4 },













                

				{ coords = vector3(1391.68, 3603.62, 38.94), description = "GSD Methlab", type = "bonfire", numberOfFlames = 2 },



				{ coords = vector3(1788.53, 3895.95, 34.39), description = "GSD Wrecked Shelter", type = "normal", numberOfFlames = 2 },






				{ coords = vector3(2186.47, 3728.54, 34.47), description = "GSD Homeless Camp", type = "normal2", numberOfFlames = 1 },
				{ coords = vector3(2275.42, 3757.44, 38.44), description = "GSD Railway Transformer", type = "electrical", numberOfFlames = 2 },
				{ coords = vector3(2465.87, 3829.83, 40.26), description = "GSD Hotdog Stand", type = "normal", numberOfFlames = 1 },
				{ coords = vector3(2521.91, 4212.65, 40.17), description = "GSD Gas Station", type = "normal2", numberOfFlames = 3 },
				{ coords = vector3(2892.09, 4496.74, 47.98), description = "GSD Wood Pile", type = "bonfire", numberOfFlames = 2 },
				{ coords = vector3(2940.34, 4620.13, 48.72), description = "GSD Train Storage", type = "normal", numberOfFlames = 5 },
				{ coords = vector3(2680.89, 4572.22, 40.62), description = "Grapeseed Telecommunications Pole", type = "electrical", numberOfFlames = 3 },
				{ coords = vector3(2433.29, 4968.62, 42.35), description = "Grapeseed Methlab", type = "normal2", numberOfFlames = 3 },
				{ coords = vector3(2329.19, 4900.57, 41.81), description = "Grapeseed Wood Pile", type = "bonfire", numberOfFlames = 1 },
				{ coords = vector3(1677.77, 4884.26, 42.04), description = "Grapeseed Grocery Stand", type = "normal", numberOfFlames = 1 },
				{ coords = vector3(725.07, 4190.91, 40.71), description = "Grapeseed Home 1", type = "electrical", numberOfFlames = 2 },
            },
            ["paleto"] = {
                { coords = vector3(-2199.24, 4282.4, 49.17), description = "Hookies Restaurant", type = "normal", numberOfFlames = 2 },
				{ coords = vector3(-1592.22, 5193.49, 4.31), description = "Paleto Pier Cafe", type = "normal2", size = 1.0, numberOfFlames = 1 },
				{ coords = vector3(-785.29, 5398.01, 34.13), description = "Paleto Logging Camp", type = "bonfire", numberOfFlames = 3 },
				{ coords = vector3(-501.67, 5280.94, 80.62), description = "Paleto Sawmill - Plank Storage", type = "bonfire", size = 2.0, numberOfFlames = 3 },
				{ coords = vector3(-470.27, 5382.18, 79.79), description = "Paleto Sawmill - Log Storage", type = "bonfire", size = 1.0, numberOfFlames = 2 },
				{ coords = vector3(-665.16, 5809.71, 17.52), description = " Bayview Lodge - Rear Patio", type = "normal", size = 1.0, numberOfFlames = 1 },
				{ coords = vector3(-701.54, 5792.99, 17.33), description = "Bayview Lodge - Front Patio", type = "normal3", numberOfFlames = 3 },
				{ coords = vector3(-471.32, 6288.76, 13.61), description = "Paleto Beach Barn", type = "normal", size = 1.0, numberOfFlames = 2 },
				{ coords = vector3(-289.88, 6021.72, 31.51), description = "Paleto Trainyard Transformer", type = "electrical", size = 1.0, numberOfFlames = 2 },
				{ coords = vector3(-112.15, 6219.17, 31.32), description = "Clucking Bell Electrical Shed", type = "electrical", numberOfFlames = 2 },
				{ coords = vector3(-161.86, 6158.63, 31.21), description = "Clucking Bell Warehouse", type = "normal2", size = 2.0, numberOfFlames = 4 },
				{ coords = vector3(-433.9, 6156.37, 31.48), description = "Paleto Postal Depot", type = "normal2", numberOfFlames = 1 },
				{ coords = vector3(-286.86, 6307.65, 31.69), description = "Paleto Home - Bin Fire", type = "normal", numberOfFlames = 1 },
				{ coords = vector3(-178.8, 6268.88, 32.01), description = "Reds Auto Repairs", type = "normal3", size = 2.0, numberOfFlames = 2 },
				{ coords = vector3(55.73, 6645.41, 32.28), description = "Paleto Old Home", type = "normal", numberOfFlames = 2 },
				{ coords = vector3(356.38, 6476.46, 29.71), description = "Paleto Farm - Crops", type = "normal2", size = 1.0, numberOfFlames = 4 },
				{ coords = vector3(-38.31, 6579.78, 31.4), description = "Paleto Home - Garbage Bags", type = "normal2", size = 1.0, numberOfFlames = 1.0 },
				{ coords = vector3(-74.35, 6505.76, 31.49), description = "Paleto Willies Pharmacy", type = "normal", numberOfFlames = 2 },
				{ coords = vector3(-97.03, 6475.27, 31.43), description = "Paleto Savings Bank - Rear Electics", type = "electrical", numberOfFlames = 2 },
				{ coords = vector3(29.51, 6496.38, 31.43), description = "Paleto Warehouse - Crates", type = "normal", size = 2.0, numberOfFlames = 3 },
				{ coords = vector3(-61.29, 6651.01, 29.66), description = "Paleto Home - Old Sofa", type = "normal3", numberOfFlames = 1 },
				{ coords = vector3(36.8, 6631.61, 31.49), description = "Paleto Electrical Box", type = "electrical", numberOfFlames = 2 },
				{ coords = vector3(1087.65, 6510.9, 21.05), description = "Paleto Fruit Stall", type = "normal", size = 2.0, numberOfFlames = 3 },
				{ coords = vector3(1580.88, 6458.28, 25.45), description = "Paleto Diner - Trash", type = "normal2", numberOfFlames = 1 },
				{ coords = vector3(1442.06, 6332.95, 23.92), description = "Paleto Homeless Camp", type = "normal", size = 2.0, numberOfFlames = 5 },
				{ coords = vector3(1347.29, 6385.61, 33.21), description = "Paleto Electrical Transformer", type = "electrical", numberOfFlames = 3 },
				{ coords = vector3(1741.98, 6411.23, 35.02), description = "Chiliad Store - Pallets", type = "bonfire", size = 1.0, numberOfFlames = 1 },
				{ coords = vector3(2195.65, 5595.34, 53.77), description = "Chiliad Farm Storage", type = "normal3", numberOfFlames = 2 },
            },
        },
        fireTypesToSpawn = { -- Chance set out of 1
        -- To set the minimum and maximum automatic fire sizes, see each fire type individually
        -- The fire types listed here must all be valid fire types configured in the fireTypes section.
            { type = "normal", chance = 0.6},
            { type = "chemical", chance = 0.2},
            { type = "normal2", chance = 0.4},
            { type = "normal3", chance = 0.5},
            { type = "bonfire", chance = 0.1},
            { type = "electrical", chance = 0.2},
        },

        -- Below you can set the script to spawn in relation to the number of players currently on a certain job, e.g, firefighters
        -- If you aren't using a job check and just want fires to spawn randomly, ignore the job check section and just configure "frequencyOfFires" and "removeFiresAutomatically"
        -- Only enable one of the frameworks below if you want to spawn fires in relation to the number of players on a certain job
        main = {
            QBCore = { -- This enables the job check for QBCore
                enabled = false,
                jobs = {"fire", "firefighter"},
            },
            ESX = {  -- This enables the job check for ESX
                enabled = false,
                jobs = {"fire", "firefighter"},
            },
            vRP = {   -- This enables the job check for vRP
                enabled = false,
                groups = {"fire", "firefighter"},
                permissions = {}, -- Leave blank if you do not want to use permissions to spawn fires
            },
            -- This command is designed for standalone servers who still want to use automatic fires and spawn them according to the number of clocked on users
            clockOnSystem = {
                enabled = true,
                clockOnCommand = {
                    enabled = true, -- Disable this command if you are using a menu to trigger an event/export to clock people on instead
                    commandName = "clockfireon",
                    acePermissions = {
                        enabled = true,
                        -- This enables ace permissions on the clock on command
                    },
                },
                -- We do not need permission on the clock off command, as we have already checked it for them to clock on
                clockOffCommand = {
                    enabled = true,  -- Disable this command if you are using a menu to trigger an event/export to clock people off instead
                    commandName = "clockfireoff",
                },
            },
            startFiresWithLessThanMinimum = true, -- This determines whether fires should start below the minimum number of players per fire below
            playersPerFire = 1, -- This means that for every 3 players (or below) part of that group/job, we will spawn one fire (ignore this if you aren't using automatic fires)
            frequencyOfFires = 540, -- Fires will spawn every 540 seconds (9 minutes)
            removeFiresAutomatically = {
                enabled = true,
                timer = 900, -- 15 minutes will lead to the automatic removal of a fire if it is not extinguished already
            },
        },

        blips = {
            enabled = true,
            sprite = 436,
            colour = 49,
            scale = 1.0,
            shortRange = false,
            routeEnabled = true, -- This sets up a route on the map to the blip
            routeColour = 49,
        },
        
        -- These will be sent to players using jobs specified above (if you've enabled either ESX, vRP or QBCore above)
        -- Please note though, if you have not set the above up, it will be sent to all players
        -- Enable this if you aren't using any other alert system, such as our integrtation with the Inferno Pager system.
        inGameAlerts = {
            notification = true,
            sound = { -- https://wiki.rage.mp/index.php?title=Sounds (titles are the audio ref)
                enabled = true,
                soundName = "CONFIRM_BEEP",
                soundSet = "HUD_MINI_GAME_SOUNDSET",
            }
        },

        cdDispatch = {
            enabled = false,
            jobs = {'firefighter'},
            title = 'New Fire',
        },

        rcoreDispatch = {
            enabled = false,
            resourceName = "rcore_dispatch",
            jobs = {'police'},
            priority = "high",
            displayCode = "10-420",
            alertType = 'alerts',
            blipName = "New Fire",
            radius = 0,
            blipTime = 5, -- Time in seconds before the blip fades
            blipSprite = 436,
            blipColour = 1,
            blipScale = 1.5,
            blipLength = 3,
            blipflash = true,
            imageUrl = '', -- Insert your image URL here
            soundUrl = '' -- Insert your sound URL here
        },

        qsDispatch = {
            enabled = false,
            resourceName = "qs-dispatch",
            jobs = {'fire', 'police'},
            callCode = {
                code = '',
                snippet = '',
            },
            message = '',
            flashes = false,
            image = 'https://dunb17ur4ymx4.cloudfront.net/webstore/logos/30cf9edc7043455c60397ba2f12e620993426e29.png', -- URL here
            blipSprite = 436,
            blipColour = 1,
            blipScale = 1.5,
            blipLength = 3,
            blipflash = true,   
            blipText = "Fire", 
            blipTime = 60000, -- Time in milliseconds before the blip fades
        },

        sonoranCAD = { -- This requires the call commands plugin
            enabled = false,
            title = "New Fire Alert",
        },

        psDispatch = {
            enabled = false,
            jobs = {'police'},
            resourceName = "ps-dispatch",
            displayCode = "10-420",
            blipName = "New Fire",
            priority = 1,
            radius = 0,
            blipSprite = 436,
            blipColour = 1,
            blipScale = 1.5,
            blipLength = 3,
            blipflash = true,
            icon = 'fas fa-fire-extinguisher',
        },

        -- This allows integration with the Inferno Pager resource
        infernoPager = {
            enabled = true,
            pagersToTrigger = {"fire"}, -- These are the pagers to trigger
        },

        nightsSoftwareMdt = {
            enabled = false,
            resourceName = "nights-mdt",
        },

        toggleAutomaticFiresCommand = {
            enabled = true,
            commandName = "toggleautofires",
            acePermissions = {
                enabled = true,
                -- This enables ace permissions on the toggle automatic fires command
            },
            -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
            ESX = {
                enabled = false,
                checkJob = {
                    enabled = true, -- Enable this to use ESX job check
                    jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
                }
            },
            -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
            vRP = {
                enabled = false,
                checkGroup = {
                    enabled = false, -- Enable this to use vRP group check
                    groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
                },
                checkPermission = {
                    enabled = false, -- Enable this to use vRP permission check
                    permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
                },
            },
            -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
            QBCore = {
                enabled = false,
                checkJob = {
                    enabled = false, -- Enable this to use QBCore job check
                    jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
                },
                checkPermission = {
                    enabled = false, -- Enable this to use QBCore permission check
                    permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
                },
            },
        },

        triggerAutomaticFireCommand = {
            enabled = true,
            commandName = "triggerautofire",
            acePermissions = {
                enabled = true,
                -- This enables ace permissions on the trigger automatic fires command
            },
            -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
            ESX = {
                enabled = false,
                checkJob = {
                    enabled = true, -- Enable this to use ESX job check
                    jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
                }
            },
            -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
            vRP = {
                enabled = false,
                checkGroup = {
                    enabled = false, -- Enable this to use vRP group check
                    groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
                },
                checkPermission = {
                    enabled = false, -- Enable this to use vRP permission check
                    permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
                },
            },
            -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
            QBCore = {
                enabled = false,
                checkJob = {
                    enabled = false, -- Enable this to use QBCore job check
                    jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
                },
                checkPermission = {
                    enabled = false, -- Enable this to use QBCore permission check
                    permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
                },
            },
        },
    },

    startFireCommand = {
        enabled = true,
        commandName = "startfire",
        cooldownEnabled = false,
        cooldownLength = 60, -- Length of cooldown in seconds
        acePermissions = {
            enabled = true,
            -- This enables ace permissions on the start fire command
        },
        -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
        ESX = {
            enabled = false,
            checkJob = {
                enabled = true, -- Enable this to use ESX job check
                jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
            }
        },
        -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
        vRP = {
            enabled = false,
            checkGroup = {
                enabled = false, -- Enable this to use vRP group check
                groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use vRP permission check
                permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
        -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
        QBCore = {
            enabled = false,
            checkJob = {
                enabled = false, -- Enable this to use QBCore job check
                jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use QBCore permission check
                permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
        enableMultipleFlames = true,
    },

    -- This allows you to enable Discord logging for the fires and smokes
    -- You must add your webhook in sv_utils.lua
    logging = {
        enabled = true,
        displayName = "Smart Fires",
        colour = 31487,
        title = "**New Log**",
        icon = "https://i.imgur.com/n3n7JNW.png",
        footerIcon = "https://i.imgur.com/n3n7JNW.png",
        dateFormat = "%d-%m-%Y %H:%M:%S", -- Day-Month-Year Hour-Minute-Second
    },



    -- The stop fire command can be run without any arguments, this will stop the closest fire.
    -- Alternatively, it takes an argument of a distance, eg, 4.0
    stopFireCommand = {
        enabled = true,
        commandName = "stopfire",
        maxNearestDistance = 150.0, -- If no argument is given for radius, this is the maximum distance the "nearest fire" can be
        maxSpecifiedRadius = 150.0, -- This is the maximum radius that can be specified to put fires out within nearby
        acePermissions = {
            enabled = true,
            -- This enables ace permissions on the stop fire command
        },
        -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
        ESX = {
            enabled = false,
            checkJob = {
                enabled = true, -- Enable this to use ESX job check
                jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
            }
        },
        -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
        vRP = {
            enabled = false,
            checkGroup = {
                enabled = false, -- Enable this to use vRP group check
                groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use vRP permission check
                permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
        -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
        QBCore = {
            enabled = false,
            checkJob = {
                enabled = false, -- Enable this to use QBCore job check
                jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use QBCore permission check
                permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
    },



    -- This command stops all fires
    stopAllFiresCommand = {
        enabled = true,
        commandName = "stopallfires",
        acePermissions = {
            enabled = true,
            -- This enables ace permissions on the stop fire command
        },
        -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
        ESX = {
            enabled = false,
            checkJob = {
                enabled = true, -- Enable this to use ESX job check
                jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
            }
        },
        -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
        vRP = {
            enabled = false,
            checkGroup = {
                enabled = false, -- Enable this to use vRP group check
                groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use vRP permission check
                permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
        -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
        QBCore = {
            enabled = false,
            checkJob = {
                enabled = false, -- Enable this to use QBCore job check
                jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use QBCore permission check
                permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
    },

    -- This command starts smoke manually
    startSmokeCommand = {
        enabled = true,
        commandName = "startsmoke",
        acePermissions = {
            enabled = true,
            -- This enables ace permissions on the start smoke command
        },
        -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
        ESX = {
            enabled = false,
            checkJob = {
                enabled = true, -- Enable this to use ESX job check
                jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
            }
        },
        -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
        vRP = {
            enabled = false,
            checkGroup = {
                enabled = false, -- Enable this to use vRP group check
                groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use vRP permission check
                permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
        -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
        QBCore = {
            enabled = false,
            checkJob = {
                enabled = false, -- Enable this to use QBCore job check
                jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use QBCore permission check
                permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
    },



    -- The stop smoke command can be run without any arguments, this will stop the closest smoke.
    -- Alternatively, it takes an argument of a distance, eg, 4.0
    stopSmokeCommand = {
        enabled = true,
        commandName = "stopsmoke",
        maxNearestDistance = 150.0, -- If no argument is given for radius, this is the maximum distance the "nearest smoke" can be
        maxSpecifiedRadius = 150.0, -- This is the maximum radius that can be specified to put smokes out within nearby
        acePermissions = {
            enabled = true,
            -- This enables ace permissions on the stop smoke command
        },
        -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
        ESX = {
            enabled = false,
            checkJob = {
                enabled = true, -- Enable this to use ESX job check
                jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
            }
        },
        -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
        vRP = {
            enabled = false,
            checkGroup = {
                enabled = false, -- Enable this to use vRP group check
                groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use vRP permission check
                permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
        -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
        QBCore = {
            enabled = false,
            checkJob = {
                enabled = false, -- Enable this to use QBCore job check
                jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use QBCore permission check
                permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
    },



    -- This command stops all smoke
    stopAllSmokeCommand = {
        enabled = true,
        commandName = "stopallsmoke",
        acePermissions = {
            enabled = true,
            -- This enables ace permissions on the stop smoke command
        },
        -- We've added ESX integration. All you need to do is enable it below and configure which jobs can use the command
        ESX = {
            enabled = false,
            checkJob = {
                enabled = true, -- Enable this to use ESX job check
                jobs = {"fire", "firefighter"} -- A user can have any of the following jobs, allowing you to add multiple
            }
        },
        -- We've added vRP integration. All you need to do is enable it below. Then, configure if you wish to check for groups or permissions, or even both
        vRP = {
            enabled = false,
            checkGroup = {
                enabled = false, -- Enable this to use vRP group check
                groups = {"fire", "admin"}, -- A user can have any of the following groups, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use vRP permission check
                permissions = {"player.kick"} -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
        -- We've added QBCore integration. All you need to do is enable it below. Then, configure if you wish to check for jobs or permissions, or even both
        QBCore = {
            enabled = false,
            checkJob = {
                enabled = false, -- Enable this to use QBCore job check
                jobs = {"fire"}, -- A user can have any of the following jobs, meaning you can add different jobs
            },
            checkPermission = {
                enabled = false, -- Enable this to use QBCore permission check
                permissions = {"god"}, -- A user can have any of the following permissions, allowing you to add multiple
            },
        },
    },
}

-- This configures the weapons used to put out either fires requiring water, or those requiring an extinguisher
weapons = {
    water = {
        model = `weapon_hose`, -- If you are using HoseLS, we do not recommend changing this
        name = "Hose",
        reduceBy = 0.75, -- This is how powerful it is, lower the number the better
        increaseBy = 1.3, --  This is how powerful it is against the wrong fire type, higher the number the more powerful
    },
    extinguisher = {
        model = `weapon_fireextinguisher`,
        name = "Fire Extinguisher",
        reduceBy = 0.90,  -- This is how powerful it is, lower the number the better
        increaseBy = 1.6, --  This is how powerful it is against the wrong fire type, higher the number the more powerful
    },
    foam = { -- This will only work if you are using HoseLS and have foam mode enabled in the main config section
        name = "Foam",
        reduceBy = 0.60,   -- This is how powerful it is, lower the number the better
        increaseBy = 1.3,  --  This is how powerful it is against the wrong fire type, higher the number the more powerful
    },
    waterMonitor = { -- This will only work if you have our Water Monitor resource
        name = "Water Monitor",
        reduceBy = 0.50,   -- This is how powerful it is, lower the number the better
        increaseBy = 1.3,  --  This is how powerful it is against the wrong fire type, higher the number the more powerful
    },
    deckGun = {
        name = "Deck Gun",
        reduceBy = 0.35,
        increaseBy = 1.3,
    },
}


-- Here you can translate all elements of the resource into another language
translations = {
    foamCommandName = "foam",
    foamCommandHelpText = "Toggle foam mode on your fire hose",
    startFireCommandHelpText = "Start a fire with a defined type and size",
    startFireParameterType = "Fire Type",
    startFireParameterSize = "Fire Size",
    startFireParameterSizeHelp = "Eg, 4.0",
    startFireCommandTypeSeparator = ", ",
    startSmokeCommandTypeSeparator = ", ",
    invalidFireTypeError = "~r~Error~w~: You must select a valid fire type.",
    invalidFireTypeAndSizeError = "~r~Error~w~: You must select a valid fire type and size.",
    invalidFireSizeError = "~r~Error~w~: You must select a valid fire size.",
    fireSizeAboveMaximumError = "~r~Error~w~: You have exceeded the maximum fire size for this type.",
    fireSizeBelowMinimumError = "~r~Error~w~: The fire size is below the minimum for this type.",
    stopFireCommandHelpText = "Stop the nearest fire or within a radius",
    stopFireCommandParameterName = "Radius",
    stopFireCommandParameterHelp = "Eg, 4.0 (optional)",
    noNearbyFireFound = "~r~Error~w~: No nearby fire found to stop",
    noNearbyFiresFoundInRadius = "~r~Error~w~: No nearby fires found to stop within the specified radius",
    specifiedRadiusTooLarge = "~r~Error~w~: No nearby fires found to stop within the specified radius",
    stopped = "~g~Success~w~: Stopped",
    fire = "fire.",
    fires = "fires.",
    smoke = "smoke.",
    smokes = "smokes.",
    nearbyFireStopped = "~g~Success~w~: The closest fire has been stopped.",
    stopAllFiresCommandHelpText = "Stop all fires",
    allFiresStopped = "~g~Success~w~: All fires have been stopped.",
    noFiresFound = "~r~Error~w~: No fires found to stop",
    invalidSmokeTypeAndSizeError = "~r~Error~w~: You must select a valid smoke type and size.",
    invalidSmokeTypeError = "~r~Error~w~: You must select a valid smoke type.",
    invalidSmokeSizeError = "~r~Error~w~: You must select a valid smoke size.",
    smokeSizeAboveMaximumError = "~r~Error~w~: You have exceeded the maximum smoke size for this type.",
    smokeSizeBelowMinimumError = "~r~Error~w~: The smoke size is below the minimum for this type.",
    startSmokeCommandHelpText = "Start smoke with a defined type and size",
    startSmokeParameterType = "Smoke Type",
    startSmokeParameterSize = "Smoke Size",
    startSmokeParameterSizeHelp = "Eg, 4.0",
    specifiedRadiusTooLargeSmoke = "~r~Error~w~: No nearby smoke found to stop within the specified radius",
    noNearbySmokeFoundInRadius = "~r~Error~w~: No nearby smoke found to stop within the specified radius",
    noNearbySmokeFound = "~r~Error~w~: No nearby smoke found to stop",
    nearbySmokeStopped = "~g~Success~w~: The closest smoke has been stopped.",
    stopSmokeCommandHelpText = "Stop the nearest smoke or within a radius",
    stopSmokeCommandParameterName = "Radius",
    stopSmokeCommandParameterHelp = "Eg, 4.0 (optional)",
    allSmokeStopped = "~g~Success~w~: All smoke has been stopped.",
    noSmokeFound = "~r~Error~w~: No smoke found to stop",
    stopAllSmokeCommandHelpText = "Stop all smoke",
    numberOfFlames = "Number of flames",
    numberOfFlamesParameterHelp = "Eg, 4",
    numberOfFlamesTooLargeError = "~r~Error~w~: Number of flames exceeds the maximum allowed.",
    multiFlamesNotAllowedFireType = "~r~Error~w~: Multiple flames are not allowed for this fire type.",
    numberOfFlamesTooLargeFireType = "~r~Error~w~: Number of flames exceeds the maximum allowed for this fire type.",
    foamModeDisabled = "Foam mode is now ~r~disabled~w~.",
    foamModeEnabled = "Foam mode is now ~g~enabled~w~.",
    allFiresStoppedManually = "All Fires Stopped Manually",
    streetName = "Street Name: ",
    smokeStoppedManually = "Smoke Stopped Manually",
    type = "Type: ",
    fireExtinguished = "Fire Extinguished",
    weapon = "Weapon: ",
    fireType = ", Fire Type: ",
    initialSize = ", Initial Size: ",
    multiFlameFireStartedManually = "Fire Started Manually (Multi Flame)",
    size = ", Size: ",
    fireStartedManually = "Fire Started Manually",
    smokeStartedManually = "Smoke Started Manually",
    fireStoppedManually = "Fire Stopped Manually",
    id = ", ID: ",
    radiusSpecified = ", Radius Specified: ",
    firesStopped = ", Fires Stopped: ",
    allSmokeStoppedManually = "All Smoke Stopped Manually",
    numberOfFlames2 = "Number of flames: ",
    fireDescription = "Fire",
    fireAlert ="~r~Alert~w~: New", -- additional information is added after this notification
    toggleFireCommandHelp = "Toggle automatic fires",
    automaticFiresEnabled ="~r~Alert~w~: Automatic fires enabled",
    automaticFiresDisabled ="~r~Alert~w~: Automatic fires disabled",
    automaticFiresEnabledLog = "Toggled automatic fires on",
    automaticFiresDisabledLog = "Toggled automatic fires off",
    triggerAutomaticFireHelp = "Trigger an automatic fire immediately",
    triggeredAnAutomaticFire = "Triggered an automatic fire",
    noPermission = "~r~Error~w~: You do not have permission to access this command.",
    postal = "Postal",
    automaticFireCreated = "Automatic Fire Created",
    idAutomatic = "ID: ",
    typeAutomatic = ", Type: ",
    waterMonitorFireExtinguished = "Water Monitor - Fire Extinguished",
    descriptionAutomatic = ", Description: ",
    areaOfPatrolUpdated = "~b~Success~w~: Area of patrol updated to ",
    invalidAreaOfPatrol = "~r~Error~w~: The area of patrol selected is invalid.",
    updatedAreaOfPatrolTo = "Updated area of patrol to ",
    nowClockedOff = "~b~Success~w~: You are now clocked off.",
    nowClockedOn = "~b~Success~w~: You are now clocked on.",
    alreadyClockedOff = "~r~Error~w~: You are already clocked off.",
    alreadyClockedOn = "~r~Error~w~: You are already clocked on.",
    allAreasOfPatrolOn = "~b~Success~w~: All areas of patrol are now set to enable fire spawning.",
    allAreasOfPatrolOff = "~b~Success~w~: All areas of patrol are now disabled.",
    allAreasOfPatrolOnLog = "All areas of patrol are now set to enable fire spawning.",
    allAreasOfPatrolOffLog = "All areas of patrol are now disabled.",
    clockedOffLog = "User clocked off",
    clockedOnLog = "User clocked on",
    clockOnSuggestion = "Clock on for fires",
    clockOffSuggestion = "Clock off for fires",
    spreadable = "Spreadable",
    spreadableHelp = "true/false",
    commandOnCooldown = "~r~Error~w~: This command is currently on cooldown",
    dumpsterTooFar = "~r~Error~w~: You are not near a dumpster.",
    setAreaOfPatrolHelp = "Set the area of patrol for fire spawning",
    areaOfPatrol = "Area of Patrol",
    areaOfPatrolHelp = "Eg, city",
    toggleAllAreasOfPatrolHelp = "Toggle all areas of patrol for fire spawning",

}

smokeTypes = {
    ["normal"] = {
        dict = "scr_agencyheistb",
        name = "scr_env_agency3b_smoke",
        maximumSizeManual = 20.0,
        minimumSizeManual = 1.0,
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        },
    },
    ["electrical"] = {
        dict = "core",
        name = "ent_amb_elec_crackle",
        maximumSizeManual = 20.0,
        minimumSizeManual = 1.0,
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        },
    },
    ["normal2"] = {
        dict = "core",
        name = "ent_amb_smoke_foundry",
        maximumSizeManual = 20.0,
        minimumSizeManual = 1.0,
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        },
    },
    ["foggy"] = {
        dict = "core",
        name = "ent_amb_fbi_smoke_fogball",
        maximumSizeManual = 20.0,
        minimumSizeManual = 1.0,
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        },
    },
    ["normal3"] = {
        dict = "core",
        name = "ent_amb_stoner_vent_smoke",
        maximumSizeManual = 20.0,
        minimumSizeManual = 1.0,
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        },
    },
    ["normal4"] = {
        dict = "core",
        name = "ent_amb_smoke_general",
        maximumSizeManual = 20.0,
        minimumSizeManual = 1.0,
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        },
    },
    ["normal5"] = {
        dict = "core",
        name = "proj_grenade_smoke",
        maximumSizeManual = 20.0,
        minimumSizeManual = 1.0,
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        },
    },
    ["normal6"] = {
        dict = "core",
        name = "ent_amb_generator_smoke",
        maximumSizeManual = 20.0,
        minimumSizeManual = 1.0,
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        },
    },
    ["white"] = {
        dict = "core",
        name = "ent_amb_smoke_factory_white",
        maximumSizeManual = 20.0,
        minimumSizeManual = 1.0,
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        },
    },
}

fireTypes = {
    ["normal"] = {
        dict = "core",
        name = "fire_wrecked_truck_vent",
        smoke = {
            enabled = true,
            type = "normal2",
            sizeMultiplier = 0.1, -- This is the size of smoke compared to the size of the fire
            keepAfterFire = true,
            keepAfterFireDuration = 30, -- This keeps smoke in the area for x seconds after the fire
            keepAfterFireSize = 0.1, -- This is the size of smoke after the fire compared to the initial size
        },
        toPutOut = { weapons.water, weapons.extinguisher, weapons.foam, weapons.waterMonitor, weapons.deckGun },
        toIncrease = {},
        multiFlamesAllowed = true, -- This defines if multiple flames are allowed for this fire type
        maximumMultipleFlames = 16, -- This defines the maximum flames allowed for this fire type
        difficulty = 10, -- This is how difficult the fire is to put out (out of 50)
        maximumFireSizeManual = 10.0, -- This is the maximum fire size that can be created using the create fire command
        minimumFireSizeAutomatic = 1.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeAutomatic = 4.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeWhenExtinguishing = 10.0, -- This is the maximum fire size that can be created automatically (such as using the wrong weapon to increase the size, such as water on an electrical fire)
        minimumFireSizeManual = 0.5, -- This is the minimum fire size that can be created using the create fire command
        damageDistance = 1.5, -- The distance a player must be nearby to be damaged by the fire
        -- We do not recommend editing the offSet section. This is used to adjust the offset of fires when spawning, for example they may be spawning too low below the player.
        offSet = {
            x = 0.0,
            y = 0.0,
            z = -0.4,
        }
    },
    ["normal2"] = {
        dict = "scr_trevor3",
        name = "scr_trev3_trailer_plume",
        smoke = {
            enabled = false,
            type = "normal",
            sizeMultiplier = 1.4, -- This is the size of smoke compared to the size of the fire
            keepAfterFire = false,
            keepAfterFireDuration = 200, -- This keeps smoke in the area for x seconds after the fire
            keepAfterFireSize = 1.0, -- This is the size of smoke after the fire compared to the initial size, e.g. 1.0 = same size as initial fire
        },
        toPutOut = { weapons.water, weapons.extinguisher, weapons.foam, weapons.waterMonitor, weapons.deckGun },
        toIncrease = {},
        multiFlamesAllowed = true, -- This defines if multiple flames are allowed for this fire type
        maximumMultipleFlames = 16, -- This defines the maximum flames allowed for this fire type
        difficulty = 25, -- This is how difficult the fire is to put out (out of 50)
        minimumFireSizeAutomatic = 1.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeAutomatic = 4.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeManual = 7.0, -- This is the maximum fire size that can be created using the create fire command
        maximumFireSizeWhenExtinguishing = 6.0, -- This is the maximum fire size that can be created automatically (such as using the wrong weapon to increase the size, such as water on an electrical fire)
        minimumFireSizeManual = 0.1, -- This is the minimum fire size that can be created using the create fire command
        -- We do not recommend editing the offSet section. This is used to adjust the offset of fires when spawning, for example they may be spawning too low below the player.
        damageDistance = 1.5, -- The distance a player must be nearby to be damaged by the fire
        -- We do not recommend editing the offSet section. This is used to adjust the offset of fires when spawning, for example they may be spawning too low below the player.
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        }
    },
    ["normal3"] = {
        dict = "core",
        name = "ent_ray_meth_fires",
        smoke = {
            enabled = true,
            type = "normal6",
            sizeMultiplier = 1.4, -- This is the size of smoke compared to the size of the fire
            keepAfterFire = true,
            keepAfterFireDuration = 30, -- This keeps smoke in the area for x seconds after the fire
            keepAfterFireSize = 1.6, -- This is the size of smoke after the fire compared to the initial size, e.g. 1.0 = same size as initial fire
        },
        toPutOut = { weapons.water, weapons.extinguisher, weapons.foam, weapons.waterMonitor, weapons.deckGun },
        toIncrease = {},
        multiFlamesAllowed = true, -- This defines if multiple flames are allowed for this fire type
        maximumMultipleFlames = 16, -- This defines the maximum flames allowed for this fire type
        difficulty = 30, -- This is how difficult the fire is to put out (out of 50)
        maximumFireSizeManual = 7.0, -- This is the maximum fire size that can be created using the create fire command
        minimumFireSizeAutomatic = 1.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeAutomatic = 4.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeWhenExtinguishing = 10.0, -- This is the maximum fire size that can be created automatically (such as using the wrong weapon to increase the size, such as water on an electrical fire)
        minimumFireSizeManual = 0.5, -- This is the minimum fire size that can be created using the create fire command
        damageDistance = 1.5, -- The distance a player must be nearby to be damaged by the fire
        -- We do not recommend editing the offSet section. This is used to adjust the offset of fires when spawning, for example they may be spawning too low below the player.
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        }
    },
    ["bonfire"] = {
        dict = "scr_michael2",
        name = "scr_mich3_heli_fire",
        smoke = {
            enabled = true,
            type = "normal",
            sizeMultiplier = 1.4, -- This is the size of smoke compared to the size of the fire
            keepAfterFire = true,
            keepAfterFireDuration = 200, -- This keeps smoke in the area for x seconds after the fire
            keepAfterFireSize = 1.0, -- This is the size of smoke after the fire compared to the initial size, e.g. 1.0 = same size as initial fire
        },
        toPutOut = { weapons.water, weapons.extinguisher, weapons.foam, weapons.waterMonitor, weapons.deckGun },
        toIncrease = {},
        multiFlamesAllowed = true, -- This defines if multiple flames are allowed for this fire type
        maximumMultipleFlames = 16, -- This defines the maximum flames allowed for this fire type
        difficulty = 40, -- This is how difficult the fire is to put out (out of 50)
        maximumFireSizeManual = 10.0, -- This is the maximum fire size that can be created using the create fire command
        minimumFireSizeAutomatic = 1.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeAutomatic = 3.0, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeWhenExtinguishing = 10.0, -- This is the maximum fire size that can be created automatically (such as using the wrong weapon to increase the size, such as water on an electrical fire)
        minimumFireSizeManual = 0.5, -- This is the minimum fire size that can be created using the create fire command
        damageDistance = 1.5, -- The distance a player must be nearby to be damaged by the fire
        -- We do not recommend editing the offSet section. This is used to adjust the offset of fires when spawning, for example they may be spawning too low below the player.
        offSet = {
            x = 0.0,
            y = 0.0,
            z = 0.0,
        }
    },
    ["chemical"] = {
        dict = "core",
        name = "fire_petroltank_truck",
        smoke = {
            enabled = false,
            type = "normal",
            sizeMultiplier = 1.4, -- This is the size of smoke compared to the size of the fire
            keepAfterFire = true,
            keepAfterFireDuration = 200, -- This keeps smoke in the area for x seconds after the fire
            keepAfterFireSize = 1.0, -- This is the size of smoke after the fire compared to the initial size, e.g. 1.0 = same size as initial fire
        },
        toPutOut = { weapons.extinguisher, weapons.foam },
        toIncrease = { weapons.water, weapons.waterMonitor, weapons.deckGun },
        multiFlamesAllowed = true, -- This defines if multiple flames are allowed for this fire type
        maximumMultipleFlames = 16, -- This defines the maximum flames allowed for this fire type
        difficulty = 40, -- This is how difficult the fire is to put out (out of 50)
        maximumFireSizeManual = 10.0, -- This is the maximum fire size that can be created using the create fire command
        minimumFireSizeAutomatic = 1.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeAutomatic = 6.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeWhenExtinguishing = 10.0, -- This is the maximum fire size that can be created automatically (such as using the wrong weapon to increase the size, such as water on an electrical fire)
        minimumFireSizeManual = 0.5, -- This is the minimum fire size that can be created using the create fire command
        damageDistance = 1.5, -- The distance a player must be nearby to be damaged by the fire
        -- We do not recommend editing the offSet section. This is used to adjust the offset of fires when spawning, for example they may be spawning too low below the player.
        offSet = {
            x = 0.0,
            y = 0.0,
            z = -1.0,
        }
        
    },
    ["electrical"] = {
        dict = "core",
        name = "fire_petroltank_truck",
        smoke = {
            enabled = true,
            type = "electrical",
            sizeMultiplier = 1.4, -- This is the size of smoke compared to the size of the fire
            keepAfterFire = true,
            keepAfterFireDuration = 200, -- This keeps smoke in the area for x seconds after the fire
            keepAfterFireSize = 1.0, -- This is the size of smoke after the fire compared to the initial size, e.g. 1.0 = same size as initial fire
        },
        toPutOut = { weapons.extinguisher, weapons.foam },
        toIncrease = { weapons.water, weapons.waterMonitor, weapons.deckGun },
        multiFlamesAllowed = true, -- This defines if multiple flames are allowed for this fire type
        maximumMultipleFlames = 16, -- This defines the maximum flames allowed for this fire type
        difficulty = 40, -- This is how difficult the fire is to put out (out of 50)
        maximumFireSizeManual = 10.0, -- This is the maximum fire size that can be created using the create fire command
        minimumFireSizeAutomatic = 1.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeAutomatic = 6.5, -- This is the minimum fire size that is started automatically (if automatic fires are enabled)
        maximumFireSizeWhenExtinguishing = 10.0, -- This is the maximum fire size that can be created automatically (such as using the wrong weapon to increase the size, such as water on an electrical fire)
        minimumFireSizeManual = 0.5, -- This is the minimum fire size that can be created using the create fire command
        damageDistance = 1.5, -- The distance a player must be nearby to be damaged by the fire
        -- We do not recommend editing the offSet section. This is used to adjust the offset of fires when spawning, for example they may be spawning too low below the player.
        offSet = {
            x = 0.0,
            y = 0.0,
            z = -1.0,
        }
    },
}

main.fireProps = {
    [`v_serv_waste_bin1`] = true,
    [`prop_dumpster_02b`] = true,
    [`prop_snow_dumpster_01`] = true,
    [`prop_bin_11b`] = true,
    [`prop_bin_11a`] = true,
    [`prop_bin_10a`] = true,
    [`v_med_bin`] = true,
    [`prop_bin_04a`] = true,
    [`v_ret_gc_bin`] = true,
    [`prop_bin_07c`] = true,
    [`vw_prop_vw_casino_bin_01a`] = true,
    [`prop_cs_bin_02`] = true,
    [`ch_prop_casino_bin_01a`] = true,
    [`prop_bin_12a`] = true,
    [`sc1_07_clinical_bin`] = true,
    [`prop_bin_06a`] = true,
    [`prop_bin_01a`] = true,
    [`hei_heist_kit_bin_01`] = true,
    [`prop_cs_dumpster_01a`] = true,
    [`mp_b_kit_bin_01`] = true,
    [`prop_bin_08open`] = true,
    [`prop_dumpster_3a`] = true,
    [`prop_snow_bin_01`] = true,
    [`prop_bin_13a`] = true,
    [`prop_bin_05a`] = true,
    [`prop_bin_03a`] = true,
    [`prop_cs_bin_03`] = true,
    [`prop_dumpster_4a`] = true,
    [`prop_bin_09a`] = true,
    [`prop_bin_14b`] = true,
    [`prop_dumpster_01a`] = true,
    [`prop_dumpster_02a`] = true,
    [`prop_bin_10b`] = true,
    [`prop_bin_08a`] = true,
    [`prop_bin_delpiero_b`] = true,
    [`prop_bin_beach_01d`] = true,
    [`v_ret_csr_bin`] = true,
    [`prop_bin_07a`] = true,
    [`prop_bin_07b`] = true,
    [`prop_bin_beach_01a`] = true,
    [`prop_bin_02a`] = true,
    [`prop_gas_binunit01`] = true,
    [`prop_dumpster_4b`] = true,
    [`prop_bin_delpiero`] = true,
    [`prop_cs_bin_01`] = true,
    [`prop_snow_bin_02`] = true,
    [`prop_bin_14a`] = true,
}