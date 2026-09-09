fx_version "cerulean"
game "gta5"

author "PoliceMP - smallp13"
description "Fuel Nozzle"
version "1.0.0"

shared_script "config.lua"

client_scripts {
    "client.lua"
}

server_scripts {
    "server.lua"
}

ui_page 'html/ui.html'

files {
    'html/ui.html',
    'html/sounds/*.ogg',
}