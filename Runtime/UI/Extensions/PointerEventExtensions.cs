using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Extensions for UI-Toolkit PointerEvent
    /// </summary>
    public static class PointerEventExtensions
    {
        /// <summary>
        /// Check if the event is a context click
        /// </summary>
        /// <param name="evt"> The pointer event </param>
        /// <typeparam name="T"> The type of the pointer event </typeparam>
        /// <returns> True if the event is a context click </returns>
        public static bool IsContextClick<T>(this PointerEventBase<T> evt) where T : PointerEventBase<T>, new()
        {
            if (evt == null)
                return false;

            return (evt.button == (int) MouseButton.RightMouse)
                || ((Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                    && evt.button == (int) MouseButton.LeftMouse && evt.ctrlKey);
        }
    }
}
