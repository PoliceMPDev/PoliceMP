fx_version 'bodacious'
games { 'gta5' }

author 'London Studios'
description 'A resource providing four police shields'
version '1.0.0'
lua54 'yes'

files {
    'stream/bv_shields.ytyp',
    'stream/shieldprops.ytyp',
}

client_scripts {
    'cl_utils.lua',
    'cl_policeshields.lua',
}

server_scripts {
    -- "@vrp/lib/utils.lua",
    'sv_policeshields.lua',
}

escrow_ignore {
    'config.lua',
    'stream/**',
    'sv_policeshields.lua',
    'cl_utils.lua',
}

shared_script 'config.lua'

data_file 'DLC_ITYP_REQUEST' 'stream/shieldprops.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/bv_shields.ytyp'
dependency '/assetpacks'