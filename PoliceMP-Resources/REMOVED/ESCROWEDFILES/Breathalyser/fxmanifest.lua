fx_version 'adamant'
games { 'gta5' }
lua54 "yes"

client_scripts {
    'client/cl_breathalyser.lua'
}

server_scripts {
    'server/sv_breathalyser.lua'
}

ui_page {
    "html/index.html",
}

files {
    'html/index.html',
    'html/index.js',
    'html/index.css',
    'html/reset.css',
    "html/images/BreathFail.png",
    "html/images/BreathPass.png",
    "html/images/BreathWait.png",
    "html/images/BreahtalyserSubject.png",
    "html/sounds/BeepBreath.ogg",
    "html/sounds/BeepShort.ogg",
    'stream/breatha.ytyp'
}

data_file 'DLC_ITYP_REQUEST' 'stream/breatha.ytyp'

dependency '/assetpacks'