#if !UNITY_2022_1_OR_NEWER
using NUnit.Framework;
using Unity.AppUI.UI;

namespace Unity.AppUI.Tests.UI
{
    [TestFixture]
    [TestOf(typeof(Unity.AppUI.UI.ExpressionEvaluator))]
    class ExpressionEvaluatorTests
    {
        [Test]
        [TestCase("1 + 1", 2, true)]
        public void Evaluate(string text, double expectedValue, bool expectedResult)
        {
            var result = Unity.AppUI.UI.ExpressionEvaluator.Evaluate(text, out double value);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(expectedValue, value);
        }
    }
}

#endif