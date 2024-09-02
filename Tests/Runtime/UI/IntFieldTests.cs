using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(IntField))]
    class IntFieldTests : NumericalFieldTests<IntField, int>
    {
        protected override string mainUssClassName => IntField.ussClassName;
    }
}
