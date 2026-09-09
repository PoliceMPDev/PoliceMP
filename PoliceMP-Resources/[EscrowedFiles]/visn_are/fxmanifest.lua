--[[
-- Author: Tim Plate
-- Project: Advanced Roleplay Environment
-- Copyright (c) 2022 Tim Plate Solutions
--]]

fx_version "cerulean"
game 'gta5'

author 'Tim Plate <admin@plate-solutions.de>'
description 'Advanced Roleplay Environment'
version "b9a94955 (Feb 12 2023)"
lua54 'yes'

shared_scripts {
    'script/configuration/client_config.lua',
    'script/entities/**',
    --'script/plugins.lua',
    'script/helpers/g_*.lua',
}

client_scripts {
    'script/helpers/c_*.lua',
    'script/client.lua',
    'script/custom/*.lua',
    --'plugins/plugin_**/plugin_*_client.lua',
}

server_scripts {
    'script/configuration/server_config.lua',
    'script/server.lua',
    'script/helpers/s_*.lua',
    --'plugins/plugin_**/plugin_*_server.lua',
}

escrow_ignore {
    'script/configuration/**',
    'script/entities/**',
    'script/helpers/**',
    'script/custom/**',
    'plugins/**',
    'script/languages/**'
}

data_file 'DLC_ITYP_REQUEST' 'stream/props.ytyp'

ui_page 'nui/index.html'
files {
    'nui/**',
    'nui/assets/images/*.png',
    'nui/assets/sounds/*.wav',
    'script/languages/*.json',
    'database/*.json'
}

dependency '/assetpacks'