#if UNITY_LOCALIZATION_PRESENT
using System;
using Unity.AppUI.Core;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// <para>
    /// A manipulator that reacts to the selected locale change from the Localization Package.
    /// </para>
    /// <para>
    /// When the Localization Package's selected locale changes, the context will be updated with the new language.
    /// </para>
    /// </summary>
    /// <seealso cref="VisualElementExtensions.ProvideContext{T}"/>
    public class SelectedLocaleListener : Manipulator
    {
        readonly Action<Locale> m_OnSelectedLocaleChanged;

        public SelectedLocaleListener()
        {
            m_OnSelectedLocaleChanged = OnSelectedLocaleChanged;
        }

        /// <inheritdoc/>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<AttachToPanelEvent>(OnAttached);
            target.RegisterCallback<DetachFromPanelEvent>(OnDetached);

            SetupLocalizationContext();
        }

        /// <inheritdoc/>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<AttachToPanelEvent>(OnAttached);
            target.UnregisterCallback<DetachFromPanelEvent>(OnDetached);

            CleanupLocalizationContext();
        }

        void OnAttached(AttachToPanelEvent evt)
        {
            SetupLocalizationContext();
        }

        void OnDetached(DetachFromPanelEvent evt)
        {
            CleanupLocalizationContext();
        }

        async void SetupLocalizationContext()
        {
            CleanupLocalizationContext();
            if (target.panel == null)
                return;

            var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();
            if (localizationSettings)
            {
                var selectedLang = await localizationSettings.GetSelectedLocaleAsync().Task;
                localizationSettings.OnSelectedLocaleChanged += m_OnSelectedLocaleChanged;
                OnSelectedLocaleChanged(selectedLang);
            }
        }

        void OnSelectedLocaleChanged(Locale selectedLocale)
        {
            if (target == null)
            {
                CleanupLocalizationContext();
                return;
            }

            var lang = selectedLocale ? selectedLocale.Identifier.Code : null;
            target.ProvideContext(string.IsNullOrEmpty(lang) ? null : new LangContext(lang));
        }

        void CleanupLocalizationContext()
        {
            var localizationSettings = LocalizationSettings.GetInstanceDontCreateDefault();
            if (localizationSettings)
                localizationSettings.OnSelectedLocaleChanged -= m_OnSelectedLocaleChanged;
        }
    }
}
#endif // UNITY_LOCALIZATION_PRESENT
