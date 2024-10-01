using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A base class for all progress UI elements. This class is not meant to be used directly.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class Progress : BaseVisualElement, ISizeableElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = new BindingId(nameof(value));

        internal static readonly BindingId roundedProgressCornersProperty = new BindingId(nameof(roundedProgressCorners));

        internal static readonly BindingId bufferValueProperty = new BindingId(nameof(bufferValue));

        internal static readonly BindingId bufferOpacityProperty = new BindingId(nameof(bufferOpacity));

        internal static readonly BindingId colorOverrideProperty = new BindingId(nameof(colorOverride));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId variantProperty = new BindingId(nameof(variant));

#endif

        static readonly Vertex[] k_Vertices = new Vertex[4];
        static readonly ushort[] k_Indices = { 0, 1, 2, 2, 3, 0 };

        static Progress()
        {
            k_Vertices[0].tint = Color.white;
            k_Vertices[1].tint = Color.white;
            k_Vertices[2].tint = Color.white;
            k_Vertices[3].tint = Color.white;
        }

        /// <summary>
        /// The progress variant.
        /// </summary>
        public enum Variant
        {
            /// <summary>
            /// The progress is indeterminate. A loop animation is displayed.
            /// </summary>
            Indeterminate = 0,
            /// <summary>
            /// The progress is determinate. The real progress is displayed.
            /// </summary>
            Determinate,
        }

        static Material s_Material;

        /// <summary>
        /// The Progress main styling class.
        /// </summary>
        public const string ussClassName = "appui-progress";

        /// <summary>
        /// The Progress image styling class.
        /// </summary>
        public const string imageUssClassName = ussClassName + "__image";

        /// <summary>
        /// The Progress container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The Progress rounded corners styling class.
        /// </summary>
        public const string roundedProgressCornersUssClassName = ussClassName + "--rounded-corners";

        /// <summary>
        /// The Progress size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Progress variant styling class.
        /// </summary>
        [EnumName("GetVariantUssClassName", typeof(Variant))]
        public const string variantUssClassName = ussClassName + "--";

        static readonly CustomStyleProperty<Color> k_UssColor = new CustomStyleProperty<Color>("--progress-color");

        Size m_Size;

        Optional<Color> m_ColorFromCode;

        Variant m_Variant;

        float m_Value;

        float m_BufferValue;

        /// <summary>
        /// The image that contains the rendered texture of the progress.
        /// </summary>
        protected readonly Image m_Image;

        /// <summary>
        /// The rendered texture of the progress.
        /// </summary>
        protected RenderTexture m_RT;

        Vector2 m_PreviousSize;

        /// <summary>
        /// The main color of the progress.
        /// </summary>
        protected Color m_ColorFromStyle;

        float m_BufferOpacity = 0.1f;

        IVisualElementScheduledItem m_Update;

        static Handler s_Handler;

        readonly VisualElement m_Container;

        /// <summary>
        /// The content container of the progress.
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Progress()
        {
            AddToClassList(ussClassName);

            focusable = false;
            pickingMode = PickingMode.Ignore;

            m_Image = new Image { name = imageUssClassName, pickingMode = PickingMode.Ignore };
            m_Image.AddToClassList(imageUssClassName);
            hierarchy.Add(m_Image);

            m_Container = new VisualElement { name = containerUssClassName, pickingMode = PickingMode.Ignore };
            m_Container.AddToClassList(containerUssClassName);
            hierarchy.Add(m_Container);

            m_ColorFromStyle = new Color(0.82f, 0.82f, 0.82f);
            variant = Variant.Indeterminate;
            size = Size.M;
            value = 0;
            bufferValue = 0;
            roundedProgressCorners = true;

            m_Image.generateVisualContent += OnGenerateVisualContent;
            generateVisualContent = OnGenerateVisualMainContent;
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
            RegisterCallback<CustomStyleResolvedEvent>(OnStylesResolved);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        void MarkContentDirtyRepaint()
        {
            m_Image.MarkDirtyRepaint();
        }

        const int k_ProgressGenerateTexturesMsgId = 521;

        static Handler handler
        {
            get
            {
                if (s_Handler == null)
                    s_Handler = new Handler(global::Unity.AppUI.Core.AppUI.mainLooper, HandleMessage);

                return s_Handler;
            }
        }

        static bool HandleMessage(Message message)
        {
            if (message.what == k_ProgressGenerateTexturesMsgId)
            {
                ((Progress)message.obj).GenerateTextures();
                return true;
            }

            return false;
        }

        // When the element goes to Display.None, GeometryChangedEvent will be called.
        void OnGeometryChanged(GeometryChangedEvent _)
        {
            UpdateScheduledItem();
        }

        // When the element MarkDirtyRepaint is called, GenerateVisualMainContent will be called.
        void OnGenerateVisualMainContent(MeshGenerationContext _)
        {
            UpdateScheduledItem();
        }

        void UpdateScheduledItem()
        {
            if (this.IsInvisible())
            {
                m_Update?.Pause();
                m_Update = null;
                return;
            }

            if (variant == Variant.Indeterminate)
                m_Update ??= schedule.Execute(MarkContentDirtyRepaint).Every(Styles.animationRefreshDelayMs);
        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            // handler.SendMessage(Message.Obtain(handler, k_ProgressGenerateTexturesMsgId, this));
            var msg = Message.Obtain(null, k_ProgressGenerateTexturesMsgId, this);
            HandleMessage(msg);
            msg.Recycle();

            var left = paddingRect.xMin;
            var right = paddingRect.xMax;
            var top = paddingRect.yMin;
            var bottom = paddingRect.yMax;

            k_Vertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
            k_Vertices[1].position = new Vector3(left, top, Vertex.nearZ);
            k_Vertices[2].position = new Vector3(right, top, Vertex.nearZ);
            k_Vertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

            var mwd = mgc.Allocate(k_Vertices.Length, k_Indices.Length, m_RT);

#if !UNITY_2023_1_OR_NEWER
            // Since the texture may be stored in an atlas, the UV coordinates need to be
            // adjusted. Simply rescale them in the provided uvRegion.
            var uvRegion = mwd.uvRegion;
#else
            var uvRegion = new Rect(0, 0, 1, 1);
#endif
            k_Vertices[0].uv = new Vector2(0, 0) * uvRegion.size + uvRegion.min;
            k_Vertices[1].uv = new Vector2(0, 1) * uvRegion.size + uvRegion.min;
            k_Vertices[2].uv = new Vector2(1, 1) * uvRegion.size + uvRegion.min;
            k_Vertices[3].uv = new Vector2(1, 0) * uvRegion.size + uvRegion.min;

            mwd.SetAllVertices(k_Vertices);
            mwd.SetAllIndices(k_Indices);
        }

        /// <summary>
        /// Whether to use rounded corners for the progress.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool roundedProgressCorners
        {
            get => ClassListContains(roundedProgressCornersUssClassName);
            set
            {
                var changed = roundedProgressCorners != value;
                EnableInClassList(roundedProgressCornersUssClassName, value);
                MarkContentDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in roundedProgressCornersProperty);
#endif
            }
        }

        /// <summary>
        /// The LinearProgress size.
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
        /// The variant of the progress.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Variant variant
        {
            get => m_Variant;
            set
            {
                var changed = m_Variant != value;
                RemoveFromClassList(GetVariantUssClassName(m_Variant));
                m_Variant = value;
                AddToClassList(GetVariantUssClassName(m_Variant));
                UpdateScheduledItem();
                MarkContentDirtyRepaint();
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in variantProperty);
#endif
            }
        }

        /// <summary>
        /// The opacity of the secondary progress (buffer).
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float bufferOpacity
        {
            get => m_BufferOpacity;
            set
            {
                var changed = !Mathf.Approximately(m_BufferOpacity, value);
                m_BufferOpacity = value;
                MarkContentDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in bufferOpacityProperty);
#endif
            }
        }

        /// <summary>
        /// The color of the progress.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Color colorOverride
        {
            get => m_ColorFromCode.IsSet ? m_ColorFromCode.Value : m_ColorFromStyle;
            set
            {
                var changed = colorOverride != value;
                m_ColorFromCode = value;
                MarkContentDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in colorOverrideProperty);
#endif
            }
        }

        /// <summary>
        /// The progress value (normalized).
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
                if (Mathf.Approximately(m_Value, value))
                    return;
                m_Value = value;
                MarkContentDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        /// <summary>
        /// The secondary progress (buffer) value (normalized).
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float bufferValue
        {
            get => m_BufferValue;
            set
            {
                if (Mathf.Approximately(m_BufferValue, value))
                    return;
                m_BufferValue = value;
                MarkContentDirtyRepaint();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in bufferValueProperty);
#endif
            }
        }

        void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            if (m_RT)
            {
                m_RT.Release();
                UnityObject.Destroy(m_RT);
            }

            m_RT = null;

            m_Update?.Pause();
            m_Update = null;
        }

        void OnStylesResolved(CustomStyleResolvedEvent evt)
        {
            if (evt.customStyle.TryGetValue(k_UssColor, out var c))
            {
                m_ColorFromStyle = c;
            }

            MarkContentDirtyRepaint();
        }

        /// <summary>
        /// Generates the textures for the progress.
        /// </summary>
        protected virtual void GenerateTextures() { }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Progress"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlColorAttributeDescription m_Color = new UxmlColorAttributeDescription
            {
                name = "color-override",
                defaultValue = Color.white,
            };

            readonly UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
            {
                name = "value",
                defaultValue = 0,
            };

            readonly UxmlFloatAttributeDescription m_ValueBuffer = new UxmlFloatAttributeDescription
            {
                name = "buffer-value",
                defaultValue = 0,
            };

            readonly UxmlFloatAttributeDescription m_BufferOpacity = new UxmlFloatAttributeDescription
            {
                name = "buffer-opacity",
                defaultValue = 0.1f,
            };

            readonly UxmlEnumAttributeDescription<Variant> m_Variant = new UxmlEnumAttributeDescription<Variant>
            {
                name = "variant",
                defaultValue = Variant.Indeterminate,
            };

            readonly UxmlBoolAttributeDescription m_RoundedCorners = new UxmlBoolAttributeDescription()
            {
                name = "rounded-progress-corners",
                defaultValue = true,
            };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                m_PickingMode.defaultValue = PickingMode.Ignore;
                base.Init(ve, bag, cc);

                var element = (Progress)ve;
                element.variant = m_Variant.GetValueFromBag(bag, cc);
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.value = m_Value.GetValueFromBag(bag, cc);
                element.bufferValue = m_ValueBuffer.GetValueFromBag(bag, cc);
                element.bufferOpacity = m_BufferOpacity.GetValueFromBag(bag, cc);
                element.roundedProgressCorners = m_RoundedCorners.GetValueFromBag(bag, cc);
                var color = Color.white;
                if (m_Color.TryGetValueFromBag(bag, cc, ref color))
                    element.colorOverride = color;
            }
        }

#endif
    }
}
