DOCTOR = {
    ["esa"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"hems", "lasdoctor", "t2dev"},
        clears_injuries = {"blocked_airways"}
    },

    ["ammuputation"] = {
        effectiveness = 1,
        cooldown = 60,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"lasdoctor", "t2dev"},
        clears_injuries = {"loss_of_circulation"},
        ["loss_of_circulation"] = {
            effectiveness = 35,
            reopeningChance = 0.1,
            reopeningMinDelay = 20,
            reopeningMaxDelay = 50
            
        },
        ["compartmentsyndrome"] = {
            effectiveness = 0,
            reopeningChance = 0.1,
            reopeningMinDelay = 20,
            reopeningMaxDelay = 50
            
        },
        ["puncturedlung"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["abrasion"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["avulsion"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["contusion"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["crush"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["cut"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["laceration"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["burn_injury"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["puncture_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["drowned"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["tbi"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["impalement"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["blocked_airways"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["partial_blockage"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["broken_leg"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["broken_arm"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["open_fracture"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["closed_fracture"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["broken_neck"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["broken_finger"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["broken_toe"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["broken_pelvis"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["broken_ribs"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        }
            
    },
    ["surgical_scalpol"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"lasdoctor", "t2dev"},
        clears_injuries = {"compartmentsyndrome"}
    },
    ["reset_fracture"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"hems", "lasdoctor", "t2dev"},
        clears_injuries = {"open_fracture"}
    },
    ["closed_reduction"] = {
        effectiveness = 1,
        cooldown = 10,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"lasdoctor", "t2dev"},
        clears_injuries = {"dislocation"}
    },
    ["thoracostomy"] = {
        effectiveness = 1,
        cooldown = 25,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"lasdoctor", "t2dev"},
        clears_injuries = {"hemothorax", "tension_pneumothorax"}
    },
    ["thoracotomy"] = {
        effectiveness = 1,
        cooldown = 25,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"lasdoctor", "t2dev"},
        clears_injuries = {"blunt_chest_trauma", "cardiac_tamponade"}
    },
    ["needle_decompression"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"hems", "lasdoctor", "t2dev" },
        clears_injuries = {"puncturedlung"}
    },
    ["surgical_kit"] = {
        cooldown = 40,
        permissions_needed = {"hems", "lasdoctor", "t2dev"}
    },
    ["peppapig"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"t2dev"}
    },
    ["lollypop"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"t2dev"}
    },
    ["sticker"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"t2dev"}
    }
}