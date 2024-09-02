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
    [TestOf(typeof(MasonryGridView))]
    class MasonryGridViewTests : VisualElementTests<MasonryGridView>
    {
        protected override string mainUssClassName => BaseGridView.ussClassName;

        protected override bool uxmlConstructable => true;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", _ => new MasonryGridView
                {
                    style =
                    {
                        height = 400,
                    },
                    makeItem = () => new Text(),
                    bindItem = (e, i) =>
                    {
                        var length = Mathf.Min(i * 10, Utils.loremIpsum.Length - 10);
                        var loremIpsumPart = Utils.loremIpsum.Substring(10, length);
                        ((Text)e).text = loremIpsumPart;
                    },
                    itemsSource = Enumerable.Range(0, 50).ToList(),
                });

                yield return new Story("ThreeColumns", _ => new MasonryGridView
                {
                    style =
                    {
                        height = 400,
                    },
                    columnCount = 3,
                    makeItem = () => new Text(),
                    bindItem = (e, i) =>
                    {
                        var length = Mathf.Min(i * 10, Utils.loremIpsum.Length - 10);
                        var loremIpsumPart = Utils.loremIpsum.Substring(10, length);
                        ((Text)e).text = loremIpsumPart;
                    },
                    itemsSource = Enumerable.Range(0, 50).ToList(),
                });
            }
        }

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:MasonryGridView />",
            @"<appui:MasonryGridView selection-type=""Multiple"" allow-no-selection=""true"" prevent-scroll-with-modifiers=""true""  />",
        };

        [UnityTest, Order(10)]
        public IEnumerator CanConstructGridView()
        {
            yield break;
        }
    }
}
