fx_version 'bodacious'
games { 'gta5' }

description 'A resource providing a realistic decontamination tent experience'
version '1.0.0'

files {
    'stream/decon_tent.ytyp',
}

client_scripts {
    'config_decontent.lua',
    'cl_decontent.lua',
}

server_scripts {
    'config_decontent.lua',
    'sv_decontent.lua',
}

data_file 'DLC_ITYP_REQUEST' 'stream/decon_tent.ytyp'