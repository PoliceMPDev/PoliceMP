fx_version 'cerulean'
games { 'gta5' }
lua54 'yes'

ui_page "html/index.html"
files {
    "html/*.html",
    "html/img/*.png",
    "html/sounds/*.ogg",
}
shared_script "Config.lua"
client_scripts {"cl_functions.lua", "client.lua"}

server_scripts {"sv_functions.lua", "server.lua"}


escrow_ignore {
    'Config.lua',
}



dependency '/assetpacks'