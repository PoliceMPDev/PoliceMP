fx_version 'cerulean'
games {'gta5'}
description 'MarkoMods.com KVR'

files{
	'**/markomods-kvr-components.meta',
	'**/markomods-kvr-archetypes.meta',
	'**/markomods-kvr-animations.meta',
	'**/markomods-kvr-pedpersonality.meta',
	'**/markomods-kvr.meta',
}

data_file 'WEAPONCOMPONENTSINFO_FILE' '**/markomods-kvr-components.meta'
data_file 'WEAPON_METADATA_FILE' '**/markomods-kvr-archetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/markomods-kvr-animations.meta'
data_file 'PED_PERSONALITY_FILE' '**/markomods-kvr-pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/markomods-kvr.meta'

client_script 'cl_weaponNames.lua'


escrow_ignore {
	'stream/**/*.ytd',
	'meta/**/*.meta',
	'cl_weaponNames.lua'
  }
  
  lua54 'yes'
dependency '/assetpacks'