using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(StackView))]
    class StackViewTests : VisualElementTests<StackView>
    {
        protected override string mainUssClassName => StackView.ussClassName;
    }
}
