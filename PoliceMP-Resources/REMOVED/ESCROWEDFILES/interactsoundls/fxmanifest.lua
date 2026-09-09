fx_version 'bodacious'
game 'gta5'
lua54 'yes'


author 'London Studios'
description 'PlayCustomSounds'
version '1.0.0'

client_script {
    'Client.net.dll',
    "main.lua"
}
server_script 'Server.net.dll'

ui_page 'html/index.html'

files {
    'html/index.html',
    
    -- Sound files must be in html/sounds/
    -- Sound files must be in .ogg format
    'html/sounds/*.ogg',
    'html/sounds/CMA/*.ogg'
}

dependency '/assetpacks'