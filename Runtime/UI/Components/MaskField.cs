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
    /// MaskField UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class MaskField : Dropdown, IInputElement<int>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId maskValueProperty = nameof(value);

        internal static readonly BindingId getMaskValueProperty = nameof(getMaskValue);

        internal static readonly BindingId getDisplayNameProperty = nameof(getDisplayName);
#endif

        /// <summary>
        /// The main styling class for the MaskField.
        /// </summary>
        public new const string ussClassName = "appui-mask-field";

        int m_DefaultMaskValue;

        GetDisplayNameDelegate m_GetDisplayName;

        GetMaskValueDelegate m_GetMaskValue;

        const string k_NothingMessage =
#if UNITY_LOCALIZATIONkPRESENT
            "@AppUI:nothing";
#else
            "Nothing";
#endif

        const string k_EverythingMessage =
#if UNITY_LOCALIZATION_PRESENT
            "@AppUI:everything";
#else
            "Everything";
#endif

        const string k_MixedMessage =
#if UNITY_LOCALIZATION_PRESENT
            "@AppUI:mixed";
#else
            "Mixed...";
#endif

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MaskField() : this(null) { }

        /// <summary>
        /// Constructor with a default mask value.
        /// </summary>
        /// <param name="sourceItems"> The source items collection. </param>
        /// <param name="defaultMaskValue"> The default value for the field. </param>
        /// <param name="getDisplayNameFunc"> An optional function to get the display name. </param>
        /// <param name="getMaskValueFunc"> An optional function to get the mask value. </param>
        /// <param name="bindItemFunc"> An optional function to bind the item. </param>
        /// <param name="bindTitleFunc"> An optional function to bind the title. </param>
        public MaskField(
            IList sourceItems,
            int defaultMaskValue = 0,
            GetDisplayNameDelegate getDisplayNameFunc = null,
            GetMaskValueDelegate getMaskValueFunc = null,
            BindItemFunc bindItemFunc = null,
            BindTitleFunc bindTitleFunc = null)
            : base(sourceItems, bindItemFunc, bindTitleFunc)
        {
            AddToClassList(ussClassName);

            selectionType = PickerSelectionType.Multiple;

            makeItem = MakeItem;
            makeTitle = MakeTitle;
            getDisplayName = getDisplayNameFunc;
            getMaskValue = getMaskValueFunc;
            bindItem = bindItemFunc ?? OnBindMaskItem;
            bindTitle = bindTitleFunc ?? OnBindTitle;

            Init(defaultMaskValue);
        }

        void Init(int newMaskValue)
        {
            if (value != newMaskValue)
                SetValueWithoutNotify(newMaskValue);
            else
                Refresh();
        }

        /// <summary>
        /// Set the value of the field without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value to set. </param>
        public void SetValueWithoutNotify(int newValue)
        {
            var indices = ConvertMaskValueToIndices(newValue);
            base.SetValueWithoutNotify(indices);
        }

        /// <inheritdoc />
        protected override void OnValueSet()
        {
            base.OnValueSet();
            if (validateValue != null)
                invalid = !validateValue(value);
        }

        /// <summary>
        /// The current value of the field.
        /// </summary>
        public new int value
        {
            get
            {
                var indices = base.value;
                if (indices == null)
                    return 0;
                var result = 0;
                foreach (var index in indices)
                {
                    var itemValue = GetItemValue(index);
                    result |= itemValue;
                }
                return result;
            }
            set
            {
                var previousValue = this.value;
                if (value == previousValue)
                    return;
                using var evt = ChangeEvent<int>.GetPooled(previousValue, value);
                evt.target = this;
                base.value = ConvertMaskValueToIndices(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in maskValueProperty);
#endif
            }
        }

        /// <summary>
        /// An optional function to validate the value.
        /// </summary>
        public new Func<int, bool> validateValue { get; set; }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("mask-value")]
#endif
        int defaultMaskValue
        {
            get => m_DefaultMaskValue;
            set
            {
                m_DefaultMaskValue = value;
                Init(m_DefaultMaskValue);
            }
        }

        /// <summary>
        /// Method to get the mask value for a specific index in the source items collection.
        /// </summary>
        /// <param name="index"> The index of the item in the source items collection. </param>
        /// <returns> The mask value for the item at the specified index. </returns>
        public delegate int GetMaskValueDelegate(int index);

        /// <summary>
        /// Get the mask value for a specific index in the source items collection.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public GetMaskValueDelegate getMaskValue
        {
            get => m_GetMaskValue;
            set
            {
                if (m_GetMaskValue == value)
                    return;

                m_GetMaskValue = value;
                Refresh();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in getMaskValueProperty);
#endif
            }
        }

        /// <summary>
        /// Method to get the display name for a specific index in the source items collection.
        /// </summary>
        /// <param name="index"> The index of the item in the source items collection. </param>
        /// <returns> The display name for the item at the specified index. </returns>
        public delegate string GetDisplayNameDelegate(int index);

        /// <summary>
        /// Get the display name for a specific index in the source items collection.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public GetDisplayNameDelegate getDisplayName
        {
            get => m_GetDisplayName;
            set
            {
                if (m_GetDisplayName == value)
                    return;

                m_GetDisplayName = value;
                Refresh();

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in getDisplayNameProperty);
#endif
            }
        }

        /// <inheritdoc />
        protected override void OnPickerItemsCreated()
        {
            base.OnPickerItemsCreated();

            var everythingContent = new DropdownItem { label = k_EverythingMessage };
            var everythingItem = new PickerItem {index = -2};
            everythingItem.clickable.clickedWithEventInfo += OnEverythingClicked;
            everythingItem.Add(everythingContent);
            m_Items.Insert(0, everythingItem);

            var nothingContent = new DropdownItem { label = k_NothingMessage };
            var nothingItem = new PickerItem {index = -1};
            nothingItem.clickable.clickedWithEventInfo += OnNothingClicked;
            nothingItem.Add(nothingContent);
            m_Items.Insert(1, nothingItem);
        }

        /// <inheritdoc />
        protected override void OnPickerItemRemoved(PickerItem item, int index)
        {
            base.OnPickerItemRemoved(item, index);
            switch (index)
            {
                case -2:
                    item.clickable.clickedWithEventInfo -= OnEverythingClicked;
                    break;
                case -1:
                    item.clickable.clickedWithEventInfo -= OnNothingClicked;
                    break;
            }
        }

        void OnEverythingClicked(EventBase evt)
        {
            var newValue = 0;
            for (var i = 0; i < sourceItems.Count; i++)
            {
                newValue |= GetItemValue(i);
            }
            value = newValue;
        }

        void OnNothingClicked(EventBase evt)
        {
            value = 0;
        }

        bool HasValidDataBindings()
        {
            if (sourceItems == null)
                return false;

            var isIntegerCollection = sourceItems is IList<int> or IList<Enum>;
            return isIntegerCollection || getMaskValue != null;
        }

        IEnumerable<int> ConvertMaskValueToIndices(int newValue)
        {
            var result = new List<int>();
            if (!HasValidDataBindings())
                return result;
            for (var i = 0; i < sourceItems.Count; i++)
            {
                var itemValue = GetItemValue(i);
                if ((newValue & itemValue) == itemValue)
                    result.Add(i);
            }
            return result;
        }

        void OnBindMaskItem(DropdownItem item, int index)
        {
            item.label = GetDisplayName(index);
        }

        void OnBindTitle(DropdownItem item, IEnumerable<int> indices)
        {
            using var enumerator = indices?.GetEnumerator();

            var lastIndex = -1;
            var count = 0;
            if (enumerator != null)
            {
                while (enumerator.MoveNext())
                {
                    lastIndex = enumerator.Current;
                    count++;
                }
            }

            if (count == 0)
            {
                item.label = k_NothingMessage;
                return;
            }

            if (count == sourceItems.Count)
            {
                item.label = k_EverythingMessage;
                return;
            }

            if (count > 1)
            {
                item.label = k_MixedMessage;
                return;
            }

            item.label = GetDisplayName(lastIndex);
        }

        string GetDisplayName(int index)
        {
            if (sourceItems == null)
                return string.Empty;

            if (getDisplayName != null)
                return getDisplayName(index);

            return sourceItems switch
            {
                IList<string> stringList => stringList[index],
                IList<Enum> enumList => Enum.GetNames(enumList[index].GetType())[index],
                _ => index.ToString()
            };
        }

        int GetItemValue(int index)
        {
            if (sourceItems == null)
                return 0;

            if (getMaskValue != null)
                return getMaskValue(index);

            return sourceItems switch
            {
                IList<int> intList => intList[index],
                IList<Enum> enumList => Convert.ToInt32(enumList[index]),
                _ => 0
            };
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="MaskField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<MaskField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="MaskField"/>.
        /// </summary>
        public new class UxmlTraits : Dropdown.UxmlTraits
        {
            static readonly UxmlIntAttributeDescription k_DefaultMaskValue = new UxmlIntAttributeDescription
            {
                name = "mask-value",
                defaultValue = 0
            };

            /// <inheritdoc />
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var field = (MaskField)ve;
                field.defaultMaskValue = k_DefaultMaskValue.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
