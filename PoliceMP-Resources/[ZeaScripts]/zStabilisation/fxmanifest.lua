fx_version { 'cerulean' }; games { 'gta5' }; lua54 { 'yes' }

name { 'zStabilisation' };
author { 'Zea Development - https://discord.gg/zHvPyJzhQU // Props by Jessica#2207' };
version ( 'v1.0' )

client_scripts { 'Client/_cMain.lua' }

server_scripts { 'Server/_sMain.lua' };

files { 'Settings.json' }

escrow_ignore { 'Settings.json'; 'readme.md' }

this_is_a_map 'yes'
data_file 'DLC_ITYP_REQUEST' 'stream/zStabilisation.ytyp'
dependency '/assetpacks'