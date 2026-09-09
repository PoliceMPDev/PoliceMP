lua54 'yes'
fx_version 'cerulean'

game 'gta5'
this_is_a_map 'yes'

author 'Apollo developments'
description 'PD IMPOUND LOT'
version '1.0.0'

client_script 'client.lua'


replace_level_meta 'gta5'

file {
'gta5.meta',
'doortuning.ymt',
}

escrow_ignore {
    'stream/*.ytd',
    'stream/pd_impound_doorlocks'
}

dependency '/assetpacks'