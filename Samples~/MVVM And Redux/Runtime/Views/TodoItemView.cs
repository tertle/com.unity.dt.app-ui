using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public class TodoItemView : VisualElement
    {
        object m_DataContext;

        Checkbox m_Checkbox;

        ActionButton m_DeleteButton;

        ActionButton m_EditButton;

        public object dataContext
        {
            get => m_DataContext;
            set
            {
                m_DataContext = value;
                BindDataContext();
            }
        }

        void BindDataContext()
        {
            // clear
            m_Checkbox.UnregisterValueChangedCallback(OnToggled);
            m_Checkbox.label = "";
            m_DeleteButton.clickable.clicked -= OnDeleteButtonClicked;
            m_EditButton.clickable.clicked -= OnEditButtonClicked;

            if (m_DataContext == null)
                return;

            var data = viewModel;
            m_Checkbox.RegisterValueChangedCallback(OnToggled);
            m_Checkbox.label = data.todo.text;
            m_Checkbox.SetValueWithoutNotify(data.completed ? CheckboxState.Checked : CheckboxState.Unchecked);
            m_DeleteButton.clickable.clicked += OnDeleteButtonClicked;
            m_EditButton.clickable.clicked += OnEditButtonClicked;
        }

        void OnEditButtonClicked()
        {
            var dlg = new AlertDialog
            {
                title = "Edit Todo",
                variant = AlertSemantic.Information,
                description = "Enter a new name for the todo"
            };
            var textField = new UI.TextField();
            textField.SetValueWithoutNotify(m_Checkbox.label);
            dlg.Add(textField);
            dlg.SetPrimaryAction(1234, "Rename", () =>
            {
                if (m_Checkbox.label != textField.value && !string.IsNullOrWhiteSpace(textField.value))
                    viewModel.editCommand.Execute(textField.value);
            });
            dlg.SetCancelAction(1122, "Cancel");
            var modal = Modal.Build(this, dlg);
            modal.Show();
        }

        void OnDeleteButtonClicked()
        {
            viewModel.deleteCommand.Execute();
        }

        void OnToggled(ChangeEvent<CheckboxState> evt)
        {
            viewModel.completed = evt.newValue == CheckboxState.Checked;
        }

        TodoItemViewModel viewModel => (TodoItemViewModel) dataContext;

        public TodoItemView()
        {
            InitializeComponents();
        }

        void InitializeComponents()
        {
            var visualTree = MVVMReduxAppBuilder.instance.todoItemTemplate;
            visualTree.CloneTree(this);

            m_Checkbox = this.Q<Checkbox>("checkbox");
            m_DeleteButton = this.Q<ActionButton>("deleteButton");
            m_EditButton = this.Q<ActionButton>("editButton");
        }
    }
}
