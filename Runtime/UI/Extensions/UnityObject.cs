using UnityEngine;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Helper class to destroy objects in the editor and in play mode.
    /// </summary>
    public static class UnityObject
    {
        /// <summary>
        /// Destroys an object in the editor and in play mode.
        /// </summary>
        /// <param name="obj"> The object to destroy. </param>
        public static void Destroy(Object obj)
        {
            if (Application.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);
        }
    }
}
