fx_version 'adamant'
games {'gta5'}

file 'gusepe_timecycle_mods_1.xml'
data_file 'TIMECYCLEMOD_FILE' 'gusepe_timecycle_mods_1.xml'

client_scripts {
	'lift/client.lua'
}

data_file('DLC_ITYP_REQUEST')('stream/v2_sheriff_props.ytyp')
data_file('DLC_ITYP_REQUEST')('stream/int_sheriff_main.ytyp')
data_file('DLC_ITYP_REQUEST')('stream/int_sheriff_stairs2.ytyp')
data_file('DLC_ITYP_REQUEST')('stream/int_sheriff_second.ytyp')
data_file('DLC_ITYP_REQUEST')('stream/int_sheriff_stairs1.ytyp')
data_file('DLC_ITYP_REQUEST')('stream/int_sheriff_first.ytyp')
data_file('DLC_ITYP_REQUEST')('stream/Speedhumps/speed_hump.ytyp')
data_file('DLC_ITYP_REQUEST')('stream/Speedhumps/emergonly.ytyp')
data_file('DLC_ITYP_REQUEST')('stream/VLevDoor/v_lev_doors.ytyp')
data_file('DLC_ITYP_REQUEST')('stream/zonah_helipad/helipad_zonah.ytyp')

client_script 'client.lua'

--data_file('DLC_ITYP_REQUEST')('stream/v_int_40.ytyp')

data_file 'DLC_ITYP_REQUEST' 'stream/RoadSigns/v_signs.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/AFOShoothouse/afotra.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/aa_sign.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/PMPSign/pmp_sign.ytyp'

--data_file 'DLC_ITYP_REQUEST' 'stream/LegionSquareMemorial/pmp_centoph.ytyp'

client_script 'map.lua'

this_is_a_map 'yes'
