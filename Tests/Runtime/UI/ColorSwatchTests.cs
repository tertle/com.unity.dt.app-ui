using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ColorSwatch))]
    class ColorSwatchTests : VisualElementTests<ColorSwatch>
    {
        protected override string mainUssClassName => ColorSwatch.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:ColorSwatch />",
            @"<appui:ColorSwatch value=""Blend:[0.0,#000000FF];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,1.0]"" size=""M"" round=""false"" />",
        };
    }
}
