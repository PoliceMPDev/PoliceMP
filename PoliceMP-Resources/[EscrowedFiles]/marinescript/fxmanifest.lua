fx_version "cerulean"
games { "gta5" }

lua54 "yes"

ui_page "html/index.html"
files { 
    "html/*.html", 
    "html/sounds/*.ogg",  
}

client_scripts { "cl_marinepolicing.lua", "functions.lua"}
server_scripts { "sv_marinepolicing.lua" }
dependency '/assetpacks'