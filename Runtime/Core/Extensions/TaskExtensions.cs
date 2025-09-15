using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Extensions for the <see cref="Task"/>.
    /// </summary>
    public static class TaskExtensions
    {
        const int CONTINUED_TASK = 1;

        internal static readonly Handler taskHandler = new (AppUI.mainLooper, msg =>
        {
            if (msg.what == CONTINUED_TASK)
            {
                ((Action)msg.obj).Invoke();
                return true;
            }

            return false;
        });

        /// <summary>
        /// Converts a <see cref="Task"/> to a coroutine.
        /// </summary>
        /// <param name="task"> The task to convert. </param>
        /// <returns> The coroutine. </returns>
        /// <remarks>
        /// If the task is null, the coroutine will be empty and break immediately.
        /// </remarks>
        internal static IEnumerator AsCoroutine(this Task task)
        {
            if (task != null)
            {
                while (!task.IsCompleted)
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Continues a task on the main thread.
        /// </summary>
        /// <param name="task"> The task to continue. </param>
        /// <param name="continuation"> The continuation action. </param>
        /// <param name="cancellationToken"> The cancellation token. Please provide a cancellation token if you want to cancel the continuation. </param>
        /// <typeparam name="T"> The type of the task. </typeparam>
        /// <returns> The continued task. </returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="task"/> or <paramref name="continuation"/> is null. </exception>
        internal static Task<T> ContinueOnMainThread<T>(
            this Task<T> task,
            Action<Task<T>> continuation,
            CancellationToken cancellationToken = default)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            if (continuation == null)
                throw new ArgumentNullException(nameof(continuation));

            var completionSource = new TaskCompletionSource<T>();
            task.ContinueWith(t =>
            {
                var action = new Action(() =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        completionSource.TrySetCanceled();
                        return;
                    }

                    try
                    {
                        continuation(t);
                        completionSource.SetResult(t.Result);
                    }
                    catch (Exception e)
                    {
                        completionSource.SetException(e);
                    }
                });
                var msg = taskHandler.ObtainMessage(CONTINUED_TASK, action);
                taskHandler.SendMessage(msg);
            }, TaskContinuationOptions.ExecuteSynchronously);

            return completionSource.Task;
        }

        /// <summary>
        /// Continues a task on the main thread and returns a result.
        /// </summary>
        /// <param name="task"> The task to continue. </param>
        /// <param name="continuation"> The continuation function that returns a result. </param>
        /// <param name="cancellationToken"> The cancellation token. Please provide a cancellation token if you want to cancel the continuation. </param>
        /// <typeparam name="TResult"> The type of the result. </typeparam>
        /// <typeparam name="T"> The type of the task. </typeparam>
        /// <returns> The continued task. </returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="task"/> or <paramref name="continuation"/> is null. </exception>
        internal static Task<TResult> ContinueOnMainThread<TResult,T>(
            this Task<T> task,
            Func<Task<T>,TResult> continuation,
            CancellationToken cancellationToken = default)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            if (continuation == null)
                throw new ArgumentNullException(nameof(continuation));

            var completionSource = new TaskCompletionSource<TResult>();
            task.ContinueWith(t =>
            {
                var action = new Action(() =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        completionSource.TrySetCanceled();
                        return;
                    }

                    try
                    {
                        var result = continuation(t);
                        completionSource.SetResult(result);
                    }
                    catch (Exception e)
                    {
                        completionSource.SetException(e);
                    }
                });
                var msg = taskHandler.ObtainMessage(CONTINUED_TASK, action);
                taskHandler.SendMessage(msg);
            }, TaskContinuationOptions.ExecuteSynchronously);

            return completionSource.Task;
        }

        /// <summary>
        /// Waits for any of the provided tasks to complete.
        /// </summary>
        /// <param name="tasks"> The tasks to wait for. </param>
        /// <returns> The task that completed. </returns>
        internal static async Task<Task> WhenAny(params Task[] tasks)
        {
#if !UNITY_WEBGL
            return await Task.WhenAny(tasks);
#else
            // implementation for single threaded environments
            while (true)
            {
                foreach (var task in tasks)
                {
                    if (task.IsCompleted)
                        return task;
                }
                await Task.Yield();
            }
#endif
        }
    }
}
