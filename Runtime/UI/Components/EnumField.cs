using System;
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
    /// EnumField UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class EnumField : Dropdown, IInputElement<Enum>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS
        internal static readonly BindingId enumValueProperty = nameof(value);

        internal static readonly BindingId enumTypeProperty = nameof(enumType);
#endif

        /// <summary>
        /// The main styling class for the EnumField.
        /// </summary>
        public new const string ussClassName = "appui-enum-field";

        Type m_EnumType;

        string m_DefaultEnumValueStr;

        static readonly Dictionary<Type, (Array values,string[] names)> k_EnumData = new Dictionary<Type, (Array, string[])>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// You should use the constructor with the defaultEnumValue parameter to ensure the field is properly initialized.
        /// </remarks>
        public EnumField() : this(null) { }

        /// <summary>
        /// Constructor with a default enum value.
        /// </summary>
        /// <param name="defaultEnumValue"> The default value for the field. </param>
        /// <param name="bindItemFunc"> An optional function to bind the item. </param>
        /// <param name="bindTitleFunc"> An optional function to bind the title. </param>
        public EnumField(
            Enum defaultEnumValue = null,
            BindItemFunc bindItemFunc = null,
            BindTitleFunc bindTitleFunc = null)
            : base(null, bindItemFunc, bindTitleFunc)
        {
            AddToClassList(ussClassName);

            makeItem = MakeItem;
            makeTitle = MakeTitle;
            bindItem = bindItemFunc ?? OnBindEnum;
            bindTitle = bindTitleFunc ?? OnBindTitle;

            if (defaultValue != null)
                Init(defaultEnumValue);
        }

        void Init(Enum defaultEnumValue)
        {
            if (defaultEnumValue == null)
                throw new ArgumentNullException(nameof (defaultEnumValue));
            PopulateDataFromType(defaultEnumValue.GetType());
            if (!Equals(value, defaultEnumValue))
                SetValueWithoutNotify(defaultEnumValue);
            else
                Refresh();
        }

        void PopulateDataFromType(Type t)
        {
            m_EnumType = t;
            if (m_EnumType == null)
            {
                sourceItems = null;
                return;
            }

            if (!k_EnumData.ContainsKey(m_EnumType))
            {
                var values = Enum.GetValues(m_EnumType);
                var names = Enum.GetNames(m_EnumType);
                k_EnumData[m_EnumType] = (values, names);
            }
            sourceItems = k_EnumData[m_EnumType].values;
        }

        /// <summary>
        /// Set the value of the field without notifying the change.
        /// </summary>
        /// <param name="newValue"> The new value to set. </param>
        public void SetValueWithoutNotify(Enum newValue)
        {
            var indices = ConvertEnumValueToIndices(newValue);
            base.SetValueWithoutNotify(indices);
        }

        /// <inheritdoc />
        protected override void OnValueSet()
        {
            base.OnValueSet();
            if (validateValue != null)
                invalid = !validateValue.Invoke(value);
        }

        /// <summary>
        /// The current value of the field.
        /// </summary>
        public new Enum value
        {
            get => ConvertIndicesToEnumValue(base.value);
            set
            {
                if (Equals(value, this.value))
                    return;
                using var evt = ChangeEvent<Enum>.GetPooled(this.value, value);
                evt.target = this;
                base.value = ConvertEnumValueToIndices(value);
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in enumValueProperty);
#endif
            }
        }

        /// <summary>
        /// An optional function to validate the value.
        /// </summary>
        public new Func<Enum, bool> validateValue { get; set; }

        /// <summary>
        /// The type of the enum.
        /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("enum-type")]
#endif
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Type enumType
        {
            get => m_EnumType;
            set
            {
                if (m_EnumType == value)
                    return;
                m_EnumType = value;
                PopulateDataFromType(value);
                Init(k_EnumData[value].values.GetValue(0) as Enum);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in enumTypeProperty);
#endif
            }
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("enum-value")]
#endif
        string defaultValueStr
        {
            get => m_DefaultEnumValueStr;
            set
            {
                m_DefaultEnumValueStr = value;
                if (m_EnumType == null)
                    return;
                Init((Enum)Enum.Parse(m_EnumType, value));
            }
        }

        Enum ConvertIndicesToEnumValue(IEnumerable<int> indices)
        {
            using var enumerator = indices?.GetEnumerator();
            if (enumerator == null || !enumerator.MoveNext())
                return null;
            var values = k_EnumData[m_EnumType].values;
            return (Enum)values.GetValue(enumerator.Current);
        }

        IEnumerable<int> ConvertEnumValueToIndices(Enum newValue)
        {
            var result = new List<int>();
            if (newValue == null || m_EnumType == null)
                return result;
            result.Add(Array.IndexOf(k_EnumData[m_EnumType].values, newValue));
            return result;
        }

        void OnBindEnum(DropdownItem item, int index)
        {
            item.label = GetEnumName(index);
        }

        void OnBindTitle(DropdownItem item, IEnumerable<int> indices)
        {
            using var enumerator = indices?.GetEnumerator();
            if (enumerator == null || !enumerator.MoveNext())
            {
                item.label = defaultMessage;
                return;
            }
            item.label = GetEnumName(enumerator.Current);
            while (enumerator.MoveNext())
            {
                item.label = MemoryUtils.Concatenate(item.label, ", ", GetEnumName(enumerator.Current));
            }
        }

        string GetEnumName(int index)
        {
            if (m_EnumType == null)
                return null;
            var enumName = k_EnumData[m_EnumType].names[index];
            var field = m_EnumType.GetField(enumName);
            var attributes = field.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), false);
            if (attributes.Length > 0)
            {
                var displayName = ((System.ComponentModel.DisplayNameAttribute)attributes[0]).DisplayName;
                return displayName;
            }
            return enumName;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="EnumField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<EnumField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="EnumField"/>.
        /// </summary>
        public new class UxmlTraits : Dropdown.UxmlTraits
        {
            static readonly UxmlTypeAttributeDescription<Enum> k_EnumType = new UxmlTypeAttributeDescription<Enum>
            {
                name = "enum-type",
                defaultValue = null,
                use = UxmlAttributeDescription.Use.Required
            };

            static readonly UxmlStringAttributeDescription k_DefaultEnumValue = new UxmlStringAttributeDescription
            {
                name = "enum-value",
                defaultValue = null
            };

            /// <inheritdoc />
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var field = (EnumField)ve;
                field.enumType = k_EnumType.GetValueFromBag(bag, cc);
                field.defaultValueStr = k_DefaultEnumValue.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
