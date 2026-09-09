fx_version 'cerulean'
games { 'gta5' }

client_scripts {
	"client/PoliceMP.Client.net.dll",
	"client/PoliceMP.Core.Client.net.dll",
	"client/PoliceMP.Shared.dll",
	"client/PoliceMP.Core.Shared.dll",
	"client/PoliceMP.Core.Mediator.dll",
	
	"client/Newtonsoft.Json.dll",
	"client/DotNetZip.dll",
	"client/Stateless.dll",
}

server_scripts {
	"server/PoliceMP.Server.net.dll",
	"server/PoliceMP.Shared.dll",
	"server/PoliceMP.Core.Shared.dll",
	"server/PoliceMP.Data.net.dll",
	"server/PoliceMP.Core.Mediator.dll",

	"server/DotNetZip.dll",
	"server/Microsoft.Bcl.AsyncInterfaces.dll",
	"server/Microsoft.Extensions.Caching.Abstractions.dll",
	"server/Microsoft.Extensions.Caching.Memory.dll",
	"server/Microsoft.Extensions.DependencyInjection.Abstractions.dll",
	"server/Microsoft.Extensions.DependencyInjection.dll",
	"server/Microsoft.Extensions.Logging.Abstractions.dll",
	"server/Microsoft.Extensions.Options.dll",
	"server/Microsoft.Extensions.Primitives.dll",

	"server/Newtonsoft.Json.dll",
	"server/MySql.Data.dll",
}

ui_page "client/ui/index.html"

files {
	"client/MenuAPI.dll",
	"client/NativeUI.dll",
	"**/*.html",
	"**/*.css",
	"**/*.js",
	"**/*.map",
	"**/*.png",
	"**/*.woff",
	"**/*.wav",
	"**/*.ogg"
}