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
    /// The type of selection for a Picker.
    /// </summary>
    public enum PickerSelectionType
    {
        /// <summary>
        /// The Picker allows only one item to be selected.
        /// </summary>
        Single,

        /// <summary>
        /// The Picker allows multiple items to be selected.
        /// </summary>
        Multiple
    }

    /// <summary>
    /// An Item from a Picker.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class PickerItem : BaseVisualElement, IPressable
    {
        /// <summary>
        /// The main styling class for the PickerItem.
        /// </summary>
        public const string ussClassName = "appui-picker-item";

        Pressable m_Clickable;

        /// <summary>
        /// Clickable Manipulator for this MenuItem.
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
        /// Default constructor.
        /// </summary>
        public PickerItem()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Position;
            focusable = true;
            tabIndex = 0;
            clickable = new Pressable(OnClick);

            RegisterCallback<KeyDownEvent>(OnKeyDown);
            this.AddManipulator(new KeyboardFocusController());
        }

        void OnClick()
        {
            using var evt = ActionTriggeredEvent.GetPooled();
            evt.target = this;
            SendEvent(evt);
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            var handled = false;

            switch (evt.keyCode)
            {
                case KeyCode.DownArrow:
                    if (parent.IndexOf(this) != parent.childCount - 1)
                        focusController.FocusNextInDirectionEx(VisualElementFocusChangeDirection.right);
                    handled = true;
                    break;
                case KeyCode.UpArrow:
                    if (parent.IndexOf(this) != 0)
                        focusController.FocusNextInDirectionEx(VisualElementFocusChangeDirection.left);
                    handled = true;
                    break;
                case KeyCode.RightArrow:
                case KeyCode.LeftArrow:
                    handled = true;
                    break;
            }

            if (handled)
            {
                evt.StopPropagation();

            }
        }
    }

    /// <summary>
    /// Picker UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class Picker : ExVisualElement, IInputElement<IEnumerable<int>>, ISizeableElement, IPressable
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId closeOnSelectionProperty = new BindingId(nameof(closeOnSelection));

        internal static readonly BindingId defaultMessageProperty = new BindingId(nameof(defaultMessage));

        internal static readonly BindingId defaultValueProperty = new BindingId(nameof(defaultValue));

        internal static readonly BindingId sourceItemsProperty = new BindingId(nameof(sourceItems));

        internal static readonly BindingId emphasizedProperty = new BindingId(nameof(emphasized));

        internal static readonly BindingId selectionTypeProperty = new BindingId(nameof(selectionType));

        internal static readonly BindingId selectedIndexProperty = new BindingId(nameof(selectedIndex));

        internal static readonly BindingId sizeProperty = new BindingId(nameof(size));

        internal static readonly BindingId valueProperty = new BindingId(nameof(value));

        internal static readonly BindingId invalidProperty = new BindingId(nameof(invalid));

        internal static readonly BindingId validateValueProperty = new BindingId(nameof(validateValue));

#endif

        /// <summary>
        /// The Picker main styling class.
        /// </summary>
        public const string ussClassName = "appui-picker";

        /// <summary>
        /// The Picker title container styling class.
        /// </summary>
        public const string titleContainerUssClassName = ussClassName + "__titlecontainer";

        /// <summary>
        /// The Picker title styling class.
        /// </summary>
        public const string titleUssClassName = ussClassName + "__title";

        /// <summary>
        /// The Picker trailing container styling class.
        /// </summary>
        public const string trailingContainerUssClassName = ussClassName + "__trailingcontainer";

        /// <summary>
        /// The Picker caret styling class.
        /// </summary>
        public const string caretUssClassName = ussClassName + "__caret";

        /// <summary>
        /// The Picker size styling class.
        /// </summary>
        [EnumName("GetSizeUssClassName", typeof(Size))]
        public const string sizeUssClassName = ussClassName + "--size-";

        /// <summary>
        /// The Picker emphasized mode styling class.
        /// </summary>
        public const string emphasizedUssClassName = ussClassName + "--emphasized";

        /// <summary>
        /// The Picker menu styling class.
        /// </summary>
        public const string appuiPickerMenu = ussClassName + "__menu";

        /// <summary>
        /// The list of items contained in the Picker.
        /// </summary>
        protected readonly List<PickerItem> m_Items = new List<PickerItem>();

        int[] m_DefaultValue = null;

        Size m_Size;

        IList m_SourceItems;

        /// <summary>
        /// The list of indices of the selected items.
        /// </summary>
        protected readonly List<int> m_Value = new List<int>();

        bool m_ValueSet;

        Pressable m_Clickable;

        MenuBuilder m_MenuBuilder;

        bool m_CloseOnSelection;

        Func<IEnumerable<int>, bool> m_ValidateValue;

        /// <summary>
        /// The container for the Picker title.
        /// </summary>
        protected readonly VisualElement m_TitleContainer;

        PickerSelectionType m_SelectionType;

        string m_DefaultMessage =
#if UNITY_LOCALIZATION_PRESENT
            "@AppUI:dropdownSelectMessage";
#else
            "Select";
#endif

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Picker()
            : this(null) { }

        /// <summary>
        /// Construct a Picker UI element with a provided dynamic collection of items.
        /// </summary>
        /// <param name="items">An items collection.</param>
        /// <param name="defaultIndices">The selected index by default.</param>
        public Picker(IList items, int[] defaultIndices = null)
        {
            AddToClassList(ussClassName);

            clickable = new Pressable(OnClicked);
            focusable = true;
            pickingMode = PickingMode.Position;
            passMask = 0;
            tabIndex = 0;

            m_TitleContainer = new VisualElement { name = titleContainerUssClassName, pickingMode = PickingMode.Ignore };
            m_TitleContainer.AddToClassList(titleContainerUssClassName);

            var trailingContainer = new VisualElement { name = trailingContainerUssClassName, pickingMode = PickingMode.Ignore };
            trailingContainer.AddToClassList(trailingContainerUssClassName);

            var caret = new Icon { name = caretUssClassName, pickingMode = PickingMode.Ignore };
            caret.AddToClassList(caretUssClassName);

            hierarchy.Add(m_TitleContainer);
            hierarchy.Add(trailingContainer);
            trailingContainer.hierarchy.Add(caret);

            defaultValue = defaultIndices;
            size = Size.M;
            sourceItems = items;
            closeOnSelection = true;

            if (defaultIndices is { Length: >0 })
                SetValueWithoutNotify(defaultIndices);

            this.AddManipulator(new KeyboardFocusController(OnKeyboardFocusIn, OnPointerFocusIn));
        }

        /// <summary>
        /// Close the picker menu automatically when an item is selected.
        /// </summary>
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
                if (m_CloseOnSelection == value)
                    return;

                m_CloseOnSelection = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in closeOnSelectionProperty);
#endif
            }
        }

        /// <summary>
        /// Clickable Manipulator for this Picker.
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
        /// The Picker default message when no item is selected.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string defaultMessage
        {
            get => m_DefaultMessage;
            set
            {
                if (m_DefaultMessage == value)
                    return;
                m_DefaultMessage = value;
                RefreshUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in defaultMessageProperty);
#endif
            }
        }

        /// <summary>
        /// The Picker default value. This is the value that will be selected if no value is set.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public int[] defaultValue
        {
            get => m_DefaultValue;

            set
            {
                if (m_DefaultValue == value)
                    return;
                m_DefaultValue = value;
                if (!m_ValueSet)
                    SetValueWithoutNotify(m_DefaultValue);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in defaultValueProperty);
#endif
            }
        }

        /// <summary>
        /// The source items collection.
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
                this.value = m_DefaultValue;
                m_ValueSet = false;
                RefreshUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in sourceItemsProperty);
#endif
            }
        }

        /// <summary>
        /// The Picker size.
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
                RemoveFromClassList(GetSizeUssClassName(m_Size));
                m_Size = value;
                AddToClassList(GetSizeUssClassName(m_Size));

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in sizeProperty);
#endif
            }
        }

        /// <summary>
        /// The Picker emphasized mode.
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
                EnableInClassList(emphasizedUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in emphasizedProperty);
#endif
            }
        }

        /// <summary>
        /// The Picker selection type.
        /// </summary>
        /// <remarks>
        /// If the selection type is changed, the Picker value will be reset.
        /// </remarks>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public PickerSelectionType selectionType
        {
            get => m_SelectionType;
            set
            {
                m_SelectionType = value;
                this.value = new int[] { };

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in selectionTypeProperty);
#endif
            }
        }

        /// <summary>
        /// Quick access to the currently selected index for a Picker in Single selection mode.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public int selectedIndex
        {
            get => m_Value.Count > 0 ? m_Value[0] : -1;
            set
            {
                var unchanged = (m_Value.Count == 0 && value == -1) || (m_Value.Count == 1 && m_Value[0] == value);
                if (unchanged)
                    return;

                this.value = new[] { value };

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in selectedIndexProperty);
#endif
            }
        }

        /// <summary>
        /// The Picker invalid state.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public bool invalid
        {
            get => ClassListContains(Styles.invalidUssClassName);
            set
            {
                var changed = invalid != value;
                EnableInClassList(Styles.invalidUssClassName, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in invalidProperty);
#endif
            }
        }

        /// <summary>
        /// The Picker validation function.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<IEnumerable<int>, bool> validateValue
        {
            get => m_ValidateValue;
            set
            {
                var changed = m_ValidateValue != value;
                m_ValidateValue = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in validateValueProperty);
#endif
            }
        }

        /// <summary>
        /// Set the Picker value without notifying any listeners.
        /// </summary>
        /// <param name="newValue"> The new value to set. </param>
        public void SetValueWithoutNotify(IEnumerable<int> newValue)
        {
            if (sourceItems == null)
                return;

            var values = newValue != null ? new List<int>(newValue) : new List<int>();
            foreach (var nv in values)
            {
                if (nv < 0 || nv >= sourceItems.Count)
                {
                    Debug.LogWarning($"<b>[App UI]</b> [Picker]: Trying to set a value out of range. " +
                        $"Value: {nv}, Range: [0, {sourceItems.Count - 1}]");
                    return;
                }
            }

            m_ValueSet = true;
            m_Value.Clear();
            if (m_SelectionType == PickerSelectionType.Multiple)
            {
                m_Value.AddRange(values);
            }
            else if (values.Count > 0)
            {
                if (values.Count > 1)
                    Debug.LogWarning($"<b>[App UI]</b> [Picker]: Trying to set multiple values on a single selection Picker. " +
                        $"Only the first value will be set. Value: {values[0]}");
                m_Value.Add(values[0]);
            }
            RefreshTitleUI();

            var menu = m_MenuBuilder?.currentMenu;
            if (menu != null)
            {
                for (var i = 0; i < menu.childCount; i++)
                {
                    var item = menu[i];
                    var isSelected = m_Value.Contains(i);
                    item.EnableInClassList(Styles.selectedUssClassName, isSelected);
                }
            }

            if (m_ValidateValue != null)
                invalid = !m_ValidateValue.Invoke(m_Value);
        }

        /// <summary>
        /// The Picker value. This is the index of the selected item.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public IEnumerable<int> value
        {
            get => m_Value;

            set
            {
                var newValue = value != null ? new List<int>(value) : new List<int>();
                if (EnumerableExtensions.SequenceEqual(m_Value, newValue))
                    return;

                using var evt = ChangeEvent<IEnumerable<int>>.GetPooled(new List<int>(m_Value), newValue);
                evt.target = this;
                SetValueWithoutNotify(newValue);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
                NotifyPropertyChanged(in selectedIndexProperty);
#endif
            }
        }

        /// <summary>
        /// Refresh the Picker UI.
        /// </summary>
        public void Refresh()
        {
            RefreshUI();
        }

        void OnPointerFocusIn(FocusInEvent evt)
        {
            passMask = 0;
        }

        void OnKeyboardFocusIn(FocusInEvent evt)
        {
            passMask = Passes.Clear | Passes.Outline;
        }

        void OnClicked()
        {
            m_MenuBuilder?.Dismiss(DismissType.Consecutive);
            if (sourceItems is not { Count: > 0 })
                return;

            m_MenuBuilder = MenuBuilder.Build(this, BuildPickerMenu());
            AddToClassList(Styles.openUssClassName);
            m_MenuBuilder.SetCloseOnSelection(closeOnSelection);
            m_MenuBuilder.dismissed += OnMenuDismissed;
            m_MenuBuilder.Show();
        }

        void OnMenuDismissed(MenuBuilder menu, DismissType reason)
        {
            menu.dismissed -= OnMenuDismissed;
            if (menu == m_MenuBuilder)
            {
                RemoveFromClassList(Styles.openUssClassName);
                m_MenuBuilder = null;
            }
        }

        /// <summary>
        /// Refresh the Picker UI.
        /// </summary>
        protected void RefreshUI()
        {
            RefreshListUI();
            RefreshTitleUI();
            SetValueWithoutNotify(m_Value);
        }

        /// <summary>
        /// Refresh the Picker list UI.
        /// </summary>
        protected void RefreshListUI()
        {
            // clear items
            for (var i = 0; i < m_Items.Count; i++)
            {
                OnUnbindItem(m_Items[i], i);
                m_Items[i].clickable.clickedWithEventInfo -= OnItemClicked;
            }
            m_Items.Clear();

            // create menu items
            if (sourceItems != null)
            {
                for (var i = 0; i < sourceItems.Count; i++)
                {
                    var item = new PickerItem();
                    item.clickable.clickedWithEventInfo += OnItemClicked;
                    var content = OnRequestItemCreation(i);
                    item.Add(content);
                    m_Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Refresh the Picker title UI.
        /// </summary>
        protected abstract void RefreshTitleUI();

        /// <summary>
        /// Create a Picker item.
        /// </summary>
        /// <param name="i"> The index of the item to create. </param>
        /// <returns> The created item. </returns>
        protected abstract VisualElement OnRequestItemCreation(int i);

        /// <summary>
        /// Unbind a Picker item.
        /// </summary>
        /// <param name="item"> The item to unbind. </param>
        /// <param name="index"> The index of the item to unbind. </param>
        protected abstract void OnUnbindItem(VisualElement item, int index);

        void OnItemClicked(EventBase evt)
        {
            if (evt.target is VisualElement item)
            {
                var idx = item.parent.IndexOf(item);
                if (m_SelectionType == PickerSelectionType.Single)
                {
                    value = new[] {idx};
                }
                else
                {
                    var values = new List<int>(m_Value);
                    if (values.Contains(idx))
                        values.Remove(idx);
                    else
                        values.Add(idx);
                    value = values;
                }
            }
        }

        Menu BuildPickerMenu()
        {
            var menu = new Menu();

            menu.AddToClassList(appuiPickerMenu);
            menu.style.minWidth = paddingRect.width - 6;

            for (var i = 0; i < m_Items.Count; i++)
            {
                var isSelected = m_Value.Contains(i);
                m_Items[i].EnableInClassList(Styles.selectedUssClassName, isSelected);
                menu.Add(m_Items[i]);
            }

            return menu;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Picker"/>.
        /// </summary>
        public new class UxmlTraits : ExVisualElement.UxmlTraits
        {

            readonly UxmlBoolAttributeDescription m_Emphasized = new UxmlBoolAttributeDescription
            {
                name = "emphasized",
                defaultValue = false
            };

            readonly UxmlEnumAttributeDescription<Size> m_Size = new UxmlEnumAttributeDescription<Size>
            {
                name = "size",
                defaultValue = Size.M,
            };

            readonly UxmlBoolAttributeDescription m_CloseOnSelection = new UxmlBoolAttributeDescription
            {
                name = "close-on-selection",
                defaultValue = true,
            };

            readonly UxmlEnumAttributeDescription<PickerSelectionType> m_SelectionType = new UxmlEnumAttributeDescription<PickerSelectionType>
            {
                name = "selection-type",
                defaultValue = PickerSelectionType.Single,
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

                var el = (Picker)ve;
                el.size = m_Size.GetValueFromBag(bag, cc);
                el.emphasized = m_Emphasized.GetValueFromBag(bag, cc);
                el.closeOnSelection = m_CloseOnSelection.GetValueFromBag(bag, cc);

                el.selectionType = m_SelectionType.GetValueFromBag(bag, cc);
            }
        }

#endif
    }

    /// <summary>
    /// A Picker UI element with a title and a list of items.
    /// </summary>
    /// <typeparam name="TItemType"> The type of the items contained in the Picker. </typeparam>
    /// <typeparam name="TTitleType"> The type of the title contained in the Picker. </typeparam>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public abstract partial class Picker<TItemType, TTitleType> : Picker
        where TItemType : BaseVisualElement, new()
        where TTitleType : BaseVisualElement, new()
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId bindItemProperty = new BindingId(nameof(bindItem));

        internal static readonly BindingId bindTitleProperty = new BindingId(nameof(bindTitle));

        internal static readonly BindingId makeItemProperty = new BindingId(nameof(makeItem));

        internal static readonly BindingId makeTitleProperty = new BindingId(nameof(makeTitle));

        internal static readonly BindingId unbindItemProperty = new BindingId(nameof(unbindItem));

#endif

        Action<TItemType, int> m_BindItem;

        Func<TItemType> m_MakeItem;

        Func<TTitleType> m_MakeTitle;

        Action<TTitleType, IEnumerable<int>> m_BindTitle;

        Action<TItemType, int> m_UnbindItem;

        /// <summary>
        /// The function used to create a Picker item.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<TItemType> makeItem
        {
            get => m_MakeItem;
            set
            {
                if (m_MakeItem == value)
                    return;
                m_MakeItem = value;
                RefreshListUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in makeItemProperty);
#endif
            }
        }

        /// <summary>
        /// The function used to bind a Picker item.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Action<TItemType, int> bindItem
        {
            get => m_BindItem;
            set
            {
                if (m_BindItem == value)
                    return;
                m_BindItem = value;
                RefreshListUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in bindItemProperty);
#endif
            }
        }

        /// <summary>
        /// The function used to unbind a Picker item.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Action<TItemType, int> unbindItem
        {
            get => m_UnbindItem;
            set
            {
                if (m_UnbindItem == value)
                    return;
                m_UnbindItem = value;
                RefreshListUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in unbindItemProperty);
#endif
            }
        }

        /// <summary>
        /// The function used to create a Picker title.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<TTitleType> makeTitle
        {
            get => m_MakeTitle;
            set
            {
                if (m_MakeTitle == value)
                    return;
                m_MakeTitle = value;
                RefreshTitleUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in makeTitleProperty);
#endif
            }
        }

        /// <summary>
        /// The function used to bind a Picker title.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Action<TTitleType, IEnumerable<int>> bindTitle
        {
            get => m_BindTitle;
            set
            {
                if (m_BindTitle == value)
                    return;
                m_BindTitle = value;
                RefreshTitleUI();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in bindTitleProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="items"> The items collection. </param>
        /// <param name="makeItemFunc"> The function used to create a Picker item. </param>
        /// <param name="makeTitleFunc"> The function used to create a Picker title. </param>
        /// <param name="bindItemFunc"> The function used to bind a Picker item. </param>
        /// <param name="bindTitleFunc"> The function used to bind a Picker title. </param>
        /// <param name="unbindItemFunc"> The function used to unbind a Picker item. </param>
        /// <param name="defaultIndices"> The selected index by default. </param>
        public Picker(
            IList items,
            Func<TItemType> makeItemFunc = null,
            Func<TTitleType> makeTitleFunc = null,
            Action<TItemType, int> bindItemFunc = null,
            Action<TTitleType, IEnumerable<int>> bindTitleFunc = null,
            Action<TItemType, int> unbindItemFunc = null,
            int[] defaultIndices = null)
            : base(items, defaultIndices)
        {
            unbindItem = unbindItemFunc;
            makeItem = makeItemFunc;
            makeTitle = makeTitleFunc;
            bindItem = bindItemFunc;
            bindTitle = bindTitleFunc;
        }

        /// <inheritdoc cref="Picker.RefreshTitleUI"/>
        protected override void RefreshTitleUI()
        {
            m_TitleContainer.Clear();
            var title = makeTitle?.Invoke() ?? new TTitleType();
            title.name = titleUssClassName;
            title.pickingMode = PickingMode.Ignore;
            title.AddToClassList(titleUssClassName);
            m_TitleContainer.hierarchy.Add(title);

            bindTitle?.Invoke(title, m_Value);
        }

        /// <inheritdoc cref="Picker.OnRequestItemCreation"/>
        protected override VisualElement OnRequestItemCreation(int i)
        {
            var content = makeItem?.Invoke() ?? new TItemType();
            if (bindItem != null)
                bindItem.Invoke(content, i);
            else if (content is LocalizedTextElement text)
                text.text = sourceItems[i].ToString();

            return content;
        }

        /// <inheritdoc cref="Picker.OnUnbindItem"/>
        protected override void OnUnbindItem(VisualElement item, int index)
        {
            if (item is TItemType typedItem)
                unbindItem?.Invoke(typedItem, index);
        }

        /// <summary>
        /// Get the Picker item at the given index.
        /// </summary>
        /// <param name="index"> The index of the item to get. </param>
        /// <returns> The Picker item at the given index. </returns>
        protected TItemType GetPickerItem(int index)
        {
            if (index < 0 || index >= sourceItems.Count)
                return null;

            return m_Items[index] as TItemType;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="Picker{T,TU}"/>.
        /// </summary>
        public new class UxmlTraits : Picker.UxmlTraits { }

#endif
    }
}
