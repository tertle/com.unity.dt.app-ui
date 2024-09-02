using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Checkbox))]
    class CheckboxTests : VisualElementTests<Checkbox>
    {
        protected override string mainUssClassName => Checkbox.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:Checkbox />",
            @"<appui:Checkbox label=""Checkbox"" />",
            @"<appui:Checkbox label=""Checkbox"" size=""M"" />",
            @"<appui:Checkbox label=""Checkbox"" size=""M"" value=""Checked"" emphasized=""true"" invalid=""false"" />",
        };

    }
}
