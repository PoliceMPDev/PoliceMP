fx_version 'bodacious'
games { 'gta5' }

author 'London Studios'
description 'Setup and control water monitors'
version '1.0.0'
lua54 'yes'

files {
    'stream/watermonitor.ytyp',
}

client_scripts {
    'cl_utils.lua',
    'cl_watermonitor.lua',
}

shared_script '@es_extended/imports.lua'

server_scripts {
    -- "@vrp/lib/utils.lua",
    'sv_watermonitor.lua',
}

escrow_ignore {
    'stream/**',
    'stream/watermonitor.ytyp',
    'cl_utils.lua',
    'sv_watermonitor.lua',
    'config.lua',
}

shared_script 'config.lua'

data_file 'DLC_ITYP_REQUEST' 'stream/watermonitor.ytyp'
dependency '/assetpacks'
