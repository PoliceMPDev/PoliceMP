fx_version 'cerulean'
games {'gta5'}
description 'MarkoMods.com agc'

files{
	'**/markomods-agc-components.meta',
	'**/markomods-agc-archetypes.meta',
	'**/markomods-agc-animations.meta',
	'**/markomods-agc-pedpersonality.meta',
	'**/markomods-agc.meta',
}

data_file 'WEAPONCOMPONENTSINFO_FILE' '**/markomods-agc-components.meta'
data_file 'WEAPON_METADATA_FILE' '**/markomods-agc-archetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/markomods-agc-animations.meta'
data_file 'PED_PERSONALITY_FILE' '**/markomods-agc-pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/markomods-agc.meta'

client_script 'cl_weaponNames.lua'

escrow_ignore {
	'stream/**/*.ytd',
	'data/**/*.meta',
	'cl_weaponNames.lua'
}
  
lua54 'yes'
dependency '/assetpacks'