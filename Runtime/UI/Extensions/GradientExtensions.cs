using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Extensions for the Gradient class.
    /// </summary>
    public static class GradientExtensions
    {
        /// <summary>
        /// Try to parse a string to a Gradient.
        /// </summary>
        /// <param name="value"> The string to parse. </param>
        /// <param name="v"> The parsed Gradient. </param>
        /// <returns> True if the string was successfully parsed, false otherwise. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown if the string is null. </exception>
        public static bool TryParse(string value, out Gradient v)
        {
            if (value == null)
                throw new ArgumentOutOfRangeException(nameof(value), "Cannot parse a null string to a Gradient.");

            v = default;

            var index = value.IndexOf(":");
            if (index == -1)
                return false;

            var mode = value[..index];
            var keys = value.Remove(0, index + 1);
            var parsedMode = Enum.TryParse<GradientMode>(mode, out var gradientMode);
            if (!parsedMode)
                return false;

            var gradientItems = keys.Split('+');
            if (gradientItems.Length != 2)
                return false;

            using var poolColor = ListPool<GradientColorKey>.Get(out var gradientColorKeys);
            var colorKeyItems = gradientItems[0].Split(';');
            foreach (var item in colorKeyItems)
            {
                if (item.Length < 3)
                    return false;

                var valueList = item.Trim().Substring(1, item.Length - 2); // Remove brackets
                var colorKeys = valueList.Split(',');
                if (colorKeys.Length is <= 0 or > 2)
                    return false;

                var success = float.TryParse(colorKeys[0], out var time);
                success &= ColorUtility.TryParseHtmlString(colorKeys[1], out var color);

                if (!success)
                    return false;

                gradientColorKeys.Add(new GradientColorKey(color, time));
            }

            using var poolAlpha = ListPool<GradientAlphaKey>.Get(out var gradientAlphaKeys);
            var alphaKeyItems = gradientItems[1].Split(';');
            foreach (var item in alphaKeyItems)
            {
                if (item.Length < 3)
                    return false;

                var valueList = item.Trim().Substring(1, item.Length - 2); // Remove brackets
                var alphaKeys = valueList.Split(',');
                if (alphaKeys.Length is <= 0 or > 2)
                    return false;

                var success = float.TryParse(alphaKeys[0], out var time);
                success &= float.TryParse(alphaKeys[1], out var alpha);

                if (!success)
                    return false;

                gradientAlphaKeys.Add(new GradientAlphaKey(alpha, time));
            }

            var gradient = new Gradient { mode = gradientMode};
            gradient.SetKeys(gradientColorKeys.ToArray(), gradientAlphaKeys.ToArray());

            v = gradient;
            return true;
        }
    }
}
