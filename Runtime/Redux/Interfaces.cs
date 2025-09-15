using System;
using System.Collections.Generic;
using System.Threading;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// Representation of a Redux Store.
    /// </summary>
    public interface IStore : IDispatchable, IDisposable { }

    /// <summary>
    /// Representation of a Redux Store.
    /// </summary>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    public interface IStore<TState> : IStore, IStateProvider<TState>, INotifiable<TState>
    {
        /// <summary>
        /// The reducer function that will be used to update the state.
        /// </summary>
        Reducer<TState> reducer { get; }
    }

    /// <summary>
    /// An object that can dispatch an <see cref="Action"/>.
    /// </summary>
    public interface IDispatchable
    {
        /// <summary>
        /// Dispatches an action. This is the only way to trigger a state change.
        /// </summary>
        /// <param name="action"> An object describing the change that makes up the action. </param>
        /// <exception cref="ArgumentException"> Thrown if the reducer for the action type does not exist. </exception>
        /// <exception cref="ArgumentNullException"> Thrown if the action is null. </exception>
        void Dispatch(IAction action);

        /// <summary>
        /// The dispatcher that will be used to dispatch actions.
        /// </summary>
        Dispatcher dispatcher { get; set; }
    }

    /// <summary>
    /// Interface for a Store with a specific state type.
    /// </summary>
    /// <typeparam name="TState"> The type of the state. </typeparam>
    public interface IStateProvider<out TState>
    {
        /// <summary>
        /// Returns a state of type <typeparamref name="TState"/>.
        /// </summary>
        /// <returns> The state. </returns>
        TState GetState();
    }

    /// <summary>
    /// Interface for a store that can notify subscribers of state changes.
    /// </summary>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    public interface INotifiable<out TState>
    {
        /// <summary>
        /// Subscribe to a state change and listen to a specific part of the state.
        /// </summary>
        /// <param name="selector"> The selector to get the selected part of the state. </param>
        /// <param name="listener"> The listener to be invoked on every dispatch. </param>
        /// <param name="options"> The options for the subscription. </param>
        /// <typeparam name="TSelected"> The type of the selected part of the state. </typeparam>
        /// <returns> A Subscription object that can be disposed. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if the selector or listener is null. </exception>
        IDisposableSubscription Subscribe<TSelected>(
            Selector<TState,TSelected> selector,
            Listener<TSelected> listener,
            SubscribeOptions<TSelected> options = default);

        /// <summary>
        /// Remove a subscription from the store.
        /// </summary>
        /// <param name="subscription"> The subscription to remove. </param>
        /// <returns> True if the subscription was removed, false otherwise. </returns>
        bool Unsubscribe(ISubscription<TState> subscription);

        /// <summary>
        /// Notify the subscribers of a new state.
        /// </summary>
        void NotifySubscribers();
    }

    /// <summary>
    /// Interface to implement a store state that can be sliced.
    /// </summary>
    public interface IPartionableState
    {
        /// <summary>
        /// Implementation of the method to get the slice state from the store state.
        /// </summary>
        /// <param name="sliceName"> The name of the slice. </param>
        /// <typeparam name="TSliceState"> The type of the slice state. </typeparam>
        /// <returns> The slice state. </returns>
        TSliceState Get<TSliceState>(string sliceName);
    }

    /// <summary>
    /// Interface to implement a store state that can be sliced.
    /// </summary>
    /// <typeparam name="TStoreState"> The type of the store state. </typeparam>
    public interface IPartionableState<out TStoreState> : IPartionableState
    {
        /// <summary>
        /// Implementation of the method to set the slice state in the store state.
        /// </summary>
        /// <param name="sliceName"> The name of the slice. </param>
        /// <param name="sliceState">c The slice state. </param>
        /// <typeparam name="TSliceState"> The type of the slice state. </typeparam>
        /// <returns> The new store state. </returns>
        /// <remarks>
        /// It is important to note that this method should return a new instance of the store state
        /// to ensure immutability.
        /// </remarks>
        TStoreState Set<TSliceState>(string sliceName, TSliceState sliceState);
    }

    /// <summary>
    /// Base interface for an ActionCreator.
    /// </summary>
    public interface IActionCreator
    {
        /// <summary>
        /// The type of the action.
        /// This is used to determine which reducer to call.
        /// </summary>
        string type { get; }

        /// <summary>
        /// Check if the action matches the action creator.
        /// </summary>
        /// <param name="action"> The action creator. </param>
        /// <returns> True if the action matches the action creator. </returns>
        bool Match(IAction action);
    }

    /// <summary>
    /// Interface for an action creator with a payload.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public interface IActionCreator<out TPayload> : IActionCreator
    {
    }

    /// <summary>
    /// Interface for an action.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// The type of the action.
        /// This is used to determine which reducer to call.
        /// </summary>
        string type { get; }
    }

    /// <summary>
    /// Interface for an action with a payload.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public interface IAction<out TPayload> : IAction
    {
        /// <summary>
        /// The payload of the action.
        /// </summary>
        TPayload payload { get; }
    }

    /// <summary>
    /// Interface for an async thunk creator.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public interface IAsyncThunkCreator<TPayload>
    {
        /// <summary>
        /// Creates an async thunk.
        /// </summary>
        /// <returns> The async thunk. </returns>
        AsyncThunkAction<TPayload> Invoke();
    }

    /// <summary>
    /// Interface for an async thunk creator with argument.
    /// </summary>
    /// <typeparam name="TArg"> The type of the thunk argument. </typeparam>
    /// <typeparam name="TPayload"> The type of the payload. </typeparam>
    public interface IAsyncThunkCreator<TArg, TPayload>
    {
        /// <summary>
        /// Creates an async thunk.
        /// </summary>
        /// <param name="arg"> The argument to pass to the thunk. </param>
        /// <returns> The async thunk. </returns>
        AsyncThunkAction<TArg, TPayload> Invoke(TArg arg);
    }

    /// <summary>
    /// Definition of a switch statement builder to generate a Reducer.
    /// </summary>
    /// <typeparam name="TBuilder"> The type of the builder. </typeparam>
    /// <typeparam name="TState"> The type of the state. </typeparam>
    public interface ISwitchBuilder<out TBuilder,TState>
    {
        /// <summary>
        /// Adds a case to the reducer switch statement.
        /// </summary>
        /// <param name="actionCreator"> The action creator for the action type you want to handle. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <returns> The builder instance to chain calls. </returns>
        TBuilder AddCase(IActionCreator actionCreator, Reducer<TState> reducer);

        /// <summary>
        /// Adds a case to the reducer switch statement.
        /// </summary>
        /// <param name="actionCreator"> The action creator for the action type you want to handle. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <typeparam name="TPayload"> The type of the action payload. </typeparam>
        /// <returns> The builder instance to chain calls. </returns>
        TBuilder AddCase<TPayload>(IActionCreator<TPayload> actionCreator, Reducer<TPayload, TState> reducer);

        /// <summary>
        /// Create a dictionary of action creator per action type.
        /// </summary>
        /// <returns> The dictionary of action creators. </returns>
        Dictionary<string, IActionCreator> GetActionCreators();

        /// <summary>
        /// Get the reducer output function.
        /// </summary>
        /// <returns> The reducer function. </returns>
        Reducer<TState> GetReducer();
    }

    /// <summary>
    /// A switch statement builder that can be used to build a switch statement for a reducer.
    /// </summary>
    /// <typeparam name="TBuilder"> The type of the builder. </typeparam>
    /// <typeparam name="TState"> The type of the state. </typeparam>
    public interface ISwitchMatchBuilder<out TBuilder,TState>
    {
        /// <summary>
        /// Adds a matcher case to the reducer switch statement.
        /// A matcher case is a case that will be executed if the action type matches the predicate.
        /// </summary>
        /// <param name="actionMatcher"> The predicate that will be used to match the action type. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <returns> The Reducer Switch Builder. </returns>
        TBuilder AddCase(ActionMatcher actionMatcher, Reducer<TState> reducer);

        /// <summary>
        /// Adds a matcher case to the reducer switch statement.
        /// A matcher case is a case that will be executed if the action type matches the predicate.
        /// </summary>
        /// <typeparam name="TPayload"> The type of the action payload. </typeparam>
        /// <param name="actionMatcher"> The predicate that will be used to match the action type. </param>
        /// <param name="reducer"> The reducer function for the action type you want to handle. </param>
        /// <returns> The Reducer Switch Builder. </returns>
        TBuilder AddCase<TPayload>(ActionMatcher actionMatcher, Reducer<TPayload, TState> reducer);

        /// <summary>
        /// Adds a default case to the reducer switch statement.
        /// A default case is a case that will be executed if no other cases match.
        /// </summary>
        /// <param name="reducer"> The reducer function for the default case. </param>
        /// <returns> The Reducer Switch Builder. </returns>
        TBuilder AddDefault(Reducer<TState> reducer);
    }

    /// <summary>
    /// Base interface for a slice.
    /// </summary>
    /// <typeparam name="TStoreState"> The type of the store state. </typeparam>
    public interface ISlice<TStoreState>
    {
        /// <summary>
        /// The wrapped reducer of the slice for being used in the store.
        /// </summary>
        /// <param name="state"> The current state of the store. </param>
        /// <param name="action"> The action to apply to the state. </param>
        /// <returns> The new state of the store. </returns>
        TStoreState Reduce(TStoreState state, IAction action);

        /// <summary>
        /// Set the initial state of the slice.
        /// This is used internally by
        /// <see cref="M:Unity.AppUI.Redux.Store.CreateStore``2(System.Collections.Generic.IReadOnlyCollection{Unity.AppUI.Redux.ISlice{``1}},Unity.AppUI.Redux.StoreEnhancer{Unity.AppUI.Redux.Store{``1}.CreationContext,``1})"/>.
        /// </summary>
        /// <param name="state"> The initial state of the store. </param>
        /// <returns> The new state of the store. </returns>
        TStoreState InitializeState(TStoreState state);
    }

    /// <summary>
    /// Base interface for the Thunk API.
    /// </summary>
    public interface IThunkAPI
    {
        /// <summary>
        /// The request id of the thunk.
        /// </summary>
        string requestId { get; }

        /// <summary>
        /// Whether the thunk has been cancelled.
        /// </summary>
        bool isCancellationRequested { get; }

        /// <summary>
        /// The cancellation token of the thunk (if any).
        /// </summary>
        CancellationToken cancellationToken { get; }

        /// <summary>
        /// The store that the thunk is dispatching to.
        /// </summary>
        IDispatchable store { get; }

        /// <summary>
        /// Cancels the thunk with a reason.
        /// </summary>
        /// <param name="reason"> The reason for the cancellation. </param>
        void Abort(object reason);
    }

    /// <summary>
    /// Thunk API definition.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload that the thunk will return. </typeparam>
    public interface IThunkAPI<in TPayload> : IAPIInterceptor<TPayload>, IThunkAPI { }

    /// <summary>
    /// A Thunk API that can early reject or fulfill the thunk with a value.
    /// </summary>
    /// <typeparam name="TPayload"> The type of the payload that the thunk will return. </typeparam>
    public interface IAPIInterceptor<in TPayload>
    {
        /// <summary>
        /// Rejects the thunk with a reason.
        /// </summary>
        /// <param name="value"> The reason for the rejection. </param>
        void RejectWithValue(TPayload value);

        /// <summary>
        /// Fulfills the thunk with a value.
        /// </summary>
        /// <param name="value"> The value to fulfill the thunk with. </param>
        void FulFillWithValue(TPayload value);
    }

    /// <summary>
    /// Thunk API definition with an argument passed when dispatching the thunk.
    /// </summary>
    /// <typeparam name="TArg"> The type of the argument that the thunk has received. </typeparam>
    /// <typeparam name="TPayload"> The type of the payload that the thunk will return. </typeparam>
    public interface IThunkAPI<out TArg,in TPayload> : IThunkAPI<TPayload>
    {
        /// <summary>
        /// The argument that the thunk has received when it was dispatched.
        /// </summary>
        TArg arg { get; }
    }

    /// <summary>
    /// Represents a subscription to a store.
    /// </summary>
    public interface IDisposableSubscription : IDisposable
    {
        /// <summary>
        /// Whether the subscription is still valid.
        /// A Subscription is no longer valid when it has been unsubscribed
        /// or of the store it was subscribed to has been disposed.
        /// </summary>
        /// <returns> True if the subscription is still valid, false otherwise. </returns>
        bool IsValid();
    }

    /// <summary>
    /// A Subscription to a store. This abstraction is used by the store to manage subscriptions.
    /// </summary>
    /// <typeparam name="TState"> The type of the state. </typeparam>
    public interface ISubscription<in TState> : IDisposableSubscription
    {
        /// <summary>
        /// Unsubscribe from the store.
        /// </summary>
        /// <returns> True if the subscription was removed, false otherwise. </returns>
        bool Unsubscribe();

        /// <summary>
        /// Notify the listener of a new state.
        /// </summary>
        /// <param name="state"> The new state of the store. </param>
        void Notify(TState state);
    }
}
