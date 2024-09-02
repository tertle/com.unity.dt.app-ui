using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Divider))]
    class DividerTests : VisualElementTests<Divider>
    {
        protected override string mainUssClassName => Divider.ussClassName;
    }
}
