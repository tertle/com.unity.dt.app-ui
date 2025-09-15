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
    /// A color swatch is a visual element that displays a color or a gradient.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class ColorSwatch : BaseVisualElement, INotifyValueChanged<Gradient>, ISizeableElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId colorProperty = nameof(color);

        internal static readonly BindingId roundProperty = nameof(round);

        internal static readonly BindingId orientationProperty = nameof(orientation);

#endif

        const int k_MaxGradientSteps = 16;

        /// <summary>
        /// The ColorSwatch main styling class.
        /// </summary>
        public const string ussClassName = "appui-colorswatch";

        /// <summary>
        /// The ColorSwatch image styling class.
        /// </summary>
        public const string imageUssClassName = ussClassName + "__image";

        /// <summary>
        /// The ColorSwatch size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The ColorSwatch variant styling class.
        /// </summary>
        [EnumName("GetOrientationUssClassName", typeof(Direction))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The ColorSwatch round styling class.
        /// </summary>
        public const string roundUssClassName = ussClassName + "--round";

        static readonly CustomStyleProperty<Color> k_UssCheckerColor1 = new CustomStyleProperty<Color>("--checker-color-1");

        static readonly CustomStyleProperty<Color> k_UssCheckerColor2 = new CustomStyleProperty<Color>("--checker-color-2");

        static readonly CustomStyleProperty<int> k_UssCheckerSize = new CustomStyleProperty<int>("--checker-size");

        Gradient m_Value;

        static Material s_Material;

        Vector2 m_PreviousSize = Vector2.zero;

        readonly Image m_Image;

        RenderTexture m_RT;

        Color m_CheckerColor1;

        Color m_CheckerColor2;

        int m_CheckerSize;

        static readonly int k_CheckerColor1 = Shader.PropertyToID("_CheckerColor1");

        static readonly int k_CheckerColor2 = Shader.PropertyToID("_CheckerColor2");

        static readonly int k_CheckerSize = Shader.PropertyToID("_CheckerSize");

        static readonly int k_Width = Shader.PropertyToID("_Width");

        static readonly int k_Height = Shader.PropertyToID("_Height");

        static readonly int k_IsFixed = Shader.PropertyToID("_IsFixed");

        static readonly int k_Orientation = Shader.PropertyToID("_Orientation");

        static readonly int k_ColorCount = Shader.PropertyToID("_ColorCount");

        static readonly int k_AlphaCount = Shader.PropertyToID("_AlphaCount");

        static readonly int k_Colors = Shader.PropertyToID("_Colors");

        static readonly int k_Alphas = Shader.PropertyToID("_Alphas");

        static readonly Color[] k_ColorsVector = new Color[k_MaxGradientSteps];

        static readonly Vector4[] k_AlphasVectors = new Vector4[k_MaxGradientSteps];

        static readonly GradientColorKey[] k_DefaultColorKeys = new[]
        {
            new GradientColorKey(Color.black, 0),
        };

        static readonly GradientAlphaKey[] k_DefaultAlphaKeys = new[]
        {
            new GradientAlphaKey(1, 0),
        };

        Size m_Size;

        Direction m_Orientation;

        /// <summary>
        /// The color entry list.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Gradient value
        {
            get => m_Value;
            set
            {
                if (m_Value == null && value == null)
                    return;

                using var evt = ChangeEvent<Gradient>.GetPooled(m_Value, value);
                SetValueWithoutNotify(value);
                evt.target = this;
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
                NotifyPropertyChanged(in colorProperty);
#endif
            }
        }

        /// <summary>
        /// The single color of the <see cref="ColorSwatch"/>.
        /// Setting this property will overwrite the current gradient value to contain only the given single color value.
        /// The property's getter always return the first item of the gradient.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Color color
        {
            get => value?.Evaluate(0) ?? default;
            set
            {
                var g = this.value ?? new Gradient();
                g.SetKeys(new[]
                {
                    new GradientColorKey(value, 0)
                }, new[]
                {
                    new GradientAlphaKey(value.a, 0)
                });
                this.value = g;

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in colorProperty);
#endif
            }
        }

        /// <summary>
        /// The size of the <see cref="ColorSwatch"/> element.
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
                if (m_Size == value)
                    return;
                SetSize(value);
            }
        }

        void SetSize(Size newSize)
        {
            RemoveFromClassList(GetSizeUssClassName(m_Size));
            m_Size = newSize;
            AddToClassList(GetSizeUssClassName(m_Size));
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in sizeProperty);
#endif
        }

        /// <summary>
        /// Round variant of the <see cref="ColorSwatch"/>.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool round
        {
            get => ClassListContains(roundUssClassName);
            set
            {
                if (round == value)
                    return;
                SetRound(value);
            }
        }

        void SetRound(bool newRound)
        {
            EnableInClassList(roundUssClassName, newRound);
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in roundProperty);
#endif
        }

        /// <summary>
        /// The orientation of the <see cref="ColorSwatch"/>.
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
                 if (m_Orientation == value)
                    return;
                 SetOrientation(value);
            }
        }

        void SetOrientation(Direction newOrientation)
        {
            RemoveFromClassList(GetOrientationUssClassName(m_Orientation));
            m_Orientation = newOrientation;
            AddToClassList(GetOrientationUssClassName(m_Orientation));
            Refresh();
#if ENABLE_RUNTIME_DATA_BINDINGS
            NotifyPropertyChanged(in orientationProperty);
#endif
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ColorSwatch()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Position;

            m_Image = new Image { name = imageUssClassName, pickingMode = PickingMode.Ignore };
            m_Image.AddToClassList(imageUssClassName);
            hierarchy.Add(m_Image);
            m_Image.StretchToParentSize();

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            RegisterCallback<CustomStyleResolvedEvent>(OnStylesResolved);

            SetValueWithoutNotify(null);

            SetSize(Size.M);
            SetRound(false);
            SetOrientation(Direction.Horizontal);
        }

        /// <summary>
        /// Force the refresh of the visual element.
        /// </summary>
        public void Refresh()
        {
            GenerateTextures();
        }

        /// <summary>
        /// Set the color entry list value, without being notified of any changes.
        /// </summary>
        /// <param name="newValue">The new color entry list.</param>
        public void SetValueWithoutNotify(Gradient newValue)
        {
            m_Value = newValue;
            GenerateTextures();
        }

        void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            GenerateTextures();
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

        void OnStylesResolved(CustomStyleResolvedEvent evt)
        {
            if (evt.customStyle.TryGetValue(k_UssCheckerColor1, out var checkerColor1))
                m_CheckerColor1 = checkerColor1;

            if (evt.customStyle.TryGetValue(k_UssCheckerColor2, out var checkerColor2))
                m_CheckerColor2 = checkerColor2;

            if (evt.customStyle.TryGetValue(k_UssCheckerSize, out var checkerSize))
                m_CheckerSize = checkerSize;

            GenerateTextures();
        }

        void GenerateTextures()
        {
            if (!s_Material)
            {
                s_Material = MaterialUtils.CreateMaterial("Hidden/App UI/ColorSwatch");
                if (!s_Material)
                {
                    ReleaseTextures();
                    return;
                }
            }

            var rect = paddingRect;

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

            var colorCount = Mathf.Min(m_Value?.colorKeys?.Length ?? k_DefaultColorKeys.Length, k_MaxGradientSteps);
            var alphaCount = Mathf.Min(m_Value?.alphaKeys?.Length ?? k_DefaultAlphaKeys.Length, k_MaxGradientSteps);

            var colorKeys = m_Value?.colorKeys ?? k_DefaultColorKeys;
            for (var i = 0; i < colorCount; i++)
            {
                k_ColorsVector[i] = new Color(
                    colorKeys[i].color.r,
                    colorKeys[i].color.g,
                    colorKeys[i].color.b,
                    colorKeys[i].time);
            }

            var alphaKeys = m_Value?.alphaKeys ?? k_DefaultAlphaKeys;
            for (var i = 0; i < alphaCount; i++)
            {
                k_AlphasVectors[i] = new Vector4(
                    alphaKeys[i].alpha,
                    alphaKeys[i].time,
                    0,
                    0);
            }

            s_Material.SetInt(k_Orientation, (int)m_Orientation);
            s_Material.SetInt(k_ColorCount, colorCount);
            s_Material.SetInt(k_AlphaCount, alphaCount);
            s_Material.SetColorArray(k_Colors, k_ColorsVector);
            s_Material.SetVectorArray(k_Alphas, k_AlphasVectors);
            s_Material.SetColor(k_CheckerColor1, m_CheckerColor1);
            s_Material.SetColor(k_CheckerColor2, m_CheckerColor2);
            s_Material.SetFloat(k_CheckerSize, m_CheckerSize);
            s_Material.SetFloat(k_Width, rect.width);
            s_Material.SetFloat(k_Height, rect.height);
            s_Material.SetInt(k_IsFixed, m_Value?.mode == GradientMode.Fixed ? 1 : 0);

            var prevRt = RenderTexture.active;
            Graphics.Blit(null, m_RT, s_Material, 0);
            if (m_Value != null)
                Graphics.Blit(null, m_RT, s_Material, 1);
            RenderTexture.active = prevRt;

            m_Image.image = null;
            m_Image.image = m_RT;
        }

        void ReleaseTextures()
        {
            if (m_RT)
                RenderTexture.ReleaseTemporary(m_RT);
            m_RT = null;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Instantiates an <see cref="ColorSwatch"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ColorSwatch, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="ColorSwatch"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription
            {
                name = "value",
                defaultValue = null,
            };

            readonly UxmlBoolAttributeDescription m_Round = new UxmlBoolAttributeDescription
            {
                name = "round",
                defaultValue = false,
            };

            readonly UxmlEnumAttributeDescription<Direction> m_Orientation = new UxmlEnumAttributeDescription<Direction>
            {
                name = "orientation",
                defaultValue = Direction.Horizontal,
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
                var element = (ColorSwatch)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.round = m_Round.GetValueFromBag(bag, cc);
                element.orientation = m_Orientation.GetValueFromBag(bag, cc);

                var valueFromBag = m_Value.GetValueFromBag(bag, cc);
                if (!string.IsNullOrEmpty(valueFromBag) && GradientExtensions.TryParse(valueFromBag, out var gradient))
                    element.value = gradient;
            }
        }
#endif
    }
}
