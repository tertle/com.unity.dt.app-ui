using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(DateRangeField))]
    class DateRangeFieldTests : VisualElementTests<DateRangeField>
    {
        protected override string mainUssClassName => DateRangeField.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", (ctx) => new DateRangeField
                {
                    value = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2))
                });
                yield return new Story("DifferentMonths", (ctx) => new DateRangeField
                {
                    value = new DateRange(new Date(2020, 1, 1), new Date(2020, 2, 2))
                });
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:DateRangeField />",
            @"<appui:DateRangeField value=""1998-07-12,1998-07-20"" />",
        };
    }
}
