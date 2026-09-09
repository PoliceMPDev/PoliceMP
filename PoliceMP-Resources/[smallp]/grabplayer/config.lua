Config = {
    Language = 'en', 

    CommandEnabled = true, 
    GrabCommand = '+xgrab', 
    PutCarCommand = '+xputcar',
    ExitCarCommand = '+xexitcar', 

    ESX = { 
        enabled = false, 
        jobs = { 
            'police',
            'fbi',
        },
        exceptionJobs = false 
    },
        
    QB = { 
        enabled = false, 
        jobs = { 
            'police',
            'fbi',
        }
    },

    DropPlayer = true, 

    DisableSprint = true, 

    CollisionRange = 0.5, 

    AttachPosition = vector3(0.20, 0.45, 0.0), 

    Animations = {
        policeAnimation = {
            enabled = true, 
            animDict = 'amb@world_human_drinking@coffee@male@base',
            anim = 'base'
        },
        citizenAnimation = {
            enabled = false, 
            animDict = 'amb@world_human_drinking@coffee@male@base',
            anim = 'base'
        }
    }
}

Config.Keys = {
    GrabAndDropKey = 73, -- If CommandEnabled is equal to false you must put a key to take the person.
    GrabAndDropKeyString = '~INPUT_VEH_DUCK~', -- Name of the button.

    TaskEnterKey = 206, -- Button to put the person inside the vehicle
    TaskEnterKeyString = '~INPUT_FRONTEND_RB~' -- Name of the button to put the person inside the vehicle
}

Config.Languages = {
    ['en'] = {
        ['taskenter'] = 'Put person in the vehicle '..Config.Keys.TaskEnterKeyString,
        ['exitped'] = 'Take person out the vehicle '..Config.Keys.TaskEnterKeyString,
        ['releaseperson'] = 'Release grab press '..Config.Keys.GrabAndDropKeyString
    }
}