fx_version 'cerulean'
games {'gta5'}
description 'markomods addon weapon menu'

server_scripts {
    './lua/server.lua'
}
client_scripts {
    '@menuv/menuv.lua',
    './lua/main.lua'
}


file "./json/addonlist.json"
file "./json/defaultlist.json"