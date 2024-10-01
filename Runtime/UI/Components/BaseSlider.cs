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
    /// Base class for any Slider (<see cref="TouchSliderFloat"/>, <see cref="TouchSliderInt"/>,
    /// <see cref="SliderFloat"/>, <see cref="SliderInt"/>).
    /// </summary>
    /// <typeparam name="TValueType">A comparable value type.</typeparam>
    /// <typeparam name="THandleValueType">A value type for a single handle of the slider. This can be the same as <typeparamref name="TValueType"/>.</typeparam>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class BaseSlider<TValueType, THandleValueType> : ExVisualElement, IInputElement<TValueType>, INotifyValueChanging<TValueType>
        where TValueType : IEquatable<TValueType>
        where THandleValueType : struct, IComparable, IEquatable<THandleValueType>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId lowValueProperty = nameof(lowValue);

        internal static readonly BindingId highValueProperty = nameof(highValue);

        internal static readonly BindingId formatStringProperty = nameof(formatString);

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

#endif

        /// <summary>
        /// The dragger manipulator used to move the slider.
        /// </summary>
        protected Draggable m_DraggerManipulator;

        /// <summary>
        /// Slider max value.
        /// </summary>
        protected THandleValueType m_HighValue;

        /// <summary>
        /// Slider min value.
        /// </summary>
        protected THandleValueType m_LowValue;

        /// <summary>
        /// The previous value of the slider before the user started interacting with it.
        /// </summary>
        protected TValueType m_PreviousValue;

        /// <summary>
        /// The current value of the slider.
        /// </summary>
        protected TValueType m_Value;

        string m_FormatString;

        /// <summary>
        /// The current direction of the layout.
        /// </summary>
        protected Dir m_CurrentDirection;

        Func<TValueType, bool> m_ValidateValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseSlider()
        {
            passMask = Passes.Clear;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            this.RegisterContextChangedCallback<DirContext>(OnDirectionChanged);
        }

        /// <summary>
        /// Specify the minimum value in the range of this slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public THandleValueType lowValue
        {
            get => m_LowValue;
            set
            {
                if (!EqualityComparer<THandleValueType>.Default.Equals(m_LowValue, value))
                {
                    m_LowValue = value;
                    OnSliderRangeChanged();

#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in lowValueProperty);
#endif
                }
            }
        }

        /// <summary>
        /// Specify the maximum value in the range of this slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public THandleValueType highValue
        {
            get => m_HighValue;
            set
            {
                if (!EqualityComparer<THandleValueType>.Default.Equals(m_HighValue, value))
                {
                    m_HighValue = value;
                    OnSliderRangeChanged();

#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in highValueProperty);
#endif
                }
            }
        }

        /// <summary>
        /// The format string used to display the value of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
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
        /// Set the value of the slider without sending any event.
        /// </summary>
        /// <param name="newValue"> The new value of the slider.</param>
        public abstract void SetValueWithoutNotify(TValueType newValue);

        /// <summary>
        /// The current value of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public TValueType value
        {
            get => m_Value;
            set
            {
                var newValue = GetClampedValue(value);

                if (!EqualityComparer<TValueType>.Default.Equals(m_Value, newValue))
                {
                    if (panel != null)
                    {
                        using var evt = ChangeEvent<TValueType>.GetPooled(m_Value, newValue);
                        evt.target = this;
                        SetValueWithoutNotify(newValue);
                        SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                        NotifyPropertyChanged(in valueProperty);
#endif
                        InvokeValueChangedCallbacks();
                    }
                    else
                    {
                        SetValueWithoutNotify(newValue);
                    }
                }
            }
        }

        /// <summary>
        /// The invalid state of the slider.
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
                if (invalid == value)
                    return;

                EnableInClassList(Styles.invalidUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in invalidProperty);
#endif
            }
        }

        /// <summary>
        /// The validation function used to validate the value of the slider.
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
                invalid = !m_ValidateValue?.Invoke(m_Value) ?? false;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        /// <summary>
        /// Called when the low or high value of Slider has changed.
        /// </summary>
        protected virtual void OnSliderRangeChanged()
        {
            ClampValue();
        }

        /// <summary>
        /// Called when the value of the slider has changed via the <see cref="value"/> property.
        /// </summary>
        protected virtual void InvokeValueChangedCallbacks() {}

        /// <summary>
        /// Event callback called when the geometry of the slider has changed in the layout.
        /// </summary>
        /// <param name="evt"> The geometry changed event.</param>
        protected virtual void OnGeometryChanged(GeometryChangedEvent evt)
        {
            SetValueWithoutNotify(value);
        }

        /// <summary>
        /// Event callback called when the direction of the layout has changed.
        /// </summary>
        /// <param name="evt"> The direction context changed event</param>
        protected virtual void OnDirectionChanged(ContextChangedEvent<DirContext> evt)
        {
            m_CurrentDirection = evt.context?.dir ?? Dir.Ltr;
            SetValueWithoutNotify(value);
        }

        /// <summary>
        /// Event callback called when a pointer up event is received.
        /// </summary>
        /// <param name="dragger"> The dragger manipulator.</param>
        protected virtual void OnTrackUp(Draggable dragger)
        {
            if (value.Equals(m_PreviousValue))
                return;

            using var evt = ChangeEvent<TValueType>.GetPooled(m_PreviousValue, value);
            evt.target = this;
            SendEvent(evt);
        }

        /// <summary>
        /// Event callback called when a pointer down event is received.
        /// </summary>
        /// <param name="dragger"> The dragger manipulator.</param>
        protected virtual void OnTrackDown(Draggable dragger)
        {
            m_PreviousValue = value;
        }

        /// <summary>
        /// Event callback called when the dragger is dragged.
        /// </summary>
        /// <param name="dragger"> The dragger manipulator.</param>
        protected virtual void OnTrackDragged(Draggable dragger)
        {
            SetValueFromDrag(m_DraggerManipulator.localPosition.x);
        }

        /// <summary>
        /// Custom implementation of the slider value from the drag position.
        /// </summary>
        /// <param name="newPos"> The new position of the dragger.</param>
        protected virtual void SetValueFromDrag(float newPos)
        {
            var sliderRect = GetSliderRect();
            var newValue = ComputeValueFromHandlePosition(sliderRect.width, newPos - sliderRect.x);
            SetValueWithoutNotify(newValue);

            using var evt = ChangingEvent<TValueType>.GetPooled();
            evt.previousValue = m_PreviousValue;
            evt.newValue = newValue;
            evt.target = this;
            SendEvent(evt);
        }

        /// <summary>
        /// Returns the rect of the interactive part of the slider.
        /// </summary>
        /// <returns> The rect of the interactive part of the slider.</returns>
        protected virtual Rect GetSliderRect() => new Rect(0, 0, contentRect.width, contentRect.height);

        /// <summary>
        /// Returns the value to set as slider value based on a given dragger position.
        /// </summary>
        /// <param name="sliderLength"> The length of the slider.</param>
        /// <param name="dragElementPos"> The position of the dragger.</param>
        /// <returns> The value to set as slider value based on a given dragger position.</returns>
        protected virtual TValueType ComputeValueFromHandlePosition(float sliderLength, float dragElementPos)
        {
            if (sliderLength < Mathf.Epsilon)
                return default;

            var finalPos = m_CurrentDirection == Dir.Ltr ? dragElementPos : sliderLength - dragElementPos;
            var normalizedDragElementPosition = Mathf.Max(0f, Mathf.Min(finalPos, sliderLength)) / sliderLength;
            return SliderLerpUnclamped(lowValue, highValue, normalizedDragElementPosition);
        }

        /// <summary>
        /// Called when the track has received a click event.
        /// </summary>
        /// <remarks>
        /// Always check if the mouse has moved using <see cref="Draggable.hasMoved"/>.
        /// </remarks>
        protected virtual void OnTrackClicked()
        {
            if (!m_DraggerManipulator.hasMoved)
            {
                OnTrackDragged(m_DraggerManipulator);
                OnTrackUp(m_DraggerManipulator);
            }
        }

        /// <summary>
        /// Return the clamped value using current <see cref="lowValue"/> and <see cref="highValue"/> values.
        /// </summary>
        /// <param name="newValue">The value to clamp.</param>
        /// <returns> The clamped value.</returns>
        /// <remarks>
        /// The method also checks if low and high values are inverted.
        /// </remarks>
        protected virtual TValueType GetClampedValue(TValueType newValue)
        {
            THandleValueType lowest = lowValue, highest = highValue;
            if (lowest is IComparable lowC && highest is IComparable highC && lowC.CompareTo(highC) > 0)
            {
                // ReSharper disable once SwapViaDeconstruction
                var t = lowest;
                lowest = highest;
                highest = t;
            }

            return Clamp(newValue, lowest, highest);
        }

        /// <summary>
        /// Called when the low or high value has changed and needs to check if the current value fits in this new range.
        /// </summary>
        protected virtual void ClampValue()
        {
            value = m_Value;
        }

        /// <summary>
        /// Utility method to clamp a <typeparamref name="TValueType"/> value between specified bounds.
        /// </summary>
        /// <param name="v">The value to clamp.</param>
        /// <param name="lowBound">Minimum</param>
        /// <param name="highBound">Maximum</param>
        /// <returns> The clamped value.</returns>
        protected abstract TValueType Clamp(TValueType v, THandleValueType lowBound, THandleValueType highBound);

        /// <summary>
        /// <para>Method to implement to resolve a <typeparamref name="TValueType"/> value into a <see cref="string"/> value.</para>
        /// <para>You can use <see cref="object.ToString"/> for floating point value types for example.</para>
        /// <para>You can also round the value if you want a specific number of decimals.</para>
        /// </summary>
        /// <param name="val">The <typeparamref name="TValueType"/> value to convert.</param>
        /// <returns> The converted value.</returns>
        protected virtual string ParseValueToString(TValueType val)
        {
            return val.ToString();
        }


        /// <summary>
        /// Method to implement to resolve a <typeparamref name="TValueType"/> value into a <see cref="string"/> value.
        /// </summary>
        /// <param name="val"> The <typeparamref name="TValueType"/> value to convert.</param>
        /// <returns> The converted value.</returns>
        /// <remarks>
        /// This method is used to convert the value to a string when the user is editing the value in the input field.
        /// This must not use the <see cref="formatString"/> property.
        /// </remarks>
        protected virtual string ParseRawValueToString(TValueType val)
        {
            return val.ToString();
        }

        /// <summary>
        /// <para>Method to implement to resolve a <typeparamref name="TValueType"/> value into a <see cref="string"/> value.</para>
        /// <para>You can use <see cref="object.ToString"/> for floating point value types for example.</para>
        /// <para>You can also round the value if you want a specific number of decimals.</para>
        /// </summary>
        /// <param name="val">The <typeparamref name="TValueType"/> value to convert.</param>
        /// <returns> The converted value.</returns>
        protected virtual string ParseHandleValueToString(THandleValueType val)
        {
            return val.ToString();
        }

        /// <summary>
        /// <para>Method to implement to resolve a <see cref="string"/> value into a <typeparamref name="TValueType"/> value.</para>
        /// <para>You can use <see cref="float.TryParse(string, out float)"/> for floating point value types for example.</para>
        /// </summary>
        /// <param name="strValue">The <see cref="string"/> value to convert.</param>
        /// <param name="value">The <see cref="string"/>The converted value.</param>
        /// <returns> True if can be parsed, False otherwise.</returns>
        protected abstract bool ParseStringToValue(string strValue, out TValueType value);


        /// <summary>
        /// <para>Method to implement which returns a value based on the linear interpolation of a given interpolant between
        /// a specific range.</para>
        /// <para>Usually, you can use directly <see cref="Mathf.LerpUnclamped"/> for floating point value types.</para>
        /// </summary>
        /// <param name="a">The lowest value in the range.</param>
        /// <param name="b">The highest value in the range.</param>
        /// <param name="interpolant">The normalized value to process.</param>
        /// <returns> The interpolated value.</returns>
        protected abstract TValueType SliderLerpUnclamped(THandleValueType a, THandleValueType b, float interpolant);

        /// <summary>
        /// <para>Method to implement which returns the normalized value of a given value in a specific range.</para>
        /// <para>Usually, you can use directly an <see cref="Mathf.InverseLerp"/> for floating point value types.</para>
        /// </summary>
        /// <param name="currentValue">The value to normalize.</param>
        /// <param name="lowerValue">The lowest value in the range.</param>
        /// <param name="higherValue">The highest value in the range.</param>
        /// <returns> The normalized value.</returns>
        protected abstract float SliderNormalizeValue(THandleValueType currentValue, THandleValueType lowerValue, THandleValueType higherValue);


        /// <summary>
        /// Method to implement which returns the decrement of a given value.
        /// </summary>
        /// <param name="val"> The value to decrement.</param>
        /// <returns> The decremented value.</returns>
        protected abstract THandleValueType Decrement(THandleValueType val);

        /// <summary>
        /// Method to implement which returns the increment of a given value.
        /// </summary>
        /// <param name="val"> The value to increment.</param>
        /// <returns> The incremented value.</returns>
        protected abstract THandleValueType Increment(THandleValueType val);

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="BaseSlider{TValueType,THandleValueType}"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits {}

#endif

    }
}
