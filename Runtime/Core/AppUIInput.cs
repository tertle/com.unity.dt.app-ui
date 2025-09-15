using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Additional input management for AppUI.
    /// </summary>
    public static class AppUIInput
    {
        static readonly Dictionary<Type,IGestureRecognizer> k_Recognizers = new Dictionary<Type, IGestureRecognizer>();

        static PinchGestureRecognizer s_PinchGestureRecognizer;

        static readonly AppUITouch[] k_TouchPool = new AppUITouch[512];

        /// <summary>
        /// Initializes AppUI input management.
        /// </summary>
        internal static void Initialize()
        {
            if (k_Recognizers.Count == 0)
            {
                s_PinchGestureRecognizer = new PinchGestureRecognizer();
                k_Recognizers.Add(typeof(PinchGestureRecognizer), s_PinchGestureRecognizer);
            }

#if ENABLE_INPUT_SYSTEM && UNITY_INPUTSYSTEM_PRESENT
            if (Application.isMobilePlatform)
                UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
#endif
        }

        /// <summary>
        /// This method is called internally every frame to poll for touch events and recognize gestures.
        /// </summary>
        internal static void PollEvents()
        {
            pinchGestureChangedThisFrame = false;
            pinchGesture = default;
            Recognize(Platform.touches);
        }

        /// <summary>
        /// Internal method to fetch touches from either the old or new input system and convert them to AppUITouch objects.
        /// <para />
        /// If the New Input System is enabled, it will be used to fetch the touches.
        /// If the Enhanced Touch Support is enabled, it will be used in priority.
        /// Otherwise, the current <see cref="UnityEngine.InputSystem.Touchscreen"/> controls will be returned.
        /// Finally, if the legacy Input Manager is enabled, the <see cref="UnityEngine.Input.touches"/> will be returned.
        /// <remarks>
        /// If no input system is enabled, this method will return null.
        /// </remarks>
        /// </summary>
        /// <returns> An array of AppUITouch objects representing the current touches. </returns>
        internal static ReadOnlySpan<AppUITouch> GetCurrentInputSystemTouches()
        {
            var count = 0;
#if ENABLE_INPUT_SYSTEM
#if UNITY_INPUTSYSTEM_PRESENT
            if (UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.enabled)
            {
                foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
                {
                    k_TouchPool[count++] = touch.ToAppUITouch();
                }
                return k_TouchPool.AsSpan(0, count);
            }

            if (UnityEngine.InputSystem.Touchscreen.current != null)
            {
                foreach (var touch in UnityEngine.InputSystem.Touchscreen.current.touches)
                {
                    k_TouchPool[count++] = touch.ToAppUITouch();
                }
                return k_TouchPool.AsSpan(0, count);
            }
#endif
#elif ENABLE_LEGACY_INPUT_MANAGER

            if (Input.touchSupported)
            {
                foreach (var touch in Input.touches)
                {
                    k_TouchPool[count++] = touch.ToAppUITouch();
                }
                return k_TouchPool.AsSpan(0, count);
            }

#endif
            // no touch support
            return null;
        }

        static void Recognize(ReadOnlySpan<AppUITouch> appuiTouches)
        {
            if (appuiTouches == null)
                return;

            foreach (var recognizer in k_Recognizers.Values)
            {
                recognizer.Recognize(appuiTouches);
                if (recognizer == s_PinchGestureRecognizer)
                {
                    pinchGestureChangedThisFrame = s_PinchGestureRecognizer.hasChangedThisFrame;
                    pinchGesture = new PinchGesture(s_PinchGestureRecognizer.value, s_PinchGestureRecognizer.state);
                }
            }
        }

        /// <summary>
        /// Resets all gesture recognizers to their initial state.
        /// </summary>
        internal static void Reset()
        {
            foreach (var recognizer in k_Recognizers.Values)
            {
                recognizer.Reset();
            }
        }

        /// <summary>
        /// Whether a pinch gesture has been processed this frame.
        /// </summary>
        /// <remarks>
        /// This will be set to true for any phase of the pinch gesture.
        /// To know the current pinch gesture, check <see cref="pinchGesture"/>.
        /// </remarks>
        public static bool pinchGestureChangedThisFrame { get; private set; }

        /// <summary>
        /// The current pinch gesture.
        /// </summary>
        /// <remarks>
        /// To know if a pinch gesture has been processed this frame, check <see cref="pinchGestureChangedThisFrame"/>.
        /// </remarks>
        public static PinchGesture pinchGesture { get; private set; }

        /// <summary>
        /// Registers a gesture recognizer to be used by AppUI.
        /// </summary>
        /// <typeparam name="TRecognizer"> The type of the gesture recognizer to register. </typeparam>
        public static void RegisterGestureRecognizer<TRecognizer>()
            where TRecognizer : IGestureRecognizer, new()
        {
            k_Recognizers.Add(typeof(TRecognizer), Activator.CreateInstance<TRecognizer>());
        }

        /// <summary>
        /// Unregisters a gesture recognizer from AppUI.
        /// </summary>
        /// <typeparam name="TRecognizer"> The type of the gesture recognizer to unregister. </typeparam>
        public static void UnregisterGestureRecognizer<TRecognizer>()
            where TRecognizer : IGestureRecognizer
        {
            k_Recognizers.Remove(typeof(TRecognizer));
        }
    }
}
