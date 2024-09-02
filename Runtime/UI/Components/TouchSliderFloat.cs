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
    /// TouchSlider UI element for floating point values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class TouchSliderFloat : TouchSlider<float>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId incrementFactorProperty = nameof(incrementFactor);

#endif

        const float k_DefaultIncrement = 0.1f;

        float m_IncrementFactor = k_DefaultIncrement;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TouchSliderFloat()
        {
            formatString = UINumericFieldsUtils.k_FloatFieldFormatString;
            incrementFactor = k_DefaultIncrement;

            lowValue = 0f;
            highValue = 1f;
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
        [Min(0.0001f)]
#endif
        public float incrementFactor
        {
            get => m_IncrementFactor;
            set
            {
                var changed = !Mathf.Approximately(m_IncrementFactor, value);
                m_IncrementFactor = Mathf.Max(0.0001f, value);

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in incrementFactorProperty);
#endif
            }
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("low-value")]
        float lowValueOverride
        {
            get => lowValue;
            set => lowValue = value;
        }

        [UxmlAttribute("high-value")]
        float highValueOverride
        {
            get => highValue;
            set => highValue = value;
        }

        [UxmlAttribute("value")]
        float valueOverride
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
        protected override bool ParseStringToValue(string strValue, out float val)
        {
            var ret = UINumericFieldsUtils.StringToDouble(strValue, out var d);
            var f  = ret ? UINumericFieldsUtils.ClampToFloat(d) : value;
            val = f;
            return ret;
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.ParseValueToString"/>
        protected override string ParseValueToString(float val)
        {
            return val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.ParseRawValueToString"/>
        protected override string ParseRawValueToString(float val)
        {
            return val.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.SliderLerpUnclamped"/>
        protected override float SliderLerpUnclamped(float a, float b, float interpolant)
        {
            return Mathf.LerpUnclamped(a, b, interpolant);
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.SliderNormalizeValue"/>
        protected override float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
        {
            return Mathf.InverseLerp(lowerValue, higherValue, currentValue);
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.Increment"/>
        protected override float Increment(float val)
        {
            return val + incrementFactor;
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.Decrement"/>
        protected override float Decrement(float val)
        {
            return val - incrementFactor;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="TouchSliderFloat"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<TouchSliderFloat, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="TouchSliderFloat"/>.
        /// </summary>
        public new class UxmlTraits : TouchSlider<float>.UxmlTraits
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

                var elem = (TouchSliderFloat)ve;

                var val = elem.ParseStringToValue(m_Value.GetValueFromBag(bag, cc), out var convertedVal) ? convertedVal : 0;
                var highVal = elem.ParseStringToValue(m_HighValue.GetValueFromBag(bag, cc), out var convertedHighVal) ? convertedHighVal : 1f;
                var lowVal = elem.ParseStringToValue(m_LowValue.GetValueFromBag(bag, cc), out var convertedLowVal) ? convertedLowVal : 0;

                elem.highValue = highVal;
                elem.lowValue = lowVal;
                elem.SetValueWithoutNotify(val);
            }
        }

#endif
    }
}
