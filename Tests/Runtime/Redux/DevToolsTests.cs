using NUnit.Framework;
using Unity.AppUI.Redux;
using Unity.AppUI.Redux.DevTools;

namespace Unity.AppUI.Tests.Redux
{
    [TestFixture]
    [TestOf(typeof(DevToolsService))]
    class DevToolsTests
    {
        record DummyState {}

        [Test]
        public void CanCreateStoreWithDevTools()
        {
            var defaultEnhancer =
                StoreFactory.DefaultEnhancer<DummyState>(
                    new DefaultEnhancerConfiguration()
                    {
                        devTools = { enabled = true}
                    });
            var store = StoreFactory.CreateStore((state, _) => state with {}, new DummyState(), defaultEnhancer);
            Assert.IsNotNull(store);
            Assert.DoesNotThrow(() => store.Dispatch(new ActionCreator("test").Invoke()));
            Assert.DoesNotThrow(() => store.Dispatch( new ActionCreator<int>("testWithPayload"), 0));
        }
    }
}
