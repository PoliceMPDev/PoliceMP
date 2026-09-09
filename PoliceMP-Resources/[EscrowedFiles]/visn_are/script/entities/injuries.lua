--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

INJURIES = {
    -- Available options:
    
    -- causes: table: Table of the damage types that causes this injury
    -- bleeding: number: How much this injury will affect the bleeding
    -- pain: number: How much this injury will affect the pain
    -- causeLimping: bool: If this injury will cause limping when active
    -- causeFracture: bool: If this injury will cause a fracture (not implemented yet)
    -- needSeewing: bool: If this injury could cause a need for seewing
    -- exclusiveBodyParts: table: Table of body parts that this injury can only be applied to
    
    -- Also called scrapes, they occur when the skin is rubbed away by friction against another rough surface (e.g. rope burns and skinned knees).
    ["abrasion"] = {
        causes = { "falling", "vehicle_crash", "collision", "punch", "unknown" },
        bleeding = 0.001,
        minDamage = 4,
        maxDamage = 75,
        pain = 0.4
    },

    -- Occur when an entire structure or part of it is forcibly pulled away, such as the loss of a permanent tooth or an ear lobe. Explosions, gunshots, and animal bites may cause avulsions.
    ["avulsion"] = {
        causes = { "explosion", "bullet" },
        bleeding = 0.008,
        pain = 1.0,
        minDamage = 15,
        causeLimping = true,
        needSewing = true,
        causeWeaponAimShake = true
    },

    -- Also called bruises, these are the result of a forceful trauma that injures an internal structure without breaking the skin. Blows to the chest, abdomen, or head with a blunt instrument (e.g. a football or a fist) can cause contusions.
    ["contusion"] = {
        causes = { "punch", "vehicle_crash", "falling", "collision", "unknown" },
        bleeding = 0.005,
        timeout = 200,
        minDamage = 2,
        maxDamage = 25,
        pain = 0.3
    },

    -- Occur when a heavy object falls onto a person, splitting the skin and shattering or tearing underlying structures.
    ["crush"] = {
        causes = { "punch", "vehicle_crash", "falling", "collision" },
        bleeding = 0.005,
        pain = 0.8,
        minDamage = 50,
        causeLimping = true,
        needSewing = true,
        causeFracture = true
    },

    -- Slicing wounds made with a sharp instrument, leaving even edges. They may be as minimal as a paper cut or as significant as a surgical incision.
    ["cut"] = {
        causes = { "stab", "vehicle_crash", "explosion", "punch", "unknown", "bullet", "collision", "falling", "burn" },
        bleeding = 0.003,
        minDamage = 10,
        pain = 0.1
    },

    -- Also called tears, these are separating wounds that produce ragged edges. They are produced by a tremendous force against the body, either from an internal source as in childbirth, or from an external source like a punch.
    ["laceration"] = {
        causes = { "vehicle_crash", "stab", "explosion", "punch", "unknown", "bullet", "collision", "falling" },
        bleeding = 0.008,
        minDamage = 50,
        needSewing = true,
        pain = 0.2
    },

    -- Also called velocity wounds, they are caused by an object entering the body at a high speed, typically a bullet or small peices of shrapnel.
    ["velocity_wound"] = {
        causes = { "explosion", "bullet" },
        bleeding = 0.004,
        pain = 0.9,
        minDamage = 5,
        causeLimping = true,
        causeFracture = true,
        needSewing = true,
        causeWeaponAimShake = true
    },

    ["burn_injury"] = {
        causes = { "burn" },
        bleeding = 0.003,
        pain = 0.2,
        minDamage = 5,
        causeLimping = true
    },

    -- Deep, narrow wounds produced by sharp objects such as nails, knives, and broken glass.
    ["puncture_wound"] = {
        causes = { "stab", "explosion", "falling" },
        bleeding = 0.003,
        pain = 0.5,
        minDamage = 2,
        needSewing = true,
        causeLimping = true
    },

    ["drowned"] = {
        causes = { "drowned" },
        pain = 0.4,
        bleeding = 0.003,
        minDamage = 2,
        maxDamage = 10,
        causeLimping = true,
        exclusiveBodyParts = {"TORSO"}
    },
    ["puncturedlung"] = {
        causes = { "stab", "bullet", "vehicle_crash", "collision", "explosion", "unknown" },
        pain = 0.4,
        bleeding = 0.003,
        minDamage = 50,
        needSewing = true,
        exclusiveBodyParts = {"TORSO"}
    },
    ["tbi"] = {
        causes = { "bullet", "falling", "collision", "explosion", "vehicle_crash", "unknown" },
        pain = 0.4,
        bleeding = 0.003,
        minDamage = 2,
        maxDamage = 75,
        causeLimping = true,
        exclusiveBodyParts = {"HEAD"}
    },
    ["impalement"] = {
        causes = { "explosion", "vehicle_crash", "collision", "falling", "unknown" },
        pain = 0.4,
        bleeding = 0.003,
        minDamage = 2,
        maxDamage = 50,
        causeLimping = true,
        needSewing = true,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "LEFT_ARM", "RIGHT_ARM"}
    },
    ["compartmentsyndrome"] = {
        causes = {"vehicle_crash", "collision", "falling" },
        pain = 0.4,
        minDamage = 2,
        bleeding = 0.003,
        maxDamage = 50,
        causeLimping = true,
        needSewing = true,
        exclusiveBodyParts = {"LEFT_LEG", "RIGHT_LEG", "LEFT_ARM", "RIGHT_ARM"}
    },
    ["blocked_airways"] = {
        causes = {"stab", "vehicle_crash", "explosion", "punch", "unknown", "bullet", "collision", "falling", "burn" },
        pain = 0.4,
        minDamage = 2,
        bleeding = 0.003,
        maxDamage = 80,
        causeLimping = true,
        exclusiveBodyParts = {"HEAD"}
    },
    ["partial_blockage"] = {
        causes = {"stab", "vehicle_crash", "explosion", "punch", "unknown", "bullet", "collision", "falling", "burn" },
        pain = 0.4,
        minDamage = 2,
        bleeding = 0.003,
        maxDamage = 50,
        causeLimping = true,
        exclusiveBodyParts = {"HEAD"}
    },
    ["broken_leg"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.4,
        minDamage = 2,
        maxDamage = 30,
        bleeding = 0.003,
        causeLimping = true,
        exclusiveBodyParts = {"LEFT_LEG", "RIGHT_LEG"}
    },
    ["broken_arm"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.4,
        minDamage = 2,
        maxDamage = 30,
        bleeding = 0.003,
        causeLimping = true,
        exclusiveBodyParts = {"LEFT_ARM", "RIGHT_ARM"}
    },
    ["open_fracture"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.4,
        minDamage = 2,
        maxDamage = 60,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = true,
        exclusiveBodyParts = {"LEFT_ARM", "RIGHT_ARM", "RIGHT_LEG", "LEFT_LEG"}
    },
    ["closed_fracture"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.4,
        minDamage = 2,
        maxDamage = 40,
        bleeding = 0.003,
        causeLimping = true,
        exclusiveBodyParts = {"LEFT_ARM", "RIGHT_ARM", "RIGHT_LEG", "LEFT_LEG"}
    },
    ["broken_neck"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.4,
        minDamage = 2,
        maxDamage = 80,
        bleeding = 0.003,
        causeLimping = true,
        exclusiveBodyParts = {"HEAD"}
    },
    ["broken_finger"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.4,
        minDamage = 2,
        maxDamage = 20,
        bleeding = 0.003,
        causeLimping = true,
        exclusiveBodyParts = {"LEFT_ARM", "RIGHT_ARM"}
    },
    ["broken_toe"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.4,
        minDamage = 2,
        maxDamage = 20,
        bleeding = 0.003,
        causeLimping = true,
        exclusiveBodyParts = {"RIGHT_LEG", "LEFT_LEG"}
    },
    ["broken_pelvis"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.4,
        minDamage = 2,
        maxDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        exclusiveBodyParts = {"RIGHT_LEG", "LEFT_LEG"}
    },
    ["broken_ribs"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.4,
        minDamage = 80,
        bleeding = 0.003,
        causeLimping = true,
        exclusiveBodyParts = {"TORSO"}
    },
    ["loss_of_circulation"] = {
        causes = {"vehicle_crash", "explosion", "unknown", "bullet", "collision", "falling" },
        pain = 0.4,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = true,
        exclusiveBodyParts = {"LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["heart_attack"] = {
        causes = {"unknown" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO"}
    },
    ["overdose"] = {
        causes = {"unknown" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["anaphylaxis"] = {
        causes = {"unknown" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"HEAD", "TORSO"}
    },
    ["asthma_attack"] = {
        causes = {"unknown" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"HEAD", "TORSO"}
    },
    ["hypothermia"] = {
        causes = {"unknown", "drowned" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["electrocution"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["poisoned"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["hypoglycemia"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["cardio_disturbance"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["seizure"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["internal_bleeding"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.2,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = false,
        needSewing = false,
        exclusiveBodyParts = {"HEAD", "TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["chest_infection"] = {
        causes = {"unknown"},
        pain = 0.3,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"HEAD", "TORSO"}
    },
    ["mid_femur_fracture"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.5,
        minDamage = 60,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"LEFT_LEG", "RIGHT_LEG"}
    },
    ["dislocation"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown" },
        pain = 0.5,
        minDamage = 30,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"LEFT_ARM", "RIGHT_ARM", "LEFT_LEG", "RIGHT_LEG"}
    },
    ["choking"] = {
        causes = {"vehicle_crash", "collision", "falling", "punch", "unknown", "explosion" },
        pain = 0.5,
        minDamage = 40,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"HEAD", "TORSO"}
    },
    ["degloved_wound"] = {
        causes = {"vehicle_crash", "collision", "falling", "unknown", "explosion" },
        pain = 0.5,
        minDamage = 50,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = true,
        exclusiveBodyParts = {"TORSO", "LEFT_ARM", "RIGHT_ARM", "LEFT_LEG", "RIGHT_LEG"}
    },
    ["headache"] = {
        causes = {"vehicle_crash", "collision", "falling", "unknown", "explosion", "punch" },
        pain = 0.5,
        minDamage = 30,
        bleeding = 0.003,
        causeLimping = false,
        needSewing = false,
        exclusiveBodyParts = {"HEAD"}
    },
    ["diabetic_ketoacidosis"] = {
        causes = {"unknown"},
        pain = 0.6,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO"}
    },
    ["sucking_chest_wound"] = {
        causes = { "vehicle_crash", "stab", "explosion", "unknown", "bullet" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO"}
    },
    ["abdominal_aneurysm"] = {
        causes = { "unknown" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO"}
    },
    ["hemothorax"] = {
        causes = { "vehicle_crash", "collision", "falling", "unknown", "explosion", "punch", "bullet" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO"}
    },
    ["tension_pneumothorax"] = {
        causes = { "vehicle_crash", "collision", "falling", "unknown", "explosion", "punch", "bullet" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO"}
    },
    ["blunt_chest_trauma"] = {
        causes = {"vehicle_crash", "collision", "falling", "unknown", "explosion", "punch" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO"}
    },
    ["cardiac_tamponade"] = {
        causes = {"vehicle_crash", "collision", "explosion", "bullet" },
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO"}
    },
    ["smoke_inhalation"] = {
        causes = {"explosion", "burn", "unknown" },
        pain = 0.5,
        minDamage = 65,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"HEAD", "TORSO"}
    },
    ["heat_stroke"] = {
        causes = {"unknown"},
        pain = 0.3,
        minDamage = 50,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"HEAD", "TORSO"}
    },
    ["super_glued"] = {
        causes = {"unknown"},
        pain = 0.2,
        minDamage = 45,
        bleeding = 0.000,
        causeLimping = false,
        needSewing = false,
    },
    ["barbs"] = {
        causes = {"stun" },
        pain = 0.7,
        minDamage = 1,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["iwf"] = {
        causes = { "ammo" },
        pain = 999,
        minDamage = 999,
        bleeding = 999,
        causeLimping = false,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "HEAD"}
    },
    -- Start of HART Infectious Deseases
    ["covid"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    }

    --[[ Disabled untreatable viruses

    ["ebola"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["anthrax"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["tuberculosis"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["smallpox"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["yellowfever"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },
    ["meningitis"] = {
        causes = {"unknown"},
        pain = 0.7,
        minDamage = 75,
        bleeding = 0.003,
        causeLimping = true,
        needSewing = false,
        exclusiveBodyParts = {"TORSO", "LEFT_LEG", "RIGHT_LEG", "RIGHT_ARM", "LEFT_ARM"}
    },  ]]--
    
}