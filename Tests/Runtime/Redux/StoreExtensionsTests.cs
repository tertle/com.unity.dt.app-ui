using System;
using NUnit.Framework;
using Unity.AppUI.Redux;

namespace Unity.AppUI.Tests.Redux
{
    [TestFixture]
    [TestOf(typeof(StoreExtensions))]
    class StoreExtensionsTests
    {
        const string dummySliceName = "dummySlice";

        record DummyState
        {
            public IAction action;
        }

        static readonly ActionCreator k_DummyAction = dummySliceName + "/action";
        static readonly ActionCreator<int> k_DummyActionWithPayload = dummySliceName + "/actionWithPayload";

        static DummyState DummyActionReducer(DummyState state, IAction action)
        {
            return state with { action = action };
        }

        static DummyState DummyActionWithPayloadReducer(DummyState state, IAction<int> action)
        {
            return state with { action = action };
        }

        [Test]
        public void Dispatch_ActionTypeStringValue_CreatesAction()
        {
            var store = StoreFactory.CreateStore(new[]
            {
                StoreFactory.CreateSlice(dummySliceName, new DummyState(), reducers =>
                {
                    reducers.AddCase(k_DummyAction, DummyActionReducer);
                })
            });

            Assert.DoesNotThrow(() => store.Dispatch(k_DummyAction.type));
            var state = store.GetState<DummyState>(dummySliceName);
            Assert.AreEqual(k_DummyAction.type, state.action.type);
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Dispatch(null, k_DummyAction.type));
            Assert.Throws<ArgumentNullException>(() => store.Dispatch(string.Empty));
        }

        [Test]
        public void Dispatch_ActionTypeWithPayloadStringValue_CreatesAction()
        {
            var store = StoreFactory.CreateStore(new[]
            {
                StoreFactory.CreateSlice(dummySliceName, new DummyState(), reducers =>
                {
                    reducers.AddCase(k_DummyActionWithPayload, DummyActionWithPayloadReducer);
                })
            });

            Assert.DoesNotThrow(() => store.Dispatch(k_DummyActionWithPayload.type, 42));
            var state = store.GetState<DummyState>(dummySliceName);
            Assert.AreEqual(k_DummyActionWithPayload.type, state.action.type);
            Assert.AreEqual(42, ((IAction<int>)state.action).payload);
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Dispatch(null, k_DummyActionWithPayload.type, 42));
            Assert.Throws<ArgumentNullException>(() => store.Dispatch(string.Empty, 42));
        }

        [Test]
        public void Dispatch_ActionCreator_CreatesAction()
        {
            var store = StoreFactory.CreateStore(new[]
            {
                StoreFactory.CreateSlice(dummySliceName, new DummyState(), reducers =>
                {
                    reducers.AddCase(k_DummyAction, DummyActionReducer);
                })
            });

            Assert.DoesNotThrow(() => store.Dispatch(k_DummyAction));
            var state = store.GetState<DummyState>(dummySliceName);
            Assert.AreEqual(k_DummyAction.type, state.action.type);
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Dispatch(null, k_DummyAction));
            Assert.Throws<ArgumentNullException>(() => store.Dispatch((ActionCreator)null));
        }

        [Test]
        public void Dispatch_ActionCreatorWithPayload_CreatesAction()
        {
            var store = StoreFactory.CreateStore(new[]
            {
                StoreFactory.CreateSlice(dummySliceName, new DummyState(), reducers =>
                {
                    reducers.AddCase(k_DummyActionWithPayload, DummyActionWithPayloadReducer);
                })
            });

            Assert.DoesNotThrow(() => store.Dispatch(k_DummyActionWithPayload, 42));
            var state = store.GetState<DummyState>(dummySliceName);
            Assert.AreEqual(k_DummyActionWithPayload.type, state.action.type);
            Assert.AreEqual(42, ((IAction<int>)state.action).payload);
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Dispatch(null, k_DummyActionWithPayload, 42));
            Assert.Throws<ArgumentNullException>(() => store.Dispatch((ActionCreator<int>)null, 42));
        }

        [Test]
        public void GetState_WithSliceName_ReturnsState()
        {
            var store = StoreFactory.CreateStore(new[]
            {
                StoreFactory.CreateSlice(dummySliceName, new DummyState(), reducers =>
                {
                    reducers.AddCase(k_DummyAction, DummyActionReducer);
                })
            });

            store.Dispatch(k_DummyAction.type);
            var state = store.GetState<DummyState>(dummySliceName);
            Assert.AreEqual(k_DummyAction.type, state.action.type);
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.GetState<DummyState>(null, dummySliceName));
            Assert.Throws<ArgumentNullException>(() => store.GetState<DummyState>(string.Empty));
        }

        [Test]
        public void Subscribe_ToSliceStateChange()
        {
            var store = StoreFactory.CreateStore(new[]
            {
                StoreFactory.CreateSlice(dummySliceName, new DummyState(), reducers =>
                {
                    reducers.AddCase(k_DummyAction, DummyActionReducer);
                })
            });

            var called = false;
            var sub = StoreExtensions.Subscribe(store, dummySliceName, sliceState =>
            {
                Assert.AreEqual(k_DummyAction.type, sliceState.action.type);
                called = true;
            }, new SubscribeOptions<DummyState> { fireImmediately = false });
            Assert.IsNotNull(sub);
            store.Dispatch(k_DummyAction.type);
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Subscribe<PartitionedState,DummyState>(null, dummySliceName, _ => { }));
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Subscribe(store, string.Empty, (DummyState _) => { }));
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Subscribe<PartitionedState,DummyState>(store, dummySliceName, null));

            Assert.IsTrue(called);
        }

        static IAction Selector(DummyState state) => state.action;

        [Test]
        public void Subscribe_ToSliceStateChange_WithSelector()
        {
            var store = StoreFactory.CreateStore(new[]
            {
                StoreFactory.CreateSlice(dummySliceName, new DummyState(), reducers =>
                {
                    reducers.AddCase(k_DummyAction, DummyActionReducer);
                })
            });

            var selector = new Selector<DummyState, IAction>(Selector);
            var called = false;
            var sub = StoreExtensions.Subscribe(store, dummySliceName, selector, actionInState =>
            {
                Assert.AreEqual(k_DummyAction.type, actionInState.type);
                called = true;
            }, new SubscribeOptions<IAction> { fireImmediately = false });
            Assert.IsNotNull(sub);
            store.Dispatch(k_DummyAction.type);
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Subscribe<PartitionedState,IAction,DummyState>(null, dummySliceName, selector, _ => { }));
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Subscribe(store, string.Empty, selector, _ => { }));
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Subscribe(store, dummySliceName, (Selector<DummyState, IAction>)null, _ => { }));
            Assert.Throws<ArgumentNullException>(() => StoreExtensions.Subscribe(store, dummySliceName, selector, null));

            Assert.IsTrue(called);
        }
    }
}
