fx_version 'cerulean'
games {'gta5'}
description 'MarkoMods.com Revolver'

files{
	'**/markomods-revolver-components.meta',
	'**/markomods-revolver-archetypes.meta',
	'**/markomods-revolver-animations.meta',
	'**/markomods-revolver-pedpersonality.meta',
	'**/markomods-revolver.meta',
}

data_file 'WEAPONCOMPONENTSINFO_FILE' '**/markomods-revolver-components.meta'
data_file 'WEAPON_METADATA_FILE' '**/markomods-revolver-archetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/markomods-revolver-animations.meta'
data_file 'PED_PERSONALITY_FILE' '**/markomods-revolver-pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/markomods-revolver.meta'

client_script 'cl_weaponNames.lua'

escrow_ignore {
	'stream/**/*.ytd',
	'data/**/*.meta',
	'cl_weaponNames.lua'
  }
  
  lua54 'yes'
dependency '/assetpacks'