fx_version 'cerulean'
game 'gta5'
lua54 'yes'

author 'PoliceMP - smallp13'
description 'Breathalyser Script'
version '1.0.0'

client_script "client.lua"
client_script "c_functions.lua"
server_script "server.lua"

ui_page "html/index.html"

files {
    "html/index.html",
    "html/index.js",
    "html/styles.css",
    "html/breathalyzer.png",
    "html/digital-7.ttf",
    "html/sounds/BeepShort.ogg" 
}

escrow_ignore {
    "html/**/*",
    "stream/**/*"

}

data_file 'DLC_ITYP_REQUEST' 'stream/breatha.ytyp'

dependency '/assetpacks'