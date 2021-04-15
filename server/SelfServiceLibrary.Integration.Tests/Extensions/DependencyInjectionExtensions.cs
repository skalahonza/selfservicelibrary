using System;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace SelfServiceLibrary.Integration.Tests.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
            services.Remove(descriptorToRemove);
            var descriptorToAdd = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);
            services.Add(descriptorToAdd);
            return services;
        }

        public static IServiceCollection Replace<TService>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            ServiceLifetime lifetime)
            where TService : class
        {
            var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
            services.Remove(descriptorToRemove);
            var descriptorToAdd = new ServiceDescriptor(typeof(TService), implementationFactory, lifetime);
            services.Add(descriptorToAdd);
            return services;
        }
    }
}
