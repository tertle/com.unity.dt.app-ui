using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// ActionBar UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class ActionBar : BaseVisualElement
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId messageProperty = nameof(message);

        internal static readonly BindingId collectionViewProperty = nameof(collectionView);
#endif

#if UNITY_LOCALIZATION_PRESENT
        const string k_DefaultMessage = "@AppUI:selectedItemsMessage";
#else
        const string k_DefaultMessage = "{0} Selected item(s)";
#endif

        /// <summary>
        /// The ActionBar main styling class.
        /// </summary>
        public const string ussClassName = "appui-actionbar";

        /// <summary>
        /// The ActionBar action group styling class.
        /// </summary>
        public const string actionGroupUssClassName = ussClassName + "__actiongroup";

        /// <summary>
        /// The ActionBar checkbox styling class.
        /// </summary>
        public const string checkboxUssClassName = ussClassName + "__checkbox";

        /// <summary>
        /// The ActionBar label styling class.
        /// </summary>
        public const string labelUssClassName = ussClassName + "__label";

        readonly ActionGroup m_ActionGroup;

        readonly Checkbox m_SelectAllCheckbox;

        BaseVerticalCollectionView m_CollectionView;

        readonly LocalizedTextElement m_Label;

        string m_Message;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ActionBar()
        {
            AddToClassList(ussClassName);

            pickingMode = PickingMode.Ignore;

            m_SelectAllCheckbox = new Checkbox { name = checkboxUssClassName, emphasized = true };
            m_SelectAllCheckbox.AddToClassList(checkboxUssClassName);
            hierarchy.Add(m_SelectAllCheckbox);
            m_SelectAllCheckbox.RegisterValueChangedCallback(OnCheckboxValueChanged);

            m_Label = m_SelectAllCheckbox.Q<LocalizedTextElement>(Checkbox.labelUssClassName);
            m_Label.variables = new object[]
            {
                new Dictionary<string, object>
                {
                    {"itemCount", 0}
                }
            };

            m_ActionGroup = new ActionGroup { name = actionGroupUssClassName };
            m_ActionGroup.AddToClassList(actionGroupUssClassName);
            hierarchy.Add(m_ActionGroup);

            collectionView = null;
            message = k_DefaultMessage;
        }

        /// <summary>
        /// The collection view attached to this <see cref="ActionBar"/>.
        /// </summary>
        [Tooltip("The collection view attached to this ActionBar. " +
            "The collection view is used to get the selected indices and the items source.")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public BaseVerticalCollectionView collectionView
        {
            get => m_CollectionView;

            set
            {
                if (m_CollectionView != null)
#if UITK_SELECTED_INDICES_CHANGED
                    m_CollectionView.selectedIndicesChanged -= OnSelectedIndicesChanged;
#else
                    m_CollectionView.onSelectedIndicesChange -= OnSelectedIndicesChanged;
#endif
                m_CollectionView = value;
                if (m_CollectionView != null)
#if UITK_SELECTED_INDICES_CHANGED
                    m_CollectionView.selectedIndicesChanged += OnSelectedIndicesChanged;
#else
                    m_CollectionView.onSelectedIndicesChange += OnSelectedIndicesChanged;
#endif

                RefreshUI();
            }
        }

        /// <summary>
        /// The list of selected indices from the Collection View.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public IEnumerable<int> selectedIndices => m_CollectionView?.selectedIndices ?? new List<int>();

        /// <summary>
        /// The items source from the Collection View.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty(ReadOnly = true)]
#endif
        public IList itemsSource => m_CollectionView?.itemsSource;

        /// <summary>
        /// Text used for item selection message.
        /// </summary>
        /// <remarks>
        /// We recommend to use a SmartString text in order to adjust the
        /// text based on the number of selected items.
        /// </remarks>
        /// <example>
        /// <code>
        /// {itemCount:plural:Nothing selected|One selected item|{} selected items}
        /// </code>
        /// </example>
        [Tooltip("Text used for item selection message.\n\n" +
            "We recommend to use a SmartString text in order to adjust the text based on the number of selected items.\n" +
            "Example: {itemCount:plural:Nothing selected|One selected item|{} selected items}")]
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Header("Action Bar")]
#endif
        public string message
        {
            get => m_Message;
            set
            {
                var changed = m_Message != value;
                m_Message = value;
                RefreshUI();
#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(messageProperty);
#endif
            }
        }

        /// <summary>
        /// The content container of the <see cref="ActionBar"/>.
        /// </summary>
        public override VisualElement contentContainer => m_ActionGroup.contentContainer;

        void OnCheckboxValueChanged(ChangeEvent<CheckboxState> evt)
        {
            var val = evt.newValue;

            if (m_CollectionView == null)
                return;

            switch (val)
            {
                case CheckboxState.Unchecked:
                    m_CollectionView.ClearSelection();
                    break;
                case CheckboxState.Intermediate:
                    // do nothing
                    break;
                case CheckboxState.Checked:
                    var range = new int[itemsSource.Count];
                    for (var i = 0; i < itemsSource.Count; i++)
                    {
                        range[i] = i;
                    }
                    m_CollectionView.SetSelection(range);
                    break;
                default:
                    throw new ValueOutOfRangeException(nameof(val), val);
            }
        }

        void OnSelectedIndicesChanged(IEnumerable<int> _)
        {
            RefreshUI();
        }

        void RefreshUI()
        {
            var selectionCount = 0;
            foreach (var unused in selectedIndices)
            {
                selectionCount++;
            }
            var checkboxValue = CheckboxState.Unchecked;
            if (selectionCount > 0)
            {
                checkboxValue = selectionCount == itemsSource.Count
                    ? CheckboxState.Checked
                    : CheckboxState.Intermediate;
            }
            m_SelectAllCheckbox.SetValueWithoutNotify(checkboxValue);
            m_Label.variables = new object[]
            {
                new Dictionary<string, object>
                {
                    {"itemCount", selectionCount}
                }
            };
            m_SelectAllCheckbox.label = string.IsNullOrEmpty(m_Message) ? m_Message : string.Format(m_Message, selectionCount);
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// The UXML factory for the <see cref="ActionBar"/>.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<ActionBar, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="ActionBar"/>.
        /// </summary>
        public new class UxmlTraits : BaseVisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Message = new UxmlStringAttributeDescription
            {
                name = "message",
                defaultValue = k_DefaultMessage
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
                var el = (ActionBar)ve;

                var msg = k_DefaultMessage;
                if (m_Message.TryGetValueFromBag(bag, cc, ref msg))
                    el.message = msg;
            }
        }
#endif
    }
}
