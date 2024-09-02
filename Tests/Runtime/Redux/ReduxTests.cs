using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.AppUI.Redux;
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

        CounterState Increment(CounterState state, Action action)
        {
            return state with { value = state.value + 1 };
        }

        CounterState Decrement(CounterState state, Action action)
        {
            return state with { value = state.value - 1 };
        }

        CounterState IncrementBy(CounterState state, AppUI.Redux.Action<int> action)
        {
            return state with { value = state.value + action.payload };
        }

        CounterState Set(CounterState state, AppUI.Redux.Action<int> action)
        {
            return state with { value = action.payload };
        }

        CounterState Reset(CounterState state, Action action)
        {
            return state with { value = 0 };
        }

        CounterState OnPending(CounterState state, Action action)
        {
            return state with { loadingState = LoadingState.Pending };
        }

        CounterState OnFulfilled(CounterState state, Action action)
        {
            return state with { loadingState = LoadingState.Fulfilled };
        }

        CounterState OnRejected(CounterState state, Action action)
        {
            return state with { loadingState = LoadingState.Rejected };
        }

        ActionCreator m_IncrementAction;

        ActionCreator m_DecrementAction;

        ActionCreator<int> m_IncrementByAction;

        ActionCreator<int> m_SetAction;

        ActionCreator m_ResetAction;

        AsyncThunkActionCreator<bool, int> m_LoadDataAction;

        Unsubscriber m_CounterSubscription;

        Store m_Store;

        int m_ExpectedCounterValue = 0;

        bool m_Called = false;

        [Test, Order(1)]
        public void ShouldCreateAStore()
        {
            Assert.DoesNotThrow(() => m_Store = new Store());
            Assert.IsNotNull(m_Store);
        }

        [Test, Order(2)]
        public void ShouldCreateAnEmptySlice()
        {
            Slice<DummyState> slice = null;
            Assert.DoesNotThrow(() => slice = m_Store.CreateSlice<DummyState>("dummy", new DummyState(), builder => { }));
            Assert.IsNotNull(slice);

            Assert.Throws<ArgumentException>(() => m_Store.CreateSlice("dummy", new DummyState(), builder => { }));

            slice = new Slice<DummyState>("dummy", new Dictionary<string, ActionCreator>(), new DummyState());
            Assert.IsNotNull(slice);
            Assert.AreEqual("dummy", slice.name);
            Assert.AreEqual(0, slice.actionCreators.Count);
            Assert.IsNotNull(slice.initialState);
        }

        [Test, Order(3)]
        public void ShouldCreateActions()
        {
            m_IncrementAction = null;
            Assert.DoesNotThrow(() => m_IncrementAction = Store.CreateAction("counter/Increment"));
            Assert.IsNotNull(m_IncrementAction);

            m_IncrementByAction = null;
            Assert.DoesNotThrow(() => m_IncrementByAction = Store.CreateAction<int>("counter/IncrementBy"));
            Assert.IsNotNull(m_IncrementByAction);

            m_ResetAction = null;
            Assert.DoesNotThrow(() => m_ResetAction = Store.CreateAction("Reset"));
            Assert.IsNotNull(m_ResetAction);

            m_LoadDataAction = null;
            Assert.DoesNotThrow(() => m_LoadDataAction = Store.CreateAsyncThunk<bool, int>(
                "LoadData", async (id, thunkAPI, token) =>
            {
                await Task.Delay(250, token);
                return true;
            }));
            Assert.IsNotNull(m_LoadDataAction);
        }

        [Test, Order(3)]
        public void ShouldThrowWhenUsingLambda()
        {
            Assert.Throws<ArgumentException>(() => m_Store.CreateSlice("err0", new DummyState(), builder =>
            {
                builder.Add(((state, action) => state));
            }));

            Assert.Throws<ArgumentException>(() => m_Store.CreateSlice("err1", new DummyState(), builder =>
            {
                builder.Add<int>(((state, action) => state));
            }));
        }

        [Test, Order(4)]
        public void ShouldCreateASliceWithReducers()
        {
            Slice<CounterState> slice = null;

            Assert.DoesNotThrow(() => slice = m_Store.CreateSlice("counter", new CounterState(0), builder =>
            {
                builder.Add("counter/Increment", Increment);
                builder.Add(Decrement);
                builder.Add<int>("counter/IncrementBy", IncrementBy);
                builder.Add<int>(Set);
            }, builder =>
            {
                builder.AddCase(m_ResetAction, Reset);
            }));
            Assert.IsNotNull(slice);

            Assert.AreEqual(0, slice.initialState.value);
            Assert.AreEqual(4, slice.actionCreators.Count);

            foreach (var actionCreatorPerType in slice.actionCreators)
            {
                Assert.IsNotNull(actionCreatorPerType);
                Assert.AreEqual(actionCreatorPerType.Key, actionCreatorPerType.Value.type);

                if (actionCreatorPerType.Key == "counter/Decrement")
                    m_DecrementAction = actionCreatorPerType.Value;

                if (actionCreatorPerType.Key == "counter/Set")
                    m_SetAction = (ActionCreator<int>)actionCreatorPerType.Value;
            }

            Assert.IsNotNull(m_DecrementAction);
            Assert.IsNotNull(m_SetAction);

            var slices = m_Store.GetState();
            Assert.AreEqual(2, slices.Count);
            Assert.IsTrue(slices.ContainsKey("dummy"));
            Assert.IsTrue(slices.ContainsKey("counter"));

            var incrementActionCreator = slice.actionCreators["counter/Increment"];
            Assert.DoesNotThrow(() => slice = m_Store.CreateSlice("otherSlice", new CounterState(0),
                _ => {}, builder =>
                {
                    builder.AddCase(incrementActionCreator, Increment);
                    builder.AddCase(m_LoadDataAction.pending, OnPending);
                    builder.AddCase(m_LoadDataAction.fulfilled,OnFulfilled);
                    builder.AddCase(m_LoadDataAction.rejected, OnRejected);
                }));

            Assert.IsNotNull(slice);
            Assert.AreEqual(0, slice.initialState.value);
            Assert.AreEqual(0, slice.actionCreators.Count);

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
            Assert.DoesNotThrow(() => m_CounterSubscription = m_Store.Subscribe<CounterState>("counter", OnCounterStateChanged));
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
            Assert.DoesNotThrow(() => m_Store.DispatchAsyncThunk(asyncThunkAction));
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
            Assert.DoesNotThrow(() => m_CounterSubscription());
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
