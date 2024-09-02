using NUnit.Framework;
using Unity.AppUI.Core;
using UnityEngine;

namespace Unity.AppUI.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(PinchGesture))]
    public class PinchGestureTests
    {
        [Test]
        public void GetDeltaMagnification_ShouldReturnDeltaMagnification()
        {
            var gesture = new PinchGesture(1.0f, GestureRecognizerState.Recognized);
            Assert.AreEqual(1.0f, gesture.deltaMagnification);
        }

        [Test]
        public void GetScrollDelta_ShouldReturnScrollDelta()
        {
            var gesture = new PinchGesture(1.0f, GestureRecognizerState.Recognized);
            Assert.AreEqual(new Vector2(0, -50f), gesture.scrollDelta);

            gesture = new PinchGesture(2.0f, GestureRecognizerState.Recognized);
            Assert.AreEqual(new Vector2(0, -100f), gesture.scrollDelta);
        }

        [Test]
        public void GetPhase_ShouldReturnPhase()
        {
            var gesture = new PinchGesture(1.0f, GestureRecognizerState.Recognized);
            Assert.AreEqual(GestureRecognizerState.Recognized, gesture.state);

            gesture = new PinchGesture(1.0f, GestureRecognizerState.Ended);
            Assert.AreEqual(GestureRecognizerState.Ended, gesture.state);
        }

        [Test]
        public void Equals_ShouldReturnTrue()
        {
            var gesture1 = new PinchGesture(1.0f, GestureRecognizerState.Recognized);
            var gesture2 = new PinchGesture(1.0f, GestureRecognizerState.Recognized);
            Assert.IsTrue(gesture1.Equals(gesture2));
            Assert.IsTrue(gesture1 == gesture2);
        }

        [Test]
        public void Equals_ShouldReturnFalse()
        {
            var gesture1 = new PinchGesture(1.0f, GestureRecognizerState.Recognized);
            var gesture2 = new PinchGesture(2.0f, GestureRecognizerState.Recognized);
            Assert.IsFalse(gesture1.Equals(gesture2));
            Assert.IsFalse(gesture1 == gesture2);
            Assert.IsTrue(gesture1 != gesture2);

            gesture1 = new PinchGesture(1.0f, GestureRecognizerState.Recognized);
            gesture2 = new PinchGesture(1.0f, GestureRecognizerState.Ended);
            Assert.IsFalse(gesture1.Equals(gesture2));
            Assert.IsFalse(gesture1 == gesture2);
            Assert.IsTrue(gesture1 != gesture2);
        }

        [Test]
        public void Equals_ShouldReturnFalse_WhenObjectIsNotPinchGesture()
        {
            var gesture = new PinchGesture(1.0f, GestureRecognizerState.Recognized);
            var obj = new object();
            Assert.IsFalse(gesture.Equals(obj));
        }

        [Test]
        public void GetHashCode_ShouldReturnHashCode()
        {
            var gesture = new PinchGesture(1.0f, GestureRecognizerState.Recognized);
            Assert.AreEqual(gesture.GetHashCode(), new PinchGesture(1.0f, GestureRecognizerState.Recognized).GetHashCode());
        }
    }
}
