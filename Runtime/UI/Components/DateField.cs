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
    /// Date Field UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class DateField : ExVisualElement, IInputElement<Date>, INotifyValueChanging<Date>, ISizeableElement
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
        /// The DateField input styling class.
        /// </summary>
        public static readonly string inputUssClassName = ussClassName + "__input";

        /// <summary>
        /// The DateField picker button styling class.
        /// </summary>
        public static readonly string pickerButtonUssClassName = ussClassName + "__picker-button";

        /// <summary>
        /// The DateField size styling class.
        /// </summary>
        public static readonly string sizeUssClassName = ussClassName + "--size-";

        readonly UnityEngine.UIElements.TextField m_InputElement;

        readonly VisualElement m_PickerButton;

        Date m_Value;

        Size m_Size;

        Date m_PreviousValue;

        DatePicker m_Picker;

        Func<Date, bool> m_ValidateValue;

        Popover m_Popover;

        string m_FormatString;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DateField()
        {
            AddToClassList(ussClassName);

            focusable = true;
            pickingMode = PickingMode.Position;
            tabIndex = 0;
            passMask = 0;

            m_InputElement = new UnityEngine.UIElements.TextField()
            {
                name = inputUssClassName,
                pickingMode = PickingMode.Ignore,
                isDelayed = true,
            };
            m_InputElement.AddToClassList(inputUssClassName);
            m_InputElement.RegisterValueChangedCallback(OnInputValueChanged);
            m_InputElement.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);

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

            hierarchy.Add(m_InputElement);
            hierarchy.Add(m_PickerButton);

            size = Size.M;
            SetValueWithoutNotify(Date.now);
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
            if (evt.target == m_InputElement.Q<TextElement>())
            {
                var newStartDate = ((DateTime)m_Value).AddDays(delta);
                value = new Date(newStartDate);
            }
        }

        void OnInputValueChanged(ChangeEvent<string> e)
        {
            var newValue = !string.IsNullOrEmpty(e.newValue) && DateTime.TryParse(e.newValue, out var date)
                ? new Date(date)
                : m_PreviousValue;
            value = newValue;
            SetFormattedString();
        }

        void OnClick()
        {
            m_Picker?.parent?.Remove(m_Picker);

            m_PreviousValue = value;
            m_Picker ??= new DatePicker
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
                using var evt = ChangeEvent<Date>.GetPooled(m_PreviousValue, m_Picker.value);
                SetValueWithoutNotify(m_Picker.value);
                evt.target = this;
                SendEvent(evt);
            }
            Focus();
        }

        void OnPickerValueChanged(ChangeEvent<Date> e)
        {
            if (e.newValue != value)
            {
                SetValueWithoutNotify(e.newValue);
                using var evt = ChangingEvent<Date>.GetPooled();
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
        public Func<Date, bool> validateValue
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
        public void SetValueWithoutNotify(Date newValue)
        {
            m_Value = newValue;
            SetFormattedString();
            if (validateValue != null) invalid = !validateValue(m_Value);
        }

        void SetFormattedString()
        {
            var formattedValue = string.IsNullOrEmpty(m_FormatString)
                ? ((DateTime)m_Value).ToString(CultureInfo.InvariantCulture)
                : ((DateTime)m_Value).ToString(m_FormatString, CultureInfo.InvariantCulture);
            m_InputElement.SetValueWithoutNotify(formattedValue);
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
        public Date value
        {
            get => m_Value;
            set
            {
                if (m_Value == value)
                    return;

                using var evt = ChangeEvent<Date>.GetPooled(m_Value, value);
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
        /// Class to instantiate a <see cref="DateField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<DateField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="DateField"/>.
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
                defaultValue = Date.now.ToString()
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

                var element = (DateField)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.invalid = m_Invalid.GetValueFromBag(bag, cc);
                element.formatString = m_FormatString.GetValueFromBag(bag, cc);
                string dateRaw = null;
                if (m_Value.TryGetValueFromBag(bag, cc, ref dateRaw) && !string.IsNullOrEmpty(dateRaw) && DateTime.TryParse(dateRaw, out var date))
                    element.value = new Date(date);
            }
        }
#endif
    }
}
