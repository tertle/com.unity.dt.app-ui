using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// A builder to create an App instance with required services.
    /// </summary>
    public class AppBuilder
    {
        readonly AppUIServiceCollection m_Services = new AppUIServiceCollection();

        /// <summary>
        /// The services of the application.
        /// </summary>
        public IServiceCollection services => m_Services;

        /// <summary>
        /// Private constructor to prevent instantiation.
        /// </summary>
        AppBuilder() { }

        /// <summary>
        /// Instantiates a new AppBuilder with the default services according to the given App type.
        /// </summary>
        /// <typeparam name="T"> The type of the app to build. It is expected that this type is a subclass of <see cref="App"/>. </typeparam>
        /// <returns> The instantiated AppBuilder. </returns>
        public static AppBuilder InstantiateWith<T>() where T : class, IApp
        {
            var builder = new AppBuilder();
            builder.services.TryAddSingleton<IApp, T>();
            builder.SetupDefaults();
            return builder;
        }

        /// <summary>
        /// Build and initialize the app with the given host.
        /// </summary>
        /// <param name="host"> The host to use. </param>
        /// <returns> The built app instance. </returns>
        public IApp BuildWith(IHost host)
        {
            var serviceProvider = m_Services.BuildServiceProvider();
            m_Services.IsReadOnly = true;
            var app = serviceProvider.GetRequiredService<IApp>();
            app.Initialize(serviceProvider, host);
            return app;
        }

        void SetupDefaults()
        {
            // todo do some initialization here
        }
    }
}
