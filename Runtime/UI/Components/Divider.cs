using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Divider UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Divider : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId directionProperty = new BindingId(nameof(direction));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId spacingProperty = new BindingId(nameof(spacing));

#endif

        /// <summary>
        /// The Divider main styling class.
        /// </summary>
        public const string ussClassName = "appui-divider";

        /// <summary>
        /// The Divider size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Divider spacing styling class.
        /// </summary>
        [EnumName("GetSpacingUssClassName", typeof(Spacing))]
        public const string spacingUssClassName = ussClassName + "--spacing-";

        /// <summary>
        /// The Divider vertical mode styling class.
        /// </summary>
        [EnumName("GetDirectionUssClassName", typeof(Direction))]
        public const string verticalUssClassName = ussClassName + "--";

        /// <summary>
        /// The Divider content styling class.
        /// </summary>
        public const string contentUssClassName = ussClassName + "__content";

        /// <summary>
        /// The content container of the Divider. This is always null.
        /// </summary>
        public override VisualElement contentContainer => null;

        Size m_Size;

        Spacing m_Spacing;

        Direction m_Direction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Divider()
        {
            AddToClassList(ussClassName);

            var content = new VisualElement { name = contentUssClassName, pickingMode = PickingMode.Ignore };
            content.AddToClassList(contentUssClassName);
            hierarchy.Add(content);

            pickingMode = PickingMode.Ignore;

            size = Size.M;
            spacing = Spacing.M;
            direction = Direction.Horizontal;
        }

        /// <summary>
        /// The orientation of the Divider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Direction direction
        {
            get => m_Direction;
            set
            {
                var changed = m_Direction != value;
                RemoveFromClassList(GetDirectionUssClassName(m_Direction));
                m_Direction = value;
                AddToClassList(GetDirectionUssClassName(m_Direction));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in directionProperty);
#endif
            }
        }

        /// <summary>
        /// The size of the Divider.
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
        /// The spacing of the Divider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Spacing spacing
        {
            get => m_Spacing;
            set
            {
                var changed = m_Spacing != value;
                RemoveFromClassList(GetSpacingUssClassName(m_Spacing));
                m_Spacing = value;
                AddToClassList(GetSpacingUssClassName(m_Spacing));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in spacingProperty);
#endif
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// The UXML factory for the Divider.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Divider, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Divider"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlEnumAttributeDescription<Spacing> m_Spacing = new UxmlEnumAttributeDescription<Spacing>
            {
                name = "spacing",
                defaultValue = Spacing.M,
            };

            readonly UxmlEnumAttributeDescription<Direction> m_Direction = new UxmlEnumAttributeDescription<Direction>
            {
                name = "direction",
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
                m_PickingMode.defaultValue = PickingMode.Ignore;
                base.Init(ve, bag, cc);

                var element = (Divider)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.direction = m_Direction.GetValueFromBag(bag, cc);
                element.spacing = m_Spacing.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
