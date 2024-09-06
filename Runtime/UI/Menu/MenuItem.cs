using System;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// An item contained inside a <see cref="Menu"/> element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class MenuItem : BaseVisualElement, INotifyValueChanged<bool>, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId labelProperty = new BindingId(nameof(label));

        internal static readonly BindingId shortcutProperty = new BindingId(nameof(shortcut));

        internal static readonly BindingId iconProperty = new BindingId(nameof(icon));

        internal static readonly BindingId valueProperty = new BindingId(nameof(value));

        internal static readonly BindingId selectableProperty = new BindingId(nameof(selectable));

        internal static readonly BindingId activeProperty = new BindingId(nameof(active));

        internal static readonly BindingId subMenuProperty = new BindingId(nameof(subMenu));

        internal static readonly BindingId hasSubMenuProperty = new BindingId(nameof(hasSubMenu));

#endif

        static readonly Stack<Menu> k_SubMenuStack = new Stack<Menu>();

        internal const string checkmarkIconName = "check";

        const string k_SubMenuIconName = "sub-menu-indicator";

        /// <summary>
        /// The MenuItem main styling class.
        /// </summary>
        public const string ussClassName = "appui-menuitem";

        /// <summary>
        /// The MenuItem label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The MenuItem shortcut styling class.
        /// </summary>
        public const string shortcutUssClassName = ussClassName + "__shortcut";

        /// <summary>
        /// The MenuItem icon styling class.
        /// </summary>
        public const string iconUssClassName = ussClassName + "__icon";

        /// <summary>
        /// The MenuItem checkmark styling class.
        /// </summary>
        public const string checkmarkUssClassName = ussClassName + "__checkmark";

        /// <summary>
        /// The MenuItem submenu mode styling class.
        /// </summary>
        public const string subMenuItemUssClassname = ussClassName + "--submenu";

        /// <summary>
        /// The MenuItem submenu icon styling class.
        /// </summary>
        public const string subMenuIconUssClassname = ussClassName + "__submenu-icon";

        /// <summary>
        /// The MenuItem selectable mode styling class.
        /// </summary>
        public const string selectableUssClassname = ussClassName + "--selectable";

        /// <summary>
        /// The MenuItem active styling class.
        /// </summary>
        public const string activeUssClassname = ussClassName + "--active";

        /// <summary>
        /// The content container of the MenuItem.
        /// </summary>
        public override VisualElement contentContainer => m_SubMenuContainer;

        /// <summary>
        /// The event raised when the item's submenu is opened.
        /// </summary>
        public event Action subMenuOpened;

        readonly Icon m_Icon;

        readonly LocalizedTextElement m_Label;

        readonly LocalizedTextElement m_Shortcut;

        bool m_Selected;

        readonly VisualElement m_SubMenuContainer;

        Menu m_SubMenu;

        IVisualElementScheduledItem m_ScheduledItem;

        Pressable m_Clickable;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MenuItem()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;
            clickable = new Pressable(OnClick);

            var checkmark = new Icon { name = checkmarkUssClassName, iconName = checkmarkIconName, pickingMode = PickingMode.Ignore };
            checkmark.AddToClassList(checkmarkUssClassName);
            m_Icon = new Icon { name = iconUssClassName, pickingMode = PickingMode.Ignore };
            m_Icon.AddToClassList(iconUssClassName);
            m_Label = new LocalizedTextElement { name = labelUssClassName, pickingMode = PickingMode.Ignore };
            m_Label.AddToClassList(labelUssClassName);
            m_Shortcut = new LocalizedTextElement { name = shortcutUssClassName, pickingMode = PickingMode.Ignore };
            m_Shortcut.AddToClassList(shortcutUssClassName);
            var subMenuIcon = new Icon { name = subMenuIconUssClassname, iconName = k_SubMenuIconName, pickingMode = PickingMode.Ignore };
            subMenuIcon.AddToClassList(subMenuIconUssClassname);
            hierarchy.Add(checkmark);
            hierarchy.Add(m_Icon);
            hierarchy.Add(m_Label);
            hierarchy.Add(m_Shortcut);
            hierarchy.Add(subMenuIcon);

            this.AddManipulator(new KeyboardFocusController());

            m_SubMenuContainer = new VisualElement { style = { display = DisplayStyle.None } };
            hierarchy.Add(m_SubMenuContainer);

            selectable = false;
            active = false;
            icon = null;
            label = null;
            subMenu = null;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<PointerOverEvent>(OnEntered);
            RegisterCallback<PointerOutEvent>(OnLeft);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            var handled = false;

            var dir = this.GetContext<DirContext>()?.dir ?? Dir.Ltr;

            switch (evt.keyCode)
            {
                case KeyCode.DownArrow:
                    focusController.FocusNextInDirectionEx(VisualElementFocusChangeDirection.right);
                    handled = true;
                    break;
                case KeyCode.UpArrow:
                    focusController.FocusNextInDirectionEx(VisualElementFocusChangeDirection.left);
                    handled = true;
                    break;
                case KeyCode.RightArrow when dir is Dir.Ltr:
                case KeyCode.LeftArrow when dir is Dir.Rtl:
                    if (hasSubMenu)
                        clickable?.SimulateSingleClickInternal(evt);
                    handled = true;
                    break;
                case KeyCode.LeftArrow when dir is Dir.Ltr:
                case KeyCode.RightArrow when dir is Dir.Rtl:
                    if (GetFirstAncestorOfType<Menu>() is { parentItem: { } item } menu)
                    {
                        CloseSubMenus(Vector2.negativeInfinity, menu);
                        item.Focus();
                    }
                    handled = true;
                    break;
            }

            if (handled)
            {
                evt.StopPropagation();

            }
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (m_SubMenuContainer.childCount > 0)
            {
                subMenu = (Menu)m_SubMenuContainer.ElementAt(0);
                subMenu.parentItem = this;
                m_SubMenuContainer.Remove(subMenu);
            }
        }

        void OnEntered(PointerOverEvent evt)
        {
            var menu = GetFirstAncestorOfType<Menu>();
            if (menu != null)
            {
                foreach (var menuItem in menu.GetMenuItems())
                {
                    if (menuItem.subMenu != null && menuItem != this)
                        CloseSubMenus(evt.localPosition, menuItem.subMenu);
                }
            }

            if (UnityEngine.Device.Application.isMobilePlatform)
                return;

            if (subMenu != null)
                ScheduleOpenSubMenu(420);
        }

        void OnLeft(PointerOutEvent evt)
        {
            if (UnityEngine.Device.Application.isMobilePlatform)
                return;

            if (subMenu != null)
            {
                m_ScheduledItem?.Pause();
                CloseSubMenus(evt.localPosition, subMenu);
            }
        }

        void ScheduleOpenSubMenu(int delayMs)
        {
            m_ScheduledItem?.Pause();

            if (!enabledInHierarchy || !enabledSelf)
                return;

            m_ScheduledItem = schedule.Execute(OpenSubMenu);
            if (delayMs > 0)
                m_ScheduledItem.ExecuteLater(delayMs);
        }

        void OpenSubMenu()
        {
            m_ScheduledItem?.Pause();
            if (subMenu.parent != null)
                return;

            var popoverElement = MenuBuilder.CreateMenuPopoverVisualElement(subMenu).popoverElement;

            var popover = GetFirstAncestorOfType<Popover.PopoverVisualElement>();
            popover.popoverElement.parent.hierarchy.Add(popoverElement);
            popoverElement.visible = false;
            popoverElement.style.opacity = 0.00001f;
            popover.schedule.Execute(() =>
            {
                var dir = this.GetContext<DirContext>()?.dir ?? Dir.Ltr;
                var pos = AnchorPopupUtils
                    .ComputePosition(
                        popoverElement,
                        this,
                        GetFirstAncestorOfType<Panel>(),
                        new PositionOptions(dir == Dir.Ltr ? PopoverPlacement.EndTop : PopoverPlacement.StartTop,
                            -4,
                            -8));
                popoverElement.style.left = pos.left;
                popoverElement.style.top = pos.top;
                popoverElement.style.marginLeft = pos.marginLeft;
                popoverElement.style.marginTop = pos.marginTop;
                popoverElement.visible = true;
                popoverElement.style.opacity = 1f;
                popoverElement.schedule.Execute(() =>
                {
                    popoverElement.Focus();
                    subMenuOpened?.Invoke();
                });
            });

            k_SubMenuStack.Push(subMenu);
        }

        internal void CloseSubMenus(Vector2 localMousePosition, Menu targetMenu)
        {
            if (targetMenu?.parent == null)
                return;

            while (k_SubMenuStack.TryPeek(out var stackedMenu))
            {
                var popoverElement = stackedMenu.parent.parent;
                var mousePosition = popoverElement.WorldToLocal(this.LocalToWorld(localMousePosition));
                if (popoverElement.ContainsPoint(mousePosition))
                    break;

                popoverElement.parent.hierarchy.Remove(popoverElement);
                stackedMenu.parent.hierarchy.Remove(stackedMenu);

                k_SubMenuStack.Pop();
                if (stackedMenu == targetMenu)
                    break;
            }
        }

        /// <summary>
        /// Clickable Manipulator for this MenuItem.
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
        /// The label text value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string label
        {
            get => m_Label.text;
            set
            {
                var changed = m_Label.text != value;
                m_Label.text = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// The shortcut text value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string shortcut
        {
            get => m_Shortcut.text;
            set
            {
                var changed = m_Shortcut.text != value;
                m_Shortcut.text = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in shortcutProperty);
#endif
            }
        }

        /// <summary>
        /// The icon to display next to the label.
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
                m_Icon.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(value));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in iconProperty);
#endif
            }
        }

        /// <summary>
        /// The selected state of the item.
        /// </summary>
        /// <remarks>You should set the item as <see cref="selectable"/> first to see any result.</remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool value
        {
            get => ClassListContains(Styles.selectedUssClassName);
            set
            {
                if (m_Selected == value)
                    return;
                using var evt = ChangeEvent<bool>.GetPooled(m_Selected, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                if (selectable)
                    SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        /// <summary>
        /// <para>Enable or disable the selectable mode of the item.</para>
        /// <para>
        /// A selectable item is an item with a small checkmark as leading UI element.
        /// </para>
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool selectable
        {
            get => ClassListContains(selectableUssClassname);
            set
            {
                var changed = selectable != value;
                EnableInClassList(selectableUssClassname, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in selectableProperty);
#endif
            }
        }

        /// <summary>
        /// Enable or disable the active mode of the item.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool active
        {
            get => ClassListContains(activeUssClassname);
            set
            {
                var changed = active != value;
                EnableInClassList(activeUssClassname, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in activeProperty);
#endif
            }
        }

        /// <summary>
        /// <para>Sub Menu linked to this item.</para>
        /// <para>
        /// An item with a submenu mode enabled has a small caret as trailing UI element which defines that a sub menu
        /// will appear if you trigger the item's action.
        /// </para>
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Menu subMenu
        {
            get => m_SubMenu;
            set
            {
                var changed = m_SubMenu != value;
                m_SubMenu = value;
                EnableInClassList(subMenuItemUssClassname, m_SubMenu != null);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in subMenuProperty);
                    NotifyPropertyChanged(in hasSubMenuProperty);
                }
#endif
            }
        }

        /// <summary>
        /// Whether the item has a sub menu.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public bool hasSubMenu => subMenu != null;

        /// <summary>
        /// <para>Set the selected state of this item.</para>
        /// <para>
        /// See <see cref="value"/> and <see cref="selectable"/> properties for more info.
        /// </para>
        /// </summary>
        /// <param name="newValue">The new selected state.</param>
        public void SetValueWithoutNotify(bool newValue)
        {
            EnableInClassList(Styles.selectedUssClassName, newValue);
            m_Selected = newValue;
        }

        void OnClick()
        {
            if (selectable)
                value = !value;

            if (subMenu != null)
            {
                ScheduleOpenSubMenu(0);
            }
            else
            {
                using var evt = ActionTriggeredEvent.GetPooled();
                evt.target = this;
                SendEvent(evt);
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to be able to instantiate a MenuItem from UXML.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<MenuItem, UxmlTraits>
        {
            /// <summary>
            /// Describes the types of element that can appear as children of this element in a UXML file.
            /// </summary>
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription =>
                new List<UxmlChildElementDescription>
                {
                    new UxmlChildElementDescription(typeof(Menu))
                };
        }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="MenuItem"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Icon = new UxmlStringAttributeDescription
            {
                name = "icon",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_Shortcut = new UxmlStringAttributeDescription
            {
                name = "shortcut",
                defaultValue = null
            };

            readonly UxmlBoolAttributeDescription m_Selectable = new UxmlBoolAttributeDescription
            {
                name = "selectable",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription m_SelectedByDefault = new UxmlBoolAttributeDescription
            {
                name = "default-selected",
                defaultValue = false
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

                var element = (MenuItem)ve;
                element.icon = m_Icon.GetValueFromBag(bag, cc);
                element.label = m_Label.GetValueFromBag(bag, cc);
                element.shortcut = m_Shortcut.GetValueFromBag(bag, cc);
                element.selectable = m_Selectable.GetValueFromBag(bag, cc);
                element.SetValueWithoutNotify(m_SelectedByDefault.GetValueFromBag(bag, cc));


            }
        }

#endif
    }
}
