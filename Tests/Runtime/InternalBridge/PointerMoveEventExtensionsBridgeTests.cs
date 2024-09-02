using NUnit.Framework;
using Unity.AppUI.Bridge;

namespace Unity.AppUI.Tests.Bridge
{
    [TestFixture]
    [TestOf(typeof(PointerMoveEventExtensionsBridge))]
    class PointerMoveEventExtensionsBridgeTests
    {
        [Test]
        public void CanGetAndSetIsHandledByDraggable()
        {
            var evt = UnityEngine.UIElements.PointerMoveEvent.GetPooled();

            Assert.IsFalse(evt.GetIsHandledByDraggable());

            Assert.DoesNotThrow(() =>
            {
                evt.SetIsHandledByDraggable(true);
            });

            Assert.IsTrue(evt.GetIsHandledByDraggable());
        }
    }
}
