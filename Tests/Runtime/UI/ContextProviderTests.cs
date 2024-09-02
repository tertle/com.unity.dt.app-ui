using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ContextProvider))]
    class ContextProviderTests : VisualElementTests<ContextProvider>
    {
        protected override string mainUssClassName => ContextProvider.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield break;
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:ContextProvider />",
            @"<appui:ContextProvider theme=""dark"" scale=""medium"" dir=""Ltr"" lang=""en-US"" tooltip-delay-ms=""500"" enabled=""true"" preferred-tooltip-placement=""Bottom"" />",
        };
    }
}
