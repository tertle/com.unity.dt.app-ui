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
    /// A view containing recycled rows with items inside.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class GridView : BaseGridView
    {
        /// <summary>
        /// Available Operations.
        /// </summary>
        [Flags]
        public enum GridOperations
        {
            /// <summary>
            /// No operation.
            /// </summary>
            None = 0,

            /// <summary>
            /// Select all items.
            /// </summary>
            SelectAll = 1 << 0,

            /// <summary>
            /// Cancel selection.
            /// </summary>
            Cancel = 1 << 1,

            /// <summary>
            /// Move selection cursor left.
            /// </summary>
            Left = 1 << 2,

            /// <summary>
            /// Move selection cursor right.
            /// </summary>
            Right = 1 << 3,

            /// <summary>
            /// Move selection cursor up.
            /// </summary>
            Up = 1 << 4,

            /// <summary>
            /// Move selection cursor down.
            /// </summary>
            Down = 1 << 5,

            /// <summary>
            /// Move selection cursor to the beginning of the list.
            /// </summary>
            Begin = 1 << 6,

            /// <summary>
            /// Move selection cursor to the end of the list.
            /// </summary>
            End = 1 << 7,

            /// <summary>
            /// Choose selected items.
            /// </summary>
            Choose = 1 << 8,
        }

#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId itemHeightProperty = new BindingId(nameof(itemHeight));

#endif
        const float k_PageSizeFactor = 0.25f;

        const int k_ExtraVisibleRows = 2;

        /// <summary>
        /// The USS class name of rows in the GridView.
        /// </summary>
        const string k_RowUssClassName = ussClassName + "__row";

        const int k_DefaultItemHeight = 30;

        static CustomStyleProperty<int> s_ItemHeightProperty = new CustomStyleProperty<int>("--unity-item-height");

        int m_ItemHeight = k_DefaultItemHeight;

        bool m_ItemHeightIsInline;

        float m_LastHeight;

        // we keep this list in order to minimize temporary gc allocs
        List<RecycledRow> m_ScrollInsertionList = new List<RecycledRow>();

        // Persisted.
        float m_ScrollOffset;

        int m_VisibleRowCount;

        int m_FirstVisibleIndex;

        float m_LastPadding;

        /// <summary>
        /// Creates a <see cref="GridView"/> with all default properties. The <see cref="BaseGridView.itemsSource"/>,
        /// <see cref="GridView.itemHeight"/>, <see cref="BaseGridView.makeItem"/> and <see cref="BaseGridView.bindItem"/> properties
        /// must all be set for the GridView to function properly.
        /// </summary>
        public GridView()
        {
            m_ScrollOffset = 0.0f;
            // Scroll views with virtualized content shouldn't have the "view transform" optimization
            scrollView.contentContainer.usageHints &= ~UsageHints.GroupTransform;
        }

        /// <summary>
        /// Constructs a <see cref="GridView"/>, with all required properties provided.
        /// </summary>
        /// <param name="itemsSource">The list of items to use as a data source.</param>
        /// <param name="makeItem">The factory method to call to create a display item. The method should return a
        /// VisualElement that can be bound to a data item.</param>
        /// <param name="bindItem">The method to call to bind a data item to a display item. The method
        /// receives as parameters the display item to bind, and the index of the data item to bind it to.</param>
        public GridView(IList itemsSource, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
            : base(itemsSource, makeItem, bindItem)
        {
            m_ItemHeightIsInline = true;
            m_ScrollOffset = 0.0f;
            // Scroll views with virtualized content shouldn't have the "view transform" optimization
            scrollView.contentContainer.usageHints &= ~UsageHints.GroupTransform;
        }

        /// <summary>
        /// The height of a single item in the list, in pixels.
        /// </summary>
        /// <remarks>
        /// GridView requires that all visual elements have the same height so that it can calculate the
        /// scroller size.
        ///
        /// This property must be set for the list view to function.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int itemHeight
        {
            get => m_ItemHeight;
            set
            {
                if (m_ItemHeight != value && value > 0)
                {
                    m_ItemHeightIsInline = true;
                    m_ItemHeight = value;
                    scrollView.verticalPageSize = m_ItemHeight * k_PageSizeFactor;
                    Refresh();

#if ENABLE_RUNTIME_DATA_BINDINGS
                    NotifyPropertyChanged(in itemHeightProperty);
#endif
                }
            }
        }

        /// <summary>
        /// The computed pixel-aligned height for the list elements.
        /// </summary>
        /// <remarks>
        /// This value changes depending on the current panel's DPI scaling.
        /// </remarks>
        /// <seealso cref="GridView.itemHeight"/>
        public float resolvedItemHeight
        {
            get
            {
                var dpiScaling = 1f;
                return Mathf.Round(itemHeight * dpiScaling) / dpiScaling;
            }
        }

        internal List<RecycledRow> rowPool { get; private set; } = new List<RecycledRow>();

        /// <summary>
        /// Clears the GridView, recreates all visible visual elements, and rebinds all items.
        /// </summary>
        /// <remarks>
        /// Call this method whenever the data source changes.
        /// </remarks>
        public override void Refresh()
        {
            base.Refresh();

            foreach (var recycledRow in rowPool)
            {
                recycledRow.Clear();
            }

            rowPool.Clear();
            m_VisibleRowCount = 0;
            m_ScrollOffset = 0;

            if (!HasValidDataAndBindings())
                return;

            m_LastHeight = scrollView.layout.height;

            if (float.IsNaN(m_LastHeight))
                return;

            m_FirstVisibleIndex = Math.Min((int)(m_ScrollOffset / resolvedItemHeight) * columnCount, itemsSource.Count - 1);
            OnContainerHeightChanged(m_LastHeight);

            if (!allowNoSelection && selectionCount == 0)
            {
                if (itemsSource.Count > 0)
                    SetSelectionInternal(new[]
                    {
                        m_FirstVisibleIndex >= 0 ? m_FirstVisibleIndex : 0
                    }, true, true);
            }
            else
            {
                PostSelection(true, true);
            }
        }

        /// <summary>
        /// Scrolls to a specific item index and makes it visible.
        /// </summary>
        /// <param name="index">Item index to scroll to. Specify -1 to make the last item visible.</param>
        public override void ScrollToItem(int index)
        {
            if (!HasValidDataAndBindings())
                return;

            if (m_VisibleRowCount == 0 || index < -1)
                return;

            var pixelAlignedItemHeight = resolvedItemHeight;
            var lastRowIndex = Mathf.FloorToInt((itemsSource.Count - 1) / (float) columnCount);
            var maxOffset = Mathf.Max(0, lastRowIndex * pixelAlignedItemHeight - m_LastHeight + pixelAlignedItemHeight);
            var targetRowIndex = Mathf.FloorToInt(index / (float) columnCount);
            var targetOffset = targetRowIndex * pixelAlignedItemHeight;
            var currentOffset = scrollView.scrollOffset.y;
            var d = targetOffset - currentOffset;

            if (index == -1)
            {
                scrollView.scrollOffset = Vector2.up * maxOffset;
            }
            else if (d < 0)
            {
                scrollView.scrollOffset = Vector2.up * targetOffset;
            }
            else if (d > m_LastHeight - pixelAlignedItemHeight)
            {
                // need to scroll up so the item should be visible in last row
                targetOffset += pixelAlignedItemHeight - m_LastHeight;
                scrollView.scrollOffset = Vector2.up * Mathf.Min(maxOffset, targetOffset);
            }
            // else do nothing because the item is already entirely visible

            schedule.Execute(() => OnContainerHeightChanged(m_LastHeight)).ExecuteLater(2L);
        }

        /// <inheritdoc cref="BaseGridView.GetVisualElementInternal"/>
        protected override VisualElement GetVisualElementInternal(int index)
        {
            var id = GetIdFromIndex(index);

            foreach (var recycledRow in rowPool)
            {
                if (recycledRow.ContainsId(id, out var indexInRow))
                    return recycledRow.ElementAt(indexInRow);
            }

            return null;
        }

        /// <inheritdoc cref="BaseGridView.OnCustomStyleResolved"/>
        protected override void OnCustomStyleResolved(CustomStyleResolvedEvent e)
        {
            int height;
            if (!m_ItemHeightIsInline && e.customStyle.TryGetValue(s_ItemHeightProperty, out height))
            {
                if (m_ItemHeight != height)
                {
                    m_ItemHeight = height;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Returns the index of the item at the given position.
        /// </summary>
        /// <remarks>
        /// The position is relative to the top left corner of the grid. No check is made to see if the index is valid.
        /// </remarks>
        /// <param name="worldPosition">The position of the item in the world-space.</param>
        /// <returns> The index of the item at the given position.</returns>
        public override int GetIndexByWorldPosition(Vector2 worldPosition)
        {
            var localPosition = scrollView.contentContainer.WorldToLocal(worldPosition);
            var totalWidth = scrollView.contentContainer.layout.width;
            var dir = this.GetContext<DirContext>()?.dir ?? Dir.Ltr;
            var posX = dir == Dir.Ltr ? localPosition.x : totalWidth - localPosition.x;
            return Mathf.FloorToInt(localPosition.y / resolvedItemHeight) * columnCount + Mathf.FloorToInt(posX / resolvedItemWidth);
        }

        VisualElement CreateDummyItemElement()
        {
            var item = new VisualElement();
            SetupItemElement(item);
            return item;
        }

        /// <inheritdoc cref="BaseGridView.OnScroll"/>
        protected override void OnScroll(float offset)
        {
            if (!HasValidDataAndBindings())
                return;

            m_ScrollOffset = offset;
            var pixelAlignedItemHeight = resolvedItemHeight;
            var firstVisibleIndex = Mathf.FloorToInt(offset / pixelAlignedItemHeight) * columnCount;

            scrollView.contentContainer.style.paddingTop = Mathf.FloorToInt(firstVisibleIndex / (float)columnCount) * pixelAlignedItemHeight;
            scrollView.contentContainer.style.height = (Mathf.CeilToInt(itemsSource.Count / (float)columnCount) * pixelAlignedItemHeight);

            if (firstVisibleIndex != m_FirstVisibleIndex)
            {
                m_FirstVisibleIndex = firstVisibleIndex;

                if (rowPool.Count > 0)
                {
                    // we try to avoid rebinding a few items
                    if (m_FirstVisibleIndex < rowPool[0].firstIndex) //we're scrolling up
                    {
                        //How many do we have to swap back
                        var count = rowPool[0].firstIndex - m_FirstVisibleIndex;

                        var inserting = m_ScrollInsertionList;

                        for (var i = 0; i < count && rowPool.Count > 0; ++i)
                        {
                            var last = rowPool[rowPool.Count - 1];
                            inserting.Add(last);
                            rowPool.RemoveAt(rowPool.Count - 1); //we remove from the end

                            last.SendToBack(); //We send the element to the top of the list (back in z-order)
                        }

                        inserting.Reverse();

                        m_ScrollInsertionList = rowPool;
                        rowPool = inserting;
                        rowPool.AddRange(m_ScrollInsertionList);
                        m_ScrollInsertionList.Clear();
                    }
                    else if (m_FirstVisibleIndex > rowPool[0].firstIndex) //down
                    {
                        var inserting = m_ScrollInsertionList;

                        var checkIndex = 0;
                        while (checkIndex < rowPool.Count && m_FirstVisibleIndex > rowPool[checkIndex].firstIndex)
                        {
                            var first = rowPool[checkIndex];
                            inserting.Add(first);
                            first.BringToFront(); //We send the element to the bottom of the list (front in z-order)
                            checkIndex++;
                        }

                        rowPool.RemoveRange(0, checkIndex); //we remove them all at once
                        rowPool.AddRange(inserting); // add them back to the end
                        inserting.Clear();
                    }

                    //Let's rebind everything
                    for (var rowIndex = 0; rowIndex < rowPool.Count; rowIndex++)
                    {
                        for (var colIndex = 0; colIndex < columnCount; colIndex++)
                        {
                            var index = rowIndex * columnCount + colIndex + m_FirstVisibleIndex;

                            var isFirstColumn = colIndex == 0;
                            var isLastColumn = colIndex == columnCount - 1;

                            if (index < itemsSource.Count)
                            {
                                var item = rowPool[rowIndex].ElementAt(colIndex);
                                if (rowPool[rowIndex].indices[colIndex] == RecycledRow.kUndefinedIndex)
                                {
                                    var newItem = makeItem != null ? makeItem.Invoke() : CreateDummyItemElement();
                                    SetupItemElement(newItem);
                                    rowPool[rowIndex].RemoveAt(colIndex);
                                    rowPool[rowIndex].Insert(colIndex, newItem);
                                    item = newItem;
                                }

                                Setup(item, index);
                                item.EnableInClassList(firstColumnUssClassName, isFirstColumn);
                                item.EnableInClassList(lastColumnUssClassName, isLastColumn);
                            }
                            else
                            {
                                var remainingOldItems = columnCount - colIndex;

                                while (remainingOldItems > 0)
                                {
                                    rowPool[rowIndex].RemoveAt(colIndex);
                                    rowPool[rowIndex].Insert(colIndex, CreateDummyItemElement());
                                    rowPool[rowIndex][colIndex].EnableInClassList(firstColumnUssClassName, isFirstColumn);
                                    rowPool[rowIndex][colIndex].EnableInClassList(lastColumnUssClassName, isLastColumn);
                                    rowPool[rowIndex].ids.RemoveAt(colIndex);
                                    rowPool[rowIndex].ids.Insert(colIndex, RecycledRow.kUndefinedIndex);
                                    rowPool[rowIndex].indices.RemoveAt(colIndex);
                                    rowPool[rowIndex].indices.Insert(colIndex, RecycledRow.kUndefinedIndex);
                                    remainingOldItems--;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <inheritdoc cref="BaseGridView.OnContainerHeightChanged"/>
        protected override void OnContainerHeightChanged(float height)
        {
            if (!HasValidDataAndBindings())
                return;

            var pixelAlignedItemHeight = resolvedItemHeight;
            var rowCountForSource = Mathf.CeilToInt(itemsSource.Count / (float)columnCount);
            var contentHeight = rowCountForSource * pixelAlignedItemHeight;
            scrollView.contentContainer.style.height = contentHeight;

            var scrollableHeight = Mathf.Max(0, contentHeight - scrollView.contentViewport.layout.height);
            scrollView.verticalScroller.highValue = scrollableHeight;
            scrollView.verticalScroller.value = Mathf.Min(m_ScrollOffset, scrollView.verticalScroller.highValue);

            var rowCountForHeight = Mathf.FloorToInt(height / pixelAlignedItemHeight) + k_ExtraVisibleRows;
            var rowCount = Math.Min(rowCountForHeight, rowCountForSource);

            if (m_VisibleRowCount != rowCount)
            {
                if (m_VisibleRowCount > rowCount)
                {
                    // Shrink
                    var removeCount = m_VisibleRowCount - rowCount;
                    for (var i = 0; i < removeCount; i++)
                    {
                        var lastIndex = rowPool.Count - 1;
                        rowPool[lastIndex].Clear();
                        scrollView.Remove(rowPool[lastIndex]);
                        rowPool.RemoveAt(lastIndex);
                    }
                }
                else
                {
                    // Grow
                    var addCount = rowCount - m_VisibleRowCount;
                    for (var i = 0; i < addCount; i++)
                    {
                        var recycledRow = new RecycledRow(resolvedItemHeight);

                        for (var indexInRow = 0; indexInRow < columnCount; indexInRow++)
                        {
                            var index = rowPool.Count * columnCount + indexInRow + m_FirstVisibleIndex;
                            var item = makeItem != null && index < itemsSource.Count ? makeItem.Invoke() : CreateDummyItemElement();
                            SetupItemElement(item);

                            recycledRow.Add(item);

                            if (index < itemsSource.Count)
                            {
                                Setup(item, index);
                            }
                            else
                            {
                                recycledRow.ids.Add(RecycledRow.kUndefinedIndex);
                                recycledRow.indices.Add(RecycledRow.kUndefinedIndex);
                            }

                            var isFirstColumn = indexInRow == 0;
                            var isLastColumn = indexInRow == columnCount - 1;
                            item.EnableInClassList(firstColumnUssClassName, isFirstColumn);
                            item.EnableInClassList(lastColumnUssClassName, isLastColumn);
                        }

                        rowPool.Add(recycledRow);
                        recycledRow.style.height = pixelAlignedItemHeight;

                        scrollView.Add(recycledRow);
                    }
                }

                m_VisibleRowCount = rowCount;
            }

            m_LastHeight = height;
        }

        /// <inheritdoc cref="BaseGridView.ApplySelectedState"/>
        protected override void ApplySelectedState()
        {
            if (!HasValidDataAndBindings())
                return;

            foreach (var recycledRow in rowPool)
            {
                for (var c = 0; c < columnCount; c++)
                {
                    var index = recycledRow.indices[c];
                    var id = recycledRow.ids[c];
                    recycledRow.SetSelected(c, index != RecycledRow.kUndefinedIndex && IsSelectedId(id));
                }
            }
        }

        void Setup(VisualElement item, int newIndex)
        {
            var newId = GetIdFromIndex(newIndex);

            if (!(item.parent is RecycledRow recycledRow))
                throw new Exception("The item to setup can't be orphan");

            var indexInRow = recycledRow.IndexOf(item);

            if (recycledRow.indices.Count <= indexInRow)
            {
                recycledRow.indices.Add(RecycledRow.kUndefinedIndex);
                recycledRow.ids.Add(RecycledRow.kUndefinedIndex);
            }

            if (recycledRow.indices[indexInRow] == newIndex)
                return;

            if (recycledRow.indices[indexInRow] != RecycledRow.kUndefinedIndex)
                unbindItem?.Invoke(item, recycledRow.indices[indexInRow]);

            recycledRow.indices[indexInRow] = newIndex;
            recycledRow.ids[indexInRow] = newId;

            bindItem.Invoke(item, recycledRow.indices[indexInRow]);

            recycledRow.SetSelected(indexInRow, IsSelectedId(recycledRow.ids[indexInRow]));
        }

        void SetupItemElement(VisualElement item)
        {
            item.AddToClassList(itemUssClassName);
            item.style.position = Position.Relative;
            item.style.flexBasis = 0;
            item.style.flexGrow = 1f;
            item.style.flexShrink = 1f;
        }

#if ENABLE_UXML_TRAITS


        /// <summary>
        /// Instantiates a <see cref="GridView"/> using data from a UXML file.
        /// </summary>
        /// <remarks>
        /// This class is added to every <see cref="VisualElement"/> created from UXML.
        /// </remarks>
        public new class UxmlFactory : UxmlFactory<GridView, UxmlTraits> {}

        /// <summary>
        /// Defines <see cref="UxmlTraits"/> for the <see cref="GridView"/>.
        /// </summary>
        /// <remarks>
        /// This class defines the GridView element properties that you can use in a UI document asset (UXML file).
        /// </remarks>
        public new class UxmlTraits : BaseGridView.UxmlTraits
        {
            readonly UxmlIntAttributeDescription m_ItemHeight = new UxmlIntAttributeDescription
            {
                name = "item-height",
                obsoleteNames = new[] { "itemHeight" },
                defaultValue = k_DefaultItemHeight
            };

            /// <summary>
            /// Initializes <see cref="GridView"/> properties using values from the attribute bag.
            /// </summary>
            /// <param name="ve">The object to initialize.</param>
            /// <param name="bag">The attribute bag.</param>
            /// <param name="cc">The creation context; unused.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var view = (GridView)ve;

                // Avoid setting itemHeight unless it's explicitly defined.
                // Setting itemHeight property will activate inline property mode.
                var itemHeight = 0;
                if (m_ItemHeight.TryGetValueFromBag(bag, cc, ref itemHeight))
                    view.itemHeight = itemHeight;
            }
        }

#endif

        internal class RecycledRow : BaseVisualElement
        {
            public const int kUndefinedIndex = -1;

            public readonly List<int> ids;

            public readonly List<int> indices;

            public RecycledRow(float height)
            {
                pickingMode = PickingMode.Ignore;
                AddToClassList(k_RowUssClassName);
                style.height = height;

                indices = new List<int>();
                ids = new List<int>();
            }

            public int firstIndex => indices.Count > 0 ? indices[0] : kUndefinedIndex;
            public int lastIndex => indices.Count > 0 ? indices[indices.Count - 1] : kUndefinedIndex;

            public void ClearSelection()
            {
                for (var i = 0; i < childCount; i++)
                {
                    SetSelected(i, false);
                }
            }

            public bool ContainsId(int id, out int indexInRow)
            {
                indexInRow = ids.IndexOf(id);
                return indexInRow >= 0;
            }

            public bool ContainsIndex(int index, out int indexInRow)
            {
                indexInRow = indices.IndexOf(index);
                return indexInRow >= 0;
            }

            public void SetSelected(int indexInRow, bool selected)
            {
                if (childCount > indexInRow && indexInRow >= 0)
                {
                    if (selected)
                    {
                        ElementAt(indexInRow).AddToClassList(Styles.selectedUssClassName);
                    }
                    else
                    {
                        ElementAt(indexInRow).RemoveFromClassList(Styles.selectedUssClassName);
                    }
                }
            }
        }
    }
}
