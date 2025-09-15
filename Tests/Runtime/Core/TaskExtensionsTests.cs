using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.AppUI.Core;
using UnityEngine;
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

        [Test]
        public void ContinueOnMainThread_WhenTaskOrContinuationIsNull_ThrowsArgumentNullException()
        {
            Task<int> task = null;
            Assert.Throws<System.ArgumentNullException>(() => task.ContinueOnMainThread(_ => { }));
            Assert.Throws<System.ArgumentNullException>(() => task.ContinueOnMainThread(t => t.Result));

            task = Task.FromResult(42);
            Assert.Throws<System.ArgumentNullException>(() => task.ContinueOnMainThread(null));
            Assert.Throws<System.ArgumentNullException>(() => task.ContinueOnMainThread<int, int>(null));
        }

        static readonly Thread k_MainThread = Thread.CurrentThread;

        [Test]
        public async Task ContinueOnMainThread_ContinuesTaskOnMainThread()
        {
            var task = Task.FromResult(42);
            var continuedTask = task.ContinueOnMainThread(t =>
            {
                Assert.AreEqual(42, t.Result);
                Assert.IsTrue(Thread.CurrentThread == k_MainThread);
            });
            await continuedTask;
            Assert.IsTrue(continuedTask.IsCompleted);
        }

        [Test]
        public async Task ContinueOnMainThread_ContinuesTaskOnMainThreadWithResult()
        {
            var task = Task.FromResult(42);
            var continuedTask = task.ContinueOnMainThread(t =>
            {
                Assert.IsTrue(t.IsCompleted);
                Assert.AreEqual(42, t.Result);
                Assert.IsTrue(Thread.CurrentThread == k_MainThread);
                return t.Result + 27;
            });
            await continuedTask;
            Assert.IsTrue(continuedTask.IsCompleted);
            Assert.AreEqual(69, continuedTask.Result);
        }

        [UnityTest]
        public IEnumerator ContinueOnMainThread_WhenTaskIsCanceled_ContinuesTaskOnMainThread()
        {
            var continuationExecuted = false;

            var taskCts = new CancellationTokenSource();
            taskCts.Cancel();
            var task = Task.FromCanceled<int>(taskCts.Token);

            var continuedTaskCts = new CancellationTokenSource();
            continuedTaskCts.Cancel();

            var continuedTask = task.ContinueOnMainThread(t =>
            {
                continuationExecuted = true;
            }, continuedTaskCts.Token);

            yield return AwaitTaskComplete(continuedTask);

            Assert.IsFalse(continuationExecuted);
            Assert.IsTrue(continuedTask.IsCanceled);
        }

        [UnityTest]
        public IEnumerator ContinueOnMainThread_WhenTaskIsCanceled_ContinuesTaskOnMainThreadWithResult()
        {
            var continuationExecuted = false;

            var taskCts = new CancellationTokenSource();
            taskCts.Cancel();
            var task = Task.FromCanceled<int>(taskCts.Token);

            var continuedTaskCts = new CancellationTokenSource();
            continuedTaskCts.Cancel();

            var continuedTask = task.ContinueOnMainThread(t =>
            {
                continuationExecuted = true;
                return t.Result + 27;
            }, continuedTaskCts.Token);

            yield return AwaitTaskComplete(continuedTask);

            Assert.IsFalse(continuationExecuted);
            Assert.IsTrue(continuedTask.IsCanceled);
        }

        IEnumerator AwaitTaskComplete(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                throw task.Exception;
            }
        }
    }
}
