using UnityEngine;
using Unity.AppUI.MVVM;

namespace Unity.AppUI.Samples.MVVM
{
    public class MyApp : App
    {
        public new static MyApp current => (MyApp)App.current;

        public override void InitializeComponent()
        {
            base.InitializeComponent();
            rootVisualElement.Add(services.GetRequiredService<MainPage>());
        }

        public override void Shutdown()
        {
            base.Shutdown();
            Debug.Log("MyApp.Shutdown");
        }
    }
}
