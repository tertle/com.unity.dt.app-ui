using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Tray))]
    class TrayTests : PopupTests<Tray>
    {
        protected override Tray CreatePopup()
        {
            return Tray.Build(GetReferenceElement(), GetContentElement());
        }

        protected override bool shouldContainView => false;
    }
}
