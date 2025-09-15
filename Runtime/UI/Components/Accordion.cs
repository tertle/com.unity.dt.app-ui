using System;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The position of an element inside a Flex container.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum FlexPosition
    {
        /// <summary>
        /// The element is at the start of the container.
        /// </summary>
        /// <remarks>
        /// In a row with Left-to-Right layout, this is the left side.
        /// In a row with Right-to-Left layout, this is the right side.
        /// For a column, this is the top side.
        /// </remarks>
        Start,

        /// <summary>
        /// The element is at the end of the container.
        /// </summary>
        /// <remarks>
        /// In a row with Left-to-Right layout, this is the right side.
        /// In a row with Right-to-Left layout, this is the left side.
        /// For a column, this is the bottom side.
        /// </remarks>
        End
    }

    /// <summary>
    /// Item used inside an <see cref="Accordion"/> element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class AccordionItem : BaseVisualElement, INotifyValueChanged<bool>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId titleProperty = nameof(title);

        internal static readonly BindingId indicatorPositionProperty = nameof(indicatorPosition);

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId trailingContentTemplateProperty = nameof(trailingContentTemplate);

        internal static readonly BindingId leadingContentTemplateProperty = nameof(leadingContentTemplate);
#endif

        const string k_IndicatorIconName = "caret-down";

        /// <summary>
        /// The AccordionItem main styling class.
        /// </summary>
        public const string ussClassName = "appui-accordionitem";

        /// <summary>
        /// The AccordionItem content parent styling class.
        /// </summary>
        public const string contentParentUssClassName = ussClassName + "__content-parent";

        /// <summary>
        /// The AccordionItem content styling class.
        /// </summary>
        public const string contentUssClassName = ussClassName + "__content";

        /// <summary>
        /// The AccordionItem header styling class.
        /// </summary>
        public const string headerUssClassName = ussClassName + "__header";

        /// <summary>
        /// The AccordionItem headertext styling class.
        /// </summary>
        public const string headerTextUssClassName = ussClassName + "__headertext";

        /// <summary>
        /// The AccordionItem leading container styling class.
        /// </summary>
        public const string leadingContainerUssClassName = ussClassName + "__leading-container";

        /// <summary>
        /// The AccordionItem trailing container styling class.
        /// </summary>
        public const string trailingContainerUssClassName = ussClassName + "__trailing-container";

        /// <summary>
        /// The AccordionItem indicator styling class.
        /// </summary>
        public const string indicatorUssClassName = ussClassName + "__indicator";

        /// <summary>
        /// The AccordionItem heading styling class.
        /// </summary>
        public const string headingUssClassName = ussClassName + "__heading";

        /// <summary>
        /// The AccordionItem indicator position styling class.
        /// </summary>
        [EnumName("GetIndicatorPosUssClassName", typeof(FlexPosition))]
        public const string indicatorPosUssClassName = ussClassName + "--indicator-";

        readonly VisualElement m_ContentElement;

        readonly VisualElement m_ContentParentElement;

        readonly LocalizedTextElement m_HeaderTextElement;

        readonly VisualElement m_HeaderIndicatorElement;

        readonly Pressable m_Clickable;

        readonly ExVisualElement m_HeaderElement;

        VisualTreeAsset m_TrailingContentTemplate;

        VisualTreeAsset m_LeadingContentTemplate;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AccordionItem()
        {
            AddToClassList(ussClassName);

            focusable = false;
            pickingMode = PickingMode.Ignore;

            m_HeaderTextElement = new LocalizedTextElement { name = headerTextUssClassName, pickingMode = PickingMode.Ignore };
            m_HeaderTextElement.AddToClassList(headerTextUssClassName);

            leadingContainer = new VisualElement { name = leadingContainerUssClassName, pickingMode = PickingMode.Ignore };
            leadingContainer.AddToClassList(leadingContainerUssClassName);

            trailingContainer = new VisualElement { name = trailingContainerUssClassName, pickingMode = PickingMode.Ignore };
            trailingContainer.AddToClassList(trailingContainerUssClassName);

            m_HeaderIndicatorElement = new Icon { name = indicatorUssClassName, iconName = k_IndicatorIconName, pickingMode = PickingMode.Ignore };
            m_HeaderIndicatorElement.AddToClassList(indicatorUssClassName);

            m_HeaderElement = new ExVisualElement
            {
                name = headerUssClassName,
                pickingMode = PickingMode.Position,
                focusable = true,
                passMask = 0,
            };
            m_HeaderElement.AddToClassList(headerUssClassName);
            m_Clickable = new Pressable(OnClicked);
            m_HeaderElement.AddManipulator(m_Clickable);
            m_HeaderElement.AddManipulator(new KeyboardFocusController(OnKeyboardFocus, OnFocus));
            m_HeaderElement.hierarchy.Add(leadingContainer);
            m_HeaderElement.hierarchy.Add(m_HeaderTextElement);
            m_HeaderElement.hierarchy.Add(trailingContainer);
            m_HeaderElement.hierarchy.Add(m_HeaderIndicatorElement);

            var headingElement = new VisualElement { pickingMode = PickingMode.Ignore };
            headingElement.AddToClassList(headingUssClassName);
            headingElement.hierarchy.Add(m_HeaderElement);

            m_ContentParentElement = new VisualElement
            {
                name = contentParentUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_ContentParentElement.AddToClassList(contentParentUssClassName);

            m_ContentElement = new VisualElement
            {
                name = contentUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_ContentElement.AddToClassList(contentUssClassName);
            m_ContentParentElement.hierarchy.Add(m_ContentElement);
            m_ContentElement.RegisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);

            hierarchy.Add(headingElement);
            hierarchy.Add(m_ContentParentElement);

            AddToClassList(GetIndicatorPosUssClassName(FlexPosition.End));
            SetValueWithoutNotify(false);

            title = "Header";
            indicatorPosition = FlexPosition.End;
            leadingContentTemplate = null;
            trailingContentTemplate = null;
        }

        void OnContentGeometryChanged(GeometryChangedEvent evt)
        {
            if (value)
            {
                if (float.IsNaN(evt.newRect.height) || Mathf.Approximately(evt.newRect.height, m_ContentParentElement.resolvedStyle.height))
                    return;
                m_ContentParentElement.style.height = evt.newRect.height;
            }
        }

        void OnFocus(FocusInEvent evt)
        {
            m_HeaderElement.passMask = 0;
        }

        void OnKeyboardFocus(FocusInEvent evt)
        {
            m_HeaderElement.passMask = ExVisualElement.Passes.Clear | ExVisualElement.Passes.Outline;
        }

        /// <summary>
        /// The content container of the AccordionItem.
        /// </summary>
        public override VisualElement contentContainer => m_ContentElement;

        /// <summary>
        /// The header's trailing container of the AccordionItem.
        /// </summary>
        public VisualElement trailingContainer { get; }

        /// <summary>
        /// The header's leading container of the AccordionItem.
        /// </summary>
        public VisualElement leadingContainer { get; }

        /// <summary>
        /// The header's leading container template of the AccordionItem.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public VisualTreeAsset leadingContentTemplate
        {
            get => m_LeadingContentTemplate;
            set
            {
                var changed = m_LeadingContentTemplate != value;
                m_LeadingContentTemplate = value;
                leadingContainer.Clear();
                if (m_LeadingContentTemplate)
                    m_LeadingContentTemplate.CloneTree(leadingContainer);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in leadingContentTemplateProperty);
#endif
            }
        }

        /// <summary>
        /// The header's trailing container template of the AccordionItem.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public VisualTreeAsset trailingContentTemplate
        {
            get => m_TrailingContentTemplate;
            set
            {
                var changed = m_TrailingContentTemplate != value;
                m_TrailingContentTemplate = value;
                trailingContainer.Clear();
                if (m_TrailingContentTemplate)
                    m_TrailingContentTemplate.CloneTree(trailingContainer);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in trailingContentTemplateProperty);
#endif
            }
        }

        /// <summary>
        /// The position of the indicator.
        /// </summary>
        [Tooltip("The position of the indicator.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public FlexPosition indicatorPosition
        {
            get => m_HeaderElement.hierarchy.IndexOf(m_HeaderIndicatorElement) == 0 ? FlexPosition.Start : FlexPosition.End;
            set
            {
                var previousValue = indicatorPosition;
                if (previousValue == value)
                    return;

                m_HeaderIndicatorElement.RemoveFromHierarchy();
                if (value == FlexPosition.Start)
                    m_HeaderElement.hierarchy.Insert(0, m_HeaderIndicatorElement);
                else
                    m_HeaderElement.hierarchy.Add(m_HeaderIndicatorElement);
                RemoveFromClassList(GetIndicatorPosUssClassName(previousValue));
                AddToClassList(GetIndicatorPosUssClassName(value));

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(indicatorPositionProperty);
#endif
            }
        }

        /// <summary>
        /// The title of the AccordionItem.
        /// </summary>
        [Tooltip("The title of the AccordionItem.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string title
        {
            get => m_HeaderTextElement.text;
            set
            {
                var previousValue = m_HeaderTextElement.text;

                m_HeaderTextElement.text = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (previousValue != value)
                    NotifyPropertyChanged(titleProperty);
#endif
            }
        }

        /// <summary>
        /// The value of the item, which represents its open state.
        /// </summary>
        [Tooltip("The value of the item, which represents its open state.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool value
        {
            get => ClassListContains(Styles.openUssClassName);
            set
            {
                var previousValue = ClassListContains(Styles.openUssClassName);
                if (previousValue == value)
                    return;
                using var evt = ChangeEvent<bool>.GetPooled(previousValue, value);
                using var itemEvt = AccordionItemValueChangedEvent.GetPooled();
                itemEvt.target = this;
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);
                SendEvent(itemEvt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(valueProperty);
#endif
            }
        }

        /// <summary>
        /// Set the open state of the item without triggering any event.
        /// </summary>
        /// <param name="newValue">The new open state of the item.</param>
        public void SetValueWithoutNotify(bool newValue)
        {
            if (newValue)
            {
                m_ContentParentElement.style.height = m_ContentElement.resolvedStyle.height;
            }
            else
            {
                m_ContentParentElement.style.height = 0;
            }
            EnableInClassList(Styles.openUssClassName, newValue);
        }

        void OnClicked()
        {
            value = !value;
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Class to be able to use the <see cref="AccordionItem"/> in UXML.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<AccordionItem, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="AccordionItem"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription
            {
                name = "title",
                defaultValue = "Header",
            };

            readonly UxmlEnumAttributeDescription<FlexPosition> m_IndicatorPosition = new UxmlEnumAttributeDescription<FlexPosition>
            {
                name = "indicator-position",
                defaultValue = FlexPosition.End,
            };

            readonly UxmlBoolAttributeDescription m_Value = new UxmlBoolAttributeDescription
            {
                name = "value",
                defaultValue = false,
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

                var element = (AccordionItem)ve;
                element.title = m_Title.GetValueFromBag(bag, cc);
                element.indicatorPosition = m_IndicatorPosition.GetValueFromBag(bag, cc);
                element.value = m_Value.GetValueFromBag(bag, cc);
            }
        }
#endif
    }

    /// <summary>
    /// Accordion UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Accordion : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId isExclusiveProperty = nameof(isExclusive);
#endif

        /// <summary>
        /// The Accordion main styling class.
        /// </summary>
        public const string ussClassName = "appui-accordion";

        bool m_IsExclusive;

        /// <summary>
        /// The behavior of the Accordion when multiple items are open.
        /// </summary>
        /// <remarks>
        /// If true, a maximum of one item can be open at a time.
        /// </remarks>
        [Tooltip("If true, a maximum of one item can be open at a time.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Accordion")]
#endif
        public bool isExclusive
        {
            get => m_IsExclusive;
            set
            {
                var previousValue = m_IsExclusive;
                m_IsExclusive = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (previousValue != value)
                    NotifyPropertyChanged(isExclusiveProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Accordion()
        {
            AddToClassList(ussClassName);

            RegisterCallback<AccordionItemValueChangedEvent>(OnAccordionItemValueChanged);

            pickingMode = PickingMode.Ignore;

            isExclusive = false;
        }

        void OnAccordionItemValueChanged(AccordionItemValueChangedEvent evt)
        {
            if (evt.target is AccordionItem item && item.parent == this)
            {
                if (isExclusive)
                {
                    foreach (var child in Children())
                    {
                        if (child != item && child is AccordionItem accordionItem)
                        {
                            accordionItem.SetValueWithoutNotify(false);
                        }
                    }
                }
                evt.StopPropagation();
            }
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// The UXML factory for the Accordion.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Accordion, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Accordion"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            /// <summary>
            /// The behavior of the Accordion when multiple items are open.
            /// <para>
            /// If true, a maximum of one item can be open at a time.
            /// </para>
            /// </summary>
            readonly UxmlBoolAttributeDescription m_IsExclusive = new UxmlBoolAttributeDescription
            {
                name = "is-exclusive",
                defaultValue = false
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

                var element = (Accordion)ve;
                element.isExclusive = m_IsExclusive.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
