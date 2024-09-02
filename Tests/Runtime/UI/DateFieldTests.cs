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
    [TestOf(typeof(DateField))]
    class DateFieldTests : VisualElementTests<DateField>
    {
        protected override string mainUssClassName => DateField.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", (ctx) => new DateField
                {
                    value = new Date(2020, 1, 1)
                });
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:DateField />",
            @"<appui:DateField value=""1998-07-12"" />",
        };
    }
}
