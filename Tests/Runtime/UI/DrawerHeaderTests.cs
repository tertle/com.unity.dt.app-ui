using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(DrawerHeader))]
    class DrawerHeaderTests : VisualElementTests<DrawerHeader>
    {
        protected override string mainUssClassName => DrawerHeader.ussClassName;

        protected override bool uxmlConstructable => false;
    }
}
