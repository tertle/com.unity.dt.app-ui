using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Interface for an application builder.
    /// </summary>
    public interface IAppBuilder
    {
        /// <summary>
        /// The available services of the application.
        /// </summary>
        /// <remarks>
        /// The services can be registered in the builder before building the service provider.
        /// After building the service provider, the services become read-only.
        /// </remarks>
        public IServiceCollection services { get; }
    }
}
