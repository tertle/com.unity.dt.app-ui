using System;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// IconButton UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class IconButton : ExVisualElement, ISizeableElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId iconProperty = new BindingId(nameof(icon));

        internal static readonly BindingId primaryProperty = new BindingId(nameof(primary));

        internal static readonly BindingId quietProperty = new BindingId(nameof(quiet));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId variantProperty = new BindingId(nameof(variant));

#endif

        /// <summary>
        /// The IconButton main styling class.
        /// </summary>
        public const string ussClassName = "appui-button";

        /// <summary>
        /// The IconButton primary variant styling class.
        /// </summary>
        public const string primaryUssClassName = ussClassName + "--primary";

        /// <summary>
        /// The IconButton quiet mode styling class.
        /// </summary>
        public const string quietUssClassName = ussClassName + "--quiet";

        /// <summary>
        /// The IconButton leading container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__leadingcontainer";

        /// <summary>
        /// The IconButton leading icon styling class.
        /// </summary>
        public const string iconUssClassName = ussClassName + "__leadingicon";

        /// <summary>
        /// The IconButton size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        readonly VisualElement m_Container;

        readonly Icon m_Icon;

        Size m_Size;

        Pressable m_Clickable;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IconButton()
            : this(null)
        {

        }

        /// <summary>
        /// Construct an IconButton with a given icon.
        /// </summary>
        /// <param name="iconName">The name of the icon.</param>
        /// <param name="clickEvent">The click event callback.</param>
        public IconButton(string iconName, Action clickEvent = null)
        {
            AddToClassList(ussClassName);
            AddToClassList(Button.iconOnlyUssClassName);

            clickable = new Pressable(clickEvent);
            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;
            passMask = 0;

            m_Container = new VisualElement { name = containerUssClassName, pickingMode = PickingMode.Ignore };
            m_Container.AddToClassList(containerUssClassName);
            m_Icon = new Icon { name = iconUssClassName, pickingMode = PickingMode.Ignore };
            m_Icon.AddToClassList(iconUssClassName);

            m_Container.hierarchy.Add(m_Icon);
            hierarchy.Add(m_Container);

            primary = false;
            quiet = false;
            icon = iconName;
            size = Size.M;
            variant = IconVariant.Regular;

            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            passMask = 0;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            passMask = Passes.Clear | Passes.Outline;
        }

        /// <summary>
        /// Event triggered when the Button has been clicked.
        /// </summary>
        public event Action clicked
        {
            add => m_Clickable.clicked += value;
            remove => m_Clickable.clicked -= value;
        }

        /// <summary>
        /// Clickable Manipulator for this Button.
        /// </summary>
        public Pressable clickable
        {
            get => m_Clickable;
            set
            {
                if (m_Clickable != null && m_Clickable.target == this)
                    this.RemoveManipulator(m_Clickable);
                m_Clickable = value;
                if (m_Clickable == null)
                    return;
                this.AddManipulator(m_Clickable);
            }
        }

        /// <summary>
        /// Use the primary variant of the Button.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool primary
        {
            get => ClassListContains(primaryUssClassName);
            set
            {
                var changed = ClassListContains(primaryUssClassName) != value;
                EnableInClassList(primaryUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in primaryProperty);
#endif
            }
        }

        /// <summary>
        /// The quiet state of the Button.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool quiet
        {
            get => ClassListContains(quietUssClassName);
            set
            {
                var changed = ClassListContains(quietUssClassName) != value;
                EnableInClassList(quietUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in quietProperty);
#endif
            }
        }

        /// <summary>
        /// The IconButton icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string icon
        {
            get => m_Icon.iconName;
            set
            {
                var changed = m_Icon.iconName != value;
                m_Icon.iconName = value;
                m_Container.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_Icon.iconName));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in iconProperty);
#endif
            }
        }

        /// <summary>
        /// The IconButton icon variant.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public IconVariant variant
        {
            get => m_Icon.variant;
            set
            {
                var changed = m_Icon.variant != value;
                m_Icon.variant = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in variantProperty);
#endif
            }
        }

        /// <summary>
        /// The Button size.
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
                m_Icon.size = m_Size switch
                {
                    Size.S => IconSize.S,
                    Size.M => IconSize.M,
                    Size.L => IconSize.L,
                    _ => IconSize.M
                };
                AddToClassList(GetSizeUssClassName(m_Size));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }


#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Factory class to instantiate a <see cref="IconButton"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<IconButton, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="IconButton"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {

            readonly UxmlStringAttributeDescription m_Icon = new UxmlStringAttributeDescription
            {
                name = "icon",
                defaultValue = null
            };

            readonly UxmlEnumAttributeDescription<IconVariant> m_Variant = new UxmlEnumAttributeDescription<IconVariant>
            {
                name = "variant",
                defaultValue = IconVariant.Regular
            };

            readonly UxmlBoolAttributeDescription m_Primary = new UxmlBoolAttributeDescription
            {
                name = "primary",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription m_Quiet = new UxmlBoolAttributeDescription
            {
                name = "quiet",
                defaultValue = false
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

                var element = (IconButton)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.primary = m_Primary.GetValueFromBag(bag, cc);
                element.quiet = m_Quiet.GetValueFromBag(bag, cc);
                element.icon = m_Icon.GetValueFromBag(bag, cc);
                element.variant = m_Variant.GetValueFromBag(bag, cc);

            }
        }

#endif
    }
}
