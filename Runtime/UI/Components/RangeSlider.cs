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
    /// Base class for Sliders (<see cref="RangeSliderFloat"/>, <see cref="RangeSliderInt"/>).
    /// </summary>
    /// <typeparam name="TRangeType">The range type. </typeparam>
    /// <typeparam name="TValueType">A comparable value type.</typeparam>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class RangeSliderBase<TRangeType, TValueType> : BaseSlider<TRangeType, TValueType>
        where TRangeType : IEquatable<TRangeType>
        where TValueType : struct, IComparable, IEquatable<TValueType>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId filledProperty = new BindingId(nameof(filled));

        internal static readonly BindingId inlineValueProperty = new BindingId(nameof(inlineValue));

        internal static readonly BindingId labelProperty = new BindingId(nameof(label));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId tickCountProperty = new BindingId(nameof(tickCount));

        internal static readonly BindingId tickLabelProperty = new BindingId(nameof(tickLabel));

#endif

        /// <summary>
        /// The Slider main styling class.
        /// </summary>
        public const string ussClassName = SliderBase<TValueType>.ussClassName;

        /// <summary>
        /// The Slider size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = SliderBase<TValueType>.sizeUssClassName;

        /// <summary>
        /// The Slider with tick labels variant styling class.
        /// </summary>
        public const string tickLabelVariantUssClassName = SliderBase<TValueType>.tickLabelVariantUssClassName;

        /// <summary>
        /// The Slider no label variant styling class.
        /// </summary>
        public const string noLabelUssClassName = SliderBase<TValueType>.noLabelUssClassName;

        /// <summary>
        /// The Slider tick styling class.
        /// </summary>
        public const string tickUssClassName = SliderBase<TValueType>.tickUssClassName;

        /// <summary>
        /// The Slider inline value styling class.
        /// </summary>
        [EnumName("GetInlineValueUssClassName", typeof(InlineValue))]
        public const string inlineValueUssClassName = SliderBase<TValueType>.inlineValueUssClassName;

        /// <summary>
        /// The Slider tick label styling class.
        /// </summary>
        public const string tickLabelUssClassName = SliderBase<TValueType>.tickLabelUssClassName;

        /// <summary>
        /// The Slider ticks container styling class.
        /// </summary>
        public const string ticksUssClassName = SliderBase<TValueType>.ticksUssClassName;

        /// <summary>
        /// The Slider track styling class.
        /// </summary>
        public const string trackUssClassName = SliderBase<TValueType>.trackUssClassName;

        /// <summary>
        /// The Slider padded container styling class.
        /// </summary>
        public const string paddedContainerUssClassName = SliderBase<TValueType>.paddedContainerUssClassName;

        /// <summary>
        /// The Slider progress container styling class.
        /// </summary>
        public const string progressContainerUssClassName = SliderBase<TValueType>.interactiveAreaUssClassName;

        /// <summary>
        /// The Slider progress styling class.
        /// </summary>
        public const string progressUssClassName = SliderBase<TValueType>.progressUssClassName;

        /// <summary>
        /// The Slider handle styling class.
        /// </summary>
        public const string handleUssClassName = SliderBase<TValueType>.handleUssClassName;

        /// <summary>
        /// The Slider handle container styling class.
        /// </summary>
        public const string handleContainerUssClassName = SliderBase<TValueType>.handleContainerUssClassName;

        /// <summary>
        /// The Slider label container styling class.
        /// </summary>
        public const string labelContainerUssClassName = SliderBase<TValueType>.labelContainerUssClassName;

        /// <summary>
        /// The Slider label styling class.
        /// </summary>
        public const string labelUssClassName = SliderBase<TValueType>.labelUssClassName;

        /// <summary>
        /// The Slider value label styling class.
        /// </summary>
        public const string valueLabelUssClassName = SliderBase<TValueType>.valueLabelUssClassName;

        /// <summary>
        /// The Slider inline value label styling class.
        /// </summary>
        public const string inlineValueLabelUssClassName = SliderBase<TValueType>.inlineValueLabelUssClassName;

        /// <summary>
        /// The Slider controls styling class.
        /// </summary>
        public const string controlsUssClassName = SliderBase<TValueType>.controlsUssClassName;

        /// <summary>
        /// The Slider control container styling class.
        /// </summary>
        public const string controlContainerUssClassName = SliderBase<TValueType>.controlContainerUssClassName;

        float m_FillOffset;

        readonly ExVisualElement m_MinHandle;

        readonly ExVisualElement m_MaxHandle;

        readonly LocalizedTextElement m_Label;

        readonly VisualElement m_Progress;

        Size m_Size;

        int m_TickCount;

        bool m_TickLabel;

        readonly VisualElement m_Ticks;

        readonly LocalizedTextElement m_ValueLabel;

        readonly VisualElement m_MinHandleContainer;

        readonly VisualElement m_MaxHandleContainer;

        InlineValue m_InlineValue;

        readonly VisualElement m_Controls;

        readonly LocalizedTextElement m_InlineValueLabel;

        string m_LabelStr;

        readonly VisualElement m_PaddedContainer;

        readonly VisualElement m_ProgressContainer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected RangeSliderBase()
        {
            AddToClassList(ussClassName);
            AddToClassList("appui-rangeslider");

            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;

            var labelContainer = new VisualElement { name = labelContainerUssClassName, pickingMode = PickingMode.Ignore };
            labelContainer.AddToClassList(labelContainerUssClassName);
            hierarchy.Add(labelContainer);

            m_Label = new LocalizedTextElement { name = labelUssClassName, pickingMode = PickingMode.Ignore };
            m_Label.AddToClassList(labelUssClassName);
            labelContainer.hierarchy.Add(m_Label);

            m_ValueLabel = new LocalizedTextElement { name = valueLabelUssClassName, pickingMode = PickingMode.Ignore };
            m_ValueLabel.AddToClassList(valueLabelUssClassName);
            labelContainer.hierarchy.Add(m_ValueLabel);

            var controlContainer = new VisualElement { name = controlContainerUssClassName, pickingMode = PickingMode.Ignore };
            controlContainer.AddToClassList(controlContainerUssClassName);
            hierarchy.Add(controlContainer);

            m_Controls = new VisualElement { name = controlsUssClassName, pickingMode = PickingMode.Ignore };
            m_Controls.AddToClassList(controlsUssClassName);
            controlContainer.hierarchy.Add(m_Controls);

            m_InlineValueLabel = new LocalizedTextElement { name = inlineValueLabelUssClassName, pickingMode = PickingMode.Ignore };
            m_InlineValueLabel.AddToClassList(valueLabelUssClassName);
            m_InlineValueLabel.AddToClassList(inlineValueLabelUssClassName);
            controlContainer.hierarchy.Add(m_InlineValueLabel);

            var track = new VisualElement { name = trackUssClassName, pickingMode = PickingMode.Ignore };
            track.AddToClassList(trackUssClassName);
            m_Controls.hierarchy.Add(track);

            m_Ticks = new VisualElement { name = ticksUssClassName, pickingMode = PickingMode.Ignore };
            m_Ticks.AddToClassList(ticksUssClassName);
            m_Controls.hierarchy.Add(m_Ticks);

            m_PaddedContainer = new VisualElement
            {
                name = paddedContainerUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_PaddedContainer.AddToClassList(paddedContainerUssClassName);
            m_Controls.hierarchy.Add(m_PaddedContainer);

            m_ProgressContainer = new VisualElement
            {
                name = progressContainerUssClassName,
                pickingMode = PickingMode.Position,
            };
            m_ProgressContainer.AddToClassList(progressContainerUssClassName);
            m_PaddedContainer.hierarchy.Add(m_ProgressContainer);

            m_Progress = new VisualElement
            {
                name = progressUssClassName,
                usageHints = UsageHints.DynamicTransform,
                pickingMode = PickingMode.Ignore
            };
            m_Progress.AddToClassList(progressUssClassName);
            m_ProgressContainer.hierarchy.Add(m_Progress);

            m_MinHandleContainer = new VisualElement
            {
                name = handleContainerUssClassName,
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.DynamicTransform,
            };
            m_MinHandleContainer.AddToClassList(handleContainerUssClassName);
            m_ProgressContainer.hierarchy.Add(m_MinHandleContainer);

            m_MinHandle = new ExVisualElement
            {
                name = handleUssClassName,
                pickingMode = PickingMode.Ignore,
                passMask = 0
            };
            m_MinHandle.AddToClassList(handleUssClassName);
            m_MinHandleContainer.hierarchy.Add(m_MinHandle);

            m_MaxHandleContainer = new VisualElement
            {
                name = handleContainerUssClassName,
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.DynamicTransform,
            };
            m_MaxHandleContainer.AddToClassList(handleContainerUssClassName);
            m_ProgressContainer.hierarchy.Add(m_MaxHandleContainer);

            m_MaxHandle = new ExVisualElement
            {
                name = handleUssClassName,
                pickingMode = PickingMode.Ignore,
                passMask = 0
            };
            m_MaxHandle.AddToClassList(handleUssClassName);
            m_MaxHandleContainer.hierarchy.Add(m_MaxHandle);

            size = Size.M;
            tickCount = 0;
            label = null;
            filled = false;
            inlineValue = InlineValue.None;

            RegisterCallback<KeyDownEvent>(OnKeyDown);
            m_DraggerManipulator = new Draggable(OnTrackClicked, OnTrackDragged, OnTrackUp, OnTrackDown)
            {
                dragDirection = Draggable.DragDirection.Horizontal
            };
            m_ProgressContainer.AddManipulator(m_DraggerManipulator);
            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            m_MinHandle.passMask = 0;
            m_MaxHandle.passMask = 0;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            m_MinHandle.passMask = Passes.Clear | Passes.Outline;
            m_MaxHandle.passMask = Passes.Clear | Passes.Outline;
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.target == this && focusController.focusedElement == this)
            {
                var handled = false;
                var previousValue = value;
                var newValue = previousValue;

                var focusedHandleValue = m_MinHandle.tabIndex == 0 ? minValue : maxValue;

                if (evt.keyCode == KeyCode.LeftArrow)
                {
                    var decrement = Decrement(focusedHandleValue);
                    handled = decrement.CompareTo(focusedHandleValue) != 0;
                    focusedHandleValue = decrement;
                }
                else if (evt.keyCode == KeyCode.RightArrow)
                {
                    var increment = Increment(focusedHandleValue);
                    handled = increment.CompareTo(focusedHandleValue) != 0;
                    focusedHandleValue = increment;
                }

                if (handled)
                {
                    newValue = MakeRangeValue(
                        m_MinHandle.tabIndex == 0 ? focusedHandleValue : minValue,
                        m_MinHandle.tabIndex == 0 ? maxValue : focusedHandleValue);
                    evt.StopPropagation();


                    SetValueWithoutNotify(newValue);

                    using var changingEvt = ChangingEvent<TRangeType>.GetPooled();
                    changingEvt.previousValue = previousValue;
                    changingEvt.newValue = newValue;
                    changingEvt.target = this;
                    SendEvent(changingEvt);
                }
            }
        }

        /// <summary>
        /// If the slider progress is filled.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool filled
        {
            get => !m_Progress.ClassListContains(Styles.hiddenUssClassName);
            set
            {
                m_Progress.EnableInClassList(Styles.hiddenUssClassName, !value);
                RefreshUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in filledProperty);
#endif
            }
        }

        /// <summary>
        /// The inline mode for the slider value element.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public InlineValue inlineValue
        {
            get => m_InlineValue;
            set
            {
                RemoveFromClassList(GetInlineValueUssClassName(m_InlineValue));
                m_InlineValue = value;
                if (m_InlineValue != InlineValue.None)
                    AddToClassList(GetInlineValueUssClassName(m_InlineValue));

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in inlineValueProperty);
#endif
            }
        }

        /// <summary>
        /// Text which will be used for the Slider label.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string label
        {
            get => m_LabelStr;
            set
            {
                m_LabelStr = value;
                RefreshLabel();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// The number of ticks to display on the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int tickCount
        {
            get => m_TickCount;
            set
            {
                m_TickCount = value;
                m_Ticks.EnableInClassList(Styles.hiddenUssClassName, m_TickCount <= 0);
                m_Ticks.Clear();
                for (var i = 0; i < m_TickCount; i++)
                {
                    var tickItem = new VisualElement { name = tickUssClassName, pickingMode = PickingMode.Ignore };
                    tickItem.AddToClassList(tickUssClassName);
                    m_Ticks.Add(tickItem);
                }

                RefreshTickLabels();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in tickCountProperty);
#endif
            }
        }

        /// <summary>
        /// Should the tick labels be displayed.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool tickLabel
        {
            get => m_TickLabel;
            set
            {
                m_TickLabel = value;
                RefreshTickLabels();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in tickLabelProperty);
#endif
            }
        }

        /// <summary>
        /// The size of the slider.
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

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("format-string")]
        string formatStringOverride
        {
            get => formatString;
            set => formatString = value;
        }
#endif

        /// <summary>
        /// The value of the left handle.
        /// </summary>
        /// <remarks>
        /// This is not the same as the <see cref="BaseSlider{TRangeType,TValueType}.lowValue"/>
        /// which is the minimum value of the slider.
        /// </remarks>
        public abstract TValueType minValue { get; set; }

        /// <summary>
        /// The value of the right handle.
        /// </summary>
        /// <remarks>
        /// This is not the same as the <see cref="BaseSlider{TRangeType,TValueType}.highValue"/>
        /// which is the maximum value of the slider.
        /// </remarks>
        public abstract TValueType maxValue { get; set; }

        /// <summary>
        /// Create a range value from the min and max values.
        /// </summary>
        /// <param name="minValue"> The minimum value of the range. </param>
        /// <param name="maxValue"> The maximum value of the range. </param>
        /// <returns> A new range value. </returns>
        protected abstract TRangeType MakeRangeValue(TValueType minValue, TValueType maxValue);

        /// <summary>
        /// Get the minimum value from the range value.
        /// </summary>
        /// <param name="rangeValue"> The range value. </param>
        /// <returns> The minimum value of the range. </returns>
        protected abstract TValueType GetMinValue(TRangeType rangeValue);

        /// <summary>
        /// Get the maximum value from the range value.
        /// </summary>
        /// <param name="rangeValue"> The range value. </param>
        /// <returns> The maximum value of the range. </returns>
        protected abstract TValueType GetMaxValue(TRangeType rangeValue);

        /// <summary>
        /// Get the clamped value.
        /// </summary>
        /// <param name="value"> The value to clamp. </param>
        /// <param name="lowerValue"> The lower bound. </param>
        /// <param name="higherValue"> The higher bound. </param>
        /// <returns> The clamped value. </returns>
        protected abstract TValueType GetClampedValue(TValueType value, TValueType lowerValue, TValueType higherValue);

        /// <summary>
        /// Lerp between two values.
        /// </summary>
        /// <param name="a"> The first value. </param>
        /// <param name="b"> The second value. </param>
        /// <param name="interpolant"> The interpolant. </param>
        /// <returns> The lerped value. </returns>
        protected abstract TValueType LerpUnclamped(TValueType a, TValueType b, float interpolant);

        /// <summary>
        /// Find the closest handle index to the mouse position.
        /// </summary>
        /// <param name="mousePosition"> The mouse position. </param>
        /// <returns> The closest handle index. </returns>
        protected int ClosestHandleIndex(float mousePosition)
        {
            return Mathf.Abs(mousePosition - m_MinHandleContainer.layout.xMin) < Mathf.Abs(mousePosition - m_MaxHandleContainer.layout.xMin) ? 0 : 1;
        }

        /// <summary>
        /// Set the value of the slider without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value of the slider. </param>
        public override void SetValueWithoutNotify(TRangeType newValue)
        {
            newValue = GetClampedValue(newValue);
            var strValue = ParseValueToString(newValue);

            m_Value = newValue;
            m_ValueLabel.text = strValue;
            m_InlineValueLabel.text = strValue;

            if (validateValue != null) invalid = !validateValue(m_Value);

            RefreshUI();
        }

        /// <inheritdoc cref="BaseSlider{TRangeType,TValueType}.GetSliderRect"/>
        protected override Rect GetSliderRect() => m_ProgressContainer.contentRect;

        /// <inheritdoc cref="BaseSlider{TRangeType,TValueType}.OnSliderRangeChanged"/>
        protected override void OnSliderRangeChanged()
        {
            base.OnSliderRangeChanged();
            RefreshTickLabels();
        }

        /// <inheritdoc cref="BaseSlider{TRangeType,TValueType}.Clamp"/>
        protected override TRangeType Clamp(TRangeType v, TValueType lowBound, TValueType highBound)
        {
            var min = GetMinValue(v);
            var max = GetMaxValue(v);

            return MakeRangeValue(
                GetClampedValue(min, lowBound, highBound),
                GetClampedValue(max, lowBound, highBound));
        }

        /// <inheritdoc cref="BaseSlider{TRangeType,TValueType}.ComputeValueFromHandlePosition"/>
        protected override TRangeType ComputeValueFromHandlePosition(float sliderLength, float dragElementPos)
        {
            if (sliderLength < Mathf.Epsilon)
                return m_Value;

            var finalPos = m_CurrentDirection == Dir.Ltr ? dragElementPos : sliderLength - dragElementPos;
            var normalizedValue = Mathf.Clamp01(finalPos / sliderLength);
            var newVal = LerpUnclamped(lowValue, highValue, normalizedValue);

            return ClosestHandleIndex(dragElementPos) == 0 ? MakeRangeValue(newVal, maxValue) : MakeRangeValue(minValue, newVal);
        }

        void RefreshLabel()
        {
            m_Label.text = label;
            EnableInClassList(noLabelUssClassName, string.IsNullOrEmpty(label));
        }

        void RefreshUI()
        {
            if (panel == null || !contentRect.IsValid())
                return;

            // set the label
            RefreshLabel();

            var trackWidth = GetSliderRect().width;

            // min handle
            var minVal = Mathf.Clamp01(SliderNormalizeValue(minValue, lowValue, highValue));
            m_MinHandleContainer.style.left = m_CurrentDirection == Dir.Ltr ?
                trackWidth * minVal : trackWidth - trackWidth * minVal;

            // max handle
            var maxVal = Mathf.Clamp01(SliderNormalizeValue(maxValue, lowValue, highValue));
            m_MaxHandleContainer.style.left = m_CurrentDirection == Dir.Ltr ?
                trackWidth * maxVal : trackWidth - trackWidth * maxVal;

            // progress bar
            m_Progress.style.width = trackWidth * Mathf.Abs(maxVal - minVal);
            m_Progress.style.left = m_CurrentDirection == Dir.Ltr ?
                trackWidth * minVal : trackWidth - trackWidth * minVal - trackWidth * Mathf.Abs(maxVal - minVal);

            MarkDirtyRepaint();
        }

        void RefreshTickLabels()
        {
            EnableInClassList(tickLabelVariantUssClassName, tickLabel);
            for (var i = 0; i < tickCount; i++)
            {
                var tick = m_Ticks.ElementAt(i);
                if (tickLabel)
                {
                    var tickLabelElement = tick.childCount == 0 ? new TextElement { pickingMode = PickingMode.Ignore } : (TextElement)tick.ElementAt(0);
                    var ratio = i / ((float)tickCount - 1);
                    var tickVal = LerpUnclamped(lowValue, highValue, ratio);
                    tickLabelElement.text = ParseHandleValueToString(tickVal);
                    tickLabelElement.AddToClassList(tickLabelUssClassName);
                    if (tickLabelElement.parent == null)
                        tick.Add(tickLabelElement);
                }
                else
                {
                    tick.Clear();
                }
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="SliderBase{TValueType}"/>.
        /// </summary>
        public new class UxmlTraits : BaseSlider<TRangeType,TValueType>.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_Filled = new UxmlBoolAttributeDescription
            {
                name = "filled",
                defaultValue = false
            };

            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Format = new UxmlStringAttributeDescription
            {
                name = "format-string",
                defaultValue = null
            };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlIntAttributeDescription m_TickCount = new UxmlIntAttributeDescription
            {
                name = "tick-count",
                defaultValue = 0
            };

            readonly UxmlBoolAttributeDescription m_TickLabel = new UxmlBoolAttributeDescription
            {
                name = "tick-label",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<InlineValue> m_InlineValue = new UxmlEnumAttributeDescription<InlineValue>
            {
                name = "inline-value",
                defaultValue = InlineValue.None
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

                var el = (RangeSliderBase<TRangeType,TValueType>)ve;
                el.label = m_Label.GetValueFromBag(bag, cc);
                el.size = m_Size.GetValueFromBag(bag, cc);
                el.tickCount = m_TickCount.GetValueFromBag(bag, cc);
                el.tickLabel = m_TickLabel.GetValueFromBag(bag, cc);
                el.filled = m_Filled.GetValueFromBag(bag, cc);
                el.inlineValue = m_InlineValue.GetValueFromBag(bag, cc);

                string formatStr = null;
                if (m_Format.TryGetValueFromBag(bag, cc, ref formatStr) && !string.IsNullOrEmpty(formatStr))
                    el.formatString = formatStr;
            }
        }

#endif
    }
}
