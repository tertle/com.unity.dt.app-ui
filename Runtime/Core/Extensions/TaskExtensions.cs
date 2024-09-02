using System;
using System.Collections;
using System.Threading.Tasks;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Extensions for the <see cref="Task"/>.
    /// </summary>
    public static class TaskExtensions
    {
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
    }
}
