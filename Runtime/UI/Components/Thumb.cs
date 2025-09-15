using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Thumb UI element.
    /// </summary>
    public partial class Thumb : VisualElement
    {
        /// <summary>
        /// USS class name for the thumb.
        /// </summary>
        public const string ussClassName = "appui-thumb";

        /// <summary>
        /// USS class name for the thumb content.
        /// </summary>
        public const string contentUssClassName = ussClassName + "__content";

        /// <summary>
        /// USS class name for the thumb fill content.
        /// </summary>
        public const string fillUssClassName = ussClassName + "__fill";

        /// <summary>
        /// USS class name for the thumb value label container.
        /// </summary>
        public const string valueLabelContainerUssClassName = ussClassName + "__value-label-container";

        /// <summary>
        /// USS class name for the thumb value label.
        /// </summary>
        public const string valueLabelUssClassName = ussClassName + "__value-label";

        /// <summary>
        /// USS class name for the thumb display value label.
        /// </summary>
        [EnumName("GetDisplayValueLabelUssClassName", typeof(ValueDisplayMode))]
        public const string displayValueLabelVariantUssClassName = ussClassName + "--display-value-label-";

        readonly ExVisualElement m_Content;

        readonly ExVisualElement m_ValueLabelContainer;

        readonly TextElement m_ValueLabel;

        readonly VisualElement m_Fill;

        ValueDisplayMode m_DisplayValueLabel;

        static readonly EventCallback<FocusInEvent> k_OnPointerFocusIn = OnPointerFocusIn;

        static readonly EventCallback<FocusOutEvent> k_OnPointerFocusOut = OnPointerFocusOut;

        static readonly EventCallback<FocusInEvent> k_OnKeyboardFocusIn = OnKeyboardFocusIn;

        /// <summary>
        /// Display mode for the value label.
        /// </summary>
        public ValueDisplayMode displayValueLabel
        {
            get => m_DisplayValueLabel;
            set
            {
                if (m_DisplayValueLabel == value)
                    return;
                RemoveFromClassList(GetDisplayValueLabelUssClassName(m_DisplayValueLabel));
                m_DisplayValueLabel = value;
                AddToClassList(GetDisplayValueLabelUssClassName(m_DisplayValueLabel));
                m_ValueLabelContainer.passMask = m_DisplayValueLabel == ValueDisplayMode.Off
                    ? 0
                    : ExVisualElement.Passes.Clear | ExVisualElement.Passes.OutsetShadows;
            }
        }

        /// <summary>
        /// Text of the value label.
        /// </summary>
        public string text
        {
            get => m_ValueLabel.text;
            set => m_ValueLabel.text = value;
        }

        /// <inheritdoc />
        public override VisualElement contentContainer => null;

        /// <summary>
        /// A fill color for the thumb.
        /// </summary>
        public Color fill
        {
            get => m_Fill.resolvedStyle.backgroundColor;
            set => m_Fill.style.backgroundColor = value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Thumb()
        {
            AddToClassList(ussClassName);

            focusable = true;
            tabIndex = 0;
            pickingMode = PickingMode.Ignore;
            this.EnableDynamicTransform(true);
            m_Content = new ExVisualElement
            {
                name = contentUssClassName,
                pickingMode = PickingMode.Position,
                passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.OutsetShadows,
            };
            m_Content.AddToClassList(MemoryUtils.Concatenate(Styles.elevationUssClassName, "3"));
            m_Content.AddToClassList(contentUssClassName);
            hierarchy.Add(m_Content);

            m_Fill = new VisualElement
            {
                pickingMode = PickingMode.Ignore,
            };
            m_Fill.usageHints |= UsageHints.DynamicColor;
            m_Fill.AddToClassList(fillUssClassName);
            m_Content.Add(m_Fill);

            m_ValueLabelContainer = new ExVisualElement
            {
                name = valueLabelContainerUssClassName,
                pickingMode = PickingMode.Ignore,
                passMask = 0
            };
            m_ValueLabelContainer.AddToClassList(valueLabelContainerUssClassName);
            m_ValueLabelContainer.AddToClassList(MemoryUtils.Concatenate(Styles.elevationUssClassName, "6"));
            hierarchy.Add(m_ValueLabelContainer);

            m_ValueLabel = new TextElement
            {
                name = valueLabelUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_ValueLabel.AddToClassList(valueLabelUssClassName);
            m_ValueLabelContainer.Add(m_ValueLabel);

            this.AddManipulator(new KeyboardFocusController(k_OnKeyboardFocusIn, k_OnPointerFocusIn, k_OnPointerFocusOut));
        }

        static void OnPointerFocusOut(FocusOutEvent evt)
        {
            ((Thumb)evt.target).m_Content.passMask &= ~ExVisualElement.Passes.Outline;
        }

        static void OnPointerFocusIn(FocusInEvent evt)
        {
            ((Thumb)evt.target).m_Content.passMask &= ~ExVisualElement.Passes.Outline;
        }

        static void OnKeyboardFocusIn(FocusInEvent evt)
        {
            ((Thumb)evt.target).m_Content.passMask |= ExVisualElement.Passes.Outline;
        }

        /// <summary>
        /// Ensures the thumb is focused.
        /// </summary>
        public void EnsureKeyboardFocus()
        {
            AddToClassList(Styles.keyboardFocusUssClassName);
            using var evt = FocusInEvent.GetPooled();
            evt.target = this;
            OnKeyboardFocusIn(evt);
        }
    }
}
