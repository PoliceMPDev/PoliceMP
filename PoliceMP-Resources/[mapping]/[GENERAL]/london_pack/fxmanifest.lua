fx_version 'cerulean'
game 'gta5'

author 'BABANI STORE'
version '3.0.0'

lua54 'yes'
this_is_a_map 'yes'


escrow_ignore {
    'config.lua'
}

dependency '/assetpacks'


data_file 'INTERIOR_PROXY_ORDER_FILE' 'interiorproxies.meta'
replace_level_meta 'gta5'

files {
    'gta5.meta'
}

shared_scripts {
    'config.lua'
}

ui_page 'html/index.html'

files {
    'html/index.html',
    'html/script.js',
    'html/style.css'
}


client_scripts {
    'client.lua'
}

server_scripts {
    'server.lua'
}

dependency '/assetpacks'
dependency '/assetpacks'