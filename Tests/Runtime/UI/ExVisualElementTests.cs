using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ExVisualElement))]
    class ExVisualElementTests : VisualElementTests<ExVisualElement>
    {
        protected override string mainUssClassName => null;
    }
}
