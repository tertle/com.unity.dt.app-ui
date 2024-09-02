using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(SwipeViewItem))]
    class SwipeViewItemTests : VisualElementTests<SwipeViewItem>
    {
        protected override string mainUssClassName => SwipeViewItem.ussClassName;
    }
}
