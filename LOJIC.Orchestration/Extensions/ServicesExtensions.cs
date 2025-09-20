using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace LOJIC.Orchestration.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddDerivedServices<T>(this IServiceCollection services, ServiceLifetime lifetime) where T : class
        {
            var derivedFromBase = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(type => type.IsSubclassOf(typeof(T)));
            foreach (var service in derivedFromBase)
            {
                services.Add(new ServiceDescriptor(service, service, lifetime));
            }
        }
    }
}
