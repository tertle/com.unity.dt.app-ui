using System.Globalization;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(UINumericFieldsUtils))]
    class UINumericFieldsUtilsTests
    {
        [Test]
        [TestCase("0", false)]
        [TestCase("0.0", false)]
        [TestCase("0.00", false)]
        [TestCase("0.000", false)]
        [TestCase("0P", false)]
        [TestCase("0.0P", false)]
        [TestCase("0.00P", false)]
        [TestCase("P", true)]
        [TestCase("P0", true)]
        [TestCase("P0.0", false)]
        [TestCase("P0.00", false)]
        [TestCase("P0.000", false)]
        [TestCase("P00", true)]
        [TestCase("P000", true)]
        [TestCase("P0000", true)]
        [TestCase("P1", true)]
        [TestCase("P2", true)]
        [TestCase("P3", true)]
        [TestCase("P4", true)]
        [TestCase("0%", true)]
        [TestCase("0.0%", true)]
        [TestCase("0.00%", true)]
        [TestCase("%", false)]
        [TestCase("%0", true)]
        [TestCase("%0.0", true)]
        [TestCase("%0.00", true)]
        [TestCase(" %", false)]
        [TestCase("0 %", true)]
        [TestCase("0.0 %", true)]
        [TestCase("0.00 %", true)]
        [TestCase("% ", false)]
        [TestCase("% 0", true)]
        [TestCase("% 0.0", true)]
        [TestCase("% 0.00", true)]
        [TestCase("% 000", true)]
        [TestCase("%%% 000", false)]
        [TestCase("0%0", false)]
        [TestCase("## %", true)]
        public void IsPercentFormatString_Succeed(string formatString, bool isPercent)
        {
            Assert.AreEqual(isPercent, UINumericFieldsUtils.IsPercentFormatString(formatString));
        }
    }
}
