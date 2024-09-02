using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(AvatarGroup))]
    class AvatarGroupTests : VisualElementTests<AvatarGroup>
    {
        protected override string mainUssClassName => AvatarGroup.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:AvatarGroup />",
            @"<appui:AvatarGroup size=""L"" spacing=""L"" variant=""Rounded"" total=""42"" max=""40"" />"
        };
    }
}
