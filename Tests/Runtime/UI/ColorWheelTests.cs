using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ColorWheel))]
    class ColorWheelTests : VisualElementTests<ColorWheel>
    {
        protected override string mainUssClassName => ColorWheel.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:ColorWheel />",
            @"<appui:ColorWheel value=""0"" opacity=""1"" brightness=""1"" saturation=""1"" inner-radius=""0.4"" checker-size=""4"" checker-color-1=""#FFF"" checker-color-2=""#000"" increment-factor=""0.1""  />",
        };
    }
}
