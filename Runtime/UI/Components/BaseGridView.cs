using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.AppUI.Bridge;
using System.Runtime.CompilerServices;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// The base class for GridView elements.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class BaseGridView : BindableElement, ISerializationCallbackReceiver
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId selectionTypeProperty = new BindingId(nameof(selectionType));

        internal static readonly BindingId allowNoSelectionProperty = new BindingId(nameof(allowNoSelection));

#endif

        /// <summary>
        /// The USS class name for GridView elements.
        /// </summary>
        /// <remarks>
        /// Unity adds this USS class to every instance of the GridView element. Any styling applied to
        /// this class affects every GridView located beside, or below the stylesheet in the visual tree.
        /// </remarks>
        public const string ussClassName = "appui-grid-view";

        /// <summary>
        /// The USS class name of item elements in GridView elements.
        /// </summary>
        /// <remarks>
        /// Unity adds this USS class to every item element the GridView contains. Any styling applied to
        /// this class affects every item element located beside, or below the stylesheet in the visual tree.
        /// </remarks>
        public static readonly string itemUssClassName = ussClassName + "__item";

        /// <summary>
        /// The first column USS class name for GridView elements.
        /// </summary>
        public static readonly string firstColumnUssClassName = ussClassName + "__first-column";

        /// <summary>
        /// The last column USS class name for GridView elements.
        /// </summary>
        public static readonly string lastColumnUssClassName = ussClassName + "__last-column";

        const bool k_DefaultPreventScrollWithModifiers = true;

        /// <summary>
        /// The <see cref="UnityEngine.UIElements.ScrollView"/> used by the GridView.
        /// </summary>
        public ScrollView scrollView { get; }

        readonly List<int> m_SelectedIds = new List<int>();

        readonly List<int> m_SelectedIndices = new List<int>();

        readonly List<object> m_SelectedItems = new List<object>();

        readonly List<int> m_PreviouslySelectedIndices = new List<int>();

        readonly List<int> m_OriginalSelection = new List<int>();

        float m_OriginalScrollOffset;

        int m_SoftSelectIndex = -1;

        Action<VisualElement, int> m_BindItem;

        int m_ColumnCount = 1;

        Func<int, int> m_GetItemId;

        IList m_ItemsSource;

        Func<VisualElement> m_MakeItem;

        int m_RangeSelectionOrigin = -1;

        bool m_IsRangeSelectionDirectionUp;

        SelectionType m_SelectionType;

        bool m_AllowNoSelection = true;

        NavigationMoveEvent m_NavigationMoveAdapter;

        NavigationCancelEvent m_NavigationCancelAdapter;

        bool m_HasPointerMoved;

        bool m_SoftSelectIndexWasPreviouslySelected;

        /// <summary>
        /// Creates a <see cref="BaseGridView"/> with all default properties.
        /// </summary>
        protected BaseGridView()
        {
            AddToClassList(ussClassName);

            selectionType = SelectionType.Single;
            scrollView = new ScrollView
            {
                viewDataKey = "grid-view__scroll-view",
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };
            scrollView.StretchToParentSize();
            scrollView.contentContainer.SetDisableClipping(false);
            scrollView.verticalScroller.valueChanged += OnScroll;

            dragger = new Dragger(OnDraggerStarted, OnDraggerMoved, OnDraggerEnded, OnDraggerCanceled);
            dragger.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });

            RegisterCallback<GeometryChangedEvent>(OnSizeChanged);
            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);

            hierarchy.Add(scrollView);

            focusable = true;
            dragger.acceptStartDrag = DefaultAcceptStartDrag;
        }

        /// <summary>
        /// Constructs a <see cref="BaseGridView"/>, with all required properties provided.
        /// </summary>
        /// <param name="itemsSource">The list of items to use as a data source.</param>
        /// <param name="makeItem">The factory method to call to create a display item. The method should return a
        /// VisualElement that can be bound to a data item.</param>
        /// <param name="bindItem">The method to call to bind a data item to a display item. The method
        /// receives as parameters the display item to bind, and the index of the data item to bind it to.</param>
        public BaseGridView(IList itemsSource, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
            : this()
        {
            m_ItemsSource = itemsSource;
            m_MakeItem = makeItem;
            m_BindItem = bindItem;

            operationMask = ~GridView.GridOperations.None;
        }

        bool Apply(GridView.GridOperations operation, bool shiftKey)
        {
            if ((operation & operationMask) == 0)
                return false;

            void HandleSelectionAndScroll(int index)
            {
                if (selectionType == SelectionType.Multiple && shiftKey && m_SelectedIndices.Count != 0)
                    DoRangeSelection(index, true, true);
                else
                    selectedIndex = index;

                ScrollToItem(index);
            }

            var dir = this.GetContext<DirContext>()?.dir ?? Dir.Ltr;

            switch (operation)
            {
                case GridView.GridOperations.None:
                    break;
                case GridView.GridOperations.SelectAll:
                    SelectAll();
                    return true;
                case GridView.GridOperations.Cancel:
                    ClearSelection();
                    return true;
                case GridView.GridOperations.Left when dir is Dir.Ltr:
                case GridView.GridOperations.Right when dir is Dir.Rtl:
                {
                    var newIndex = Mathf.Max(selectedIndex - 1, 0);
                    if (newIndex != selectedIndex)
                    {
                        HandleSelectionAndScroll(newIndex);
                        return true;
                    }
                }
                    break;
                case GridView.GridOperations.Right when dir is Dir.Ltr:
                case GridView.GridOperations.Left when dir is Dir.Rtl:
                {
                    var newIndex = Mathf.Min(selectedIndex + 1, itemsSource.Count - 1);
                    if (newIndex != selectedIndex)
                    {
                        HandleSelectionAndScroll(newIndex);
                        return true;
                    }
                }
                    break;
                case GridView.GridOperations.Up:
                {
                    var newIndex = Mathf.Max(selectedIndex - columnCount, 0);
                    if (newIndex != selectedIndex)
                    {
                        HandleSelectionAndScroll(newIndex);
                        return true;
                    }
                }
                    break;
                case GridView.GridOperations.Down:
                {
                    var newIndex = Mathf.Min(selectedIndex + columnCount, itemsSource.Count - 1);
                    if (newIndex != selectedIndex)
                    {
                        HandleSelectionAndScroll(newIndex);
                        return true;
                    }
                }
                    break;
                case GridView.GridOperations.Begin:
                    HandleSelectionAndScroll(0);
                    return true;
                case GridView.GridOperations.End:
                    HandleSelectionAndScroll(itemsSource.Count - 1);
                    return true;
                case GridView.GridOperations.Choose:
                    if (m_SelectedIndices.Count > 0)
                        itemsChosen?.Invoke(selectedItems);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }

            return false;
        }

        void Apply(GridView.GridOperations operation, EventBase sourceEvent)
        {
            if ((operation & operationMask) != 0 && Apply(operation, (sourceEvent as IKeyboardEvent)?.shiftKey ?? false))
                sourceEvent?.StopPropagation();
        }

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="operation"></param>
        internal void Apply(GridView.GridOperations operation) => Apply(operation, null);

        void OnDraggerStarted(PointerMoveEvent evt)
        {
            dragStarted?.Invoke(evt);
        }

        void OnDraggerMoved(PointerMoveEvent evt)
        {
            dragUpdated?.Invoke(evt);
        }

        void OnDraggerEnded(PointerUpEvent evt)
        {
            dragFinished?.Invoke(evt);
            CancelSoftSelect();
        }

        void OnDraggerCanceled()
        {
            dragCanceled?.Invoke();
            CancelSoftSelect();
        }

        /// <summary>
        /// Cancel drag operation.
        /// </summary>
        public void CancelDrag()
        {
            dragger?.Cancel();
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            var operation = evt.keyCode switch
            {
                KeyCode.A when evt.actionKey => GridView.GridOperations.SelectAll,
                KeyCode.Escape => GridView.GridOperations.Cancel,
                KeyCode.Home => GridView.GridOperations.Begin,
                KeyCode.End => GridView.GridOperations.End,
                KeyCode.UpArrow => GridView.GridOperations.Up,
                KeyCode.DownArrow => GridView.GridOperations.Down,
                KeyCode.LeftArrow => GridView.GridOperations.Left,
                KeyCode.RightArrow => GridView.GridOperations.Right,
                _ => GridView.GridOperations.None
            };

            Apply(operation, evt);
        }

        void OnKeyUp(KeyUpEvent evt)
        {
            var operation = evt.keyCode switch
            {
                KeyCode.KeypadEnter or KeyCode.Return => GridView.GridOperations.Choose,
                _ => GridView.GridOperations.None
            };

            Apply(operation, evt);
        }

        void OnNavigationMove(NavigationMoveEvent evt)
        {
            evt.StopPropagation();

        }

        void OnNavigationCancel(NavigationCancelEvent evt)
        {
            evt.StopPropagation();

        }

        /// <summary>
        /// Callback for binding a data item to the visual element.
        /// </summary>
        /// <remarks>
        /// The method called by this callback receives the VisualElement to bind, and the index of the
        /// element to bind it to.
        /// </remarks>
        public Action<VisualElement, int> bindItem
        {
            get { return m_BindItem; }
            set
            {
                m_BindItem = value;
                Refresh();
            }
        }

        /// <summary>
        /// The number of columns for this grid.
        /// </summary>
        public int columnCount
        {
            get => m_ColumnCount;

            set
            {
                if (m_ColumnCount != value && value > 0)
                {
                    m_ColumnCount = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// The <see cref="Dragger"/> manipulator used by this <see cref="GridView"/>.
        /// </summary>
        public Dragger dragger { get; }

        /// <summary>
        /// A mask describing available operations in this <see cref="GridView"/> when the user interacts with it.
        /// </summary>
        public GridView.GridOperations operationMask { get; set; } =
            GridView.GridOperations.Choose |
            GridView.GridOperations.SelectAll | GridView.GridOperations.Cancel |
            GridView.GridOperations.Begin | GridView.GridOperations.End |
            GridView.GridOperations.Left | GridView.GridOperations.Right |
            GridView.GridOperations.Up | GridView.GridOperations.Down;

        /// <summary>
        /// Returns the content container for the <see cref="GridView"/>. Because the GridView control automatically manages
        /// its content, this always returns null.
        /// </summary>
        public override VisualElement contentContainer => null;

        /// <summary>
        /// The width of the BaseGridView items.
        /// </summary>
        public float itemWidth => (scrollView.contentViewport.layout.width / columnCount);

        /// <summary>
        /// The data source for list items.
        /// </summary>
        /// <remarks>
        /// This list contains the items that the <see cref="GridView"/> displays.
        ///
        /// This property must be set for the list view to function.
        /// </remarks>
        public IList itemsSource
        {
            get { return m_ItemsSource; }
            set
            {
                if (m_ItemsSource is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= OnItemsSourceCollectionChanged;
                }

                m_ItemsSource = value;
                if (m_ItemsSource is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += OnItemsSourceCollectionChanged;
                }

                Refresh();
            }
        }

        /// <summary>
        /// Callback for constructing the VisualElement that is the template for each recycled and re-bound element in the list.
        /// </summary>
        /// <remarks>
        /// This callback needs to call a function that constructs a blank <see cref="VisualElement"/> that is
        /// bound to an element from the list.
        ///
        /// The GridView automatically creates enough elements to fill the visible area, and adds more if the area
        /// is expanded. As the user scrolls, the GridView cycles elements in and out as they appear or disappear.
        ///
        ///  This property must be set for the list view to function.
        /// </remarks>
        public Func<VisualElement> makeItem
        {
            get { return m_MakeItem; }
            set
            {
                if (m_MakeItem == value)
                    return;
                m_MakeItem = value;
                Refresh();
            }
        }

        /// <summary>
        /// The width of the BaseGridView items in pixels.
        /// </summary>
        public float resolvedItemWidth
        {
            get
            {
                var dpiScaling = 1f;
                return Mathf.Round(itemWidth * dpiScaling) / dpiScaling;
            }
        }

        /// <summary>
        /// Returns or sets the selected item's index in the data source. If multiple items are selected, returns the
        /// first selected item's index. If multiple items are provided, sets them all as selected.
        /// </summary>
        public int selectedIndex
        {
            get { return m_SelectedIndices.Count == 0 ? -1 : m_SelectedIndices.First(); }
            set { SetSelection(value); }
        }

        /// <summary>
        /// Returns the indices of selected items in the data source. Always returns an enumerable, even if no item  is selected, or a
        /// single item is selected.
        /// </summary>
        public IEnumerable<int> selectedIndices => m_SelectedIndices;

        /// <summary>
        /// Returns the selected item from the data source. If multiple items are selected, returns the first selected item.
        /// </summary>
        public object selectedItem => m_SelectedItems.Count == 0 ? null : m_SelectedItems.First();

        /// <summary>
        /// Returns the selected items from the data source. Always returns an enumerable, even if no item is selected, or a single
        /// item is selected.
        /// </summary>
        public IEnumerable<object> selectedItems => m_SelectedItems;

        /// <summary>
        /// Returns the IDs of selected items in the data source. Always returns an enumerable, even if no item  is selected, or a
        /// single item is selected.
        /// </summary>
        public IEnumerable<int> selectedIds => m_SelectedIds;

        /// <summary>
        /// The number of selected Items.
        /// </summary>
        public int selectionCount => m_SelectedIndices.Count;

        /// <summary>
        /// Check if the item with the given id is selected.
        /// </summary>
        /// <param name="id"> The id of the item to check. </param>
        /// <returns> True if the item is selected, false otherwise. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSelectedId(int id) => m_SelectedIds.Contains(id);

        /// <summary>
        /// Controls the selection type.
        /// </summary>
        /// <remarks>
        /// You can set the GridView to make one item selectable at a time, make multiple items selectable, or disable selections completely.
        ///
        /// When you set the GridView to disable selections, any current selection is cleared.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public SelectionType selectionType
        {
            get { return m_SelectionType; }
            set
            {
                // ReSharper disable once UnusedVariable
                var changed = m_SelectionType != value;
                m_SelectionType = value;

                if (m_SelectionType == SelectionType.None)
                {
                    ClearSelectionWithoutValidation();
                }
                else
                {
                    if (allowNoSelection)
                    {
                        ClearSelectionWithoutValidation();
                    }
                    else if (m_ItemsSource.Count > 0)
                    {
                        SetSelectionInternal(new[] { 0 }, false, false);
                    }
                    else
                    {
                        ClearSelectionWithoutValidation();
                    }
                }

                m_RangeSelectionOrigin = -1;
                PostSelection(updatePreviousSelection: true, sendNotification: true);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in selectionTypeProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the GridView allows to have no selection when the selection type is <see cref="SelectionType.Single"/> or <see cref="SelectionType.Multiple"/>.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool allowNoSelection
        {
            get => m_AllowNoSelection;

            set
            {
                // ReSharper disable once UnusedVariable
                var changed = m_AllowNoSelection != value;
                m_AllowNoSelection = value;
                if (HasValidDataAndBindings() && !m_AllowNoSelection && m_SelectedIndices.Count == 0 && m_ItemsSource.Count > 0)
                    SetSelectionInternal(new []{ 0 }, true, true);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in allowNoSelectionProperty);
#endif
            }
        }

        /// <summary>
        /// Returns true if the soft-selection is in progress.
        /// </summary>
        public bool isSelecting => m_SoftSelectIndex != -1;

        /// <summary>
        /// Prevents the grid view from scrolling when the user presses a modifier key at the same time as scrolling.
        /// </summary>
        public bool preventScrollWithModifiers { get; set; } = k_DefaultPreventScrollWithModifiers;

        /// <summary>
        /// Callback for unbinding a data item from the VisualElement.
        /// </summary>
        /// <remarks>
        /// The method called by this callback receives the VisualElement to unbind, and the index of the
        /// element to unbind it from.
        /// </remarks>
        public Action<VisualElement, int> unbindItem
        {
            get;
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            set;
        }

        /// <summary>
        /// Callback for getting the ID of an item.
        /// </summary>
        /// <remarks>
        /// The method called by this callback receives the index of the item to get the ID from.
        /// </remarks>
        public Func<int, int> getItemId
        {
            get { return m_GetItemId; }
            set
            {
                m_GetItemId = value;
                Refresh();
            }
        }

        bool DefaultAcceptStartDrag(Vector2 worldPosition)
        {
            if (!HasValidDataAndBindings())
                return false;

            var idx = GetIndexByWorldPosition(worldPosition);
            return idx >= 0 && idx < itemsSource.Count;
        }

        /// <inheritdoc cref="ISerializationCallbackReceiver.OnAfterDeserialize"/>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Refresh();
        }

        /// <inheritdoc cref="ISerializationCallbackReceiver.OnBeforeSerialize"/>
        void ISerializationCallbackReceiver.OnBeforeSerialize() {}

        /// <summary>
        /// Callback triggered when the user acts on a selection of one or more items, for example by double-clicking or pressing Enter.
        /// </summary>
        /// <remarks>
        /// This callback receives an enumerable that contains the item or items chosen.
        /// </remarks>
        public event Action<IEnumerable<object>> itemsChosen;

        /// <summary>
        /// Callback triggered when the selection changes.
        /// </summary>
        /// <remarks>
        /// This callback receives an enumerable that contains the item or items selected.
        /// </remarks>
        public event Action<IEnumerable<object>> selectionChanged;

        /// <summary>
        /// Callback triggered when the selection changes.
        /// </summary>
        /// <remarks>
        /// This callback receives an enumerable that contains the indices of selected items.
        /// </remarks>
        public event Action<IEnumerable<int>> selectedIndicesChanged;

        /// <summary>
        /// Callback triggered when the user right-clicks on an item.
        /// </summary>
        /// <remarks>
        /// This callback receives an enumerable that contains the item or items selected.
        /// </remarks>
        public event Action<PointerDownEvent> contextClicked;

        /// <summary>
        /// Callback triggered when the user double-clicks on an item.
        /// </summary>
        public event Action<int> doubleClicked;

        /// <summary>
        /// Callback triggered when drag has started.
        /// </summary>
        public event Action<PointerMoveEvent> dragStarted;

        /// <summary>
        /// Callback triggered when items are dragged.
        /// </summary>
        public event Action<PointerMoveEvent> dragUpdated;

        /// <summary>
        /// Callback triggered when drag has finished.
        /// </summary>
        public event Action<PointerUpEvent> dragFinished;

        /// <summary>
        /// Callback triggered when drag has been canceled.
        /// </summary>
        public event Action dragCanceled;

        /// <summary>
        /// Adds an item to the collection of selected items.
        /// </summary>
        /// <param name="index">Item index.</param>
        public void AddToSelection(int index)
        {
            AddToSelection(new[] { index }, true, true);
        }

        internal void AddToSelection(int index, bool updatePrevious, bool notify)
        {
            AddToSelection(new[] { index }, updatePrevious, notify);
        }

        /// <summary>
        /// Deselects any selected items.
        /// </summary>
        public void ClearSelection()
        {
            ClearSelectionWithoutNotify();
            PostSelection(true, true);
        }

        /// <summary>
        /// Clear the selection without triggering selection changed event.
        /// </summary>
        public void ClearSelectionWithoutNotify()
        {
            if (!HasValidDataAndBindings() || m_SelectedIndices.Count == 0 || !allowNoSelection)
                return;

            ClearSelectionWithoutValidation();
            m_RangeSelectionOrigin = -1;
            m_PreviouslySelectedIndices.Clear();
        }

        /// <summary>
        /// Clears the GridView, recreates all visible visual elements, and rebinds all items.
        /// </summary>
        /// <remarks>
        /// Call this method whenever the data source changes.
        /// </remarks>
        public virtual void Refresh()
        {
            scrollView.Clear();
            m_SelectedIndices.Clear();
            m_SelectedItems.Clear();
            m_SoftSelectIndex = -1;
            m_SoftSelectIndexWasPreviouslySelected = false;
            m_PreviouslySelectedIndices.Clear();
            m_OriginalSelection.Clear();

            var newSelectedIds = new List<int>();

            if (m_SelectedIds.Count > 0 && itemsSource != null)
            {
                // Add selected objects to working lists.
                for (var index = 0; index < itemsSource.Count; ++index)
                {
                    var id = GetIdFromIndex(index);
                    if (!m_SelectedIds.Contains(id))
                        continue;

                    m_SelectedIndices.Add(index);
                    m_SelectedItems.Add(itemsSource[index]);
                    newSelectedIds.Add(id);
                }
            }

            m_SelectedIds.Clear();
            m_SelectedIds.AddRange(newSelectedIds);
        }

        /// <summary>
        /// Removes an item from the collection of selected items.
        /// </summary>
        /// <param name="index">The item index.</param>
        public void RemoveFromSelection(int index)
        {
            RemoveFromSelectionInternal(index, true, true);
        }

        internal void RemoveFromSelectionInternal(int index, bool updatePrevious, bool notify)
        {
            if (!HasValidDataAndBindings())
                return;

            if (m_SelectedIndices.Count == 1 && m_SelectedIndices[0] == index && !allowNoSelection)
                return;

            RemoveFromSelectionWithoutValidation(index);

            PostSelection(updatePrevious, notify);
        }

        /// <summary>
        /// Scrolls to a specific item index and makes it visible.
        /// </summary>
        /// <param name="index">Item index to scroll to. Specify -1 to make the last item visible.</param>
        public abstract void ScrollToItem(int index);

        /// <summary>
        /// Sets the currently selected item.
        /// </summary>
        /// <param name="index">The item index.</param>
        public void SetSelection(int index)
        {
            if (index < 0 || itemsSource == null || index >= itemsSource.Count)
            {
                ClearSelection();
                return;
            }

            SetSelection(new[] { index });
        }

        /// <summary>
        /// Sets a collection of selected items.
        /// </summary>
        /// <param name="indices">The collection of the indices of the items to be selected.</param>
        public void SetSelection(IEnumerable<int> indices)
        {
            switch (selectionType)
            {
                case SelectionType.None:
                    return;
                case SelectionType.Single:
                    if (indices != null)
                        indices = new[] { indices.Last() };
                    break;
                case SelectionType.Multiple:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetSelectionInternal(indices, true, true);
        }

        /// <summary>
        /// Sets a collection of selected items without triggering a selection change callback.
        /// </summary>
        /// <param name="indices">The collection of items to be selected.</param>
        public void SetSelectionWithoutNotify(IEnumerable<int> indices)
        {
            SetSelectionInternal(indices, true, false);
        }

        internal void AddToSelection(IList<int> indexes, bool updatePrevious, bool notify)
        {
            if (!HasValidDataAndBindings() || indexes == null || indexes.Count == 0)
                return;

            foreach (var index in indexes)
            {
                AddToSelectionWithoutValidation(index);
            }

            PostSelection(updatePrevious, notify);

            //SaveViewData();
        }

        /// <summary>
        /// Set the selected visual state on items.
        /// </summary>
        protected abstract void ApplySelectedState();

        internal void SelectAll()
        {
            if (!HasValidDataAndBindings())
                return;

            if (selectionType != SelectionType.Multiple)
            {
                return;
            }

            for (var index = 0; index < itemsSource.Count; index++)
            {
                var id = GetIdFromIndex(index);
                var item = itemsSource[index];

                if (!m_SelectedIds.Contains(id))
                {
                    m_SelectedIds.Add(id);
                    m_SelectedIndices.Add(index);
                    m_SelectedItems.Add(item);
                }
            }

            ApplySelectedState();

            PostSelection(true, true);

            //SaveViewData();
        }

        internal void SetSelectionInternal(IEnumerable<int> indices, bool updatePrevious, bool sendNotification)
        {
            if (!HasValidDataAndBindings() || indices == null)
                return;

            var indicesList = new List<int>(indices);

            if (!allowNoSelection && indicesList.Count == 0)
                return;

            ClearSelectionWithoutValidation();
            foreach (var index in indicesList)
            {
                AddToSelectionWithoutValidation(index);
            }

            PostSelection(updatePrevious, sendNotification);

            //SaveViewData();
        }

        void AddToSelectionWithoutValidation(int index)
        {
            if (index < 0 || index >= m_ItemsSource.Count || m_SelectedIndices.Contains(index))
                return;

            var id = GetIdFromIndex(index);
            var item = m_ItemsSource[index];

            m_SelectedIds.Add(id);
            m_SelectedIndices.Add(index);
            m_SelectedItems.Add(item);

            ApplySelectedState();
        }

        internal VisualElement GetVisualElement(int index)
        {
            if (index < 0 || index >= m_ItemsSource.Count || !m_SelectedIndices.Contains(index))
                return null;

            return GetVisualElementInternal(index);
        }

        internal VisualElement GetVisualElementWithoutSelection(int index)
        {
            if (index < 0 || index >= m_ItemsSource.Count)
                return null;

            return GetVisualElementInternal(index);
        }

        /// <summary>
        /// Implement this method to return the VisualElement at the specified index.
        /// </summary>
        /// <param name="index"> The index of the VisualElement to return. </param>
        /// <returns> The VisualElement at the specified index. </returns>
        protected abstract VisualElement GetVisualElementInternal(int index);

        void ClearSelectionWithoutValidation()
        {
            m_SelectedIds.Clear();
            m_SelectedIndices.Clear();
            m_SelectedItems.Clear();

            ApplySelectedState();
        }

        void DoRangeSelection(int rangeSelectionFinalIndex, bool updatePrevious, bool notify)
        {
            m_RangeSelectionOrigin = m_IsRangeSelectionDirectionUp ? m_SelectedIndices.Max() : m_SelectedIndices.Min();

            ClearSelectionWithoutValidation();

            // Add range
            var range = new List<int>();
            m_IsRangeSelectionDirectionUp = rangeSelectionFinalIndex < m_RangeSelectionOrigin;
            if (m_IsRangeSelectionDirectionUp)
            {
                for (var i = rangeSelectionFinalIndex; i <= m_RangeSelectionOrigin; i++)
                {
                    range.Add(i);
                }
            }
            else
            {
                for (var i = rangeSelectionFinalIndex; i >= m_RangeSelectionOrigin; i--)
                {
                    range.Add(i);
                }
            }

            AddToSelection(range, updatePrevious, notify);
        }

        void DoContextClickAfterSelect(PointerDownEvent evt)
        {
            contextClicked?.Invoke(evt);
        }

        void DoSoftSelect(PointerDownEvent evt, int clickCount)
        {
            var clickedIndex = GetIndexByWorldPosition(evt.position);
            if (clickedIndex > m_ItemsSource.Count - 1 || clickedIndex < 0)
            {
                if (evt.button == (int)MouseButton.LeftMouse && allowNoSelection)
                    ClearSelection();
                return;
            }

            m_SoftSelectIndex = clickedIndex;
            m_SoftSelectIndexWasPreviouslySelected = m_SelectedIndices.Contains(clickedIndex);

            if (clickCount == 1)
            {
                if (selectionType == SelectionType.None)
                    return;

                if (selectionType == SelectionType.Multiple && evt.actionKey)
                {
                    m_RangeSelectionOrigin = clickedIndex;

                    // Add/remove single clicked element
                    var clickedItemId = GetIdFromIndex(clickedIndex);
                    if (m_SelectedIds.Contains(clickedItemId))
                        RemoveFromSelectionInternal(clickedIndex, false, false);
                    else
                        AddToSelection(clickedIndex, false, false);
                }
                else if (selectionType == SelectionType.Multiple && evt.shiftKey)
                {
                    if (m_RangeSelectionOrigin == -1 || m_SelectedIndices.Count == 0)
                    {
                        m_RangeSelectionOrigin = clickedIndex;
                        SetSelectionInternal(new[] { clickedIndex }, false, false);
                    }
                    else
                    {
                        DoRangeSelection(clickedIndex, false, false);
                    }
                }
                else if (selectionType == SelectionType.Multiple && m_SoftSelectIndexWasPreviouslySelected)
                {
                    // Do noting, selection will be processed OnPointerUp.
                    // If drag and drop will be started GridViewDragger will capture the mouse and GridView will not receive the mouse up event.
                }
                else // single
                {
                    m_RangeSelectionOrigin = clickedIndex;
                    if (!(m_SelectedIndices.Count == 1 && m_SelectedIndices[0] == clickedIndex))
                    {
                        SetSelectionInternal(new[] { clickedIndex }, false, false);
                    }
                }
            }

            ScrollToItem(clickedIndex);
        }

        /// <summary>
        /// Returns the index of the item at the given position.
        /// </summary>
        /// <param name="index"> The index of the item to get the ID from. </param>
        /// <returns> The ID of the item at the given index. </returns>
        protected int GetIdFromIndex(int index)
        {
            if (m_GetItemId == null)
                return index;
            return m_GetItemId(index);
        }

        /// <summary>
        /// Whether the GridView has valid data and bindings.
        /// </summary>
        /// <returns> True if the GridView has valid data and bindings, false otherwise. </returns>
        protected bool HasValidDataAndBindings()
        {
            return itemsSource != null && makeItem != null && bindItem != null;
        }

        /// <summary>
        /// Method to call after the selection has been changed to update the selection state and send the selection changed event.
        /// </summary>
        /// <param name="updatePreviousSelection"> Whether to update the previous selection. </param>
        /// <param name="sendNotification"> Whether to send the selection changed event. </param>
        protected void PostSelection(bool updatePreviousSelection, bool sendNotification)
        {
            if (!HasValidDataAndBindings())
                return;

            if (m_PreviouslySelectedIndices.SequenceEqual(m_SelectedIndices))
                return;

            if (updatePreviousSelection)
            {
                m_PreviouslySelectedIndices.Clear();
                m_PreviouslySelectedIndices.AddRange(m_SelectedIndices);
            }

            if (sendNotification)
            {
                selectionChanged?.Invoke(m_SelectedItems);
                selectedIndicesChanged?.Invoke(m_SelectedIndices);
            }
        }

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            if (evt.destinationPanel == null)
                return;

            if (!UnityEngine.Device.Application.isMobilePlatform)
                scrollView.AddManipulator(dragger);

            scrollView.RegisterCallback<ClickEvent>(OnClick);
            scrollView.RegisterCallback<PointerDownEvent>(OnPointerDown);
            scrollView.RegisterCallback<PointerUpEvent>(OnPointerUp);
            scrollView.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
            RegisterCallback<KeyUpEvent>(OnKeyUp);
            RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
            RegisterCallback<NavigationCancelEvent>(OnNavigationCancel);
            scrollView.RegisterCallback<WheelEvent>(OnWheel, TrickleDown.TrickleDown);
        }

        /// <summary>
        /// Method to implement to handle the custom style resolved event.
        /// </summary>
        /// <param name="e"> The custom style resolved event. </param>
        protected abstract void OnCustomStyleResolved(CustomStyleResolvedEvent e);

        void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            if (evt.originPanel == null)
                return;

            scrollView.RemoveManipulator(dragger);

            scrollView.UnregisterCallback<ClickEvent>(OnClick);
            scrollView.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            scrollView.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            scrollView.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            UnregisterCallback<KeyDownEvent>(OnKeyDown);
            UnregisterCallback<KeyUpEvent>(OnKeyUp);
            UnregisterCallback<NavigationMoveEvent>(OnNavigationMove);
            UnregisterCallback<NavigationCancelEvent>(OnNavigationCancel);
            scrollView.UnregisterCallback<WheelEvent>(OnWheel, TrickleDown.TrickleDown);
        }

        void OnWheel(WheelEvent evt)
        {
            if (preventScrollWithModifiers && evt.modifiers != EventModifiers.None)
                evt.StopImmediatePropagation();
        }

        void OnClick(ClickEvent evt)
        {
            if (!HasValidDataAndBindings())
                return;

            if (evt.clickCount == 2)
            {
                var clickedIndex = GetIndexByWorldPosition(evt.position);
                if (clickedIndex >= 0 && clickedIndex < m_ItemsSource.Count)
                {
                    doubleClicked?.Invoke(clickedIndex);
                    Apply(GridView.GridOperations.Choose, evt.shiftKey);
                }
            }
        }

        void OnPointerMove(PointerMoveEvent evt)
        {
            m_HasPointerMoved = true;
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            evt.StopImmediatePropagation();
            if (!HasValidDataAndBindings())
                return;

            if (!evt.isPrimary)
                return;

            var capturingElement = panel?.GetCapturingElement(evt.pointerId);

            // if the pointer is captured by a child of the scroll view, abort any selection
            if (capturingElement is VisualElement ve &&
                ve != scrollView &&
                ve.FindCommonAncestor(scrollView) != null)
                return;

            m_OriginalSelection.Clear();
            m_OriginalSelection.AddRange(m_SelectedIndices);
            m_OriginalScrollOffset = scrollView.verticalScroller.value;
            m_SoftSelectIndex = -1;

            var clickCount = m_HasPointerMoved ? 1 : evt.clickCount;
            m_HasPointerMoved = false;

            DoSoftSelect(evt, clickCount);

            if (evt.IsContextClick())
                DoContextClickAfterSelect(evt);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (!HasValidDataAndBindings())
                return;

            if (!evt.isPrimary)
                return;

            // if (shouldCancelSoftSelection)
            // {
            //     CancelSoftSelect();
            //     return;
            // }

            if (m_SoftSelectIndex == -1)
                return;

            var index = m_SoftSelectIndex;
            m_SoftSelectIndex = -1;

            if (m_SoftSelectIndexWasPreviouslySelected &&
                evt.button == (int)MouseButton.LeftMouse &&
                evt.modifiers == EventModifiers.None)
            {
                ProcessSingleClick(index);
                return;
            }

            PostSelection(true, true);
        }

        void CancelSoftSelect()
        {
            if (m_SoftSelectIndex != -1)
            {
                SetSelectionInternal(m_OriginalSelection, false, false);
                scrollView.verticalScroller.value = m_OriginalScrollOffset;
            }

            m_SoftSelectIndex = -1;
        }

        /// <summary>
        /// Returns the index of the item at the given position.
        /// </summary>
        /// <remarks>
        /// The position is relative to the top left corner of the grid. No check is made to see if the index is valid.
        /// </remarks>
        /// <param name="worldPosition">The position of the item in the world-space.</param>
        /// <returns> The index of the item at the given position.</returns>
        public abstract int GetIndexByWorldPosition(Vector2 worldPosition);

        void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Refresh();
        }

        /// <summary>
        /// Callback called when the scroll value changes.
        /// </summary>
        /// <remarks>
        /// You can also call this method one frame your <see cref="Refresh"/> implementation to ensure
        /// the visual state of the grid is updated correctly.
        /// </remarks>
        /// <param name="offset"> The new scroll offset. </param>
        protected abstract void OnScroll(float offset);

        void OnSizeChanged(GeometryChangedEvent evt)
        {
            if (!HasValidDataAndBindings())
                return;

            if (Mathf.Approximately(evt.newRect.height, evt.oldRect.height))
                return;

            OnContainerHeightChanged(evt.newRect.height);
        }

        void ProcessSingleClick(int clickedIndex)
        {
            m_RangeSelectionOrigin = clickedIndex;
            SetSelection(clickedIndex);
        }

        void RemoveFromSelectionWithoutValidation(int index)
        {
            if (!m_SelectedIndices.Contains(index))
                return;

            var id = GetIdFromIndex(index);
            var item = m_ItemsSource[index];

            m_SelectedIds.Remove(id);
            m_SelectedIndices.Remove(index);
            m_SelectedItems.Remove(item);

            ApplySelectedState();
        }

        /// <summary>
        /// Callback called when the ScrollView container height changes.
        /// </summary>
        /// <param name="height"> The new height of the container. </param>
        protected abstract void OnContainerHeightChanged(float height);

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Defines <see cref="UxmlTraits"/> for the <see cref="BaseGridView"/>.
        /// </summary>
        /// <remarks>
        /// This class defines the BaseGridView element properties that you can use in a UI document asset (UXML file).
        /// </remarks>
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<SelectionType> m_SelectionType = new UxmlEnumAttributeDescription<SelectionType>
            {
                name = "selection-type",
                defaultValue = SelectionType.Single
            };

            readonly UxmlBoolAttributeDescription m_PreventScrollWithModifiers = new UxmlBoolAttributeDescription
            {
                name = "prevent-scroll-with-modifiers",
                defaultValue = k_DefaultPreventScrollWithModifiers
            };

            readonly UxmlBoolAttributeDescription m_AllowNoSelection = new UxmlBoolAttributeDescription
            {
                name = "allow-no-selection",
                defaultValue = true
            };

            /// <summary>
            /// Returns an empty enumerable, because list views usually do not have child elements.
            /// </summary>
            /// <returns>An empty enumerable.</returns>
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            /// <summary>
            /// Initializes <see cref="GridView"/> properties using values from the attribute bag.
            /// </summary>
            /// <param name="ve">The object to initialize.</param>
            /// <param name="bag">The attribute bag.</param>
            /// <param name="cc">The creation context; unused.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var view = (BaseGridView)ve;

                view.preventScrollWithModifiers = m_PreventScrollWithModifiers.GetValueFromBag(bag, cc);
                view.selectionType = m_SelectionType.GetValueFromBag(bag, cc);
                view.allowNoSelection = m_AllowNoSelection.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
