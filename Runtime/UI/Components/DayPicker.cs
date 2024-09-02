using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.AppUI.Core;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    sealed class DayPicker : BaseDatePickerPane
    {
        public static readonly string previousYearButtonUssClassName = ussClassName + "__previous-year-button";

        public static readonly string previousMonthButtonUssClassName = ussClassName + "__previous-month-button";

        public static readonly string monthButtonUssClassName = ussClassName + "__month-header-button";

        public static readonly string yearButtonUssClassName = ussClassName + "__year-header-button";

        public static readonly string nextMonthButtonUssClassName = ussClassName + "__next-month-button";

        public static readonly string nextYearButtonUssClassName = ussClassName + "__next-year-button";

        public static readonly string weekDaysContainerUssClassName = ussClassName + "__week-days-container";

        public static readonly string dayOfWeekLabelUssClassName = ussClassName + "__day-of-week-label";

        public static readonly string daysContainerUssClassName = ussClassName + "__days-container";

        public static readonly string dayButtonUssClassName = ussClassName + "__day-button";

        public static readonly string extraDayUssClassName = dayButtonUssClassName + "--extra";

        public static readonly string startDateUssClassName = dayButtonUssClassName + "--start";

        public static readonly string endDateUssClassName = dayButtonUssClassName + "--end";

        public static readonly string inRangeUssClassName = dayButtonUssClassName + "--in-range";

        static readonly Dictionary<DayOfWeek, DayOfWeek[]> k_WeekDaysSequences = new Dictionary<DayOfWeek, DayOfWeek[]>
        {
            { DayOfWeek.Sunday, new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday } },
            { DayOfWeek.Monday, new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday } },
            { DayOfWeek.Tuesday, new[] { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday } },
            { DayOfWeek.Wednesday, new[] { DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday } },
            { DayOfWeek.Thursday, new[] { DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday } },
            { DayOfWeek.Friday, new[] { DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday } },
            { DayOfWeek.Saturday, new[] { DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday } }
        };

        readonly Text[] m_DaysElementsPool = new Text[42]; // maximum of 6 weeks visible

        readonly Text[] m_WeekDaysElementsPool = new Text[7];

        IconButton m_PreviousYearButton;

        IconButton m_PreviousMonthButton;

        Button m_MonthButton;

        Button m_YearButton;

        IconButton m_NextMonthButton;

        IconButton m_NextYearButton;

        VisualElement m_WeekDaysContainer;

        VisualElement m_DaysContainer;

        public DayPicker(BaseDatePicker datePicker)
            : base(datePicker)
        { }

        protected override void ConstructLeftButtonGroupUI()
        {
            m_PreviousYearButton = new IconButton("caret-double-left", m_DatePicker.GoToPreviousYear)
            {
                name = previousYearButtonUssClassName,
                quiet = true,
            };
            m_PreviousYearButton.AddToClassList(previousYearButtonUssClassName);
            m_LeftButtonGroup.hierarchy.Add(m_PreviousYearButton);

            m_PreviousMonthButton = new IconButton("caret-left", m_DatePicker.GoToPreviousMonth)
            {
                name = previousMonthButtonUssClassName,
                quiet = true,
            };
            m_PreviousMonthButton.AddToClassList(previousMonthButtonUssClassName);
            m_LeftButtonGroup.hierarchy.Add(m_PreviousMonthButton);
        }

        protected override void ConstructHeaderContentUI()
        {
            m_MonthButton = new Button(OnMonthButtonPressed)
            {
                name = monthButtonUssClassName,
                quiet = true,
            };
            m_MonthButton.AddToClassList(monthButtonUssClassName);
            m_HeaderContentElement.hierarchy.Add(m_MonthButton);

            m_YearButton = new Button(OnYearButtonPressed)
            {
                name = yearButtonUssClassName,
                quiet = true,
            };
            m_YearButton.AddToClassList(yearButtonUssClassName);
            m_HeaderContentElement.hierarchy.Add(m_YearButton);
        }

        protected override void ConstructRightButtonGroupUI()
        {
            m_NextMonthButton = new IconButton("caret-right", m_DatePicker.GoToNextMonth)
            {
                name = nextMonthButtonUssClassName,
                quiet = true,
            };
            m_NextMonthButton.AddToClassList(nextMonthButtonUssClassName);
            m_RightButtonGroup.hierarchy.Add(m_NextMonthButton);

            m_NextYearButton = new IconButton("caret-double-right", m_DatePicker.GoToNextYear)
            {
                name = nextYearButtonUssClassName,
                quiet = true,
            };
            m_NextYearButton.AddToClassList(nextYearButtonUssClassName);
            m_RightButtonGroup.hierarchy.Add(m_NextYearButton);
        }

        protected override void ConstructBodyUI()
        {
            m_WeekDaysContainer = new VisualElement
            {
                name = weekDaysContainerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_WeekDaysContainer.AddToClassList(weekDaysContainerUssClassName);
            m_BodyElement.hierarchy.Add(m_WeekDaysContainer);

            m_DaysContainer = new VisualElement
            {
                name = daysContainerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_DaysContainer.AddToClassList(daysContainerUssClassName);
            m_BodyElement.hierarchy.Add(m_DaysContainer);

            ConstructWeekDaysContainerUI();
            ConstructDaysContainerUI();
        }

        void ConstructWeekDaysContainerUI()
        {
            for (var i = 0; i < 7; i++)
            {
                var dow = new Text
                {
                    name = dayOfWeekLabelUssClassName,
                    pickingMode = PickingMode.Ignore,
                    primary = false
                };
                dow.AddToClassList(dayOfWeekLabelUssClassName);
                m_WeekDaysElementsPool[i] = dow;
                m_WeekDaysContainer.hierarchy.Add(dow);
            }
        }

        void ConstructDaysContainerUI()
        {
            for (var i = 0; i < m_DaysElementsPool.Length; i++)
            {
                var day = new Text
                {
                    name = dayButtonUssClassName,
                    pickingMode = PickingMode.Position,
                    focusable = true,
                    primary = true,
                };
                day.AddToClassList(dayButtonUssClassName);
                m_DaysElementsPool[i] = day;
                var clickable = new Pressable(m_DatePicker.OnDaySelected);
                var focusManipulator = new KeyboardFocusController();
                day.AddManipulator(clickable);
                day.AddManipulator(focusManipulator);
                m_DaysContainer.hierarchy.Add(day);
            }
        }

        protected override void ConstructFooterUI()
        {

        }

        void OnMonthButtonPressed()
        {
            m_DatePicker.displayMode = DatePicker.DisplayMode.Months;
        }

        void OnYearButtonPressed()
        {
            m_DatePicker.displayMode = DatePicker.DisplayMode.Years;
        }

        internal override void RefreshUI()
        {
            var weekDaysSequence = k_WeekDaysSequences[m_DatePicker.firstDayOfWeek];
            if (m_DatePicker.currentYear < DateTime.UnixEpoch.Year || m_DatePicker.currentMonth is < 1 or > 12)
                return;
            // find the first day of the month
            var firstDayOfMonth = new DateTime(m_DatePicker.currentYear, m_DatePicker.currentMonth, 1);
            var start = Array.IndexOf(weekDaysSequence, firstDayOfMonth.DayOfWeek);
            var currentMonthDaysCount = DateTime.DaysInMonth(m_DatePicker.currentYear, m_DatePicker.currentMonth);
            // set previous days
            var previousMonth = firstDayOfMonth.AddMonths(-1);
            var previousMonthDaysCount = DateTime.DaysInMonth(firstDayOfMonth.Year, previousMonth.Month);
            for (var i = 0; i < start; i++)
            {
                var dayNb = previousMonthDaysCount - start + i + 1;
                m_DaysElementsPool[i].userData = new Date(previousMonth.Year, previousMonth.Month, dayNb);
                m_DaysElementsPool[i].AddToClassList(extraDayUssClassName);
            }
            // bind days starting from the first day of the month
            for (var i = start; i < currentMonthDaysCount + start; i++)
            {
                var dayNb = i - start + 1;
                m_DaysElementsPool[i].userData = new Date(m_DatePicker.currentYear, m_DatePicker.currentMonth, dayNb);
                m_DaysElementsPool[i].RemoveFromClassList(extraDayUssClassName);
            }
            // set next days
            var nextMonth = firstDayOfMonth.AddMonths(1);
            for (var i = currentMonthDaysCount + start; i < m_DaysElementsPool.Length; i++)
            {
                var dayNb = i - start - currentMonthDaysCount + 1;
                m_DaysElementsPool[i].userData = new Date(nextMonth.Year, nextMonth.Month, dayNb);
                m_DaysElementsPool[i].AddToClassList(extraDayUssClassName);
            }
            foreach (var day in m_DaysElementsPool)
            {
                var date = (Date)day.userData;
                day.text = date.day.ToString();

                var isSelectedDate = m_DatePicker.IsSelectedDate(date);
                var isStartDate = m_DatePicker.IsStartDate(date);
                var isEndDate = m_DatePicker.IsEndDate(date);
                var isInBetween = m_DatePicker.IsInRange(date);
                day.EnableInClassList(Styles.selectedUssClassName, isSelectedDate || isStartDate || isEndDate);
                day.EnableInClassList(startDateUssClassName, isStartDate);
                day.EnableInClassList(endDateUssClassName, isEndDate);
                day.EnableInClassList(inRangeUssClassName, isInBetween);
            }
            // rebind week days labels
            for (var i = 0; i < m_WeekDaysElementsPool.Length; i++)
            {
                var dow = m_WeekDaysElementsPool[i];
                dow.text = GetWeekDayText(weekDaysSequence[i], true, m_DatePicker.cultureInfo);
                dow.userData = weekDaysSequence[i];
            }
            m_YearButton.title = m_DatePicker.currentYear.ToString();
            m_YearButton.userData = m_DatePicker.currentYear;
            m_MonthButton.title = GetMonthText(m_DatePicker.currentMonth, false, m_DatePicker.cultureInfo);
            m_MonthButton.userData = m_DatePicker.currentMonth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetMonthText(int monthNumber, bool shortVersion = true, CultureInfo cultureInfo = null)
        {
            var dateTimeFormatInfo = cultureInfo?.DateTimeFormat ?? CultureInfo.InvariantCulture.DateTimeFormat;
            return shortVersion
                ? dateTimeFormatInfo.GetAbbreviatedMonthName(monthNumber)
                : dateTimeFormatInfo.GetMonthName(monthNumber);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string GetWeekDayText(DayOfWeek dow, bool shortVersion = true, CultureInfo cultureInfo = null)
        {
            var dateTimeFormatInfo = cultureInfo?.DateTimeFormat ?? CultureInfo.InvariantCulture.DateTimeFormat;
            return shortVersion
                ? dateTimeFormatInfo.GetAbbreviatedDayName(dow)
                : dateTimeFormatInfo.GetDayName(dow);
        }
    }
}
