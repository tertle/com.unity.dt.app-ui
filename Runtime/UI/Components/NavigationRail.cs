using System;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A Navigation Rail is a UI element that provides a set of navigation options to the user on the side of the screen.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class NavigationRail : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId anchorProperty = new BindingId(nameof(anchor));

        internal static readonly BindingId labelTypeProperty = new BindingId(nameof(labelType));

        internal static readonly BindingId groupAlignmentProperty = new BindingId(nameof(groupAlignment));

#endif

        /// <summary>
        /// The NavigationRail main styling class.
        /// </summary>
        public const string ussClassName = "appui-navigation-rail";

        /// <summary>
        /// The leading container styling class.
        /// </summary>
        public const string leadingContainerUssClassName = ussClassName + "__leading-container";

        /// <summary>
        /// The trailing container styling class.
        /// </summary>
        public const string trailingContainerUssClassName = ussClassName + "__trailing-container";

        /// <summary>
        /// The content container styling class.
        /// </summary>
        public const string contentContainerUssClassName = ussClassName + "__content-container";

        /// <summary>
        /// The NavigationRail variant styling class.
        /// </summary>
        [EnumName("GetAnchorUssClassName", typeof(NavigationRailAnchor))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The NavigationRail label type styling class.
        /// </summary>
        [EnumName("GetLabelTypeUssClassName", typeof(LabelType))]
        public const string labelTypeUssClassName = ussClassName + "-label-type--";

        /// <summary>
        /// The NavigationRail group alignment styling class.
        /// </summary>
        [EnumName("GetGroupAlignmentUssClassName", typeof(GroupAlignment))]
        public const string groupAlignmentUssClassName = ussClassName + "-group-align--";

        NavigationRailAnchor m_Anchor;

        LabelType m_LabelType;

        VisualElement m_ContentContainer;

        GroupAlignment m_GroupAlignment;

        /// <summary>
        /// The content container of the NavigationRail.
        /// </summary>
        public override VisualElement contentContainer => this;

        /// <summary>
        /// The anchor of the NavigationRail. The NavigationRail will be anchored to the left or right side of the screen.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public NavigationRailAnchor anchor
        {
            get => m_Anchor;
            set
            {
                var changed = m_Anchor != value;
                RemoveFromClassList(GetAnchorUssClassName(m_Anchor));
                m_Anchor = value;
                AddToClassList(GetAnchorUssClassName(m_Anchor));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in anchorProperty);
                }
#endif
            }
        }

        /// <summary>
        /// The label type of the NavigationRail.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public LabelType labelType
        {
            get => m_LabelType;
            set
            {
                var changed = m_LabelType != value;
                RemoveFromClassList(GetLabelTypeUssClassName(m_LabelType));
                m_LabelType = value;
                AddToClassList(GetLabelTypeUssClassName(m_LabelType));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in labelTypeProperty);
                }
#endif
            }
        }

        /// <summary>
        /// The alignment of the group of items.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public GroupAlignment groupAlignment
        {
            get => m_GroupAlignment;
            set
            {
                var changed = m_GroupAlignment != value;
                RemoveFromClassList(GetGroupAlignmentUssClassName(m_GroupAlignment));
                m_GroupAlignment = value;
                AddToClassList(GetGroupAlignmentUssClassName(m_GroupAlignment));

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in groupAlignmentProperty);
                }
#endif
            }
        }

        /// <summary>
        /// The leading container of the NavigationRail.
        /// </summary>
        public VisualElement leadingContainer { get; }

        /// <summary>
        /// The trailing container of the NavigationRail.
        /// </summary>
        public VisualElement trailingContainer { get; }

        /// <summary>
        /// The main container of the NavigationRail.
        /// </summary>
        public VisualElement mainContainer => m_ContentContainer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NavigationRail()
        {
            pickingMode = PickingMode.Ignore;
            AddToClassList(ussClassName);

            leadingContainer = new VisualElement { name = leadingContainerUssClassName, pickingMode = PickingMode.Ignore };
            leadingContainer.AddToClassList(leadingContainerUssClassName);
            hierarchy.Add(leadingContainer);

            m_ContentContainer = new VisualElement { name = contentContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_ContentContainer.AddToClassList(contentContainerUssClassName);
            hierarchy.Add(m_ContentContainer);

            trailingContainer = new VisualElement { name = trailingContainerUssClassName, pickingMode = PickingMode.Ignore };
            trailingContainer.AddToClassList(trailingContainerUssClassName);
            hierarchy.Add(trailingContainer);

            anchor = NavigationRailAnchor.Start;
            labelType = LabelType.All;
            groupAlignment = GroupAlignment.Start;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="NavigationRail"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<NavigationRail, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="NavigationRail"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<NavigationRailAnchor> m_Anchor =
                new UxmlEnumAttributeDescription<NavigationRailAnchor>()
                {
                    name = "anchor",
                    defaultValue = NavigationRailAnchor.Start,
                };

            readonly UxmlEnumAttributeDescription<LabelType> m_LabelType =
                new UxmlEnumAttributeDescription<LabelType>()
                {
                    name = "label-type",
                    defaultValue = LabelType.All,
                };

            readonly UxmlEnumAttributeDescription<GroupAlignment> m_GroupAlignment =
                new UxmlEnumAttributeDescription<GroupAlignment>()
                {
                    name = "group-alignment",
                    defaultValue = GroupAlignment.Start,
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

                var el = (NavigationRail)ve;
                el.anchor = m_Anchor.GetValueFromBag(bag, cc);
                el.labelType = m_LabelType.GetValueFromBag(bag, cc);
                el.groupAlignment = m_GroupAlignment.GetValueFromBag(bag, cc);
            }
        }

#endif
    }

    /// <summary>
    /// The anchor of the Navigation Rail. The Rail will be anchored to the left or right side of the screen.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum NavigationRailAnchor
    {
        /// <summary>
        /// The Rail will be anchored to the left side of the screen.
        /// </summary>
        Start,
        /// <summary>
        /// The Rail will be anchored to the right side of the screen.
        /// </summary>
        End,
    }

    /// <summary>
    /// How to display the label of the NavigationRailItem.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum LabelType
    {
        /// <summary>
        /// No label will be displayed.
        /// </summary>
        None,
        /// <summary>
        /// Every label will be displayed.
        /// </summary>
        All,
        /// <summary>
        /// Only the selected label will be displayed.
        /// </summary>
        Selected,
    }

    /// <summary>
    /// The alignment of the group of items.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum GroupAlignment
    {
        /// <summary>
        /// Aligns the group to the start of the container.
        /// </summary>
        Start,
        /// <summary>
        /// Aligns the group to the center of the container.
        /// </summary>
        Center,
        /// <summary>
        /// Aligns the group to the end of the container.
        /// </summary>
        End,
    }
}
