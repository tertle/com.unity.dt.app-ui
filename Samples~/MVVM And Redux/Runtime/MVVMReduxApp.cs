using UnityEngine;
using Unity.AppUI.MVVM;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public class MVVMReduxApp : App
    {
        public new static MVVMReduxApp current => (MVVMReduxApp)App.current;

        public override void InitializeComponent()
        {
            base.InitializeComponent();
            rootVisualElement.Add(services.GetRequiredService<MainPage>());
        }
    }
}
