using System;
using System.Collections.Generic;

namespace Unity.AppUI.Redux
{
    /// <summary>
    /// Options for subscribing to a store.
    /// </summary>
    /// <typeparam name="TResult"> The type of the result of the subscription. </typeparam>
    public struct SubscribeOptions<TResult>
    {
        /// <summary>
        /// The comparer to use to compare the result of the subscription.
        /// If not provided, the default comparer for the type will be used.
        /// </summary>
        /// <seealso cref="EqualityComparer{T}.Default"/>
        public EqualityComparer<TResult> comparer;

        /// <summary>
        /// Whether to invoke the listener immediately when subscribing.
        /// </summary>
        public bool fireImmediately;
    }

    /// <summary>
    /// A base subscription to a store for a selected state.
    /// </summary>
    /// <typeparam name="TStore"> The type of the store. </typeparam>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    /// <typeparam name="TSelected"> The type of the selected state. </typeparam>
    public abstract class SubscriptionBase<TStore,TState,TSelected> : ISubscription<TState>
        where TStore : class, IStore
    {
        WeakReference<TStore> m_Store;

        Listener<TSelected> m_Listener;

        Selector<TState,TSelected> m_Selector;

        TSelected m_LastSelected;

        readonly SubscribeOptions<TSelected> m_Options;

        /// <summary>
        /// Create a new subscription to a store for a selected state.
        /// </summary>
        /// <param name="selector"> The selector to use to select the state. </param>
        /// <param name="listener"> The listener to notify of state changes. </param>
        /// <param name="store"> The store to subscribe to. </param>
        /// <param name="options"> The options for the subscription. </param>
        protected SubscriptionBase(
            Selector<TState,TSelected> selector,
            Listener<TSelected> listener,
            TStore store,
            SubscribeOptions<TSelected> options)
        {
            m_Selector = selector;
            m_Store = new WeakReference<TStore>(store);
            m_Listener = listener;
            m_Options = options;
            Initialize();
        }

        void Initialize()
        {
            if (m_Store.TryGetTarget(out var store))
                m_LastSelected = m_Selector(GetStoreState(store));
            else
                throw new InvalidOperationException("Store has been garbage collected during subscription initialization.");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Unsubscribe();
        }

        /// <inheritdoc/>
        public bool IsValid()
        {
            return m_Store.TryGetTarget(out _) && m_Listener != null && m_Selector != null;
        }

        /// <inheritdoc/>
        public bool Unsubscribe()
        {
            var removed = false;
            if (m_Store != null && m_Store.TryGetTarget(out var store))
                removed = RemoveFromStore(store);
            m_Store = null;
            m_Listener = null;
            m_Selector = null;
            return removed;
        }

        /// <inheritdoc/>
        public void Notify(TState state)
        {
            if (m_Listener == null || m_Selector == null)
            {
                Unsubscribe();
                return;
            }

            var selected = m_Selector(state);
            var comparer = m_Options.comparer ?? EqualityComparer<TSelected>.Default;
            if (!comparer.Equals(selected, m_LastSelected))
            {
                m_LastSelected = selected;
                m_Listener(selected);
            }
        }

        /// <summary>
        /// Get the state from the store.
        /// </summary>
        /// <param name="store"> The store to get the state from. </param>
        /// <returns> The state from the store. </returns>
        protected abstract TState GetStoreState(TStore store);

        /// <summary>
        /// Remove the subscription from the store.
        /// </summary>
        /// <param name="store"> The store to remove the subscription from. </param>
        /// <returns> Whether the subscription was removed. </returns>
        protected abstract bool RemoveFromStore(TStore store);
    }

    /// <summary>
    /// A subscription to a store for a selected state.
    /// </summary>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    /// <typeparam name="TSelected"> The type of the selected state. </typeparam>
    public class Subscription<TState, TSelected> : SubscriptionBase<IStore<TState>,TState,TSelected>
    {
        /// <summary>
        /// Create a new subscription to a store for a selected state.
        /// </summary>
        /// <param name="selector"> The selector to use to select the state. </param>
        /// <param name="listener"> The listener to notify of state changes. </param>
        /// <param name="store"> The store to subscribe to. </param>
        /// <param name="options"> The options for the subscription. </param>
        public Subscription(
            Selector<TState, TSelected> selector,
            Listener<TSelected> listener,
            IStore<TState> store,
            SubscribeOptions<TSelected> options)
            : base(selector, listener, store, options) { }

        /// <inheritdoc />
        protected override TState GetStoreState(IStore<TState> store)
        {
            return store.GetState();
        }

        /// <inheritdoc />
        protected override bool RemoveFromStore(IStore<TState> store)
        {
            return store.Unsubscribe(this);
        }
    }
}
