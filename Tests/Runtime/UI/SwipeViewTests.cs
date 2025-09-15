using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(SwipeView))]
    class SwipeViewTests : VisualElementTests<SwipeView>
    {
        protected override string mainUssClassName => SwipeView.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:SwipeView />",
            @"<appui:SwipeView skip-animation-threshold=""2"" style=""width: 200px; height: 400px;"" direction=""Horizontal"" animation-speed=""1"" wrap=""true"" visible-item-count=""2"" start-swipe-threshold=""1"" auto-play-duration=""-1"" swipeable=""true"" resistance=""10"">
                <appui:SwipeViewItem>
                    <appui:Button title=""Item 1"" />
                </appui:SwipeViewItem>
                <appui:SwipeViewItem>
                    <appui:Text text=""Item 2"" />
                </appui:SwipeViewItem>
                <appui:SwipeViewItem>
                    <appui:Text text=""Item 3"" />
                </appui:SwipeViewItem>
                <appui:SwipeViewItem>
                    <appui:Text text=""Item 4"" />
                </appui:SwipeViewItem>
                <appui:SwipeViewItem>
                    <appui:Text text=""Item 5"" />
                </appui:SwipeViewItem>
            </appui:SwipeView>"
        };

        /// <summary>
        /// Time out for the tests in seconds.
        /// </summary>
        const double k_TimeOut = 2;

        [UnityTest]
        public IEnumerator CanSwipe()
        {
            m_TestUI.rootVisualElement.Clear();
            var panel = new Panel();
            var swipeView = new SwipeView
            {
                style =
                {
                    width = 100,
                    height = 100
                }
            };

            var item1 = new SwipeViewItem();
            var text1 = new Text { text = "Item 1" };
            item1.Add(text1);

            var item2 = new SwipeViewItem();
            var text2 = new Text { text = "Item 2" };
            item2.Add(text2);

            var item3 = new SwipeViewItem();
            var text3 = new Text { text = "Item 3" };
            item3.Add(text3);

            swipeView.Add(item1);
            swipeView.Add(item2);
            swipeView.Add(item3);

            m_TestUI.rootVisualElement.Add(panel);
            panel.Add(swipeView);

            yield return new WaitUntilOrTimeOut(() => swipeView.count == 3,
                true, TimeSpan.FromSeconds(k_TimeOut));

            item1 = swipeView.ElementAt(0) as SwipeViewItem;
            item2 = swipeView.ElementAt(1) as SwipeViewItem;
            item3 = swipeView.ElementAt(2) as SwipeViewItem;

            Assert.IsNotNull(item1);
            Assert.IsNotNull(item2);
            Assert.IsNotNull(item3);

            yield return new WaitUntilOrTimeOut(() => Mathf.Abs(item1.worldBound.width - 100f) < 1f,
                true, TimeSpan.FromSeconds(k_TimeOut));

            Assert.AreEqual(0, swipeView.value);

            var gone = swipeView.GoTo(1);

            Assert.IsTrue(gone);
            Assert.AreEqual(1, swipeView.value);

            yield return new WaitUntilOrTimeOut(() =>
            {
                var pos = swipeView.WorldToLocal(item2.worldBound.min);
                return Mathf.Abs(pos.x) < 1f;
            }, true, TimeSpan.FromSeconds(k_TimeOut));

            Assert.AreEqual(1, swipeView.value);

            gone = swipeView.GoToNext();

            Assert.IsTrue(gone);
            Assert.AreEqual(2, swipeView.value);

            yield return new WaitUntilOrTimeOut(() =>
            {
                var pos = swipeView.WorldToLocal(item3.worldBound.min);
                return Mathf.Abs(pos.x) < 1f;
            }, true, TimeSpan.FromSeconds(k_TimeOut));

            Assert.AreEqual(2, swipeView.value);

            gone = swipeView.GoToPrevious();

            Assert.IsTrue(gone);
            Assert.AreEqual(1, swipeView.value);

            yield return new WaitUntilOrTimeOut(() =>
            {
                var pos = swipeView.WorldToLocal(item2.worldBound.min);
                return Mathf.Abs(pos.x) < 1f;
            }, true, TimeSpan.FromSeconds(k_TimeOut));

            Assert.AreEqual(1, swipeView.value);

            Assert.IsFalse(swipeView.wrap);

            swipeView.wrap = true;

            Assert.IsTrue(swipeView.wrap);

            swipeView.value = 2;

            Assert.AreEqual(2, swipeView.value);

            yield return new WaitUntilOrTimeOut(() =>
            {
                var pos = swipeView.WorldToLocal(item3.worldBound.min);
                return Mathf.Abs(pos.x) < 1f;
            }, true, TimeSpan.FromSeconds(k_TimeOut));

            Assert.AreEqual(2, swipeView.value);

            gone = swipeView.GoToNext();

            Assert.IsTrue(gone);
            Assert.AreEqual(0, swipeView.value);

            yield return null;

            yield return new WaitUntilOrTimeOut(() =>
            {
                var pos = swipeView.WorldToLocal(item1.worldBound.min);
                return Mathf.Abs(pos.x) < 1f;
            }, true, TimeSpan.FromSeconds(k_TimeOut));
        }

        [UnityTest]
        public IEnumerator AutoPlay_ShouldSwipeOverTime()
        {
            m_TestUI.rootVisualElement.Clear();
            var panel = new Panel();
            var swipeView = CreateSwipeView();
            swipeView.autoPlayDuration = 500;
            swipeView.wrap = true;

            m_TestUI.rootVisualElement.Add(panel);
            panel.Add(swipeView);

            Assert.AreEqual(0, swipeView.value);

            yield return new WaitUntilOrTimeOut(() => swipeView.value == 1,
                true, TimeSpan.FromSeconds(k_TimeOut));

            yield return new WaitUntilOrTimeOut(() => swipeView.value == 2,
                true, TimeSpan.FromSeconds(k_TimeOut));

            yield return new WaitUntilOrTimeOut(() => swipeView.value == 0,
                true, TimeSpan.FromSeconds(k_TimeOut));
        }

        [Test]
        public void GoTo_InvalidIndex_ReturnsFalse()
        {
            var swipeView = CreateSwipeView();

            Assert.IsFalse(swipeView.GoTo(3));
            Assert.IsFalse(swipeView.GoTo(-1));
        }

        [Test]
        public void GoPrevious_InvalidIndex_ReturnsFalse()
        {
            var swipeView = CreateSwipeView();

            Assert.IsFalse(swipeView.canGoToPrevious);
            Assert.IsFalse(swipeView.GoToPrevious());
        }

        [Test]
        public void GoNext_InvalidIndex_ReturnsFalse()
        {
            var swipeView = CreateSwipeView();

            Assert.IsTrue(swipeView.canGoToNext);
            Assert.IsTrue(swipeView.GoToNext());

            Assert.IsTrue(swipeView.canGoToNext);
            Assert.IsTrue(swipeView.GoToNext());

            Assert.IsFalse(swipeView.canGoToNext);
            Assert.IsFalse(swipeView.GoToNext());
        }

        static SwipeView CreateSwipeView()
        {
            var swipeView = new SwipeView
            {
                unbindItem = (item, index) => item.Clear(),
                bindItem = (item, index) => item.Add(new Text { text = $"Item {index}" }),
                sourceItems = new List<int> { 0, 1, 2 },
                style =
                {
                    width = 100,
                    height = 100
                }
            };

            Assert.AreEqual(3, swipeView.count);

            return swipeView;
        }
    }
}
