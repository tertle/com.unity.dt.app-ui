using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Quote UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Quote : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId colorProperty = nameof(color);

#endif

        /// <summary>
        /// The Quote main styling class.
        /// </summary>
        public const string ussClassName = "appui-quote";

        /// <summary>
        /// The Quote container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        readonly VisualElement m_Container;

        Optional<Color> m_InlineColor;

        /// <summary>
        /// The content container of the Quote.
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        /// <summary>
        /// The Quote outline color.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Optional<Color> color
        {
            get => m_InlineColor;
            set
            {
                m_InlineColor = value;
                var borderColor = m_InlineColor.IsSet ? m_InlineColor.Value : new StyleColor(StyleKeyword.Null);
                var previousBorderColor = resolvedStyle.borderLeftColor;
                style.borderLeftColor = borderColor;
                style.borderRightColor = borderColor;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (borderColor != previousBorderColor)
                    NotifyPropertyChanged(in colorProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Quote()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Position;
            focusable = false;

            m_Container = new VisualElement { name = containerUssClassName, pickingMode = PickingMode.Ignore };
            m_Container.AddToClassList(containerUssClassName);
            hierarchy.Add(m_Container);

            color = Optional<Color>.none;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines the UxmlFactory for the Quote.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Quote, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Quote"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {

            readonly UxmlColorAttributeDescription m_Color = new UxmlColorAttributeDescription
            {
                name = "color",
                defaultValue = Color.gray
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

                var element = (Quote)ve;

                var color = Color.gray;
                if (m_Color.TryGetValueFromBag(bag, cc, ref color))
                    element.color = color;


            }
        }

#endif
    }
}
