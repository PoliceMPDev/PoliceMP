fx_version 'cerulean'
game 'gta5'

author 'Code5Mods'
description 'Code5Mods Stretcher Resource (Script)'
version '1.2'
lua54 'yes'

client_scripts {
    "@NativeUI/NativeUI.lua",
    "config.lua",
    "client/main.lua"
}


escrow_ignore {
    "config.lua",
  }
dependency '/assetpacks'