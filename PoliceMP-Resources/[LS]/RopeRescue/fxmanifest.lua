fx_version 'bodacious'
games { 'gta5' }

author 'London Studios'
description 'Adds a realistic rope rescue & basket experience'
version '1.0.0'
lua54 'yes'

files {
    'stream/roperescue.ytyp',
}

client_scripts {
    'cl_utils.lua',
    'cl_roperescue.lua',
}

server_scripts {
    -- "@vrp/lib/utils.lua",
    'sv_roperescue.lua',
}

escrow_ignore {
    'stream/**',
    'cl_utils.lua',
    'config.lua',
    'sv_roperescue.lua',
}

shared_script 'config.lua'

data_file 'DLC_ITYP_REQUEST' 'stream/roperescue.ytyp'
dependency '/assetpacks'