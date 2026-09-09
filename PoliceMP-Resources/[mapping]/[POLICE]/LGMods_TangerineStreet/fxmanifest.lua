fx_version { 'cerulean' }
games { 'gta5' }
lua54 { 'yes' }

name { 'LGMods_TangerineStreet' }
author { 'LGMods - https://discord.gg/lgmods' }
version('v1.0.1')

this_is_a_map 'yes'

data_file 'INTERIOR_PROXY_ORDER_FILE' 'interiorproxies.meta'
data_file 'DLC_ITYP_REQUEST' 'stream/lgmods_tangstreet.ytyp'

ui_page "html/index.html"

client_scripts {
    "Config.lua",
    "Client.lua",
    "Client_Target.lua"
}

server_scripts {
    "Server.lua"
}

files {
    "interiorproxies.meta",
    'stream/lgmods_tangstreet.ytyp',
    "html/index.html",
    "html/sounds/*.ogg"
}

escrow_ignore { 'Config.lua', 'stream/*.ytd' };
dependency '/assetpacks'

dependency '/assetpacks'