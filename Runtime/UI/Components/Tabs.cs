using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Direction of a UI container.
    /// </summary>
    [GenerateLowerCaseStrings]
    public enum Direction
    {
        /// <summary>
        /// Container's items are stacked horizontally.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Container's items are stacked vertically.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Tabs UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class Tabs : BaseVisualElement, INotifyValueChanged<int>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId sizeProperty = nameof(size);

        internal static readonly BindingId directionProperty = nameof(direction);

        internal static readonly BindingId emphasizedProperty = nameof(emphasized);

        internal static readonly BindingId justifiedProperty = nameof(justified);

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId itemsProperty = nameof(items);

        internal static readonly BindingId sourceItemsProperty = nameof(sourceItems);

        internal static readonly BindingId bindItemProperty = nameof(bindItem);

        internal static readonly BindingId unbindItemProperty = nameof(unbindItem);

#endif

        /// <summary>
        /// The Tabs main styling class.
        /// </summary>
        public const string ussClassName = "appui-tabs";

        /// <summary>
        /// The Tabs size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Tabs direction styling class.
        /// </summary>
        [EnumName("GetOrientationUssClassName", typeof(Direction))]
        public const string orientationUssClassName = ussClassName + "--";

        /// <summary>
        /// The Tabs emphasized mode styling class.
        /// </summary>
        public const string emphasizedUssClassName = ussClassName + "--emphasized";

        /// <summary>
        /// The Tabs justified mode styling class.
        /// </summary>
        public const string justifiedUssClassName = ussClassName + "--justified";

        /// <summary>
        /// The Tabs container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The Tabs ScrollView styling class.
        /// </summary>
        public const string scrollViewUssClassName = ussClassName + "__scroll-view";

        /// <summary>
        /// The Tabs indicator styling class.
        /// </summary>
        public const string indicatorUssClassName = ussClassName + "__indicator";

        /// <summary>
        /// The Tabs animated indicator styling class.
        /// </summary>
        public const string animatedIndicatorUssClassName = indicatorUssClassName + "--animated";

        readonly VisualElement m_Indicator;

        readonly List<TabItem> m_Items = new List<TabItem>();

        readonly ScrollView m_ScrollView;

        readonly VisualElement m_LambdaContainer;

        readonly VisualElement m_Container;

        Action<TabItem, int> m_BindItem;

        Action<TabItem, int> m_UnbindItem;

        int m_DefaultValue;

        Direction m_Direction;

        Size m_Size;

        IList m_SourceItems;

        int m_Value;

        IVisualElementScheduledItem m_ScheduledRefreshIndicator;

        IVisualElementScheduledItem m_PollHierarchyItem;

        List<TabItem> m_StaticItems;

        readonly EventCallback<ITransitionEvent> m_TransitionEndAction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Tabs()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Ignore;

            m_ScrollView = new ScrollView
            {
                name = scrollViewUssClassName,
#if UITK_NESTED_INTERACTION_KIND
                nestedInteractionKind = ScrollView.NestedInteractionKind.StopScrolling,
#endif
                mode = ScrollViewMode.Horizontal,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden,
                verticalScrollerVisibility = ScrollerVisibility.Hidden,
            };
            m_ScrollView.AddToClassList(scrollViewUssClassName);

            m_Container = new VisualElement
            {
                name = containerUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_Container.AddToClassList(containerUssClassName);
            m_ScrollView.Add(m_Container);

            m_Indicator = new VisualElement
            {
                name = indicatorUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_Indicator.AddToClassList(indicatorUssClassName);

            m_LambdaContainer = new VisualElement
            {
                name = "lambda-container",
                pickingMode = PickingMode.Ignore,
            };
            hierarchy.Add(m_LambdaContainer);

            hierarchy.Add(m_ScrollView);
            hierarchy.Add(m_Indicator);

            size = Size.M;
            emphasized = false;
            justified = false;
            direction = Direction.Horizontal;
            value = -1;

            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            this.RegisterContextChangedCallback<DirContext>(OnDirectionChanged);
            m_PollHierarchyItem = schedule.Execute(PollHierarchy).Every(50L);
            m_ScrollView.verticalScroller.valueChanged += OnVerticalScrollerChanged;
            m_ScrollView.horizontalScroller.valueChanged += OnHorizontalScrollerChanged;
            RegisterCallback<ActionTriggeredEvent>(OnItemClicked);
            m_TransitionEndAction = OnIndicatorTransitionEnd;
        }

        void OnIndicatorTransitionEnd(ITransitionEvent evt)
        {
            m_Indicator.UnregisterCallback<TransitionEndEvent>(m_TransitionEndAction);
            m_Indicator.UnregisterCallback<TransitionCancelEvent>(m_TransitionEndAction);
            m_Indicator.RemoveFromClassList(animatedIndicatorUssClassName);

            // make sure the indicator is in the right position,
            // that would happen if the geometry has changed just after clicking on a tab
            RefreshIndicator();
        }

        void OnDirectionChanged(ContextChangedEvent<DirContext> evt)
        {
            RefreshVisuals();
        }

        /// <summary>
        /// The size of the Tabs.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Size size
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
        /// The direction of the Tabs. Horizontal or Vertical.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Direction direction
        {
            get => m_Direction;
            set
            {
                var changed = m_Direction != value;
                RemoveFromClassList(GetOrientationUssClassName(m_Direction));
                m_Direction = value;
                AddToClassList(GetOrientationUssClassName(m_Direction));
                m_ScrollView.mode = m_Direction switch
                {
                    Direction.Vertical => ScrollViewMode.Vertical,
                    _ => ScrollViewMode.Horizontal
                };
                SetValueWithoutNotify(m_Value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in directionProperty);
#endif
            }
        }

        /// <summary>
        /// The current list of items used to populate the Tabs.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public IList items => m_SourceItems ?? m_StaticItems;

        /// <summary>
        /// The emphasized mode of the Tabs.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool emphasized
        {
            get => ClassListContains(emphasizedUssClassName);
            set
            {
                var changed = emphasized != value;
                EnableInClassList(emphasizedUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in emphasizedProperty);
#endif
            }
        }

        /// <summary>
        /// The justified mode of the Tabs.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool justified
        {
            get => ClassListContains(justifiedUssClassName);
            set
            {
                var changed = justified != value;
                EnableInClassList(justifiedUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in justifiedProperty);
#endif
            }
        }

        /// <summary>
        /// Method to bind the TabItem.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Action<TabItem, int> bindItem
        {
            get => m_BindItem;

            set
            {
                var changed = m_BindItem != value;
                m_BindItem = value;
                RefreshItems();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in bindItemProperty);
#endif
            }
        }

        /// <summary>
        /// Method to unbind the TabItem.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Action<TabItem, int> unbindItem
        {
            get => m_UnbindItem;
            set
            {
                var changed = m_UnbindItem != value;
                m_UnbindItem = value;
                RefreshItems();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in unbindItemProperty);
#endif
            }
        }

        /// <summary>
        /// Collection of items used to populate the Tabs.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public IList sourceItems
        {
            get => m_SourceItems;
            set
            {
                if (m_SourceItems == value)
                    return;

                m_SourceItems = value;

                m_PollHierarchyItem?.Pause();
                m_PollHierarchyItem = null;
                RefreshItems();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in sourceItemsProperty);
                NotifyPropertyChanged(in itemsProperty);
#endif
            }
        }

        /// <summary>
        /// The virtual content container of the Tabs.
        /// </summary>
        public override VisualElement contentContainer => m_LambdaContainer;

        /// <summary>
        /// The item container of the Tabs.
        /// </summary>
        public VisualElement itemContainer => m_Container;

        /// <summary>
        /// Set the value of the Tabs without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value.</param>
        /// <exception cref="ValueOutOfRangeException"> Throws if the value is out of range.</exception>
        public void SetValueWithoutNotify(int newValue)
        {
            SetValueWithoutNotifyInternal(newValue);
        }

        void SetValueWithoutNotifyInternal(int newValue, bool scroll = true, bool animateIndicator = false)
        {
            var previousValue = m_Value;
            m_Value = IsValid(newValue) ? newValue : previousValue;

            // refresh selection visually
            if (previousValue >= 0 && previousValue < m_Items.Count && previousValue != m_Value)
                m_Items[previousValue].selected = false;

            RefreshVisuals(scroll, animateIndicator);
        }

        void RefreshVisuals(bool scroll = true, bool animateIndicator = false)
        {
            if (panel == null || !paddingRect.IsValid())
                return;

            if (m_Value >= 0 && m_Value < m_Items.Count)
            {
                m_Items[m_Value].selected = true;
                if (scroll)
                    m_ScrollView.ScrollTo(m_Items[m_Value]);
                m_ScheduledRefreshIndicator?.Pause();
                m_Indicator.EnableInClassList(animatedIndicatorUssClassName, animateIndicator);
                m_Indicator.RegisterCallback<TransitionEndEvent>(m_TransitionEndAction);
                m_Indicator.RegisterCallback<TransitionCancelEvent>(m_TransitionEndAction);
                m_ScheduledRefreshIndicator = schedule.Execute(RefreshIndicator);
            }
            else
            {
                m_Indicator.RemoveFromClassList(animatedIndicatorUssClassName);
                if (direction == Direction.Horizontal)
                {
                    m_Indicator.style.left = 0;
                    m_Indicator.style.width = 0;
                    m_Indicator.style.top = StyleKeyword.Null;
                    m_Indicator.style.height = StyleKeyword.Null;
                }
                else
                {
                    m_Indicator.style.top = 0;
                    m_Indicator.style.height = 0;
                    m_Indicator.style.left = StyleKeyword.Null;
                    m_Indicator.style.width = StyleKeyword.Null;
                }
            }
        }

        void RefreshIndicator()
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    var x = m_Items[m_Value].layout.x - m_ScrollView.scrollOffset.x;
                    if (!Mathf.Approximately(x, m_Indicator.resolvedStyle.left))
                        m_Indicator.style.left = x;
                    if (!Mathf.Approximately(m_Items[m_Value].layout.width, m_Indicator.resolvedStyle.width))
                        m_Indicator.style.width = m_Items[m_Value].layout.width;
                    m_Indicator.style.height = StyleKeyword.Null;
                    m_Indicator.style.top = StyleKeyword.Null;
                    break;
                case Direction.Vertical:
                    var y = m_Items[m_Value].layout.y - m_ScrollView.scrollOffset.y;
                    if (!Mathf.Approximately(y, m_Indicator.resolvedStyle.top))
                        m_Indicator.style.top = y;
                    if (!Mathf.Approximately(m_Items[m_Value].layout.height, m_Indicator.resolvedStyle.scale.value.y))
                        m_Indicator.style.height = m_Items[m_Value].layout.height;
                    m_Indicator.style.left = StyleKeyword.Null;
                    m_Indicator.style.width = StyleKeyword.Null;
                    break;
                default:
                    throw new ValueOutOfRangeException(nameof(direction), direction);
            }
        }

        /// <summary>
        /// The value of the Tabs. This is the index of the selected TabItem.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int value
        {
            get => m_Value;
            set
            {
                if (value == m_Value || !IsValid(value))
                    return;

                var previousValue = m_Value;
                SetValueWithoutNotifyInternal(value, true, true);

                using var evt = ChangeEvent<int>.GetPooled(previousValue, value);
                evt.target = this;
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        bool IsValid(int v)
        {
            if (v == -1)
                return true;
            return v >= 0 && v < m_Items.Count && m_Items[v].enabledSelf;
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            var handled = false;

            if (direction == Direction.Horizontal)
            {
                if (evt.keyCode == KeyCode.LeftArrow)
                    handled = GoToPrevious();
                else if (evt.keyCode == KeyCode.RightArrow) handled = GoToNext();
            }
            else
            {
                if (evt.keyCode == KeyCode.UpArrow)
                    handled = GoToPrevious();
                else if (evt.keyCode == KeyCode.DownArrow) handled = GoToNext();
            }

            if (handled)
            {

                evt.StopPropagation();
            }
        }

        /// <summary>
        /// Go to the next TabItem.
        /// </summary>
        /// <returns> True if the next TabItem is selected, false otherwise.</returns>
        public bool GoToNext()
        {
            var nextIndex = Mathf.Clamp(value + 1, 0, childCount - 1);
            while (!ElementAt(nextIndex).enabledSelf) nextIndex = Mathf.Clamp(nextIndex + 1, 0, childCount - 1);
            if (nextIndex >= childCount || nextIndex == value)
                return false;
            value = nextIndex;
            return true;
        }

        /// <summary>
        /// Go to the previous TabItem.
        /// </summary>
        /// <returns> True if the previous TabItem is selected, false otherwise.</returns>
        public bool GoToPrevious()
        {
            var nextIndex = Mathf.Clamp(value - 1, 0, childCount - 1);
            while (!ElementAt(nextIndex).enabledSelf) nextIndex = Mathf.Clamp(nextIndex - 1, 0, childCount - 1);
            if (nextIndex == value || nextIndex < 0)
                return false;
            value = nextIndex;
            return true;
        }

        void OnHorizontalScrollerChanged(float offset)
        {
            if (direction == Direction.Horizontal)
                SetValueWithoutNotifyInternal(value, false);
        }

        void OnVerticalScrollerChanged(float offset)
        {
            if (direction == Direction.Vertical)
                SetValueWithoutNotifyInternal(value, false);
        }

        void PollHierarchy()
        {
            if (m_StaticItems == null && childCount > 0 && m_SourceItems == null)
            {
                m_PollHierarchyItem?.Pause();
                m_PollHierarchyItem = null;
                m_StaticItems = new List<TabItem>();
                foreach (var c in Children())
                {
                    m_StaticItems.Add((TabItem)c);
                }

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in itemsProperty);
#endif

                RefreshItems();
            }
        }

        void RefreshItems()
        {
            for (var i = 0; i < itemContainer.childCount; i++)
            {
                var item = (TabItem)itemContainer.ElementAt(i);
                unbindItem?.Invoke(item, i);
                item.UnregisterCallback<GeometryChangedEvent>(OnItemGeometryChanged);
            }

            itemContainer.Clear();
            m_Items.Clear();

            if (m_SourceItems != null)
            {
                for (var i = 0; i < m_SourceItems.Count; i++)
                {
                    var item = new TabItem();
                    bindItem?.Invoke(item, i);
                    item.RegisterCallback<GeometryChangedEvent>(OnItemGeometryChanged);
                    itemContainer.Add(item);
                    m_Items.Add(item);
                }
            }
            else if (m_StaticItems != null)
            {
                foreach (var item in m_StaticItems)
                {
                    item.RegisterCallback<GeometryChangedEvent>(OnItemGeometryChanged);
                    itemContainer.Add(item);
                    m_Items.Add(item);
                }
            }

            if (itemContainer.childCount > 0)
            {
                // find the next valid item
                var newValue = 0;
                while (m_Items[newValue].enabledSelf == false && newValue < m_Items.Count)
                    newValue++;
                if (newValue < m_Items.Count)
                    SetValueWithoutNotifyInternal(newValue);
                else
                    SetValueWithoutNotifyInternal(-1);
            }
            else
            {
                SetValueWithoutNotifyInternal(-1);
            }
        }

        void OnItemGeometryChanged(GeometryChangedEvent evt)
        {
            if (m_Indicator.ClassListContains(animatedIndicatorUssClassName))
                return;

            if (evt.target is TabItem { selected: true })
                SetValueWithoutNotify(m_Value);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            SetValueWithoutNotify(m_Value);
        }

        void OnItemClicked(ActionTriggeredEvent evt)
        {
            if (evt.target is TabItem item)
            {
                var newValue = item.parent.IndexOf(item);
                if (value != newValue)
                    value = item.parent.IndexOf(item);
                else
                    RefreshVisuals(true, true);
                evt.StopPropagation();
            }
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="Tabs"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<Tabs, UxmlTraits>
        {
            /// <summary>
            /// Describes the types of element that can appear as children of this element in a UXML file.
            /// </summary>
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription => new[]
            {
                new UxmlChildElementDescription(typeof(TabItem))
            };
        }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Tabs"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlIntAttributeDescription m_DefaultValue = new UxmlIntAttributeDescription
            {
                name = "value",
                defaultValue = 0
            };

            readonly UxmlBoolAttributeDescription m_Emphasized = new UxmlBoolAttributeDescription
            {
                name = "emphasized",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription m_Justified = new UxmlBoolAttributeDescription
            {
                name = "justified",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<Direction> m_Orientation = new UxmlEnumAttributeDescription<Direction>
            {
                name = "direction",
                defaultValue = Direction.Horizontal,
            };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
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
                var el = (Tabs)ve;
                el.size = m_Size.GetValueFromBag(bag, cc);
                el.direction = m_Orientation.GetValueFromBag(bag, cc);
                el.emphasized = m_Emphasized.GetValueFromBag(bag, cc);
                el.justified = m_Justified.GetValueFromBag(bag, cc);
                el.value = m_DefaultValue.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
