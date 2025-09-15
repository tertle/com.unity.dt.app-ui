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

            return Add(serviceCollection, ServiceDescriptor.Singleton(serviceType, implementationType));
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddSingleton<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return TryAddSingleton(serviceCollection, typeof(TService), typeof(TService));
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <typeparam name="TImplementation"> The type of the implementation to use. </typeparam>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddSingleton<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return TryAddSingleton(serviceCollection, typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Try to add a singleton service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddSingleton(this IServiceCollection serviceCollection, Type serviceType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            return TryAddSingleton(serviceCollection, serviceType, serviceType);
        }

        /// <summary>
        /// Try to add a singleton service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <param name="implementationType"> The type of the implementation to use. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddSingleton(IServiceCollection serviceCollection, Type serviceType, Type implementationType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            return TryAdd(serviceCollection, ServiceDescriptor.Singleton(serviceType, implementationType));
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="T"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="T"> The type of the service to add. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection AddScoped<T>(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.AddScoped(typeof(T));
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService"/> to the specified
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <typeparam name="TImplementation"> The type of the implementation to add. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.AddScoped(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Adds a scoped service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection AddScoped(this IServiceCollection serviceCollection, Type serviceType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            return serviceCollection.AddScoped(serviceType, serviceType);
        }

        /// <summary>
        /// Adds a scoped service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <param name="implementationType"> The type of the implementation to add. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection AddScoped(this IServiceCollection serviceCollection, Type serviceType, Type implementationType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            return Add(serviceCollection, ServiceDescriptor.Scoped(serviceType, implementationType));
        }

        /// <summary>
        /// Try to add a scoped service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddScoped<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.TryAddScoped(typeof(TService), typeof(TService));
        }

        /// <summary>
        /// Try to add a scoped service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <typeparam name="TImplementation"> The type of the implementation to add. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddScoped<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.TryAddScoped(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Try to add a scoped service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddScoped(this IServiceCollection serviceCollection, Type serviceType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            return serviceCollection.AddScoped(serviceType, serviceType);
        }

        /// <summary>
        /// Try to add a scoped service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <param name="implementationType"> The type of the implementation to add. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddScoped(this IServiceCollection serviceCollection, Type serviceType,
            Type implementationType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            return TryAdd(serviceCollection, ServiceDescriptor.Scoped(serviceType, implementationType));
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

            return Add(serviceCollection, ServiceDescriptor.Transient(serviceType, implementationType));
        }

        /// <summary>
        /// Try to add a transient service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddTransient<TService>(this IServiceCollection serviceCollection)
            where TService : class
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.TryAddScoped(typeof(TService), typeof(TService));
        }

        /// <summary>
        /// Try to add a transient service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <typeparam name="TService"> The type of the service to add. </typeparam>
        /// <typeparam name="TImplementation"> The type of the implementation to add. </typeparam>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddTransient<TService, TImplementation>(this IServiceCollection serviceCollection)
            where TService : class
            where TImplementation : class, TService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return serviceCollection.TryAddScoped(typeof(TService), typeof(TImplementation));
        }

        /// <summary>
        /// Try to add a transient service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddTransient(this IServiceCollection serviceCollection, Type serviceType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            return serviceCollection.AddScoped(serviceType, serviceType);
        }

        /// <summary>
        /// Try to add a transient service of a given type to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to add the service to. </param>
        /// <param name="serviceType"> The type of the service to add. </param>
        /// <param name="implementationType"> The type of the implementation to add. </param>
        /// <returns> The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static IServiceCollection TryAddTransient(this IServiceCollection serviceCollection, Type serviceType,
            Type implementationType)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            return TryAdd(serviceCollection, ServiceDescriptor.Transient(serviceType, implementationType));
        }



        /// <summary>
        /// Builds a <see cref="IServiceProvider"/> from the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"> The <see cref="IServiceCollection"/> to build the <see cref="IServiceProvider"/> from. </param>
        /// <returns> The <see cref="IServiceProvider"/>. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when <paramref name="serviceCollection"/> is null. </exception>
        public static ServiceProvider BuildServiceProvider(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            return new ServiceProvider(serviceCollection);
        }


        static IServiceCollection TryAdd(IServiceCollection serviceCollection, ServiceDescriptor descriptor)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var count = serviceCollection.Count;
            for (var i = 0; i < count; i++)
            {
                if (serviceCollection[i].serviceType == descriptor.serviceType)
                    return serviceCollection;
            }

            serviceCollection.Add(descriptor);
            return serviceCollection;
        }

        static IServiceCollection Add(IServiceCollection serviceCollection, ServiceDescriptor serviceDescriptor)
        {
            serviceCollection.Add(serviceDescriptor);
            return serviceCollection;
        }

        /// <summary>
        /// Extension method to add conditional resolution to any ServiceDescriptor.
        /// </summary>
        /// <param name="descriptor"> The service descriptor to add condition to. </param>
        /// <param name="condition"> The condition for service resolution. </param>
        /// <returns> A new ServiceDescriptor with the condition applied. </returns>
        public static ServiceDescriptor When(this ServiceDescriptor descriptor, ContextMatch condition)
        {
            return descriptor.When(condition);
        }

        /// <summary>
        /// Adds a conditional singleton service to the collection.
        /// </summary>
        /// <param name="serviceCollection"> The service collection. </param>
        /// <param name="serviceType"> The service type. </param>
        /// <param name="implementationType"> The implementation type. </param>
        /// <param name="condition"> The condition for resolution. </param>
        /// <returns> The service collection for chaining. </returns>
        public static IServiceCollection AddSingletonWhen(this IServiceCollection serviceCollection, Type serviceType, Type implementationType, ContextMatch condition)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            var descriptor = ServiceDescriptor.Singleton(serviceType, implementationType).When(condition);
            return Add(serviceCollection, descriptor);
        }

        /// <summary>
        /// Adds a conditional scoped service to the collection.
        /// </summary>
        /// <param name="serviceCollection"> The service collection. </param>
        /// <param name="serviceType"> The service type. </param>
        /// <param name="implementationType"> The implementation type. </param>
        /// <param name="condition"> The condition for resolution. </param>
        /// <returns> The service collection for chaining. </returns>
        public static IServiceCollection AddScopedWhen(this IServiceCollection serviceCollection, Type serviceType, Type implementationType, ContextMatch condition)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            var descriptor = ServiceDescriptor.Scoped(serviceType, implementationType).When(condition);
            return Add(serviceCollection, descriptor);
        }

        /// <summary>
        /// Adds a conditional transient service to the collection.
        /// </summary>
        /// <param name="serviceCollection"> The service collection. </param>
        /// <param name="serviceType"> The service type. </param>
        /// <param name="implementationType"> The implementation type. </param>
        /// <param name="condition"> The condition for resolution. </param>
        /// <returns> The service collection for chaining. </returns>
        public static IServiceCollection AddTransientWhen(this IServiceCollection serviceCollection, Type serviceType, Type implementationType, ContextMatch condition)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            var descriptor = ServiceDescriptor.Transient(serviceType, implementationType).When(condition);
            return Add(serviceCollection, descriptor);
        }
    }
}
