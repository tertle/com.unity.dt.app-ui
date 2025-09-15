using System;
using UnityEngine.Scripting;
using UnityEngine.UIElements;
#if FOCUSABLE_AS_VISUALELEMENT
using CallbackEventHandler = UnityEngine.UIElements.VisualElement;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Extensions to register a <see cref="VisualElement"/> update callback.
    /// This callback will be invoked when App UI main loop is updating.
    /// At runtime, the callback will be invoked once per frame.
    /// In the Editor, the callback will be invoked once per Editor update.
    /// </summary>
    public static class RegisterUpdateCallbackExtensions
    {
        /// <summary>
        /// Register a callback which will be invoked when App UI main loop is updating.
        /// At runtime, the callback will be invoked once per frame.
        /// In the Editor, the callback will be invoked once per Editor update.
        /// </summary>
        /// <param name="control">The UI component.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>True if the UI component can handle callbacks, False otherwise.</returns>
        [Preserve]
        public static bool RegisterUpdateCallback(this VisualElement control, EventCallback<UpdateEvent> callback)
        {
            if (control is not CallbackEventHandler callbackEventHandler)
                return false;
            callbackEventHandler.RegisterCallback(callback);
            Unity.AppUI.Core.AppUI.RegisterUpdateCallback(control);
            return true;
        }

        /// <summary>
        /// Unregister a callback which has been invoked when App UI main loop is updating.
        /// At runtime, the callback will be invoked once per frame.
        /// In the Editor, the callback will be invoked once per Editor update.
        /// </summary>
        /// <param name="control">The UI component.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>True if the UI component can handle callbacks, False otherwise.</returns>
        [Preserve]
        public static bool UnregisterUpdateCallback(this VisualElement control, EventCallback<UpdateEvent> callback)
        {
            if (control is not CallbackEventHandler callbackEventHandler)
                return false;
            Unity.AppUI.Core.AppUI.UnregisterUpdateCallback(control);
            callbackEventHandler.UnregisterCallback(callback);
            return true;
        }
    }

    /// <summary>
    /// THe AppUI Update event.
    /// At runtime, the callback will be invoked once per frame.
    /// In the Editor, the callback will be invoked once per Editor update.
    /// </summary>
    public class UpdateEvent : EventBase<UpdateEvent>
    {
        /// <summary>
        /// Resets all event members to their initial values.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            tricklesDown = false;
            bubbles = false;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UpdateEvent() => LocalInit();
    }
}
