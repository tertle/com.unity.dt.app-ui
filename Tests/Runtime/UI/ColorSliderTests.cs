using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ColorSlider))]
    class ColorSliderTests : SliderTests<ColorSlider,float,float,FloatField>
    {
        protected override string mainUssClassName => ColorSlider.ussClassName;

        protected override IEnumerable<string> uxmlTestCases => new[]
        {
            @"<appui:ColorSlider />",
            @"<appui:ColorSlider value=""0.5"" low-value=""0"" high-value=""1"" step=""0.1"" shift-step=""0.2"" color-range=""Blend:[0.0,#FF000000];[1.0,#FF0000FF]+[0.0,1.0];[1.0,1.0]"" />",
            @"<appui:ColorSlider value=""1.0"" low-value=""0"" high-value=""1"" step=""0.1"" shift-step=""0.2"" color-range=""Blend:[0.0,#FF000000];[1.0,#FF0000FF]+[0.0,1.0];[1.0,1.0]"" />",
            @"<appui:ColorSlider style=""height:300px;"" orientation=""Vertical"" value=""0.5"" low-value=""0"" high-value=""1"" step=""0.1"" shift-step=""0.2"" color-range=""Blend:[0.0,#FF000000];[1.0,#FF0000FF]+[0.0,1.0];[1.0,1.0]"" />",
            @"<appui:ColorSlider style=""height:300px;"" orientation=""Vertical"" value=""1.0"" low-value=""0"" high-value=""1"" step=""0.1"" shift-step=""0.2"" color-range=""Blend:[0.0,#FF000000];[1.0,#FF0000FF]+[0.0,1.0];[1.0,1.0]"" />",
        };

        protected override IEnumerable<Story> stories
        {
            get
            {
                // default
                yield return new Story("Default", ctx => new ColorSlider());
                // rainbow colorRange
                yield return new Story("Rainbow", ctx => new ColorSlider
                {
                    colorRange = rainbowGradient,
                    value = 0.5f,
                });
                yield return new Story("Vertical Rainbow", ctx => new ColorSlider
                {
                    colorRange = rainbowGradient,
                    value = 0.5f,
                    orientation = Direction.Vertical,
                    style = { height = 300 }
                });
            }
        }

        static Gradient rainbowGradient
        {
            get
            {
                var gradient = new Gradient();
                var gradientColorKeys = new GradientColorKey[7];
                var gradientAlphaKeys = new GradientAlphaKey[2];
                gradientColorKeys[0].color = Color.red;
                gradientColorKeys[0].time = 0.0f;
                gradientColorKeys[1].color = Color.magenta;
                gradientColorKeys[1].time = 0.166f;
                gradientColorKeys[2].color = Color.blue;
                gradientColorKeys[2].time = 0.333f;
                gradientColorKeys[3].color = Color.cyan;
                gradientColorKeys[3].time = 0.5f;
                gradientColorKeys[4].color = Color.green;
                gradientColorKeys[4].time = 0.666f;
                gradientColorKeys[5].color = Color.yellow;
                gradientColorKeys[5].time = 0.833f;
                gradientColorKeys[6].color = Color.red;
                gradientColorKeys[6].time = 1.0f;
                gradientAlphaKeys[0].alpha = 1.0f;
                gradientAlphaKeys[0].time = 0.0f;
                gradientAlphaKeys[1].alpha = 1.0f;
                gradientAlphaKeys[1].time = 1.0f;
                gradient.SetKeys(gradientColorKeys, gradientAlphaKeys);
                return gradient;
            }
        }
    }
}
