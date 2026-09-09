using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using PoliceMP.Web.Server.Controllers;

namespace PoliceMP.Web.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPoliceMPWebServices(this IServiceCollection services)
        {
            services.AddMvc()
                .AddApplicationPart(typeof(HomeController).Assembly);
            services.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());
            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment());
            services.AddSingleton<DiagnosticSource>(new DiagnosticListener("PoliceMP"));

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        // TODO: Pull URL from config
                        .WithOrigins("https://localhost:44350")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            return services;
        }
    }
}