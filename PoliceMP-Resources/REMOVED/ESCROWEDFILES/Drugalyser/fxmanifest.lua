fx_version 'adamant'
games { 'gta5' }
lua54 'yes'

client_scripts {
    'client/cl_drugalyser.lua'
}

server_scripts {
    'server/sv_drugalyser.lua'
}

ui_page {
    "html/index.html",
}

files {
    'html/index.html',
    'html/index.js',
    'html/index.css',
    'html/reset.css',
    "html/images/DrugSubject.png",
    "html/images/DrugCannabis.png",
    "html/images/DrugCocaine.png",
    "html/images/DrugClear.png",
    "html/images/DrugCocaineAndCannabis.png",
    'stream/druga.ytyp'
}

data_file 'DLC_ITYP_REQUEST' 'stream/druga.ytyp'

dependency '/assetpacks'