using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Tabs))]
    class TabsTests : VisualElementTests<Tabs>
    {
        protected override string mainUssClassName => Tabs.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                var sourceItems = new List<string>();
                for (var i = 0; i < 2; ++i)
                {
                    sourceItems.Add($"Tab {i}");
                }
                yield return new Story("Default", (ctx) => new Tabs
                {
                    sourceItems = sourceItems,
                    bindItem = (tab, i) =>
                    {
                        tab.label = sourceItems[i];
                        tab.icon = "info";
                    }
                });
                yield return new Story("Justified", (ctx) => new Tabs
                {
                    justified = true,
                    sourceItems = sourceItems,
                    bindItem = (tab, i) =>
                    {
                        tab.label = sourceItems[i];
                        tab.icon = "info";
                    }
                });
                yield return new Story("Vertical", (ctx) => new Tabs
                {
                    direction = Direction.Vertical,
                    sourceItems = sourceItems,
                    bindItem = (tab, i) =>
                    {
                        tab.label = sourceItems[i];
                        tab.icon = "info";
                    }
                });
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:Tabs />",
            @"<appui:Tabs>
                <appui:TabItem label=""Tab 1"" icon=""info"" />
                <appui:TabItem label=""Tab 2"" icon=""info"" enabled=""false"" />
                <appui:TabItem label=""Tab 3"" icon=""info"" />
            </appui:Tabs>",
            @"<appui:Tabs emphasized=""true"" justified=""true"">
                <appui:TabItem label=""Tab 1"" icon=""info"" />
                <appui:TabItem label=""Tab 2"" icon=""info"" enabled=""false"" />
                <appui:TabItem label=""Tab 3"" icon=""info"" />
            </appui:Tabs>",
            @"<appui:Tabs emphasized=""true"" direction=""Vertical"">
                <appui:TabItem label=""Tab 1"" icon=""info"" />
                <appui:TabItem label=""Tab 2"" icon=""info"" enabled=""false"" />
                <appui:TabItem label=""Tab 3"" icon=""info"" />
            </appui:Tabs>",
        };

        [UnityTest]
        public IEnumerator CanBindItems()
        {
            m_TestUI.rootVisualElement.Clear();
            var panel = new Panel();
            m_TestUI.rootVisualElement.Add(panel);

            var sourceItems = new List<string>();
            for (var i = 0; i < 2; ++i)
            {
                sourceItems.Add($"Tab {i}");
            }
            var tabs = new Tabs
            {
                sourceItems = sourceItems,
                bindItem = (tab, i) =>
                {
                    tab.label = sourceItems[i];
                    tab.icon = "info";
                }
            };

            panel.Add(tabs);

            yield return null;

            var container = tabs.Q<VisualElement>(Tabs.containerUssClassName);
            Assert.NotNull(container);
            Assert.AreEqual(2, container.childCount);
            Assert.IsTrue(container[0] is TabItem);
            Assert.AreEqual("Tab 0", ((TabItem)container[0]).label);
            Assert.AreEqual(0, tabs.value);
        }
    }
}
