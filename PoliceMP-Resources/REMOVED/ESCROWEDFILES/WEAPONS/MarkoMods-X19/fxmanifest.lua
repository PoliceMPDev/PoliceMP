fx_version 'cerulean'
games {'gta5'}
description 'MarkoMods.com x19'

files{
	'**/markomods-x19-components.meta',
	'**/markomods-x19-archetypes.meta',
	'**/markomods-x19-animations.meta',
	'**/markomods-x19-pedpersonality.meta',
	'**/markomods-x19.meta',
}

data_file 'WEAPONCOMPONENTSINFO_FILE' '**/markomods-x19-components.meta'
data_file 'WEAPON_METADATA_FILE' '**/markomods-x19-archetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/markomods-x19-animations.meta'
data_file 'PED_PERSONALITY_FILE' '**/markomods-x19-pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/markomods-x19.meta'

client_script 'cl_weaponNames.lua'

escrow_ignore {
	'stream/**/*.ytd',
	'data/**/*.meta',
	'cl_weaponNames.lua'
}
  
lua54 'yes'
dependency '/assetpacks'