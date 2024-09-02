using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(GridView))]
    class GridViewTests : VisualElementTests<GridView>
    {
        protected override string mainUssClassName => BaseGridView.ussClassName;

        protected override bool uxmlConstructable => true;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", _ => new GridView
                {
                    style =
                    {
                        height = 400,
                    },
                    makeItem = () => new Text(),
                    bindItem = (e, i) => ((Text)e).text = $"Item {i}",
                    itemsSource = Enumerable.Range(0, 50).ToList(),
                });

                yield return new Story("ThreeColumns", _ => new GridView
                {
                    style =
                    {
                        height = 400,
                    },
                    columnCount = 3,
                    makeItem = () => new Text(),
                    bindItem = (e, i) => ((Text)e).text = $"Item {i}",
                    itemsSource = Enumerable.Range(0, 50).ToList(),
                });
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:GridView />",
            @"<appui:GridView item-height=""100"" selection-type=""Multiple"" allow-no-selection=""true"" prevent-scroll-with-modifiers=""true""  />",
        };

        [UnityTest, Order(10)]
        public IEnumerator CanConstructGridView()
        {
            if (!Application.isEditor)
            {
                // skip test and mark as ignored
                Assert.Ignore("Can't run this test outside of the editor");
                yield break;
            }

            GridView gridView = null;

            Assert.DoesNotThrow(() =>
            {
                gridView = new GridView(itemsSource: new List<int>() {1, 2, 3, 4, 5}, makeItem: () => new Text(),
                    bindItem: (e, i) => ((Text) e).text = i.ToString());
            });

            Assert.DoesNotThrow(() =>
            {
                gridView = new GridView
                {
                    makeItem = () => new Text(),
                    bindItem = (e, i) => ((Text)e).text = i.ToString(),
                    itemHeight = 100,
                    columnCount = 5,
                    selectionType = SelectionType.Multiple,
                    allowNoSelection = true,
                    preventScrollWithModifiers = true
                };
                gridView.selectionChanged += _ => { };
                gridView.itemsSource = Enumerable.Range(1,1000).ToList();

            });

            Assert.NotNull(gridView);

            m_TestUI.rootVisualElement.Clear();
            m_Panel = new Panel();
            m_Panel.Add(gridView);
            m_TestUI.rootVisualElement.Add(m_Panel);
            gridView.StretchToParentSize();
            m_Panel.StretchToParentSize();

            yield return null;

            gridView.selectedIndex = 500;

            yield return null;

            Assert.AreEqual(500, gridView.selectedIndex);

            gridView.ScrollToItem(500);

            yield return new WaitForSeconds(0.2f);

            gridView.scrollView.verticalScroller.value = gridView.scrollView.verticalScroller.lowValue;

            yield return new WaitForSeconds(0.2f);

            gridView.scrollView.verticalScroller.value = gridView.scrollView.verticalScroller.highValue;

            yield return new WaitForSeconds(0.2f);

            gridView.selectionType = SelectionType.None;

            yield return null;

            Assert.AreEqual(SelectionType.None, gridView.selectionType);
            Assert.AreEqual(-1, gridView.selectedIndex);

            m_Panel = null;
            m_TestUI.rootVisualElement.Clear();
        }
    }
}
