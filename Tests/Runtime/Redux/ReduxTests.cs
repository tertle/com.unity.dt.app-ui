using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.AppUI.Redux;
using UnityEngine;
using UnityEngine.TestTools;
using Action = Unity.AppUI.Redux.Action;

namespace Unity.AppUI.Tests.Redux
{
    [TestFixture]
    class ReduxTests
    {
        record DummyState {}

        enum LoadingState { Idle, Pending, Fulfilled, Rejected }

        record CounterState(int value)
        {
            public int value { get; set; } = value;

            public LoadingState loadingState { get; set; } = LoadingState.Idle;
        }

        CounterState Increment(CounterState state, IAction action)
        {
            return state with { value = state.value + 1 };
        }

        CounterState Decrement(CounterState state, IAction action)
        {
            return state with { value = state.value - 1 };
        }

        CounterState IncrementBy(CounterState state, IAction<int> action)
        {
            return state with { value = state.value + action.payload };
        }

        CounterState Set(CounterState state, IAction<int> action)
        {
            return state with { value = action.payload };
        }

        CounterState Reset(CounterState state, IAction action)
        {
            return state with { value = 0 };
        }

        CounterState OnPending(CounterState state, IAction action)
        {
            return state with { loadingState = LoadingState.Pending };
        }

        CounterState OnFulfilled(CounterState state, IAction action)
        {
            return state with { loadingState = LoadingState.Fulfilled };
        }

        CounterState OnRejected(CounterState state, IAction action)
        {
            return state with { loadingState = LoadingState.Rejected };
        }

        ActionCreator m_IncrementAction;

        ActionCreator m_DecrementAction;

        ActionCreator<int> m_IncrementByAction;

        ActionCreator<int> m_SetAction;

        ActionCreator m_ResetAction;

        AsyncThunkCreator<int,bool> m_LoadDataAction;

        IDisposableSubscription m_CounterSubscription;

        IStore<PartitionedState> m_Store;

        int m_ExpectedCounterValue;

        bool m_Called;

        [Test]
        public void CanComposeEnhancers()
        {
            var result = 0;
            var enhancer1 = new StoreEnhancer<PartitionedState>(createStore => (reducer, initialState) =>
            {
                result += 2;
                var store = createStore(reducer, initialState);
                return store;
            });
            var enhancer2 = new StoreEnhancer<PartitionedState>(createStore => (reducer, initialState) =>
            {
                result *= 2;
                var store = createStore(reducer, initialState);
                return store;
            });
            var composedEnhancer = StoreFactory.ComposeEnhancers(enhancer2, enhancer1);
            var store = StoreFactory.CreateStore((state, _) => state, new PartitionedState(), composedEnhancer);
            Assert.IsNotNull(store);
            Assert.AreEqual(4, result);
        }

        [Test]
        public void CanCombineReducers()
        {
            Reducer<PartitionedState> combinedReducer = null;
            var reducer1 = new Reducer<PartitionedState>((state, _) => state);
            var reducer2 = new Reducer<PartitionedState>((state, _) => state);
            Assert.DoesNotThrow(() => combinedReducer = StoreFactory.CombineReducers(new List<Reducer<PartitionedState>>
            {
                reducer1,
                reducer2,
            }));
            Assert.IsNotNull(combinedReducer);
            Assert.DoesNotThrow(() => combinedReducer.Invoke(new PartitionedState(), new Action("")));
        }

        [Test, Order(1)]
        public void ShouldCreateAStore()
        {
            Assert.Throws(typeof(ArgumentNullException), () => StoreFactory.CreateStore((ISlice<PartitionedState>[]) null));
            Assert.Throws(typeof(ArgumentNullException), () => StoreFactory.CreateStore((Reducer<PartitionedState>) null));
            Assert.DoesNotThrow(() => m_Store = StoreFactory.CreateStore((state, _) => state));
            Assert.IsNotNull(m_Store);
            Assert.IsNotNull(m_Store.GetState());
        }

        [Test, Order(2)]
        public void ShouldCreateAnEmptySlice()
        {
            Assert.Throws<ArgumentException>(() => StoreFactory.CreateSlice(null, new DummyState(), null));

            Slice<DummyState,PartitionedState> slice = null;
            Assert.DoesNotThrow(() => slice = StoreFactory.CreateSlice("dummy", new DummyState(), null));
            Assert.IsNotNull(slice);

            slice = new Slice<DummyState,PartitionedState>("dummy", new DummyState(), null);
            Assert.IsNotNull(slice);
            Assert.AreEqual("dummy", slice.name);
            Assert.AreEqual(0, slice.actionCreators.Count);
            Assert.IsNotNull(slice.initialState);
            Assert.IsNotNull(slice.reducer);
        }

        [Test, Order(3)]
        public void ShouldCreateActions()
        {
            m_IncrementAction = null;
            Assert.DoesNotThrow(() => m_IncrementAction = new ActionCreator("counter/Increment"));
            Assert.IsNotNull(m_IncrementAction);
            Assert.AreEqual("counter/Increment", m_IncrementAction.type);
            Assert.IsTrue(m_IncrementAction.Match(new Action("counter/Increment")));
            Assert.IsFalse(m_IncrementAction.Match(new Action("counter/Decrement")));

            var incrementAction = m_IncrementAction.Invoke();
            Assert.IsNotNull(incrementAction);
            Assert.AreEqual("counter/Increment", incrementAction.type);
            Assert.IsTrue(incrementAction.Equals((object) incrementAction));
            Assert.IsTrue(incrementAction.GetHashCode() != 0);

            m_IncrementByAction = null;
            Assert.DoesNotThrow(() => m_IncrementByAction = new ActionCreator<int>("counter/IncrementBy"));
            Assert.IsNotNull(m_IncrementByAction);
            Assert.AreEqual("counter/IncrementBy", m_IncrementByAction.type);
            Assert.IsTrue(m_IncrementByAction.Match(new AppUI.Redux.Action<int>("counter/IncrementBy", 0)));

            var incrementByAction = m_IncrementByAction.Invoke(0);
            Assert.IsNotNull(incrementByAction);
            Assert.AreEqual("counter/IncrementBy", incrementByAction.type);
            Assert.IsTrue(incrementByAction.Equals((object) incrementByAction));
            Assert.IsTrue(incrementByAction.GetHashCode() != 0);

            m_ResetAction = null;
            Assert.DoesNotThrow(() => m_ResetAction = new ActionCreator("Reset"));
            Assert.IsNotNull(m_ResetAction);
            Assert.AreEqual("Reset", m_ResetAction.type);
            Assert.IsTrue(m_ResetAction.Match(new Action("Reset")));

            m_LoadDataAction = null;
            Assert.DoesNotThrow(() => m_LoadDataAction = new AsyncThunkCreator<int,bool>(
                "LoadData", async api =>
            {
                await Task.Delay(250, api.cancellationToken);
                return true;
            }));
            Assert.IsNotNull(m_LoadDataAction);
            Assert.AreEqual("LoadData", m_LoadDataAction.type);
            Assert.IsNotNull(m_LoadDataAction.pending);
            Assert.IsNotNull(m_LoadDataAction.fulfilled);
            Assert.IsNotNull(m_LoadDataAction.rejected);
        }

        [Test, Order(4)]
        public void ShouldCreateASliceWithReducers()
        {
            ISlice<PartitionedState> slice = null;

            Slice<DummyState,PartitionedState> dummySlice = null;
            Slice<CounterState,PartitionedState> counterSlice = null;
            Slice<CounterState,PartitionedState> otherSlice = null;

            Assert.DoesNotThrow(() => slice = counterSlice = StoreFactory.CreateSlice(
                name: "counter",
                initialState: new CounterState(0),
                reducers =>
            {
                reducers.AddCase(m_IncrementAction, Increment);
                reducers.AddCase((ActionCreator)"counter/Decrement", Decrement);
                reducers.AddCase(m_IncrementByAction, IncrementBy);
                reducers.AddCase((ActionCreator<int>)"counter/Set", Set);
            },
                extraReducers =>
            {
                extraReducers.AddCase(m_ResetAction, Reset);
            }));
            Assert.IsNotNull(slice);

            Assert.AreEqual(0, counterSlice.initialState.value);
            Assert.AreEqual(4, counterSlice.actionCreators.Count);

            foreach (var actionCreatorPerType in counterSlice.actionCreators)
            {
                Assert.IsNotNull(actionCreatorPerType);
                Assert.AreEqual(actionCreatorPerType.Key, actionCreatorPerType.Value.type);

                if (actionCreatorPerType.Key == "counter/Decrement")
                    m_DecrementAction = (ActionCreator)actionCreatorPerType.Value;

                if (actionCreatorPerType.Key == "counter/Set")
                    m_SetAction = (ActionCreator<int>)actionCreatorPerType.Value;
            }

            Assert.IsNotNull(m_DecrementAction);
            Assert.IsNotNull(m_SetAction);

            Assert.DoesNotThrow(() => slice = dummySlice = StoreFactory.CreateSlice("dummy", new DummyState(), _ => { }));

            m_Store = StoreFactory.CreateStore(new ISlice<PartitionedState>[]
            {
                dummySlice,
                counterSlice,
            });

            var slices = m_Store.GetState();
            Assert.AreEqual(2, slices.Count);
            Assert.IsTrue(slices.ContainsKey("dummy"));
            Assert.IsTrue(slices.ContainsKey("counter"));

            var incrementActionCreator = (ActionCreator)counterSlice.actionCreators["counter/Increment"];
            Assert.DoesNotThrow(() => slice = otherSlice = StoreFactory.CreateSlice("otherSlice", new CounterState(0),
                _ => {}, builder =>
                {
                    builder.AddCase(incrementActionCreator, Increment);
                    builder.AddCase(m_LoadDataAction.pending, OnPending);
                    builder.AddCase(m_LoadDataAction.fulfilled,OnFulfilled);
                    builder.AddCase(m_LoadDataAction.rejected, OnRejected);
                }));

            m_Store = StoreFactory.CreateStore(new ISlice<PartitionedState>[]
            {
                dummySlice,
                counterSlice,
                otherSlice,
            });

            Assert.IsNotNull(slice);
            Assert.AreEqual(0, otherSlice.initialState.value);
            Assert.AreEqual(0, otherSlice.actionCreators.Count);

            Assert.DoesNotThrow(() =>
            {
                m_Store.Dispatch(incrementActionCreator.Invoke());
            });

            Assert.AreEqual(1, m_Store.GetState<CounterState>("counter").value);
            Assert.AreEqual(1, m_Store.GetState<CounterState>("otherSlice").value);

            Assert.DoesNotThrow(() =>
            {
                m_Store.Dispatch(m_ResetAction.Invoke());
            });

            Assert.AreEqual(0, m_Store.GetState<CounterState>("counter").value);
            Assert.AreEqual(1, m_Store.GetState<CounterState>("otherSlice").value);
        }

        [Test, Order(5)]
        public void ShouldSubscribe()
        {
            Assert.DoesNotThrow(() => m_CounterSubscription = m_Store.Subscribe((s) => { }));
            Assert.DoesNotThrow(() => m_Store.Dispatch(m_ResetAction));
            Assert.DoesNotThrow(() => m_CounterSubscription.Dispose());
            Assert.DoesNotThrow(() => m_CounterSubscription = m_Store.Subscribe(s => s.Get<CounterState>("counter"), OnCounterStateChanged));
            Assert.IsNotNull(m_CounterSubscription);
        }

        [UnityTest, Order(6)]
        public IEnumerator ShouldDispatchActions()
        {
            m_ExpectedCounterValue = 1;
            m_Called = false;
            var action = m_IncrementAction.Invoke();
            Assert.AreEqual("counter/Increment", action.type);
            Assert.DoesNotThrow(() => m_Store.Dispatch(action));
            Assert.IsTrue(m_Called);

            Assert.AreEqual(m_ExpectedCounterValue, m_Store.GetState<CounterState>("counter").value);

            m_ExpectedCounterValue = 0;
            m_Called = false;
            Assert.DoesNotThrow(() => m_Store.Dispatch(m_DecrementAction.Invoke()));
            Assert.IsTrue(m_Called);

            m_ExpectedCounterValue = 5;
            m_Called = false;
            action = m_IncrementByAction.Invoke(5);
            Assert.AreEqual("counter/IncrementBy", action.type);
            Assert.AreEqual(5, ((AppUI.Redux.Action<int>)action).payload);
            Assert.DoesNotThrow(() => m_Store.Dispatch(action));
            Assert.IsTrue(m_Called);

            m_ExpectedCounterValue = 10;
            m_Called = false;
            Assert.DoesNotThrow(() => m_Store.Dispatch(m_SetAction.Invoke(10)));
            Assert.IsTrue(m_Called);

            m_ExpectedCounterValue = 0;
            m_Called = false;
            Assert.DoesNotThrow(() => m_Store.Dispatch(m_ResetAction.Invoke()));
            Assert.IsTrue(m_Called);

            var asyncThunkAction = m_LoadDataAction.Invoke(0);
            Assert.AreEqual("LoadData", asyncThunkAction.type);
            Assert.DoesNotThrow(() => _ = m_Store.DispatchAsyncThunk(asyncThunkAction));
            Assert.AreEqual(LoadingState.Pending, m_Store.GetState<CounterState>("otherSlice").loadingState);

            yield return new WaitUntilOrTimeOut(
                () => m_Store.GetState<CounterState>("otherSlice").loadingState == LoadingState.Fulfilled,
                false,
                TimeSpan.FromSeconds(2));

            Assert.AreEqual(LoadingState.Fulfilled, m_Store.GetState<CounterState>("otherSlice").loadingState);
        }

        [Test, Order(7)]
        public void ShouldUnsubscribe()
        {
            Assert.DoesNotThrow(() => m_CounterSubscription.Dispose());
            m_Called = false;
            Assert.DoesNotThrow(() => m_Store.Dispatch(m_IncrementAction.Invoke()));
            Assert.IsFalse(m_Called);
        }

        void OnCounterStateChanged(CounterState state)
        {
            Assert.AreEqual(m_ExpectedCounterValue, state.value);
            m_Called = true;
        }
    }
}
