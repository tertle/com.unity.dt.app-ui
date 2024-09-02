using System;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class DrawerHeaderPage : StoryBookPage
    {
        public override string displayName => "DrawerHeader";

        public override Type componentType => typeof(DrawerHeaderComponent);
    }

    public class DrawerHeaderComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(DrawerHeader);

        public override void Setup(VisualElement element)
        {
            element.parent.style.alignItems = Align.Stretch;

            ((DrawerHeader)element).title = "Title";
        }

        public DrawerHeaderComponent()
        {
            m_Properties.Add(new StoryBookStringProperty(
                nameof(DrawerHeader.title),
                (item) => ((DrawerHeader)item).title,
                (item, val) => ((DrawerHeader)item).title = val));

        }
    }
}
