using Unity.AppUI.Bridge;
using UnityEngine.UIElements;
#if FOCUSABLE_AS_VISUALELEMENT
using Focusable = UnityEngine.UIElements.VisualElement;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Extension methods for FocusController.
    /// </summary>
    public static class FocusControllerExtensions
    {
        /// <summary>
        /// Focus the next element in the given direction.
        /// </summary>
        /// <param name="controller">The focus controller.</param>
        /// <param name="currentlyFocusedElement">The currently focused element.</param>
        /// <param name="direction">The direction to focus.</param>
        /// <returns>The focused element.</returns>
        public static Focusable FocusNextInDirectionEx(this FocusController controller,
            Focusable currentlyFocusedElement, FocusChangeDirection direction)
        {
            var r = controller?.FocusNextInDirection(currentlyFocusedElement, direction);
            return r;
        }
    }
}
