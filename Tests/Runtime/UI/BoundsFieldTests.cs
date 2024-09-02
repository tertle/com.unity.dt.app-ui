using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(BoundsField))]
    class BoundsFieldTests : VisualElementTests<BoundsField>
    {
        protected override string mainUssClassName => BoundsField.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:BoundsField />",
            @"<appui:BoundsField value=""0,0,100,100"" size=""M"" invalid=""false"" />"
        };
    }
}
