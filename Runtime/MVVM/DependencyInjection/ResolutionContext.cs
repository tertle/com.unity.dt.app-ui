using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Context information available during service resolution for conditional binding.
    /// </summary>
    public class ResolutionContext
    {
        /// <summary>
        /// The type being requested.
        /// </summary>
        public Type ServiceType { get; internal set; }

        /// <summary>
        /// The type that is requesting this service (if available).
        /// </summary>
        public Type RequestingType { get; internal set; }

        /// <summary>
        /// The service provider performing the resolution.
        /// </summary>
        public IServiceProvider ServiceProvider { get; internal set; }

        /// <summary>
        /// Whether this resolution is happening in a scoped context.
        /// </summary>
        public bool IsScoped { get; internal set; }

        internal ResolutionContext(Type serviceType, IServiceProvider serviceProvider, bool isScoped, Type requestingType = null)
        {
            ServiceType = serviceType;
            ServiceProvider = serviceProvider;
            IsScoped = isScoped;
            RequestingType = requestingType;
        }
    }

    /// <summary>
    /// Delegate type for conditional service resolution.
    /// </summary>
    /// <param name="context">The resolution context.</param>
    /// <returns>True if this service binding should be used.</returns>
    public delegate bool ContextMatch(ResolutionContext context);
}