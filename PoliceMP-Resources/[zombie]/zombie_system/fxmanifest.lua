fx_version 'cerulean'
games { 'gta5' }

author 'PoliceMP - smallp13'
description 'Standalone Zombie System'
version '1.0.0'

ui_page 'html/index.html'

files {
    'html/index.html',
    'html/script.js',
    'html/zombie_aggressive_1.mp3',
    'html/zombie_aggressive_2.mp3',
    'html/zombie_aggressive_3.mp3',
    'html/zombie_aggressive_4.mp3',
    'html/zombie_aggressive_5.mp3',
    'html/zombie_growl_1.mp3',
    'html/zombie_growl_2.mp3',
    'html/zombie_alert.mp3'
}

client_script 'client.lua'
server_script 'server.lua'
