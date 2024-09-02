using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A link visual element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Link : LocalizedTextElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId urlProperty = nameof(url);

#endif

        /// <summary>
        /// The Link's USS class name.
        /// </summary>
        public new const string ussClassName = "appui-link";

        /// <summary>
        /// The Link's size USS class name.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(TextSize))]
        public const string sizeUssClassName = ussClassName + "--size-";

        Pressable m_Clickable;

        TextSize m_Size;

        string m_Url;

        /// <summary>
        /// The clickable manipulator.
        /// </summary>
        public Pressable clickable
        {
            get => m_Clickable;
            set
            {
                if (m_Clickable != null && m_Clickable.target == this)
                    this.RemoveManipulator(m_Clickable);
                m_Clickable = value;
                if (m_Clickable == null)
                    return;
                this.AddManipulator(m_Clickable);
            }
        }

        /// <summary>
        /// The size of the link.
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

        /// <summary>
        /// The URL of the link.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string url
        {
            get => m_Url;
            set
            {
                var changed = m_Url != value;
                m_Url = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in urlProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Link() : this(null)
        {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text"> The text of the link. </param>
        public Link(string text) : this(text, null)
        {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text"> The text of the link. </param>
        /// <param name="url"> The URL of the link. </param>
        public Link(string text, string url) : base(text)
        {
            AddToClassList(ussClassName);

            clickable = new Pressable();
            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;

            this.url = url;
            size = TextSize.M;

            this.AddManipulator(new KeyboardFocusController());
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines the UxmlFactory for the Link.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Link, UxmlTraits> { }

        /// <summary>
        /// Class containing the UXML traits for the Link.
        /// </summary>
        public new class UxmlTraits : LocalizedTextElement.UxmlTraits
        {

            readonly UxmlEnumAttributeDescription<TextSize> m_Size = new UxmlEnumAttributeDescription<TextSize>
            {
                name = "size",
                defaultValue = TextSize.M,
            };

            readonly UxmlStringAttributeDescription m_Url = new UxmlStringAttributeDescription
            {
                name = "url",
                defaultValue = null,
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

                var element = (Link)ve;
                element.size = m_Size.GetValueFromBag(bag, cc);
                string url = element.url;
                if (m_Url.TryGetValueFromBag(bag, cc, ref url))
                    element.url = url;
            }
        }

#endif
    }
}
