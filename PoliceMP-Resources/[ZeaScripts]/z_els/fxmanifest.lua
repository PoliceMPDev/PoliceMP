
fx_version 'cerulean'
game 'gta5'
lua54 'yes'

name 'z_els'
author 'Zea Development - https://discord.gg/zHvPyJzhQU'
url 'https://zeadevelopment.com/'
version 'v1.3.2'

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type dev_mode

dev_mode 'false'

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type shared_script

shared_scripts ({
   'config/patterns.lua',
   'config/config.lua',
})

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type client_script

client_scripts ({
   'core/client/cl-main.lua',
   'core/client/cl-handlers.lua',
   'core/client/classes/*.lua'
})

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type server_script

server_scripts ({
   'core/server/*.lua',
})

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type ui_page

ui_page 'web/index.html'

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type files

files ({ 
   'web/**',
   'config/vehicles/*.lua'
})

------------- # ------------- # ------------- # ------------- # ------------- # ------------- # ------------- # -------------
---@type: escrow_ignore

escrow_ignore ({
   'config/config.lua',
   'config/patterns.lua',
   'config/vehicles/*.lua',
   'core/client/classes/*.lua'
})
dependency '/assetpacks'