using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Represents a scope for service resolution.
    /// </summary>
    public class ServiceScope : IServiceScope
    {
        readonly IServiceProvider m_ParentServiceProvider;
        readonly IServiceProvider m_ServiceProvider;

        /// <inheritdoc />
        public IServiceProvider ServiceProvider => m_ServiceProvider;

        internal ServiceScope(ServiceProvider parent, IServiceCollection serviceCollection)
        {
            m_ParentServiceProvider = parent;
            m_ServiceProvider = new ServiceProvider(serviceCollection, parent);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (ServiceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
