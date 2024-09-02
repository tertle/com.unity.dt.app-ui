using NUnit.Framework;
using Unity.AppUI.Bridge;

namespace Unity.AppUI.Tests.Bridge
{
    [TestFixture]
    [TestOf(typeof(VisualElementExtensionsBridge))]
    class VisualElementExtensionsBridgeTests
    {
        [Test, Order(1)]
        public void CanGetAndSetPseudoStates()
        {
            var element = new UnityEngine.UIElements.VisualElement();

            Assert.IsFalse(element.GetPseudoStates().HasFlag(PseudoStates.Checked));

            Assert.DoesNotThrow(() =>
            {
                element.SetPseudoStates(PseudoStates.Checked);
            });

            Assert.IsTrue(element.GetPseudoStates().HasFlag(PseudoStates.Checked));
        }

        [Test, Order(2)]
        public void CanGetWorldBoundingBox()
        {
            var element = new UnityEngine.UIElements.VisualElement();

            Assert.DoesNotThrow(() =>
            {
                element.GetWorldBoundingBox();
            });
        }

        [Test, Order(3)]
        public void CanGetAndSetIsCompositeRoot()
        {
            var element = new UnityEngine.UIElements.VisualElement { focusable = true };

            Assert.IsFalse(element.GetIsCompositeRoot());

            Assert.DoesNotThrow(() =>
            {
                element.SetIsCompositeRoot(true);
            });

            Assert.IsTrue(element.GetIsCompositeRoot());
        }

        [Test, Order(4)]
        public void CanGetAndSetExcludeFromFocusRing()
        {
            var element = new UnityEngine.UIElements.VisualElement { focusable = true };

            Assert.DoesNotThrow(() =>
            {
                element.SetIsCompositeRoot(true);
            });

            Assert.IsFalse(element.GetExcludeFromFocusRing());

            Assert.DoesNotThrow(() =>
            {
                element.SetExcludeFromFocusRing(true);
            });

            Assert.IsTrue(element.GetExcludeFromFocusRing());
        }
    }
}
