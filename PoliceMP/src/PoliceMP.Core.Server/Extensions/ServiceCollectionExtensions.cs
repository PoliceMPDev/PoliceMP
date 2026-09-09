using Microsoft.Extensions.DependencyInjection;
using PoliceMP.Core.Server.Networking;
using System.Linq;
using System.Reflection;

namespace PoliceMP.Core.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add PoliceMP Event Controllers to IServiceCollection
        /// </summary>
        public static IServiceCollection AddEventControllers(this IServiceCollection services)
        {
            var controllers = Assembly
                .GetCallingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Controller)));

            foreach (var controller in controllers)
            {
                services.AddSingleton(typeof(Controller), controller);
            }

            return services;
        }
    }
}
