using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// By providing a type prop, you can specify the type of Dialog that is rendered by your DialogTrigger.
    /// </summary>
    /// <remarks>
    /// Note that pressing the Esc key will close the Dialog regardless of its type.
    /// </remarks>
    public enum PopupPresentationType
    {
        /// <summary>
        /// Modal Dialogs create an underlay that blocks access to the underlying user interface until the Dialog is closed.
        /// Sizing options can be found on the Dialog page.
        /// Focus is trapped inside the Modal.
        /// </summary>
        Modal,

        /// <summary>
        /// If a Dialog without an underlay is needed, consider using a Popover Dialog.
        /// See Dialog placement for how you can customize the positioning.
        /// Note that popovers are automatically rendered as modals on mobile by default.
        /// See the mobile type option for more information.
        /// </summary>
        Popover,

        /// <summary>
        /// Tray Dialogs are typically used to portray information on mobile devices or smaller screens.
        /// </summary>
        Tray,

        /// <summary>
        /// Fullscreen Dialogs are a fullscreen variant of the Modal Dialog, only revealing a small portion of the page
        /// behind the underlay. Use this variant for more complex workflows that do not fit in the available
        /// Modal Dialog sizes.
        /// This variant does not support dismissible.
        /// </summary>
        FullScreen,

        /// <summary>
        /// Fullscreen takeover Dialogs are similar to the fullscreen variant except that the Dialog covers the entire screen.
        /// </summary>
        FullScreenTakeOver,
    }

    /// <summary>
    /// Same as <see cref="PopupPresentationType"/> but for Mobile explicitly.
    /// </summary>
    public enum MobilePopupPresentationType
    {
        /// <summary>
        /// Modal Dialogs create an underlay that blocks access to the underlying user interface until the Dialog is closed.
        /// Sizing options can be found on the Dialog page.
        /// Focus is trapped inside the Modal.
        /// </summary>
        Modal,

        /// <summary>
        /// Tray Dialogs are typically used to portray information on mobile devices or smaller screens.
        /// </summary>
        Tray,

        /// <summary>
        /// Fullscreen Dialogs are a fullscreen variant of the Modal Dialog, only revealing a small portion of the page
        /// behind the underlay. Use this variant for more complex workflows that do not fit in the available
        /// Modal Dialog sizes.
        /// This variant does not support dismissible.
        /// </summary>
        FullScreen,

        /// <summary>
        /// Fullscreen takeover Dialogs are similar to the fullscreen variant except that the Dialog covers the entire screen.
        /// </summary>
        FullScreenTakeOver,
    }

    /// <summary>
    /// DialogTrigger serves as a wrapper around a Dialog and its associated trigger,
    /// linking the Dialog's open state with the trigger's press state. Additionally,
    /// it allows you to customize the type and positioning of the Dialog.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class DialogTrigger : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId triggerProperty = new BindingId(nameof(trigger));

        internal static readonly BindingId anchorProperty = new BindingId(nameof(anchor));

        internal static readonly BindingId dialogProperty = new BindingId(nameof(dialog));

        internal static readonly BindingId typeProperty = new BindingId(nameof(type));

        internal static readonly BindingId trayPositionProperty = new BindingId(nameof(trayPosition));

        internal static readonly BindingId transitionDurationProperty = new BindingId(nameof(transitionDuration));

        internal static readonly BindingId hideArrowProperty = new BindingId(nameof(hideArrow));

        internal static readonly BindingId mobileTypeProperty = new BindingId(nameof(mobileType));

        internal static readonly BindingId containerPaddingProperty = new BindingId(nameof(containerPadding));

        internal static readonly BindingId offsetProperty = new BindingId(nameof(offset));

        internal static readonly BindingId crossOffsetProperty = new BindingId(nameof(crossOffset));

        internal static readonly BindingId shouldFlipProperty = new BindingId(nameof(shouldFlip));

        internal static readonly BindingId isOpenProperty = new BindingId(nameof(isOpen));

        internal static readonly BindingId keyboardDismissEnabledProperty = new BindingId(nameof(keyboardDismissEnabled));

        internal static readonly BindingId outsideClickDismissEnabledProperty = new BindingId(nameof(outsideClickDismissEnabled));

        internal static readonly BindingId modalBackdropProperty = new BindingId(nameof(modalBackdrop));

        internal static readonly BindingId placementProperty = new BindingId(nameof(placement));

        internal static readonly BindingId resizableProperty = new BindingId(nameof(resizable));

        internal static readonly BindingId resizeDirectionProperty = new BindingId(nameof(resizeDirection));

#endif

        string m_AnchorName = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DialogTrigger()
        {
            pickingMode = PickingMode.Ignore;

            anchor = null;
            type = PopupPresentationType.Modal;
            trayPosition = TrayPosition.Bottom;
            transitionDuration = 150;
            hideArrow = false;
            mobileType = MobilePopupPresentationType.Modal;
            containerPadding = 0;
            offset = 0;
            crossOffset = 0;
            shouldFlip = true;
            keyboardDismissEnabled = true;
            outsideClickDismissEnabled = true;
            modalBackdrop = false;
            placement = PopoverPlacement.Top;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        BaseDialog m_Dialog;

        /// <summary>
        /// The dialog to display.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public BaseDialog dialog
        {
            get => m_Dialog;
            private set
            {
                var changed = m_Dialog != value;
                m_Dialog = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in dialogProperty);
#endif
            }
        }

        /// <summary>
        /// The trigger that will be used to start the display of the <see cref="dialog"/> element.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public VisualElement trigger { get; private set; }

        PopupPresentationType m_Type;

        /// <summary>
        /// The type of presentation used for this <see cref="dialog"/> element.
        /// </summary>
        /// <remarks>
        /// Some types are not available on mobile, to specify different presentation on mobile context use the <see cref="mobileType"/> property.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public PopupPresentationType type
        {
            get => m_Type;
            set
            {
                var changed = m_Type != value;
                m_Type = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in typeProperty);
#endif
            }
        }

        TrayPosition m_TrayPosition;

        /// <summary>
        /// The position of the Tray element.
        /// </summary>
        /// <remarks>
        /// This property is useful only if you set the <see cref="type"/> property to <see cref="PopupPresentationType.Tray"/>.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public TrayPosition trayPosition
        {
            get => m_TrayPosition;
            set
            {
                var changed = m_TrayPosition != value;
                m_TrayPosition = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in trayPositionProperty);
#endif
            }
        }

        int m_TransitionDuration;

        /// <summary>
        /// The duration of the transition in milliseconds.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int transitionDuration
        {
            get => m_TransitionDuration;
            set
            {
                var changed = m_TransitionDuration != value;
                m_TransitionDuration = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in transitionDurationProperty);
#endif
            }
        }

        bool m_HideArrow;

        /// <summary>
        /// Should the arrow be hidden.
        /// </summary>
        /// <remarks>
        /// This property is only useful with <see cref="PopupPresentationType.Popover"/> presentation type.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool hideArrow
        {
            get => m_HideArrow;
            set
            {
                var changed = m_HideArrow != value;
                m_HideArrow = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in hideArrowProperty);
#endif
            }
        }

        MobilePopupPresentationType m_MobileType;

        /// <summary>
        /// The type of presentation used for this <see cref="dialog"/> element on mobile platforms.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public MobilePopupPresentationType mobileType
        {
            get => m_MobileType;
            set
            {
                var changed = m_MobileType != value;
                m_MobileType = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in mobileTypeProperty);
#endif
            }
        }

        int m_ContainerPadding;

        /// <summary>
        /// The padding in pixels of the content inside the Popup.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int containerPadding
        {
            get => m_ContainerPadding;
            set
            {
                var changed = m_ContainerPadding != value;
                m_ContainerPadding = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in containerPaddingProperty);
#endif
            }
        }

        int m_Offset;

        /// <summary>
        /// The offset in pixels in the direction of the <see cref="placement"/> primary vector.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int offset
        {
            get => m_Offset;
            set
            {
                var changed = m_Offset != value;
                m_Offset = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in offsetProperty);
#endif
            }
        }

        int m_CrossOffset;

        /// <summary>
        /// The offset in pixels in the direction of the <see cref="placement"/> secondary vector.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int crossOffset
        {
            get => m_CrossOffset;
            set
            {
                var changed = m_CrossOffset != value;
                m_CrossOffset = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in crossOffsetProperty);
#endif
            }
        }

        bool m_ShouldFlip;

        /// <summary>
        /// Should the Popover <see cref="placement"/> be flipped if there's not enough space.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool shouldFlip
        {
            get => m_ShouldFlip;
            set
            {
                var changed = m_ShouldFlip != value;
                m_ShouldFlip = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in shouldFlipProperty);
#endif
            }
        }

        bool m_IsOpen;

        /// <summary>
        /// The open state of the dialog.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public bool isOpen
        {
            get => m_IsOpen;
            private set
            {
                var changed = m_IsOpen != value;
                m_IsOpen = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in isOpenProperty);
#endif
            }
        }

        bool m_KeyboardDismissEnabled = true;

        /// <summary>
        /// Disallow the use of Escape key or Return button to dismiss the <see cref="dialog"/>.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool keyboardDismissEnabled
        {
            get => m_KeyboardDismissEnabled;
            set
            {
                var changed = m_KeyboardDismissEnabled != value;
                m_KeyboardDismissEnabled = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in keyboardDismissEnabledProperty);
#endif
            }
        }

        bool m_OutsideClickDismissEnabled;

        /// <summary>
        /// Allow the use of clicking outside the <see cref="dialog"/> to dismiss it.
        /// </summary>
        /// <remarks>
        /// This property works only with <see cref="PopupPresentationType.Popover"/> presentation type.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool outsideClickDismissEnabled
        {
            get => m_OutsideClickDismissEnabled;
            set
            {
                var changed = m_OutsideClickDismissEnabled != value;
                m_OutsideClickDismissEnabled = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in outsideClickDismissEnabledProperty);
#endif
            }
        }

        bool m_ModalBackdrop;

        /// <summary>
        /// Enable or disable the blocking of the UI behind the <see cref="dialog"/>.
        /// </summary>
        /// <remarks>
        /// This property works only with <see cref="PopupPresentationType.Popover"/> presentation type.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool modalBackdrop
        {
            get => m_ModalBackdrop;
            set
            {
                var changed = m_ModalBackdrop != value;
                m_ModalBackdrop = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in modalBackdropProperty);
#endif
            }
        }

        VisualElement m_Anchor;

        /// <summary>
        /// The UI element used as an anchor.
        /// </summary>
        /// <remarks>
        /// This is only useful for presentations using popups of type <see cref="AnchorPopup{T}"/>.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public VisualElement anchor
        {
            get => m_Anchor;
            set
            {
                var changed = m_Anchor != value;
                m_Anchor = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in anchorProperty);
#endif
            }
        }

        PopoverPlacement m_Placement;

        /// <summary>
        /// The placement of the Popover.
        /// </summary>
        /// <remarks>
        /// This is only useful for presentations using popups of type <see cref="AnchorPopup{T}"/>.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public PopoverPlacement placement
        {
            get => m_Placement;
            set
            {
                var changed = m_Placement != value;
                m_Placement = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in placementProperty);
#endif
            }
        }

        bool m_Resizable;

        /// <summary>
        /// Whether the dialog is resizable.
        /// </summary>
        /// <remarks>
        /// This is only useful for presentations using popups of type <see cref="Popover"/>.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool resizable
        {
            get => m_Resizable;
            set
            {
                var changed = m_Resizable != value;
                m_Resizable = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in resizableProperty);
#endif
            }
        }

        Draggable.DragDirection m_ResizeDirection = Draggable.DragDirection.Vertical;

        /// <summary>
        /// Which direction the dialog can be resized.
        /// </summary>
        /// <remarks>
        /// This is only useful for presentations using popups of type <see cref="Popover"/>.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Draggable.DragDirection resizeDirection
        {
            get => m_ResizeDirection;
            set
            {
                var changed = m_ResizeDirection != value;
                m_ResizeDirection = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in resizeDirectionProperty);
#endif
            }
        }

        /// <summary>
        /// The content container of the DialogTrigger.
        /// </summary>
        public override VisualElement contentContainer => this;

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            BaseDialog dlg = null;
            VisualElement ve = null;

            foreach (var child in Children())
            {
                if (dlg == null && child is BaseDialog d)
                    dlg = d;

                if (ve == null && !(child is BaseDialog))
                    ve = child;

                if (dlg != null && ve != null)
                    break;
            }

            if (dlg != null && dlg != dialog)
            {
                // New Dialog attached as child
                dialog = dlg;
                Remove(dlg);
            }

            if (ve != null && ve != trigger)
            {
                if (trigger is IPressable c1)
                    c1.clickable.clicked -= OnActionTriggered;
                trigger = ve;
                if (trigger is IPressable c2)
                    c2.clickable.clicked += OnActionTriggered;
            }

            // we can also try to find the anchor (if any has been given with the UXML attribute)
            if (!string.IsNullOrEmpty(m_AnchorName) && panel != null)
            {
                var anchorElement = panel.visualTree.Q<VisualElement>(m_AnchorName);
                if (anchorElement != null)
                    anchor = anchorElement;
                else
                    Debug.LogWarning($"Unable to find {m_AnchorName}");
            }
        }

        void OnActionTriggered()
        {
            switch (type)
            {
                case PopupPresentationType.Modal:
                    Modal.Build(trigger, dialog)
                        .SetOutsideClickDismiss(outsideClickDismissEnabled)
                        .Show();
                    break;
                case PopupPresentationType.Popover:
                    Popover.Build(trigger, dialog)
                        .SetPlacement(placement)
                        .SetShouldFlip(shouldFlip)
                        .SetOffset(offset)
                        .SetCrossOffset(crossOffset)
                        .SetArrowVisible(!hideArrow)
                        .SetContainerPadding(containerPadding)
                        .SetOutsideClickDismiss(outsideClickDismissEnabled)
                        .SetModalBackdrop(modalBackdrop)
                        .SetKeyboardDismiss(keyboardDismissEnabled)
                        .SetResizable(resizable)
                        .SetResizeDirection(resizeDirection)
                        .Show();
                    break;
                case PopupPresentationType.Tray:
                    Tray.Build(trigger, dialog)
                        .SetPosition(trayPosition)
                        .SetTransitionDuration(transitionDuration)
                        .Show();
                    break;
                case PopupPresentationType.FullScreen:
                    Modal.Build(trigger, dialog).SetFullScreenMode(ModalFullScreenMode.FullScreen).Show();
                    break;
                case PopupPresentationType.FullScreenTakeOver:
                    Modal.Build(trigger, dialog).SetFullScreenMode(ModalFullScreenMode.FullScreenTakeOver).Show();
                    break;
                default:
                    throw new ValueOutOfRangeException(nameof(type), type);
            }

            isOpen = true;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class used to instantiate <see cref="DialogTrigger"/> from UXML.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<DialogTrigger, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="DialogTrigger"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Anchor = new UxmlStringAttributeDescription
            {
                name = "anchor",
                defaultValue = null
            };

            readonly UxmlIntAttributeDescription m_ContainerPadding = new UxmlIntAttributeDescription
            {
                name = "container-padding",
                defaultValue = 0
            };

            readonly UxmlIntAttributeDescription m_CrossOffset = new UxmlIntAttributeDescription
            {
                name = "cross-offset",
                defaultValue = 0
            };

            readonly UxmlBoolAttributeDescription m_HideArrow = new UxmlBoolAttributeDescription
            {
                name = "hide-arrow",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription m_KeyboardDismissEnabled = new UxmlBoolAttributeDescription
            {
                name = "keyboard-dismiss-enabled",
                defaultValue = true
            };

            readonly UxmlBoolAttributeDescription m_OutsideClickDismissEnabled = new UxmlBoolAttributeDescription()
            {
                name = "outside-click-dismiss-enabled",
                defaultValue = true
            };

            readonly UxmlBoolAttributeDescription m_ModalBackdrop = new UxmlBoolAttributeDescription()
            {
                name = "modal-backdrop",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<MobilePopupPresentationType> m_MobileType = new UxmlEnumAttributeDescription<MobilePopupPresentationType>
            {
                name = "mobile-type",
                defaultValue = MobilePopupPresentationType.Modal
            };

            readonly UxmlIntAttributeDescription m_Offset = new UxmlIntAttributeDescription
            {
                name = "offset",
                defaultValue = 0
            };

            readonly UxmlEnumAttributeDescription<PopoverPlacement> m_Placement = new UxmlEnumAttributeDescription<PopoverPlacement>
            {
                name = "placement",
                defaultValue = PopoverPlacement.Top
            };

            readonly UxmlBoolAttributeDescription m_ShouldFlip = new UxmlBoolAttributeDescription
            {
                name = "should-flip",
                defaultValue = true
            };

            readonly UxmlEnumAttributeDescription<PopupPresentationType> m_Type = new UxmlEnumAttributeDescription<PopupPresentationType>
            {
                name = "type",
                defaultValue = PopupPresentationType.Modal
            };

            readonly UxmlEnumAttributeDescription<TrayPosition> m_TrayPosition = new UxmlEnumAttributeDescription<TrayPosition>
            {
                name = "tray-position",
                defaultValue = TrayPosition.Bottom
            };

            readonly UxmlIntAttributeDescription m_TransitionDuration = new UxmlIntAttributeDescription
            {
                name = "transition-duration",
                defaultValue = 150
            };

            readonly UxmlBoolAttributeDescription m_Resizable = new UxmlBoolAttributeDescription
            {
                name = "resizable",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<Draggable.DragDirection> m_ResizeDirection = new UxmlEnumAttributeDescription<Draggable.DragDirection>
            {
                name = "resize-direction",
                defaultValue = Draggable.DragDirection.Vertical
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
                var el = (DialogTrigger)ve;

                el.type = m_Type.GetValueFromBag(bag, cc);
                el.trayPosition = m_TrayPosition.GetValueFromBag(bag, cc);
                el.transitionDuration = m_TransitionDuration.GetValueFromBag(bag, cc);
                el.mobileType = m_MobileType.GetValueFromBag(bag, cc);
                el.hideArrow = m_HideArrow.GetValueFromBag(bag, cc);
                el.placement = m_Placement.GetValueFromBag(bag, cc);
                el.offset = m_Offset.GetValueFromBag(bag, cc);
                el.crossOffset = m_CrossOffset.GetValueFromBag(bag, cc);
                el.containerPadding = m_ContainerPadding.GetValueFromBag(bag, cc);
                el.m_AnchorName = m_Anchor.GetValueFromBag(bag, cc);
                el.shouldFlip = m_ShouldFlip.GetValueFromBag(bag, cc);
                el.keyboardDismissEnabled = m_KeyboardDismissEnabled.GetValueFromBag(bag, cc);
                el.outsideClickDismissEnabled = m_OutsideClickDismissEnabled.GetValueFromBag(bag, cc);
                el.modalBackdrop = m_ModalBackdrop.GetValueFromBag(bag, cc);
                el.resizable = m_Resizable.GetValueFromBag(bag, cc);
                el.resizeDirection = m_ResizeDirection.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
