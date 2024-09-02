using System;
using NUnit.Framework;
using Unity.AppUI.Core;

namespace Unity.AppUI.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(Optional<>))]
    class OptionalTests
    {
        [Test]
        public void OptionalConverter_Equals_ShouldReturnTrue_WhenBothAreNone()
        {
            var actual = Optional<int>.none.Equals(Optional<int>.none);
            Assert.IsTrue(actual);
        }

        [Test]
        public void OptionalConverter_Equals_ShouldReturnFalse_WhenOneIsNone()
        {
            var actual = Optional<int>.none.Equals(new Optional<int>(42));
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalConverter_Equals_ShouldReturnTrue_WhenBothAreEqual()
        {
            var actual = new Optional<int>(42).Equals(new Optional<int>(42));
            Assert.IsTrue(actual);
            actual = new Optional<int>(42) == (new Optional<int>(42));
            Assert.IsTrue(actual);
        }

        [Test]
        public void OptionalConverter_Equals_ShouldReturnFalse_WhenBothAreNotEqual()
        {
            var actual = new Optional<int>(42).Equals(new Optional<int>(43));
            Assert.IsFalse(actual);
            actual = new Optional<int>(42) == new Optional<int>(43);
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalConverter_Equals_ShouldReturnTrue_WhenOtherCanBeCastToOptional()
        {
            var actual = new Optional<int>(42).Equals(42);
            Assert.IsTrue(actual);
            actual = new Optional<int>(42) == 42;
            Assert.IsTrue(actual);
            actual = new Optional<int>(42) != 42;
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalConverter_Equals_ShouldReturnFalse_WhenOtherCannotBeCastToOptional()
        {
            var actual = new Optional<int>(42).Equals("42");
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalConverter_Equals_ShouldReturnFalse_WhenOtherIsNull()
        {
            var actual = new Optional<int>(42).Equals(null);
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalConverter_IsSet_ShouldReturnTrue_WhenValueIsSet()
        {
            var actual = new Optional<int>(42).IsSet;
            Assert.IsTrue(actual);
        }

        [Test]
        public void OptionalConverter_IsSet_ShouldReturnFalse_WhenValueIsNotSet()
        {
            var actual = Optional<int>.none.IsSet;
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalConverter_Value_ShouldReturnValue_WhenValueIsSet()
        {
            var expected = 42;
            var actual = new Optional<int>(expected).Value;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OptionalConverter_GetHashCode_ShouldReturnSameHashCode_WhenValuesAreEqual()
        {
            var expected = new Optional<int>(42).GetHashCode();
            var actual = new Optional<int>(42).GetHashCode();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OptionalConverter_GetHashCode_ShouldReturnDifferentHashCode_WhenValuesAreNotEqual()
        {
            var expected = new Optional<int>(42).GetHashCode();
            var actual = new Optional<int>(43).GetHashCode();
            Assert.AreNotEqual(expected, actual);
        }
    }

    [TestFixture]
    [TestOf(typeof(OptionalEnum<>))]
    class OptionalEnumTests
    {
        Type enumType => typeof(Dir);

        [Test]
        public void OptionalEnumConverter_Equals_ShouldReturnTrue_WhenBothAreNone()
        {
            var actual = OptionalEnum<Dir>.none.Equals(OptionalEnum<Dir>.none);
            Assert.IsTrue(actual);
        }

        [Test]
        public void OptionalEnumConverter_Equals_ShouldReturnFalse_WhenOneIsNone()
        {
            var actual = OptionalEnum<Dir>.none.Equals(new OptionalEnum<Dir>(Dir.Ltr));
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalEnumConverter_Equals_ShouldReturnTrue_WhenBothAreEqual()
        {
            var actual = new OptionalEnum<Dir>(Dir.Ltr).Equals(new OptionalEnum<Dir>(Dir.Ltr));
            Assert.IsTrue(actual);
            actual = new OptionalEnum<Dir>(Dir.Ltr) == (new OptionalEnum<Dir>(Dir.Ltr));
            Assert.IsTrue(actual);
        }

        [Test]
        public void OptionalEnumConverter_Equals_ShouldReturnFalse_WhenBothAreNotEqual()
        {
            var actual = new OptionalEnum<Dir>(Dir.Ltr).Equals(new OptionalEnum<Dir>(Dir.Rtl));
            Assert.IsFalse(actual);
            actual = new OptionalEnum<Dir>(Dir.Ltr) == new OptionalEnum<Dir>(Dir.Rtl);
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalEnumConverter_Equals_ShouldReturnTrue_WhenOtherCanBeCastToOptional()
        {
            var actual = new OptionalEnum<Dir>(Dir.Ltr).Equals(Dir.Ltr);
            Assert.IsTrue(actual);
            actual = new OptionalEnum<Dir>(Dir.Ltr) == Dir.Ltr;
            Assert.IsTrue(actual);
            actual = new OptionalEnum<Dir>(Dir.Ltr) != Dir.Ltr;
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalEnumConverter_Equals_ShouldReturnFalse_WhenOtherCannotBeCastToOptional()
        {
            var actual = new OptionalEnum<Dir>(Dir.Ltr).Equals("Ltr");
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalEnumConverter_Equals_ShouldReturnFalse_WhenOtherIsNull()
        {
            var actual = new OptionalEnum<Dir>(Dir.Ltr).Equals(null);
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalEnumConverter_IsSet_ShouldReturnTrue_WhenValueIsSet()
        {
            var actual = new OptionalEnum<Dir>(Dir.Ltr).IsSet;
            Assert.IsTrue(actual);
        }

        [Test]
        public void OptionalEnumConverter_IsSet_ShouldReturnFalse_WhenValueIsNotSet()
        {
            var actual = OptionalEnum<Dir>.none.IsSet;
            Assert.IsFalse(actual);
        }

        [Test]
        public void OptionalEnumConverter_Value_ShouldReturnValue_WhenValueIsSet()
        {
            var expected = Dir.Ltr;
            var actual = new OptionalEnum<Dir>(expected).Value;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void OptionalEnumConverter_GetHashCode_ShouldReturnSameHashCode_WhenValuesAreEqual()
        {
            var expected = new OptionalEnum<Dir>(Dir.Ltr).GetHashCode();
            var actual = new OptionalEnum<Dir>(Dir.Ltr).GetHashCode();
            Assert.AreEqual(expected, actual);
        }
    }
}