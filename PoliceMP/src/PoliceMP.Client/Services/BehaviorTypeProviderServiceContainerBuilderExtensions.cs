using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.IoC;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Services
{
    public static class BehaviorTypeProviderServiceContainerBuilderExtensions
    {
        public static IServiceContainerBuilder AddBehaviorTypes(this IServiceContainerBuilder services,
            params Assembly[] assembliesToScan)
        {
            services.Add<IBehaviorService, BehaviorService>();

            services.Add<IBehaviorTypeProvider>(s =>
                new BehaviorTypeProvider(GetBehaviorDefinitionImplementations(assembliesToScan)));
            return services;
        }


        private static Dictionary<Type, Type> GetBehaviorDefinitionImplementations(Assembly[] assembliesToScan)
        {
            Dictionary<Type, Type> response = new Dictionary<Type, Type>();
            foreach (var assembly in assembliesToScan)
            {
                Debug.WriteLine($"Scanning assembly for Behaviors {assembly.FullName}...");
                var implementations = assembly.GetTypes()
                    .Where(t => t.IsSubclassOfRawGeneric(typeof(PedBehavior<>)));

                foreach (var type in implementations)
                {
                    var definitionType = type.BaseType!.GetGenericArguments().First();
                    if (response.ContainsKey(definitionType))
                    {
                        Debug.WriteLine($"ERROR: There are multiple behaviors for type {definitionType.FullName}. Cannot proceed.");
                        continue;
                    }

                    response.Add(definitionType, type);
                    Debug.WriteLine($"Added behavior handler for behavior {type.Name}");
                }
            }

            return response;
        }
    }
}
