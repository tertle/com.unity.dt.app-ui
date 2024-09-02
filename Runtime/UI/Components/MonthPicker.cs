using System.Globalization;
using Unity.AppUI.Core;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    sealed class MonthPicker : BaseDatePickerPane
    {
        public static readonly string previousYearButtonUssClassName = ussClassName + "__previous-year-button";

        public static readonly string nextYearButtonUssClassName = ussClassName + "__next-year-button";

        public static readonly string yearLabelUssClassName = ussClassName + "__year-header-button";

        public static readonly string monthsContainerUssClassName = ussClassName + "__months-container";

        public static readonly string monthButtonUssClassName = ussClassName + "__month-button";

        readonly Text[] m_MonthsElementsPool = new Text[12];

        IconButton m_PreviousYearButton;

        Button m_YearHeaderButton;

        IconButton m_NextYearButton;

        VisualElement m_MonthsContainer;

        public MonthPicker(BaseDatePicker datePicker)
            : base(datePicker)
        { }

        protected override void ConstructLeftButtonGroupUI()
        {
            m_PreviousYearButton = new IconButton("caret-left", OnPreviousYearButtonClick)
            {
                name = previousYearButtonUssClassName,
                quiet = true,
            };
            m_PreviousYearButton.AddToClassList(previousYearButtonUssClassName);
            m_LeftButtonGroup.hierarchy.Add(m_PreviousYearButton);
        }

        protected override void ConstructHeaderContentUI()
        {
            m_YearHeaderButton = new Button(OnYearPressed)
            {
                name = yearLabelUssClassName,
                quiet = true,
            };
            m_YearHeaderButton.AddToClassList(yearLabelUssClassName);
            m_HeaderContentElement.hierarchy.Add(m_YearHeaderButton);
        }

        protected override void ConstructRightButtonGroupUI()
        {
            m_NextYearButton = new IconButton("caret-right", OnNextYearButtonClick)
            {
                name = nextYearButtonUssClassName,
                quiet = true,
            };
            m_NextYearButton.AddToClassList(nextYearButtonUssClassName);
            m_RightButtonGroup.hierarchy.Add(m_NextYearButton);
        }

        protected override void ConstructBodyUI()
        {
            m_MonthsContainer = new VisualElement
            {
                name = monthsContainerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_MonthsContainer.AddToClassList(monthsContainerUssClassName);
            m_BodyElement.hierarchy.Add(m_MonthsContainer);

            ConstructMonthsUI();
        }

        void ConstructMonthsUI()
        {
            for (var i = 0; i < 12; i++)
            {
                var monthButton = new Text
                {
                    name = monthButtonUssClassName,
                    pickingMode = PickingMode.Position,
                    focusable = true,
                    primary = true,
                    text = DayPicker.GetMonthText(i + 1, true, m_DatePicker.cultureInfo),
                    userData = i + 1
                };
                monthButton.AddToClassList(monthButtonUssClassName);
                m_MonthsContainer.hierarchy.Add(monthButton);
                m_MonthsElementsPool[i] = monthButton;
                var clickable = new Pressable(m_DatePicker.OnMonthSelected);
                monthButton.AddManipulator(clickable);
                var focusManipulator = new KeyboardFocusController();
                monthButton.AddManipulator(focusManipulator);
            }
        }

        internal override void RefreshUI()
        {
            var year = m_DatePicker.currentYear;
            m_YearHeaderButton.title = year.ToString();
        }

        void OnPreviousYearButtonClick()
        {
            m_DatePicker.GoToPreviousYear();
        }

        void OnNextYearButtonClick()
        {
            m_DatePicker.GoToNextYear();
        }

        void OnYearPressed()
        {
            m_DatePicker.displayMode = DatePicker.DisplayMode.Years;
        }
    }
}
