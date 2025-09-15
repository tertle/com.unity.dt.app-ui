using System;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class ActionBarPage : StoryBookPage
    {
        public override string displayName => "ActionBar";

        public override Type componentType => null;

        VisualElement DefaultStory()
        {
            var root = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    alignSelf = Align.Stretch
                }
            };

            var popover = new VisualElement();
            popover.AddToClassList("appui-popover__popover");
            root.Add(popover);

            var popoverContainer = new VisualElement();
            popoverContainer.AddToClassList("appui-popover__container");
            popover.Add(popoverContainer);

            var actionBar = new ActionBar();
            popoverContainer.Add(actionBar);

            var exportButton = new ActionButton { label = "Export" };
            exportButton.clicked += () => Debug.Log("Export button clicked");
            actionBar.Add(exportButton);

            var deleteButton = new ActionButton { label = "Delete" };
            deleteButton.clicked += () => Debug.Log("Delete button clicked");
            actionBar.Add(deleteButton);

            return root;
        }

        public ActionBarPage()
        {
            m_Stories.Add(new StoryBookStory("Default", DefaultStory));
        }
    }
}
