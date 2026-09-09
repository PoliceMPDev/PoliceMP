using System;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Overlays;

namespace PoliceMP.Client.Services.Overlays
{
    public interface IOverlayManager
    {
        /// <summary>
        /// Get an overlay using the overlay type as an id.
        /// Make sure that the Overlay id in React matches type T.
        /// </summary>
        /// <typeparam name="T">Type of overlay</typeparam>
        /// <returns>Overlay</returns>
        T GetOverlay<T>() where T : IOverlay;

        /// <summary>
        /// Get an overlay using the overlay type as an id.
        /// Make sure that the Overlay id in React matches type T.
        /// </summary>
        /// <typeparam name="T">Type of overlay</typeparam>
        /// <returns>Overlay</returns>
        IOverlay GetOverlay(Type type);

        /// <summary>
        /// Get an overlay using a specific id.
        /// Make sure that the Overlay id in React matches the id.
        /// </summary>
        /// <typeparam name="T">Type of overlay</typeparam>
        /// <param name="id">id of the overlay</param>
        /// <returns>Overlay</returns>
        T GetOverlay<T>(string id) where T : IOverlay;

        /// <summary>
        /// Get an overlay using a specific id.
        /// Make sure that the Overlay id in React matches the id.
        /// </summary>
        /// <typeparam name="T">Type of overlay</typeparam>
        /// <param name="id">id of the overlay</param>
        /// <returns>Overlay</returns>
        IOverlay GetOverlay(Type type, string id);
    }
}
