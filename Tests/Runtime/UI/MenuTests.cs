using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Menu))]
    class MenuTests : VisualElementTests<Menu>
    {
        protected override string mainUssClassName => Menu.ussClassName;
    }
}
