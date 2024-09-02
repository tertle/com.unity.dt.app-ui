using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A container for a set of <see cref="Radio"/> UI elements.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class RadioGroup : BaseVisualElement, IInputElement<string>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = nameof(value);

        internal static readonly BindingId invalidProperty = nameof(invalid);

        internal static readonly BindingId validateValueProperty = nameof(validateValue);

#endif

        /// <summary>
        /// The RadioGroup main styling class.
        /// </summary>
        public const string ussClassName = "appui-radiogroup";

        string m_Value = null;

        Func<string, bool> m_ValidateValue;

        readonly Dictionary<string, Radio> m_RadioByKey = new Dictionary<string, Radio>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RadioGroup()
        {
            AddToClassList(ussClassName);
            RegisterCallback<ChangeEvent<bool>>(OnItemChosen);
        }

        /// <summary>
        /// The RadioGroup content container.
        /// </summary>
        public override VisualElement contentContainer => this;

        /// <summary>
        /// The selected item key.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> if the value is out of range.</exception>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public string value
        {
            get => m_Value;
            set
            {
                if (value == m_Value)
                    return;
                using var evt = ChangeEvent<string>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        /// <summary>
        /// The RadioGroup invalid state.
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
        /// The RadioGroup validation function.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
        public Func<string, bool> validateValue
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
        /// Set the value without notifying the listeners.
        /// </summary>
        /// <param name="newValue"> The new value.</param>
        public void SetValueWithoutNotify(string newValue)
        {
            foreach (var radioByKey in m_RadioByKey)
            {
                radioByKey.Value.SetValueWithoutNotify(radioByKey.Key == newValue);
            }

            m_Value = newValue;

            if (validateValue != null)
                invalid = !validateValue.Invoke(newValue);
        }

        void OnItemChosen(ChangeEvent<bool> evt)
        {
            if (evt.target is Radio radio && m_RadioByKey.ContainsKey(radio.key))
            {
                evt.StopPropagation();

                if (evt.newValue)
                    value = radio.key;
            }
        }

        internal void AddRadio(Radio radio)
        {
            if (string.IsNullOrEmpty(radio.key))
                return;

            m_RadioByKey[radio.key] = radio;

            value ??= radio.key;

            radio.SetValueWithoutNotify(radio.key == value);
        }

        internal void RemoveRadio(Radio radio)
        {
            m_RadioByKey.Remove(radio.key);
            if (value == radio.key)
                value = m_RadioByKey.Keys.FirstOrDefault();
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="RadioGroup"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<RadioGroup, UxmlTraits> { }

#endif
    }
}