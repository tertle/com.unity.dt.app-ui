using System;
using System.Linq;
using NUnit.Framework;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine.UIElements;
using VisualElementExtensions = Unity.AppUI.UI.VisualElementExtensions;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(VisualElementExtensions))]
    class VisualElementExtensionsTests
    {
        [Test]
        public void VisualElementExtensions_GetChildren_ShouldReturnChildren()
        {
            var v = new VisualElement();
            var c1 = new VisualElement();
            var c2 = new VisualElement();
            var c3 = new VisualElement();
            v.Add(c1);
            v.Add(c2);
            v.Add(c3);

            var children = v.GetChildren<VisualElement>(true).ToList();
            Assert.AreEqual(3, children.Count);
            Assert.AreEqual(c1, children[0]);
            Assert.AreEqual(c2, children[1]);
            Assert.AreEqual(c3, children[2]);
        }

        [Test]
        public void VisualElementExtensions_GetContext_ShouldReturnContextFromApplication()
        {
            var v = new Panel();

            LangContext langContext = default;
            ThemeContext themeContext = default;
            ScaleContext scaleContext = default;

            Assert.DoesNotThrow(() =>
            {
                langContext = v.GetContext<LangContext>();
                themeContext = v.GetContext<ThemeContext>();
                scaleContext = v.GetContext<ScaleContext>();
            });

            Assert.AreEqual(v.lang, langContext.lang);
            Assert.AreEqual(v.theme, themeContext.theme);
            Assert.AreEqual(v.scale, scaleContext.scale);
        }

        [Test]
        [TestCase("fr", "dark")]
        [TestCase("de", "light")]
        [TestCase("en", "light")]
        [TestCase("en", "dark")]
        public void VisualElementExtensions_GetContext_ShouldComputeContextWithOverrides(string lang, string theme)
        {
            var v = new Panel();
            var overrideElement = new VisualElement();
            overrideElement.ProvideContext(new LangContext(lang));
            overrideElement.ProvideContext(new ThemeContext(theme));

            Assert.AreEqual(lang, overrideElement.GetSelfContext<LangContext>().lang);
            Assert.AreEqual(theme, overrideElement.GetSelfContext<ThemeContext>().theme);
            v.Add(overrideElement);

            LangContext langContext = default;
            ThemeContext themeContext = default;
            ScaleContext scaleContext = default;

            Assert.DoesNotThrow(() =>
            {
                langContext = overrideElement.GetContext<LangContext>();
                themeContext = overrideElement.GetContext<ThemeContext>();
                scaleContext = overrideElement.GetContext<ScaleContext>();
            });
            Assert.AreEqual(overrideElement.GetSelfContext<LangContext>().lang, langContext.lang);
            Assert.AreEqual(overrideElement.GetSelfContext<ThemeContext>().theme, themeContext.theme);
            Assert.AreEqual(v.scale, scaleContext.scale);
        }

        [Test]
        public void VisualElementExtensions_SetTooltipTemplate_ShouldSucceed()
        {
            var v = new VisualElement();
            var changed = 0;
            var tooltipTemplate = new VisualElement();

            void OnTooltipTemplateChanged()
            {
                changed++;
            }

            Assert.Throws<ArgumentNullException>(() => v.RegisterTooltipTemplateChangedCallback(null));
            Assert.Throws<ArgumentNullException>(() => VisualElementExtensions.RegisterTooltipTemplateChangedCallback(null, OnTooltipTemplateChanged));
            v.RegisterTooltipTemplateChangedCallback(OnTooltipTemplateChanged);

            v.SetTooltipTemplate(tooltipTemplate);
            Assert.AreEqual(tooltipTemplate, v.GetTooltipTemplate());
            Assert.AreEqual(1, changed);

            Assert.Throws<ArgumentNullException>(() => VisualElementExtensions.SetTooltipTemplate(null, tooltipTemplate));
            Assert.DoesNotThrow(() => VisualElementExtensions.SetTooltipTemplate(v, null));
            Assert.AreEqual(null, v.GetTooltipTemplate());
            Assert.AreEqual(2, changed);

            Assert.Throws<ArgumentNullException>(() => v.UnregisterTooltipTemplateChangedCallback(null));
            Assert.Throws<ArgumentNullException>(() => VisualElementExtensions.UnregisterTooltipTemplateChangedCallback(null, OnTooltipTemplateChanged));
            v.UnregisterTooltipTemplateChangedCallback(OnTooltipTemplateChanged);
            v.SetTooltipTemplate(tooltipTemplate);
            Assert.AreEqual(2, changed);
        }

        [Test]
        public void VisualElementExtensions_SetTooltipContent_ShouldSucceed()
        {
            var v = new VisualElement();
            var template = new Text();

            var changed = 0;

            void OnTooltipContentChanged()
            {
                changed++;
            }

            Assert.Throws<ArgumentNullException>(() => v.RegisterTooltipContentChangedCallback(null));
            Assert.Throws<ArgumentNullException>(() => VisualElementExtensions.RegisterTooltipContentChangedCallback(null, OnTooltipContentChanged));
            v.RegisterTooltipContentChangedCallback(OnTooltipContentChanged);

            void TemplateContent(VisualElement t)
            {
                ((Text)t).text = "Tooltip";
            }

            Assert.Throws<InvalidOperationException>(() => v.SetTooltipContent(TemplateContent));
            v.SetTooltipTemplate(template);
            v.SetTooltipContent(TemplateContent);
            Assert.AreEqual((VisualElementExtensions.TooltipContentCallback)TemplateContent, v.GetTooltipContent());
            Assert.AreEqual(1, changed);

            Assert.Throws<ArgumentNullException>(() => VisualElementExtensions.SetTooltipContent(null, TemplateContent));
            Assert.DoesNotThrow(() => VisualElementExtensions.SetTooltipContent(v, null));
            Assert.AreEqual(null, v.GetTooltipContent());
            Assert.AreEqual(2, changed);

            Assert.Throws<ArgumentNullException>(() => v.UnregisterTooltipContentChangedCallback(null));
            Assert.Throws<ArgumentNullException>(() => VisualElementExtensions.UnregisterTooltipContentChangedCallback(null, OnTooltipContentChanged));
            v.UnregisterTooltipContentChangedCallback(OnTooltipContentChanged);
            v.SetTooltipContent(TemplateContent);
            Assert.AreEqual(2, changed);
        }
    }
}
