using System;
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
    /// DateRange Field UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class DateRangeField : ExVisualElement, IInputElement<DateRange>, INotifyValueChanging<DateRange>, ISizeableElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

        internal static readonly BindingId formatStringProperty = nameof(formatString);

#endif

        /// <summary>
        /// The DateField main styling class.
        /// </summary>
        public static readonly string ussClassName = "appui-date-field";

        /// <summary>
        /// The DateField input container styling class.
        /// </summary>
        public static readonly string inputContainerUssClassName = ussClassName + "__input-container";

        /// <summary>
        /// The DateField input styling class.
        /// </summary>
        public static readonly string inputUssClassName = ussClassName + "__input";

        /// <summary>
        /// The DateField separator styling class.
        /// </summary>
        public static readonly string separatorUssClassName = ussClassName + "__separator";

        /// <summary>
        /// The DateField picker button styling class.
        /// </summary>
        public static readonly string pickerButtonUssClassName = ussClassName + "__picker-button";

        /// <summary>
        /// The DateField size styling class.
        /// </summary>
        public static readonly string sizeUssClassName = ussClassName + "--size-";

        readonly UnityEngine.UIElements.TextField m_StartInputElement;

        readonly UnityEngine.UIElements.TextField m_EndInputElement;

        readonly VisualElement m_PickerButton;

        DateRange m_Value;

        Size m_Size;

        DateRange m_PreviousValue;

        DateRangePicker m_Picker;

        Func<DateRange, bool> m_ValidateValue;

        Popover m_Popover;

        string m_FormatString;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DateRangeField()
        {
            AddToClassList(ussClassName);

            focusable = true;
            pickingMode = PickingMode.Position;
            tabIndex = 0;
            passMask = 0;

            var inputContainer = new VisualElement
            {
                name = inputContainerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            inputContainer.AddToClassList(inputContainerUssClassName);

            m_StartInputElement = new UnityEngine.UIElements.TextField()
            {
                name = inputUssClassName + "__start",
                pickingMode = PickingMode.Ignore,
                isDelayed = true
            };
            m_StartInputElement.AddToClassList(inputUssClassName);
            m_StartInputElement.RegisterValueChangedCallback(OnInputValueChanged);
            m_StartInputElement.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);

            var separator = new Text
            {
                text = " - ",
                name = separatorUssClassName,
                pickingMode = PickingMode.Ignore
            };
            separator.AddToClassList(separatorUssClassName);

            m_EndInputElement = new UnityEngine.UIElements.TextField()
            {
                name = inputUssClassName + "__end",
                pickingMode = PickingMode.Ignore,
                isDelayed = true
            };
            m_EndInputElement.AddToClassList(inputUssClassName);
            m_EndInputElement.RegisterValueChangedCallback(OnInputValueChanged);
            m_EndInputElement.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);

            m_PickerButton = new VisualElement()
            {
                focusable = true,
                pickingMode = PickingMode.Position,
                name = pickerButtonUssClassName,
            };
            m_PickerButton.AddToClassList(pickerButtonUssClassName);
            m_PickerButton.AddManipulator(new Pressable(OnClick));
            var icon = new Icon { iconName = "calendar", pickingMode = PickingMode.Ignore };
            m_PickerButton.Add(icon);

            inputContainer.Add(m_StartInputElement);
            inputContainer.Add(separator);
            inputContainer.Add(m_EndInputElement);
            hierarchy.Add(inputContainer);
            hierarchy.Add(m_PickerButton);

            size = Size.M;
            SetValueWithoutNotify(new DateRange(Date.now, new Date(DateTime.Now.AddDays(1))));
            formatString = "yyyy-MM-dd";
            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));
        }

        void OnInputKeyDown(KeyDownEvent evt)
        {
            var delta = evt.keyCode switch
            {
                KeyCode.UpArrow => 1,
                KeyCode.DownArrow => -1,
                _ => 0
            };

            if (delta == 0)
                return;

            evt.StopPropagation();
            if (evt.target == m_StartInputElement.Q<TextElement>())
            {
                var newStartDate = ((DateTime)m_Value.start).AddDays(delta);
                var newEndDate = newStartDate > m_Value.end ? newStartDate : m_Value.end;
                value = new DateRange(new Date(newStartDate), new Date(newEndDate));
            }
            else if (evt.target == m_EndInputElement.Q<TextElement>())
            {
                var newEndDate = ((DateTime)m_Value.end).AddDays(delta);
                var newStartDate = newEndDate < m_Value.start ? newEndDate : m_Value.start;
                value = new DateRange(new Date(newStartDate), new Date(newEndDate));
            }
        }

        void OnInputValueChanged(ChangeEvent<string> e)
        {
            if (e.target == m_StartInputElement)
            {
                var newStartDate = !string.IsNullOrEmpty(e.newValue) && DateTime.TryParse(e.newValue, out var date)
                    ? new Date(date)
                    : m_PreviousValue.start;
                var endDate = newStartDate > m_PreviousValue.end ? ((DateTime)newStartDate).AddDays(1) : m_PreviousValue.end;
                value = new DateRange(newStartDate, new Date(endDate));
            }
            else if (e.target == m_EndInputElement)
            {
                var newEndDate = !string.IsNullOrEmpty(e.newValue) && DateTime.TryParse(e.newValue, out var date)
                    ? new Date(date)
                    : m_PreviousValue.end;
                var startDate = newEndDate < m_PreviousValue.start ? ((DateTime)newEndDate).AddDays(-1) : m_PreviousValue.start;
                value = new DateRange(new Date(startDate), newEndDate);
            }
            SetFormattedString();
        }

        void OnClick()
        {
            m_Picker?.parent?.Remove(m_Picker);

            m_PreviousValue = value;
            m_Picker ??= new DateRangePicker
            {
                //todo add settings
            };
            m_Picker.SetValueWithoutNotify(m_PreviousValue);
            m_Picker.RegisterValueChangedCallback(OnPickerValueChanged);
            if (m_Popover != null)
            {
                m_Popover.Dismiss(DismissType.Consecutive);
                m_Popover.dismissed -= OnPopoverDismissed;
            }
            m_Popover = Popover.Build(this, m_Picker);
            m_Popover.dismissed += OnPopoverDismissed;
            m_Popover.Show();
            AddToClassList(Styles.focusedUssClassName);
        }

        void OnPopoverDismissed(Popover popover, DismissType reason)
        {
            popover.dismissed -= OnPopoverDismissed;
            if (popover == m_Popover)
                m_Popover = null;
            RemoveFromClassList(Styles.focusedUssClassName);
            m_Picker.UnregisterValueChangedCallback(OnPickerValueChanged);
            if (m_PreviousValue != m_Picker.value)
            {
                using var evt = ChangeEvent<DateRange>.GetPooled(m_PreviousValue, m_Picker.value);
                SetValueWithoutNotify(m_Picker.value);
                evt.target = this;
                SendEvent(evt);
            }
            Focus();
        }

        void OnPickerValueChanged(ChangeEvent<DateRange> e)
        {
            if (e.newValue != value)
            {
                SetValueWithoutNotify(e.newValue);
                using var evt = ChangingEvent<DateRange>.GetPooled();
                evt.previousValue = m_PreviousValue;
                evt.newValue = e.newValue;
                evt.target = this;
                SendEvent(evt);
            }
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            m_PreviousValue = value;
            passMask = 0;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            m_PreviousValue = value;
            passMask = Passes.Clear | Passes.Outline;
        }

        /// <summary>
        /// The content container of this DateField. This is null for DateField.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// The DateField size.
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
                var changed = m_Size != value;
                RemoveFromClassList(sizeUssClassName + m_Size.ToString().ToLower());
                m_Size = value;
                AddToClassList(sizeUssClassName + m_Size.ToString().ToLower());

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

        /// <summary>
        /// The DateField invalid state.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool invalid
        {
            get => ClassListContains(Styles.invalidUssClassName);
            set
            {
                var changed = ClassListContains(Styles.invalidUssClassName) != value;
                EnableInClassList(Styles.invalidUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in invalidProperty);
#endif
            }
        }

        /// <summary>
        /// The DateField validation function.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<DateRange, bool> validateValue
        {
            get => m_ValidateValue;
            set
            {
                var changed = m_ValidateValue != value;
                m_ValidateValue = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        /// <summary>
        /// Sets the DateField value without notifying the DateField.
        /// </summary>
        /// <param name="newValue"> The new DateField value. </param>
        public void SetValueWithoutNotify(DateRange newValue)
        {
            m_Value = newValue;
            SetFormattedString();
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        void SetFormattedString()
        {
            var formattedStartValue = string.IsNullOrEmpty(m_FormatString)
                ? ((DateTime)m_Value.start).ToString(CultureInfo.InvariantCulture)
                : ((DateTime)m_Value.start).ToString(m_FormatString, CultureInfo.InvariantCulture);
            m_StartInputElement.SetValueWithoutNotify(formattedStartValue);

            var formattedEndValue = string.IsNullOrEmpty(m_FormatString)
                ? ((DateTime)m_Value.end).ToString(CultureInfo.InvariantCulture)
                : ((DateTime)m_Value.end).ToString(m_FormatString, CultureInfo.InvariantCulture);
            m_EndInputElement.SetValueWithoutNotify(formattedEndValue);
        }

        /// <summary>
        /// The DateField value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public DateRange value
        {
            get => m_Value;
            set
            {
                if (m_Value == value)
                    return;

                using var evt = ChangeEvent<DateRange>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        /// <summary>
        /// The DateField string formatting.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string formatString
        {
            get => m_FormatString;
            set
            {
                var changed = m_FormatString != value;
                m_FormatString = value;
                SetFormattedString();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in formatStringProperty);
#endif
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class to instantiate a <see cref="DateRangeField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<DateRangeField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="DateRangeField"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_Invalid = new UxmlBoolAttributeDescription
            {
                name = "invalid",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlStringAttributeDescription m_FormatString = new UxmlStringAttributeDescription
            {
                name = "format-string",
                defaultValue = "yyyy-MM-dd"
            };

            readonly UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription
            {
                name = "value",
                defaultValue = new DateRange(Date.now, new Date(DateTime.Now.AddDays(1))).ToString()
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

                var element = (DateRangeField)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.invalid = m_Invalid.GetValueFromBag(bag, cc);
                element.formatString = m_FormatString.GetValueFromBag(bag, cc);
                string dateRangeRaw = null;
                if (m_Value.TryGetValueFromBag(bag, cc, ref dateRangeRaw) && !string.IsNullOrEmpty(dateRangeRaw) && DateRange.TryParse(dateRangeRaw, out var dateRange))
                    element.value = dateRange;
            }
        }
#endif
    }
}
