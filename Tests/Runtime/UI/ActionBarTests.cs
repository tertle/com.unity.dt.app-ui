using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ActionBar))]
    class ActionBarTests : VisualElementTests<ActionBar>
    {
        protected override string mainUssClassName => ActionBar.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            "<appui:ActionBar/>",
            @"<appui:ActionBar message=""Message"" />",
            @"<appui:ActionBar message=""Message"">
                <appui:ActionButton label=""Export"" icon=""info"" quiet=""true"" />
                <appui:ActionButton label=""Delete"" icon=""info"" quiet=""true"" />
              </appui:ActionBar>",
        };
    }
}
