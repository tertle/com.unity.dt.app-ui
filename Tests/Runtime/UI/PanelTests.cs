using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.Core;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Panel))]
    class PanelTests : VisualElementTests<Panel>
    {
        protected override string mainUssClassName => Panel.ussClassName;

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield break;
            }
        }

        [Test]
        public void Panel_ShouldHaveDefaultContext()
        {
            var panel = new Panel();
            Assert.AreEqual(Panel.defaultLang, panel.lang);
            Assert.AreEqual(Panel.defaultTheme, panel.theme);
            Assert.AreEqual(Panel.defaultScale, panel.scale);
            Assert.AreEqual(Panel.defaultDir, panel.layoutDirection);

            Assert.AreEqual(Panel.defaultLang, panel.GetContext<LangContext>().lang);
            Assert.AreEqual(Panel.defaultTheme, panel.GetContext<ThemeContext>().theme);
            Assert.AreEqual(Panel.defaultScale, panel.GetContext<ScaleContext>().scale);
            Assert.AreEqual(Panel.defaultDir, panel.GetContext<DirContext>().dir);

            Assert.IsTrue(panel.ClassListContains(Panel.contextPrefix + Panel.defaultLang));
            Assert.IsTrue(panel.ClassListContains(Panel.contextPrefix + Panel.defaultTheme));
            Assert.IsTrue(panel.ClassListContains(Panel.contextPrefix + Panel.defaultScale));
            Assert.IsTrue(panel.ClassListContains(Panel.contextPrefix + Panel.defaultDir.ToString().ToLower()));
        }

        [Test]
        [TestCase("fr", "dark", "small", Dir.Rtl)]
        [TestCase("de", "light", "medium", Dir.Ltr)]
        [TestCase("en", "light", "large", Dir.Rtl)]
        [TestCase("en", "dark", "small", Dir.Ltr)]
        public void Panel_WithInitializers_ShouldHaveNewContext(string lang, string theme, string scale, Dir dir)
        {
            var panel = new Panel()
            {
                lang = lang,
                theme = theme,
                scale = scale,
                layoutDirection = dir
            };
            Assert.AreEqual(lang, panel.lang);
            Assert.AreEqual(theme, panel.theme);
            Assert.AreEqual(scale, panel.scale);
            Assert.AreEqual(dir, panel.layoutDirection);

            Assert.AreEqual(lang, panel.GetContext<LangContext>().lang);
            Assert.AreEqual(theme, panel.GetContext<ThemeContext>().theme);
            Assert.AreEqual(scale, panel.GetContext<ScaleContext>().scale);
            Assert.AreEqual(dir, panel.GetContext<DirContext>().dir);

            Assert.IsTrue(panel.ClassListContains(Panel.contextPrefix + lang));
            Assert.IsTrue(panel.ClassListContains(Panel.contextPrefix + theme));
            Assert.IsTrue(panel.ClassListContains(Panel.contextPrefix + scale));
            Assert.IsTrue(panel.ClassListContains(Panel.contextPrefix + dir.ToString().ToLower()));
        }
    }
}
