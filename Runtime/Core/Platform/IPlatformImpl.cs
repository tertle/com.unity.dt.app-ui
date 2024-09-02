using System;
using UnityEngine;

namespace Unity.AppUI.Core
{
    /// <summary>
    /// Interface for platform specific implementations.
    /// </summary>
    interface IPlatformImpl
    {
        /// <summary>
        /// Event that is triggered when the system dark mode changes.
        /// </summary>
        event Action<bool> darkModeChanged;

        /// <summary>
        /// Event that is triggered when the high contrast mode changes.
        /// </summary>
        event Action<bool> highContrastChanged;

        /// <summary>
        /// Event that is triggered when the reduce motion accessibility setting changes.
        /// </summary>
        event Action<bool> reduceMotionChanged;

        /// <summary>
        /// Event that is triggered when the scale factor of the Game view's window changes.
        /// </summary>
        /// <remarks>
        /// The Game View's window refers to the window that the game is running in (either standalone or in the editor).
        /// Multiple game views are not supported.
        /// </remarks>
        event Action<float> scaleFactorChanged;

        /// <summary>
        /// Event that is triggered when the text scale factor of system changes.
        /// </summary>
        /// <remarks>
        /// This is not window specific. The system text scale factor is the scale factor of the text globally applied by the system.
        /// For the window specific scale factor, use <see cref="scaleFactorChanged"/>.
        /// </remarks>
        event Action<float> textScaleFactorChanged;

        /// <summary>
        /// Event that is triggered when the layout direction of the platform changes.
        /// </summary>
        event Action<Dir> layoutDirectionChanged;

        /// <summary>
        /// The reference DPI of the system.
        /// </summary>
        float referenceDpi { get; }

        /// <summary>
        /// The current scale factor applied to the Game view's window.
        /// </summary>
        /// <remarks>
        /// The Game View's window refers to the window that the game is running in (either standalone or in the editor).
        /// Multiple game views are not supported.
        /// </remarks>
        float scaleFactor { get; }

        /// <summary>
        /// The current system-wide text scale factor.
        /// </summary>
        /// <remarks>
        /// This is not window specific. The system text scale factor is the scale factor of the text globally applied by the system.
        /// For the window specific display scale factor (not text-only), use <see cref="scaleFactor"/>.
        /// </remarks>
        float textScaleFactor { get; }

        /// <summary>
        /// Whether touch gestures are supported on the current platform.
        /// </summary>
        bool isTouchGestureSupported { get; }

        /// <summary>
        /// Whether the system is in dark mode.
        /// </summary>
        bool darkMode { get; }

        /// <summary>
        /// Whether the system is in high contrast mode.
        /// </summary>
        bool highContrast { get; }

        /// <summary>
        /// Whether the system uses the "Reduce Motion" accessibility setting.
        /// </summary>
        bool reduceMotion { get; }

        /// <summary>
        /// The current system layout direction.
        /// </summary>
        int layoutDirection { get; }

        /// <summary>
        /// Whether the current platform supports haptic feedback.
        /// </summary>
        bool isHapticFeedbackSupported { get; }

        /// <summary>
        /// The touches on the trackpad.
        /// </summary>
        AppUITouch[] touches { get; }

        /// <summary>
        /// Get the system color for the given color type.
        /// </summary>
        /// <param name="colorType"> The type of system color to get. </param>
        /// <returns> The system color for the given color type if any, otherwise Color.clear. </returns>
        Color GetSystemColor(SystemColorType colorType);

        /// <summary>
        /// Event to update the native integration.
        /// </summary>
        /// <remarks>
        /// This will be called once per frame. It is mainly used to do long polling for native events.
        /// </remarks>
        void UpdateLoop();

        /// <summary>
        /// Run a haptic feedback on the current platform.
        /// </summary>
        /// <param name="feedbackType"> The type of haptic feedback to trigger. </param>
        /// <remarks>
        /// If the platform does not support haptic feedback, this will do nothing.
        /// To check if haptic feedback is supported, use <see cref="isHapticFeedbackSupported"/>.
        /// </remarks>
        void RunNativeHapticFeedback(HapticFeedbackType feedbackType);

        /// <summary>
        /// Handle a native message. This is mainly used by Android JNI.
        /// </summary>
        /// <param name="message"> The message to handle. </param>
        void HandleNativeMessage(string message);
    }
}
