using CitizenFX.Core;
using PoliceMP.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PoliceMP.Core.Shared.Constants;

namespace PoliceMP.Core.Client.IoC
{
    public class ServiceContainer : IServiceContainer
    {
        private readonly Dictionary<Type, object> _services;
        private readonly ILogger<ServiceContainer> _logger;

        public ServiceContainer(Dictionary<Type, object> services)
        {
            services.Add(typeof(IServiceContainer), this);

            _services = services;

            _logger = Get<ILogger<ServiceContainer>>();
        }

        public T Get<T>() => (T)Get(typeof(T));
        public T GetAdhoc<T>() => (T) GetAdhoc(typeof(T));

        public T GetRequired<T>() => (T)GetRequired(typeof(T));
        public T GetAdhocRequired<T>() => (T)GetAdhocRequired(typeof(T));
        
        public object Get(Type type) => ResolveInternal(type, false, false);
        public object GetAdhoc(Type type) => ResolveInternal(type, true, false);

        public object GetRequired(Type type) => ResolveInternal(type, false, true);
        public object GetAdhocRequired(Type type) => ResolveInternal(type, true, true);

        public IEnumerable<T> GetAll<T>()
            => GetAllInternal<T>(typeof(T), false);

        public IEnumerable<T> GetAllRequired<T>()
            => GetAllInternal<T>(typeof(T), true);

        public IEnumerable<object> GetAll(Type type)
            => GetAllInternal<object>(type, false);

        public IEnumerable<object> GetAllRequired(Type type)
            => GetAllInternal<object>(type, true);

        public void ClearCache(Type type)
        {
            _services.Remove(type);
        }

        private IEnumerable<T> GetAllInternal<T>(Type type, bool throwOnError)
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new ServiceContainerException($"\"{typeof(T).FullName}\" is not assignable from \"{type.FullName}\"");
            }

            _logger?.Debug($"Finding all instances of {type.FullName}");
            IEnumerable<T> response = null;
            var services = _services
                .Where(s => s.Key == type 
                    || s.Value.GetType().IsSubclassOf(type) 
                    || ((s.Value as Type)?.IsSubclassOf(type) ?? false)
                    || s.Value.GetType().IsAssignableFrom(type)
                    || ((s.Value as Type)?.IsAssignableFrom(type) ?? false)
                    || s.Key.GetInterfaces().Contains(type)
                    || (s.Value as Type)?.GetInterfaces().Contains(type) == true)
                .Select(s => ResolveInternal(s.Key, false, throwOnError))
                .Where(s => s != null)
                .ToList();

            if (services.Any())
            {
                response =
                    (IEnumerable<T>)typeof(Enumerable)
                        .GetMethod(nameof(Enumerable.Cast))
                        ?.MakeGenericMethod(type)
                        .Invoke(null, new object[] { services.ToArray() });
            }
            else if (throwOnError)
            {
                throw new ServiceContainerException($"Failed to find any services of type \"{type.FullName}\".");
            }

            // If response is still null and we're not throwing on error then create an instance of IEnumerable<type>
            if (response == null)
            {
                _logger?.Trace($"Could not find any instances of {type.FullName}. Building empty IEnumerable<{type.Name}>");
                // Direct compile-time cast
                if (typeof(T) == type)
                {
                    response = new List<T>();
                }
                else
                {
                    // Slower cast. eg. if T is object
                    var genericListType = typeof(List<>).MakeGenericType(type);
                    response = (IEnumerable<T>)Activator.CreateInstance(genericListType);
                }
            }

            return response;
        }

        private object ResolveInternal(Type type, bool adhoc, bool throwOnError)
        {
            bool isGeneric = false;
            _logger?.Debug($"Resolving type {type.FullName}...");
            
            if (adhoc || !_services.TryGetValue(type, out var resource))
            {
                // Is is an IEnumerable request? Maybe there are multiple!
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) 
                                           || type.GetGenericTypeDefinition().IsSubclassOf(typeof(IEnumerable<>))))
                {
                    var genericType = type.GetGenericArguments().Single();
                    var types = throwOnError ? GetAllRequired(genericType) : GetAll(genericType);

                    _logger?.Debug($"Found {types.Count()} of type {genericType.Name} to inject");

                    return types;
                }
                // Well do we have a generic mapping? Eg. ILogger<> => Logger<>
                if (adhoc)
                {
                    _logger?.Trace($"Building adhoc: {type}");
                    resource = type;
                }
                else if (type.IsGenericType && _services.TryGetValue(type.GetGenericTypeDefinition(), out resource))
                {
                    _logger?.Trace($"Found a generic binding: {type} ==> {resource}");
                    isGeneric = true;
                }
                else
                {
                    if (throwOnError)
                        throw new ServiceContainerException($"Failed to resolve type {type.FullName}");

                    _logger?.Trace($"Could not find an instance of {type}. Returning null.");
                    return null;
                }
            }

            // If the resource is a factory, then invoke the factory and replace the value.
            // Alternatively if the resource is a Type, then figure out it's dependencies and create the instance!
            if (resource is Func<IServiceContainer, object> factory)
            {
                _logger?.Trace($"Found a factory method. Building {type} from factory {resource}");

                try
                {
                    var result = factory(this);
                    Debug.WriteLine($"Factory: {resource}");
                    Debug.WriteLine($"Type {type}");
                    Debug.WriteLine($"Result {result}");

                    if (!adhoc)
                    {
                        _services[type] = result;
                    }
                    
                    resource = result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("{0}{1}", ConsoleColors.Red, $"Failed to run Factory Method for type {type.Name}. Reason: {ex.Message}");
                    Debug.WriteLine("{0}{1}", ConsoleColors.Red, ex.Message);
                    Debug.WriteLine("{0}{1}", ConsoleColors.Red, ex.InnerException?.Message);
                    Debug.WriteLine("{0}{1}", ConsoleColors.Red, ex.StackTrace);
                    throw;
                }
            }
            else if (resource is Type typeResource)
            {
                _logger?.Trace($"Found a type mapping: {type} ==> {typeResource}");

                var constructors = typeResource.GetConstructors()
                    .OrderByDescending(c => c.GetParameters().Length)
                    .ToList();

                if (!constructors.Any())
                {
                    if (throwOnError)
                        throw new ServiceContainerException($"Could not find any constructors for type \"{typeResource.FullName}\"");

                    _logger?.Debug($"Could not find any constructors for type \"{typeResource.FullName}\"");
                    return null;
                }

                foreach (var constructorInfo in constructors)
                {
                    if (constructorInfo.GetParameters().Any(p => p.GetType() == typeResource))
                    {
                        _logger?.Warn($"Ignoring constructor {constructorInfo} as it contains a reference to itself!");
                        continue;
                    }
                    
                    var services = GetConstructorServices(constructorInfo);

                    // This is the constructor with the most services that we can serve
                    if (services != null)
                    {
                        if (isGeneric)
                        {
                            _logger?.Trace($"Building generic and Constructing {type} using constructor {constructorInfo}");

                            var genericType = typeResource.MakeGenericType(type.GetGenericArguments());
                            Debug.WriteLine($"Creating new instance of generic: {genericType.FullName}");
                            resource = Activator.CreateInstance(genericType, services);
                        }
                        else
                        {
                            _logger?.Trace($"Constructing {typeResource} using constructor {constructorInfo}");
                            resource = Activator.CreateInstance(typeResource, services);
                        }

                        _services[type] = resource;
                    }
                    else
                    {
                        resource = null;
                    }
                }

                if (resource == null)
                    // Let's never look at this exception message builder.
                    throw new ServiceContainerException($"Failed to inject dependencies for resource {typeResource.FullName}." +
                                           $"\n\tValidConstructors are:" +
                                           $"\n\t\t{string.Join("\n\t\t", constructors.Select(c => $"ctor({string.Join(", ", c.GetParameters().Select(p => p.ParameterType))})"))}");
            }

            return resource;
        }

        private object[] GetConstructorServices(ConstructorInfo constructorInfo)
        {
            var objects = new List<object>();

            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                var service = ResolveInternal(parameterInfo.ParameterType, false, false);
                if (service == null) return null;
                objects.Add(service);
            }

            return objects.ToArray();
        }
    }
}