using System;
using System.Collections.Generic;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Interface for an application.
    /// </summary>
    public interface IApp : IDisposable, IInitializableComponent
    {
        /// <summary>
        /// The available services.
        /// </summary>
        public IServiceProvider services { get; }

        /// <summary>
        /// Called to shutdown the application.
        /// </summary>
        void Shutdown();
    }

    /// <summary>
    /// Interface for an application.
    /// </summary>
    /// <typeparam name="THostType"> The type of the host. </typeparam>
    public interface IApp<THostType> : IApp
        where THostType : class, IHost
    {
        /// <summary>
        /// The hosts of the application.
        /// </summary>
        IEnumerable<THostType> hosts { get; }

        /// <summary>
        /// Called to initialize the application.
        /// </summary>
        /// <param name="serviceProvider"> The service provider to use. </param>
        /// <param name="host"> The host to use. </param>
        void Initialize(IServiceProvider serviceProvider, THostType host = null);
    }
}
