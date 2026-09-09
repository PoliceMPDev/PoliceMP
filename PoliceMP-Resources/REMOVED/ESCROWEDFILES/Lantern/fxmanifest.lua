fx_version 'adamant'
games { 'gta5' }
lua54 "yes"

client_scripts {
    'client/cl_lantern.lua'
}

server_scripts {
    'server/sv_lantern.lua'
}

ui_page {
    "html/index.html",
}

files {
    'html/index.html',
    'html/index.js',
    'html/index.css',
    'html/reset.css',
    "html/images/Lantern.png",
    "html/images/LanternTwo.png"
}

data_file 'DLC_ITYP_REQUEST' 'stream/aed.ytyp'

dependency '/assetpacks'