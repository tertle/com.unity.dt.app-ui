using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(PageView))]
    class PageViewTests : VisualElementTests<PageView>
    {
        protected override string mainUssClassName => PageView.ussClassName;
    }
}
