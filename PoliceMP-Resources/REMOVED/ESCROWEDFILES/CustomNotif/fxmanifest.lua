fx_version "cerulean"

description "SetFreeroam"
author "Sertinox"
version '1.0.0'

lua54 'yes'

games {
  "gta5",
}

ui_page 'web/build/index.html'

client_script {'client.lua', 'utils.lua'}
server_script 'server.lua'
escrow_ignore 'client.lua'

files {
	'web/build/index.html',
	'web/build/**/*',
}
dependency '/assetpacks'