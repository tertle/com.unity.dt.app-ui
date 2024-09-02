using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(TabItem))]
    class TabItemTests : VisualElementTests<TabItem>
    {
        protected override string mainUssClassName => TabItem.ussClassName;
    }
}
