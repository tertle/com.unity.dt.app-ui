using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Avatar))]
    class AvatarTests : VisualElementTests<Avatar>
    {
        protected override string mainUssClassName => Avatar.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:Avatar />",
            @"<appui:Avatar size=""L"" variant=""Rounded"" background-color=""#FF0000"" outline-width=""3"" outline-color=""#00FF00"" />"
        };
    }
}
