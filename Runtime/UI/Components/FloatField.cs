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
    /// Float Field UI element.
    /// </summary>
#if ENABLE_UXML_SERIALIZED_DATA
    [UxmlElement]
#endif
    public partial class FloatField : NumericalField<float>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FloatField()
        {
            formatString = UINumericFieldsUtils.k_FloatFieldFormatString;
        }

        /// <inheritdoc cref="NumericalField{T}.ParseStringToValue"/>
        protected override bool ParseStringToValue(string strValue, out float val)
        {
            var ret = UINumericFieldsUtils.StringToDouble(strValue, out var d);
            val = ret ? UINumericFieldsUtils.ClampToFloat(d) : value;
            return ret;
        }

        /// <inheritdoc cref="NumericalField{T}.ParseValueToString"/>
        protected override string ParseValueToString(float val)
        {
            if (formatFunction != null)
                return formatFunction(val);

            return val.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc cref="NumericalField{T}.ParseRawValueToString"/>
        protected override string ParseRawValueToString(float val)
        {
            return val.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <inheritdoc cref="NumericalField{T}.AreEqual"/>
        protected override bool AreEqual(float a, float b)
        {
            return Mathf.Approximately(a, b);
        }

        /// <inheritdoc cref="NumericalField{T}.Min(T,T)"/>
        protected override float Min(float a, float b)
        {
            return Mathf.Min(a, b);
        }

        /// <inheritdoc cref="NumericalField{T}.Max(T,T)"/>
        protected override float Max(float a, float b)
        {
            return Mathf.Max(a, b);
        }

        /// <inheritdoc cref="NumericalField{T}.Increment"/>
        protected override float Increment(float originalValue, float delta)
        {
            return originalValue + delta;
        }

        /// <inheritdoc cref="NumericalField{T}.GetIncrementFactor"/>
        protected override float GetIncrementFactor(float baseValue)
        {
            return 0.001f * (Mathf.Approximately(baseValue, 0) ? 1f : baseValue);
        }

#if ENABLE_UXML_TRAITS

        /// <summary>
        /// Factory class to instantiate a <see cref="FloatField"/> using the data read from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<FloatField, UxmlTraits> { }

        /// <summary>
        /// Class containing the <see cref="UxmlTraits"/> for the <see cref="FloatField"/>.
        /// </summary>
        public new class UxmlTraits : NumericalField<float>.UxmlTraits { }

#endif
    }
}
