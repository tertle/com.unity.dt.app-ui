using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.AppUI.MVVM;
using Unity.AppUI.Redux;
using UnityEngine;

namespace Unity.AppUI.Samples.MVVMRedux
{
    [ObservableObject]
    public partial class MainViewModel
    {
        readonly IStoreService m_StoreService;

        readonly ILocalStorageService m_LocalStorageService;

        IDisposableSubscription m_Subscription;

        [ObservableProperty]
        Todo[] m_Todos;

        [ObservableProperty]
        Todo[] m_TodosSearchResults;

        public MainViewModel(IStoreService storeService, ILocalStorageService localStorageService)
        {
            // Services
            m_StoreService = storeService;
            m_LocalStorageService = localStorageService;

            // State
            m_Todos = m_StoreService.store.GetState<AppState>(m_StoreService.sliceName).todos;

            // Events
            m_Subscription = m_StoreService.store.Subscribe(SelectAppSlice, OnStateChanged);
            App.shuttingDown += OnShuttingDown;
        }

        AppState SelectAppSlice(PartitionedState state)
        {
            return state.Get<AppState>(m_StoreService.sliceName);
        }

        [ICommand]
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
            foreach (var todo in Todos)
            {
                if (todo.text.Contains(input))
                {
                    result.Add(todo);
                }
            }

            // Simulate a network request
            await Task.Delay(300, cancellationToken);

            TodosSearchResults = result.ToArray();
        }

        async void OnStateChanged(AppState state)
        {
            Debug.Log("Redux state has changed:\n" + state);

            if (state.todos != Todos)
            {
                Todos = state.todos;
                await SearchInternal(state.searchInput, CancellationToken.None);
            }
        }

        [ICommand]
        void CreateTodo(string text)
        {
            m_StoreService.store.Dispatch(Actions.createTodo, text);
        }

        [ICommand]
        void ToggleCompleteTodo(Todo todo)
        {
            m_StoreService.store.Dispatch(Actions.completeTodo, (todo.id, todo.completed));
        }

        [ICommand]
        void EditTodo((Todo todo, string newName) args)
        {
            m_StoreService.store.Dispatch(Actions.editTodo, (args.todo.id, args.newName));
        }

        [ICommand]
        void DeleteTodo(Todo todo)
        {
            m_StoreService.store.Dispatch(Actions.deleteTodo, todo.id);
        }

        void OnShuttingDown()
        {
            m_StoreService.SaveState();
            App.shuttingDown -= OnShuttingDown;
            m_Subscription?.Dispose();
            m_Subscription = null;
        }
    }
}
