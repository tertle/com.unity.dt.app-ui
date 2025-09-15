using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.Redux.DevTools;
using UnityEngine;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// Configuration for the default enhancer.
    /// </summary>
    /// <seealso cref="StoreFactory.DefaultEnhancer{TStoreState}"/>
    public class DefaultEnhancerConfiguration
    {
        /// <summary>
        /// The configuration for the dev tools.
        /// </summary>
        public DevToolsConfiguration devTools { get; set; } = new ();
    }

    /// <summary>
    /// Factory methods to create Redux stores.
    /// </summary>
    public static class StoreFactory
    {
        /// <summary>
        /// Creates a Redux store that holds the complete state tree of your app.
        /// </summary>
        /// <typeparam name="TState"> The type of the store state. </typeparam>
        /// <param name="rootReducer"> The root reducer of the store. </param>
        /// <param name="initialState"> The initial state of the store. </param>
        /// <param name="enhancer"> The enhancer for the store. </param>
        /// <returns> A new store. </returns>
        public static IStore<TState> CreateStore<TState>(
            Reducer<TState> rootReducer,
            TState initialState = default,
            StoreEnhancer<TState> enhancer = null)
            where TState : new()
        {
            rootReducer = rootReducer ?? throw new ArgumentNullException(nameof(rootReducer));
            initialState ??= new TState();
            enhancer ??= DefaultEnhancer<TState>();

            var baseCreator = new StoreEnhancerStoreCreator<TState>((r, s) => new Store<TState>(r, s));
            var createStore = enhancer(baseCreator);
            var store = createStore(rootReducer, initialState);
            store.Dispatch(new ActionCreator("@@redux/INIT").Invoke());
            return store;
        }

        /// <summary>
        /// Creates a Redux store that holds the complete state tree of your app.
        /// </summary>
        /// <param name="rootReducer"> The root reducer of the store. </param>
        /// <param name="initialState"> The initial state of the store. </param>
        /// <param name="enhancer"> The enhancer for the store. </param>
        /// <returns> A new store. </returns>
        /// <remarks>
        /// This is a specialization of
        /// <see cref="CreateStore{TStoreState}(Reducer{TStoreState},TStoreState,StoreEnhancer{TStoreState})"/>
        /// for a <see cref="Store{TState}"/> that uses <see cref="PartitionedState"/>.
        /// </remarks>
        public static IStore<PartitionedState> CreateStore(
            Reducer<PartitionedState> rootReducer,
            PartitionedState initialState = null,
            StoreEnhancer<PartitionedState> enhancer = null)
        {
            return CreateStore<PartitionedState>(rootReducer, initialState, enhancer);
        }

        /// <summary>
        /// Creates a Redux store composed of multiple slices.
        /// </summary>
        /// <param name="enhancer"> The enhancer for the store. </param>
        /// <param name="slices"> The slices that compose the store. </param>
        /// <typeparam name="TState"> The type of the store state. </typeparam>
        /// <returns> The new store. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if the slices are null. </exception>
        public static IStore<TState> CreateStore<TState>(
            IReadOnlyCollection<ISlice<TState>> slices,
            StoreEnhancer<TState> enhancer = null)
            where TState : IPartionableState<TState>, new()
        {
            if (slices == null)
                throw new ArgumentNullException(nameof(slices));

            var rootReducer = CombineReducers(slices);
            var initialState = new TState();
            foreach (var slice in slices)
            {
                initialState = slice.InitializeState(initialState);
            }

            return CreateStore(rootReducer, initialState, enhancer);
        }

        /// <summary>
        /// Creates a Redux store composed of multiple slices.
        /// </summary>
        /// <param name="enhancer"> The enhancer for the store. </param>
        /// <param name="slices"> The slices that compose the store. </param>
        /// <returns> The new store. </returns>
        /// <remarks>
        /// This is a specialization of
        /// <see cref="CreateStore{TStoreState}(IReadOnlyCollection{ISlice{TStoreState}},StoreEnhancer{TStoreState})"/>
        /// for a <see cref="Store{TState}"/> that uses <see cref="PartitionedState"/>.
        /// </remarks>
        public static IStore<PartitionedState> CreateStore(
            IReadOnlyCollection<ISlice<PartitionedState>> slices,
            StoreEnhancer<PartitionedState> enhancer = null)
        {
            return CreateStore<PartitionedState>(slices, enhancer);
        }

        /// <summary>
        /// Create a new state slice. A state slice is a part of the state tree.
        /// You can provide reducers that will "own" the state slice at the same time.
        /// </summary>
        /// <remarks>
        /// You can also provide extra reducers that will be called if the action type does not match any of
        /// the main reducers.
        /// </remarks>
        /// <param name="name"> The name of the state slice. </param>
        /// <param name="initialState"> The initial state of the state slice. </param>
        /// <param name="reducers"> The reducers that will "own" the state slice. </param>
        /// <param name="extraReducers"> The reducers that will be called if the action type does not match any of the
        /// main reducers. </param>
        /// <typeparam name="TState"> The type of the slice state. </typeparam>
        /// <returns> A slice object that can be used to access the state slice. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if the name is null or empty. </exception>
        public static Slice<TState,PartitionedState> CreateSlice<TState>(
            string name,
            TState initialState,
            System.Action<SliceReducerSwitchBuilder<TState>> reducers,
            System.Action<ReducerSwitchBuilder<TState>> extraReducers = null)
        {
            return new Slice<TState,PartitionedState>(name, initialState, reducers, extraReducers);
        }

        /// <summary>
        /// Create a reducer that combines multiple reducers into one.
        /// </summary>
        /// <typeparam name="TState"> The type of the state. </typeparam>
        /// <param name="reducers"> The reducers to combine. </param>
        /// <returns> A reducer that combines the given reducers. </returns>
        public static Reducer<TState> CombineReducers<TState>(IEnumerable<Reducer<TState>> reducers)
        {
            return CombinedReducers;

            TState CombinedReducers(TState state, IAction action)
            {
                var newState = state;

                foreach (var reducer in reducers)
                {
                    newState = reducer(newState, action);
                }

                return newState;
            }
        }

        /// <summary>
        /// Create a reducer that combines multiple reducers into one.
        /// </summary>
        /// <typeparam name="TState"> The type of the state. </typeparam>
        /// <param name="reducers"> The reducers to combine. </param>
        /// <returns> A reducer that combines the given reducers. </returns>
        public static Reducer<TState> CombineReducers<TState>(params Reducer<TState>[] reducers)
        {
            return CombinedReducers;

            TState CombinedReducers(TState state, IAction action)
            {
                var newState = state;

                foreach (var reducer in reducers)
                {
                    newState = reducer(newState, action);
                }

                return newState;
            }
        }

        /// <summary>
        /// Create a reducer that combines multiple slices into one.
        /// </summary>
        /// <param name="slices"> The slices to combine. </param>
        /// <typeparam name="TState"> The type of the store state. </typeparam>
        /// <returns> A reducer that combines the given slices. </returns>
        public static Reducer<TState> CombineReducers<TState>(IReadOnlyCollection<ISlice<TState>> slices)
        {
            return CombinedReducers;

            TState CombinedReducers(TState state, IAction action)
            {
                var newState = state;
                foreach (var slice in slices)
                {
                    newState = slice.Reduce(newState, action);
                }
                return newState;
            }
        }

        /// <summary>
        /// Apply middlewares to the store.
        /// </summary>
        /// <typeparam name="TState"> The type of the store state. </typeparam>
        /// <param name="middlewares"> The middlewares to apply. </param>
        /// <returns> A new store enhancer. </returns>
        public static StoreEnhancer<TState> ApplyMiddleware<TState>(params Middleware<TState>[] middlewares)
        {
            return createStore => (reducer, initialState) =>
            {
                var store = createStore(reducer, initialState);
                var middlewareChain = middlewares.Select(middleware => middleware(store));
                var dispatcher = middlewareChain
                    .Reverse()
                    .Aggregate(store.dispatcher, (nextDispatch, middleware) => middleware(nextDispatch));
                store.dispatcher = dispatcher;
                return store;
            };
        }

        /// <summary>
        /// Compose enhancers into a single enhancer.
        /// </summary>
        /// <typeparam name="TState"> The type of the store state. </typeparam>
        /// <param name="enhancers"> The enhancers to compose. </param>
        /// <returns> A new store enhancer. </returns>
        public static StoreEnhancer<TState> ComposeEnhancers<TState>(params StoreEnhancer<TState>[] enhancers)
        {
            return enhancers
                .Aggregate((prev, next) =>
                    createStore => next(prev(createStore)));
        }

        /// <summary>
        /// Get the default enhancer for the store.
        /// </summary>
        /// <typeparam name="TState"> The type of the store state. </typeparam>
        /// <param name="cfg"> The configuration for the default enhancer. </param>
        /// <returns> The default enhancer. </returns>
        public static StoreEnhancer<TState> DefaultEnhancer<TState>(DefaultEnhancerConfiguration cfg = null)
        {
            cfg ??= new DefaultEnhancerConfiguration();
            var middlewares = ApplyMiddleware(Thunk.ThunkMiddleware<TState>());
            var enhancer = middlewares;
            if (cfg.devTools.enabled)
                enhancer = ComposeEnhancers(enhancer, Instrument<TState>.Enhancer(cfg.devTools));
            return enhancer;
        }
    }
}
