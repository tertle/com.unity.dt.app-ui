using System;
using UnityEngine.UIElements;

namespace Unity.AppUI.Bridge
{
    static class EventBaseExtensionsBridge
    {
#if APPUI_USE_INTERNAL_API_BRIDGE

#if UNITY_2023_2_OR_NEWER
        [Flags]
        internal enum EventPropagation
        {
            None = EventBase.EventPropagation.None,
            Bubbles = EventBase.EventPropagation.Bubbles,
            TricklesDown = EventBase.EventPropagation.TricklesDown,
            SkipDisabledElements = EventBase.EventPropagation.SkipDisabledElements,
            BubblesOrTricklesDown = Bubbles | TricklesDown,
        }
#else
        [Flags]
        internal enum EventPropagation
        {
            None = EventBase.EventPropagation.None,
            Bubbles = EventBase.EventPropagation.Bubbles,
            TricklesDown = EventBase.EventPropagation.TricklesDown,
            Cancellable = EventBase.EventPropagation.Cancellable,
            SkipDisabledElements = EventBase.EventPropagation.SkipDisabledElements,
            IgnoreCompositeRoots = EventBase.EventPropagation.IgnoreCompositeRoots,
        }
#endif

        internal static void SetPropagation(this EventBase evt, EventPropagation propagation)
        {
            evt.propagation = (EventBase.EventPropagation)propagation;
        }

        internal static EventPropagation GetPropagation(this EventBase evt)
        {
            return (EventPropagation)evt.propagation;
        }

#else // REFLECTION

#if UNITY_2023_2_OR_NEWER
        [Flags]
        internal enum EventPropagation
        {
            None = 0,
            Bubbles = 1,
            TricklesDown = 2,
            SkipDisabledElements = 4,
            BubblesOrTricklesDown = Bubbles | TricklesDown,
        }
#else
        [Flags]
        internal enum EventPropagation
        {
            None = 0,
            Bubbles = 1,
            TricklesDown = 2,
            Cancellable = 4,
            SkipDisabledElements = 8,
            IgnoreCompositeRoots = 16,
        }
#endif

        static readonly System.Reflection.PropertyInfo k_Propagation =
            typeof(EventBase).GetProperty("propagation",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        internal static void SetPropagation(this EventBase evt, EventPropagation propagation)
        {
            k_Propagation.SetValue(evt, (int)propagation);
        }

        internal static EventPropagation GetPropagation(this EventBase evt)
        {
            return (EventPropagation)((int)k_Propagation.GetValue(evt));
        }

#endif

    }
}
