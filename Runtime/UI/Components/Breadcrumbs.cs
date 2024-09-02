using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Breadcrumbs visual element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Breadcrumbs : BaseVisualElement
    {
        /// <summary>
        /// The Breadcrumbs' USS class name.
        /// </summary>
        public const string ussClassName = "appui-breadcrumbs";

        /// <summary>
        /// Constructor.
        /// </summary>
        public Breadcrumbs()
        {
            AddToClassList(ussClassName);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// UXML Factory for Breadcrumbs.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Breadcrumbs, UxmlTraits> { }

        /// <summary>
        /// UXML Traits for Breadcrumbs.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {

        }
#endif
    }

    /// <summary>
    /// BreadcrumbItem visual element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class BreadcrumbItem : Link
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId isCurrentProperty = nameof(isCurrent);
#endif
        /// <summary>
        /// The BreadcrumbItem's USS class name.
        /// </summary>
        public new const string ussClassName = "appui-breadcrumb-item";

        /// <summary>
        /// The BreadcrumbItem's active USS class name.
        /// </summary>
        public const string currentUssClassName = ussClassName + "--current";

        /// <summary>
        /// Whether the BreadcrumbItem is the current item.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Breadcrumb Item")]
#endif
        public bool isCurrent
        {
            get => ClassListContains(currentUssClassName);
            set
            {
                var changed = isCurrent != value;
                EnableInClassList(currentUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in isCurrentProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BreadcrumbItem()
        {
            AddToClassList(ussClassName);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// UXML Factory for BreadcrumbItem.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<BreadcrumbItem, UxmlTraits> { }

        /// <summary>
        /// UXML Traits for BreadcrumbItem.
        /// </summary>
        public new class UxmlTraits : Link.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_IsCurrent = new UxmlBoolAttributeDescription
            {
                name = "is-current",
                defaultValue = false
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var item = (BreadcrumbItem)ve;
                item.isCurrent = m_IsCurrent.GetValueFromBag(bag, cc);
            }
        }
#endif
    }

    /// <summary>
    /// BreadcrumbSeparator visual element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class BreadcrumbSeparator : BaseTextElement
    {
        /// <summary>
        /// The BreadcrumbSeparator's USS class name.
        /// </summary>
        public new const string ussClassName = "appui-breadcrumb-separator";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BreadcrumbSeparator()
        {
            AddToClassList(ussClassName);

            text = "/";
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// UXML Factory for BreadcrumbSeparator.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<BreadcrumbSeparator, UxmlTraits> { }

        /// <summary>
        /// UXML Traits for BreadcrumbSeparator.
        /// </summary>
        public new class UxmlTraits : BaseTextElement.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var separator = (BreadcrumbSeparator)ve;

                if (string.IsNullOrEmpty(separator.text))
                    separator.text = "/";
            }
        }
#endif
    }
}
