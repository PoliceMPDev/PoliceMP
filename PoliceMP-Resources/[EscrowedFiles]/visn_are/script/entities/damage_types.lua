--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

DAMAGE_TYPES = {
    ["bullet"] = {
        thresholds = { { damage = 1, amount = 1 }, { damage = 50, amount = 3 }, { damage = 100, amount = 4 } },
        selectionSpecific = true,
    },

    ["explosion"] = {
        thresholds = { { damage = 1, amount = 3 }, { damage = 40, amount = 4 }, { damage = 100, amount = 6 } },
        selectionSpecific = true,
    },

    ["vehicle_crash"] = {
        thresholds = { { damage = 1, amount = 1 }, { damage = 100, amount = 2 }, { damage = 150, amount = 3 } },
        selectionSpecific = true,
    },

    ["collision"] = {
        thresholds = { { damage = 1, amount = 1 }, { damage = 40, amount = 2 }, { damage = 70, amount = 3 } },
        selectionSpecific = true,
    },

    ["stab"] = {
        thresholds = { { damage = 1, amount = 1 } },
        selectionSpecific = true,
    },

    ["punch"] = {
        thresholds = { { damage = 1, amount = 1 } },
        selectionSpecific = true,
    },

    ["falling"] = {
        thresholds = { { damage = 5, amount = 1 }, { damage = 100, amount = 2 }, { damage = 150, amount = 2 } },
        selectionSpecific = true,
    },

    ["burn"] = {
        thresholds = { { damage = 1, amount = 1 } },
        selectionSpecific = true,
    },

    ["drowned"] = {
        thresholds = { { damage = 1, amount = 1 } },
        selectionSpecific = true,
    },
    ["unknown"] = {
        thresholds = { { damage = 1, amount = 1 } },
        selectionSpecific = true,
    },
    ["stun"] = {
        thresholds = { { damage = 1, amount = 1 } },
        selectionSpecific = true,
    }
}

for k, v in pairs(DAMAGE_TYPES) do v.key = k end


local weaponUnarmed = GetHashKey("WEAPON_UNARMED")

ENUM_DAMAGE_HASHES = {
    -- Melee
    -- TYPE = { hash = HASH, category = CATERGORY }

    WEAPON_UNARMED = { hash = -1569615261, category = DAMAGE_TYPES["punch"] },
    WEAPON_KNIFE = { hash = -1716189206, category = DAMAGE_TYPES["stab"] },
    WEAPON_NIGHTSTICK = { hash = 1737195953, category = DAMAGE_TYPES["punch"] },
    WEAPON_HAMMER = { hash = 1317494643, category = DAMAGE_TYPES["punch"] },
    WEAPON_BAT = { hash = -1786099057, category = DAMAGE_TYPES["punch"] },
    WEAPON_CROWBAR = { hash = -2067956739, category = DAMAGE_TYPES["punch"] },
    WEAPON_GOLFCLUB = { hash = 1141786504, category = DAMAGE_TYPES["punch"] },
    WEAPON_BOTTLE = { hash = -102323637, category = DAMAGE_TYPES["stab"] },
    WEAPON_DAGGER = { hash = -1834847097, category = DAMAGE_TYPES["stab"] },
    WEAPON_HATCHET = { hash = -102973651, category = DAMAGE_TYPES["stab"] },
    WEAPON_KNUCKLE = { hash = -656458692, category = DAMAGE_TYPES["punch"] },
    WEAPON_MACHETE = { hash = -581044007, category = DAMAGE_TYPES["stab"] },
    WEAPON_FLASHLIGHT = { hash = -1951375401, category = DAMAGE_TYPES["punch"] },
    WEAPON_SWITCHBLADE = { hash = -538741184, category = DAMAGE_TYPES["stab"] },
    WEAPON_POOLCUE = { hash = -1810795771, category = DAMAGE_TYPES["punch"] },
    WEAPON_WRENCH = { hash = 419712736, category = DAMAGE_TYPES["punch"] },
    WEAPON_BATTLEAXE = { hash = -853065399, category = DAMAGE_TYPES["stab"] },
    WEAPON_STONE_HATCHET = { hash = 940833800, category = DAMAGE_TYPES["stab"] },

    -- Handguns    
    WEAPON_PISTOL = { hash = 453432689, category = DAMAGE_TYPES["bullet"] },
    WEAPON_PISTOL_MK2 = { hash = -1075685676, category = DAMAGE_TYPES["bullet"] },
    WEAPON_COMBATPISTOL = { hash = 1593441988, category = DAMAGE_TYPES["bullet"] },
    WEAPON_APPISTOL = { hash = 584646201, category = DAMAGE_TYPES["bullet"] },
    WEAPON_STUNGUN = { hash = 911657153, category = DAMAGE_TYPES["stun"] },
    WEAPON_PISTOL50 = { hash = -1716589765, category = DAMAGE_TYPES["bullet"] },
    WEAPON_SNSPISTOL = { hash = -1076751822, category = DAMAGE_TYPES["bullet"] },
    WEAPON_SNSPISTOL_MK2 = { hash = -2009644972, category = DAMAGE_TYPES["bullet"] },
    WEAPON_HEAVYPISTOL = { hash = -771403250, category = DAMAGE_TYPES["bullet"] },
    WEAPON_VINTAGEPISTOL = { hash = 137902532, category = DAMAGE_TYPES["bullet"] },
    WEAPON_FLAREGUN = { hash = 1198879012, category = DAMAGE_TYPES["burn"] },
    WEAPON_MARKSMANPISTOL = { hash = -598887786, category = DAMAGE_TYPES["bullet"] },
    WEAPON_REVOLVER = { hash = -1045183535, category = DAMAGE_TYPES["bullet"] },
    WEAPON_REVOLVER_MK2 = { hash = -879347409, category = DAMAGE_TYPES["bullet"] },
    WEAPON_DOUBLEACTION = { hash = -1746263880, category = DAMAGE_TYPES["bullet"] },
    WEAPON_CERAMICPISTOL = { hash = 727643628, category = DAMAGE_TYPES["bullet"] },
    WEAPON_NAVYREVOLVER = { hash = -1853920116, category = DAMAGE_TYPES["bullet"] },
    WEAPON_GADGETPISTOL = { hash = 1470379660, category = DAMAGE_TYPES["bullet"] },


    -- Submachine Guns
    WEAPON_MICROSMG = { hash = 324215364, category = DAMAGE_TYPES["bullet"] },
    WEAPON_SMG = { hash = 736523883, category = DAMAGE_TYPES["bullet"] },
    WEAPON_SMG_MK2 = { hash = 2024373456, category = DAMAGE_TYPES["bullet"] },
    WEAPON_ASSAULTSMG = { hash = -270015777, category = DAMAGE_TYPES["bullet"] },
    WEAPON_COMBATPDW = { hash = 171789620, category = DAMAGE_TYPES["bullet"] },
    WEAPON_MACHINEPISTOL = { hash = -619010992, category = DAMAGE_TYPES["bullet"] },
    WEAPON_MINISMG = { hash = -1121678507, category = DAMAGE_TYPES["bullet"] },

    -- Shotguns
    WEAPON_PUMPSHOTGUN = { hash = 487013001, category = DAMAGE_TYPES["bullet"] },
    WEAPON_PUMPSHOTGUN_MK2 = { hash = 1432025498, category = DAMAGE_TYPES["bullet"] },
    WEAPON_PUMPSHOTGUNMK2 = { hash = 1432025498, category = DAMAGE_TYPES["bullet"] },
    WEAPON_SAWNOFFSHOTGUN = { hash = 2017895192, category = DAMAGE_TYPES["punch"] },
    WEAPON_ASSAULTSHOTGUN = { hash = -494615257, category = DAMAGE_TYPES["bullet"] },
    WEAPON_BULLPUPSHOTGUN = { hash = -1654528753, category = DAMAGE_TYPES["bullet"] },
    WEAPON_MUSKET = { hash = -1466123874, category = DAMAGE_TYPES["bullet"] },
    WEAPON_HEAVYSHOTGUN = { hash = 984333226, category = DAMAGE_TYPES["bullet"] },
    WEAPON_DBSHOTGUN = { hash = -275439685, category = DAMAGE_TYPES["bullet"] },
    WEAPON_AUTOSHOTGUN = { hash = 317205821, category = DAMAGE_TYPES["bullet"] },
    WEAPON_COMBATSHOTGUN = { hash = 94989220, category = DAMAGE_TYPES["bullet"] },

    -- Assault Rifles
    WEAPON_ASSAULTRIFLE = { hash = -1074790547, category = DAMAGE_TYPES["bullet"] },
    WEAPON_ASSAULTRIFLE_MK2 = { hash = 961495388, category = DAMAGE_TYPES["bullet"] },
    WEAPON_CARBINERIFLE = { hash = -2084633992, category = DAMAGE_TYPES["bullet"] },
    WEAPON_CARBINERIFLE_MK2 = { hash = -86904375, category = DAMAGE_TYPES["bullet"] },
    WEAPON_ADVANCEDRIFLE = { hash = -1357824103, category = DAMAGE_TYPES["bullet"] },
    WEAPON_SPECIALCARBINE = { hash = -1063057011, category = DAMAGE_TYPES["bullet"] },
    WEAPON_SPECIALCARBINE_MK2 = { hash = -1768145561, category = DAMAGE_TYPES["bullet"] },
    WEAPON_BULLPUPRIFLE = { hash = 2132975508, category = DAMAGE_TYPES["bullet"] },
    WEAPON_BULLPUPRIFLE_MK2 = { hash = -2066285827, category = DAMAGE_TYPES["bullet"] },
    WEAPON_COMPACTIRIFLE = { hash = 1649403952, category = DAMAGE_TYPES["bullet"] },
    WEAPON_MILITARYRIFLE = { hash = -1658906650, category = DAMAGE_TYPES["bullet"] },
    -- WEAPON_MILITARYRIFLE = { hash = -1658906650, category = DAMAGE_TYPES["bullet"] },


    -- WEAPON_LIVEMP5SEMI
    
    -- Light Machine Guns
    WEAPON_MG = { hash = -1660422300, category = DAMAGE_TYPES["bullet"] },
    WEAPON_COMBATMG = { hash = 2144741730, category = DAMAGE_TYPES["bullet"] },
    WEAPON_COMBATMG_MK2 = { hash = -608341376, category = DAMAGE_TYPES["bullet"] },
    WEAPON_GUSENBERG = { hash = 1627465347, category = DAMAGE_TYPES["bullet"] },
    WEAPON_LIVEMP5SEMI = { hash = 1735312347, category = DAMAGE_TYPES["bullet"] },
    WEAPON_SIGMCX = { hash = 1819330933, category = DAMAGE_TYPES["bullet"] },


    -- Sniper Rifles
    WEAPON_SNIPERRIFLE = { hash = 100416529, category = DAMAGE_TYPES["bullet"] },
    WEAPON_HEAVYSNIPER = { hash = 205991906, category = DAMAGE_TYPES["bullet"] },
    WEAPON_HEAVYSNIPER_MK2 = { hash = 177293209, category = DAMAGE_TYPES["bullet"] },
    WEAPON_MARKSMANRIFLE = { hash = -952879014, category = DAMAGE_TYPES["bullet"] },
    WEAPON_MARKSMANRIFLE_MK2 = { hash = 1785463520, category = DAMAGE_TYPES["bullet"] },
    WEAPON_BULLPUPRIFLEMK2 = { hash = -2066285827, category = DAMAGE_TYPES["bullet"] },
    




    -- Heavy Weapons
    WEAPON_RPG = { hash = -1312131151, category = DAMAGE_TYPES["explosion"] },
    WEAPON_GRENADELAUNCHER = { hash = -1568386805, category = DAMAGE_TYPES["explosion"] },
    WEAPON_MINIGUN = { hash = 1119849093, category = DAMAGE_TYPES["bullet"] },
    WEAPON_FIREWORK = { hash = 2138347493, category = DAMAGE_TYPES["explosion"] },
    WEAPON_HOMINGLAUNCHER = { hash = 1672152130, category = DAMAGE_TYPES["explosion"] },
    WEAPON_COMPACTLAUNCHER = { hash = 125959754, category = DAMAGE_TYPES["explosion"] },

    -- Throwables
    WEAPON_GRENADE = { hash = -1813897027, category = DAMAGE_TYPES["explosion"] },
    WEAPON_BZGAS = { hash = 2694266206, category = DAMAGE_TYPES["unknown"] },
    WEAPON_MOLOTOV = { hash = 615608432, category = DAMAGE_TYPES["burn"] },
    WEAPON_FLASHBANG = { hash = -73270376, category = DAMAGE_TYPES["burn"] },
    WEAPON_FIRE = { hash = -544306709, category = DAMAGE_TYPES["burn"] },



    WEAPON_STICKYBOMB = { hash = 741814745, category = DAMAGE_TYPES["explosion"] },
    WEAPON_SNOWBALL = { hash = 126349499, category = DAMAGE_TYPES["unknown"] },
    WEAPON_PIPEBOMB = { hash = 3125143736, category = DAMAGE_TYPES["explosion"] },




    -- Miscellaneous
    WEAPON_PETROLCAN = { hash = -544306709, category = DAMAGE_TYPES["burn"] },



    -- Explosion Types
    WEAPON_HELICOPTERCRASH = { hash = -1945616459, category = DAMAGE_TYPES["explosion"] },



    -- GTA-Damage Types
    WEAPON_CAR_CRASH = { hash = 133987706, category = DAMAGE_TYPES["vehicle_crash"] },
    WEAPON_DROWNED = { hash = -10959621, category = DAMAGE_TYPES["drowned"] },
    WEAPON_DROWNED_IN_CAR = { hash = 1936677264, category = DAMAGE_TYPES["drowned"] },
    WEAPON_EXPLOSION = { hash = 539292904, category = DAMAGE_TYPES["explosion"] },
    WEAPON_HIT_BY_WATER_CANNON = { hash = -868994466, category = DAMAGE_TYPES["collision"] },
    WEAPON_HELI_BLADES = { hash = -1323279794, category = DAMAGE_TYPES["stab"] },
    WEAPON_RAMMED_BY_CAR = { hash = -1553120962, category = DAMAGE_TYPES["collision"] },
    WEAPON_FALL = { hash = -842959696, category = DAMAGE_TYPES["falling"] },



-- Custom Weapons - weapon menu tier 2
    WEAPON_FM1_P320 = { hash = -204828888, category = DAMAGE_TYPES["bullet"]},
    WEAPON_FM3_REMINGTON700 = { hash = 989821624, category = DAMAGE_TYPES["bullet"]},
    WEAPON_X45 = { hash = -1404689587, category = DAMAGE_TYPES["bullet"]},
    WEAPON_KVR = { hash = -1666052367, category = DAMAGE_TYPES["bullet"]},
    WEAPON_R9 = { hash = 2087259267, category = DAMAGE_TYPES["bullet"]},
    WEAPON_FM1_REMINGTON700 = { hash = -878306099, category = DAMAGE_TYPES["bullet"]},
    WEAPON_FM2_HK416 = { hash = 1659810914, category = DAMAGE_TYPES["bullet"]},
    WEAPON_AGC = { hash = -1189752405, category = DAMAGE_TYPES["bullet"]},
    WEAPON_MI9 = { hash = 745660374, category = DAMAGE_TYPES["bullet"]},
    WEAPON_FM2_REMINGTON700 = { hash = -563510050, category = DAMAGE_TYPES["bullet"]},
    WEAPON_C36 = { hash = -1142851258, category = DAMAGE_TYPES["bullet"]},
    WEAPON_X19 = { hash = -134954891, category = DAMAGE_TYPES["bullet"]},
    WEAPON_MP5SDFM = { hash = 1653333080, category = DAMAGE_TYPES["bullet"]},
    WEAPON_FM1_P226 = { hash = -1630761003, category = DAMAGE_TYPES["bullet"]},
    WEAPON_FM1_HK416 = { hash = 1196757952, category = DAMAGE_TYPES["bullet"]},
    WEAPON_MRevolver = { hash = -411537688, category = DAMAGE_TYPES["bullet"]},
    WEAPON_HKUMP = { hash = 1846078594, category = DAMAGE_TYPES["bullet"]},
    WEAPON_FM1_GLOCK19 = { hash = -1409371625, category = DAMAGE_TYPES["bullet"]},
    WEAPON_MRevolver2 = { hash = -1159865767, category = DAMAGE_TYPES["bullet"]},
    WEAPON_A45 = { hash = -1553175928, category = DAMAGE_TYPES["bullet"]},
    WEAPON_M700a = { hash = -1656504045, category = DAMAGE_TYPES["bullet"]},
    WEAPON_DOUBLEBARRELFM = { hash = -888260167, category = DAMAGE_TYPES["bullet"] },
    WEAPON_SIG716 = { hash = 772798974, category = DAMAGE_TYPES["bullet"] },
    weapon_rammed_by_car = { hash = 133987706, category = DAMAGE_TYPES["vehicle_crash"] }
     
    --x19 modular
    -- Custom Damage



}

ENUM_COMPARE_DAMAGE_TYPES = {}


for _, v in pairs(ENUM_DAMAGE_HASHES) do ENUM_COMPARE_DAMAGE_TYPES[v.hash] = v.category end