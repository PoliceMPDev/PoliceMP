fx_version 'cerulean'
game 'gta5'

author 'PoliceMP'
description 'Standalone TCAS alert system using xsound'
version '1.0.0'

client_scripts {
    'client/main.lua',
    'client/c_helidoors.lua'
}

server_script {
    'server/main.lua',
    'server/s_helidoors.lua'
}

shared_script 'config.lua'

files {
    'config.lua'
}

dependencies {
    'xsound'
}
