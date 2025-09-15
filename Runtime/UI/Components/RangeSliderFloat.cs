using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
#if ENABLE_RUNTIME_DATA_BINDINGS
using Unity.Properties;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// A simple comparer for Vector2 that compares the x and y values lexicographically.
    /// </summary>
    public class Vector2LexicographicalComparer : IComparer<Vector2>
    {
        /// <summary>
        /// Compares two Vector2 values lexicographically.
        /// </summary>
        /// <param name="v1"> The first Vector2 value to compare.</param>
        /// <param name="v2"> The second Vector2 value to compare.</param>
        /// <returns> A value indicating the relative order of the two Vector2 values.</returns>
        public int Compare(Vector2 v1, Vector2 v2)
        {
            var xComparison = v1.x.CompareTo(v2.x);
            return xComparison != 0 ? xComparison : v1.y.CompareTo(v2.y);
        }
    }

    /// <summary>
    /// Range Slider UI element for floating point values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class RangeSliderFloat : Slider<Vector2, float, Vector2Field>
    {
        const float k_DefaultStep = 0.1f;

        const float k_DefaultShiftStep = 1f;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RangeSliderFloat()
        {
            comparer = new Vector2LexicographicalComparer();
            formatString = UINumericFieldsUtils.k_FloatFieldFormatString;
            stepOverride = k_DefaultStep;
            shiftStepOverride = k_DefaultShiftStep;
            lowValueOverride = 0;
            highValueOverride = 100;
            minValueOverride = 0;
            maxValueOverride = 100;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("step")]
#endif
        float stepOverride
        {
            get => step;
            set => step = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("shift-step")]
#endif
        float shiftStepOverride
        {
            get => shiftStep;
            set => shiftStep = value;
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

        /// <summary>
        /// The low part of the range value.
        /// </summary>
        public float minValue
        {
            get => m_Value.x;
            set => this.value = new Vector2(value, m_Value.y);
        }

        /// <summary>
        /// The high part of the range value.
        /// </summary>
        public float maxValue
        {
            get => m_Value.y;
            set => this.value = new Vector2(m_Value.x, value);
        }

        /// <inheritdoc />
        protected override int thumbCount => 2;

        /// <inheritdoc />
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

        /// <inheritdoc />
        protected override string ParseValueToString(Vector2 val)
        {
            return $"[{ParseSubValueToString(val.x)} - {ParseSubValueToString(val.y)}]";
        }

        /// <inheritdoc />
        protected override string ParseSubValueToString(float val)
        {
            if (formatFunction != null)
                return formatFunction(val);

            return val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc />
        protected override float SliderLerpUnclamped(float a, float b, float interpolant)
        {
            return Mathf.LerpUnclamped(a, b, interpolant);
        }

        /// <inheritdoc />
        protected override float SliderNormalizeValue(float currentValue, float lowerValue, float higherValue)
        {
            return Mathf.InverseLerp(lowerValue, higherValue, currentValue);
        }

        /// <inheritdoc />
        protected override float Mad(int m, float a, float b)
        {
            return m * a + b;
        }

        /// <inheritdoc />
        protected override int GetStepCount(float stepValue)
        {
            return Mathf.FloorToInt((highValue - lowValue) / stepValue) + 1;
        }

        /// <inheritdoc />
        protected override float ClampThumb(float x, float min, float max)
        {
            return Mathf.Clamp(x, min, max);
        }

        /// <inheritdoc cref="BaseSlider{TValue,TScalar}.GetValueFromScalarValues"/>
        protected override Vector2 GetValueFromScalarValues(Span<float> values)
        {
            return new Vector2(values[0], values[1]);
        }

        /// <inheritdoc cref="BaseSlider{TValue,TScalar}.GetScalarValuesFromValue"/>
        protected override void GetScalarValuesFromValue(Vector2 v, Span<float> values)
        {
            values[0] = v.x;
            values[1] = v.y;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="RangeSliderFloat"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<RangeSliderFloat, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="RangeSliderFloat"/>.
        /// </summary>
        public new class UxmlTraits : Slider<Vector2, float, Vector2Field>.UxmlTraits
        {

            readonly UxmlFloatAttributeDescription m_Step = new UxmlFloatAttributeDescription
            {
                name = "step",
                defaultValue = k_DefaultStep
            };

            readonly UxmlFloatAttributeDescription m_ShiftStep = new UxmlFloatAttributeDescription
            {
                name = "shift-step",
                defaultValue = k_DefaultShiftStep
            };

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
                el.step = m_Step.GetValueFromBag(bag, cc);
                el.shiftStep = m_ShiftStep.GetValueFromBag(bag, cc);
                el.lowValue = m_LowValue.GetValueFromBag(bag, cc);
                el.highValue = m_HighValue.GetValueFromBag(bag, cc);
                el.value = new Vector2(m_MinValue.GetValueFromBag(bag, cc), m_MaxValue.GetValueFromBag(bag, cc));
            }
        }
#endif
    }
}
