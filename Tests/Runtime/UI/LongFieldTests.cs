using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(LongField))]
    class LongFieldTests : VisualElementTests<LongField>
    {
        protected override string mainUssClassName => LongField.ussClassName;
    }
}
