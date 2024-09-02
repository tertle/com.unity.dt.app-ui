// This file draws upon the concepts found in the ServiceDescriptor implementation from the .NET Runtime library (dotnet/runtime),
// more information in Third Party Notices.md

using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Describes a service with its service type, implementation, and lifetime.
    /// </summary>
    public class ServiceDescriptor
    {
        /// <summary>
        /// Gets the implementation type of the service.
        /// </summary>
        public Type implementationType { get; private set; }

        /// <summary>
        /// Gets the lifetime of the service.
        /// </summary>
        public ServiceLifetime lifetime { get; private set; }

        /// <summary>
        /// Gets the service type.
        /// </summary>
        public Type serviceType { get; private set; }

        ServiceDescriptor(Type serviceType, ServiceLifetime lifetime)
        {
            this.lifetime = lifetime;
            this.serviceType = serviceType;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ServiceDescriptor"/>.
        /// </summary>
        /// <param name="serviceType"> The service type. </param>
        /// <param name="implementationType"> The implementation type. </param>
        /// <param name="lifetime"> The lifetime of the service. </param>
        /// <exception cref="ArgumentNullException"> Thrown if <paramref name="serviceType"/> or <paramref name="implementationType"/> is null. </exception>
        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
            : this(serviceType, lifetime)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            this.implementationType = implementationType;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ServiceDescriptor"/> as a singleton.
        /// </summary>
        /// <param name="serviceType"> The service type. </param>
        /// <param name="implementationType"> The implementation type. </param>
        /// <returns> The service descriptor. </returns>
        public static ServiceDescriptor Singleton(Type serviceType, Type implementationType)
        {
            return Describe(serviceType, implementationType, ServiceLifetime.Singleton);
        }

        static ServiceDescriptor Describe(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            return new ServiceDescriptor(serviceType, implementationType, lifetime);
        }
    }
}
