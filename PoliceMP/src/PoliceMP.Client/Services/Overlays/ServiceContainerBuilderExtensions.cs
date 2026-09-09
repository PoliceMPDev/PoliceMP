using PoliceMP.Core.Client.IoC;

namespace PoliceMP.Client.Services.Overlays
{
    public static class ServiceContainerBuilderExtensions
    {
        public static IOverlayManagerBuilder AddOverlayManager(this IServiceContainerBuilder services)
        {
            return new OverlayManagerBuilder(services);
        }
    }
}