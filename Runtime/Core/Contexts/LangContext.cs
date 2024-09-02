#if UNITY_LOCALIZATION_PRESENT
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Unity.AppUI.Core
{
    /// <summary>
    /// The Lang context of the application.
    /// </summary>
    /// <param name="lang"> The language. </param>
    public record LangContext(string lang) : IContext
    {
        /// <summary>
        /// The current language.
        /// </summary>
        public string lang { get; } = lang;

#if UNITY_LOCALIZATION_PRESENT
        /// <summary>
        /// The current locale.
        /// </summary>
        public Locale locale => GetLocaleForLang(lang);

        public static Locale GetLocaleForLang(string lang)
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
