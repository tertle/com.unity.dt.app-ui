using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A bottom navigation bar visual element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class BottomNavBar : BaseVisualElement
    {
        /// <summary>
        /// The BottomNavBar's USS class name.
        /// </summary>
        public const string ussClassName = "appui-bottom-navbar";

        /// <summary>
        /// The content container of the BottomNavBar.
        /// </summary>
        public override VisualElement contentContainer => this;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BottomNavBar()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Ignore;
        }
    }

    /// <summary>
    /// A bottom navigation bar item visual element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class BottomNavBarItem : BaseVisualElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId iconProperty = nameof(icon);

        internal static readonly BindingId labelProperty = nameof(label);

        internal static readonly BindingId isSelectedProperty = nameof(isSelected);

        internal static readonly BindingId iconVariantProperty = nameof(iconVariant);

        internal static readonly BindingId selectedIconVariantProperty = nameof(selectedIconVariant);
#endif
        /// <summary>
        /// The BottomNavBarItem's USS class name.
        /// </summary>
        public const string ussClassName = "appui-bottom-navbar-item";

        /// <summary>
        /// The BottomNavBarItem's icon USS class name.
        /// </summary>
        public const string iconUssClassName = ussClassName + "__icon";

        /// <summary>
        /// The BottomNavBarItem's label USS class name.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        Icon m_Icon;

        LocalizedTextElement m_Label;

        Pressable m_Clickable;

        IconVariant m_SelectedIconVariant = IconVariant.Regular;

        IconVariant m_IconVariant = IconVariant.Regular;

        /// <summary>
        /// The BottomNavBarItem's icon.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Bottom Navigation Bar")]
#endif
        public string icon
        {
            get => m_Icon.iconName;
            set
            {
                var changed = m_Icon.iconName != value;
                m_Icon.iconName = value;
                m_Icon.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(value));
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in iconProperty);
#endif
            }
        }

        /// <summary>
        /// The BottomNavBarItem's label.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string label
        {
            get => m_Label.text;
            set
            {
                var changed = m_Label.text != value;
                m_Label.text = value;
                m_Label.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(value));
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the BottomNavBarItem is selected.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool isSelected
        {
            get => ClassListContains(Styles.selectedUssClassName);
            set
            {
                var changed = isSelected != value;
                EnableInClassList(Styles.selectedUssClassName, value);
                m_Icon.variant = value ? m_SelectedIconVariant : m_IconVariant;
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in isSelectedProperty);
#endif
            }
        }

        /// <summary>
        /// The BottomNavBarItem's icon variant.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public IconVariant iconVariant
        {
            get => m_IconVariant;
            set
            {
                var changed = m_IconVariant != value;
                m_IconVariant = value;

                if (!isSelected)
                    m_Icon.variant = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in iconVariantProperty);
#endif
            }
        }

        /// <summary>
        /// The BottomNavBarItem's selected icon variant.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public IconVariant selectedIconVariant
        {
            get => m_SelectedIconVariant;
            set
            {
                var changed = m_SelectedIconVariant != value;
                m_SelectedIconVariant = value;

                if (isSelected)
                    m_Icon.variant = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in selectedIconVariantProperty);
#endif
            }
        }

        /// <summary>
        /// Clickable Manipulator for this BottomNavBarItem.
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
        /// Default constructor.
        /// </summary>
        public BottomNavBarItem()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="icon"> The BottomNavBarItem's icon. </param>
        /// <param name="label"> The BottomNavBarItem's label. </param>
        /// <param name="clickHandler"> The BottomNavBarItem's click handler. </param>
        public BottomNavBarItem(string icon, string label, Action clickHandler)
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;
            clickable = new Pressable(clickHandler);

            m_Icon = new Icon { iconName = icon, variant = IconVariant.Regular, pickingMode = PickingMode.Ignore };
            m_Icon.AddToClassList(iconUssClassName);
            hierarchy.Add(m_Icon);

            m_Label = new LocalizedTextElement(label) { pickingMode = PickingMode.Ignore };
            m_Label.AddToClassList(labelUssClassName);
            hierarchy.Add(m_Label);

            this.label = label;
            this.icon = icon;
        }
    }
}
