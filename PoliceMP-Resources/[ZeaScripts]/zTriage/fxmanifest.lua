fx_version { 'bodacious' }; games { 'gta5' }; lua54 { 'yes' }

name { 'zTriage' };
author { 'Zea Development - https://discord.gg/zHvPyJzhQU' };
version ( 'v1.0.0' )

client_scripts { 'config.lua', 'client.lua' }

escrow_ignore { 'config.lua', 'readme.md' }

data_file 'DLC_ITYP_REQUEST' 'stream/Tarps.ytyp'
dependency '/assetpacks'