fx_version 'cerulean'
games { 'gta5' }

lua54 "yes"

this_is_a_map 'yes'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_scoop.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_headblocks.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_primarybag.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_o2cylinder.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_primarybagopen.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_alsbag.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_alsbagopen.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_lucas.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_zollvent.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_entonox.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_appbag.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_appbagopen.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_hemsbags.ytyp'

ui_page "html/index.html"
files { 
    "html/*.html",  
    "html/sounds/*.ogg", 
    "json/*.json" 
}

client_scripts {
    "Config.lua",
	'Client/*.lua',
    
}

server_scripts {
	'Server/*.lua'
}

escrow_ignore {
    'stream/prop_primarybag.ytd',
    'stream/prop_primarybagopen.ytd',
    'stream/prop_alsbag.ytd',
    'stream/prop_alsbagopen.ytd',
    "stream/prop_hemsbagorange.ytd",
    "stream/prop_hemsbagblack.ytd",
    "stream/prop_appbag.ytd",
    "stream/prop_appbagopen.ytd",
    "Config.lua"
}

dependency '/assetpacks'