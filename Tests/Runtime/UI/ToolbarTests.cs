using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Toolbar))]
    class ToolbarTests : VisualElementTests<Toolbar>
    {
        protected override string mainUssClassName => Toolbar.ussClassName;
    }
}
