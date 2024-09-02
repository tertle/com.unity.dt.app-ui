namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Use this interface to be notified when all dependencies have been injected into your service.
    /// </summary>
    public interface IDependencyInjectionListener
    {
        /// <summary>
        /// Called when all dependencies have been injected into the service.
        /// </summary>
        void OnDependenciesInjected();
    }
}
