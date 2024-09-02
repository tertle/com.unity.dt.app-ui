using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Heading))]
    class HeadingTests : VisualElementTests<Heading>
    {
        protected override string mainUssClassName => Heading.ussClassName;
    }
}
