fx_version 'bodacious'

games { 'gta5' }

author 'London Studios'
description 'Adds fire supply lines, integrating with HoseLS'
version '1.0.0'
lua54 'yes'

files {
    'stream/supplyline.ytyp',
}

client_scripts {
    'cl_utils.lua',
    'cl_supplyline.lua',
}

shared_script '@es_extended/imports.lua'

server_scripts {
    -- "@vrp/lib/utils.lua",
    'sv_supplyline.lua',
}

escrow_ignore {
    'stream/**',
    'sv_supplyline.lua',
    'config.lua',
    'cl_utils.lua',
}

shared_script 'config.lua'

data_file 'DLC_ITYP_REQUEST' 'stream/supplyline.ytyp'

-- Join the London Studios Discord Server here: https://discord.gg/F2zmUTD
-- Ensure you have the latest version of HoseLS Downloaded
dependency '/assetpacks'
