--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

BANDAGES = {
    ["compression_bandage"] = {
        effectiveness = 1,
        cooldown = 5,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = { "firefighter", "stjohns", "afo", "pc", "studentparamedic", "paramedic", "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev" },
            ["puncturedlung"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["abrasion"] = {
                effectiveness = 3,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["avulsion"] = {
                effectiveness = 2,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["contusion"] = {
                effectiveness = 3,
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
                effectiveness = 4,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["laceration"] = {
                effectiveness = 4,
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
            ["compartmentsyndrome"] = {
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
                effectiveness = 4,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["loss_of_circulation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["heart_attack"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["seizure"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["internal_bleeding"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["chest_infection"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["mid_femur_fracture"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["dislocation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["choking"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["degloved_wound"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["headache"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["diabetic_ketoacidosis"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },  
            ["sucking_chest_wound"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["abdominal_aneurysm"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["hemothorax"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["tension_pneumothorax"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["blunt_chest_trauma"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["cardiac_tamponade"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["smoke_inhalation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["heat_stroke"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["asthma_attack"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["iwf"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
    },
    ["gauze"] = {
            effectiveness = 1,
            cooldown = 5,
            reopeningChance = 0.1,
            reopeningMinDelay = 20,
            reopeningMaxDelay = 50,
            permissions_needed = { "firefighter", "stjohns", "pc", "afo", "studentparamedic", "paramedic", "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev" },
                ["puncturedlung"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["abrasion"] = {
                    effectiveness = 4,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["avulsion"] = {
                    effectiveness = 3,
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
                    effectiveness = 3,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["cut"] = {
                    effectiveness = 4,
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
                    effectiveness = 3,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["compartmentsyndrome"] = {
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
                },  
                ["loss_of_circulation"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["heart_attack"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["seizure"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["internal_bleeding"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["chest_infection"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["mid_femur_fracture"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["dislocation"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["choking"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["degloved_wound"] = {
                    effectiveness = 1,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["headache"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["diabetic_ketoacidosis"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },  
                ["sucking_chest_wound"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["abdominal_aneurysm"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["hemothorax"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["tension_pneumothorax"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["blunt_chest_trauma"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["cardiac_tamponade"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["smoke_inhalation"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["heat_stroke"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["asthma_attack"] = {
                    effectiveness = 0,
                    reopeningChance = 2,
                    reopeningMinDelay = 60,
                    reopeningMaxDelay = 120
                },
                ["iwf"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
                },
                 
    },
    ["salinesoakedgauze"] = {
        effectiveness = 1,
        cooldown = 5,
        reopeningChance = 0.1,
        reopeningMinDelay = 20,
        reopeningMaxDelay = 50,
        permissions_needed = {"studentparamedic", "paramedic", "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev" },
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
                effectiveness = 4,
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
            ["compartmentsyndrome"] = {
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
            },  
            ["loss_of_circulation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["heart_attack"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["seizure"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["internal_bleeding"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["chest_infection"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["mid_femur_fracture"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["dislocation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["choking"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["degloved_wound"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["headache"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["diabetic_ketoacidosis"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },  
            ["sucking_chest_wound"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["abdominal_aneurysm"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["hemothorax"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["tension_pneumothorax"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["blunt_chest_trauma"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["cardiac_tamponade"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["smoke_inhalation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["heat_stroke"] = {
                effectiveness = 1,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["asthma_attack"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["iwf"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },             
},
["pelvic_splint"] = {
    effectiveness = 1,
    cooldown = 7,
    reopeningChance = 0.1,
    reopeningMinDelay = 120,
    reopeningMaxDelay = 200,
    permissions_needed = { "advancedparamedic", "hems", "lasdoctor", "t2dev"},
    clears_injuries = {"broken_pelvis"},


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
        effectiveness = 3,
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
    },
    ["loss_of_circulation"] = {
        effectiveness = 0,
        reopeningChance = 2,
        reopeningMinDelay = 60,
        reopeningMaxDelay = 120
    }
},
    ["portable_suction"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"studentparamedic", "paramedic", "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev"},
        clears_injuries = {"drowned","partial_blockage","choking"},
    },
    ["splint"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"paramedic", "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev"},
        clears_injuries = {"broken_toe","broken_leg","broken_arm","broken_finger","closed_fracture"}
    },
    ["kendric_splint"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"advancedparamedic", "hems", "lasdoctor", "t2dev"},
        clears_injuries = {"mid_femur_fracture"}
    },
    ["russel_chest_seal"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"hart", "hems", "lasdoctor", "t2dev"},
        clears_injuries = {"sucking_chest_wound"}
    },
    ["acetone"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"hart", "t2dev"},
        clears_injuries = {"super_glued"}
    },
    ["apply_ccollar"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = { "fru", "studentparamedic", "paramedic", "advancedparamedic", "hart", "hems", "lasdoctor", "t2dev"},
        ["broken_neck"] = {
            effectiveness = 3,
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
        },  
        ["loss_of_circulation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heart_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["seizure"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["internal_bleeding"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["chest_infection"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["mid_femur_fracture"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["dislocation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["choking"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["degloved_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["headache"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["diabetic_ketoacidosis"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["sucking_chest_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["abdominal_aneurysm"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["hemothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["tension_pneumothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["blunt_chest_trauma"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["cardiac_tamponade"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["smoke_inhalation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heat_stroke"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["asthma_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["iwf"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },   
    },

    
    ["sling"] = {
        effectiveness = 1,
        cooldown = 15,
        reopeningChance = 0.1,
        reopeningMinDelay = 20,
        reopeningMaxDelay = 50,
        permissions_needed = {"studentparamedic", "paramedic", "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev"},
            ["compartmentsyndrome"] = {
                effectiveness = 0,
                reopeningChance = 0.0,
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
                effectiveness = 1,
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
            },  
            ["loss_of_circulation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["heart_attack"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["seizure"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["internal_bleeding"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["chest_infection"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["mid_femur_fracture"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["dislocation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["choking"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["degloved_wound"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["headache"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["diabetic_ketoacidosis"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },  
            ["sucking_chest_wound"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["abdominal_aneurysm"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["hemothorax"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["tension_pneumothorax"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["blunt_chest_trauma"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["cardiac_tamponade"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["smoke_inhalation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["heat_stroke"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["asthma_attack"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },  
    },
     ["elastic_bandage"] = {
        effectiveness = 1,
        cooldown = 4,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"hems","lasdoctor","t2dev"},
        ["velocity_wound"] = {
            effectiveness = 1,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
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
            effectiveness = 3,
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
            effectiveness = 3,
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
        },  
        ["loss_of_circulation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["cardio_disturbance"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heart_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["seizure"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["internal_bleeding"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["chest_infection"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["mid_femur_fracture"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["dislocation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["choking"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["degloved_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["headache"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["diabetic_ketoacidosis"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["sucking_chest_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["abdominal_aneurysm"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["hemothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["tension_pneumothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["blunt_chest_trauma"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["cardiac_tamponade"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["smoke_inhalation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heat_stroke"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["asthma_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        }, 
    },
    ["quickclot"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"afo", "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev" },
        ["velocity_wound"] = {
            effectiveness = 2,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
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
            effectiveness = 3,
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
            effectiveness = 3,
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
        },  
        ["loss_of_circulation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heart_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["seizure"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["internal_bleeding"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["chest_infection"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["mid_femur_fracture"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["dislocation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["choking"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["degloved_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["headache"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["diabetic_ketoacidosis"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["sucking_chest_wound"] = {
            effectiveness = 1,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["abdominal_aneurysm"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["hemothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["tension_pneumothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["blunt_chest_trauma"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["cardiac_tamponade"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["smoke_inhalation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heat_stroke"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["asthma_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
    },
    ["foil_blanket"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"afo", "studentparamedic", "paramedic", "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev" },
        clears_injuries = {"hypothermia"},
        ["velocity_wound"] = {
            effectiveness = 2,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
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
            effectiveness = 3,
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
            effectiveness = 3,
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
        },  
        ["loss_of_circulation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heart_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["seizure"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["internal_bleeding"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["chest_infection"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["mid_femur_fracture"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["dislocation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["choking"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["degloved_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["headache"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["diabetic_ketoacidosis"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["sucking_chest_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["abdominal_aneurysm"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["hemothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["tension_pneumothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["blunt_chest_trauma"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["cardiac_tamponade"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["smoke_inhalation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heat_stroke"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["asthma_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
    },
    ["prongs"] = {
        effectiveness = 1,
        cooldown = 7,
        reopeningChance = 0.1,
        reopeningMinDelay = 120,
        reopeningMaxDelay = 200,
        permissions_needed = {"afo", "studentparamedic", "paramedic", "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev" },
        clears_injuries = {"barbs"},
        ["velocity_wound"] = {
            effectiveness = 2,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
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
            effectiveness = 3,
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
            effectiveness = 3,
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
        },  
        ["loss_of_circulation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heart_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["seizure"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["internal_bleeding"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["chest_infection"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["mid_femur_fracture"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["dislocation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["choking"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["degloved_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["headache"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["diabetic_ketoacidosis"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
        ["sucking_chest_wound"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["abdominal_aneurysm"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["hemothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["tension_pneumothorax"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["blunt_chest_trauma"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["cardiac_tamponade"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["smoke_inhalation"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["heat_stroke"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },
        ["asthma_attack"] = {
            effectiveness = 0,
            reopeningChance = 2,
            reopeningMinDelay = 60,
            reopeningMaxDelay = 120
        },  
    },
    ["dermabond"] = {
        effectiveness = 1,
        cooldown = 5,
        reopeningChance = 2,
        reopeningMinDelay = 60,
        reopeningMaxDelay = 120,
        permissions_needed = { "advancedparamedic", "hems", "hart", "lasdoctor", "t2dev" },
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
                effectiveness = 1,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["laceration"] = {
                effectiveness = 1,
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
            ["compartmentsyndrome"] = {
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
            },
            ["loss_of_circulation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["heart_attack"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["seizure"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["internal_bleeding"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["chest_infection"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["mid_femur_fracture"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["dislocation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["choking"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["degloved_wound"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["headache"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["diabetic_ketoacidosis"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },  
            ["sucking_chest_wound"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["abdominal_aneurysm"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["hemothorax"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["tension_pneumothorax"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["blunt_chest_trauma"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["cardiac_tamponade"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["smoke_inhalation"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["heat_stroke"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
            ["asthma_attack"] = {
                effectiveness = 0,
                reopeningChance = 2,
                reopeningMinDelay = 60,
                reopeningMaxDelay = 120
            },
        }
    }