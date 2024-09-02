using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The variant of the <see cref="HelpText"/>.
    /// </summary>
    public enum HelpTextVariant
    {
        /// <summary>
        /// The default variant.
        /// </summary>
        Default,

        /// <summary>
        /// The warning variant.
        /// </summary>
        Warning,

        /// <summary>
        /// The destructive variant.
        /// </summary>
        Destructive,
    }

    /// <summary>
    /// A help text.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class HelpText : LocalizedTextElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId variantProperty = new BindingId(nameof(variant));

#endif

        /// <summary>
        /// The HelpText main styling class.
        /// </summary>
        public new const string ussClassName = "appui-help-text";

        /// <summary>
        /// The HelpText variant styling class.
        /// </summary>
        [EnumName("GetVariantUssClassName", typeof(HelpTextVariant))]
        public const string variantUssClassName = ussClassName + "--";

        HelpTextVariant m_Variant;

        /// <summary>
        /// The variant of the <see cref="HelpText"/>.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public HelpTextVariant variant
        {
            get => m_Variant;
            set
            {
                var changed = m_Variant != value;
                RemoveFromClassList(GetVariantUssClassName(m_Variant));
                m_Variant = value;
                AddToClassList(GetVariantUssClassName(m_Variant));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in variantProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public HelpText()
            : this(null) {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text"> The message to display. </param>
        public HelpText(string text)
        {
            pickingMode = PickingMode.Ignore;
            AddToClassList(ussClassName);

            this.text = text;
            variant = HelpTextVariant.Default;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="HelpText"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<HelpText, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="HelpText"/>.
        /// </summary>
        public new class UxmlTraits : LocalizedTextElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<HelpTextVariant> m_Variant = new UxmlEnumAttributeDescription<HelpTextVariant>
            {
                name = "variant",
                defaultValue = HelpTextVariant.Default,
            };


            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var helpText = (HelpText)ve;

                var variant = HelpTextVariant.Default;
                if (m_Variant.TryGetValueFromBag(bag, cc, ref variant))
                    helpText.variant = variant;
            }
        }

#endif
    }
}
