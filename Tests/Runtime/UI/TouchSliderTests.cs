using System;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(TouchSliderFloat))]
    class TouchSliderFloatTests : TouchSliderTests<TouchSliderFloat, float>
    {
        protected override string mainUssClassName => TouchSliderFloat.ussClassName;
    }

    [TestFixture]
    [TestOf(typeof(TouchSliderInt))]
    class TouchSliderIntTests : TouchSliderTests<TouchSliderInt, int>
    {
        protected override string mainUssClassName => TouchSliderInt.ussClassName;
    }

    class TouchSliderTests<T, TU> : VisualElementTests<T>
        where T : TouchSlider<TU>, new()
        where TU : unmanaged, IComparable, IEquatable<TU>
    {

    }
}
