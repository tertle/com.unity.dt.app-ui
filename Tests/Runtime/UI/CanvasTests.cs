using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Canvas))]
    class CanvasTests : VisualElementTests<Canvas>
    {
        protected override string mainUssClassName => Canvas.ussClassName;

        protected override bool uxmlConstructable => true;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            "<appui:Canvas/>",
            @"<Unity.AppUI.UI.Canvas enabled=""true"" frame-container=""0,0,100,100"" scroll-speed=""2.1"" min-zoom=""0.11"" max-zoom=""99"" zoom-speed=""0.0751"" zoom-multiplier=""2.1"" pan-multiplier=""3.1"" scroll-direction=""Inverse"" zoom=""1.1"" frame-margin=""12.2"" use-space-bar=""false"" control-scheme=""Editor"" primary-manipulator=""Pan"" style=""width: 277px; height: 257px;"" />",
        };
    }
}
