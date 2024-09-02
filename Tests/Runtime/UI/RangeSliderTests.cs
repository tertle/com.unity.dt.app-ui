using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(RangeSliderFloat))]
    class RangeSliderFloatTests : VisualElementTests<RangeSliderFloat>
    {
        protected override string mainUssClassName => RangeSliderFloat.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"appui:RangeSliderFloat />",
            @"appui:RangeSliderFloat low-value=""0"" high-value=""100"" min-value=""10"" max-value=""90"" size=""M"" filled=""true"" label=""Test"" />",
        };
    }

    [TestFixture]
    [TestOf(typeof(RangeSliderInt))]
    class RangeSliderIntTests : VisualElementTests<RangeSliderInt>
    {
        protected override string mainUssClassName => RangeSliderInt.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"appui:RangeSliderInt />",
            @"appui:RangeSliderInt low-value=""0"" high-value=""100"" min-value=""10"" max-value=""90"" size=""M"" filled=""true"" label=""Test"" />",
        };
    }
}
