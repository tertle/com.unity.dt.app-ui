using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
#if UNITY_LOCALIZATION_PRESENT
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(LocalizedTextElement))]
    class LocalizedTextElementTests : VisualElementTests<LocalizedTextElement>
    {
        protected override string mainUssClassName => LocalizedTextElement.ussClassName;

        protected override IEnumerable<string> uxmlTestCases
        {
            get
            {
                yield return "<appui:LocalizedTextElement/>";
                yield return "<appui:LocalizedTextElement text=\"Hello\"/>";
                yield return "<appui:LocalizedTextElement text=\"@UI:hello\" />";
            }
        }

        [UnityTest, Order(10)]
        public IEnumerator CanLocalizeText()
        {
            const string table = "UI";
            const string key = "hello";
            const string frenchLang = "fr";
            const string englishLang = "en";

            m_TestUI.rootVisualElement.Clear();
            m_Panel = new Panel();

            var contextReceived = (string)null;
            m_Panel.RegisterContextChangedCallback<LangContext>(e =>
            {
                contextReceived = e.context?.lang;
            });

            m_TestUI.rootVisualElement.Add(m_Panel);
            m_Panel.StretchToParentSize();

            yield return null;

            Assert.AreEqual(englishLang, m_Panel.lang);
            Assert.AreEqual(Panel.defaultLang, m_Panel.lang);
            Assert.AreEqual(null, contextReceived,
                "Context should not have been received yet because it has not changed");
            var ctx = m_Panel.GetContext<LangContext>();
            var isProvider = m_Panel.IsContextProvider<LangContext>();
            Assert.AreEqual(Panel.defaultLang, ctx.lang);
            Assert.IsTrue(isProvider);

            var localizedTextElement = new LocalizedTextElement($"@{table}:{key}");
            m_Panel.Add(localizedTextElement);
            localizedTextElement.StretchToParentSize();
            yield return null;

            Assert.AreEqual(Panel.defaultLang, m_Panel.lang);
            Assert.AreEqual(null, contextReceived,
                "Context should not have been received yet because it has not changed");

            var localizedEnglishText = $"@{table}:{key}";
            var localizedFrenchText = $"@{table}:{key}";

#if UNITY_LOCALIZATION_PRESENT

            var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();

            if (localizationSettings && localizationSettings.GetStringDatabase() is {} db)
            {
                var op = db.GetTableEntryAsync(table, key, GetLocaleForLang(ctx.lang));

                while (!op.IsDone)
                {
                    yield return null;
                }

                if (op is {Status: AsyncOperationStatus.Succeeded, Result: {} res})
                {
                    localizedEnglishText = res.Entry?.GetLocalizedString() ?? localizedEnglishText;
                }

                var frenchCtx = new LangContext(frenchLang);
                op = db.GetTableEntryAsync(table, key, GetLocaleForLang(frenchCtx.lang));

                while (!op.IsDone)
                {
                    yield return null;
                }

                if (op is {Status: AsyncOperationStatus.Succeeded, Result: {} fRes})
                {
                    localizedFrenchText = fRes.Entry?.GetLocalizedString() ?? localizedFrenchText;
                }
            }

#endif

            Assert.AreEqual("@UI:hello", localizedTextElement.text,
                "The text property must return the reference text and not the localized text");

            Assert.AreEqual(localizedEnglishText, localizedTextElement.localizedText,
                "The localizedText property must return the localized text");


            m_Panel.lang = frenchLang;
            yield return null;

            Assert.AreEqual(frenchLang, m_Panel.lang);
            Assert.AreEqual(frenchLang, contextReceived,
                "Context should have been received because it has changed");

            Assert.AreEqual("@UI:hello", localizedTextElement.text,
                "The text property must return the reference text and not the localized text");

            yield return new WaitUntilOrTimeOut(
                () => localizedTextElement.localizedText == "Bonjour",
                false,
                TimeSpan.FromSeconds(2));

            Assert.AreEqual(localizedFrenchText, localizedTextElement.localizedText,
                "The localizedText property must return the localized text");
        }

#if UNITY_LOCALIZATION_PRESENT
        static Locale GetLocaleForLang(string lang)
        {
            var settings = LocalizationSettings.GetInstanceDontCreateDefault();
            if (!settings)
                return null;

            var globalLocale = settings.GetSelectedLocaleAsync();
            if (!globalLocale.IsDone)
                return null;

            if (string.IsNullOrEmpty(lang))
                return globalLocale.Result;

            var availableLocales = settings.GetAvailableLocales();
            if (availableLocales is LocalesProvider localesProvider &&
                (!localesProvider.PreloadOperation.IsValid() || !localesProvider.PreloadOperation.IsDone))
                return null;

            var scopedLocale = availableLocales.GetLocale(lang);
            return scopedLocale ? scopedLocale : globalLocale.Result;
        }
#endif
    }
}
