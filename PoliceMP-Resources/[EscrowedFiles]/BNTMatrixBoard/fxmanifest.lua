---@section: fx_information

fx_version 'cerulean'
game 'gta5'
lua54 'yes'

---@section: resource_information

name 'bnt_matrix'
author 'BNT x Zea Development'
description 'Animated Matrix Board for all vehicles across British FiveM'
url 'https://bntmodding.com/'
version '40af2c9'

---@section: client_script

client_script({
    'client.lua',
    'mm.lua'
})

---@section: server_script

server_script({
    'server.lua'
})

---@section: files

files({
    '*.json',
    'stream/*.ycd',
})

---@section: data_files

data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_01.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_02.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_03.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_04.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_05.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_06.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_07.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_08.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_09.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_10.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_11.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_12.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_13.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_14.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_15.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_16.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/prop_matrix_bnt.ytyp'

escrow_ignore {
    'config.json'
}
dependency '/assetpacks'