using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PoliceMP.Web.Server
{
    public static class WebServer
    {
        public static void BuildAndRun(IServiceProvider serviceProvider, string contentRoot)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(contentRoot: contentRoot)
                .Configure(app =>
                {
                    app.ApplicationServices = serviceProvider;
                    app.UseCors();
                    app.UseMvcWithDefaultRoute();
                })
                .ConfigureLogging(l => l.AddConsole())
                .Build();

            host.Run();
        }
    }
}
