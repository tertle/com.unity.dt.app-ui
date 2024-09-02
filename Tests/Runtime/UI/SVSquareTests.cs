using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(SVSquare))]
    class SVSquareTests : VisualElementTests<SVSquare>
    {
        protected override string mainUssClassName => SVSquare.ussClassName;
    }
}
