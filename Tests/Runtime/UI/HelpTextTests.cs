using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(HelpText))]
    class HelpTextTests : VisualElementTests<HelpText>
    {
        protected override string mainUssClassName => HelpText.ussClassName;
    }
}
