using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Redux.DevTools
{
    /// <summary>
    /// The instrumented store interface.
    /// </summary>
    public interface IInstrumentedStore : IStore
    {
        /// <summary>
        /// The display name of the store.
        /// </summary>
        public string displayName { get; }

        /// <summary>
        /// The unique identifier of the store.
        /// </summary>
        public string id { get; }

        /// <summary>
        /// Get the current lifted state of the store.
        /// </summary>
        /// <returns> The current lifted state. </returns>
        LiftedState GetLiftedState();

        /// <summary>
        /// Subscribe to the lifted state of the store.
        /// </summary>
        /// <param name="selector"> The selector to use to select the state. </param>
        /// <param name="listener"> The listener to notify of state changes. </param>
        /// <param name="options"> The options for the subscription. </param>
        /// <typeparam name="TSelected"> The type of the selected state. </typeparam>
        /// <returns> The subscription. </returns>
        IDisposableSubscription Subscribe<TSelected>(
            Selector<LiftedState, TSelected> selector,
            Listener<TSelected> listener,
            SubscribeOptions<TSelected> options = default);

        /// <summary>
        /// Unsubscribe from the lifted state of the store.
        /// </summary>
        /// <param name="subscription"> The subscription to remove. </param>
        /// <returns> True if the subscription was removed, false otherwise. </returns>
        bool Unsubscribe(ISubscription<LiftedState> subscription);
    }

    /// <summary>
    /// The instrumented store is a wrapper around a store that allows for the lifted state to be
    /// accessed and subscribed to.
    /// </summary>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    public interface IInstrumentedStore<TState> : IStore<TState>, IInstrumentedStore
    {

    }

    /// <summary>
    /// The instrumented store is a wrapper around a store that allows for the lifted state to be
    /// accessed and subscribed to.
    /// </summary>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    public class InstrumentedStore<TState> : IInstrumentedStore<TState>
    {
        readonly object m_StateLock = new object();
        readonly object m_LiftedSubscriptionsLock = new object();
        readonly Reducer<LiftedState> m_LiftedReducer;
        readonly HashSet<ISubscription<LiftedState>> m_LiftedSubscriptions;
        readonly DevToolsConfiguration m_Options;
        LiftedState m_LiftedState;
        IStore<TState> m_Store;

        /// <summary>
        /// Create a new instrumented store.
        /// </summary>
        /// <param name="createStore"> The store creation function. </param>
        /// <param name="rootReducer"> The root reducer for the store. </param>
        /// <param name="initialState"> The initial state of the store. </param>
        /// <param name="options"> The options for the store. </param>
        /// <exception cref="ArgumentNullException"> Thrown if any of the arguments are null. </exception>
        public InstrumentedStore(
            StoreEnhancerStoreCreator<TState> createStore,
            Reducer<TState> rootReducer,
            TState initialState,
            DevToolsConfiguration options)
        {
            m_Options = options ?? throw new ArgumentNullException(nameof(options));

            if (createStore == null)
                throw new ArgumentNullException(nameof(createStore));

            if (rootReducer == null)
                throw new ArgumentNullException(nameof(rootReducer));

            m_LiftedSubscriptions = new HashSet<ISubscription<LiftedState>>();
            m_LiftedState = null; // no need initialization here because it is done in the lifted reducer.
            m_LiftedReducer = Instrument<TState>.LiftReducerWith(rootReducer, initialState, options);
            m_Store = createStore(UnliftReducer, initialState);
        }

        /// <summary>
        /// The display name of the store.
        /// </summary>
        public string displayName => m_Options.name;

        /// <summary>
        /// The unique identifier of the store.
        /// </summary>
        public string id { get; } = Guid.NewGuid().ToString();

        /// <inheritdoc/>
        public LiftedState GetLiftedState() => m_LiftedState;

        /// <inheritdoc/>
        public IDisposableSubscription Subscribe<TSelected>(
            Selector<LiftedState, TSelected> selector,
            Listener<TSelected> listener,
            SubscribeOptions<TSelected> options = default)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            var subscription = new LiftedSubscription<TState,TSelected>(selector, listener, this, options);
            lock (m_LiftedSubscriptionsLock)
            {
                m_LiftedSubscriptions.Add(subscription);
            }

            if (m_LiftedState != null)
                listener(selector(m_LiftedState));

            return subscription;
        }

        /// <inheritdoc/>
        bool IInstrumentedStore.Unsubscribe(ISubscription<LiftedState> subscription)
        {
            lock (m_LiftedSubscriptionsLock)
            {
                return m_LiftedSubscriptions.Remove(subscription);
            }
        }

        void NotifyLiftedSubscribers()
        {
            ISubscription<LiftedState>[] subscribers;
            lock (m_LiftedSubscriptionsLock)
            {
                subscribers = new ISubscription<LiftedState>[m_LiftedSubscriptions.Count];
                m_LiftedSubscriptions.CopyTo(subscribers);
            }

            foreach (var subscriber in subscribers)
            {
                try
                {
                    subscriber.Notify(m_LiftedState);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <inheritdoc/>
        public IDisposableSubscription Subscribe<TSelected>(
            Selector<TState, TSelected> selector,
            Listener<TSelected> listener,
            SubscribeOptions<TSelected> options = default)
        {
            return m_Store.Subscribe(selector, listener, options);
        }

        /// <inheritdoc/>
        bool INotifiable<TState>.Unsubscribe(ISubscription<TState> subscription)
        {
            return m_Store.Unsubscribe(subscription);
        }

        /// <inheritdoc/>
        void INotifiable<TState>.NotifySubscribers()
        {
            m_Store.NotifySubscribers();
        }

        /// <inheritdoc/>
        public void Dispatch(IAction action)
        {
            m_Store.Dispatch(LiftedAction.From(action));
        }

        /// <inheritdoc/>
        public Dispatcher dispatcher
        {
            get => m_Store.dispatcher;
            set => m_Store.dispatcher = value;
        }

        /// <inheritdoc/>
        public Reducer<TState> reducer => m_Store.reducer;

        /// <inheritdoc/>
        public TState GetState()
        {
            return m_Store.GetState();
        }

        TState UnliftReducer(TState state, IAction action)
        {
            LiftedState previousLiftedState;
            LiftedState newLiftedState;

            lock (m_StateLock)
            {
                previousLiftedState = m_LiftedState;
                newLiftedState = m_LiftedReducer(previousLiftedState, action);
                m_LiftedState = newLiftedState;
            }

            if (!Equals(previousLiftedState, newLiftedState))
                NotifyLiftedSubscribers();

            if (newLiftedState.currentStateIndex >= 0 && newLiftedState.currentStateIndex < newLiftedState.computedStates.Count)
                return (TState)newLiftedState.computedStates[newLiftedState.currentStateIndex].state;

            return default;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            DevToolsService.Instance.Disconnect(this);

            ISubscription<LiftedState>[] liftedSubscriptions;

            lock (m_LiftedSubscriptionsLock)
            {
                liftedSubscriptions = new ISubscription<LiftedState>[m_LiftedSubscriptions.Count];
                m_LiftedSubscriptions.CopyTo(liftedSubscriptions);
            }

            foreach (var subscription in liftedSubscriptions)
            {
                subscription.Dispose();
            }

            lock (m_LiftedSubscriptionsLock)
            {
                m_LiftedSubscriptions.Clear();
            }

            m_Store?.Dispose();
            m_Store = null;
            m_LiftedState = null;
        }
    }

    /// <summary>
    /// The instrumented store extensions.
    /// </summary>
    public static class InstrumentedStoreExtensions
    {
        /// <summary>
        /// Get the instrumented store from a store.
        /// </summary>
        /// <param name="store"> The store. </param>
        /// <typeparam name="TState"> The type of the store state. </typeparam>
        /// <returns> The instrumented store. </returns>
        /// <exception cref="InvalidOperationException"> Thrown if the store is not instrumented. </exception>
        public static IInstrumentedStore<TState> AsInstrumentedStore<TState>(this IStore<TState> store)
        {
            if (store is not IInstrumentedStore<TState> instrumentedStore)
                throw new InvalidOperationException("Store is not an instrumented store.");
            return instrumentedStore;
        }
    }
}
