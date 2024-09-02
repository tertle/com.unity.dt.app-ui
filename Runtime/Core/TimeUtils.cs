using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Utility class for time related operations.
    /// </summary>
    static class TimeUtils
    {
        /// <summary>
        /// A time override value. If set, this value will be used instead of the current time given by the engine.
        /// </summary>
        internal static float? timeOverride;

        /// <summary>
        /// Gets the current time.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float GetCurrentTime()
        {
            return timeOverride ?? (Application.isEditor ?
#if UNITY_EDITOR
                (float)EditorApplication.timeSinceStartup
#else
                Time.time
#endif
                : Time.time);
        }

        /// <summary>
        /// Gets the current time as a vector.
        /// <br/>
        /// The first component is the time divided by 20. This is useful for animations that need to be slower.
        /// <br/>
        /// The second component is the current time.
        /// <br/>
        /// The third component is the current time multiplied by 2.
        /// <br/>
        /// The fourth component is the current time multiplied by 3.
        /// </summary>
        /// <returns> The current time as a vector. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector4 GetCurrentTimeVector()
        {
            var time = GetCurrentTime();
            return new Vector4(time / 20f, time, time * 2f, time * 3f);
        }
    }
}
