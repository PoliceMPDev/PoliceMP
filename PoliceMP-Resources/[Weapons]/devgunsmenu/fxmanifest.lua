fx_version 'cerulean'
game 'gta5'

author '3devs.co.uk'
description 'Weapons menu with Ace perms'
version '1.0.0'

client_scripts {
    '@NativeUI/NativeUI.lua',
    'config.lua', 
    'client/client.lua'   
}

server_scripts {
    'server/server.lua'   
}

files {
    'weapons.json',  
    'config.lua'     
}
