using System;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Sizing values for <see cref="Text"/> UI element.
    /// </summary>
    public enum TextSize
    {
        /// <summary>
        /// Extra-extra-small
        /// </summary>
        XXS,

        /// <summary>
        /// Extra-small
        /// </summary>
        XS,

        /// <summary>
        /// Small
        /// </summary>
        S,

        /// <summary>
        /// Medium
        /// </summary>
        M,

        /// <summary>
        /// Large
        /// </summary>
        L,

        /// <summary>
        /// Extra-large
        /// </summary>
        XL,

        /// <summary>
        /// Double Extra-large
        /// </summary>
        XXL,

        /// <summary>
        /// Triple Extra-large
        /// </summary>
        XXXL,
    }

    /// <summary>
    /// Text UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public sealed partial class Text : LocalizedTextElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId primaryProperty = nameof(primary);

        internal static readonly BindingId sizeProperty = nameof(size);

#endif

        /// <summary>
        /// The Text main styling class.
        /// </summary>
        public new const string ussClassName = "appui-text";

        /// <summary>
        /// The Text primary variant styling class.
        /// </summary>
        public const string primaryUssClassName = ussClassName + "--primary";

        /// <summary>
        /// The Text size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(TextSize))]
        public const string sizeUssClassName = ussClassName + "--size-";

        TextSize m_Size = TextSize.M;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Text()
            : this(string.Empty) { }

        /// <summary>
        /// Construct a Text UI element and use the provided text as display text.
        /// </summary>
        /// <param name="text">The text that will be displayed.</param>
        public Text(string text)
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Position; // tooltip support

            this.text = text;
            size = TextSize.M;
            primary = true;
        }

        /// <summary>
        /// The primary variant of the text.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool primary
        {
            get => ClassListContains(primaryUssClassName);
            set
            {
                var changed = ClassListContains(primaryUssClassName) != value;
                EnableInClassList(primaryUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in primaryProperty);
#endif
            }
        }

        /// <summary>
        /// The size of the text.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public TextSize size
        {
            get => m_Size;
            set
            {
                var changed = m_Size != value;
                RemoveFromClassList(GetSizeUssClassName(m_Size));
                m_Size = value;
                AddToClassList(GetSizeUssClassName(m_Size));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Text"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Text, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Text"/>.
        /// </summary>
        public new class UxmlTraits : LocalizedTextElement.UxmlTraits
        {

            readonly UxmlBoolAttributeDescription m_Primary = new UxmlBoolAttributeDescription
            {
                name = "primary",
                defaultValue = true,
            };

            readonly UxmlEnumAttributeDescription<TextSize> m_Size = new UxmlEnumAttributeDescription<TextSize>
            {
                name = "size",
                defaultValue = TextSize.M,
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

                var element = (Text)ve;
                element.primary = m_Primary.GetValueFromBag(bag, cc);
                element.size = m_Size.GetValueFromBag(bag, cc);

            }
        }

#endif
    }
}
