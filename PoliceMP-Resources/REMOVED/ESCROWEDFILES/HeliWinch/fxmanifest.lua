fx_version 'cerulean'
games { 'gta5' }

author "BKing Development"
description 'Heli Winch'
version '1.1.0'

lua54 "yes"

this_is_a_map 'yes'
data_file 'DLC_ITYP_REQUEST' 'stream/heliwinch_props.ytyp'


files {
    "html/index.html",
    "html/img/*.png"
}
ui_page "html/index.html"
shared_script "config.lua"

--RMENU
client_scripts {
    "RMenu.lua",
    "menu/RageUI.lua",
    "menu/Menu.lua",
    "menu/MenuController.lua",

    "components/*.lua",

    "menu/elements/*.lua",

    "menu/items/*.lua",

    "menu/panels/*.lua",

    "menu/windows/*.lua",
}
----

server_script "server.lua"
client_scripts {"client.lua", "client_menu.lua"}



escrow_ignore {"config.lua", 
    "RMenu.lua",
    "menu/RageUI.lua",
    "menu/Menu.lua",
    "menu/MenuController.lua",

    "components/*.lua",

    "menu/elements/*.lua",

    "menu/items/*.lua",

    "menu/panels/*.lua",

    "menu/windows/*.lua",
    "prop_carabiner.ydr", "prop_hoist_hook.ydr", "prop_carabiner.ytd", "prop_litter_rigging_01a.ydr", "prop_litter_rigging_02a.ydr", "prop_stokes_litter.ydr", "prop_stokes_litter.ytd"
}

export "Hover" --Toggles Hovering
dependency '/assetpacks'