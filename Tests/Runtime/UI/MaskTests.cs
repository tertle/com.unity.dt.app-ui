using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Mask))]
    class MaskTests : VisualElementTests<Mask>
    {
        protected override string mainUssClassName => Mask.ussClassName;
    }
}
