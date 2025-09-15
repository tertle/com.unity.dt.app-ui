using System;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A color wheel that allows the user to select a color hue by rotating the wheel.
    /// It is also possible to set the saturation and brightness and opacity of the wheel.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class ColorWheel : BaseVisualElement, IInputElement<float>, INotifyValueChanging<float>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valuePropertyKey = new BindingId(nameof(value));

        internal static readonly BindingId incrementFactorProperty = new BindingId(nameof(incrementFactor));

        internal static readonly BindingId opacityPropertyKey = new BindingId(nameof(opacity));

        internal static readonly BindingId brightnessPropertyKey = new BindingId(nameof(brightness));

        internal static readonly BindingId saturationPropertyKey = new BindingId(nameof(saturation));

        internal static readonly BindingId innerRadiusPropertyKey = new BindingId(nameof(innerRadius));

        internal static readonly BindingId checkerSizePropertyKey = new BindingId(nameof(checkerSize));

        internal static readonly BindingId checkerColor1PropertyKey = new BindingId(nameof(checkerColor1));

        internal static readonly BindingId checkerColor2PropertyKey = new BindingId(nameof(checkerColor2));

        internal static readonly BindingId selectedColorPropertyKey = new BindingId(nameof(selectedColor));

        internal static readonly BindingId invalidProperty = new BindingId(nameof(invalid));

        internal static readonly BindingId validateValueProperty = new BindingId(nameof(validateValue));

#endif

        const float k_InvTwoPI = 0.15915494309f;

        const float k_TwoPI = 6.28318530718f;

        /// <summary>
        /// The ColorWheel main styling class.
        /// </summary>
        public const string ussClassName = "appui-colorwheel";

        /// <summary>
        /// The ColorWheel image styling class.
        /// </summary>
        public const string imageUssClassName = ussClassName + "__image";

        /// <summary>
        /// The ColorWheel thumb styling class.
        /// </summary>
        public const string thumbUssClassName = ussClassName + "__thumb";

        /// <summary>
        /// The ColorWheel thumb swatch styling class.
        /// </summary>
        public const string thumbSwatchUssClassName = ussClassName + "__thumbswatch";

        static readonly CustomStyleProperty<Color> k_UssCheckerColor1 = new CustomStyleProperty<Color>("--checker-color-1");

        static readonly CustomStyleProperty<Color> k_UssCheckerColor2 = new CustomStyleProperty<Color>("--checker-color-2");

        static readonly CustomStyleProperty<int> k_UssCheckerSize = new CustomStyleProperty<int>("--checker-size");

        static readonly CustomStyleProperty<float> k_UssOpacity = new CustomStyleProperty<float>("--opacity");

        static readonly CustomStyleProperty<float> k_UssBrightness = new CustomStyleProperty<float>("--brightness");

        static readonly CustomStyleProperty<float> k_UssSaturation = new CustomStyleProperty<float>("--saturation");

        static readonly CustomStyleProperty<float> k_UssInnerRadius = new CustomStyleProperty<float>("--inner-radius");

        static readonly int k_CheckerColor1 = Shader.PropertyToID("_CheckerColor1");

        static readonly int k_CheckerColor2 = Shader.PropertyToID("_CheckerColor2");

        static readonly int k_CheckerSize = Shader.PropertyToID("_CheckerSize");

        static readonly int k_Width = Shader.PropertyToID("_Width");

        static readonly int k_Height = Shader.PropertyToID("_Height");

        static readonly int k_InnerRadius = Shader.PropertyToID("_InnerRadius");

        static readonly int k_Saturation = Shader.PropertyToID("_Saturation");

        static readonly int k_Brightness = Shader.PropertyToID("_Brightness");

        static readonly int k_Opacity = Shader.PropertyToID("_Opacity");

        static readonly int k_AA = Shader.PropertyToID("_AA");

        readonly Image m_Image;

        Color m_CheckerColor1FromStyle;

        Optional<Color> m_CheckerColor1FromCode;

        Color m_CheckerColor2FromStyle;

        Optional<Color> m_CheckerColor2FromCode;

        int m_CheckerSizeFromStyle = k_DefaultCheckerSize;

        Optional<int> m_CheckerSizeFromCode;

        RenderTexture m_RT;

        Vector2 m_PreviousSize;

        float m_Value;

        static Material s_Material;

        float m_OpacityFromStyle = k_DefaultOpacity;

        Optional<float> m_OpacityFromCode;

        float m_BrightnessFromStyle = k_DefaultBrightness;

        Optional<float> m_BrightnessFromCode;

        float m_SaturationFromStyle = k_DefaultSaturation;

        Optional<float> m_SaturationFromCode;

        float m_InnerRadiusFromStyle = k_DefaultInnerRadius;

        Optional<float> m_InnerRadiusFromCode;

        readonly Draggable m_DraggerManipulator;

        readonly ExVisualElement m_Thumb;

        float m_PreviousValue;

        readonly VisualElement m_ThumbSwatch;

        float m_IncrementFactor = k_DefaultIncrementFactor;

        const float k_DefaultIncrementFactor = 0.01f;

        const float k_DefaultInnerRadius = 0.4f;

        const int k_DefaultCheckerSize = 4;

        const float k_DefaultOpacity = 1f;

        const float k_DefaultBrightness = 1f;

        const float k_DefaultSaturation = 1f;

        Func<float, bool> m_ValidateValue;

        /// <summary>
        /// The hue value of the color wheel.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float value
        {
            get => m_Value;
            set
            {
                var validValue = Mathf.Repeat(value, 1f);
                if (Mathf.Approximately(m_Value, validValue))
                    return;
                using var evt = ChangeEvent<float>.GetPooled(m_Value, validValue);
                SetValueWithoutNotify(validValue);
                evt.target = this;
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valuePropertyKey);
                NotifyPropertyChanged(in selectedColorPropertyKey);
#endif
            }
        }

        /// <summary>
        /// The opacity of the color wheel. Note that a checkerboard pattern is always drawn behind the color wheel.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Range(0,1)]
#endif
        public float opacity
        {
            get => m_OpacityFromCode.IsSet ? m_OpacityFromCode.Value : m_OpacityFromStyle;
            set
            {
                var validatedValue = Mathf.Clamp01(value);
                var changed = !Mathf.Approximately(validatedValue, opacity);
                m_OpacityFromCode = validatedValue;

                if (changed)
                    GenerateTextures();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in opacityPropertyKey);
#endif
            }
        }

        /// <summary>
        /// The brightness of the color wheel.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Range(0,1)]
#endif
        public float brightness
        {
            get => m_BrightnessFromCode.IsSet ? m_BrightnessFromCode.Value : m_BrightnessFromStyle;
            set
            {
                var validatedValue = Mathf.Clamp01(value);
                var changed = !Mathf.Approximately(validatedValue, brightness);
                m_BrightnessFromCode = validatedValue;

                if (changed)
                    GenerateTextures();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in brightnessPropertyKey);
                    NotifyPropertyChanged(in selectedColorPropertyKey);
                }
#endif
            }
        }

        /// <summary>
        /// The saturation of the color wheel.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Range(0,1)]
#endif
        public float saturation
        {
            get => m_SaturationFromCode.IsSet ? m_SaturationFromCode.Value : m_SaturationFromStyle;
            set
            {
                var validatedValue = Mathf.Clamp01(value);
                var changed = !Mathf.Approximately(validatedValue, saturation);
                m_SaturationFromCode = validatedValue;

                if (changed)
                    GenerateTextures();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in saturationPropertyKey);
                    NotifyPropertyChanged(in selectedColorPropertyKey);
                }
#endif
            }
        }

        /// <summary>
        /// The inner radius of the color wheel.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Range(0,0.49999f)]
#endif
        public float innerRadius
        {
            get => m_InnerRadiusFromCode.IsSet ? m_InnerRadiusFromCode.Value : m_InnerRadiusFromStyle;
            set
            {
                var validatedValue = Mathf.Clamp(value, 0, 0.5f - Mathf.Epsilon);
                var changed = !Mathf.Approximately(validatedValue, innerRadius);
                m_InnerRadiusFromCode = value;

                if (changed)
                    GenerateTextures();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in innerRadiusPropertyKey);
#endif
            }
        }

        /// <summary>
        /// The size of the checkerboard pattern.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Min(0)]
#endif
        public int checkerSize
        {
            get => m_CheckerSizeFromCode.IsSet ? m_CheckerSizeFromCode.Value : m_CheckerSizeFromStyle;
            set
            {
                var changed = checkerSize != value;
                m_CheckerSizeFromCode = value;

                if (changed)
                    GenerateTextures();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in checkerSizePropertyKey);
#endif
            }
        }

        /// <summary>
        /// The first color of the checkerboard pattern.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Color checkerColor1
        {
            get => m_CheckerColor1FromCode.IsSet ? m_CheckerColor1FromCode.Value : m_CheckerColor1FromStyle;
            set
            {
                var changed = checkerColor1 != value;
                m_CheckerColor1FromCode = value;

                if (changed)
                    GenerateTextures();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in checkerColor1PropertyKey);
#endif
            }
        }

        /// <summary>
        /// The second color of the checkerboard pattern.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Color checkerColor2
        {
            get => m_CheckerColor2FromCode.IsSet ? m_CheckerColor2FromCode.Value : m_CheckerColor2FromStyle;
            set
            {
                var changed = checkerColor2 != value;
                m_CheckerColor2FromCode = value;

                if (changed)
                    GenerateTextures();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in checkerColor2PropertyKey);
#endif
            }
        }

        /// <summary>
        /// The currently selected color.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public Color selectedColor => Color.HSVToRGB(value, saturation, brightness);

        /// <summary>
        /// The factor by which the value is incremented when interacting with the wheel from the keyboard.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Min(0.00001f)]
#endif
        public float incrementFactor
        {
            get => m_IncrementFactor;
            set
            {
                if (Mathf.Approximately(m_IncrementFactor, value))
                    return;

                m_IncrementFactor = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in incrementFactorProperty);
#endif
            }
        }

        /// <summary>
        /// The ColorWheel invalid state.
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
                var changed = invalid != value;
                EnableInClassList(Styles.invalidUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in invalidProperty);
#endif
            }
        }

        /// <summary>
        /// The ColorWheel validation function.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<float, bool> validateValue
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
        /// Default constructor.
        /// </summary>
        public ColorWheel()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Position;
            focusable = true;

            m_Image = new Image { name = imageUssClassName, pickingMode = PickingMode.Ignore };
            m_Image.AddToClassList(imageUssClassName);
            hierarchy.Add(m_Image);

            m_Thumb = new ExVisualElement
            {
                name = thumbUssClassName,
                pickingMode = PickingMode.Position,
                passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.OutsetShadows
            };
            m_Thumb.EnableDynamicTransform(true);
            m_Thumb.AddToClassList(thumbUssClassName);
            hierarchy.Add(m_Thumb);

            m_ThumbSwatch = new VisualElement
            {
                name = thumbSwatchUssClassName,
                pickingMode = PickingMode.Ignore,
                usageHints = UsageHints.DynamicColor,
            };
            m_ThumbSwatch.AddToClassList(thumbSwatchUssClassName);
            m_Thumb.Add(m_ThumbSwatch);

            m_DraggerManipulator = new Draggable(OnTrackClicked, OnTrackDragged, OnTrackUp, OnTrackDown);
            this.AddManipulator(m_DraggerManipulator);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
            RegisterCallback<CustomStyleResolvedEvent>(OnStylesResolved);
            RegisterCallback<KeyDownEvent>(OnKeyDown);

            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));

            incrementFactor = k_DefaultIncrementFactor;

            SetValueWithoutNotify(0);
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            m_Thumb.passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.Borders | ExVisualElement.Passes.BackgroundColor | ExVisualElement.Passes.OutsetShadows;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            m_Thumb.passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.Borders | ExVisualElement.Passes.BackgroundColor | ExVisualElement.Passes.OutsetShadows | ExVisualElement.Passes.Outline;
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            var handled = false;
            var previousValue = value;
            var val = previousValue;

            if (evt.keyCode is KeyCode.LeftArrow or KeyCode.DownArrow)
            {
                val -= incrementFactor;
                handled = true;
            }

            if (evt.keyCode is KeyCode.RightArrow or KeyCode.UpArrow)
            {
                val += incrementFactor;
                handled = true;
            }

            if (handled)
            {

                evt.StopPropagation();

                val = Mathf.Repeat(val, 1f);
                SetValueWithoutNotify(val);

                using var changingEvt = ChangingEvent<float>.GetPooled();
                changingEvt.previousValue = previousValue;
                changingEvt.newValue = val;
                changingEvt.target = this;
                SendEvent(changingEvt);
            }
        }

        void OnTrackClicked()
        {
            if (!m_DraggerManipulator.hasMoved)
            {
                OnTrackDragged(m_DraggerManipulator);
                OnTrackUp(m_DraggerManipulator);
            }
        }

        void OnTrackDown(Draggable obj)
        {
            m_PreviousValue = value;
        }

        void OnTrackUp(Draggable obj)
        {
            if (Mathf.Approximately(value, m_PreviousValue))
                return;

            using var evt = ChangeEvent<float>.GetPooled(m_PreviousValue, value);
            evt.target = this;
            SendEvent(evt);

        }

        void OnTrackDragged(Draggable obj)
        {
            var val = ComputeValue(m_DraggerManipulator.localPosition);
            val = Mathf.Repeat(val, 1f);
            SetValueWithoutNotify(val);

            using var evt = ChangingEvent<float>.GetPooled();
            evt.previousValue = m_PreviousValue;
            evt.newValue = val;
            evt.target = this;
            SendEvent(evt);
        }

        void OnStylesResolved(CustomStyleResolvedEvent evt)
        {
            var changed = false;

            if (evt.customStyle.TryGetValue(k_UssCheckerColor1, out var ussCheckerColor1))
            {
                m_CheckerColor1FromStyle = ussCheckerColor1;
                changed = true;
            }

            if (evt.customStyle.TryGetValue(k_UssCheckerColor2, out var ussCheckerColor2))
            {
                m_CheckerColor2FromStyle = ussCheckerColor2;
                changed = true;
            }

            if (evt.customStyle.TryGetValue(k_UssCheckerSize, out var ussCheckerSize))
            {
                m_CheckerSizeFromStyle = ussCheckerSize;
                changed = true;
            }

            if (evt.customStyle.TryGetValue(k_UssOpacity, out var ussOpacity))
            {
                m_OpacityFromStyle = ussOpacity;
                changed = true;
            }

            if (evt.customStyle.TryGetValue(k_UssBrightness, out var ussBrightness))
            {
                m_BrightnessFromStyle = ussBrightness;
                changed = true;
            }

            if (evt.customStyle.TryGetValue(k_UssSaturation, out var ussSaturation))
            {
                m_SaturationFromStyle = ussSaturation;
                changed = true;
            }

            if (evt.customStyle.TryGetValue(k_UssInnerRadius, out var ussInnerRadius))
            {
                m_InnerRadiusFromStyle = ussInnerRadius;
                changed = true;
            }

            if (changed)
            {
                GenerateTextures();
                SetValueWithoutNotify(value);
            }
        }

        void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            ReleaseTextures();
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            var isNullSize =
                paddingRect.width <= Mathf.Epsilon ||
                paddingRect.height <= Mathf.Epsilon;

            var isSameSize =
                Mathf.Approximately(paddingRect.width, m_PreviousSize.x) &&
                Mathf.Approximately(paddingRect.height, m_PreviousSize.y);

            m_PreviousSize.x = paddingRect.width;
            m_PreviousSize.y = paddingRect.height;

            if (!isNullSize && !isSameSize)
            {
                GenerateTextures();
            }

            SetValueWithoutNotify(value);
        }

        /// <summary>
        /// Sets the value without sending any event.
        /// </summary>
        /// <param name="newValue"> The new value to set. </param>
        public void SetValueWithoutNotify(float newValue)
        {
            m_Value = newValue;

            var rect = m_Image.contentRect;

            if (!rect.IsValid())
                return;

            var center = rect.size * 0.5f;
            var refPoint = new Vector2(1, 0);
            var targetAngle = Mathf.Atan2(refPoint.y, refPoint.x) - m_Value * k_TwoPI;
            const float maxRadius = 0.5f;
            var selectorSize = m_Thumb.resolvedStyle.width;
            var radius = Mathf.Min(
                rect.width * ((maxRadius - innerRadius) * 0.5f + innerRadius) - 0.65f,
                rect.height * ((maxRadius - innerRadius) * 0.5f + innerRadius) - 0.65f);
            var x = center.x + radius * Mathf.Cos(targetAngle);
            var y = center.y + radius * Mathf.Sin(targetAngle);
            m_Thumb.style.top = y - selectorSize * 0.5f;
            m_Thumb.style.left = x - selectorSize * 0.5f;
            m_ThumbSwatch.style.backgroundColor = selectedColor;

            if (m_ValidateValue != null)
                invalid = !m_ValidateValue(newValue);
        }

        void GenerateTextures()
        {
            if (!s_Material)
            {
                s_Material = MaterialUtils.CreateMaterial("Hidden/App UI/ColorWheel");
                if (!s_Material)
                {
                    ReleaseTextures();
                    return;
                }
            }

            var rect = m_Image.contentRect;

            if (!rect.IsValid())
            {
                ReleaseTextures();
                return;
            }

            var dpi = Mathf.Max(Platform.scaleFactor, 1f);
            var texSize = rect.size * dpi;

            if (!texSize.IsValidForTextureSize())
            {
                ReleaseTextures();
                return;
            }

            if (m_RT && (Mathf.Abs(m_RT.width - texSize.x) > 1 || Mathf.Abs(m_RT.height - texSize.y) > 1))
                ReleaseTextures();

            if (!m_RT)
                m_RT = RenderTexture.GetTemporary((int)texSize.x, (int)texSize.y, 24);

            s_Material.SetColor(k_CheckerColor1, checkerColor1);
            s_Material.SetColor(k_CheckerColor2, checkerColor2);
            s_Material.SetFloat(k_CheckerSize, checkerSize);
            s_Material.SetFloat(k_Width, rect.width);
            s_Material.SetFloat(k_Height, rect.height);
            s_Material.SetFloat(k_AA, 2.0f / texSize.x);

            s_Material.SetFloat(k_InnerRadius, innerRadius);
            s_Material.SetFloat(k_Saturation, saturation);
            s_Material.SetFloat(k_Brightness, brightness);
            s_Material.SetFloat(k_Opacity, opacity);

            var prevRt = RenderTexture.active;
            Graphics.Blit(null, m_RT, s_Material);
            RenderTexture.active = prevRt;

            if (m_Image.image != m_RT)
                m_Image.image = m_RT;
            m_Image.MarkDirtyRepaint();
        }

        float ComputeValue(Vector2 localPosition)
        {
            var center = new Vector2(paddingRect.width * 0.5f, paddingRect.height * 0.5f);
            var direction = (localPosition - center).normalized;

            // simplified since atan of red color is 0.
            // var refPoint = new Vector2(1, 0); // angle = 0 = red = position(1,0) in wheel
            return (/*Mathf.Atan2(refPoint.y, refPoint.x)*/ -Mathf.Atan2(direction.y, direction.x)) * k_InvTwoPI;
        }

        void ReleaseTextures()
        {
            if (m_RT)
                RenderTexture.ReleaseTemporary(m_RT);
            m_RT = null;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Instantiates an <see cref="ColorWheel"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ColorWheel, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="ColorWheel"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
            {
                name = "value",
                defaultValue = 0
            };

            readonly UxmlFloatAttributeDescription m_IncrementFactor = new UxmlFloatAttributeDescription
            {
                name = "increment-factor",
                defaultValue = k_DefaultIncrementFactor
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

                var el = (ColorWheel)ve;
                el.SetValueWithoutNotify(m_Value.GetValueFromBag(bag, cc));

                var incrementFactor = k_DefaultIncrementFactor;
                if (m_IncrementFactor.TryGetValueFromBag(bag, cc, ref incrementFactor))
                    el.incrementFactor = incrementFactor;

            }
        }
#endif
    }
}
