using PoliceMP.Core.Client.IoC;

namespace PoliceMP.Core.Client.Scripts
{
    public static class ServiceContainerExtensions
    {
        public static IScriptManagerBuilder AddScriptManager(this IServiceContainerBuilder services)
        {
            var builder = new ScriptManagerBuilder(services);
            return builder;
        }
    }
}