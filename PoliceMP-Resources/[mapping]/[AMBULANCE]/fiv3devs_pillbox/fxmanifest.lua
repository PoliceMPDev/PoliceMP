fx_version 'cerulean'
game 'gta5'
this_is_a_map 'yes'
author 'Fiv3Devs'
description 'Pillbox Hospital'
version '1.0.4'
lua54 'yes'

replace_level_meta 'gta5'

ui_page 'script/nui/ascensore.html'

files {
	'5d_timecycle_mods_1.xml',
	'gta5.meta',
    'doortuning.ymt',
	'audio/*.dat151.rel',
	'stream/5d_pillbox_mri.ycd',
	'stream/5d_pillbox_xray.ycd',
	'script/nui/ascensore.html',
	'script/nui/ascensore.js',
	'script/nui/ascensore.css',
	'script/nui/ascensore.mp3',
	'sp_manifest.ymt'
}

client_script {
	'ipl.lua',
	'script/config.lua',
	'script/client-side/main.lua'
}

server_script 'script/server-side/server.lua'

data_file 'TIMECYCLEMOD_FILE' '5d_timecycle_mods_1.xml'
data_file 'AUDIO_GAMEDATA' 'audio/5d_hospital_shell_game.dat'
data_file 'SCENARIO_POINTS_OVERRIDE_PSO_FILE' 'sp_manifest.ymt'

escrow_ignore {
  'script/config.lua'
}
dependency '/assetpacks'