using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Button))]
    class ButtonTests : VisualElementTests<Button>
    {
        protected override string mainUssClassName => Button.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:Button />",
            @"<appui:Button title=""Button"" />",
            @"<appui:Button title=""Button"" leading-icon=""info"" />",
            @"<appui:Button title=""Button"" trailing-icon=""info"" />",
            @"<appui:Button title=""Button"" leading-icon=""info"" trailing-icon=""info"" />",
            @"<appui:Button title=""Button"" leading-icon=""info"" trailing-icon=""info"" subtitle=""Subtitle"" />",
            @"<appui:Button title=""Button"" size=""M"" variant=""Accent"" quiet=""false"" />",
        };
    }
}
