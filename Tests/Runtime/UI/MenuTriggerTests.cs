using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(MenuTrigger))]
    class MenuTriggerTests : VisualElementTests<MenuTrigger>
    {
        protected override string mainUssClassName => null;
    }
}
