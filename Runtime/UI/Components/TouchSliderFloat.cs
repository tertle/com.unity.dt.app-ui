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
    /// TouchSlider UI element for floating point values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class TouchSliderFloat : TouchSlider<float>
    {
        const float k_DefaultStep = 0.1f;

        const float k_DefaultShiftStep = 1f;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TouchSliderFloat()
        {
            formatString = UINumericFieldsUtils.k_FloatFieldFormatString;
            step = k_DefaultStep;
            shiftStep = k_DefaultShiftStep;
            lowValue = 0f;
            highValue = 1f;
            value = 0;
        }

#if ENABLE_UXML_SERIALIZED_DATA
        [UxmlAttribute("step")]
        float stepOverride
        {
            get => step;
            set => step = value;
        }

        [UxmlAttribute("shift-step")]
        float shiftStepOverride
        {
            get => shiftStep;
            set => shiftStep = value;
        }

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
#endif

        /// <inheritdoc />
        protected override int thumbCount => 1;

        /// <inheritdoc />
        protected override bool ParseStringToValue(string strValue, out float val)
        {
            var ret = UINumericFieldsUtils.StringToDouble(strValue, out var d);
            var f  = ret ? UINumericFieldsUtils.ClampToFloat(d) : value;
            val = f;
            return ret;
        }

        /// <inheritdoc />
        protected override string ParseValueToString(float val)
        {
            return formatFunction != null
                ? formatFunction(val)
                : val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc />
        protected override string ParseSubValueToString(float val) => ParseValueToString(val);

        /// <inheritdoc />
        protected override string ParseRawValueToString(float val)
        {
            return val.ToString(CultureInfo.InvariantCulture.NumberFormat);
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

        /// <inheritdoc />
        protected override float GetValueFromScalarValues(Span<float> values)
        {
            return values[0];
        }

        /// <inheritdoc />
        protected override void GetScalarValuesFromValue(float v, Span<float> values)
        {
            values[0] = v;
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
            readonly UxmlFloatAttributeDescription m_Step = new UxmlFloatAttributeDescription { name = "step", defaultValue = k_DefaultStep };

            readonly UxmlFloatAttributeDescription m_ShiftStep = new UxmlFloatAttributeDescription { name = "shift-step", defaultValue = k_DefaultShiftStep };

            readonly UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription { name = "high-value", defaultValue = 1f };

            readonly UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription { name = "low-value", defaultValue = 0 };

            readonly UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription { name = "value", defaultValue = 0 };

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
