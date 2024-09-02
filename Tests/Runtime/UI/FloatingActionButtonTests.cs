using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(FloatingActionButton))]
    class FloatingActionButtonTests : VisualElementTests<FloatingActionButton>
    {
        protected override string mainUssClassName => FloatingActionButton.ussClassName;
    }
}
