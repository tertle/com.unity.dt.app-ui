using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// <para>
    /// A store holds the whole state tree of your application. The only way to change the state inside it is to dispatch an action on it.
    /// Your application should only have a single store in a Redux app. As your app grows, instead of adding stores,
    /// you split the root reducer into smaller reducers independently operating on the different parts of the state tree.
    /// </para>
    /// <para>
    /// The store has the following responsibilities:<br/>
    ///  - Holds application state <br/>
    ///  - Allows access to state via <see cref="GetState"/><br/>
    ///  - Allows state to be updated via <see cref="Dispatch(IAction)"/><br/>
    ///  - Registers listeners via <see cref="Subscribe{TState}"/><br/>
    ///  - Handles unregistering of listeners via the function returned by <see cref="Subscribe{TState}"/><br/>
    /// </para>
    /// <para>
    /// Here are some important principles you should understand about Reducers:<br/>
    ///  - Reducers are the only way to update the state.<br/>
    ///  - Reducers are pure functions that take the previous state and an action, and return the next state.<br/>
    ///  - Reducers must be pure functions. They should not mutate the state, perform side effects like API calls or routing transitions, or call non-pure functions.<br/>
    ///  - Reducers must not do asynchronous logic.<br/>
    ///  - Reducers must not call other <see cref="Reducer{TState}"/>.<br/>
    ///  - Reducers must not call <see cref="Subscribe{TState}"/>.<br/>
    ///  - Reducers must not call <see cref="GetState"/><br/>
    ///  - Reducers must not call <see cref="Dispatch(IAction)"/><br/>
    /// </para>
    /// </summary>
    /// <typeparam name="TStoreState"> The type of the store state. </typeparam>
    [Preserve]
    public partial class Store<TStoreState> : IStore<TStoreState>
    {
        bool m_Disposed;

        /// <summary>
        /// The subscriptions of the store.
        /// </summary>
        protected readonly List<ISubscription<TStoreState>> m_Subscriptions = new();

        /// <summary>
        /// The subscribe lock of the store.
        /// </summary>
        protected readonly object m_SubscribeLock = new();

        /// <summary>
        /// The state of the store.
        /// </summary>
        protected TStoreState m_State;

        /// <summary>
        /// The reducer of the store.
        /// </summary>
        protected readonly Reducer<TStoreState> m_Reducer;

        /// <summary>
        /// The reducer of the store.
        /// </summary>
        public Reducer<TStoreState> reducer => m_Reducer;

        /// <summary>
        /// The dispatcher of the store.
        /// </summary>
        public Dispatcher dispatcher { get; set; }

        /// <summary>
        /// Creates a Redux store that holds the complete state tree of your app.
        /// </summary>
        /// <param name="reducer"> The root reducer that returns the next state tree. </param>
        /// <param name="initialState"> The initial state of the store. </param>
        /// <remarks>
        /// You should not use directly this constructor to create a store.
        /// Instead, use <c>Store.CreateStore{TStoreState}</c> to create a store with optional enhancers.
        /// This constructor should be called only by enhancers to return an enhanced store.
        /// </remarks>
        [Preserve]
        public Store(Reducer<TStoreState> reducer, TStoreState initialState)
        {
            m_Reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
            m_State = initialState;
            dispatcher = DefaultDispatcher;
        }

        /// <summary>
        /// Returns the current state tree of your application. It is equal to the last value returned by the store's reducer.
        /// </summary>
        /// <returns> The current state tree of your application. </returns>
        public TStoreState GetState()
        {
            return m_State;
        }

        /// <summary>
        /// Dispatches an action. This is the only way to trigger a state change.
        /// </summary>
        /// <param name="action"> An object describing the change that makes up the action. </param>
        /// <exception cref="ArgumentException"> Thrown if the reducer for the action type does not exist. </exception>
        /// <exception cref="ArgumentNullException"> Thrown if the action is null. </exception>
        public void Dispatch(IAction action)
        {
            dispatcher(action);
        }

        void DefaultDispatcher(IAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            m_State = m_Reducer(m_State, action);

            NotifySubscribers();
        }

        /// <summary>
        /// Subscribe to a state change and listen to a specific part of the state.
        /// </summary>
        /// <param name="selector"> The selector to get the selected part of the state. </param>
        /// <param name="listener"> The listener to be invoked on every dispatch. </param>
        /// <param name="options"> The options for the subscription. </param>
        /// <typeparam name="TSelected"> The type of the selected part of the state. </typeparam>
        /// <returns> A Subscription object that can be disposed. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if the selector or listener is null. </exception>
        public IDisposableSubscription Subscribe<TSelected>(
            Selector<TStoreState,TSelected> selector,
            Listener<TSelected> listener,
            SubscribeOptions<TSelected> options = default)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            var subscription = new Subscription<TStoreState,TSelected>(selector, listener, this, options);
            lock (m_SubscribeLock)
            {
                m_Subscriptions.Add(subscription);
            }

            if (options.fireImmediately)
                listener(selector(m_State));

            return subscription;
        }

        /// <inheritdoc />
        bool INotifiable<TStoreState>.Unsubscribe(ISubscription<TStoreState> subscription)
        {
            lock (m_SubscribeLock)
            {
                return m_Subscriptions.Remove(subscription);
            }
        }

        /// <inheritdoc />
        public void NotifySubscribers()
        {
            lock (m_SubscribeLock)
            {
                foreach (var subscription in m_Subscriptions)
                {
                    subscription.Notify(GetState());
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (m_Disposed)
                return;

            ISubscription<TStoreState>[] subscriptions;
            lock (m_SubscribeLock)
            {
                subscriptions = m_Subscriptions.ToArray();
            }

            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            lock (m_SubscribeLock)
            {
                if (m_Subscriptions.Count > 0)
                    Debug.LogWarning($"The Store still has {m_Subscriptions.Count} active subscriptions. " +
                        $"You should not subscribe to the store when it is being disposed.");
                m_Subscriptions.Clear();
            }

            m_Disposed = true;
        }
    }
}
