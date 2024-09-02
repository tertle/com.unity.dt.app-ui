using System;
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
    /// Slider UI element for floating point values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class SliderFloat : SliderBase<float>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId incrementFactorProperty = new BindingId(nameof(incrementFactor));

#endif

        const float k_DefaultIncrement = 0.1f;

        float m_IncrementFactor = k_DefaultIncrement;

        /// <summary>
        /// The increment factor used when the slider is interacted with using the keyboard.
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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SliderFloat()
        {
            formatStringOverride = UINumericFieldsUtils.k_FloatFieldFormatString;
            incrementFactor = k_DefaultIncrement;
            lowValueOverride = 0;
            highValueOverride = 100f;
            valueOverride = 0;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("low-value")]
#endif
        float lowValueOverride
        {
            get => lowValue;
            set => lowValue = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("high-value")]
#endif
        float highValueOverride
        {
            get => highValue;
            set => highValue = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("value")]
#endif
        float valueOverride
        {
            get => value;
            set => this.value = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("format-string")]
#endif
        string formatStringOverride
        {
            get => formatString;
            set => formatString = value;
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.ParseStringToValue"/>
        protected override bool ParseStringToValue(string strValue, out float v)
        {
            var ret = float.TryParse(strValue, out var val);
            v = val;
            return ret;
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.ParseValueToString"/>
        protected override string ParseValueToString(float val)
        {
            return val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
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
        /// Factory class to instantiate a <see cref="SliderFloat"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SliderFloat, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="SliderFloat"/>.
        /// </summary>
        public new class UxmlTraits : SliderBase<float>.UxmlTraits
        {
            readonly UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription
            {
                name = "high-value",
                defaultValue = 100
            };

            readonly UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription
            {
                name = "low-value",
                defaultValue = 0
            };

            readonly UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
            {
                name = "value",
                defaultValue = 0
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

                var el = (SliderFloat)ve;
                el.lowValue = m_LowValue.GetValueFromBag(bag, cc);
                el.highValue = m_HighValue.GetValueFromBag(bag, cc);
                el.value = m_Value.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
