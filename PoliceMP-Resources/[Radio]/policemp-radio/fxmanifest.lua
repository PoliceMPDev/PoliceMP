version '1.0.0'
author 'Bronson Newton'
description 'Radio system designed to work for PoliceMP.'

fx_version 'adamant'
games {'gta5'}

ui_page 'ui/index.html'

client_scripts {
    'client/core.lua',
}

server_scripts {
    'server/core.lua',
}

files {
    'ui/index.html',
    'ui/reset.css',
    'ui/resources/**.png',
    'ui/resources/**.ogg',
    'ui/script/**.js',
    'ui/style/**.css',
}
