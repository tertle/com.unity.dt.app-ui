using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(PageIndicator))]
    class PageIndicatorTests : VisualElementTests<PageIndicator>
    {
        protected override string mainUssClassName => PageIndicator.ussClassName;
    }
}
