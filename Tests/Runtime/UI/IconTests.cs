using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Icon))]
    class IconTests : VisualElementTests<Icon>
    {
        protected override string mainUssClassName => Icon.ussClassName;

    }
}
