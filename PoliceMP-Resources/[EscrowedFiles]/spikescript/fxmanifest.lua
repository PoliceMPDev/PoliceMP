fx_version 'bodacious'
games {'gta5'}

-- Resource stuff
name 'Spike Script V2'
description 'Realistic Spike Script By ToxicScripts'
version 'v1'
author 'Toxic Scripts'

lua54 'yes'

data_file "DLC_ITYP_REQUEST" "stream/strip_prop.ytyp"

-- Adds additional logging, useful when debugging issues.
client_debug_mode 'false'
server_debug_mode 'false'

-- Leave this set to '0' to prevent compatibility issues 
-- and to keep the save files your users.
experimental_features_enabled '0'

files {
    'settings.ini',
}

-- Files & scripts
client_scripts {
    '@PolyZone/client.lua',
    '@PolyZone/BoxZone.lua',
    '@PolyZone/EntityZone.lua',
    'SpikeScriptV2.net.dll',
    'client/client.lua',
}

-- Files & scripts
shared_script 'config.lua'

server_scripts {
    'server/server.lua'
}

escrow_ignore {
    'config.lua',
    'settings.ini',
    'README.md'
}
dependency '/assetpacks'