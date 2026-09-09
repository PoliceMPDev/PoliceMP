fx_version 'cerulean'
games {'gta5'}
description 'Markomods.com X45'

files{
	'**/markomods-x45-components.meta',
	'**/markomods-x45-archetypes.meta',
	'**/markomods-x45-animations.meta',
	'**/markomods-x45-pedpersonality.meta',
	'**/markomods-x45.meta',
}

data_file 'WEAPONCOMPONENTSINFO_FILE' '**/markomods-x45-components.meta'
data_file 'WEAPON_METADATA_FILE' '**/markomods-x45-archetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/markomods-x45-animations.meta'
data_file 'PED_PERSONALITY_FILE' '**/markomods-x45-pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/markomods-x45.meta'

client_script 'cl_weaponNames.lua'

escrow_ignore {
	'stream/**/*.ytd',
	'data/**/*.meta',
	'cl_weaponNames.lua'
}
  
lua54 'yes'
dependency '/assetpacks'