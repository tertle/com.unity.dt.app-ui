using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(NavigationRail))]
    class NavigationRailTests : VisualElementTests<NavigationRail>
    {
        protected override string mainUssClassName => NavigationRail.ussClassName;

    }

    [TestFixture]
    [TestOf(typeof(NavigationRailItem))]
    class NavigationRailItemTests : VisualElementTests<NavigationRailItem>
    {
        protected override string mainUssClassName => NavigationRailItem.ussClassName;

    }
}
