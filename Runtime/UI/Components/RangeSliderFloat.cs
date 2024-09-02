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
    /// Range Slider UI element for floating point values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class RangeSliderFloat : RangeSliderBase<Vector2, float>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

        internal static readonly BindingId incrementFactorProperty = new BindingId(nameof(incrementFactor));

#endif

        /// <summary>
        /// The increment factor used when the slider is interacted with using the keyboard.
        /// </summary>
#if ENABLE_RUNTIME_DATA_BINDINGS
        [CreateProperty]
#endif
#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute]
#endif
        public float incrementFactor
        {
            get => m_IncrementFactor;
            set
            {
                var changed = !Mathf.Approximately(m_IncrementFactor, value);
                m_IncrementFactor = value;

#if ENABLE_RUNTIME_DATA_BINDINGS
                if (changed)
                    NotifyPropertyChanged(in incrementFactorProperty);
#endif
            }
        }

        const float k_DefaultIncrementFactor = 0.1f;

        float m_IncrementFactor;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RangeSliderFloat()
        {
            formatStringFloatOverride = UINumericFieldsUtils.k_FloatFieldFormatString;
            incrementFactor = k_DefaultIncrementFactor;
            lowValueOverride = 0;
            highValueOverride = 100;
            minValueOverride = 0;
            maxValueOverride = 100;
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
        [UxmlAttribute("min-value")]
#endif
        float minValueOverride
        {
            get => minValue;
            set => minValue = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("max-value")]
#endif
        float maxValueOverride
        {
            get => maxValue;
            set => maxValue = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("format-string")]
#endif
        string formatStringFloatOverride
        {
            get => formatString;
            set => formatString = value;
        }

        /// <inheritdoc cref="BaseSlider{TValueType}.ParseStringToValue"/>
        protected override bool ParseStringToValue(string strValue, out Vector2 val)
        {
            var strValues = strValue.Split(" - ");
            var xStr = strValues[0];
            var yStr = strValues[1];
            var xRet = float.TryParse(xStr, out var val1);
            var yRet = float.TryParse(yStr, out var val2);
            val = new Vector2(val1, val2);
            return xRet && yRet;
        }

        /// <inheritdoc cref="BaseSlider{Vector2,Single}.ParseValueToString"/>
        protected override string ParseValueToString(Vector2 val)
        {
            return $"[{val.x.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat)} - " +
                $"{val.y.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat)}]";
        }

        /// <inheritdoc cref="BaseSlider{Vector2,Single}.SliderLerpUnclamped"/>
        protected override Vector2 SliderLerpUnclamped(float a, float b, float interpolant)
        {
            throw new InvalidOperationException("Cannot lerp between two integers and return a Vector2.");
        }

        /// <inheritdoc cref="BaseSlider{Vector2,Single}.SliderNormalizeValue"/>
        protected override float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
        {
            return Mathf.InverseLerp(lowerValue, higherValue, currentValue);
        }

        /// <inheritdoc cref="BaseSlider{Vector2,Single}.Increment"/>
        protected override float Increment(float val)
        {
            return val + incrementFactor;
        }

        /// <inheritdoc cref="BaseSlider{Vector2,Single}.Decrement"/>
        protected override float Decrement(float val)
        {
            return val - incrementFactor;
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.minValue"/>
        public override float minValue
        {
            get => m_Value.x;
            set => this.value = new Vector2(value, m_Value.y);
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.maxValue"/>
        public override float maxValue
        {
            get => m_Value.y;
            set => this.value = new Vector2(m_Value.x, value);
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.MakeRangeValue"/>
        protected override Vector2 MakeRangeValue(float minValue, float maxValue)
        {
            return new Vector2(minValue, maxValue);
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.GetMinValue"/>
        protected override float GetMinValue(Vector2 rangeValue)
        {
            return rangeValue.x;
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.GetMaxValue"/>
        protected override float GetMaxValue(Vector2 rangeValue)
        {
            return rangeValue.y;
        }

        /// <inheritdoc cref="M:Unity.AppUI.UI.RangeSliderBase`2.GetClampedValue(`1,`1,`1)"/>
        protected override float GetClampedValue(float value, float lowerValue, float higherValue)
        {
            return Mathf.Clamp(value, lowerValue, higherValue);
        }

        /// <inheritdoc cref="RangeSliderBase{TRangeType,TValueType}.LerpUnclamped"/>
        protected override float LerpUnclamped(float a, float b, float interpolant)
        {
            return Mathf.LerpUnclamped(a, b, interpolant);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="RangeSliderFloat"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<RangeSliderFloat, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="RangeSliderFloat"/>.
        /// </summary>
        public new class UxmlTraits : RangeSliderBase<Vector2, float>.UxmlTraits
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

            readonly UxmlFloatAttributeDescription m_MinValue = new UxmlFloatAttributeDescription
            {
                name = "min-value",
                defaultValue = 0
            };

            readonly UxmlFloatAttributeDescription m_MaxValue = new UxmlFloatAttributeDescription
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

                var el = (RangeSliderFloat)ve;
                el.lowValue = m_LowValue.GetValueFromBag(bag, cc);
                el.highValue = m_HighValue.GetValueFromBag(bag, cc);
                el.value = new Vector2(m_MinValue.GetValueFromBag(bag, cc), m_MaxValue.GetValueFromBag(bag, cc));
            }
        }
#endif
    }
}
