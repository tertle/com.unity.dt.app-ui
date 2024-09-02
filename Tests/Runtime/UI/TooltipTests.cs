using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Tooltip))]
    class TooltipTests : AnchorPopupTests<Tooltip>
    {
        protected override AnchorPopup<Tooltip> CreatePopup()
        {
            return Tooltip.Build(GetReferenceElement());
        }

        protected override bool shouldContainView => false;

        protected override void OnCanBuildPopupTested()
        {
            base.OnCanBuildPopupTested();

            Assert.IsNotNull(popup);
            Assert.IsNull(popup.anchor);
            Assert.AreEqual(0, popup.offset);
            Assert.AreEqual(PopoverPlacement.Bottom, popup.placement);
            Assert.IsTrue(popup.arrowVisible);
            Assert.AreEqual(0, popup.containerPadding);
            Assert.AreEqual(0, popup.crossOffset);
            Assert.AreEqual(PopoverPlacement.Bottom, popup.currentPlacement);
            Assert.IsTrue(popup.shouldFlip);
            Assert.IsTrue(popup.outsideClickDismissEnabled);
        }
    }
}
