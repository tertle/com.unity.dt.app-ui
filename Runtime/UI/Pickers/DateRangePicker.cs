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
    /// A date range picker control.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class DateRangePicker : BaseDatePicker, INotifyValueChanged<DateRange>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId valueProperty = new BindingId(nameof(value));

#endif
        DateRange m_Value;

        Date m_TempStartDate;

        bool m_IsTemporarilySelected;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DateRangePicker()
        {
            value = new DateRange(DateTime.Now, DateTime.Now.AddDays(1));
        }

        /// <summary>
        /// Set the value of the date picker without sending a change event.
        /// </summary>
        /// <param name="newValue"> The new value. </param>
        public void SetValueWithoutNotify(DateRange newValue)
        {
            m_IsTemporarilySelected = false;
            m_Value = newValue;
            GoTo(newValue.end);
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
        public DateRange value
        {
            get => m_Value;
            set
            {
                if (value == m_Value)
                    return;

                using var evt = ChangeEvent<DateRange>.GetPooled(m_Value, value);
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
            if (!m_IsTemporarilySelected)
            {
                m_TempStartDate = date;
                m_IsTemporarilySelected = true;
                RefreshUI();
            }
            else
            {
                m_IsTemporarilySelected = false;
                var startDate = m_TempStartDate;
                var endDate = date;
                if (startDate > endDate)
                {
                    startDate = date;
                    endDate = m_TempStartDate;
                }
                value = new DateRange(startDate, endDate);
            }
        }

        internal override bool IsStartDate(Date date)
        {
            return m_IsTemporarilySelected ? date == m_TempStartDate : date == m_Value.start;
        }

        internal override bool IsEndDate(Date date)
        {
            return !m_IsTemporarilySelected && date == m_Value.end;
        }

        internal override bool IsInRange(Date date)
        {
            return !m_IsTemporarilySelected && m_Value.Contains(date, includeStartAndEnd: false);
        }

#if ENABLE_UXML_TRAITS
        /// <summary>
        /// Class used to create instances of <see cref="DateRangePicker"/> from UXML.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<DateRangePicker, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="DateRangePicker"/>.
        /// </summary>
        public new class UxmlTraits : BaseDatePicker.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription
            {
                name = "value",
                defaultValue = new DateRange(DateTime.Now, DateTime.Now.AddDays(1)).ToString()
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var datePicker = (DateRangePicker)ve;

                var value = m_Value.GetValueFromBag(bag, cc);
                if (m_Value.TryGetValueFromBag(bag, cc, ref value) && DateRange.TryParse(value, out var dateRange))
                    datePicker.value = dateRange;
            }
        }
#endif
    }
}
