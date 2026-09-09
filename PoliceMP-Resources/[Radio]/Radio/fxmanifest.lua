fx_version 'adamant'
games { 'gta5' }
lua54 'yes'

client_scripts {
    'client.lua',
}

server_scripts {
    'server.lua',
}

ui_page {
    "html/radio.html",
}

files {
    'stream/prop_sc20.ytyp',
    'html/radio.html',
    'html/radio.css',
    'html/images/Radio.png',
    "html/talkGroup.html"
}

data_file 'DLC_ITYP_REQUEST' 'stream/prop_sc20.ytyp'

dependency '/assetpacks'