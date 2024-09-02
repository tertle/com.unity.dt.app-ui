using System.Collections;
using NUnit.Framework;
using Unity.AppUI.UI;
using UnityEngine;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(ColorExtensions))]
    public class ColorExtensionsTests
    {
        static IEnumerable ColorToRgbaHex_ValidColor_ReturnsRgbaHex_TestCases
        {
            get
            {
                yield return new TestCaseData(Color.clear, false).Returns("000000");
                yield return new TestCaseData(Color.clear, true).Returns("00000000");

                yield return new TestCaseData(Color.black, false).Returns("000000");
                yield return new TestCaseData(Color.black, true).Returns("000000FF");

                yield return new TestCaseData(Color.white, false).Returns("FFFFFF");
                yield return new TestCaseData(Color.white, true).Returns("FFFFFFFF");
            }
        }

        [Test]
        [TestCaseSource(nameof(ColorToRgbaHex_ValidColor_ReturnsRgbaHex_TestCases))]
        public string ColorToRgbaHex_ValidColor_ReturnsRgbaHex(Color color, bool withAlpha)
        {
            return ColorExtensions.ColorToRgbaHex(color, withAlpha);
        }

        static IEnumerable ArgbToRgbaHex_ValidArgbHex_ReturnsRgbaHex_TestCases
        {
            get
            {
                yield return new TestCaseData("00000000", false).Returns("000000");
                yield return new TestCaseData("00000000", true).Returns("00000000");

                yield return new TestCaseData("000", false).Returns("000");
                yield return new TestCaseData("000", true).Returns("000F");

                yield return new TestCaseData("FF000000", false).Returns("000000");
                yield return new TestCaseData("FF000000", true).Returns("000000FF");

                yield return new TestCaseData("FFFFFFFF", false).Returns("FFFFFF");
                yield return new TestCaseData("FFFFFFFF", true).Returns("FFFFFFFF");

                yield return new TestCaseData("FF123456", false).Returns("123456");
                yield return new TestCaseData("00123456", true).Returns("12345600");

                yield return new TestCaseData("F123", false).Returns("123");
                yield return new TestCaseData("F123", true).Returns("123F");
            }
        }

        [Test]
        [TestCaseSource(nameof(ArgbToRgbaHex_ValidArgbHex_ReturnsRgbaHex_TestCases))]
        public string ArgbToRgbaHex_ValidArgbHex_ReturnsRgbaHex(string str, bool alpha)
        {
            return ColorExtensions.ArgbToRgbaHex(str, alpha);
        }

        [Test]
        public void ArgbToRgbaHex_InvalidArgbHex_ReturnsNull()
        {
            Assert.IsNull(ColorExtensions.ArgbToRgbaHex("FF", true));
            Assert.IsNull(ColorExtensions.ArgbToRgbaHex("FF", false));
            Assert.IsNull(ColorExtensions.ArgbToRgbaHex("FF00FF00FF", true));
            Assert.IsNull(ColorExtensions.ArgbToRgbaHex("FF00FF00FF", false));
        }
    }
}
