using System;
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(GradientExtensions))]
    class GradientExtensionsTests
    {
        [Test]
        public void TryParse_NullString_ThrowsException()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => GradientExtensions.TryParse(null, out _));
        }

        [Test]
        public void TryParse_InvalidString_ReturnsFalse()
        {
            var result = GradientExtensions.TryParse("invalid", out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryParse_WithoutItems_ReturnsFalse()
        {
            var result = GradientExtensions.TryParse("Blend:", out _);
            Assert.IsFalse(result);

            result = GradientExtensions.TryParse("Blend:+++", out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryParse_ValidString_ReturnsTrue()
        {
            var result = GradientExtensions.TryParse("Blend:[0.0,#000000FF];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,1.0]", out _);
            Assert.IsTrue(result);

            result = GradientExtensions.TryParse("Blend:(0.0,#000000FF);(1.0,#FFFFFFFF)+(0.0,1.0);(1.0,1.0)", out _);
            Assert.IsTrue(result);
        }

        [Test]
        public void TryParse_WithoutMode_ReturnsFalse()
        {
            var result = GradientExtensions.TryParse("[0.0,#000000FF];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,1.0]", out _);
            Assert.IsFalse(result);

            result = GradientExtensions.TryParse(":[0.0,#000000FF];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,1.0]", out _);
            Assert.IsFalse(result);

            result = GradientExtensions.TryParse("Toto:[0.0,#000000FF];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,1.0]", out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryParse_WithoutColorKeys_ReturnsFalse()
        {
            var result = GradientExtensions.TryParse("Blend:+[0.0,1.0];[1.0,1.0]", out _);
            Assert.IsFalse(result);

            result = GradientExtensions.TryParse("Blend:[];[]+[0.0,1.0];[1.0,1.0]", out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryParse_WithoutAlphaKeys_ReturnsFalse()
        {
            var result = GradientExtensions.TryParse("Blend:[0.0,#000000FF];[1.0,#FFFFFFFF]+[];[]", out _);
            Assert.IsFalse(result);

            result = GradientExtensions.TryParse("Blend:[0.0,#000000FF];[1.0,#FFFFFFFF]+", out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryParse_InvalidColorKey_ReturnsFalse()
        {
            var result = GradientExtensions.TryParse("Blend:[0.0,#000000FF,x];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,1.0]", out _);
            Assert.IsFalse(result);

            result = GradientExtensions.TryParse("Blend:[0.0,#ZZZZZ];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,1.0", out _);
            Assert.IsFalse(result);

            result = GradientExtensions.TryParse("Blend:[ZZZ,#000000FF];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,1.0]", out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryParse_InvalidAlphaKey_ReturnsFalse()
        {
            var result = GradientExtensions.TryParse("Blend:[0.0,#000000FF];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,1.0,x]", out _);
            Assert.IsFalse(result);

            result = GradientExtensions.TryParse("Blend:[0.0,#000000FF];[1.0,#FFFFFFFF]+[0.0,1.0];[1.0,toto]", out _);
            Assert.IsFalse(result);

            result = GradientExtensions.TryParse("Blend:[0.0,#000000FF];[1.0,#FFFFFFFF]+[0.0,1.0];[toto,1.0]", out _);
            Assert.IsFalse(result);
        }
    }
}
