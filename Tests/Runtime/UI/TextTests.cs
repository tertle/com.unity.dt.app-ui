using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Text))]
    class TextTests : VisualElementTests<Text>
    {
        protected override string mainUssClassName => Text.ussClassName;
    }
}
