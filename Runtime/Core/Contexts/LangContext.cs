using System;
using System.Threading.Tasks;

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

        /// <summary>
        /// The delegate to get a localized string asynchronously.
        /// </summary>
        /// <param name="referenceText"> The reference text. </param>
        /// <param name="lang"> The language. </param>
        /// <param name="arguments"> The arguments to format the string. </param>
        /// <returns> The localized string. </returns>
        public delegate Task<string> GetLocalizedStringAsyncDelegate(string referenceText, string lang, params object[] arguments);

        /// <summary>
        /// The delegate to get a localized string.
        /// </summary>
        public GetLocalizedStringAsyncDelegate GetLocalizedStringAsyncFunc { get; set; }

        /// <summary>
        /// Get a localized string asynchronously.
        /// </summary>
        /// <param name="referenceText"> The reference text. </param>
        /// <param name="arguments"> The arguments to format the string. </param>
        /// <returns> The localized string. </returns>
        internal Task<string> GetLocalizedStringAsync(string referenceText, params object[] arguments)
        {
            if (GetLocalizedStringAsyncFunc != null)
                return GetLocalizedStringAsyncFunc.Invoke(referenceText, lang, arguments);
#if UNITY_LOCALIZATION_PRESENT
            return LocalizationUtils.GetLocalizedStringFromLocalizationPackage(referenceText, lang, arguments);
#else
            return Task.FromResult(referenceText);
#endif
        }
    }
}
