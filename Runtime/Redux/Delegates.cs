using System.Threading.Tasks;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// A function that takes the current state and an action, and returns a new state.
    /// </summary>
    /// <typeparam name="TState"> The type of the state. </typeparam>
    /// <param name="state"> The current state. </param>
    /// <param name="action"> The Action to handle </param>
    /// <returns> The new state. </returns>
    public delegate TState Reducer<TState>(TState state, IAction action);

    /// <summary>
    /// A function that takes the current state and an action with a payload, and returns a new state.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the action payload. </typeparam>
    /// <typeparam name="TState"> The type of the state. </typeparam>
    /// <param name="state"> The current state. </param>
    /// <param name="action"> The Action to handle </param>
    /// <returns> The new state. </returns>
    public delegate TState Reducer<TPayload,TState>(TState state, IAction<TPayload> action);

    /// <summary>
    /// A predicate function that takes an action and returns true if the action should be handled by the reducer.
    /// </summary>
    /// <param name="action"> The Action to handle. </param>
    /// <returns> True if there's a match, false otherwise. </returns>
    public delegate bool ActionMatcher(IAction action);

    /// <summary>
    /// A function to enhance the store creation.
    /// </summary>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    /// <param name="createStore"> The store creation function. </param>
    /// <returns> The enhanced store creation function. </returns>
    /// <example>
    /// <para>
    /// The following example shows how to enhance the store creation to replace the dispatcher with a custom dispatcher.
    /// </para>
    /// <code>
    /// // Enhancer that logs the action and the state before and after the action.
    /// var enhancer = (createStore) => (reducer, initialState) =>
    /// {
    ///     var store = createStore(reducer, initialState);
    ///     var originalDispatcher = store.dispatcher;
    ///     store.dispatcher = new Dispatcher(action =>
    ///     {
    ///         Debug.Log($"Dispatching action: {action}");
    ///         originalDispatcher(action);
    ///         Debug.Log($"State after action: {store.GetState()}");
    ///     });
    ///     return store;
    /// };
    /// // You can compose enhancers using Store.ComposeEnhancers
    /// var composedEnhancer = Store.ComposeEnhancers(enhancer, DefaultEnhancer.GetDefaultEnhancer());
    /// // Create the store with one enhancer or a composed enhancer
    /// var store = Store.CreateStore(reducer, initialState, composedEnhancer);
    /// </code>
    /// </example>
    public delegate StoreEnhancerStoreCreator<TState> StoreEnhancer<TState>(StoreEnhancerStoreCreator<TState> createStore);

    /// <summary>
    /// A function that creates a new store. This is a parameter to the store enhancer.
    /// </summary>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    /// <param name="rootReducer"> The root reducer of the store. </param>
    /// <param name="initialState"> The initial state of the store. </param>
    /// <returns> The new store. </returns>
    /// <seealso cref="StoreEnhancer{TState}"/>
    public delegate IStore<TState> StoreEnhancerStoreCreator<TState>(Reducer<TState> rootReducer, TState initialState);

    /// <summary>
    /// A function that dispatches an action in the store to update the state.
    /// </summary>
    /// <param name="action"> The action to dispatch. </param>
    public delegate void Dispatcher(IAction action);

    /// <summary>
    /// A function that enhances the dispatcher. This is the returned type of a <see cref="Middleware{TStoreState}"/>
    /// and is used to compose the dispatcher with other middleware using <see cref="StoreFactory.ApplyMiddleware{TStoreState}"/>.
    /// </summary>
    /// <param name="dispatcher"> The dispatcher to enhance. </param>
    /// <returns> The enhanced dispatcher. </returns>
    /// <seealso cref="Middleware{TStoreState}"/>
    /// <seealso cref="StoreFactory.ApplyMiddleware{TStoreState}"/>
    public delegate Dispatcher DispatcherComposer(Dispatcher dispatcher);

    /// <summary>
    /// A Redux middleware is a higher-order function that composes a dispatch function to return a new dispatch function.
    /// </summary>
    /// <typeparam name="TStoreState"> The type of the store state. </typeparam>
    /// <param name="store"> The store. </param>
    /// <returns> The dispatcher composer. </returns>
    /// <example>
    /// <para>
    /// The following example shows how to create a middleware that logs all actions.
    /// </para>
    /// <code>
    /// // Middleware that logs all actions
    /// var loggerMiddleware = (store) => next => action =>
    /// {
    ///    Debug.Log($"Dispatching action: {action}");
    ///    // Call the next middleware in the chain
    ///    next(action);
    /// };
    /// // Apply the middleware to the store
    /// var middlewares = Store.ApplyMiddlewares(loggerMiddleware);
    /// var enhancer = Store.ComposeEnhancers(middlewares, DefaultEnhancer.GetDefaultEnhancer());
    /// var store = Store.CreateStore(reducer, initialState, enhancer);
    /// </code>
    /// </example>
    /// <seealso cref="StoreFactory.ApplyMiddleware{TStoreState}"/>
    public delegate DispatcherComposer Middleware<TStoreState>(IStore<TStoreState> store);

    /// <summary>
    /// A listener for the store state change.
    /// </summary>
    /// <typeparam name="TState"> The type of the state. </typeparam>
    /// <param name="state"> The state. </param>
    public delegate void Listener<in TState>(TState state);

    /// <summary>
    /// A selector for a state.
    /// </summary>
    /// <typeparam name="TState"> The type of the state. </typeparam>
    /// <typeparam name="TSelected"> The type of the selected part of the state. </typeparam>
    /// <param name="state"> The state. </param>
    /// <returns> The selected part of the state. </returns>
    public delegate TSelected Selector<in TState,out TSelected>(TState state);

    /// <summary>
    /// The thunk task runner.
    /// </summary>
    /// <typeparam name="TArg"> The type of the thunk argument. </typeparam>
    /// <typeparam name="TPayload"> The type of the action payload. </typeparam>
    /// <param name="api"> The thunk API. </param>
    /// <returns> The task. </returns>
    public delegate Task<TPayload> AsyncThunk<in TArg,TPayload>(IThunkAPI<TArg,TPayload> api);

    /// <summary>
    /// The thunk task runner.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the action payload. </typeparam>
    /// <param name="api"> The thunk API. </param>
    /// <returns> The task. </returns>
    public delegate Task<TPayload> AsyncThunk<TPayload>(IThunkAPI<TPayload> api);
}
