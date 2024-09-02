using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Popover))]
    class PopoverTests : AnchorPopupTests<Popover>
    {
        protected override AnchorPopup<Popover> CreatePopup()
        {
            return Popover.Build(GetReferenceElement(), GetContentElement());
        }

        protected override void OnCanBuildPopupTested()
        {
            base.OnCanBuildPopupTested();

            var popover = (Popover)popup;

            Assert.IsNotNull(popover);
            Assert.IsNotNull(popover.anchor);
            Assert.AreEqual(0, popover.offset);
            Assert.IsTrue(popover.arrowVisible);
            Assert.AreEqual(0, popover.containerPadding);
            Assert.AreEqual(0, popover.crossOffset);
            Assert.AreEqual(PopoverPlacement.Bottom, popover.currentPlacement);
            Assert.IsTrue(popover.shouldFlip);
            Assert.IsTrue(popover.outsideClickDismissEnabled);

            Assert.IsFalse(popover.modalBackdrop);
            popover.SetModalBackdrop(true);
            Assert.IsTrue(popover.modalBackdrop);

            Assert.IsNotNull(popover.view.contentContainer);

            Assert.AreEqual(PopoverPlacement.Bottom, popover.placement);
            Assert.AreEqual(((IPlaceableElement)popover.view).placement, popover.placement);
            popover.SetPlacement(PopoverPlacement.Left);
            Assert.AreEqual(PopoverPlacement.Left, popover.placement);
            popover.SetPlacement(PopoverPlacement.Right);
            Assert.AreEqual(PopoverPlacement.Right, popover.placement);

            Assert.Throws<ValueOutOfRangeException>(() => popover.SetPlacement((PopoverPlacement)100));
        }
    }
}
