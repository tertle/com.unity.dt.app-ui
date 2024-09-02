using System.Collections.Generic;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Enumerable extension methods.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Check if two enumerable have the same elements in the same order.
        /// </summary>
        /// <param name="first"> The first enumerable.</param>
        /// <param name="second"> The second enumerable.</param>
        /// <typeparam name="T"> The type of the enumerable.</typeparam>
        /// <returns> True if the two enumerable have the same elements in the same order.</returns>
        public static bool SequenceEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null && second == null)
                return true;

            if (first == null || second == null)
                return false;

            using var enumerator = first.GetEnumerator();
            using var otherEnumerator = second.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (!otherEnumerator.MoveNext())
                    return false;

                if ((enumerator.Current == null && otherEnumerator.Current != null) ||
                    (enumerator.Current != null && !enumerator.Current.Equals(otherEnumerator.Current)))
                    return false;
            }

            if (otherEnumerator.MoveNext())
                return false;

            return true;
        }

        /// <summary>
        /// Get the first element in an enumerable.
        /// </summary>
        /// <param name="enumerable"> The enumerable.</param>
        /// <typeparam name="T"> The type of the enumerable.</typeparam>
        /// <returns> The first element in the enumerable, or default if the enumerable is empty.</returns>
        public static T GetFirst<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return default;

            using var enumerator = enumerable.GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current : default;
        }

        /// <summary>
        /// Get the first integer in an enumerable.
        /// </summary>
        /// <param name="enumerable"> The enumerable.</param>
        /// <returns> The first integer in the enumerable, or -1 if the enumerable is empty.</returns>
        public static int GetFirst(IEnumerable<int> enumerable)
        {
            if (enumerable == null)
                return -1;

            using var enumerator = enumerable.GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current : -1;
        }
    }
}
