using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(SwipeView))]
    class SwipeViewTests : VisualElementTests<SwipeView>
    {
        protected override string mainUssClassName => SwipeView.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:SwipeView />",
            @"<appui:SwipeView skip-animation-threshold=""2"" style=""width: 200px; height: 400px;"" direction=""Horizontal"" animation-speed=""1"" wrap=""true"" visible-item-count=""2"" start-swipe-threshold=""1"" auto-play-duration=""-1"" swipeable=""true"" resistance=""10"">
                <appui:SwipeViewItem>
                    <appui:Button title=""Item 1"" />
                </appui:SwipeViewItem>
                <appui:SwipeViewItem>
                    <appui:Text text=""Item 2"" />
                </appui:SwipeViewItem>
                <appui:SwipeViewItem>
                    <appui:Text text=""Item 3"" />
                </appui:SwipeViewItem>
                <appui:SwipeViewItem>
                    <appui:Text text=""Item 4"" />
                </appui:SwipeViewItem>
                <appui:SwipeViewItem>
                    <appui:Text text=""Item 5"" />
                </appui:SwipeViewItem>
            </appui:SwipeView>"
        };
    }
}
