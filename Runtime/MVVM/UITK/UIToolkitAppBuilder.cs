using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// <para>A MonoBehaviour that can be used to build and host an app in a UIDocument.</para>
    /// <para>This class is intended to be used as a base class for a MonoBehaviour that is attached to a GameObject in a scene.</para>
    /// </summary>
    /// <typeparam name="T"> The type of the app to build. It is expected that this type is a subclass of <see cref="App"/>. </typeparam>
    public class UIToolkitAppBuilder<T> : MonoBehaviour where T : App
    {
        /// <summary>
        /// The UIDocument to host the app in.
        /// </summary>
        [Tooltip("The UIDocument to host the app in.")]
        public UIDocument uiDocument;

        void OnEnable()
        {
            if (!uiDocument)
            {
                Debug.LogWarning("No UIDocument assigned to Program component. Aborting App startup.");
                return;
            }

            var builder = AppBuilder.InstantiateWith<T, UIToolkitHost>();
            OnConfiguringApp(builder);

            var host = new UIToolkitHost(uiDocument);
            var app = (T)builder.BuildWith(host);
            OnAppInitialized(app);
        }

        /// <summary>
        /// Called when the app builder is being configured.
        /// </summary>
        /// <param name="builder"> The app builder. </param>
        protected virtual void OnConfiguringApp(AppBuilder builder)
        {

        }

        /// <summary>
        /// Called when the app has been initialized.
        /// </summary>
        /// <param name="app"> The app that was initialized. </param>
        protected virtual void OnAppInitialized(T app)
        {

        }

        /// <summary>
        /// Called when the app is shutting down.
        /// </summary>
        /// <param name="app"> The app that is shutting down. </param>
        protected virtual void OnAppShuttingDown(T app)
        {

        }

        void OnDisable()
        {
            if (App.current is not T app)
                return;

            OnAppShuttingDown(app);
            if (uiDocument)
                uiDocument.rootVisualElement?.Clear();
            app.Shutdown();
            app.Dispose();
        }
    }
}
