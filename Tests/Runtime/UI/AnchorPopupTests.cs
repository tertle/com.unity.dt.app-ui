using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;

namespace Unity.AppUI.Tests.UI
{
    class AnchorPopupTests<T> : PopupTests<AnchorPopup<T>> where T : AnchorPopup<T>
    {
        protected override void OnCanBuildPopupTested()
        {
            base.OnCanBuildPopupTested();

            var anchorPopup = (AnchorPopup<T>)popup;
            anchorPopup.SetOffset(10);
            Assert.AreEqual(10, anchorPopup.offset);
            anchorPopup.SetOffset(0);
            Assert.AreEqual(0, anchorPopup.offset);
            anchorPopup.SetCrossOffset(10);
            Assert.AreEqual(10, anchorPopup.crossOffset);
            anchorPopup.SetCrossOffset(0);
            Assert.AreEqual(0, anchorPopup.crossOffset);
            anchorPopup.SetContainerPadding(10);
            Assert.AreEqual(10, anchorPopup.containerPadding);
            anchorPopup.SetContainerPadding(0);
            Assert.AreEqual(0, anchorPopup.containerPadding);
            anchorPopup.SetArrowVisible(false);
            Assert.IsFalse(anchorPopup.arrowVisible);
            anchorPopup.SetArrowVisible(true);
            Assert.IsTrue(anchorPopup.arrowVisible);
            anchorPopup.SetShouldFlip(false);
            Assert.IsFalse(anchorPopup.shouldFlip);
            anchorPopup.SetShouldFlip(true);
            Assert.IsTrue(anchorPopup.shouldFlip);
            anchorPopup.SetOutsideClickDismiss(false);
            Assert.IsFalse(anchorPopup.outsideClickDismissEnabled);
            anchorPopup.SetOutsideClickDismiss(true);
            Assert.IsTrue(anchorPopup.outsideClickDismissEnabled);
            anchorPopup.SetOutsideScrollEnabled(false);
            Assert.IsFalse(anchorPopup.outsideScrollEnabled);
            anchorPopup.SetOutsideScrollEnabled(true);
            Assert.IsTrue(anchorPopup.outsideScrollEnabled);
            anchorPopup.SetOutsideClickStrategy(OutsideClickStrategy.Pick);
            Assert.AreEqual(OutsideClickStrategy.Pick, anchorPopup.outsideClickStrategy);
            anchorPopup.SetOutsideClickStrategy(OutsideClickStrategy.Bounds);
            Assert.AreEqual(OutsideClickStrategy.Bounds, anchorPopup.outsideClickStrategy);
        }
    }
}
