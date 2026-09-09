fx_version 'cerulean'
games {'gta5'}
description 'MarkoMods.com revolver2'

files{
	'**/markomods-revolver2-components.meta',
	'**/markomods-revolver2-archetypes.meta',
	'**/markomods-revolver2-animations.meta',
	'**/markomods-revolver2-pedpersonality.meta',
	'**/markomods-revolver2.meta',
}

data_file 'WEAPONCOMPONENTSINFO_FILE' '**/markomods-revolver2-components.meta'
data_file 'WEAPON_METADATA_FILE' '**/markomods-revolver2-archetypes.meta'
data_file 'WEAPON_ANIMATIONS_FILE' '**/markomods-revolver2-animations.meta'
data_file 'PED_PERSONALITY_FILE' '**/markomods-revolver2-pedpersonality.meta'
data_file 'WEAPONINFO_FILE' '**/markomods-revolver2.meta'

client_script 'cl_weaponNames.lua'

escrow_ignore {
	'stream/**/*.ytd',
	'data/**/*.meta',
	'cl_weaponNames.lua'
  }
  
  lua54 'yes'
dependency '/assetpacks'