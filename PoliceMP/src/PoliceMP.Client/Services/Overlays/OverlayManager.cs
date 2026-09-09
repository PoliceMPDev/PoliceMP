using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.IoC;
using PoliceMP.Core.Client.Overlays;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Services.Overlays
{
    public class OverlayManager : IOverlayManager
    {
        private readonly ILogger<OverlayManager> _logger;
        private readonly IServiceContainer _services;
        private readonly Dictionary<string, KeyValuePair<Type, Type>> _overlayIdMappings;
        private readonly ConcurrentDictionary<string, Overlay> _overlays = new ConcurrentDictionary<string, Overlay>();

        public OverlayManager(IServiceContainer services, Dictionary<string, KeyValuePair<Type, Type>> overlayIdMappings)
        {
            _services = services;
            _logger = _services.GetRequired<ILogger<OverlayManager>>();
            _overlayIdMappings = overlayIdMappings;
        }

        public T GetOverlay<T>() where T : IOverlay
            => (T) GetOverlay(typeof(T));

        public IOverlay GetOverlay(Type type)
        {
            var typeMapping = _overlayIdMappings
                .Where(om => om.Value.Key == type)
                .Select(tm => tm.Value.Value)
                .SingleOrDefault();

            if (typeMapping != null)
            {
                return GetOverlay(type, typeMapping.Name);
            }

            throw new OverlayManagerException($"Failed to find an overlay of type {type.FullName}");
        }

        public T GetOverlay<T>(string id) where T : IOverlay
            => (T) GetOverlay(typeof(T), id);

        public IOverlay GetOverlay(Type type, string id)
        {
            _logger.Trace($"GetOverlay with id: {id}");
            if (!_overlays.TryGetValue(id, out var overlay))
            {
                _logger.Trace($"Couldn't find an existing overlay for id {id}. Creating a new one!");
                if (!_overlayIdMappings.TryGetValue(id, out var mapping))
                {
                    _logger.Trace($"Couldn't find a type mapping for id. Check the configuration in {nameof(Client)}.cs!");
                    return null;
                }

                if (!mapping.Key.IsAssignableFrom(type))
                {
                    throw new OverlayManagerException($"Could not convert overlay {id}[{mapping.Key.FullName}] to typeof({type.FullName}");
                }

                _logger.Trace($"Building AdHoc service for {mapping.Value.FullName}");

                var overlayConstructors = mapping.Value.GetConstructors();
                if (overlayConstructors.Length > 1)
                    throw new OverlayManagerException("An Overlay can only have one constructor!");

                var overlayConstructor = overlayConstructors.Single();
                var constructorParams = new[]
                {
                    id
                }.Concat(
                    overlayConstructor
                        .GetParameters()
                        .Skip(1) // Skip Id parameter
                        .Select(pi => _services.GetRequired(pi.ParameterType))
                );

                overlay = Activator.CreateInstance(mapping.Value, constructorParams.ToArray()) as Overlay;

                if (!_overlays.TryAdd(id, overlay))
                {
                    return null;
                }
            }

            return overlay;
        }
    }
}