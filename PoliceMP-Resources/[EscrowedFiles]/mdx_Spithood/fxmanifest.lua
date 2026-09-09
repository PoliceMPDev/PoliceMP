fx_version 'adamant'
games { 'gta5' }

lua54 'yes'

files{
	'**/weaponarchetypes.meta',
	'**/weaponanimations.meta',
	'**/pedpersonality.meta',
	'**/weapons.meta',
}

client_scripts {
    'client.lua',
    'cl_weaponNames.lua'
}

server_scripts {
    'server.lua'
}

data_file 'DLC_ITYP_REQUEST' 'stream/mdx_spithoodx.ytyp'
data_file 'WEAPON_METADATA_FILE' '**/weaponarchetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/weaponanimations.meta'
data_file 'PED_PERSONALITY_FILE' '**/pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/weapons.meta'
