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
    /// A visual element that can be used to mask color.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Mask : Image
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId innerMaskColorProperty = nameof(innerMaskColor);

        internal static readonly BindingId outerMaskColorProperty = nameof(outerMaskColor);

        internal static readonly BindingId maskRectProperty = nameof(maskRect);

        internal static readonly BindingId radiusProperty = nameof(radius);

        internal static readonly BindingId blurProperty = nameof(blur);

        internal static readonly BindingId useNormalizedMaskRectProperty = nameof(useNormalizedMaskRect);

#endif

        /// <summary>
        /// The Mask main styling class.
        /// </summary>
        public new const string ussClassName = "appui-mask";

        /// <summary>
        /// The content container of this element.
        /// </summary>
        public override VisualElement contentContainer => null;

        RenderTexture m_RT;

        static Material s_Material;

        static readonly int k_MaskRect = Shader.PropertyToID("_MaskRect");

        static readonly int k_Radius = Shader.PropertyToID("_Radius");

        static readonly int k_InnerMaskColor = Shader.PropertyToID("_InnerMaskColor");

        static readonly int k_OuterMaskColor = Shader.PropertyToID("_OuterMaskColor");

        static readonly int k_Ratio = Shader.PropertyToID("_Ratio");

        static readonly int k_Sigma = Shader.PropertyToID("_Sigma");

        Vector2 m_PreviousSize;

        Color m_InnerMaskColor = Color.black;

        Color m_OuterMaskColor = Color.clear;

        Rect m_MaskRect = new Rect(100f, 100f, 100f, 40f);

        Rect m_PreviousMaskRect;

        float m_Radius = 0;

        float m_Blur = 0;

        bool m_UseNormalizedMaskRect;

        static readonly Color k_DefaultInnerMaskColor = Color.white;

        static readonly Color k_DefaultOuterMaskColor = Color.black;

        static readonly Rect k_DefaultMaskRect = new Rect(20f, 20f, 20f, 20f);

        const float k_DefaultRadius = 0f;

        const float k_DefaultBlur = 0f;

        const bool k_DefaultUseNormalizedMaskRect = false;

        /// <summary>
        /// The inner mask color. Sets the color of the inner mask.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Color innerMaskColor
        {
            get => m_InnerMaskColor;
            set
            {
                var changed = m_InnerMaskColor != value;
                m_InnerMaskColor = value;
                GenerateTextures();
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in innerMaskColorProperty);
#endif
            }
        }

        /// <summary>
        /// The outer mask color. The color of the area outside the mask.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Color outerMaskColor
        {
            get => m_OuterMaskColor;
            set
            {
                var changed = m_OuterMaskColor != value;
                m_OuterMaskColor = value;
                GenerateTextures();
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in outerMaskColorProperty);
#endif
            }
        }

        /// <summary>
        /// The mask rect. Sets the rect of the mask (in pixels or normalized if <see cref="useNormalizedMaskRect"/> is true).
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Rect maskRect
        {
            get => m_MaskRect;
            set
            {
                var changed = m_MaskRect != value;
                m_MaskRect = value;
                GenerateTextures();
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in maskRectProperty);
#endif
            }
        }

        /// <summary>
        /// The mask radius. Sets the radius of the rounded corners (in pixels).
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float radius
        {
            get => m_Radius;
            set
            {
                var changed = !Mathf.Approximately(m_Radius, value);
                m_Radius = value;
                GenerateTextures();
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in radiusProperty);
#endif
            }
        }

        /// <summary>
        /// The mask blur. Sets the blur of the mask (in pixels).
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float blur
        {
            get => m_Blur;
            set
            {
                var changed = !Mathf.Approximately(m_Blur, value);
                m_Blur = value;
                GenerateTextures();
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in blurProperty);
#endif
            }
        }

        /// <summary>
        /// If true, the mask rect you will provide through <see cref="maskRect"/> must be normalized (0-1) instead of using pixels coordinates.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool useNormalizedMaskRect
        {
            get => m_UseNormalizedMaskRect;
            set
            {
                var changed = m_UseNormalizedMaskRect != value;
                m_UseNormalizedMaskRect = value;
                GenerateTextures();
                MarkDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in useNormalizedMaskRectProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Mask()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Ignore;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);

            innerMaskColor = k_DefaultInnerMaskColor;
            outerMaskColor = k_DefaultOuterMaskColor;
            radius = k_DefaultRadius;
            blur = k_DefaultBlur;
            maskRect = k_DefaultMaskRect;
            useNormalizedMaskRect = k_DefaultUseNormalizedMaskRect;
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
                MarkDirtyRepaint();
            }
        }

        void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            ReleaseTextures();
        }

        void GenerateTextures()
        {
            if (!s_Material)
            {
                s_Material = MaterialUtils.CreateMaterial("Hidden/App UI/Mask");
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

            s_Material.SetColor(k_InnerMaskColor, innerMaskColor);
            s_Material.SetColor(k_OuterMaskColor, outerMaskColor);

            var ratio = rect.width / rect.height;
            var maskRect =
                useNormalizedMaskRect ? new Vector4(m_MaskRect.x, m_MaskRect.y / ratio, m_MaskRect.width, m_MaskRect.height / ratio) :
                new Vector4(m_MaskRect.x / rect.width, (m_MaskRect.y / rect.height) / ratio,
                m_MaskRect.width / rect.width, (m_MaskRect.height / rect.height) / ratio);
            s_Material.SetVector(k_MaskRect, maskRect);
            s_Material.SetFloat(k_Ratio, ratio);
            s_Material.SetFloat(k_Radius, m_Radius / rect.width);
            s_Material.SetFloat(k_Sigma, m_Blur / rect.width);

            var prevRt = RenderTexture.active;
            Graphics.Blit(null, m_RT, s_Material);
            RenderTexture.active = prevRt;

            if (image != m_RT)
                image = m_RT;
        }

        void ReleaseTextures()
        {
            if (m_RT)
            {
                RenderTexture.ReleaseTemporary(m_RT);
                m_RT = null;
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Mask"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Mask, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="ExVisualElement"/>.
        /// </summary>
        public new class UxmlTraits : Image.UxmlTraits
        {
            readonly UxmlColorAttributeDescription m_InnerMaskColor = new UxmlColorAttributeDescription
            {
                name = "inner-mask-color",
                defaultValue = k_DefaultInnerMaskColor
            };

            readonly UxmlColorAttributeDescription m_OuterMaskColor = new UxmlColorAttributeDescription
            {
                name = "outer-mask-color",
                defaultValue = k_DefaultOuterMaskColor
            };

            readonly UxmlFloatAttributeDescription m_Radius = new UxmlFloatAttributeDescription
            {
                name = "radius",
                defaultValue = k_DefaultRadius
            };

            readonly UxmlFloatAttributeDescription m_Blur = new UxmlFloatAttributeDescription
            {
                name = "blur",
                defaultValue = k_DefaultBlur
            };

            readonly UxmlFloatAttributeDescription m_MaskRectX = new UxmlFloatAttributeDescription
            {
                name = "mask-rect-x",
                defaultValue = k_DefaultMaskRect.x
            };

            readonly UxmlFloatAttributeDescription m_MaskRectY = new UxmlFloatAttributeDescription
            {
                name = "mask-rect-y",
                defaultValue = k_DefaultMaskRect.y
            };

            readonly UxmlFloatAttributeDescription m_MaskRectWidth = new UxmlFloatAttributeDescription
            {
                name = "mask-rect-width",
                defaultValue = k_DefaultMaskRect.width
            };

            readonly UxmlFloatAttributeDescription m_MaskRectHeight = new UxmlFloatAttributeDescription
            {
                name = "mask-rect-height",
                defaultValue = k_DefaultMaskRect.height
            };

            readonly UxmlBoolAttributeDescription m_UseNormalizedMaskRect = new UxmlBoolAttributeDescription
            {
                name = "use-normalized-mask-rect",
                defaultValue = k_DefaultUseNormalizedMaskRect
            };

            /// <summary>
            /// Initialize the <see cref="Mask"/> using values from the attribute bag.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> to read values from.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var mask = (Mask)ve;
                mask.innerMaskColor = m_InnerMaskColor.GetValueFromBag(bag, cc);
                mask.outerMaskColor = m_OuterMaskColor.GetValueFromBag(bag, cc);
                mask.radius = m_Radius.GetValueFromBag(bag, cc);
                mask.blur = m_Blur.GetValueFromBag(bag, cc);
                mask.useNormalizedMaskRect = m_UseNormalizedMaskRect.GetValueFromBag(bag, cc);
                mask.maskRect = new Rect(m_MaskRectX.GetValueFromBag(bag, cc), m_MaskRectY.GetValueFromBag(bag, cc), m_MaskRectWidth.GetValueFromBag(bag, cc), m_MaskRectHeight.GetValueFromBag(bag, cc));
            }
        }

#endif
    }
}
