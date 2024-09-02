using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The variant of the Button.
    /// </summary>
    public enum ButtonVariant
    {
        /// <summary>
        /// Default variant.
        /// </summary>
        Default,

        /// <summary>
        /// Primary variant.
        /// </summary>
        Accent,

        /// <summary>
        /// Quiet variant.
        /// </summary>
        Destructive,
    }

    /// <summary>
    /// Button UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Button : ExVisualElement, ISizeableElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId variantProperty = nameof(variant);

        internal static readonly BindingId quietProperty = nameof(quiet);

        internal static readonly BindingId titleProperty = nameof(title);

        internal static readonly BindingId subtitleProperty = nameof(subtitle);

        internal static readonly BindingId leadingIconProperty = nameof(leadingIcon);

        internal static readonly BindingId trailingIconProperty = nameof(trailingIcon);

        internal static readonly BindingId sizeProperty = nameof(size);
#endif
        /// <summary>
        /// The Button main styling class.
        /// </summary>
        public const string ussClassName = "appui-button";

        /// <summary>
        /// The Button variant styling class.
        /// </summary>
        [EnumName("GetVariantUssClassName", typeof(ButtonVariant))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The Button quiet mode styling class.
        /// </summary>
        public const string quietUssClassName = ussClassName + "--quiet";

        /// <summary>
        /// The Button leading container styling class.
        /// </summary>
        public const string leadingContainerUssClassName = ussClassName + "__leadingcontainer";

        /// <summary>
        /// The Button title container styling class.
        /// </summary>
        public const string titleContainerUssClassName = ussClassName + "__titlecontainer";

        /// <summary>
        /// The Button trailing container styling class.
        /// </summary>
        public const string trailingContainerUssClassName = ussClassName + "__trailingcontainer";

        /// <summary>
        /// The Button trailing icon styling class.
        /// </summary>
        public const string trailingIconUssClassName = ussClassName + "__trailingicon";

        /// <summary>
        /// The Button leading icon styling class.
        /// </summary>
        public const string leadingIconUssClassName = ussClassName + "__leadingicon";

        /// <summary>
        /// The Button title styling class.
        /// </summary>
        public const string titleUssClassName = ussClassName + "__title";

        /// <summary>
        /// The Button subtitle styling class.
        /// </summary>
        public const string subtitleUssClassName = ussClassName + "__subtitle";

        /// <summary>
        /// The Button size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Button icon only variant styling class.
        /// </summary>
        public const string iconOnlyUssClassName = ussClassName + "--icon-only";

        readonly VisualElement m_LeadingContainer;

        readonly Icon m_LeadingIcon;

        readonly LocalizedTextElement m_Subtitle;

        readonly LocalizedTextElement m_Title;

        readonly VisualElement m_TitleContainer;

        readonly VisualElement m_TrailingContainer;

        readonly Icon m_TrailingIcon;

        Size m_Size;

        Pressable m_Clickable;

        ButtonVariant m_Variant;

        /// <summary>
        /// The content container of this element.
        /// </summary>
        public override VisualElement contentContainer => this;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Button() : this(null) { }

        /// <summary>
        /// Constructs a button with an Action that is triggered when the button is clicked.
        /// </summary>
        /// <param name="clickEvent">The action triggered when the button is clicked.</param>
        /// <remarks>
        /// By default, a single left mouse click triggers the Action. To change the activator, modify <see cref="clickable"/>.
        /// </remarks>
        public Button(Action clickEvent)
        {
            AddToClassList(ussClassName);

            clickable = new Pressable(clickEvent);
            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;

            m_LeadingContainer = new VisualElement { name = leadingContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_LeadingContainer.AddToClassList(leadingContainerUssClassName);
            m_LeadingIcon = new Icon { name = leadingIconUssClassName, pickingMode = PickingMode.Ignore };
            m_LeadingIcon.AddToClassList(leadingIconUssClassName);
            m_TrailingContainer = new VisualElement { name = trailingContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_TrailingContainer.AddToClassList(trailingContainerUssClassName);
            m_TrailingIcon = new Icon { name = trailingIconUssClassName, pickingMode = PickingMode.Ignore };
            m_TrailingIcon.AddToClassList(trailingIconUssClassName);
            m_TitleContainer = new VisualElement { name = titleContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_TitleContainer.AddToClassList(titleContainerUssClassName);
            m_Title = new LocalizedTextElement { name = titleUssClassName, pickingMode = PickingMode.Ignore };
            m_Title.AddToClassList(titleUssClassName);
            m_Subtitle = new LocalizedTextElement { name = subtitleUssClassName, pickingMode = PickingMode.Ignore };
            m_Subtitle.AddToClassList(subtitleUssClassName);

            m_TitleContainer.hierarchy.Add(m_Title);
            m_TitleContainer.hierarchy.Add(m_Subtitle);

            m_LeadingContainer.hierarchy.Add(m_LeadingIcon);
            m_TrailingContainer.hierarchy.Add(m_TrailingIcon);

            hierarchy.Add(m_LeadingContainer);
            hierarchy.Add(m_TitleContainer);
            hierarchy.Add(m_TrailingContainer);

            passMask = 0;

            variant = ButtonVariant.Default;
            quiet = false;
            title = null;
            subtitle = null;
            leadingIcon = null;
            trailingIcon = null;
            size = Size.M;

            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocus, OnPointerFocus));
        }

        void OnPointerFocus(FocusInEvent evt)
        {
            passMask = 0;
        }

        void OnKeyboardFocus(FocusInEvent evt)
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
            private set
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
        /// The button variant.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Button")]
#endif
        public ButtonVariant variant
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
                var changed = quiet != value;
                EnableInClassList(quietUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in quietProperty);
#endif
            }
        }

        /// <summary>
        /// The title of the Button.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string title
        {
            get => m_Title.text;
            set
            {
                var changed = m_Title.text != value;
                m_Title.text = value;
                m_TitleContainer.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_Title.text));
                EnableInClassList(iconOnlyUssClassName, string.IsNullOrEmpty(m_Title.text));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in titleProperty);
#endif
            }
        }

        /// <summary>
        /// The subtitle of the Button.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string subtitle
        {
            get => m_Subtitle.text;
            set
            {
                var changed = m_Subtitle.text != value;
                m_Subtitle.text = value;
                m_Subtitle.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_Subtitle.text));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in subtitleProperty);
#endif
            }
        }

        /// <summary>
        /// The Button leading icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string leadingIcon
        {
            get => m_LeadingIcon.iconName;
            set
            {
                var changed = m_LeadingIcon.iconName != value;
                m_LeadingIcon.iconName = value;
                m_LeadingContainer.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_LeadingIcon.iconName));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in leadingIconProperty);
#endif
            }
        }

        /// <summary>
        /// The Button trailing icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string trailingIcon
        {
            get => m_TrailingIcon.iconName;
            set
            {
                var changed = m_TrailingIcon.iconName != value;
                m_TrailingIcon.iconName = value;
                m_TrailingContainer.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_TrailingIcon.iconName));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in trailingIconProperty);
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
                m_LeadingIcon.size = m_Size switch
                {
                    Size.S => IconSize.S,
                    Size.M => IconSize.M,
                    Size.L => IconSize.L,
                    _ => IconSize.M
                };
                m_TrailingIcon.size = m_LeadingIcon.size;
                AddToClassList(GetSizeUssClassName(m_Size));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines the UxmlFactory for the Button.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Button, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Button"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {

            readonly UxmlStringAttributeDescription m_LeadingIcon = new UxmlStringAttributeDescription
            {
                name = "leading-icon",
                defaultValue = null
            };

            readonly UxmlEnumAttributeDescription<ButtonVariant> m_Variant = new UxmlEnumAttributeDescription<ButtonVariant>
            {
                name = "variant",
                defaultValue = ButtonVariant.Default
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

            readonly UxmlStringAttributeDescription m_Subtitle = new UxmlStringAttributeDescription
            {
                name = "subtitle",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription
            {
                name = "title",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_TrailingIcon = new UxmlStringAttributeDescription
            {
                name = "trailing-icon",
                defaultValue = null
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

                var element = (Button)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                element.variant = m_Variant.GetValueFromBag(bag, cc);
                element.quiet = m_Quiet.GetValueFromBag(bag, cc);
                element.title = m_Title.GetValueFromBag(bag, cc);
                element.subtitle = m_Subtitle.GetValueFromBag(bag, cc);
                element.leadingIcon = m_LeadingIcon.GetValueFromBag(bag, cc);
                element.trailingIcon = m_TrailingIcon.GetValueFromBag(bag, cc);

            }
        }
#endif
    }
}
