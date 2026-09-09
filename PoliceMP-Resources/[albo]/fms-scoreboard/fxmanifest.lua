description 'Scoreboard'

fx_version 'adamant'
game 'gta5'

dependencies {
	'fms'
}

ui_page 'html/fms-scoreboard.html'

client_script 'scoreboard.lua'
server_script 'sv_scoreboard.lua'

files {
    'html/fms-scoreboard.html',
    'html/style.css',
    'html/reset.css',
    'html/listener.js',
    'html/res/futurastd-medium.css',
    'html/res/futurastd-medium.eot',
    'html/res/futurastd-medium.woff',
    'html/res/futurastd-medium.ttf',
    'html/res/futurastd-medium.svg',
}

