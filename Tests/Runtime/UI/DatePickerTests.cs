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
    [TestOf(typeof(DatePicker))]
    class DatePickerTests : VisualElementTests<DatePicker>
    {
        protected override string mainUssClassName => DatePicker.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", (ctx) => new DatePicker
                {
                    value = new Date(2020, 1, 1)
                });
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:DatePicker />",
            @"<appui:DatePicker value=""1998-07-12"" />",
        };

        [Test]
        public void DatePicker_CanGoTo()
        {
            var datePicker = new DatePicker();
            datePicker.GoTo(new Date(2020, 1, 1));
            Assert.AreEqual(2020, datePicker.currentYear);
            Assert.AreEqual(1, datePicker.currentMonth);
        }

        [Test]
        public void DatePicker_CanGoToPreviousYear()
        {
            var datePicker = new DatePicker();
            datePicker.GoTo(new Date(2020, 1, 1));
            datePicker.GoToPreviousYear();
            Assert.AreEqual(2019, datePicker.currentYear);
            Assert.AreEqual(1, datePicker.currentMonth);
        }

        [Test]
        public void DatePicker_CanGoToNextYear()
        {
            var datePicker = new DatePicker();
            datePicker.GoTo(new Date(2020, 1, 1));
            datePicker.GoToNextYear();
            Assert.AreEqual(2021, datePicker.currentYear);
            Assert.AreEqual(1, datePicker.currentMonth);
        }

        [Test]
        public void DatePicker_CanGoToYear()
        {
            var datePicker = new DatePicker();
            datePicker.GoTo(new Date(2020, 2, 29));
            datePicker.GoToYear(2019);
            Assert.AreEqual(2019, datePicker.currentYear);
            Assert.AreEqual(2, datePicker.currentMonth);
        }

        [Test]
        public void DatePicker_CanGoToPreviousMonth()
        {
            var datePicker = new DatePicker();
            datePicker.GoTo(new Date(2020, 1, 1));
            datePicker.GoToPreviousMonth();
            Assert.AreEqual(2019, datePicker.currentYear);
            Assert.AreEqual(12, datePicker.currentMonth);
        }

        [Test]
        public void DatePicker_CanGoToNextMonth()
        {
            var datePicker = new DatePicker();
            datePicker.GoTo(new Date(2020, 1, 1));
            datePicker.GoToNextMonth();
            Assert.AreEqual(2020, datePicker.currentYear);
            Assert.AreEqual(2, datePicker.currentMonth);
        }

        [Test]
        public void DatePicker_CanGoToMonth()
        {
            var datePicker = new DatePicker();
            datePicker.GoTo(new Date(2020, 1, 31));
            datePicker.GoToMonth(2);
            Assert.AreEqual(2020, datePicker.currentYear);
            Assert.AreEqual(2, datePicker.currentMonth);

            Assert.Throws<System.ArgumentOutOfRangeException>(() => datePicker.GoToMonth(13));
        }

        [Test]
        public void DatePicker_CanSetValue()
        {
            var datePicker = new DatePicker();
            datePicker.SetValueWithoutNotify(new Date(2020, 1, 1));
            Assert.AreEqual(2020, datePicker.currentYear);
            Assert.AreEqual(1, datePicker.currentMonth);
        }

        [UnityTest]
        public IEnumerator DatePicker_CanSetValueWithEvent()
        {
            m_TestUI.rootVisualElement.Clear();
            var panel = new Panel();
            var datePicker = new DatePicker();
            m_TestUI.rootVisualElement.Add(panel);
            panel.Add(datePicker);

            yield return null;

            var called = 0;
            datePicker.RegisterValueChangedCallback(evt =>
            {
                called++;
                Assert.AreEqual(new Date(2020, 1, 1), evt.newValue);
            });
            datePicker.value = new Date(2020, 1, 1);

            yield return null;

            Assert.AreEqual(1, called);
            Assert.AreEqual(new Date(2020, 1, 1), datePicker.value);

            datePicker.value = new Date(2020, 1, 1);

            yield return null;

            Assert.AreEqual(1, called);
        }
    }
}
