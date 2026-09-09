fx_version 'cerulean'
game 'gta5'
lua54 'yes'

author 'OfficerGalvin Development'
version '1.0.0'

escrow_ignore {
    'config.lua',
}

client_scripts { 
    'cl_weaponNames.lua',
    'config.lua',
    'client.lua',
    'utils.lua',
}
server_scripts {
    'config.lua',
    'server.lua',
    'permissions.lua',
}


files{
	'**/weaponcomponents.meta',
	'**/weaponarchetypes.meta',
	'**/weaponanimations.meta',
	'**/pedpersonality.meta',
	'**/weapons.meta',
}

data_file 'WEAPONCOMPONENTSINFO_FILE' '**/weaponcomponents.meta'
data_file 'WEAPON_METADATA_FILE' '**/weaponarchetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/weaponanimations.meta'
data_file 'PED_PERSONALITY_FILE' '**/pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/weapons.meta'
data_file 'DLC_ITYP_REQUEST' 'stream/SEPROPS.ytyp'

dependency 'OG_Lib'
dependency '/assetpacks'