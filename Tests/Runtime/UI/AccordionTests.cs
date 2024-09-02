using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine.TestTools;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Accordion))]
    class AccordionTests : VisualElementTests<Accordion>
    {
        protected override string mainUssClassName => Accordion.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            "<appui:Accordion/>",
            @"<appui:Accordion is-exclusive=""true"">
                <appui:AccordionItem title=""Item 1"">
                    <appui:Button text=""Button 1"" />
                </appui:AccordionItem>
                <appui:AccordionItem title=""Item 2"">
                    <appui:Button text=""Button 2"" />
                </appui:AccordionItem>
             </appui:Accordion>",
        };
    }

    [TestFixture]
    [TestOf(typeof(AccordionItem))]
    class AccordionItemTests : VisualElementTests<AccordionItem>
    {
        protected override string mainUssClassName => AccordionItem.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            "<appui:AccordionItem/>",
            @"<appui:AccordionItem title=""Item 1"" value=""true"">
                <appui:Button text=""Button 1"" />
             </appui:AccordionItem>",
            @"<appui:AccordionItem title=""Item 2"" value=""false"">
                <appui:Button text=""Button 2"" />
             </appui:AccordionItem>",
        };

        [Test, Order(2)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        [TestCase("A title")]
        public void AccordionItem_Title_ShouldBeSettable(string title)
        {
            var expected = title;
            element.title = title;
            Assert.AreEqual(expected, element.title);
        }

        [Test, Order(2)]
        public void AccordionItem_SubmitEvent_ShouldBeHandled()
        {
            //todo simulate submitevent
        }

        [Test, Order(2)]
        public void AccordionItem_ClickEvent_ShouldBeHandled()
        {
            //todo simulate clickevent
        }

        [UnityTest]
        [Order(10)]
        public IEnumerator AccordionItem_IndicatorPosition_ShouldBeSettable()
        {
            m_TestUI.rootVisualElement.Clear();
            var panel = new Panel();
            var accordion = new Accordion();
            var accordionItem = new AccordionItem();
            m_TestUI.rootVisualElement.Add(panel);
            panel.Add(accordion);
            accordion.Add(accordionItem);

            yield return null;

            Assert.AreEqual(FlexPosition.End, accordionItem.indicatorPosition);
            var headerElement = accordionItem.hierarchy.ElementAt(0).hierarchy.ElementAt(0);
            Assert.NotNull(headerElement);
            Assert.AreEqual(AccordionItem.headerUssClassName, headerElement.name);
            var indicatorElement = headerElement.hierarchy.ElementAt(headerElement.hierarchy.childCount - 1);
            Assert.NotNull(indicatorElement);
            Assert.AreEqual(AccordionItem.indicatorUssClassName, indicatorElement.name);

            accordionItem.indicatorPosition = FlexPosition.Start;

            yield return null;

            Assert.AreEqual(FlexPosition.Start, accordionItem.indicatorPosition);
            Assert.AreEqual(0, headerElement.hierarchy.IndexOf(indicatorElement));

            accordionItem.indicatorPosition = FlexPosition.Start;

            yield return null;

            Assert.AreEqual(FlexPosition.Start, accordionItem.indicatorPosition);
            Assert.AreEqual(0, headerElement.hierarchy.IndexOf(indicatorElement));
        }
    }
}
