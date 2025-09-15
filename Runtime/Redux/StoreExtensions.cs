using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// Extensions for the <see cref="IDispatchable"/>.
    /// </summary>
    public static class StoreExtensions
    {
        /// <summary>
        /// Dispatches an action. This is the only way to trigger a state change.
        /// </summary>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="actionType"> The type of the action. </param>
        /// <exception cref="ArgumentNullException"> Thrown when the store is null. </exception>
        /// <exception cref="ArgumentNullException"> Thrown when the actionType is null or empty. </exception>
        public static void Dispatch(this IDispatchable store, string actionType)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (string.IsNullOrEmpty(actionType))
                throw new ArgumentNullException(nameof(actionType));

            store.Dispatch(new ActionCreator(actionType).Invoke());
        }

        /// <summary>
        /// Dispatches an action. This is the only way to trigger a state change.
        /// </summary>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="actionType"> The type of the action. </param>
        /// <param name="payload"> The payload of the action. </param>
        /// <typeparam name="TPayload"> The type of the payload. </typeparam>
        /// <exception cref="ArgumentNullException"> Thrown when the store is null. </exception>
        /// <exception cref="ArgumentNullException"> Thrown when the actionType is null or empty. </exception>
        public static void Dispatch<TPayload>(this IDispatchable store, string actionType, TPayload payload)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (string.IsNullOrEmpty(actionType))
                throw new ArgumentNullException(nameof(actionType));

            store.Dispatch(new ActionCreator<TPayload>(actionType).Invoke(payload));
        }

        /// <summary>
        /// Dispatches an action from the given creator.
        /// This is a shortcut to call the creator and dispatch the action.
        /// </summary>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="creator"> The action creator. </param>
        /// <exception cref="ArgumentNullException"> Thrown when the store is null. </exception>
        /// <exception cref="ArgumentNullException"> Thrown when the creator is null. </exception>
        public static void Dispatch(this IDispatchable store, ActionCreator creator)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            store.Dispatch(creator.Invoke());
        }

        /// <summary>
        /// Dispatches an action from the given creator with a payload.
        /// This is a shortcut to call the creator and dispatch the action.
        /// </summary>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="creator"> The action creator. </param>
        /// <param name="payload"> The payload of the action. </param>
        /// <typeparam name="TPayload"> The type of the payload. </typeparam>
        /// <exception cref="ArgumentNullException"> Thrown when the store is null. </exception>
        /// <exception cref="ArgumentNullException"> Thrown when the creator is null. </exception>
        public static void Dispatch<TPayload>(this IDispatchable store, ActionCreator<TPayload> creator, TPayload payload)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            store.Dispatch(creator.Invoke(payload));
        }

        /// <summary>
        /// <para>Dispatches an async thunk action.</para>
        /// <para>
        /// This method will dispatch the pending action,
        /// then call the thunk, and finally dispatch the fulfilled or rejected action based on the result of the thunk.
        /// </para>
        /// </summary>
        /// <typeparam name="TPayload"> The type of the payload (the result of the thunk). </typeparam>
        /// <typeparam name="TArg"> The type of the argument to pass to the thunk. </typeparam>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="asyncThunkAction"> The async thunk action to dispatch. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <remarks>
        /// Dispatching an async thunk action with this method invloves the use of async/await.
        /// If your target platform doesn't support async/await, you can use
        /// <see cref="DispatchAsyncThunkCoroutine{TPayload,TArg}"/> instead.
        /// </remarks>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <exception cref="System.ArgumentNullException"> Thrown when the store is null. </exception>
        /// <exception cref="System.ArgumentNullException"> Thrown when the asyncThunkAction is null. </exception>
        public static async Task DispatchAsyncThunk<TPayload,TArg>(
            this IDispatchable store,
            AsyncThunkAction<TPayload,TArg> asyncThunkAction,
            CancellationToken cancellationToken = default)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (asyncThunkAction == null)
                throw new ArgumentNullException(nameof(asyncThunkAction));

            await asyncThunkAction.ExecuteAsync(store, cancellationToken);
        }

        /// <summary>
        /// <para>Dispatches an async thunk action.</para>
        /// <para>
        /// This method will dispatch the pending action,
        /// then call the thunk, and finally dispatch the fulfilled or rejected action based on the result of the thunk.
        /// </para>
        /// </summary>
        /// <typeparam name="TPayload"> The type of the payload (the result of the thunk). </typeparam>
        /// <typeparam name="TArg"> The type of the argument to pass to the thunk. </typeparam>
        /// <param name="store"> The store to dispatch the action to. </param>
        /// <param name="asyncThunkAction"> The async thunk action to dispatch. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> The coroutine. </returns>
        /// <exception cref="System.ArgumentNullException"> Thrown when the store is null. </exception>
        /// <exception cref="System.ArgumentNullException"> Thrown when the asyncThunkAction is null. </exception>
        public static Coroutine DispatchAsyncThunkCoroutine<TPayload,TArg>(
            this IDispatchable store,
            AsyncThunkAction<TPayload,TArg> asyncThunkAction,
            CancellationToken cancellationToken = default)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (asyncThunkAction == null)
                throw new ArgumentNullException(nameof(asyncThunkAction));

            return asyncThunkAction.ExecuteCoroutine(store, cancellationToken);
        }

        /// <summary>
        /// Get the state of a slice.
        /// </summary>
        /// <param name="store"> The store. </param>
        /// <param name="sliceName"> The name of the slice. </param>
        /// <typeparam name="TSliceState"> The type of the slice state. </typeparam>
        /// <returns> The state of the slice. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when the store is null. </exception>
        /// <exception cref="ArgumentNullException"> Thrown when the sliceName is null or empty. </exception>
        public static TSliceState GetState<TSliceState>(this IStateProvider<IPartionableState> store, string sliceName)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (string.IsNullOrEmpty(sliceName))
                throw new ArgumentNullException(nameof(sliceName));

            return store.GetState().Get<TSliceState>(sliceName);
        }

        /// <summary>
        /// Get a selected part of the state.
        /// </summary>
        /// <param name="store"> The store. </param>
        /// <param name="selector"> The selector to get the selected part of the state. </param>
        /// <typeparam name="TState"> The type of the state. </typeparam>
        /// <typeparam name="TSelected"> The type of the selected part of the state. </typeparam>
        /// <returns> The selected part of the state. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when the store is null. </exception>
        public static TSelected GetState<TState,TSelected>(this IStateProvider<TState> store,
            Selector<TState, TSelected> selector)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return selector(store.GetState());
        }

        /// <summary>
        /// Get the selected part of the state in a slice.
        /// </summary>
        /// <param name="store"> The store. </param>
        /// <param name="sliceName"> The name of the slice. </param>
        /// <param name="selector"> The selector to get the selected part of the state. </param>
        /// <typeparam name="TSliceState"> The type of the slice state. </typeparam>
        /// <typeparam name="TSelected"> The type of the selected part of the state. </typeparam>
        /// <returns> The selected part of the state. </returns>
        /// <exception cref="ArgumentNullException"> Thrown when any of the arguments is null. </exception>
        public static TSelected GetState<TSliceState, TSelected>(this IStateProvider<IPartionableState> store,
            string sliceName, Selector<TSliceState, TSelected> selector)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (string.IsNullOrEmpty(sliceName))
                throw new ArgumentNullException(nameof(sliceName));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            return selector(store.GetState().Get<TSliceState>(sliceName));
        }

        /// <summary>
        /// Adds a change listener.
        /// It will be called any time an action is dispatched, and some part of the state tree may potentially have changed.
        /// </summary>
        /// <remarks>
        /// This method doesn't check for duplicate listeners,
        /// so calling it multiple times with the same listener will result in the listener being called multiple times.
        /// </remarks>
        /// <typeparam name="TStoreState"> The type of the store state. </typeparam>
        /// <param name="store"> The store. </param>
        /// <param name="listener"> A callback to be invoked on every dispatch. </param>
        /// <param name="options"> The options for the subscription. </param>
        /// <returns> A Subscription object that can be disposed. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if the listener is null. </exception>
        public static IDisposableSubscription Subscribe<TStoreState>(
            this INotifiable<TStoreState> store,
            Listener<TStoreState> listener,
            SubscribeOptions<TStoreState> options = default)
        {
            return store.Subscribe(DefaultStateSelector, listener, options);

            [Pure]
            static TStoreState DefaultStateSelector(TStoreState state) => state;
        }

        /// <summary>
        /// Subscribe to a state change and listen to a specific part of the state.
        /// </summary>
        /// <param name="store"> The store. </param>
        /// <param name="sliceName"> The name of the slice. </param>
        /// <param name="listener"> The listener to be invoked on state change. </param>
        /// <param name="options"> The options for the subscription. </param>
        /// <typeparam name="TStoreState"> The type of the store state. </typeparam>
        /// <typeparam name="TSliceState"> The type of the slice state. </typeparam>
        /// <returns> A Subscription object that can be disposed. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if the store is null. </exception>
        /// <exception cref="ArgumentNullException"> Thrown if the slice name is null or empty. </exception>
        /// <exception cref="ArgumentNullException"> Thrown if the listener is null. </exception>
        public static IDisposableSubscription Subscribe<TStoreState,TSliceState>(
            this INotifiable<TStoreState> store,
            string sliceName,
            Listener<TSliceState> listener,
            SubscribeOptions<TSliceState> options = default)
            where TStoreState : IPartionableState
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));
            if (string.IsNullOrEmpty(sliceName))
                throw new ArgumentNullException(nameof(sliceName));
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            return store.Subscribe(GetSliceState, listener, options);

            TSliceState GetSliceState(TStoreState state) => state.Get<TSliceState>(sliceName);
        }

        /// <summary>
        /// Subscribe to a state change and listen to a specific part of the state in a slice.
        /// </summary>
        /// <param name="store"> The store. </param>
        /// <param name="sliceName"> The name of the slice. </param>
        /// <param name="selector"> The selector to get the selected part of the state. </param>
        /// <param name="listener"> The listener to be invoked on state change. </param>
        /// <param name="options"> The options for the subscription. </param>
        /// <typeparam name="TStoreState"> The type of the store state. </typeparam>
        /// <typeparam name="TSelected"> The type of the selected part of the state. </typeparam>
        /// <typeparam name="TSliceState"> The type of the slice state. </typeparam>
        /// <returns> A Subscription object that can be disposed. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if the store is null. </exception>
        /// <exception cref="ArgumentNullException"> Thrown if the slice name is null or empty. </exception>
        /// <exception cref="ArgumentNullException"> Thrown if the selector is null. </exception>
        /// <exception cref="ArgumentNullException"> Thrown if the listener is null. </exception>
        public static IDisposableSubscription Subscribe<TStoreState,TSelected,TSliceState>(
            this INotifiable<TStoreState> store,
            string sliceName,
            Selector<TSliceState,TSelected> selector,
            Listener<TSelected> listener,
            SubscribeOptions<TSelected> options = default)
            where TStoreState : IPartionableState
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));
            if (string.IsNullOrEmpty(sliceName))
                throw new ArgumentNullException(nameof(sliceName));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            return store.Subscribe(SliceSelector, listener, options);

            TSelected SliceSelector(TStoreState state) => selector(state.Get<TSliceState>(sliceName));
        }
    }
}
