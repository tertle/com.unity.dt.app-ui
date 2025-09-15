using System;
using UnityEngine.Scripting;
using UnityEngine.UIElements;
using UnityEngine;
#if FOCUSABLE_AS_VISUALELEMENT
using CallbackEventHandler = UnityEngine.UIElements.VisualElement;
#endif

namespace Unity.AppUI.UI
{
    /// <summary>
    /// Interface that must be implemented to UI components which can change their value progressively, like a <see cref="Slider"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of value handled by the UI component.</typeparam>
    public interface INotifyValueChanging<TValue> : INotifyValueChanged<TValue> { }

    /// <summary>
    /// Extensions for <see cref="INotifyValueChanging{TValue}"/>.
    /// </summary>
    public static class NotifyValueChangingExtensions
    {
        /// <summary>
        /// Register a callback which will be invoked when the UI component's value is changing.
        /// </summary>
        /// <param name="control">The UI component.</param>
        /// <param name="callback">The callback.</param>
        /// <typeparam name="TValue">The type of value handled by the UI component.</typeparam>
        /// <returns>True if the UI component can handle callbacks, False otherwise.</returns>
        [Preserve]
        public static bool RegisterValueChangingCallback<TValue>(this INotifyValueChanging<TValue> control, EventCallback<ChangingEvent<TValue>> callback)
        {
            if (control is not CallbackEventHandler callbackEventHandler)
                return false;
            callbackEventHandler.RegisterCallback(callback);
            return true;
        }

        /// <summary>
        /// Unregister a callback which has been invoked when the UI component's value was changing.
        /// </summary>
        /// <param name="control">The UI component.</param>
        /// <param name="callback">The callback.</param>
        /// <typeparam name="TValue">The type of value handled by the UI component.</typeparam>
        /// <returns>True if the UI component can handle callbacks, False otherwise.</returns>
        [Preserve]
        public static bool UnregisterValueChangingCallback<TValue>(this INotifyValueChanging<TValue> control, EventCallback<ChangingEvent<TValue>> callback)
        {
            if (control is not CallbackEventHandler callbackEventHandler)
                return false;
            callbackEventHandler.UnregisterCallback(callback);
            return true;
        }
    }

    /// <summary>
    /// THe value changing event.
    /// </summary>
    /// <typeparam name="TValue">The type of value handled by the UI component.</typeparam>
    public class ChangingEvent<TValue> : EventBase<ChangingEvent<TValue>>
    {
        /// <summary>
        /// The previous value.
        /// </summary>
        public TValue previousValue { get; set; }

        /// <summary>
        /// The new value.
        /// </summary>
        public TValue newValue { get; set; }
    }
}
