using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The Avatar variant.
    /// </summary>
    public enum AvatarVariant
    {
        /// <summary>
        /// Display the Avatar component as a square.
        /// </summary>
        Square,

        /// <summary>
        /// Display the Avatar component as a rounded square.
        /// </summary>
        Rounded,

        /// <summary>
        /// Display the Avatar component as a circle.
        /// </summary>
        Circular,
    }

    /// <summary>
    /// Avatar UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Avatar : BaseVisualElement, ISizeableElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId backgroundColorProperty = nameof(backgroundColor);

        internal static readonly BindingId outlineColorProperty = nameof(outlineColor);

        internal static readonly BindingId outlineWidthProperty = nameof(outlineWidth);

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId srcProperty = nameof(src);

        internal static readonly BindingId variantProperty = nameof(variant);
#endif

        /// <summary>
        /// The Avatar main styling class.
        /// </summary>
        public const string ussClassName = "appui-avatar";

        /// <summary>
        /// The Avatar container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The Avatar size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Avatar variant styling class.
        /// </summary>
        [EnumName("GetVariantUssClassName", typeof(AvatarVariant))]
        public const string variantUssClassName = ussClassName + "--";

        const Size k_DefaultSize = Size.M;

        const AvatarVariant k_DefaultVariant = AvatarVariant.Circular;

        Size m_Size = Size.M;

        readonly ExVisualElement m_Container;

        Optional<Color> m_BackgroundColor;

        Optional<Color> m_OutlineColor;

        Optional<float> m_OutlineWidth;

        AvatarVariant m_Variant;

        /// <summary>
        /// The content container of the Avatar.
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        /// <summary>
        /// The Avatar size.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Avatar")]
#endif
        public Size size
        {
            get => m_Size;
            set
            {
                RemoveFromClassList(GetSizeUssClassName(m_Size));
                m_Size = value;
                AddToClassList(GetSizeUssClassName(m_Size));
            }
        }

        /// <summary>
        /// The Avatar variant.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public AvatarVariant variant
        {
            get => m_Variant;
            set
            {
                RemoveFromClassList(GetVariantUssClassName(m_Variant));
                m_Variant = value;
                AddToClassList(GetVariantUssClassName(m_Variant));
            }
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("src")]
#endif
        Object srcTex
        {
            get => src.GetSelectedImage();
            set => src = BackgroundExtensions.FromObject(value);
        }

        /// <summary>
        /// The Avatar source image.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Background src
        {
            get => m_Container.style.backgroundImage.value;
            set
            {
                var changed = m_Container.style.backgroundImage.value != value;
                m_Container.style.backgroundImage = value;
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(srcProperty);
#endif
            }
        }

        /// <summary>
        /// The Avatar background color.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Optional<Color> backgroundColor
        {
            get => m_BackgroundColor;
            set
            {
                m_BackgroundColor = value;
                m_Container.style.backgroundColor = m_BackgroundColor.IsSet ?
                    m_BackgroundColor.Value : new StyleColor(StyleKeyword.Null);
            }
        }

        /// <summary>
        /// The Avatar outline width.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Optional<float> outlineWidth
        {
            get => m_OutlineWidth;
            set
            {
                m_OutlineWidth = value;

                var borderWidthStyle = m_OutlineWidth.IsSet ? m_OutlineWidth.Value : new StyleFloat(StyleKeyword.Null);
                style.borderBottomWidth = borderWidthStyle;
                style.borderLeftWidth = borderWidthStyle;
                style.borderRightWidth = borderWidthStyle;
                style.borderTopWidth = borderWidthStyle;
            }
        }

        /// <summary>
        /// The Avatar outline color.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Optional<Color> outlineColor
        {
            get => m_OutlineColor;
            set
            {
                m_OutlineColor = value;

                var colorStyle = m_OutlineColor.IsSet ? m_OutlineColor.Value : new StyleColor(StyleKeyword.Null);
                style.borderBottomColor = colorStyle;
                style.borderLeftColor = colorStyle;
                style.borderRightColor = colorStyle;
                style.borderTopColor = colorStyle;

                const float paddingValue = 1;

                var paddingStyle = m_OutlineColor.IsSet ? paddingValue : new StyleLength(StyleKeyword.Null);

                style.paddingBottom = paddingStyle;
                style.paddingLeft = paddingStyle;
                style.paddingRight = paddingStyle;
                style.paddingTop = paddingStyle;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Avatar()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Position;
            focusable = false;

            m_Container = new ExVisualElement { name = containerUssClassName, pickingMode = PickingMode.Ignore };
            m_Container.AddToClassList(containerUssClassName);
            hierarchy.Add(m_Container);

            size = k_DefaultSize;
            variant = k_DefaultVariant;
            backgroundColor = Optional<Color>.none;
            outlineColor = Optional<Color>.none;
            outlineWidth = 2;
            src = new Background();

            this.RegisterContextChangedCallback<AvatarVariantContext>(OnVariantContextChanged);
            this.RegisterContextChangedCallback<SizeContext>(OnSizeContextChanged);
        }

        void OnSizeContextChanged(ContextChangedEvent<SizeContext> evt)
        {
            if (evt.context != null)
                size = evt.context.size;
        }

        void OnVariantContextChanged(ContextChangedEvent<AvatarVariantContext> evt)
        {
            if (evt.context != null)
                variant = evt.context.variant;
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Defines the UxmlFactory for the Avatar.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Avatar, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Avatar"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {

            readonly UxmlColorAttributeDescription m_BackgroundColor = new UxmlColorAttributeDescription
            {
                name = "background-color",
                defaultValue = Color.gray
            };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = k_DefaultSize,
            };

            readonly UxmlEnumAttributeDescription<AvatarVariant> m_Variant = new UxmlEnumAttributeDescription<AvatarVariant>
            {
                name = "variant",
                defaultValue = k_DefaultVariant,
            };

            readonly UxmlStringAttributeDescription m_Src = new UxmlStringAttributeDescription
            {
                name = "src",
                defaultValue = null
            };

            readonly UxmlColorAttributeDescription m_OutlineColor = new UxmlColorAttributeDescription
            {
                name = "outline-color",
                defaultValue = Color.gray
            };

            readonly UxmlFloatAttributeDescription m_OutlineWidth = new UxmlFloatAttributeDescription
            {
                name = "outline-width",
                defaultValue = 2
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

                var element = (Avatar)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.variant = m_Variant.GetValueFromBag(bag, cc);
                var bgColor = Color.gray;
                if (m_BackgroundColor.TryGetValueFromBag(bag, cc, ref bgColor))
                    element.backgroundColor = bgColor;
                if (m_OutlineColor.TryGetValueFromBag(bag, cc, ref bgColor))
                    element.outlineColor = bgColor;
                string src = null;
                if (m_Src.TryGetValueFromBag(bag, cc, ref src))
                    element.src = BackgroundExtensions.FromObject(Resources.Load(src));
                element.outlineWidth = m_OutlineWidth.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
