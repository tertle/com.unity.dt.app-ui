using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Styles))]
    public class StylesTests
    {
        [Test]
        [TestCase(Size.S, ExpectedResult = IconSize.S)]
        [TestCase(Size.M, ExpectedResult = IconSize.M)]
        [TestCase(Size.L, ExpectedResult = IconSize.L)]
        public IconSize ToIconSize_ValidSize_ReturnsIconSize(Size size)
        {
            return size.ToIconSize();
        }

        [Test]
        public void ToIconSize_InvalidSize_ThrowsException()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => ((Size) 100).ToIconSize());
        }

        [Test]
        [TestCase(IconSize.S, ExpectedResult = Size.S)]
        [TestCase(IconSize.M, ExpectedResult = Size.M)]
        [TestCase(IconSize.L, ExpectedResult = Size.L)]
        public Size ToSize_ValidIconSize_ReturnsSize(IconSize size)
        {
            return size.ToSize();
        }

        [Test]
        public void ToSize_InvalidIconSize_ThrowsException()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => ((IconSize) 100).ToSize());
        }
    }
}
