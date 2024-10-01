using System;

namespace Unity.AppUI.MVVM
{
    /// <summary>
    /// A builder to create an App instance with required services.
    /// </summary>
    public class AppBuilder : IAppBuilder
    {
        readonly AppUIServiceCollection m_Services = new AppUIServiceCollection();

        /// <inheritdoc />
        public IServiceCollection services => m_Services;

        /// <summary>
        /// Private constructor to prevent instantiation.
        /// </summary>
        AppBuilder() { }

        /// <summary>
        /// Instantiates a new AppBuilder with the default services according to the given App type.
        /// </summary>
        /// <typeparam name="TAppType"> The type of the app to build. It is expected that this type is a subclass of <see cref="App"/>. </typeparam>
        /// <typeparam name="THostType"> The type of the host to use. </typeparam>
        /// <returns> The instantiated AppBuilder. </returns>
        public static AppBuilder InstantiateWith<TAppType,THostType>()
            where TAppType : class, IApp<THostType>
            where THostType : class, IHost
        {
            var builder = new AppBuilder();
            builder.services.TryAddSingleton<IApp<THostType>, TAppType>();
            builder.SetupDefaults();
            return builder;
        }

        /// <summary>
        /// Build and initialize the app with the given host.
        /// </summary>
        /// <typeparam name="THostType"> The type of the host to use. </typeparam>
        /// <param name="host"> The host to use. </param>
        /// <returns> The built app instance. </returns>
        public IApp<THostType> BuildWith<THostType>(THostType host)
            where THostType : class, IHost
        {
            var serviceProvider = m_Services.BuildServiceProvider();
            m_Services.IsReadOnly = true;
            var app = serviceProvider.GetRequiredService<IApp<THostType>>();
            app.Initialize(serviceProvider, host);
            return app;
        }

        void SetupDefaults()
        {
            // todo do some initialization here
        }
    }
}
