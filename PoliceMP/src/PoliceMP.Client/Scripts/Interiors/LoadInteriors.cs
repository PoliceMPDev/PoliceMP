using System.Threading.Tasks;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.Interiors
{
    public class LoadInteriors : Script
    {
        protected override Task OnStartAsync()
        {
            LoadAllInteriors();
            return Task.FromResult(0);
        }

        private void LoadAllInteriors()
        {
            API.RequestIpl("ex_dt1_02_office_02b");
            var locationOne = API.GetInteriorAtCoords(-141.1987f, -620.913f, 168.8205f);
            API.PinInteriorInMemory(locationOne);API.RequestIpl("chop_props");
            API.RequestIpl("FIBlobby");
            API.RemoveIpl("FIBlobbyfake");
            API.RequestIpl("FBI_colPLUG");
            API.RequestIpl("FBI_repair");
            API.RequestIpl("v_tunnel_hole");
            API.RequestIpl("TrevorsMP");
            API.RequestIpl("TrevorsTrailer");
            API.RequestIpl("TrevorsTrailerTidy");
            API.RemoveIpl("farm_burnt");
            API.RemoveIpl("farm_burnt_lod");
            API.RemoveIpl("farm_burnt_props");
            API.RemoveIpl("farmint_cap");
            API.RemoveIpl("farmint_cap_lod");
            API.RequestIpl("farm");
            API.RequestIpl("farmint");
            API.RequestIpl("farm_lod");
            API.RequestIpl("farm_props");
            API.RequestIpl("facelobby");
            API.RemoveIpl("CS1_02_cf_offmission");
            API.RequestIpl("CS1_02_cf_onmission1");
            API.RequestIpl("CS1_02_cf_onmission2");
            API.RequestIpl("CS1_02_cf_onmission3");
            API.RequestIpl("CS1_02_cf_onmission4");
            API.RequestIpl("v_rockclub");
            API.RequestIpl("v_janitor");
            API.RemoveIpl("hei_bi_hw1_13_door");
            API.RequestIpl("bkr_bi_hw1_13_int");
            API.RequestIpl("ufo");
            API.RequestIpl("ufo_lod");
            API.RequestIpl("ufo_eye");
            API.RemoveIpl("v_carshowroom");
            API.RemoveIpl("shutter_open");
            API.RemoveIpl("shutter_closed");
            API.RemoveIpl("shr_int");
            API.RequestIpl("csr_afterMission");
            API.RequestIpl("v_carshowroom");
            API.RequestIpl("shr_int");
            API.RequestIpl("shutter_closed");
            API.RequestIpl("smboat");
            API.RequestIpl("smboat_distantlights");
            API.RequestIpl("smboat_lod");
            API.RequestIpl("smboat_lodlights");
            API.RequestIpl("cargoship");
            API.RequestIpl("railing_start");
            API.RemoveIpl("sp1_10_fake_interior");
            API.RemoveIpl("sp1_10_fake_interior_lod");
            API.RequestIpl("sp1_10_real_interior");
            API.RequestIpl("sp1_10_real_interior_lod");
            API.RemoveIpl("id2_14_during_door");
            API.RemoveIpl("id2_14_during1");
            API.RemoveIpl("id2_14_during2");
            API.RemoveIpl("id2_14_on_fire");
            API.RemoveIpl("id2_14_post_no_int");
            API.RemoveIpl("id2_14_pre_no_int");
            API.RemoveIpl("id2_14_during_door");
            API.RequestIpl("id2_14_during1");
            API.RemoveIpl("Coroner_Int_off");
            API.RequestIpl("coronertrash");
            API.RequestIpl("Coroner_Int_on");
            API.RemoveIpl("bh1_16_refurb");
            API.RemoveIpl("jewel2fake");
            API.RemoveIpl("bh1_16_doors_shut");
            API.RequestIpl("refit_unload");
            API.RequestIpl("post_hiest_unload");
            API.RequestIpl("Carwash_with_spinners");
            API.RequestIpl("KT_CarWash");
            API.RequestIpl("ferris_finale_Anim");
            API.RemoveIpl("ch1_02_closed");
            API.RequestIpl("ch1_02_open");
            API.RequestIpl("AP1_04_TriAf01");
            API.RequestIpl("CS2_06_TriAf02");
            API.RequestIpl("CS4_04_TriAf03");
            API.RemoveIpl("scafstartimap");
            API.RequestIpl("scafendimap");
            API.RemoveIpl("DT1_05_HC_REMOVE");
            API.RequestIpl("DT1_05_HC_REQ");
            API.RequestIpl("DT1_05_REQUEST");
            API.RequestIpl("dt1_05_hc_remove");
            API.RequestIpl("dt1_05_hc_remove_lod");
            API.RequestIpl("FINBANK");
            API.RemoveIpl("DT1_03_Shutter");
            API.RemoveIpl("DT1_03_Gr_Closed");
            API.RequestIpl("golfflags");
            API.RequestIpl("airfield");
            API.RequestIpl("v_garages");
            API.RequestIpl("v_foundry");
            //API.RequestIpl("hei_yacht_heist");
            //API.RequestIpl("hei_yacht_heist_Bar");
            //API.RequestIpl("hei_yacht_heist_Bedrm");
            //API.RequestIpl("hei_yacht_heist_Bridge");
            //API.RequestIpl("hei_yacht_heist_DistantLights");
            //API.RequestIpl("hei_yacht_heist_enginrm");
            //API.RequestIpl("hei_yacht_heist_LODLights");
            //API.RequestIpl("hei_yacht_heist_Lounge");
            API.RequestIpl("hei_carrier");
            API.RequestIpl("hei_Carrier_int1");
            API.RequestIpl("hei_Carrier_int2");
            API.RequestIpl("hei_Carrier_int3");
            API.RequestIpl("hei_Carrier_int4");
            API.RequestIpl("hei_Carrier_int5");
            API.RequestIpl("hei_Carrier_int6");
            API.RequestIpl("hei_carrier_LODLights");
            API.RequestIpl("bkr_bi_id1_23_door");
            API.RequestIpl("lr_cs6_08_grave_closed");
            API.RequestIpl("hei_sm_16_interior_v_bahama_milo_");
            API.RequestIpl("CS3_07_MPGates");
            API.RequestIpl("cs5_4_trains");
            API.RequestIpl("v_lesters");
            API.RequestIpl("v_trevors");
            API.RequestIpl("v_michael");
            API.RequestIpl("v_comedy");
            API.RequestIpl("v_cinema");
            API.RequestIpl("V_Sweat");
            API.RequestIpl("V_35_Fireman");
            API.RequestIpl("redCarpet");
            API.RequestIpl("triathlon2_VBprops");
            API.RequestIpl("jetsteAPIurnel");
            API.RequestIpl("Jetsteal_ipl_grp1");
            API.RequestIpl("v_hospital");
            API.RequestIpl("canyonriver01");
            API.RequestIpl("canyonriver01_lod");
            API.RequestIpl("cs3_05_water_grp1");
            API.RequestIpl("cs3_05_water_grp1_lod");
            API.RequestIpl("trv1_trail_start");
            API.RequestIpl("CanyonRvrShallow");

            API.RequestIpl("bh1_47_joshhse_unburnt");
            API.RequestIpl("bh1_47_joshhse_unburnt_lod");

            // CASINO

            API.RequestIpl("hei_dlc_windows_casino");
            API.RequestIpl("hei_dlc_casino_door");
            API.RequestIpl("vw_dlc_casino_door");
            API.RequestIpl("hei_dlc_casino_aircon");

            API.RequestIpl("vw_casino_penthouse");
            API.RequestIpl("vw_casino_main");

            // Cayo Perico Island

            API.RequestIpl("h4_islandairstrip");
            API.RequestIpl("h4_islandairstrip_props");
            API.RequestIpl("h4_islandx_mansion");
            API.RequestIpl("h4_islandx_mansion_props");
            API.RequestIpl("h4_islandx_props");
            API.RequestIpl("h4_islandxdock");
            API.RequestIpl("h4_islandxdock_props");
            API.RequestIpl("h4_islandxdock_props_2");
            API.RequestIpl("h4_islandxtower");
            API.RequestIpl("h4_islandx_maindock");
            API.RequestIpl("h4_islandx_maindock_props");
            API.RequestIpl("h4_islandx_maindock_props_2");
            API.RequestIpl("h4_IslandX_Mansion_Vault");
            API.RequestIpl("h4_islandairstrip_propsb");
            API.RequestIpl("h4_beach");
            API.RequestIpl("h4_beach_props");
            API.RequestIpl("h4_beach_bar_props");
            API.RequestIpl("h4_islandx_barrack_props");
            API.RequestIpl("h4_islandx_checkpoint");
            API.RequestIpl("h4_islandx_checkpoint_props");
            API.RequestIpl("h4_islandx_Mansion_Office");
            API.RequestIpl("h4_islandx_Mansion_LockUp_01");
            API.RequestIpl("h4_islandx_Mansion_LockUp_02");
            API.RequestIpl("h4_islandx_Mansion_LockUp_03");
            API.RequestIpl("h4_islandairstrip_hangar_props");
            API.RequestIpl("h4_IslandX_Mansion_B");
            API.RequestIpl("h4_islandairstrip_doorsclosed");
            API.RequestIpl("h4_Underwater_Gate_Closed");
            API.RequestIpl("h4_mansion_gate_closed");
            API.RequestIpl("h4_aa_guns");
            API.RequestIpl("h4_IslandX_Mansion_GuardFence");
            API.RequestIpl("h4_IslandX_Mansion_Entrance_Fence");
            API.RequestIpl("h4_IslandX_Mansion_B_Side_Fence");
            API.RequestIpl("h4_IslandX_Mansion_Lights");
            API.RequestIpl("h4_islandxcanal_props");
            API.RequestIpl("h4_beach_props_party");
            API.RequestIpl("h4_islandX_Terrain_props_06_a");
            API.RequestIpl("h4_islandX_Terrain_props_06_b");
            API.RequestIpl("h4_islandX_Terrain_props_06_c");
            API.RequestIpl("h4_islandX_Terrain_props_05_a");
            API.RequestIpl("h4_islandX_Terrain_props_05_b");
            API.RequestIpl("h4_islandX_Terrain_props_05_c");
            API.RequestIpl("h4_islandX_Terrain_props_05_d");
            API.RequestIpl("h4_islandX_Terrain_props_05_e");
            API.RequestIpl("h4_islandX_Terrain_props_05_f");
            API.RequestIpl("H4_islandx_terrain_01");
            API.RequestIpl("H4_islandx_terrain_02");
            API.RequestIpl("H4_islandx_terrain_03");
            API.RequestIpl("H4_islandx_terrain_04");
            API.RequestIpl("H4_islandx_terrain_05");
            API.RequestIpl("H4_islandx_terrain_06");
            API.RequestIpl("h4_ne_ipl_00");
            API.RequestIpl("h4_ne_ipl_01");
            API.RequestIpl("h4_ne_ipl_02");
            API.RequestIpl("h4_ne_ipl_03");
            API.RequestIpl("h4_ne_ipl_04");
            API.RequestIpl("h4_ne_ipl_05");
            API.RequestIpl("h4_ne_ipl_06");
            API.RequestIpl("h4_ne_ipl_07");
            API.RequestIpl("h4_ne_ipl_08");
            API.RequestIpl("h4_ne_ipl_09");
            API.RequestIpl("h4_nw_ipl_00");
            API.RequestIpl("h4_nw_ipl_01");
            API.RequestIpl("h4_nw_ipl_02");
            API.RequestIpl("h4_nw_ipl_03");
            API.RequestIpl("h4_nw_ipl_04");
            API.RequestIpl("h4_nw_ipl_05");
            API.RequestIpl("h4_nw_ipl_06");
            API.RequestIpl("h4_nw_ipl_07");
            API.RequestIpl("h4_nw_ipl_08");
            API.RequestIpl("h4_nw_ipl_09");
            API.RequestIpl("h4_se_ipl_00");
            API.RequestIpl("h4_se_ipl_01");
            API.RequestIpl("h4_se_ipl_02");
            API.RequestIpl("h4_se_ipl_03");
            API.RequestIpl("h4_se_ipl_04");
            API.RequestIpl("h4_se_ipl_05");
            API.RequestIpl("h4_se_ipl_06");
            API.RequestIpl("h4_se_ipl_07");
            API.RequestIpl("h4_se_ipl_08");
            API.RequestIpl("h4_se_ipl_09");
            API.RequestIpl("h4_sw_ipl_00");
            API.RequestIpl("h4_sw_ipl_01");
            API.RequestIpl("h4_sw_ipl_02");
            API.RequestIpl("h4_sw_ipl_03");
            API.RequestIpl("h4_sw_ipl_04");
            API.RequestIpl("h4_sw_ipl_05");
            API.RequestIpl("h4_sw_ipl_06");
            API.RequestIpl("h4_sw_ipl_07");
            API.RequestIpl("h4_sw_ipl_08");
            API.RequestIpl("h4_sw_ipl_09");
            API.RequestIpl("h4_islandx_mansion");
            API.RequestIpl("h4_islandxtower_veg");
            API.RequestIpl("h4_islandx_sea_mines");
            API.RequestIpl("h4_islandx");
            API.RequestIpl("h4_islandx_barrack_hatch");
            API.RequestIpl("h4_islandxdock_water_hatch");
            API.RequestIpl("h4_beach_party");
            API.RequestIpl("h4_mph4_terrain_01_grass_0");
            API.RequestIpl("h4_mph4_terrain_01_grass_1");
            API.RequestIpl("h4_mph4_terrain_02_grass_0");
            API.RequestIpl("h4_mph4_terrain_02_grass_1");
            API.RequestIpl("h4_mph4_terrain_02_grass_2");
            API.RequestIpl("h4_mph4_terrain_02_grass_3");
            API.RequestIpl("h4_mph4_terrain_04_grass_0");
            API.RequestIpl("h4_mph4_terrain_04_grass_1");
            API.RequestIpl("h4_mph4_terrain_04_grass_2");
            API.RequestIpl("h4_mph4_terrain_04_grass_3");
            API.RequestIpl("h4_mph4_terrain_05_grass_0");
            API.RequestIpl("h4_mph4_terrain_06_grass_0");

            var casinoManagerInterior = API.GetInteriorAtCoords(1100.000f, 220.000f, -50.000f);

            if (API.IsValidInterior(casinoManagerInterior))
            {
                API.ActivateInteriorEntitySet(casinoManagerInterior, "casino_manager_default");
                API.RefreshInterior(casinoManagerInterior);
            }

            var interiorLocationTwo = API.GetInteriorAtCoords(1100.0f, 220.0f, -50.0f);

            if (API.IsValidInterior(interiorLocationTwo))
            {
                API.ActivateInteriorEntitySet(casinoManagerInterior, "0x30240D11");
                API.ActivateInteriorEntitySet(casinoManagerInterior, "0xA3C89BB2");
                API.RefreshInterior(interiorLocationTwo);
            }
        }
    }
}