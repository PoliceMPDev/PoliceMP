fx_version 'cerulean'
game 'gta5'
lua54 'yes'
author 'rytrak.fr'
ui_page 'html/index.html'

files {
    'html/*',

    'data/contentunlocks.meta',
    'data/weaponanimations.meta',
    'data/weaponarchetypes.meta',
    'data/loadouts.meta',
    'data/weapons.meta'
}

escrow_ignore {
	'config.lua',
	'cl_utils.lua',
    'sv_utils.lua'
}

shared_script 'config.lua'

server_script {
    'sv_utils.lua',
    'server/server.lua',
    'server/create.lua',
    'server/spreaders.lua',
    'server/cutters.lua',
    'server/cut_protection.lua',
    'server/window.lua',
    'server/damage.lua'
}

client_scripts {
    'cl_utils.lua',
	'client/client.lua',
    'client/create.lua',
    'client/sound.lua',
    'client/raycast.lua',
    'client/spreaders.lua',
    'client/cutters.lua',
    'client/cut_protection.lua',
    'client/window.lua',
    'client/extra.lua',
    'client/damage.lua'
}

data_file 'WEAPONINFO_FILE' 'data/weapons.meta'
data_file 'WEAPON_METADATA_FILE' 'data/weaponarchetypes.meta'
data_file 'LOADOUTS_FILE' 'data/loadouts.meta'
data_file 'WEAPON_ANIMATIONS_FILE' 'data/weaponanimations.meta'
data_file 'CONTENT_UNLOCKING_META_FILE' 'data/contentunlocks.meta'

data_file "DLC_ITYP_REQUEST" "stream/props_extrication.ytyp"
dependency '/assetpacks'