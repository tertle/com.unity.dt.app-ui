using System;
using System.Collections.Generic;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif
#pragma warning disable CS8524 // The switch expression does not handle some values...

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The direction towards which the SplitView's splitter will move to collapse.
    /// </summary>
    public enum CollapseDirection
    {
        /// <summary>
        /// The splitter will move towards the next splitter in the current layout direction.
        /// </summary>
        Forward = 1,

        /// <summary>
        /// The splitter will move towards the previous splitter in the current layout direction.
        /// </summary>
        Backward = 2
    }

    /// <summary>
    /// A SplitView is a visual element that can be used to split its children into panes.
    /// A SplitView can be either horizontal or vertical, and it will have splitters between the panes.
    /// This component can contain any number of <see cref="Pane"/> elements as children.
    /// </summary>
    /// <seealso cref="Pane"/>
    /// <seealso cref="Splitter"/>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class SplitView : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId directionProperty = nameof(direction);

        internal static readonly BindingId realtimeResizeProperty = nameof(realtimeResize);

        internal static readonly BindingId paneCountProperty = nameof(paneCount);

        internal static readonly BindingId splitterCountProperty = nameof(splitterCount);
#endif

        Direction m_Direction = Direction.Horizontal;

        bool m_RealtimeResize = true;

        readonly VisualElement m_PaneContainer;

        readonly VisualElement m_SplitterContainer;

        internal readonly List<Splitter> splitters = new List<Splitter>();

        static readonly EventCallback<GeometryChangedEvent> k_OnGeometryChanged = OnGeometryChanged;

        Rect m_PreviousRect;

        IVisualElementScheduledItem m_ScheduledRedrawSplitters;

        Dir m_LayoutDirection = Dir.Ltr;

        /// <summary>
        /// The USS class name of the SplitView.
        /// </summary>
        public const string ussClassName = "appui-splitview";

        /// <summary>
        /// The USS class name of the SplitView with a specific direction.
        /// </summary>
        [EnumName("GetOrientationUssClassName", typeof(Direction))]
        public const string directionUssClassName = ussClassName + "--direction-";

        /// <summary>
        /// The USS class name of the pane container.
        /// </summary>
        public const string paneContainerUssClassName = ussClassName + "__pane-container";

        /// <summary>
        /// The USS class name of the splitter container.
        /// </summary>
        public const string splitterContainerUssClassName = ussClassName + "__splitter-container";

        /// <summary>
        /// The USS class name of a child element of the SplitView.
        /// </summary>
        public const string itemUssClassName = ussClassName + "__item";

        /// <summary>
        /// The USS class name of the first child element of the SplitView.
        /// </summary>
        public const string firstItemUssClassName = itemUssClassName + "--first";

        /// <summary>
        /// The USS class name of the last child element of the SplitView.
        /// </summary>
        public const string lastItemUssClassName = itemUssClassName + "--last";

        /// <summary>
        /// Child elements are added to it, usually this is the same as the element itself.
        /// </summary>
        public override VisualElement contentContainer => m_PaneContainer;

        /// <summary>
        /// The direction of the SplitView. A horizontal SplitView will have the splitters between the panes
        /// be vertical, and a vertical SplitView will have the splitters between the panes be horizontal.
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
#if ENABLE_RUNTIME_DATA_BINDINGS
                var changed = m_Direction != value;
#endif
                RemoveFromClassList(GetOrientationUssClassName(m_Direction));
                m_Direction = value;
                AddToClassList(GetOrientationUssClassName(m_Direction));
                RedrawSplitters();

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in directionProperty);
#endif
            }
        }

        /// <summary>
        /// <para>Whether the SplitView should resize in real-time or not.</para>
        /// <para>
        /// When set to true, the SplitView will resize the panes as the user drags the splitter.
        /// When set to false, the SplitView will only resize the panes when the user releases the dragged splitter.
        /// </para>
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool realtimeResize
        {
            get => m_RealtimeResize;
            set
            {
#if ENABLE_RUNTIME_DATA_BINDINGS
                var changed = m_RealtimeResize != value;
#endif
                m_RealtimeResize = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in realtimeResizeProperty);
#endif
            }
        }

        /// <summary>
        /// The number of panes in the SplitView.
        /// </summary>
        public int paneCount => childCount;

        /// <summary>
        /// The number of splitters in the SplitView.
        /// </summary>
        public int splitterCount => splitters.Count;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SplitView()
            : this(Direction.Horizontal)
        {

        }

        /// <summary>
        /// Construct a SplitView with the given direction.
        /// </summary>
        /// <param name="direction"> The direction of the SplitView. </param>
        public SplitView(Direction direction)
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Ignore;

            m_PaneContainer = new VisualElement
            {
                name = paneContainerUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_PaneContainer.AddToClassList(paneContainerUssClassName);
            hierarchy.Add(m_PaneContainer);

            m_SplitterContainer = new VisualElement
            {
                name = splitterContainerUssClassName,
                pickingMode = PickingMode.Ignore,
            };
            m_SplitterContainer.AddToClassList(splitterContainerUssClassName);
            hierarchy.Add(m_SplitterContainer);

            this.direction = direction;
            realtimeResize = true;

            RegisterCallback(k_OnGeometryChanged);
            this.RegisterContextChangedCallback<DirContext>(OnLayoutDirectionChanged);
        }

        static void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (evt.target is SplitView splitView)
            {
                if (evt.newRect.IsValid() && splitView.m_PreviousRect != evt.newRect)
                {
                    splitView.m_PreviousRect = evt.newRect;
                    splitView.RedrawSplitters();
                }
            }
        }

        void OnLayoutDirectionChanged(ContextChangedEvent<DirContext> evt)
        {
            var newDirection = evt.context?.dir ?? Dir.Ltr;
            if (m_LayoutDirection != newDirection)
            {
                m_LayoutDirection = newDirection;
                // Defer because the layout direction might change multiple times in a frame
                DeferRedrawSplitters();
            }
        }

        /// <summary>
        /// Remove the pane at the given index.
        /// </summary>
        /// <param name="pane"> The pane to remove. </param>
        public void RemovePane(Pane pane)
        {
            pane.RemoveFromHierarchy();
            DeferRedrawSplitters();
        }

        /// <summary>
        /// Add a pane to the SplitView.
        /// </summary>
        /// <param name="pane"> The pane to add. </param>
        public void AddPane(Pane pane)
        {
            Add(pane);
            DeferRedrawSplitters();
        }

        /// <summary>
        /// Insert a pane at the given index.
        /// </summary>
        /// <param name="index"> The index to insert the pane at. </param>
        /// <param name="pane"> The pane to insert. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the index is out of range. </exception>
        public void InsertPane(int index, Pane pane)
        {
            Insert(index, pane);
            DeferRedrawSplitters();
        }

        void DeferRedrawSplitters()
        {
            m_ScheduledRedrawSplitters?.Pause();
            m_ScheduledRedrawSplitters = schedule.Execute(RedrawSplitters);
        }

        /// <summary>
        /// Get the pane at the given index.
        /// </summary>
        /// <param name="index"> The index of the pane. </param>
        /// <returns> The pane at the given index. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the index is out of range. </exception>
        public Pane PaneAt(int index)
        {
            return (Pane)ElementAt(index);
        }

        /// <summary>
        /// Get the index of the given pane.
        /// </summary>
        /// <param name="pane"> The pane to get the index of. </param>
        /// <returns> The index of the pane. -1 if the pane is not found. </returns>
        public int IndexOfPane(Pane pane)
        {
            return IndexOf(pane);
        }

        /// <summary>
        /// Redraw the splitters.
        /// </summary>
        void RedrawSplitters()
        {
            if (!layout.IsValid())
                return;

            RepopulateSplitters();

            for (var i = 0; i < splitters.Count; i++)
            {
                var child = ElementAt(i);
                var position = m_Direction switch
                {
                    Direction.Horizontal when m_LayoutDirection == Dir.Rtl => child.layout.xMin,
                    Direction.Horizontal when m_LayoutDirection == Dir.Ltr => child.layout.xMax,
                    Direction.Vertical => child.layout.yMax
                };
                var splitter = splitters[i];

                if (m_Direction == Direction.Horizontal)
                {
                    splitter.style.left = position;
                    splitter.style.top = 0;
                }
                else
                {
                    splitter.style.left = 0;
                    splitter.style.top = position;
                }
            }
        }

        /// <summary>
        /// Refresh the position of the splitter at the given index.
        /// </summary>
        /// <param name="index"> The index of the splitter. </param>
        internal void RefreshSplitterPosition(int index)
        {
            if (index < 0 || index >= splitterCount)
                return;

            var pane = ElementAt(index);
            var paneLayout = pane.layout;

            var splitter = splitters[index];

            if (m_Direction == Direction.Horizontal)
            {
                var position = m_LayoutDirection == Dir.Rtl ? paneLayout.xMin : paneLayout.xMax;
                splitter.style.left = float.IsNaN(position) ? 0 : position;
                splitter.style.top = 0;
            }
            else
            {
                splitter.style.left = 0;
                splitter.style.top = float.IsNaN(paneLayout.yMax) ? 0 : paneLayout.yMax;
            }
        }

        /// <summary>
        /// Called when the splitter at the given index is released.
        /// </summary>
        /// <param name="index"> The index of the splitter. </param>
        /// <param name="worldPosition"> The world position of the splitter. </param>
        internal void OnSplitterUp(int index, Vector2 worldPosition)
        {
            if (realtimeResize || index < 0 || index >= splitterCount)
                return;

            var localPosition = this.WorldToLocal(worldPosition);
            GetRange(index, out var min, out var max);
            var newPosition = GetLegalSplitterPositionFromRange(
                index,
                m_Direction == Direction.Horizontal ? localPosition.x : localPosition.y,
                min,
                max);

            var pane = (Pane)ElementAt(index);
            var nextPane = (Pane)ElementAt(index + 1);

            var paneLayout = pane.layout;
            var nextPaneLayout = nextPane.layout;

            var horizontalPosition = m_LayoutDirection == Dir.Rtl ? paneLayout.xMin : paneLayout.xMax;
            var delta = newPosition - (m_Direction == Direction.Horizontal ? horizontalPosition : paneLayout.yMax);

            if (Mathf.Approximately(delta, 0))
                return;

            if (m_Direction == Direction.Horizontal)
            {
                var sign = m_LayoutDirection == Dir.Ltr ? 1f : -1f;
                if (!pane.stretch)
                    pane.style.width = paneLayout.width + delta * sign;
                if (!nextPane.stretch)
                    nextPane.style.width = nextPaneLayout.width - delta * sign;
            }
            else
            {
                if (!pane.stretch)
                    pane.style.height = paneLayout.height + delta;
                if (!nextPane.stretch)
                    nextPane.style.height = nextPaneLayout.height - delta;
            }

            var paneCompact = Mathf.Approximately(newPosition, m_LayoutDirection == Dir.Ltr ? min : max);
            var nextPaneCompact = Mathf.Approximately(newPosition, m_LayoutDirection == Dir.Ltr ? max : min);
            if (paneCompact != pane.compact)
                pane.compact = paneCompact;
            if (nextPaneCompact != nextPane.compact)
                nextPane.compact = nextPaneCompact;
        }

        /// <summary>
        /// Called when the splitter at the given index is dragged.
        /// </summary>
        /// <param name="index"> The index of the splitter. </param>
        /// <param name="worldPosition"> The world position of the splitter. </param>
        internal void OnSplitterDragged(int index, Vector2 worldPosition)
        {
            if (index < 0 || index >= splitterCount)
                return;

            var localPosition = this.WorldToLocal(worldPosition);
            GetRange(index, out var min, out var max);
            var newPosition = GetLegalSplitterPositionFromRange(
                index,
                m_Direction == Direction.Horizontal ? localPosition.x : localPosition.y,
                min,
                max);
            var currentPosition = m_Direction == Direction.Horizontal
                ? splitters[index].resolvedStyle.left
                : splitters[index].resolvedStyle.top;
            var delta = newPosition - currentPosition;

            if (Mathf.Approximately(delta, 0))
                return;

            if (realtimeResize)
            {
                var pane = (Pane)ElementAt(index);
                var nextPane = (Pane)ElementAt(index + 1);

                var paneLayout = pane.layout;
                var nextPaneLayout = nextPane.layout;

                if (m_Direction == Direction.Horizontal)
                {
                    var sign = m_LayoutDirection == Dir.Ltr ? 1f : -1f;
                    if (!pane.stretch)
                        pane.style.width = paneLayout.width + delta * sign;
                    if (!nextPane.stretch)
                        nextPane.style.width = nextPaneLayout.width - delta * sign;
                }
                else
                {
                    if (!pane.stretch)
                        pane.style.height = paneLayout.height + delta;
                    if (!nextPane.stretch)
                        nextPane.style.height = nextPaneLayout.height - delta;
                }

                var minPaneSize = m_Direction == Direction.Horizontal && m_LayoutDirection == Dir.Rtl ? max : min;
                var minNextPaneSize = m_Direction == Direction.Horizontal && m_LayoutDirection == Dir.Rtl ? min : max;
                var paneCompact = Mathf.Approximately(newPosition, minPaneSize);
                var nextPaneCompact = Mathf.Approximately(newPosition, minNextPaneSize);
                if (paneCompact != pane.compact)
                    pane.compact = paneCompact;
                if (nextPaneCompact != nextPane.compact)
                    nextPane.compact = nextPaneCompact;
            }
            else
            {
                // instead of resizing the panes directly, we should try to move the splitter while taking in account
                // the min/max sizes of the panes and if they are resizable or not

                if (m_Direction == Direction.Horizontal)
                {
                    splitters[index].style.left = newPosition;
                    splitters[index].style.top = 0;
                }
                else
                {
                    splitters[index].style.left = 0;
                    splitters[index].style.top = newPosition;
                }
            }

        }

        /// <summary>
        /// Get the legal position of the splitter at the given index.
        /// </summary>
        /// <param name="index"> The index of the splitter. </param>
        /// <param name="desiredPosition"> The desired position of the splitter. </param>
        /// <returns> The legal position of the splitter. </returns>
        public float GetLegalSplitterPosition(int index, float desiredPosition)
        {
            // Clamp the desired position to the range of the splitter
            GetRange(index, out var min, out var max);
            desiredPosition = Mathf.Clamp(desiredPosition, min, max);

            return GetLegalSplitterPositionFromRange(index, desiredPosition, min, max);
        }

        float GetLegalSplitterPositionFromRange(int index, float desiredPosition, float min, float max)
        {
            // Snap the position to the nearest pane's min size if the threshold is greater than 0.
            var pane = (Pane)ElementAt(index);
            var nextPane = (Pane)ElementAt(index + 1);
            var paneThreshold = m_LayoutDirection == Dir.Rtl && m_Direction == Direction.Horizontal ? nextPane.compactThreshold : pane.compactThreshold;
            var nextPaneThreshold =  m_LayoutDirection == Dir.Rtl && m_Direction == Direction.Horizontal ? pane.compactThreshold : nextPane.compactThreshold;

            if (desiredPosition < min + paneThreshold)
                desiredPosition = min;

            if (desiredPosition > max - nextPaneThreshold)
                desiredPosition = max;

            return desiredPosition;
        }

        /// <summary>
        /// Get the range of the splitter at the given index.
        /// </summary>
        /// <param name="index"> The index of the splitter. </param>
        /// <param name="min"> The minimum position of the splitter. </param>
        /// <param name="max"> The maximum position of the splitter. </param>
        /// <remarks>
        /// The returned value depends on the current layout direction of the SplitView.
        /// </remarks>
        public void GetRange(int index, out float min, out float max)
        {
            min = 0;
            max = m_Direction == Direction.Horizontal ? layout.width : layout.height;

            if (index > 0)
            {
                var previousPane = ElementAt(index - 1);
                if (m_Direction == Direction.Horizontal)
                {
                    if (m_LayoutDirection == Dir.Ltr)
                        min = previousPane.layout.xMax;
                    else
                        max = previousPane.layout.xMin;
                }
                else
                {
                    min = previousPane.layout.yMax;
                }
            }

            var currentPane = ElementAt(index);
            var currentPaneMinSize = m_Direction == Direction.Horizontal ? currentPane.resolvedStyle.minWidth : currentPane.resolvedStyle.minHeight;

            if (m_Direction == Direction.Horizontal)
            {
                if (m_LayoutDirection == Dir.Ltr)
                    min += currentPaneMinSize.value;
                else
                    max -= currentPaneMinSize.value;
            }
            else
            {
                min += currentPaneMinSize.value;
            }

            if (index < childCount - 1)
            {
                var nextPane = ElementAt(index + 1);
                if (m_Direction == Direction.Horizontal)
                {
                    if (m_LayoutDirection == Dir.Ltr)
                        max = nextPane.layout.xMax;
                    else
                        min = nextPane.layout.xMin;
                }
                else
                {
                    max = nextPane.layout.yMax;
                }

                var nextPaneMinSize = m_Direction == Direction.Horizontal ? nextPane.resolvedStyle.minWidth : nextPane.resolvedStyle.minHeight;

                if (m_Direction == Direction.Horizontal)
                {
                    if (m_LayoutDirection == Dir.Ltr)
                        max -= nextPaneMinSize.value;
                    else
                        min += nextPaneMinSize.value;
                }
                else
                {
                    max -= nextPaneMinSize.value;
                }
            }
        }

        void RepopulateSplitters()
        {
            var targetSplitterCount = Mathf.Max(0, paneCount - 1);

            for (var i = splitterCount - 1; i >= targetSplitterCount; i--)
            {
                splitters[i].RemoveFromHierarchy();
                splitters.RemoveAt(i);
            }

            for (var i = splitterCount; i < targetSplitterCount; i++)
            {
                var splitter = new Splitter(this, i);
                splitters.Add(splitter);
                m_SplitterContainer.Add(splitter);
            }

            for (var i = 0; i < paneCount; i++)
            {
                var child = ElementAt(i);
                child.AddToClassList(itemUssClassName);
                child.EnableInClassList(firstItemUssClassName, i == 0);
                child.EnableInClassList(lastItemUssClassName, i == paneCount - 1);
            }
        }

        /// <summary>
        /// Collapse the splitter at the given index in the given direction.
        /// </summary>
        /// <param name="index"> The index of the splitter. </param>
        /// <param name="collapseDirection"> Whether to collapse the splitter forward or not.</param>
        /// <remarks>
        /// <para>
        /// Collapsing forward means that the splitter will move towards the next splitter in the current layout direction
        /// (in Left-to-Right or Top-to-Bottom layout, forward means right or down, respectively).
        /// </para>
        /// <para>If the splitter is already collapsed, this method does nothing.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the index is out of range. </exception>
        public void CollapseSplitter(int index, CollapseDirection collapseDirection)
        {
            if (index < 0 || index >= splitterCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (IsSplitterCollapsed(index))
                return;

            var pane = PaneAt(index);
            var nextPane = PaneAt(index + 1);

            var paneLayout = pane.layout;
            var nextPaneLayout = nextPane.layout;

            if (m_Direction == Direction.Horizontal)
            {
                if (collapseDirection == CollapseDirection.Forward)
                {
                    var delta = nextPaneLayout.width;
                    if (!pane.stretch)
                        pane.style.width = paneLayout.width + delta;
                    nextPane.style.display = DisplayStyle.None;
                    splitters[index].style.left = m_LayoutDirection == Dir.Ltr ? nextPaneLayout.xMax : nextPaneLayout.xMin;
                    splitters[index].collapsedState = Splitter.CollapsedState.Forward;
                }
                else
                {
                    var delta = paneLayout.width;
                    if (!nextPane.stretch)
                        nextPane.style.width = nextPaneLayout.width + delta;
                    pane.style.display = DisplayStyle.None;
                    splitters[index].style.left = m_LayoutDirection == Dir.Ltr ? paneLayout.xMin : paneLayout.xMax;
                    splitters[index].collapsedState = Splitter.CollapsedState.Backward;
                }
            }
            else
            {
                if (collapseDirection == CollapseDirection.Forward)
                {
                    var delta = nextPaneLayout.height;
                    if (!pane.stretch)
                        pane.style.height = paneLayout.height + delta;
                    nextPane.style.display = DisplayStyle.None;
                    splitters[index].style.top = nextPaneLayout.yMax;
                    splitters[index].collapsedState = Splitter.CollapsedState.Forward;
                }
                else
                {
                    var delta = paneLayout.height;
                    if (!nextPane.stretch)
                        nextPane.style.height = nextPaneLayout.height + delta;
                    pane.style.display = DisplayStyle.None;
                    splitters[index].style.top = paneLayout.yMin;
                    splitters[index].collapsedState = Splitter.CollapsedState.Backward;
                }
            }
        }

        float GetTotalPaneSize()
        {
            var totalSize = 0f;
            for (var i = 0; i < paneCount; i++)
            {
                var pane = PaneAt(i);
                totalSize += m_Direction == Direction.Horizontal ? pane.layout.width : pane.layout.height;
            }
            return totalSize;
        }

        float ShrinkPanes(int index, int dir, float spaceToAllocate, bool dryRun = false)
        {
            var calculatedTotalSize = GetTotalPaneSize();
            var containerSize = m_Direction == Direction.Horizontal ? layout.width : layout.height;

            for (var i = index; i >= 0 && i < paneCount; i += dir)
            {
                if (calculatedTotalSize + spaceToAllocate <= containerSize)
                    break;

                var p = PaneAt(i);
                if (p.resolvedStyle.display == DisplayStyle.None)
                    continue;

                var pSize = m_Direction == Direction.Horizontal ? p.layout.width : p.layout.height;
                var pMinSize = m_Direction == Direction.Horizontal ? p.resolvedStyle.minWidth.value : p.resolvedStyle.minHeight.value;
                var delta = pSize - pMinSize;

                // We could have edge cases where pSize < pMinSize,
                // but that should never happen in the flexbox system.
                if (!dryRun && !Mathf.Approximately(0, delta))
                {
                    if (m_Direction == Direction.Horizontal)
                        p.style.width = pSize - delta;
                    else
                        p.style.height = pSize - delta;
                }

                calculatedTotalSize -= delta;
            }

            return calculatedTotalSize;
        }

        /// <summary>
        /// Expand the splitter at the given index (if it is collapsed).
        /// </summary>
        /// <param name="index"> The index of the splitter. </param>
        /// <remarks>
        /// If the splitter is not collapsed, this method does nothing.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the index is out of range. </exception>
        public void ExpandSplitter(int index)
        {
            const float minimumSize = 16f;

            if (index < 0 || index >= splitterCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (!IsSplitterCollapsed(index))
                return;

            var splitter = splitters[index];
            var paneToExpand = splitter.collapsedState == Splitter.CollapsedState.Forward ? PaneAt(index + 1) : PaneAt(index);
            var paneMinSize = m_Direction == Direction.Horizontal ? paneToExpand.resolvedStyle.minWidth.value : paneToExpand.resolvedStyle.minHeight.value;
            var spaceToAllocate = Mathf.Max(minimumSize, paneMinSize);
            var dir = splitter.collapsedState == Splitter.CollapsedState.Forward ? -1 : 1;
            var containerSize = m_Direction == Direction.Horizontal ? layout.width : layout.height;

            // simulate shrinking the others panes to see if there is enough space to expand the splitter
            var simulatedTotalSize = ShrinkPanes(index, dir, spaceToAllocate, true);
            if (simulatedTotalSize + spaceToAllocate > containerSize)
            {
                Debug.LogWarning("Not enough space to expand the splitter. Aborting.");
                return;
            }

            // shrink the others panes for real this time (if needed)
            ShrinkPanes(index, dir, spaceToAllocate);

            // expand the splitter and the pane
            paneToExpand.style.display = DisplayStyle.Flex;
            splitter.collapsedState = Splitter.CollapsedState.None;
            DeferRedrawSplitters();

            if (m_Direction == Direction.Horizontal)
                paneToExpand.style.width = spaceToAllocate;
            else
                paneToExpand.style.height = spaceToAllocate;
        }

        /// <summary>
        /// Whether the splitter at the given index is collapsed or not.
        /// </summary>
        /// <param name="index"> The index of the splitter. </param>
        /// <returns> True if the splitter is collapsed, false otherwise. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the index is out of range. </exception>
        public bool IsSplitterCollapsed(int index)
        {
            if (index < 0 || index >= splitterCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            return splitters[index].collapsed;
        }

        /// <summary>
        /// Save the state of the SplitView.
        /// </summary>
        /// <returns> The state of the SplitView. </returns>
        public State SaveState()
        {
            var paneSize = new List<float>();
            var collapsedPanes = new List<bool>();

            for (var i = 0; i < childCount; i++)
            {
                var pane = ElementAt(i);
                var isStretch = pane.resolvedStyle.flexGrow > 0;
                var size = isStretch ? -1 : m_Direction switch
                {
                    Direction.Horizontal => pane.layout.width,
                    Direction.Vertical => pane.layout.height
                };
                paneSize.Add(size);
                collapsedPanes.Add(pane is Pane {compact: true});
            }

            return new State
            {
                direction = direction,
                realtimeResize = realtimeResize,
                paneSizes = paneSize,
                collapsedPanes = collapsedPanes
            };
        }

        /// <summary>
        /// Restore the state of the SplitView.
        /// </summary>
        /// <param name="state"> The state to restore. </param>
        public void RestoreState(State state)
        {
            direction = state.direction;
            realtimeResize = state.realtimeResize;

            if (state.paneSizes != null)
            {
                for (var i = 0; i < state.paneSizes.Count; i++)
                {
                    if (i >= childCount)
                        break;

                    var pane = ElementAt(i);
                    var size = state.paneSizes[i];
                    if (size < 0)
                        pane.style.flexGrow = 1;
                    else
                    {
                        if (m_Direction == Direction.Horizontal)
                            pane.style.width = size;
                        else
                            pane.style.height = size;
                    }

                    if (pane is Pane p)
                        p.compact = state.collapsedPanes[i];
                }
            }
        }

        /// <summary>
        /// The state of the SplitView.
        /// </summary>
        [Serializable]
        public struct State
        {
            /// <summary>
            /// The direction of the SplitView.
            /// </summary>
            public Direction direction;

            /// <summary>
            /// Whether the SplitView should resize in real-time or not.
            /// </summary>
            public bool realtimeResize;

            /// <summary>
            /// The sizes of the panes.
            /// </summary>
            public List<float> paneSizes;

            /// <summary>
            /// Whether the panes are collapsed or not.
            /// </summary>
            public List<bool> collapsedPanes;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="SplitView"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
        {
            /// <summary>
            /// Describes the types of element that can appear as children of this element in a UXML file.
            /// </summary>
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription => new[]
            {
                new UxmlChildElementDescription(typeof(Pane))
            };
        }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="SplitView"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_RealtimeResize = new UxmlBoolAttributeDescription
            {
                name = "realtime-resize",
                defaultValue = true
            };

            readonly UxmlEnumAttributeDescription<Direction> m_Orientation = new UxmlEnumAttributeDescription<Direction>
            {
                name = "direction",
                defaultValue = Direction.Horizontal,
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
                var el = (SplitView)ve;
                el.realtimeResize = m_RealtimeResize.GetValueFromBag(bag, cc);
                el.direction = m_Orientation.GetValueFromBag(bag, cc);
            }
        }
#endif
    }

    /// <summary>
    /// The splitter between the panes.
    /// </summary>
    public partial class Splitter : VisualElement
    {
        /// <summary>
        /// The collapse state the splitter.
        /// </summary>
        [GenerateLowerCaseStrings]
        public enum CollapsedState
        {
            /// <summary>
            /// The splitter is not collapsed.
            /// </summary>
            None = 0,

            /// <summary>
            /// The splitter is collapsed forward.
            /// </summary>
            Forward = CollapseDirection.Forward,

            /// <summary>
            /// The splitter is collapsed backward.
            /// </summary>
            Backward = CollapseDirection.Backward
        }

        readonly SplitView m_SplitView;

        readonly int m_Index;

        readonly VisualElement m_Anchor;

        readonly Draggable m_Draggable;

        readonly VisualElement m_ExpandButton;

        readonly Pressable m_ExpandButtonPressable;

        readonly Image m_ExpandIcon;

        CollapsedState m_CollapsedState;

        /// <summary>
        /// The USS class name of the Splitter.
        /// </summary>
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public const string ussClassName = "appui-splitter";

        /// <summary>
        /// The USS class name of the anchor of the Splitter.
        /// </summary>
        public const string anchorUssClassName = ussClassName + "__anchor";

        /// <summary>
        /// The USS class name of the expand button of the Splitter.
        /// </summary>
        public const string expandButtonUssClassName = ussClassName + "__expand-button";

        /// <summary>
        /// The USS class name of the expand icon of the Splitter.
        /// </summary>
        public const string expandIconUsClassName = ussClassName + "__expand-icon";

        /// <summary>
        /// The USS class name of the Splitter for its collapsed state.
        /// </summary>
        [EnumName("GenerateCollapsedUssClassName", typeof(CollapsedState))]
        public const string collapsedStateUssClassName = ussClassName + "--collapsed-";

        /// <summary>
        /// The USS class name of the Splitter when it is collapsed.
        /// </summary>
        public const string collapsedUssClassName = ussClassName + "--collapsed";

        /// <summary>
        /// Whether the splitter is collapsed or not.
        /// </summary>
        internal bool collapsed => m_CollapsedState != CollapsedState.None;

        /// <summary>
        /// The collapse state of the splitter.
        /// </summary>
        internal CollapsedState collapsedState
        {
            get => m_CollapsedState;
            set
            {
                if (m_CollapsedState != CollapsedState.None)
                    RemoveFromClassList(GenerateCollapsedUssClassName(m_CollapsedState));
                m_CollapsedState = value;
                if (m_CollapsedState != CollapsedState.None)
                    AddToClassList(GenerateCollapsedUssClassName(m_CollapsedState));
                EnableInClassList(collapsedUssClassName, m_CollapsedState != CollapsedState.None);
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="splitView"> The SplitView the splitter belongs to. </param>
        /// <param name="index"> The index of the splitter. </param>
        internal Splitter(SplitView splitView, int index)
        {
            AddToClassList(ussClassName);
            pickingMode = PickingMode.Position;
            usageHints |= UsageHints.DynamicTransform;

            m_SplitView = splitView;
            m_Index = index;

            m_Anchor = new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                name = anchorUssClassName
            };
            m_Anchor.AddToClassList(anchorUssClassName);
            hierarchy.Add(m_Anchor);

            m_ExpandButton = new VisualElement
            {
                pickingMode = PickingMode.Position,
                name = expandButtonUssClassName
            };
            m_ExpandButton.AddToClassList(expandButtonUssClassName);
            hierarchy.Add(m_ExpandButton);

            m_ExpandIcon = new Image
            {
                name = expandIconUsClassName,
                pickingMode = PickingMode.Ignore
            };
            m_ExpandIcon.AddToClassList(expandIconUsClassName);
            m_ExpandButton.Add(m_ExpandIcon);

            m_Draggable = new Draggable(k_OnClick, k_OnDrag, k_OnPointerUp);
            m_Draggable.acceptDrag = k_AcceptDrag;
            this.AddManipulator(m_Draggable);

            m_ExpandButtonPressable = new Pressable(OnExpandButtonClicked);
            m_ExpandButton.AddManipulator(m_ExpandButtonPressable);

            collapsedState = CollapsedState.None;
        }

        void OnExpandButtonClicked()
        {
            m_SplitView.ExpandSplitter(m_Index);
        }

        static readonly Action k_OnClick = OnClick;
        static readonly Action<Draggable> k_OnDrag = OnDrag;
        static readonly Action<Draggable> k_OnPointerUp = OnPointerUp;
        static readonly Func<Draggable, bool> k_AcceptDrag = AcceptDrag;

        static void OnClick()
        {
            // Do nothing
        }

        static void OnDrag(Draggable draggable)
        {
            if (draggable.target is Splitter splitter)
            {
                splitter.m_SplitView.OnSplitterDragged(splitter.m_Index, draggable.position);
            }
        }

        static void OnPointerUp(Draggable draggable)
        {
            if (draggable.target is Splitter splitter)
            {
                splitter.m_SplitView.OnSplitterUp(splitter.m_Index, draggable.position);
            }
        }

        static bool AcceptDrag(Draggable draggable)
        {
            return draggable.target is Splitter { collapsed: false };
        }
    }
}
