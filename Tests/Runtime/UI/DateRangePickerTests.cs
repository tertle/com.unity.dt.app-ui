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
    [TestOf(typeof(DateRangePicker))]
    class DateRangePickerTests : VisualElementTests<DateRangePicker>
    {
        protected override string mainUssClassName => DateRangePicker.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", (ctx) => new DateRangePicker
                {
                    value = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2))
                });
                yield return new Story("DifferentMonths", (ctx) => new DateRangePicker
                {
                    value = new DateRange(new Date(2020, 1, 1), new Date(2020, 2, 2))
                });
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:DateRangePicker />",
            @"<appui:DateRangePicker value=""2020-01-01,2020-01-02"" />",
        };

        [Test]
        public void DatePicker_CanSetValue()
        {
            var datePicker = new DateRangePicker();
            datePicker.SetValueWithoutNotify(new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2)));
            Assert.AreEqual(2020, datePicker.currentYear);
            Assert.AreEqual(1, datePicker.currentMonth);
            Assert.AreEqual(2020, datePicker.value.start.year);
            Assert.AreEqual(1, datePicker.value.start.month);
            Assert.AreEqual(1, datePicker.value.start.day);
            Assert.AreEqual(2020, datePicker.value.end.year);
            Assert.AreEqual(1, datePicker.value.end.month);
            Assert.AreEqual(2, datePicker.value.end.day);
        }

        [UnityTest]
        public IEnumerator DatePicker_CanSetValueWithEvent()
        {
            m_TestUI.rootVisualElement.Clear();
            var panel = new Panel();
            var datePicker = new DateRangePicker();
            m_TestUI.rootVisualElement.Add(panel);
            panel.Add(datePicker);

            yield return null;

            var called = 0;
            datePicker.RegisterValueChangedCallback(evt =>
            {
                called++;
                Assert.AreEqual(new Date(2020, 1, 1), evt.newValue.start);
                Assert.AreEqual(new Date(2020, 1, 2), evt.newValue.end);
            });
            datePicker.value = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));

            yield return null;

            Assert.AreEqual(1, called);
            Assert.AreEqual(new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2)), datePicker.value);

            datePicker.value = new DateRange(new Date(2020, 1, 1), new Date(2020, 1, 2));

            yield return null;

            Assert.AreEqual(1, called);
        }
    }
}
