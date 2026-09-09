fx_version { 'bodacious' } games { 'gta5' } lua54 { 'yes' }

name { 'zCustody-Alarm' }
author { 'Zea Development - https://discord.gg/zHvPyJzhQU' }
url ( 'https://zeadevelopment.com/' )
version ( 'v1.0.1' )

ui_page { 'interface/index.html' }

shared_scripts { 'config.lua' }

client_scripts { 'client/*.lua' }

server_scripts { 'server/*.lua' }

files { 
    'interface/index.html';
    'interface/script.js';
    'interface/style.css';
    'interface/sounds/*.mp3';
}

escrow_ignore { 'config.lua', 'client/utilities.lua' };
data_file 'DLC_ITYP_REQUEST' 'stream/ytyp.ytyp'
dependency '/assetpacks'