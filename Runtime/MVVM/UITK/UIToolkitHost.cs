using System;
using UnityEngine.UIElements;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// <para>A class used to host an app in a UIDocument.</para>
    /// <para>This is a wrapper around a UIDocument that implements <see cref="IUIToolkitHost"/>.</para>
    /// </summary>
    public sealed class UIToolkitHost : IUIToolkitHost
    {
        UIDocument m_Document;

        bool m_Disposed;

        /// <summary>
        /// Default constructor. Creates a new instance of <see cref="UIToolkitHost"/> that hosts an app in the given UIDocument.
        /// </summary>
        /// <param name="uiDocument"> The UIDocument to host the app in. </param>
        public UIToolkitHost(UIDocument uiDocument)
        {
            m_Document = uiDocument;
        }

        /// <summary>
        /// <para>Called when the app is being hosted.</para>
        /// <para>A service provider is provided that can be used to resolve services through the host itself.</para>
        /// </summary>
        /// <param name="app"> The app to host. </param>
        /// <param name="serviceProvider"> The service provider to use. </param>
        public void HostApplication(IUIToolkitApp app, IServiceProvider serviceProvider)
        {
            m_Document.rootVisualElement?.Clear();
            m_Document.rootVisualElement?.Add(app.rootVisualElement);
        }

        /// <summary>
        /// Tries to find an element of the given type in the visual tree of the app.
        /// </summary>
        /// <param name="element"> The element that was found. </param>
        /// <typeparam name="T"> The type of the element to find. </typeparam>
        /// <returns> True if the element was found, false otherwise. </returns>
        public bool TryFindElement<T>(out T element)
            where T : VisualElement
        {
            if (m_Disposed || !m_Document)
            {
                element = null;
                return false;
            }

            element = m_Document.rootVisualElement.Q<T>();
            return element != null;
        }

        /// <summary>
        /// Disposes of the host.
        /// </summary>
        public void Dispose()
        {
            if (m_Disposed)
                return;

            m_Document = null;
            m_Disposed = true;
        }
    }
}
