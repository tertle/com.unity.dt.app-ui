using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A drawer header visual element.
    /// </summary>
    public class DrawerHeader : BaseVisualElement
    {
        /// <summary>
        /// The DrawerHeader's USS class name.
        /// </summary>
        public const string ussClassName = "appui-drawer-header";

        /// <summary>
        /// The DrawerHeader's title USS class name.
        /// </summary>
        public const string titleUssClassName = ussClassName + "__title";

        /// <summary>
        /// The DrawerHeader's container USS class name.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        LocalizedTextElement m_Title;

        VisualElement m_Container;

        /// <summary>
        /// The DrawerHeader's title.
        /// </summary>
        public string title
        {
            get => m_Title.text;
            set => m_Title.text = value;
        }

        /// <summary>
        /// Child elements are added to this element.
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DrawerHeader()
        {
            AddToClassList(ussClassName);

            m_Title = new LocalizedTextElement();
            m_Title.AddToClassList(titleUssClassName);
            hierarchy.Add(m_Title);

            m_Container = new VisualElement();
            m_Container.AddToClassList(containerUssClassName);
            hierarchy.Add(m_Container);
        }
    }
}
