using Unity.AppUI.Core;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    sealed class YearPicker : BaseDatePickerPane
    {
        public static readonly string previousDecadeButtonUssClassName = ussClassName + "__previous-decade-button";

        public static readonly string nextDecadeButtonUssClassName = ussClassName + "__next-decade-button";

        public static readonly string yearLabelUssClassName = ussClassName + "__year-header-label";

        public static readonly string yearsContainerUssClassName = ussClassName + "__years-container";

        public static readonly string yearButtonUssClassName = ussClassName + "__year-button";

        readonly Text[] m_YearsElementsPool = new Text[10];

        IconButton m_PreviousDecadeButton;

        Text m_YearLabel;

        IconButton m_NextDecadeButton;

        VisualElement m_YearsContainer;

        public YearPicker(BaseDatePicker datePicker)
            : base(datePicker)
        {
        }

        protected override void ConstructLeftButtonGroupUI()
        {
            m_PreviousDecadeButton = new IconButton("caret-double-left", OnPreviousDecadeButtonClick)
            {
                name = previousDecadeButtonUssClassName,
                quiet = true,
            };
            m_PreviousDecadeButton.AddToClassList(previousDecadeButtonUssClassName);
            m_LeftButtonGroup.hierarchy.Add(m_PreviousDecadeButton);
        }

        protected override void ConstructHeaderContentUI()
        {
            m_YearLabel = new Text
            {
                name = yearLabelUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_YearLabel.AddToClassList(yearLabelUssClassName);
            m_HeaderContentElement.hierarchy.Add(m_YearLabel);
        }

        protected override void ConstructRightButtonGroupUI()
        {
            m_NextDecadeButton = new IconButton("caret-double-right", OnNextDecadeButtonClick)
            {
                name = nextDecadeButtonUssClassName,
                quiet = true,
            };
            m_NextDecadeButton.AddToClassList(nextDecadeButtonUssClassName);
            m_RightButtonGroup.hierarchy.Add(m_NextDecadeButton);
        }

        protected override void ConstructBodyUI()
        {
            m_YearsContainer = new VisualElement
            {
                name = yearsContainerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_YearsContainer.AddToClassList(yearsContainerUssClassName);
            m_BodyElement.hierarchy.Add(m_YearsContainer);

            ConstructYearsUI();
        }

        void ConstructYearsUI()
        {
            for (var i = 0; i < m_YearsElementsPool.Length; i++)
            {
                var yearElement = new Text
                {
                    name = yearButtonUssClassName,
                    focusable = true,
                    pickingMode = PickingMode.Position,
                    primary = true,
                };
                yearElement.AddToClassList(yearButtonUssClassName);
                m_YearsElementsPool[i] = yearElement;
                var clickable = new Pressable(m_DatePicker.OnYearSelected);
                var focusManipulator = new KeyboardFocusController();
                yearElement.AddManipulator(clickable);
                yearElement.AddManipulator(focusManipulator);
                m_YearsContainer.hierarchy.Add(yearElement);
            }
        }

        internal override void RefreshUI()
        {
            var currentYear = m_DatePicker.currentYear;
            var currentDecade = currentYear - currentYear % 10;
            m_YearLabel.text = $"{currentYear}";
            for (var i = 0; i < m_YearsElementsPool.Length; i++)
            {
                var yearElement = m_YearsElementsPool[i];
                var year = currentDecade + i;
                if (yearElement.userData is not int data || data != year)
                {
                    yearElement.text = $"{year}";
                    yearElement.userData = year;
                }
            }
        }

        void OnPreviousDecadeButtonClick()
        {
            m_DatePicker.GoTo(new Date(m_DatePicker.currentYear - 10, m_DatePicker.currentMonth, 1));
        }

        void OnNextDecadeButtonClick()
        {
            m_DatePicker.GoTo(new Date(m_DatePicker.currentYear + 10, m_DatePicker.currentMonth, 1));
        }
    }
}
