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
    /// Slider UI element for integer values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class SliderInt : SliderBase<int>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId incrementFactorProperty = new BindingId(nameof(incrementFactor));

#endif

        const int k_DefaultIncrement = 1;

        int m_IncrementFactor = k_DefaultIncrement;

        /// <summary>
        /// The increment factor used when the slider is interacted with using the keyboard.
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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SliderInt()
        {
            formatStringOverride = UINumericFieldsUtils.k_IntFieldFormatString;
            incrementFactor = k_DefaultIncrement;
            lowValueOverride = 0;
            highValueOverride = 100;
            valueOverride = 0;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("low-value")]
#endif
        int lowValueOverride
        {
            get => lowValue;
            set => lowValue = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("high-value")]
#endif
        int highValueOverride
        {
            get => highValue;
            set => highValue = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("value")]
#endif
        int valueOverride
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
        protected override bool ParseStringToValue(string strValue, out int v)
        {
            var ret = int.TryParse(strValue, out var val);
            v = val;
            return ret;
        }

        /// <inheritdoc cref="BaseSlider{TValueType,TValueType}.ParseValueToString"/>
        protected override string ParseValueToString(int val)
        {
            if (UINumericFieldsUtils.IsPercentFormatString(formatString))
                Debug.LogWarning("Percent format string is not supported for integer values.\n" +
                    "Please use a SliderFloat instead.");

            return val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.SliderLerpUnclamped"/>
        protected override int SliderLerpUnclamped(int a, int b, float interpolant)
        {
            return Mathf.RoundToInt(Mathf.LerpUnclamped(a, b, interpolant));
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.SliderNormalizeValue"/>
        protected override float SliderNormalizeValue(int currentValue, int lowerValue, int higherValue)
        {
            return Mathf.InverseLerp(lowerValue, higherValue, currentValue);
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
        /// Factory class to instantiate a <see cref="SliderInt"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SliderInt, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="SliderInt"/>.
        /// </summary>
        public new class UxmlTraits : SliderBase<int>.UxmlTraits
        {
            readonly UxmlIntAttributeDescription m_HighValue = new UxmlIntAttributeDescription
            {
                name = "high-value",
                defaultValue = 100
            };

            readonly UxmlIntAttributeDescription m_LowValue = new UxmlIntAttributeDescription
            {
                name = "low-value",
                defaultValue = 0
            };

            readonly UxmlIntAttributeDescription m_Value = new UxmlIntAttributeDescription
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

                var el = (SliderInt)ve;
                el.lowValue = m_LowValue.GetValueFromBag(bag, cc);
                el.highValue = m_HighValue.GetValueFromBag(bag, cc);
                el.value = m_Value.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
