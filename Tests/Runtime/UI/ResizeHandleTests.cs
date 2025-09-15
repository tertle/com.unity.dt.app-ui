using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ResizeHandle))]
    class ResizeHandleTests : VisualElementTests<ResizeHandle>
    {
        protected override string mainUssClassName => ResizeHandle.ussClassName;
    }
}
