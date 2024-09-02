using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(FieldLabel))]
    class FieldLabelTests : VisualElementTests<FieldLabel>
    {
        protected override string mainUssClassName => FieldLabel.ussClassName;
    }
}
