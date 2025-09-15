using UnityEngine;
using Unity.AppUI.Redux;

namespace Unity.AppUI.Samples.Redux
{
    public class ReduxSample : MonoBehaviour
    {
        const string RESET_COUNTER = "counter/Reset";

        const string INCREMENT_COUNTER = "counter/Increment";

        const string DECREMENT_COUNTER = "counter/Decrement";

        const string INCREMENT_COUNTER_BY = "counter/IncrementBy";

        const string COUNTER_SLICE = "counter";

        void Start()
        {
            // You can create general purpose Actions that can be dispatched to the store
            // When building reducers, Actions are used to determine which reducer to call
            // You can also create Actions that are specific to a slice of state by using the naming convention "sliceName/actionName"
            var reset = new ActionCreator(RESET_COUNTER);

            var increment = new ActionCreator(INCREMENT_COUNTER);

            var decrement = new ActionCreator(DECREMENT_COUNTER);

            var incrementBy = new ActionCreator<int>(INCREMENT_COUNTER_BY);

            // You can create slices of state that will be stored in the store
            // Each slice of state can have its own reducers and actions
            // It is possible to pass extra reducers that are bound to external actions (no prefix)
            var slice = StoreFactory.CreateSlice(
                COUNTER_SLICE,
                new CounterState(0),
                (builder) =>
            {
                builder.AddCase(increment, Reducers.Increment);
                builder.AddCase(decrement, Reducers.Decrement);
                builder.AddCase(incrementBy, Reducers.IncrementBy);
            },
                (builder) =>
            {
                builder.AddCase(reset, Reducers.Reset);
            });

            // Now you need to create a store that will hold the state of your application
            // Store creation can occur only when all its dependencies are created and configured
            // (slices, reducers, actions, enhancers, middlewares).
            var store = StoreFactory.CreateStore(new[] { slice });

            // You can then retrieve the state of a slice of state from the store
            // The state is immutable, so you can't modify it directly
            // You need to dispatch an action to the store to modify the state
            Debug.Log($"[State]: Counter state is {store.GetState<CounterState>(COUNTER_SLICE)}");

            // You can then subscribe to the store to be notified when the state changes
            // The store will notify you of any change to the state, so you need to filter the changes yourself
            // The subscribe method returns an unsubscribe function that you can call to unsubscribe from the store
            Debug.Log($"[Subscribe]: Subscribing to the store for changes to the counter slice");
            var unSub = store.Subscribe(
                s => s.Get<CounterState>(COUNTER_SLICE),
                state =>
            {
                Debug.Log($"[Notification]: Counter state has changed to {state}");
            });

            // You can then dispatch actions to the store
            // The store will call the reducers that are bound to the action
            // The reducers will then return a new state that will be stored in the store
            // The store will then notify all subscribers of the change
            Debug.Log($"[Dispatch]: Dispatching Increment action");
            store.Dispatch(increment.Invoke());

            Debug.Log($"[State]: Counter state is {store.GetState<CounterState>(COUNTER_SLICE)}");

            // Call the unsubscribe function to stop receiving notifications
            Debug.Log($"[Unsubscribe]: Unsubscribing from the store");
            unSub.Dispose();

            Debug.Log($"[Dispatch]: Dispatching Reset action");
            store.Dispatch(reset.Invoke());

            Debug.Log($"[State]: Counter state is {store.GetState<CounterState>(COUNTER_SLICE)}");

            Debug.Log($"[Dispatch]: Dispatching IncrementBy action with payload 10");
            store.Dispatch(incrementBy.Invoke(10));

            Debug.Log($"[State]: Counter state is {store.GetState<CounterState>(COUNTER_SLICE)}");
        }
    }
}
