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

        /// <summary>
        /// Gets the condition for conditional service resolution.
        /// </summary>
        public ContextMatch condition { get; private set; }

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
        /// Creates a new instance of <see cref="ServiceDescriptor"/> with a condition.
        /// </summary>
        /// <param name="serviceType"> The service type. </param>
        /// <param name="implementationType"> The implementation type. </param>
        /// <param name="lifetime"> The lifetime of the service. </param>
        /// <param name="condition"> The condition for service resolution. </param>
        /// <exception cref="ArgumentNullException"> Thrown if <paramref name="serviceType"/> or <paramref name="implementationType"/> is null. </exception>
        public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime, ContextMatch condition)
            : this(serviceType, implementationType, lifetime)
        {
            this.condition = condition;
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

        /// <summary>
        /// Creates a new instance of <see cref="ServiceDescriptor"/> as a scoped.
        /// </summary>
        /// <param name="serviceType"> The service type. </param>
        /// <param name="implementationType"> The implementation type. </param>
        /// <returns> The service descriptor </returns>
        public static ServiceDescriptor Scoped(Type serviceType, Type implementationType)
        {
            return Describe(serviceType, implementationType, ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ServiceDescriptor"/> as a transient.
        /// </summary>
        /// <param name="type"> The service type. </param>
        /// <param name="implementationType"> The implementation type. </param>
        /// <returns> The service descriptor. </returns>
        public static ServiceDescriptor Transient(Type type, Type implementationType)
        {
            return Describe(type, implementationType, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Creates a copy of this descriptor with a condition applied.
        /// </summary>
        /// <param name="newCondition"> The condition to apply. </param>
        /// <returns> A new service descriptor with the condition. </returns>
        public ServiceDescriptor When(ContextMatch newCondition)
        {
            return Describe(serviceType, implementationType, lifetime, newCondition);
        }

        static ServiceDescriptor Describe(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            return new ServiceDescriptor(serviceType, implementationType, lifetime);
        }

        static ServiceDescriptor Describe(Type serviceType, Type implementationType, ServiceLifetime lifetime, ContextMatch condition)
        {
            return new ServiceDescriptor(serviceType, implementationType, lifetime, condition);
        }
    }
}
