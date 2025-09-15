using System;
using System.Globalization;
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
        const int k_DefaultStep = 1;

        const int k_DefaultShiftStep = 10;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TouchSliderInt()
        {
            formatString = UINumericFieldsUtils.k_IntFieldFormatString;
            step = k_DefaultStep;
            shiftStep = k_DefaultShiftStep;
            lowValue = 0;
            highValue = 1;
            value = 0;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("step")]
        int stepOverride
        {
            get => step;
            set => step = value;
        }

        [UxmlAttribute("shift-step")]
        int shiftStepOverride
        {
            get => shiftStep;
            set => shiftStep = value;
        }

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
#endif

        /// <inheritdoc />
        protected override int thumbCount => 1;

        /// <inheritdoc />
        protected override bool ParseStringToValue(string strValue, out int val)
        {
            var ret = UINumericFieldsUtils.StringToLong(strValue, out var v);
            val = ret ? UINumericFieldsUtils.ClampToInt(v) : value;
            return ret;
        }

        /// <inheritdoc />
        protected override string ParseValueToString(int val)
        {
            return formatFunction != null
                ? formatFunction(val)
                : val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc />
        protected override string ParseSubValueToString(int val) => ParseValueToString(val);

        /// <inheritdoc />
        protected override string ParseRawValueToString(int val)
        {
            return val.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc />
        protected override int SliderLerpUnclamped(int a, int b, float interpolant)
        {
            return Mathf.RoundToInt(Mathf.LerpUnclamped(a, b, interpolant));
        }

        /// <inheritdoc />
        protected override float SliderNormalizeValue(int currentValue, int lowerValue, int higherValue)
        {
            return Mathf.InverseLerp(lowerValue,higherValue, currentValue);
        }

        /// <inheritdoc />
        protected override int Mad(int m, int a, int b)
        {
            return m * a + b;
        }

        /// <inheritdoc />
        protected override int GetStepCount(int stepValue)
        {
            return (highValue - lowValue) / stepValue + 1;
        }

        /// <inheritdoc />
        protected override int ClampThumb(int x, int min, int max)
        {
            return Mathf.Clamp(x, min, max);
        }

        /// <inheritdoc />
        protected override int GetValueFromScalarValues(Span<int> values)
        {
            return values[0];
        }

        /// <inheritdoc />
        protected override void GetScalarValuesFromValue(int v, Span<int> values)
        {
            values[0] = v;
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
            readonly UxmlIntAttributeDescription m_Step = new UxmlIntAttributeDescription { name = "step", defaultValue = 1 };

            readonly UxmlIntAttributeDescription m_ShiftStep = new UxmlIntAttributeDescription { name = "shift-step", defaultValue = 10 };

            readonly UxmlIntAttributeDescription m_HighValue = new UxmlIntAttributeDescription { name = "high-value", defaultValue = 1 };

            readonly UxmlIntAttributeDescription m_LowValue = new UxmlIntAttributeDescription { name = "low-value", defaultValue = 0 };

            readonly UxmlIntAttributeDescription m_Value = new UxmlIntAttributeDescription { name = "value", defaultValue = 0 };

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

                elem.step = m_Step.GetValueFromBag(bag, cc);
                elem.shiftStep = m_ShiftStep.GetValueFromBag(bag, cc);
                elem.highValue = m_HighValue.GetValueFromBag(bag, cc);
                elem.lowValue = m_LowValue.GetValueFromBag(bag, cc);
                elem.SetValueWithoutNotify(m_Value.GetValueFromBag(bag, cc));
            }
        }

#endif
    }

}
