using Unity.AppUI.Redux;

namespace Unity.AppUI.Samples.MVVMRedux
{
    public class StoreService : IStoreService
    {
        readonly ILocalStorageService m_LocalStorageService;

        public IStore<PartitionedState> store { get; }

        public string sliceName => "app";

        public void SaveState()
        {
            var state = store.GetState<AppState>(sliceName);
            m_LocalStorageService.SetValue(sliceName, state);
        }

        public StoreService(ILocalStorageService localStorageService)
        {
            m_LocalStorageService = localStorageService;
            var initialState = localStorageService.GetValue(sliceName, new AppState());
            store = StoreFactory.CreateStore(new []
            {
                StoreFactory.CreateSlice<AppState>(sliceName, initialState, builder =>
                {
                    builder
                        .AddCase(Actions.createTodo, Reducers.CreateTodoReducer)
                        .AddCase(Actions.deleteTodo, Reducers.DeleteTodoReducer)
                        .AddCase(Actions.editTodo, Reducers.EditTodoReducer)
                        .AddCase(Actions.completeTodo, Reducers.CompleteTodoReducer)
                        .AddCase(Actions.setSearchInput, Reducers.SetSearchInputReducer);
                })
            });
        }
    }
}
