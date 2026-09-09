fx_version "cerulean"
games { "gta5" }

lua54 "yes"

ui_page "html/index.html"
files { 
    "html/*.html",  
    "html/sounds/*.ogg", 
}
client_scripts {
    "functions.lua",
	"cl_afa.lua"
}
server_scripts { "sv_functions.lua", "sv_afa.lua" }

data_file 'DLC_ITYP_REQUEST' 'stream/prop_falarmpanel.ytyp'
dependency '/assetpacks'