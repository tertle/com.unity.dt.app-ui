using Unity.AppUI.MVVM;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public class MVVMReduxAppBuilder : UIToolkitAppBuilder<MVVMReduxApp>
    {
        public VisualTreeAsset mainUITemplate;

        public VisualTreeAsset todoItemTemplate;

        internal static MVVMReduxAppBuilder instance { get; private set; }

        protected override void OnConfiguringApp(AppBuilder builder)
        {
            base.OnConfiguringApp(builder);

            instance = this;

            // Services
            builder.services.AddSingleton<ILocalStorageService, LocalStorageService>();
            builder.services.AddSingleton<IStoreService, StoreService>();

            // ViewModels
            builder.services.AddTransient<MainViewModel>();

            // Views
            builder.services.AddTransient<MainPage>();
        }
    }
}
