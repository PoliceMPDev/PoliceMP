fx_version { 'bodacious' }; games { 'gta5' }; lua54 { 'yes' }

name { 'zPump' };
author { 'Zea Development' };
version ( 'v1.1' )

ui_page 'Client/NUI/index.html'

client_scripts { '_Settings.lua'; 'Client/_*.lua' };

server_scripts { 'Server/_*.lua' };

files {
    'Client/NUI/index.html',
    'Client/NUI/Main.js',
    'Client/NUI/*.css';
}

escrow_ignore { '_Settings.lua' };
dependency '/assetpacks'