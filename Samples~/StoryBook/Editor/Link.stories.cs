using System;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class LinkPage : StoryBookPage
    {
        public override string displayName => "Link";

        public override Type componentType => typeof(LinkComponent);
    }

    public class LinkComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(Link);

        public override void Setup(VisualElement element)
        {
            ((Link)element).text = "Link";
        }

        public LinkComponent()
        {
            m_Properties.Add(new StoryBookStringProperty(
                nameof(Link.text),
                (link) => ((Link)link).text,
                (link, val) => ((Link)link).text = val));
        }
    }
}
