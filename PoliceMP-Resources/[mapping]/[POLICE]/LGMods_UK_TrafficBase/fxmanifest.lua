description 'LGMods MPS RTPC Traffic Base by Lewie Gee'
name 'LGMods MPS RTPC Traffic Base by Lewie Gee'

fx_version "cerulean"
game "gta5"

files {
    'stream/rtpcprops.ytyp',
	'stream/lg_traffic_base.ytyp',
	'stream/noose_lewie_gee_ytyp.ytyp',
}

data_file 'DLC_ITYP_REQUEST' 'stream/rtpcprops.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/noose_lewie_gee_ytyp.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/lg_traffic_base.ytyp'

escrow_ignore {
    'stream/lgmods_trafficbase.ytd',
}


this_is_a_map 'yes'
dependency '/assetpacks'