using System;
using System.Runtime.CompilerServices;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Utility class for memory operations.
    /// </summary>
    public static class MemoryUtils
    {
        internal const int bufferSize = 8 * 1024;

        static readonly char[] k_Buffer = new char[bufferSize];

        static int s_BufferOffset = 0;

        /// <summary>
        /// Concatenates the strings into a single string.
        /// </summary>
        /// <remarks>
        /// This method is optimized for performance by using a shared buffer.
        /// The buffer size is 8KB, and if the total length of the strings exceeds the buffer size, an exception is thrown.
        /// </remarks>
        /// <param name="str1"> The first string to concatenate. </param>
        /// <param name="str2"> The second string to concatenate. </param>
        /// <returns> The concatenated string. </returns>
        /// <exception cref="ArgumentException"> Thrown when the total length of the strings exceeds the buffer size. </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string Concatenate(string str1, string str2)
        {
            return Concatenate(str1, str2, null, null, null);
        }

        /// <summary>
        /// Concatenates the strings into a single string.
        /// </summary>
        /// <remarks>
        /// This method is optimized for performance by using a shared buffer.
        /// The buffer size is 8KB, and if the total length of the strings exceeds the buffer size, an exception is thrown.
        /// </remarks>
        /// <param name="str1"> The first string to concatenate. </param>
        /// <param name="str2"> The second string to concatenate. </param>
        /// <param name="str3"> The third string to concatenate. </param>
        /// <returns> The concatenated string. </returns>
        /// <exception cref="ArgumentException"> Thrown when the total length of the strings exceeds the buffer size. </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string Concatenate(string str1, string str2, string str3)
        {
            return Concatenate(str1, str2, str3, null, null);
        }

        /// <summary>
        /// Concatenates the strings into a single string.
        /// </summary>
        /// <remarks>
        /// This method is optimized for performance by using a shared buffer.
        /// The buffer size is 8KB, and if the total length of the strings exceeds the buffer size, an exception is thrown.
        /// </remarks>
        /// <param name="str1"> The first string to concatenate. </param>
        /// <param name="str2"> The second string to concatenate. </param>
        /// <param name="str3"> The third string to concatenate. </param>
        /// <param name="str4"> The fourth string to concatenate. </param>
        /// <returns> The concatenated string. </returns>
        /// <exception cref="ArgumentException"> Thrown when the total length of the strings exceeds the buffer size. </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string Concatenate(string str1, string str2, string str3, string str4)
        {
            return Concatenate(str1, str2, str3, str4, null);
        }

        /// <summary>
        /// Concatenates the strings into a single string.
        /// </summary>
        /// <remarks>
        /// This method is optimized for performance by using a shared buffer.
        /// The buffer size is 8KB, and if the total length of the strings exceeds the buffer size, an exception is thrown.
        /// </remarks>
        /// <param name="str1"> The first string to concatenate. </param>
        /// <param name="str2"> The second string to concatenate. </param>
        /// <param name="str3"> The third string to concatenate. </param>
        /// <param name="str4"> The fourth string to concatenate. </param>
        /// <param name="str5"> The fifth string to concatenate. </param>
        /// <returns> The concatenated string. </returns>
        /// <exception cref="ArgumentException"> Thrown when the total length of the strings exceeds the buffer size. </exception>
        internal static string Concatenate(string str1, string str2, string str3, string str4, string str5)
        {
            s_BufferOffset = 0;

            AppendStringToBuffer(str1);
            AppendStringToBuffer(str2);
            AppendStringToBuffer(str3);
            AppendStringToBuffer(str4);
            AppendStringToBuffer(str5);

            return new string(k_Buffer, 0, s_BufferOffset);
        }

        static void AppendStringToBuffer(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (s_BufferOffset + str.Length > bufferSize)
                    throw new ArgumentException("The total length of the strings exceeds the buffer size.");

                str.CopyTo(0, k_Buffer, s_BufferOffset, str.Length);
                s_BufferOffset += str.Length;
            }
        }
    }
}
