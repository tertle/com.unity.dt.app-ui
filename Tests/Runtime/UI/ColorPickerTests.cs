using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ColorPicker))]
    class ColorPickerTests : VisualElementTests<ColorPicker>
    {
        protected override string mainUssClassName => ColorPicker.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:ColorPicker />",
            @"<appui:ColorPicker value=""#FF0000"" previous-value=""#00FF00"" show-alpha=""true"" show-hex=""true"" show-toolbar=""true"" />",
        };
    }
}
