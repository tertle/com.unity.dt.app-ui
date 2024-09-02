using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Vector2IntField))]
    class Vector2IntFieldTests : VisualElementTests<Vector2IntField>
    {
        protected override string mainUssClassName => Vector2IntField.ussClassName;
    }
}
