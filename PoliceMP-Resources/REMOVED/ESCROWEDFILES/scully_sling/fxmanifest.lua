-- Created by Scully | https://discord.gg/scully
fx_version 'cerulean'
lua54 'yes'
game 'gta5'

author 'https://discord.gg/scully'
description 'very noice weapon sling'
version '3.1.0'

shared_scripts {
	'config.lua',
	'weapons.lua',
    'components.lua'
}

client_scripts {
	'utils.lua',
	'client/main.lua',
	'client/bridge/*.lua'
}

server_script 'server/main.lua'

ui_page 'ui/ui.html'

files {
    'ui/ui.html',
    'ui/*.js',
    'ui/*.css'
}

escrow_ignore {
	'config.lua',
	'weapons.lua',
    'components.lua'
}
dependency '/assetpacks'