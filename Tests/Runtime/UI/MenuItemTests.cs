using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(MenuItem))]
    class MenuItemTests : VisualElementTests<MenuItem>
    {
        protected override string mainUssClassName => MenuItem.ussClassName;
    }
}
