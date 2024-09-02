using NUnit.Framework;
using Unity.AppUI.Core;
using UnityEngine;

namespace Unity.AppUI.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(AppUITouch))]
    public class AppUITouchTests
    {
        [Test]
        public void GetFingerId_ShouldReturnFingerId()
        {
            var touch = new AppUITouch(
                fingerId:1,
                position:Vector2.one,
                deltaPos:Vector2.one,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            Assert.AreEqual(1, touch.fingerId);

            touch = new AppUITouch(
                fingerId:2,
                position:Vector2.one,
                deltaPos:Vector2.one,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            Assert.AreEqual(2, touch.fingerId);
        }

        [Test]
        public void GetPosition_ShouldReturnPosition()
        {
            var touch = new AppUITouch(
                fingerId:1,
                position:Vector2.one,
                deltaPos:Vector2.one,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            Assert.AreEqual(Vector2.one, touch.position);

            touch = new AppUITouch(
                fingerId:1,
                position:Vector2.zero,
                deltaPos:Vector2.one,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            Assert.AreEqual(Vector2.zero, touch.position);
        }

        [Test]
        public void GetDeltaPos_ShouldReturnDeltaPos()
        {
            var touch = new AppUITouch(
                fingerId:1,
                position:Vector2.one,
                deltaPos:Vector2.one,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            Assert.AreEqual(Vector2.one, touch.deltaPos);

            touch = new AppUITouch(
                fingerId:1,
                position:Vector2.zero,
                deltaPos:Vector2.zero,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            Assert.AreEqual(Vector2.zero, touch.deltaPos);
        }

        [Test]
        public void GetDeltaTime_ShouldReturnDeltaTime()
        {
            var touch = new AppUITouch(
                fingerId:1,
                position:Vector2.one,
                deltaPos:Vector2.one,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            Assert.AreEqual(1.0f, touch.deltaTime);

            touch = new AppUITouch(
                fingerId:1,
                position:Vector2.zero,
                deltaPos:Vector2.zero,
                deltaTime:2.0f,
                phase:TouchPhase.Began);
            Assert.AreEqual(2.0f, touch.deltaTime);
        }

        [Test]
        public void GetPhase_ShouldReturnPhase()
        {
            var touch = new AppUITouch(
                fingerId:1,
                position:Vector2.one,
                deltaPos:Vector2.one,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            Assert.AreEqual(TouchPhase.Began, touch.phase);

            touch = new AppUITouch(
                fingerId:1,
                position:Vector2.zero,
                deltaPos:Vector2.zero,
                deltaTime:2.0f,
                phase:TouchPhase.Moved);
            Assert.AreEqual(TouchPhase.Moved, touch.phase);
        }

        [Test]
        public void Equals_ShouldReturnTrue()
        {
            var touch1 = new AppUITouch(
                fingerId:1,
                position:Vector2.one,
                deltaPos:Vector2.one,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            var touch2 = new AppUITouch(
                fingerId:1,
                position:Vector2.one,
                deltaPos:Vector2.one,
                deltaTime:1.0f,
                phase:TouchPhase.Began);
            Assert.IsTrue(touch1.Equals(touch2));
        }

        [Test]
        public void Equals_ShouldReturnFalse()
        {
            var touch1 = new AppUITouch(fingerId: 1, position: Vector2.one, deltaPos: Vector2.one,
                deltaTime: 1.0f, phase: TouchPhase.Began);
            var touch2 = new AppUITouch(fingerId: 2, position: Vector2.one, deltaPos: Vector2.one,
                deltaTime: 1.0f, phase: TouchPhase.Began);
            Assert.IsFalse(touch1.Equals(touch2));
        }
    }
}
