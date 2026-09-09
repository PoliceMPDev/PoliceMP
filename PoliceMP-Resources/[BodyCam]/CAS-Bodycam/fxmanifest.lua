fx_version  "cerulean"
game "gta5"
lua54 "yes"


client_scripts {
    "config.lua",
    "client.lua"

}

server_scripts {
    "config.lua",
    "server_main.lua",


    "videopatch.json",

}

ui_page "ui/index.html"


files {
    "ui/index.html",
    "ui/cas.js",
    "ui/style.css",
    "ui/key.png",
    "ui/lspdlogo.png",
    "ui/recordcalendar.png",
    "ui/chevron-left.png",
    "ui/chevron-right.png",
    "ui/Satoshi-Bold.otf",
    "ui/Satoshi-Regular.otf",
    "ui/calendar.png",
    "ui/yes.png",
    "ui/*.png",
    "ui/*.js",
    "ui/*.css",
    "ui/bodycam.mp3"
}

dependency 'utk_render'
