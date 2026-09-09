fx_version "adamant"
game "gta5"
lua54 'yes'

name "mumble-voip"
description "A tokovoip replacement that uses fivems mumble voip"
author "Frazzle (frazzle9999@gmail.com)"
version "1.3"

ui_page "ui/index.html"

files {
	"ui/index.html",
	"ui/mic_click_on.ogg",
	"ui/mic_click_off.ogg",
}

shared_scripts {
	"config.lua",
	"grid.lua",
}

escrow_ignore {
    'client.lua',
    'config.lua',
	'grid.lua',
	'server.lua'
}

client_scripts {
	"client.lua",
}

server_scripts {
	"server.lua",
}

provide "tokovoip_script"
dependency '/assetpacks'
dependency '/assetpacks'