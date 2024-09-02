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
    /// Inline mode for the Slider value element.
    /// </summary>
    public enum InlineValue
    {
        /// <summary>
        /// Not inline.
        /// </summary>
        None,
        /// <summary>
        /// Inline on the left.
        /// </summary>
        Start,
        /// <summary>
        /// Inline on the right.
        /// </summary>
        End,
    }

    /// <summary>
    /// Base class for Sliders (<see cref="SliderFloat"/>, <see cref="SliderInt"/>).
    /// </summary>
    /// <typeparam name="TValueType">A comparable value type.</typeparam>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class SliderBase<TValueType> : BaseSlider<TValueType, TValueType>
        where TValueType : struct, IEquatable<TValueType>, IComparable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId filledProperty = new BindingId(nameof(filled));

        internal static readonly BindingId fillOffsetProperty = new BindingId(nameof(fillOffset));

        internal static readonly BindingId inlineValueProperty = new BindingId(nameof(inlineValue));

        internal static readonly BindingId labelProperty = new BindingId(nameof(label));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId tickCountProperty = new BindingId(nameof(tickCount));

        internal static readonly BindingId tickLabelProperty = new BindingId(nameof(tickLabel));

#endif

        /// <summary>
        /// The Slider main styling class.
        /// </summary>
        public const string ussClassName = "appui-slider";

        /// <summary>
        /// The Slider size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Slider with tick labels variant styling class.
        /// </summary>
        public const string tickLabelVariantUssClassName = ussClassName + "--tick-labels";

        /// <summary>
        /// The Slider no label variant styling class.
        /// </summary>
        public const string noLabelUssClassName = ussClassName + "--no-label";

        /// <summary>
        /// The Slider tick styling class.
        /// </summary>
        public const string tickUssClassName = ussClassName + "__tick";

        /// <summary>
        /// The Slider inline value styling class.
        /// </summary>
        [EnumName("GetInlineValueUssClassName", typeof(InlineValue))]
        public const string inlineValueUssClassName = ussClassName + "--inline-value-";

        /// <summary>
        /// The Slider tick label styling class.
        /// </summary>
        public const string tickLabelUssClassName = ussClassName + "__ticklabel";

        /// <summary>
        /// The Slider ticks container styling class.
        /// </summary>
        public const string ticksUssClassName = ussClassName + "__ticks";

        /// <summary>
        /// The Slider track styling class.
        /// </summary>
        public const string trackUssClassName = ussClassName + "__track";

        /// <summary>
        /// The Slider padded container styling class.
        /// </summary>
        public const string paddedContainerUssClassName = ussClassName + "__padded-container";

        /// <summary>
        /// The Slider progress container styling class.
        /// </summary>
        public const string interactiveAreaUssClassName = ussClassName + "__interactive-area";

        /// <summary>
        /// The Slider progress styling class.
        /// </summary>
        public const string progressUssClassName = ussClassName + "__progress";

        /// <summary>
        /// The Slider handle styling class.
        /// </summary>
        public const string handleUssClassName = ussClassName + "__handle";

        /// <summary>
        /// The Slider handle container styling class.
        /// </summary>
        public const string handleContainerUssClassName = ussClassName + "__handle-container";

        /// <summary>
        /// The Slider label container styling class.
        /// </summary>
        public const string labelContainerUssClassName = ussClassName + "__labelcontainer";

        /// <summary>
        /// The Slider label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The Slider value label styling class.
        /// </summary>
        public const string valueLabelUssClassName = ussClassName + "__valuelabel";

        /// <summary>
        /// The Slider inline value label styling class.
        /// </summary>
        public const string inlineValueLabelUssClassName = ussClassName + "__inline-valuelabel";

        /// <summary>
        /// The Slider controls styling class.
        /// </summary>
        public const string controlsUssClassName = ussClassName + "__controls";

        /// <summary>
        /// The Slider control container styling class.
        /// </summary>
        public const string controlContainerUssClassName = ussClassName + "__control-container";

        float m_FillOffset;

        readonly ExVisualElement m_Handle;

        readonly LocalizedTextElement m_Label;

        readonly VisualElement m_Progress;

        Size m_Size;

        int m_TickCount;

        bool m_TickLabel;

        readonly VisualElement m_Ticks;

        readonly LocalizedTextElement m_ValueLabel;

        readonly VisualElement m_HandleContainer;

        InlineValue m_InlineValue;

        readonly VisualElement m_Controls;

        readonly LocalizedTextElement m_InlineValueLabel;

        string m_LabelStr;

        readonly VisualElement m_InteractiveArea;

        readonly VisualElement m_PaddedContainer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected SliderBase()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Ignore;
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

            m_Ticks = new VisualElement
            {
                name = ticksUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_Ticks.AddToClassList(ticksUssClassName);
            m_Controls.hierarchy.Add(m_Ticks);

            m_Progress = new VisualElement
            {
                name = progressUssClassName,
                usageHints = UsageHints.DynamicTransform,
                pickingMode = PickingMode.Ignore
            };
            m_Progress.AddToClassList(progressUssClassName);
            m_Controls.hierarchy.Add(m_Progress);

            m_PaddedContainer = new VisualElement
            {
                name = paddedContainerUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_PaddedContainer.AddToClassList(paddedContainerUssClassName);
            m_Controls.hierarchy.Add(m_PaddedContainer);

            m_InteractiveArea = new VisualElement
            {
                name = interactiveAreaUssClassName,
                pickingMode = PickingMode.Position,
            };
            m_InteractiveArea.AddToClassList(interactiveAreaUssClassName);
            m_PaddedContainer.hierarchy.Add(m_InteractiveArea);

            m_HandleContainer = new VisualElement
            {
                name = handleContainerUssClassName,
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.DynamicTransform,
            };
            m_HandleContainer.AddToClassList(handleContainerUssClassName);
            m_InteractiveArea.hierarchy.Add(m_HandleContainer);

            m_Handle = new ExVisualElement
            {
                name = handleUssClassName,
                pickingMode = PickingMode.Ignore,
                passMask = 0
            };
            m_Handle.AddToClassList(handleUssClassName);
            m_HandleContainer.hierarchy.Add(m_Handle);

            size = Size.M;
            tickCount = 0;
            label = null;
            filled = false;
            fillOffset = 0;
            inlineValue = InlineValue.None;

            RegisterCallback<KeyDownEvent>(OnKeyDown);
            m_DraggerManipulator = new Draggable(OnTrackClicked, OnTrackDragged, OnTrackUp, OnTrackDown)
            {
                dragDirection = Draggable.DragDirection.Horizontal
            };
            m_InteractiveArea.AddManipulator(m_DraggerManipulator);
            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            m_Handle.passMask = 0;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            m_Handle.passMask = Passes.Clear | Passes.Outline;
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.target == this && focusController.focusedElement == this)
            {
                var handled = false;
                var previousValue = value;
                var newValue = previousValue;

                if (evt.keyCode == KeyCode.LeftArrow)
                {
                    newValue = Decrement(newValue);
                    handled = true;
                }
                else if (evt.keyCode == KeyCode.RightArrow)
                {
                    newValue = Increment(newValue);
                    handled = true;
                }

                if (handled)
                {
                    evt.StopPropagation();


                    SetValueWithoutNotify(newValue);

                    using var changingEvt = ChangingEvent<TValueType>.GetPooled();
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
                var changed = !m_Progress.ClassListContains(Styles.hiddenUssClassName) != value;
                m_Progress.EnableInClassList(Styles.hiddenUssClassName, !value);
                RefreshUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
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
                var changed = m_InlineValue != value;
                RemoveFromClassList(GetInlineValueUssClassName(m_InlineValue));
                m_InlineValue = value;
                if (m_InlineValue != InlineValue.None)
                    AddToClassList(GetInlineValueUssClassName(m_InlineValue));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in inlineValueProperty);
#endif
            }
        }

        /// <summary>
        /// Should be normalized.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float fillOffset
        {
            get => m_FillOffset;
            set
            {
                var changed = !Mathf.Approximately(m_FillOffset, value);
                m_FillOffset = value;
                RefreshUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in fillOffsetProperty);
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
                var changed = m_LabelStr != value;
                m_LabelStr = value;
                RefreshLabel();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
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
                var changed = m_TickCount != value;
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
                if (changed)
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
                var changed = m_TickLabel != value;
                m_TickLabel = value;
                RefreshTickLabels();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
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
        /// Set the value of the slider without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value of the slider. </param>
        public override void SetValueWithoutNotify(TValueType newValue)
        {
            newValue = GetClampedValue(newValue);
            var strValue = ParseValueToString(newValue);

            m_Value = newValue;
            m_ValueLabel.text = strValue;
            m_InlineValueLabel.text = strValue;

            if (validateValue != null) invalid = !validateValue(m_Value);

            RefreshUI();
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.GetSliderRect"/>
        protected override Rect GetSliderRect() => m_InteractiveArea.contentRect;

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.OnSliderRangeChanged"/>
        protected override void OnSliderRangeChanged()
        {
            base.OnSliderRangeChanged();
            RefreshTickLabels();
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

            // progress bar
            var val = Mathf.Clamp01(SliderNormalizeValue(m_Value, lowValue, highValue));
            var trackWidth = GetSliderRect().width;

            var width = trackWidth * Mathf.Abs(val - fillOffset);
            m_Progress.style.width = trackWidth * Mathf.Abs(val - fillOffset);
            m_Progress.style.left = m_CurrentDirection == Dir.Ltr
                ? trackWidth * Mathf.Min(fillOffset, val)
                : trackWidth - width - Mathf.Min(fillOffset, val) * trackWidth;

            // handle
            m_HandleContainer.style.left = m_CurrentDirection == Dir.Ltr ?
                trackWidth * val : trackWidth - trackWidth * val;

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
                    var tickVal = SliderLerpUnclamped(lowValue, highValue, ratio);
                    tickLabelElement.text = ParseValueToString(tickVal);
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

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.Clamp"/>
        protected override TValueType Clamp(TValueType v, TValueType lowBound, TValueType highBound)
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
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="SliderBase{TValueType}"/>.
        /// </summary>
        public new class UxmlTraits : BaseSlider<TValueType,TValueType>.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_Filled = new UxmlBoolAttributeDescription
            {
                name = "filled",
                defaultValue = false
            };

            readonly UxmlFloatAttributeDescription m_FillOffset = new UxmlFloatAttributeDescription
            {
                name = "fill-offset",
                defaultValue = 0
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

                var el = (SliderBase<TValueType>)ve;
                el.label = m_Label.GetValueFromBag(bag, cc);
                el.size = m_Size.GetValueFromBag(bag, cc);
                el.tickCount = m_TickCount.GetValueFromBag(bag, cc);
                el.tickLabel = m_TickLabel.GetValueFromBag(bag, cc);
                el.filled = m_Filled.GetValueFromBag(bag, cc);
                el.fillOffset = m_FillOffset.GetValueFromBag(bag, cc);
                el.inlineValue = m_InlineValue.GetValueFromBag(bag, cc);

                string formatStr = null;
                if (m_Format.TryGetValueFromBag(bag, cc, ref formatStr) && !string.IsNullOrEmpty(formatStr))
                    el.formatString = formatStr;
            }
        }

#endif
    }

}
