using System;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class DrawerPage : StoryBookPage
    {
        public override string displayName => "Drawer";

        public override Type componentType => typeof(DrawerComponent);
    }

    public class DrawerComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(Drawer);

        public override void Setup(VisualElement element)
        {
            element.parent.style.alignItems = Align.Stretch;

            ((Drawer)element).anchor = DrawerAnchor.Left;
            ((Drawer)element).elevation = 2;
            ((Drawer)element).swipeable = true;
            ((Drawer)element).variant = DrawerVariant.Temporary;
            ((Drawer)element).Add(new DrawerHeader { title = "A Drawer Title" });
        }

        public DrawerComponent()
        {


        }
    }
}
