fx_version 'cerulean'
games {'gta5'}

this_is_a_map 'yes'

lua54 'yes'

before_level_meta 'roxpop.meta'

after_level_meta 'roxheight.meta'

client_scripts
{
    'water.lua',
	'streetnames.lua',
	'birdheight.lua'
}


escrow_ignore 
{
    'streetnames.lua',
	'doortuning.ymt',
  }

file 'roxheight.meta'
file 'roxpop.meta'
file 'roxwood_game.dat151.rel'
file 'water.xml'
file 'heightmap.dat'
file 'gtxd.meta'
file 'popzonerox.ipl'
file 'zonebind.ymt'
file 'doortuning.ymt'

data_file 'AUDIO_GAMEDATA' 'roxwood_game.dat'
data_file 'GTXD_PARENTING_DATA' 'gtxd.meta'
data_file 'ZONEBIND_FILE' 'zonebind.ymt'

dependency '/assetpacks'
dependency '/assetpacks'