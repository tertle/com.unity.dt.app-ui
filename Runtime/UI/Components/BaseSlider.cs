using System;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Base class for any kind of Slider such as <see cref="TouchSliderFloat"/>,
    /// <see cref="SliderFloat"/> or <see cref="RangeSliderFloat"/>.
    /// </summary>
    /// <typeparam name="TValue">A value type for the slider.</typeparam>
    /// <typeparam name="TScalar">A value type for a single thumb of the slider.</typeparam>
    /// <seealso cref="Slider{TValue,TScalar,TInputField}"/>
    /// <seealso cref="SliderFloat"/>
    /// <seealso cref="SliderInt"/>
    /// <seealso cref="RangeSliderFloat"/>
    /// <seealso cref="RangeSliderInt"/>
    /// <seealso cref="TouchSlider{TValue}"/>
    /// <seealso cref="TouchSliderFloat"/>
    /// <seealso cref="TouchSliderInt"/>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class BaseSlider<TValue, TScalar>
        : ExVisualElement, IInputElement<TValue>, INotifyValueChanging<TValue>, IFormattable<TScalar>
        where TScalar : unmanaged
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId lowValueProperty = nameof(lowValue);

        internal static readonly BindingId highValueProperty = nameof(highValue);

        internal static readonly BindingId formatStringProperty = nameof(formatString);

        internal static readonly BindingId formatFunctionProperty = nameof(formatFunction);

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

        internal static readonly BindingId orientationProperty = new (nameof(orientation));

        internal static readonly BindingId stepProperty = new (nameof(step));

        internal static readonly BindingId shiftStepProperty = new (nameof(shiftStep));

        internal static readonly BindingId swapThumbsProperty = new (nameof(swapThumbs));

#endif

        /// <summary>
        /// The dragger manipulator used to move the slider.
        /// </summary>
        protected Draggable m_DraggerManipulator;

        /// <summary>
        /// Slider max value.
        /// </summary>
        protected TScalar m_HighValue;

        /// <summary>
        /// Slider min value.
        /// </summary>
        protected TScalar m_LowValue;

        /// <summary>
        /// The previous value of the slider before the user started interacting with it.
        /// </summary>
        protected TValue m_PreviousValue;

        /// <summary>
        /// The current value of the slider.
        /// </summary>
        protected TValue m_Value;

        /// <summary>
        /// The current direction of the layout.
        /// </summary>
        protected Dir m_CurrentDirection;

        /// <summary>
        /// The validation function used to validate the value of the slider.
        /// </summary>
        protected Func<TValue, bool> m_ValidateValue;

        /// <summary>
        /// The orientation of the slider.
        /// </summary>
        protected Direction m_Orientation;

        /// <summary>
        /// The step value of the slider.
        /// </summary>
        protected TScalar m_Step;

        /// <summary>
        /// The shift step value of the slider.
        /// </summary>
        protected TScalar m_ShiftStep;

        /// <summary>
        /// Whether the thumbs can be swapped if the low value is greater than the high value.
        /// If false, each thumb will be clamped to its respective value.
        /// </summary>
        protected bool m_SwapThumbs;

        /// <summary>
        /// The format string used to display the value of the slider.
        /// </summary>
        protected string m_FormatString;

        /// <summary>
        /// The format function used to display the value of the slider.
        /// </summary>
        protected FormatFunction<TScalar> m_FormatFunc;

        /// <summary>
        /// The current thumb index that is being dragged.
        /// </summary>
        protected int m_DraggedThumbIndex = -1;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseSlider()
        {
            passMask = Passes.Clear;

            orientation = Direction.Horizontal;
            invalid = false;
            swapThumbs = false;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<CustomStyleResolvedEvent>(OnStyleResolved);
            this.RegisterContextChangedCallback<DirContext>(OnDirectionChanged);
        }

        /// <summary>
        /// Specify the minimum value in the range of this slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public TScalar lowValue
        {
            get => m_LowValue;
            set
            {
                if (thumbComparer.Compare(m_LowValue, value) != 0)
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
        public TScalar highValue
        {
            get => m_HighValue;
            set
            {
                if (thumbComparer.Compare(m_HighValue, value) != 0)
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
        /// Set the value of the slider without sending any event.
        /// </summary>
        /// <param name="newValue"> The new value of the slider.</param>
        public abstract void SetValueWithoutNotify(TValue newValue);

        /// <summary>
        /// The current value of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public TValue value
        {
            get => m_Value;
            set
            {
                var newValue = GetClampedValue(value);

                if (!EqualityComparer<TValue>.Default.Equals(m_Value, newValue))
                {
                    if (panel != null)
                    {
                        using var evt = ChangeEvent<TValue>.GetPooled(m_Value, newValue);
                        evt.target = this;
                        SetValueWithoutNotify(newValue);
                        SendEvent(evt);
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
        /// The format string used to display the value of the slider.
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
        /// The format function used to display the value of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public FormatFunction<TScalar> formatFunction
        {
            get => m_FormatFunc;
            set
            {
                var changed = m_FormatFunc != value;
                m_FormatFunc = value;
                SetValueWithoutNotify(this.value);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in formatFunctionProperty);
#endif
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
        public Func<TValue, bool> validateValue
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
        /// The orientation of the slider. It can be horizontal or vertical.
        /// In vertical orientation, the slider will be drawn from bottom to top.
        /// In horizontal orientation, the slider will be drawn from left to right in LTR context
        /// and from right to left in RTL context.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Direction orientation
        {
            get => m_Orientation;
            set
            {
                var previousValue = m_Orientation;
                SetOrientation(value);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (previousValue != value)
                    NotifyPropertyChanged(in orientationProperty);
#endif
            }
        }

        /// <summary>
        /// The step value of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public TScalar step
        {
            get => m_Step;
            set
            {
                if (m_Step.Equals(value))
                    return;
                SetStep(value);
            }
        }

        /// <summary>
        /// The step value of the slider when the shift key is pressed.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public TScalar shiftStep
        {
            get => m_ShiftStep;
            set
            {
                if (m_ShiftStep.Equals(value))
                    return;
                SetShiftStep(value);
            }
        }

        /// <summary>
        /// Whether the thumbs can be swapped a thumb value becomes greater than the next one or when
        /// it becomes smaller than the previous one.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool swapThumbs
        {
            get => m_SwapThumbs;
            set
            {
                if (m_SwapThumbs == value)
                    return;
                SetSwapThumbs(value);
            }
        }

        /// <summary>
        /// The comparer used to compare the values of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public IComparer<TValue> comparer { get; set; } = Comparer<TValue>.Default;

        /// <summary>
        /// The comparer used to compare the thumb values of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public IComparer<TScalar> thumbComparer { get; set; } = Comparer<TScalar>.Default;

        /// <summary>
        /// Called when the low or high value of Slider has changed.
        /// </summary>
        protected virtual void OnSliderRangeChanged()
        {
            value = m_Value;
        }

        /// <summary>
        /// Called when the orientation of the slider has changed.
        /// </summary>
        /// <param name="newValue"> The new orientation value.</param>
        protected virtual void SetOrientation(Direction newValue)
        {
            var changed = m_Orientation != newValue;
            m_Orientation = newValue;
#if ENABLE_RUNTIME_DATA_BINDINGS
            if (changed)
                NotifyPropertyChanged(in orientationProperty);
#endif
        }

        /// <summary>
        /// Set the step value of the slider.
        /// </summary>
        /// <param name="newStep"> The new step value.</param>
        protected virtual void SetStep(TScalar newStep)
        {
            var changed = thumbComparer.Compare(m_Step, newStep) != 0;
            m_Step = newStep;
#if ENABLE_RUNTIME_DATA_BINDINGS
            if (changed)
                NotifyPropertyChanged(in stepProperty);
#endif
        }

        /// <summary>
        /// Set the shift step value of the slider.
        /// </summary>
        /// <param name="newShiftStep"> The new shift step value.</param>
        protected virtual void SetShiftStep(TScalar newShiftStep)
        {
            var changed = thumbComparer.Compare(m_ShiftStep, newShiftStep) != 0;
            m_ShiftStep = newShiftStep;
#if ENABLE_RUNTIME_DATA_BINDINGS
            if (changed)
                NotifyPropertyChanged(in shiftStepProperty);
#endif
        }

        /// <summary>
        /// Set the swap thumbs value of the slider.
        /// </summary>
        /// <param name="newSwapThumbs"> The new swap thumbs value.</param>
        protected virtual void SetSwapThumbs(bool newSwapThumbs)
        {
            m_SwapThumbs = newSwapThumbs;
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in swapThumbsProperty);
#endif
        }

        /// <summary>
        /// Called when the value of the slider has changed via the <see cref="value"/> property.
        /// </summary>
        protected virtual void InvokeValueChangedCallbacks()
        {
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in valueProperty);
#endif
        }

        /// <summary>
        /// Event callback called when the geometry of the slider has changed in the layout.
        /// </summary>
        /// <param name="evt"> The geometry changed event.</param>
        protected virtual void OnGeometryChanged(GeometryChangedEvent evt)
        {
            SetValueWithoutNotify(value);
        }

        /// <summary>
        /// Event callback called when the style of the slider has been resolved.
        /// </summary>
        /// <param name="evt"> The custom style resolved event.</param>
        protected virtual void OnStyleResolved(CustomStyleResolvedEvent evt)
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
            m_DraggedThumbIndex = -1;
            if (comparer.Compare(m_PreviousValue, value) == 0)
                return;

            using var evt = ChangeEvent<TValue>.GetPooled(m_PreviousValue, value);
            evt.target = this;
            SetValueWithoutNotify(value);
            SendEvent(evt);
            InvokeValueChangedCallbacks();
        }

        /// <summary>
        /// Event callback called when a pointer down event is received.
        /// </summary>
        /// <param name="dragger"> The dragger manipulator.</param>
        protected virtual void OnTrackDown(Draggable dragger)
        {
            m_PreviousValue = value;

            Span<TScalar> values = stackalloc TScalar[thumbCount];
            GetScalarValuesFromValue(m_Value, values);
            FindClosestThumbValue(dragger, values, out var index);
            m_DraggedThumbIndex = index;
        }

        /// <summary>
        /// Event callback called when the dragger is dragged.
        /// </summary>
        /// <param name="dragger"> The dragger manipulator.</param>
        protected virtual void OnTrackDragged(Draggable dragger)
        {
            if (m_DraggedThumbIndex == -1)
                return;

            var newValue = ComputeValueFromDrag(dragger);
            SetValueWithoutNotify(newValue);

            using var evt = ChangingEvent<TValue>.GetPooled();
            evt.previousValue = m_PreviousValue;
            evt.newValue = newValue;
            evt.target = this;
            SendEvent(evt);
        }

        /// <summary>
        /// Get the element that represent the entire track of the slider.
        /// </summary>
        /// <returns> The element that represent the entire track of the slider.</returns>
        protected virtual VisualElement GetTrackElement() => this;

        /// <summary>
        /// Returns the value to set as slider value based on a given dragger position.
        /// </summary>
        /// <param name="dragger"> The dragger manipulator.</param>
        /// <returns> The value to set as slider value based on a given dragger position.</returns>
        protected virtual TValue ComputeValueFromDrag(Draggable dragger)
        {
            Span<TScalar> values = stackalloc TScalar[thumbCount];
            GetScalarValuesFromValue(m_Value, values);
            var normalizedValue = GetNormalizedValueFromDrag(dragger);
            var newValue = SliderLerpUnclamped(lowValue, highValue, normalizedValue);
            var dir = m_Orientation switch
            {
                Direction.Horizontal when m_CurrentDirection is Dir.Ltr => dragger.deltaPos.x > 0 ? 1 : -1,
                Direction.Horizontal when m_CurrentDirection is Dir.Rtl => dragger.deltaPos.x > 0 ? -1 : 1,
                Direction.Vertical => dragger.deltaPos.y > 0 ? -1 : 1, // Y-axis is inverted
                _ => 0
            };
            var newIndex = HandleSwap(values, m_DraggedThumbIndex, newValue, dir);
            m_DraggedThumbIndex = newIndex;
            return GetValueFromScalarValues(values);
        }

        /// <summary>
        /// Get a normalized value from a dragger position.
        /// </summary>
        /// <param name="dragger"> The dragger manipulator.</param>
        /// <returns> The normalized value from a dragger position.</returns>
        protected float GetNormalizedValueFromDrag(Draggable dragger)
        {
            var track = GetTrackElement();
            var newPos = track.WorldToLocal(dragger.position);
            var length = orientation switch
            {
                Direction.Horizontal => track.layout.width,
                Direction.Vertical => track.layout.height,
                _ => 0
            };
            var pos = orientation switch
            {
                Direction.Horizontal when m_CurrentDirection is Dir.Ltr => newPos.x,
                Direction.Horizontal when m_CurrentDirection is Dir.Rtl => length - newPos.x,
                Direction.Vertical => length - newPos.y,
                _ => 0
            };
            return Mathf.Max(0f, Mathf.Min(pos, length)) / length;
        }

        /// <summary>
        /// Find the closest thumb value to the given dragger position.
        /// </summary>
        /// <param name="dragger"> The dragger manipulator.</param>
        /// <param name="values"> The values of the thumbs.</param>
        /// <param name="index"> The index of the closest thumb value.</param>
        /// <returns> The closest thumb value to the given dragger position.</returns>
        protected TScalar FindClosestThumbValue(Draggable dragger, ReadOnlySpan<TScalar> values, out int index)
        {
            if (values.Length == 1)
            {
                index = 0;
                return values[0];
            }

            var normalizedDragElementPosition = GetNormalizedValueFromDrag(dragger);
            var newValue = SliderLerpUnclamped(lowValue, highValue, normalizedDragElementPosition);
            return FindClosestThumbValue(newValue, values, out index);
        }

        /// <summary>
        /// Find the closest thumb value to the given value.
        /// </summary>
        /// <param name="v"> The value to find the closest thumb value to.</param>
        /// <param name="values"> The values of the thumbs.</param>
        /// <param name="index"> The index of the closest thumb value.</param>
        /// <returns> The closest thumb value to the given value.</returns>
        protected TScalar FindClosestThumbValue(TScalar v, ReadOnlySpan<TScalar> values,
            out int index)
        {
            if (values.Length == 1)
            {
                index = 0;
                return values[0];
            }

            var low = 0;
            var high = values.Length - 1;
            var bestIdx = -1;
            TScalar bestVal = default;
            var minDistance = float.MaxValue;
            var newNormalizedValue = SliderNormalizeValue(v, lowValue, highValue);

            while (low <= high)
            {
                var mid = low + (high - low) / 2;
                var currentValue = values[mid];
                var currentNormalizedValue = SliderNormalizeValue(currentValue, lowValue, highValue);
                var distance = Mathf.Abs(currentNormalizedValue - newNormalizedValue);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestIdx = mid;
                    bestVal = currentValue;
                }

                if (currentNormalizedValue < newNormalizedValue)
                {
                    low = mid + 1;
                }
                else if (currentNormalizedValue > newNormalizedValue)
                {
                    high = mid - 1;
                }
                else
                {
                    // Exact match found
                    index = mid;
                    return currentValue;
                }
            }

            index = bestIdx;
            return bestVal;
        }

        /// <summary>
        /// Find the closest thumb value to the given value.
        /// </summary>
        /// <param name="v"> The value to find the closest thumb value to.</param>
        /// <param name="count"> The number of thumbs.</param>
        /// <param name="predicate"> The predicate to get the thumb value at a specific index.</param>
        /// <param name="index"> The index of the closest thumb value.</param>
        /// <returns> The closest thumb value to the given value.</returns>
        protected TScalar FindClosestThumbValue(TScalar v, int count, Func<int, TScalar> predicate, out int index)
        {
            if (count == 1)
            {
                index = 0;
                return predicate(0);
            }

            var low = 0;
            var high = count - 1;
            var bestIdx = -1;
            TScalar bestVal = default;
            var minDistance = float.MaxValue;
            var newNormalizedValue = SliderNormalizeValue(v, lowValue, highValue);

            while (low <= high)
            {
                var mid = low + (high - low) / 2;
                var currentValue = predicate(mid);
                var currentNormalizedValue = SliderNormalizeValue(currentValue, lowValue, highValue);
                var distance = Mathf.Abs(currentNormalizedValue - newNormalizedValue);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestIdx = mid;
                    bestVal = currentValue;
                }

                if (currentNormalizedValue < newNormalizedValue)
                {
                    low = mid + 1;
                }
                else if (currentNormalizedValue > newNormalizedValue)
                {
                    high = mid - 1;
                }
                else
                {
                    // Exact match found
                    index = mid;
                    return currentValue;
                }
            }

            index = bestIdx;
            return bestVal;
        }

        /// <summary>
        /// Base implementation of KeyDown event handler.
        /// </summary>
        /// <param name="evt"> The key down event.</param>
        protected virtual void OnKeyDown(KeyDownEvent evt)
        {
            // if (evt.target is not Thumb t)
            //     return;

            var thumbIndex = evt.target is Thumb thumb ? thumb.parent.IndexOf(thumb) : 0;
            var dir = 0;
            var previousValue = value;
            Span<TScalar> values = stackalloc TScalar[thumbCount];
            GetScalarValuesFromValue(m_Value, values);
            var previousThumbValue = values[thumbIndex];
            var newThumbValue = previousThumbValue;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (evt.keyCode)
            {
                case KeyCode.LeftArrow when m_Orientation is Direction.Horizontal && m_CurrentDirection is Dir.Ltr:
                case KeyCode.RightArrow when m_Orientation is Direction.Horizontal && m_CurrentDirection is Dir.Rtl:
                case KeyCode.DownArrow when m_Orientation is Direction.Vertical:
                    newThumbValue = KeyDecrement(newThumbValue, evt.shiftKey ? m_ShiftStep : m_Step, evt);
                    dir = -1;
                    break;
                case KeyCode.RightArrow when m_Orientation is Direction.Horizontal && m_CurrentDirection is Dir.Ltr:
                case KeyCode.LeftArrow when m_Orientation is Direction.Horizontal && m_CurrentDirection is Dir.Rtl:
                case KeyCode.UpArrow when m_Orientation is Direction.Vertical:
                    newThumbValue = KeyIncrement(newThumbValue, evt.shiftKey ? m_ShiftStep : m_Step, evt);
                    dir = 1;
                    break;
            }

            var newThumbIndex = HandleSwap(values, thumbIndex, newThumbValue, dir);
            var newValue = GetValueFromScalarValues(values);

            if (dir != 0)
            {
                evt.StopPropagation();
                newValue = GetClampedValue(newValue);
                if (comparer.Compare(newValue, previousValue) != 0)
                {
                    using var changingEvent = ChangingEvent<TValue>.GetPooled();
                    changingEvent.previousValue = previousValue;
                    changingEvent.newValue = newValue;
                    changingEvent.target = this;
                    SetValueWithoutNotify(newValue);
                    SendEvent(changingEvent);
                    using var changeEvent = ChangeEvent<TValue>.GetPooled(previousValue, newValue);
                    changeEvent.target = this;
                    SendEvent(changeEvent);
                    InvokeValueChangedCallbacks();
                }

                // if thumbs have swapped, focus the new thumb
                if (evt.target is Thumb t)
                {
                    if (evt.target != t.parent[newThumbIndex])
                        t.parent[newThumbIndex].Focus();
                    else // make sure the thumb has the keyboard focus class
                        ((Thumb)t.parent[newThumbIndex]).EnsureKeyboardFocus();
                }
            }
        }

        int HandleSwap(Span<TScalar> values, int thumbIndex, TScalar newThumbValue, int dir)
        {
            if (values.Length == 1)
            {
                values[0] = newThumbValue;
                return 0;
            }

            var newThumbIndex = thumbIndex;
            if (!swapThumbs)
            {
                // clamp the thumb value to the range of the neighboring thumbs
                newThumbValue = ClampThumb(
                    x: newThumbValue,
                    min: thumbIndex == 0 ? lowValue : values[thumbIndex - 1],
                    max: thumbIndex == values.Length - 1 ? highValue : values[thumbIndex + 1]);
                values[thumbIndex] = newThumbValue;
            }
            else
            {
                // swap thumb in values if the list becomes unsorted
                values[thumbIndex] = newThumbValue;
                switch (dir)
                {
                    case 1:
                        while (newThumbIndex < values.Length - 1 &&
                               thumbComparer.Compare(values[newThumbIndex], values[newThumbIndex + 1]) > 0)
                        {
                            (values[newThumbIndex], values[newThumbIndex + 1]) = (values[newThumbIndex + 1], values[newThumbIndex]);
                            newThumbIndex++;
                        }
                        break;
                    case -1:
                        while (newThumbIndex > 0 &&
                               thumbComparer.Compare(values[newThumbIndex], values[newThumbIndex - 1]) < 0)
                        {
                            (values[newThumbIndex], values[newThumbIndex - 1]) = (values[newThumbIndex - 1], values[newThumbIndex]);
                            newThumbIndex--;
                        }
                        break;
                }
            }

            return newThumbIndex;
        }

        /// <summary>
        /// Called when the track has received a click event.
        /// </summary>
        /// <remarks>
        /// Always check if the mouse has moved using <see cref="Draggable.hasMoved"/>.
        /// </remarks>
        protected virtual void OnTrackClicked() { }

        /// <summary>
        /// Return the clamped value using current <see cref="lowValue"/> and <see cref="highValue"/> values.
        /// </summary>
        /// <param name="newValue">The value to clamp.</param>
        /// <returns> The clamped value.</returns>
        /// <remarks>
        /// The method also checks if low and high values are inverted.
        /// </remarks>
        protected virtual TValue GetClampedValue(TValue newValue)
        {
            TScalar lowest = lowValue, highest = highValue;
            if (thumbComparer.Compare(lowest, highest) > 0)
                (lowest, highest) = (highest, lowest);

            return Clamp(newValue, lowest, highest);
        }

        /// <summary>
        /// Decrement the value of the slider on a specific thumb.
        /// </summary>
        /// <param name="baseValue"> The base value to decrement.</param>
        /// <param name="stepValue"> The decrement step value.</param>
        /// <param name="evt"> The key down event.</param>
        /// <returns> The decremented value.</returns>
        protected virtual TScalar KeyDecrement(TScalar baseValue, TScalar stepValue, KeyDownEvent evt)
        {
            return Mad(-1, stepValue, baseValue);
        }

        /// <summary>
        /// Increment the value of the slider on a specific thumb.
        /// </summary>
        /// <param name="baseValue"> The base value to increment.</param>
        /// <param name="stepValue"> The increment step value.</param>
        /// <param name="evt"> The key down event.</param>
        /// <returns> The incremented value.</returns>
        protected virtual TScalar KeyIncrement(TScalar baseValue, TScalar stepValue, KeyDownEvent evt)
        {
            return Mad(1, stepValue, baseValue);
        }

        /// <summary>
        /// Utility method to clamp a <typeparamref name="TValue"/> value between specified bounds.
        /// </summary>
        /// <param name="v">The value to clamp.</param>
        /// <param name="lowBound">Minimum</param>
        /// <param name="highBound">Maximum</param>
        /// <returns> The clamped value.</returns>
        protected virtual TValue Clamp(TValue v, TScalar lowBound, TScalar highBound)
        {
            Span<TScalar> values = stackalloc TScalar[thumbCount];
            GetScalarValuesFromValue(v, values);
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = ClampThumb(values[i], lowBound, highBound);
            }
            return GetValueFromScalarValues(values);
        }

        /// <summary>
        /// Get the number of thumbs for this slider.
        /// </summary>
        protected abstract int thumbCount { get; }

        /// <summary>
        /// Get the TValue value from the list of thumb values.
        /// </summary>
        /// <param name="values"> The list of thumb values.</param>
        /// <returns> The TValue value.</returns>
        protected abstract TValue GetValueFromScalarValues(Span<TScalar> values);

        /// <summary>
        /// Get the list of thumb values from the TValue value. This must be sorted.
        /// </summary>
        /// <param name="value"> The TValue value.</param>
        /// <param name="values"> The list of thumb values.</param>
        protected abstract void GetScalarValuesFromValue(TValue value, Span<TScalar> values);

        /// <summary>
        /// Clamp the thumb value.
        /// </summary>
        /// <param name="x"> The value to clamp.</param>
        /// <param name="min"> The minimum value.</param>
        /// <param name="max"> The maximum value.</param>
        /// <returns> The clamped values.</returns>
        protected abstract TScalar ClampThumb(TScalar x, TScalar min, TScalar max);

        /// <summary>
        /// <para>Method to implement to resolve a <typeparamref name="TValue"/> value into a <see cref="string"/> value.</para>
        /// <para>You can use <see cref="object.ToString"/> for floating point value types for example.</para>
        /// <para>You can also round the value if you want a specific number of decimals.</para>
        /// </summary>
        /// <param name="val">The <typeparamref name="TValue"/> value to convert.</param>
        /// <returns> The converted value.</returns>
        protected virtual string ParseValueToString(TValue val)
        {
            return val.ToString();
        }

        /// <summary>
        /// Method to implement to resolve a <typeparamref name="TValue"/> value into a <see cref="string"/> value.
        /// </summary>
        /// <param name="val"> The <typeparamref name="TValue"/> value to convert.</param>
        /// <returns> The converted value.</returns>
        /// <remarks>
        /// This method is used to convert the value to a string when the user is editing the value in the input field.
        /// This must not use the <see cref="formatString"/> property.
        /// </remarks>
        protected virtual string ParseRawValueToString(TValue val)
        {
            return val.ToString();
        }

        /// <summary>
        /// <para>Method to implement to resolve a <typeparamref name="TValue"/> value into a <see cref="string"/> value.</para>
        /// <para>You can use <see cref="object.ToString"/> for floating point value types for example.</para>
        /// <para>You can also round the value if you want a specific number of decimals.</para>
        /// </summary>
        /// <param name="val">The <typeparamref name="TValue"/> value to convert.</param>
        /// <returns> The converted value.</returns>
        protected virtual string ParseSubValueToString(TScalar val)
        {
            return val.ToString();
        }

        /// <summary>
        /// <para>Method to implement to resolve a <see cref="string"/> value into a <typeparamref name="TValue"/> value.</para>
        /// <para>You can use <see cref="float.TryParse(string, out float)"/> for floating point value types for example.</para>
        /// </summary>
        /// <param name="strValue">The <see cref="string"/> value to convert.</param>
        /// <param name="value">The <see cref="string"/>The converted value.</param>
        /// <returns> True if can be parsed, False otherwise.</returns>
        protected abstract bool ParseStringToValue(string strValue, out TValue value);

        /// <summary>
        /// <para>Method to implement which returns a value based on the linear interpolation of a given interpolant between
        /// a specific range.</para>
        /// <para>Usually, you can use directly <see cref="Mathf.LerpUnclamped"/> for floating point value types.</para>
        /// </summary>
        /// <param name="a">The lowest value in the range.</param>
        /// <param name="b">The highest value in the range.</param>
        /// <param name="interpolant">The normalized value to process.</param>
        /// <returns> The interpolated value.</returns>
        protected abstract TScalar SliderLerpUnclamped(TScalar a, TScalar b, float interpolant);

        /// <summary>
        /// <para>Method to implement which returns the normalized value of a given value in a specific range.</para>
        /// <para>Usually, you can use directly an <see cref="Mathf.InverseLerp"/> for floating point value types.</para>
        /// </summary>
        /// <param name="currentValue">The value to normalize.</param>
        /// <param name="lowerValue">The lowest value in the range.</param>
        /// <param name="higherValue">The highest value in the range.</param>
        /// <returns> The normalized value.</returns>
        protected abstract float SliderNormalizeValue(TScalar currentValue, TScalar lowerValue, TScalar higherValue);

        /// <summary>
        /// Performs an arithmetic multiply/add operation.
        /// </summary>
        /// <param name="m"> The number of times to multiply the a value.</param>
        /// <param name="a"> The value that will be multiplied N times and then added to the base value.</param>
        /// <param name="b"> The addition value.</param>
        /// <returns> The incremented value.</returns>
        protected abstract TScalar Mad(int m, TScalar a, TScalar b);

        /// <summary>
        /// Get the number of possible steps between the <see cref="lowValue"/> and <see cref="highValue"/>.
        /// </summary>
        /// <param name="stepValue"> The step value.</param>
        /// <returns> The number of possible steps.</returns>
        protected abstract int GetStepCount(TScalar stepValue);

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the BaseSlider.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Format = new UxmlStringAttributeDescription
            {
                name = "format-string",
                defaultValue = null
            };

            readonly UxmlBoolAttributeDescription m_Invalid = new UxmlBoolAttributeDescription
            {
                name = "invalid",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<Direction> m_Orientation = new UxmlEnumAttributeDescription<Direction>
            {
                name = "orientation",
                defaultValue = Direction.Horizontal
            };

            readonly UxmlBoolAttributeDescription m_SwapThumbs = new UxmlBoolAttributeDescription
            {
                name = "swap-thumbs",
                defaultValue = false
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is not BaseSlider<TValue, TScalar> slider)
                    return;

                slider.orientation = m_Orientation.GetValueFromBag(bag, cc);
                slider.swapThumbs = m_SwapThumbs.GetValueFromBag(bag, cc);

                string formatStr = null;
                if (m_Format.TryGetValueFromBag(bag, cc, ref formatStr) && !string.IsNullOrEmpty(formatStr))
                    slider.formatString = formatStr;

                slider.invalid = m_Invalid.GetValueFromBag(bag, cc);
            }
        }

#endif

    }
}
