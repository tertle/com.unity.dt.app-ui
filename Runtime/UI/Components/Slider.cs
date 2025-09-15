using System;
using System.Collections.Generic;
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
    /// The mode to display the value of the slider.
    /// </summary>
    public enum ValueDisplayMode
    {
        /// <summary>
        /// Do not display the value.
        /// </summary>
        Off,

        /// <summary>
        /// Always display the value next to the thumb.
        /// </summary>
        On,

        /// <summary>
        /// Display the value next to the thumb is interacted with.
        /// </summary>
        Auto
    }

    /// <summary>
    /// The mode to display the track of the slider.
    /// </summary>
    public enum TrackDisplayType
    {
        /// <summary>
        /// Do not display the track.
        /// </summary>
        Off,

        /// <summary>
        /// Display the track.
        /// </summary>
        On,

        /// <summary>
        /// Display the inverted track. For a range slider, the track is displayed outside the thumbs.
        /// </summary>
        Inverted
    }

    /// <summary>
    /// The policy to restrict the values of the slider.
    /// </summary>
    public enum RestrictedValuesPolicy
    {
        /// <summary>
        /// No restriction. The slider can take any value in its range.
        /// </summary>
        None,

        /// <summary>
        /// The slider can only take values that are multiples of the step.
        /// </summary>
        Step,

        /// <summary>
        /// The slider can only take values from a list of custom marks.
        /// </summary>
        CustomMarks,

        /// <summary>
        /// The slider can only take values that are multiples of the step when the control key is pressed,
        /// and any value otherwise.
        /// </summary>
        CtrlStep,

        /// <summary>
        /// The slider can only take values from a list of custom marks when the control key is pressed,
        /// and any value otherwise.
        /// </summary>
        CtrlCustomMarks
    }

    /// <summary>
    /// Small Data structure which represents a mark on the slider.
    /// </summary>
    /// <typeparam name="TScalar"> The value type for the mark (same as thumb value type).</typeparam>
    public struct SliderMark<TScalar>
    {
        /// <summary>
        /// The value of the mark.
        /// </summary>
        public TScalar value;

        /// <summary>
        /// The label of the mark.
        /// </summary>
        public string label;
    }

    /// <summary>
    /// Base class for Sliders (<see cref="SliderFloat"/>, <see cref="SliderInt"/>).
    /// </summary>
    /// <typeparam name="TValue">The final value type.</typeparam>
    /// <typeparam name="TScalar">The thumb single value type.</typeparam>
    /// <typeparam name="TInputField">The input field type.</typeparam>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class Slider<TValue, TScalar, TInputField>
        : BaseSlider<TValue, TScalar>
        where TScalar : unmanaged
        where TInputField : VisualElement, IValidatableElement<TValue>, IFormattable<TScalar>, new()
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId showMarksProperty = new (nameof(showMarks));

        internal static readonly BindingId showMarksLabelProperty = new (nameof(showMarksLabel));

        internal static readonly BindingId customMarksProperty = new (nameof(customMarks));

        internal static readonly BindingId displayValueLabelProperty = new (nameof(displayValueLabel));

        internal static readonly BindingId restrictedValuesProperty = new (nameof(restrictedValues));

        internal static readonly BindingId scaleProperty = new (nameof(scale));

        internal static readonly BindingId trackProperty = new (nameof(track));

#endif

        /// <summary>
        /// Handler to scale the value of the slider thumbs and marks.
        /// </summary>
        /// <param name="value"> The value to scale.</param>
        /// <returns> The scaled value.</returns>
        /// <remarks>
        /// The scale is only applied to the formatted value displayed as text.
        /// </remarks>
        /// <example>
        /// <code>
        /// var scaledSlider = new SliderInt
        /// {
        ///    scale = v => (int)Mathf.Pow(2, v),
        ///    formatFunction = v => {
        ///       var units = new string[] { "KB", "MB", "GB", "TB" };
        ///       var unitIndex = 0;
        ///       var scaledValue = v;
        ///       while (scaledValue &gt;= 1024 &amp;&amp; unitIndex &lt; units.Length - 1)
        ///       {
        ///           scaledValue /= 1024;
        ///           unitIndex++;
        ///       }
        ///       return $"{scaledValue} {units[unitIndex]}";
        ///    }
        /// };
        /// </code>
        /// </example>
        public delegate TScalar ScaleHandler(TScalar value);

        /// <summary>
        /// The USS class name for the slider.
        /// </summary>
        public const string ussClassName = "appui-slider";

        /// <summary>
        /// The USS class name for the slider control.
        /// </summary>
        public const string sliderControlUssClassName = ussClassName + "__slider-control";

        /// <summary>
        /// The USS class name for the track.
        /// </summary>
        public const string trackUssClassName = ussClassName + "__track";

        /// <summary>
        /// The USS class name for the mark container.
        /// </summary>
        public const string markContainerUssClassName = ussClassName + "__mark-container";

        /// <summary>
        /// The USS class name for the progress container.
        /// </summary>
        public const string progressContainerUssClassName = ussClassName + "__progress-container";

        /// <summary>
        /// The USS class name for the mark.
        /// </summary>
        public const string markUssClassName = ussClassName + "__mark";

        /// <summary>
        /// The USS class name for the mark label.
        /// </summary>
        public const string markLabelUssClassName = ussClassName + "__mark-label";

        /// <summary>
        /// The USS class name for the progress element.
        /// </summary>
        public const string progressUssClassName = ussClassName + "__progress";

        /// <summary>
        /// The USS class name for the thumbs container.
        /// </summary>
        public const string thumbsContainerUssClassName = ussClassName + "__thumbs-container";

        /// <summary>
        /// The USS class name for the thumb.
        /// </summary>
        public const string thumbUssClassName = ussClassName + "__thumb";

        /// <summary>
        /// The USS class name for the input field.
        /// </summary>
        public const string inputFieldUssClassName = ussClassName + "__input-field";

        /// <summary>
        /// The orientation variant USS class name.
        /// </summary>
        [EnumName("GetOrientationClassName", typeof(Direction))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The show marks variant USS class name.
        /// </summary>
        public const string showMarksUssClassName = ussClassName + "--show-marks";

        /// <summary>
        /// The show marks label variant USS class name.
        /// </summary>
        [EnumName("GetValueDisplayModeUssClassName", typeof(ValueDisplayMode))]
        public const string valueDisplayVariantUssClassName = ussClassName + "--value-display-";

        /// <summary>
        /// The track display type variant USS class name.
        /// </summary>
        [EnumName("GetTrackDisplayTypeUssClassName", typeof(TrackDisplayType))]
        public const string trackDisplayVariantUssClassName = ussClassName + "--track-";

        bool m_ShowMarks;
        bool m_ShowMarksLabel;
        IList<SliderMark<TScalar>> m_CustomMarks;
        ValueDisplayMode m_DisplayValueLabel;
        RestrictedValuesPolicy m_RestrictedValues;
        ScaleHandler m_Scale;
        TrackDisplayType m_TrackDisplayMode;
        bool m_ShowInputField;

        /// <summary>
        /// The Slider control element.
        /// </summary>
        protected readonly VisualElement m_SliderControl;

        /// <summary>
        /// The Track element of the slider. By default, this element is empty.
        /// </summary>
        protected readonly VisualElement m_TrackElement;

        /// <summary>
        /// The Progress container element of the slider. This is populated with 3 progress elements
        /// to support the track display mode.
        /// </summary>
        protected readonly VisualElement m_ProgressContainer;

        /// <summary>
        /// The Mark container element of the slider. This is populated with marks elements based on the step value
        /// or the custom marks.
        /// </summary>
        protected readonly VisualElement m_MarkContainer;

        /// <summary>
        /// The Thumbs container element of the slider. This is populated with thumb elements based on the thumb count.
        /// </summary>
        protected readonly VisualElement m_ThumbsContainer;

        /// <summary>
        /// The Input field element of the slider.
        /// </summary>
        protected readonly TInputField m_InputField;

        /// <summary>
        /// Whether to show the marks on the slider.
        /// If no custom marks are set, the marks are generated based on the step value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool showMarks
        {
            get => m_ShowMarks;
            set
            {
                if (m_ShowMarks == value)
                    return;
                SetShowMarks(value);
            }
        }

        /// <summary>
        /// Whether to show the marks labels on the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool showMarksLabel
        {
            get => m_ShowMarksLabel;
            set
            {
                if (m_ShowMarksLabel == value)
                    return;
                SetShowMarksLabel(value);
            }
        }

        /// <summary>
        /// The custom marks to display on the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public IList<SliderMark<TScalar>> customMarks
        {
            get => m_CustomMarks;
            set
            {
                if (EnumerableExtensions.SequenceEqual(m_CustomMarks, value))
                    return;
                SetCustomMarks(value);
            }
        }

        /// <summary>
        /// The mode to display the value of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public ValueDisplayMode displayValueLabel
        {
            get => m_DisplayValueLabel;
            set
            {
                if (m_DisplayValueLabel == value)
                    return;
                SetDisplayValueLabel(value);
            }
        }

        /// <summary>
        /// The policy to restrict the values of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public RestrictedValuesPolicy restrictedValues
        {
            get => m_RestrictedValues;
            set
            {
                if (m_RestrictedValues == value)
                    return;
                SetRestrictedValues(value);
            }
        }

        /// <summary>
        /// Handler to scale the value of the slider thumbs and marks.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public ScaleHandler scale
        {
            get => m_Scale;
            set
            {
                if (m_Scale == value)
                    return;
                SetScale(value);
            }
        }

        /// <summary>
        /// The mode to display the track of the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public TrackDisplayType track
        {
            get => m_TrackDisplayMode;
            set
            {
                if (m_TrackDisplayMode == value)
                    return;
                SetTrackDisplayMode(value);
            }
        }

        /// <summary>
        /// Whether to show the input field.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool showInputField
        {
            get => m_ShowInputField;
            set
            {
                if (m_ShowInputField == value)
                    return;
                SetShowInputField(value);
            }
        }

        /// <inheritdoc />
        public override VisualElement contentContainer => null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Slider()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Ignore;

            focusable = true;
            this.SetIsCompositeRoot(true);
            this.SetExcludeFromFocusRing(true);
            delegatesFocus = true;

            m_SliderControl = new VisualElement
            {
                name = sliderControlUssClassName,
                focusable = true,
                pickingMode = PickingMode.Position
            };
            m_SliderControl.SetIsCompositeRoot(true);
            m_SliderControl.SetExcludeFromFocusRing(true);
            m_SliderControl.delegatesFocus = true;
            m_SliderControl.AddToClassList(sliderControlUssClassName);
            hierarchy.Add(m_SliderControl);

            m_InputField = new TInputField();
            m_InputField.AddToClassList(inputFieldUssClassName);

            m_TrackElement = new VisualElement
            {
                name = trackUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_TrackElement.AddToClassList(trackUssClassName);
            m_SliderControl.Add(m_TrackElement);

            m_ProgressContainer = new VisualElement
            {
                name = progressContainerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_ProgressContainer.AddToClassList(progressContainerUssClassName);
            m_SliderControl.Add(m_ProgressContainer);

            m_MarkContainer = new VisualElement
            {
                name = markContainerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_MarkContainer.AddToClassList(markContainerUssClassName);
            m_SliderControl.Add(m_MarkContainer);

            m_ThumbsContainer = new VisualElement
            {
                name = thumbsContainerUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_ThumbsContainer.usageHints |= UsageHints.GroupTransform;
            m_ThumbsContainer.AddToClassList(thumbsContainerUssClassName);
            m_SliderControl.Add(m_ThumbsContainer);

            m_DraggerManipulator = new Draggable(OnTrackClicked, OnTrackDragged, OnTrackUp, OnTrackDown)
            {
                dragDirection = Draggable.DragDirection.Horizontal
            };

            orientation = Direction.Horizontal;
            SetShowMarks(false);
            SetCustomMarks(null);
            SetDisplayValueLabel(ValueDisplayMode.Off);
            SetStep(default);
            SetShiftStep(default);
            SetRestrictedValues(RestrictedValuesPolicy.None);
            SetSwapThumbs(false);
            SetScale(null);

            m_SliderControl.RegisterCallback<KeyDownEvent>(OnKeyDown);
            m_SliderControl.AddManipulator(m_DraggerManipulator);
        }

        /// <inheritdoc />
        protected override TScalar KeyDecrement(TScalar baseValue, TScalar stepValue, KeyDownEvent evt)
        {
            if (m_RestrictedValues != RestrictedValuesPolicy.None)
            {
                var restrictedValue = GetRestrictedThumbValue(baseValue, stepValue, evt.ctrlKey, out var idx);
                if (idx >= 0)
                    return m_RestrictedValues == RestrictedValuesPolicy.Step
                        ? Mad(Mathf.Max(0, idx - 1), stepValue, lowValue)
                        : m_CustomMarks[Mathf.Max(0, idx - 1)].value;
                return restrictedValue;
            }
            return Mad(-1, stepValue, baseValue);;
        }

        /// <inheritdoc />
        protected override TScalar KeyIncrement(TScalar baseValue, TScalar stepValue, KeyDownEvent evt)
        {
            if (m_RestrictedValues != RestrictedValuesPolicy.None)
            {
                var restrictedValue = GetRestrictedThumbValue(baseValue, stepValue, evt.ctrlKey, out var idx);
                if (idx >= 0)
                    return m_RestrictedValues == RestrictedValuesPolicy.Step
                        ? Mad(idx + 1, stepValue, lowValue) // do not care about clamping here, it will be done later
                        : m_CustomMarks[Mathf.Min(m_CustomMarks.Count - 1, idx + 1)].value;
                return restrictedValue;
            }
            return Mad(1, stepValue, baseValue);
        }

        /// <inheritdoc />
        protected override VisualElement GetTrackElement() => m_TrackElement;

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
        protected override void SetStep(TScalar newStep)
        {
            base.SetStep(newStep);
            RefreshUI();
        }

        /// <inheritdoc />
        protected override void OnTrackDown(Draggable dragger)
        {
            base.OnTrackDown(dragger);
            // simulate drag event to update the thumb position
            OnTrackDragged(dragger);
        }

        /// <inheritdoc />
        protected override void OnTrackUp(Draggable dragger)
        {
            base.OnTrackUp(dragger);
            ClearThumbsActiveState();
        }

        /// <inheritdoc />
        protected override TValue ComputeValueFromDrag(Draggable dragger)
        {
            var newValue = base.ComputeValueFromDrag(dragger);
            return GetRestrictedValue(newValue, dragger.ctrlKey);
        }

        /// <inheritdoc />
        protected override TValue GetClampedValue(TValue newValue)
        {
            var ret = base.GetClampedValue(newValue);
            return GetRestrictedValue(ret, false);
        }

        /// <summary>
        /// Check if the step value is valid.
        /// An invalid step value is equal to the default value (usually 0).
        /// </summary>
        /// <returns> True if the step value is valid, false otherwise. </returns>
        bool HasValidStep() => !EqualityComparer<TScalar>.Default.Equals(m_Step, default);

        /// <inheritdoc />
        public override void SetValueWithoutNotify(TValue newValue)
        {
            m_Value = GetClampedValue(newValue);
            if (validateValue != null)
                invalid = !validateValue(m_Value);
            RefreshUI();
        }

        /// <summary>
        /// Refresh the UI of the slider. This is usually called during <see cref="SetValueWithoutNotify"/>
        /// or directly in a setter of a bound property.
        /// </summary>
        protected virtual void RefreshUI()
        {
            if (panel == null || !layout.IsValid())
                return;

            Span<TScalar> newValues = stackalloc TScalar[thumbCount];
            GetScalarValuesFromValue(m_Value, newValues);

            BindInputField();
            RefreshMarks(newValues);
            RefreshProgressElements(newValues);
            RefreshThumbs(newValues);
        }

        void RefreshThumbs(ReadOnlySpan<TScalar> values)
        {
            var trackSize = m_Orientation == Direction.Horizontal ? m_TrackElement.layout.width : m_TrackElement.layout.height;
            for (var i = 0; i < values.Length; i++)
            {
                Thumb thumb;
                if (m_ThumbsContainer.childCount <= i)
                {
                    thumb = new Thumb { name = thumbUssClassName };
                    thumb.AddToClassList(thumbUssClassName);
                    m_ThumbsContainer.Add(thumb);
                }
                else
                {
                    thumb = (Thumb)m_ThumbsContainer[i];
                }

                var thumbPos = SliderNormalizeValue(values[i], lowValue, highValue);
                if (m_Orientation == Direction.Vertical || m_CurrentDirection == Dir.Rtl)
                    thumbPos = 1f - thumbPos;
                thumbPos *= trackSize;
                thumb.style.translate = m_Orientation == Direction.Horizontal
                    ? new Translate(thumbPos, 0)
                    : new Translate(0, thumbPos);
                thumb.displayValueLabel = m_DisplayValueLabel;
                thumb.text = ParseSubValueToString(scale?.Invoke(values[i]) ?? values[i]);
                thumb.EnableInClassList(Styles.invalidUssClassName, invalid);
            }

            for (var i = m_ThumbsContainer.childCount - 1; i >= values.Length; i--)
            {
                m_ThumbsContainer.RemoveAt(i);
            }

            var idx = -1;
            if (m_DraggerManipulator.active && m_DraggerManipulator.isDown)
                FindClosestThumbValue(m_DraggerManipulator, values, out idx);

            if (idx >= 0 && idx < m_ThumbsContainer.childCount)
            {
                var thumb = (Thumb)m_ThumbsContainer[idx];
                thumb.AddToClassList(Styles.activeUssClassName);
            }
            else
            {
                ClearThumbsActiveState();
            }
        }

        void RefreshProgressElements(ReadOnlySpan<TScalar> values)
        {
            if (m_TrackDisplayMode is not (TrackDisplayType.On or TrackDisplayType.Inverted))
            {
                m_ProgressContainer.Clear();
                return;
            }

            // if the display is On, progress element is visible from lowValue to thumb position if
            // there's a single thumb, or between the thumbs if there are more than one
            // if the display is Inverted, progress element is visible from thumb position to highValue
            // if there's a single thumb, or between lowValue and first thumb plus last thumb an highValue
            // if there is more than one.
            // NOTE: We assume values are sorted in ascending order.
            EnsureProgressElements(3);
            var p1 = m_ProgressContainer[0];
            var p2 = m_ProgressContainer[1];
            var p3 = m_ProgressContainer[2];

            var isHorizontal = m_Orientation == Direction.Horizontal;
            var isRtl = m_CurrentDirection == Dir.Rtl;
            var multiThumb = values.Length > 1;
            var trackSize = isHorizontal ? m_TrackElement.layout.width : m_TrackElement.layout.height;

            var thumbPositions = new float[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                var position = SliderNormalizeValue(values[i], lowValue, highValue) * trackSize;
                thumbPositions[i] = position;
            }

            if ((isHorizontal && isRtl) || !isHorizontal)
            {
                for (var i = 0; i < thumbPositions.Length; i++)
                {
                    thumbPositions[i] = trackSize - thumbPositions[i];
                }
                Array.Reverse(thumbPositions);
            }

            // Set transforms for progress elements
            SetProgressElementTransform(p1, 0, thumbPositions[0], isHorizontal);
            SetProgressElementTransform(p2, thumbPositions[0], thumbPositions[^1], isHorizontal);
            SetProgressElementTransform(p3, thumbPositions[^1], trackSize, isHorizontal);

            // Determine activation states
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            var isP1Active = m_TrackDisplayMode switch
            {
                TrackDisplayType.On when isHorizontal && !isRtl => !multiThumb,
                TrackDisplayType.On when isHorizontal && isRtl => false,
                TrackDisplayType.On when !isHorizontal => false, // y-axis is inverted
                TrackDisplayType.Inverted when isHorizontal && !isRtl => multiThumb,
                TrackDisplayType.Inverted when isHorizontal && isRtl => true,
                TrackDisplayType.Inverted when !isHorizontal => true, // y-axis is inverted
                _ => false
            };
            var isP2Active = m_TrackDisplayMode switch
            {
                TrackDisplayType.On => multiThumb,
                _ => false
            };
            var isP3Active = m_TrackDisplayMode switch
            {
                TrackDisplayType.On when isHorizontal && !isRtl => false,
                TrackDisplayType.On when isHorizontal && isRtl => !multiThumb,
                TrackDisplayType.On when !isHorizontal => !multiThumb, // y-axis is inverted
                TrackDisplayType.Inverted when isHorizontal && !isRtl => true,
                TrackDisplayType.Inverted when isHorizontal && isRtl => multiThumb,
                TrackDisplayType.Inverted when !isHorizontal => multiThumb, // y-axis is inverted
                _ => false
            };
            // ReSharper restore ConditionIsAlwaysTrueOrFalse

            p1.EnableInClassList(Styles.activeUssClassName, isP1Active);
            p2.EnableInClassList(Styles.activeUssClassName, isP2Active);
            p3.EnableInClassList(Styles.activeUssClassName, isP3Active);
        }

        void EnsureProgressElements(int count)
        {
            while (m_ProgressContainer.childCount < count)
            {
                var progressElement = new VisualElement
                {
                    name = progressUssClassName,
                    pickingMode = PickingMode.Ignore
                };
                progressElement.AddToClassList(progressUssClassName);
                m_ProgressContainer.Add(progressElement);
            }

            for (var i = m_TrackElement.childCount - 1; i >= count; i--)
            {
                m_ProgressContainer.RemoveAt(i);
            }
        }

        static void SetProgressElementTransform(VisualElement element, float start, float end, bool isHorizontal)
        {
            var position = start;
            var size = end - start;

            if (isHorizontal)
            {
                element.style.left = position;
                element.style.top = StyleKeyword.Null;
                element.style.width = size;
                element.style.height = StyleKeyword.Null;
            }
            else
            {
                element.style.top = position;
                element.style.left = StyleKeyword.Null;
                element.style.height = size;
                element.style.width = StyleKeyword.Null;
            }
        }

        void RefreshMarks(ReadOnlySpan<TScalar> _)
        {
            if (m_DraggerManipulator.active)
                return;

            if (m_ShowMarks)
            {
                AddToClassList(showMarksUssClassName);
                if (m_CustomMarks != null)
                    RefreshCustomMarks();
                else if (HasValidStep())
                    RefreshStepMarks();
            }
            else
            {
                m_MarkContainer.RemoveFromClassList(showMarksUssClassName);
                m_MarkContainer.Clear();
            }
        }

        void RefreshStepMarks()
        {
            var stepIndex = 0;
            var stepValue = Mad(stepIndex, m_Step, lowValue);
            const int maxSteps = 32;
            while (thumbComparer.Compare(stepValue, highValue) <= 0 && stepIndex < maxSteps)
            {
                var scaledValue =  scale != null ? scale(stepValue) : stepValue;
                var pct = SliderNormalizeValue(stepValue, lowValue, highValue);
                VisualElement markElement;
                if (m_MarkContainer.childCount > stepIndex)
                {
                    markElement = m_MarkContainer[stepIndex];
                }
                else
                {
                    markElement = new VisualElement
                    {
                        name = markUssClassName,
                        pickingMode = PickingMode.Ignore,
                    };
                    markElement.AddToClassList(markUssClassName);
                    m_MarkContainer.Add(markElement);
                }

                if (m_Orientation == Direction.Horizontal)
                    markElement.style.left = new Length(pct * 100f, LengthUnit.Percent);
                else
                    markElement.style.bottom = new Length(pct * 100f, LengthUnit.Percent);

                if (showMarksLabel)
                {
                    TextElement labelElement;
                    if (markElement.childCount > 0)
                    {
                        labelElement = (TextElement)markElement[0];
                    }
                    else
                    {
                        labelElement = new TextElement
                        {
                            name = markLabelUssClassName,
                            pickingMode = PickingMode.Ignore
                        };
                        labelElement.AddToClassList(markLabelUssClassName);
                        markElement.Add(labelElement);
                    }
                    labelElement.text = ParseSubValueToString(scaledValue);
                }
                else if (markElement.childCount > 0)
                {
                    markElement.Clear();
                }

                stepIndex++;
                stepValue = Mad(stepIndex, m_Step, lowValue);
            }

            for (var i = m_MarkContainer.childCount - 1; i >= stepIndex; i--)
            {
                m_MarkContainer.RemoveAt(i);
            }
        }

        void RefreshCustomMarks()
        {
            // add/remove mark visual elements based on custom marks
            for (var i = 0; i < m_CustomMarks.Count; i++)
            {
                var mark = m_CustomMarks[i];
                var pct = SliderNormalizeValue(mark.value, lowValue, highValue);
                VisualElement markElement;
                if (m_MarkContainer.childCount > i)
                {
                    markElement = m_MarkContainer[i];
                }
                else
                {
                    markElement = new VisualElement
                    {
                        name = markUssClassName,
                        pickingMode = PickingMode.Ignore,
                    };
                    markElement.AddToClassList(markUssClassName);
                    m_MarkContainer.Add(markElement);
                }
                if (m_Orientation == Direction.Vertical)
                    markElement.style.bottom = new Length(pct * 100f, LengthUnit.Percent);
                else
                    markElement.style.left = new Length(pct * 100f, LengthUnit.Percent);

                if (string.IsNullOrEmpty(mark.label) || !m_ShowMarksLabel)
                {
                    if (markElement.childCount > 0)
                        markElement.RemoveAt(0);
                }
                else
                {
                    TextElement labelElement;
                    if (markElement.childCount > 0)
                    {
                        labelElement = (TextElement)markElement[0];
                    }
                    else
                    {
                        labelElement = new TextElement
                        {
                            name = markLabelUssClassName,
                            pickingMode = PickingMode.Ignore
                        };
                        labelElement.AddToClassList(markLabelUssClassName);
                        markElement.Add(labelElement);
                    }
                    labelElement.text = mark.label;
                }
            }

            for (var i = m_MarkContainer.childCount - 1; i >= m_CustomMarks.Count; i--)
            {
                m_MarkContainer.RemoveAt(i);
            }
        }

        void SetShowMarks(bool newShowMarks)
        {
            m_ShowMarks = newShowMarks;
            EnableInClassList(showMarksUssClassName, m_ShowMarks);
            RefreshUI();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in showMarksProperty);
#endif
        }

        void SetShowMarksLabel(bool newShowMarksLabel)
        {
            m_ShowMarksLabel = newShowMarksLabel;
            RefreshUI();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in showMarksLabelProperty);
#endif
        }

        void SetCustomMarks(IList<SliderMark<TScalar>> newCustomMarks)
        {
            m_CustomMarks = newCustomMarks;
            RefreshUI();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in customMarksProperty);
#endif
        }

        void SetDisplayValueLabel(ValueDisplayMode newValueDisplayMode)
        {
            m_DisplayValueLabel = newValueDisplayMode;
            RefreshUI();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in displayValueLabelProperty);
#endif
        }

        void SetRestrictedValues(RestrictedValuesPolicy newRestrictedValues)
        {
            m_RestrictedValues = newRestrictedValues;
            SetValueWithoutNotify(m_Value);
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in restrictedValuesProperty);
#endif
        }

        void SetScale(ScaleHandler newScale)
        {
            m_Scale = newScale;
            RefreshUI();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in scaleProperty);
#endif
        }

        void SetTrackDisplayMode(TrackDisplayType newTrackDisplayMode)
        {
            RemoveFromClassList(GetTrackDisplayTypeUssClassName(m_TrackDisplayMode));
            m_TrackDisplayMode = newTrackDisplayMode;
            AddToClassList(GetTrackDisplayTypeUssClassName(m_TrackDisplayMode));
            RefreshUI();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in trackProperty);
#endif
        }

        void BindInputField()
        {
            m_InputField.formatString = formatString;
            m_InputField.formatFunction = formatFunction;
            m_InputField.invalid = invalid;
            m_InputField.SetValueWithoutNotify(m_Value);
        }

        void SetShowInputField(bool newShowInputField)
        {
            m_ShowInputField = newShowInputField;
            if (m_ShowInputField)
            {
                m_InputField.RegisterValueChangedCallback(OnInputFieldValueChanged);
                hierarchy.Add(m_InputField);
            }
            else
            {
                m_InputField.UnregisterValueChangedCallback(OnInputFieldValueChanged);
                m_InputField.RemoveFromHierarchy();
            }
            SetValueWithoutNotify(m_Value);
        }

        void OnInputFieldValueChanged(ChangeEvent<TValue> evt)
        {
            value = evt.newValue;
        }

        void ClearThumbsActiveState()
        {
            for (var i = 0; i < m_ThumbsContainer.childCount; i++)
            {
                var thumb = (Thumb)m_ThumbsContainer[i];
                thumb.RemoveFromClassList(Styles.activeUssClassName);
            }
        }

        TValue GetRestrictedValue(TValue newValue, bool ctrlKey)
        {
            Span<TScalar> newValues = stackalloc TScalar[thumbCount];
            GetScalarValuesFromValue(newValue, newValues);

            for (var i = 0; i < newValues.Length; i++)
            {
                newValues[i] = GetRestrictedThumbValue(newValues[i], m_Step, ctrlKey, out _);
            }

            return GetValueFromScalarValues(newValues);
        }

        TScalar RestrictedThumbValue(TScalar newValue, TScalar stepValue,
            out int stepOrMarkIndex)
        {
            m_GetCustomMarkValuePredicate ??= GetCustomMarkValue;
            m_GetStepValuePredicate ??= GetStepValue;
            stepOrMarkIndex = -1;
            if (restrictedValues is RestrictedValuesPolicy.CustomMarks or RestrictedValuesPolicy.CtrlCustomMarks &&
                m_CustomMarks is {Count: >0})
                newValue = FindClosestThumbValue(newValue, m_CustomMarks.Count, m_GetCustomMarkValuePredicate, out stepOrMarkIndex);
            if (restrictedValues is RestrictedValuesPolicy.Step or RestrictedValuesPolicy.CtrlStep && HasValidStep())
                newValue = FindClosestThumbValue(newValue, GetStepCount(stepValue), m_GetStepValuePredicate, out stepOrMarkIndex);
            return newValue;
        }

        Func<int,TScalar> m_GetCustomMarkValuePredicate;

        TScalar GetCustomMarkValue(int index)
        {
            return m_CustomMarks[index].value;
        }

        Func<int,TScalar> m_GetStepValuePredicate;

        TScalar GetStepValue(int index)
        {
            return Mad(index, m_Step, lowValue);
        }

        TScalar GetRestrictedThumbValue(
            TScalar newValue,
            TScalar stepValue,
            bool ctrlKey,
            out int stepOrMarkIndex)
        {
            stepOrMarkIndex = -1;
            switch (m_RestrictedValues)
            {
                case RestrictedValuesPolicy.Step:
                case RestrictedValuesPolicy.CustomMarks:
                case RestrictedValuesPolicy.CtrlStep when ctrlKey:
                case RestrictedValuesPolicy.CtrlCustomMarks when ctrlKey:
                    return RestrictedThumbValue(newValue, stepValue, out stepOrMarkIndex);
                case RestrictedValuesPolicy.None:
                default:
                    return newValue;
            }
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the Slider.
        /// </summary>
        public new class UxmlTraits : BaseSlider<TValue,TScalar>.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_ShowMarks = new UxmlBoolAttributeDescription
            {
                name = "show-marks",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<ValueDisplayMode> m_ValueDisplayMode = new UxmlEnumAttributeDescription<ValueDisplayMode>
            {
                name = "display-value-label",
                defaultValue = ValueDisplayMode.Off
            };

            readonly UxmlEnumAttributeDescription<RestrictedValuesPolicy> m_RestrictedValues = new UxmlEnumAttributeDescription<RestrictedValuesPolicy>
            {
                name = "restricted-values",
                defaultValue = RestrictedValuesPolicy.None
            };

            readonly UxmlEnumAttributeDescription<TrackDisplayType> m_Track = new UxmlEnumAttributeDescription<TrackDisplayType>
            {
                name = "track",
                defaultValue = TrackDisplayType.Off
            };

            readonly UxmlBoolAttributeDescription m_ShowInputField = new UxmlBoolAttributeDescription
            {
                name = "show-input-field",
                defaultValue = false
            };

            /// <inheritdoc />
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var slider = (Slider<TValue, TScalar, TInputField>)ve;

                slider.showMarks = m_ShowMarks.GetValueFromBag(bag, cc);
                slider.displayValueLabel = m_ValueDisplayMode.GetValueFromBag(bag, cc);
                slider.restrictedValues = m_RestrictedValues.GetValueFromBag(bag, cc);
                slider.track = m_Track.GetValueFromBag(bag, cc);
                slider.showInputField = m_ShowInputField.GetValueFromBag(bag, cc);
            }
        }
#endif
    }

}
