using System;

namespace Unity.AppUI.Redux.DevTools
{
    /// <summary>
    /// Subscription to a lifted store.
    /// </summary>
    /// <typeparam name="TState"> The type of the store state. </typeparam>
    /// <typeparam name="TSelected"> The type of the selected state. </typeparam>
    /// <seealso cref="IInstrumentedStore{TState}"/>
    class LiftedSubscription<TState,TSelected> : SubscriptionBase<IInstrumentedStore<TState>,LiftedState,TSelected>
    {
        /// <inheritdoc />
        public LiftedSubscription(
            Selector<LiftedState, TSelected> selector,
            Listener<TSelected> listener,
            IInstrumentedStore<TState> store,
            SubscribeOptions<TSelected> options)
            : base(selector, listener, store, options) { }

        /// <inheritdoc />
        protected override LiftedState GetStoreState(IInstrumentedStore<TState> store)
        {
            return store.GetLiftedState();
        }

        /// <inheritdoc />
        protected override bool RemoveFromStore(IInstrumentedStore<TState> store)
        {
            return store.Unsubscribe(this);
        }
    }
}
