fx_version 'adamant'
games { 'gta5' }
lua54 'yes'

client_scripts {
    'config.lua',
    'utils.lua',
    'client.lua'
}

server_scripts {
    'config.lua',
    'utils.lua',
    'server.lua',
    'sv_functions.lua'
}

escrow_ignore {
    'config.lua',
    'sv_functions.lua'
}

ui_page 'html/index.html'

files {
    "html/index.html",
    "html/index.js",
    "html/constants/*.js",
    "html/components/*.js",
    "html/css/*.css",
    'html/media/images/*.png',
    'html/media/audio/*.ogg'
}

data_file 'DLC_ITYP_REQUEST' 'stream/mdx_sc20.ytyp'

dependency '/assetpacks'