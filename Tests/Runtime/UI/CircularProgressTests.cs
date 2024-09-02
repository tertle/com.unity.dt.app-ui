using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(CircularProgress))]
    class CircularProgressTests : VisualElementTests<CircularProgress>
    {
        protected override string mainUssClassName => CircularProgress.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", ctx => new CircularProgress());
                yield return new Story("Determinate", ctx => new CircularProgress
                {
                    variant = Progress.Variant.Determinate,
                    bufferValue = 0.75f,
                    value = 0.5f,
                });
                yield return new Story("DeterminateNoRoundedCorners", ctx => new CircularProgress
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
            @"<appui:CircularProgress />",
            @"<appui:CircularProgress size=""M"" buffer-opacity=""0.5"" variant=""Determinate"" value=""0.5"" buffer-value=""0.75"" color-override=""#FF0000"" />",
            @"<appui:CircularProgress size=""M"" buffer-opacity=""0.5"" variant=""Determinate"" value=""0.5"" buffer-value=""0.75"" color-override=""#FF0000"" rounded-progress-corners=""false"" />",
        };
    }
}
