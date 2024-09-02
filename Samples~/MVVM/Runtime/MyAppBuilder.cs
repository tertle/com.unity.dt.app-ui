using Unity.AppUI.MVVM;
using UnityEngine;

namespace Unity.AppUI.Samples.MVVM
{
    public class MyAppBuilder : UIToolkitAppBuilder<MyApp>
    {
        protected override void OnAppInitialized(MyApp app)
        {
            base.OnAppInitialized(app);
            Debug.Log("MyAppBuilder.OnAppInitialized");
        }

        protected override void OnConfiguringApp(AppBuilder builder)
        {
            base.OnConfiguringApp(builder);
            Debug.Log("MyAppBuilder.OnConfiguringApp");

            // Add services here

            // Add ViewModels and Views as services
            builder.services.AddSingleton<MainViewModel>();
            builder.services.AddSingleton<MainPage>();
        }

        protected override void OnAppShuttingDown(MyApp app)
        {
            base.OnAppShuttingDown(app);
            Debug.Log("MyAppBuilder.OnAppShuttingDown");
        }
    }
}
