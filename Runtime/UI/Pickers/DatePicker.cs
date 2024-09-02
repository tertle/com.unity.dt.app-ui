using System;
using System.Runtime.CompilerServices;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A date picker control.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class DatePicker : BaseDatePicker, INotifyValueChanged<Date>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = new BindingId(nameof(value));

#endif
        Date m_Value;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DatePicker()
        {
            value = Date.now;
        }

        /// <summary>
        /// Set the value of the date picker without sending a change event.
        /// </summary>
        /// <param name="newValue"> The new value. </param>
        public void SetValueWithoutNotify(Date newValue)
        {
            m_Value = newValue;

            GoTo(newValue);
            displayMode = DisplayMode.Days;
        }

        /// <summary>
        /// The current value of the date picker.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public Date value
        {
            get => m_Value;
            set
            {
                if (value == m_Value)
                    return;

                using var evt = ChangeEvent<Date>.GetPooled(m_Value, value);
                evt.target = this;
                SetValueWithoutNotify(value);
                SendEvent(evt);

#if ENABLE_RUNTIME_DATA_BINDINGS
                NotifyPropertyChanged(in valueProperty);
#endif
            }
        }

        internal override void OnDaySelected(EventBase evt)
        {
            if (evt.target is not VisualElement { userData: Date date })
                return;

            evt.StopPropagation();
            value = date;
        }

        internal override bool IsSelectedDate(Date date)
        {
            return date == value;
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Class used to create instances of <see cref="DatePicker"/> from UXML.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<DatePicker, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="DatePicker"/>.
        /// </summary>
        public new class UxmlTraits : BaseDatePicker.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription
            {
                name = "value",
                defaultValue = Date.now.ToString()
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var datePicker = (DatePicker)ve;

                var value = m_Value.GetValueFromBag(bag, cc);
                if (m_Value.TryGetValueFromBag(bag, cc, ref value) && DateTime.TryParse(value, out var date))
                    datePicker.value = new Date(date);
            }
        }
#endif
    }
}
