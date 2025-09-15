using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                    _ = UpdateTextWithCurrentLocale();
#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in textProperty);
#endif
                }
            }
        }

        internal string localizedText
        {
            get => base.text;
            private set
            {
                if (base.text != value)
                    base.text = value;
            }
        }

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
                _ = UpdateTextWithCurrentLocale();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in variablesProperty);
#endif
            }
        }

        void OnLangContextChanged(ContextChangedEvent<LangContext> evt)
        {
            _ = UpdateTextWithCurrentLocale();
        }

        async Task UpdateTextWithCurrentLocale()
        {
            if (!LocalizationUtils.TryGetTableAndEntry(m_ReferenceText, out _, out _)
                || this.GetContext<LangContext>() is not {} ctx)
            {
                localizedText = m_ReferenceText;
                return;
            }

            localizedText = await ctx.GetLocalizedStringAsync(m_ReferenceText, m_Variables?.ToArray());
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
