using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Heading sizing.
    /// </summary>
    public enum HeadingSize
    {
        /// <summary>
        /// Double Extra-small
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
    }

    /// <summary>
    /// Heading UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public sealed partial class Heading : LocalizedTextElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId primaryProperty = new BindingId(nameof(primary));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

#endif

        /// <summary>
        /// The Heading main styling class.
        /// </summary>
        public new const string ussClassName = "appui-heading";

        /// <summary>
        /// The Heading primary variant styling class.
        /// </summary>
        public const string primaryUssClassName = ussClassName + "--primary";

        /// <summary>
        /// The Heading size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(HeadingSize))]
        public const string sizeUssClassName = ussClassName + "--size-";

        HeadingSize m_Size = HeadingSize.M;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Heading()
            : this(string.Empty) { }

        /// <summary>
        /// Construct a Heading UI element with a provided text to display.
        /// </summary>
        /// <param name="text">The text that will be displayed.</param>
        public Heading(string text)
        {
            AddToClassList(ussClassName);

            focusable = false;
            pickingMode = PickingMode.Position; // in case we want a tooltip

            this.text = text;
            primary = true;
            size = HeadingSize.M;
        }

        /// <summary>
        /// The primary variant of the Heading.
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
        /// The size of the Heading.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public HeadingSize size
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
        /// Factory class to instantiate a <see cref="Heading"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Heading, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Heading"/>.
        /// </summary>
        public new class UxmlTraits : LocalizedTextElement.UxmlTraits
        {

            readonly UxmlBoolAttributeDescription m_Primary = new UxmlBoolAttributeDescription
            {
                name = "primary",
                defaultValue = true,
            };

            readonly UxmlEnumAttributeDescription<HeadingSize> m_Size = new UxmlEnumAttributeDescription<HeadingSize>
            {
                name = "size",
                defaultValue = HeadingSize.M,
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

                var element = (Heading)ve;
                element.primary = m_Primary.GetValueFromBag(bag, cc);
                element.size = m_Size.GetValueFromBag(bag, cc);

            }
        }

#endif
    }
}
