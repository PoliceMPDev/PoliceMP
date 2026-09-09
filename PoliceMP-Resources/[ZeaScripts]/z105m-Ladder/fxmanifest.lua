fx_version { 'bodacious' } games { 'gta5' } lua54 { 'yes' }

name { 'z105m-Ladder' }
author { 'Zea Development - https://discord.gg/zHvPyJzhQU' }
url ( 'https://zeadevelopment.com/' )
version ( 'v1.0.3' )

shared_scripts { 'config.lua' }

client_scripts { 'client/*.lua' }

server_scripts { 'server/*.lua' }

escrow_ignore { 'config.lua', 'client/utilities.lua' };

data_file 'DLC_ITYP_REQUEST' 'stream/105.ytyp'
dependency '/assetpacks'