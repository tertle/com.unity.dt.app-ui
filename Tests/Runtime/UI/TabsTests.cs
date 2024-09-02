using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Tabs))]
    class TabsTests : VisualElementTests<Tabs>
    {
        protected override string mainUssClassName => Tabs.ussClassName;
    }
}
