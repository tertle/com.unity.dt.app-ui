// This file draws upon the concepts found in the ServiceCollectionServiceExtensions implementation from the .NET Runtime library (dotnet/runtime),
// more information in Third Party Notices.md

using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServicesCollectionExtensions
    {
        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="T"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="T"> The type of the service to add. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection AddSingleton<T>(this IServiceCollection serviceCollection)
            where T : class
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.AddSingleton(typeof(T));
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <typeparam name="TImplementation"> The type of the implementation to use. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.AddSingleton(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> or <paramref name="serviceType"/> is null. </exception>
        public static IServiceCollection AddSingleton(this IServiceCollection serviceCollection, Type serviceType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            return serviceCollection.AddSingleton(serviceType, serviceType);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> with an
        /// implementation type specified in <paramref name="implementationType"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <param name="implementationType"> The type of the implementation to use. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/>, <paramref name="serviceType"/> or <paramref name="implementationType"/> is null. </exception>
        public static IServiceCollection AddSingleton(this IServiceCollection serviceCollection, Type serviceType, Type implementationType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            return Add(serviceCollection, serviceType, implementationType, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static void TryAddSingleton<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            TryAddSingleton(serviceCollection, typeof(TService), typeof(TService));
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <typeparam name="TImplementation"> The type of the implementation to use. </typeparam>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static void TryAddSingleton<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            TryAddSingleton(serviceCollection, typeof(TService), typeof(TImplementation));
        }

        static void TryAddSingleton(IServiceCollection serviceCollection, Type serviceType, Type implementationType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            var descriptor = ServiceDescriptor.Singleton(serviceType, implementationType);
            TryAdd(serviceCollection, descriptor);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="T"/> to the specified
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="T"> The type of the service to add. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection AddTransient<T>(this IServiceCollection serviceCollection)
            where T : class
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.AddTransient(typeof(T));
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <typeparam name="TImplementation"> The type of the implementation to use. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.AddTransient(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> or <paramref name="serviceType"/> is null. </exception>
        public static IServiceCollection AddTransient(this IServiceCollection serviceCollection, Type serviceType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            return serviceCollection.AddTransient(serviceType, serviceType);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType"/> with an
        /// implementation type specified in <paramref name="implementationType"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <param name="implementationType"> The type of the implementation to use. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/>, <paramref name="serviceType"/> or <paramref name="implementationType"/> is null. </exception>
        public static IServiceCollection AddTransient(this IServiceCollection serviceCollection, Type serviceType, Type implementationType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            return Add(serviceCollection, serviceType, implementationType, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Builds a <see cref="IServiceProvider"/> from the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to build the <see cref="IServiceProvider"/> from. </param>
        /// <returns> The <see cref="IServiceProvider"/>. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceProvider BuildServiceProvider(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return new ServiceProvider(serviceCollection);
        }

        static void TryAdd(IServiceCollection serviceCollection, ServiceDescriptor descriptor)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var count = serviceCollection.Count;
            for (var i = 0; i < count; i++)
            {
                if (serviceCollection[i].serviceType == descriptor.serviceType)
                    return;
            }

            serviceCollection.Add(descriptor);
        }

        static IServiceCollection Add(IServiceCollection serviceCollection, Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
            serviceCollection.Add(descriptor);
            return serviceCollection;
        }
    }
}
