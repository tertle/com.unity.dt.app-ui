using NUnit.Framework;
using Unity.AppUI.Bridge;

namespace Unity.AppUI.Tests.Bridge
{
    [TestFixture]
    [TestOf(typeof(ValueAnimationExtensionsBridge))]
    class ValueAnimationExtensionsBridgeTests
    {
        [Test]
        public void CanGetIsRecycled()
        {
            var element = new UnityEngine.UIElements.VisualElement();
            var animation = element.experimental.animation
                .Start(0f, 1f, 100, (_, _) => { })
                .KeepAlive();

            Assert.IsFalse(animation.IsRecycled());

            animation.Recycle();

            Assert.IsTrue(animation.IsRecycled());
        }
    }
}
