using System;
using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// A base class to implement an App instance using UI Toolkit.
    /// </summary>
    public class App : IUIToolkitApp
    {
        /// <summary>
        /// Event called when the application is shutting down.
        /// </summary>
        public static event Action shuttingDown;

        readonly List<UIToolkitHost> m_Hosts = new List<UIToolkitHost>();

        bool m_Disposed;

        IServiceProvider m_Services;

        /// <summary>
        /// The current App instance.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the current App instance is not available. </exception>
        public static App current { get; private set; }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~App() => Dispose(false);

        /// <summary>
        /// The main page of the application.
        /// </summary>
        public VisualElement rootVisualElement { get; set; }

        /// <summary>
        /// The services of the application.
        /// </summary>
        public IServiceProvider services => m_Services;

        /// <summary>
        /// The hosts of the application.
        /// </summary>
        public IEnumerable<UIToolkitHost> hosts => m_Hosts;

        /// <summary>
        /// Initializes the current App instance.
        /// </summary>
        /// <param name="serviceProvider"> The service provider to use. </param>
        /// <param name="host"> The host to use. </param>
        /// <exception cref="InvalidOperationException"> Thrown when a current App instance already exists. </exception>
        /// <exception cref="ArgumentNullException"> Thrown when serviceProvider is null. </exception>
        public void Initialize(IServiceProvider serviceProvider, UIToolkitHost host = null)
        {
            if (current != null)
                throw new InvalidOperationException($"An {nameof(App)} has been already initialized.");

            if (m_Hosts.Count > 0)
                throw new InvalidOperationException($"Trying to create the {nameof(App)} main window more than once.");

            m_Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            SetCurrentApp(this);
            if (host != null)
                m_Hosts.Add(host);
            InitializeComponent();
            host?.HostApplication(this, serviceProvider);
        }

        /// <summary>
        /// Called to shutdown the application.
        /// </summary>
        public virtual void Shutdown()
        {
            shuttingDown?.Invoke();
        }

        /// <summary>
        /// Disposes the current App instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public virtual void InitializeComponent()
        {
            rootVisualElement = new Panel();
        }

        /// <summary>
        /// Disposes the current App instance.
        /// </summary>
        /// <param name="disposing"> True to dispose managed resources. </param>
        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed)
                return;

            if (disposing)
            {
                for (var i = m_Hosts.Count - 1; i >= 0; i--)
                {
                    if (m_Hosts[i] != null)
                        m_Hosts[i].Dispose();
                }
                m_Hosts.Clear();
            }

            rootVisualElement = null;
            SetCurrentApp(null);
            m_Disposed = true;
        }

        static void SetCurrentApp(App app)
        {
            if (app != null && current != null)
                throw new InvalidOperationException($"There's a conflict between {nameof(App)} instances");
            current = app;
        }
    }
}
