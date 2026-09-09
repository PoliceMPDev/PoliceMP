lua54 'yes'
fx_version 'cerulean'

game 'gta5'
this_is_a_map 'yes'

author 'Apollo Developments'
description 'Sandy Fire Station'
version '1.0.0'


client_script 'client.lua'

data_file "DLC_ITYP_REQUEST" "apollo_sandy_firestation_int.ytyp"

escrow_ignore {
    'stream/unlocked/*.ydr',
    'stream/unlocked/*.ydd',
    'stream/ytd/*.ytd',
    'client.lua',
}
dependency '/assetpacks'