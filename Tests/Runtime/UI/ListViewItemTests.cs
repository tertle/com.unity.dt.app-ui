using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ListViewItem))]
    class ListViewItemTests : VisualElementTests<ListViewItem>
    {
        protected override string mainUssClassName => ListViewItem.ussClassName;

        protected override bool uxmlConstructable => false;
    }
}
