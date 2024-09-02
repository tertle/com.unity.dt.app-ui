using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(AlertDialog))]
    class AlertDialogTests : VisualElementTests<AlertDialog>
    {
        protected override string mainUssClassName => BaseDialog.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:AlertDialog/>",
            @"<appui:AlertDialog is-primary-action-disabled=""true"" is-secondary-action-disabled=""true"" variant=""Information"" dismissable=""false"" />",
        };
    }
}
