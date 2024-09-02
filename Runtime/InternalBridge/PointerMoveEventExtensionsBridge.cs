using UnityEngine.UIElements;

namespace Unity.AppUI.Bridge
{
    static class PointerMoveEventExtensionsBridge
    {
#if APPUI_USE_INTERNAL_API_BRIDGE

        internal static void SetIsHandledByDraggable(this PointerMoveEvent evt, bool val)
        {
            evt.isHandledByDraggable = val;
        }

        internal static bool GetIsHandledByDraggable(this PointerMoveEvent evt)
        {
            return evt.isHandledByDraggable;
        }

#else // REFLECTION

        static readonly System.Reflection.PropertyInfo k_IsHandledByDraggable =
            typeof(PointerMoveEvent).GetProperty("isHandledByDraggable",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        internal static void SetIsHandledByDraggable(this PointerMoveEvent evt, bool val)
        {
            k_IsHandledByDraggable.SetValue(evt, val);
        }

        internal static bool GetIsHandledByDraggable(this PointerMoveEvent evt)
        {
            return (bool)k_IsHandledByDraggable.GetValue(evt);
        }

#endif
    }
}
