using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(MenuSection))]
    class MenuSectionTests : VisualElementTests<MenuSection>
    {
        protected override string mainUssClassName => MenuSection.ussClassName;
    }
}
