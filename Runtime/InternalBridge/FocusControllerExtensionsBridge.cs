using UnityEngine.UIElements;

namespace Unity.AppUI.Bridge
{
    static class FocusControllerExtensionsBridge
    {
#if APPUI_USE_INTERNAL_API_BRIDGE

#if UNITY_2023_2_OR_NEWER
        internal static Focusable FocusNextInDirection(this FocusController controller, Focusable currentFocusable, FocusChangeDirection direction)
        {
            return controller.FocusNextInDirection(currentFocusable, direction);
        }
#else
        internal static Focusable FocusNextInDirection(this FocusController controller, FocusChangeDirection direction)
        {
            return controller.FocusNextInDirection(direction);
        }
#endif

#else // REFLECTION

        static readonly System.Reflection.MethodInfo k_FocusNextInDirection = typeof(FocusController)
            .GetMethod("FocusNextInDirection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

#if UNITY_2023_2_OR_NEWER
        internal static Focusable FocusNextInDirection(this FocusController controller, Focusable currentFocusable, FocusChangeDirection direction)
        {
            return k_FocusNextInDirection.Invoke(controller, new object[] { currentFocusable, direction }) as Focusable;
        }
#else
        internal static Focusable FocusNextInDirection(this FocusController controller, FocusChangeDirection direction)
        {
            return k_FocusNextInDirection.Invoke(controller, new object[] { direction }) as Focusable;
        }
#endif

#endif
    }
}
