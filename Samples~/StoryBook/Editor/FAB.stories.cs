using System;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class FABPage : StoryBookPage
    {
        public override string displayName => "Floating Action Button";

        public override Type componentType => typeof(FABComponent);

        VisualElement DefaultStory()
        {
            var root = new VisualElement();
            root.style.flexGrow = 1;
            root.style.alignSelf = Align.Stretch;

            return root;
        }

        public FABPage()
        {
            m_Stories.Add(new StoryBookStory("Default", DefaultStory));
        }
    }

    public class FABComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(FloatingActionButton);

        public override void Setup(VisualElement element)
        {
        }

        public FABComponent()
        {

        }
    }
}
