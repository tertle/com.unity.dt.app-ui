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
    /// Range Slider UI element for integer values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class RangeSliderInt : RangeSliderBase<Vector2Int, int>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId incrementFactorProperty = new BindingId(nameof(incrementFactor));

#endif

        const int k_DefaultIncrementFactor = 1;

        int m_IncrementFactor = k_DefaultIncrementFactor;

        /// <summary>
        /// The increment factor used when the slider is interacted with using the keyboard.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public int incrementFactor
        {
            get => m_IncrementFactor;
            set
            {
                var changed = m_IncrementFactor != value;
                m_IncrementFactor = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in incrementFactorProperty);
#endif
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RangeSliderInt()
        {
            formatStringIntOverride = UINumericFieldsUtils.k_IntFieldFormatString;
            incrementFactor = k_DefaultIncrementFactor;
            lowValueOverride = 0;
            highValueOverride = 100;
            minValueOverride = 0;
            maxValueOverride = 100;
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
        [UxmlAttribute("min-value")]
#endif
        int minValueOverride
        {
            get => minValue;
            set => minValue = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("max-value")]
#endif
        int maxValueOverride
        {
            get => maxValue;
            set => maxValue = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("format-string")]
#endif
        string formatStringIntOverride
        {
            get => formatString;
            set => formatString = value;
        }

        /// <inheritdoc cref="BaseSlider{Vector2Int,Integer}.ParseStringToValue"/>
        protected override bool ParseStringToValue(string strValue, out Vector2Int val)
        {
            var strValues = strValue.Split(" - ");
            var xStr = strValues[0];
            var yStr = strValues[1];
            var xRet = int.TryParse(xStr, out var val1);
            var yRet = int.TryParse(yStr, out var val2);
            val = new Vector2Int(val1, val2);
            return xRet && yRet;
        }

        /// <inheritdoc cref="BaseSlider{Vector2Int,Integer}.ParseValueToString"/>
        protected override string ParseValueToString(Vector2Int val)
        {
            if (UINumericFieldsUtils.IsPercentFormatString(formatString))
                Debug.LogWarning("Percent format string is not supported for integer values.\n" +
                    "Please use a RangeSliderFloat instead.");

            return val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc cref="BaseSlider{Vector2Int,Integer}.SliderLerpUnclamped"/>
        protected override Vector2Int SliderLerpUnclamped(int a, int b, float interpolant)
        {
            throw new InvalidOperationException("Cannot lerp between two integers and return a Vector2Int.");
        }

        /// <inheritdoc cref="BaseSlider{Vector2Int,Integer}.SliderNormalizeValue"/>
        protected override float SliderNormalizeValue(int currentValue, int lowerValue, int higherValue)
        {
            return Mathf.InverseLerp(lowerValue, higherValue, currentValue);
        }

        /// <inheritdoc cref="BaseSlider{Vector2Int,Integer}.Increment"/>
        protected override int Increment(int val)
        {
            return val + incrementFactor;
        }

        /// <inheritdoc cref="BaseSlider{Vector2Int,Integer}.Decrement"/>
        protected override int Decrement(int val)
        {
            return val - incrementFactor;
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.minValue"/>
        public override int minValue
        {
            get => m_Value.x;
            set => this.value = new Vector2Int(value, m_Value.y);
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.maxValue"/>
        public override int maxValue
        {
            get => m_Value.y;
            set => this.value = new Vector2Int(m_Value.x, value);
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.MakeRangeValue"/>
        protected override Vector2Int MakeRangeValue(int minValue, int maxValue)
        {
            return new Vector2Int(minValue, maxValue);
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.GetMinValue"/>
        protected override int GetMinValue(Vector2Int rangeValue)
        {
            return rangeValue.x;
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.GetMaxValue"/>
        protected override int GetMaxValue(Vector2Int rangeValue)
        {
            return rangeValue.y;
        }

        /// <inheritdoc cref="M:Unity.AppUI.UI.RangeSliderBase`2.GetClampedValue(`1,`1,`1)"/>
        protected override int GetClampedValue(int value, int lowerValue, int higherValue)
        {
            return Mathf.Clamp(value, lowerValue, higherValue);
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.LerpUnclamped"/>
        protected override int LerpUnclamped(int a, int b, float interpolant)
        {
            return Mathf.RoundToInt(Mathf.LerpUnclamped(a, b, interpolant));
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="RangeSliderInt"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<RangeSliderInt, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="RangeSliderInt"/>.
        /// </summary>
        public new class UxmlTraits : RangeSliderBase<Vector2Int, int>.UxmlTraits
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

            readonly UxmlIntAttributeDescription m_MinValue = new UxmlIntAttributeDescription
            {
                name = "min-value",
                defaultValue = 0
            };

            readonly UxmlIntAttributeDescription m_MaxValue = new UxmlIntAttributeDescription
            {
                name = "max-value",
                defaultValue = 100
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

                var el = (RangeSliderInt)ve;
                el.lowValue = m_LowValue.GetValueFromBag(bag, cc);
                el.highValue = m_HighValue.GetValueFromBag(bag, cc);
                el.value = new Vector2Int(m_MinValue.GetValueFromBag(bag, cc), m_MaxValue.GetValueFromBag(bag, cc));
            }
        }

#endif
    }
}
