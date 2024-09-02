using System;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A utility class to build a Menu programmatically.
    /// </summary>
    public class MenuBuilder : AnchorPopup<MenuBuilder>
    {
        readonly Stack<Menu> m_MenuStack;

        IVisualElementScheduledItem m_ScheduledItem;

        bool m_CloseOnSelection = true;

        const string k_MenuPopoverUssClassName = "appui-popover--menu";

        Popover.PopoverVisualElement popover => (Popover.PopoverVisualElement)view;

        /// <summary>
        /// The last menu in the stack.
        /// </summary>
        public Menu currentMenu => m_MenuStack.Peek();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="referenceView"> The element used as context provider for the Menu. </param>
        /// <param name="view"> The Menu visual element (used by the popup system). </param>
        /// <param name="contentView"> The Menu's content visual element. </param>
        public MenuBuilder(VisualElement referenceView, VisualElement view, VisualElement contentView)
            : base(referenceView, view, contentView)
        {
            m_MenuStack = new Stack<Menu>();
            m_MenuStack.Push((Menu)contentView);
        }

        void OnWheel(WheelEvent evt)
        {
            if (outsideScrollEnabled)
                return;

            var inside = GetMovableElement().worldBound.Contains((Vector2)evt.mousePosition);
            if (!inside)
                evt.StopImmediatePropagation();
        }

        void OnTreeDown(PointerDownEvent evt)
        {
            if (!outsideClickDismissEnabled || outsideClickStrategy == 0 || view.parent == null)
                return;

            var shouldDismiss = true;
            if ((outsideClickStrategy & OutsideClickStrategy.Bounds) != 0)
            {
                foreach (var child in popover.hierarchy.Children())
                {
                    if (child.worldBound.Contains((Vector2)evt.position))
                    {
                        shouldDismiss = false;
                        break;
                    }
                }
            }

            if (shouldDismiss && (outsideClickStrategy & OutsideClickStrategy.Pick) != 0 && popover.panel is {} panel)
            {
                var picked = panel.Pick(evt.position);
                if (picked != popover)
                {
                    var commonAncestor = picked?.FindCommonAncestor(popover);
                    if (commonAncestor == popover) // if the picked element is a child of the popover, don't dismiss
                        shouldDismiss = false;
                }
            }

            if (!shouldDismiss)
                return;

            var insideAnchor = anchor?.worldBound.Contains((Vector2)evt.position) ?? false;
            var insideLastFocusedElement = (m_LastFocusedElement as VisualElement)?.worldBound.Contains((Vector2)evt.position) ?? false;
            if (insideAnchor || insideLastFocusedElement)
            {
                // prevent reopening the same popover again...
                evt.StopImmediatePropagation();
            }

            Dismiss(DismissType.OutOfBounds);
        }

        /// <summary>
        /// Add an Action menu item to the current menu.
        /// </summary>
        /// <param name="actionId"> A unique identifier for the action. </param>
        /// <param name="labelStr"> The raw label of the menu item (will be localized). </param>
        /// <param name="iconName"> The icon of the menu item. </param>
        /// <param name="callback"> The callback to invoke when the menu item is clicked. </param>
        /// <returns> The MenuBuilder instance. </returns>
        public MenuBuilder AddAction(int actionId, string labelStr, string iconName, EventCallback<ClickEvent> callback)
        {
            var item = new MenuItem
            {
                label = labelStr,
                icon = iconName,
                userData = actionId,
            };
            item.RegisterCallback(callback);
            currentMenu.Add(item);
            return this;
        }

        /// <summary>
        /// Add an Action menu item to the current menu.
        /// </summary>
        /// <param name="actionId"> A unique identifier for the action. </param>
        /// <param name="labelStr"> The raw label of the menu item (will be localized). </param>
        /// <param name="iconName"> The icon of the menu item. </param>
        /// <param name="shortcut"> The shortcut of the menu item. </param>
        /// <param name="callback"> The callback to invoke when the menu item is clicked. </param>
        /// <returns> The MenuBuilder instance. </returns>
        public MenuBuilder AddAction(int actionId, string labelStr, string iconName, string shortcut, EventCallback<ClickEvent> callback)
        {
            var item = new MenuItem
            {
                label = labelStr,
                icon = iconName,
                shortcut = shortcut,
                userData = actionId,
            };
            item.RegisterCallback(callback);
            currentMenu.Add(item);
            return this;
        }

        /// <summary>
        /// Add an Action menu item to the current menu.
        /// </summary>
        /// <param name="actionId"> A unique identifier for the action. </param>
        /// <param name="bindItemFunc"> A callback to bind the action. </param>
        /// <returns> The MenuBuilder instance. </returns>
        public MenuBuilder AddAction(int actionId, Action<MenuItem> bindItemFunc)
        {
            var item = new MenuItem
            {
                userData = actionId,
            };
            currentMenu.Add(item);
            bindItemFunc?.Invoke(item);
            return this;
        }

        /// <summary>
        /// Add a Menu Divider to the current menu.
        /// </summary>
        /// <param name="bindDividerFunc"> A callback to bind the divider. </param>
        /// <returns> The MenuBuilder instance. </returns>
        public MenuBuilder AddDivider(Action<MenuDivider> bindDividerFunc = null)
        {
            var item = new MenuDivider();
            currentMenu.Add(item);
            bindDividerFunc?.Invoke(item);
            return this;
        }

        /// <summary>
        /// Add a Section to the current menu.
        /// </summary>
        /// <param name="bindSectionFunc"> A callback to bind the section. </param>
        /// <returns> The MenuBuilder instance. </returns>
        public MenuBuilder AddSection(Action<MenuSection> bindSectionFunc = null)
        {
            var item = new MenuSection();
            currentMenu.Add(item);
            bindSectionFunc?.Invoke(item);
            return this;
        }

        /// <summary>
        /// Create an action menu item, add a sub-menu to the current menu, and make it the current menu.
        /// </summary>
        /// <param name="actionId"> A unique identifier for the action. </param>
        /// <param name="labelStr"> The label of the menu item. </param>
        /// <param name="iconName"> The icon of the menu item. </param>
        /// <returns> The MenuBuilder instance. </returns>
        public MenuBuilder PushSubMenu(int actionId, string labelStr, string iconName)
        {
            return PushSubMenu(actionId, labelStr, iconName, null);
        }

        /// <summary>
        /// Create an action menu item, add a sub-menu to the current menu, and make it the current menu.
        /// </summary>
        /// <param name="actionId"> A unique identifier for the action. </param>
        /// <param name="labelStr"> The label of the menu item. </param>
        /// <param name="iconName"> The icon of the menu item. </param>
        /// <param name="subMenuOpenedCallback"> The callback to invoke when the sub-menu is opened. </param>
        /// <returns> The MenuBuilder instance. </returns>
        public MenuBuilder PushSubMenu(int actionId, string labelStr, string iconName, Action subMenuOpenedCallback)
        {
            var subMenu = new Menu();
            var item = new MenuItem
            {
                label = labelStr,
                icon = iconName,
                userData = actionId,
                subMenu = subMenu,
            };
            if (subMenuOpenedCallback != null)
                item.subMenuOpened += subMenuOpenedCallback;
            currentMenu.Add(item);
            m_MenuStack.Push(subMenu);
            return this;
        }

        /// <summary>
        /// Create an action menu item, add a sub-menu to the current menu, and make it the current menu.
        /// </summary>
        /// <param name="bindItemFunc"> A callback to bind the action. </param>
        /// <returns> The MenuBuilder instance. </returns>
        /// <remarks>
        /// When the binding callback is invoked, the sub-menu is already pushed on the stack and set on the menu item.
        /// </remarks>
        public MenuBuilder PushSubMenu(Action<MenuItem> bindItemFunc)
        {
            var subMenu = new Menu();
            var item = new MenuItem { subMenu = subMenu };
            currentMenu.Add(item);
            m_MenuStack.Push(subMenu);
            bindItemFunc?.Invoke(item);
            return this;
        }

        /// <summary>
        /// Go back to the parent menu and make it the current menu.
        /// </summary>
        /// <returns> The MenuBuilder instance. </returns>
        public MenuBuilder Pop()
        {
            m_MenuStack.Pop();
            return this;
        }

        /// <summary>
        /// Close the menu automatically when an item is selected.
        /// </summary>
        public bool closeOnSelection => m_CloseOnSelection;

        /// <summary>
        /// Close the menu automatically when an item is selected.
        /// </summary>
        /// <param name="value"> True to close the menu automatically when an item is selected. </param>
        /// <returns> The MenuBuilder instance. </returns>
        public MenuBuilder SetCloseOnSelection(bool value)
        {
            m_CloseOnSelection = value;
            return this;
        }

        /// <inheritdoc cref="Popup.ShouldAnimate"/>
        protected override bool ShouldAnimate()
        {
            return true;
        }

        /// <inheritdoc cref="Popup.GetFocusableElement"/>
        protected override VisualElement GetFocusableElement()
        {
            return popover.popoverElement;
        }

        /// <inheritdoc cref="AnchorPopup{T}.GetMovableElement"/>
        public override VisualElement GetMovableElement()
        {
            return popover.popoverElement;
        }

        /// <inheritdoc />
        protected override void InvokeShownEventHandlers()
        {
            base.InvokeShownEventHandlers();
            containerView?.panel?.visualTree?.RegisterCallback<PointerDownEvent>(OnTreeDown, TrickleDown.TrickleDown);
            containerView?.panel?.visualTree?.RegisterCallback<WheelEvent>(OnWheel, TrickleDown.TrickleDown);
            popover.RegisterCallback<ActionTriggeredEvent>(OnActionTriggered);
        }

        /// <inheritdoc />
        protected override void HideView(DismissType reason)
        {
            containerView?.panel?.visualTree?.UnregisterCallback<PointerDownEvent>(OnTreeDown, TrickleDown.TrickleDown);
            containerView?.panel?.visualTree?.UnregisterCallback<WheelEvent>(OnWheel, TrickleDown.TrickleDown);
            popover.UnregisterCallback<ActionTriggeredEvent>(OnActionTriggered);
            base.HideView(reason);
        }

        void OnActionTriggered(ActionTriggeredEvent evt)
        {
            if (closeOnSelection || evt.target is MenuItem { selectable: false })
            {
                evt.StopPropagation();
                Dismiss(DismissType.Action);
            }
        }

        /// <summary>
        ///  Create a MenuBuilder instance.
        /// </summary>
        /// <param name="referenceView"> The reference view to position the menu. </param>
        /// <param name="menu"> The menu to display. </param>
        /// <returns> The MenuBuilder instance. </returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="referenceView"/> is null. </exception>
        public static MenuBuilder Build(VisualElement referenceView, Menu menu = null)
        {
            if (referenceView == null)
                throw new ArgumentNullException(nameof(referenceView));

            var dir = referenceView.GetContext<DirContext>()?.dir ?? Dir.Ltr;
            menu ??= new Menu();
            var popoverVisualElement = CreateMenuPopoverVisualElement(menu);
            var menuBuilder = new MenuBuilder(referenceView, popoverVisualElement, menu)
                .SetLastFocusedElement(referenceView)
                .SetArrowVisible(false)
                .SetPlacement(dir == Dir.Ltr ? PopoverPlacement.BottomStart : PopoverPlacement.BottomEnd)
                .SetOutsideClickStrategy(OutsideClickStrategy.Pick)
                .SetCrossOffset(-8)
                .SetAnchor(referenceView);
            menuBuilder.dismissed += (_, _) => menu.CloseSubMenus();

            return menuBuilder;
        }

        internal static Popover.PopoverVisualElement CreateMenuPopoverVisualElement(Menu menu)
        {
            var popoverVisualElement = new Popover.PopoverVisualElement(menu);
            popoverVisualElement.pickingMode = PickingMode.Position;
            popoverVisualElement.AddToClassList(k_MenuPopoverUssClassName);
            popoverVisualElement.AddToClassList(Styles.noArrowUssClassName);
            return popoverVisualElement;
        }
    }
}
