using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.IoC;
using PoliceMP.Core.Client.Overlays;

namespace PoliceMP.Client.Services.Overlays
{
    public interface IOverlayManagerBuilder
    {
        /// <summary>
        /// Adds an overlay using the type as it's ID.
        /// Make sure that the Overlay id in react project matches type T.
        /// </summary>
        /// <typeparam name="T">Type of the overlay</typeparam>
        /// <returns>Fluent builder</returns>
        IOverlayManagerBuilder AddOverlay<T>() where T : Overlay;

        /// <summary>
        /// Adds an overlay with a specific id.
        /// Make sure that the Overlay id in react project matches the id.
        /// </summary>
        /// <typeparam name="T">Type of the overlay</typeparam>
        /// <param name="id">id of the overlay</param>
        /// <returns></returns>
        IOverlayManagerBuilder AddOverlay<T>(string id) where T : Overlay;

        /// <summary>
        /// Adds an overlay using the type as it's ID.
        /// Make sure that the Overlay id in react project matches type TImplementation.
        /// </summary>
        /// <typeparam name="T">Type of the overlay</typeparam>
        /// <typeparam name="TImplementation">Implementation Type</typeparam>
        /// <returns>Fluent builder</returns>
        IOverlayManagerBuilder AddOverlay<T, TImplementation>()
            where TImplementation : Overlay;

        /// <summary>
        /// Adds an overlay using the type as it's ID.
        /// Make sure that the Overlay id in react project matches type T.
        /// </summary>
        /// <typeparam name="T">Type of the overlay</typeparam>
        /// <typeparam name="TImplementation">Implementation Type</typeparam>
        /// <param name="id">id of the overlay</param>
        /// <returns>Fluent builder</returns>
        IOverlayManagerBuilder AddOverlay<T, TImplementation>(string id)
            where TImplementation : Overlay;

        /// <summary>
        /// Build and inject the IOverlayManager
        /// </summary>
        /// <returns>Service collection builder</returns>
        IServiceContainerBuilder Build();
    }
}