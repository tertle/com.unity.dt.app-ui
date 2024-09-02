using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Menu UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Menu : BaseVisualElement
    {
        /// <summary>
        /// The Menu main styling class.
        /// </summary>
        public const string ussClassName = "appui-menu";

        /// <summary>
        /// The Menu container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The Menu selectable mode styling class.
        /// </summary>
        public const string selectableUssClassName = ussClassName + "--selectable";

        readonly ScrollView m_ScrollView;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Menu()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Ignore;

            m_ScrollView = new ScrollView
            {
                name = containerUssClassName,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden,
                verticalScrollerVisibility = ScrollerVisibility.Auto
            };
            m_ScrollView.AddToClassList(containerUssClassName);
            hierarchy.Add(m_ScrollView);

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        /// <summary>
        /// The content container of the Menu.
        /// </summary>
        public override VisualElement contentContainer => m_ScrollView.contentContainer;

        /// <summary>
        /// The parent MenuItem of this Menu (if this Menu is a sub-menu).
        /// </summary>
        public MenuItem parentItem { get; internal set; } = null;

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (panel != null)
            {
                var query = this.Query<MenuItem>().Where(item => item.selectable).Build();
                EnableInClassList(selectableUssClassName, query.ToList().Count > 0);
            }
        }

        /// <summary>
        /// Close all sub-menus.
        /// </summary>
        public void CloseSubMenus()
        {
            foreach (var child in Children())
            {
                switch (child)
                {
                    case MenuSection menuSection:
                    {
                        foreach (var menuItem in menuSection.GetChildren<MenuItem>(false))
                        {
                            if (menuItem.subMenu != null)
                                menuItem.CloseSubMenus(Vector2.negativeInfinity, menuItem.subMenu);
                        }

                        break;
                    }
                    case MenuItem menuItem:
                    {
                        if (menuItem.subMenu != null)
                            menuItem.CloseSubMenus(Vector2.negativeInfinity, menuItem.subMenu);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Get all the MenuItems of this Menu (including sub-menus).
        /// </summary>
        /// <returns> The list of MenuItems. </returns>
        public IEnumerable<MenuItem> GetMenuItems()
        {
            return this.GetChildren<MenuItem>(true);
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Class to be able to instantiate a <see cref="Menu"/> from UXML.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Menu, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Menu"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
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
            }
        }
#endif
    }
}
