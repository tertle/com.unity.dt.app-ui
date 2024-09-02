using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(LinearProgress))]
    class LinearProgressTests : VisualElementTests<LinearProgress>
    {
        protected override string mainUssClassName => LinearProgress.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", ctx => new LinearProgress());
                yield return new Story("Determinate", ctx => new LinearProgress
                {
                    variant = Progress.Variant.Determinate,
                    bufferValue = 0.75f,
                    value = 0.5f,
                });
                yield return new Story("DeterminateNoRoundedCorners", ctx => new LinearProgress
                {
                    variant = Progress.Variant.Determinate,
                    bufferValue = 0.75f,
                    value = 0.5f,
                    roundedProgressCorners = false,
                });
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:LinearProgress />",
            @"<appui:LinearProgress size=""M"" buffer-opacity=""0.5"" variant=""Determinate"" value=""0.5"" buffer-value=""0.75"" color-override=""#FF0000"" />",
            @"<appui:LinearProgress size=""M"" buffer-opacity=""0.5"" variant=""Determinate"" value=""0.5"" buffer-value=""0.75"" color-override=""#FF0000"" rounded-progress-corners=""false"" />",
        };
    }
}
