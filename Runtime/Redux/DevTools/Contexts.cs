using Unity.AppUI.Core;

namespace Unity.AppUI.Redux.DevTools
{
    record StoreContext<TState>(IStore<TState> store) : IContext
    {
        public IStore<TState> store { get; } = store;
    }
}
