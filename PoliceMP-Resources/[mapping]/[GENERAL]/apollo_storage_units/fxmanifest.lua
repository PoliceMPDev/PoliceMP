fx_version 'cerulean'
game 'gta5'
lua54 'yes'

author 'apollo'

description 'Storage units'

version '1.0.0'

client_script 'client.lua'

escrow_ignore {
    'client.lua',
    'stream/ytd/*.ytd',
    'stream/unlocked/*.ydr',
    'stream/unlocked/*.ydd',
}

dependency '/assetpacks'