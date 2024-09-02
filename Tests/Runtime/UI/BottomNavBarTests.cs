using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(BottomNavBar))]
    class BottomNavBarTests : VisualElementTests<BottomNavBar>
    {
        protected override string mainUssClassName => BottomNavBar.ussClassName;

        protected override bool uxmlConstructable => false;
    }
}
