using System.ComponentModel;
using Unity.AppUI.UI;
using UnityEngine.UIElements;
using Button = Unity.AppUI.UI.Button;

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
            m_Button = new Button { title = "Increment" };
            Add(m_Text);
            Add(m_Button);

            m_BindingContext.PropertyChanged += OnBindingContextPropertyChanged;
            m_Button.clicked += OnButtonClicked;
        }

        void OnButtonClicked()
        {
            m_BindingContext.IncrementCounter();
        }

        void OnBindingContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.counter))
            {
                m_Text.text = $"Click count: {m_BindingContext.counter}";
            }
        }
    }
}
