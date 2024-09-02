using System;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A section inside a menu, with a heading.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class MenuSection : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId titleProperty = new BindingId(nameof(title));

#endif

        /// <summary>
        /// The MenuSection main styling class.
        /// </summary>
        public const string ussClassName = "appui-menusection";

        /// <summary>
        /// The MenuSection title styling class.
        /// </summary>
        public const string titleUssClassName = ussClassName + "__title";

        /// <summary>
        /// The MenuSection container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        readonly VisualElement m_Container;

        readonly LocalizedTextElement m_Title;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MenuSection()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Ignore;

            m_Title = new LocalizedTextElement { name = titleUssClassName, pickingMode = PickingMode.Ignore };
            m_Title.AddToClassList(titleUssClassName);

            m_Container = new VisualElement { name = containerUssClassName, pickingMode = PickingMode.Ignore };
            m_Container.AddToClassList(containerUssClassName);

            hierarchy.Add(m_Title);
            hierarchy.Add(m_Container);

            title = null;
        }

        /// <summary>
        /// The MenuSection container.
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        /// <summary>
        /// The text to display in the section heading.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string title
        {
            get => m_Title.text;
            set
            {
                var changed = m_Title.text != value;
                m_Title.text = value;
                m_Title.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(value));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in titleProperty);
#endif
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// The MenuSection UXML factory.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<MenuSection, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="MenuSection"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription
            {
                name = "title",
                defaultValue = null
            };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                m_PickingMode.defaultValue = PickingMode.Ignore;
                base.Init(ve, bag, cc);

                var element = (MenuSection)ve;
                element.title = m_Title.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
