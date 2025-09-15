using System;
using NUnit.Framework;
using Unity.AppUI.Redux;

namespace Unity.AppUI.Tests.Redux
{
    [TestFixture]
    [TestOf(typeof(Store<>))]
    class StoreTests
    {
        [Test]
        public void CanCreateStore()
        {
            var store = new Store<PartitionedState>((state, _) => state, new PartitionedState());
            Assert.IsNotNull(store);
            Assert.IsNotNull(store.GetState());
            Assert.Throws<ArgumentNullException>(() => store.Dispatch((IAction)null));
        }

        [Test]
        public void CanDispatch()
        {
            var store = new Store<PartitionedState>((state, _) => state, new PartitionedState());
            Assert.Throws<ArgumentNullException>(() => store.Dispatch((IAction)null));
            Assert.DoesNotThrow(() => store.Dispatch(new ActionCreator("test").Invoke()));
        }

        [Test]
        public void CanSubscribe()
        {
            var store = new Store<PartitionedState>((state, _) => state, new PartitionedState());
            IDisposableSubscription subscription = null;
            Assert.Throws<ArgumentNullException>(() => store.Subscribe(null));
            Assert.DoesNotThrow(() => subscription = store.Subscribe(_ => { }, new SubscribeOptions<PartitionedState> { fireImmediately = true }));
            Assert.IsNotNull(subscription);
            Assert.IsTrue(subscription.IsValid());
            Assert.DoesNotThrow(() => subscription.Dispose());
        }

        [Test]
        public void CanSubscribeWithSelector()
        {
            var store = new Store<PartitionedState>((state, _) => state, new PartitionedState());
            IDisposableSubscription subscription = null;
            Assert.Throws<ArgumentNullException>(() => store.Subscribe<PartitionedState>(null,
                _ => { }));
            Assert.Throws<ArgumentNullException>(() => store.Subscribe(state => state, null));
            Assert.DoesNotThrow(() => subscription = store.Subscribe(state => state, _ => { }, new SubscribeOptions<PartitionedState> { fireImmediately = true }));
            Assert.IsNotNull(subscription);
            Assert.IsTrue(subscription.IsValid());
            Assert.DoesNotThrow(() => subscription.Dispose());
        }
    }
}
