fx_version "cerulean"
game "gta5"

author "PoliceMP - smallp13"
description "Elec Nozzle"
version "1.0.0"

shared_script "config.lua"

client_scripts {
    "electric.lua"
}

server_scripts {
    "server.lua"
}

ui_page 'html/ui.html'

files {
    'html/ui.html',
    'html/sounds/*.ogg',
}