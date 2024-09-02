using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Chip UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Chip : BaseVisualElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId deleteIconProperty = nameof(deleteIcon);

        internal static readonly BindingId ornamentProperty = nameof(ornament);

        internal static readonly BindingId labelProperty = nameof(label);

        internal static readonly BindingId variantProperty = nameof(variant);

        internal static readonly BindingId deletableProperty = nameof(deletable);

#endif

        /// <summary>
        /// The possible variants for a <see cref="Chip"/>.
        /// </summary>
        public enum Variant
        {
            /// <summary>
            /// The <see cref="Chip"/> is displayed with a fill color.
            /// </summary>
            Filled,
            /// <summary>
            /// The <see cref="Chip"/> is displayed with an outline.
            /// </summary>
            Outlined,
        }

        const string k_DefaultDeleteIconName = "x";

        /// <summary>
        /// The Chip main styling class.
        /// </summary>
        public const string ussClassName = "appui-chip";

        /// <summary>
        /// The Chip variant styling class.
        /// </summary>
        [EnumName("GetVariantUssClassName", typeof(Variant))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The Chip Clickable variant styling class.
        /// </summary>
        public const string clickableUssClassName = ussClassName + "--clickable";

        /// <summary>
        /// The Chip Deletable variant styling class.
        /// </summary>
        public const string deletableUssClassName = ussClassName + "--deletable";

        /// <summary>
        /// The Chip with ornament variant styling class.
        /// </summary>
        public const string withOrnamentUssClassName = ussClassName + "--with-ornament";

        /// <summary>
        /// The Chip label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        /// <summary>
        /// The Chip ornament container styling class.
        /// </summary>
        public const string ornamentContainerUssClassName = ussClassName + "__ornament-container";

        /// <summary>
        /// The Chip delete Button styling class.
        /// </summary>
        public const string deleteButtonUssClassName = ussClassName + "__delete-button";

        /// <summary>
        /// The Chip delete Icon styling class.
        /// </summary>
        public const string deleteIconUssClassName = ussClassName + "__delete-icon";

        static readonly CustomStyleProperty<Color> k_UssColor = new CustomStyleProperty<Color>("--chip-color");

        static readonly CustomStyleProperty<Color> k_UssBgColor = new CustomStyleProperty<Color>("--chip-background-color");

        readonly VisualElement m_DeleteButton;

        readonly Icon m_DeleteIcon;

        Pressable m_Clickable;

        readonly LocalizedTextElement m_Label;

        Variant m_Variant;

        EventHandler m_Clicked;

        EventHandler m_Deleted;

        VisualElement m_Ornament;

        readonly VisualElement m_OrnamentContainer;

        readonly Pressable m_Deletable;

        /// <summary>
        /// The content container of the Chip. This is the ornament container.
        /// </summary>
        public override VisualElement contentContainer => m_OrnamentContainer;

        /// <summary>
        /// The icon name for the delete button.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string deleteIcon
        {
            get => m_DeleteIcon.iconName;
            set
            {
                var changed = m_DeleteIcon.iconName != value;
                m_DeleteIcon.iconName = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in deleteIconProperty);
#endif
            }
        }

        /// <summary>
        /// The Chip variant.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Variant variant
        {
            get => m_Variant;
            set
            {
                var changed = m_Variant != value;
                RemoveFromClassList(GetVariantUssClassName(m_Variant));
                m_Variant = value;
                AddToClassList(GetVariantUssClassName(m_Variant));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in variantProperty);
#endif
            }
        }

        /// <summary>
        /// The Chip ornament.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public VisualElement ornament
        {
            get => m_Ornament;
            set
            {
                var changed = m_Ornament != value;
                if (m_Ornament != null && m_Ornament.parent == m_OrnamentContainer)
                    m_OrnamentContainer.Remove(m_Ornament);
                m_Ornament = value;
                if (m_Ornament != null)
                    m_OrnamentContainer.Add(m_Ornament);
                EnableInClassList(withOrnamentUssClassName, m_Ornament != null);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in ornamentProperty);
#endif
            }
        }

        /// <summary>
        /// The Chip label.
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

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in labelProperty);
#endif
            }
        }

        /// <summary>
        /// Set the Chip as deletable.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool deletable
        {
            get => ClassListContains(deletableUssClassName);
            set
            {
                var changed = ClassListContains(deletableUssClassName) != value;
                EnableInClassList(deletableUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in deletableProperty);
#endif
            }
        }

        /// <summary>
        /// Clickable Manipulator for this Chip.
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
        /// Event fired when the Chip is clicked.
        /// </summary>
        public event Action clicked
        {
            add => m_Clickable.clicked += value;
            remove => m_Clickable.clicked -= value;
        }

        /// <summary>
        /// Event fired when the Chip is clicked.
        /// </summary>
        public event Action<EventBase> clickedWithEventInfo
        {
            add => m_Clickable.clickedWithEventInfo += value;
            remove => m_Clickable.clickedWithEventInfo -= value;
        }

        /// <summary>
        /// Event fired when the Chip is deleted.
        /// </summary>
        public event Action deleted
        {
            add => m_Deletable.clicked += value;
            remove => m_Deletable.clicked -= value;
        }

        /// <summary>
        /// Event fired when the Chip is deleted.
        /// </summary>
        public event Action<EventBase> deletedWithEventInfo
        {
            add => m_Deletable.clickedWithEventInfo += value;
            remove => m_Deletable.clickedWithEventInfo -= value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Chip()
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Position;
            clickable = new Pressable();
            focusable = true;
            tabIndex = 0;

            m_OrnamentContainer = new VisualElement { name = ornamentContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_OrnamentContainer.AddToClassList(ornamentContainerUssClassName);
            hierarchy.Add(m_OrnamentContainer);

            m_Label = new LocalizedTextElement { name = labelUssClassName, pickingMode = PickingMode.Ignore };
            m_Label.AddToClassList(labelUssClassName);
            hierarchy.Add(m_Label);

            m_DeleteButton = new VisualElement { name = deleteButtonUssClassName, pickingMode = PickingMode.Position, focusable = true };
            m_DeleteButton.AddToClassList(deleteButtonUssClassName);
            m_Deletable = new Pressable();
            m_DeleteButton.AddManipulator(m_Deletable);
            hierarchy.Add(m_DeleteButton);

            m_DeleteIcon = new Icon { name = deleteIconUssClassName, pickingMode = PickingMode.Ignore };
            m_DeleteIcon.AddToClassList(deleteIconUssClassName);
            m_DeleteButton.hierarchy.Add(m_DeleteIcon);

            deleteIcon = k_DefaultDeleteIconName;
            variant = Variant.Filled;
            ornament = null;

            AddToClassList(clickableUssClassName);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines the UxmlFactory for the Chip.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Chip, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Chip"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {



            readonly UxmlEnumAttributeDescription<Variant> m_Variant = new UxmlEnumAttributeDescription<Variant>
            {
                name = "variant",
                defaultValue = Variant.Filled
            };

            readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = null
            };

            readonly UxmlStringAttributeDescription m_DeleteIcon = new UxmlStringAttributeDescription
            {
                name = "delete-icon",
                defaultValue = k_DefaultDeleteIconName
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

                var element = (Chip)ve;
                element.variant = m_Variant.GetValueFromBag(bag, cc);
                element.label = m_Label.GetValueFromBag(bag, cc);
                element.deleteIcon = m_DeleteIcon.GetValueFromBag(bag, cc);

            }
        }
#endif
    }
}
