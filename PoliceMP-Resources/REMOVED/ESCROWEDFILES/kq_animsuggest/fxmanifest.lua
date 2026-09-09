fx_version 'cerulean'
games { 'gta5' }
lua54 'yes'

author 'KuzQuality | Kuzkay'
description 'Animation suggestions by KuzQuality'
version '1.1.2'


ui_page 'nui/index.html'

files {
    'nui/*.html',
    'nui/js/*.js',
}

--
-- Server
--

server_scripts {
}

--
-- Client
--

client_scripts {
    'config.lua',
    'locale.lua',
    'client/constants.lua',
    'client/settings.lua',
    'client/functions.lua',
    'client/cache.lua',
    'client/client.lua',
    'client/editable/client.lua',
}

escrow_ignore {
    'config.lua',
    'locale.lua',
    'client/editable/*.lua',
}

dependency '/assetpacks'