using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ActionButton))]
    class ActionButtonTests : VisualElementTests<ActionButton>
    {
        protected override string mainUssClassName => ActionButton.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", ctx => new ActionButton());
                yield return new Story("WithLabel", ctx => new ActionButton { label = "Test" });
                yield return new Story("WithIconAndLabel", ctx => new ActionButton { label = "Test", icon = "info", iconVariant = IconVariant.Regular });
                yield return new Story("IconOnly", ctx => new ActionButton { icon = "info" });
                yield return new Story("WithTrailingIcon", ctx => new ActionButton { label = "My Label", trailingIcon = "info", trailingIconVariant = IconVariant.Regular });
            }
        }

        protected override IEnumerable<string> uxmlTestCases
        {
            get
            {
                yield return @"<appui:ActionButton/>";
                yield return @"<appui:ActionButton label=""Test""/>";
                yield return @"<appui:ActionButton icon=""info""/>";
                yield return @"<appui:ActionButton label=""My Label"" trailing-icon=""info"" trailing-icon-variant=""Regular"" />";
                yield return @"<appui:ActionButton label=""Test"" icon=""info"" icon-variant=""Regular"" size=""M"" accent=""true"" selected=""false"" quiet=""true""/>";
            }
        }
    }
}
