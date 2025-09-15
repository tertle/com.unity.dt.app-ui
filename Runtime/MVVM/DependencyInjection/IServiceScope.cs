using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Represents a scope for service resolution.
    /// </summary>
    public interface IServiceScope : IDisposable
    {
        /// <summary>
        /// Gets the service provider for this scope.
        /// </summary>
        IServiceProvider ServiceProvider { get; }
    }
}
