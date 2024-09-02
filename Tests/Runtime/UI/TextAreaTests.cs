using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(TextArea))]
    class TextAreaTests : VisualElementTests<TextArea>
    {
        protected override string mainUssClassName => TextArea.ussClassName;
    }
}
