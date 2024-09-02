using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Dialog))]
    class DialogTests : VisualElementTests<Dialog>
    {
        protected override string mainUssClassName => BaseDialog.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:Dialog />",
            @"<appui:Dialog title=""Title"" description=""Description"" size=""M"" dismissable=""true"" />",
        };
    }
}
