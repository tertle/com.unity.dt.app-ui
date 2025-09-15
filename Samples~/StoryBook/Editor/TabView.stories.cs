using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class TabViewPage : StoryBookPage
    {
        public override string displayName => "TabView";

        public override Type componentType => null;

        public TabViewPage()
        {
            m_Stories.Add(new StoryBookStory("With ListView", WithListView));
        }

        VisualElement WithListView()
        {
            var root = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    alignSelf = Align.Stretch,
                    alignItems = Align.Stretch,
                }
            };

            var sourceItems = new List<string>();
            for (var i = 0; i < 2; ++i)
            {
                sourceItems.Add($"Tab {i}");
            }
            var tabs = new Tabs
            {
                justified = true,
                sourceItems = sourceItems,
                bindItem = (tab, i) => tab.label = sourceItems[i]
            };
            root.Add(tabs);

            // Populate source list
            var srcItems = new List<string>();
            for (var i = 0; i < 10; ++i)
            {
                srcItems.Add($"Item {i}");
            }

            var grid1 = new GridView
            {
                columnCount = 1,
                itemsSource = srcItems,
                itemHeight = 60,
                selectionType = SelectionType.None,
                makeItem = MakeItem,
                bindItem = BindItem,
                style =
                {
                    flexGrow = 1,
                    alignSelf = Align.Stretch
                }
            };
            grid1.dragger.acceptStartDrag = pos => false;

            var item1 = new SwipeViewItem();
            item1.Add(grid1);

            var grid2 = new GridView
            {
                columnCount = 1,
                itemsSource = srcItems,
                itemHeight = 60,
                selectionType = SelectionType.None,
                makeItem = MakeItem,
                bindItem = BindItem,
                style =
                {
                    flexGrow = 1,
                    alignSelf = Align.Stretch
                }
            };
            grid2.dragger.acceptStartDrag = pos => false;

            var item2 = new SwipeViewItem();
            item2.Add(grid2);

            var swipeView = new SwipeView();
            swipeView.snapAnimationSpeed = 2f;
            swipeView.Add(item1);
            swipeView.Add(item2);

            swipeView.style.flexGrow = 1;
            swipeView.RegisterValueChangedCallback(evt =>
            {
                tabs.SetValueWithoutNotify(evt.newValue);
            });
            tabs.RegisterValueChangedCallback(evt =>
            {
                swipeView.SetValueWithoutNotify(evt.newValue);
            });

            root.Add(swipeView);

            return root;
        }

        void BindItem(VisualElement el, int idx)
        {
            var item = (ListViewItem)el.ElementAt(0);
            item.title = $"Item {idx}";
        }

        static VisualElement MakeItem()
        {
            var element = new VisualElement();
            element.Add(new ListViewItem());
            return element;
        }
    }
}
