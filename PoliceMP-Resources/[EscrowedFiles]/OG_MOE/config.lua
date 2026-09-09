Config = {} -- Main configuration table (do not modify directly)

--------------------------------------------------------------
--                         Shared                           --
--------------------------------------------------------------

Config.EnforcerWeapon = 'WEAPON_GOLFCLUB' -- Weapon hash required to use the INK feature

-- MOE Permissions Configuration
Config.MOE = { -- Method Of Entry (ENFORCER)
    IsJobPermissionsRequired = false, -- Set to true if specific jobs are required for access
    Jobs = { -- List of allowed jobs (used if permissions are required)
        'police',
        'police2'
    }
}

Config.EMOE = { -- Explosive Method Of Entry
    IsJobPermissionsRequired = false, -- Set to true if specific jobs are required for access
    Jobs = { -- List of allowed jobs (used if permissions are required)
        'police',
        'police2'
    },
    IsUseItem = false, -- Whether requires an item to use
    ItemName = 'emoe', -- Item name.
    DoorBlownTime = 10000, -- Time in milliseconds for the door to be blown up before reset (10 seconds)
}

Config.Commands = {
    IsUseDoorToggleCommands = true,
    Lock = 'lock',
    Unlock = 'unlock',
}

--------------------------------------------------------------
--                         Client                           --
--------------------------------------------------------------

-- Adds an interaction to nearby players for the Ink functionality.
-- This function integrates with the ox_target system to allow players to use the INK feature
-- on nearby players within a specified distance.

local canUseEmoe = false

-- Receive ACE permission result from server
RegisterNetEvent('moe:returnAcePermission', function(allowed)
    canUseEmoe = allowed
    if allowed then
        TryUseEmoe()
    else
        Notify("You are not allowed to use EMOE.")
    end
end)

-- Simple in-game notification
function Notify(msg)
    SetNotificationTextEntry("STRING")
    AddTextComponentString(msg)
    DrawNotification(false, false)
end

-- Get entity player is looking at
function GetEntityPlayerIsLookingAt()
    local playerPed = PlayerPedId()
    local coords = GetEntityCoords(playerPed)
    local forward = GetEntityForwardVector(playerPed)
    local distance = 2.5
    local targetCoords = coords + (forward * distance)

    local rayHandle = StartShapeTestRay(coords.x, coords.y, coords.z, targetCoords.x, targetCoords.y, targetCoords.z, -1, playerPed, 0)
    local _, hit, _, _, entityHit = GetShapeTestResult(rayHandle)

    if hit == 1 and DoesEntityExist(entityHit) then
        return entityHit
    end
    return nil
end

-- Try to use EMOE
function TryUseEmoe()
    local entity = GetEntityPlayerIsLookingAt()
    if entity and IsPlayerAllowedToEMOE(entity) then
        UseEMOE(entity)
    else
        --Notify("You cannot use EMOE on this target.")
    end
end

-- Command: /moe
RegisterCommand("moe", function()
    local entity = GetEntityPlayerIsLookingAt()
    local coords = GetEntityCoords(PlayerPedId())

    if entity and IsPlayerAllowedToMOE(entity) then
        UseEnforcer(entity, coords)
    else
        --Notify("You do not have Enforcer.")
    end
end, false)

-- Command: /emoe
RegisterCommand("emoe", function()
    TriggerServerEvent("moe:checkAcePermission")
end, false)

RegisterCommand("-emoe", function(source, args, raw)
    -- Do nothing to suppress
end, false)

Config.Doors = {
    "am_fm4_doorixr_l",
    "am_fm4_doorixr_r",
    "v_ilev_cor_windowsmash",
    "v_ilev_cor_windowsolid",
    "v_ilev_cm_door1",
    "v_ilev_fib_doorbrn",
    "v_ilev_mm_windowwc",
	'v_ilev_fa_frontdoor', -- Franklin's House (Main Door)
	'gabz_mp_house_10_1_singledoor', -- mirror park house
	'gabz_mp_house_10_1_kitchendoor', -- mirror park house
	'gabz_mp_house_10_1_entrancedoor_b', -- mirror park house
	'gabz_mp_house_10_1_entrancedoor_f', -- mirror park house
	'gabz_mp_house_10_1_singledoor', -- mirror park house
    'gabz_mp_house_08_1_singledoor', -- mirror park house
    'gabz_mp_house_08_1_entrancedoor_b', -- mirror park house
    'gabz_mp_house_08_1_entrancedoor_f', -- mirror park house
    'hei_prop_hei_bankdoor_new', -- Pacific Standard Exterior Doors
    'v_ilev_bk_door', -- Pacific Standard Interior Doors
    'v_ilev_bk_door2', -- Pacific Standard Officer Doors 
    'v_ilev_cs_door01_r', -- Store door (Right)
    'v_ilev_cs_door01', -- Store door (Left)
    'v_ilev_ss_door5_r', -- Route 68 Garage Office Door
    'v_ilev_spraydoor', -- Route 68 Garage Interior Door
    'v_ilev_ss_door5_r', -- Paleto Bay Office Door
    'v_ilev_gc_door03', -- Ammunation (Right)
    'v_ilev_gc_door04', -- Ammunation (Left)
    'v_ilev_gc_door01', -- Ammunation (Interior - Shooting Range)
    'v_ilev_ch_glassdoor', -- Ponsonby's Door
    'v_ilev_clothmiddoor', -- Sub Urban Door
    'v_ilev_ml_door1', -- Rob's Liquor Main Door
    'v_ilev_ss_door04', -- Rob's Liquor Staff Doors
    'v_ilev_hd_door_r', -- Bob Mulet Barber Shop Door (Right)
    'v_ilev_hd_door_l', -- Bob Mulet Barber Shop Door (Left)
    'v_ilev_bs_door', -- Generic Barber Shop Door
    'v_ilev_csr_door_r', -- Premium Deluxe Motorsport Parking Door (Right)
    'v_ilev_csr_door_l', -- Premium Deluxe Motorsport Parking Door (Left)
    'v_ilev_fib_door1', -- Premium Deluxe Motorsport Office Door
    'prop_strip_door_01', -- Vanilla Unicorn Main Entrance
    'prop_magenta_door', -- Vanilla Unicorn Back Door
    'v_ilev_roc_door2', -- Vanilla Unicorn Office Door
    'v_ilev_door_orangesolid', -- Vanilla Unicorn Dress Door
    'v_ilev_door_orange', -- Vanilla Unicorn Private Rooms Door
    'v_ilev_mm_doorm_l', -- Micheal's House Front Door (Left)
    'v_ilev_mm_doorm_r', -- Micheal's House Front Door (Right)
    'prop_bh1_48_backdoor_l', -- Micheal's House Back Door (Left)
    'prop_bh1_48_backdoor_r', -- Micheal's House Back Door (Right)
    'v_ilev_mm_door', -- Micheal's House Interior Door
    'v_ilev_ph_cellgate', -- Cell Door
    'v_ilev_ph_cellgate02', -- Cell Door 2
    "v_ilev_arm_secdoor",
    "v_ilev_abbmaindoor",
    "v_ilev_ch_glassdoor",
    "v_ilev_247door",
    "v_ilev_247door_r",
    "v_ilev_genbankdoor2",
    "v_ilev_genbankdoor1",
    "lr_prop_supermod_door_01",
    "v_ilev_trev_doorfront",
    "v_ilev_trev_door",
    "v_ilev_trev_doorbath",
    "v_ilev_ta_door",
    "v_ilev_ta_door2",
    "v_ilev_clothmiddoor",
    "v_ilev_fb_doorshortl",
    "v_ilev_fb_doorshortr",
    "v_ilev_fb_door01",
    "v_ilev_fb_door02",
    "v_ilev_fib_door2",
    "v_ilev_j2_door",
    "p_jewel_door_r1",
    "p_jewel_door_l",
    "v_ilev_roc_door4",
    "v_ilev_roc_door1_l",
    "v_ilev_roc_door1_r",
    "v_ilev_roc_door2",
    "ap_cp_doors",
    "ap_cp_doors_em",
    "ap_cp_door_01",
    "ap_cp_door_02",
    "bm_e2_doorframe",
    "bm_new_door1",
    "bm_new_door2",
    "bm_new_door3",
    "bm_rub_door1",
    "bx_hp2_grge_door01",
    "cabaret_door_l",
    "cabaret_door_r",
    "cj_angel_door_l",
    "cj_angel_door_r",
    "cj_bank_door_l",
    "cj_bank_door_r",
    "cj_boat_door",
    "cj_bs_door_l",
    "cj_bs_door_r",
    "cj_church_door_l",
    "cj_church_door_r",
    "cj_db_mh3_door1",
    "cj_e1_door_1",
    "cj_e1_door_lost",
    "cj_ext_door_1",
    "cj_ext_door_10",
    "cj_ext_door_11",
    "cj_ext_door_15b",
    "cj_ext_door_16",
    "cj_ext_door_17",
    "cj_ext_door_18",
    "cj_ext_door_19_l",
    "cj_ext_door_19_r",
    "cj_ext_door_22",
    "cj_ext_door_6",
    "cj_ext_door_9",
    "cj_ext_door_cm",
    "cj_garage_door_big",
    "cj_gm_door_04",
    "cj_gm_door_05",
    "cj_gm_door_1",
    "cj_gm_door_2",
    "cj_g_door_big",
    "cj_g_door_big2",
    "cj_int_door_1",
    "cj_int_door_10",
    "cj_int_door_12_h",
    "cj_int_door_2",
    "cj_int_door_22",
    "cj_int_door_24",
    "cj_int_door_27l",
    "cj_int_door_27r",
    "cj_int_door_29",
    "cj_int_door_30",
    "cj_int_door_3l",
    "cj_int_door_3r",
    "cj_int_door_6",
    "cj_int_door_7",
    "cj_int_door_9",
    "cj_ja_door1",
    "cj_ld_garage_door",
    "cj_ld_met_door_l",
    "cj_ld_met_door_r",
    "cj_lift_l_door",
    "cj_lift_l_door_2",
    "cj_lift_l_door_out",
    "cj_lift_l_door_out_2",
    "cj_lift_r_door",
    "cj_lift_r_door_2",
    "cj_lift_r_door_out",
    "cj_lift_r_door_out_2",
    "cj_lost_door",
    "cj_mc_door_1",
    "cj_mision_door_1",
    "cj_mp_fact_door_2",
    "cj_new_bowl_door_l",
    "cj_new_bowl_door_r",
    "cj_new_china_door_l",
    "cj_new_china_door_r",
    "cj_nf_garage_door",
    "cj_nf_garage_door2",
    "cj_per_door_l",
    "cj_per_door_r",
    "cj_rus_door_1",
    "cj_rus_door_2",
    "cj_shoot_t_door",
    "cj_shop_door_1",
    "cj_sm_dave_door",
    "cj_t_door_brk",
    "cj_t_door_eng",
    "cj_t_door_vac",
    "cj_vault_door",
    "cj_vault_door_dam",
    "cj_ware_door",
    "ck_ts_door_03",
    "deal_doora",
    "deal_doorb",
    "doorred",
    "e1_ash_door",
    "e1_ash_doorfrm1",
    "e1_bowl_doorfrm",
    "e1_club_doors",
    "e1_pris_door_l",
    "e1_pris_door_l_dam",
    "e1_pris_door_r",
    "e1_pris_door_r_dam",
    "e2_bowl_doorfrm",
    "e2_bowl_door_l",
    "e2_bowl_door_r",
    "e2_int6_door",
    "e2_int6_door05",
    "e2_int6_door06",
    "e2_int6_doorglss",
    "e2_int6_doorglss03",
    "e2_int6_doorglss04",
    "e2_int6_doorglss2",
    "e2_sky_door_l01",
    "e2_sky_door_l02",
    "e2_sky_door_l03",
    "e2_sky_door_r01",
    "e2_sky_door_r02",
    "e2_xjoff_door02",
    "e2_xjoff_door22",
    "e2_xjoff_door25",
    "e2_xjoff_door27",
    "e2_xjoff_door29",
    "ec_cell_door",
    "ec_jet_door",
    "ec_ml_door_l",
    "ec_ml_door_r",
    "garage_door_1",
    "garage_door_1_tmp05",
    "garage_door_2",
    "garage_door_3",
    "gb_doormat05",
    "gb_safe02_door",
    "int_door01_mx5",
    "ld_show_door_l",
    "ld_show_door_r",
    "lev6_bwl_doorfrm",
    "lift_door1_mx5",
    "magkiosk_door",
    "nge_doorf_mh1",
    "nsexcounter_door",
    "proj_doorrm1",
    "proj_doorrm1d",
    "proj_doorrm2",
    "proj_doorrm4",
    "p_e2_bm_door",
    "p_int_door_ah",
    "p_jet_door_1",
    "retail2_sex_door",
    "tg_f_doorbell",
    "xjcj_int_door_02",
    "xj_int_door_bnx",
    "v_ilev_ph_cellgate",
    "amb_cluck_door_arm",
    "amb_dreamotel_main_door",
    "amb_fbi_fire_door",
    "amb_fbi_smoke_door_hvy",
    "amb_fbi_smoke_door_lt",
    "amb_fbi_smoke_door_med",
    "amb_hosp_interior_door_arm",
    "amb_hosp_interior_door_arm_02",
    "amb_hosp_interior_door_arm_03",
    "amb_house1_door_l",
    "amb_house1_door_r",
    "amb_house2int_door",
    "amb_house8int_prop_door",
    "amb_house8int_prop_door1_l",
    "amb_house8int_prop_door1_r",
    "amb_rox_01_bayin_door",
    "amb_rox_01_bayin_door_2",
    "amb_rox_01_coast_guard_05_door",
    "amb_rox_01_coast_guard_door",
    "amb_rox_01_garage_door",
    "amb_rox_01_getaweigh_door",
    "amb_rox_01_getaweigh_front_door",
    "amb_rox_01_pn_door_01",
    "amb_rox_01_pn_door_02",
    "amb_rox_02_int_beanmachine_doorsfx_game.dat151.rel",
    "amb_rox_02_int_beanmachine_prop_doors_01",
    "amb_rox_02_int_beanmachine_prop_doors_02",
    "amb_rox_02_int_beanmachine_prop_door_txd.ytd",
    "amb_rox_02_int_dikinbaus_prop_doors_txd.ytd",
    "amb_rox_02_int_dikinbaus_prop_door_01",
    "amb_rox_02_int_dikinbaus_prop_door_02",
    "amb_rox_02_int_dikinbaus_prop_door_03",
    "amb_rox_03_bds_big_door_l",
    "amb_rox_03_bds_big_door_r",
    "amb_rox_03_bds_door_l",
    "amb_rox_03_bds_door_r",
    "amb_rox_03_donut_back_doors",
    "amb_rox_03_donut_door_001",
    "amb_rox_03_donut_ent_02_door",
    "amb_rox_03_donut_ent_door",
    "amb_rox_03_pine_bank_ent_door",
    "amb_rox_03_pine_bank_vault_door",
    "amb_rox_04_taxi_int_door",
    "amb_rox_04_taxi_int_door_02",
    "amb_rox_gasstation_prop_door_01l",
    "amb_rox_gasstation_prop_door_01r",
    "amb_rox_gasstation_prop_door_02",
    "amb_rox_gasstation_prop_door_shitbooth",
    "amb_rox_gasstation_prop_door_shitter_01",
    "amb_rox_gasstation_prop_door_shitter_02",
    "amb_rpd_door01",
    "amb_rpd_door02",
    "amb_rpd_door03",
    "amb_rpd_door04",
    "ba_prop_door_club_glass",
    "denis3d_catcafe_doors",
    "denis3d_catcafe_doorsA",
    "denis3d_catcafe_doorsB",
    "denis3d_ts_container_doors",
    "dnxprops_electronics_doorbell01_a",
    "dnxprops_electronics_doorbell01_a_custom01",
    "dnxprops_electronics_doorbell01_a_custom02",
    "dnxprops_electronics_doorbell01_a_custom03",
    "dnxprops_electronics_doorbell01_a_custom04",
    "dnxprops_electronics_doorbell01_a_custom05",
    "dnxprops_electronics_doorbell01_b",
    "dnxprops_electronics_doorbell01_b_custom01",
    "dnxprops_electronics_doorbell01_b_custom02",
    "dnxprops_electronics_doorbell01_b_custom03",
    "dnxprops_electronics_doorbell01_b_custom04",
    "dnxprops_electronics_doorbell01_b_custom05",
    "doorframe_shoot_main",
    "doortuning.ymt",
    "gabz_ammu_big_door",
    "gabz_atom_counter_door",
    "gabz_atom_door_01",
    "gabz_atom_door_02",
    "gabz_atom_drivethru_door_l",
    "gabz_atom_drivethru_door_r",
    "gabz_atom_entrance_door_l",
    "gabz_atom_entrance_door_r",
    "gabz_atom_freezer_door",
    "gabz_atom_toilet_door",
    "gabz_bahama_door_01",
    "gabz_bahama_door_02",
    "gabz_bahama_door_03",
    "gabz_bahama_door_04a",
    "gabz_bahama_door_04b",
    "gabz_bahama_toilet_door",
    "gabz_bennys_prop_garage_door_01",
    "gabz_bennys_prop_garage_door_02",
    "gabz_bennys_room01_spraybooth_doors",
    "gabz_binco_changing_door",
    "gabz_binco_door_l",
    "gabz_binco_door_r",
    "gabz_bmlegion_door_left",
    "gabz_bmlegion_door_right",
    "gabz_bmlegion_door_small",
    "gabz_burgershot_door_01",
    "gabz_burgershot_door_02",
    "gabz_burgershot_door_03",
    "gabz_burgershot_door_04",
    "gabz_burgershot_door_05a",
    "gabz_burgershot_door_05b",
    "gabz_burgershot_door_ext_01",
    "gabz_burgershot_door_ext_02",
    "gabz_burgershot_freezer_door",
    "gabz_diner_ext_door_001",
    "gabz_diner_ext_door_002",
    "gabz_diner_int_doors",
    "gabz_gangs_int01_door_01",
    "gabz_gangs_int01_door_02",
    "gabz_gangs_int01_door_03",
    "gabz_gangs_int02_door_01",
    "gabz_gangs_int02_door_02",
    "gabz_gangs_int02_door_03",
    "gabz_g_aztecas_door_01",
    "gabz_g_aztecas_door_back",
    "gabz_g_aztecas_door_front",
    "gabz_g_ballas_door_01",
    "gabz_g_ballas_door_02",
    "gabz_g_ballas_door_03",
    "gabz_g_families_door_01",
    "gabz_g_families_door_02",
    "gabz_g_families_toilet_door",
    "gabz_g_hillbillies_door_01",
    "gabz_g_hillbillies_door_entrance",
    "gabz_g_hillbillies_door_toilet_01",
    "gabz_g_vagos_door_01",
    "gabz_g_vagos_door_back",
    "gabz_g_vagos_door_front",
    "gabz_g_vagos_toilet_door",
    "gabz_harmony_garage_door_01",
    "gabz_harmony_int_door_01",
    "gabz_harmony_int_door_02",
    "gabz_harmony_int_door_03",
    "gabz_harmony_int_door_04",
    "gabz_harmony_office_door",
    "gabz_harmony_showroom_door",
    "gabz_hermit_toilet_door",
    "gabz_hermit_wooden_door",
    "gabz_hermit_wooden_door_l",
    "gabz_hermit_wooden_door_r",
    "gabz_hsbc_door_01_l",
    "gabz_hsbc_door_01_r",
    "gabz_hsbc_door_02_l",
    "gabz_hsbc_door_02_r",
    "gabz_lost_mc_bat_doors_01",
    "gabz_lost_mc_bat_doors_02",
    "gabz_lost_mc_door_01",
    "gabz_lost_mc_door_02",
    "gabz_lost_mc_door_03",
    "gabz_lost_mc_int_door_01",
    "gabz_lost_mc_int_door_02",
    "gabz_lost_mc_int_door_03",
    "gabz_lost_mc_int_door_04",
    "gabz_lost_mc_int_door_05",
    "gabz_lost_mc_int_door_06",
    "gabz_lost_mc_meet_doors",
    "gabz_lost_mc_meet_door_l",
    "gabz_lost_mc_meet_door_r",
    "gabz_lost_mc_office_doors",
    "gabz_lost_mc_office_door_l",
    "gabz_lost_mc_office_door_r",
    "gabz_mabanks_door_big_r",
    "gabz_mabanks_door_big_l",
    "gabz_mabanks_door_01",
    "gabz_mabanks_door_02",
    "gabz_mabanks_door_03",
    "gabz_mabanks_door_04",
    "gabz_madrazos_door",
    "gabz_madrazos_door_ext",
    "gabz_madrazos_front_door_r",
    "gabz_madrazos_main_door",
    "gabz_madrazos_toilet_door",
    "gabz_mechanic_door_l",
    "gabz_mechanic_door_r",
    "gabz_mechanic_door_roller",
    "gabz_mechanic_office_door_01",
    "gabz_mechanic_office_door_02",
    "gabz_mechanic_roller_door",
    "gabz_mechanic_toilet_door",
    "gabz_pawnbroker_back_door",
    "gabz_pawnbroker_main_door",
    "gabz_tuner_door_01_l",
    "gabz_tuner_door_01_r",
    "gabz_tuner_door_02_l",
    "gabz_tuner_door_02_r",
    "gabz_tuner_door_shop_01",
    "gabz_tuner_door_shop_02",
    "gabz_vshop_ext_doors_l",
    "gabz_vshop_ext_doors_r",
    "gabz_vshop_ext_door_roller",
    "gabz_vshop_int_doors_01",
    "gabz_vshop_int_doors_02",
    "gabz_vshop_int_doors_03",
    "gabz_vshop_showroom_door_01",
    "gabz_vshop_showroom_door_02",
    "gabz_vshop_toilet_door_01",
    "gabz_vshop_toilet_door_02",
    "gabz_vshop_warehouse_door_01",
    "gabz_vshop_warehouse_door_02"
}
