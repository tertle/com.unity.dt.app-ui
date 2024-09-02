using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// ActionButton UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class ActionButton : ExVisualElement, ISizeableElement, ISelectableElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId labelProperty = nameof(label);

        internal static readonly BindingId iconProperty = nameof(icon);

        internal static readonly BindingId trailingIconProperty = nameof(trailingIcon);

        internal static readonly BindingId iconVariantProperty = nameof(iconVariant);

        internal static readonly BindingId trailingIconVariantProperty = nameof(trailingIconVariant);

        internal static readonly BindingId quietProperty = nameof(quiet);

        internal static readonly BindingId selectedProperty = nameof(selected);

        internal static readonly BindingId accentProperty = nameof(accent);
#endif

        /// <summary>
        /// The ActionButton main styling class.
        /// </summary>
        public const string ussClassName = "appui-actionbutton";

        /// <summary>
        /// The ActionButton icon styling class.
        /// </summary>
        public const string iconUssClassName = ussClassName + "__icon";

        /// <summary>
        /// The ActionButton trailing icon styling class.
        /// </summary>
        public const string trailingIconUssClassName = ussClassName + "__trailing-icon";

        /// <summary>
        /// The ActionButton label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The ActionButton icon and label variant styling class.
        /// </summary>
        public const string iconAndLabelUssClassName = ussClassName + "--icon-and-label";

        /// <summary>
        /// The ActionButton with trailing icon variant styling class.
        /// </summary>
        public const string withTrailingIconUSsClassName = ussClassName + "--with-trailing-icon";

        /// <summary>
        /// The ActionButton icon only variant styling class.
        /// </summary>
        public const string iconOnlyUssClassName = ussClassName + "--icon-only";

        /// <summary>
        /// The ActionButton quiet variant styling class.
        /// </summary>
        public const string quietUssClassName = ussClassName + "--quiet";

        /// <summary>
        /// The ActionButton size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The ActionButton accent styling class.
        /// </summary>
        public const string accentUssClassName = ussClassName + "--accent";

        readonly Icon m_IconElement;

        readonly LocalizedTextElement m_LabelElement;

        readonly Icon m_TrailingIconElement;

        Size m_Size;

        Pressable m_Clickable;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ActionButton() : this(null) { }

        /// <summary>
        /// Construct a <see cref="ActionButton"/> with a given click event callback.
        /// </summary>
        /// <param name="clickEvent">THe given click event callback.</param>
        public ActionButton(Action clickEvent)
        {
            AddToClassList(ussClassName);

            clickable = new Pressable(clickEvent);
            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;

            m_IconElement = new Icon { name = iconUssClassName, iconName = null, pickingMode = PickingMode.Ignore };
            m_IconElement.AddToClassList(iconUssClassName);
            m_LabelElement = new LocalizedTextElement { name = labelUssClassName, text = null, pickingMode = PickingMode.Ignore };
            m_LabelElement.AddToClassList(labelUssClassName);
            m_TrailingIconElement = new Icon { name = trailingIconUssClassName, iconName = null, pickingMode = PickingMode.Ignore };
            m_TrailingIconElement.AddToClassList(trailingIconUssClassName);

            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocus, OnFocus));

            hierarchy.Add(m_IconElement);
            hierarchy.Add(m_LabelElement);
            hierarchy.Add(m_TrailingIconElement);

            passMask = 0;
            size = Size.M;
            accent = false;
            quiet = false;
            iconVariant = IconVariant.Regular;
            trailingIconVariant = IconVariant.Regular;

            Refresh();
        }

        void OnFocus(FocusInEvent evt)
        {
            passMask = 0;
        }

        void OnKeyboardFocus(FocusInEvent evt)
        {
            passMask = Passes.Clear | Passes.Outline;
        }

        /// <summary>
        /// Clickable Manipulator for this ActionButton.
        /// </summary>
        public Pressable clickable
        {
            get => m_Clickable;
            set
            {
                if (m_Clickable != null)
                {
                    m_Clickable.clicked -= OnClick;
                    if (m_Clickable.target == this)
                        this.RemoveManipulator(m_Clickable);
                }
                m_Clickable = value;
                if (m_Clickable == null)
                    return;
                this.AddManipulator(m_Clickable);
                m_Clickable.clicked += OnClick;
            }
        }

        /// <summary>
        /// The ActionButton click event.
        /// </summary>
        public event Action clicked
        {
            add => clickable.clicked += value;
            remove => clickable.clicked -= value;
        }

        /// <summary>
        /// The ActionButton label.
        /// </summary>
        [Tooltip("The ActionButton label.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Action Button")]
#endif
        public string label
        {
            get => m_LabelElement.text;
            set
            {
                var changed = m_LabelElement.text != value;
                m_LabelElement.text = value;
                Refresh();
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// The ActionButton icon.
        /// </summary>
        [Tooltip("The ActionButton icon.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string icon
        {
            get => m_IconElement.iconName;
            set
            {
                var changed = m_IconElement.iconName != value;
                m_IconElement.iconName = value;
                Refresh();
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in iconProperty);
#endif
            }
        }

        /// <summary>
        /// The ActionButton trailing icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string trailingIcon
        {
            get => m_TrailingIconElement.iconName;
            set
            {
                var changed = m_TrailingIconElement.iconName != value;
                m_TrailingIconElement.iconName = value;
                Refresh();
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in trailingIconProperty);
#endif
            }
        }

        /// <summary>
        /// The ActionButton icon variant.
        /// </summary>
        [Tooltip("The ActionButton icon variant.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public IconVariant iconVariant
        {
            get => m_IconElement.variant;
            set
            {
                var changed = m_IconElement.variant != value;
                m_IconElement.variant = value;
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in iconVariantProperty);
#endif
            }
        }

        /// <summary>
        /// The ActionButton trailing icon variant.
        /// </summary>
        [Tooltip("The ActionButton trailing icon variant.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public IconVariant trailingIconVariant
        {
            get => m_TrailingIconElement.variant;
            set
            {
                var changed = m_TrailingIconElement.variant != value;
                m_TrailingIconElement.variant = value;
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in trailingIconVariantProperty);
#endif
            }
        }

        /// <summary>
        /// The selected state of the ActionButton.
        /// </summary>
        [Tooltip("The selected state of the ActionButton")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool selected
        {
            get => ClassListContains(Styles.selectedUssClassName);
            set
            {
                var changed = selected != value;
                SetSelectedWithoutNotify(value);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in selectedProperty);
#endif
            }
        }

        /// <summary>
        /// The quiet state of the ActionButton.
        /// </summary>
        [Tooltip("The quiet state of the ActionButton")]
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
        /// The accent variant of the ActionButton.
        /// </summary>
        [Tooltip("The accent variant of the ActionButton")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool accent
        {
            get => ClassListContains(accentUssClassName);
            set
            {
                var changed = accent != value;
                EnableInClassList(accentUssClassName, value);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in accentProperty);
#endif
            }
        }

        /// <summary>
        /// The content container of the ActionButton.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// The current size of the ActionButton.
        /// </summary>
        [Tooltip("The current size of the ActionButton.")]
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
        /// Set the selected state of the ActionButton without notifying the click event.
        /// </summary>
        /// <param name="newValue"> The new selected state.</param>
        public void SetSelectedWithoutNotify(bool newValue)
        {
            EnableInClassList(Styles.selectedUssClassName, newValue);
        }

        void Refresh()
        {
            EnableInClassList(iconAndLabelUssClassName, !string.IsNullOrEmpty(icon) && !string.IsNullOrEmpty(label));
            EnableInClassList(withTrailingIconUSsClassName, !string.IsNullOrEmpty(trailingIcon));
            EnableInClassList(iconOnlyUssClassName, !string.IsNullOrEmpty(icon) && string.IsNullOrEmpty(label) && string.IsNullOrEmpty(trailingIcon));
            m_LabelElement.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(label));
            m_IconElement.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(icon));
            m_TrailingIconElement.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(trailingIcon));
        }

        void OnClick()
        {
            using var evt = ActionTriggeredEvent.GetPooled();
            evt.target = this;
            SendEvent(evt);
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// The ActionButton UXML factory.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ActionButton, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="ActionButton"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Icon = new UxmlStringAttributeDescription
            {
                name = "icon",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_TrailingIcon = new UxmlStringAttributeDescription
            {
                name = "trailing-icon",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = null
            };

            readonly UxmlBoolAttributeDescription m_Quiet = new UxmlBoolAttributeDescription
            {
                name = "quiet",
                defaultValue = false,
            };

            readonly UxmlBoolAttributeDescription m_Selected = new UxmlBoolAttributeDescription
            {
                name = "selected",
                defaultValue = false,
            };

            readonly UxmlBoolAttributeDescription m_Accent = new UxmlBoolAttributeDescription
            {
                name = "accent",
                defaultValue = false,
            };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlEnumAttributeDescription<IconVariant> m_IconVariant = new UxmlEnumAttributeDescription<IconVariant>
            {
                name = "icon-variant",
                defaultValue = IconVariant.Regular,
            };

            readonly UxmlEnumAttributeDescription<IconVariant> m_TrailingIconVariant = new UxmlEnumAttributeDescription<IconVariant>
            {
                name = "trailing-icon-variant",
                defaultValue = IconVariant.Regular,
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

                var el = (ActionButton)ve;
                el.label = m_Label.GetValueFromBag(bag, cc);
                el.icon = m_Icon.GetValueFromBag(bag, cc);
                el.iconVariant = m_IconVariant.GetValueFromBag(bag, cc);
                el.trailingIcon = m_TrailingIcon.GetValueFromBag(bag, cc);
                el.trailingIconVariant = m_TrailingIconVariant.GetValueFromBag(bag, cc);
                el.size = m_Size.GetValueFromBag(bag, cc);
                el.accent = m_Accent.GetValueFromBag(bag, cc);
                el.selected = m_Selected.GetValueFromBag(bag, cc);
                el.quiet = m_Quiet.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
