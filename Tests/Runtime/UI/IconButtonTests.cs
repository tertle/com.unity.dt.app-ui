using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(IconButton))]
    class IconButtonTests : VisualElementTests<IconButton>
    {
        protected override string mainUssClassName => IconButton.ussClassName;
    }
}
