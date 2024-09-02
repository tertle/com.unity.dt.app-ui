using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(AppBar))]
    class AppBarTests : VisualElementTests<AppBar>
    {
        protected override string mainUssClassName => AppBar.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:AppBar/>",
            @"<appui:AppBar stretch=""true"" expanded-height=""60"" compact=""false"" elevation=""10"" show-back-button=""false"" show-drawer-button=""true"" />",
        };
    }
}
