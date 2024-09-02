using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Modal))]
    class ModalTests : PopupTests<Modal>
    {
        protected override Modal CreatePopup()
        {
            return Modal.Build(GetReferenceElement(), GetContentElement());
        }

        protected override void OnCanBuildPopupTested()
        {
            Assert.IsNotNull(popup);

            Assert.AreEqual(ModalFullScreenMode.None, popup.fullscreenMode);

            popup.SetFullScreenMode(ModalFullScreenMode.FullScreen);

            Assert.AreEqual(ModalFullScreenMode.FullScreen, popup.fullscreenMode);

            Assert.IsNotNull(popup.view.contentContainer);
        }
    }
}
