using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(RectField))]
    class RectFieldTests : VisualElementTests<RectField>
    {
        protected override string mainUssClassName => RectField.ussClassName;
    }
}
