using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(InputLabel))]
    class InputLabelTests : VisualElementTests<InputLabel>
    {
        protected override string mainUssClassName => InputLabel.ussClassName;
    }
}
