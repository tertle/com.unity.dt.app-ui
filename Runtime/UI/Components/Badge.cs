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
    /// The variant of the Badge.
    /// </summary>
    public enum BadgeVariant
    {
        /// <summary>
        /// The default variant. The Badge will contain some text.
        /// </summary>
        Default,

        /// <summary>
        /// The dot variant. The Badge will be a small dot.
        /// </summary>
        Dot
    }

    /// <summary>
    /// The overlap type of the Badge.
    /// </summary>
    public enum BadgeOverlapType
    {
        /// <summary>
        /// The Badge overlap type is rectangular.
        /// </summary>
        Rectangular,

        /// <summary>
        /// The Badge overlap type is circular.
        /// </summary>
        Circular
    }

    /// <summary>
    /// A horizontal anchor.
    /// </summary>
    public enum HorizontalAnchor
    {
        /// <summary>
        /// The element is anchored at the left.
        /// </summary>
        Left,

        /// <summary>
        /// The element is anchored at the right.
        /// </summary>
        Right
    }

    /// <summary>
    /// A vertical anchor.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum VerticalAnchor
    {
        /// <summary>
        /// The element is anchored at the top.
        /// </summary>
        Top,

        /// <summary>
        /// The element is anchored at the bottom.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Badge UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Badge : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId backgroundColorProperty = nameof(backgroundColor);

        internal static readonly BindingId colorProperty = nameof(color);

        internal static readonly BindingId variantProperty = nameof(variant);

        internal static readonly BindingId overlapTypeProperty = nameof(overlapType);

        internal static readonly BindingId horizontalAnchorProperty = nameof(horizontalAnchor);

        internal static readonly BindingId verticalAnchorProperty = nameof(verticalAnchor);

        internal static readonly BindingId contentProperty = nameof(content);

        internal static readonly BindingId maxProperty = nameof(max);

        internal static readonly BindingId showZeroProperty = nameof(showZero);

#endif
        /// <summary>
        /// The Badge main styling class.
        /// </summary>
        public const string ussClassName = "appui-badge";

        /// <summary>
        /// The Badge label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The Badge badge styling class.
        /// </summary>
        public const string badgeUssClassName = ussClassName + "__badge";

        /// <summary>
        /// The Badge Zero content styling class.
        /// </summary>
        public const string zeroUssClassName = ussClassName + "--zero";

        /// <summary>
        /// The Badge variant styling class prefix.
        /// </summary>
        [EnumName("GetVariantUssClassName", typeof(BadgeVariant))]
        public const string variantClassName = ussClassName + "--";

        /// <summary>
        /// The Badge overlap type styling class prefix.
        /// </summary>
        [EnumName("GetOverlapUssClassName", typeof(BadgeOverlapType))]
        public const string overlapUssClassName = ussClassName + "--overlap-";

        /// <summary>
        /// The Badge horizontal anchor styling class prefix.
        /// </summary>
        [EnumName("GetHorizontalAnchorUssClassName", typeof(HorizontalAnchor))]
        public const string horizontalAnchorUssClassName = ussClassName + "--anchor-horizontal-";

        /// <summary>
        /// The Badge vertical anchor styling class prefix.
        /// </summary>
        [EnumName("GetVerticalAnchorUssClassName", typeof(VerticalAnchor))]
        public const string verticalAnchorUssClassName = ussClassName + "--anchor-vertical-";

        Optional<Color> m_BackgroundColor;

        BadgeVariant m_Variant;

        BadgeOverlapType m_BadgeOverlapType;

        HorizontalAnchor m_HorizontalAnchor;

        VerticalAnchor m_VerticalAnchor;

        int m_Content;

        readonly Text m_LabelElement;

        bool m_ShowZero;

        int m_Max;

        Optional<Color> m_Color;

        readonly VisualElement m_BadgeElement;

        /// <summary>
        /// The content container of the Badge.
        /// </summary>
        public override VisualElement contentContainer => this;

        /// <summary>
        /// The background color of the Badge.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Badge")]
#endif
        public Optional<Color> backgroundColor
        {
            get => m_BackgroundColor;
            set
            {
                var changed = m_BackgroundColor != value;
                m_BackgroundColor = value;
                m_BadgeElement.style.backgroundColor = m_BackgroundColor.IsSet ?
                    m_BackgroundColor.Value : new StyleColor(StyleKeyword.Null);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in backgroundColorProperty);
#endif
            }
        }

        /// <summary>
        /// The content color of the Badge.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Optional<Color> color
        {
            get => m_Color;
            set
            {
                var changed = m_Color != value;
                m_Color = value;
                m_LabelElement.style.color = m_Color.IsSet ? m_Color.Value : new StyleColor(StyleKeyword.Null);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in colorProperty);
#endif
            }
        }

        /// <summary>
        /// The variant of the Badge.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public BadgeVariant variant
        {
            get => m_Variant;
            set
            {
                var changed = m_Variant != value;
                RemoveFromClassList(GetVariantUssClassName(m_Variant));
                m_Variant = value;
                AddToClassList(GetVariantUssClassName(m_Variant));
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in variantProperty);
#endif
            }
        }

        /// <summary>
        /// The overlap type of the Badge.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public BadgeOverlapType overlapType
        {
            get => m_BadgeOverlapType;
            set
            {
                var changed = m_BadgeOverlapType != value;
                RemoveFromClassList(GetOverlapUssClassName(m_BadgeOverlapType));
                m_BadgeOverlapType = value;
                AddToClassList(GetOverlapUssClassName(m_BadgeOverlapType));
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in overlapTypeProperty);
#endif
            }
        }

        /// <summary>
        /// The horizontal anchor of the Badge.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public HorizontalAnchor horizontalAnchor
        {
            get => m_HorizontalAnchor;
            set
            {
                var changed = m_HorizontalAnchor != value;
                RemoveFromClassList(GetHorizontalAnchorUssClassName(m_HorizontalAnchor));
                m_HorizontalAnchor = value;
                AddToClassList(GetHorizontalAnchorUssClassName(m_HorizontalAnchor));
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in horizontalAnchorProperty);
#endif
            }
        }

        /// <summary>
        /// The vertical anchor of the Badge.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public VerticalAnchor verticalAnchor
        {
            get => m_VerticalAnchor;
            set
            {
                var changed = m_VerticalAnchor != value;
                RemoveFromClassList(GetVerticalAnchorUssClassName(m_VerticalAnchor));
                m_VerticalAnchor = value;
                AddToClassList(GetVerticalAnchorUssClassName(m_VerticalAnchor));
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in verticalAnchorProperty);
#endif
            }
        }

        /// <summary>
        /// The text of the Badge.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int content
        {
            get => m_Content;
            set
            {
                var changed = m_Content != value;
                m_Content = value;
                RefreshContent();
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in contentProperty);
#endif
            }
        }

        /// <summary>
        /// The maximum value of the Badge.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int max
        {
            get => m_Max;
            set
            {
                var changed = m_Max != value;
                m_Max = value;
                RefreshContent();
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in maxProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the Badge should show zero values.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool showZero
        {
            get => m_ShowZero;
            set
            {
                var changed = m_ShowZero != value;
                m_ShowZero = value;
                RefreshContent();
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in showZeroProperty);
#endif
            }
        }

        void RefreshContent()
        {
            m_LabelElement.text = m_Content > m_Max ? $"{m_Max}+" : m_Content.ToString();
            EnableInClassList(zeroUssClassName, !m_ShowZero && (m_Max == 0 || m_Content == 0));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Badge()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Position;
            focusable = false;

            m_BadgeElement = new VisualElement {name = badgeUssClassName, pickingMode = PickingMode.Ignore};
            m_BadgeElement.AddToClassList(badgeUssClassName);
            hierarchy.Add(m_BadgeElement);

            m_LabelElement = new Text { pickingMode = PickingMode.Ignore, name = labelUssClassName };
            m_LabelElement.AddToClassList(labelUssClassName);
            m_BadgeElement.Add(m_LabelElement);

            backgroundColor = Optional<Color>.none;
            color = Optional<Color>.none;
            variant = BadgeVariant.Default;
            overlapType = BadgeOverlapType.Rectangular;
            horizontalAnchor = HorizontalAnchor.Right;
            verticalAnchor = VerticalAnchor.Top;
            showZero = false;
            max = int.MaxValue;
            content = 0;
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Defines the UxmlFactory for the Badge.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Badge, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Badge"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlColorAttributeDescription m_BackgroundColor = new UxmlColorAttributeDescription
            {
                name = "background-color",
                defaultValue = new Color(1, 0.3f, 0.3f)
            };

            readonly UxmlEnumAttributeDescription<BadgeVariant> m_Variant = new UxmlEnumAttributeDescription<BadgeVariant>
            {
                name = "variant",
                defaultValue = BadgeVariant.Default
            };

            readonly UxmlEnumAttributeDescription<BadgeOverlapType> m_OverlapType = new UxmlEnumAttributeDescription<BadgeOverlapType>
            {
                name = "overlap-type",
                defaultValue = BadgeOverlapType.Rectangular
            };

            readonly UxmlEnumAttributeDescription<HorizontalAnchor> m_HorizontalAnchor = new UxmlEnumAttributeDescription<HorizontalAnchor>
            {
                name = "horizontal-anchor",
                defaultValue = HorizontalAnchor.Right
            };

            readonly UxmlEnumAttributeDescription<VerticalAnchor> m_VerticalAnchor = new UxmlEnumAttributeDescription<VerticalAnchor>
            {
                name = "vertical-anchor",
                defaultValue = VerticalAnchor.Top
            };

            readonly UxmlIntAttributeDescription m_Content = new UxmlIntAttributeDescription
            {
                name = "content",
                defaultValue = 0
            };

            readonly UxmlIntAttributeDescription m_Max = new UxmlIntAttributeDescription
            {
                name = "max",
                defaultValue = int.MaxValue
            };

            readonly UxmlBoolAttributeDescription m_ShowZero = new UxmlBoolAttributeDescription
            {
                name = "show-zero",
                defaultValue = false
            };

            readonly UxmlColorAttributeDescription m_Color = new UxmlColorAttributeDescription
            {
                name = "color",
                defaultValue = Color.white
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

                var el = (Badge)ve;

                var color = Color.clear;
                if (m_BackgroundColor.TryGetValueFromBag(bag, cc, ref color))
                    el.backgroundColor = color;

                if (m_Color.TryGetValueFromBag(bag, cc, ref color))
                    el.color = color;

                el.variant = m_Variant.GetValueFromBag(bag, cc);
                el.overlapType = m_OverlapType.GetValueFromBag(bag, cc);
                el.horizontalAnchor = m_HorizontalAnchor.GetValueFromBag(bag, cc);
                el.verticalAnchor = m_VerticalAnchor.GetValueFromBag(bag, cc);
                el.content = m_Content.GetValueFromBag(bag, cc);
                el.max = m_Max.GetValueFromBag(bag, cc);
                el.showZero = m_ShowZero.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
