using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Toggle))]
    class ToggleTests : VisualElementTests<Toggle>
    {
        protected override string mainUssClassName => Toggle.ussClassName;
    }
}
