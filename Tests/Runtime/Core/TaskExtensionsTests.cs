using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.AppUI.Core;
using UnityEngine.TestTools;
using TaskExtensions = Unity.AppUI.Core.TaskExtensions;

namespace Unity.AppUI.Tests.Core
{
    [TestFixture]
    [TestOf(typeof(TaskExtensions))]
    public class TaskExtensionsTests
    {
        [Test]
        public void AsCoroutine_WhenTaskIsNull_ReturnsEmptyCoroutine()
        {
            Task task = null;
            var coroutine = task.AsCoroutine();
            Assert.IsFalse(coroutine.MoveNext());
        }

        [Test]
        public void AsCoroutine_WhenTaskIsCompleted_ReturnsEmptyCoroutine()
        {
            var task = Task.CompletedTask;
            var coroutine = task.AsCoroutine();
            Assert.IsFalse(coroutine.MoveNext());
        }

        [Test]
        public void AsCoroutine_WhenTaskIsNotCompleted_ReturnsNonEmptyCoroutine()
        {
            var task = Task.Delay(10);
            var coroutine = task.AsCoroutine();
            Assert.IsTrue(coroutine.MoveNext());
        }

        bool m_AsCoroutineTestFinished;

        async Task AsCoroutineTask()
        {
            await Task.Delay(10);
            m_AsCoroutineTestFinished = true;
        }

        [UnityTest]
        public IEnumerator AsCoroutine_CanBeUsedInUnityTest()
        {
            m_AsCoroutineTestFinished = false;
            Assert.IsFalse(m_AsCoroutineTestFinished);

            var task = AsCoroutineTask();
            yield return task.AsCoroutine();

            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(m_AsCoroutineTestFinished);
        }
    }
}
