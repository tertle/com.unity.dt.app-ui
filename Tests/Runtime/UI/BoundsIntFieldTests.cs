using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(BoundsIntField))]
    class BoundsIntFieldTests : VisualElementTests<BoundsIntField>
    {
        protected override string mainUssClassName => BoundsIntField.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", (ctx) => new BoundsIntField());
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:BoundsIntField />",
            @"<appui:BoundsIntField value=""0,0,100,100"" size=""M"" invalid=""false"" />"
        };
    }
}
