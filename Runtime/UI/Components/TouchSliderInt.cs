using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// TouchSlider UI element for integer values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class TouchSliderInt : TouchSlider<int>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId incrementFactorProperty = nameof(incrementFactor);

#endif

        const int k_DefaultIncrementFactor = 1;

        int m_IncrementFactor = k_DefaultIncrementFactor;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TouchSliderInt()
        {
            formatString = UINumericFieldsUtils.k_IntFieldFormatString;
            incrementFactor = k_DefaultIncrementFactor;

            lowValue = 0;
            highValue = 1;
            value = 0;
        }

        /// <summary>
        /// The increment factor for the slider.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
        [Min(1)]
#endif
        public int incrementFactor
        {
            get => m_IncrementFactor;
            set
            {
                var changed = m_IncrementFactor != value;
                m_IncrementFactor = Mathf.Max(1, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in incrementFactorProperty);
#endif
            }
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("low-value")]
        int lowValueOverride
        {
            get => lowValue;
            set => lowValue = value;
        }

        [UxmlAttribute("high-value")]
        int highValueOverride
        {
            get => highValue;
            set => highValue = value;
        }

        [UxmlAttribute("value")]
        int valueOverride
        {
            get => value;
            set => this.value = value;
        }

        [UxmlAttribute("format-string")]
        string formatStringOverride
        {
            get => formatString;
            set => formatString = value;
        }
#endif

        /// <inheritdoc cref="BaseSlider{TValueType}.ParseStringToValue"/>
        protected override bool ParseStringToValue(string strValue, out int val)
        {
            var ret = UINumericFieldsUtils.StringToLong(strValue, out var v);
            val = ret ? UINumericFieldsUtils.ClampToInt(v) : value;
            return ret;
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.ParseValueToString"/>
        protected override string ParseValueToString(int val)
        {
            return val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.ParseRawValueToString"/>
        protected override string ParseRawValueToString(int val)
        {
            return val.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.SliderLerpUnclamped"/>
        protected override int SliderLerpUnclamped(int a, int b, float interpolant)
        {
            return Mathf.RoundToInt(Mathf.LerpUnclamped(a, b, interpolant));
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.SliderNormalizeValue"/>
        protected override float SliderNormalizeValue(int currentValue, int lowerValue, int higherValue)
        {
            return Mathf.InverseLerp(lowerValue,higherValue, currentValue);
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.Increment"/>
        protected override int Increment(int val)
        {
            return val + incrementFactor;
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.Decrement"/>
        protected override int Decrement(int val)
        {
            return val - incrementFactor;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="TouchSliderInt"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<TouchSliderInt, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="TouchSliderInt"/>.
        /// </summary>
        public new class UxmlTraits : TouchSlider<int>.UxmlTraits
        {
            readonly UxmlStringAttributeDescription m_HighValue = new UxmlStringAttributeDescription { name = "high-value", defaultValue = "1" };

            readonly UxmlStringAttributeDescription m_LowValue = new UxmlStringAttributeDescription { name = "low-value", defaultValue = "0" };

            readonly UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription { name = "value", defaultValue = "0" };

            /// <summary>
            /// Initializes the VisualElement from the UXML attributes.
            /// </summary>
            /// <param name="ve"> The <see cref="VisualElement"/> to initialize.</param>
            /// <param name="bag"> The <see cref="IUxmlAttributes"/> bag to use to initialize the <see cref="VisualElement"/>.</param>
            /// <param name="cc"> The <see cref="CreationContext"/> to use to initialize the <see cref="VisualElement"/>.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var elem = (TouchSliderInt)ve;

                var val = elem.ParseStringToValue(m_Value.GetValueFromBag(bag, cc), out var convertedVal) ? convertedVal : 0;
                var highVal = elem.ParseStringToValue(m_HighValue.GetValueFromBag(bag, cc), out var convertedHighVal) ? convertedHighVal : 1;
                var lowVal = elem.ParseStringToValue(m_LowValue.GetValueFromBag(bag, cc), out var convertedLowVal) ? convertedLowVal : 0;

                elem.highValue = highVal;
                elem.lowValue = lowVal;
                elem.SetValueWithoutNotify(val);
            }
        }

#endif
    }

}
