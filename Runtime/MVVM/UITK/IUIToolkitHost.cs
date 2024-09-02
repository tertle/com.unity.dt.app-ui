using System;
using UnityEngine.UIElements;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// Interface for the host of the application interface using UI Toolkit.
    /// </summary>
    public interface IUIToolkitHost : IHost
    {
        /// <summary>
        /// Hosts the application.
        /// </summary>
        /// <param name="app"> The app to host. </param>
        /// <param name="serviceProvider"> The service provider to use. </param>
        public void HostApplication(IUIToolkitApp app, IServiceProvider serviceProvider);

        /// <summary>
        /// Tries to find an element of the given type in the visual tree of the app.
        /// </summary>
        /// <param name="element"> The element that was found. </param>
        /// <typeparam name="T"> The type of the element to find. </typeparam>
        /// <returns> True if the element was found, false otherwise. </returns>
        bool TryFindElement<T>(out T element) where T : VisualElement;
    }
}
