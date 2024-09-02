using System;
using Unity.AppUI.Bridge;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Numerical Field UI element.
    /// </summary>
    /// <typeparam name="TValueType">The type of the numerical value.</typeparam>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class NumericalField<TValueType> : ExVisualElement, IInputElement<TValueType>, ISizeableElement, INotifyValueChanging<TValueType>
        where TValueType : struct, IComparable, IComparable<TValueType>, IFormattable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = new BindingId(nameof(value));

        internal static readonly BindingId formatStringProperty = new BindingId(nameof(formatString));

        internal static readonly BindingId unitProperty = new BindingId(nameof(unit));

        internal static readonly BindingId invalidProperty = new BindingId(nameof(invalid));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId lowValueProperty = new BindingId(nameof(lowValue));

        internal static readonly BindingId highValueProperty = new BindingId(nameof(highValue));

        internal static readonly BindingId validateValueProperty = new BindingId(nameof(validateValue));

#endif

        /// <summary>
        /// The NumericalField main styling class.
        /// </summary>
        public const string ussClassName = "appui-numericalfield";

        /// <summary>
        /// The NumericalField input container styling class.
        /// </summary>
        public const string inputContainerUssClassName = ussClassName + "__inputcontainer";

        /// <summary>
        /// The NumericalField input styling class.
        /// </summary>
        public const string inputUssClassName = ussClassName + "__input";

        /// <summary>
        /// The NumericalField unit styling class.
        /// </summary>
        public const string unitUssClassName = ussClassName + "__unit";

        /// <summary>
        /// The NumericalField trailing container styling class.
        /// </summary>
        public const string trailingContainerUssClassName = ussClassName + "__trailingcontainer";

        /// <summary>
        /// The NumericalField size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The input container.
        /// </summary>
        protected readonly VisualElement m_InputContainer;

        /// <summary>
        /// The input element.
        /// </summary>
        protected readonly UnityEngine.UIElements.TextField m_InputElement;

        /// <summary>
        /// The size of the element.
        /// </summary>
        protected Size m_Size;

        /// <summary>
        /// The trailing container.
        /// </summary>
        protected readonly VisualElement m_TrailingContainer;

        /// <summary>
        /// The unit element.
        /// </summary>
        protected readonly LocalizedTextElement m_UnitElement;

        /// <summary>
        /// The value of the element.
        /// </summary>
        protected TValueType m_Value;

        string m_FormatString;

        string m_PreviousValue;

        /// <summary>
        /// The last value of the element set during <see cref="SetValueWithoutNotify"/>.
        /// </summary>
        protected TValueType m_LastValue;

        Optional<TValueType> m_LowValue;

        Optional<TValueType> m_HighValue;

        Func<TValueType, bool> m_ValidateValue;

        /// <summary>
        /// The format string of the element.
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
                SetValueWithoutNotify(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in formatStringProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected NumericalField()
        {
            AddToClassList(ussClassName);

            focusable = true;
            pickingMode = PickingMode.Position;
            passMask = 0;
            tabIndex = 0;
            this.SetIsCompositeRoot(true);
            this.SetExcludeFromFocusRing(true);
            delegatesFocus = true;

            m_InputContainer = new VisualElement { name = inputContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_InputContainer.AddToClassList(inputContainerUssClassName);
            m_TrailingContainer = new VisualElement { name = trailingContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_TrailingContainer.AddToClassList(trailingContainerUssClassName);

            m_InputElement = new UnityEngine.UIElements.TextField { name = inputUssClassName, pickingMode = PickingMode.Ignore };
            m_InputElement.AddToClassList(inputUssClassName);
            m_InputElement.AddManipulator(new BlinkingCursor());
            m_UnitElement = new LocalizedTextElement { name = unitUssClassName, pickingMode = PickingMode.Ignore };
            m_UnitElement.AddToClassList(unitUssClassName);

            m_InputContainer.hierarchy.Add(m_InputElement);
            m_TrailingContainer.hierarchy.Add(m_UnitElement);

            hierarchy.Add(m_InputContainer);
            hierarchy.Add(m_TrailingContainer);

            m_InputElement.AddManipulator(new KeyboardFocusController(OnKeyboardFocusedIn, OnFocusedIn, OnFocusedOut));
            m_InputElement.RegisterValueChangedCallback(OnInputValueChanged);
        }

        void OnInputValueChanged(ChangeEvent<string> evt)
        {

            evt.StopPropagation();
            if (ParseStringToValue(evt.newValue, out var newValue))
            {
                if (lowValue.IsSet)
                    newValue = Max(newValue, lowValue.Value);
                if (highValue.IsSet)
                    newValue = Min(newValue, highValue.Value);

                var previousValue = m_Value;
                m_Value = newValue;

                if (validateValue != null) invalid = !validateValue(newValue);

                if (previousValue.CompareTo(m_Value) == 0)
                    return;

                using var changeEvent = ChangingEvent<TValueType>.GetPooled();
                changeEvent.target = this;
                changeEvent.previousValue = previousValue;
                changeEvent.newValue = m_Value;
                SendEvent(changeEvent);
            }
            else if (validateValue != null)
            {
                invalid = true;
            }
        }

        /// <summary>
        /// The unit of the element.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string unit
        {
            get => m_UnitElement.text;
            set
            {
                var changed = m_UnitElement.text != value;
                m_UnitElement.text = value;
                m_UnitElement.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_UnitElement.text));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in unitProperty);
#endif
            }
        }

        /// <summary>
        /// Minimum value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Optional<TValueType> lowValue
        {
            get => m_LowValue;
            set
            {
                var changed = m_LowValue != value;
                m_LowValue = value;
                SetValueWithoutNotify(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in lowValueProperty);
#endif
            }
        }

        /// <summary>
        /// Maximum value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Optional<TValueType> highValue
        {
            get => m_HighValue;
            set
            {
                var changed = m_HighValue != value;
                m_HighValue = value;
                SetValueWithoutNotify(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in highValueProperty);
#endif
            }
        }

        /// <summary>
        /// The content container of the element.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// The size of the element.
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
                RemoveFromClassList(GetSizeUssClassName(m_Size));
                m_Size = value;
                AddToClassList(GetSizeUssClassName(m_Size));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

        /// <summary>
        /// Set the value of the element without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value of the element. </param>
        public void SetValueWithoutNotify(TValueType newValue)
        {
            if (lowValue.IsSet)
                newValue = Max(newValue, lowValue.Value);
            if (highValue.IsSet)
                newValue = Min(newValue, highValue.Value);
            m_Value = newValue;
            m_LastValue = m_Value;
            var valStr = ParseValueToString(newValue);
            m_InputElement.SetValueWithoutNotify(valStr);
            if (validateValue != null) invalid = !validateValue(newValue);
        }

        /// <summary>
        /// The value of the element.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public TValueType value
        {
            get => string.IsNullOrEmpty(m_InputElement.value) && ParseStringToValue(m_InputElement.value, out var val) ? val : m_Value;
            set
            {
                var val = value;
                if (lowValue.IsSet)
                    val = Max(val, lowValue.Value);
                if (highValue.IsSet)
                    val = Min(val, highValue.Value);
                if (AreEqual(m_LastValue, val) && AreEqual(m_Value, val))
                    return;

                using var evt = ChangeEvent<TValueType>.GetPooled(m_LastValue, val);
                evt.target = this;
                SetValueWithoutNotify(val);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        /// <summary>
        /// The invalid state of the element.
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
        /// Method to validate the value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<TValueType, bool> validateValue
        {
            get => m_ValidateValue;
            set
            {
                var changed = m_ValidateValue != value;
                m_ValidateValue = value;
                if (m_ValidateValue != null)
                    invalid = !m_ValidateValue(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        void OnFocusedOut(FocusOutEvent evt)
        {
            RemoveFromClassList(Styles.focusedUssClassName);
            RemoveFromClassList(Styles.keyboardFocusUssClassName);

            var valueStr = ParseValueToString(m_Value);
            var val = valueStr != m_InputElement.value && ParseStringToValue(m_InputElement.value, out var newValue) ? newValue : m_Value;
            value = val;
            SetValueWithoutNotify(val);
#if UNITY_2022_1_OR_NEWER
            m_InputElement.cursorIndex = 0;
#endif
        }

        void OnFocusedIn(FocusInEvent evt)
        {
            AddToClassList(Styles.focusedUssClassName);
            passMask = 0;
            m_InputElement.SetValueWithoutNotify(ParseRawValueToString(m_Value));
        }

        void OnKeyboardFocusedIn(FocusInEvent evt)
        {
            AddToClassList(Styles.focusedUssClassName);
            AddToClassList(Styles.keyboardFocusUssClassName);
            passMask = Passes.Clear | Passes.Outline;
            m_InputElement.SetValueWithoutNotify(ParseRawValueToString(m_Value));
        }

        /// <summary>
        /// Define the conversion from the <see cref="string"/> value to a <typeparamref name="TValueType"/> value.
        /// </summary>
        /// <param name="strValue">The <see cref="string"/> value to convert.</param>
        /// <param name="val">The <typeparamref name="TValueType"/> value returned.</param>
        /// <returns>True if the conversion is possible, False otherwise.</returns>
        protected abstract bool ParseStringToValue(string strValue, out TValueType val);

        /// <summary>
        /// Define the conversion from a <typeparamref name="TValueType"/> value to a <see cref="string"/> value.
        /// </summary>
        /// <param name="val">The <typeparamref name="TValueType"/> value to convert.</param>
        /// <returns>The converted value.</returns>
        protected abstract string ParseValueToString(TValueType val);

        /// <summary>
        /// Define the conversion from a <typeparamref name="TValueType"/> value to a <see cref="string"/> value.
        /// </summary>
        /// <param name="val"> The <typeparamref name="TValueType"/> value to convert. </param>
        /// <returns> The converted value. </returns>
        /// <remarks>
        /// This method is used to convert the value to a string without any formatting.
        /// </remarks>
        protected abstract string ParseRawValueToString(TValueType val);

        /// <summary>
        /// Check if two values of type <typeparamref name="TValueType"/> are equal.
        /// </summary>
        /// <param name="a">The first value to test.</param>
        /// <param name="b">The second value to test.</param>
        /// <returns>True if both values are considered equals, false otherwise.</returns>
        protected abstract bool AreEqual(TValueType a, TValueType b);

        /// <summary>
        /// Increment a given value with a given delta.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        /// <param name="delta">The delta used for increment.</param>
        /// <returns>The incremented value.</returns>
        protected abstract TValueType Increment(TValueType originalValue, float delta);

        /// <summary>
        /// Return the smallest value between a and b.
        /// </summary>
        /// <param name="a">The first value to test.</param>
        /// <param name="b">The second value to test.</param>
        /// <returns>The smallest value.</returns>
        protected abstract TValueType Min(TValueType a, TValueType b);

        /// <summary>
        /// Return the biggest value between a and b.
        /// </summary>
        /// <param name="a">The first value to test.</param>
        /// <param name="b">The second value to test.</param>
        /// <returns>The biggest value.</returns>
        protected abstract TValueType Max(TValueType a, TValueType b);

        /// <summary>
        /// Calculate the increment factor based on a base value.
        /// </summary>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The increment factor.</returns>
        protected abstract float GetIncrementFactor(TValueType baseValue);

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="NumericalField{TValueType}"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_HighValue = new UxmlStringAttributeDescription { name = "high-value", defaultValue = null };

            readonly UxmlStringAttributeDescription m_LowValue = new UxmlStringAttributeDescription { name = "low-value", defaultValue = null };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlStringAttributeDescription m_Unit = new UxmlStringAttributeDescription
            {
                name = "unit",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription { name = "value", defaultValue = "0" };

            readonly UxmlStringAttributeDescription m_Format = new UxmlStringAttributeDescription { name = "format-string", defaultValue = null };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var element = (NumericalField<TValueType>)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.unit = m_Unit.GetValueFromBag(bag, cc);

                var strValue = m_Value.GetValueFromBag(bag, cc);
                if (!string.IsNullOrEmpty(strValue) && element.ParseStringToValue(strValue, out var value))
                    element.SetValueWithoutNotify(value);

                var strLowValue = m_LowValue.GetValueFromBag(bag, cc);
                if (!string.IsNullOrEmpty(strLowValue) && element.ParseStringToValue(strLowValue, out var lowValue))
                    element.lowValue = lowValue;

                var strHighValue = m_HighValue.GetValueFromBag(bag, cc);
                if (!string.IsNullOrEmpty(strHighValue) && element.ParseStringToValue(strHighValue, out var highValue))
                    element.highValue = highValue;

                string formatStr = null;
                if (m_Format.TryGetValueFromBag(bag, cc, ref formatStr) && !string.IsNullOrEmpty(formatStr))
                    element.formatString = formatStr;


            }
        }

#endif
    }
}
