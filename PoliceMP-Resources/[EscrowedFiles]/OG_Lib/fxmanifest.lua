fx_version 'adamant'
games { 'gta5' }
lua54 'yes'

author 'OfficerGalvin Development'
version '1.0.0'

escrow_ignore {
    'config.lua',
}

shared_script '@ox_lib/init.lua'

client_scripts { 
    'config.lua',
    'client.lua',
}
server_scripts {
    'config.lua',
    'server.lua',
}

ui_page 'html/index.html'
files {
	'html/index.html',
    'html/index.js',
    "html/audio/*.ogg",
}

server_export 'GetOGLCore'
export 'GetOGLCore'

dependency 'ox_lib'
dependency '/assetpacks'