using System;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class ListViewItemPage : StoryBookPage
    {
        public override string displayName => "ListViewItem";

        public override Type componentType => typeof(ListViewItemComponent);
    }

    public class ListViewItemComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(ListViewItem);

        public override void Setup(VisualElement element)
        {
            element.parent.style.alignItems = Align.Stretch;

            ((ListViewItem)element).title = "Title";
            ((ListViewItem)element).subtitle = "Subtitle";
        }

        public ListViewItemComponent()
        {
            m_Properties.Add(new StoryBookStringProperty(
                nameof(ListViewItem.title),
                (item) => ((ListViewItem)item).title,
                (item, val) => ((ListViewItem)item).title = val));

            m_Properties.Add(new StoryBookStringProperty(
                nameof(ListViewItem.subtitle),
                (item) => ((ListViewItem)item).subtitle,
                (item, val) => ((ListViewItem)item).subtitle = val));

            m_Properties.Add(new StoryBookBooleanProperty(
                nameof(ListViewItem.isLoading),
                (item) => ((ListViewItem)item).isLoading,
                (item, val) => ((ListViewItem)item).isLoading = val));

            m_Properties.Add(new StoryBookEnumProperty<Size>(
                nameof(ListViewItem.size),
                (item) => ((ListViewItem)item).size,
                (item, val) => ((ListViewItem)item).size = val));
        }
    }
}
