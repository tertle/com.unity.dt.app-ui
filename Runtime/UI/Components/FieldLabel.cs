using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The type of indicator to display when a field is required.
    /// </summary>
    public enum IndicatorType
    {
        /// <summary>
        /// No indicator.
        /// </summary>
        None,

        /// <summary>
        /// An asterisk.
        /// </summary>
        Asterisk,

        /// <summary>
        /// A localized "Required" text.
        /// </summary>
        Text,
    }

    /// <summary>
    /// A label for a field.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class FieldLabel : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId labelProperty = new BindingId(nameof(label));

        internal static readonly BindingId requiredProperty = new BindingId(nameof(required));

        internal static readonly BindingId indicatorTypeProperty = new BindingId(nameof(indicatorType));

        internal static readonly BindingId requiredTextProperty = new BindingId(nameof(requiredText));

        internal static readonly BindingId labelOverflowProperty = new BindingId(nameof(labelOverflow));

#endif

        /// <summary>
        /// The FieldLabel main styling class.
        /// </summary>
        public const string ussClassName = "appui-field-label";

        /// <summary>
        /// The FieldLabel variant styling class.
        /// </summary>
        [EnumName("GetIndicatorTypeUssClassName", typeof(IndicatorType))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The FieldLabel required variant styling class.
        /// </summary>
        public const string requiredUssClassName = ussClassName + "--required";

        /// <summary>
        /// The FieldLabel label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The FieldLabel required label styling class.
        /// </summary>
        public const string requiredLabelUssClassName = ussClassName + "__required-label";

        /// <summary>
        /// The FieldLabel label overflow variant styling class.
        /// </summary>
        [EnumName("GetLabelOverflowUssClassName", typeof(TextOverflow))]
        public const string labelOverflowUssClassName = ussClassName + "--label-overflow-";

        readonly LocalizedTextElement m_LabelElement;

        readonly LocalizedTextElement m_RequiredLabelElement;

        IndicatorType m_IndicatorType = IndicatorType.Asterisk;

        string m_RequiredText;

        TextOverflow m_LabelOverflow;

        /// <summary>
        /// Whether the field is required.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool required
        {
            get => ClassListContains(requiredUssClassName);
            set
            {
                var changed = required != value;
                EnableInClassList(requiredUssClassName, value);
                m_RequiredLabelElement.text = m_IndicatorType switch
                {
                    IndicatorType.Asterisk => "*",
                    IndicatorType.Text => m_RequiredText,
                    _ => null
                };

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in requiredProperty);
#endif
            }
        }

        /// <summary>
        /// The type of indicator to display when a field is required.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public IndicatorType indicatorType
        {
            get => m_IndicatorType;
            set
            {
                var changed = m_IndicatorType != value;
                RemoveFromClassList(GetIndicatorTypeUssClassName(m_IndicatorType));
                m_IndicatorType = value;
                AddToClassList(GetIndicatorTypeUssClassName(m_IndicatorType));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in indicatorTypeProperty);
#endif
            }
        }

        /// <summary>
        /// The text to display next to the label when the field is required and the indicator type is <see cref="IndicatorType.Text"/>.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string requiredText
        {
            get => m_RequiredText;
            set
            {
                var changed = m_RequiredText != value;
                m_RequiredText = value;
                if (m_IndicatorType == IndicatorType.Text)
                    m_RequiredLabelElement.text = m_RequiredText;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in requiredTextProperty);
#endif
            }
        }

        /// <summary>
        /// The text to display in the label.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string label
        {
            get => m_LabelElement.text;
            set
            {
                var changed = m_LabelElement.text != value;
                m_LabelElement.text = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// The text overflow mode.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public TextOverflow labelOverflow
        {
            get => m_LabelOverflow;
            set
            {
                var changed = m_LabelOverflow != value;
                RemoveFromClassList(GetLabelOverflowUssClassName(m_LabelOverflow));
                m_LabelOverflow = value;
                AddToClassList(GetLabelOverflowUssClassName(m_LabelOverflow));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelOverflowProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FieldLabel()
            : this(null) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text"> The text to display in the label. </param>
        public FieldLabel(string text)
        {
            pickingMode = PickingMode.Ignore;
            AddToClassList(ussClassName);

            m_LabelElement = new LocalizedTextElement
            {
                name = labelUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_LabelElement.AddToClassList(labelUssClassName);
            hierarchy.Add(m_LabelElement);

            m_RequiredLabelElement = new LocalizedTextElement
            {
                name = requiredLabelUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_RequiredLabelElement.AddToClassList(requiredLabelUssClassName);
            hierarchy.Add(m_RequiredLabelElement);

            label = text;
            required = false;
            indicatorType = IndicatorType.Asterisk;
            requiredText = "(Required)";
            labelOverflow = TextOverflow.Ellipsis;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="FieldLabel"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<FieldLabel, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="FieldLabel"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_Required = new UxmlBoolAttributeDescription
            {
                name = "required",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<IndicatorType> m_IndicatorType = new UxmlEnumAttributeDescription<IndicatorType>
            {
                name = "indicator-type",
                defaultValue = IndicatorType.None
            };

            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = string.Empty
            };




            readonly UxmlStringAttributeDescription m_RequiredText = new UxmlStringAttributeDescription
            {
                name = "required-text",
                defaultValue = "(Required)"
            };

            readonly UxmlEnumAttributeDescription<TextOverflow> m_LabelOverflow = new UxmlEnumAttributeDescription<TextOverflow>
            {
                name = "label-overflow",
                defaultValue = TextOverflow.Ellipsis
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var fieldLabel = (FieldLabel)ve;

                var required = false;
                if (m_Required.TryGetValueFromBag(bag, cc, ref required))
                    fieldLabel.required = required;

                var indicatorType = IndicatorType.None;
                if (m_IndicatorType.TryGetValueFromBag(bag, cc, ref indicatorType))
                    fieldLabel.indicatorType = indicatorType;

                var label = string.Empty;
                if (m_Label.TryGetValueFromBag(bag, cc, ref label))
                    fieldLabel.label = label;

                var requiredText = string.Empty;
                if (m_RequiredText.TryGetValueFromBag(bag, cc, ref requiredText))
                    fieldLabel.requiredText = requiredText;

                var labelOverflow = TextOverflow.Ellipsis;
                if (m_LabelOverflow.TryGetValueFromBag(bag, cc, ref labelOverflow))
                    fieldLabel.labelOverflow = labelOverflow;
            }
        }
#endif
    }
}
