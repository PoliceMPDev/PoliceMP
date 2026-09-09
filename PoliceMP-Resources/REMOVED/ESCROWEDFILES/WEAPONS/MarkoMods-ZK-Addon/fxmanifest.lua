fx_version 'cerulean'
games {'gta5'}
description 'MarkoMods.com zk'

files{
	'**/markomods-zk-components.meta',
	'**/markomods-zk-archetypes.meta',
	'**/markomods-zk-animations.meta',
	'**/markomods-zk-pedpersonality.meta',
	'**/markomods-zk.meta',
}

data_file 'WEAPONCOMPONENTSINFO_FILE' '**/markomods-zk-components.meta'
data_file 'WEAPON_METADATA_FILE' '**/markomods-zk-archetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/markomods-zk-animations.meta'
data_file 'PED_PERSONALITY_FILE' '**/markomods-zk-pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/markomods-zk.meta'

client_script 'cl_weaponNames.lua'

escrow_ignore {
	'stream/*.ytd',
	'meta/*.meta',
	'cl_weaponNames.lua'
}

lua54 'yes'
dependency '/assetpacks'