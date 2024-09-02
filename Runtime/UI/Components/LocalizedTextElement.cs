using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.AppUI.Core;
#if UNITY_LOCALIZATION_PRESENT
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A localized text element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class LocalizedTextElement : BaseTextElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId textProperty = new BindingId(nameof(text));

        internal static readonly BindingId variablesProperty = new BindingId(nameof(variables));

#endif

        /// <summary>
        /// The main USS class name of this element.
        /// </summary>
        public new const string ussClassName = "appui-localized-text";

        static readonly CustomStyleProperty<int> k_FontWeightProperty = new ("--unity-font-weight");

        string m_ReferenceText;

        IList<object> m_Variables;

# if UNITY_LOCALIZATION_PRESENT
        AsyncOperationHandle<LocalizationSettings> m_LocalizationSettingsHandle;

        static readonly Dictionary<string, AsyncOperationHandle<StringTable>> s_GetTableOps =
            new Dictionary<string, AsyncOperationHandle<StringTable>>();

        string m_Entry;
#endif

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LocalizedTextElement()
            : this(null) { }

        /// <summary>
        /// Constructor with a reference text.
        /// </summary>
        /// <param name="text"> The reference text to use when formatting the localized string. You can also use plain text for no translation. </param>
        public LocalizedTextElement(string text)
        {
            AddToClassList(ussClassName);

            this.text = text;

            this.RegisterContextChangedCallback<LangContext>(OnLangContextChanged);
            RegisterCallback<DetachFromPanelEvent>(OnDetachedFromPanel);
        }

        /// <summary>
        /// The reference text to use when formatting the localized string. You can also use plain text for no translation.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public new string text
        {
            get => m_ReferenceText;
            set
            {
                var changed = m_ReferenceText != value;
                if (changed)
                {
                    m_ReferenceText = value;
                    UpdateTextWithCurrentLocale();
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in textProperty);
#endif
                }
            }
        }

        internal string localizedText => base.text;

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("text")]
        string textOverride
        {
            get => this.text;
            set => this.text = value;
        }
#endif

        /// <summary>
        /// The variables to use when formatting the localized string.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public IList<object> variables
        {
            get => m_Variables;
            set
            {
                var changed = m_Variables != value;
                m_Variables = value;
                UpdateTextWithCurrentLocale();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in variablesProperty);
#endif
            }
        }

        void OnLangContextChanged(ContextChangedEvent<LangContext> evt)
        {
            UpdateTextWithCurrentLocale();
            BindLocaleChanges();
        }

        void BindLocaleChanges()
        {
#if UNITY_LOCALIZATION_PRESENT
            var settings = LocalizationSettings.GetInstanceDontCreateDefault();
            if (settings)
            {
                settings.OnSelectedLocaleChanged -= UpdateTextWithLocale;
                settings.OnSelectedLocaleChanged += UpdateTextWithLocale;
                if (m_LocalizationSettingsHandle.IsValid())
                    m_LocalizationSettingsHandle.Completed -= OnLocalizationConfigCompleted;
                m_LocalizationSettingsHandle = settings.GetInitializationOperation();
                m_LocalizationSettingsHandle.Completed += OnLocalizationConfigCompleted;
            }
#endif
        }

        void UnbindLocaleChanges()
        {
#if UNITY_LOCALIZATION_PRESENT
            var settings = LocalizationSettings.GetInstanceDontCreateDefault();
            if (settings)
            {
                settings.OnSelectedLocaleChanged -= UpdateTextWithLocale;
                if (m_LocalizationSettingsHandle.IsValid())
                    m_LocalizationSettingsHandle.Completed -= OnLocalizationConfigCompleted;
            }
#endif
        }

        void OnDetachedFromPanel(DetachFromPanelEvent evt)
        {
            UnbindLocaleChanges();
        }

        void UpdateTextWithCurrentLocale()
        {
#if UNITY_LOCALIZATION_PRESENT

            if (panel == null)
                return;

            var locale = this.GetContext<LangContext>()?.locale;

            if (!locale)
            {
                base.text = m_ReferenceText;
                return;
            }

            if (TryGetTableAndEntry(m_ReferenceText, out var table, out var entry))
            {
                var settings = LocalizationSettings.GetInstanceDontCreateDefault();
                var db = (settings) ? settings.GetStringDatabase() : null;
                if (db == null)
                {
                    base.text = m_ReferenceText;
                    return;
                }

                m_Entry = entry;
                if (!s_GetTableOps.TryGetValue(table + "/" + locale.Identifier.Code, out var op))
                    s_GetTableOps[table + "/" + locale.Identifier.Code] = db.GetTableAsync(table, locale);
                op = s_GetTableOps[table + "/" + locale.Identifier.Code];

                if (op.IsDone)
                    OnTableFound(op);
                else
                    op.Completed += OnTableFound;
            }
            else
            {
                base.text = m_ReferenceText;
            }
#else
            base.text = m_ReferenceText;
#endif
        }

#if UNITY_LOCALIZATION_PRESENT

        void OnTableFound(AsyncOperationHandle<StringTable> op)
        {
            if (op.IsValid() && op.Status == AsyncOperationStatus.Succeeded)
            {
                if (op.Result.GetEntry(m_Entry) is {} dbEntry)
                {
                    if (dbEntry.IsSmart && (m_Variables == null || m_Variables.Count == 0))
                    {
                        base.text = m_ReferenceText;
                    }
                    else
                    {
                        base.text = dbEntry.GetLocalizedString(m_Variables);
                    }
                }
                else
                {
                    base.text = m_ReferenceText;
                }
            }
            else
            {
                base.text = m_ReferenceText;
            }
        }

        void UpdateTextWithLocale(Locale obj)
        {
            UpdateTextWithCurrentLocale();
        }

        void OnLocalizationConfigCompleted(AsyncOperationHandle<LocalizationSettings> obj)
        {
            if (m_LocalizationSettingsHandle.IsValid())
                m_LocalizationSettingsHandle.Completed -= OnLocalizationConfigCompleted;
            UpdateTextWithCurrentLocale();
        }
#endif

        /// <summary>
        /// Try to get the table and string reference from a reference text.
        /// The naming convention is `@table:tableEntry`.
        /// </summary>
        /// <param name="referenceText"> The reference text to parse. </param>
        /// <param name="table"> The table name. </param>
        /// <param name="tableEntry"> The table entry reference. </param>
        /// <returns> True if the reference text is valid. </returns>
        internal static bool TryGetTableAndEntry(string referenceText, out string table, out string tableEntry)
        {
            table = null;
            tableEntry = null;
            if (string.IsNullOrEmpty(referenceText) || !referenceText.Contains(":") || !referenceText.StartsWith("@"))
                return false;

            var split = referenceText[1..].Split(':');
            if (split.Length != 2)
                return false;

            table = split[0];
            tableEntry = split[1];
            return true;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Uxml factory for the <see cref="LocalizedTextElement"/>.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<LocalizedTextElement, UxmlTraits> { }

        /// <summary>
        /// Uxml traits for the <see cref="LocalizedTextElement"/>.
        /// </summary>
        public new class UxmlTraits : BaseTextElement.UxmlTraits
        {
            /// <summary>
            /// Initialize the <see cref="LocalizedTextElement"/> using the attribute bag.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize. </param>
            /// <param name="bag"> The attribute bag. </param>
            /// <param name="cc"> The creation context. </param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var element = (LocalizedTextElement)ve;
                element.text = ((TextElement)ve).text;
            }
        }

#endif
    }
}
