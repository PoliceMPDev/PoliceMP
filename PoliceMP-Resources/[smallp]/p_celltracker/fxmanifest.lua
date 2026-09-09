fx_version 'cerulean'
game 'gta5'
lua54 'yes'

author 'PoliceMP - smallp13'
description 'Standalone Cell Trace Script'
version '1.0.0'

client_scripts {
    'client.lua'
}

server_scripts {
    'server.lua'
}

shared_script '@ox_lib/init.lua'

dependencies {
    'ox_lib',
    'nearest-postal'
}
