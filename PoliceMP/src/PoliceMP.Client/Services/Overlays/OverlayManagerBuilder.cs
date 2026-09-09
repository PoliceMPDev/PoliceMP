using System;
using System.Collections.Generic;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.IoC;
using PoliceMP.Core.Client.Overlays;

namespace PoliceMP.Client.Services.Overlays
{
    public class OverlayManagerBuilder : IOverlayManagerBuilder
    {
        private readonly IServiceContainerBuilder _services;
        private readonly Dictionary<string, KeyValuePair<Type, Type>> _overlayIdMappings = new Dictionary<string, KeyValuePair<Type, Type>>();

        public OverlayManagerBuilder(IServiceContainerBuilder services)
        {
            _services = services;
        }

        public IOverlayManagerBuilder AddOverlay<T>() where T : Overlay
            => AddOverlay<T, T>(typeof(T).Name);

        public IOverlayManagerBuilder AddOverlay<T>(string id) where T : Overlay
            => AddOverlay<T, T>(id);

        public IOverlayManagerBuilder AddOverlay<T, TImplementation>() where TImplementation : Overlay
            => AddOverlay<T, TImplementation>(typeof(TImplementation).Name);

        public IOverlayManagerBuilder AddOverlay<T, TImplementation>(string id) where TImplementation : Overlay
        {
            _overlayIdMappings.Add(id, new KeyValuePair<Type, Type>(typeof(T), typeof(TImplementation)));
            return this;
        }

        public IServiceContainerBuilder Build()
        {
            _services.Add<IOverlayManager>(services => new OverlayManager(services, _overlayIdMappings));

            // Add each default HUD. Eg. Requesting IHud
            foreach (var hudMap in _overlayIdMappings)
            {
                _services.Add(hudMap.Value.Key, services =>
                {
                    var overlayManager = services.GetRequired<IOverlayManager>();
                    return overlayManager.GetOverlay(hudMap.Value.Key, hudMap.Value.Value.Name);
                });
            }

            return _services;
        }
    }
}