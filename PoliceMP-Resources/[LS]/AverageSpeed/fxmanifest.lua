fx_version 'bodacious'
games { 'gta5' }

author 'London Studios'
description 'A resource providing an average speed camera system'
version '1.0.0'
lua54 'yes'

files {
     'stream/prop_speed_camera.ytyp'
}

shared_script 'config.lua'
shared_script '@es_extended/imports.lua'

client_scripts {
    'cl_utils.lua',
    'cl_averagespeed.lua',
}

server_scripts {
    -- "@vrp/lib/utils.lua",
    'sv_averagespeed.lua',
}

escrow_ignore {
    'stream/**',
    'cl_utils.lua',
    'config.lua',
    'sv_averagespeed.lua',
}

data_file 'DLC_ITYP_REQUEST' 'stream/prop_speed_camera.ytyp'
dependency '/assetpacks'
