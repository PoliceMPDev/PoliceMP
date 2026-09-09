fx_version 'bodacious'

games { 'gta5' }

author 'London Studios'
description 'A resource providing a realistic EOD experience'
version '1.0.0'
lua54 'yes'

files {
   'data/vehicles.meta', 
   'data/carvariations.meta',
}

shared_script 'config.lua'

client_scripts {
	'cl_utils.lua',
	'cl_eod.lua',
}

server_scripts {
	-- "@vrp/lib/utils.lua",
	'sv_eod.lua',
}

escrow_ignore {
	'stream/**',
	'data/**',
	'cl_utils.lua',
	'config.lua',
	'sv_eod.lua',
}

data_file 'VEHICLE_METADATA_FILE' 'data/vehicles.meta'
data_file 'VEHICLE_VARIATION_FILE' 'data/carvariations.meta'

-- Join the London Studios Discord server here: https://discord.gg/htyaZNaG
dependency '/assetpacks'