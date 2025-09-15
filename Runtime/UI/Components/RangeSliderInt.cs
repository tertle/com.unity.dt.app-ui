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
    /// A simple comparer for Vector2Int that compares the x and y values lexicographically.
    /// </summary>
    public class Vector2IntLexicographicalComparer : IComparer<Vector2Int>
    {
        /// <summary>
        /// Compares two Vector2Int values lexicographically.
        /// </summary>
        /// <param name="v1"> The first Vector2Int value to compare.</param>
        /// <param name="v2"> The second Vector2Int value to compare.</param>
        /// <returns> A value indicating the relative order of the two Vector2Int values.</returns>
        public int Compare(Vector2Int v1, Vector2Int v2)
        {
            var xComparison = v1.x.CompareTo(v2.x);
            return xComparison != 0 ? xComparison : v1.y.CompareTo(v2.y);
        }
    }

    /// <summary>
    /// Range Slider UI element for integer values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class RangeSliderInt : Slider<Vector2Int, int, Vector2IntField>
    {
        const int k_DefaultStep = 1;

        const int k_DefaultShiftStep = 10;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RangeSliderInt()
        {
            comparer = new Vector2IntLexicographicalComparer();
            formatString = UINumericFieldsUtils.k_IntFieldFormatString;
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
        int stepOverride
        {
            get => step;
            set => step = value;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("shift-step")]
#endif
        int shiftStepOverride
        {
            get => shiftStep;
            set => shiftStep = value;
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

        /// <summary>
        /// The low part of the range value.
        /// </summary>
        public int minValue
        {
            get => m_Value.x;
            set => this.value = new Vector2Int(value, m_Value.y);
        }

        /// <summary>
        /// The high part of the range value.
        /// </summary>
        public int maxValue
        {
            get => m_Value.y;
            set => this.value = new Vector2Int(m_Value.x, value);
        }

        /// <inheritdoc />
        protected override int thumbCount => 2;

        /// <inheritdoc />
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

        /// <inheritdoc />
        protected override string ParseValueToString(Vector2Int val)
        {
            if (UINumericFieldsUtils.IsPercentFormatString(formatString))
                Debug.LogWarning("Percent format string is not supported for integer values.\n" +
                    "Please use a RangeSliderFloat instead.");

            return $"[{ParseSubValueToString(val.x)} - {ParseSubValueToString(val.y)}]";
        }

        /// <inheritdoc />
        protected override string ParseSubValueToString(int val)
        {
            if (formatFunction != null)
                return formatFunction(val);

            return val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc />
        protected override int SliderLerpUnclamped(int a, int b, float interpolant)
        {
            return Mathf.RoundToInt(Mathf.LerpUnclamped(a, b, interpolant));
        }

        /// <inheritdoc />
        protected override float SliderNormalizeValue(int currentValue, int lowerValue, int higherValue)
        {
            return Mathf.InverseLerp(lowerValue, higherValue, currentValue);
        }

        /// <inheritdoc />
        protected override int Mad(int m, int a, int b)
        {
            return m * a + b;
        }

        /// <inheritdoc />
        protected override int ClampThumb(int x, int min, int max)
        {
            return Mathf.Clamp(x, min, max);
        }

        /// <inheritdoc cref="BaseSlider{TValue,TScalar}.GetValueFromScalarValues"/>
        protected override Vector2Int GetValueFromScalarValues(Span<int> values)
        {
            return new Vector2Int(values[0], values[1]);
        }

        /// <inheritdoc cref="BaseSlider{TValue,TScalar}.GetScalarValuesFromValue"/>
        protected override void GetScalarValuesFromValue(Vector2Int v, Span<int> values)
        {
            values[0] = v.x;
            values[1] = v.y;
        }

        /// <inheritdoc />
        protected override int GetStepCount(int stepValue)
        {
            return (highValue - lowValue) / stepValue + 1;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="RangeSliderInt"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<RangeSliderInt, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="RangeSliderInt"/>.
        /// </summary>
        public new class UxmlTraits : Slider<Vector2Int,int,Vector2IntField>.UxmlTraits
        {
            readonly UxmlIntAttributeDescription m_Step = new UxmlIntAttributeDescription
            {
                name = "step",
                defaultValue = k_DefaultStep
            };

            readonly UxmlIntAttributeDescription m_ShiftStep = new UxmlIntAttributeDescription
            {
                name = "shift-step",
                defaultValue = k_DefaultShiftStep
            };

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
                el.step = m_Step.GetValueFromBag(bag, cc);
                el.shiftStep = m_ShiftStep.GetValueFromBag(bag, cc);
                el.lowValue = m_LowValue.GetValueFromBag(bag, cc);
                el.highValue = m_HighValue.GetValueFromBag(bag, cc);
                el.value = new Vector2Int(m_MinValue.GetValueFromBag(bag, cc), m_MaxValue.GetValueFromBag(bag, cc));
            }
        }

#endif
    }
}
