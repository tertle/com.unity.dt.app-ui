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
    /// A slider that allows the user to select a color value.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public sealed partial class ColorSlider : BaseSlider<float,float>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId colorValueProperty = nameof(colorValue);

        internal static readonly BindingId colorRangeProperty = nameof(colorRange);

        internal static readonly BindingId incrementFactorProperty = nameof(incrementFactor);

#endif

        /// <summary>
        /// The ColorSlider main styling class.
        /// </summary>
        public const string ussClassName = "appui-colorslider";

        /// <summary>
        /// The ColorSlider thumb container styling class.
        /// </summary>
        public const string thumbContainerUssClassName = ussClassName + "__thumbcontainer";

        /// <summary>
        /// The ColorSlider thumb container container styling class.
        /// </summary>
        public const string thumbContainerContainerUssClassName = ussClassName + "__thumbcontainer-container";

        /// <summary>
        /// The ColorSlider thumb styling class.
        /// </summary>
        public const string thumbUssClassName = ussClassName + "__thumb";

        /// <summary>
        /// The ColorSlider track styling class.
        /// </summary>
        public const string trackUssClassName = ussClassName + "__track";

        /// <summary>
        /// The ColorSlider track swatch styling class.
        /// </summary>
        public const string trackSwatchUssClassName = ussClassName + "__colorcontainer";

        /// <summary>
        /// The ColorSlider thumb content styling class.
        /// </summary>
        public const string thumbContentUssClassName = ussClassName + "__thumb-content";

        /// <summary>
        /// The ColorSlider size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        readonly ColorSwatch m_TrackSwatch;

        readonly VisualElement m_Track;

        readonly ExVisualElement m_Thumb;

        readonly VisualElement m_ThumbContainer;

        readonly VisualElement m_ThumbContainerContainer;

        readonly VisualElement m_ThumbContent;

        Size m_Size;

        float m_IncrementFactor;

        const float k_DefaultIncrement = 0.01f;

        /// <summary>
        /// The currently selected color value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public Color colorValue => m_ThumbContent.resolvedStyle.backgroundColor;

        /// <summary>
        /// The current color range in the track.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Gradient colorRange
        {
            get => m_TrackSwatch.value;
            set
            {
                var changed = !m_TrackSwatch.value?.Equals(value) ?? value != null;
                m_TrackSwatch.value = value;
                SetValueWithoutNotify(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in colorRangeProperty);
#endif
            }
        }

        /// <summary>
        /// The current size of the ActionButton.
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
        /// The delta value used when interacting with the slider with the keyboard.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float incrementFactor
        {
            get => m_IncrementFactor;
            set
            {
                var changed = !Mathf.Approximately(m_IncrementFactor, value);
                m_IncrementFactor = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in incrementFactorProperty);
#endif
            }
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("low-value")]
        [HideInInspector]
        float lowValueAttributeOverride
        {
            get => lowValue;
            set => lowValue = value;
        }

        [UxmlAttribute("high-value")]
        [HideInInspector]
        float highValueAttributeOverride
        {
            get => highValue;
            set => highValue = value;
        }

        [UxmlAttribute("value")]
        [Range(0, 1)]
        float valueAttributeOverride
        {
            get => value;
            set => this.value = value;
        }
#endif

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColorSlider()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;
            passMask = 0;

            m_Track = new VisualElement { name = trackUssClassName, pickingMode = PickingMode.Ignore };
            m_Track.AddToClassList(trackUssClassName);

            m_TrackSwatch = new ColorSwatch { name = trackSwatchUssClassName, pickingMode = PickingMode.Ignore };
            m_TrackSwatch.AddToClassList(trackSwatchUssClassName);
            var g = new Gradient();
            g.SetKeys(new GradientColorKey[]
            {
                new GradientColorKey(Color.red, 0),
                new GradientColorKey(Color.red, 1),
            }, new GradientAlphaKey[]
            {
                new GradientAlphaKey(0, 0),
                new GradientAlphaKey(1, 1),
            });
            m_TrackSwatch.SetValueWithoutNotify(g);

            m_ThumbContent = new VisualElement
            {
                name = thumbContentUssClassName,
                usageHints = UsageHints.DynamicColor,
                pickingMode = PickingMode.Ignore
            };
            m_ThumbContent.AddToClassList(thumbContentUssClassName);
            m_ThumbContent.style.backgroundColor = Color.clear;

            m_ThumbContainerContainer = new VisualElement
            {
                name = thumbContainerContainerUssClassName,
                pickingMode = PickingMode.Position,
            };
            m_ThumbContainerContainer.AddToClassList(thumbContainerContainerUssClassName);

            m_ThumbContainer = new VisualElement
            {
                name = thumbContainerUssClassName,
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.DynamicTransform,
            };
            m_ThumbContainer.AddToClassList(thumbContainerUssClassName);

            m_Thumb = new ExVisualElement
            {
                name = thumbUssClassName,
                pickingMode = PickingMode.Ignore,
                passMask = Passes.Clear | Passes.Borders | Passes.BackgroundColor | Passes.OutsetShadows
            };
            m_Thumb.AddToClassList(thumbUssClassName);

            hierarchy.Add(m_Track);
            m_Track.hierarchy.Add(m_TrackSwatch);
            hierarchy.Add(m_ThumbContainerContainer);
            m_ThumbContainerContainer.hierarchy.Add(m_ThumbContainer);
            m_ThumbContainer.hierarchy.Add(m_Thumb);
            m_Thumb.hierarchy.Add(m_ThumbContent);

            m_TrackSwatch.StretchToParentSize();

            size = Size.M;
            lowValue = 0;
            highValue = 1f;
            incrementFactor = k_DefaultIncrement;
            SetValueWithoutNotify(0);

            m_DraggerManipulator = new Draggable(OnTrackClicked, OnTrackDragged, OnTrackUp, OnTrackDown)
            {
                dragDirection = Draggable.DragDirection.Horizontal
            };
            m_ThumbContainerContainer.AddManipulator(m_DraggerManipulator);
            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        /// <inheritdoc cref="BaseSlider{T,TU}.InvokeValueChangedCallbacks"/>
        protected override void InvokeValueChangedCallbacks()
        {
            base.InvokeValueChangedCallbacks();

#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in colorValueProperty);
#endif
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            m_Thumb.passMask = Passes.Clear | Passes.Borders | Passes.BackgroundColor | Passes.OutsetShadows;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            m_Thumb.passMask = Passes.Clear | Passes.Borders | Passes.BackgroundColor | Passes.OutsetShadows | Passes.Outline;
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


                    if (!Mathf.Approximately(newValue, previousValue))
                    {
                        SetValueWithoutNotify(newValue);

                        using var changingEvt = ChangingEvent<float>.GetPooled();
                        changingEvt.previousValue = previousValue;
                        changingEvt.newValue = newValue;
                        changingEvt.target = this;
                        SendEvent(changingEvt);
                    }
                }
            }
        }

        /// <inheritdoc cref="BaseSlider{T,TU}.GetSliderRect"/>
        protected override Rect GetSliderRect()
        {
            return m_ThumbContainerContainer.contentRect;
        }

        /// <summary>
        /// Sets the value of the slider without notifying the listeners.
        /// </summary>
        /// <param name="newValue"> The new value of the slider. </param>
        public override void SetValueWithoutNotify(float newValue)
        {
            newValue = Clamp(newValue, lowValue, highValue);

            m_Value = newValue;

            if (validateValue != null) invalid = !validateValue(m_Value);

            RefreshUI();
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.ParseStringToValue"/>
        protected override bool ParseStringToValue(string strValue, out float v)
        {
            return float.TryParse(strValue, out v);
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.SliderLerpUnclamped"/>
        protected override float SliderLerpUnclamped(float a, float b, float t)
        {
            return Mathf.LerpUnclamped(a, b, t);
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.SliderNormalizeValue"/>
        protected override float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
        {
            return Mathf.InverseLerp(lowerValue, higherValue, currentValue);
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.Decrement"/>
        protected override float Decrement(float val)
        {
            return Clamp(val - incrementFactor, lowValue, highValue);
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.Increment"/>
        protected override float Increment(float val)
        {
            return Clamp(val + incrementFactor, lowValue, highValue);
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.Clamp"/>
        protected override float Clamp(float v, float lowBound, float highBound)
        {
            return Mathf.Clamp(v, lowBound, highBound);
        }

        void RefreshUI()
        {
            m_ThumbContent.style.backgroundColor = colorRange.Evaluate(m_Value);
            m_ThumbContainer.style.left = new StyleLength(new Length(m_Value * 100f, LengthUnit.Percent));
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Instantiates an <see cref="ColorSlider"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ColorSlider, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="ColorSlider"/>.
        /// </summary>
        public new class UxmlTraits : BaseSlider<float,float>.UxmlTraits
        {
            readonly UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
            {
                name = "value",
                defaultValue = 0
            };

            readonly UxmlColorAttributeDescription m_From = new UxmlColorAttributeDescription
            {
                name = "from",
                defaultValue = new Color(1, 0, 0, 0)
            };

            readonly UxmlColorAttributeDescription m_To = new UxmlColorAttributeDescription
            {
                name = "to",
                defaultValue = new Color(1, 0, 0, 1)
            };

            readonly UxmlStringAttributeDescription m_ColorRange = new UxmlStringAttributeDescription
            {
                name = "color-range",
                defaultValue = null,
            };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
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

                var el = (ColorSlider)ve;
                el.size = m_Size.GetValueFromBag(bag, cc);
                Color from = Color.black, to = Color.black;
                if (m_From.TryGetValueFromBag(bag, cc, ref from) && m_To.TryGetValueFromBag(bag, cc, ref to))
                {
                    var g = new Gradient();
                    g.SetKeys(new GradientColorKey[]
                    {
                        new GradientColorKey(new Color(from.r, from.g, from.b, 1f), 0),
                        new GradientColorKey(new Color(to.r, to.g, to.b, 1f), 1),
                    }, new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(from.a, 0),
                        new GradientAlphaKey(to.a, 1),
                    });
                    el.colorRange = g;
                }
                el.SetValueWithoutNotify(m_Value.GetValueFromBag(bag, cc));
                var colorRange = m_ColorRange.GetValueFromBag(bag, cc);
                if (!string.IsNullOrEmpty(colorRange) && GradientExtensions.TryParse(colorRange, out var gradient))
                    el.colorRange = gradient;
            }
        }
#endif
    }
}
