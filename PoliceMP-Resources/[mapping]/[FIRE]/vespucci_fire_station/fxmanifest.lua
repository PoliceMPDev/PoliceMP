lua54 'yes'
fx_version 'cerulean'

game 'gta5'

author 'Apollo developments'
description 'FIRE STATION VESPUCCI'
version '1.0.0'

this_is_a_map 'yes'

file 'cfa_vas_manifest.ymt'

data_file 'SCENARIO_POINTS_OVERRIDE_FILE' 'cfa_vas_manifest.ymt'


data_file 'INTERIOR_PROXY_ORDER_FILE' 'interiorproxies.meta'

files {
	'interiorproxies.meta'
}


escrow_ignore {
    'stream/*.ytd',
   }

dependency '/assetpacks'