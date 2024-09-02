using System;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class MenuItemPage : StoryBookPage
    {
        public override string displayName => "MenuItem";

        public override Type componentType => typeof(MenuItemComponent);
    }

    public class MenuItemComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(MenuItem);

        public override void Setup(VisualElement element)
        {
            element.parent.style.alignItems = Align.Stretch;

            ((MenuItem)element).label = "Information";
            ((MenuItem)element).icon = "info";
        }

        public MenuItemComponent()
        {
            m_Properties.Add(new StoryBookStringProperty(
                nameof(MenuItem.label),
                (item) => ((MenuItem)item).label,
                (item, val) => ((MenuItem)item).label = val));

            m_Properties.Add(new StoryBookStringProperty(
                nameof(MenuItem.icon),
                (item) => ((MenuItem)item).icon,
                (item, val) => ((MenuItem)item).icon = val));

        }
    }
}
