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
    /// Slider UI element for integer values.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class SliderInt : Slider<int,int,IntField>
    {
#if ENABLE_RUNTIME_DATA_BINDINGS

#endif

        const int k_DefaultStep = 1;

        const int k_DefaultShiftStep = 10;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SliderInt()
        {
            formatStringOverride = UINumericFieldsUtils.k_IntFieldFormatString;
            stepOverride = k_DefaultStep;
            shiftStepOverride = k_DefaultShiftStep;
            lowValueOverride = 0;
            highValueOverride = 100;
            valueOverride = 0;
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

        /// <inheritdoc />
        protected override int thumbCount => 1;

        /// <inheritdoc />
        protected override bool ParseStringToValue(string strValue, out int v)
        {
            var ret = int.TryParse(strValue, out var val);
            v = val;
            return ret;
        }

        /// <inheritdoc />
        protected override string ParseValueToString(int val)
        {
            if (formatFunction != null)
                return formatFunction(val);

            if (UINumericFieldsUtils.IsPercentFormatString(formatString))
                Debug.LogWarning("Percent format string is not supported for integer values.\n" +
                    "Please use a SliderFloat instead.");

            return val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc />
        protected override string ParseSubValueToString(int val) => ParseValueToString(val);

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
        protected override int GetStepCount(int stepValue)
        {
            return (highValue - lowValue) / stepValue + 1;
        }

        /// <inheritdoc />
        protected override int GetValueFromScalarValues(Span<int> values)
        {
            return values[0];
        }

        /// <inheritdoc />
        protected override int ClampThumb(int x, int min, int max)
        {
            return Mathf.Clamp(x, min, max);
        }

        /// <inheritdoc />
        protected override void GetScalarValuesFromValue(int v, Span<int> values)
        {
            values[0] = v;
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="SliderInt"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<SliderInt, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="SliderInt"/>.
        /// </summary>
        public new class UxmlTraits : Slider<int,int,IntField>.UxmlTraits
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
                el.step = m_Step.GetValueFromBag(bag, cc);
                el.shiftStep = m_ShiftStep.GetValueFromBag(bag, cc);
                el.lowValue = m_LowValue.GetValueFromBag(bag, cc);
                el.highValue = m_HighValue.GetValueFromBag(bag, cc);
                el.value = m_Value.GetValueFromBag(bag, cc);
            }
        }

#endif
    }
}
