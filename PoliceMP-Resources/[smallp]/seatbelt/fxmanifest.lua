fx_version 'cerulean'
games { 'gta5' }

author 'PoliceMP - smallp13'
description 'Seatbelt'
version '1.0.0'

client_scripts {
    'config.lua',
    'client/main.lua',
}

server_scripts {
    'config.lua',
    'server/main.lua',
}

ui_page "client/html/index.html"

files {
    'client/html/index.html',
    'client/html/buckle.ogg',
    'client/html/unbuckle.ogg',
}

-- Note: "setr game_enableFlyThroughWindscreen true" in your server cfg