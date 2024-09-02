using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(BottomNavBarItem))]
    class BottomNavBarItemTests : VisualElementTests<BottomNavBarItem>
    {
        protected override string mainUssClassName => BottomNavBarItem.ussClassName;

        protected override bool uxmlConstructable => false;
    }
}
