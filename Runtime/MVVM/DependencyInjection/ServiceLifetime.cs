// This file draws upon the concepts found in the ServiceLifetime implementation from the .NET Runtime library (dotnet/runtime),
// more information in Third Party Notices.md

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// The lifetime of a service.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// A single instance of the service will be created.
        /// </summary>
        Singleton,

        /// <summary>
        /// Specifies that a new instance of the service will be created for each scope.
        /// </summary>
        Scoped,

        /// <summary>
        /// A new instance of the service will be created each time it is requested.
        /// </summary>
        Transient
    }
}
