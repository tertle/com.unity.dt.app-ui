using System;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Capitalizes the first letter of a string.
        /// </summary>
        /// <param name="arg"> The string to capitalize. </param>
        /// <returns> The capitalized string. </returns>
        public static string Capitalize(this string arg)
        {
            if (string.IsNullOrEmpty(arg))
                return arg;

            return arg[0].ToString().ToUpper() + arg[1..];
        }
    }
}
