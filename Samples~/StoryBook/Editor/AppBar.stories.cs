using System;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class AppBarPage : StoryBookPage
    {
        public override string displayName => "AppBar";

        public override Type componentType => typeof(AppBarComponent);

        VisualElement DefaultStory()
        {
            var root = new VisualElement();
            root.style.flexGrow = 1;
            root.style.alignSelf = Align.Stretch;
            var appBar = new AppBar();
            appBar.style.position = Position.Absolute;
            appBar.style.left = 0;
            appBar.style.right = 0;
            appBar.title = "Default";
            appBar.stretch = true;
            appBar.elevation = 10;
            appBar.showDrawerButton = true;
            appBar.expandedHeight = 92;
            appBar.AddAction(new ActionButton { icon = "info", quiet = true } );


            var scrollView = new ScrollView
            {
                style =
                {
                    flexGrow = 1,
                    alignSelf = Align.Stretch,
                    overflow = Overflow.Hidden,
                },
                verticalScrollerVisibility = ScrollerVisibility.Hidden
            };

            root.Add(scrollView);
            root.Add(appBar);

            for (var i = 0; i < 24; i++)
            {
                var item = new Label("Item " + i);
                item.style.paddingLeft = 16;
                item.style.paddingRight = 16;
                item.style.paddingTop = 8;
                item.style.paddingBottom = 8;
                item.style.marginBottom = 8;
                item.style.marginLeft = 16;
                item.style.marginRight = 16;
                item.style.marginTop = 8;
                item.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.15f);
                item.style.borderTopLeftRadius = 4;
                item.style.borderTopRightRadius = 4;
                item.style.borderBottomLeftRadius = 4;
                item.style.borderBottomRightRadius = 4;
                item.style.unityTextAlign = TextAnchor.MiddleLeft;
                scrollView.Add(item);
            }

            scrollView.verticalScroller.valueChanged += (evt) =>
            {
                appBar.scrollOffset = scrollView.verticalScroller.value;
            };

            appBar.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                var h = evt.newRect.height;
                scrollView.style.paddingTop = h;
            });

            return root;
        }

        VisualElement WithBottomElement()
        {
            var root = DefaultStory();
            var appBar = root.Q<AppBar>();
            appBar.title = "With Bottom Element";
            appBar.stretch = true;
            appBar.bottom.style.paddingLeft = 16;
            appBar.bottom.style.paddingRight = 16;
            appBar.bottom.style.paddingTop = 16;
            appBar.bottom.style.paddingBottom = 16;

            var btnContainer = new ExVisualElement();
            btnContainer.AddToClassList("appui-elevation-8");
            var btn = new UI.Button();
            btn.title = "Button";
            btnContainer.Add(btn);

            appBar.bottom.Add(btnContainer);

            return root;
        }

        VisualElement CompactStory()
        {
            var root = DefaultStory();
            var appBar = root.Q<AppBar>();
            appBar.title = "Compact";
            appBar.compact = true;

            return root;
        }

        VisualElement CompactNoStretchStory()
        {
            var root = CompactStory();
            var appBar = root.Q<AppBar>();
            appBar.stretch = false;
            appBar.title = "Compact No Stretch";

            return root;
        }

        public AppBarPage()
        {
            m_Stories.Add(new StoryBookStory("Default", DefaultStory));
            m_Stories.Add(new StoryBookStory("With Bottom Element", WithBottomElement));
            m_Stories.Add(new StoryBookStory("Compact", CompactStory));
            m_Stories.Add(new StoryBookStory("Compact No Stretch", CompactNoStretchStory));
        }
    }

    public class AppBarComponent : StoryBookComponent
    {
        public override Type uiElementType => typeof(AppBar);

        public override void Setup(VisualElement element)
        {
            element.style.alignSelf = Align.Stretch;
        }

        public AppBarComponent()
        {
            m_Properties.Add(new StoryBookStringProperty(
                nameof(AppBar.title),
                (btn) => ((AppBar)btn).title,
                (btn, val) => ((AppBar)btn).title = val));
        }
    }
}
