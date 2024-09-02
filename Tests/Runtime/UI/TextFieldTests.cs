using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(TextField))]
    class TextFieldTests : VisualElementTests<TextField>
    {
        protected override string mainUssClassName => TextField.ussClassName;
    }
}
