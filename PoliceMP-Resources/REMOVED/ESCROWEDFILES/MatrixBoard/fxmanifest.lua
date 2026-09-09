fx_version 'adamant'
games { 'gta5' }
lua54 'yes'

client_scripts {
    "@NativeUI/NativeUI.lua",
    'client.lua',
    'menu.lua'
}

server_scripts {
    'server.lua',
}

ui_page {
    "html/index.html",
}

files {
    'stream/mxdprops.ytyp',
    'html/index.html',
    'html/DotsAllForNow.ttf',
}

data_file 'DLC_ITYP_REQUEST' 'stream/mxdprops.ytyp'

dependency '/assetpacks'