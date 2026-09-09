fx_version 'cerulean'
games { 'gta5' }


author 'PoliceMP - smallp13'

files {
    'html/index.html',
    'html/sounds/zip.ogg',
    'html/sounds/unzip.ogg'
}
ui_page 'html/index.html'

shared_scripts {
    'config.lua'
}

client_scripts {
    'client/client.lua',
    'config.lua'
}

server_scripts {
    'server/server.lua'
}