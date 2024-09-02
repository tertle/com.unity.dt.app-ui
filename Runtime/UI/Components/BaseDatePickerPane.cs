using System;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    abstract class BaseDatePickerPane : VisualElement
    {
        public const string ussClassName = "appui-date-picker-pane";

        public static readonly string headerUssClassName = ussClassName + "__header";

        public static readonly string leftButtonGroupUssClassName = ussClassName + "__button-group--left";

        public static readonly string headerContentUssClassName = ussClassName + "__header-content";

        public static readonly string rightButtonGroupUssClassName = ussClassName + "__button-group--right";

        public static readonly string bodyUssClassName = ussClassName + "__body";

        public static readonly string footerUssClassName = ussClassName + "__footer";

        protected readonly BaseDatePicker m_DatePicker;

        protected VisualElement m_HeaderElement;

        protected VisualElement m_LeftButtonGroup;

        protected VisualElement m_HeaderContentElement;

        protected VisualElement m_RightButtonGroup;

        protected VisualElement m_BodyElement;

        protected VisualElement m_FooterElement;

        protected BaseDatePickerPane(BaseDatePicker datePicker)
        {
            m_DatePicker = datePicker;
            ConstructUI();
        }

        void ConstructUI()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Ignore;

            m_HeaderElement = new VisualElement
            {
                name = headerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_HeaderElement.AddToClassList(headerUssClassName);
            hierarchy.Add(m_HeaderElement);

            m_BodyElement = new VisualElement
            {
                name = bodyUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_BodyElement.AddToClassList(bodyUssClassName);
            hierarchy.Add(m_BodyElement);

            m_FooterElement = new VisualElement
            {
                name = footerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_FooterElement.AddToClassList(footerUssClassName);
            hierarchy.Add(m_FooterElement);

            ConstructHeaderUI();
            ConstructBodyUI();
            ConstructFooterUI();
        }

        void ConstructHeaderUI()
        {
            m_LeftButtonGroup = new VisualElement
            {
                name = leftButtonGroupUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_LeftButtonGroup.AddToClassList(leftButtonGroupUssClassName);
            m_HeaderElement.hierarchy.Add(m_LeftButtonGroup);

            m_HeaderContentElement = new VisualElement
            {
                name = headerContentUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_HeaderContentElement.AddToClassList(headerContentUssClassName);
            m_HeaderElement.hierarchy.Add(m_HeaderContentElement);

            m_RightButtonGroup = new VisualElement
            {
                name = rightButtonGroupUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_RightButtonGroup.AddToClassList(rightButtonGroupUssClassName);
            m_HeaderElement.hierarchy.Add(m_RightButtonGroup);

            ConstructLeftButtonGroupUI();
            ConstructHeaderContentUI();
            ConstructRightButtonGroupUI();
        }

        protected virtual void ConstructLeftButtonGroupUI()
        {

        }

        protected virtual void ConstructHeaderContentUI()
        {

        }

        protected virtual void ConstructRightButtonGroupUI()
        {

        }

        protected virtual void ConstructBodyUI()
        {

        }

        protected virtual void ConstructFooterUI()
        {

        }

        internal virtual void RefreshUI()
        {

        }
    }
}
