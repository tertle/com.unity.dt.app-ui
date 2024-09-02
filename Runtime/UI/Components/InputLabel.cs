using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The text overflow mode.
    /// </summary>
    public enum TextOverflow
    {
        /// <summary>
        /// The text will be truncated with an ellipsis.
        /// </summary>
        Ellipsis,

        /// <summary>
        /// The text won't be truncated.
        /// </summary>
        Normal,
    }

    /// <summary>
    /// InputLabel UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class InputLabel : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId labelProperty = new BindingId(nameof(label));

        internal static readonly BindingId directionProperty = new BindingId(nameof(direction));

        internal static readonly BindingId labelOverflowProperty = new BindingId(nameof(labelOverflow));

        internal static readonly BindingId inputAlignmentProperty = new BindingId(nameof(inputAlignment));

        internal static readonly BindingId requiredProperty = new BindingId(nameof(required));

        internal static readonly BindingId indicatorTypeProperty = new BindingId(nameof(indicatorType));

        internal static readonly BindingId requiredTextProperty = new BindingId(nameof(requiredText));

        internal static readonly BindingId helpMessageProperty = new BindingId(nameof(helpMessage));

        internal static readonly BindingId helpVariantProperty = new BindingId(nameof(helpVariant));

#endif

        /// <summary>
        /// The InputLabel main styling class.
        /// </summary>
        public const string ussClassName = "appui-inputlabel";

        /// <summary>
        /// The InputLabel size styling class.
        /// </summary>
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The InputLabel direction styling class.
        /// </summary>
        [EnumName("GetOrientationUssClassName", typeof(Direction))]
        public const string orientationUssClassName = ussClassName + "--";

        /// <summary>
        /// The InputLabel input container styling class.
        /// </summary>
        public const string inputContainerUssClassName = ussClassName + "__input-container";

        /// <summary>
        /// The InputLabel container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The InputLabel label container styling class.
        /// </summary>
        public const string labelContainerUssClassName = ussClassName + "__label-container";

        /// <summary>
        /// The InputLabel field-label styling class.
        /// </summary>
        public const string fieldLabelUssClassName = ussClassName + "__field-label";

        /// <summary>
        /// The InputLabel help text styling class.
        /// </summary>
        public const string helpTextUssClassName = ussClassName + "__help-text";

        /// <summary>
        /// The InputLabel input alignment styling class.
        /// </summary>
        [EnumName("GetInputAlignmentUssClassName", typeof(Align))]
        public const string inputAlignmentUssClassName = ussClassName + "--input-alignment-";

        /// <summary>
        /// The InputLabel with help text styling class.
        /// </summary>
        public const string withHelpTextUssClassName = ussClassName + "--with-help-text";

        readonly FieldLabel m_FieldLabel;

        readonly VisualElement m_Container;

        readonly HelpText m_HelpText;

        Direction m_Direction = Direction.Horizontal;

        Align m_InputAlignment = Align.Stretch;

        /// <summary>
        /// The content container.
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        /// <summary>
        /// The label value.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string label
        {
            get => m_FieldLabel.label;
            set
            {
                var changed = m_FieldLabel.label != value;
                m_FieldLabel.label = value;
                m_FieldLabel.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(value));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// The orientation of the label.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Direction direction
        {
            get => m_Direction;
            set
            {
                var changed = m_Direction != value;
                RemoveFromClassList(GetOrientationUssClassName(m_Direction));
                m_Direction = value;
                AddToClassList(GetOrientationUssClassName(m_Direction));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in directionProperty);
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
            get => m_FieldLabel.labelOverflow;
            set
            {
                var changed = m_FieldLabel.labelOverflow != value;
                m_FieldLabel.labelOverflow = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelOverflowProperty);
#endif
            }
        }

        /// <summary>
        /// The alignment of the input.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Align inputAlignment
        {
            get => m_InputAlignment;
            set
            {
                var changed = m_InputAlignment != value;
                RemoveFromClassList(GetInputAlignmentUssClassName(m_InputAlignment));
                m_InputAlignment = value;
                AddToClassList(GetInputAlignmentUssClassName(m_InputAlignment));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in inputAlignmentProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the input is required or not in the form. This will add an asterisk next to the label.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool required
        {
            get => m_FieldLabel.required;
            set
            {
                var changed = m_FieldLabel.required != value;
                m_FieldLabel.required = value;
                EnableInClassList(Styles.requiredUssClassName,  m_FieldLabel.required);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in requiredProperty);
#endif
            }
        }

        /// <summary>
        /// The requirement indicator to display.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public IndicatorType indicatorType
        {
            get => m_FieldLabel.indicatorType;
            set
            {
                var changed = m_FieldLabel.indicatorType != value;
                m_FieldLabel.indicatorType = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in indicatorTypeProperty);
#endif
            }
        }

        /// <summary>
        /// The requirement indicator to display.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string requiredText
        {
            get => m_FieldLabel.requiredText;
            set
            {
                var changed = m_FieldLabel.requiredText != value;
                m_FieldLabel.requiredText = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in requiredTextProperty);
#endif
            }
        }

        /// <summary>
        /// The error message to display.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string helpMessage
        {
            get => m_HelpText.text;
            set
            {
                var changed = m_HelpText.text != value;
                m_HelpText.text = value;
                EnableInClassList(withHelpTextUssClassName, !string.IsNullOrEmpty(value));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in helpMessageProperty);
#endif
            }
        }

        /// <summary>
        /// The variant of the <see cref="HelpText"/>.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public HelpTextVariant helpVariant
        {
            get => m_HelpText.variant;
            set
            {
                var changed = m_HelpText.variant != value;
                m_HelpText.variant = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in helpVariantProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InputLabel()
            : this(null)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="label"> The label value. </param>
        public InputLabel(string label)
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Position;

            var labelContainer = new VisualElement { name = labelContainerUssClassName, pickingMode = PickingMode.Ignore };
            labelContainer.AddToClassList(labelContainerUssClassName);
            hierarchy.Add(labelContainer);

            m_FieldLabel = new FieldLabel(label) { name = fieldLabelUssClassName, pickingMode = PickingMode.Ignore };
            m_FieldLabel.AddToClassList(fieldLabelUssClassName);
            labelContainer.hierarchy.Add(m_FieldLabel);

            var cell = new HelpText { pickingMode = PickingMode.Ignore };
            cell.AddToClassList(helpTextUssClassName);
            labelContainer.hierarchy.Add(cell);

            var inputContainer = new VisualElement { name = inputContainerUssClassName, pickingMode = PickingMode.Ignore };
            inputContainer.AddToClassList(inputContainerUssClassName);
            hierarchy.Add(inputContainer);

            m_Container = new VisualElement { name = containerUssClassName, pickingMode = PickingMode.Ignore };
            m_Container.AddToClassList(containerUssClassName);
            inputContainer.hierarchy.Add(m_Container);

            m_HelpText = new HelpText
            {
                name = helpTextUssClassName,
                pickingMode = PickingMode.Ignore
            };
            m_HelpText.AddToClassList(helpTextUssClassName);
            inputContainer.hierarchy.Add(m_HelpText);

            this.label = label;
            direction = Direction.Horizontal;
            inputAlignment = Align.Stretch;
            labelOverflow = TextOverflow.Ellipsis;
            requiredText = "(Required)";
            indicatorType = IndicatorType.Asterisk;
            required = false;
            helpMessage = null;
            helpVariant = HelpTextVariant.Destructive;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="InputLabel"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<InputLabel, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="InputLabel"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {

            readonly UxmlEnumAttributeDescription<TextSize> m_Size = new UxmlEnumAttributeDescription<TextSize>
            {
                name = "size",
                defaultValue = TextSize.S,
            };

            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = null,
            };

            readonly UxmlEnumAttributeDescription<Direction> m_Orientation = new UxmlEnumAttributeDescription<Direction>
            {
                name = "direction",
                defaultValue = Direction.Horizontal,
            };

            readonly UxmlEnumAttributeDescription<TextOverflow> m_LabelOverflow = new UxmlEnumAttributeDescription<TextOverflow>
            {
                name = "label-overflow",
                defaultValue = TextOverflow.Ellipsis,
            };

            readonly UxmlEnumAttributeDescription<Align> m_InputAlignment = new UxmlEnumAttributeDescription<Align>
            {
                name = "input-alignment",
                defaultValue = Align.Stretch,
            };

            readonly UxmlBoolAttributeDescription m_Required = new UxmlBoolAttributeDescription
            {
                name = "required",
                defaultValue = false,
            };

            readonly UxmlStringAttributeDescription m_HelpMessage = new UxmlStringAttributeDescription
            {
                name = "help-message",
                defaultValue = null,
            };

            readonly UxmlEnumAttributeDescription<HelpTextVariant> m_HelpVariant = new UxmlEnumAttributeDescription<HelpTextVariant>
            {
                name = "help-variant",
                defaultValue = HelpTextVariant.Destructive,
            };

            readonly UxmlStringAttributeDescription m_RequiredText = new UxmlStringAttributeDescription
            {
                name = "required-text",
                defaultValue = "(Required)",
            };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var element = (InputLabel)ve;

                var direction = Direction.Horizontal;
                if (m_Orientation.TryGetValueFromBag(bag, cc, ref direction))
                    element.direction = direction;

                var inputAlignment = Align.Stretch;
                if (m_InputAlignment.TryGetValueFromBag(bag, cc, ref inputAlignment))
                    element.inputAlignment = inputAlignment;

                var labelOverflow = TextOverflow.Ellipsis;
                if (m_LabelOverflow.TryGetValueFromBag(bag, cc, ref labelOverflow))
                    element.labelOverflow = labelOverflow;

                var label = string.Empty;
                if (m_Label.TryGetValueFromBag(bag, cc, ref label))
                    element.label = label;

                var required = false;
                if (m_Required.TryGetValueFromBag(bag, cc, ref required))
                    element.required = required;

                var helpMessage = string.Empty;
                if (m_HelpMessage.TryGetValueFromBag(bag, cc, ref helpMessage))
                    element.helpMessage = helpMessage;

                var helpVariant = HelpTextVariant.Destructive;
                if (m_HelpVariant.TryGetValueFromBag(bag, cc, ref helpVariant))
                    element.helpVariant = helpVariant;

                var requiredText = string.Empty;
                if (m_RequiredText.TryGetValueFromBag(bag, cc, ref requiredText))
                    element.requiredText = requiredText;
            }
        }

#endif
    }
}
