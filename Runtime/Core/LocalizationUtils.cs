#if UNITY_LOCALIZATION_PRESENT
using System.Threading.Tasks;
using UnityEngine.Localization.Settings;
#endif

namespace Unity.AppUI.Core
{
    /// <summary>
    /// A class that provides utility methods for localization.
    /// </summary>
    public static class LocalizationUtils
    {
        /// <summary>
        /// Try to get the table and string reference from a reference text.
        /// The naming convention is `@table:tableEntry`.
        /// </summary>
        /// <param name="referenceText"> The reference text to parse. </param>
        /// <param name="table"> The table name. </param>
        /// <param name="tableEntry"> The table entry reference. </param>
        /// <returns> True if the reference text is valid. </returns>
        public static bool TryGetTableAndEntry(string referenceText, out string table, out string tableEntry)
        {
            table = null;
            tableEntry = null;
            if (string.IsNullOrEmpty(referenceText) || !referenceText.StartsWith("@") || !referenceText.Contains(":"))
                return false;

            var split = referenceText[1..].Split(':');
            if (split.Length != 2)
                return false;

            table = split[0];
            tableEntry = split[1];
            return true;
        }

#if UNITY_LOCALIZATION_PRESENT
        /// <summary>
        /// Get the localized string from the localization package.
        /// </summary>
        /// <param name="referenceText"> The reference text. </param>
        /// <param name="lang"> The language. </param>
        /// <param name="arguments"> The arguments to format the string. </param>
        /// <returns> The localized string. </returns>
        public static async Task<string> GetLocalizedStringFromLocalizationPackage(string referenceText, string lang, params object[] arguments)
        {
            var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();
            if (lang != null
                && localizationSettings
                && TryGetTableAndEntry(referenceText, out var tableReference, out var entryReference))
            {
                await localizationSettings.GetInitializationOperation().Task;
                await localizationSettings.GetSelectedLocaleAsync().Task;
                var locales = localizationSettings.GetAvailableLocales();
                if (locales is LocalesProvider localesProvider)
                {
                    if (localesProvider.PreloadOperation.IsValid() && !localesProvider.PreloadOperation.IsDone)
                        await localesProvider.PreloadOperation.Task;
                }

                var locale = locales.GetLocale(lang);
                if (locale)
                {
                    var table = await localizationSettings.GetStringDatabase().GetTableAsync(tableReference, locale).Task;
                    if (table)
                    {
                        var entry = table.GetEntry(entryReference);
                        if (entry != null)
                        {
                            if (!entry.IsSmart)
                                return entry.GetLocalizedString();
                            if (arguments?.Length > 0)
                                return entry.GetLocalizedString(arguments);

                            // If the entry is smart and we don't have arguments, fall back to the reference text.
                        }
                    }
                }
            }

            return referenceText;
        }
#endif
    }
}
