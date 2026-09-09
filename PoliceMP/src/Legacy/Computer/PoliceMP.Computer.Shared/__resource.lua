client_scripts {
	"PoliceMP.Computer.Client.net.dll"
}

server_scripts {
	"PoliceMP.Computer.Server.net.dll",
}

ui_page "ui/desktop.html"
files {
	-- C# Dependencies
	"Newtonsoft.Json.dll",

	-- HTML
	"ui/desktop.html",
	
	-- CSS
	"ui/css/descktop.css",
	"ui/css/descktop.css.map",
	"ui/css/descktop.less",
	"ui/css/metro-all.min.css",
	
	-- JS
	"ui/js/desktop.js",
	"ui/js/callouts.js",
	"ui/js/games.js",
	"ui/js/metro.min.js",
	
	-- Images
	"ui/images/police_background.jpg"
}