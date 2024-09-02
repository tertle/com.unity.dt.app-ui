using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Breadcrumbs))]
    class BreadcrumbsTests : VisualElementTests<Breadcrumbs>
    {
        protected override string mainUssClassName => Breadcrumbs.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:Breadcrumbs>
                <appui:BreadcrumbItem text=""Home"" />
                <appui:BreadcrumbSeparator />
                <appui:BreadcrumbItem text=""Library"" />
                <appui:BreadcrumbSeparator />
                <appui:BreadcrumbItem text=""Assets"" />
                <appui:BreadcrumbSeparator />
                <appui:BreadcrumbItem text=""MyAsset"" is-current=""true"" />
            </appui:Breadcrumbs>"
        };
    }

    [TestFixture]
    [TestOf(typeof(BreadcrumbItem))]
    class BreadcrumbItemTests : VisualElementTests<BreadcrumbItem>
    {
        protected override string mainUssClassName => BreadcrumbItem.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:BreadcrumbItem text=""Home"" />",
            @"<appui:BreadcrumbItem text=""Home"" is-current=""true"" />"
        };
    }

    [TestFixture]
    [TestOf(typeof(BreadcrumbSeparator))]
    class BreadcrumbSeparatorTests : VisualElementTests<BreadcrumbSeparator>
    {
        protected override string mainUssClassName => BreadcrumbSeparator.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:BreadcrumbSeparator />",
            @"<appui:BreadcrumbSeparator text="">"" />"
        };
    }
}
