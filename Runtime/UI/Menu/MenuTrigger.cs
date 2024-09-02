using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A wrapper to display a menu when a trigger has been activated.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class MenuTrigger : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId triggerProperty = new BindingId(nameof(trigger));

        internal static readonly BindingId anchorProperty = new BindingId(nameof(anchor));

        internal static readonly BindingId menuProperty = new BindingId(nameof(menu));

        internal static readonly BindingId closeOnSelectionProperty = new BindingId(nameof(closeOnSelection));

#endif

        string m_AnchorName;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MenuTrigger()
        {
            pickingMode = PickingMode.Ignore;

            anchor = null;
            closeOnSelection = true;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        VisualElement m_Trigger;

        /// <summary>
        /// The trigger used to determine when to display them <see cref="menu"/>.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public VisualElement trigger
        {
            get => m_Trigger;
            private set
            {
                var changed = m_Trigger != value;
                m_Trigger = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in triggerProperty);
#endif
            }
        }

        VisualElement m_Anchor;

        /// <summary>
        /// The UI element used as an anchor for the menu's popover.
        /// </summary>
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

        Menu m_Menu;

        /// <summary>
        /// The menu to display.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public Menu menu
        {
            get => m_Menu;
            private set
            {
                var changed = m_Menu != value;
                m_Menu = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in menuProperty);
#endif
            }
        }

        bool m_CloseOnSelection = true;

        /// <summary>
        /// Whether the menu should close when a selection is made.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool closeOnSelection
        {
            get => m_CloseOnSelection;
            set
            {
                var changed = m_CloseOnSelection != value;
                m_CloseOnSelection = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in closeOnSelectionProperty);
#endif
            }
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            Menu childMenu = null;
            VisualElement ve = null;

            foreach (var child in Children())
            {
                if (childMenu == null && child is Menu m)
                    childMenu = m;

                if (ve == null && !(child is Menu))
                    ve = child;

                if (childMenu != null && ve != null)
                    break;
            }

            if (childMenu != null && childMenu != menu)
            {
                // New Dialog attached as child
                menu = childMenu;
                Remove(childMenu);
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
            if (anchor != null && !string.IsNullOrEmpty(m_AnchorName) && panel != null)
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
            var popover = MenuBuilder.Build(anchor ?? trigger, menu);
            popover.SetCloseOnSelection(closeOnSelection);
            popover.Show();
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// UXML factory for the <see cref="MenuTrigger"/>.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<MenuTrigger, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="MenuTrigger"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Anchor = new UxmlStringAttributeDescription
            {
                name = "anchor",
                defaultValue = null
            };

            readonly UxmlBoolAttributeDescription m_CloseOnSelection = new UxmlBoolAttributeDescription
            {
                name = "close-on-selection",
                defaultValue = true
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

                var el = (MenuTrigger)ve;

                el.m_AnchorName = m_Anchor.GetValueFromBag(bag, cc);
                el.closeOnSelection = m_CloseOnSelection.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
