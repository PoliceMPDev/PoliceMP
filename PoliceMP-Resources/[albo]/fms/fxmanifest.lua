--By Albo1125

-- Configure these

allowedLiveMapIDs '1,2' -- Comma separated IDs of the Maps for which the live map will be updated using this server.
SmartSignsIntegration 'true' -- Set this to 'false' to disable the FMS SmartSigns integration
AutomaticSmartFiresIntegration 'true' -- Set this to 'false' to disable the FMS CAD creation integration for automatic SmartFires

-- No need to touch the below

websiteurl 'https://pmp.forcemanagementsystem.com/'
websitetoken 'S8Tkk843AQhhVusObEohv1nrpJFz7QtOkpS'

fx_version 'cerulean'
games { 'gta5' }

dependencies {
	'pNotify'
}

ui_page "nui/ui.html"

files {
	'package.json',
	"nui/ui.html",
	"nui/css/style.css",
	"nui/css/darktheme.css"
}

server_scripts {
	'fmsinteraction.js'
}

client_scripts {
	'tabclient.lua',
	'fms.net.dll'
}