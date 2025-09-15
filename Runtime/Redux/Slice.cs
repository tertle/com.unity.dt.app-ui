using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// A Slice is a collection of reducers and actions for a specific domain.
    /// It is a convenient way to bundle them together for use in a Redux store.
    /// </summary>
    /// <typeparam name="TState"> The type of the state associated with this slice. </typeparam>
    /// <typeparam name="TStoreState"> The type of the store state. </typeparam>
    [Preserve]
    public class Slice<TState, TStoreState> : ISlice<TStoreState>
        where TStoreState : IPartionableState<TStoreState>
    {
        /// <summary>
        /// The name of the slice.
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// The action creators for this slice.
        /// </summary>
        public Dictionary<string, IActionCreator> actionCreators { get; private set; }

        /// <summary>
        /// The reducer for this slice.
        /// </summary>
        public Reducer<TState> reducer { get; private set; }

        /// <summary>
        /// The initial state for this slice.
        /// </summary>
        public TState initialState { get; private set; }

        /// <summary>
        /// Definition of a Redux Selector for a Slice.
        /// </summary>
        /// <typeparam name="TSelected"> The type of the selected state. </typeparam>
        /// <param name="state"> The state to select from. </param>
        /// <returns> The selected state. </returns>
        public delegate TSelected Selector<out TSelected>(TState state);

        /// <summary>
        /// Definition of the delegate method to get the slice state from the store state.
        /// </summary>
        /// <param name="state"> The store state. </param>
        /// <returns> The slice state. </returns>
        public delegate TState GetSliceState(TStoreState state);

        /// <summary>
        /// Definition of the delegate method to set the slice state in the store state.
        /// </summary>
        /// <param name="state"> The store state. </param>
        /// <param name="sliceState"> The slice state. </param>
        /// <returns> The new store state. </returns>
        public delegate TStoreState SetSliceState(TStoreState state, TState sliceState);

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
        [Preserve]
        public Slice(
            string name,
            TState initialState,
            System.Action<SliceReducerSwitchBuilder<TState>> reducers,
            System.Action<ReducerSwitchBuilder<TState>> extraReducers = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentException("The name of the slice cannot be null or empty.", nameof(name));

            this.name = name;
            this.initialState = initialState;

            // populate the first builder for slice related reducers
            var sliceReducerbuilder = new SliceReducerSwitchBuilder<TState>(name);
            reducers?.Invoke(sliceReducerbuilder);

            // convert the builder to a general purpose builder
            var extraReducerBuilder = new ReducerSwitchBuilder<TState>(sliceReducerbuilder);
            extraReducers?.Invoke(extraReducerBuilder);

            actionCreators = sliceReducerbuilder.GetActionCreators();
            reducer = extraReducerBuilder.GetReducer();
        }

        /// <inheritdoc/>
        public TStoreState Reduce(TStoreState state, IAction action)
        {
            var currentSliceState = state.Get<TState>(name);
            var newSliceState = reducer(currentSliceState, action);
            return state.Set(name, newSliceState);
        }

        /// <inheritdoc/>
        public TStoreState InitializeState(TStoreState state)
        {
            return state.Set(name, initialState);
        }
    }
}
