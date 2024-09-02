using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Badge))]
    class BadgeTests : VisualElementTests<Badge>
    {
        protected override string mainUssClassName => Badge.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:Badge />",
            @"<ui:VisualElement style=""width: 100px; height: 100px;"">
                <appui:Badge variant=""Default"" content=""5"" background-color=""blue"" overlap-type=""Rectangular"" horizontal-anchor=""Right"" vertical-anchor=""Top"" max=""1"" show-zero=""true"" color=""white"" />
            </ui:VisualElement>",
        };
    }
}
