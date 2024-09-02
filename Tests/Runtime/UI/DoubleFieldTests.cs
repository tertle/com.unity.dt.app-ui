using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(DoubleField))]
    class DoubleFieldTests : VisualElementTests<DoubleField>
    {
        protected override string mainUssClassName => DoubleField.ussClassName;
    }
}
