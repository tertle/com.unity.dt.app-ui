using NUnit.Framework;
using Unity.AppUI.MVVM;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.MVVM
{
    class TestableUITKAppMonoBehaviour : UIToolkitAppBuilder<TestableUITKAppMonoBehaviour.TestApp>
    {
        internal class TestApp : App
        {
            public TestApp(MainPage mainPage)
            {
                var panel = new AppUI.UI.Panel();
                panel.Add(mainPage);
                this.rootVisualElement = panel;
            }
        }

        internal class MainPage : VisualElement
        {
            public MainPage() { } // explicit default constructor required for dependency injection
        }

        protected override void OnConfiguringApp(AppBuilder builder)
        {
            base.OnConfiguringApp(builder);

            builder.services.AddSingleton<MainPage>();
        }
    }
}
