fx_version 'bodacious'
game 'gta5'

author 'London Studios'
description 'Setup and control dynamic traffic signs'
version '1.0.0'
lua54 'yes'

client_scripts {
    'cl_utils.lua',
    'cl_smartsigns.lua',
}

shared_script 'config.lua'

server_scripts {
    -- "@vrp/lib/utils.lua",
    'sv_smartsigns.lua',
}

files {
    'stream/data/*.ytyp',
    'locations.json',
}

escrow_ignore {
    'stream/*',
    'stream/data/*',
    'config.lua',
    'cl_utils.lua',
    'sv_smartsigns.lua',
    'locations.json',
}

data_file 'DLC_ITYP_REQUEST' 'stream/data/*.ytyp'
dependency '/assetpacks'