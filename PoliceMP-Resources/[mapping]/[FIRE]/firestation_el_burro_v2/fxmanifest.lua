lua54 'yes'
fx_version 'cerulean'

game 'gta5'
this_is_a_map 'yes'

client_script 'client.lua'


data_file 'AUDIO_GAMEDATA' 'stream/occlusions/game.dat' -- dat151


files {
'stream/occlusions/game.dat151.rel',
}

author 'Apollo Developments'
description 'El Burro Fire Station'
version '1.0.0'

escrow_ignore {
    'client.lua',
    'stream/ytd/*.ytd',
    'stream/ydd/*.ydd',
    'stream/ydr/*.ydr',
    'stream/occlusions/*.ymt',
    'stream/occlusions/*.rel',
    'stream/yft/*.yft',
}
dependency '/assetpacks'