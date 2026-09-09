fx_version 'adamant'
games { 'gta5' }
lua54 'yes'

client_scripts {
    "@NativeUI/NativeUI.lua",
    'config.lua',
    'client.lua',
    "menu.lua",
}

server_scripts {
    'config.lua',
    "server.lua"
}

ui_page {
    "html/index.html",
}

escrow_ignore {
    'config.lua'
}


files{
    'html/index.html',
    "html/index.css",
    "html/images/ECG.png",
    "html/images/CodeSummary.png",
    'stream/aed15.ytyp'
}

data_file 'DLC_ITYP_REQUEST' 'stream/aed15.ytyp'
dependency '/assetpacks'