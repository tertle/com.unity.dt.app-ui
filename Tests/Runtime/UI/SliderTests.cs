using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine.UIElements;
using SliderInt = Unity.AppUI.UI.SliderInt;
using SliderFloat = Unity.AppUI.UI.SliderFloat;
using FloatField = Unity.AppUI.UI.FloatField;
using IntField = Unity.AppUI.UI.IntField;

namespace Unity.AppUI.Tests.UI
{
    class SliderTests<T,TValue,TScalar,TInputField> : VisualElementTests<T>
        where T : Slider<TValue,TScalar,TInputField>, new()
        where TScalar : unmanaged, IFormattable
        where TInputField : VisualElement, IValidatableElement<TValue>, IFormattable<TScalar>, new()
    {
        protected virtual TScalar highValue { get; }

        protected virtual TScalar lowValue { get; }

        protected virtual TValue value { get; }

        protected virtual TScalar step { get; }

        protected virtual TScalar shiftStep { get; }

        protected virtual List<SliderMark<TScalar>> marks { get; }

        protected override IEnumerable<Story> stories
        {
            get
            {
                yield return new Story("Default", context => new T());
                yield return new Story("With Step Marks", context =>
                {
                    var slider = new T
                    {
                        highValue = highValue,
                        step = step,
                        shiftStep = shiftStep,
                        showMarks = true,
                        formatString = "0.0"
                    };
                    return slider;
                });
                yield return new Story("With Custom Marks And Labels", context =>
                {
                    var slider = new T
                    {
                        highValue = highValue,
                        showMarks = true,
                        showMarksLabel = true,
                        customMarks = marks
                    };
                    return slider;
                });
                yield return new Story("With Input Field", context =>
                {
                    var slider = new T
                    {
                        showInputField = true,
                        formatFunction = v => v.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture.NumberFormat),
                    };
                    return slider;
                });
                yield return new Story("With Label Display", context =>
                {
                    var slider = new T
                    {
                        displayValueLabel = ValueDisplayMode.On
                    };
                    return slider;
                });
                yield return new Story("With Track", context =>
                {
                    var slider = new T
                    {
                        highValue = highValue,
                        value = value,
                        track = TrackDisplayType.On
                    };
                    return slider;
                });
                yield return new Story("With Inverted Track", context =>
                {
                    var slider = new T
                    {
                        highValue = highValue,
                        value = value,
                        track = TrackDisplayType.Inverted
                    };
                    return slider;
                });
                yield return new Story("Vertical With Track And Marks And Labels", context =>
                {
                    var slider = new T
                    {
                        highValue = highValue,
                        value = value,
                        track = TrackDisplayType.On,
                        orientation = Direction.Vertical,
                        step = step,
                        showMarks = true,
                        showMarksLabel = true,
                        formatString = "0.0",
                        style = { height = 300 }
                    };
                    return slider;
                });
            }
        }

        protected virtual string highValueToUxml => $"high-value=\"{highValue.ToString()}\"";

        protected virtual string lowValueToUxml => $"low-value=\"{lowValue.ToString()}\"";

        protected virtual string valueToUxml => $"value=\"{value.ToString()}\"";

        protected override IEnumerable<string> uxmlTestCases
        {
            get
            {
                yield return $"<{uxmlNamespaceName}:{componentName} />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} show-marks=\"true\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} show-marks=\"true\" show-marks-label=\"true\" format-string=\"0.0\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} show-marks=\"true\" show-marks-label=\"true\" format-string=\"0.0\" style=\"height:300px;\" orientation=\"Vertical\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} show-input-field=\"true\" format-string=\"0.0\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} display-value-label=\"On\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} track=\"On\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} track=\"Inverted\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} show-marks=\"true\" show-marks-label=\"true\" format-string=\"0.0\" track=\"On\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} show-marks=\"true\" show-marks-label=\"true\" format-string=\"0.0\" track=\"Inverted\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} show-marks=\"true\" show-marks-label=\"true\" format-string=\"0.0\" style=\"height:300px;\" orientation=\"Vertical\" track=\"On\" />";
                yield return $"<{uxmlNamespaceName}:{componentName} {highValueToUxml} {valueToUxml} show-marks=\"true\" show-marks-label=\"true\" format-string=\"0.0\" style=\"height:300px;\" orientation=\"Vertical\" track=\"Inverted\" />";
            }
        }
    }

    [TestFixture]
    [TestOf(typeof(SliderFloat))]
    class SliderFloatTests : SliderTests<SliderFloat,float,float,FloatField>
    {
        protected override string mainUssClassName => SliderFloat.ussClassName;

        protected override List<SliderMark<float>> marks => new List<SliderMark<float>>
        {
            new() {label = "Low", value = 0f},
            new() {label = "Middle", value = 5f},
            new() {label = "High", value = 10f},
        };

        protected override float highValue => 10f;

        protected override float lowValue => 0f;

        protected override float value => 3f;

        protected override float step => 1f;

        protected override float shiftStep => 2f;
    }

    [TestFixture]
    [TestOf(typeof(SliderInt))]
    class SliderIntTests : SliderTests<SliderInt,int,int,IntField>
    {
        protected override string mainUssClassName => SliderInt.ussClassName;

        protected override List<SliderMark<int>> marks => new List<SliderMark<int>>
        {
            new() {label = "Low", value = 0},
            new() {label = "Middle", value = 5},
            new() {label = "High", value = 10},
        };

        protected override int highValue => 10;

        protected override int lowValue => 0;

        protected override int value => 3;

        protected override int step => 1;

        protected override int shiftStep => 2;
    }
}
