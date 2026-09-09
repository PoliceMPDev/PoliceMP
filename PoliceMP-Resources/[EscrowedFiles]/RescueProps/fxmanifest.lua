fx_version 'cerulean'
games { 'gta5' }

client_scripts {
    'Client/*.lua'
}

server_scripts {
    'Server/*.lua'
}

shared_scripts {
    '@ox_lib/init.lua',
    'Shared/*.lua'
}

dependencies {
    'ox_target',
    'ox_lib'
}

ox_libs {
    'interface'
}

data_file 'DLC_ITYP_REQUEST' 'stream/ambulance_props.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/Fire/p_chock.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/Fire/p_cribbing.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/Fire/p_setup_chock.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/Fire/prop_drysorb.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/Fire/prop_kfire.ytyp'

lua54 'yes'

escrow_ignore {
    'Shared/*.lua'
}

dependency '/assetpacks'