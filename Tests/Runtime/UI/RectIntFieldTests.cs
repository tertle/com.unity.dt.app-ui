using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(RectIntField))]
    class RectIntFieldTests : VisualElementTests<RectIntField>
    {
        protected override string mainUssClassName => RectIntField.ussClassName;
    }
}
