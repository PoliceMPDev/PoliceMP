fx_version 'cerulean'
games { 'gta5' }
lua54 'yes'

client_scripts {
	"config.lua",
	'client.lua',

}
server_scripts{
	'server.lua'
}

escrow_ignore "config.lua"


dependency '/assetpacks'