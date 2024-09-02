using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ColorSlider))]
    class ColorSliderTests : VisualElementTests<ColorSlider>
    {
        protected override string mainUssClassName => ColorSlider.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:ColorSlider />",
            @"<appui:ColorSlider size=""M"" value=""0.5"" increment-factor=""0.1"" from=""#FF000000"" to=""#FF0000FF"" />",
            @"<appui:ColorSlider size=""M"" value=""0.5"" low-value=""0"" high-value=""1"" increment-factor=""0.1"" color-range=""Blend:[0.0,#FF000000];[1.0,#FF0000FF]+[0.0,1.0];[1.0,1.0]"" />",
        };
    }
}
