using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.AppUI.MVVM;
using Unity.AppUI.Redux;
using UnityEngine;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public class MainViewModel : ObservableObject
    {
        const string k_SliceName = "app";

        readonly IStoreService m_StoreService;

        readonly ILocalStorageService m_LocalStorageService;

        readonly Unsubscriber m_Unsubscribe;

        Todo[] m_Todos;

        Todo[] m_TodosSearchResults;

        public RelayCommand<string> createTodoCommand { get; }

        public RelayCommand<Todo> deleteTodoCommand { get; }

        public RelayCommand<(Todo, string)> editTodoCommand { get; }

        public RelayCommand<Todo> toggleCompleteTodoCommand { get; }

        public AsyncRelayCommand<string> searchTodoCommand { get; }

        public Todo[] todos
        {
            get => m_Todos;
            set => SetProperty(ref m_Todos, value);
        }

        public Todo[] todosSearchResults
        {
            get => m_TodosSearchResults;
            set => SetProperty(ref m_TodosSearchResults, value);
        }

        public MainViewModel(IStoreService storeService, ILocalStorageService localStorageService)
        {
            // Services
            m_StoreService = storeService;
            m_LocalStorageService = localStorageService;

            // Commands
            createTodoCommand = new RelayCommand<string>(CreateTodo);
            deleteTodoCommand = new RelayCommand<Todo>(DeleteTodo);
            editTodoCommand = new RelayCommand<(Todo,string)>(EditTodo);
            toggleCompleteTodoCommand = new RelayCommand<Todo>(ToggleCompleteTodo);
            searchTodoCommand = new AsyncRelayCommand<string>(SearchTodo, AsyncRelayCommandOptions.None);

            // State
            var initialState = m_LocalStorageService.GetValue(k_SliceName, new AppState());
            m_StoreService.store.CreateSlice(k_SliceName, initialState, builder =>
            {
                builder
                    .Add<string>(Actions.createTodo, Reducers.CreateTodoReducer)
                    .Add<string>(Actions.deleteTodo, Reducers.DeleteTodoReducer)
                    .Add<(string, string)>(Actions.editTodo, Reducers.EditTodoReducer)
                    .Add<(string, bool)>(Actions.completeTodo, Reducers.CompleteTodoReducer)
                    .Add<string>(Actions.setSearchInput, Reducers.SetSearchInputReducer);
            });

            m_Todos = initialState.todos;

            // Events
            m_Unsubscribe = m_StoreService.store.Subscribe<AppState>(k_SliceName, OnStateChanged);
            App.shuttingDown += OnShuttingDown;
        }

        async Task SearchTodo(string input, CancellationToken cancellationToken)
        {
            // store the input in the state
            m_StoreService.store.Dispatch(Actions.setSearchInput, input);
            await SearchInternal(input, cancellationToken);
        }

        async Task SearchInternal(string input, CancellationToken cancellationToken)
        {
            // We could have a search service, but for the sake of the demo, we'll just filter the todos
            var result = new List<Todo>();
            foreach (var todo in todos)
            {
                if (todo.text.Contains(input))
                {
                    result.Add(todo);
                }
            }

            // Simulate a network request
            await Task.Delay(300, cancellationToken);

            todosSearchResults = result.ToArray();
        }

        async void OnStateChanged(AppState state)
        {
            if (state.todos != todos)
            {
                todos = state.todos;
                await SearchInternal(state.searchInput, CancellationToken.None);
            }
        }

        void CreateTodo(string text)
        {
            m_StoreService.store.Dispatch(Actions.createTodo, text);
        }

        void ToggleCompleteTodo(Todo todo)
        {
            m_StoreService.store.Dispatch(Actions.completeTodo, (todo.id, todo.completed));
        }

        void EditTodo((Todo todo, string newName) args)
        {
            m_StoreService.store.Dispatch(Actions.editTodo, (args.todo.id, args.newName));
        }

        void DeleteTodo(Todo todo)
        {
            m_StoreService.store.Dispatch(Actions.deleteTodo, todo.id);
        }

        void OnShuttingDown()
        {
            m_LocalStorageService?.SetValue(k_SliceName, m_StoreService.store.GetState<AppState>(k_SliceName));
            App.shuttingDown -= OnShuttingDown;
            m_Unsubscribe?.Invoke();
        }
    }
}
