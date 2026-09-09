fx_version 'cerulean'
game 'gta5'
author 'Codesign#2715'
description 'Props'
version '4.0.2'
lua54 'yes'

shared_scripts {
    'configs/locales.lua',
    'configs/config.lua',
}

client_scripts {
    'configs/client_customise_me.lua',
    'client/*.lua'
}

server_scripts {
    'server/*.lua'
}

ui_page {
    'html/index.html'
}
files {
    'configs/locales_ui.js',
    'html/index.html',
    'html/css/*css',
    'html/js/*.js'
}

dependency '/server:4960' -- ⚠️PLEASE READ⚠️; Requires at least server build 4960.

escrow_ignore {
    'client/functions.lua',
    'configs/*.lua',
    'server/version_check.lua'
}
dependency '/assetpacks'