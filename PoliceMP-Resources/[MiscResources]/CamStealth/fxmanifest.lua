fx_version 'cerulean'
game 'gta5'

author 'PoliceMP'
description 'Camera and Stealth Mode'
version '1.0.0'

client_scripts {
    'client.lua',
    'disableroll.lua',
    'afktrigger.lua'   
}

server_scripts {
    'server.lua'
}

lua54 'yes'
shared_script '@ox_lib/init.lua'
