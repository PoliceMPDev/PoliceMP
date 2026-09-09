fx_version 'cerulean'
game 'gta5'
lua54 'yes'

author 'PoliceMP - smallp13'
description 'Drugalyser Script'
version '1.0.0'

client_script "client.lua"
client_script "c_functions.lua"
server_script "server.lua"

ui_page "html/index.html"

files {
    "html/index.html",
    "html/index.js",
    "html/styles.css",
    "html/digital-7.ttf",
    "html/drugalyser.png"
}

escrow_ignore {
    "html/**/*",
    "stream/**/*"

}
