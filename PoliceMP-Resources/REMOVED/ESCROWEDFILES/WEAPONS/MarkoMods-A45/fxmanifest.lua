fx_version 'cerulean'
games {'gta5'}
description 'MarkoMods.com A45'

files{
	'**/markomods-a45-components.meta',
	'**/markomods-a45-archetypes.meta',
	'**/markomods-a45-animations.meta',
	'**/markomods-a45-pedpersonality.meta',
	'**/markomods-a45.meta',
}

data_file 'WEAPONCOMPONENTSINFO_FILE' '**/markomods-a45-components.meta'
data_file 'WEAPON_METADATA_FILE' '**/markomods-a45-archetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/markomods-a45-animations.meta'
data_file 'PED_PERSONALITY_FILE' '**/markomods-a45-pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/markomods-a45.meta'

client_script 'cl_weaponNames.lua'

escrow_ignore {
	'stream/*.ytd',
	'meta/*.meta',
	'cl_weaponNames.lua'
}

lua54 'yes'
dependency '/assetpacks'