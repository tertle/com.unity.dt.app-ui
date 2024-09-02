// This file draws upon the concepts found in the ServiceProviderServiceExtensions implementation from the .NET Runtime library (dotnet/runtime),
// more information in Third Party Notices.md

using System;
using UnityEngine;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Extension methods for <see cref="IServiceProvider"/>.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets a service from the service provider.
        /// </summary>
        /// <param name="serviceProvider"> The service provider. </param>
        /// <typeparam name="T"> The type of the service. </typeparam>
        /// <returns> The service. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceProvider"/> is null. </exception>
        public static T GetService<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            return serviceProvider.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Gets a service from the service provider. Throws an exception if the service is not found.
        /// </summary>
        /// <param name="serviceProvider"> The service provider. </param>
        /// <typeparam name="T"> The type of the service. </typeparam>
        /// <returns> The service. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceProvider"/> is null. </exception>
        public static T GetRequiredService<T>(this IServiceProvider serviceProvider)
            where T : class
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var service = serviceProvider.GetService<T>();

            Debug.Assert(service != null, "Service type not found in the service provider.");

            return service;
        }
    }
}
