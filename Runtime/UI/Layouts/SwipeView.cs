using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.Bridge;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A SwipeViewItem is an item that must be used as a child of a <see cref="SwipeView"/>.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class SwipeViewItem : BaseVisualElement
    {
        /// <summary>
        /// The main styling class of the SwipeViewItem. This is the class that is used in the USS file.
        /// </summary>
        public const string ussClassName = "appui-swipeview-item";

        /// <summary>
        /// The index of the item in the SwipeView.
        /// </summary>
        public int index { get; internal set; }

        /// <summary>
        /// The SwipeView that contains this item.
        /// </summary>
        public SwipeView view => parent as SwipeView;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SwipeViewItem()
        {
            AddToClassList(ussClassName);

            usageHints = UsageHints.DynamicTransform;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines the UxmlFactory for the SwipeViewItem.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SwipeViewItem, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="SwipeViewItem"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {

        }

#endif
    }

    /// <summary>
    /// A SwipeView is a container that displays one or more children at a time and provides a UI to
    /// navigate between them. It is similar to a <see cref="ScrollView"/> but here children are
    /// snapped to the container's edges. See <see cref="PageView"/> for a similar container that
    /// includes a page indicator.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class SwipeView : BaseVisualElement, INotifyValueChanged<int>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId directionProperty = new BindingId(nameof(direction));

        internal static readonly BindingId wrapProperty = new BindingId(nameof(wrap));

        internal static readonly BindingId visibleItemCountProperty = new BindingId(nameof(visibleItemCount));

        internal static readonly BindingId skipAnimationThresholdProperty = new BindingId(nameof(skipAnimationThreshold));

        internal static readonly BindingId autoPlayDurationProperty = new BindingId(nameof(autoPlayDuration));

        internal static readonly BindingId swipeableProperty = new BindingId(nameof(swipeable));

        internal static readonly BindingId resistanceProperty = new BindingId(nameof(resistance));

        internal static readonly BindingId valueProperty = new BindingId(nameof(value));

        internal static readonly BindingId canGoToNextProperty = new BindingId(nameof(canGoToNext));

        internal static readonly BindingId canGoToPreviousProperty = new BindingId(nameof(canGoToPrevious));

        internal static readonly BindingId currentItemProperty = new BindingId(nameof(currentItem));

        internal static readonly BindingId countProperty = new BindingId(nameof(count));

        internal static readonly BindingId sourceItemsProperty = new BindingId(nameof(sourceItems));

        internal static readonly BindingId bindItemProperty = new BindingId(nameof(bindItem));

        internal static readonly BindingId unbindItemProperty = new BindingId(nameof(unbindItem));

        internal static readonly BindingId snapAnimationSpeedProperty = new BindingId(nameof(snapAnimationSpeed));

        internal static readonly BindingId snapAnimationEasingProperty = new BindingId(nameof(snapAnimationEasing));

        internal static readonly BindingId startSwipeThresholdProperty = new BindingId(nameof(startSwipeThreshold));

#endif

        /// <summary>
        /// The main styling class of the SwipeView. This is the class that is used in the USS file.
        /// </summary>
        public const string ussClassName = "appui-swipeview";

        /// <summary>
        /// The styling class applied to the container of the SwipeView.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The styling class applied to the SwipeView depending on its orientation.
        /// </summary>
        [EnumName("GetDirectionUssClassName", typeof(Direction))]
        public const string variantUssClassName = ussClassName + "--";

        /// <summary>
        /// The default duration of the auto play animation.
        /// </summary>
        public const int noAutoPlayDuration = -1;

        bool m_Wrap;

        List<SwipeViewItem> m_StaticItems;

        Direction m_Direction;

        int m_Value = -1;

        ValueAnimation<float> m_Animation;

        int m_VisibleItemCount = k_DefaultVisibleItemCount;

        IVisualElementScheduledItem m_PollHierarchyItem;

        IList m_SourceItems;

        readonly VisualElement m_Container;

        bool m_ForceDisableWrap;

        bool m_GoingPrevious;

        bool m_GoingNext;

        readonly Scrollable m_Scrollable;

        Vector2 m_PointerDistance;

        int m_AutoPlayDuration = noAutoPlayDuration;

        IVisualElementScheduledItem m_AutoPlayAnimation;

        bool m_Swipeable;

        Dir m_CurrentDirection;

        IVisualElementScheduledItem m_ScheduledNextValue;

        /// <summary>
        /// The container of the SwipeView.
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        const float k_DefaultSnapAnimationSpeed = 0.5f;

        float m_SnapAnimationSpeed = k_DefaultSnapAnimationSpeed;

        /// <summary>
        /// The speed of the animation when snapping to an item.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float snapAnimationSpeed
        {
            get => m_SnapAnimationSpeed;
            set
            {
                var changed = !Mathf.Approximately(m_SnapAnimationSpeed, value);
                m_SnapAnimationSpeed = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in snapAnimationSpeedProperty);
#endif
            }
        }

        static readonly Func<float, float> k_DefaultSnapAnimationEasing = Easing.OutCubic;

        Func<float, float> m_SnapAnimationEasing = k_DefaultSnapAnimationEasing;

        /// <summary>
        /// The easing of the animation when snapping to an item.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<float, float> snapAnimationEasing
        {
            get => m_SnapAnimationEasing;
            set
            {
                var changed = m_SnapAnimationEasing != value;
                m_SnapAnimationEasing = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in snapAnimationEasingProperty);
#endif
            }
        }

        const float k_DefaultStartSwipeThreshold = 5f;

        /// <summary>
        /// The amount of pixels that must be swiped before the SwipeView begins to swipe.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float startSwipeThreshold
        {
            get => m_Scrollable.threshold;
            set
            {
                var changed = !Mathf.Approximately(m_Scrollable.threshold, value);
                m_Scrollable.threshold = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in startSwipeThresholdProperty);
#endif
            }
        }

        const int k_DefaultVisibleItemCount = 1;

        /// <summary>
        /// The number of items that are visible at the same time.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int visibleItemCount
        {
            get => m_VisibleItemCount;
            set
            {
                var changed = m_VisibleItemCount != value;
                m_VisibleItemCount = value;
                SetValueWithoutNotify(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in visibleItemCountProperty);
                    NotifyPropertyChanged(in canGoToNextProperty);
                    NotifyPropertyChanged(in canGoToPreviousProperty);
                }
#endif
            }
        }

        const int k_DefaultAutoPlayDuration = noAutoPlayDuration;

        /// <summary>
        /// The number of milliseconds between each automatic swipe.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int autoPlayDuration
        {
            get => m_AutoPlayDuration;
            set
            {
                if (m_AutoPlayDuration == value)
                    return;

                m_AutoPlayDuration = value;
                if (m_AutoPlayDuration > 0)
                {
                    m_AutoPlayAnimation = schedule.Execute(() => GoToNext());
                    m_AutoPlayAnimation.Every(m_AutoPlayDuration);
                }
                else
                {
                    m_AutoPlayAnimation?.Pause();
                    m_AutoPlayAnimation = null;
                }

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in autoPlayDurationProperty);
#endif
            }
        }

        const Direction k_DefaultDirection = Direction.Horizontal;

        /// <summary>
        /// The orientation of the SwipeView.
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
                RemoveFromClassList(GetDirectionUssClassName(m_Direction));
                m_Direction = value;
                m_Scrollable.direction = value == Direction.Horizontal ? ScrollViewMode.Horizontal : ScrollViewMode.Vertical;
                AddToClassList(GetDirectionUssClassName(m_Direction));
                SetValueWithoutNotify(this.value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in directionProperty);
#endif
            }
        }

        Action<SwipeViewItem, int> m_BindItem;

        /// <summary>
        /// A method that is called when an item is bound to the SwipeView.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Action<SwipeViewItem, int> bindItem
        {
            get => m_BindItem;
            set
            {
                var changed = m_BindItem != value;
                m_BindItem = value;
                RefreshList();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in bindItemProperty);
#endif
            }
        }

        Action<SwipeViewItem, int> m_UnbindItem;

        /// <summary>
        /// A method that is called when an item is unbound from the SwipeView.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Action<SwipeViewItem, int> unbindItem
        {
            get => m_UnbindItem;
            set
            {
                var changed = m_UnbindItem != value;
                m_UnbindItem = value;
                RefreshList();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in unbindItemProperty);
#endif
            }
        }

        /// <summary>
        /// The source of items that are used to populate the SwipeView.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public IList sourceItems
        {
            get => m_SourceItems;
            set
            {
                var changed = m_SourceItems != value;
                m_SourceItems = value;

                // Stop Polling the hierarchy as we provided a new set of items
                m_PollHierarchyItem?.Pause();
                m_PollHierarchyItem = null;

                RefreshList();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in sourceItemsProperty);
                    NotifyPropertyChanged(in countProperty);
                    NotifyPropertyChanged(in canGoToNextProperty);
                    NotifyPropertyChanged(in canGoToPreviousProperty);
                }
#endif
            }
        }

        const bool k_DefaultWrap = false;

        /// <summary>
        /// This property determines whether or not the view wraps around when it reaches the start or end.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool wrap
        {
            get => m_Wrap;
            set
            {
                var changed = m_Wrap != value;
                m_Wrap = value;
                RefreshEverything();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                {
                    NotifyPropertyChanged(in wrapProperty);
                    NotifyPropertyChanged(in canGoToNextProperty);
                    NotifyPropertyChanged(in canGoToPreviousProperty);
                }
#endif
            }
        }

        /// <summary>
        /// The total number of items.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public int count => items?.Count ?? 0;

        /// <summary>
        /// Determine if the SwipeView should wrap around.
        /// </summary>
        internal bool shouldWrap => count > visibleItemCount && wrap && !m_ForceDisableWrap;

        bool ShouldResist(Vector2 delta)
        {
            if (wrap)
                return false;

            if (direction == Direction.Horizontal)
            {
                var left = m_Container.resolvedStyle.left;
                var width = m_Container.resolvedStyle.width - resolvedStyle.width ;
                return (left > 0 && delta.x >= 0) || (left < -width && delta.x <= 0);
            }
            else
            {
                var top = m_Container.resolvedStyle.top;
                var height = m_Container.resolvedStyle.height - resolvedStyle.height;
                return (top > 0 && delta.y >= 0) || (top < -height && delta.y <= 0);
            }
        }

        /// <summary>
        /// The current item.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public SwipeViewItem currentItem => GetItem(value);

        IList items => m_SourceItems ?? m_StaticItems;

        SwipeViewItem GetItem(int index)
        {
            foreach (var child in Children())
            {
                if (child is SwipeViewItem item && item.index == index)
                    return item;
            }

            return null;
        }

        /// <summary>
        /// The event that is called when the value of the SwipeView changes (i.e. when its being swiped or when it snaps to an item).
        /// </summary>
        public event Action<SwipeViewItem, float> beingSwiped;

        /// <summary>
        /// The value of the SwipeView (i.e. the index of the current item).
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
                if (count == 0)
                    return;

                if (value < 0 || value > count - 1)
                    return;

                var previousValue = m_Value;
                SetValueWithoutNotify(value);
                if (previousValue != m_Value)
                {
                    using var evt = ChangeEvent<int>.GetPooled(previousValue, m_Value);
                    evt.target = this;
                    SendEvent(evt);
                }

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
                NotifyPropertyChanged(in canGoToNextProperty);
                NotifyPropertyChanged(in canGoToPreviousProperty);
                NotifyPropertyChanged(in currentItemProperty);
#endif
            }
        }

        const float k_DefaultResistance = 1f;

        float m_Resistance = k_DefaultResistance;

        /// <summary>
        /// <para>The resistance of the SwipeView.</para>
        /// <para>
        /// By default, the SwipeView has a resistance of 1.
        /// </para>
        /// <para>
        /// If you set this property to more than 1, the SwipeView will
        /// be harder to swipe. If you set this property to less than 1, the SwipeView will be easier to swipe.
        /// </para>
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float resistance
        {
            get => m_Resistance;
            set
            {
                var changed = !Mathf.Approximately(m_Resistance, value);
                m_Resistance = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in resistanceProperty);
#endif
            }
        }

        const bool k_DefaultSwipeable = true;

        /// <summary>
        /// <para>Whether or not the SwipeView is swipeable.</para>
        /// <para>
        /// By default, the SwipeView is swipeable. If you set this property to <see langword="false" />, you won't be
        /// able to interact with the SwipeView (except programmatically).
        /// </para>
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool swipeable
        {
            get => m_Swipeable;
            set
            {
                var changed = m_Swipeable != value;
                m_Swipeable = value;
                if (m_Swipeable)
                    this.AddManipulator(m_Scrollable);
                else
                    this.RemoveManipulator(m_Scrollable);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in swipeableProperty);
#endif
            }
        }

        const int k_DefaultSkipAnimationThreshold = 2;

        int m_SkipAnimationThreshold = k_DefaultSkipAnimationThreshold;

        /// <summary>
        /// This property determines the threshold at which the animation will be skipped.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int skipAnimationThreshold
        {
            get => m_SkipAnimationThreshold;
            set
            {
                var changed = m_SkipAnimationThreshold != value;
                m_SkipAnimationThreshold = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in skipAnimationThresholdProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SwipeView()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;
            usageHints = UsageHints.GroupTransform;

            m_Container = new VisualElement { name = containerUssClassName, pickingMode = PickingMode.Ignore };
            m_Container.AddToClassList(containerUssClassName);
            hierarchy.Add(m_Container);

            m_PollHierarchyItem = schedule.Execute(PollHierarchy).Every(50L);

            m_Scrollable = new Scrollable(OnDrag, OnUp, OnDown);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            m_Container.RegisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);

            direction = k_DefaultDirection;
            wrap = k_DefaultWrap;
            visibleItemCount = k_DefaultVisibleItemCount;
            autoPlayDuration = k_DefaultAutoPlayDuration;
            swipeable = k_DefaultSwipeable;
            resistance = k_DefaultResistance;
            skipAnimationThreshold = k_DefaultSkipAnimationThreshold;
            startSwipeThreshold = k_DefaultStartSwipeThreshold;
            snapAnimationSpeed = k_DefaultSnapAnimationSpeed;
            snapAnimationEasing = k_DefaultSnapAnimationEasing;

            this.RegisterContextChangedCallback<DirContext>(OnDirectionChanged);
        }

        void OnDirectionChanged(ContextChangedEvent<DirContext> evt)
        {
            m_CurrentDirection = evt.context?.dir ?? Dir.Ltr;
            schedule.Execute(RefreshEverything);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (!evt.newRect.IsValid())
                return;

            RefreshEverything();
        }

        void RefreshEverything()
        {
            SetValueWithoutNotify(value);
            InvokeSwipeEvents();
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            var handled = false;
            if (evt.target == this)
            {
                if (direction == Direction.Horizontal)
                {
                    if (evt.keyCode == KeyCode.LeftArrow)
                        handled = GoToPrevious();
                    else if (evt.keyCode == KeyCode.RightArrow)
                        handled = GoToNext();
                }
                else
                {
                    if (evt.keyCode == KeyCode.UpArrow)
                        handled = GoToPrevious();
                    else if (evt.keyCode == KeyCode.DownArrow)
                        handled = GoToNext();
                }
            }

            if (handled)
            {

                evt.StopPropagation();
            }
        }

        void OnDown(Scrollable draggable)
        {
            m_PointerDistance = draggable.position;
        }

        void OnUp(Scrollable draggable)
        {
            m_ForceDisableWrap = true;
            if (draggable.hasMoved)
            {
                var closestIndex = GetClosestIndex();
                if (closestIndex != value)
                {
                    value = closestIndex;
                }
                else
                {
                    // check the distance during the swipe to see if we should snap to the next item
                    var distance = direction == Direction.Horizontal ? draggable.position.x - m_PointerDistance.x : draggable.position.y - m_PointerDistance.y;
                    var threshold = direction == Direction.Horizontal ? resolvedStyle.width : resolvedStyle.height;
                    threshold /= 4;
                    var previousSpeed = snapAnimationSpeed;
                    snapAnimationSpeed *= ShouldResist(Vector2.zero) ? resistance : 1;
                    if (Mathf.Abs(distance) > threshold)
                    {
                        var res = distance > 0 ? GoToPrevious() : GoToNext();
                        if (!res)
                            value = closestIndex;
                    }
                    else
                    {
                        value = closestIndex;
                    }
                    snapAnimationSpeed = previousSpeed;
                }
            }
            else
            {
                var pos = this.LocalToWorld(draggable.localPosition);
                VisualElement hoveredChild = null;
                foreach (var child in Children())
                {
                    if (child.ContainsPoint(child.WorldToLocal(pos)))
                    {
                        hoveredChild = child;
                        break;
                    }
                }
                if (hoveredChild != null)
                    value = ((SwipeViewItem)hoveredChild).index;
            }
            m_ForceDisableWrap = false;
        }

        void OnDrag(Scrollable drag)
        {
            if (m_Animation != null && !m_Animation.IsRecycled())
                m_Animation.Recycle();

            var multiplier = ShouldResist(drag.deltaPos) ? 1f / resistance : 1f;

            if (direction == Direction.Horizontal)
                m_Container.style.left = m_Container.resolvedStyle.left + drag.deltaPos.x * multiplier;
            else
                m_Container.style.top = m_Container.resolvedStyle.top + drag.deltaPos.y * multiplier;
        }

        SwipeViewItem GetClosestElement()
        {
            if (items == null || count <= 0)
                return null;

            var best = (SwipeViewItem)ElementAt(0);
            var center = this.WorldToLocal(best.worldBound.min);
            var bestDistance = Mathf.Abs(direction == Direction.Horizontal ? center.x : center.y);

            for (var i = 1; i < childCount; i++)
            {
                var candidate = (SwipeViewItem)ElementAt(i);
                center = this.WorldToLocal(candidate.worldBound.min);
                var candidateDistance = Mathf.Abs(direction == Direction.Horizontal ? center.x : center.y);
                if (candidateDistance < bestDistance)
                {
                    bestDistance = candidateDistance;
                    best = candidate;
                }
            }

            return best;
        }

        int GetClosestIndex()
        {
            return GetClosestElement()?.index ?? -1; ;
        }

        bool IsItemCurrentlyVisible(VisualElement c)
        {
            var rect = this.WorldToLocal(c.worldBound);
            return direction == Direction.Horizontal
                ? (rect.x >= 0 && rect.x < localBound.width) || (rect.xMax > 0 && rect.xMax <= localBound.width)
                : (rect.y >= 0 && rect.y < localBound.height) || (rect.yMax > 0 && rect.yMax <= localBound.height);
        }

        /// <summary>
        /// Sets the value without notifying the listeners.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        public void SetValueWithoutNotify(int newValue)
        {
            if (count == 0)
            {
                m_Value = -1;
                return;
            }

            if (newValue < 0 || newValue > count - 1)
                return;

            m_ScheduledNextValue?.Pause();

            RefreshItemsSize();

            if (shouldWrap)
            {
                var currentElementIndex = IndexOf(currentItem);
                var nextElementIndex = IndexOf(GetItem(newValue));
                if (!m_GoingPrevious && (m_Value < newValue || m_GoingNext) && nextElementIndex < currentElementIndex)
                {
                    var goingNext = m_GoingNext;
                    // the next item is placed before the current one,
                    // move items to the end to get a more pleasant order
                    var itemsToMove = new List<VisualElement>();
                    var children = new List<VisualElement>(Children());
                    var i = 0;
                    while (i < children.Count && !IsItemCurrentlyVisible(children[i]))
                    {
                        itemsToMove.Add(children[i]);
                        i++;
                    }
                    m_Container.UnregisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);
                    SwapFirstToLast(itemsToMove.Count);
                    m_ScheduledNextValue = schedule.Execute(() =>
                    {
                        m_GoingNext = goingNext;
                        value = newValue;
                        m_GoingNext = false;
                    });
                    return;
                }

                if (!m_GoingNext && (m_Value > newValue || m_GoingPrevious) && nextElementIndex > currentElementIndex)
                {
                    var goingPrevious = m_GoingPrevious;
                    // the previous item is placed after the current one,
                    // move items to the start to get a more pleasant order
                    var itemsToMove = new List<VisualElement>();
                    var children = new List<VisualElement>(Children());
                    var i = children.Count - 1;
                    while (i >= 0 && !IsItemCurrentlyVisible(children[i]))
                    {
                        itemsToMove.Add(children[i]);
                        i--;
                    }
                    m_Container.UnregisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);
                    SwapLastToFirst(itemsToMove.Count);
                    m_ScheduledNextValue = schedule.Execute(() =>
                    {
                        m_GoingPrevious = goingPrevious;
                        value = newValue;
                        m_GoingPrevious = false;
                    });
                    return;
                }
            }
            else
            {
                newValue = Mathf.Clamp(newValue, 0, count - m_VisibleItemCount);
            }

            var from = m_Value >= 0 ? GetItem(m_Value) : null;
            var to = GetItem(newValue);

            if (paddingRect.IsValid())
                StartSwipeAnimation(from, to);

            from?.RemoveFromClassList(Styles.selectedUssClassName);
            m_Value = newValue;
            to?.AddToClassList(Styles.selectedUssClassName);
        }

        void StartSwipeAnimation(VisualElement from, VisualElement to)
        {
            // Need a valid destination to create the animation
            if (to == null)
                return;

            // Find the position where the container must be at the end of the animation
            var newElementMin = this.WorldToLocal(to.worldBound.min);
            var newElementSize = direction == Direction.Horizontal ? to.worldBound.width : to.worldBound.height;
            var newElementOffset = direction == Direction.Horizontal ? newElementMin.x : newElementMin.y;
            var currentContainerOffset = direction == Direction.Horizontal
                ? m_Container.resolvedStyle.left
                : m_Container.resolvedStyle.top;
            var targetContainerOffset = currentContainerOffset - newElementOffset;

            // Recycle previous animation
            if (m_Animation != null && !m_Animation.IsRecycled())
                m_Animation.Recycle();

            // Find the best duration and distance to use in the animation
            var duration = from == null || newElementOffset == 0
                ? 0
                : Mathf.RoundToInt(Mathf.Abs(newElementOffset) / snapAnimationSpeed);

            // The best distance takes in account the max distance based on skipAnimationThreshold property
            var distance = Mathf.Abs(targetContainerOffset - currentContainerOffset);
            var sign = Mathf.Sign(targetContainerOffset - currentContainerOffset);
            distance = Mathf.Min(distance, skipAnimationThreshold * newElementSize);
            currentContainerOffset = targetContainerOffset - sign * distance;

            // Start the animation
            m_Animation = experimental.animation.Start(currentContainerOffset, targetContainerOffset, duration, (_, f) =>
            {
                if (direction == Direction.Horizontal)
                    m_Container.style.left = f;
                else
                    m_Container.style.top = f;
            }).Ease(snapAnimationEasing).KeepAlive();
        }

        void PollHierarchy()
        {
            if (m_StaticItems == null && childCount > 0 && m_SourceItems == null)
            {
                m_PollHierarchyItem?.Pause();
                m_PollHierarchyItem = null;
                m_StaticItems = new List<SwipeViewItem>();
                foreach (var c in Children())
                {
                    m_StaticItems.Add((SwipeViewItem)c);
                }
                RefreshList();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in sourceItemsProperty);
                NotifyPropertyChanged(in countProperty);
                NotifyPropertyChanged(in canGoToNextProperty);
                NotifyPropertyChanged(in canGoToPreviousProperty);
#endif
            }
        }

        void RefreshItemsSize()
        {
            if (!contentRect.IsValid())
                return;

            foreach (var c in Children())
            {
                if (direction == Direction.Horizontal)
                    c.style.width = contentRect.width / m_VisibleItemCount;
                else
                    c.style.height = contentRect.height / m_VisibleItemCount;
            }
        }

        void RefreshList()
        {
            for (var i = 0; i < childCount; i++)
            {
                var item = (SwipeViewItem)ElementAt(i);
                unbindItem?.Invoke(item, i);
            }

            Clear();

            if (m_SourceItems != null)
            {
                for (var i = 0; i < m_SourceItems.Count; i++)
                {
                    var item = new SwipeViewItem { index = i };
                    bindItem?.Invoke(item, i);
                    Add(item);
                }
            }
            else if (m_StaticItems != null)
            {
                for (var i = 0; i < m_StaticItems.Count; i++)
                {
                    var item = new SwipeViewItem { index = i };
                    if (m_StaticItems[i].childCount > 0)
                        item.Add(m_StaticItems[i].ElementAt(0));
                    Add(item);
                }
            }

            if (childCount > 0)
                value = 0;
            else
                m_Value = -1;
        }

        void SwapLastToFirst() => SwapLastToFirst(1);

        void SwapLastToFirst(int times)
        {
            if (direction == Direction.Horizontal)
                m_Container.style.left = m_Container.resolvedStyle.left - contentRect.width * times;
            else
                m_Container.style.top = m_Container.resolvedStyle.top - contentRect.height * times;

            while (times > 0)
            {
                var item = ElementAt(childCount - 1);
                item.SendToBack();
                times--;
            }
            m_Container.RegisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);
        }

        void SwapFirstToLast() => SwapFirstToLast(1);

        void SwapFirstToLast(int times)
        {
            if (direction == Direction.Horizontal)
                m_Container.style.left = m_Container.resolvedStyle.left + contentRect.width * times;
            else
                m_Container.style.top = m_Container.resolvedStyle.top + contentRect.height * times;

            while (times > 0)
            {
                var item = ElementAt(0);
                item.BringToFront();
                times--;
            }

            m_Container.RegisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);
        }

        void InvokeSwipeEvents()
        {
            if (!paddingRect.IsValid() || beingSwiped == null)
                return;

            foreach (var item in Children())
            {
                var size = direction == Direction.Horizontal ? item.localBound.width : item.localBound.height;
                var localRect = this.WorldToLocal(item.worldBound);
                var normalizedDistance = direction == Direction.Horizontal ? localRect.x / size : localRect.y / size;
                beingSwiped?.Invoke((SwipeViewItem)item, normalizedDistance);
            }
        }

        void OnContainerGeometryChanged(GeometryChangedEvent evt)
        {
            if (!evt.newRect.IsValid())
                return;

            var containerMin = direction == Direction.Horizontal ? evt.newRect.x : evt.newRect.y;
            var containerMax = direction == Direction.Horizontal ? evt.newRect.xMax : evt.newRect.yMax;

            switch (shouldWrap)
            {
                case true when containerMin > 0:
                    m_Container.UnregisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);
                    schedule.Execute(SwapLastToFirst).ExecuteLater(16L);
                    break;
                case true when containerMax < paddingRect.width:
                    m_Container.UnregisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);
                    schedule.Execute(SwapFirstToLast).ExecuteLater(16L);
                    break;
            }

            InvokeSwipeEvents();
        }

        /// <summary>
        /// Check if there is a next item or not.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public bool canGoToNext => shouldWrap || (value + 1 < childCount && value + 1 >= 0);

        /// <summary>
        /// Check if there is a previous item or not.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public bool canGoToPrevious => shouldWrap || (value - 1 < childCount && value - 1 >= 0);

        /// <summary>
        /// Go to item at index.
        /// </summary>
        /// <param name="index"> Index of the item to go to. </param>
        /// <returns> True if the operation was successful, false otherwise. </returns>
        public bool GoTo(int index)
        {
            if (index < 0 || index >= childCount)
                return false;

            value = index;
            return true;
        }

        /// <summary>
        /// Snap to item at index.
        /// </summary>
        /// <param name="index"> Index of the item to snap to. </param>
        /// <returns> True if the operation was successful, false otherwise. </returns>
        public bool SnapTo(int index)
        {
            var skipAnimation = skipAnimationThreshold;
            skipAnimationThreshold = 0;
            var result = GoTo(index);
            skipAnimationThreshold = skipAnimation;
            return result;
        }

        /// <summary>
        /// Go to next item.
        /// </summary>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool GoToNext()
        {
            if (!canGoToNext)
                return false;

            var nextIndex = shouldWrap
                ? (int)Mathf.Repeat(value + 1, childCount)
                : Mathf.Clamp(value + 1, 0, childCount - visibleItemCount);

            if (nextIndex == value)
                return false;

            m_GoingNext = true;
            value = nextIndex;
            m_GoingNext = false;

            return true;
        }

        /// <summary>
        /// Go to previous item.
        /// </summary>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool GoToPrevious()
        {
            if (!canGoToPrevious)
                return false;

            var nextIndex = shouldWrap
                ? (int)Mathf.Repeat(value - 1, childCount)
                : Mathf.Clamp(value - 1, 0, childCount - visibleItemCount);

            if (nextIndex == value)
                return false;

            m_GoingPrevious = true;
            value = nextIndex;
            m_GoingPrevious = false;

            return true;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines the UxmlFactory for the SwipeView.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SwipeView, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="SwipeView"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<Direction> m_Direction = new UxmlEnumAttributeDescription<Direction>()
            {
                name = "direction",
                defaultValue = k_DefaultDirection,
            };

            readonly UxmlFloatAttributeDescription m_AnimationSpeed = new UxmlFloatAttributeDescription()
            {
                name = "animation-speed",
                defaultValue = k_DefaultSnapAnimationSpeed,
            };

            readonly UxmlIntAttributeDescription m_SkipAnim = new UxmlIntAttributeDescription()
            {
                name = "skip-animation-threshold",
                defaultValue = k_DefaultSkipAnimationThreshold,
            };

            readonly UxmlBoolAttributeDescription m_Wrap = new UxmlBoolAttributeDescription()
            {
                name = "wrap",
                defaultValue = k_DefaultWrap,
            };

            readonly UxmlIntAttributeDescription m_VisibleItemCount = new UxmlIntAttributeDescription()
            {
                name = "visible-item-count",
                defaultValue = k_DefaultVisibleItemCount,
            };

            readonly UxmlFloatAttributeDescription m_StartSwipeThreshold = new UxmlFloatAttributeDescription()
            {
                name = "start-swipe-threshold",
                defaultValue = k_DefaultStartSwipeThreshold,
            };

            readonly UxmlIntAttributeDescription m_AutoPlayDuration = new UxmlIntAttributeDescription()
            {
                name = "auto-play-duration",
                defaultValue = k_DefaultAutoPlayDuration,
            };

            readonly UxmlBoolAttributeDescription m_Swipeable = new UxmlBoolAttributeDescription()
            {
                name = "swipeable",
                defaultValue = k_DefaultSwipeable,
            };

            readonly UxmlFloatAttributeDescription m_Resistance = new UxmlFloatAttributeDescription()
            {
                name = "resistance",
                defaultValue = k_DefaultResistance,
            };

            /// <summary>
            /// Returns an enumerable containing UxmlChildElementDescription(typeof(VisualElement)), since VisualElements can contain other VisualElements.
            /// </summary>
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription =>
                new UxmlChildElementDescription[]
                {
                    new UxmlChildElementDescription(typeof(SwipeViewItem))
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

                var el = (SwipeView)ve;
                el.direction = m_Direction.GetValueFromBag(bag, cc);
                el.wrap = m_Wrap.GetValueFromBag(bag, cc);
                el.visibleItemCount = m_VisibleItemCount.GetValueFromBag(bag, cc);
                el.skipAnimationThreshold = m_SkipAnim.GetValueFromBag(bag, cc);
                el.snapAnimationSpeed = m_AnimationSpeed.GetValueFromBag(bag, cc);
                el.startSwipeThreshold = m_StartSwipeThreshold.GetValueFromBag(bag, cc);
                el.autoPlayDuration = m_AutoPlayDuration.GetValueFromBag(bag, cc);
                el.swipeable = m_Swipeable.GetValueFromBag(bag, cc);
                el.resistance = m_Resistance.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}