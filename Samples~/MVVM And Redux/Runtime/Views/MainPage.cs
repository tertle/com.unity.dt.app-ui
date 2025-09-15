using System.ComponentModel;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;
using Button = Unity.AppUI.UI.Button;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public class MainPage : VisualElement
    {
        readonly MainViewModel m_ViewModel;

        ListView m_TodoListView;

        SearchBar m_SearchTextField;

        Text m_NothingFoundLabel;

        Button m_AddButton;

        public MainPage(MainViewModel viewModel)
        {
            m_ViewModel = viewModel;
            InitializeComponent();
            m_ViewModel.PropertyChanged += OnPropertyChanged;
            RefreshTodoList();
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.Todos))
            {
                RefreshTodoList();
            }
            else if (e.PropertyName == nameof(MainViewModel.TodosSearchResults))
            {
                RefreshTodoList();
            }
        }

        void RefreshTodoList()
        {
            if (string.IsNullOrEmpty(m_SearchTextField.value))
            {
                m_TodoListView.itemsSource = m_ViewModel.Todos;
            }
            else
            {
                m_TodoListView.itemsSource = m_ViewModel.TodosSearchResults;
            }

            var hasResults = m_TodoListView.itemsSource is {Count: > 0};
            m_NothingFoundLabel.EnableInClassList(Styles.hiddenUssClassName, hasResults);
        }

        void InitializeComponent()
        {
            var template = MVVMReduxAppBuilder.instance.mainUITemplate;
            template.CloneTree(this);

            m_SearchTextField = this.Q<SearchBar>();
            m_TodoListView = this.Q<ListView>();
            m_TodoListView.selectionType = SelectionType.None;
            m_TodoListView.fixedItemHeight = 60;
            m_TodoListView.bindItem = BindItem;
            m_TodoListView.makeItem = MakeItem;
            m_TodoListView.unbindItem = UnbindItem;
            m_NothingFoundLabel = this.Q<Text>("nothingFound");
            m_AddButton = this.Q<Button>("addButton");

            m_SearchTextField.RegisterValueChangingCallback(OnSearchTextChanged);
            m_AddButton.clicked += OnAddButtonClicked;
        }

        void UnbindItem(VisualElement element, int index)
        {
            var item = (TodoItemView) element;
            var modelView = (TodoItemViewModel) item.viewModel;
            modelView.PropertyChanged -= OnItemPropertyChanged;
            modelView.deleteRequested -= OnItemDeleteRequested;
            modelView.renamed -= OnItemEditRequested;
            item.viewModel = null;
        }

        VisualElement MakeItem()
        {
            return new TodoItemView();
        }

        void BindItem(VisualElement element, int index)
        {
            var todo = (Todo)m_TodoListView.itemsSource[index];
            var item = (TodoItemView) element;
            var viewModel = new TodoItemViewModel(todo);
            item.viewModel = viewModel;
            viewModel.PropertyChanged += OnItemPropertyChanged;
            viewModel.deleteRequested += OnItemDeleteRequested;
            viewModel.renamed += OnItemEditRequested;
        }

        void OnItemEditRequested((TodoItemViewModel vm, string newName) args)
        {
            m_ViewModel.EditTodoCommand.Execute((args.vm.todo, args.newName));
        }

        void OnItemDeleteRequested(TodoItemViewModel viewModel)
        {
            m_ViewModel.DeleteTodoCommand.Execute(viewModel.todo);
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TodoItemViewModel.completed))
            {
                var modelView = (TodoItemViewModel) sender;
                m_ViewModel.ToggleCompleteTodoCommand.Execute(modelView.todo);
            }
        }

        void OnAddButtonClicked()
        {
            var index = m_ViewModel.Todos?.Length ?? 0;
            m_ViewModel.CreateTodoCommand.Execute("New Todo " + (index + 1));
        }

        void OnSearchTextChanged(ChangingEvent<string> evt)
        {
            if (evt.newValue != evt.previousValue)
            {
                m_ViewModel.SearchTodoCommand.Execute(evt.newValue);
            }
        }
    }
}
