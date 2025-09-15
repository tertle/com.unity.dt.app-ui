using UnityEngine;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Utility class for the numeric fields.
    /// </summary>
    static class UINumericFieldsUtils
    {
        public const string k_AllowedCharactersForFloat = "inftynaeINFTYNAE0123456789.,-*/+%^()cosqrludxvRL=pP#";
        public const string k_AllowedCharactersForInt = "0123456789-*/+%^()cosintaqrtelfundxvRL,=pPI#";
        public const string k_DoubleFieldFormatString = "R";
        public const string k_FloatFieldFormatString = "g7";
        public const string k_IntFieldFormatString = "#######0";

        /// <summary>
        /// Convert a string to a double by evaluating it as an expression.
        /// </summary>
        /// <param name="str"> The string to convert.</param>
        /// <param name="value"> The converted value.</param>
        /// <returns> True if the conversion was successful, False otherwise.</returns>
        public static bool StringToDouble(string str, out double value)
        {
            var lowered = str.ToLower();
            if (lowered == "inf" || lowered == "infinity")
                value = double.PositiveInfinity;
            else if (lowered == "-inf" || lowered == "-infinity")
                value = double.NegativeInfinity;
            else if (lowered == "nan")
                value = double.NaN;
            else
                return ExpressionEvaluator.Evaluate(str, out value);

            return true;
        }

        /// <summary>
        /// Convert a string to a float by evaluating it as an expression.
        /// </summary>
        /// <param name="str"> The string to convert.</param>
        /// <param name="value"> The converted value.</param>
        /// <returns> True if the conversion was successful, False otherwise.</returns>
        public static bool StringToLong(string str, out long value)
        {
            return ExpressionEvaluator.Evaluate(str, out value);
        }

        internal static int ClampToInt(long value)
        {
            if (value < int.MinValue)
                return int.MinValue;

            if (value > int.MaxValue)
                return int.MaxValue;

            return (int)value;
        }

        /// <summary>
        /// Clamp a double to a float.
        /// </summary>
        /// <param name="value"> The double value to clamp.</param>
        /// <returns> The clamped float value.</returns>
        public static float ClampToFloat(double value)
        {
            if (value < float.MinValue)
                return float.MinValue;

            if (value > float.MaxValue)
                return float.MaxValue;

            return (float)value;
        }

        /// <summary>
        /// Check if the string formatting code is a percent format.
        /// </summary>
        /// <param name="formatString"> The string formatting code.</param>
        /// <returns> True if the string formatting code is a percent format, False otherwise.</returns>
        public static bool IsPercentFormatString(string formatString)
        {
            if (string.IsNullOrEmpty(formatString))
                return false;

            if (formatString.ToUpperInvariant().StartsWith("P"))
                return !formatString.Contains(".");

            var pCount = 0;
            var dCount = 0;
            foreach (var c in formatString)
            {
                switch (c)
                {
                    case '%':
                        pCount++;
                        break;
                    case '#' or '0':
                        dCount++;
                        break;
                }
            }

            return pCount == 1 && dCount > 0 && (formatString.StartsWith("%") || formatString.EndsWith("%"));
        }
    }
}
