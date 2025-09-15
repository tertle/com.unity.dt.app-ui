using System;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A NavigationRailItem is a selectable item in a NavigationRail.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class NavigationRailItem : VisualElement, IPressable, ISelectableElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId selectedProperty = nameof(selected);

        internal static readonly BindingId labelProperty = nameof(label);

        internal static readonly BindingId iconProperty = nameof(icon);

        internal static readonly BindingId clickableProperty = nameof(clickable);

#endif
        /// <summary>
        /// Main styling class for the NavigationRailItem.
        /// </summary>
        public const string ussClassName = "appui-navigation-rail-item";

        /// <summary>
        /// Icon styling class for the NavigationRailItem.
        /// </summary>
        public const string iconUssClassName = ussClassName + "__icon";

        /// <summary>
        /// Label styling class for the NavigationRailItem.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        Icon m_IconElement;

        LocalizedTextElement m_LabelElement;

        Pressable m_Clickable;

        /// <summary>
        /// Whether the NavigationRailItem is in selected state.
        /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public bool selected
        {
            get => ClassListContains(Styles.selectedUssClassName);
            set
            {
                var changed = selected != value;
                SetSelectedWithoutNotify(value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in selectedProperty);
#endif
            }
        }

        /// <summary>
        /// The icon of the NavigationRailItem.
        /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public string icon
        {
            get => m_IconElement.iconName;
            set
            {
                var changed = m_IconElement.iconName != value;
                m_IconElement.iconName = value;
                m_IconElement.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_IconElement.iconName));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in iconProperty);
#endif
            }
        }

        /// <summary>
        /// The label of the NavigationRailItem.
        /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public string label
        {
            get => m_LabelElement.text;
            set
            {
                var changed = m_LabelElement.text != value;
                m_LabelElement.text = value;
                m_LabelElement.EnableInClassList(Styles.hiddenUssClassName, string.IsNullOrEmpty(m_LabelElement.text));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// Clickable Manipulator for this NavigationRailItem.
        /// </summary>
        public Pressable clickable
        {
            get => m_Clickable;
            set
            {
                var changed = m_Clickable != value;
                if (m_Clickable != null && m_Clickable.target == this)
                    this.RemoveManipulator(m_Clickable);
                m_Clickable = value;
                if (m_Clickable == null)
                    return;
                this.AddManipulator(m_Clickable);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in clickableProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NavigationRailItem()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;

            clickable = new Pressable();
            this.AddManipulator(clickable);

            m_IconElement = new Icon { name = iconUssClassName, pickingMode = PickingMode.Ignore };
            m_IconElement.AddToClassList(iconUssClassName);
            hierarchy.Add(m_IconElement);

            m_LabelElement = new LocalizedTextElement { name = labelUssClassName, pickingMode = PickingMode.Ignore };
            m_LabelElement.AddToClassList(labelUssClassName);
            hierarchy.Add(m_LabelElement);

            icon = null;
            label = null;
            selected = false;
        }

        /// <inheritdoc/>
        public void SetSelectedWithoutNotify(bool newValue)
        {
            EnableInClassList(Styles.selectedUssClassName, newValue);
        }


#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="NavigationRailItem"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<NavigationRailItem, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="NavigationRailItem"/>.
        /// </summary>
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Icon = new UxmlStringAttributeDescription { name = "icon" };
            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription { name = "label" };
            readonly UxmlBoolAttributeDescription m_Selected = new UxmlBoolAttributeDescription { name = "selected" };

            /// <inheritdoc/>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var item = ve as NavigationRailItem;
                if (item == null)
                    return;

                item.icon = m_Icon.GetValueFromBag(bag, cc);
                item.label = m_Label.GetValueFromBag(bag, cc);
                item.selected = m_Selected.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
