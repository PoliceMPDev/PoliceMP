fx_version "cerulean"
game "gta5"

author "PoliceMP - smallp13"
version "1.0.0"
description "Stretcher Script"

client_scripts {
	'config.lua',
  	'client/main.lua',
	'client/main2.lua'
}

server_scripts {
	'server/main.lua'
}

files {
	'data/vehicles.meta',
	'data/carvariations.meta',
}

data_file 'VEHICLE_METADATA_FILE' 'data/vehicles.meta'
data_file 'VEHICLE_VARIATION_FILE' 'data/carvariations.meta'