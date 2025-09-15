using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Base class for TouchSlider UI elements (<see cref="TouchSliderFloat"/>, <see cref="TouchSliderInt"/>).
    /// </summary>
    /// <typeparam name="TValue">A comparable value type.</typeparam>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class TouchSlider<TValue> : BaseSlider<TValue, TValue>
        where TValue : unmanaged, IComparable, IEquatable<TValue>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId labelProperty = nameof(label);

#endif

        /// <summary>
        /// The TouchSlider main styling class.
        /// </summary>
        public const string ussClassName = "appui-touchslider";

        /// <summary>
        /// The TouchSlider progress styling class.
        /// </summary>
        public const string progressUssClassName = ussClassName + "__progress";

        /// <summary>
        /// The TouchSlider label container styling class.
        /// </summary>
        public const string labelContainerUssClassName = ussClassName + "__label-container";

        /// <summary>
        /// The TouchSlider label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The TouchSlider value label container styling class.
        /// </summary>
        public const string valueContainerUssClassName = ussClassName + "__valuelabel-container";

        /// <summary>
        /// The TouchSlider value label styling class.
        /// </summary>
        public const string valueUssClassName = ussClassName + "__valuelabel";

        /// <summary>
        /// The TouchSlider size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The orientation variant USS class name.
        /// </summary>
        [EnumName("GetOrientationClassName", typeof(Direction))]
        public const string variantUssClassName = ussClassName + "--";

        readonly VisualElement m_ProgressElement;

        readonly UnityEngine.UIElements.TextField m_InputField;

        bool m_IsEditingTextField;

        readonly LocalizedTextElement m_LabelElement;

        Size m_Size;

        readonly LocalizedTextElement m_ValueLabelElement;

        readonly VisualElement m_LabelContainer;

        readonly VisualElement m_ValueLabelContainer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected TouchSlider()
        {
            AddToClassList(ussClassName);

            focusable = true;
            pickingMode = PickingMode.Position;
            passMask = Passes.Clear;
            tabIndex = 0;

            m_ProgressElement = new VisualElement
            {
                name = progressUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_ProgressElement.AddToClassList(progressUssClassName);
            hierarchy.Add(m_ProgressElement);

            m_LabelContainer = new VisualElement
            {
                name = labelContainerUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_LabelContainer.AddToClassList(labelContainerUssClassName);
            hierarchy.Add(m_LabelContainer);

            m_LabelElement = new LocalizedTextElement { name = labelUssClassName, pickingMode = PickingMode.Ignore };
            m_LabelElement.AddToClassList(labelUssClassName);
            m_LabelContainer.Add(m_LabelElement);

            m_ValueLabelContainer = new VisualElement
            {
                name = valueContainerUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_ValueLabelContainer.AddToClassList(valueContainerUssClassName);
            hierarchy.Add(m_ValueLabelContainer);

            m_ValueLabelElement = new LocalizedTextElement { name = valueUssClassName, pickingMode = PickingMode.Ignore };
            m_ValueLabelElement.AddToClassList(valueUssClassName);
            m_ValueLabelContainer.Add(m_ValueLabelElement);

            m_InputField = new UnityEngine.UIElements.TextField { name = valueUssClassName, pickingMode = PickingMode.Position };
            m_InputField.AddManipulator(new BlinkingCursor());
            m_InputField.AddToClassList(valueUssClassName);
            m_InputField.RegisterCallback<FocusEvent>(OnInputFocusedIn);
            m_InputField.RegisterCallback<FocusOutEvent>(OnInputFocusedOut);
            m_InputField.RuntimeContextMenu();
            m_InputField.RegisterValueChangedCallback(OnInputValueChanged);
            m_ValueLabelContainer.Add(m_InputField);

            // Manipulators
            m_DraggerManipulator = new Draggable(OnTrackClicked, OnTrackDragged, OnTrackUp, OnTrackDown)
            {
                dragDirection = Draggable.DragDirection.Horizontal
            };
            this.AddManipulator(m_DraggerManipulator);
            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));

            // Default values
            HideInputField();
            size = Size.M;

            // Event listeners
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        void OnInputValueChanged(ChangeEvent<string> evt)
        {
            evt.StopPropagation();
        }

        /// <summary>
        /// Specify the size of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Size size
        {
            get => m_Size;
            set
            {
                RemoveFromClassList(GetSizeUssClassName(m_Size));
                m_Size = value;
                AddToClassList(GetSizeUssClassName(m_Size));
            }
        }

        /// <summary>
        /// <para>Specify a unit for the value encapsulated in this slider.</para>
        /// <para>This unit will be displayed next to value into the slider.</para>
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string label
        {
            get => m_LabelElement.text;
            set => m_LabelElement.text = value;
        }

        /// <inheritdoc />
        public override VisualElement contentContainer => null;

        /// <summary>
        /// Set the value of the slider without notifying the value change.
        /// </summary>
        /// <param name="newValue"> The new value to set.</param>
        public override void SetValueWithoutNotify(TValue newValue)
        {
            if (m_IsEditingTextField)
                return;

            newValue = GetClampedValue(newValue);
            var strValue = ParseValueToString(newValue);

            m_Value = newValue;
            m_InputField.SetValueWithoutNotify(strValue);
            m_ValueLabelElement.text = strValue;
            if (validateValue != null)
                invalid = !validateValue(m_Value);
            RefreshUI();
        }

        void RefreshUI()
        {
            if (panel == null || !layout.IsValid())
                return;

            var norm = SliderNormalizeValue(m_Value, lowValue, highValue);
            var scale = 1f;
            var pos = 0f;
            switch (m_Orientation)
            {
                case Direction.Horizontal:
                    scale = layout.width * Mathf.Clamp01(norm);
                    pos = m_CurrentDirection == Dir.Ltr ? 0 : layout.width - scale;
                    m_ProgressElement.style.left = pos;
                    m_ProgressElement.style.top = StyleKeyword.Null;
                    m_ProgressElement.style.width = scale;
                    m_ProgressElement.style.height = StyleKeyword.Null;
                    break;
                case Direction.Vertical:
                    scale = layout.height * Mathf.Clamp01(norm);
                    pos = layout.height - scale;
                    m_ProgressElement.style.top = pos;
                    m_ProgressElement.style.left = StyleKeyword.Null;
                    m_ProgressElement.style.width = StyleKeyword.Null;
                    m_ProgressElement.style.height = scale;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Callback when the interactive part of the slider is clicked.
        /// </summary>
        protected override void OnTrackClicked()
        {
            if (!m_DraggerManipulator.hasMoved)
                ShowInputField();
        }

        void OnInputFocusedIn(FocusEvent evt)
        {
            m_IsEditingTextField = true;
            AddToClassList(Styles.focusedUssClassName);
        }

        void OnInputFocusedOut(FocusOutEvent evt)
        {
            m_IsEditingTextField = false;
            RemoveFromClassList(Styles.focusedUssClassName);
            HideInputField();

            var currentValueStr = ParseValueToString(value);
            if (m_InputField.value != currentValueStr && ParseStringToValue(m_InputField.value, out var newValue))
            {
                value = newValue;
                SetValueWithoutNotify(newValue);
            }
            else
            {
                m_InputField.SetValueWithoutNotify(currentValueStr);
            }
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            passMask = 0;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            passMask = Passes.Clear | Passes.Outline;
        }

        void ShowInputField()
        {
            m_ValueLabelElement.style.display = DisplayStyle.None;
            m_InputField.style.display = DisplayStyle.Flex;
            m_InputField.schedule.Execute(OnInputFieldShown);
        }

        void OnInputFieldShown()
        {
            m_InputField.SetValueWithoutNotify(ParseRawValueToString(value));
            m_InputField.Focus();
        }

        void HideInputField()
        {
            m_ValueLabelElement.style.display = DisplayStyle.Flex;
            m_InputField.style.display = DisplayStyle.None;
        }

        /// <inheritdoc />
        protected override void SetOrientation(Direction newValue)
        {
            RemoveFromClassList(GetOrientationClassName(m_Orientation));
            m_Orientation = newValue;
            AddToClassList(GetOrientationClassName(m_Orientation));
            if (m_DraggerManipulator != null)
                m_DraggerManipulator.dragDirection = m_Orientation == Direction.Horizontal
                    ? Draggable.DragDirection.Horizontal
                    : Draggable.DragDirection.Vertical;
            RefreshUI();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in orientationProperty);
#endif
        }

        /// <inheritdoc />
        protected override TValue Clamp(TValue v, TValue lowBound, TValue highBound)
        {
            var result = v;
            if (lowBound.CompareTo(v) > 0)
                result = lowBound;
            if (highBound.CompareTo(v) < 0)
                result = highBound;
            return result;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="TouchSlider{TValue}"/>.
        /// </summary>
        public new class UxmlTraits : BaseSlider<TValue, TValue>.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription { name = "label" };

            readonly UxmlStringAttributeDescription m_Format = new UxmlStringAttributeDescription { name = "format-string", defaultValue = null };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M
            };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var element = (TouchSlider<TValue>)ve;
                element.label = m_Label.GetValueFromBag(bag, cc);
                element.size = m_Size.GetValueFromBag(bag, cc);

                string formatStr = null;
                if (m_Format.TryGetValueFromBag(bag, cc, ref formatStr) && !string.IsNullOrEmpty(formatStr))
                    element.formatString = formatStr;


            }
        }

#endif
    }
}
