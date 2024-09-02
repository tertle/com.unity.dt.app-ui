using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// ActionGroup UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class ActionGroup : BaseVisualElement
    {

#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId quietProperty = nameof(quiet);

        internal static readonly BindingId compactProperty = nameof(compact);

        internal static readonly BindingId directionProperty = nameof(direction);

        internal static readonly BindingId justifiedProperty = nameof(justified);

        internal static readonly BindingId selectionTypeProperty = nameof(selectionType);

        internal static readonly BindingId closeOnSelectionProperty = nameof(closeOnSelection);

        internal static readonly BindingId allowNoSelectionProperty = nameof(allowNoSelection);
#endif

        /// <summary>
        /// The ActionGroup main styling class.
        /// </summary>
        public const string ussClassName = "appui-actiongroup";

        /// <summary>
        /// The ActionGroup quiet mode styling class.
        /// </summary>
        public const string quietUssClassName = ussClassName + "--quiet";

        /// <summary>
        /// The ActionGroup compact mode styling class.
        /// </summary>
        public const string compactUssClassName = ussClassName + "--compact";

        /// <summary>
        /// The ActionGroup vertical mode styling class.
        /// </summary>
        [EnumName("GetDirectionUssClassName", typeof(Direction))]
        public const string verticalUssClassName = ussClassName + "--";

        /// <summary>
        /// The ActionGroup justified mode styling class.
        /// </summary>
        public const string justifiedUssClassName = ussClassName + "--justified";

        /// <summary>
        /// The ActionGroup selectable mode styling class.
        /// </summary>
        public const string selectableUssClassName = ussClassName + "--selectable";

        /// <summary>
        /// The ActionGroup container styling class.
        /// </summary>
        public const string containerUssClassName = ussClassName + "__container";

        /// <summary>
        /// The ActionGroup More Button styling class.
        /// </summary>
        public const string moreButtonUssClassName = ussClassName + "__more-button";

        /// <summary>
        /// Event sent when the selection changes.
        /// </summary>
        public event Action<IEnumerable<int>> selectionChanged;

        SelectionType m_SelectionType = k_DefaultSelectionType;

        readonly List<VisualElement> m_HandledChildren = new List<VisualElement>();

        readonly List<int> m_SelectedIndices = new List<int>();

        readonly List<int> m_SelectedIds = new List<int>();

        readonly VisualElement m_Container;

        readonly ActionButton m_MoreButton;

        int m_FirstIndexOutOfBound = -1;

        MenuBuilder m_MenuBuilder;

        Rect m_LastContainerLayout;

        Rect m_LastLayout;

        int m_LastChildCount = 0;

        Dir m_CurrentLayoutDirection;

        Func<int, int> m_GetItemId;

        bool m_AllowNoSelection = true;

        bool m_CloseOnSelection;

        Direction m_Direction;

        const SelectionType k_DefaultSelectionType = SelectionType.None;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ActionGroup()
        {
            AddToClassList(ussClassName);

            focusable = false;
            pickingMode = PickingMode.Ignore;

            m_Container = new VisualElement { name = containerUssClassName, pickingMode = PickingMode.Ignore };
            m_Container.AddToClassList(containerUssClassName);
            hierarchy.Add(m_Container);

            m_MoreButton = new ActionButton
            {
                name = moreButtonUssClassName,
                icon = "dots-three",
                iconVariant = IconVariant.Bold,
                usageHints = UsageHints.DynamicTransform
            };
            m_MoreButton.AddToClassList(ussClassName + "__item");
            m_MoreButton.AddToClassList("unity-last-child");
            m_MoreButton.AddToClassList(moreButtonUssClassName);
            m_MoreButton.clicked += OnMoreButtonClicked;
            hierarchy.Add(m_MoreButton);
            direction = Direction.Horizontal;
            closeOnSelection = true;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            m_MoreButton.RegisterCallback<GeometryChangedEvent>(OnMoreButtonGeometryChanged);
            RegisterCallback<ActionTriggeredEvent>(OnActionTriggered);
            this.RegisterContextChangedCallback<DirContext>(OnDirectionChanged);
        }

        void OnDirectionChanged(ContextChangedEvent<DirContext> evt)
        {
            m_CurrentLayoutDirection = evt.context?.dir ?? Dir.Ltr;
            schedule.Execute(RefreshUI);
        }

        /// <summary>
        /// The content container of the ActionGroup.
        /// </summary>
        public override VisualElement contentContainer => m_Container;

        /// <summary>
        /// The quiet state of the ActionGroup.
        /// </summary>
        [Tooltip("The quiet state of the ActionGroup. A quiet ActionGroup has no background and no border.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Action Group")]
#endif
        public bool quiet
        {
            get => ClassListContains(quietUssClassName);
            set
            {
                var changed = quiet != value;
                EnableInClassList(quietUssClassName, value);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in quietProperty);
#endif
            }
        }

        /// <summary>
        /// The compact state of the ActionGroup.
        /// </summary>
        [Tooltip("The compact state of the ActionGroup. A compact ActionGroup doesn't have any gap between its items.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool compact
        {
            get => ClassListContains(compactUssClassName);
            set
            {
                var changed = compact != value;
                EnableInClassList(compactUssClassName, value);
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in compactProperty);
#endif
            }
        }

        /// <summary>
        /// The orientation of the ActionGroup.
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
                AddToClassList(GetDirectionUssClassName(m_Direction));
                m_MoreButton.icon = m_Direction switch
                {
                    Direction.Horizontal => "dots-three",
                    Direction.Vertical => "dots-three-vertical",
                    _ => throw new ArgumentOutOfRangeException()
                };
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in directionProperty);
#endif
            }
        }

        /// <summary>
        /// The justified state of the ActionGroup.
        /// </summary>
        [Tooltip("The justified state of the ActionGroup. A justified ActionGroup has its items stretched to fill the available space.")]
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
        /// The selection type of the ActionGroup.
        /// </summary>
        [Tooltip("The selection type of the ActionGroup. " +
            "A selection type of None means that no item can be selected. " +
            "A selection type of Single means that only one item can be selected at a time. " +
            "A selection type of Multiple means that multiple items can be selected at a time.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public SelectionType selectionType
        {
            get => m_SelectionType;
            set
            {
                var changed = m_SelectionType != value;
                m_SelectionType = value;
                EnableInClassList(selectableUssClassName, m_SelectionType != SelectionType.None);
                if (m_SelectionType == SelectionType.None)
                {
                    ClearSelection();
                }
                else if (m_SelectionType == SelectionType.Single && m_SelectedIndices.Count != 1)
                {
                    if (allowNoSelection)
                        ClearSelection();
                    else
                        SetSelection(new[] { m_SelectedIndices.Last() });
                }
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in selectionTypeProperty);
#endif
            }
        }

        /// <summary>
        /// Whether the ActionGroup's menu popover should close when a selection is made.
        /// </summary>
        [Tooltip("Whether the ActionGroup's menu popover should close when a selection is made.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool closeOnSelection
        {
            get => m_CloseOnSelection;
            set
            {
                var changed = m_CloseOnSelection != value;
                m_CloseOnSelection = value;
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in closeOnSelectionProperty);
#endif
            }
        }

        /// <summary>
        /// Callback used to get the ID of an item.
        /// </summary>
        public Func<int, int> getItemId
        {
            get => m_GetItemId;
            set
            {
                m_GetItemId = value;
                RefreshUI();
            }
        }

        /// <summary>
        /// The selected items.
        /// </summary>
        public IEnumerable<int> selectedIndices => m_SelectedIndices;

        /// <summary>
        /// The selected items.
        /// </summary>
        public IEnumerable<int> selectedIds => m_SelectedIds;

        /// <summary>
        /// Whether the ActionGroup allows no selection when in single or multi selection mode.
        /// </summary>
        [Tooltip("Whether the ActionGroup allows no selection when in single or multi selection mode.")]
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
                var changed = m_AllowNoSelection != value;
                m_AllowNoSelection = value;
                if (!m_AllowNoSelection && m_SelectedIndices.Count == 0)
                    SetSelection(new[] { 0 });
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in allowNoSelectionProperty);
#endif
            }
        }

        /// <summary>
        /// Deselects any selected items.
        /// </summary>
        public void ClearSelection()
        {
            var notify = m_SelectedIds.Count > 0;
            ClearSelectionWithoutNotify();
            if (notify)
                NotifyOfSelectionChange();
        }

        /// <summary>
        /// Deselects any selected items without sending an event through the visual tree.
        /// </summary>
        public void ClearSelectionWithoutNotify()
        {
            m_SelectedIndices.Clear();
            m_SelectedIds.Clear();
            RefreshSelectionUI(closeOnSelection);
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
                    throw new InvalidOperationException("Invalid selection type");
            }

            SetSelectionInternal(indices, true);
        }

        /// <summary>
        /// Sets a collection of selected items without triggering a selection change callback.
        /// </summary>
        /// <param name="indices">The collection of items to be selected.</param>
        public void SetSelectionWithoutNotify(IEnumerable<int> indices)
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
                    throw new InvalidOperationException("Invalid selection type");
            }

            SetSelectionInternal(indices, false);
        }

        void SetSelectionInternal(IEnumerable<int> indices, bool sendEvent)
        {
            indices ??= new int[] { };

            var newIndices = indices.ToList();
            newIndices.Sort();
            var newIds = newIndices.Select(GetIdFromIndex).ToList();
            newIds.Sort();
            var hasChanged = !EnumerableExtensions.SequenceEqual(newIds, m_SelectedIds);

            m_SelectedIndices.Clear();
            m_SelectedIds.Clear();
            m_SelectedIds.AddRange(newIds);
            m_SelectedIndices.AddRange(newIndices);
            RefreshSelectionUI(closeOnSelection);
            if (sendEvent && hasChanged)
                NotifyOfSelectionChange();
        }

        void NotifyOfSelectionChange()
        {
            selectionChanged?.Invoke(m_SelectedIndices);
        }

        void OnActionTriggered(ActionTriggeredEvent evt)
        {
            evt.StopPropagation();

            if (selectionType != SelectionType.Single && selectionType != SelectionType.Multiple)
                return;

            if (evt.target is VisualElement el && el != m_MoreButton)
            {
                var currentSelection = new List<int>(m_SelectedIndices);
                var index = el.parent.IndexOf(el);
                switch (selectionType)
                {
                    case SelectionType.Single:
                    {
                        if (!currentSelection.Contains(index))
                            SetSelection(new[] { index });
                        else if (allowNoSelection)
                            SetSelection(null);
                        break;
                    }
                    case SelectionType.Multiple:
                    {
                        if (!currentSelection.Contains(index))
                        {
                            currentSelection.Add(index);
                            SetSelection(currentSelection);
                        }
                        else
                        {
                            currentSelection.Remove(index);
                            if (currentSelection.Count > 0 || allowNoSelection)
                                SetSelection(currentSelection);
                        }
                        break;
                    }
                    case SelectionType.None:
                    default:
                        break;
                }
            }
        }

        int GetIdFromIndex(int index)
        {
            return m_GetItemId?.Invoke(index) ?? index;
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            m_HandledChildren.Clear();
            m_HandledChildren.AddRange(Children());

            RefreshUI();
        }

        void OnMoreButtonGeometryChanged(GeometryChangedEvent evt)
        {
            RefreshUI();
        }

        void RefreshSelectionUI(bool dismissPopover = true)
        {
            if (dismissPopover)
            {
                m_MenuBuilder?.Dismiss(DismissType.Action);
                m_MenuBuilder = null;
            }

            for (var i = 0; i < m_HandledChildren.Count; i++)
            {
                var child = m_HandledChildren[i];
                if (child is ISelectableElement selectableElement)
                    selectableElement.SetSelectedWithoutNotify(m_SelectedIndices.Contains(i));
            }
        }

        void RefreshUI()
        {
            var actionGroupLayout = layout;
            var containerLayout = m_Container.layout;
            var groupChildCount = m_HandledChildren.Count;

            if (!actionGroupLayout.IsValid() || (
                    containerLayout == m_LastContainerLayout
                    && actionGroupLayout == m_LastLayout
                    && groupChildCount == m_LastChildCount))
                return;

            m_LastChildCount = groupChildCount;
            m_LastContainerLayout = containerLayout;
            m_LastLayout = actionGroupLayout;
            var moreButtonStyle = m_MoreButton.resolvedStyle;
            var moreButtonLayout = m_MoreButton.layout;

            float size;
            float containerSize;
            float moreButtonSize;
            Func<VisualElement, float> getChildSize;

            switch (m_Direction)
            {
                case Direction.Horizontal:
                    size = actionGroupLayout.width;
                    containerSize = containerLayout.width;
                    moreButtonSize = moreButtonStyle.width + (m_CurrentLayoutDirection == Dir.Ltr
                        ? moreButtonStyle.marginLeft
                        : moreButtonStyle.marginRight);
                    getChildSize = GetElementFullWidth;
                    break;
                case Direction.Vertical:
                    size = actionGroupLayout.height;
                    containerSize = containerLayout.height;
                    moreButtonSize = moreButtonStyle.height + moreButtonStyle.marginTop;
                    getChildSize = GetElementFullHeight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var outOfBounds = size < containerSize;
            var spaceUsed = outOfBounds ? moreButtonSize : 0;
            m_FirstIndexOutOfBound = -1;
            for (var i = 0; i < m_HandledChildren.Count; i++)
            {
                var child = m_HandledChildren[i];
                if (child == m_MoreButton)
                    continue;

                child.EnableInClassList("unity-first-child", i == 0);
                child.EnableInClassList(ussClassName + "__inbetween-item", i != 0 && i != m_HandledChildren.Count - 1);
                child.EnableInClassList("unity-last-child", i == m_HandledChildren.Count - 1);
                child.AddToClassList(ussClassName + "__item");

                if (outOfBounds)
                {
                    var childSize = getChildSize.Invoke(child);
                    var newSpaceUsed = spaceUsed + childSize;
                    if (spaceUsed <= size && newSpaceUsed > size)
                    {
                        // first item that doesn't fit
                        m_FirstIndexOutOfBound = i;
                    }
                    spaceUsed += childSize;
                    child.visible = spaceUsed <= size;
                }
                else
                {
                    child.visible = true;
                }
            }

            m_MoreButton.visible = m_FirstIndexOutOfBound >= 0;
            if (m_FirstIndexOutOfBound >= 0) // overflow detected
            {
                var firstChildOutOfBoundLayout = m_HandledChildren[m_FirstIndexOutOfBound].layout;
                var left = m_Direction switch
                {
                    Direction.Horizontal when m_CurrentLayoutDirection == Dir.Ltr => firstChildOutOfBoundLayout.x,
                    Direction.Horizontal => firstChildOutOfBoundLayout.xMax + (size - containerSize) - moreButtonSize,
                    Direction.Vertical => containerLayout.x,
                    _ => 0
                };
                var top = m_Direction switch
                {
                    Direction.Horizontal => containerLayout.y,
                    Direction.Vertical => firstChildOutOfBoundLayout.y,
                    _ => 0
                };
                if (!Mathf.Approximately(moreButtonLayout.x, left))
                    m_MoreButton.style.left = left;
                if (!Mathf.Approximately(moreButtonLayout.y, top))
                    m_MoreButton.style.top = top;
                m_MoreButton.EnableInClassList("unity-first-child", m_FirstIndexOutOfBound == 0);
            }

            RefreshSelectionUI();
        }

        static float GetElementFullWidth(VisualElement ve)
        {
            var style = ve.resolvedStyle;
            return style.width + style.marginLeft + style.marginRight;
        }

        static float GetElementFullHeight(VisualElement ve)
        {
            var style = ve.resolvedStyle;
            return style.height + style.marginTop + style.marginBottom;
        }

        void OnMoreButtonClicked()
        {
            if (m_FirstIndexOutOfBound < 0)
                return;

            var dir = this.GetContext<DirContext>()?.dir ?? Dir.Ltr;
            var horizontalPlacement = dir == Dir.Ltr ? PopoverPlacement.BottomStart : PopoverPlacement.BottomEnd;
            var placement = m_Direction switch
            {
                Direction.Horizontal => horizontalPlacement,
                Direction.Vertical => PopoverPlacement.EndBottom,
                _ => throw new ArgumentOutOfRangeException()
            };
            m_MenuBuilder?.Dismiss(DismissType.Consecutive);
            m_MenuBuilder = MenuBuilder.Build(m_MoreButton)
                .SetCloseOnSelection(closeOnSelection)
                .SetPlacement(placement);

            var selectable = selectionType != SelectionType.None;
            for (var i = m_FirstIndexOutOfBound; i < m_HandledChildren.Count; i++)
            {
                if (m_HandledChildren[i] is ActionButton button)
                {
                    m_MenuBuilder.AddAction(i, button.label, button.icon, null, OnMenuActionPressed);
                    var item = (MenuItem) m_MenuBuilder.currentMenu.ElementAt(m_MenuBuilder.currentMenu.childCount - 1);
                    item.selectable = selectable;
                    if (selectable)
                        item.SetValueWithoutNotify(m_SelectedIndices.Contains(i));
                }
            }

            m_MenuBuilder.Show();
        }

        void OnMenuActionPressed(ClickEvent evt)
        {
            if (
                evt.target is MenuItem {userData: int actionId and >= 0} &&
                actionId < m_HandledChildren.Count &&
                m_HandledChildren[actionId] is ActionButton btn)
            {
                btn.clickable?.InvokePressed(evt);
            }
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// The UXML factory for the ActionGroup.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ActionGroup, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="ActionGroup"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription m_Compact = new UxmlBoolAttributeDescription
            {
                name = "compact",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription m_Justified = new UxmlBoolAttributeDescription
            {
                name = "justified",
                defaultValue = false
            };

            readonly UxmlBoolAttributeDescription m_Quiet = new UxmlBoolAttributeDescription
            {
                name = "quiet",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<Direction> m_Direction = new UxmlEnumAttributeDescription<Direction>
            {
                name = "direction",
                defaultValue = Direction.Horizontal
            };

            readonly UxmlEnumAttributeDescription<SelectionType> m_SelectionType = new UxmlEnumAttributeDescription<SelectionType>
            {
                name = "selection-type",
                defaultValue = k_DefaultSelectionType
            };

            readonly UxmlBoolAttributeDescription m_CloseOnSelection = new UxmlBoolAttributeDescription
            {
                name = "close-on-selection",
                defaultValue = true
            };

            readonly UxmlBoolAttributeDescription m_AllowNoSelection = new UxmlBoolAttributeDescription
            {
                name = "allow-no-selection",
                defaultValue = true
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
                var el = (ActionGroup)ve;
                el.quiet = m_Quiet.GetValueFromBag(bag, cc);
                el.compact = m_Compact.GetValueFromBag(bag, cc);
                el.direction = m_Direction.GetValueFromBag(bag, cc);
                el.justified = m_Justified.GetValueFromBag(bag, cc);
                el.selectionType = m_SelectionType.GetValueFromBag(bag, cc);
                el.closeOnSelection = m_CloseOnSelection.GetValueFromBag(bag, cc);
                el.allowNoSelection = m_AllowNoSelection.GetValueFromBag(bag, cc);
            }
        }
#endif
    }
}
