using System;
using NUnit.Framework;
using Unity.AppUI.Core;

namespace Unity.AppUI.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(MemoryUtils))]
    public class MemoryUtilsTests
    {
        [Test]
        [TestCase("Hello", "World", ExpectedResult = "HelloWorld")]
        [TestCase("Hello", "", ExpectedResult = "Hello")]
        [TestCase(null, "World", ExpectedResult = "World")]
        public string Concatenate_WhenCalledWithTwoStrings_ReturnsConcatenatedString(string str1, string str2)
        {
            var result = MemoryUtils.Concatenate(str1, str2);
            return result;
        }

        [Test]
        [TestCase("Hello", "World", "A", ExpectedResult = "HelloWorldA")]
        [TestCase("Hello", "", "A", ExpectedResult = "HelloA")]
        [TestCase(null, "World", "A", ExpectedResult = "WorldA")]
        public string Concatenate_WhenCalledWithThreeStrings_ReturnsConcatenatedString(string str1, string str2, string str3)
        {
            var result = MemoryUtils.Concatenate(str1, str2, str3);
            return result;
        }

        [Test]
        [TestCase("Hello", "World", "A", "B", ExpectedResult = "HelloWorldAB")]
        [TestCase("Hello", "", "A", "B", ExpectedResult = "HelloAB")]
        [TestCase(null, "World", "A", "B", ExpectedResult = "WorldAB")]
        public string Concatenate_WhenCalledWithFourStrings_ReturnsConcatenatedString(string str1, string str2, string str3, string str4)
        {
            var result = MemoryUtils.Concatenate(str1, str2, str3, str4);
            return result;
        }

        [Test]
        [TestCase("Hello", "World", "A", "B", "C", ExpectedResult = "HelloWorldABC")]
        [TestCase("Hello", "", "A", "B", "C", ExpectedResult = "HelloABC")]
        [TestCase(null, "World", "A", "B", "C", ExpectedResult = "WorldABC")]
        public string Concatenate_WhenCalledWithFiveStrings_ReturnsConcatenatedString(string str1, string str2, string str3, string str4, string str5)
        {
            var result = MemoryUtils.Concatenate(str1, str2, str3, str4, str5);
            return result;
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("a", true)]
        public void Concatenate_WhenCalledWithStringTotalLengthExceedsBufferSize_ThrowsArgumentException(string secondString, bool shouldThrow)
        {
            var str = new string('a', MemoryUtils.bufferSize);
            if (shouldThrow)
            {
                // 2
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(str, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, str));

                // 3
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(str, secondString, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, str, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, secondString, str));

                // 4
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(str, secondString, secondString, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, str, secondString, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, secondString, str, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, secondString, secondString, str));

                // 5
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(str, secondString, secondString, secondString, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, str, secondString, secondString, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, secondString, str, secondString, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, secondString, secondString, str, secondString));
                Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(secondString, secondString, secondString, secondString, str));
            }
            else
            {
                // 2
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(str, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, str));

                // 3
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(str, secondString, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, str, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, secondString, str));

                // 4
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(str, secondString, secondString, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, str, secondString, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, secondString, str, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, secondString, secondString, str));

                // 5
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(str, secondString, secondString, secondString, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, str, secondString, secondString, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, secondString, str, secondString, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, secondString, secondString, str, secondString));
                Assert.DoesNotThrow(() => MemoryUtils.Concatenate(secondString, secondString, secondString, secondString, str));
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("a")]
        public void Concatenate_WhenFirstStringTotalLengthExceedsBufferSize_ThrowsArgumentException(string secondString)
        {
            var str = new string('a', MemoryUtils.bufferSize + 1);
            Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(str, secondString));
            Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(str, secondString, secondString));
            Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(str, secondString, secondString, secondString));
            Assert.Throws<ArgumentException>(() => MemoryUtils.Concatenate(str, secondString, secondString, secondString, secondString));
        }
    }
}
