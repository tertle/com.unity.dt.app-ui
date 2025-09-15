using System.Collections.Generic;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(RangeSliderFloat))]
    class RangeSliderFloatTests : SliderTests<RangeSliderFloat,Vector2,float,Vector2Field>
    {
        protected override string mainUssClassName => RangeSliderFloat.ussClassName;

        protected override List<SliderMark<float>> marks => new List<SliderMark<float>>
        {
            new() {label = "Low", value = 0f},
            new() {label = "Middle", value = 5f},
            new() {label = "High", value = 10f},
        };

        protected override float highValue => 10f;

        protected override float lowValue => 0f;

        protected override Vector2 value => new Vector2(3f, 7f);

        protected override float step => 1f;

        protected override float shiftStep => 2f;

        protected override string highValueToUxml => $"high-value=\"{highValue}\"";

        protected override string valueToUxml => $"min-value=\"{value.x}\" max-value=\"{value.y}\"";
    }

    [TestFixture]
    [TestOf(typeof(RangeSliderInt))]
    class RangeSliderIntTests : SliderTests<RangeSliderInt,Vector2Int,int,Vector2IntField>
    {
        protected override string mainUssClassName => RangeSliderInt.ussClassName;

        protected override List<SliderMark<int>> marks => new List<SliderMark<int>>
        {
            new() {label = "Low", value = 0},
            new() {label = "Middle", value = 5},
            new() {label = "High", value = 10},
        };

        protected override int highValue => 10;

        protected override int lowValue => 0;

        protected override Vector2Int value => new Vector2Int(3, 7);

        protected override int step => 1;

        protected override int shiftStep => 2;

        protected override string highValueToUxml => $"high-value=\"{highValue}\"";

        protected override string valueToUxml => $"min-value=\"{value.x}\" max-value=\"{value.y}\"";
    }
}
