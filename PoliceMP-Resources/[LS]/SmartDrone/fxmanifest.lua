fx_version 'bodacious'

games { 'gta5' }

author 'London Studios'
description 'Smart Drone v2'
version '2.0.0'
lua54 'yes'

client_scripts {
	'config.lua',
	'cl_utils.lua',
	'cl_drone.lua',
}

server_scripts {
	'config.lua',
	'sv_utils.lua',
	'sv_drone.lua',
}

escrow_ignore {
	'cl_utils.lua',
	'sv_utils.lua',
	'config.lua',
	'stream/*.ytd',
	'data/**/handling.meta',
	'data/**/carvariations.meta',
	'data/**/vehicles.meta',
	'stream/*.ytyp',
}

files {
    'data/**/vehicles.meta',
    'data/**/carvariations.meta',
	'data/**/handling.meta',
	'stream/*.ytyp',
}

data_file 'VEHICLE_METADATA_FILE' 'data/**/vehicles.meta'
data_file 'VEHICLE_VARIATION_FILE' 'data/**/carvariations.meta'
data_file 'HANDLING_FILE' 'data/**/handling.meta'
data_file 'DLC_ITYP_REQUEST' 'stream/*.ytyp'

-- Join the London Studios Discord server here: https://discord.gg/htyaZNaG
dependency '/assetpacks'