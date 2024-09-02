#if ENABLE_UXML_SERIALIZED_DATA
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    // Generic Converters

    public abstract class OptionalConverter<T> : UxmlAttributeConverter<Optional<T>>
    {
        protected virtual bool TryParse(string value, out T v)
        {
            v = default;
            return false;
        }

        public override Optional<T> FromString(string value)
        {
            if (string.IsNullOrEmpty(value) || !TryParse(value, out var v))
                return Optional<T>.none;

            return v;
        }

        public override string ToString(Optional<T> value)
        {
            return value.IsSet ? value.Value.ToString() : string.Empty;
        }
    }

    public class OptionalValueConverter<T> : OptionalConverter<T>
        where T : IComparable, IComparable<T>, IFormattable { }

    public abstract class OptionalEnumConverter<T> : UxmlAttributeConverter<OptionalEnum<T>>
        where T : struct, Enum
    {
        public override OptionalEnum<T> FromString(string value)
        {
            if (string.IsNullOrEmpty(value) || !Enum.TryParse<T>(value, out var v))
                return OptionalEnum<T>.none;

            return v;
        }

        public override string ToString(OptionalEnum<T> value)
        {
            return value.IsSet ? value.Value.ToString() : string.Empty;
        }
    }

    // Specific Converters

    public class OptionalPopoverPlacementConverter : OptionalEnumConverter<PopoverPlacement> { }

    public class OptionalDirConverter : OptionalEnumConverter<Dir> { }

    public class OptionalIntConverter : OptionalValueConverter<int>
    {
        protected override bool TryParse(string value, out int v)
        {
            return int.TryParse(value, out v);
        }
    }

    public class OptionalLongConverter : OptionalValueConverter<long>
    {
        protected override bool TryParse(string value, out long v)
        {
            return long.TryParse(value, out v);
        }
    }

    public class OptionalFloatConverter : OptionalValueConverter<float>
    {
        protected override bool TryParse(string value, out float v)
        {
            return float.TryParse(value, out v);
        }
    }

    public class OptionalDoubleConverter : OptionalValueConverter<double>
    {
        protected override bool TryParse(string value, out double v)
        {
            return double.TryParse(value, out v);
        }
    }

    public class OptionalStringConverter : OptionalConverter<string>
    {
        protected override bool TryParse(string value, out string v)
        {
            v = value;
            return !string.IsNullOrEmpty(value);
        }
    }

    public class OptionalColorConverter : OptionalConverter<Color>
    {
        protected override bool TryParse(string value, out Color v)
        {
            return ColorUtility.TryParseHtmlString(value, out v);
        }
    }

    public class OptionalRectConverter : OptionalConverter<Rect>
    {
        protected override bool TryParse(string value, out Rect v)
        {
            return RectExtensions.TryParse(value, out v);
        }

        public override string ToString(Optional<Rect> value)
        {
            if (!value.IsSet)
                return string.Empty;

            var rect = value.Value;
            return $"{rect.x},{rect.y},{rect.width},{rect.height}";
        }
    }

    public class DateConverter : UxmlAttributeConverter<Date>
    {
        public override Date FromString(string value)
        {
            if (string.IsNullOrEmpty(value) || !DateTime.TryParse(value, out var v))
                return default;

            return new Date(v);
        }

        public override string ToString(Date value)
        {
            return ((DateTime)value).ToString(CultureInfo.InvariantCulture);
        }
    }

    public class DateRangeConverter : UxmlAttributeConverter<DateRange>
    {
        public override DateRange FromString(string value)
        {
            if (string.IsNullOrEmpty(value) || !DateRange.TryParse(value, out var v))
                return default;

            return v;
        }

        public override string ToString(DateRange value)
        {
            return value.ToString();
        }
    }

#if !UNITY_2023_3_OR_NEWER
    public class GradientConverter : UxmlAttributeConverter<Gradient>
    {
        public override Gradient FromString(string value)
        {
            if (GradientExtensions.TryParse(value, out var v))
                return v;

            throw new InvalidOperationException($"Cannot parse {value} to a Gradient.");
        }

        public override string ToString(Gradient value)
        {
            var sb = new StringBuilder();
            sb.Append($"{value.mode}:");

            for (var i = 0; i < value.colorKeys.Length; i++)
            {
                var colorKey = value.colorKeys[i];
                var s = $"{{{colorKey.time.ToString(CultureInfo.InvariantCulture)},#{ColorUtility.ToHtmlStringRGBA(colorKey.color)}}}";
                sb.Append(s);

                if (i + 1 != value.colorKeys.Length)
                    sb.Append(";");
            }

            sb.Append("+");

            for (var i = 0; i < value.alphaKeys.Length; i++)
            {
                var colorKey = value.alphaKeys[i];
                var s = $"{{{colorKey.time.ToString(CultureInfo.InvariantCulture)},{colorKey.alpha.ToString(CultureInfo.InvariantCulture)}}}";
                sb.Append(s);

                if (i + 1 != value.alphaKeys.Length)
                    sb.Append(";");
            }

            var result = sb.ToString();
            return result;
        }
    }
#endif
}
#endif
