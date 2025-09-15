using System.ComponentModel;
using Unity.AppUI.UI;
using UnityEngine.UIElements;
using Button = Unity.AppUI.UI.Button;
#if UNITY_2023_2_OR_NEWER
using Unity.Properties;
#endif

namespace Unity.AppUI.Samples.MVVM
{
    public class MainPage : VisualElement
    {
        readonly Text m_Text;
        readonly Button m_Button;
        readonly MainViewModel m_BindingContext;

        public MainPage(MainViewModel viewModel)
        {
            style.marginBottom = 100;
            style.marginLeft = 100;
            style.marginRight = 100;
            style.marginTop = 100;

            m_BindingContext = viewModel;
            m_Text = new Text("Click count: 0");
            m_Button = new Button(m_BindingContext.IncrementCounterCommand.Execute) { title = "Increment" };
            Add(m_Text);
            Add(m_Button);

            // Starting Unity 2023.2, we can use UITK data binding to update the text.
#if UNITY_2023_2_OR_NEWER
            dataSource = m_BindingContext;
            m_Text.SetBinding(nameof(Text.text), new DataBinding
            {
                bindingMode = BindingMode.ToTarget,
                dataSourcePath = PropertyPath.FromName(nameof(MainViewModel.ClickCountMessage))
            });
#else       // Before Unity 2023.2, we need to manually update the text via events.
            m_BindingContext.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.ClickCountMessage))
                    m_Text.text = m_BindingContext.ClickCountMessage;
            };
#endif
        }
    }
}
