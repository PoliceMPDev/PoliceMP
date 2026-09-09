--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

MEDICATIONS  = {
    -- Available options: --
       
    -- painReduce: number: How much does the pain get reduced?
    -- hrIncreaseLow: table: How much will the heart rate be increased when the HR is low (below 55)? {minIncrease, maxIncrease}
    -- hrIncreaseNormal: 55 <= _heartRate <= 110
    -- hrIncreaseHigh: 110 > _heartRate

    -- timeInSystem: number: How long until this medication has disappeared
    -- timeTillMaxEffect: number: How long until the maximum effect is reached
    -- maxDose: number: How many of this type of medication can be in the system before the patient overdoses?
    -- onOverDose: number: Function to execute upon overdose. -1 for no overdose.
    -- causesAnesthesia: bool: If this medication causes anesthesia
    -- viscosityChange: number: The viscosity of a fluid is a measure of its resistance to gradual deformation by shear stress or tensile stress. For liquids, it corresponds to the informal concept of "thickness". This value will increase/decrease the viscoty of the blood with the percentage given. Where 100 = max. Using the minus will decrease viscosity.
    -- Joe Edgar & Woody

    ["paracetamol"] = {
        painReduce        = 0.2,
        hrIncreaseLow     = { 0, 0 },
        hrIncreaseNormal  = { 0, 0 },
        hrIncreaseHigh    = { 0, 0 },
        timeInSystem      = 600,
        timeTillMaxEffect = 30,
        maxDose           = 8,
        viscosityChange   = 0,
        clears_injuries = {"headache"},
        permissions_needed = {"studentparamedic", "paramedic", "hart", "t2dev"}
    },
    ["aspirin"] = {
        painReduce        = 0.5,
        hrIncreaseLow     = { -5, 5 },
        hrIncreaseNormal  = { -5, 5 },
        hrIncreaseHigh    = { -5, 5 },
        timeInSystem      = 600,
        timeTillMaxEffect = 30,
        maxDose           = 8,
        viscosityChange   = -5,
        clears_injuries = {"heart_attack"},
        permissions_needed = {"studentparamedic", "paramedic", "hart", "t2dev"}
    },
    ["adrenaline"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 10, 20 },
        hrIncreaseNormal  = { 10, 30 },
        hrIncreaseHigh    = { 5, 20 },
        timeInSystem      = 180,
        timeTillMaxEffect = 10,
        maxDose           = 5,
        viscosityChange   = 5,
        permissions_needed = {"paramedic", "hart", "t2dev"}

    },
    ["epinephrine"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 5, 10 },
        hrIncreaseNormal  = { 5, 10 },
        hrIncreaseHigh    = { 5, 10 },
        timeInSystem      = 180,
        timeTillMaxEffect = 10,
        maxDose           = 5,
        viscosityChange   = 5,
        clears_injuries = {"anaphylaxis"},
        permissions_needed = {"paramedic", "hart", "t2dev"}

    },
    ["midazolam"] = {
        painReduce        = 0.2,
        hrIncreaseLow     = { -2, 0 },
        hrIncreaseNormal  = { -5, -2 },
        hrIncreaseHigh    = { -5, -3 },
        timeInSystem      = 600,
        timeTillMaxEffect = 15,
        maxDose           = 5,
        viscosityChange   = 0,
        permissions_needed = {"paramedic","hart", "t2dev"}
    },
    ["mannitol"] = {
        painReduce        = 0.2,
        hrIncreaseLow     = { -2, 0 },
        hrIncreaseNormal  = { -5, -2 },
        hrIncreaseHigh    = { -5, -3 },
        timeInSystem      = 600,
        timeTillMaxEffect = 15,
        maxDose           = 5,
        viscosityChange   = 0,
        clears_injuries = {"tbi"},
        permissions_needed = {"hems", "t2dev"}
    },

    ["morphine"] = {
        painReduce        = 0.5,
        hrIncreaseLow     = { -3, 0 },
        hrIncreaseNormal  = { -6, -3 },
        hrIncreaseHigh    = { -6, -4 },
        timeInSystem      = 600,
        timeTillMaxEffect = 15,
        maxDose           = 5,
        viscosityChange   = 0,
        permissions_needed = {"paramedic", "hart", "t2dev"}
    },
    ["fentanyl"] = {
        painReduce        = 0.5,
        hrIncreaseLow     = { -3, 0 },
        hrIncreaseNormal  = { -6, -3 },
        hrIncreaseHigh    = { -6, -4 },
        timeInSystem      = 600,
        timeTillMaxEffect = 15,
        maxDose           = 5,
        viscosityChange   = 0,
        permissions_needed = {"paramedic", "hart", "t2dev"}
    },
    ["fentanyl_lozenges"] = {
        painReduce        = 0.1,
        hrIncreaseLow     = { -2, 0 },
        hrIncreaseNormal  = { -3, -2 },
        hrIncreaseHigh    = { -3, -2 },
        timeInSystem      = 1200,
        timeTillMaxEffect = 15,
        maxDose           = 5,
        viscosityChange   = 0,
        permissions_needed = {"afo", "t2dev"}
    },
    ["naloxone"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 0, 10 },
        hrIncreaseNormal  = { 0, 10 },
        hrIncreaseHigh    = { 0, 10 },
        timeInSystem      = 90,
        timeTillMaxEffect = 5,
        maxDose           = 5,
        viscosityChange   = 0,
        clears_injuries = {"overdose"},
        permissions_needed = {"studentparamedic","pc","paramedic","hart", "t2dev"}
    },
    ["salbutamol"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 10, 15 },
        hrIncreaseNormal  = { 10, 15 },
        hrIncreaseHigh    = { 5, 10 },
        timeInSystem      = 240,
        timeTillMaxEffect = 15,
        maxDose           = 10,
        viscosityChange   = 0,
        clears_injuries = {"overdose", "asthma_attack", "smoke_inhalation"},
        permissions_needed = {"studentparamedic", "paramedic","hart", "t2dev"}
    },
    ["ramipril"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { -5, -3 },
        hrIncreaseNormal  = { -10, -5 },
        hrIncreaseHigh    = { -10, -7 },
        timeInSystem      = 86400,
        timeTillMaxEffect = 180,
        maxDose           = 1,
        viscosityChange   = 0,
        permissions_needed = {"paramedic","hart", "t2dev"}
    },
    ["prochlorperazine"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { -2, 0 },
        hrIncreaseNormal  = { -2, 0 },
        hrIncreaseHigh    = { -3, 0 },
        timeInSystem      = 1800,
        timeTillMaxEffect = 30,
        maxDose           = 2,
        viscosityChange   = 0,
        permissions_needed = {"paramedic", "hart", "t2dev"}
    },
    ["ibuprofen"] = {
        painReduce        = 0.6,
        hrIncreaseLow     = { 0, 0 },
        hrIncreaseNormal  = { 0, 0 },
        hrIncreaseHigh    = { 0, 0 },
        timeInSystem      = 2400,
        timeTillMaxEffect = 30,
        maxDose           = 6,
        viscosityChange   = 0,
        clears_injuries = {"headache"},
        permissions_needed = {"paramedic","hart", "t2dev"}
    },
    ["glucose"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 5, 10 },
        hrIncreaseNormal  = { 5, 10 },
        hrIncreaseHigh    = { 0, 5 },
        timeInSystem      = 900,
        timeTillMaxEffect = 10,
        maxDose           = 4,
        viscosityChange   = 5,
        clears_injuries = {"hypoglycemia"},
        permissions_needed = {"studentparamedic", "paramedic", "hart", "t2dev"}
    },
    ["insulin"] = {
        painReduce        = 0.2,
        hrIncreaseLow     = { 5, 10 },
        hrIncreaseNormal  = { 5, 10 },
        hrIncreaseHigh    = { 0, 5 },
        timeInSystem      = 900,
        timeTillMaxEffect = 10,
        maxDose           = 4,
        viscosityChange   = 5,
        clears_injuries = {"diabetic_ketoacidosis"},
        permissions_needed = {"lasdoctor", "t2dev"}
    },
    ["diazepam"] = {
        painReduce        = 0.5,
        hrIncreaseLow     = { -5, -2 },
        hrIncreaseNormal  = { -5, -2 },
        hrIncreaseHigh    = { -5, -2 },
        timeInSystem      = 3600,
        timeTillMaxEffect = 30,
        maxDose           = 4,
        viscosityChange   = 0,
        clears_injuries = {"seizure"},
        permissions_needed = {"paramedic", "hart", "t2dev"}
    },
    ["ondansetron"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 0, 0 },
        hrIncreaseNormal  = { 0, 0 },
        hrIncreaseHigh    = { 0, 0 },
        timeInSystem      = 1200,
        timeTillMaxEffect = 20,
        maxDose           = 3,
        viscosityChange   = 0,
        permissions_needed = {"paramedic", "hart", "t2dev"}
    },
    ["nitroglycerin"] = {
        painReduce        = 0.4,
        hrIncreaseLow     = { 5, 10 },
        hrIncreaseNormal  = { 10, 15 },
        hrIncreaseHigh    = { 8, 12 },
        timeInSystem      = 600,
        timeTillMaxEffect = 5,
        maxDose           = 3,
        viscosityChange   = -5,
        permissions_needed = {"advancedparamedic", "hems", "hart", "lasdoctor", "t2dev"}
    },
    ["amlodipine"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { -3, -1 },
        hrIncreaseNormal  = { -5, -2 },
        hrIncreaseHigh    = { -6, -3 },
        timeInSystem      = 86400,
        timeTillMaxEffect = 120,
        maxDose           = 1,
        viscosityChange   = 0,
        permissions_needed = {"hems", "hart", "lasdoctor", "t2dev"}
    },
    ["lorazepam"] = {
        painReduce        = 0.1,
        hrIncreaseLow     = { -2, 0 },
        hrIncreaseNormal  = { -3, -1 },
        hrIncreaseHigh    = { -3, -1 },
        timeInSystem      = 3600,
        timeTillMaxEffect = 20,
        maxDose           = 3,
        viscosityChange   = 0,
        permissions_needed = {"advancedparamedic", "hems", "hart", "lasdoctor", "t2dev"}
    },
    ["amiodarone"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 0, 0 },
        hrIncreaseNormal  = { -5, -2 },
        hrIncreaseHigh    = { -10, -5 },
        timeInSystem      = 3600,
        timeTillMaxEffect = 60,
        maxDose           = 3,
        viscosityChange   = 0,
        permissions_needed = {"advancedparamedic", "hems", "hart", "lasdoctor", "t2dev"}
    },
    ["metoprolol"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { -5, -3 },
        hrIncreaseNormal  = { -10, -6 },
        hrIncreaseHigh    = { -10, -8 },
        timeInSystem      = 43200,
        timeTillMaxEffect = 60,
        maxDose           = 2,
        viscosityChange   = 0,
        permissions_needed = {"hems", "lasdoctor", "t2dev"}
    },
    ["tramadol"] = {
        painReduce        = 0.7,
        hrIncreaseLow     = { 0, 5 },
        hrIncreaseNormal  = { 0, 3 },
        hrIncreaseHigh    = { -2, 2 },
        timeInSystem      = 2400,
        timeTillMaxEffect = 40,
        maxDose           = 6,
        viscosityChange   = 0,
        permissions_needed = {"advancedparamedic", "hems", "hart", "lasdoctor", "t2dev"}
    },
    ["atropine"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 10, 20 },
        hrIncreaseNormal  = { 10, 15 },
        hrIncreaseHigh    = { 5, 10 },
        timeInSystem      = 180,
        timeTillMaxEffect = 5,
        maxDose           = 5,
        viscosityChange   = 0,
        permissions_needed = {"advancedparamedic", "hems", "hart", "lasdoctor", "t2dev"}
    },
    ["TXA"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 5, 10 },
        hrIncreaseNormal  = { 0, 5 },
        hrIncreaseHigh    = { 0, 2 },
        timeInSystem      = 180,
        timeTillMaxEffect = 5,
        maxDose           = 2,
        viscosityChange   = 0,
        clears_injuries = {"internal_bleeding"},
        permissions_needed = {"advancedparamedic", "hems", "hart", "lasdoctor", "t2dev"}
    },
    ["lidocaine"] = {
        painReduce        = 0.7,
        hrIncreaseLow     = { -5, -2 },
        hrIncreaseNormal  = { -10, -5 },
        hrIncreaseHigh    = { -12, -8 },
        timeInSystem      = 1200,
        timeTillMaxEffect = 10,
        maxDose           = 5,
        viscosityChange   = 0,
        permissions_needed = {"hems", "hart", "lasdoctor", "t2dev"}
    },
    ["ketamine"] = {
        painReduce        = 0.9,
        hrIncreaseLow     = { 5, 10 },
        hrIncreaseNormal  = { 0, 5 },
        hrIncreaseHigh    = { -5, 0 },
        timeInSystem      = 1800,
        timeTillMaxEffect = 15,
        maxDose           = 3,
        viscosityChange   = 0,
        permissions_needed = {"hems", "hart", "lasdoctor", "t2dev"}
    },
    ["chlorphenamine"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { -2, 0 },
        hrIncreaseNormal  = { -2, 0 },
        hrIncreaseHigh    = { -2, 0 },
        timeInSystem      = 21600,
        timeTillMaxEffect = 30,
        maxDose           = 4,
        viscosityChange   = 0,
        permissions_needed = {"hems", "lasdoctor", "t2dev"}
    },
    ["metoclopramide"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 0, 0 },
        hrIncreaseNormal  = { 0, 0 },
        hrIncreaseHigh    = { 0, 0 },
        timeInSystem      = 1800,
        timeTillMaxEffect = 30,
        maxDose           = 3,
        viscosityChange   = 0,
        permissions_needed = {"lasdoctor", "t2dev"}
    },
    ["amoxicillin"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 0, 0 },
        hrIncreaseNormal  = { 0, 0 },
        hrIncreaseHigh    = { 0, 0 },
        timeInSystem      = 4800,
        timeTillMaxEffect = 60,
        maxDose           = 3,
        viscosityChange   = 0,
        clears_injuries = {"chest_infection"},
        permissions_needed = {"advancedparamedic", "hems", "lasdoctor", "t2dev"}
    },
    ["ciprofloxacin"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 0, 0 },
        hrIncreaseNormal  = { 0, 0 },
        hrIncreaseHigh    = { 0, 0 },
        timeInSystem      = 43200,
        timeTillMaxEffect = 120,
        maxDose           = 2,
        viscosityChange   = 0,
        permissions_needed = {"lasdoctor", "t2dev"}
    },
    ["methyleneblue"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { -2, 0 },
        hrIncreaseNormal  = { -2, 0 },
        hrIncreaseHigh    = { -2, 0 },
        timeInSystem      = 43200,
        timeTillMaxEffect = 120,
        maxDose           = 2,
        viscosityChange   = 0,
        clears_injuries = {"poisoned"},
        permissions_needed = {"hart", "t2dev"}
    },
    ["calpol"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { -2, 0 },
        hrIncreaseNormal  = { -2, 0 },
        hrIncreaseHigh    = { -2, 0 },
        timeInSystem      = 43200,
        timeTillMaxEffect = 120,
        maxDose           = 2,
        viscosityChange   = 0,
        permissions_needed = {"t2dev", "normaldev"}
    },
    ["airbubble"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 120, 130 },
        hrIncreaseNormal  = { 120, 130 },
        hrIncreaseHigh    = { 120, 130 },
        timeInSystem      = 190,
        timeTillMaxEffect = 10,
        maxDose           = 10,
        viscosityChange   = 0,
        permissions_needed = {"t2dev", "normaldev"}
    }

    --[[ DISABLED DONT MAKE SENSE TO INJECT VIRUSES UNDER MEDICATIONS

    ["covid"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 120, 130 },
        hrIncreaseNormal  = { 120, 130 },
        hrIncreaseHigh    = { 120, 130 },
        timeInSystem      = 190,
        timeTillMaxEffect = 10,
        maxDose           = 10,
        viscosityChange   = 0,
        permissions_needed = {"t2dev", "normaldev"}
    },
    ["ebola"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 120, 130 },
        hrIncreaseNormal  = { 120, 130 },
        hrIncreaseHigh    = { 120, 130 },
        timeInSystem      = 190,
        timeTillMaxEffect = 10,
        maxDose           = 10,
        viscosityChange   = 0,
        permissions_needed = {"t2dev", "normaldev"}
    },
    ["anthrax"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 120, 130 },
        hrIncreaseNormal  = { 120, 130 },
        hrIncreaseHigh    = { 120, 130 },
        timeInSystem      = 190,
        timeTillMaxEffect = 10,
        maxDose           = 10,
        viscosityChange   = 0,
        permissions_needed = {"t2dev", "normaldev"}
    },
    ["tuberculosis"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 120, 130 },
        hrIncreaseNormal  = { 120, 130 },
        hrIncreaseHigh    = { 120, 130 },
        timeInSystem      = 190,
        timeTillMaxEffect = 10,
        maxDose           = 10,
        viscosityChange   = 0,
        permissions_needed = {"t2dev", "normaldev"}
    },
    ["smallpox"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 120, 130 },
        hrIncreaseNormal  = { 120, 130 },
        hrIncreaseHigh    = { 120, 130 },
        timeInSystem      = 190,
        timeTillMaxEffect = 10,
        maxDose           = 10,
        viscosityChange   = 0,
        permissions_needed = {"t2dev", "normaldev"}
    },
    ["yellowfever"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 120, 130 },
        hrIncreaseNormal  = { 120, 130 },
        hrIncreaseHigh    = { 120, 130 },
        timeInSystem      = 190,
        timeTillMaxEffect = 10,
        maxDose           = 10,
        viscosityChange   = 0,
        permissions_needed = {"t2dev", "normaldev"}
    },
    ["meningitis"] = {
        painReduce        = 0.0,
        hrIncreaseLow     = { 120, 130 },
        hrIncreaseNormal  = { 120, 130 },
        hrIncreaseHigh    = { 120, 130 },
        timeInSystem      = 190,
        timeTillMaxEffect = 10,
        maxDose           = 10,
        viscosityChange   = 0,
        permissions_needed = {"t2dev", "normaldev"}
    } ]]

}
