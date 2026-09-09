fx_version 'cerulean'
game 'gta5'

author 'Tugamars'
description 'ALPR System.'
version '1.0.2'

ui_page 'client/nui/alpr/index.html'

shared_script 'config.lua'
shared_script 'alprs.json'

client_scripts {
	'@PolyZone/client.lua',
	'@PolyZone/CircleZone.lua',
	'client/**/*.lua'
}
server_script {
	'server/**/*.lua'
}

files{
	'client/nui/**/*.html',
	'client/nui/**/*.png',
	'client/nui/**/*.css',
	'client/nui/**/*.js',
	'client/nui/**/*.json',
}

escrow_ignore {
	'config.lua',
	'server/framework.lua',
	'client/target.lua',
}

lua54 'yes'



dependency '/assetpacks'