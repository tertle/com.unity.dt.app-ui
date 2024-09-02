using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Vector3IntField))]
    class Vector3IntFieldTests : VisualElementTests<Vector3IntField>
    {
        protected override string mainUssClassName => Vector3IntField.ussClassName;
    }
}
