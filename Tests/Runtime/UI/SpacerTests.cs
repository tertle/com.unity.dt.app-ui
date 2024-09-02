using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Spacer))]
    class SpacerTests : VisualElementTests<Spacer>
    {
        protected override string mainUssClassName => Spacer.ussClassName;
    }
}
