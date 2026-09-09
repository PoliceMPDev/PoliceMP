fx_version 'cerulean'
game 'gta5'

author 'PoliceMP - smallp13'
description 'Mechanic and Towing for ME Recovery'
version '1.0.0'

server_scripts {
    'server.lua'
}

client_scripts {
    '@NativeUI/NativeUI.lua',
    'client.lua',
    'towing/client.lua',
    'towing/config.lua'
}

dependency "NativeUI"