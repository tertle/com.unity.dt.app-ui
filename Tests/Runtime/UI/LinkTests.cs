using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Link))]
    class LinkTests : VisualElementTests<Link>
    {
        protected override string mainUssClassName => Link.ussClassName;
    }
}
